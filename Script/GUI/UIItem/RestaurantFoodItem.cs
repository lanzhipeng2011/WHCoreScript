using UnityEngine;
using System.Collections;
using GCGame.Table;
using Games.Item;
using Module.Log;
public class RestaurantFoodItem : MonoBehaviour {

    public UILabel m_LabelName;
    public UILabel m_LabelDuraion;
    public UILabel m_LabelRrewardCoin;
    public UILabel m_LabelRrewardPlayerExp;
    public UILabel m_LabelRrewardRestaurantExp;
    public UILabel m_LabelFoodLevel;
    //public GameObject m_FoodRewardItemTitle;
    public ItemSlotLogic[] m_ItemSlotLogic;

    public UILabel m_LabelFoodLevelTitle;
    public UILabel m_LabelDuraionTitle;
    public UILabel m_LabelRrewardCoinTitle;
    public UILabel m_LabelRrewardPlayerExpTitle;
    public UILabel m_LabelRrewardRestaurantExpTitle;

    public GameObject m_BGSprite;
    public GameObject m_GrayBGSprite;

    public GameObject m_RrewardCoinGameobject;
    public GameObject m_RrewardPlayerExpGameobject;

	private Color m_GrayColor = new Color(200.0f, 200.0f, 200.0f, 1.0f);
    private Color m_GrayRewardColor = new Color(184.0f / 255.0f, 184.0f / 255.0f, 184.0f / 255.0f, 1.0f);

	private Color m_NormalColor = new Color(255.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f, 1.0f);
    private Color m_NormalRewardTitleColor = new Color(255.0f, 243.0f, 183.0f, 1.0f);
    private Color m_NormalRewardColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
   
    private RestaurantFoodWindow m_parentWindow;
    //private Tab_RestaurantFood m_curTabData;
    private int m_foodID = -1;
	// Use this for initialization

    public int FoodID { get { return m_foodID; } }
	void Start () {
	    
	}

