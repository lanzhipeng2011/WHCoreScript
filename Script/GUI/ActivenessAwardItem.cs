/********************************************************************************
 *	文件名：ActivenessItem.cs
 *	全路径：	\Script\GUI\ActivenessItem.cs
 *	创建人：	贺文鹏
 *	创建时间：2014-02-24
 *
 *	功能说明： 领奖励界面 项
 *	       
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using GCGame;
using GCGame.Table;
using Games.AwardActivity;
using Games.GlobeDefine;
using Module.Log;
using Games.Item;

public class ActivenessAwardItem : MonoBehaviour
{
    public UIGrid m_ItemGrid;   // 物品grid

    public const int MaxItemCount = 7;
    private int[] m_ItemID = new int[MaxItemCount];
    public GameObject[] m_Items = new GameObject[MaxItemCount];
    public UISprite[] m_ItemsSprite = new UISprite[MaxItemCount];
    public UILabel[] m_ItemsDecText = new UILabel[MaxItemCount];
    public UISprite[] m_ItemsQualitySprite = new UISprite[MaxItemCount];

    public UIImageButton m_AwardButton;
    public UILabel m_AwardButtonDec;

    public UILabel m_AwardInfoText; // 奖励文字
    public string AwardInfoText
    {
        set
        {
            if (m_AwardInfoText)
            {
                m_AwardInfoText.text = value;
            }
        }
    }
    // 设置领取奖励按钮状态
    private AwardState m_AwardButtonState;
    public AwardState AwardButtonState
    {
        get { return m_AwardButtonState; }
        set
        {
            m_AwardButtonState = value;
            SetAwardButtonState(value);
        }
    }

    private int m_nTurnID;
    public int TurnID
    {
        get { return m_nTurnID; }
        set { m_nTurnID = value; }
    }

    private int m_ItemKind;

    enum ItemType
    {
        ITEM_MONEY,
        ITEM_EXP,
        ITEM_YUANBAO,
        ITEM_SHENGWANG,
        ITEM_ZHENQI,
        ITEM_ITEM,
    }
    // 物品点击
    private ItemType[] m_ItemType = new ItemType[MaxItemCount];
    private int[] m_ItemDataID = new int[MaxItemCount];
    private int[] m_ItemCount = new int[MaxItemCount];

    //void Start()
    //{

    //}

    void Init(int nTurnID, AwardState state)
    {
        m_nTurnID = nTurnID;
        m_AwardButtonState = state;
    }

    void CleanUp()
    {
        m_ItemKind = 0;
        m_nTurnID = 0;
        m_AwardButtonState = AwardState.AWARD_CANNNTHAVE;
        m_AwardInfoText.text = "";
        for (int nIndex = 0; nIndex < MaxItemCount; nIndex++)
        {
            m_ItemID[nIndex] = -1;
            if (m_ItemsDecText[nIndex])
            {
                m_ItemsDecText[nIndex].text = "";
            }
            if (m_Items[nIndex])
            {
                m_ItemsSprite[nIndex].spriteName = "";
                m_Items[nIndex].SetActive(false);
            }
            if (m_ItemsQualitySprite[nIndex])
            {
                m_ItemsQualitySprite[nIndex].spriteName = "";
            }
        }

        for (int i = 0; i < m_ItemDataID.Length; i++)
        {
            m_ItemType[i] = ItemType.ITEM_MONEY;
            m_ItemDataID[i] = -1;
            m_ItemCount[i] = 0;
        }

        m_ItemGrid.repositionNow = true;
    }

    public static ActivenessAwardItem CreateAwardItem(string strName, GameObject gParent, GameObject resItem)
    {
        GameObject curItem = Utils.BindObjToParent(resItem, gParent);
        if (curItem == null)
        {
            LogModule.DebugLog("ActivenessAwardItem create error");
            return null;
        }

        curItem.name = strName;
        ActivenessAwardItem AwardItem = curItem.GetComponent<ActivenessAwardItem>();
        if (AwardItem != null)
        {
            AwardItem.CleanUp();
        }
        return AwardItem;
    }

    // 设置经验、金钱、元宝、真气、声望信息
    bool SetItemInfo(int nIndex, int nValue, ItemType tType, string strSpriteName)
    {
        if (nIndex < 0 || nIndex > MaxItemCount)
        {
            return false;
        }
        if (nValue <= 0)
        {
            return false;
        }
        if (m_Items[nIndex] && m_ItemsSprite[nIndex] && m_ItemsDecText[nIndex] && m_ItemsQualitySprite[nIndex])
        {
            m_Items[nIndex].SetActive(true);
            m_ItemsSprite[nIndex].spriteName = strSpriteName;
            m_ItemsDecText[nIndex].text = nValue.ToString();
            m_ItemsQualitySprite[nIndex].gameObject.SetActive(false);
            m_ItemType[nIndex] = tType;
            m_ItemCount[nIndex] = nValue;
        }

        return true;
    }

    // 添加 经验、金钱、元宝UI
    public void AddAwardUI(int nExp, int nMoney, int nYuanbao, int nSkillExp, int nReputation)
    {
        SetItemInfo(m_ItemKind, nExp, ItemType.ITEM_EXP, "jingyan");
        m_ItemKind++;

        // 金币添加等级系数
        int nLevel = Singleton<ObjManager>.Instance.MainPlayer.BaseAttr.Level;
        int nMoneyFactor = nLevel/10+1;
        SetItemInfo(m_ItemKind, nMoney * nMoneyFactor, ItemType.ITEM_MONEY, "jinbi");
        m_ItemKind++;

        SetItemInfo(m_ItemKind, nYuanbao, ItemType.ITEM_YUANBAO, "yuanbao");
        m_ItemKind++;

        int nSkillExpFactor = nLevel / 40 + 1;
        SetItemInfo(m_ItemKind, nSkillExp * nSkillExpFactor, ItemType.ITEM_ZHENQI, "zhenqidan");
        m_ItemKind++;

        //SetItemInfo(m_ItemKind, nReputation, "wenhao");
       // m_ItemKind++;
  
        m_ItemGrid.repositionNow = true;
    }
    public void AddAwardUIRepution(int nReputation)
    {
        SetItemInfo(m_ItemKind, nReputation, ItemType.ITEM_SHENGWANG, "shengwang");
        m_ItemKind++;
        m_ItemGrid.repositionNow = true;
    }
    // 添加物品UI
    public void AddItemUI(int nItemID, int nCount)
    {
        Tab_CommonItem Item = TableManager.GetCommonItemByID(nItemID, 0);
        int nIndex = m_ItemKind;
        if (null != Item && nCount > 0 && nIndex >= 4 && nIndex < 6)
        {
            m_ItemKind++;
            if (m_Items[nIndex] && m_ItemsDecText[nIndex])
            {
                m_ItemID[nIndex] = nItemID;
                m_Items[nIndex].SetActive(true);
                m_ItemsSprite[nIndex].spriteName = Item.Icon;
                m_ItemsDecText[nIndex].text = Item.Name + "*" + nCount.ToString();
                m_ItemsQualitySprite[nIndex].spriteName = GlobeVar.QualityColorGrid[Item.Quality - 1];
                m_ItemDataID[nIndex] = nItemID;
                m_ItemType[nIndex] = ItemType.ITEM_ITEM;
                m_ItemCount[nIndex] = nCount;
            }
        }

        m_ItemGrid.repositionNow = true;
    }

    void SetAwardButtonState(AwardState state)
    {
        switch (state)
        {
            case AwardState.AWARD_CANNNTHAVE:
                {
                    m_AwardButtonDec.text = Utils.GetDicByID(1381);
                    m_AwardButton.isEnabled = false;
                }
                break;
            case AwardState.AWARD_CANHAVE:
                {
                    m_AwardButtonDec.text = Utils.GetDicByID(1378);
                    m_AwardButton.isEnabled = true;
                }
                break;
            case AwardState.AWARD_HAVEDONE:
                {
                    m_AwardButtonDec.text = Utils.GetDicByID(1380);
                    m_AwardButton.isEnabled = false;
                }
                break;
            case AwardState.AWARD_NOTHAVEDONE:
                {
                    m_AwardButtonDec.text = Utils.GetDicByID(1379);
                    m_AwardButton.isEnabled = false;
                }
                break;
            default:
                break;
        }
    }

    // 领取奖励按钮点击
    void AwardButtonClick()
    {
        GameManager.gameManager.PlayerDataPool.AwardActivityData.SendActivenessAward(m_nTurnID);
    }

    // 物品点击
    void ItemTipClick(GameObject value)
    {
        ItemType nItemType = ItemType.ITEM_MONEY;
        int nItemID = -1;
        int nCount = 0;
        for (int i = 0; i < m_Items.Length && i < m_ItemDataID.Length; i++)
        {
            if (m_Items[i].name == value.name)
            {
                nItemType = m_ItemType[i];
                nItemID = m_ItemDataID[i];
                nCount = m_ItemCount[i];
                break;
            }
        }
        if (nItemType == ItemType.ITEM_ITEM)
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
            }
        }
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
                case ItemType.ITEM_SHENGWANG:
                    tpye = MoneyTipsLogic.MoneyType.ITEM_SHENGWANG;
                    break;
                case ItemType.ITEM_ZHENQI:
                    tpye = MoneyTipsLogic.MoneyType.ITEM_ZHENQI;
                    break;
            }
            MoneyTipsLogic.ShowMoneyTip(tpye, nCount);
        }
    }
}

