//********************************************************************
// 文件名: RelationLogic.cs
// 全路径：	\Script\GUI\RelationLogic.cs
// 描述: 玩家关系界面,显示玩家好友，黑名单，邮件和队伍信息
// 作者: lijia
// 创建时间: 2014-01-07
//********************************************************************
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using GCGame;
using Games.GlobeDefine;
using Games.LogicObj;
using Module.Log;
using Games.UserCommonData;

public class RelationLogic : MonoBehaviour 
{
    private static RelationLogic m_Instance = null;
    public static RelationLogic Instance()
    {
        return m_Instance;
    }

    //外部控件
    //public UISprite m_FriendTab;                    //好友分页专属界面
    //public UISprite m_TeamTab;                      //组队分页专属界面
    //public UISprite m_MailTab;                      //邮件分页专属界面
    //public GameObject PlayerFindWindow;             //好友分页好友查找功能专属界面
    //public UIGrid m_PlayerListBG;                   //好友的列表背景
    public TabController m_TabController;
    public RelationTeamWindow m_TeamWindow;

    public GameObject m_FriendBtn;
    public GameObject m_MailBtn;
    public GameObject m_TeamBtn;

    public UIGrid m_TabControllerGrid;
    
  
    public static void OpenMailRecvWindow()
    {
		PlayerFrameLogic.Instance ().SwitchAllWhenPopUIShow (false);//关闭其它主界面
        UIManager.ShowUI(UIInfo.RelationRoot, OnShowRelationWindow); 
    }

    public static void OpenTeamWindow(RelationTeamWindow.TeamTab Teamtab)
    {
        UIManager.ShowUI(UIInfo.RelationRoot, OnShowTeamWindow, Teamtab); 
    }

    void Awake()
    {
		m_Instance = this;
    }

    void Start()
    {
        bool bFlag = GameManager.gameManager.PlayerDataPool.CommonData.GetCommondFlag((int)USER_COMMONFLAG.COUNTER_DB_OPEN_RELATION);
        m_FriendBtn.SetActive(bFlag);

        bFlag = GameManager.gameManager.PlayerDataPool.CommonData.GetCommondFlag((int)USER_COMMONFLAG.COUNTER_DB_OPEN_MAIL);
        m_MailBtn.SetActive(bFlag);

        m_TabControllerGrid.repositionNow = true;
    }

    void OnDestroy()
    {
        m_Instance = null;
    }

    static void OnShowRelationWindow(bool bSuccess, object param)
    {
        if (null != RelationLogic.Instance())
            RelationLogic.Instance().ShowMailWindow();
    }

    void ShowMailWindow()
    {
        m_TabController.ChangeTab("Button_Mail");
    }  

    static void OnShowTeamWindow(bool bSuccess, object param)
    {
        RelationTeamWindow.TeamTab teamtab = (RelationTeamWindow.TeamTab)param;
        if (null != teamtab &&
            null != RelationLogic.Instance())
            RelationLogic.Instance().ShowTeamWindow(teamtab);
    }

    void ShowTeamWindow(RelationTeamWindow.TeamTab Teamtab)
    {
        m_TabController.ChangeTab("Button_Team");
        if (m_TeamWindow != null)
        {
            m_TeamWindow.ChangeTeamTab(Teamtab);
        }
    }

	//Other UI Open Nearby Player
	public void OpenNearByPlayerFrame()
	{
        m_TabController.ChangeTab("Button_Team");
	}
    
    //关闭界面
    void OnClose()
    {
        UIManager.CloseUI(UIInfo.RelationRoot);
    }

    //public void UpdateSelectState(PlayerListItemLogic selectItem)
    //{
    //    m_TabController.GetTabButton("Button_Friend").targetObject.GetComponent<RelationFriendWindow>().UpdateSelectState(selectItem); 
    //}
   
}
