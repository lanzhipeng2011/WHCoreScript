/********************************************************************
	created:	2014/01/14
	created:	14:1:2014   13:38
	filename: 	SysShopController.cs
	author:		王迪
	
	purpose:	系统商店控制器
*********************************************************************/
using UnityEngine;
using System.Collections;
using GCGame;
using System.Collections.Generic;
using GCGame.Table;
using Games.Item;
using Games.LogicObj;
using Module.Log;
public class SysShopController : UIControllerBase<SysShopController> {

    
    

    public GameObject           shopPageRoot;                                       // 商品页父节点
    public RewardItem[]         recycleItems;                                       // 或沟槽图片
    public UILabel              LabelBuyTip;                                        // 双击购买提示
    public UILabel              LabelBuyBackTip;                                    // 回购提示
    public UILabel              LabelCoinNum;                                       // 金币数量
    public UILabel              LabelYuanbaoNum;                                    // 元宝数量
    public UILabel              LabelBindYuanbaoNum;                                // 绑定元宝数量
    private List<GameObject>    m_pageList          = new List<GameObject>();       // 商品页LIST
    private GameObject          m_curShowPage       = null;                         // 当前显示的商品页
    private int                 m_curPageNum        = 0;                            // 当前第几页
    private List<ulong>         m_recycleGUIDList   = new List<ulong>();            // 当前回购槽对应的商品GUID
    public UILabel              labelPage;                                          // 显示页数label
    private static int          m_curShopID         =2;                             // 当前商店ID，目前为1，如果增加表需要改动
    private const int           ITEMCOUNT_MIN       = 1;                            // 一次购买商品最小数量
    private const int           ITEMCOUNT_MAX       = 999;                           // 一次购买商品最大数量
    private Tab_SystemShop      m_curShopTable = null;
    private const int           PAGEITEMCOUNT_MAX = 8;                              // 每一页商品最大数量

    private GameObject m_PageItem;
    private GameObject m_ShopItem;

    // 买药新手指引
    private int m_NewPlayerGuide_Step = -1;
    public int NewPlayerGuide_Step
    {
        get { return m_NewPlayerGuide_Step; }
        set { m_NewPlayerGuide_Step = value; }
    }

    void OnEnable()
    {
        GUIData.delMoneyChanged += UpdateMoney;
        UpdateBuyBackItems();
    }

    void OnDisable()
    {
        GUIData.delMoneyChanged -= UpdateMoney;
    }
    void Awake()
    {
        SetInstance(this);

        LabelBuyTip.text = Utils.GetDicByID(1088);
        LabelBuyBackTip.text = Utils.GetDicByID(1296);
        for (int i = 0; i < recycleItems.Length; ++i)
        {
            recycleItems[i].delItemClick = OnBuyBackClick;
        }
    }

	void Start ()
    {
        UpdateMoney();
        UIManager.LoadItem(UIInfo.SysShopPage, OnLoadPageItem);
	}

    void OnLoadPageItem(GameObject pageItem, object param)
    {
        m_PageItem = pageItem;
        if (null == m_PageItem)
        {
            LogModule.ErrorLog("can not load pageobj in" + UIInfo.SysShopPage.path);
            return;
        }
        UIManager.LoadItem(UIInfo.SysShopPageItem, OnLoadShopItem);
    }

    void OnLoadShopItem(GameObject shopItem, object param)
    {
        m_ShopItem = shopItem;
        if (null == m_ShopItem)
        {
            LogModule.ErrorLog("can not load pageobj in" + UIInfo.SysShopPageItem.path);
            return;
        }

        m_pageList.Clear();

        GameObject curPage = null;
        GameObject curPageItem = null;

        // 根据表格配置，初始化ITEM
        m_curShopTable = TableManager.GetSystemShopByID(m_curShopID, 0);
        int curItemIndex = 0;       // 除去无效物品，实际物品ID
        for (int i = 0, count = m_curShopTable.getPidCount(); i < count; i++)
        {
            Tab_CommonItem curTabItem = TableManager.GetCommonItemByID(m_curShopTable.GetPidbyIndex(i), 0);
            if (null != curTabItem)
            {
                if (curItemIndex % PAGEITEMCOUNT_MAX == 0)
                {
                    curPage = Utils.BindObjToParent(m_PageItem, shopPageRoot);
                    m_pageList.Add(curPage);
                }

                curPageItem = Utils.BindObjToParent(m_ShopItem, curPage);
                curPageItem.name = i.ToString();
                curPage.GetComponent<SysShopPage>().AddItem(curPageItem, curItemIndex % PAGEITEMCOUNT_MAX, i,  m_curShopTable);
                curPage.SetActive(false);
                curItemIndex++;
            }
            else
            {
                LogModule.DebugLog("systemshop:can not find cur item in item table, item id:" + m_curShopTable.GetPidbyIndex(i));
            }
        }

        ShowPage(0);
    }

