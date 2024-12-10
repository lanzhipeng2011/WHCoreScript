using UnityEngine;
using System.Collections;
using Module.Log;
using System.Collections.Generic;
using GCGame.Table;
using GCGame;
using Games.FakeObject;
using Games.GlobeDefine;
using Games.Item;

public class YuanBaoShopLogic : UIControllerBase<YuanBaoShopLogic>{

    public enum TAB_INDEX
    {
        TAB_INVALID = -1,
        TAB_STREN = 0,
        TAB_MOUNT,
        TAB_DECORATE,
        TAB_OTHER,
        TAB_VIP,
        TAB_BLACKMARKET = 999,
    }

    public enum BUY_TYPE
    {
        TYPE_BIND,
        TYPE_UNBIND,
    }

    public TabController m_TabController;
    public GameObject m_GoodsGrid;
    public GameObject m_Deadline;
    public UIPopupList m_DeadlinePopupList;
    public GameObject m_YBShopNumChoose;
    public TabController m_BuyTypeController;
    public UILabel m_UnBindYBNumLabel;
    public UILabel m_BindYBNumLabel;
    public GameObject m_BindBuy;
    public GameObject m_UnBindBuy;
    public UIGrid m_TabGrid;
    public GameObject m_BlackMarketTab;
    public ModelDragLogic m_ModelDrag;
    public GameObject m_VIPLabel;
    public GameObject m_Pages;
    public UILabel m_PageLabel;
    public YuanBaoShopItemLogic[] m_GoodsArray;

    //private static Dictionary<string, List<Tab_YuanBaoShop>> m_YuanBaoShopTable = null;
    private BUY_TYPE m_eCurBuyType = BUY_TYPE.TYPE_BIND;
    
    private FakeObject m_FitOnFakeObj;
    private int m_FakeObjID = GlobeVar.INVALID_ID;
    private GameObject m_FitOnGameObject;
    private YuanBaoShopItemLogic.DEADLINE_PRICE m_eDeadline = YuanBaoShopItemLogic.DEADLINE_PRICE.PRICE_FOREVER;
    private const int ItemCount_PerPage = 6;
    private TAB_INDEX m_CurTabIndex = TAB_INDEX.TAB_INVALID;
    
    public struct PageInfo
    {
        public int CurPage;
        public int PageCount;
        public int PageStartID;
        public int PageEndID;

        public void Clear()
        {
            CurPage = GlobeVar.INVALID_ID;
            PageCount = GlobeVar.INVALID_ID;
            PageStartID = GlobeVar.INVALID_ID;
            PageEndID = GlobeVar.INVALID_ID;
        }
    }
    private PageInfo m_TabPageInfo = new PageInfo();

    public struct FitOnVisual
    {
        public int FashionGoodsID;
        public int ArmorGoodsID;
        public int WeaponGoodsID;
        public int FellowGoodsID;
        public int MountGoodsID;

        public int FashionID;
        public int ArmorID;
        public int WeaponID;
        public int FellowID;
        public int MountID;

        public YuanBaoShopItemLogic.DEADLINE_PRICE FashionDeadline;

        public void Clear()
        {
            FashionGoodsID = GlobeVar.INVALID_ID;
            ArmorGoodsID = GlobeVar.INVALID_ID;
            WeaponGoodsID = GlobeVar.INVALID_ID;
            FellowGoodsID = GlobeVar.INVALID_ID;
            MountGoodsID = GlobeVar.INVALID_ID;

            FashionID = GlobeVar.INVALID_ID;
            ArmorID = GlobeVar.INVALID_ID;
            WeaponID = GlobeVar.INVALID_ID;
            FellowID = GlobeVar.INVALID_ID;
            MountID = GlobeVar.INVALID_ID;

            FashionDeadline = YuanBaoShopItemLogic.DEADLINE_PRICE.PRICE_FOREVER;
        }
    }
    private FitOnVisual m_FitOnVisual = new FitOnVisual();

    void Awake()
    {
        SetInstance(this);
    }

	// Use this for initialization
	void Start () {
        
	}

