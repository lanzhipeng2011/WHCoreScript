//********************************************************************
// 文件名: PlayerFrameLogic.cs
// 描述: 玩家头像脚本
// 作者: WangZhe
//********************************************************************

using Games.GlobeDefine;
using Games.ImpactModle;
using Games.LogicObj;
using UnityEngine;
using System.Collections;
using GCGame.Table;
using Games.Item;
using System.Collections.Generic;
using GCGame;
using System;
using Games.UserCommonData;
public class PlayerFrameLogic : MonoBehaviour {

    private static PlayerFrameLogic m_Instance = null;
    public static PlayerFrameLogic Instance()
    {
        return m_Instance;
    }

    private bool m_bFold  = true;           // 菜单折叠状态
    public bool Fold
    {
        get { return m_bFold; }
    }

	public GameObject m_EffectNvqi;

    public GameObject m_FirstChild;
    public UIButtonMessage m_PlayerHeadButton;
    public UILabel m_PlayerLevel;
    public UISprite m_PlayerHPSprite;
    public UISprite m_PlayerHPBakSprite;
    public UISprite m_PlayerMPSprite;
    public UISprite m_PlayerMPBakSprite;
    public UILabel m_PlayerMPText;
    public UILabel m_PlayerHPText;
    public UISprite m_PlayerHeadSprite;
    public UILabel m_PlayerNameLabel;
    public GameObject m_PlayerFrameInfo;
    //public List<TweenAlpha> m_FoldTween;
    public UISprite m_SkillXPEnergy;

    public UILabel m_ItemDesc;
    public UISprite m_TeamCaptain;      //组队队长标志
    private bool m_IsShowItemDesc = false;
   

    public GameObject m_RemindNum;
    public UILabel m_RemindNumLabel;
    private int m_CurRemindNum = 0;

    private int m_nLastHp =0;
    private int m_nLastMp =0;
   

    public GameObject m_PlayerFrameWhole;
    public UISprite[] m_BuffShowIcon =new UISprite[(int)BUFFICON.MAX_BUFFICONUM];

    public UILabel m_VipImage;
    public GameObject m_VipRoot;

    public UILabel CombatValue;
    void Awake()
    {
        m_Instance = this;
		m_EffectNvqi.SetActive (false);
    }

	// Use this for initialization
	void Start () {
	
		InitUI ();
	
    }

	//=========
	public void InitUI()
	{
		m_FirstChild.SetActive(true);
		
		m_PlayerLevel.text = "0";
		m_PlayerHPText.text = "0/0";
		m_PlayerHPSprite.fillAmount = 0;
		
		m_PlayerMPSprite.fillAmount = 0;
		m_TeamCaptain.gameObject.SetActive(false);
		
		InitXPEnergySlot();
		UpdateData();
		InitUITweenerWhenChangeScene();
		
		UpdateBuffIcon();
		CombatValue.text = GameManager.gameManager.PlayerDataPool.PoolCombatValue.ToString(); 
		InvokeRepeating("UpdateFunctionCD", 0, 5);
	}


	void OnEnable()
	{
		CombatValue.text = GameManager.gameManager.PlayerDataPool.PoolCombatValue.ToString(); 
	}

    void OnDestroy()
    {
        m_Instance = null;
    }

    public void OnCombatValueChange(int newValue) 
    {
        CombatValue.text = GameManager.gameManager.PlayerDataPool.PoolCombatValue.ToString(); 
    }
    public void PlayerFrameHeadOnClick()
    {
        if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_YANMENGUANWAI)
        {
            return;
        }

        if (MenuBarLogic.Instance())
        {
            MenuBarLogic.Instance().PlayTween(m_bFold);
        }
        PlayTween();
        SwitchAllWhenPopUIShow(!m_bFold);
        if (!m_bFold)
        {
            if (MenuBarLogic.Instance() != null && MenuBarLogic.Instance().NewButtonEffectFlag == true)
            {
                MenuBarLogic.Instance().StopNewButtonEffect();
            }
        }

        m_bFold = !m_bFold;

        if (NewPlayerGuidLogic.Instance())
        {
            NewPlayerGuidLogic.CloseWindow();
        }

