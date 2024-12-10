//********************************************************************
// 文件名: MenuBarLogic.cs
// 描述: 菜单按钮脚本
// 作者: WangZhe
//********************************************************************

using UnityEngine;
using System.Collections;
using System;
using Module.Log;
using Games.GlobeDefine;
using Games.UserCommonData;
using Games.LogicObj;
using GCGame.Table;
public class MenuBarLogic : MonoBehaviour {

//     public GameObject m_LeftMenuBarOffset;
//     public GameObject m_TopMenuBarOffset;
    public BoxCollider m_BackgroundBox;

    //private TweenPosition m_LeftMenuBarTween;
    //private Vector3 m_LeftMenuBarPos_From = new Vector3(0, 0, 0);
    //private Vector3 m_LeftMenuBarPos_To = new Vector3(0, 0, 0);

//     private TweenPosition m_TopMenuBarTween; 
//     private Vector3 m_TopMenuBarPos_From = new Vector3(0, 0, 0);
//     private Vector3 m_TopMenuBarPos_To = new Vector3(0, 0, 0);   

    private static MenuBarLogic m_Instance = null;
    public static MenuBarLogic Instance()
    {
        return m_Instance;
    }

    // 新手指引相关
    private int m_NewPlayerGuideIndex = -1;
    public int NewPlayerGuideIndex
    {
        get { return m_NewPlayerGuideIndex; }
        set { m_NewPlayerGuideIndex = value; }
    }
    public GameObject m_BackButton;

    private bool m_bFold = false;
    public bool Fold
    {
        get { return m_bFold; }
        set { m_bFold = value; }
    }

    public GameObject m_Role;
    public GameObject m_EquipEnchance;
    public GameObject m_Belle;
    public GameObject m_Fellow;
    public GameObject m_BackPack;
    public GameObject m_XiaKe;
    public GameObject m_Guild;
    public GameObject m_GuildNewReserveFlag;        //帮会待审批成员标记
	public GameObject m_GuildEffect;
    public GameObject m_Farm;
    public GameObject m_Skill;
    public GameObject m_SysShop;
    public GameObject m_QianKunDai;
    public GameObject m_Make;
    public UILabel m_BelleCountTip;
    public UIGrid m_TopGrid;
    public UILabel m_RestaurantCountTip;
    public Transform m_OffsetTrans;
    public UILabel m_PartnerCountTip;
    public UILabel m_SkillCountTip;
    //public UIGrid m_LeftGrid;

    private GameObject m_NewButton = null;
    private bool m_bShowScaleAni = false;
    private bool m_bScaleAniDir = true;   
    
    void Awake()
    {
        m_Instance = this;

        /*m_LeftMenuBarTween = m_LeftMenuBarOffset.GetComponent<TweenPosition>();
        m_TopMenuBarTween = m_TopMenuBarOffset.GetComponent<TweenPosition>();

        //m_LeftMenuBarPos_From = m_LeftMenuBarTween.from;
        //m_LeftMenuBarPos_To = m_LeftMenuBarTween.to;
        m_TopMenuBarPos_From = m_TopMenuBarTween.from;
        m_TopMenuBarPos_To = m_TopMenuBarTween.to;*/
    }

	// Use this for initialization
	void Start () {
        //InitUITweenerWhenChangeScene();
        m_BackgroundBox.enabled = false;

        //开始的时候隐藏，等待显示的时候才激活
        //gameObject.SetActive(false);

        
        UpdateBelleTip();
        UpdateRestaurantTips();
        UpdatePartnerTip();
        UpdateSkillTip();
        UpdateGuildAndMasterReserveMember();

        gameObject.SetActive(false);

	}

    void Update()
    {
        UpdateScaleAni();
       
    }

    void OnEnable() 
    {
        InitButtonActive();
        UpdateSkillTip();
    }

    void OnDestroy()
    {
        m_Instance = null;
    }

