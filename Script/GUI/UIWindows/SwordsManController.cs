using UnityEngine;
using System.Collections;
using Module.Log;
using Games.SwordsMan;
using Games.GlobeDefine;
using System.Collections.Generic;
using GCGame;
using GCGame.Table;
/********************************************************************
    created:	2014-06-25
    filename: 	SwordsManController.cs
    author:		grx
    purpose:	侠客界面
*********************************************************************/
public class SwordsManController : MonoBehaviour {

    public GameObject   m_SwordsManGrid;
    public UILabel      m_LabelCoinValue;
    public UILabel      m_LabelScoreValue;
    public UILabel      m_LabelCostCoin;
    public UILabel      m_LabelTenVisitCostCoin;
    public UILabel      m_LabelCombatValue;

    public UILabel[]    m_LabelSlots;

    public GameObject[] m_SwordsManIcons;
    public GameObject[] m_SwordsManQualitys;
    public GameObject[] m_SlotLock;
    public UILabel[] m_SwordsManLevel;
    public GameObject[] m_SwordsManLock;

    public UILabel m_LabelPacSize;

    //public UILabel m_LableButton;
    //public UILabel m_LableTenVisitButton;

    //public UIButtonColor m_ButtonColor;
    //public UIButtonColor m_TenVisitButtonColor;

    public GameObject m_VisitWindow;
    public GameObject m_SwordsManEquipWindow;
    public UISprite []m_SwordsManVisitStateSprite;

    public GameObject m_VisitButton;
    public GameObject m_BackButton;

	public GameObject m_VisiSwordManBtn;

	public GameObject m_CloseBtn;

    static private Color m_WhiteQualityColor = new Color(1.0f, 1.0f, 1.0f, 0.8f);
    static private Color m_GreenQualityColor = new Color(51.0f / 255.0f, 204.0f / 255.0f, 102.0f / 255.0f, 0.8f);
    static private Color m_BlueQualityColor = new Color(51.0f / 255.0f, 204.0f / 255.0f, 255.0f / 255.0f, 0.8f);
    static private Color m_PurpleQualityColor = new Color(204.0f / 255.0f, 102.0f / 255.0f, 255.0f / 255.0f, 0.8f);
    static private Color m_OrangeQualityColor = new Color(255.0f / 255.0f, 153.0f / 255.0f, 51.0f / 255.0f, 0.8f);

    // 
    public const int VISIT_SWORDSMAN_COSTCOIN = 5000;
    // 
    public const int TNE_VISIT_SWORDSMAN_COUNT = 10;

    static private SwordsManController m_Instance = null;
	
	private int swordManNumber = 0;

	private int m_NewPlayerGuide_Step = -1;
	public int NewPlayerGuide_Step
	{
		get{ return m_NewPlayerGuide_Step; }
		set{ m_NewPlayerGuide_Step = value; }
	}

    public enum SWORDSMANVISITSTATE
    {
        WHITE = 0,      //白色
        GREEN = 1,      //绿色
        BLUE = 2,       //蓝色
        PURPLE = 3,     //紫色
        ORANGE = 4,     //橙色
    }

    public static SwordsManController Instance()
    {
        return m_Instance;
    }

    //void Awake()
    //{
    //    m_Instance = this;
    //}

    //// Use this for initialization
    //void Start () {
    //    Init();
    //}

    //void OnDestroy()
    //{
    //    m_Instance = null;
    //}

    void OnEnable()
    {
        m_Instance = this;
        Init();
		Check_NewPlayerGuide ();
    }

    void OnDisable()
    {
        m_Instance = null;
    }

