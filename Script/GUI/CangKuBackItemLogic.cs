using UnityEngine;
using System.Collections;
using Games.Item;

public class CangKuBackItemLogic : MonoBehaviour {

    public UISprite m_ArrowUpSprite;
    public UISprite m_ArrowDownSprite;
    public ItemSlotLogic m_ItemSlot;

    private GameItem m_Item = null;

	// Use this for initialization
	void Start () {
	
	}

    public void UpdateItem(GameItem item)
    {
        m_ArrowUpSprite.gameObject.SetActive(false);
        m_ArrowDownSprite.gameObject.SetActive(false);
        m_ItemSlot.InitInfo_Item(item.DataID, null, item.StackCount.ToString());
        m_Item = item;
    }

    public void UpdateEmpty()
    {
        m_ArrowUpSprite.gameObject.SetActive(false);
        m_ArrowDownSprite.gameObject.SetActive(false);
        m_ItemSlot.ClearInfo();
        m_Item = null;
    }

    void OnItemClick()
    {
        if (m_Item == null)
        {
            return;
        }
        if (m_Item.IsEquipMent())
        {
            EquipTooltipsLogic.ShowEquipTooltip(m_Item, EquipTooltipsLogic.ShowType.CangKuBackPack);
        }
        else
        {
            ItemTooltipsLogic.ShowItemTooltip(m_Item, ItemTooltipsLogic.ShowType.CangKuBackPack);
        }
    }
}