    void UpdateScaleAni()
    {
        if (m_bShowScaleAni)
        {
            if (m_bScaleAniDir)
            {
                m_OffsetTrans.localScale += new Vector3(0.1f, 0.1f, 0.1f);
                if (m_OffsetTrans.localScale.x >= 1 || m_OffsetTrans.localScale.y >= 1)
                {
                    m_bShowScaleAni = false;
                    m_OffsetTrans.localScale = Vector3.one;
                    AfterPlayTweenPosition();
                }
            }
            else
            {
                m_OffsetTrans.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                if (m_OffsetTrans.localScale.x <= 0 || m_OffsetTrans.localScale.y <= 0)
                {
                    m_bShowScaleAni = false;
                    m_OffsetTrans.localScale = Vector3.zero;
                    AfterPlayTweenPosition();
                }
            }
        }
    }

    public void PlayTween(bool nDirection)
    {
//         m_LeftMenuBarTween.Play(nDirection);
        //         m_TopMenuBarTween.Play(nDirection);
        gameObject.SetActive(true);
        PlayScaleAni(nDirection);
        m_BackgroundBox.enabled = nDirection;
        m_bFold = nDirection;

        m_TopGrid.Reposition();

        UpdateGuildAndMasterReserveMember();
    }

    void PlayScaleAni(bool bDirection)
    {
        m_bShowScaleAni = true;
        m_bScaleAniDir = bDirection;
    }

    void ReturnTween()
    {
        if (PlayerFrameLogic.Instance() != null)
        {
            PlayerFrameLogic.Instance().PlayerFrameHeadOnClick();
        }
    }

    /// <summary>
    /// Tween动画播放完毕后
    /// </summary>
    void AfterPlayTweenPosition()
    {
        if (true == m_bFold)
        {
            ShowNewPlayerGuide();
            UpdateGuildAndMasterReserveMember();
        }
        else
        {
            //收起的时候，隐藏
            gameObject.SetActive(false);
        }
    }

    public void OnBackPackClick()
    {
		NewPlayerGuidLogic.CloseWindow();

        UIManager.ShowUI(UIInfo.BackPackRoot, BackPackLogic.SwitchEquipPackView);
    }

    public void OnMountAndFellowClick(GameObject value)
    {
		NewPlayerGuidLogic.CloseWindow();

        if (m_NewButton != null && m_NewButton == value)
        {
            StopNewButtonEffect();
        }
        UIManager.ShowUI(UIInfo.PartnerAndMountRoot);
    }

    void OnSocialClick()
    {
		NewPlayerGuidLogic.CloseWindow();

        UIManager.ShowUI(UIInfo.RelationRoot);
    }
    void OnSkillClick()
    {
		NewPlayerGuidLogic.CloseWindow();

        UIManager.ShowUI(UIInfo.SkillInfo);
    }

    void OnBelleClick(GameObject value)
    {
		NewPlayerGuidLogic.CloseWindow();

        if (m_NewButton != null && m_NewButton == value)
        {
            StopNewButtonEffect();
        }
        UIManager.ShowUI(UIInfo.Belle);
    }

    void OnStrenClick(GameObject value)
    {
		NewPlayerGuidLogic.CloseWindow();

        if (m_NewButton != null && m_NewButton == value)
        {
            StopNewButtonEffect();
        }
        UIManager.ShowUI(UIInfo.EquipStren);
    }

    void OnConsignSaleClick()
    {
		NewPlayerGuidLogic.CloseWindow();

        UIManager.ShowUI(UIInfo.ConsignSaleRoot);
    }

    public void OnMissionLogClick()
    {
		NewPlayerGuidLogic.CloseWindow();

        UIManager.ShowUI(UIInfo.MissionLogRoot);
    }
    public void OnRoleClick()
    {
		NewPlayerGuidLogic.CloseWindow();

        UIManager.ShowUI(UIInfo.RoleView);
    }

    void OnSetupClick()
    {
		NewPlayerGuidLogic.CloseWindow();

        UIManager.ShowUI(UIInfo.SystemAndAutoFight);
    }

    void OnBiographyClick()
    {
		NewPlayerGuidLogic.CloseWindow();

        UIManager.ShowUI(UIInfo.Biography);
    }