    void OnEnable()
    {
        SetInstance(this);

        m_BuyTypeController.ChangeTab("2Bind");
        m_TabController.delTabChanged = TabOnClick;
        //m_YuanBaoShopTable = TableManager.GetYuanBaoShop();
        m_Deadline.SetActive(false);
        m_YBShopNumChoose.SetActive(false);
        m_BuyTypeController.delTabChanged = BuyTypeOnClick;
        UpdateYuanBaoInfo(false);

        InitBuyTypeTabButtonChoose();

        m_TabController.InitData();
        bool bShowMountTab = GameManager.gameManager.PlayerDataPool.IsServerFlagOpen(SERVER_FLAGS_ENUM.FLAG_MOUNTTAB);
        if (m_TabController.GetTabButton("Tab3") != null &&
            m_TabController.GetTabButton("Tab3").gameObject != null)
        {
            m_TabController.GetTabButton("Tab3").gameObject.SetActive(bShowMountTab);
        }
        bool bShowVipTab = GameManager.gameManager.PlayerDataPool.IsServerFlagOpen(SERVER_FLAGS_ENUM.FLAG_VIP);
        if (m_TabController.GetTabButton("Tab5") != null &&
            m_TabController.GetTabButton("Tab5").gameObject != null)
        {
            m_TabController.GetTabButton("Tab5").gameObject.SetActive(bShowVipTab);
        }
        m_TabGrid.Reposition();

        m_TabController.ChangeTab("Tab1");
		
        m_FitOnVisual.Clear();
    }

    void OnDisable()
    {
        m_eCurBuyType = BUY_TYPE.TYPE_BIND;
        m_FakeObjID = GlobeVar.INVALID_ID;
        m_FitOnGameObject = null;
        m_eDeadline = YuanBaoShopItemLogic.DEADLINE_PRICE.PRICE_FOREVER;
        m_CurTabIndex = TAB_INDEX.TAB_INVALID;
		
        SetInstance(null);
        GameManager.gameManager.ActiveScene.HideFakeObj();
    }

    static public void OnYuanBaoShopLoad(bool bSuccess, object param)
    {
        if (bSuccess)
        {
            bool isShowBlackMarket = (bool)param;
            if (YuanBaoShopLogic.Instance() != null)
            {
                YuanBaoShopLogic.Instance().m_BlackMarketTab.SetActive(isShowBlackMarket);
                YuanBaoShopLogic.Instance().m_TabGrid.Reposition();
            }            
        }
    }

    void InitBuyTypeTabButtonChoose()
    {
        int nPlayerYuanBaoBind = GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBaoBind();
        int nPlayerYuanBao = GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBao();

        // 绑定默认勾选看玩家是否有绑定元宝 非绑定只在玩家没有绑定元宝且有非绑定元宝时默认勾选  
        if (nPlayerYuanBaoBind == 0 && nPlayerYuanBao != 0)
        {
            m_BuyTypeController.ChangeTab("1UnBind");
        }
    }

    public void UpdateYuanBaoInfo(bool isSync = true)
    {
        int nPlayerYuanBaoBind = GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBaoBind();
        int nPlayerYuanBao = GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBao();

        m_BindBuy.GetComponent<BoxCollider>().enabled = (nPlayerYuanBaoBind != 0);
        m_UnBindBuy.GetComponent<BoxCollider>().enabled = (nPlayerYuanBao != 0);

        // 同步金钱时才会执行 当元宝减为0时 已勾选的选项清空
        if (isSync)
        {
            if (nPlayerYuanBaoBind == 0)
            {
                if (nPlayerYuanBao != 0)
                {
                    m_BuyTypeController.ChangeTab("1UnBind");
                }
            }
            if (nPlayerYuanBao == 0)
            {
                m_BuyTypeController.ChangeTab("2Bind");
            }
        }

        m_BindYBNumLabel.text = nPlayerYuanBaoBind.ToString();
        m_UnBindYBNumLabel.text = nPlayerYuanBao.ToString();        
    }

