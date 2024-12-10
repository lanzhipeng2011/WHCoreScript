//********************************************************************
// 文件名: MainUILogic.cs
// 描述: 主界面逻辑 包括人物头像 主菜单等 一般是需要长期显示在屏幕的
// 作者: WangZhe
// 创建时间: 2013-10-31
//
// 修改历史:
// 2013-10-31 王喆创建
// 2013-11-7 给任务追踪和技能增加空的处理函数
// 2013-11-8 增加任务信息界面和空的处理函数
// 2013-11-9 将人物属性和任务信息的打开和关闭 交由UIButton Activate控件控制
// 2013-11-12 增加左侧tabs执行完TweenPosition后的回调函数 更新任务追踪收起按钮的sprite
// 2013-11-13 增加剧情对话UI 将UI制作为Prefab动态加载
// 2013-11-14 把任务相关内容挪到MissionUILogic.cs中
//********************************************************************

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GCGame;
using GCGame.Table;
using Games.Events;
using Games.GlobeDefine;

public class MainUILogic : MonoBehaviour {

    private static MainUILogic m_Instance = null;
    public static MainUILogic Instance()
    {
        return m_Instance;
    }

    private Dictionary<string, GameObject> m_dicUI = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> DicUI
    {
        get { return m_dicUI; }
        set { m_dicUI = value; }
    }

    private Dictionary<string, int> m_dicUILevel = new Dictionary<string, int>();
    public Dictionary<string, int> DicUILevel
    {
        get { return m_dicUILevel; }
        set { m_dicUILevel = value; }
    }
    

    void Awake()
    {
        m_Instance = this;
        
    }
	// Use this for initialization
	void Start () {
        LoadUIPrefab();

        if (NetWorkLogic.GetMe().GetConnectStautus() == NetWorkLogic.ConnectStatus.DISCONNECTED)
        {
            NetManager.Instance().ConnectLost();
        }

        PVPData.CheckAutoShowChallengeUI();
     
	}
	

    void OnDestroy()
    {
        m_Instance = null;
        m_dicUI = null;
        m_dicUILevel = null;
    }
	
    /// <summary>
    /// 加载UIPrefab
    /// </summary>
    void LoadUIPrefab()
    {
        if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_YANMENGUANWAI)
        {
            UIManager.ShowUI(UIInfo.TargetFrameRoot);
            UIManager.ShowUI(UIInfo.PlayerFrameRoot);
          //   UIManager.ShowUI(UIInfo.JoyStickRoot);
          //   UIManager.ShowUI(UIInfo.SkillBarRoot);
            UIManager.ShowUI(UIInfo.SkillProgress);
            UIManager.ShowUI(UIInfo.CentreNotice);
            UIManager.ShowUI(UIInfo.PlayerHitsRoot);
        }
        else
        {
            try
            {
                UIManager.ShowUI(UIInfo.TargetFrameRoot);
                NGUIDebug.Log("TargetFrameRoot");
                UIManager.ShowUI(UIInfo.PlayerFrameRoot);
                NGUIDebug.Log("PlayerFrameRoot");
                UIManager.ShowUI(UIInfo.MenuBarRoot);
                UIManager.ShowUI(UIInfo.FunctionButtonRoot);
                UIManager.ShowUI(UIInfo.MissionDialogAndLeftTabsRoot);  // 任务界面算一级界面 任务对话框特殊处理
                //UIManager.ShowUI(UIInfo.PlayerAimBar);  // 任务界面算一级界面 任务对话框特殊处理
                UIManager.ShowUI(UIInfo.RechargeBar);
                UIManager.ShowUI(UIInfo.ChatFrameRoot);
                UIManager.ShowUI(UIInfo.ExpRoot);
                UIManager.ShowUI(UIInfo.JoyStickRoot);
                NGUIDebug.Log("JoyStickRoot");
                UIManager.ShowUI(UIInfo.SkillBarRoot);
                NGUIDebug.Log("SkillBarRoot");
                UIManager.ShowUI(UIInfo.PlayerHitsRoot);
                UIManager.ShowUI(UIInfo.SkillProgress);
                UIManager.ShowUI(UIInfo.CentreNotice);
                UIManager.ShowUI(UIInfo.RollNotice);
                UIManager.ShowUI(UIInfo.PkNotice);
                UIManager.ShowUI(UIInfo.DyingBlood);
                UIManager.ShowUI(UIInfo.AutoMedicine);
                UIManager.ShowUI(UIInfo.AutoFightBtn);
            }
            catch (System.Exception e) 
            {
                NGUIDebug.Log(e.ToString());
            }
        }       
		
