/********************************************************************
	created:	2014/02/17
	created:	17:2:2014   9:49
	filename: 	MissionInfoController.cs
	author:		王迪
	
	purpose:	从MissionDialogAndLeftTabsLogic分离，单独显示任务对话框
*********************************************************************/
using UnityEngine;
using System.Collections;
using GCGame.Table;
using Games.LogicObj;
using Games.Mission;
using Games.GlobeDefine;
using Module.Log;
using Games.Item;

public class MissionInfoController : UIControllerBase<MissionInfoController> {

    public GameObject m_GameObj;

    //接受任务界面信息
    public UILabel m_NpcName;        // 对话NPC
    public UITexture m_NpcSprite;		//任务NPC头像

    //任务经济系统奖励（经验，金钱，绑定元宝三选二）
    private const int MaxItemNum = 2;
    public UIGrid m_MissionItemGrid;
    public UILabel[] m_MissionEconomicAwardText = new UILabel[MaxItemNum];       //任务经济系统奖励文字
    public UISprite[] m_MissionEconomicAwardSprite = new UISprite[MaxItemNum];   //任务经济系统奖励图标
    public GameObject[] m_MissionEconomicAwardItem = new GameObject[MaxItemNum];   //任务经济系统奖物品

    public GameObject[] m_MissionBonusItem = new GameObject[MaxItemNum]; // 奖励物品
    public UILabel[] m_MissionBonusItemNum = new UILabel[MaxItemNum]; // 奖励物品数量
    public UISprite[] m_MissionBonusItemSprite = new UISprite[MaxItemNum]; //任务奖励信息
    public UISprite[] m_MissionBonusItemQuality = new UISprite[MaxItemNum];
    private int[]    m_MissionBonusItemID = new int[MaxItemNum]; // 物品ID

    public UILabel m_MissionConent;			//任务内容
    public GameObject m_AcceptButton;         //接受按钮
    public GameObject m_CompleteButton;       //完成按钮
    public GameObject m_MissionInfoAward;   // 任务奖励信息

    private int m_CurMissionID;			//当前任务ID
    private MissionUIType m_MissionType = MissionUIType.TYPE_NONE;

    public enum MissionUIType
    {
        TYPE_NONE,
        TYPE_ACCETP, 
        TYPE_COMPLETE,
    }

    public class MissionUIInfo
    {
        public MissionUIInfo(int missionID, MissionUIType type)
        {
            _nMissionID = missionID;
            _type = type;
        }
        public int _nMissionID;
        public MissionUIType _type;
    }
    public static void ShowNpcDialogUI(int nDialogID)
    {
        UIManager.ShowUI(UIInfo.MissionInfoController, OnShowNpcDialogUI, nDialogID);

    }

    static void OnShowNpcDialogUI(bool bSuccess, object dialogID)
    {
        if (!bSuccess)
        {
            LogModule.ErrorLog("load MissionInfoController fail");
            return;
        }

        int nDialogID = (int)dialogID;

        if (null != MissionInfoController.Instance())
        {
            MissionInfoController.Instance().DoShowNPCDialog(nDialogID);
        }
    }
    /// <summary>
    /// 显示对话框
    /// </summary>
    /// <param name='bMission'>
    /// 是否是任务
    /// </param>
    /// <param name='nDialogId'>
    /// 任务ID/对白ID
    /// </param>
    public static void ShowMissionDialogUI(int nMissionID)
    {
        Tab_MissionBase table = TableManager.GetMissionBaseByID(nMissionID, 0);
        if (table != null)
        {
            // NPC 距离判断
            Obj_NPC TargetNpc = Singleton<DialogCore>.GetInstance().CareNPC;
            if (TargetNpc == null)
            {
                return;
            }

            if (!TargetNpc.IsHaveMission(nMissionID))
                return;

            // 是否已接取
            bool isHaveMission = GameManager.gameManager.MissionManager.IsHaveMission(nMissionID);
            MissionState misState = (MissionState)GameManager.gameManager.MissionManager.GetMissionState(nMissionID);
            if (isHaveMission && misState != MissionState.Mission_Failed)
            {
                if (MissionState.Mission_Completed == misState)
                {
                    if (table.CompleteDataID == TargetNpc.BaseAttr.RoleBaseID)
                    {
                        ShowMissionDialogUI(nMissionID, MissionInfoController.MissionUIType.TYPE_COMPLETE);
                    }
                }

            }
            else
            {
                bool isCanAcceptMission = GameManager.gameManager.MissionManager.CanAcceptMission(nMissionID);
                if (isCanAcceptMission)
                {
                    if (table.AcceptDataID == TargetNpc.BaseAttr.RoleBaseID)
                    {
                        ShowMissionDialogUI(nMissionID, MissionInfoController.MissionUIType.TYPE_ACCETP);
                    }
                }
            }
        }
    }

