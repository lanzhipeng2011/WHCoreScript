using UnityEngine;
using System.Collections;
using GCGame.Table;
using Games.Item;

public class YuanBaoShopItemLogic : MonoBehaviour {

    public enum ITEM_TYPE
    {
        TYPE_INVALID = -1,
        TYPE_ITEM = 0,
        TYPE_FASHION,
        TYPE_FELLOW,
        TYPE_MOUNT,
    }

    public enum DEADLINE_PRICE
    {
        PRICE_WEEK,
        PRICE_MONTH,
        PRICE_FOREVER,
    }

    public ItemSlotLogic m_ItemSlot;
    public UILabel m_NumContent;
    public UILabel m_PriceContent;
    public UILabel m_NameLabel;

    private int m_GoodsId;   // 商品ID 表格第一列
    private ITEM_TYPE m_eItemType;
    private int m_ItemID;    // 物品ID 表格第五列
    private int m_Num;
    private string m_Name = "";

    private int m_PriceWeek;
    private int m_SaleWeek;
    private int m_PriceMonth;
    private int m_SaleMonth;
    private int m_PriceForever;
    private int m_SaleForever;

    private int m_CurPrice;
    private CG_BUY_YUANBAOGOODS.DEADLINE_TYPE m_eDeadlineType;

	// Use this for initialization
	void Start () {
	
	}

    public void Init(Tab_YuanBaoShop tabYuanBaoShop)
    {
        m_GoodsId = tabYuanBaoShop.Id;
        m_eItemType = (ITEM_TYPE)tabYuanBaoShop.ItemType;
        m_ItemID = tabYuanBaoShop.ItemID;
        m_Num = tabYuanBaoShop.Num;
        m_PriceWeek = tabYuanBaoShop.PriceWeek;
        m_SaleWeek = tabYuanBaoShop.SaleWeek;
        m_PriceMonth = tabYuanBaoShop.PriceMonth;
        m_SaleMonth = tabYuanBaoShop.SaleMonth;
        m_PriceForever = tabYuanBaoShop.PriceForever;
        m_SaleForever = tabYuanBaoShop.SaleForever;

        m_ItemSlot.InitInfo(ConvertGoodsTypeToSlotType(m_eItemType), m_ItemID, GoodsOnClick);
        m_NumContent.text = m_Num.ToString();
        SetNameLabel();
        SetPriceLabel();
    }

