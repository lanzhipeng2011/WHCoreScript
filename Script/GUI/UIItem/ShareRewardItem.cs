using UnityEngine;
using System.Collections;
using GCGame.Table;
using Games.GlobeDefine;
using Games.Item;
public class ShareRewardItem : MonoBehaviour {

    private int m_nItemDataId;
    public UISprite m_ItemIcon;
  
    public void ClearInfo()
    {
        m_nItemDataId = GlobeVar.INVALID_ID;
        m_ItemIcon.gameObject.SetActive(false);
    }

    void SlotOnClick()
    {
        ShowItemTips();
    }

    public void InitItem(int id)
    {
        m_nItemDataId = id;
        Tab_CommonItem tabCommonItem = TableManager.GetCommonItemByID(m_nItemDataId, 0);
        if (tabCommonItem == null)
        {
            return;
        }
        m_ItemIcon.spriteName = tabCommonItem.Icon;
        m_ItemIcon.gameObject.SetActive(true);
    }

    public void OnItemClick()
    {
        ShowItemTips();
    }

    void ShowItemTips()
    {
        GameItem item = new GameItem();
        item.DataID = m_nItemDataId;
        if (item.IsValid())
        {
            ItemTooltipsLogic.ShowItemTooltip(item, ItemTooltipsLogic.ShowType.Info);
        }
    }
}