        if (m_RemindNum.activeSelf)
        {
            m_CurRemindNum = 0;
            //m_RemindNum.SetActive(false);
            //m_RemindNumLabel.text = "";
            UpdateRemainNum();

            if (MenuBarLogic.Instance() != null)
            {
                MenuBarLogic.Instance().PlayNewButtonEffect();
            }
        }    
    }

    void PlayTween()
    {
        m_PlayerFrameWhole.SetActive(!m_bFold);
    }

    public void ChangeHP(int nCurHp, int nMaxHp)
    {
        m_PlayerHPText.text = Utils.ConvertLargeNumToString(nCurHp) + "/" + Utils.ConvertLargeNumToString(nMaxHp);
        if (nMaxHp!=0)
        {
           m_PlayerHPSprite.fillAmount = (float)nCurHp / (float)nMaxHp;

           if (SGDyingBlood.Instance != null)
           if (m_PlayerHPSprite.fillAmount < 0.3f && SGDyingBlood.IsShow == false)
           {
              SGDyingBlood.Instance.Show();  
           }
           else if (SGDyingBlood.IsShow == true&&m_PlayerHPSprite.fillAmount > 0.3f)
           {
              SGDyingBlood.Instance.Hide();    
           }
        }
        else
        {
            m_PlayerHPSprite.fillAmount = 0;
        }
        m_nLastHp = nCurHp;
        m_PlayerHPSprite.MakePixelPerfect();
    }

    public void ChangeMP(int nCurMp, int nMaxMp)
    {
        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (_mainPlayer)
        {
            if (nMaxMp != 0)
            {
                m_PlayerMPText.text = Utils.ConvertLargeNumToString(nCurMp) + "/" + Utils.ConvertLargeNumToString(nMaxMp);
                m_PlayerMPSprite.fillAmount = (float) nCurMp/(float) nMaxMp;
            }
            else
            {
                m_PlayerMPText.text = "0/0";
                m_PlayerMPSprite.fillAmount = 0;
            }
        }
        else
        {
            m_PlayerMPText.text = "0/0";
            m_PlayerMPSprite.fillAmount = 0;
        }
        m_nLastMp = nCurMp;
    }

    public void ChangeLev(int nLev)
    {
        m_PlayerLevel.text = nLev.ToString();
    }

    public void ChangeHeadPic(string strPic)
    {
        m_PlayerHeadSprite.spriteName = strPic;
    }

   
    
    public void ChangeName(string strName)
    {  
        
        m_PlayerNameLabel.text = strName;
    }
    public void UpdateData()
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            LoginData.PlayerRoleData curRoleData = LoginData.GetPlayerRoleData(PlayerPreferenceData.LastRoleGUID);
            if (null != curRoleData)
            {
                ChangeName(curRoleData.name);
                ChangeHeadPic(null);
            }
            return;
        }
      
        int nPlayerHP = Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.HP;
        int nPlayerMaxHP = Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.MaxHP;
        int nPlayerMP = Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.MP;
        int nPlayerMaxMP = Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.MaxMP;
        int nPlayerXP = Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.XP;
        int nPlayerMaxXP = Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.MaxXP;
        int nPlayerLevel = Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.Level;
        string strPlayerHeadPic = Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.HeadPic;
        string strPlayerName = Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.RoleName;
        ChangeHP(nPlayerHP, nPlayerMaxHP);
        ChangeMP(nPlayerMP, nPlayerMaxMP);
        ChangeXPEnergy(nPlayerXP, nPlayerMaxXP);
        ChangeLev(nPlayerLevel);
        ChangeHeadPic(strPlayerHeadPic);
        ChangeName(strPlayerName);

    }

    /// <summary>
    /// Tween动画播放完毕后改变头像按钮图片
    /// </summary>
    public void AfterPlayTweenFold()
    {
    }

    /// <summary>
    /// 应对切换场景时UI异常消失 重新初始化Tween动画
    /// </summary>
    void InitUITweenerWhenChangeScene()
    {
        BoxCollider[] box = gameObject.GetComponentsInChildren<BoxCollider>();
        for (int i=0; i<box.Length; ++i)
        {
            if (null != box[i])
                box[i].enabled = true;
        }
    }

    public void NewPlayerGuide(int nIndex)
    {
        // 判断折叠状态
        if (m_bFold)
        {
			NewPlayerGuidLogic.OpenWindow(m_PlayerFrameInfo, 460, 150, "", "right", 2, true, true);

            if (MenuBarLogic.Instance())
            {
                MenuBarLogic.Instance().NewPlayerGuideIndex = nIndex;
            }
        }
    }

    #region autoFight
    /*
    public void InitAutoFight()
    {
    
        m_HpItemInfo.SetActive(true);
        m_MpItemInfo.SetActive(true);
        GameItemContainer BackPack = GameManager.gameManager.PlayerDataPool.BackPack;
        Obj_MainPlayer User = Singleton<ObjManager>.Instance.MainPlayer;
        if (User)  //更新信息
        {
            if (User.AutoHpID != -1)
            {
                Tab_CommonItem curItem = TableManager.GetCommonItemByID(User.AutoHpID, 0);
                if (curItem != null)
                {
                    m_HpItemIconPic.spriteName = "HPcao";
                    if (BackPack.GetItemCountByDataId(User.AutoHpID) <= 0)
                    {
                           //m_HpItemCDPic.fillAmount = 1;
                          // m_HpItemInfo.SetActive(false);
                           m_HpItemIconPic.spriteName = "";
                           m_HpItemCDPic.fillAmount = 0;
                           GameManager.gameManager.PlayerDataPool.HpItemCDTime = 0;
                    }
                }
              
            }
            if (User.AutoMpID != -1)
            {
                Tab_CommonItem curItem = TableManager.GetCommonItemByID(User.AutoMpID, 0);
                if (curItem != null)
                {
                    m_MpItemIconPic.spriteName = "MPcao";
                    if (BackPack.GetItemCountByDataId(User.AutoMpID) <= 0)
                    {
                          // m_MpItemCDPic.fillAmount = 1;
                           //m_MpItemInfo.SetActive(false);
                           m_MpItemIconPic.spriteName = "";
                           m_MpItemCDPic.fillAmount = 0;
                           GameManager.gameManager.PlayerDataPool.MpItemCDTime = 0;
                    }
                }
            }
        }
    }
     */
    #endregion

    private static float m_fCDTimeSecond = Time.realtimeSinceStartup;
    private static float m_fItemDescSecound = 0;

    public void OnItemClick()
    {
        if (m_IsShowItemDesc == false)
        {
            m_ItemDesc.gameObject.SetActive(true);
            m_IsShowItemDesc = true;
        }     
    }
   
    void InitXPEnergySlot()
    {
		m_SkillXPEnergy.fillAmount = 0;

    }

    public void ChangeXPEnergy(int nValue,int maxXP)
    {
       // m_SkillXPEnergy.UpdateEnergy(nValue, maxXP);
		//float nFillAmount = (float)nValue / (float)maxXP * 0.65f + 0.3f;//不清楚系数作用暂时屏蔽掉
		float nFillAmount = (float)nValue / (float)maxXP;
		m_SkillXPEnergy.fillAmount = nFillAmount;
		if (nFillAmount >= 1.0f) 
		{
			m_EffectNvqi.SetActive (true);
		} 
		else 
		{
			m_EffectNvqi.SetActive(false);
		}
	}
	
	static void PlayXPEffect(bool showEffect)
    {
        if (SkillBarLogic.Instance() != null)
        {
            SkillBarLogic.Instance().PlayXPActiveEffect(showEffect);
        }
    }

    public void OnVipCostChange(int nCost)
    {
        if (nCost > 0)
        {
            m_VipRoot.gameObject.SetActive(true);
            int nLevel = 0;
            int nLeft = 0;
            VipData.GetVipLevel(nCost, ref nLevel, ref nLeft);
            m_VipImage.text = nLevel.ToString();
        }
        else
        {
            m_VipRoot.gameObject.SetActive(false);
        }
    }

    public void AddRemindNum()
    {
        m_CurRemindNum += 1;
        UpdateRemainNum();
    }

    int GetPartnerTipCount()
    {
        int remainCount = PartnerFrameLogic.GetPartnerRemainCount();
        bool bRet = GameManager.gameManager.PlayerDataPool.CommonData.GetCommondFlag((int)USER_COMMONFLAG.CF_FELLOWFUNCTION_OPENFLAG);
        if (remainCount > 0 && bRet)
        {
            return 1;
        }
        return 0;
    }

    int GetMasterAndGuildTipCount()
    {
        if (Singleton<ObjManager>.Instance.MainPlayer != null)
        {
            if (Singleton<ObjManager>.GetInstance().MainPlayer.ShowGuildNewReserveFlag)
            {
                UInt64 myGuid = Singleton<ObjManager>.GetInstance().MainPlayer.GUID;
                if (GameManager.gameManager.PlayerDataPool.IsGuildChief() ||
                    GameManager.gameManager.PlayerDataPool.IsGuildViceChief(myGuid))
                {
                    return 1;
                }
            }
            if (MasterWindow.GetMasterRemainNum() > 0)
            {
                return 1;
            }
        }
        return 0;
    }

    public void UpdateRemainNum()
    {
        int nShowNum = m_CurRemindNum + RestaurantData.m_restaurantTipsCount;
        nShowNum += BelleData.GetBelleTipCount();
        nShowNum += GetPartnerTipCount();
        nShowNum += SkillRootLogic.GetCanLevUpSkillCount();
        nShowNum += GetMasterAndGuildTipCount();

        if (nShowNum > 0 )
        {
            m_RemindNum.SetActive(true);
            m_RemindNumLabel.text = nShowNum.ToString();
        }
        else
        {
             m_RemindNum.SetActive(false);
        }
    }

    //更新头像的BUFF位
    public void UpdateBuffIcon()
    {
        for (int i = 0; i < (int) BUFFICON.MAX_BUFFICONUM; i++)
        {
            m_BuffShowIcon[i].gameObject.SetActive(false);
        }

        int nBuffShowIndex = 0;
        for (int i = 0; i < GameManager.gameManager.PlayerDataPool.ClientImpactInfo.Count; ++i)
        {
            if (GameManager.gameManager.PlayerDataPool.ClientImpactInfo[i].IsVaild())
            {
                Tab_Impact _tabImpact = TableManager.GetImpactByID(GameManager.gameManager.PlayerDataPool.ClientImpactInfo[i].ImpactId, 0);
                if (_tabImpact != null)
                {
                    if (_tabImpact.BuffType == (int) BUFFTYPE.BUFF && _tabImpact.BuffIcon != "-1")
                    {
                        if (nBuffShowIndex >= 0 && nBuffShowIndex < (int) BUFFICON.MAX_BUFFICONUM)
                        {
                            m_BuffShowIcon[nBuffShowIndex].gameObject.SetActive(true);
                            m_BuffShowIcon[nBuffShowIndex].spriteName = _tabImpact.BuffIcon;
                            m_BuffShowIcon[nBuffShowIndex].MakePixelPerfect();
                            nBuffShowIndex++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
    }

    public void SwitchAllWhenPopUIShow(bool isShow)
    {
        if (MissionDialogAndLeftTabsLogic.Instance())
        {
            MissionDialogAndLeftTabsLogic.Instance().PlayTween(!isShow);
        }
        if (SGAutoFightBtn.Instance!= null) 
        {
            SGAutoFightBtn.Instance.gameObject.SetActive(isShow);
        }
        if (TargetFrameLogic.Instance())
        {
            TargetFrameLogic.Instance().PlayTween(!isShow);
        }
        if (FunctionButtonLogic.Instance())
        {
            FunctionButtonLogic.Instance().PlayTween(!isShow);
        }

        if (ExpLogic.Instance())
        {
            ExpLogic.Instance().PlayTween(!isShow);
        }
        if (ChatFrameLogic.Instance())
        {
            ChatFrameLogic.Instance().PlayTween(!isShow);
        }
        if (SkillBarLogic.Instance())
        {
            SkillBarLogic.Instance().PlayTween(!isShow);
        }
        if (PlayerHitsLogic.Instance())
        {
            PlayerHitsLogic.Instance().PlayTween(!isShow);
        }

        if (RechargeBarLogic.Instance())
        {
            RechargeBarLogic.Instance().PlayTween(!isShow);
        }
        if (SGAutoMedicine.Instance != null) 
        {
            SGAutoMedicine.Instance.gameObject.SetActive(isShow);
        }
        if (!isShow)
        {
            if (JoyStickLogic.Instance())
            {
                JoyStickLogic.Instance().CloseWindow();
            }
        }
        else
        {
            if (JoyStickLogic.Instance())
            {
                JoyStickLogic.Instance().OpenWindow();
            }                        
        }
    }

    public void SetTeamCaptain(bool bActive)
    {
        if (null != m_TeamCaptain)
        {
            m_TeamCaptain.gameObject.SetActive(bActive);
        }        
    }

    // 更新功能CD提醒图标，5秒一更新
    public void UpdateFunctionCD()
    {
        UpdateRemainNum();
        if (null != MenuBarLogic.Instance())
        {
            MenuBarLogic.Instance().UpdateBelleTip();
            MenuBarLogic.Instance().UpdatePartnerTip();
            MenuBarLogic.Instance().UpdateSkillTip();
        }
        if (RechargeBarLogic.Instance() != null)
        {
            RechargeBarLogic.Instance().UpdateChargeTip();
        }
    }
}