    public void ShowPage(int page)
    {
        if(page >= m_pageList.Count)
        {
            return;
        }

        if(null != m_curShowPage)
        {
            m_curShowPage.SetActive(false);
        }

        m_curShowPage = m_pageList[page];
        m_curShowPage.SetActive(true);

        m_curPageNum = page;
        labelPage.text = (m_curPageNum+1).ToString() + "/" + m_pageList.Count.ToString();

        Check_NewPlayerGuide();
    }

    // 购买物品
    public static void BuyItem(string strItemIndex, int count)
    {
        int curItemIndex;
        bool bCanGetID = int.TryParse(strItemIndex, out curItemIndex);
        if (!bCanGetID)
        {
            LogModule.ErrorLog("cur item id set error!");
            return;
        }

        Tab_SystemShop sysShopTable = TableManager.GetSystemShopByID(m_curShopID, 0);
        if(null == sysShopTable)
        {
            LogModule.ErrorLog("cur sysshop id isn't exist! : id " + m_curShopID.ToString());
            return;
        }

        int pid = sysShopTable.GetPricebyIndex(curItemIndex);
        if (pid < 0)
        {
            LogModule.ErrorLog("can not find cur item pid : itemID" + pid.ToString());
            return;
        }

        if (count < ITEMCOUNT_MIN || count > ITEMCOUNT_MAX)
        {
            LogModule.ErrorLog("item count is out range : count " + count.ToString());
            return;
        }

        int goodID = sysShopTable.GetPidbyIndex(curItemIndex);

        CG_SYSTEMSHOP_BUY buyPacket = (CG_SYSTEMSHOP_BUY)PacketDistributed.CreatePacket(MessageID.PACKET_CG_SYSTEMSHOP_BUY);
		buyPacket.SetBuyNum((uint)count);
        buyPacket.SetShopId(m_curShopID);
		buyPacket.SetMercIndex((uint)goodID);   
        buyPacket.SendPacket();
    }

    // 卖物品
    public static void SellItem(int packet, List<ulong> packetGUIDList)
    {
        CG_SYSTEMSHOP_SELL sellPacket = (CG_SYSTEMSHOP_SELL)PacketDistributed.CreatePacket(MessageID.PACKET_CG_SYSTEMSHOP_SELL);
       
        sellPacket.SetPackage(packet);
        for (int i = 0; i < packetGUIDList.Count; ++i)
        {
            sellPacket.AddItemGuid(packetGUIDList[i]);
        }
        sellPacket.SendPacket();
    }

    // 回购物品
    public static void BuyBack(ulong guid)
    {
        CG_SYSTEMSHOP_BUYBACK buyBackPacket = (CG_SYSTEMSHOP_BUYBACK)PacketDistributed.CreatePacket(MessageID.PACKET_CG_SYSTEMSHOP_BUYBACK);
        buyBackPacket.SetShopId(m_curShopID);
        buyBackPacket.SetItemGuid(guid);
        buyBackPacket.SendPacket();

    }

    // 更新回购槽
    public void UpdateBuyBackItems()
    {
        if (null == GameManager.gameManager)
        {
            return;
        }
        // 根据回购背包数据更新回购槽
        m_recycleGUIDList.Clear();
        GameItemContainer buyBackPack = GameManager.gameManager.PlayerDataPool.BuyBackPack;
        int index = 0;
        for (int i = 0; index < buyBackPack.GetItemCount(); i++)
        {
            GameItem curItem = buyBackPack.GetItem(i);
            if (null != curItem && index < recycleItems.Length)
            {
                //sprRecycleSlots[index].gameObject.SetActive(true);

                int itemID = curItem.DataID;
                Tab_CommonItem curTabItem = TableManager.GetCommonItemByID(itemID, 0);
                if (null != curTabItem)
                {
                    m_recycleGUIDList.Add(curItem.Guid);
                    recycleItems[index].SetData(itemID, curItem.StackCount, true);
                    index++;
                }
            }
        }

        for (int i = index; i < recycleItems.Length; i++)
        {
            recycleItems[i].SetData(-1, 0, true);
        }
    }

    // 购买当前选中物品
    public void BuyCurItem()
    {
        CloseCurItemTip();
        if (null != m_curShowPage)
        {
            SysShopPage curPage = m_curShowPage.GetComponent<SysShopPage>();
            SysShopPageItem curItem = curPage.GetCurHighLightItem();
            if (curItem == null)
            {
                return;
            }

            BuyItem(curItem.gameObject.name, 1);
        }
    }

