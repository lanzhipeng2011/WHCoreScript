using UnityEngine;
using System.Collections;
using GCGame;
using System.Collections.Generic;
using Games.GlobeDefine;
using GCGame.Table;

public class FashionLogic : MonoBehaviour {

    public enum TAB_CONTENT
    {
        FASHION_SHOP,
        MY_FASHION,
    }

    private static FashionLogic m_Instance = null;
    public static FashionLogic Instance()
    {
        return m_Instance;
    }

    public GameObject m_FashionItemGrid;
    public UIToggle m_ShowFashionToggle;
    public TabController m_BtnTabs;
    public TabController m_DeadlineTabs;
    public GameObject m_DefaultStatus;
    public GameObject m_BuyStatus;
    public UILabel m_ChooseFashionName;
    public UILabel m_YuanBaoNeedLabel;
    public UILabel m_TimeLeftLabel;
    public GameObject m_EquipButton;
    public GameObject m_TakeOffButton;
    public GameObject m_RenewButton;
    public UIImageButton m_BuyButton;
    public UILabel m_BuyButtonLabel;
    public GameObject m_BottomInfo;
    public GameObject[] m_TimeFashionObject;

    public GameObject m_FashionItem;
    private TAB_CONTENT m_eCurTab = TAB_CONTENT.FASHION_SHOP;
    public TAB_CONTENT CurTab
    {
        get { return m_eCurTab; }
        set { m_eCurTab = value; }
    }
    private int m_BuyType = (int)CG_BUY_FASHION.BUYTYPE.TYPE_WEEK;
    private int m_curChooseFashion = GlobeVar.INVALID_ID;
    public int CurChooseFashion
    {
        get { return m_curChooseFashion; }
        set { m_curChooseFashion = value; }
    }
    private bool m_IsFitOn = false;
    public bool IsFitOn
    {
        get { return m_IsFitOn; }
        set { m_IsFitOn = value; }
    }
    private int m_BuyFashionRefreshCache = GlobeVar.INVALID_ID;
    private string m_DeadLineChoose = "Timeleft1-Week";
    private bool m_IsOpenTimeFashion = true;

	// Use this for initialization
	void OnEnable () 
    {
        m_Instance = this;
        InvokeRepeating("SlowUpdate", 0f, 60.0f);
        m_ShowFashionToggle.startsActive = GameManager.gameManager.PlayerDataPool.ShowFashion;
        m_BtnTabs.delTabChanged = OnBtnTabChange;
        m_DeadlineTabs.delTabChanged = TimeleftOnClick;
        InitFashionInfo();
        m_IsOpenTimeFashion = GameManager.gameManager.PlayerDataPool.IsServerFlagOpen(SERVER_FLAGS_ENUM.FLAG_TIMEFASHION);
        InitTimeFashionObject();
	}

    void OnDisable()
    {
        CancelInvoke("SlowUpdate");

        foreach (FashionItemLogic item in m_FashionItemGrid.GetComponentsInChildren<FashionItemLogic>())
        {
            Destroy(item.gameObject);
        }

        m_Instance = null;
    }

    void SlowUpdate()
    {
        UpdateCurChooseFashion();
    }


    void InitFashionInfo()
    {
        m_BtnTabs.ChangeTab("FashionShop");
    }

    

    public void HandleSendFashionInfo(int nFashionID)
    {
        if (m_eCurTab == TAB_CONTENT.MY_FASHION && nFashionID == m_curChooseFashion)
        {
            m_DefaultStatus.SetActive(true);
            m_BuyStatus.SetActive(false);
            //m_BottomInfo.SetActive(true);

            UpdateCurChooseFashion();
        }
        if (m_eCurTab == TAB_CONTENT.FASHION_SHOP && nFashionID == m_curChooseFashion)
        {
            m_BuyFashionRefreshCache = nFashionID;
            m_BtnTabs.ChangeTab("MyFashion");
        }
    }