    void OnRestaurantClick(GameObject value)
    {
		NewPlayerGuidLogic.CloseWindow();

        if (m_NewButton != null && m_NewButton == value)
        {
            StopNewButtonEffect();
        }
        RestaurantController.OpenWindow(true);
    }
    void OnMasterAndGuildClick()
    {
		NewPlayerGuidLogic.CloseWindow();

		m_GuildEffect.SetActive (false);

        UIManager.ShowUI(UIInfo.MasterAndGuildRoot);
    }

    void OnSysShopClick()
    {
		NewPlayerGuidLogic.CloseWindow();

        UIManager.ShowUI(UIInfo.SysShop);
    }

    public void OpenFunction(int nType)
    {
        switch(nType)
        {
            case (int)USER_COMMONFLAG.CF_FELLOWFUNCTION_OPENFLAG:
                {
                    if (!m_Fellow.activeSelf)
                    {
                        NewItemGetLogic.InitItemInfo(m_Fellow.GetComponent<UIImageButton>().target.spriteName,
                            PlayerFrameLogic.Instance().m_PlayerHeadButton.gameObject,
                            NewItemGetLogic.NEWITEMTYPE.TYPE_MENUBAR, nType);
                        m_NewButton = m_Fellow;
                    }  
                }
                break;
            case (int)USER_COMMONFLAG.CF_BELLEFUNCTION_OPENFLAG:
                {
                    if (!m_Belle.activeSelf)
                    {
                        NewItemGetLogic.InitItemInfo(m_Belle.GetComponent<UIImageButton>().target.spriteName,
                            PlayerFrameLogic.Instance().m_PlayerHeadButton.gameObject,
                            NewItemGetLogic.NEWITEMTYPE.TYPE_MENUBAR, nType);
                        m_NewButton = m_Belle;
                    }  
                }
                break;
            case (int)USER_COMMONFLAG.CF_STRENGTHENFUNCTION_OPENFLAG:
                {
                    if (!m_EquipEnchance.activeSelf)
                    {
                        NewItemGetLogic.InitItemInfo(m_EquipEnchance.GetComponent<UIImageButton>().target.spriteName,
                            PlayerFrameLogic.Instance().m_PlayerHeadButton.gameObject,
                            NewItemGetLogic.NEWITEMTYPE.TYPE_MENUBAR, nType);
                        m_NewButton = m_EquipEnchance;
                    }     
                }
                break;
            case (int)USER_COMMONFLAG.CF_RESTAURANTFUNCTION_OPENFLAG:
                {
                    if (!m_Farm.activeSelf)
                    {
                        NewItemGetLogic.InitItemInfo(m_Farm.GetComponent<UIImageButton>().target.spriteName,
                            PlayerFrameLogic.Instance().m_PlayerHeadButton.gameObject,
                            NewItemGetLogic.NEWITEMTYPE.TYPE_MENUBAR, nType);
                        m_NewButton = m_Farm;
                    }   
                }
                break;
            case (int)USER_COMMONFLAG.CF_GUILDFUNCTION_OPENFLAG:
                {
                    if (!m_Guild.activeSelf)
                    {
                        NewItemGetLogic.InitItemInfo(m_Guild.GetComponent<UIImageButton>().target.spriteName,
                            PlayerFrameLogic.Instance().m_PlayerHeadButton.gameObject,
                            NewItemGetLogic.NEWITEMTYPE.TYPE_MENUBAR, nType);
                        m_NewButton = m_Guild;
                    }
                }
                break;
            case (int)USER_COMMONFLAG.CF_XIAKEFUNCTION_OPENFLAG:
                {
                    if (!m_XiaKe.activeSelf)
                    {
                        NewItemGetLogic.InitItemInfo(m_XiaKe.GetComponent<UIImageButton>().target.spriteName,
                            PlayerFrameLogic.Instance().m_PlayerHeadButton.gameObject,
                            NewItemGetLogic.NEWITEMTYPE.TYPE_MENUBAR, nType);
                        m_NewButton = m_XiaKe;
                    }
                }
                break;
        }
        InitButtonActive();
    }

