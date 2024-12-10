using UnityEngine;
using System.Collections;
using Games.Item;
using GCGame.Table;

public class LivingSkillStuffLogic : MonoBehaviour {

    public ItemSlotLogic m_ItemSlot;
    public UILabel m_NameLabel;
    public UILabel m_CountLabel;
    public GameObject m_EquipFlag;

    private int m_ItemID;
    public int ItemID
    {
        get { return m_ItemID; }
    }

    private int m_RequireCount;

	// Use this for initialization
	void Start () {

	}

    public void InitInfo(int nItemID, int nRequireCount)
    {
        Tab_CommonItem tabCommonItem = TableManager.GetCommonItemByID(nItemID, 0);
        if (tabCommonItem == null)
        {
            return;
        }

        m_ItemID = nItemID;
        m_RequireCount = nRequireCount;

        m_NameLabel.text = tabCommonItem.Name;
        m_ItemSlot.InitInfo_Stuff(m_ItemID, ShowStuffTooltips);

        UpdateItemCountInfo();
    }

    void UpdateItemCountInfo()
    {
        int BackPackItemCount = GameManager.gameManager.PlayerDataPool.BackPack.GetItemCountByDataId(m_ItemID);
        string strColor = "";
        if (BackPackItemCount >= m_RequireCount)
        {
            strColor = "[32A100]";
        }
        else
        {
            strColor = "[FF2222]";
        }
        m_CountLabel.text = strColor + BackPackItemCount.ToString() + "[FFFFFF]/" + m_RequireCount.ToString();
    }

    void ShowStuffTooltips(int nItemID, ItemSlotLogic.SLOT_TYPE eItemType, string strSlotName)
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

    void CollectBtnClick()
    {
        if (LivingSkillLogic.Instance() != null)
        {
            LivingSkillLogic.Instance().ShowStuffCollectInfo(m_ItemID);
        }
    }

    void BuyBtnClick()
    {
        UIManager.ShowUI(UIInfo.ConsignSaleRoot, BuyItemOpenConsignSale);
    }

    void BuyItemOpenConsignSale(bool bSuccess, object param)
    {
        if (bSuccess)
        {
            Tab_CommonItem tabCommonItem = TableManager.GetCommonItemByID(m_ItemID, 0);
            if (tabCommonItem == null)
            {
                return;
            }

            if (ConsignSaleLogic.Instance() != null)
            {
                ConsignSaleLogic.Instance().SearchForAskBuy(tabCommonItem.Name);
            }
        }
    }

    public void HandleUpdateItem()
    {
        UpdateItemCountInfo();
    }
}