    //强化 伙伴 装饰 其他 VIP
    void TabOnClick(TabButton value)
    {
        if (value.name.Contains("Tab1"))
        {
            m_Deadline.SetActive(false);
            m_eDeadline = YuanBaoShopItemLogic.DEADLINE_PRICE.PRICE_FOREVER;
            UpdateGoodsInfo(TAB_INDEX.TAB_STREN);
        }
        else if (value.name.Contains("Tab2"))
        {
            m_Deadline.SetActive(false);
            m_eDeadline = YuanBaoShopItemLogic.DEADLINE_PRICE.PRICE_FOREVER;
            UpdateGoodsInfo(TAB_INDEX.TAB_MOUNT);
        }
        else if (value.name.Contains("Tab3"))
        {
            m_Deadline.SetActive(false);
            m_eDeadline = YuanBaoShopItemLogic.DEADLINE_PRICE.PRICE_FOREVER;
            UpdateGoodsInfo(TAB_INDEX.TAB_DECORATE);
        }
        else if (value.name.Contains("Tab4"))
        {
            m_Deadline.SetActive(false);
            m_eDeadline = YuanBaoShopItemLogic.DEADLINE_PRICE.PRICE_FOREVER;
            UpdateGoodsInfo(TAB_INDEX.TAB_OTHER);
        }
        else if (value.name.Contains("Tab5"))
        {
            m_Deadline.SetActive(false);
            m_eDeadline = YuanBaoShopItemLogic.DEADLINE_PRICE.PRICE_FOREVER;
            UpdateGoodsInfo(TAB_INDEX.TAB_VIP);
        }
        else if (value.name.Contains("Tab999"))
        {
            m_Deadline.SetActive(false);
            m_eDeadline = YuanBaoShopItemLogic.DEADLINE_PRICE.PRICE_FOREVER;
            UpdateGoodsInfo(TAB_INDEX.TAB_BLACKMARKET);
        }
    }

    void UpdateGoodsInfo(TAB_INDEX eTabIndex)
    {
        m_TabPageInfo.Clear();
        m_CurTabIndex = eTabIndex;

        if (eTabIndex == TAB_INDEX.TAB_VIP)
        {
            if (Singleton<ObjManager>.Instance.MainPlayer != null)
            {
                int nPlayerVIPLevel = VipData.GetVipLv();
                if (nPlayerVIPLevel < 9)
                {
                    m_GoodsGrid.SetActive(false);
                    m_Pages.SetActive(false);
                    m_VIPLabel.SetActive(true);
                    return;
                }
                else
                {
                    m_GoodsGrid.SetActive(true);
                    m_Pages.SetActive(true);
                    m_VIPLabel.SetActive(false);                    
                }
            }
        }
        else
        {
            m_GoodsGrid.SetActive(true);
            m_Pages.SetActive(true);
            m_VIPLabel.SetActive(false);
        }

        int nTabItemCount = 0;
        int nPageItemIndex = 0;
        for (int i = 0; i < TableManager.GetYuanBaoShop().Count; i++)
        {
            Tab_YuanBaoShop tabYuanBaoShop = TableManager.GetYuanBaoShopByID(i, 0);
            if (tabYuanBaoShop == null)
            {
                continue;
            }

            if (IsTabNewGoods(tabYuanBaoShop))
            {
                nTabItemCount += 1;
                if (HandleNewGoods(tabYuanBaoShop, nPageItemIndex, true))
                {
                    nPageItemIndex += 1;
                }
            }
        }

        UpdateGoodsActive(nPageItemIndex);

        m_TabPageInfo.CurPage = 1;
        m_TabPageInfo.PageCount = Mathf.CeilToInt((float)nTabItemCount / (float)ItemCount_PerPage);
        m_PageLabel.text = m_TabPageInfo.CurPage.ToString() + "/" + m_TabPageInfo.PageCount.ToString();
    }