    public void LevelUpButtonActive()
    {
        return;
        int nPlayerLevel = Singleton<ObjManager>.Instance.MainPlayer.BaseAttr.Level;
        if (nPlayerLevel >= (int)GameDefine_Globe.NEWBUTTON_LEVEL.XIAKE)
        {
            if (!m_XiaKe.activeSelf && null != m_NewButton)
            {
                NewItemGetLogic.InitItemInfo(m_XiaKe.GetComponent<UIImageButton>().target.spriteName,
                    PlayerFrameLogic.Instance().m_PlayerHeadButton.gameObject,
                    NewItemGetLogic.NEWITEMTYPE.TYPE_MENUBAR);
                m_NewButton = m_XiaKe;
            }
        }
        else if (nPlayerLevel >= (int)GameDefine_Globe.NEWBUTTON_LEVEL.GUILD)
        {
            if (!m_Guild.activeSelf && null != m_NewButton)
            {
                NewItemGetLogic.InitItemInfo(m_Guild.GetComponent<UIImageButton>().target.spriteName,
                    PlayerFrameLogic.Instance().m_PlayerHeadButton.gameObject,
                    NewItemGetLogic.NEWITEMTYPE.TYPE_MENUBAR);
                m_NewButton = m_Guild;
            }            
        }
        else if (nPlayerLevel >= (int)GameDefine_Globe.NEWBUTTON_LEVEL.FARM)
        {
            if (!m_Farm.activeSelf && null != m_NewButton)
            {
                NewItemGetLogic.InitItemInfo(m_Farm.GetComponent<UIImageButton>().target.spriteName,
                    PlayerFrameLogic.Instance().m_PlayerHeadButton.gameObject,
                    NewItemGetLogic.NEWITEMTYPE.TYPE_MENUBAR);
                m_NewButton = m_Farm;
            }           
        }
        else if (nPlayerLevel >= (int)GameDefine_Globe.NEWBUTTON_LEVEL.EQUIPSTREN)
        {
            if (!m_EquipEnchance.activeSelf && null != m_NewButton)
            {
                NewItemGetLogic.InitItemInfo(m_EquipEnchance.GetComponent<UIImageButton>().target.spriteName,
                    PlayerFrameLogic.Instance().m_PlayerHeadButton.gameObject,
                    NewItemGetLogic.NEWITEMTYPE.TYPE_MENUBAR);
                m_NewButton = m_EquipEnchance;
            }            
        }
        else if (nPlayerLevel >= (int)GameDefine_Globe.NEWBUTTON_LEVEL.BELLE)
        {
            if (!m_Belle.activeSelf && null != m_NewButton)
            {
                NewItemGetLogic.InitItemInfo(m_Belle.GetComponent<UIImageButton>().target.spriteName,
                    PlayerFrameLogic.Instance().m_PlayerHeadButton.gameObject,
                    NewItemGetLogic.NEWITEMTYPE.TYPE_MENUBAR);
                m_NewButton = m_Belle;
            }            
        }
        else if (nPlayerLevel >= (int)GameDefine_Globe.NEWBUTTON_LEVEL.PARTNER)
        {
            if (!m_Fellow.activeSelf && null != m_NewButton)
            {
                NewItemGetLogic.InitItemInfo(m_Fellow.GetComponent<UIImageButton>().target.spriteName,
                    PlayerFrameLogic.Instance().m_PlayerHeadButton.gameObject,
                    NewItemGetLogic.NEWITEMTYPE.TYPE_MENUBAR);
                m_NewButton = m_Fellow;
            }           
        }
    }