		if (null ==BackCamerControll.Instance())
		{
			//ResourceManager.InstantiateResource("Prefab/Logic/BackCamera", "BackCamerControll");
			ResourceManager.LoadBackCamera();
		}
        /*
        ResourceManager.LoadUIPrefab("NewPlayerGuidRoot", 1);
        ResourceManager.LoadUIPrefab("TargetFrameRoot", 1);
        ResourceManager.LoadUIPrefab("PlayerFrameRoot", 1);
        ResourceManager.LoadUIPrefab("MenuBarRoot", 1);
        ResourceManager.LoadUIPrefab("FunctionButtonRoot", 1);
        ResourceManager.LoadUIPrefab("MissionDialogAndLeftTabsRoot", 1);  // 任务界面算一级界面 任务对话框特殊处理
        ResourceManager.LoadUIPrefab("PlayerAimBar", 1);  // 任务界面算一级界面 任务对话框特殊处理
        ResourceManager.LoadUIPrefab("ChatFrameRoot", 1);
        ResourceManager.LoadUIPrefab("ExpRoot", 1);
        ResourceManager.LoadUIPrefab("JoyStickRoot", 1);
        ResourceManager.LoadUIPrefab("SkillBarRoot", 1);
        ResourceManager.LoadUIPrefab("PlayerHitsRoot", 1);
        ResourceManager.LoadUIPrefab("StoryDialogRoot", 0);
        ResourceManager.LoadUIPrefab("AutoFightRoot", 2);
        //ResourceLoader.LoadUIPrefab("MessageBoxRoot", 3);
        ResourceManager.LoadUIPrefab("SceneMapRoot", 2);
        ResourceManager.LoadUIPrefab("BackPackRoot", 2);
        ResourceManager.LoadUIPrefab("PopMenuRoot", 3);
        ResourceManager.LoadUIPrefab("EquipTooltipsRoot", 3);
        ResourceManager.LoadUIPrefab("ItemTooltipsRoot", 3);
        ResourceManager.LoadUIPrefab("RelationRoot", 2);
        ResourceManager.LoadUIPrefab("MountAndFellowRoot", 2);
        ResourceManager.LoadUIPrefab("CollectItemSliderRoot", 0);
        */

        //NoticeLogic.TryOpen();
    }

    /*
    public void OpenWindow(string strUIName)
    {
        if (DicUI[strUIName] != null)
        {
            DicUI[strUIName].SetActive(true);

            GameEvent _event = new GameEvent(GameDefine_Globe.EVENT_DEFINE.EVENT_OPENWINDOW);
            _event.AddStringParam(strUIName);
            Singleton<EventSystem>.GetInstance().PushEvent(_event);

            // 级别为1的不参与互斥 一直显示
            if(DicUILevel[strUIName] != 1)
            {
                foreach (KeyValuePair<string, int> pair in DicUILevel)
                {
                    // 一般情况下不同级别界面的互斥处理
                    if (pair.Key != strUIName && pair.Value >= DicUILevel[strUIName])
                    {
                        if (IsWindowOpen(pair.Key))
                        {
                            CloseWindow(pair.Key);
                        }
                    }
                }
                // 任务对话框特殊处理 正常情况下走不进这个if
                if (MissionDialogAndLeftTabsLogic.Instance().IsMissionInfoRootOpen())
                {
                    MissionDialogAndLeftTabsLogic.Instance().CloseMissionInfoRoot();
                }
            }
        }
    }
    
    public void CloseWindow(string strUIName)
    {
        if (DicUI[strUIName] != null)
        {
            DicUI[strUIName].SetActive(false);

            GameEvent _event = new GameEvent(GameDefine_Globe.EVENT_DEFINE.EVENT_CLOSEWINDOW);
            _event.AddStringParam(strUIName);
            Singleton<EventSystem>.GetInstance().PushEvent(_event);
        }
    }

    public bool IsWindowOpen(string strUIName)
    {
        if (DicUI[strUIName] != null)
        {
            return DicUI[strUIName].activeSelf;
        }
        return false;
    }
     * */
}
