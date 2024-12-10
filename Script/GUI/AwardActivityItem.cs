/******************************************************************************** *	文件名：AwardActivityItem.cs *	全路径：	\Script\GUI\AwardActivityItem.cs *	创建人：	贺文鹏 *	创建时间：2014-02-24 * *	功能说明： 领奖励界面 项 *	        *	修改记录：*********************************************************************************/using UnityEngine;using System.Collections;using GCGame;using GCGame.Table;using Games.AwardActivity;
using Games.GlobeDefine;
using Module.Log;
using Games.Item;// 领取按钮三种状态public enum AwardState{    AWARD_CANNNTHAVE,    AWARD_CANHAVE,    AWARD_HAVEDONE,    AWARD_NOTHAVEDONE}public class AwardActivityItem : MonoBehaviour{    public UILabel m_AwardInfoText; // 奖励文字    public string AwardInfoText    {        set        {            if (m_AwardInfoText)            {                m_AwardInfoText.text = value;            }        }    }    public UIGrid m_ItemGrid;   // 物品grid    public const int MaxItemCount = 7;
    private int[] m_ItemID = new int[MaxItemCount];    public GameObject[] m_Items = new GameObject[MaxItemCount];    public UISprite[] m_ItemsSprite = new UISprite[MaxItemCount];    public UILabel[] m_ItemsDecText = new UILabel[MaxItemCount];
    public UISprite[] m_ItemsQualitySprite = new UISprite[MaxItemCount];    private int m_ItemKind;

    //public UISprite m_SpritMin;
    //public UISprite m_SpritSec;

    public UIImageButton m_AwardButton;
    public UILabel m_AwardButtonDec;    private AwardActivityType m_AwardType;    public AwardActivityType AwardType    {        get { return m_AwardType; }        set { m_AwardType = value; }    }    // 设置领取奖励按钮状态    private AwardState m_AwardButtonState;    public AwardState AwardButtonState    {        get { return m_AwardButtonState; }
        set
        {
            m_AwardButtonState = value;            SetAwardButtonState(value);        }    }        // 初始化    void init(AwardActivityType type, AwardState state)    {        m_AwardType = type;        m_AwardButtonState = state;    }    // 清理    void CleanUp()    {        m_ItemKind = 0;        m_AwardType = AwardActivityType.AWARD_NONE;        m_AwardButtonState = AwardState.AWARD_CANNNTHAVE;        m_AwardInfoText.text = "";        for (int nIndex = 0; nIndex < MaxItemCount; nIndex++)        {
            m_ItemID[nIndex] = -1;            if (m_ItemsDecText[nIndex])            {                m_ItemsDecText[nIndex].text = "";            }            if (m_Items[nIndex])            {
                m_ItemsSprite[nIndex].spriteName = "";                m_Items[nIndex].SetActive(false);            }
            if (m_ItemsQualitySprite[nIndex])            {
                m_ItemsQualitySprite[nIndex].spriteName = "";            }        }        m_ItemGrid.repositionNow = true;    }    // 添加 经验、金钱、元宝UI    public void AddExpMoneyYuanbaoUI(int nExp, int nMoney, int nYuanbao)    {        if (nExp > 0)        {            m_Items[0].SetActive(true);
            m_ItemsSprite[0].spriteName = "jingyan";            m_ItemsDecText[0].text = nExp.ToString();
            m_ItemsQualitySprite[0].gameObject.SetActive(false);        }        if (nMoney > 0)        {            m_Items[1].SetActive(true);
            m_ItemsSprite[1].spriteName = "jinbi";            m_ItemsDecText[1].text = nMoney.ToString();
            m_ItemsQualitySprite[1].gameObject.SetActive(false);        }        if (nYuanbao > 0)        {            m_Items[2].SetActive(true);
            m_ItemsSprite[2].spriteName = "yuanbao";            m_ItemsDecText[2].text = nYuanbao.ToString();
            m_ItemsQualitySprite[2].gameObject.SetActive(false);        }        m_ItemGrid.repositionNow = true;    }    // 添加物品UI    public void AddItemUI(int nItemID, int nCount)    {        if (m_ItemKind > 4)        {            LogModule.DebugLog("AwardActivityItem:ItemKind > 4");            return;        }        Tab_CommonItem Item = TableManager.GetCommonItemByID(nItemID, 0);        int nIndex = m_ItemKind + 3;
        if (null != Item && nCount > 0)        {            m_ItemKind++;            if (m_Items[nIndex] && m_ItemsDecText[nIndex])            {
                m_ItemID[nIndex] = nItemID;                m_Items[nIndex].SetActive(true);
                m_ItemsSprite[nIndex].spriteName = Item.Icon;
                m_ItemsDecText[nIndex].text = /*Item.Name + "*"+*/ nCount.ToString();
                m_ItemsQualitySprite[nIndex].spriteName = GlobeVar.QualityColorGrid[Item.Quality - 1];            }        }        m_ItemGrid.repositionNow = true;    }    public void SetAwardTime(int nLeftTime)
    {
        //if (m_SpritMin == null)        //{
        //    return;        //}
        //if (m_SpritSec == null)        //{
        //    return;        //}
        //int nMin = nLeftTime / 60;
        //int nShiWei = nMin / 10;
        //int nGeWei = nMin % 10;

        //if (nShiWei >= 0 && nShiWei <= 9
        //    && nGeWei >= 0 && nGeWei <= 9)
        //{
        //    m_SpritMin.spriteName = nShiWei.ToString();
        //    m_SpritSec.spriteName = nGeWei.ToString();
        //}
    }    // 设置Button状态    void SetAwardButtonState(AwardState state)    {        switch (state)        {            case AwardState.AWARD_CANNNTHAVE:                {
                    m_AwardButtonDec.text = Utils.GetDicByID(1380);                    m_AwardButton.isEnabled = false;                }                break;            case AwardState.AWARD_CANHAVE:                {
                    m_AwardButtonDec.text = Utils.GetDicByID(1378);                    m_AwardButton.isEnabled = true;                }                break;            case AwardState.AWARD_HAVEDONE:                {
                    m_AwardButtonDec.text = Utils.GetDicByID(1380);                    m_AwardButton.isEnabled = false;                }                break;            case AwardState.AWARD_NOTHAVEDONE:                {
                    m_AwardButtonDec.text = Utils.GetDicByID(1379);                    m_AwardButton.isEnabled = false;                }                break;            default:                break;        }    }    // 领取奖励按钮点击    void AwardButtonClick()    {        GameManager.gameManager.PlayerDataPool.AwardActivityData.SendAwardPacket(m_AwardType);    }    public static AwardActivityItem CreateAwardItem(string strName, GameObject gParent, GameObject resItem, AwardActivityType type, AwardState state)    {
        GameObject curItem = Utils.BindObjToParent(resItem, gParent);        if (curItem == null)        {            LogModule.DebugLog("AwardActivityItem create error");            return null;        }        curItem.name = strName;        AwardActivityItem AwardItem = curItem.GetComponent<AwardActivityItem>();        if (AwardItem != null)        {            AwardItem.CleanUp();            AwardItem.init(type, state);        }        return AwardItem;    }

    void ItemTipClick(GameObject value)
    {
        int nItemID = -1;
        for (int i = 0; i < MaxItemCount; i++)
        {
            if (m_Items[i].name == value.name)
            {
                nItemID = m_ItemID[i];
                break;
            }
        }
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
    }}