    public void HandleSendCurFashion(int nCurFashionID)
    {
        GameManager.gameManager.PlayerDataPool.CurFashionID = nCurFashionID;
        if (m_eCurTab == FashionLogic.TAB_CONTENT.MY_FASHION)
        {
            FashionLogic.Instance().UpdateCurChooseFashion();
        }

        if (GameManager.gameManager.PlayerDataPool.CurFashionID == GlobeVar.INVALID_ID)
        {
            // 脱时装
            if (m_ShowFashionToggle.value)
            {
                m_ShowFashionToggle.value = false;
                ShowFashionChange();
            }           
        }
        else
        {
            // 穿时装
            if (!m_ShowFashionToggle.value)
            {
                m_ShowFashionToggle.value = true;
                ShowFashionChange();
            }
        }
    }

    void ShowFashionChange()
    {
        CG_CHANGE_SHOWFASHION packet = (CG_CHANGE_SHOWFASHION)PacketDistributed.CreatePacket(MessageID.PACKET_CG_CHANGE_SHOWFASHION);
        packet.NoParam = 1;
        packet.SendPacket();
    }

    public void ChooseFashionItem(int nFashionID)
    {
        foreach (FashionItemLogic item in m_FashionItemGrid.GetComponentsInChildren<FashionItemLogic>())
        {
            item.UpdateHightLight(nFashionID);
        }

        Tab_FashionData tabFashion = TableManager.GetFashionDataByID(nFashionID, 0);
        if (tabFashion == null)
        {
            return;
        }

        m_curChooseFashion = nFashionID;
        m_ChooseFashionName.text = tabFashion.Name;
        m_IsFitOn = true;

        if (m_eCurTab == TAB_CONTENT.FASHION_SHOP)
        {
            m_DefaultStatus.SetActive(false);
            m_BuyStatus.SetActive(true);
            //m_BottomInfo.SetActive(true);

            m_DeadlineTabs.ChangeTab(m_DeadLineChoose);

            if (GameManager.gameManager.PlayerDataPool.FashionDeadline[m_curChooseFashion] == GlobeVar.INVALID_ID)
            {
                m_BuyButton.isEnabled = false;
                m_BuyButtonLabel.text = StrDictionary.GetClientDictionaryString("#{2031}");
            }
            else
            {
                m_BuyButton.isEnabled = true;
                m_BuyButtonLabel.text = StrDictionary.GetClientDictionaryString("#{1672}");
            }
        }
        else
        {
            m_DefaultStatus.SetActive(true);
            m_BuyStatus.SetActive(false);
            //m_BottomInfo.SetActive(true);

            UpdateCurChooseFashion();
        }
    }

    public void ClearFashionItemChoose()
    {
        ChooseFashionItem(GlobeVar.INVALID_ID);
    }

    void OnBtnTabChange(TabButton button)
    {
        if (button.name == "FashionShop")
        {
            m_eCurTab = TAB_CONTENT.FASHION_SHOP;
            UpdateTabContent();
        }
        else if (button.name == "MyFashion")
        {
            m_eCurTab = TAB_CONTENT.MY_FASHION;
            UpdateTabContent(m_BuyFashionRefreshCache);
            m_BuyFashionRefreshCache = GlobeVar.INVALID_ID;
        }
    }

