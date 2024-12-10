/********************************************************************************
 *	文件名：MissionLogLogic.cs
 *	全路径：	\Script\GUI\MissionLogLogic.cs
 *	创建人：	贺文鹏
 *	创建时间：2014-02-17
 *
 *	功能说明： 任务日志界面。
 *	       
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GCGame.Table;
using Games.GlobeDefine;
using Module.Log;
using GCGame;
using Games.Mission;
using Games.Item;

public class MissionLogLogic : MonoBehaviour {

    // 任务信息相关
    public UILabel m_MissionDec;
    public UIImageButton m_ButtonAbandon;
    public UIImageButton m_ButtonGoto;

    private const int BonusItemMaxNum = 4;
    public GameObject[] m_BonusItem = new GameObject[BonusItemMaxNum]; // 奖励物品
    public UISprite[] m_BonusItemSprite = new UISprite[BonusItemMaxNum]; //任务奖励信息
    public UISprite[] m_BonusQualitySprite = new UISprite[BonusItemMaxNum]; //任务奖励品级
    public UILabel[] m_BonusItemNum = new UILabel[BonusItemMaxNum]; // 任务奖励数量
    public UIGrid m_BonusItemGrid;

    // 物品点击
    private int[] m_ItemType = new int[BonusItemMaxNum];
    private int[] m_ItemDataID = new int[BonusItemMaxNum];
    private int[] m_ItemCount = new int[BonusItemMaxNum];

    public UIGrid m_MissionItemsGrid;
    public GameObject m_Tab_Accepted;
    public UISprite m_Accepted_Active;
    public GameObject m_Tab_CanAccept;
    public UISprite m_CanAccept_Active;

    // 维护一个用来显示的list
    private int m_CurMissionID;
    private const int MaxMissionNum = 9;

    private GameObject m_LastClickItem;

    private static MissionLogLogic m_Instance = null;
    public static MissionLogLogic Instance()
    {
        return m_Instance;
    }

    enum ItemType
    {
        ITEM_MONEY,
        ITEM_EXP,
        ITEM_YUANBAO,
        ITEM_ITEM,
    }

    void Awake()
    {
        m_Instance = this;
    }

	// Use this for initialization
	void Start () {
        InitWindow();
	}

    void OnDestroy()
    {
        m_Instance = null;
    }

    void CloseWindow()
    {
        ClearMyUIGrid();
        UIManager.CloseUI(UIInfo.MissionLogRoot);
    }

    void InitWindow()
    {
        m_CurMissionID = -1;
        CleanTabButtonState();
        // 创建LogItem
        List<int> nMissionIDList = GameManager.gameManager.MissionManager.GetAllNotDailyMissionList();
        if (nMissionIDList.Count > 0)
        {
            if (m_Tab_Accepted)
            {
                MissionTabClick(m_Tab_Accepted);
            }
        }
        else
        {
            if (m_Tab_CanAccept)
            {
                MissionTabClick(m_Tab_CanAccept);
            }
        }
    }

    void ClearMyUIGrid()
    {
        m_LastClickItem = null;

        MissionLogItem[] logItem = m_MissionItemsGrid.gameObject.GetComponentsInChildren<MissionLogItem>();
        for (int i = 0; i < logItem.Length; ++i)
        {
            logItem[i].transform.parent = null;
            Destroy(logItem[i].gameObject);
        }

        m_MissionItemsGrid.repositionNow = true;

        // 清空显示信息
        CleanMissionInfo();
    }

    void CleanTabButtonState()
    {
        m_Accepted_Active.gameObject.SetActive(false);
        m_CanAccept_Active.gameObject.SetActive(false);
    }

    void SetTabAcceptedActive()
    {
        CleanTabButtonState();
        m_Accepted_Active.gameObject.SetActive(true);
    }

    void SetTabCanAcceptedActive()
    {
        CleanTabButtonState();
        m_CanAccept_Active.gameObject.SetActive(true);
    }

    void MissionTabClick(GameObject value)
    {
        // 先清理
        ClearMyUIGrid();
        if (value.name == "Tab_Accepted")
        {
            SetTabAcceptedActive();
            MissionAccepted();
        }
        else if (value.name == "Tab_CanAccept")
        {
            SetTabCanAcceptedActive();
            MissionCanAcceped();
        }
    }

    // 已接任务
    void MissionAccepted()
    {
        MainMissionButtonClick();
    }

    // 可接任务
    void MissionCanAcceped()
    {
        UIManager.LoadItem(UIInfo.MissionLogItem, OnLoadMissionLogItemAcceped);
    }

    void OnLoadMissionLogItemAcceped(GameObject resItem, object param)
    {
        if (null == resItem)
        {
            LogModule.ErrorLog("load MissionLogItem fail");
            return;
        }

        List<int> MissionList = GameManager.gameManager.MissionManager.GetCanAcceptedMissionID(MaxMissionNum);
        if (MissionList.Count <= 0)
        {
            return;
        }

        for (int i = 0; i < MissionList.Count; ++i)
        {
            GameObject ItemObj = Utils.BindObjToParent(resItem, m_MissionItemsGrid.gameObject, "MissionLogItem" + i.ToString());
            if (null != ItemObj && null != ItemObj.GetComponent<MissionLogItem>())
            {
                ItemObj.GetComponent<MissionLogItem>().MissionID = MissionList[i];
            }

            if (i == 0)
            {
                m_LastClickItem = ItemObj;
            }
        }

        m_MissionItemsGrid.repositionNow = true;
        m_MissionItemsGrid.GetComponent<UITopGrid>().recenterTopNow = true;

        // 默认点击第一个可接任务
        UpdateMissionInfo(MissionList[0], m_LastClickItem);

    }
    void MainMissionButtonClick()
    {
        UIManager.LoadItem(UIInfo.MissionLogItem, OnLoadMissionLogItemClick);
    }

    void OnLoadMissionLogItemClick(GameObject resItem, object param)
    {
        if (null == resItem)
        {
            LogModule.ErrorLog("load MissionLogItem fail");
            return;
        }

        // 创建LogItem
        List<int> nMissionIDList = GameManager.gameManager.MissionManager.GetAllNotDailyMissionList();
        if (nMissionIDList.Count <= 0)
        {
            return;
        }

        for (int i = 0; i < MaxMissionNum && i < nMissionIDList.Count; i++)
        {
            // 日常任务 不显示在日志上
            Tab_MissionBase MissionBase = TableManager.GetMissionBaseByID(nMissionIDList[i], 0);
            if (MissionBase.MissionType == (int)MISSIONTYPE.MISSION_DAILY)
            {
                continue;
            }
            GameObject ItemObj = Utils.BindObjToParent(resItem, m_MissionItemsGrid.gameObject, "MissionLogItem" + i.ToString());
            if (ItemObj)
            {
                ItemObj.GetComponent<MissionLogItem>().MissionID = nMissionIDList[i];
                if (i == 0)
                {
                    m_LastClickItem = ItemObj;
                }
            }
        }
        m_MissionItemsGrid.repositionNow = true;
        m_MissionItemsGrid.GetComponent<UITopGrid>().recenterTopNow = true;

        // 默认点击第一个可接任务
        UpdateMissionInfo(nMissionIDList[0], m_LastClickItem);
    }

    // 更新任务内容信息
    public void UpdateMissionInfo(int nMissionID,GameObject gObj)
    {
		ChangeMissionClean();
		
        if (nMissionID < 0)
        {
            return;
        }

        Tab_MissionBase MisBase = TableManager.GetMissionBaseByID(nMissionID, 0);
        if (MisBase == null)
        {
            return;
        }

        // 日常任务不显示
        if (MisBase.MissionType == (int)MISSIONTYPE.MISSION_DAILY)
        {
            //DailyMissionBonusInfo(MisBase);
            return;
        }

        if (null != gObj && null != m_LastClickItem && null != m_LastClickItem.GetComponent<MissionLogItem>())
        {
            m_LastClickItem.GetComponent<MissionLogItem>().SetChooseState(false);
            m_LastClickItem = gObj;
            m_LastClickItem.GetComponent<MissionLogItem>().SetChooseState(true);
        }
        m_CurMissionID = nMissionID;

        if (m_ButtonGoto)
        {
            m_ButtonGoto.gameObject.SetActive(true);
        }
		else
		{
			m_ButtonGoto.gameObject.SetActive(false);
		}

		if (GameManager.gameManager.MissionManager.IsHaveMission(nMissionID) && m_ButtonAbandon)
		{
           	m_ButtonAbandon.gameObject.SetActive(true);
        }
		else if(m_ButtonAbandon)
		{
			m_ButtonAbandon.gameObject.SetActive(false);
		}

        // 描述
        Tab_MissionDictionary MisDictionary = TableManager.GetMissionDictionaryByID(nMissionID, 0);
        if (MisDictionary != null)
        {
            if (m_MissionDec)
            {
                m_MissionDec.text = StrDictionary.GetClientString_WithNameSex(MisDictionary.MissionDesc);
            }
        }

//         if (MisBase.MissionType == (int)MISSIONTYPE.MISSION_MAIN
//             || MisBase.MissionType == (int)MISSIONTYPE.MISSION_BRANCH)
//         {
            MissionBonusInfo(MisBase);
       // }
    }

    void MissionBonusInfo(Tab_MissionBase MisBase)
    {
        // 奖励
        Tab_MissionBonus MisBonus = TableManager.GetMissionBonusByID(MisBase.BonusID, 0);
        if (MisBonus == null)
        {
            for (int i = 0; i < 4; ++i)
            {
                m_BonusItem[i].SetActive(false);
            }
            return;
        }
        
        // 物品信息
        int nItemShowCount = 0;

        //如果有经验
        if (MisBonus.Exp > 0)
        {
            if (nItemShowCount < BonusItemMaxNum && m_BonusItem[nItemShowCount] && m_BonusItemSprite[nItemShowCount] 
                && m_BonusItemNum[nItemShowCount] && m_BonusQualitySprite[nItemShowCount])
            {
                m_BonusItem[nItemShowCount].SetActive(true);
                m_BonusItemSprite[nItemShowCount].spriteName = "jingyan";
                m_BonusQualitySprite[nItemShowCount].spriteName = "";
                m_BonusItemNum[nItemShowCount].text = MisBonus.Exp.ToString();
                m_ItemType[nItemShowCount] = (int)ItemType.ITEM_EXP;
                m_ItemCount[nItemShowCount] = MisBonus.Exp;
                nItemShowCount++;
            }
        }

        //如果有金币
        if (MisBonus.Money > 0)
        {
            if (nItemShowCount < BonusItemMaxNum && m_BonusItem[nItemShowCount] && m_BonusItemSprite[nItemShowCount]
                && m_BonusQualitySprite[nItemShowCount] && m_BonusItemNum[nItemShowCount])
            {
                m_BonusItem[nItemShowCount].SetActive(true);
                m_BonusItemSprite[nItemShowCount].spriteName = "jinbi";
                m_BonusQualitySprite[nItemShowCount].spriteName = "";
                m_BonusItemNum[nItemShowCount].text = MisBonus.Money.ToString();
                m_ItemType[nItemShowCount] = (int)ItemType.ITEM_MONEY;
                m_ItemCount[nItemShowCount] = MisBonus.Money;
                nItemShowCount++;
            }
        }

        //如果有绑定元宝
        if (MisBonus.BindYuanBao > 0)
        {
            if (nItemShowCount < BonusItemMaxNum && m_BonusItem[nItemShowCount] && m_BonusItemSprite[nItemShowCount]
                && m_BonusQualitySprite[nItemShowCount] && m_BonusItemNum[nItemShowCount])
            {
                m_BonusItem[nItemShowCount].SetActive(true);
                m_BonusItemSprite[nItemShowCount].spriteName = "bdyuanbao";
                m_BonusQualitySprite[nItemShowCount].spriteName = "";
                m_BonusItemNum[nItemShowCount].text = MisBonus.BindYuanBao.ToString();
                m_ItemType[nItemShowCount] = (int)ItemType.ITEM_YUANBAO;
                m_ItemCount[nItemShowCount] = MisBonus.BindYuanBao;
                nItemShowCount++;
            }
        }

        // 显示物品
        for (int i = 0; i < MisBonus.getToolIDCount(); i++)
        {
            Tab_CommonItem commonItem = TableManager.GetCommonItemByID(MisBonus.GetToolIDbyIndex(i), 0);
            if (commonItem != null && MisBonus.GetToolNumbyIndex(i) > 0)
            {
                if (nItemShowCount < BonusItemMaxNum && m_BonusItem[nItemShowCount] && m_BonusItemSprite[nItemShowCount]
                    && m_BonusQualitySprite[nItemShowCount] && m_BonusItemNum[nItemShowCount])
                {
                    m_ItemDataID[nItemShowCount] = commonItem.Id;
                    m_BonusItem[nItemShowCount].SetActive(true);
                    m_BonusItemNum[nItemShowCount].text = MisBonus.GetToolNumbyIndex(i).ToString();
                    m_BonusItemSprite[nItemShowCount].spriteName = commonItem.Icon;
                    m_BonusQualitySprite[nItemShowCount].spriteName = GlobeVar.QualityColorGrid[commonItem.Quality - 1];
                    m_ItemType[nItemShowCount] = (int)ItemType.ITEM_ITEM;
                    nItemShowCount++;
                }
            }
        }

        int nProfess = Singleton<ObjManager>.GetInstance().MainPlayer.Profession;
        if (nProfess >= 0 && nProfess < MisBonus.getProfessionItemIDCount())
        {
            if (ShowSpecialMissionItem(MisBase.Id, MisBonus.GetProfessionItemIDbyIndex(nProfess), nItemShowCount))
            {
                nItemShowCount++;
            }
            else
            {
                Tab_CommonItem commonItem = TableManager.GetCommonItemByID(MisBonus.GetProfessionItemIDbyIndex(nProfess), 0);
                if (commonItem != null)
                {
                    if (nItemShowCount < BonusItemMaxNum && m_BonusItem[nItemShowCount] && m_BonusItemSprite[nItemShowCount]
                        && m_BonusQualitySprite[nItemShowCount] && m_BonusItemNum[nItemShowCount])
                    {
                        m_ItemDataID[nItemShowCount] = commonItem.Id;
                        m_BonusItem[nItemShowCount].SetActive(true);
                        m_BonusItemNum[nItemShowCount].text = MisBonus.GetProfessionNumbyIndex(nProfess).ToString();
                        m_BonusItemSprite[nItemShowCount].spriteName = commonItem.Icon;
                        m_BonusQualitySprite[nItemShowCount].spriteName = GlobeVar.QualityColorGrid[commonItem.Quality - 1];
                        m_ItemType[nItemShowCount] = (int)ItemType.ITEM_ITEM;
                        nItemShowCount++;
                    }
                }
            }
        }

        // 多选物品
        int nChoosedItemdNum = MisBonus.ChoosedItemNum;
        if (nChoosedItemdNum > 0 && nChoosedItemdNum <= MisBonus.getItemIDCount())
        {
            Tab_CommonItem commonItem = TableManager.GetCommonItemByID(MisBonus.GetItemIDbyIndex(nChoosedItemdNum - 1), 0);
            if (commonItem != null && MisBonus.GetItemNumbyIndex(nChoosedItemdNum - 1) > 0)
            {
                if (nItemShowCount < BonusItemMaxNum && m_BonusItem[nItemShowCount] && m_BonusItemSprite[nItemShowCount]
                    && m_BonusQualitySprite[nItemShowCount] && m_BonusItemNum[nItemShowCount])
                {
                    m_BonusItem[nItemShowCount].SetActive(true);
                    m_BonusItemSprite[nItemShowCount].spriteName = commonItem.Icon;
                    m_BonusQualitySprite[nItemShowCount].spriteName = GlobeVar.QualityColorGrid[commonItem.Quality - 1];
                    m_BonusItemNum[nItemShowCount].text = MisBonus.GetItemNumbyIndex(nChoosedItemdNum - 1).ToString();
                    m_ItemDataID[nItemShowCount] = commonItem.Id;
                    m_ItemType[nItemShowCount] = (int)ItemType.ITEM_ITEM;
                    nItemShowCount++;
                }
            }
        }

		for( ;nItemShowCount < 4; ++nItemShowCount)
		{
			m_BonusItem[nItemShowCount].SetActive(false);
		}

        m_BonusItemGrid.repositionNow = true;
    }

    void DailyMissionBonusInfo(Tab_MissionBase MissionBase)
    {
        Tab_DailyMission DailyMission = TableManager.GetDailyMissionByID(MissionBase.DalityMissionTabID, 0);
        if (DailyMission == null)
        {
            return;
        }

        int nIndex = 0;
        // 金钱
        if (DailyMission.AwardMoney > 0 && nIndex >= 0 && nIndex < BonusItemMaxNum)
        {
            m_BonusItem[nIndex].SetActive(true);
        }

        // 经验
        nIndex++;
        if (DailyMission.AwardExp > 0 && nIndex >= 0 && nIndex < BonusItemMaxNum)
        {
            m_BonusItem[nIndex].SetActive(true);
        }

        // 声望
        nIndex++;
        if (DailyMission.AwardReputation > 0 && nIndex >= 0 && nIndex < BonusItemMaxNum)
        {
            m_BonusItem[nIndex].SetActive(true);
        }

        // 物品
        nIndex++;
        if (DailyMission.AwardItemNum > 0)
        {
            Tab_CommonItem Item = TableManager.GetCommonItemByID(DailyMission.AwardItemID, 0);
            if (Item != null && nIndex >= 0 && nIndex < BonusItemMaxNum)
            {
                m_BonusItem[nIndex].SetActive(true);
                m_BonusItemSprite[nIndex].spriteName = Item.Icon;
            }
        }

    }

	void ChangeMissionClean() 
	{
		if (m_MissionDec)
		{
			m_MissionDec.text = "";
		}
		
		for (int i = 0; i < BonusItemMaxNum; i++)
		{
			if (m_BonusItem[i] && m_BonusItemSprite[i] && m_BonusQualitySprite[i] && m_BonusItemNum[i])
			{
				m_BonusItemSprite[i].spriteName = "";
				m_BonusQualitySprite[i].spriteName = "";
				m_BonusItemNum[i].text = "";
			}
		}
		
		m_CurMissionID = -1;
		
		for (int i = 0; i < m_ItemDataID.Length; i++)
		{
			m_ItemType[i] = -1;
			m_ItemDataID[i] = -1;
			m_ItemCount[i] = 0;
		}
	}
	
	// 清空任务显示信息
	void CleanMissionInfo()
	{
		if (m_MissionDec)
		{
			m_MissionDec.text = "";
		}
		
		for (int i = 0; i < BonusItemMaxNum; i++)
		{
			if (m_BonusItem[i] && m_BonusItemSprite[i] && m_BonusQualitySprite[i] && m_BonusItemNum[i])
            {
                m_BonusItemSprite[i].spriteName = "";
                m_BonusQualitySprite[i].spriteName = "";
                m_BonusItemNum[i].text = "";
                m_BonusItem[i].SetActive(false);
            }
        }

        m_CurMissionID = -1;
        if (m_ButtonGoto)
        {
            m_ButtonGoto.gameObject.SetActive(false);
        }
        if (m_ButtonAbandon)
        {
            m_ButtonAbandon.gameObject.SetActive(false);
        }

        for (int i = 0; i < m_ItemDataID.Length; i++)
        {
            m_ItemType[i] = -1;
            m_ItemDataID[i] = -1;
            m_ItemCount[i] = 0;
        }
    }

    // 立即前往
    void GotoButtonClick()
    {
        if (m_CurMissionID < 0)
        {
            return;
        }
        if (GameManager.gameManager.MissionManager != null)
        {
            GameManager.gameManager.MissionManager.MissionPathFinder(m_CurMissionID);
        }

        CloseWindow();
    }

    // 放弃任务
    void AbandonButtonClick()
    {
        if (m_CurMissionID < 0)
        {
            return;
        }
        if (GameManager.gameManager.MissionManager != null)
        {
            GameManager.gameManager.MissionManager.AbandonMission(m_CurMissionID);
        }

        CloseWindow();
    }

    // 临时加吧，物品不支持，没办法
    bool ShowSpecialMissionItem(int nMissionID, int nItemID, int nItemShowCount)
    {
        if (nMissionID == 233 || nMissionID == 234 || nMissionID == 235)
        {

            Tab_CommonItem commonItem = TableManager.GetCommonItemByID(nItemID, 0);
            if (commonItem != null)
            {
                if (m_BonusItem[nItemShowCount] && m_BonusItemSprite[nItemShowCount] && m_BonusQualitySprite[nItemShowCount])
                {
                    m_BonusItem[nItemShowCount].SetActive(true);
                    m_BonusItemSprite[nItemShowCount].spriteName = commonItem.Icon;
                    m_BonusQualitySprite[nItemShowCount].spriteName = GlobeVar.QualityColorGrid[commonItem.Quality - 1];
                    m_BonusItemNum[nItemShowCount].text = "1";
                    m_ItemDataID[nItemShowCount] = commonItem.Id;
                    m_ItemType[nItemShowCount] = (int)ItemType.ITEM_ITEM;
                }
            }
            return true;
        }
        return false;
    }

    void SpecialItemClick(GameItem gItem)
    {
        int nItemStarLevel = 0;
        if (m_CurMissionID == 233)
        {
            nItemStarLevel = 12;
        }
        else if (m_CurMissionID == 234)
        {
            nItemStarLevel = 24;
        }
        else if (m_CurMissionID == 235)
        {
            nItemStarLevel = 36;
        }
        if (nItemStarLevel > 0)
        {
            gItem.StarLevel = nItemStarLevel;
        }
    }

    // 物品点击
    void ItemTipClick(GameObject value)
    {
        int nItemType = -1;
        int nItemID = -1;
        int nCount = 0;
        for (int i = 0; i < m_BonusItem.Length && i < m_ItemDataID.Length; i++)
        {
            if (m_BonusItem[i].name == value.name)
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
                SpecialItemClick(item);
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
            }
            MoneyTipsLogic.ShowMoneyTip(tpye, nCount);
        }
    }
}
