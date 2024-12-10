using UnityEngine;
using System.Collections;
using Module.Log;
using GCGame;
using Games.DailyMissionData;
using GCGame.Table;

public class DailyMissionWindow : MonoBehaviour {

    public UIGrid       m_ItemGrid;
    public UILabel      m_ActivenessTip;
    public UILabel      m_DoneCountTip;
    public GameObject m_DailyMissionItem;

	private static DailyMissionWindow m_Instance = null;
	public static DailyMissionWindow Instance()
	{
		return m_Instance;
	}

    private int m_nDoneCount;
    public int DoneCount
    {
        get { return m_nDoneCount; }
        set
        {
            m_nDoneCount = value;
            UpDateDoneCountTip();
        }
    }

    private int m_nActiveness;
    public int Activeness
    {
        get { return m_nActiveness; }
        set { 
            m_nActiveness = value;
            UpDateActivenessTip();
        }
    }

    // 新手指引
    private int m_NewPlayerGuide_Step = -1;
    public int NewPlayerGuide_Step
    {
        get { return m_NewPlayerGuide_Step; }
        set { m_NewPlayerGuide_Step = value; }
    }

	void Awake()
	{
		m_Instance = this;
	}

    // 界面加载后调用
    void Start()
    {
		UpDateMissionList();
    }

    void OnEnable()
    {
        UpDateMissionList();
    }

    void Init()
    {
        DoneCount = GameManager.gameManager.PlayerDataPool.DailyMissionData.DailyMissionDoneCount;
        Activeness = GameManager.gameManager.PlayerDataPool.AwardActivityData.Activeness;
    }

    void CleanUp()
    {
        if (m_ItemGrid && m_ActivenessTip && m_DoneCountTip)
        {
            Utils.CleanGrid(m_ItemGrid.gameObject);
            m_ActivenessTip.text = "";
            m_DoneCountTip.text = "";
        }

        DoneCount = 0;
        Activeness = 0;
    }

    // 创建奖励list
    void CreateItemList()
    {
        //UIManager.LoadItem(UIInfo.DailyMissionItem, OnLoadItemList);
        OnLoadItemList(m_DailyMissionItem);
    }

    void OnLoadItemList(GameObject DailyMissionItem)
    {
        if (null == DailyMissionItem)
        {
            return;
        }

        if (m_ItemGrid == null || m_ItemGrid.gameObject == null)
        {
            return;
        }
        m_ItemGrid.GetComponent<UITopGrid>().recenterTopNow = true;
        int nMaxKinds = DailyMissionData.MAX_DAILYMISSION_KINDS;
        for (int i = 0; i < nMaxKinds; i++)
        {
            DailyMission rDailyMission = GameManager.gameManager.PlayerDataPool.DailyMissionData.GetDailyMissionByKind(i);

            int nMissionID = rDailyMission.MissionID;
            byte yQuality = rDailyMission.Quality;

            Tab_MissionBase MissionBase = TableManager.GetMissionBaseByID(nMissionID, 0);
            if (MissionBase == null)
            {
                continue;
            }

            // 创建Item
            GameObject curItem = Utils.BindObjToParent(DailyMissionItem, m_ItemGrid.gameObject);
            if (curItem == null)
            {
                LogModule.DebugLog("AwardActivityItem create error");
                continue;
            }

            curItem.name = i.ToString();
            DailyMissionItemLogic AwardItem = curItem.GetComponent<DailyMissionItemLogic>();
            if (AwardItem != null)
            {
                AwardItem.Init(i, nMissionID, yQuality);
            }

            UpdateMissionState(nMissionID);
        }

        m_ItemGrid.repositionNow = true;
        m_ItemGrid.sorted = true;
    }

    void UpDateDoneCountTip()
    {
        if (m_DoneCountTip)
        {
            m_DoneCountTip.text = StrDictionary.GetClientDictionaryString("#{1633}", m_nDoneCount);
        }
    }

    void UpDateActivenessTip()
    {
        if (m_ActivenessTip)
        {
            m_ActivenessTip.text = StrDictionary.GetClientDictionaryString("#{1632}", m_nActiveness);
        }
    }

    // button
    void UpDateAllMission()
    {
        if (Singleton<ObjManager>.Instance.MainPlayer.IsInJianYu())
        {
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2216}");
            return;
        }

