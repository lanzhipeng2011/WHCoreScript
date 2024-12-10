using UnityEngine;
using System.Collections;
using GCGame;
using System.Collections.Generic;
using Games.GlobeDefine;
using Games.UserCommonData;

public class ActivityController : UIControllerBase<ActivityController>
{
    // 新手指引
    public GameObject m_ButtonFight;
    //For Tab Tips
    private enum TabIndex
    {
        Tab_DailyMission, //1日常任务
        Tab_QunXiong,  //2江湖名人录
		Tab_HuLaoGuan,    //3聚贤庄11 虎牢关
		Tab_TianXiaWuShuang,   //4华山论剑 天下无双
		Tab_WuShengTa,    //5藏经阁14 武神塔
		Tab_HuSongMeiRen,    //6燕子坞19 护送美人
		Tab_QiXingXuanZhen,   //7珍珑棋局28 七星玄镇
		Tab_YeXiDaYin,   //8燕王古墓27 夜袭大营
        Tab_TJQJ,   //9天降奇珍        
		Tab_FengHuoLianTian,    //10少室山31 烽火连天
        Tab_GuildWar,  //11帮战 
        Tab_Boss,   //12世界BOSS
		Tab_GuoGuanZhanJiang,   //13怒海锄奸7 过关斩将
    };
    public List<UISprite> m_TabCounts = new List<UISprite>();
    public List<UISprite> m_TabTeam = new List<UISprite>();
    public GameObject m_PVPWindow;
    public DungeonWindow m_DungeonWindow;
    public DailyMissionActiveWindow m_DailyMissionActiveWindow;
    public UISprite m_DailyTip;
    public UISprite m_AwardTip;
    public HuaShanPvPWindow m_HuaShanWindow;
    public TabController m_TabDungeon;      // 副本选项
	public TabController m_TabController;
    public TeamPlatformWindow m_TeamPlatformWindow;
    public CangJingGeWindow CangJingGeWindow;
    public UITopGrid m_TabUITopGrid;

	private string countNameStr = "ui_pub_029";

    private int m_NewPlayerGuide_Step = -1;
    public int NewPlayerGuide_Step
    {
        get { return m_NewPlayerGuide_Step; }
        set { m_NewPlayerGuide_Step = value; }
    }

    private static bool m_bHasStartTab = false;     // 是否需要在初始化时显示一个指定Tab
    private static string m_strStartTabName = "";   // 初始化指定Tab名称
    public static void SetStartTab(string tabName)
    {
        m_bHasStartTab = true;
        m_strStartTabName = tabName;
    }

    public static void TestStartTab(TabController curTabController)
    {
        if(!m_bHasStartTab)
        {
            return;
        }
        m_bHasStartTab = false;
        if (null != curTabController)
        {
            curTabController.ChangeTab(m_strStartTabName);
        }
    }
    void Awake()
    {
        SetInstance(this);
        m_TabDungeon.delTabChanged = OnTabDungeonTableau;
    }

	// Use this for initialization
	void OnEnable()
    {
         if (null != m_TabController)
         {
             m_TabController.ChangeTab("Tab1");
         }
        SetInstance(this);

        InitData();
        
        TestStartTab(m_TabController);
        UpdateTabTips();
        UpdateAutoTeam();
        
        Check_OnChangeTab();

        Check_NewPlayerGuide();
	}
    void OnDisable()
    {
        SetInstance(null);
    }

    void InitData()
    {
        if (m_TabController != null)
        {
            m_TabController.InitData();
            m_TabUITopGrid.transform.parent.GetComponent<UIDraggablePanel>().scale = new Vector3(0,1,0);
        }
    }