    void GoodsOnClick(int nItemID, ItemSlotLogic.SLOT_TYPE eSlotType, string strSlotName)
    {
        if (eSlotType == ItemSlotLogic.SLOT_TYPE.TYPE_ITEM)
        {
            GameItem item = new GameItem();
            item.DataID = nItemID;
            if (item.IsEquipMent())
            {
                EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Info);
            }
            else
            {
                ItemTooltipsLogic.ShowItemTooltip(item, ItemTooltipsLogic.ShowType.Info);
            }
        }
    }

    ItemSlotLogic.SLOT_TYPE ConvertGoodsTypeToSlotType(ITEM_TYPE eGoodsType)
    {
        switch (eGoodsType)
        {
            case ITEM_TYPE.TYPE_ITEM:
                return ItemSlotLogic.SLOT_TYPE.TYPE_ITEM;
            case ITEM_TYPE.TYPE_FASHION:
                return ItemSlotLogic.SLOT_TYPE.TYPE_FASHION;
            case ITEM_TYPE.TYPE_FELLOW:
                return ItemSlotLogic.SLOT_TYPE.TYPE_FELLOW;
            case ITEM_TYPE.TYPE_MOUNT:
                return ItemSlotLogic.SLOT_TYPE.TYPE_MOUNT;
            default:
                return ItemSlotLogic.SLOT_TYPE.TYPE_INVALID;
        }
    }

    void BuyGoods()
    {
        if (Singleton<ObjManager>.Instance.MainPlayer == null)
        {
            return;
        }

        Tab_VIPShop tabVIPShop = TableManager.GetVIPShopByID(m_ItemID, 0);
        if (tabVIPShop != null)
        {
            int nPlayerVIPLevel = VipData.GetVipLv();
            if (nPlayerVIPLevel < tabVIPShop.BuyLevelReq)
            {
                Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2512}");
                return;
            }
        }
        
        if (YuanBaoShopLogic.Instance() != null)
        {
            YuanBaoShopLogic.Instance().ShowYBShopNumChoose(m_GoodsId, ConvertGoodsTypeToSlotType(m_eItemType), m_ItemID, m_Num, m_CurPrice, m_eDeadlineType, m_Name);
        }
    }

    public void ChangePrice(DEADLINE_PRICE eDeadline)
    {
        if (eDeadline == DEADLINE_PRICE.PRICE_WEEK)
        {
            m_eDeadlineType = CG_BUY_YUANBAOGOODS.DEADLINE_TYPE.TYPE_WEEK;
            if (m_SaleWeek < 0)
            {
                m_CurPrice = m_PriceWeek;
            }
            else
            {
                m_CurPrice = m_SaleWeek;
            }
            if (m_CurPrice < 0)
            {
                ChangePrice(DEADLINE_PRICE.PRICE_FOREVER);
                return;
            }
            m_PriceContent.text = m_CurPrice.ToString();
        }
        else if (eDeadline == DEADLINE_PRICE.PRICE_MONTH)
        {
            m_eDeadlineType = CG_BUY_YUANBAOGOODS.DEADLINE_TYPE.TYPE_MONTH;
            if (m_SaleMonth < 0)
            {
                m_CurPrice = m_PriceMonth;
            }
            else
            {
                m_CurPrice = m_SaleMonth;
            }
            if (m_CurPrice < 0)
            {
                ChangePrice(DEADLINE_PRICE.PRICE_FOREVER);
                return;
            }
            m_PriceContent.text = m_CurPrice.ToString();
        }
        else if (eDeadline == DEADLINE_PRICE.PRICE_FOREVER)
        {
            m_eDeadlineType = CG_BUY_YUANBAOGOODS.DEADLINE_TYPE.TYPE_FOREVER;
            if (m_SaleForever < 0)
            {
                m_CurPrice = m_PriceForever;
            }
            else
            {
                m_CurPrice = m_SaleForever;
            }
            m_PriceContent.text = m_CurPrice.ToString();
        }
    }

    void SetNameLabel()
    {
        if (m_eItemType == ITEM_TYPE.TYPE_ITEM)
        {
            Tab_CommonItem tabCommonItem = TableManager.GetCommonItemByID(m_ItemID, 0);
            if (tabCommonItem != null)
            {
                m_Name = tabCommonItem.Name;
                m_NameLabel.text = m_Name;
                int nExistTime = tabCommonItem.ExistTime;
                if (nExistTime > 0)
                {
                    m_NameLabel.text += "(" + (float)nExistTime / 60f / 24f + "天)";
                }
            }            
        }
        else if (m_eItemType == ITEM_TYPE.TYPE_FASHION)
        {
            Tab_FashionData tabFashionData = TableManager.GetFashionDataByID(m_ItemID, 0);
            if (tabFashionData != null)
            {
                m_Name = tabFashionData.Name;
                m_NameLabel.text = m_Name;
            }            
        }
        else if (m_eItemType == ITEM_TYPE.TYPE_FELLOW)
        {
            Tab_FellowAttr tabFellowAttr = TableManager.GetFellowAttrByID(m_ItemID, 0);
            if (tabFellowAttr != null)
            {
                m_Name = tabFellowAttr.Name;
                m_NameLabel.text = m_Name;
            }            
        }
        else if (m_eItemType == ITEM_TYPE.TYPE_MOUNT)
        {
            Tab_MountBase tabMountBase = TableManager.GetMountBaseByID(m_ItemID, 0);
            if (tabMountBase != null)
            {
                m_Name = tabMountBase.Name;
                m_NameLabel.text = m_Name;
            }
        }        
    }

    void SetPriceLabel()
    {
        if (m_eItemType == ITEM_TYPE.TYPE_FASHION)
        {
            ChangePrice(DEADLINE_PRICE.PRICE_WEEK);
        }
        else
        {
            ChangePrice(DEADLINE_PRICE.PRICE_FOREVER);
        }
    }
}