    void ShowNextPage()
    {
        if (m_TabPageInfo.CurPage >= m_TabPageInfo.PageCount)
        {
            return;
        }

        int end = m_TabPageInfo.PageEndID;
        m_TabPageInfo.PageStartID = GlobeVar.INVALID_ID;
        m_TabPageInfo.PageEndID = GlobeVar.INVALID_ID;

        int nPageItemIndex = 0;
        for (int i = 0; i < TableManager.GetYuanBaoShop().Count; i++)
        {
            Tab_YuanBaoShop tabYuanBaoShop = TableManager.GetYuanBaoShopByID(i, 0);
            if (tabYuanBaoShop == null)
            {
                continue;
            }

            if (tabYuanBaoShop.Id <= end)
            {
                continue;
            }

            if (IsTabNewGoods(tabYuanBaoShop))
            {
                if (HandleNewGoods(tabYuanBaoShop, nPageItemIndex, true))
                {
                    nPageItemIndex += 1;
                }
                if (nPageItemIndex >= ItemCount_PerPage)
                {
                    break;
                }
            }
        }

        UpdateGoodsActive(nPageItemIndex);

        m_TabPageInfo.CurPage += 1;
        m_PageLabel.text = m_TabPageInfo.CurPage.ToString() + "/" + m_TabPageInfo.PageCount.ToString();
    }

    void ShowPrePage()
    {
        if (m_TabPageInfo.CurPage <= 1)
        {
            return;
        }

        int start = m_TabPageInfo.PageStartID;
        m_TabPageInfo.PageStartID = GlobeVar.INVALID_ID;
        m_TabPageInfo.PageEndID = GlobeVar.INVALID_ID;

        int nPageItemIndex = ItemCount_PerPage - 1;
        for (int i = start - 1; i >= 0; i-- )
        {
            Tab_YuanBaoShop tabYuanBaoShop = TableManager.GetYuanBaoShopByID(i, 0);
            if (tabYuanBaoShop == null)
            {
                continue;
            }

            if (IsTabNewGoods(tabYuanBaoShop))
            {
                if (HandleNewGoods(tabYuanBaoShop, nPageItemIndex, false))
                {
                    nPageItemIndex -= 1;
                }
                if (nPageItemIndex < 0)
                {
                    break;
                }
            }
        }

        // 上一页必然所有item都显示
        UpdateGoodsActive(ItemCount_PerPage);

        m_TabPageInfo.CurPage -= 1;
        m_PageLabel.text = m_TabPageInfo.CurPage.ToString() + "/" + m_TabPageInfo.PageCount.ToString();
    }

    bool IsTabNewGoods(Tab_YuanBaoShop tabYuanBaoShop)
    {
        if (tabYuanBaoShop.TabIndex != (int)m_CurTabIndex)
        {
            return false;
        }

        if (m_CurTabIndex == TAB_INDEX.TAB_VIP)
        {
            if (tabYuanBaoShop.ItemType != (int)YuanBaoShopItemLogic.ITEM_TYPE.TYPE_ITEM)
            {
                return false;
            }

            Tab_VIPShop tabVIPShop = TableManager.GetVIPShopByID(tabYuanBaoShop.ItemID, 0);
            if (tabVIPShop == null)
            {
                return false;
            }

            int nPlayerVIPLevel = VipData.GetVipLv();

            if (nPlayerVIPLevel < tabVIPShop.ShowLevelReq)
            {
                return false;
            }
        }

        return true;
    }

    bool HandleNewGoods(Tab_YuanBaoShop tabYuanBaoShop, int nPageItemIndex, bool bNextPage)
    {
        if (nPageItemIndex >= ItemCount_PerPage || nPageItemIndex < 0)
        {
            return false;
        }

        m_GoodsArray[nPageItemIndex].Init(tabYuanBaoShop);
        if (bNextPage)
        {           
            if (nPageItemIndex == 0)
            {
                m_TabPageInfo.PageStartID = tabYuanBaoShop.Id;
            }
            m_TabPageInfo.PageEndID = tabYuanBaoShop.Id;
        }
        else
        {
            if (nPageItemIndex == ItemCount_PerPage - 1)
            {
                m_TabPageInfo.PageEndID = tabYuanBaoShop.Id;
            }
            m_TabPageInfo.PageStartID = tabYuanBaoShop.Id;
        }

        return true;
    }

    void UpdateGoodsActive(int nPageItemIndex)
    {
        for (int i = 0; i < ItemCount_PerPage; i++)
        {
            m_GoodsArray[i].gameObject.SetActive(false);
            if (i < nPageItemIndex)
            {
                m_GoodsArray[i].gameObject.SetActive(true);
            }
        }
    }

    void CloseWindow()
    {
        UIManager.CloseUI(UIInfo.YuanBaoShop);
    }