    public void UpdateTabTips()
    {
        m_TabUITopGrid.recenterTopNow = true;
        string temp = "";
        for (int nindex = 0; nindex < m_TabCounts.Count; ++nindex )
        {
            m_TabCounts[nindex].spriteName = "";
        }
        //1日常
        {
            m_DailyTip.spriteName = "";
            m_AwardTip.spriteName = "";
            if (Utils.GetActivityAwardBonusLeft() > 0)
            {
				m_TabCounts[(int)TabIndex.Tab_DailyMission].spriteName = countNameStr;
				m_AwardTip.spriteName = countNameStr;
            }
        }

        //等级显示限制
        int nLevel = 0;
        if (Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            nLevel = Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.Level;
        }

        //2江湖名人录
        {
            if (PVPData.LeftFightTime > 0 && nLevel >= 20)
            {
				m_TabCounts[(int)TabIndex.Tab_QunXiong].spriteName = countNameStr;
            }
        }

        if (nLevel < 20)
            return;

        //3聚贤庄11
        {
            int nCount = 0;
            int nCur = 0;
            int nMax = 0;
			Utils.GetCopySceneCountsAll((int)GameDefine_Globe.SCENE_DEFINE.SCENE_HULAOGUAN, ref nCur, ref nMax);
            nCount += nCur;
            temp += " Tab_JXZ:" + nCount.ToString();
            if ( nCount > 0 )
				m_TabCounts[(int)TabIndex.Tab_HuLaoGuan].spriteName = countNameStr;
        }
        //5藏经阁14
        {
            int nCount = 0;
            int nCur = 0;
            int nMax = 0;
			Utils.GetCopySceneCounts((int)GameDefine_Globe.SCENE_DEFINE.SCENE_WUSHENTA, 1, 1, ref nCur, ref nMax);
            nCount += nCur;
            Utils.GetSweepCounts(ref nCur, ref nMax);
            nCount += nCur;

            temp += " Tab_CJG:" + nCount.ToString();
            if (nCount > 0)
				m_TabCounts[(int)TabIndex.Tab_WuShengTa].spriteName = countNameStr;
        }
        //6燕子坞19
        {
            int nCount = 0;
            int nCur = 0;
            int nMax = 0;
			Utils.GetCopySceneCountsAll((int)GameDefine_Globe.SCENE_DEFINE.SCENE_HUSONGMEIREN, ref nCur, ref nMax);
            nCount += nCur;
            temp += " Tab_YZW:" + nCount.ToString();
            if (nCount > 0)
				m_TabCounts[(int)TabIndex.Tab_HuSongMeiRen].spriteName = countNameStr;
        }
        //7珍珑棋局28
        {
            int nCount = 0;
            int nCur = 0;
            int nMax = 0;
			Utils.GetCopySceneCountsAll((int)GameDefine_Globe.SCENE_DEFINE.SCENE_QIXINGXUANZHEN, ref nCur, ref nMax);
            nCount += nCur;
            temp += " Tab_ZLQJ:" + nCount.ToString();
            if (nCount > 0)
				m_TabCounts[(int)TabIndex.Tab_QiXingXuanZhen].spriteName = countNameStr;
        }
        //8燕王古墓27
        {
            int nCount = 0;
            int nCur = 0;
            int nMax = 0;
			Utils.GetCopySceneCountsAll((int)GameDefine_Globe.SCENE_DEFINE.SCENE_YEXIDAYING, ref nCur, ref nMax);
            nCount += nCur;
            temp += " Tab_YWGM:" + nCount.ToString();
            if (nCount > 0)
				m_TabCounts[(int)TabIndex.Tab_YeXiDaYin].spriteName = countNameStr;
        }
        //9 天降奇珍
        {
            bool bFlag = GameManager.gameManager.PlayerDataPool.CommonData.GetCommondFlag((int)USER_COMMONFLAG.CF_GUILDACTIVITY_FLAG);
            if (bFlag)
            {
				m_TabCounts[(int)TabIndex.Tab_TJQJ].spriteName = countNameStr;
            }
        }
        //13怒海锄奸7
        {
            int nCount = 0;
            int nCur = 0;
            int nMax = 0;
			Utils.GetCopySceneCounts((int)GameDefine_Globe.SCENE_DEFINE.SCENE_GUOGUANZHANJIANG, 1, 1, ref nCur, ref nMax);
            nCount += nCur;
            temp += " Tab_NHCJ:" + nCount.ToString();
            if (nCount > 0)
				m_TabCounts[(int)TabIndex.Tab_GuoGuanZhanJiang].spriteName = countNameStr;
        }
        //10少室山31
        {
            // 少室山等级要求
            if (Singleton<ObjManager>.Instance.MainPlayer.BaseAttr.Level >= 70)
            {
                int nCount = 0;
                int nCur = 0;
                int nMax = 0;
				Utils.GetCopySceneCounts((int)GameDefine_Globe.SCENE_DEFINE.SCENE_FENGHUOLIANTIAN, 2, 1, ref nCur, ref nMax);
                nCount += nCur;
                temp += " Tab_SSS:" + nCount.ToString();
                if (nCount > 0)
					m_TabCounts[(int)TabIndex.Tab_FengHuoLianTian].spriteName = countNameStr;
            }            
        }
        
       // Debug.Log(temp);
        
    }
    public void UpdateAutoTeam()
    {
        //做自动组队处理
        if (GameManager.gameManager.PlayerDataPool.AutoTeamState == true)
        {
            if (GameManager.gameManager.PlayerDataPool.AutoTeamCopySceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_HULAOGUAN)
            {
                m_TabCounts[(int)TabIndex.Tab_HuLaoGuan].gameObject.SetActive(false);
                m_TabTeam[(int)TabIndex.Tab_HuLaoGuan].gameObject.SetActive(true);
            }
            else if (GameManager.gameManager.PlayerDataPool.AutoTeamCopySceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_HUSONGMEIREN)
            {
                m_TabCounts[(int)TabIndex.Tab_HuSongMeiRen].gameObject.SetActive(false);
                m_TabTeam[(int)TabIndex.Tab_HuSongMeiRen].gameObject.SetActive(true);
            }
            else if (GameManager.gameManager.PlayerDataPool.AutoTeamCopySceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_YEXIDAYING)
            {
                m_TabCounts[(int)TabIndex.Tab_YeXiDaYin].gameObject.SetActive(false);
                m_TabTeam[(int)TabIndex.Tab_YeXiDaYin].gameObject.SetActive(true);
            }
            else if (GameManager.gameManager.PlayerDataPool.AutoTeamCopySceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_QIXINGXUANZHEN)
            {
                m_TabCounts[(int)TabIndex.Tab_QiXingXuanZhen].gameObject.SetActive(false);
                m_TabTeam[(int)TabIndex.Tab_QiXingXuanZhen].gameObject.SetActive(true);
            }
            else if (GameManager.gameManager.PlayerDataPool.AutoTeamCopySceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_FENGHUOLIANTIAN)
            {
                m_TabCounts[(int)TabIndex.Tab_FengHuoLianTian].gameObject.SetActive(false);
                m_TabTeam[(int)TabIndex.Tab_FengHuoLianTian].gameObject.SetActive(true);
            }

        }
        else
        {
            if (GameManager.gameManager.PlayerDataPool.AutoTeamCopySceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_HULAOGUAN)
            {
                m_TabCounts[(int)TabIndex.Tab_HuLaoGuan].gameObject.SetActive(true);
                m_TabTeam[(int)TabIndex.Tab_HuLaoGuan].gameObject.SetActive(false);
            }
            else if (GameManager.gameManager.PlayerDataPool.AutoTeamCopySceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_HUSONGMEIREN)
            {
                m_TabCounts[(int)TabIndex.Tab_HuSongMeiRen].gameObject.SetActive(true);
                m_TabTeam[(int)TabIndex.Tab_HuSongMeiRen].gameObject.SetActive(false);
            }
            else if (GameManager.gameManager.PlayerDataPool.AutoTeamCopySceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_YEXIDAYING)
            {
                m_TabCounts[(int)TabIndex.Tab_YeXiDaYin].gameObject.SetActive(true);
                m_TabTeam[(int)TabIndex.Tab_YeXiDaYin].gameObject.SetActive(false);
            }
            else if (GameManager.gameManager.PlayerDataPool.AutoTeamCopySceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_QIXINGXUANZHEN)
            {
                m_TabCounts[(int)TabIndex.Tab_QiXingXuanZhen].gameObject.SetActive(true);
                m_TabTeam[(int)TabIndex.Tab_QiXingXuanZhen].gameObject.SetActive(false);
            }
            else if (GameManager.gameManager.PlayerDataPool.AutoTeamCopySceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_FENGHUOLIANTIAN)
            {
                m_TabCounts[(int)TabIndex.Tab_FengHuoLianTian].gameObject.SetActive(true);
                m_TabTeam[(int)TabIndex.Tab_FengHuoLianTian].gameObject.SetActive(false);
            }
        }
    }
	void OnCloseClick()
	{
		UIManager.CloseUI(UIInfo.Activity);
	}