    // 批量购买当前选中物品
    public void BuyBatchCurItem()
    {
       //CloseCurItemTip();
        if (null != m_curShowPage)
        {
            SysShopPage curPage = m_curShowPage.GetComponent<SysShopPage>();
            if (curPage.GetCurHighLightItem() == null)
            {
                return;
            }

            if (m_curShopTable != null && m_curShopTable.CanBuyMulty > 0)
            {
				NumChooseController.OpenWindow(ITEMCOUNT_MIN, ITEMCOUNT_MAX, "购买", OnNumChoose,10);
            }
            else
            {
                MessageBoxLogic.OpenOKBox(1004, 1000);
            }

        }
    }

    // 显示当前物品信息
    public void ShowCurItemTip()
    {
        SysShopPageItem curItem = GetSelectedItem();
        if (null == curItem)
        {
            return;
        }
       
        if (curItem.GetGameItem().IsEquipMent())
        {
            
            EquipTooltipsLogic.ShowEquipTooltip(curItem.GetGameItem(),
            m_curShopTable.CanBuyMulty > 0 ? EquipTooltipsLogic.ShowType.ShopBuyBatch : EquipTooltipsLogic.ShowType.ShopBuy);
        }
        else
        {
            ItemTooltipsLogic.ShowItemTooltip(curItem.GetGameItem(),
            m_curShopTable.CanBuyMulty > 0 ? ItemTooltipsLogic.ShowType.ShopBuyBatch : ItemTooltipsLogic.ShowType.ShopBuy);
        }
        
    }

    void CloseCurItemTip()
    {
        SysShopPageItem curItem = GetSelectedItem();

        if (null == curItem)
        {
            return;
        }

        if (curItem.GetGameItem().IsEquipMent())
        {
            UIManager.CloseUI(UIInfo.EquipTooltipsRoot);
        }
        else
        {
            UIManager.CloseUI(UIInfo.ItemTooltipsRoot);
        }
    }

    // 获取当前选中ITEM
    SysShopPageItem GetSelectedItem()
    {
        if (null == m_curShowPage)
        {
            return null;
        }
            SysShopPage curPage = m_curShowPage.GetComponent<SysShopPage>();
        if(null == curPage)
        {
            return null;
        }

        return curPage.GetCurHighLightItem();
    }

    void OnNextPageClick()
    {
        ShowPage(Mathf.Min(m_pageList.Count - 1, m_curPageNum + 1));
    }

    void OnPrevPageClick()
    {
        ShowPage(Mathf.Max(0, m_curPageNum - 1));
    }

    void OnCancelClick()
    {
        UIManager.CloseUI(UIInfo.SysShop);
    }

    void OnBuyBackClick(RewardItem item)
    {
        int curIndex = 0;
        if (!int.TryParse(item.name, out curIndex))
        {
            LogModule.ErrorLog("item is not a int value :" + item.name);
            return;
        }

        if (curIndex >= m_recycleGUIDList.Count)
        {
            return;
        }

        BuyBack(m_recycleGUIDList[curIndex]);
    }

	// buy
	void OnBuyClick()
	{
        BuyCurItem();
	}
    // 点击批量购买
    void OnBuyBatch()
    {
        BuyBatchCurItem();
    }

    // 批量购买确定
    void OnNumChoose(int curNum)
    {
        if (null == m_curShowPage)
        {
            LogModule.ErrorLog("curShowPage can not find");
            return;
        }

        SysShopPageItem curItem = m_curShowPage.GetComponent<SysShopPage>().GetCurHighLightItem();
        if (null == curItem)
        {
            LogModule.ErrorLog("cur select item is none");
            return;
        }

        BuyItem(curItem.gameObject.name, curNum);
    }

    void UpdateMoney()
    {
        LabelCoinNum.text = Utils.ConvertLargeNumToString(GameManager.gameManager.PlayerDataPool.Money.GetMoney_Coin());
        LabelYuanbaoNum.text = GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBao().ToString();
        LabelBindYuanbaoNum.text = GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBaoBind().ToString();
    }

    void Check_NewPlayerGuide()
    {
        if (m_curPageNum != 0)
        {
            return;
        }

        if (MenuBarLogic.Instance() == null)
        {
            return;
        }
        int nIndex = MenuBarLogic.Instance().NewPlayerGuideIndex;
        if (nIndex == 10)
        {
            NewPlayerGuide(1);
            MenuBarLogic.Instance().NewPlayerGuideIndex = -1;
        }
    }

    public void NewPlayerGuide(int nIndex)
    {
        m_NewPlayerGuide_Step = nIndex;
        switch (nIndex)
        {
            case 1:
                {
                    Transform gObjTrans = m_curShowPage.transform.FindChild("6");
                    if (gObjTrans)
                    {
                        NewPlayerGuidLogic.OpenWindow(gObjTrans.gameObject, 325, 100, "", "right", 2, true, true);
                    }
                }
                break;
        }
    }
}