    public void InitButtonActive()
    {
        if (m_Fellow == null || m_Belle == null || m_EquipEnchance == null
            || m_Farm == null || m_Guild == null || m_XiaKe == null)
        {
            return;
        }
        m_Fellow.SetActive(true);
        m_Belle.SetActive(false);
        //m_EquipEnchance.SetActive(false);
        m_Farm.SetActive(false);
        m_Guild.SetActive(false);
        m_XiaKe.SetActive(false);

        bool bRet = GameManager.gameManager.PlayerDataPool.CommonData.GetCommondFlag((int)USER_COMMONFLAG.CF_XIAKEFUNCTION_OPENFLAG);
        m_XiaKe.SetActive(bRet);
        //if (bRet == true)
        //{
        //    m_XiaKe.SetActive(true);
        //}
//         bRet = GameManager.gameManager.PlayerDataPool.CommonData.GetCommondFlag((int)USER_COMMONFLAG.CF_GUILDFUNCTION_OPENFLAG);
//         if (bRet == true)
//         {
//             m_Guild.SetActive(true);
//         }
        bRet = GameManager.gameManager.PlayerDataPool.CommonData.GetCommondFlag((int)USER_COMMONFLAG.CF_GUILDFUNCTION_OPENFLAG);
        m_Guild.SetActive(bRet);
        bRet = GameManager.gameManager.PlayerDataPool.CommonData.GetCommondFlag((int)USER_COMMONFLAG.CF_RESTAURANTFUNCTION_OPENFLAG);
        m_Farm.SetActive(bRet);
        //if (bRet == true)
        //{
        //    m_Farm.SetActive(true);
        //}

		bRet = GameManager.gameManager.PlayerDataPool.CommonData.GetCommondFlag((int)USER_COMMONFLAG.COUNTER_DB_OPEN_GEM);
        m_EquipEnchance.SetActive(bRet);
        //暂时开启.
        //if (/*bRet ==*/ true)
        //{
        //    m_EquipEnchance.SetActive(true);
        //}
        bRet = GameManager.gameManager.PlayerDataPool.CommonData.GetCommondFlag((int)USER_COMMONFLAG.CF_BELLEFUNCTION_OPENFLAG);
        m_Belle.SetActive(bRet);
        //if (bRet == true)
        //{
        //    m_Belle.SetActive(true);
        //}
        bRet = GameManager.gameManager.PlayerDataPool.CommonData.GetCommondFlag((int)USER_COMMONFLAG.CF_FELLOWFUNCTION_OPENFLAG);
        m_Fellow.SetActive(bRet);
        //if (bRet == true)
        //{
        //    m_Fellow.SetActive(true);
        //}
        bRet = GameManager.gameManager.PlayerDataPool.CommonData.GetCommondFlag((int)USER_COMMONFLAG.COUNTER_DB_OPEN_QIANKUNDAI);
        m_QianKunDai.SetActive(bRet);

        bRet = GameManager.gameManager.PlayerDataPool.CommonData.GetCommondFlag((int)USER_COMMONFLAG.COUNTER_DB_OPEN_MAKE);
        m_Make.SetActive(bRet);

        m_TopGrid.Reposition();
        //m_LeftGrid.Reposition();
    }

    private bool m_NewButtonEffectFlag = false;
    public bool NewButtonEffectFlag
    {
        get { return m_NewButtonEffectFlag; }
        set { m_NewButtonEffectFlag = value; }
    }
    public void StopNewButtonEffect()
    {
        if (m_NewButtonEffectFlag == false)
        {
            return;
        }
        m_NewButtonEffectFlag = false;

        if (null == m_NewButton)
            return;

        Transform effectTrans = m_NewButton.transform.FindChild("EffectPoint");
        if (effectTrans == null)
        {
            return;
        }

        Transform spriteAniTrans = effectTrans.FindChild("SpriteAni");
        if (spriteAniTrans == null)
        {
            return;
        }

        spriteAniTrans.gameObject.SetActive(false);

        m_NewButton = null;
    }

    public void PlayNewButtonEffect()
    {
        if (m_NewButton == null)
        {
            return;
        }

        m_NewButtonEffectFlag = true;

        Transform effectTrans = m_NewButton.transform.FindChild("EffectPoint");
        if (effectTrans == null)
        {
            return;
        }

        Transform spriteAniTrans = effectTrans.FindChild("SpriteAni");
        if (spriteAniTrans == null)
        {
            return;
        }

        spriteAniTrans.gameObject.SetActive(true);
    }

