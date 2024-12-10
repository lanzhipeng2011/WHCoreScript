/********************************************************************
	created:	2014/01/07
	created:	7:1:2014   11:09
	filename: 	RewardItem.cs
	author:		王迪
	
	purpose:	奖励的物品ICON
*********************************************************************/

using UnityEngine;
using System.Collections;
using GCGame.Table;
using Games.GlobeDefine;

public enum ItemType
{
    TYPE_COIN,
    TYPE_YUANBAO,
    TYPE_BINDYUANBAO,
    TYPE_EXP,
}
public class RewardItem : MonoBehaviour {

    public GameObject BackSprite;
    public GameObject FrontSprite;
	public GameObject LabelNum;
    public delegate void ItemClickDelegate(RewardItem item);
    public ItemClickDelegate delItemClick = null;
    public UISprite m_QualitySprite;

	private int m_itemID;

	void Start ()
    {

	}

    public bool SetData(int itemID, int itemCount, bool isAlwaysShowBack = false)
    {

		m_itemID = itemID;
		Tab_CommonItem curItem = TableManager.GetCommonItemByID(itemID, 0);
		if(null == curItem)
		{
            FrontSprite.SetActive(false);
            LabelNum.SetActive(false);
            m_QualitySprite.gameObject.SetActive(false);
            BackSprite.SetActive(isAlwaysShowBack);
			return false;
		}

		BackSprite.SetActive(true);
		FrontSprite.SetActive(true);
		FrontSprite.GetComponent<UISprite>().spriteName = curItem.Icon;
		if(itemCount > 0)
		{
			LabelNum.SetActive(true);
			LabelNum.GetComponent<UILabel>().text = itemCount.ToString();
		}
        else
        {
            LabelNum.SetActive(false);
        }
        m_QualitySprite.gameObject.SetActive(true);
        int colorQuality = curItem.Quality - 1;
        if (colorQuality >= 0 && colorQuality < GlobeVar.QualityColorGrid.Length)
        {
            m_QualitySprite.spriteName = GlobeVar.QualityColorGrid[curItem.Quality - 1];
        }
       
        return true;
    }

    public bool SetMoneyData(ItemType itemType, int moneyCount)
    {
        FrontSprite.SetActive(false);
        BackSprite.SetActive(false);
        LabelNum.SetActive(false);

        switch (itemType)
        {
            case ItemType.TYPE_COIN:
                FrontSprite.GetComponent<UISprite>().spriteName = "jinbi";
                break;
            case ItemType.TYPE_YUANBAO:
                FrontSprite.GetComponent<UISprite>().spriteName = "yuanbao";
                break;
            case ItemType.TYPE_BINDYUANBAO:
                FrontSprite.GetComponent<UISprite>().spriteName = "yuanbao";
                break;
            case ItemType.TYPE_EXP:
                FrontSprite.GetComponent<UISprite>().spriteName = "jingyan";
                break;
            default:
                return false;
        }
        LabelNum.GetComponent<UILabel>().text = moneyCount.ToString();
        FrontSprite.SetActive(true);
        LabelNum.SetActive(true);
        return true;
    }

	void OnRewardItemClick()
	{
		ItemTooltipsLogic.ShowItemTooltip(m_itemID, ItemTooltipsLogic.ShowType.Info);
	}

    void OnItemClick()
    {
        if (null != delItemClick)
        {
            delItemClick(this);
        }
    }
	
}
