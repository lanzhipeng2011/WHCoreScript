/******************************************************************************** *	文件名：RewardAwardItem.cs *	全路径：	\Script\GUI\RewardAwardItem.cs *	创建人：	贺文鹏 *	创建时间：2014-07-03 * *	功能说明： 奖励物品 Item *	        *	修改记录：*********************************************************************************/
using UnityEngine;
using System.Collections;
using GCGame.Table;
using Games.Item;

public class RewardAwardItem : MonoBehaviour {
    public enum ItemType
    {
        ITEM_MONEY,
        ITEM_EXP,
        ITEM_YUANBAO,
        ITEM_ITEM,
    }

    // 领取按钮三种状态
    public enum AwardState
    {
        AWARD_CANNNTHAVE,
        AWARD_CANHAVE,
        AWARD_HAVEDONE,
        AWARD_NOTHAVEDONE
    }

    // 物品相关
    public GameObject[] m_ItemGrid;
    public UISprite[] m_ItemIcon;
    public UILabel[] m_ItemLabel;
    public GameObject[] m_EffectPoint;

    // 状态相关
    public UISprite m_HaveAwardSprite;
    public UISprite m_ChooseSprit;

    private int m_ShowItemIndex = 0;
    private int[] m_ItemType = new int[2];
    private int[] m_ItemDataID = new int[2];
    private int[] m_ItemCount = new int[2];

	// Use this for initialization
	void Start () {
	
	}

    public void CleanUp()
    {
        m_ShowItemIndex = 0;
        for (int i = 0; i < m_ItemDataID.Length; i++ )
        {
            m_ItemType[i] = -1;
            m_ItemDataID[i] = -1;
            m_ItemCount[i] = 0;
        }
        for (int i = 0; i < m_ItemGrid.Length; i++ )        {            if (m_ItemGrid[i])            {                m_ItemGrid[i].SetActive(false);           }        }        for (int i = 0; i < m_ItemIcon.Length; i++)
        {
            if (m_ItemIcon[i])
            {
                m_ItemIcon[i].spriteName = "";
            }
        }
        for (int i = 0; i < m_ItemLabel.Length; i++)
        {
            if (m_ItemLabel[i])
            {
                m_ItemLabel[i].text = "";
            }
        }
        if (m_HaveAwardSprite && m_ChooseSprit)        {
            m_HaveAwardSprite.gameObject.SetActive(false);
            m_ChooseSprit.gameObject.SetActive(false);        }

        for (int i = 0; i < m_EffectPoint.Length; i++)
        {
            m_EffectPoint[i].SetActive(false);
        }
    }

    public void AddItem(ItemType nItemType, int nItemID, int nItemCount)
    {
        if (nItemType<ItemType.ITEM_MONEY || nItemType > ItemType.ITEM_ITEM            || nItemCount <= 0)        {
            return;        }

        if (m_ShowItemIndex <0             || m_ShowItemIndex >= m_ItemGrid.Length            || m_ShowItemIndex >= m_ItemIcon.Length            || m_ShowItemIndex >= m_ItemLabel.Length            || m_ShowItemIndex >= m_ItemType.Length            || m_ShowItemIndex >= m_ItemDataID.Length
            || m_ShowItemIndex >= m_ItemCount.Length)        {
            return;        }

        if (m_ItemGrid[m_ShowItemIndex] && m_ItemIcon[m_ShowItemIndex] && m_ItemLabel[m_ShowItemIndex])        {
            m_ItemGrid[m_ShowItemIndex].SetActive(true);
            m_ItemType[m_ShowItemIndex] = (int)nItemType;
            m_ItemCount[m_ShowItemIndex] = nItemCount;
            switch (nItemType)
            {
                case ItemType.ITEM_MONEY:
                    m_ItemIcon[m_ShowItemIndex].spriteName = "jinbi";
                    m_ItemLabel[m_ShowItemIndex].text = nItemCount.ToString();
                    break;
                case ItemType.ITEM_EXP:
                    m_ItemIcon[m_ShowItemIndex].spriteName = "jingyan";
                    m_ItemLabel[m_ShowItemIndex].text = nItemCount.ToString();
                    break;
                case ItemType.ITEM_YUANBAO:
                    m_ItemIcon[m_ShowItemIndex].spriteName = "bdyuanbao";
                    m_ItemLabel[m_ShowItemIndex].text = nItemCount.ToString();
                    break;
                case ItemType.ITEM_ITEM:
                    Tab_CommonItem cItem = TableManager.GetCommonItemByID(nItemID, 0);
                    if (cItem != null)                    {
                        m_ItemDataID[m_ShowItemIndex] = nItemID;
                        m_ItemIcon[m_ShowItemIndex].spriteName = cItem.Icon;
                        m_ItemLabel[m_ShowItemIndex].text = nItemCount.ToString();                    }
                    break;
            }
            m_ShowItemIndex += 1;        }
    }

    public void UpdateItemState(AwardState ItemState)
    {
        if (ItemState == AwardState.AWARD_HAVEDONE)        {
            m_HaveAwardSprite.gameObject.SetActive(true);        }
        else
        {
//             bool bIsCanHave = (ItemState == AwardState.AWARD_CANHAVE)?true:false;//             for (int i = 0; i < m_EffectPoint.Length; i++)//             {//                 m_EffectPoint[i].SetActive(bIsCanHave);//             }
            m_HaveAwardSprite.gameObject.SetActive(false);
        }
    }

    public void SetChooseState(bool bActive)
    {
        if (m_ChooseSprit)        {
            m_ChooseSprit.gameObject.SetActive(bActive);        }
    }
        
    // 物品点击
    void ItemTipClick(GameObject value)
    {
        int nItemType = -1;
        int nItemID = -1;
        int nCount = 0;
        for (int i = 0; i < m_ItemGrid.Length && i < m_ItemDataID.Length; i++)
        {
            if (m_ItemGrid[i].name == value.name)
            {
                nItemType = m_ItemType[i];
                nItemID = m_ItemDataID[i];
                nCount = m_ItemCount[i];
                break;
            }
        }
        if (nItemType == (int)ItemType.ITEM_ITEM)
        {
            if (nItemID <= -1)
            {
                return;
            }
            GameItem item = new GameItem();
            item.DataID = nItemID;
            if (item.IsEquipMent())
            {
                EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Info);
            }
            else
            {
                ItemTooltipsLogic.ShowItemTooltip(item, ItemTooltipsLogic.ShowType.Info);
            }        }
        else
        {
            MoneyTipsLogic.MoneyType tpye = MoneyTipsLogic.MoneyType.ITEM_NONE;
            switch ((ItemType)nItemType)
            {
                case ItemType.ITEM_EXP:
                    tpye = MoneyTipsLogic.MoneyType.ITEM_EXP;
                    break;
                case ItemType.ITEM_MONEY:
                    tpye = MoneyTipsLogic.MoneyType.ITEM_MONEY;
                    break;
                case ItemType.ITEM_YUANBAO:
                    tpye = MoneyTipsLogic.MoneyType.ITEM_YUANBAO;
                    break;
            }
            MoneyTipsLogic.ShowMoneyTip(tpye, nCount);
        }
    }

    // 播放帧动画
    public void PlayEffect()
    {
        for (int i = 0; i < m_EffectPoint.Length; i++)
        {
            m_EffectPoint[i].SetActive(true);
        }
    }
}