    public void UpdateBelleTip()
    {
        if (BelleData.GetBelleTipCount() > 0)
        {
            m_BelleCountTip.gameObject.SetActive(true);
        }
        else
        {
            m_BelleCountTip.gameObject.SetActive(false);
        }
    }
    public void UpdateRestaurantTips()
    {
        if (null == m_RestaurantCountTip)
        {
            return;
        }
        if (RestaurantData.m_restaurantTipsCount > 0)
        {
            m_RestaurantCountTip.gameObject.SetActive(true);
            m_RestaurantCountTip.text = RestaurantData.m_restaurantTipsCount.ToString();
        }
        else
        {
            m_RestaurantCountTip.gameObject.SetActive(false);
        }
    }

    void OnLivingSkillClick()
    {
        UIManager.ShowUI(UIInfo.LivingSkill);
    }

    void OnVipClick()
    {
//        Obj_MainPlayer obj = Singleton<ObjManager>.GetInstance().MainPlayer;
//        if (obj != null)
//        {
//            if (obj.VipCost >= 0 && GameManager.gameManager.PlayerDataPool.IsServerFlagOpen(SERVER_FLAGS_ENUM.FLAG_VIP))
//            {
//                UIManager.ShowUI(UIInfo.VipRoot);
//            }
//        }
    }

    void OnQianKunDaiClick()
    {
        UIManager.ShowUI(UIInfo.BackPackRoot, BackPackLogic.SwitchQianKunDaiView);
    }

	void OnSNSClick()
	{
		UIManager.ShowUI (UIInfo.SNSRoot);
	}

	void OnGongGaoFun()
	{
		UIManager.ShowUI (UIInfo.Notice);
	}

