using UnityEngine;
using System.Collections;
using GCGame.Table;
using Games.GlobeDefine;
using Games.Item;

public class SNSItemLogic : MonoBehaviour {

	private int m_nItemDataId;
    public UISprite m_ItemIcon;
   // public UISprite m_QualitySprite;
   // public UILabel m_NumLabel;

    private int m_ItemID;

	// Use this for initialization
	void Start () {
		//ClearInfo ();
	}
	
    public void ClearInfo()
    {
        m_ItemID = GlobeVar.INVALID_ID;
        m_ItemIcon.gameObject.SetActive(false);
       // m_QualitySprite.gameObject.SetActive(false);
       // m_NumLabel.gameObject.SetActive(false);
    }

    void SlotOnClick( )
    {
		ShowItemTips ();
    }

	public void InitItem(int id)
	{
		m_nItemDataId = id;
		Tab_CommonItem tabCommonItem = TableManager.GetCommonItemByID(m_nItemDataId, 0);
		if (tabCommonItem == null)
		{
			return ;
		}

        SetIconDirect(tabCommonItem.Icon);
	}

	public void OnItemClick()
	{
		ShowItemTips ();
	}

	void ShowItemTips( )
	{
		GameItem item = new GameItem();
		item.DataID = m_nItemDataId;
		
		if (item.IsValid())
		{
            ItemTooltipsLogic.ShowItemTooltip(item, ItemTooltipsLogic.ShowType.Info);
		}
	}

    public bool IsInit()
    {
        return (m_ItemID != GlobeVar.INVALID_ID);
    }

    public void SetIconDirect(string icon)
    {
        m_ItemIcon.spriteName = icon;
        m_ItemIcon.gameObject.SetActive(true);
    }
}