    void TimeleftOnClick(TabButton value)
    {
        Tab_FashionData tabFashionData = TableManager.GetFashionDataByID(m_curChooseFashion, 0);
        if (tabFashionData == null)
        {
            return;
        }

        if (m_IsOpenTimeFashion)
        {
            if (value.name == "Timeleft1-Week")
            {
                m_DeadLineChoose = "Timeleft1-Week";
                m_BuyType = (int)CG_BUY_FASHION.BUYTYPE.TYPE_WEEK;
                m_YuanBaoNeedLabel.text = StrDictionary.GetClientDictionaryString("#{1673}", tabFashionData.PriceWeek);
            }
            if (value.name == "Timeleft2-Month")
            {
                m_DeadLineChoose = "Timeleft2-Month";
                m_BuyType = (int)CG_BUY_FASHION.BUYTYPE.TYPE_MONTH;
                m_YuanBaoNeedLabel.text = StrDictionary.GetClientDictionaryString("#{1673}", tabFashionData.PriceMonth);
            }
        }
        
        if (value.name == "Timeleft3-Forever")
        {
            m_DeadLineChoose = "Timeleft3-Forever";
            m_BuyType = (int)CG_BUY_FASHION.BUYTYPE.TYPE_FOREVER;
            m_YuanBaoNeedLabel.text = StrDictionary.GetClientDictionaryString("#{1673}", tabFashionData.PriceForever);
        }
    }

