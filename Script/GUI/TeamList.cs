using UnityEngine;
using System.Collections;
using Games.GlobeDefine;

public class TeamList : MonoBehaviour
{
    private static TeamList m_Instance = null;
    public static TeamList Instance()
    {
        return m_Instance;
    }

    public MemberUIInfo[] m_MemberUI;

    // Awake
    void Awake()
    {
        m_Instance = this;
    }

	// Use this for initialization
	void Start ()
	{
	}

    void OnDestroy()
    {
        m_Instance = null;
    }


    void OnEnable()
    {
        UpdateTeamMember();
    }
	

    //更新队员的信息
    public void UpdateTeamMember()
    {
        ClearTeamListUI();
        //如果没有队，则清空
        if (GameManager.gameManager.PlayerDataPool.TeamInfo.TeamID < 0)
        {
            return;
        }

        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        //同时更新自己的队长图标
        if (null != PlayerFrameLogic.Instance() &&
            null != Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            PlayerFrameLogic.Instance().SetTeamCaptain(Singleton<ObjManager>.GetInstance().MainPlayer.IsTeamLeader());
        }

        int index = 0;
        TeamMember[] member = GameManager.gameManager.PlayerDataPool.TeamInfo.teamMember;
        for (int i = 0; i < member.Length; ++i)
        {
            //如果队员的GUID为非空，并且和主角不一样，则显示
            if (member[i].Guid != GlobeVar.INVALID_GUID && member[i].Guid != Singleton<ObjManager>.GetInstance().MainPlayer.GUID)
            {
                ShowMemberInfo(m_MemberUI[index], member[i]);
                index++;
            }      
        }
    }

    //显示某一个队员的信息
    void ShowMemberInfo(MemberUIInfo teamMemberUI, TeamMember memberInfo)
    {
        if (null == teamMemberUI)
        {
            return;
        }

        teamMemberUI.UpdateInfo(memberInfo);
        teamMemberUI.gameObject.SetActive(true);
    }

    //清空信息
    public void ClearTeamListUI()
    {
        for (int i = 0; i < m_MemberUI.Length; ++i)
        {
            if (null != m_MemberUI[i])
            {
                m_MemberUI[i].gameObject.SetActive(false);
            }
        }
        //foreach (MemberUIInfo teamMemberUI in m_MemberUI)
        //{
        //    if (null != teamMemberUI)
        //    {
        //        teamMemberUI.gameObject.SetActive(false);
        //    }            
        //}
    }

    //打开组队界面
    public void OpenTeamWindow()
    {
		PlayerFrameLogic.Instance().SwitchAllWhenPopUIShow (false);
        RelationLogic.OpenTeamWindow(RelationTeamWindow.TeamTab.TeamTab_TeamInfo);
    }
}