    // 新手指引
    void Check_NewPlayerGuide()
    {
        if (FunctionButtonLogic.Instance())
        {
            int nStep = FunctionButtonLogic.Instance().NewPlayerGuide_Step;
            if (nStep > 0)
            {
				NewPlayerGuide(nStep);
                FunctionButtonLogic.Instance().NewPlayerGuide_Step = -1;
            }
        }
    }
    public void NewPlayerGuide(int nIndex)
    {
		if (nIndex < 0) 
		{
			return;		
		}

		NewPlayerGuidLogic.CloseWindow();

        m_NewPlayerGuide_Step = nIndex;

        switch (nIndex)
        {
		case (int)GameDefine_Globe.NEWOLAYERGUIDE.ACTIVITY_MISSION:
			m_NewPlayerGuide_Step = -1;
			break;
		case (int)GameDefine_Globe.NEWOLAYERGUIDE.ACTIVITY_WUSHENTA:// 聚贤庄
			TabButton tab5 = m_TabController.GetTabButton("Tab5");
			if (tab5)
			{
				NewPlayerGuidLogic.OpenWindow(tab5.gameObject, 337, 139, "", "right", 2, true, true);
			}
			break;
		case (int)GameDefine_Globe.NEWOLAYERGUIDE.ACTIVITY_QIXINGXUANZHEN:// 藏经阁
			TabButton tab7 = m_TabController.GetTabButton("Tab7");
			if (tab7)
			{
				NewPlayerGuidLogic.OpenWindow(tab7.gameObject, 337, 139, "", "right", 2, true, true);
			}
			break;
		case (int)GameDefine_Globe.NEWOLAYERGUIDE.ACTIVITY_HULAOGUAN:// 燕子坞
			TabButton tab3 = m_TabController.GetTabButton("Tab3");
			if (tab3)
			{
				NewPlayerGuidLogic.OpenWindow(tab3.gameObject, 337, 139, "", "right", 2, true, true);
			}
			break;
		case (int)GameDefine_Globe.NEWOLAYERGUIDE.ACTIVITY_HUSONGMEIREN:// 珍珑棋局
			if (null != m_TabController)
			{
				m_TabController.ChangeTab("Tab6");
			}
			break;
		case (int)GameDefine_Globe.NEWOLAYERGUIDE.ACTIVITY_GUOGUANZHANJIANG:// 珍珑棋局
			if (null != m_TabController)
			{
				m_TabController.ChangeTab("Tab13");
			}
			break;
		case (int)GameDefine_Globe.NEWOLAYERGUIDE.ACTIVITY_QUNXIONG:
			TabButton tab2 = m_TabController.GetTabButton("Tab2");
			if (tab2)
			{
				NewPlayerGuidLogic.OpenWindow(tab2.gameObject, 337, 139, "", "right", 2, true, true);
			}
	        break;
		default:
			break;
        }
           
        // m_TabUITopGrid.transform.parent.GetComponent<UIDraggablePanel>().scale = Vector3.zero;
        
    }

#region 华山论贱
    public TabController m_HuaShanTabController;