        YuanBaoTipBox();
    }

    void YuanBaoTipBox()
    {
        int nTotalYuanBao = 0;
        for (int i = 0; i < DailyMissionData.MAX_DAILYMISSION_KINDS; i++)
        {
            DailyMission rMission = GameManager.gameManager.PlayerDataPool.DailyMissionData.GetDailyMissionByKind(i);
			int nMissionID = rMission.MissionID;
			if (nMissionID <= 0)
            {
                continue;
            }
			bool IsHaveMission = GameManager.gameManager.MissionManager.IsHaveMission(nMissionID);
            if (IsHaveMission)
            {
                continue;
            }
			Tab_MissionBase MissionBase = TableManager.GetMissionBaseByID(nMissionID, 0);
            if (MissionBase == null)
            {
                return;
            }
            Tab_DailyMission DailyMission = TableManager.GetDailyMissionByID(MissionBase.DalityMissionTabID, 0);
            if (DailyMission == null)
            {
                return;
            }
            nTotalYuanBao += DailyMission.ConsumeYuanBao;
        }
		if (nTotalYuanBao == 0) 
		{
			Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{5658}");
			return ;		
		}
        string str = "";
        str = StrDictionary.GetClientDictionaryString("#{1538}", nTotalYuanBao);
        MessageBoxLogic.OpenOKCancelBox(str, null, OnMessageOK, OnCancelClick);
    }

    void OnMessageOK()
    {
        GameManager.gameManager.PlayerDataPool.DailyMissionData.AskUpdateDailyMission(DailyMissionData.MAX_DAILYMISSION_KINDS);
    }

    void OnCancelClick()
    {
        MessageBoxLogic.CloseBox();
    }

    void AcceptAllMission()
    {
        if (Singleton<ObjManager>.Instance.MainPlayer.IsInJianYu())
        {
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2216}");
            return;
        }

        int nMaxKinds = DailyMissionData.MAX_DAILYMISSION_KINDS;
        for (int i = 0; i < nMaxKinds; i++)
        {
            DailyMission rDailyMission = GameManager.gameManager.PlayerDataPool.DailyMissionData.GetDailyMissionByKind(i);

            int nMissionID = rDailyMission.MissionID;
            //byte yQuality = rDailyMission.Quality;

            Tab_MissionBase MissionBase = TableManager.GetMissionBaseByID(nMissionID, 0);
            if (MissionBase == null)
            {
                continue;
            }

            bool isHaveMission = GameManager.gameManager.MissionManager.IsHaveMission(nMissionID);
            if (isHaveMission)
            {
                continue;
            }

            GameManager.gameManager.MissionManager.AcceptMission(nMissionID);
        }
    }

    public void UpdateMissionState(int nMissionID)
    {
        DailyMissionItemLogic[] Item = m_ItemGrid.GetComponentsInChildren<DailyMissionItemLogic>();
        for (int i = 0; i < Item.Length; ++i)
        {
            if (Item[i] && Item[i].MissionID == nMissionID)
            {
                Item[i].UpadateButtonState(nMissionID);
                break;
            }
        }
        if (m_ItemGrid)
        {
            m_ItemGrid.repositionNow = true;
            m_ItemGrid.sorted = true;
        }
    }

    public void UpDateMissionList()
    {
        CleanUp();
        Init();
        CreateItemList();
    }

    public void UpdateMissionItemByKind(int nKind)
    {
        DailyMissionItemLogic[] Item = m_ItemGrid.GetComponentsInChildren<DailyMissionItemLogic>();
        for (int i = 0; i < Item.Length; ++i)
        {
            if (Item[i].Kind == nKind)
            {
                DailyMission missionItem = GameManager.gameManager.PlayerDataPool.DailyMissionData.GetDailyMissionByKind(nKind);
                if (missionItem.MissionID <= Games.GlobeDefine.GlobeVar.INVALID_ID)
                {
                    Item[i].gameObject.SetActive(false);
                    break;
                }
                Item[i].Init(nKind, missionItem.MissionID, missionItem.Quality);
                return;
            }
            if (m_ItemGrid)
            {
                m_ItemGrid.repositionNow = true;
            }
        }
    }

    // 新手指引
    public void NewPlayerGuide(int nIndex)
    {
        m_NewPlayerGuide_Step = nIndex;
        switch (nIndex)
        {
            case 1:
                break;
        }
    }
}