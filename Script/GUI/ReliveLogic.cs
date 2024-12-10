/********************************************************************
	日期:	2014/02/20
	文件: 	D:\work\code\mldj\Version\Main\Project\Client\Assets\MLDJ\Script\GUI\ReliveLogic.cs
	作者:	YangXin
	描述:	复活UI
	
	修改:	
*********************************************************************/
using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
using GCGame.Table;
using Games.LogicObj;

public class ReliveLogic : MonoBehaviour
{

    public UILabel m_ReliveEntryTitle;
    public UILabel m_ReliveYuanBao;
    private int m_nTimeData = 0;
    public GameObject m_NewPlayerRelive;
    public GameObject m_Relive;
    // Use this for initialization
    void Start()
    {

		//==========  before Show close BonusItemGetRoot
		UIManager.CloseUI(UIInfo.BonusItemGetRoot);
		if (BackCamerControll.Instance() != null)
		{
			BackCamerControll.Instance().StopSceneEffect(138, true);
		}

		//==========

        m_NewPlayerRelive.SetActive(false);
        m_Relive.SetActive(false);
        if (GameManager.gameManager.PlayerDataPool.MainPlayerBaseAttr.Level < 30 &&
            GameManager.gameManager.RunningScene != (int)GameDefine_Globe.SCENE_DEFINE.SCENE_GUILDWAR) //帮战副本不出现新手复活
        {
            m_NewPlayerRelive.SetActive(true);
            m_ReliveYuanBao.text = "";
        }
        else
        {
            m_Relive.SetActive(true);

            int _OriginalReliveNum = GameManager.gameManager.PlayerDataPool.CommonData.GetCommonData((int)Games.UserCommonData.USER_COMMONDATA.CD_ORIGINAL_NUMBER);
            int _YuanBao = 10 + _OriginalReliveNum * 5;
            if (_OriginalReliveNum > 8)
            {
                _YuanBao = 50;
            }
            int guildReliveNum = GameManager.gameManager.PlayerDataPool.CommonData.GetCommonData((int)Games.UserCommonData.USER_COMMONDATA.CD_GUILDBUSINESS_GOTTEN_NUM);
            if (Singleton<ObjManager>.GetInstance().MainPlayer.IsInGuildBusiness() && (guildReliveNum < 3))
            {
                int leftReliveNum = 3 - guildReliveNum;
                int[] info = new int[2]{ leftReliveNum , 3};
                m_ReliveYuanBao.text = StrDictionary.GetClientDictionaryString("#{3950}", info);
            }
            else
            {
                m_ReliveYuanBao.text = StrDictionary.GetClientDictionaryString("#{1034}", _YuanBao);
            }
        }

        CG_REQ_POWERUP packet = (CG_REQ_POWERUP)PacketDistributed.CreatePacket(MessageID.PACKET_CG_REQ_POWERUP);
        packet.Type = (int)BePowerData.BePowerType.BPTDEFINE_EQUIP;
        packet.Flag = 1;
        packet.SendPacket();

        InvokeRepeating("DoSomeThing", 0, 1);
    }
    private float ReliveSend = 0;
    // Update is called once per frame

    void DoSomeThing()
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        //string str = StrDictionary.GetClientDictionaryString("#{1035}");
        string str = "";
        m_nTimeData = Singleton<ObjManager>.GetInstance().MainPlayer.ReliveEntryTime;
        if (m_nTimeData < 0)
        {
            m_nTimeData = 0;
        }
        if (m_nTimeData / 60 > 0)
        {
            str += (m_nTimeData / 60).ToString() + "分";
            str = StrDictionary.GetClientDictionaryString("#{2873}", (m_nTimeData / 60), (m_nTimeData % 60));
        }
        else
        {
            str = StrDictionary.GetClientDictionaryString("#{2872}", (m_nTimeData % 60));
        }

        m_ReliveEntryTitle.text = str;

        if (Time.realtimeSinceStartup - ReliveSend > 5)
        {
            ReliveSend = Time.realtimeSinceStartup;
            if (Singleton<ObjManager>.Instance.MainPlayer.GetAutoCombatState()) //挂机 中自动复活
            {
                Tab_SceneClass pSceneClass = TableManager.GetSceneClassByID(GameManager.gameManager.RunningScene, 0);
                if (pSceneClass != null)
                {                   
                    if (pSceneClass.GetReliveTypebyIndex(0) == 1 && Singleton<ObjManager>.Instance.MainPlayer.BaseAttr.Level <= 30)  //原地复活
                    {
                        RliveOriginalButton();
                    }
                    else if (pSceneClass.GetReliveTypebyIndex(1) == 1 )    //入口复活
                    {
                        if (m_nTimeData <= 0)
                        {
                             //可以复活
                            CG_ASK_RELIVE packet = (CG_ASK_RELIVE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_RELIVE);
                            packet.SetType((int)GameDefine_Globe.RELIVE_TYPE.RELIVE_ENTRY);
                            packet.SendPacket();
                        }
                    }
                    else if (pSceneClass.GetReliveTypebyIndex(2) == 1)  //回主城
                    {
                        RliveCityButton();
                    }
                }
            }
        }
    }

    public void CloseWindow()
    {
        UIManager.CloseUI(UIInfo.Relive);
		//UIManager.ShowUI (UIInfo.MissionDialogAndLeftTabsRoot);
    }
    public void RliveCityButton()
    {
        MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{3218}"), "", RliveCityOK, RliveCityNO);        
    }
    public void RliveCityOK()
    {
        CG_ASK_RELIVE packet = (CG_ASK_RELIVE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_RELIVE);
        packet.SetType((int)GameDefine_Globe.RELIVE_TYPE.RELIVE_CITY);
        packet.SendPacket();
    }
    public void RliveCityNO()
    {

    }
    public void ReliveEntryButton()
    {
        if (m_nTimeData <= 0)
        {
            //可以复活
            CG_ASK_RELIVE packet = (CG_ASK_RELIVE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_RELIVE);
            packet.SetType((int)GameDefine_Globe.RELIVE_TYPE.RELIVE_ENTRY);
            packet.SendPacket();
        }
        else
        {
            Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
            if (_mainPlayer != null)
            {
                //提示CD未到
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1027}");
            }           
        }
    }
    public void RliveOriginalButton()
    {
        CG_ASK_RELIVE packet = (CG_ASK_RELIVE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_RELIVE);
        packet.SetType((int)GameDefine_Globe.RELIVE_TYPE.RELIVE_ORIGINAL);
        packet.SendPacket();
    }
}