    private void Init()
    {
        UpdateSwordsManBackPack();
        UpdateSwordsManEquipPack();
        UpdateSwordsManVisitState();
        UpdateSwordsManScore();
        UpdateCoin();
        if (m_VisitWindow != null)
        {
            m_VisitWindow.SetActive(false);
        }
        if (m_SwordsManEquipWindow != null)
        {
            m_SwordsManEquipWindow.SetActive(true);
        }
        if (m_VisitButton != null)
        {
            m_VisitButton.SetActive(true);
        }
        if (m_BackButton != null)
        {
            m_BackButton.SetActive(false);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void UpdateSwordsManBackPack()
    {
        if (null == m_SwordsManGrid)
        {
            LogModule.ErrorLog("UpdateSwordsManBackPack m_SwordsManGrid is null");
            return;
        }
        UIManager.LoadItem(UIInfo.SwordsManItem, OnLoadSwordsManItem);
    }

    /// <summary>
    /// 
    /// </summary>
    void OnLoadSwordsManItem(GameObject resObj, object param)
    {
        if (null == m_SwordsManGrid)
        {
            LogModule.ErrorLog("OnLoadSwordsManItem m_SwordsManGrid is null");
            return;
        }
        Utils.CleanGrid(m_SwordsManGrid.gameObject);
        SwordsManContainer oSwordsManContainer = GameManager.gameManager.PlayerDataPool.GetSwordsManContainer(SwordsManContainer.PACK_TYPE.TYPE_BACKPACK);
        if ( null == oSwordsManContainer)
        {
            LogModule.ErrorLog("OnLoadSwordsManItem::oSwordsManContainer is null");
            return;
        }
        List<SwordsMan> itemlist = SwordsManTool.ItemFilter(oSwordsManContainer);

		swordManNumber = itemlist.Count;
        for (int i = 0; i < itemlist.Count; i++)
        {
            SwordsMan oSwordsMan = itemlist[i];
            if (null == oSwordsMan)
            {
                LogModule.ErrorLog("OnLoadSwordsManItem::oSwordsMan is null");
                break;
            }
            if (false == oSwordsMan.IsValid())
            {
                continue;
            }
            SwordsManItem oSwordsManItem = SwordsManItem.CreateItem(m_SwordsManGrid, resObj, i.ToString(), this);
            if (null == oSwordsManItem)
            {
                LogModule.ErrorLog("OnLoadSwordsManItem::oSwordsManItem is null");
                break;
            }
            oSwordsManItem.SetData(oSwordsMan);
        }
        m_SwordsManGrid.GetComponent<UIGrid>().repositionNow = true;

        if (m_LabelPacSize != null)
        {
            m_LabelPacSize.text = string.Format("{0}/{1}", oSwordsManContainer.GetSwordsManCount(), oSwordsManContainer.ContainerSize);
            
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void UpdateSwordsManEquipPack()
    {
        SwordsManContainer oSwordsManContainer = GameManager.gameManager.PlayerDataPool.GetSwordsManContainer(SwordsManContainer.PACK_TYPE.TYPE_EQUIPPACK);
        if (null == oSwordsManContainer)
        {
            LogModule.ErrorLog("UpdateSwordsManEquipPack::oSwordsManContainer is null");
            return;
        }
        int nCombatValue = 0;
        for (int i = 0; i < m_SwordsManIcons.Length && i < m_SwordsManQualitys.Length && i < m_SwordsManLock.Length && i < m_SlotLock.Length && i < m_SwordsManLevel.Length && i < m_LabelSlots.Length; i++)
        {
            if (i < oSwordsManContainer.ContainerSize)
            {
                SwordsMan oSwordsMan = oSwordsManContainer.GetSwordsMan(i);
                if (null == oSwordsMan)
                {
                    LogModule.ErrorLog("OnLoadSwordsManItem::oSwordsMan is null");
                    break;
                }              
                m_SlotLock[i].SetActive(false);
                if (oSwordsMan.IsValid())
                {
                    m_SwordsManIcons[i].SetActive(true);           
                    m_SwordsManIcons[i].GetComponent<UISprite>().spriteName = oSwordsMan.GetIcon();
                    m_SwordsManQualitys[i].GetComponent<UISprite>().spriteName = SwordsMan.GetQualitySpriteName((SwordsMan.SWORDSMANQUALITY)oSwordsMan.Quality);
                    m_SwordsManQualitys[i].SetActive(true);
                   
                    m_SwordsManLevel[i].gameObject.SetActive(true);
                    m_SwordsManLevel[i].text = StrDictionary.GetClientDictionaryString("#{2673}", oSwordsMan.Level);
                    
                    if (oSwordsMan.Locked)
                    {
                        m_SwordsManLock[i].SetActive(true);
                    }
                    else
                    {
                        m_SwordsManLock[i].SetActive(false);
                    }                                  
                    nCombatValue = nCombatValue + oSwordsMan.GetCombatValue();
                }
                else
                {
                    m_SwordsManQualitys[i].SetActive(false);
                    m_SwordsManIcons[i].SetActive(false);                              
                    m_SwordsManLock[i].SetActive(false);
                    m_SwordsManLevel[i].gameObject.SetActive(false);
                    
                }               
                m_LabelSlots[i].gameObject.SetActive(false);
                              
            }
            else
            {
                //需要解锁
              
               m_SlotLock[i].SetActive(true);               
                m_SwordsManQualitys[i].SetActive(false);
                m_SwordsManIcons[i].SetActive(false);
                m_LabelSlots[i].gameObject.SetActive(true);
                m_SwordsManLevel[i].gameObject.SetActive(false);
                Tab_SwordsEquipPackUnlock SwordsManUnLockTable = TableManager.GetSwordsEquipPackUnlockByID(i+1, 0);
                if (SwordsManUnLockTable != null)
                {
                    //string strUnLock = SwordsManUnLockTable.NeedLevel.ToString() + "级开放";
                    string strUnLock = StrDictionary.GetClientDictionaryString("#{2674}", SwordsManUnLockTable.NeedLevel);
                    m_LabelSlots[i].text = strUnLock;
                }        
            }
        }
        if (m_LabelCombatValue != null)
        {
            m_LabelCombatValue.text = nCombatValue.ToString();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void UpdateSwordsManVisitState()
    {
        int nState = GameManager.gameManager.PlayerDataPool.SwordsManVisitState;
       
        if (m_LabelCostCoin != null)
        {
            m_LabelCostCoin.text = VISIT_SWORDSMAN_COSTCOIN.ToString();
        }
        //if (m_LableButton != null)
        //{
        //   m_LableButton.text = GetButtonStringByState((SWORDSMANVISITSTATE)nState);
        //}
        //if (m_ButtonColor != null)
        //{
        //    m_ButtonColor.defaultColor = GetButtonColorByState((SWORDSMANVISITSTATE)nState);
        //    m_ButtonColor.OnHover(false);
        //}

        if (m_LabelTenVisitCostCoin != null)
        {
            int nCostCoin = VISIT_SWORDSMAN_COSTCOIN * TNE_VISIT_SWORDSMAN_COUNT;
            m_LabelTenVisitCostCoin.text = nCostCoin.ToString();
        }

        //if (m_LableTenVisitButton != null)
        //{
        //    m_LableTenVisitButton.text = GetButtonStringByState((SWORDSMANVISITSTATE)nState);
        //}

        //if (m_TenVisitButtonColor != null)
        //{
        //    m_TenVisitButtonColor.defaultColor = GetButtonColorByState((SWORDSMANVISITSTATE)nState);
        //    m_TenVisitButtonColor.OnHover(false);
        //}
        for (int i = 0; i < m_SwordsManVisitStateSprite.Length; i++ )
        {
            m_SwordsManVisitStateSprite[i].gameObject.SetActive(false);
        }
        if (nState >= 0 && nState < m_SwordsManVisitStateSprite.Length)
        {
            m_SwordsManVisitStateSprite[nState].gameObject.SetActive(true);
        }
        
    }
    
    
    public static string GetButtonStringByState(SWORDSMANVISITSTATE nVisitState)
    {
        switch (nVisitState)
        {
            case SWORDSMANVISITSTATE.WHITE:
                return StrDictionary.GetClientDictionaryString("#{2635}");
            case SWORDSMANVISITSTATE.GREEN:
                return StrDictionary.GetClientDictionaryString("#{2636}");
            case SWORDSMANVISITSTATE.BLUE:
                return StrDictionary.GetClientDictionaryString("#{2637}");
            case SWORDSMANVISITSTATE.PURPLE:
                return StrDictionary.GetClientDictionaryString("#{2638}");
            case SWORDSMANVISITSTATE.ORANGE:
                return StrDictionary.GetClientDictionaryString("#{2639}");
            default:
                return StrDictionary.GetClientDictionaryString("#{2635}");
        }
    }

    public static Color GetButtonColorByState(SWORDSMANVISITSTATE nVisitState)
    {
        switch (nVisitState)
        {
            case SWORDSMANVISITSTATE.WHITE:
                return m_WhiteQualityColor;
            case SWORDSMANVISITSTATE.GREEN:
                return m_GreenQualityColor;
            case SWORDSMANVISITSTATE.BLUE:
                return m_BlueQualityColor;
            case SWORDSMANVISITSTATE.PURPLE:
                return m_PurpleQualityColor;
            case SWORDSMANVISITSTATE.ORANGE:
                return m_OrangeQualityColor;
            default:
                return m_WhiteQualityColor;
        }
    }

    /// <summary>
    /// 关闭按钮
    /// </summary>
    void OnCloseClick()
    {
        StopEffect();
		NewPlayerGuidLogic.CloseWindow();
		NewPlayerGuide_Step = -1;
        UIManager.CloseUI(UIInfo.SwordsManRoot);
    }

    /// <summary>
    /// 积分兑换
    /// </summary>
    void OnScoreExchange()
    {
        StopEffect();
        UIManager.CloseUI(UIInfo.SwordsManRoot);
        UIManager.ShowUI(UIInfo.SwordsManShopRoot);
    }

    /// <summary>
    /// 
    /// </summary>
    void StopEffect()
    {
        if (BackCamerControll.Instance() != null)
        {
            BackCamerControll.Instance().StopSceneEffect(144, true);
            BackCamerControll.Instance().StopSceneEffect(177, true);
        }
    }

    /// <summary>
    /// 拜访侠客
    /// </summary>
    void OnVisitXiake()
    {
		if(swordManNumber+1 > SwordsManContainer.SWORDSMAN_BACKPACK_SIZE)
		{
			Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1097}");
			return;
		}

		if (1 == NewPlayerGuide_Step) 
		{
			NewPlayerGuide((int)GameDefine_Globe.NEWOLAYERGUIDE.UI_CLOSE);
		}

        CG_VISIT_SWORDSMAN packet = (CG_VISIT_SWORDSMAN)PacketDistributed.CreatePacket(MessageID.PACKET_CG_VISIT_SWORDSMAN);
        packet.Userguid = Singleton<ObjManager>.GetInstance().MainPlayer.GUID;
        packet.Tenvisit = 0;
        packet.SendPacket();
    }

    /// <summary>
    /// 拜访侠客
    /// </summary>
    void OnTenVisitXiake()
    {

		if(swordManNumber+10 > SwordsManContainer.SWORDSMAN_BACKPACK_SIZE)
		{
			Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{5648}");
			return;
		}

        CG_VISIT_SWORDSMAN packet = (CG_VISIT_SWORDSMAN)PacketDistributed.CreatePacket(MessageID.PACKET_CG_VISIT_SWORDSMAN);
        packet.Userguid = Singleton<ObjManager>.GetInstance().MainPlayer.GUID;
        packet.Tenvisit = 1;
        packet.SendPacket();
    }

    /// <summary>
    /// ZHENGLI
    /// </summary>
    void OnZhengLi()
    {

    }

    /// <summary>
    /// 设置侠客积分
    /// </summary>
    public void UpdateSwordsManScore()
    {
        int nScore = GameManager.gameManager.PlayerDataPool.SwordsManScore;
        if (null == m_LabelScoreValue)
        {
            return;
        }
        m_LabelScoreValue.text = nScore.ToString();
    }

    /// <summary>
    /// 设置金币
    /// </summary>
    public void UpdateCoin()
    {
        int nCoin = GameManager.gameManager.PlayerDataPool.Money.GetMoney_Coin();
        if (null == m_LabelCoinValue)
        {
            return;
        }
        m_LabelCoinValue.text = nCoin.ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    void OnClickEquipSwordsMan1()
    {
        SwordsMan oSwordsMan = GameManager.gameManager.PlayerDataPool.SwordsManEquipPack.GetSwordsMan(0);
        if (oSwordsMan != null && oSwordsMan.IsValid())
        {
            SwordsManToolTipsLogic.ShowSwordsManTooltip(oSwordsMan, SwordsManToolTipsLogic.SwordsMan_ShowType.Equiped);
        }
    }

    /// <summary>
    /// 
    /// </summary>
     void OnClickEquipSwordsMan2()
    {
        SwordsMan oSwordsMan = GameManager.gameManager.PlayerDataPool.SwordsManEquipPack.GetSwordsMan(1);
        if (oSwordsMan != null && oSwordsMan.IsValid())
        {
            SwordsManToolTipsLogic.ShowSwordsManTooltip(oSwordsMan, SwordsManToolTipsLogic.SwordsMan_ShowType.Equiped);
        }
    }

    /// <summary>
    /// 
    /// </summary>
     void OnClickEquipSwordsMan3()
    {
        SwordsMan oSwordsMan = GameManager.gameManager.PlayerDataPool.SwordsManEquipPack.GetSwordsMan(2);
        if (oSwordsMan != null && oSwordsMan.IsValid())
        {
            SwordsManToolTipsLogic.ShowSwordsManTooltip(oSwordsMan, SwordsManToolTipsLogic.SwordsMan_ShowType.Equiped);
        }
    }

    /// <summary>
    /// 
    /// </summary>
     void OnClickEquipSwordsMan4()
    {
        SwordsMan oSwordsMan = GameManager.gameManager.PlayerDataPool.SwordsManEquipPack.GetSwordsMan(3);
        if (oSwordsMan != null && oSwordsMan.IsValid())
        {
            SwordsManToolTipsLogic.ShowSwordsManTooltip(oSwordsMan, SwordsManToolTipsLogic.SwordsMan_ShowType.Equiped);
        }
    }

    /// <summary>
    /// 
    /// </summary>
     void OnClickEquipSwordsMan5()
    {
        SwordsMan oSwordsMan = GameManager.gameManager.PlayerDataPool.SwordsManEquipPack.GetSwordsMan(4);
        if (oSwordsMan != null && oSwordsMan.IsValid())
        {
            SwordsManToolTipsLogic.ShowSwordsManTooltip(oSwordsMan, SwordsManToolTipsLogic.SwordsMan_ShowType.Equiped);
        }
    }

    /// <summary>
    /// 
    /// </summary>
     void OnClickEquipSwordsMan6()
    {
        SwordsMan oSwordsMan = GameManager.gameManager.PlayerDataPool.SwordsManEquipPack.GetSwordsMan(5);
        if (oSwordsMan != null && oSwordsMan.IsValid())
        {
            SwordsManToolTipsLogic.ShowSwordsManTooltip(oSwordsMan, SwordsManToolTipsLogic.SwordsMan_ShowType.Equiped);
        }
    }
     /// <summary>
     /// 
     /// </summary>
     void OnClickEquipSwordsMan7()
     {
         SwordsMan oSwordsMan = GameManager.gameManager.PlayerDataPool.SwordsManEquipPack.GetSwordsMan(6);
         if (oSwordsMan != null && oSwordsMan.IsValid())
         {
             SwordsManToolTipsLogic.ShowSwordsManTooltip(oSwordsMan, SwordsManToolTipsLogic.SwordsMan_ShowType.Equiped);
         }
     }
     /// <summary>
     /// 
     /// </summary>
     void OnClickEquipSwordsMan8()
     {
         SwordsMan oSwordsMan = GameManager.gameManager.PlayerDataPool.SwordsManEquipPack.GetSwordsMan(7);
         if (oSwordsMan != null && oSwordsMan.IsValid())
         {
             SwordsManToolTipsLogic.ShowSwordsManTooltip(oSwordsMan, SwordsManToolTipsLogic.SwordsMan_ShowType.Equiped);
         }
     }

    void OpenVisitWindow()
     {
        if (null == m_VisitWindow)
        {
            LogModule.ErrorLog("OpenVisitWindow::m_VisitWindow is null");
            return;
        }
        if (null == m_SwordsManEquipWindow)
        {
            LogModule.ErrorLog("OpenVisitWindow::m_SwordsManEquipWindow is null");
            return;
        }
        if (null == m_VisitButton)
        {
            LogModule.ErrorLog("OpenVisitWindow::m_VisitButton is null");
            return;
        }
        if (null == m_BackButton)
        {
            LogModule.ErrorLog("OpenVisitWindow::m_BackButton is null");
            return;
        }
		if (0 == NewPlayerGuide_Step) 
		{
			NewPlayerGuide(1);		
		}
        m_VisitWindow.SetActive(true);
        m_SwordsManEquipWindow.SetActive(false);
        m_VisitButton.SetActive(false);
        m_BackButton.SetActive(true);
     }

    void OnBack()
    {
        if (null == m_VisitWindow)
        {
            LogModule.ErrorLog("OnBack::m_VisitWindow is null");
            return;
        }
        if (null == m_SwordsManEquipWindow)
        {
            LogModule.ErrorLog("OnBack::m_SwordsManEquipWindow is null");
            return;
        }
        if (null == m_VisitButton)
        {
            LogModule.ErrorLog("OnBack::m_VisitButton is null");
            return;
        }
        if (null == m_BackButton)
        {
            LogModule.ErrorLog("OnBack::m_BackButton is null");
            return;
        }
        m_VisitWindow.SetActive(false);
        m_SwordsManEquipWindow.SetActive(true);
        m_VisitButton.SetActive(true);
        m_BackButton.SetActive(false);
    }

	void Check_NewPlayerGuide()
	{
		if (SwordsManController.Instance())
		{
			int nStep = MenuBarLogic.Instance().NewPlayerGuideIndex;
			if ((int)GameDefine_Globe.NEWOLAYERGUIDE.BAOWU == nStep)
			{
				NewPlayerGuide(0);
				MenuBarLogic.Instance().NewPlayerGuideIndex = -1;
			}
		}
	}

	public void NewPlayerGuide(int nIndex)
	{
		if (nIndex < 0) 
		{
			return;	
		}

		NewPlayerGuidLogic.CloseWindow();
		
		m_NewPlayerGuide_Step = nIndex;
		switch(nIndex)
		{
		case 0:
			if(null != m_VisitButton)
			{
				NewPlayerGuidLogic.OpenWindow(m_VisitButton, 160, 80, "", "bottom", 2, true, true);
			}
			break;
		case 1:
			if(null != m_VisiSwordManBtn)
			{
				NewPlayerGuidLogic.OpenWindow(m_VisiSwordManBtn, 160, 80, "", "bottom", 2, true, true);
			}
			break;
		case (int)GameDefine_Globe.NEWOLAYERGUIDE.UI_CLOSE:
			if(null != m_CloseBtn)
			{
				NewPlayerGuidLogic.OpenWindow(m_CloseBtn, 140, 140, "", "right", 2, true, true);
			}
			break;
		}
	}
}