    public void ChangeToHSPvPShowMemberList( )
    {
        if (null != m_HuaShanTabController)
            m_HuaShanTabController.ChangeTab("Tab2");
    }

    public void ChangeToHSPvPShowDefault()
    {
        if (null != m_HuaShanTabController)
            m_HuaShanTabController.ChangeTab("Tab1");
    }
    public void ChangeToHSPvPShowOppoentView( )
    {
        if (null != m_HuaShanTabController)
            m_HuaShanTabController.ChangeTab("Tab3");
    }

    public void ChangeToHSPvPShowPKInfo()
    {
        if (null != m_HuaShanTabController)
            m_HuaShanTabController.ChangeTab("Tab4");
    }

    public void ChangeToHuaShanTab( )
    {
        if (null != m_TabController)
            m_TabController.ChangeTab("Tab4");
    }

    public bool IsShowHuaShanTab()
    {
        GameObject curTab = m_TabController.GetHighlightTab().gameObject;
        if (curTab.name == "Tab4")
        {
            return true;
        }
        return false;
    }
#endregion

    public void ChangeToPvP()
    {
        if (null != m_TabController)
            m_TabController.ChangeTab("Tab2");
    }

    void OnTabDungeonTableau(TabButton button)
    {
        if (null != m_TeamPlatformWindow)
        {
            m_TeamPlatformWindow.gameObject.SetActive(false);
        }
        
        if (button.name == "Tab3")
        {
			m_DungeonWindow.OnOpenCopyScene((int)GameDefine_Globe.SCENE_DEFINE.SCENE_HULAOGUAN);
        }
        else if (button.name == "Tab1")
        {
			m_DailyMissionActiveWindow.m_DailyMissionTabController.ChangeTab("DailyMission");
//             if (m_NewPlayerGuide_Step == 2)
//             {
//                 NewPlayerGuidLogic.CloseWindow();
//                 m_DailyMissionActiveWindow.NewPlayerGuide(1);
//                 m_NewPlayerGuide_Step = -1;
//             }
        }
        else if (button.name == "Tab4")
        {
            m_HuaShanWindow.gameObject.SetActive(true);
            CG_ASK_HUASHANPVP_STATE packet = (CG_ASK_HUASHANPVP_STATE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_HUASHANPVP_STATE);
            packet.None = 0;
            packet.SendPacket();
            //.. 请求数据
        }
        else if (button.name == "Tab5")
        {
			CangJingGeWindow.OnOpenCopyScene((int)GameDefine_Globe.SCENE_DEFINE.SCENE_WUSHENTA);
			if(m_NewPlayerGuide_Step == (int)GameDefine_Globe.NEWOLAYERGUIDE.ACTIVITY_WUSHENTA)
			{
				CangJingGeWindow.Instance().NewPlayerGuide();
				m_NewPlayerGuide_Step = -1;
			}
        }
        else if (button.name == "Tab6")
        {
			m_DungeonWindow.OnOpenCopyScene((int)GameDefine_Globe.SCENE_DEFINE.SCENE_HUSONGMEIREN);
        }
        else if (button.name == "Tab7")
        {
			m_DungeonWindow.OnOpenCopyScene((int)GameDefine_Globe.SCENE_DEFINE.SCENE_QIXINGXUANZHEN);
        }
        else if (button.name == "Tab8")
        {
			m_DungeonWindow.OnOpenCopyScene((int)GameDefine_Globe.SCENE_DEFINE.SCENE_YEXIDAYING);
        }
        else if (button.name == "Tab10")
        {
			m_DungeonWindow.OnOpenCopyScene((int)GameDefine_Globe.SCENE_DEFINE.SCENE_FENGHUOLIANTIAN);
        }
        else if (button.name == "Tab13")
        {
			//过关斩将
			m_DungeonWindow.OnOpenCopyScene((int)GameDefine_Globe.SCENE_DEFINE.SCENE_GUOGUANZHANJIANG);
        }
    }