    void UpdateTabContent(int nDefaultChoose = GlobeVar.INVALID_ID)
    {
        if (m_FashionItem == null)
        {
            return;
        }

        Utils.CleanGrid(m_FashionItemGrid);

        if (m_eCurTab == TAB_CONTENT.FASHION_SHOP)
        {
            int nFashionCount = TableManager.GetFashionData().Count;
            for (int i = 0; i < nFashionCount; i++)
            {
                if (i < GlobeVar.MAX_FASHION_SIZE)
                {
                    Tab_FashionData tabFashion = TableManager.GetFashionDataByID(i, 0);
                    if (tabFashion != null)
                    {
                        if (tabFashion.IsSale == 1)
                        {
                            GameObject newFashionItem = Utils.BindObjToParent(m_FashionItem, m_FashionItemGrid, i.ToString());
                            if (newFashionItem != null)
                            {
                                FashionItemLogic newFashionItemLogic = newFashionItem.GetComponent<FashionItemLogic>();
                                if (newFashionItemLogic != null)
                                {
                                    newFashionItemLogic.InitInfo(i);
                                    if (nDefaultChoose == GlobeVar.INVALID_ID)
                                    {
                                        if (i == 0)
                                        {
                                            newFashionItemLogic.HandleItemChoose();
                                        }
                                    }
                                    else
                                    {
                                        if (i == nDefaultChoose)
                                        {
                                            newFashionItemLogic.HandleItemChoose();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            m_FashionItemGrid.GetComponent<UIGrid>().Reposition();
        }
        else if (m_eCurTab == TAB_CONTENT.MY_FASHION)
        {
            bool first = true;
            int[] deadlineArray = GameManager.gameManager.PlayerDataPool.FashionDeadline;
            for (int i = 0; i < deadlineArray.Length; i++)
            {
                if (deadlineArray[i] != 0)
                {
                    GameObject newFashionItem = Utils.BindObjToParent(m_FashionItem, m_FashionItemGrid, i.ToString());
                    if (newFashionItem != null)
                    {
                        FashionItemLogic newFashionItemLogic = newFashionItem.GetComponent<FashionItemLogic>();
                        if (null != newFashionItemLogic)
                        {
                            newFashionItemLogic.InitInfo(i);
                            if (nDefaultChoose == GlobeVar.INVALID_ID)
                            {
                                if (first)
                                {
                                    newFashionItemLogic.HandleItemChoose();
                                    first = false;
                                }
                            }
                            else
                            {
                                if (i == nDefaultChoose)
                                {
                                    newFashionItemLogic.HandleItemChoose();
                                }
                            }
                            if (i == Singleton<ObjManager>.Instance.MainPlayer.CurFashionID)
                            {
                                newFashionItemLogic.ShowChosenSprite();
                            }
                        }
                    }
                }
            }
            m_FashionItemGrid.GetComponent<UIGrid>().Reposition();
        }
    }

    void BuyFashion()
    {
        CG_BUY_FASHION packet = (CG_BUY_FASHION)PacketDistributed.CreatePacket(MessageID.PACKET_CG_BUY_FASHION);
        packet.FashionID = m_curChooseFashion;
        packet.BuyType =(uint)m_BuyType;
        packet.SendPacket();
    }

    void EquipOnClick()
    {
        CG_WEAR_FASHION packet = (CG_WEAR_FASHION)PacketDistributed.CreatePacket(MessageID.PACKET_CG_WEAR_FASHION);
        packet.FashionID = m_curChooseFashion;
        packet.SendPacket();
    }

    void TakeOffOnClick()
    {
        CG_TAKEOFF_FASHION packet = (CG_TAKEOFF_FASHION)PacketDistributed.CreatePacket(MessageID.PACKET_CG_TAKEOFF_FASHION);
        packet.FashionID = m_curChooseFashion;
        packet.SendPacket();
    }

    public void UpdateCurChooseFashion()
    {
        if (m_curChooseFashion == GlobeVar.INVALID_ID)
        {
            return;
        }

        int nRemainTime = GameManager.gameManager.PlayerDataPool.FashionDeadline[m_curChooseFashion] - GlobalData.ServerAnsiTime;
        if (GameManager.gameManager.PlayerDataPool.FashionDeadline[m_curChooseFashion] == 0)
        {
            // 无时装
            m_TimeLeftLabel.text = StrDictionary.GetClientDictionaryString("#{1668}");
            m_EquipButton.SetActive(false);
            m_TakeOffButton.SetActive(false);
            m_RenewButton.SetActive(false);
        }
        else
        {
            if (GameManager.gameManager.PlayerDataPool.CurFashionID == m_curChooseFashion)
            {
                m_EquipButton.SetActive(false);
                m_TakeOffButton.SetActive(true);
            }
            else
            {
                m_EquipButton.SetActive(true);
                m_TakeOffButton.SetActive(false);
            }

            if (GameManager.gameManager.PlayerDataPool.FashionDeadline[m_curChooseFashion] == GlobeVar.INVALID_ID)
            {
                // 永久         
                m_TimeLeftLabel.text = StrDictionary.GetClientDictionaryString("#{1667}");
                m_RenewButton.SetActive(false);
            }
            else if (nRemainTime > 0)
            {
                // 显示剩余时间
                if (nRemainTime >= 24 * 3600)
                {
                    m_TimeLeftLabel.text = StrDictionary.GetClientDictionaryString("#{1669}", Mathf.RoundToInt((float)nRemainTime / 24.0f / 3600.0f));
                }
                else
                {
                    m_TimeLeftLabel.text = StrDictionary.GetClientDictionaryString("#{1670}", Mathf.RoundToInt((float)nRemainTime / 60.0f));
                }
                m_RenewButton.SetActive(true);
            }
        }
    }

    void RenewFashion()
    {
        m_DefaultStatus.SetActive(false);
        m_BuyStatus.SetActive(true);
        //m_BottomInfo.SetActive(true);

        m_DeadlineTabs.ChangeTab(m_IsOpenTimeFashion ? "Timeleft1-Week" : "Timeleft3-Forever");
    }

    public void HandleUpdateAttr()
    {
        if (m_eCurTab == TAB_CONTENT.MY_FASHION)
        {
            FashionItemLogic[] itemArray = m_FashionItemGrid.GetComponentsInChildren<FashionItemLogic>();
            for (int i = 0; i < itemArray.Length; i++ )
            {
                if (itemArray[i].FashionID == Singleton<ObjManager>.Instance.MainPlayer.CurFashionID)
                {
                    itemArray[i].ShowChosenSprite();
                }
                else
                {
                    itemArray[i].HideChosenSprite();
                }
            }
        }
    }

    void InitTimeFashionObject()
    {
        for (int i = 0; i < m_TimeFashionObject.Length; i++ )
        {
            m_TimeFashionObject[i].SetActive(m_IsOpenTimeFashion);
        }

        m_DeadLineChoose = m_IsOpenTimeFashion ? "Timeleft1-Week" : "Timeleft3-Forever";
    }
}