    public static void ShowMissionDialogUI(int nMissionID, MissionUIType type)
    {
        MissionUIInfo curInfo = new MissionUIInfo(nMissionID, type);
        UIManager.ShowUI(UIInfo.MissionInfoController, OnShowMissionDialogUI, curInfo);
        
    }

    static void OnShowMissionDialogUI(bool bSuccess, object dialogID)
    {
        if (!bSuccess)
        {
            LogModule.ErrorLog("load MissionInfoController fail");
            return;
        }

        MissionUIInfo curInfo = (MissionUIInfo)dialogID;

        if (null != MissionInfoController.Instance())
        {
            MissionInfoController.Instance().MissionUI(curInfo._nMissionID, curInfo._type);
        }
    }

    void Awake()
    {
        SetInstance(this);
    }
	// Use this for initialization
	void Start () {
        if (m_GameObj)
        {
            m_GameObj.SetActive(true);
        }
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.frameCount % 15 != 0)
        {
            if (gameObject.activeSelf)
            {
                if (false == Singleton<DialogCore>.GetInstance().IsInDialogArea())
                {
                    //UIManager.CloseUI(UIInfo.MissionInfoController);
                    OnCloseClick();
                }
            }
        }
    }

    void OnDisable()
    {
        CleanUp();
    }

    void OnDestroy()
    {
        SetInstance(null);
    }

    void CleanUp()
    {
        if (m_MissionInfoAward && m_AcceptButton && m_CompleteButton)
        {
            m_MissionInfoAward.SetActive(false);
            m_AcceptButton.SetActive(false);
            m_CompleteButton.SetActive(false);
        }

        if (m_MissionConent)
        {
            m_MissionConent.text = "";
        }

        for (int i = 0; i < MaxItemNum;i++)
        {
            if (m_MissionBonusItem[i] && m_MissionBonusItemNum[i] && m_MissionBonusItemSprite[i] && m_MissionBonusItemQuality[i])
            {
                m_MissionBonusItem[i].SetActive(false);
                m_MissionBonusItemNum[i].text = "";
                m_MissionBonusItemSprite[i].spriteName = "";
                m_MissionBonusItemQuality[i].spriteName = "";
                m_MissionBonusItemID[i] = -1;
            }
        }

        for (int i = 0; i < MaxItemNum; i++)
        {
            if (m_MissionEconomicAwardItem[i] && m_MissionEconomicAwardSprite[i] && m_MissionEconomicAwardText[i])
            {
                m_MissionEconomicAwardItem[i].SetActive(false);
                m_MissionEconomicAwardSprite[i].gameObject.SetActive(false);
                m_MissionEconomicAwardText[i].gameObject.SetActive(false);
            }
            
        }

        if (m_MissionItemGrid)
        {
            m_MissionItemGrid.repositionNow = true;
        }
    }

    void DoShowNPCDialog(int nDialogID)
    {
        // 先清理
        CleanUp();

        Tab_NpcDialog DialogLine = TableManager.GetNpcDialogByID(nDialogID, 0);
        if (DialogLine != null)
        {
            m_MissionInfoAward.SetActive(false);
            m_AcceptButton.SetActive(false);
            m_CompleteButton.SetActive(false);

            if (m_MissionConent)
            {
                m_MissionConent.text = StrDictionary.GetClientString_WithNameSex(DialogLine.Dialog);
            }
        }
        NpcDialogUI();

        GameManager.gameManager.SoundManager.PlaySoundEffect(128);
    }
    // Npc头像
    void NpcDialogUI()
    {
        Obj_NPC TargetNpc = Singleton<DialogCore>.GetInstance().CareNPC;
        if (TargetNpc == null)
        {
            return;
        }

        //int nNpcDataID = TargetNpc.BaseAttr.DataID;
        if (TargetNpc.ModelID < 0)
        {
            return;
        }

        Tab_RoleBaseAttr roleBase = TableManager.GetRoleBaseAttrByID(TargetNpc.BaseAttr.RoleBaseID, 0);
        if (roleBase == null)
        {
            return;
        }

        Tab_CharModel charModel = TableManager.GetCharModelByID(TargetNpc.ModelID, 0);
        if (null == charModel)
        {
            return;
        }

        if (m_NpcSprite && m_NpcName)
        {
            m_NpcName.text = roleBase.Name;
            m_NpcSprite.mainTexture = ResourceManager.LoadResource("Texture/MissionRole/" + charModel.NPCSpriteName, typeof(Texture)) as Texture; 
            //m_NpcSprite.spriteName = charModel.NPCSpriteName;
        }
    }

    // 接任务和交任务UI
    public void MissionUI(int nMissionID, MissionUIType type)
    {
        // 先清理
        CleanUp();

        if (nMissionID < 0)
        {
            return;
        }
        m_CurMissionID = nMissionID;
        Tab_MissionDictionary MDLine = TableManager.GetMissionDictionaryByID(nMissionID, 0);
        if (MDLine == null)
        {
            return;
        }

        m_MissionType = type;
        if (type == MissionUIType.TYPE_ACCETP)
        {
            m_AcceptButton.SetActive(true);
            m_CompleteButton.SetActive(false);
            m_MissionInfoAward.SetActive(true);

            if (m_MissionConent)
            {
                m_MissionConent.text = StrDictionary.GetClientString_WithNameSex(MDLine.MissionDesc);
            }
        }
        else if (type == MissionUIType.TYPE_COMPLETE)
        {
            m_AcceptButton.SetActive(false);
            m_CompleteButton.SetActive(true);
            m_MissionInfoAward.SetActive(true);

            if (m_MissionConent)
            {
                m_MissionConent.text = StrDictionary.GetClientString_WithNameSex(MDLine.MissionDoneDesc);
            }
        }

        Tab_MissionBase misLine = TableManager.GetMissionBaseByID(nMissionID, 0);
        if (misLine.BonusID > -1)
        {
            //显示奖励内容
            Tab_MissionBonus bonusTab = TableManager.GetMissionBonusByID(misLine.BonusID, 0);
            if (bonusTab != null)
            {
                
	            //经济系统奖励
	            int nIndex = 0;

	            //如果有经验
	            if (bonusTab.Exp > 0 && nIndex < MaxItemNum && m_MissionEconomicAwardItem[nIndex] && m_MissionEconomicAwardText[nIndex] != null && m_MissionEconomicAwardSprite[nIndex] != null)
	            {
	                m_MissionEconomicAwardItem[nIndex].SetActive(true);
	                m_MissionEconomicAwardText[nIndex].gameObject.SetActive(true);
	                m_MissionEconomicAwardText[nIndex].text = bonusTab.Exp.ToString();
	                m_MissionEconomicAwardSprite[nIndex].gameObject.SetActive(true);
	                m_MissionEconomicAwardSprite[nIndex].spriteName = "jingyan";
	                nIndex++;
	            }

	            //如果有金币
	            if (bonusTab.Money > 0 && nIndex < MaxItemNum && m_MissionEconomicAwardItem[nIndex]&& m_MissionEconomicAwardText[nIndex] != null && m_MissionEconomicAwardSprite[nIndex] != null)
	            {
	                m_MissionEconomicAwardItem[nIndex].SetActive(true);
	                m_MissionEconomicAwardText[nIndex].gameObject.SetActive(true);
	                m_MissionEconomicAwardText[nIndex].text = bonusTab.Money.ToString();
	                m_MissionEconomicAwardSprite[nIndex].gameObject.SetActive(true);
	                m_MissionEconomicAwardSprite[nIndex].spriteName = "jinbi";
	                nIndex++;
	            }

	            //如果有绑定元宝
	            if (bonusTab.BindYuanBao > 0 && nIndex < MaxItemNum && m_MissionEconomicAwardItem[nIndex] && m_MissionEconomicAwardText[nIndex] != null && m_MissionEconomicAwardSprite[nIndex] != null)
	            {
	                m_MissionEconomicAwardItem[nIndex].SetActive(true);
	                m_MissionEconomicAwardText[nIndex].gameObject.SetActive(true);
	                m_MissionEconomicAwardText[nIndex].text = bonusTab.BindYuanBao.ToString();
	                m_MissionEconomicAwardSprite[nIndex].gameObject.SetActive(true);
	                m_MissionEconomicAwardSprite[nIndex].spriteName = "yuanbao";
	                nIndex++;
	            }
	                
		        // 显示物品
		        int nItemShowCount = 0;
		        for (int i = 0; i < bonusTab.getToolIDCount(); i++)
		        {
		            Tab_CommonItem commonItem = TableManager.GetCommonItemByID(bonusTab.GetToolIDbyIndex(i), 0);
		            if (commonItem != null && bonusTab.GetToolNumbyIndex(i) > 0)
		            {
		                if (nItemShowCount < MaxItemNum && m_MissionBonusItem[nItemShowCount] && m_MissionBonusItemNum[nItemShowCount] && m_MissionBonusItemSprite[nItemShowCount] && m_MissionBonusItemQuality[nItemShowCount])
		                {
		                    m_MissionBonusItemID[nItemShowCount] = commonItem.Id;
		                    m_MissionBonusItem[nItemShowCount].SetActive(true);
		                    m_MissionBonusItemNum[nItemShowCount].text = bonusTab.GetToolNumbyIndex(i).ToString();
		                    m_MissionBonusItemSprite[nItemShowCount].spriteName = commonItem.Icon;
		                    m_MissionBonusItemQuality[nItemShowCount].spriteName = GlobeVar.QualityColorGrid[commonItem.Quality - 1];
		                    nItemShowCount++;
		                }
		            }
		        }

		        int nProfess = Singleton<ObjManager>.GetInstance().MainPlayer.Profession;
		        if (nProfess >= 0 && nProfess < bonusTab.getProfessionItemIDCount())
		        {
		            if (ShowSpecialMissionItem(nMissionID, bonusTab.GetProfessionItemIDbyIndex(nProfess), nItemShowCount))
		            {
		                nItemShowCount++;
		            }
		            else
		            {

		                Tab_CommonItem commonItem = TableManager.GetCommonItemByID(bonusTab.GetProfessionItemIDbyIndex(nProfess), 0);
		                if (commonItem != null)
		                {
		                    if (nItemShowCount < MaxItemNum && m_MissionBonusItem[nItemShowCount] && m_MissionBonusItemNum[nItemShowCount] && m_MissionBonusItemSprite[nItemShowCount] && m_MissionBonusItemQuality[nItemShowCount])
		                    {
		                        m_MissionBonusItemID[nItemShowCount] = commonItem.Id;
		                        m_MissionBonusItem[nItemShowCount].SetActive(true);
		                        m_MissionBonusItemNum[nItemShowCount].text = bonusTab.GetProfessionNumbyIndex(nProfess).ToString();
		                        m_MissionBonusItemSprite[nItemShowCount].spriteName = commonItem.Icon;
		                        m_MissionBonusItemQuality[nItemShowCount].spriteName = GlobeVar.QualityColorGrid[commonItem.Quality - 1];
		                        nItemShowCount++;
		                    }
		                }
		            }
	       		}

		        // 多选物品
		        int nChoosedItemdNum = bonusTab.ChoosedItemNum;
		        if (nChoosedItemdNum > 0 && nChoosedItemdNum <= bonusTab.getItemIDCount())
		        {
		            Tab_CommonItem commonItem = TableManager.GetCommonItemByID(bonusTab.GetItemIDbyIndex(nChoosedItemdNum-1), 0);
		            if (commonItem != null && bonusTab.GetItemNumbyIndex(nChoosedItemdNum-1) > 0)
		            {
		                if (nItemShowCount < MaxItemNum && m_MissionBonusItem[nItemShowCount] && m_MissionBonusItemNum[nItemShowCount] && m_MissionBonusItemSprite[nItemShowCount] && m_MissionBonusItemQuality[nItemShowCount])
		                {
		                    m_MissionBonusItemID[nItemShowCount] = commonItem.Id;
		                    m_MissionBonusItem[nItemShowCount].SetActive(true);
		                    m_MissionBonusItemNum[nItemShowCount].text = bonusTab.GetItemNumbyIndex(nChoosedItemdNum - 1).ToString();
		                    m_MissionBonusItemSprite[nItemShowCount].spriteName = commonItem.Icon;
		                    m_MissionBonusItemQuality[nItemShowCount].spriteName = GlobeVar.QualityColorGrid[commonItem.Quality - 1];
		                    nItemShowCount++;
		                }
		            }
		        }

		        if (m_MissionItemGrid)
		        {
		            m_MissionItemGrid.repositionNow = true;
		        }
            }
        }

        // Npc半身像
        NpcDialogUI();

        GameManager.gameManager.SoundManager.PlaySoundEffect(128);
    }

    /// <summary>
    /// 接受任务
    /// </summary>
    void MissionAccept()
    {
        NewPlayerGuidLogic.CloseWindow();
        UIManager.CloseUI(UIInfo.MissionInfoController);
        GameManager.gameManager.SoundManager.PlaySoundEffect(135);
        GameManager.gameManager.MissionManager.AcceptMission(m_CurMissionID);
    }
    /// <summary>
    /// 完成任务
    /// </summary>
    void MissionComplete()
    {
        NewPlayerGuidLogic.CloseWindow();
        UIManager.CloseUI(UIInfo.MissionInfoController);
        GameManager.gameManager.SoundManager.PlaySoundEffect(135);
        GameManager.gameManager.MissionManager.CompleteMission(m_CurMissionID);

    }

    void OnCloseClick()
    {
        UIManager.CloseUI(UIInfo.MissionInfoController);

        if (m_MissionType == MissionUIType.TYPE_ACCETP && GameManager.gameManager.MissionManager.CanDoAcceptMission(m_CurMissionID))
        {
            GameManager.gameManager.MissionManager.AcceptMission(m_CurMissionID);
        }
    }

    void ItemTipClick(GameObject value)
    {
        int nItemID = -1;
        for (int i = 0; i < MaxItemNum;i++ )
        {
            if (value.name == m_MissionBonusItem[i].name)
            {
                nItemID = m_MissionBonusItemID[i];
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
            SpecialItemClick(item);
            EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Info);
        }
        else
        {
            ItemTooltipsLogic.ShowItemTooltip(item, ItemTooltipsLogic.ShowType.Info);
        }
    }

    // 临时加吧，物品不支持，没办法
    bool ShowSpecialMissionItem(int nMissionID, int nItemID, int nItemShowCount)
    {
        if (nMissionID == 233 || nMissionID == 234 || nMissionID == 235)
        {
            Tab_CommonItem commonItem = TableManager.GetCommonItemByID(nItemID, 0);
            if (commonItem != null)
            {
                if (nItemShowCount < MaxItemNum && m_MissionBonusItem[nItemShowCount] && m_MissionBonusItemNum[nItemShowCount] && m_MissionBonusItemSprite[nItemShowCount] && m_MissionBonusItemQuality[nItemShowCount])
                {
                    m_MissionBonusItemID[nItemShowCount] = commonItem.Id;
                    m_MissionBonusItem[nItemShowCount].SetActive(true);
                    m_MissionBonusItemNum[nItemShowCount].text = "1";
                    m_MissionBonusItemSprite[nItemShowCount].spriteName = commonItem.Icon;
                    m_MissionBonusItemQuality[nItemShowCount].spriteName = GlobeVar.QualityColorGrid[commonItem.Quality - 1];
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
    
}
