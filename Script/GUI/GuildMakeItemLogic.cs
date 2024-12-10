using UnityEngine;
using System.Collections;
using GCGame.Table;
using Games.GlobeDefine;
using Games.Item;

public class GuildMakeItemLogic : MonoBehaviour {

    public ItemSlotLogic m_ItemSlot;        //物品逻辑

    public UILabel m_ItemNameLable;
    public UILabel m_ItemLvlLable;

    public UISprite m_HoverSprite;

    private int m_ID;

	// Use this for initialization
	void Start () {
	
	}

    public void Init(Tab_CommonItem tab, int ID)
    {
        if (null == tab)
        {
            return;
        }

        m_ID = ID;
        m_ItemNameLable.text = tab.Name;
        m_ItemLvlLable.text = tab.MinLevelRequire.ToString() + "级绿色腰带";

        m_ItemSlot.InitInfo(ItemSlotLogic.SLOT_TYPE.TYPE_ITEM, tab.Id, GoodsOnClick);

    }


    void GoodsOnClick(int nItemID, ItemSlotLogic.SLOT_TYPE eSlotType, string strSlotName)
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

    void OnChooseItem()
    {
        GuildMakeLogic.Instance().UpdateMakeInfo(m_ID);
    }

    public void IsShowPressBg(bool isShow)
    {
        m_HoverSprite.gameObject.SetActive(isShow);
    }

}
