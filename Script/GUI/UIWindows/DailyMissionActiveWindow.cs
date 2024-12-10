using UnityEngine;
using System.Collections;
using Module.Log;
using GCGame;
using Games.DailyMissionData;
using GCGame.Table;

public class DailyMissionActiveWindow : MonoBehaviour
{
    public DailyMissionWindow m_DailyMissionWindow;
    public ActiveAwardWindow m_ActivenessWindow;
	public TabController m_DailyMissionTabController;

    // 新手指引
    private int m_NewPlayerGuide_Step = -1;
    public int NewPlayerGuide_Step
    {
        get { return m_NewPlayerGuide_Step; }
        set { m_NewPlayerGuide_Step = value; }
    }

    // 界面加载后调用
    //void Start()
    //{
    //}

    // 日常任务界面
    public bool IsDailyMissionActive()
    {
        if (m_DailyMissionWindow)
        {
            return m_DailyMissionWindow.gameObject.activeSelf;
        }
        return false;
    }
    public void UpDateDailyMissionState(int nMissionID)
    {
        if (m_DailyMissionWindow)
        {
            m_DailyMissionWindow.UpdateMissionState(nMissionID);
        }
    }
    public void UpDateDoneCount(int nDoneCount)
    {
        if (m_DailyMissionWindow)
        {
            m_DailyMissionWindow.DoneCount = nDoneCount;
        }
    }
    public void UpDateActiveness(int nActiveness)
    {
        if (m_DailyMissionWindow)
        {
            m_DailyMissionWindow.Activeness = nActiveness;
        }
    }
    public void UpdateMissionList()
    {
        if (m_DailyMissionWindow)
        {
            m_DailyMissionWindow.UpDateMissionList();
        }
    }
    public void UpdateMissionItemByKind(int nKind)
    {
        if (m_DailyMissionWindow)
        {
            m_DailyMissionWindow.UpdateMissionItemByKind(nKind);
        }
    }

    // 活跃度奖励界面
    public bool IsActivenessWindowActive()
    {
        if (m_ActivenessWindow)
        {
            return m_ActivenessWindow.gameObject.activeSelf;
        }
        return false;
    }
    public void UpdateAwardItemState(int nTurnID)
    {
        if (m_ActivenessWindow)
        {
            m_ActivenessWindow.UpdateAwardItemState(nTurnID);
        }
    }

    // 新手指引
    public void NewPlayerGuide(int nIndex)
    {
        m_NewPlayerGuide_Step = nIndex;
        switch (nIndex)
        {
            case 1:
                m_DailyMissionWindow.NewPlayerGuide(1);
                m_NewPlayerGuide_Step = -1;
                break;
        }
    }
}