    public void ChangeDeadline()
    {
        //if (m_DeadlinePopupList.value == "7天")
        if (m_DeadlinePopupList.value == StrDictionary.GetClientDictionaryString("#{2888}"))
        {
            ChangeGoodsPrice(YuanBaoShopItemLogic.DEADLINE_PRICE.PRICE_WEEK);
        }
        //else if (m_DeadlinePopupList.value == "30天")
        else if (m_DeadlinePopupList.value == StrDictionary.GetClientDictionaryString("#{2889}"))
        {
            ChangeGoodsPrice(YuanBaoShopItemLogic.DEADLINE_PRICE.PRICE_MONTH);
        }
        //else if (m_DeadlinePopupList.value == "永久")
        else if (m_DeadlinePopupList.value == StrDictionary.GetClientDictionaryString("#{2890}"))
        {
            ChangeGoodsPrice(YuanBaoShopItemLogic.DEADLINE_PRICE.PRICE_FOREVER);
        }
    }

    void ChangeGoodsPrice(YuanBaoShopItemLogic.DEADLINE_PRICE eDeadline)
    {
        m_eDeadline = eDeadline;
        foreach(YuanBaoShopItemLogic item in m_GoodsGrid.GetComponentsInChildren<YuanBaoShopItemLogic>())
        {
            item.ChangePrice(eDeadline);
        }
    }

    public void ShowYBShopNumChoose(int nGoodsId, ItemSlotLogic.SLOT_TYPE eSlotType, int nItemID, int nGoodsNum, int nPrice,
        CG_BUY_YUANBAOGOODS.DEADLINE_TYPE eDeadlineType, string strItemName)
    {
        m_YBShopNumChoose.SetActive(true);
        m_YBShopNumChoose.GetComponent<YBShopNumChooseLogic>().InitInfo(nGoodsId, eSlotType, nItemID, nGoodsNum, nPrice,
            m_eCurBuyType == BUY_TYPE.TYPE_BIND ? true : false, eDeadlineType, strItemName);
    }

    void BuyTypeOnClick(TabButton value)
    {
        if (value.name == "1UnBind")
        {
            m_eCurBuyType = BUY_TYPE.TYPE_UNBIND;
        }
        else if (value.name == "2Bind")
        {
            m_eCurBuyType = BUY_TYPE.TYPE_BIND;
        }        
    }

    void ResetView()
    {
        m_FitOnFakeObj.initFakeObject(m_FakeObjID, GameManager.gameManager.ActiveScene.FakeObjTrans, out m_FitOnGameObject);
        if (null != m_FitOnFakeObj.ObjAnim)
            m_ModelDrag.ModelTrans = m_FitOnFakeObj.ObjAnim.transform;

        m_FitOnVisual.Clear();
    }

    void BuyView()
    {
        MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{2137}"), "", BuyViewOK, BuyViewCancel);
    }

    void BuyViewOK()
    {
        int[] viewGoods = { m_FitOnVisual.FashionGoodsID, m_FitOnVisual.ArmorGoodsID, m_FitOnVisual.WeaponGoodsID, m_FitOnVisual.FellowGoodsID, m_FitOnVisual.MountGoodsID };
        for (int i = 0; i < 5; i++)
        {
            if (viewGoods[i] != GlobeVar.INVALID_ID)
            {
                CG_BUY_YUANBAOGOODS packet = (CG_BUY_YUANBAOGOODS)PacketDistributed.CreatePacket(MessageID.PACKET_CG_BUY_YUANBAOGOODS);
                packet.GoodID = viewGoods[i];
                packet.BuyNum = 1;
				packet.IsUseBind = (uint)((m_eCurBuyType == BUY_TYPE.TYPE_BIND) ? 1 : 0);
                if (i == 0)
                {
                    // 时装按照选择的期限
					packet.Deadline = (uint)m_FitOnVisual.FashionDeadline;
                }
                else
                {
                    // 其他的按永久
                    packet.Deadline = (int)YuanBaoShopItemLogic.DEADLINE_PRICE.PRICE_FOREVER;
                }
                packet.SendPacket();
            }
        }
    }

    void BuyViewCancel(){}

    void OnClickChongZhi()
    {
        RechargeData.PayUI();
    }
}