    // 日常任务界面相关
    public void UpdateDailyMissionState(int nMissionID)
    {
        if (m_DailyMissionActiveWindow.IsDailyMissionActive() == true)
        {
            m_DailyMissionActiveWindow.UpDateDailyMissionState(nMissionID);
        }
    }
    public void UpDateDoneCount(int nDoneCount)
    {
        if (m_DailyMissionActiveWindow.IsDailyMissionActive() == true)
        {
            m_DailyMissionActiveWindow.UpDateDoneCount(nDoneCount);
        }
    }
    public void UpDateActiveness(int nActiveness)
    {
        if (m_DailyMissionActiveWindow.IsDailyMissionActive() == true)
        {
            m_DailyMissionActiveWindow.UpDateActiveness(nActiveness);
        }
        //about active
        UpdateTabTips();       
    }
    public void UpdateDailyMissionList()
    {
        if (m_DailyMissionActiveWindow.IsDailyMissionActive() == true)
        {
            m_DailyMissionActiveWindow.UpdateMissionList();
        }
    }
    public void UpdateMissionItemByKind(int nKind)
    {
        if (m_DailyMissionActiveWindow.IsDailyMissionActive() ==  true)
        {
            m_DailyMissionActiveWindow.UpdateMissionItemByKind(nKind);
        }
    }