    void OnRankClick()
    {
       // UIManager.ShowUI(UIInfo.RankRoot, OnShowRankWindow);
        CG_ASK_RANK scoreRankPak = (CG_ASK_RANK)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_RANK);
        scoreRankPak.NType = (int)GameDefine_Globe.RANKTYPE.TYPE_USERLEVELRANK;
        scoreRankPak.NPage = 0;
        scoreRankPak.SendPacket();
    }

    public void OnShowRankWindow(bool bSuccess, object param)
    {
        if (RankWindow.Instance() != null)
            RankWindow.Instance().m_TabController.ChangeTab("Tab1");
    }

    void OnClickXiake(GameObject value)
    {
        if (m_NewButton != null && m_NewButton == value)
        {
            StopNewButtonEffect();
        }
        UIManager.ShowUI(UIInfo.SwordsManRoot);
    }
    
    public void UpdateGuildAndMasterReserveMember()
    {
		//======set Flag = false  at Frist
		m_GuildNewReserveFlag.SetActive(false);
        if (null != m_GuildNewReserveFlag && null != Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            bool bFlag = false;
            if (Singleton<ObjManager>.GetInstance().MainPlayer.ShowGuildNewReserveFlag)
            {
                UInt64 myGuid = Singleton<ObjManager>.GetInstance().MainPlayer.GUID;
                if (GameManager.gameManager.PlayerDataPool.IsGuildChief() || 
                    GameManager.gameManager.PlayerDataPool.IsGuildViceChief(myGuid))
                {
                    bFlag = true;
                }
            }

            Tab_GuildBusiness tab = TableManager.GetGuildBusinessByID(GameManager.gameManager.PlayerDataPool.GuildInfo.GuildLevel, 0);
            if (null == tab )
            {
                return;
            }
            int useTime = GameManager.gameManager.PlayerDataPool.CommonData.GetCommonData((int)Games.UserCommonData.USER_COMMONDATA.CD_GUILDBUSINESS_GOTTEN_NUM);
            int curTime = tab.MemTimes - useTime;

            if (MasterWindow.GetMasterRemainNum() > 0 
                || (GameManager.gameManager.PlayerDataPool.IsHaveGuild() 
                && GameManager.gameManager.PlayerDataPool.GuildInfo.GBCanAcceptTime > 0
                && curTime > 0))
            {
                bFlag = true;
            }
			m_GuildNewReserveFlag.SetActive(bFlag);
        }
    }
    public void UpdatePartnerTip()
    {
        int remainCount = PartnerFrameLogic.GetPartnerRemainCount();
        bool bRet = GameManager.gameManager.PlayerDataPool.CommonData.GetCommondFlag((int)USER_COMMONFLAG.CF_FELLOWFUNCTION_OPENFLAG);
        if (remainCount > 0 && bRet)
        {
            m_PartnerCountTip.gameObject.SetActive(true);
            m_PartnerCountTip.text = remainCount.ToString();
        }
        else
        {
            m_PartnerCountTip.gameObject.SetActive(false);
        }
    }

    public void UpdateSkillTip()
    {
        if (null == m_SkillCountTip)
        {
            return;
        }
        if (SkillRootLogic.GetCanLevUpSkillCount() > 0)
        {
            m_SkillCountTip.gameObject.SetActive(true);
          //  m_SkillCountTip.text = SkillRootLogic.GetCanLevUpSkillCount().ToString(); 
        }
        else
        {
            m_SkillCountTip.gameObject.SetActive(false);
        }
    }

    void CloseWindow()
    {
        
        if (PlayerFrameLogic.Instance() != null)
        {
            PlayerFrameLogic.Instance().PlayerFrameHeadOnClick();
        }
    }
	
	// 新手指引
	void ShowNewPlayerGuide()
	{
		if (m_NewPlayerGuideIndex < 0) 
		{
			return;
		}
		
		NewPlayerGuidLogic.CloseWindow();

		switch (m_NewPlayerGuideIndex)
		{
		case (int)GameDefine_Globe.NEWOLAYERGUIDE.PET_GAIN: // 召唤伙伴
		case (int)GameDefine_Globe.NEWOLAYERGUIDE.PET_FIGHT: // 伙伴抽取
			NewPlayerGuidLogic.OpenWindow(m_Fellow, 140, 140, "", "right", 2, true, true);
			break;
		case (int)GameDefine_Globe.NEWOLAYERGUIDE.EQUIP_GEM: // 宝石
		case (int)GameDefine_Globe.NEWOLAYERGUIDE.EQUIP_INTENSIFY: // 装备强化
		case (int)GameDefine_Globe.NEWOLAYERGUIDE.EQUIP_START: // 装备打星
			NewPlayerGuidLogic.OpenWindow(m_EquipEnchance, 140, 140, "", "right", 2, true, true);
			break;
		case (int)GameDefine_Globe.NEWOLAYERGUIDE.MINGJIANG_QIMI:
		case (int)GameDefine_Globe.NEWOLAYERGUIDE.MINGJIANG_FIGHE:
			NewPlayerGuidLogic.OpenWindow(m_Belle, 140, 140, "", "right", 2, true, true);
			break;
		case (int)GameDefine_Globe.NEWOLAYERGUIDE.BAOWU:
			NewPlayerGuidLogic.OpenWindow(m_XiaKe, 140, 140, "", "right", 2, true, true);
			break;
		case (int)GameDefine_Globe.NEWOLAYERGUIDE.RESTAURANT:
			NewPlayerGuidLogic.OpenWindow(m_Farm, 140, 140, "", "right", 2, true, true);
			break;
		case (int)GameDefine_Globe.NEWOLAYERGUIDE.COMPOUND:
			NewPlayerGuidLogic.OpenWindow(m_QianKunDai, 140, 140, "", "right", 2, true, true);
			break;
		case (int)GameDefine_Globe.NEWOLAYERGUIDE.UI_CLOSE:
			NewPlayerGuidLogic.OpenWindow(m_BackButton, 140, 140, "", "right", 2, true, true);
			break;
		default:
			m_NewPlayerGuideIndex = -1;
			break;
		}
	}
}