    private int m_nIndex = -1;
    public int Index
    {
        get { return m_nIndex; }
        set { m_nIndex = value; }
    }
	public void SetData(RestaurantFoodWindow parent, Tab_RestaurantFood curTabData)
    {
        if (null == curTabData)
        {
            LogModule.ErrorLog("SetData::curTabData is null");
            return;
        }
        if ( null == RestaurantData.m_PlayerRestaurantInfo)
        {
            LogModule.ErrorLog("SetData::curTabData is null");
            return;
        }
        if (curTabData.OpenLevel > RestaurantData.m_PlayerRestaurantInfo.m_RestaurantLevel)
        {
            if (null != m_BGSprite)
            {
                m_BGSprite.SetActive(false);
            }
            if (null != m_GrayBGSprite)
            {
                m_GrayBGSprite.SetActive(true);
            }
            if (m_LabelName != null)
            {
                m_LabelName.color = m_GrayColor;
            }
            if (m_LabelDuraionTitle != null)
            {
                m_LabelDuraionTitle.color = m_GrayColor;
            }
            if (m_LabelFoodLevelTitle != null)
            {
                m_LabelFoodLevelTitle.color = m_GrayColor;
            }
            if (m_LabelDuraion != null)
            {
                m_LabelDuraion.color = m_GrayColor;
            }
            if (m_LabelFoodLevel != null)
            {
                m_LabelFoodLevel.color = m_GrayColor;
                //m_LabelFoodLevel.effectStyle = UILabel.Effect.None;
            }        

            if (m_LabelRrewardCoinTitle != null)
            {
                m_LabelRrewardCoinTitle.color = m_GrayRewardColor;
            }
            if (m_LabelRrewardPlayerExpTitle != null)
            {
                m_LabelRrewardPlayerExpTitle.color = m_GrayRewardColor;
            }
            if (m_LabelRrewardRestaurantExpTitle != null)
            {
                m_LabelRrewardRestaurantExpTitle.color = m_GrayRewardColor;
            }
            if (m_LabelRrewardCoin != null)
            {
                m_LabelRrewardCoin.color = m_GrayRewardColor;
            }
            if (m_LabelRrewardPlayerExp != null)
            {
                m_LabelRrewardPlayerExp.color = m_GrayRewardColor;
            }
            if (m_LabelRrewardRestaurantExp)
            {
                m_LabelRrewardRestaurantExp.color = m_GrayRewardColor;
            }

        }
        else
        {
            if (null != m_BGSprite)
            {
                m_BGSprite.SetActive(true);
            }
            if (null != m_GrayBGSprite)
            {
                m_GrayBGSprite.SetActive(false);
            }
      
            if (m_LabelName != null)
            {
                m_LabelName.color = m_NormalColor;
            }
            if (m_LabelDuraionTitle != null)
            {
                m_LabelDuraionTitle.color = m_NormalColor;
            }
            if (m_LabelFoodLevelTitle != null)
            {
                m_LabelFoodLevelTitle.color = m_NormalColor;
            }

            if (m_LabelDuraion != null)
            {
                m_LabelDuraion.color = m_NormalColor;
            }
            if (m_LabelFoodLevel != null)
            {
                m_LabelFoodLevel.color = m_NormalColor;
                //m_LabelFoodLevel.effectStyle = UILabel.Effect.Shadow;
            }

            if (m_LabelRrewardCoinTitle != null)
            {
                m_LabelRrewardCoinTitle.color = m_NormalRewardTitleColor;
            }
            if (m_LabelRrewardPlayerExpTitle != null)
            {
                m_LabelRrewardPlayerExpTitle.color = m_NormalRewardTitleColor;
            }
            if (m_LabelRrewardRestaurantExpTitle != null)
            {
                m_LabelRrewardRestaurantExpTitle.color = m_NormalRewardTitleColor;
            }

            if (m_LabelRrewardCoin != null)
            {
                m_LabelRrewardCoin.color = m_NormalRewardColor;
            }
            if (m_LabelRrewardPlayerExp != null)
            {
                m_LabelRrewardPlayerExp.color = m_NormalRewardColor;
            }
            if (m_LabelRrewardRestaurantExp)
            {
                m_LabelRrewardRestaurantExp.color = m_NormalRewardColor;
            }
            
        }

        m_parentWindow = parent;
        //m_curTabData = curTabData;
        m_foodID = curTabData.Id;
        m_LabelName.text = curTabData.Name;
        int nDurationSeconds = curTabData.CookDuration;
        int nDurationTotalMinutes = nDurationSeconds/60;
        int nDurationMinutes = nDurationTotalMinutes%60;
        int nDurationHours = (nDurationTotalMinutes/60)%24; 
        int nDurationDays = nDurationTotalMinutes/1440;

        string strDay = StrDictionary.GetClientDictionaryString("#{2446}", nDurationDays);
        string strMinutes = StrDictionary.GetClientDictionaryString("#{2448}", nDurationMinutes);
        string strHours = StrDictionary.GetClientDictionaryString("#{2447}", nDurationHours);
        string strDuration = "";
        if (nDurationDays > 0)
        {
            strDuration += strDay;
        }
        if (nDurationHours > 0)
        {
            strDuration += strHours;
        }
        if (nDurationMinutes > 0)
        {
            strDuration += strMinutes;
        }
        m_LabelDuraion.text = strDuration;
        m_LabelRrewardRestaurantExp.text = curTabData.RestaurantExp.ToString();      
         

        m_LabelRrewardCoin.gameObject.SetActive(false);
        m_LabelRrewardCoinTitle.gameObject.SetActive(false);
        m_LabelRrewardPlayerExp.gameObject.SetActive(false);
        m_LabelRrewardPlayerExpTitle.gameObject.SetActive(false);
        m_RrewardCoinGameobject.SetActive(false);
        m_RrewardPlayerExpGameobject.SetActive(false);

        if (curTabData.Money > 0)
        {
            m_LabelRrewardCoin.text = curTabData.Money.ToString();      
            m_LabelRrewardCoin.gameObject.SetActive(true);
            m_LabelRrewardCoinTitle.gameObject.SetActive(true);
            m_RrewardCoinGameobject.SetActive(true);
        }

        if (curTabData.PlayerExp > 0)
        {
            m_LabelRrewardPlayerExp.text = curTabData.PlayerExp.ToString();
            m_LabelRrewardPlayerExp.gameObject.SetActive(true);
            m_LabelRrewardPlayerExpTitle.gameObject.SetActive(true);
            m_RrewardPlayerExpGameobject.SetActive(true);
        }
        
        for (int i = 0; i < m_ItemSlotLogic.Length; i++ )
        {
            m_ItemSlotLogic[i].ClearInfo();
        }
        //m_FoodRewardItemTitle.SetActive(false);
        int nSlotIndex = 0;
        for (int i = 0; i < curTabData.getRewardItemIDCount() && i <m_ItemSlotLogic.Length; i++)
        {
            int nItemID = curTabData.GetRewardItemIDbyIndex(i);
            int nItemCount = curTabData.GetRewardItemNumbyIndex(i);
            if (nItemCount <= 0)
            {
                //m_FoodRewardItemTitle.SetActive(false);               
                m_ItemSlotLogic[i].gameObject.SetActive(false);                 
                continue;
            }
            Tab_CommonItem RewardItem = TableManager.GetCommonItemByID(nItemID, 0);
            if (null == RewardItem)
            {
                //m_FoodRewardItemTitle.SetActive(false);
                m_ItemSlotLogic[i].gameObject.SetActive(false); 
                continue;
            }
            //m_FoodRewardItemTitle.SetActive(true);
            if (nSlotIndex < m_ItemSlotLogic.Length)
            {
                m_ItemSlotLogic[i].gameObject.SetActive(true); 
                m_ItemSlotLogic[nSlotIndex].InitInfo(ItemSlotLogic.SLOT_TYPE.TYPE_RESTAURANT, nItemID, OnClickRewardItem, nItemCount.ToString(), true);
            }
            nSlotIndex++;
            //if (i < m_RewardItemIcon.Length && i < m_RewardItemFrameIcon.Length )
            //{
            //    int nItemID = curTabData.GetRewardItemIDbyIndex(i);
            //    Tab_CommonItem RewardItem = TableManager.GetCommonItemByID(nItemID.ToString(), 0);
            //    if (RewardItem != null)
            //    {
            //        m_RewardItemIcon[i].SetActive(true);
            //        m_RewardItemFrameIcon[i].SetActive(true);
            //        m_RewardItemIcon[i].GetComponent<UISprite>().spriteName = RewardItem.Icon;
            //        m_FoodRewardItemTitle.SetActive(true);
            //    }
            //    else
            //    {
            //        m_RewardItemIcon[i].SetActive(false);
            //        m_RewardItemFrameIcon[i].SetActive(false);
            //        m_FoodRewardItemTitle.SetActive(false);
            //    }
            //}
        }
        //m_LabelFoodLevel.text = curTabData.OpenLevel.ToString() + "级";
        m_LabelFoodLevel.text = StrDictionary.GetClientDictionaryString("#{2787}", curTabData.OpenLevel);
    }
    public void OnClickRewardItem(int nItemID, ItemSlotLogic.SLOT_TYPE eItemType, string strSlotName)
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