    // 活跃度领奖界面
    public void UpdateAwardItemState(int nTurnID)
    {
        if (m_DailyMissionActiveWindow.IsActivenessWindowActive() == true)
        {
            m_DailyMissionActiveWindow.UpdateAwardItemState(nTurnID);
        }
    }

    // 打开日常任务界面
    public void ChangeToDailyMissionTab()
    {
        if (null != m_TabController)
        {
            m_TabController.ChangeTab("Tab1");
            TabButton tabButton = m_TabController.GetTabButton("Tab1");
            if (tabButton)
            {
                OnTabDungeonTableau(tabButton);
            }

            if (PlayerPreferenceData.DailyMissionGuideFlag != 1)
            {
                m_DailyMissionActiveWindow.NewPlayerGuide(1);
            }
        }
    }

    private string m_StrTabName = "Tab1";
    public string StrTabName
    {
        get { return m_StrTabName; }
        set { 
            m_StrTabName = value;
            Check_OnChangeTab();
        }
    }

    void Check_OnChangeTab()
    {
        if (m_StrTabName != "")
        {
            if (null != m_TabController)
            {
                m_TabController.ChangeTab(m_StrTabName);
            }
			//暂时默认开启tab7
            m_StrTabName = "Tab1";
        }
    }

    // 燕子坞界面
    public void ChangeToHuSongMeiRenTab(bool bSuccess, object param)
    {
        if (null != m_TabController)
        {
            m_TabController.ChangeTab("Tab6");
        }
    }
    // 燕王古墓界面
    public void ChangeToYanWanggumuTab(bool bSuccess, object param)
    {
        if (null != m_TabController)
        {
            m_TabController.ChangeTab("Tab8");
        }
    }
    // 珍珑棋局界面
    public void ChangeToZhenLongQiJuTab(bool bSuccess, object param)
    {
        if (null != m_TabController)
        {
            m_TabController.ChangeTab("Tab7");
        }
    }
    // 聚贤庄界面
    public void ChangeToJuXianZhuangTab(bool bSuccess, object param)
    {
        if (null != m_TabController)
        {
            m_TabController.ChangeTab("Tab3");
        }
    }
	// 聚贤庄界面
	public void ChangeToGuoGuanZhanJiangTab(bool bSuccess, object param)
	{
		if (null != m_TabController)
		{
			m_TabController.ChangeTab("Tab13");
		}
	}
    // 少室山界面
    public void ChangeToShaoShiShanTab(bool bSuccess, object param)
    {
        if (null != m_TabController)
        {
            m_TabController.ChangeTab("Tab10");
        }
    }
    // 藏经阁
    public void ChangeToCangJingGeTab(bool bSuccess, object param)
    {
        if (null != m_TabController)
        {
            m_TabController.ChangeTab("Tab5");
        }
    }
    // 名人录
    public void ChangeToMingRenLuTab(bool bSuccess, object param)
    {
        if (null != m_TabController)
        {
            m_TabController.ChangeTab("Tab2");
        }
    }

    public void UpdateGuildActivityWindow()
    {
        UpdateTabTips();

        if (GuildDailyWindow.Instance())
        {
            GuildDailyWindow.Instance().UpdateActivityInfo();
        }
    }
}
