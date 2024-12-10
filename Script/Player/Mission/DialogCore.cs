/********************************************************************************
 *	文件名：DialogCore.cs
 *	全路径：	\Script\Mission\DialogCore.cs
 *	创建人：	贺文鹏
 *	创建时间：2014-02-17
 *
 *	功能说明： 客户端的对话逻辑，包括NPC对话、任务对话。
 *	       
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Games.LogicObj;
using Games.GlobeDefine;
using Games.Mission;
using GCGame.Table;

public class DialogCore : Singleton<DialogCore> 
{
    //当前关注的NPC,调用BeginDialog的时候，如果这个不为空，则直接读取Obj_Npc中的对话相关数据
    private Obj_NPC m_CareNPC;
    public Obj_NPC CareNPC
    {
        get { return m_CareNPC; }
        set { m_CareNPC = value; }
    }

    //当无Obj_Npc的时候，也可以直接用DialogID来触发简单对话
    private int m_nDialogID;
    public int DialogID
    {
        get { return m_nDialogID; }
        set { m_nDialogID = value; }
    }
        
    //////////////////////////////////////////////////////////////////////////
    //对话相关接口
    //////////////////////////////////////////////////////////////////////////
    // 弹出最近对话框 --寻路完响应
    public void AutoDialog(int nMissionID)
    {
        Tab_MissionBase MissionBase = TableManager.GetMissionBaseByID(nMissionID, 0);
        if (MissionBase == null)
        {
            return;
        }

        Obj_NPC TargetNpc = null;
        foreach (string strName in Singleton<ObjManager>.GetInstance().ObjPools.Keys)
        {
            Obj_Character objChar = Singleton<ObjManager>.GetInstance().ObjPools[strName] as Obj_Character;
            if (null == objChar)
                return;

            if (objChar.BaseAttr.RoleBaseID == MissionBase.AcceptDataID
                || objChar.BaseAttr.RoleBaseID == MissionBase.CompleteDataID)
            {
                TargetNpc = objChar as Obj_NPC;
                break;
            }
        }
        if (TargetNpc == null)
        {
            return;
        }

        if (null != Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.OnSelectTarget(TargetNpc.gameObject);
        }
        if (!TargetNpc.IsHaveMission(nMissionID))
            return;

        Show(TargetNpc);
    }
    
    //开始对话窗口,根绝参数来确定有无关心NPC
    //对外只提供这一个接口，之后确定是显示对白还是显示任务
    public void Show(Obj_NPC objNpc)
    {
        m_CareNPC = objNpc;
        if (m_CareNPC == null)
        {
            return;
        }
        if (false == IsInDialogArea())
        {
            return;
        }
        // 弹出已完成任务
        if (true == PopCompletedMission())
        {
            return;
        }
        
        // 弹出可接任务
        if (true == PopCanAcceptedMission())
        {
            return;
        }

		if (true == SpecialDialog (m_CareNPC)) 
		{
			return;
		}

        // 与NPC对话
        ShowText(m_CareNPC.DefaultDialogID);
    }

    // 遍历弹出已完成任务
    bool PopCompletedMission()
    {
        // 显示已完成任务
        for (int i = 0; i < m_CareNPC.MissionList.Count; ++i)
        {
            int missionID = m_CareNPC.MissionList[i];
            if (missionID < 0)
            {
                return false;
            }

			Tab_MissionBase table = TableManager.GetMissionBaseByID(missionID, 0);
            if (table == null)
            {
                return false;
            }

            bool bIsHaveMission = GameManager.gameManager.MissionManager.IsHaveMission(missionID);
            if (false == bIsHaveMission)
            {
                continue;
            }

			if (table.CompleteDataID != m_CareNPC.BaseAttr.RoleBaseID 
			    && !(table.MissionType != 4 && table.LogicType == 0))
			{
                continue;
            }

            MissionState misState = (MissionState)GameManager.gameManager.MissionManager.GetMissionState(missionID);
            if (table.MissionType != (int)MISSIONTYPE.MISSION_DAILY && MissionState.Mission_Completed == misState)
            {
                ShowMission(missionID);
                return true;
            }

            // 剧情对话特殊处理
            if (table.LogicType == (short)TableType.Table_Story) //是剧情任务,不弹出对话框
            {
                if (StoryDialogLogic.ShowStory(table.LogicID))
                {

                    return true;
                }
            }
        }

        return false;
    }

    // 遍历弹出可接任务
    bool PopCanAcceptedMission()
    {
        // 显示可接任务
        for (int i = 0; i < m_CareNPC.MissionList.Count; ++i)
        {
            int missionID = m_CareNPC.MissionList[i];
            if (missionID < 0)
            {
                return false;
            }

            Tab_MissionBase table = TableManager.GetMissionBaseByID(missionID, 0);
            if (table == null)
            {
                return false;
            }

            if (table.AcceptDataID != m_CareNPC.BaseAttr.RoleBaseID)
            {
                continue;
            }

            bool bCanAcceptMission = GameManager.gameManager.MissionManager.CanAcceptMission(missionID);
            if (bCanAcceptMission)
            {
                ShowMission(missionID);
                return true;
            }
        }

        return false;
    }

    public void Show(int nDialogID)
    {
        m_CareNPC = null;
        m_nDialogID = nDialogID;
    }

    //显示任务
    void ShowMission(int nMissionId)
    {
        if (GlobeVar.INVALID_ID == nMissionId)
        {
            return;
        }
        MissionInfoController.ShowMissionDialogUI(nMissionId);
    }

    //显示对白
    void ShowText(int nDialogId)
    {
        if (GlobeVar.INVALID_ID == nDialogId)
        {
            return;
        }
        MissionInfoController.ShowNpcDialogUI(nDialogId);
    }

    // 对话区域半径判断
    public bool IsInDialogArea(Vector3 vPos)
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
            return false;

        //int nLevel = GameManager.gameManager.RunningScene;
        Vector3 userPos = Singleton<ObjManager>.GetInstance().MainPlayer.Position;

        float dis = Mathf.Abs(Vector3.Distance(userPos, vPos));
        if (dis < 2)
        {
            return true;
        }

        return false;
    }
    public bool IsInDialogArea()
    {
        return IsInDialogArea(m_CareNPC.Position);
    }

	bool SpecialDialog(Obj_NPC obj)
	{
        if (obj && (obj.BaseAttr.RoleBaseID == GlobeVar.MARRY_NPCID
                 || obj.BaseAttr.RoleBaseID == GlobeVar.DIVORCE_NPCID
                 || obj.BaseAttr.RoleBaseID == GlobeVar.PARADE_NPCID)
            ) 
		{
            MarryRootLogic.ShowMarryDialogUI(obj.BaseAttr.RoleBaseID);
			return true;
		}
        //如果开关关闭，奖励NPC就按照普通NPC一样对话
        if (obj && obj.BaseAttr.RoleBaseID == GlobeVar.AWARD_NPCID)
        {
            if (!GameManager.gameManager.PlayerDataPool.IsServerFlagOpen(SERVER_FLAGS_ENUM.FLAG_CYFANS))
            {
                return false;
            }
        }
        //显示带选项的对话框
	    if (obj.DefaultDialogID !=-1)
	    {
	        Tab_NpcDialog _npcDialogInfo = TableManager.GetNpcDialogByID(obj.DefaultDialogID, 0);
            if (_npcDialogInfo !=null && _npcDialogInfo.OptionDialogId !=-1)
	        {
                OptionDialogLogic.ShowOptionDialogUI(obj.DefaultDialogID);
	            return true;
	        }
	    }
		return false;
	}

    // 寻路后逻辑处理
    public void MissionLogic(int nMissionID)
    {
        Tab_MissionBase table = TableManager.GetMissionBaseByID(nMissionID, 0);
        int nTableType = table.LogicType;

        MissionState misState = (MissionState)GameManager.gameManager.MissionManager.GetMissionState(nMissionID);
        if (MissionState.Mission_Accepted == misState)
        {
            if (nTableType == ((int)TableType.Table_KillMonster)
                || nTableType == ((int)TableType.Table_LootItem))
            {
                if (Singleton<ObjManager>.GetInstance().MainPlayer
                    && Singleton<ObjManager>.GetInstance().MainPlayer.OwnSkillInfo[0].IsValid())
                {
                    int nSkillID = Singleton<ObjManager>.GetInstance().MainPlayer.OwnSkillInfo[0].SkillId;
                    Singleton<ObjManager>.GetInstance().MainPlayer.SelectTarget = null;
                    Singleton<ObjManager>.GetInstance().MainPlayer.UseSkillOpt(nSkillID,null);
                }
                return;
            }
        }

        // 弹出对话
        AutoDialog(nMissionID);
    }
}