    public void Clear()
    {
        m_parentWindow = null;
        //m_curTabData = null;

        m_LabelName.text = "";
        m_LabelDuraion.text = "";
        m_LabelRrewardCoin.text = "";
        m_LabelRrewardPlayerExp.text = "";
        m_LabelRrewardRestaurantExp.text = "";
        m_LabelFoodLevel.text = "";
    }
    
    public void OnItemClick()
    {
        Check_NewPlayerGuide();
        Tab_RestaurantFood curFood = TableManager.GetRestaurantFoodByID(m_foodID, 0);
        if (null == curFood)
        {
            LogModule.ErrorLog("OnItemClick::curTabData is null");
            return;
        }
        if (null == RestaurantData.m_PlayerRestaurantInfo)
        {
            LogModule.ErrorLog("OnItemClick::RestaurantData.m_PlayerRestaurantInfo is null");
            return;
        }
        if (null == m_parentWindow)
        {
            LogModule.ErrorLog("OnItemClick::m_parentWindow is null");
            return;
        }
        if (curFood.OpenLevel > RestaurantData.m_PlayerRestaurantInfo.m_RestaurantLevel)
        {
            return;
        }
        m_parentWindow.OnFoodItemClick(this);
    }

    void Check_NewPlayerGuide()
    {
        if (m_parentWindow && m_parentWindow.NewPlayerGuide_Step == 0)
        {
            m_parentWindow.NewPlayerGuide(1);
        }
    }
}
