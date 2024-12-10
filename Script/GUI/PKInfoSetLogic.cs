using Games.GlobeDefine;
using Games.LogicObj;
using GCGame.Table;
using UnityEngine;
using System.Collections;

public class PKInfoSetLogic : MonoBehaviour {

	// Use this for initialization
    private static PKInfoSetLogic m_Instance = null;
    public static PKInfoSetLogic Instance()
    {
        return m_Instance;
    }

    private int m_nPKModle = -1;
	public int PKModle
	{
		get { return m_nPKModle; }
		set { m_nPKModle = value; }
	}
    private int m_nPKCDTime = -1;
	public int PKCDTime
	{
		get { return m_nPKCDTime; }
		set { m_nPKCDTime = value; }
	}
    private int m_nPKValue =0;
	public int PKValue
	{
		get { return m_nPKValue; }
		set { m_nPKValue = value; }
	}
    public UILabel m_PkValueLable;
    public UILabel m_PKStateLable;
    public UISprite m_PeaceBtnSprite;
    public UISprite m_KillBtnSprite;

    public GameObject PeaceSpriteState;
    public GameObject KillSpriteState;
	void Start ()
	{
	   
	}

    void OnEnable()
    {
        m_Instance = this;
		updateCount = 0;
        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (_mainPlayer)
        {
            //发包请求信息
            CG_ASK_PKINFO pkinfo = (CG_ASK_PKINFO)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_PKINFO);
            pkinfo.SetObjId(_mainPlayer.ServerID);
            pkinfo.SendPacket();
        }
        SwitchBtnState();
    }

    void OnDisable()
    {
        m_Instance = null;
    }
	
	// Update is called once per frame
    public  void UpdatePKInfo()
    {
        m_PkValueLable.text = m_nPKValue.ToString();
        if (m_nPKModle ==(int)CharacterDefine.PKMODLE.NORMAL)
        {
            string _strNotice = StrDictionary.GetClientDictionaryString("#{1109}");
            m_PKStateLable.text =_strNotice;
        }
        else if (m_nPKModle == (int)CharacterDefine.PKMODLE.KILL && m_nPKCDTime<= 0)
        {
            string _strNotice = StrDictionary.GetClientDictionaryString("#{1121}");
            m_PKStateLable.text = _strNotice;
        }
        else if (m_nPKModle == (int)CharacterDefine.PKMODLE.KILL && m_nPKCDTime > 0)
        {
            int nMin = m_nPKCDTime/60;
            int nSecond = m_nPKCDTime%60;
            string _strNotice = StrDictionary.GetClientDictionaryString("#{1110}",nMin,nSecond);
            m_PKStateLable.text =_strNotice;
        }
    }

	private int updateCount = 0;
	void FixedUpdate () 
    {
        if (m_nPKModle == (int)CharacterDefine.PKMODLE.KILL && m_nPKCDTime > 0)
        {
			updateCount ++;
			if(updateCount == (int)((float)1 / Time.deltaTime))
			{
				m_nPKCDTime -= 1;
				updateCount = 0;
			}
            int nMin = m_nPKCDTime / 60;
            int nSecond = m_nPKCDTime % 60;
            string _strNotice = StrDictionary.GetClientDictionaryString("#{1110}",nMin,nSecond);
            m_PKStateLable.text = _strNotice;
			//m_nPKCDTime = ((m_nPKCDTime * 100 - (int)(Time.deltaTime * 100)) / 100);
        }
	}

    void OnNormalModleClick()
    {
        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (_mainPlayer ==null)
        {
           return;
        }
        if (m_nPKModle == (int) CharacterDefine.PKMODLE.NORMAL)
        {
            _mainPlayer.SendNoticMsg(false, "#{1108}");
            return;
        }
        if (m_nPKCDTime >0)
        {
            int nMin = m_nPKCDTime / 60;
            int nSecond = m_nPKCDTime % 60;
            //字典提示
            _mainPlayer.SendNoticMsg(false, "#{1110}", nMin, nSecond);
            return;
        }
        if (m_nPKModle != (int)CharacterDefine.PKMODLE.NORMAL && m_nPKCDTime <=0)
        {
            //发包请求切换
            CG_CHANGE_PKMODLE pkChange= (CG_CHANGE_PKMODLE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_CHANGE_PKMODLE);
            pkChange.SetObjId(_mainPlayer.ServerID);
            pkChange.SetPKModle((int)CharacterDefine.PKMODLE.NORMAL);
            pkChange.SendPacket();
            OnCloseClick();
        }
    }
    void OnKillModleClick()
    {
        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (_mainPlayer == null)
        {
            return;
        }
        if (_mainPlayer.BaseAttr.Level <30)
        {
            _mainPlayer.SendNoticMsg(false, "#{1111}");
            return;
        }
        if (_mainPlayer.PkModle == (int)CharacterDefine.PKMODLE.KILL && m_nPKCDTime <=0)
        {
            _mainPlayer.SendNoticMsg(false, "#{1107}");
            return;
        }
        //弹出确认框
        string _strNotice = StrDictionary.GetClientDictionaryString("#{1106}");
        MessageBoxLogic.OpenOKCancelBox(_strNotice, "", SwitchKillModeOnOk, SwitchKillModeOnCancle);
    }

    public void SwitchKillModeOnOk()
    {
        if (m_nPKModle != (int)CharacterDefine.PKMODLE.KILL  ||  m_nPKCDTime >0)
        {
            Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
            if (_mainPlayer)
            {
                //发包请求切换
                CG_CHANGE_PKMODLE pkChange = (CG_CHANGE_PKMODLE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_CHANGE_PKMODLE);
                pkChange.SetObjId(_mainPlayer.ServerID);
                pkChange.SetPKModle((int)CharacterDefine.PKMODLE.KILL);
                pkChange.SendPacket();
            }
        }
        OnCloseClick();
    }
    public void SwitchKillModeOnCancle()
    {

    }
    void OnCloseClick()
    {
        UIManager.CloseUI(UIInfo.PKSetInfo);
    }
   
    public void SwitchBtnState()
    {
        if (Singleton<ObjManager>.Instance.MainPlayer == null)
        {
            return;
        }

        int nPKModel = Singleton<ObjManager>.Instance.MainPlayer.PkModle;
        if (nPKModel == (int)CharacterDefine.PKMODLE.NORMAL)
        {
            this.PeaceSpriteState.SetActive(true);
            this.KillSpriteState.SetActive(false);
            m_PeaceBtnSprite.spriteName = "ui_pk_03";
			m_KillBtnSprite.spriteName = "ui_pk_04";
        }
        else if (nPKModel == (int)CharacterDefine.PKMODLE.KILL)
        {
            this.PeaceSpriteState.SetActive(false);
            this.KillSpriteState.SetActive(true);
			m_PeaceBtnSprite.spriteName = "ui_pk_02"; 
			m_KillBtnSprite.spriteName = "ui_pk_05";
        }
    }
}
