/********************************************************************************
 *	文件名：MissionManager.cs
 *	全路径：	\Script\Mission\MissionManager.cs
 *	创建人：	王华
 *	创建时间：2013-11-05
 *
 *	功能说明： 客户端的任务管理器，负责管理所有客户端任务相关数据，供界面使用。
 *	       
 *	修改记录：
 *         2014-5-28 Lijia: 客户端效率优化，把CurOwnMission从class改为struct
*********************************************************************************/

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GCGame.Table;
using Games.Events;
using Games.LogicObj;
using Games.GlobeDefine;
using Module.Log;
using Games.UserCommonData;
using Games.DailyMissionData;
namespace Games.Mission
{
    //任务类型
    public enum MISSIONTYPE
    {
        MISSION_INVALID = -1,
        MISSION_DAILY = 0,  //日常
        MISSION_MAIN = 1,   //主线
        MISSION_BRANCH = 2, //支线
        MISSION_GUILD = 3,  //帮会
        MISSION_MASTER = 4, //师门
		MISSION_BIZ = 17
    }

    // 任务逻辑类型
    public enum TableType
    {
        Table_Invalid = -1,
        Table_Story,          //剧情
        Table_KillMonster,    //杀怪
        Table_Delivery,   // 送信
        Table_CollectItem,   //杀怪掉落
        Table_Enterarea,  // 探索区域
        Table_LootItem,   // 杀怪掉落
        Table_CopySceneMonster,   // 副本杀怪掉落
        Table_LevelUp,  // 升级
        Table_OperationNum, // 操作次数
        Mission_Num
    }
    public enum DailyMissionType
    {
        DailyMissionType_Invalid = -1,
        DailyMissionType_Master,          //门派
        DailyMissionType_KillMonster,    //杀怪
        DailyMissionType_CopyScene,     //副本
        DailyMissionType_Strength,   // 强化
        DailyMissionType_Pvp,  // 竞技
        DailyMissionType_GuildWar,   // 帮战
        DailyMissionType_Award,   // 抽奖
        DailyMissionType_Belle,   // 美人
        DailyMissionType_Fellow,   // 伙伴
        DailyMissionType_Num
    }

    public enum MissionState
    {
        Mission_None,       // 未接取
        Mission_Accepted,   // 已接任务未完成
        Mission_Completed,  // 已完成未提交
        Mission_Failed,     // 任务失败
    }

    public enum MISSION_QUALITY
    {
        MISSION_QUALITY_NONE = 0,
        MISSION_QUALITY_WHITE = 1,
        MISSION_QUALITY_GREEN = 2,
        MISSION_QUALITY_BLUE = 3,
        MISSION_QUALITY_PURPLE = 4,
        MISSION_QUALITY_ORANGE = 5,
    }

    /// <summary>
    /// 当前拥有的任务信息,需要服务器同步
    /// </summary>
    public class CurOwnMission
    {
        public const byte MAX_MISSION_PARAM_NUM = 8;

        public int m_nMissionID;   //任务ID

        public byte m_yStatus;      //任务状态，1表示进行中，2表示完成，3表示失败
        public byte m_yFlags;       // 0x0000 |FollowChanged事件|ItemChanged事件|EnterZone事件|KillObject事件|
        public byte m_yQuality;    // 任务品质 白、绿、蓝、紫、橙
        public Int32[] m_nParam;

        public CurOwnMission()
        {
            CleanUp();
        }

        public void CleanUp()
        {
            if (null == m_nParam)
            {
                m_nParam = new Int32[MAX_MISSION_PARAM_NUM];
            }
            m_nMissionID = -1;
            m_yStatus = 0;
            m_yFlags = 0;
            m_yQuality = 0;

            for (int i = 0; i < MAX_MISSION_PARAM_NUM; ++i)
            {
                m_nParam[i] = 0;
            }
        }

        public void SetStatus(byte status)
        {
            m_yStatus = status;
        }
    }

    /// <summary>
    /// 任务列表
    /// </summary>
    public class MissionList
    {
        public const byte MAX_CHAR_MISSION_NUM = 9;
        public const UInt16 MAX_CHAR_MISSION_FLAG_LEN = 32;

        public byte m_Count = 0;

        public Dictionary<int, CurOwnMission> m_aMission;         //角色目前拥有的任务数据
        public UInt32[] m_aMissionHaveDoneFlags;   //角色的任务完成标志，支持最多1024个任务

        public MissionList()
        {
            m_Count = 0;
            m_aMission = new Dictionary<int, CurOwnMission>();
            m_aMissionHaveDoneFlags = new UInt32[MAX_CHAR_MISSION_FLAG_LEN];
        }

        public bool IsMissionFull()
        {
            if (m_aMission.Count > MAX_CHAR_MISSION_NUM)
            {
                return true;
            }
            return false;
        }

        public void CleanUp()
        {
            m_aMission.Clear();
            for (int i = 0; i < MAX_CHAR_MISSION_FLAG_LEN; i++)
            {
                m_aMissionHaveDoneFlags[i] = 0;
            }
        }

    };

    public class MissionManager
    {
        private MissionList m_MissionList;  //任务列表

        // 忽略前序任务标记
        private int m_IgnoreMissionPreFlag;
        public int IgnoreMissionPreFlag
        {
            set { m_IgnoreMissionPreFlag = value; }
        }

        public MissionManager()
        {
            m_MissionList = new MissionList();
            m_IgnoreMissionPreFlag = 0;
        }

        /// <summary>
        /// 客户端检查，是否可以接受任务
        /// </summary>
        /// <param name="nMissionID">任务ID</param>
        /// <returns>成功与否</returns>
        public bool CanAcceptMission(int nMissionID)
        {
            Tab_MissionBase misLine = TableManager.GetMissionBaseByID(nMissionID, 0);
            if (misLine == null)
                return false;

            // 0、任务是否已满
            if (m_MissionList.IsMissionFull())
            {
                // 提示 任务已满
                return false;
            }
            
            // 1、检查玩家身上是否有任务
            if (true == IsHaveMission(nMissionID) 
                && MissionState.Mission_Failed != (MissionState)GetMissionState(nMissionID))
            {
                return false;
            }

            if (false == SpecialMission_CanAccept(nMissionID))
            {
                return false;
            }

            Tab_MissionLimit misLimitLine = TableManager.GetMissionLimitByID(misLine.LimitID, 0);
            if (misLimitLine == null) // 无限制
            {
                return true;
            }

            // 2、任务限制等级
            int nPlayerLevel = Singleton<ObjManager>.Instance.MainPlayer.BaseAttr.Level;
            if (misLimitLine.LowLevel > nPlayerLevel)
            {
                return false;
            }

            // 3、检查玩家是否完成任务
            if (misLimitLine.IsLoop != 1)
            {
                if (true == IsMissionHaveDone(nMissionID))
                {
                    // 提示 任务已完成
                    return false;
                }
            }

            // 4、检查前续任务
            if (m_IgnoreMissionPreFlag == 0)
            {
                if (misLimitLine.PreMission > -1 && !IsMissionHaveDone(misLimitLine.PreMission))
                {
                    return false;
                }
            }

            return true;
        }

        public bool CanDoAcceptMission(int nMissionID)
        {
            Tab_MissionBase misLine = TableManager.GetMissionBaseByID(nMissionID, 0);
            if (misLine == null)
                return false;

            // 0、任务是否已满
            if (m_MissionList.IsMissionFull())
            {
                // 提示 任务已满
                return false;
            }

            // 1、检查玩家身上是否有任务
            if (true == IsHaveMission(nMissionID)
                && MissionState.Mission_Failed != (MissionState)GetMissionState(nMissionID))
            {
                return false;
            }

            if (false == SpecialMission_CanAccept(nMissionID))
            {
                return false;
            }

            Tab_MissionLimit misLimitLine = TableManager.GetMissionLimitByID(misLine.LimitID, 0);
            if (misLimitLine == null) // 无限制
            {
                return true;
            }

            // 2、任务限制等级
            int nPlayerLevel = Singleton<ObjManager>.Instance.MainPlayer.BaseAttr.Level;
            if (misLimitLine.LowLevel > nPlayerLevel)
            {
                return false;
            }

            // 3、检查玩家是否完成任务
            if (misLimitLine.IsLoop != 1)
            {
                if (true == IsMissionHaveDone(nMissionID))
                {
                    // 提示 任务已完成
                    return false;
                }
            }

            // 4、检查前续任务
            if (m_IgnoreMissionPreFlag == 0)
            {
                if (misLimitLine.PreMission > -1 && !IsMissionHaveDone(misLimitLine.PreMission))
                {
                    return false;
                }
            }

            if (misLine.AcceptDataID == -1)
            {
                return true;
            }

            Obj_NPC TargetNpc = Singleton<DialogCore>.GetInstance().CareNPC;
            if (TargetNpc == null)
            {
                return false;
            }
            if (TargetNpc.BaseAttr.RoleBaseID != misLine.AcceptDataID)
            {
                return false;
            }


            return true;
        }



        /// <summary>
        /// 客户端添加任务
        /// </summary>
        /// <param name="nMissionID">任务ID</param>
        /// <returns>返回添加成功与否</returns>
        bool AddMission(int nMissionID, byte yQuality)
        {
            if (nMissionID < 0)
            {
                return false;
            }

            if (m_MissionList.m_aMission.ContainsKey(nMissionID))
            {
                return false;
            }

            CurOwnMission tempMission = new CurOwnMission();
            tempMission.CleanUp();
            tempMission.m_nMissionID = nMissionID;
            tempMission.m_yQuality = yQuality;
            m_MissionList.m_aMission.Add(nMissionID, tempMission);

            return true;
        }

        /// <summary>
        /// 客户端删除一个任务
        /// </summary>
        /// <param name="nMissionID"></param>
        /// <returns></returns>
        bool DelMission(int nMissionID)
        {
            if (nMissionID < 0)
            {
                return false;
            }

            bool bRet = m_MissionList.m_aMission.Remove(nMissionID);
            return bRet;
        }

        /// <summary>
        /// 完成任务，这里以后需要由服务器同步过来
        /// </summary>
        /// <param name="nMissionID"></param>
        /// <returns></returns>
        bool SetMissionHaveDone(int nMissionID)
        {
            int idIndex = (nMissionID >> 5);
            if (idIndex < MissionList.MAX_CHAR_MISSION_FLAG_LEN)
            {
                UInt32 uData = (UInt32)nMissionID & 0x0000001f;
                m_MissionList.m_aMissionHaveDoneFlags[idIndex] |= (UInt32)(0x00000001 << (Int32)uData);

                return true;
            }
            return false;
        }

        bool IsMissionHaveDone(int nMissionID)
        {
            if (nMissionID < 0)
            {
                return false;
            }
            int idIndex = (nMissionID >> 5);
            if (idIndex < MissionList.MAX_CHAR_MISSION_FLAG_LEN)
            {
                UInt32 uRet = (UInt32)(0x00000001 << ((Int32)nMissionID & 0x0000001F)) & m_MissionList.m_aMissionHaveDoneFlags[idIndex];
                return (uRet != 0);
            }

            return false;
        }

        public bool IsMissionNotFaild(int missionId)
        {
            if (IsHaveMission(missionId))
            {
                MissionState missionState = (MissionState) GetMissionState(missionId);
                if(( missionState != MissionState.Mission_None) && (missionState != MissionState.Mission_Failed))
                {
                    return true;
                }
            }
            return false;
        }

        // 通过任务ID取该任务在任务表表的索引值， 无该任务时返回UINT_MAX
        public int GetMissionIndexByID(int nMissionID) { return 1; }

        // 任务参数
        public void SetMissionParam(int nMissionID, int nParamIndex, int nValue)
        {
            if (!m_MissionList.m_aMission.ContainsKey(nMissionID))
            {
                return;
            }
            if (nParamIndex >= 0 && nParamIndex < CurOwnMission.MAX_MISSION_PARAM_NUM)
            {
                m_MissionList.m_aMission[nMissionID].m_nParam[nParamIndex] = nValue;
            }
            // 默认0位置存任务跟踪项 计数
            if (nParamIndex == 0)
            {
                // 客户端任务 存下
                if (IsClientMission(nMissionID))
                {
                    if (null != Singleton<ObjManager>.GetInstance()
                        && null != Singleton<ObjManager>.GetInstance().MainPlayer)
                    {
                        UInt64 PlayerGuid = Singleton<ObjManager>.GetInstance().MainPlayer.GUID;
                        UserConfigData.AddClientMission(PlayerGuid.ToString(), m_MissionList.m_aMission[nMissionID]);
                    }
                }

                // 通知UI
                NotifyMissionUI(nMissionID, "state");
            }

        }
        public int GetMissionParam(int nMissionID, int nParamIndex)
        {
            if (!m_MissionList.m_aMission.ContainsKey(nMissionID))
            {
                return -1;
            }
            if (nParamIndex >= 0 && nParamIndex < CurOwnMission.MAX_MISSION_PARAM_NUM)
            {
                return m_MissionList.m_aMission[nMissionID].m_nParam[nParamIndex];
            }
            return -1;
        }

        // 接任务 给服务器发包
        public bool AcceptMission(int nMissionID)
        {
            // 添加任务接取判断条件

            // 给服务器发包
            CG_ACCEPTMISSION askMission = (CG_ACCEPTMISSION)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ACCEPTMISSION);
			askMission.SetMissionID((uint)nMissionID);
            askMission.SendPacket();

            return true;
        }

        // 任务接取成功后 客户端处理
        public bool AddMissionToUser(int nMissionID, byte yQualityType)
        {
            if (IsHaveMission(nMissionID))
            {
                return false;
            }

            if (false == AddMission(nMissionID, yQualityType))
            {
                return false;
            }

            // 通知客户端更新UI
            NotifyMissionUI(nMissionID, "add");

            // 通知日常任务界面
            NotifyDailyMissionUI(nMissionID,"add");

            NotifyGuildMissionUI(nMissionID, "");

            if (false == SetMissionState(nMissionID, (byte)MissionState.Mission_Accepted))
            {
                return false;
            }

            // 第一个任务 无玩家坐标，剧情跳过
            if (nMissionID == 1)
            {
               return true;
            }
            Tab_MissionBase table = TableManager.GetMissionBaseByID(nMissionID, 0);
            if (table != null )
            {
                //是剧情任务,不弹出对话框
                if (table.LogicType == (short)TableType.Table_Story && table.MissionType != (int)MISSIONTYPE.MISSION_DAILY) 
                {


                    if (StoryDialogLogic.ShowStory(table.LogicID))
                    {
                        return true;
                    }
                }
            }

            return true;
        }

        // 提交任务，给服务器发包
        public bool CompleteMission(int nMissionID)
        {
            Tab_MissionBase misLine = TableManager.GetMissionBaseByID(nMissionID, 0);
            if (misLine == null)
                return false;

            // 2、检查玩家身上是否有任务
            if (false == IsHaveMission(nMissionID))
            {
                return false;
            }

            // 3、任务状态检查
            if ((byte)MissionState.Mission_Completed != GetMissionState(nMissionID))
            {
                return false;
            }

            // 给服务器发包
            CG_COMPLETEMISSION askMission = (CG_COMPLETEMISSION)PacketDistributed.CreatePacket(MessageID.PACKET_CG_COMPLETEMISSION);
            askMission.SetMissionID(nMissionID);
            askMission.SendPacket();

            return true;
        }

        // 任务提交成功后 客户端处理
        public bool CompleteMissionOver(int nMissionID)
        {
            // 客户端模拟包
            if (false == SetMissionHaveDone(nMissionID))
            {
                return false;
            }

            // 通知客户端更新UI
            NotifyMissionUI(nMissionID, "Del");

            bool bRet = DelMission(nMissionID);//删除任务
            if (!bRet)
            {
                return false;
            }

            if (IsClientMission(nMissionID))
            {
                if (null != Singleton<ObjManager>.GetInstance()
                        && null != Singleton<ObjManager>.GetInstance().MainPlayer)
                {
                    UInt64 PlayerGuid = Singleton<ObjManager>.GetInstance().MainPlayer.GUID;
                    UserConfigData.DelClientMission(PlayerGuid.ToString(), nMissionID);
                }
            }

            // 通知日常任务界面
            NotifyDailyMissionUI(nMissionID,"Del");

            //弹出下一个任务的接受界面
            Tab_MissionBase table = TableManager.GetMissionBaseByID(nMissionID, 0);
            if (table != null)
            {
                int nNextMissionId = table.NextMissionID;
                MissionInfoController.ShowMissionDialogUI(nNextMissionId);
            }

            return true;
        }

        // 放弃任务
        public bool AbandonMission(int nMissionID)
        {
            // 检查玩家身上是否有任务
            if (false == IsHaveMission(nMissionID))
            {
                return false;
            }

            // 给服务器发包
            CG_ABANDONMISSION askMission = (CG_ABANDONMISSION)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ABANDONMISSION);
            askMission.SetMissionID(nMissionID);
            askMission.SendPacket();

            return true;
        }

        public bool AbandonMissionOver(int nMissionID)
        {

            // 通知客户端更新UI
            NotifyMissionUI(nMissionID, "Del");
            
            // 直接删除任务
            bool bRet = DelMission(nMissionID);//删除任务
            if (!bRet)
            {
                return false;
            }

            NotifyDailyMissionUI(nMissionID, "Del");

            NotifyGuildMissionUI(nMissionID, "Abandon");

            if (IsClientMission(nMissionID))
            {
                if (null != Singleton<ObjManager>.GetInstance()
                        && null != Singleton<ObjManager>.GetInstance().MainPlayer)
                {
                    UInt64 PlayerGuid = Singleton<ObjManager>.GetInstance().MainPlayer.GUID;
                    UserConfigData.DelClientMission(PlayerGuid.ToString(), nMissionID);
                }
            }

            return true;
        }

        public bool IsHaveMission(int nMissionID)
        {
            if (nMissionID < 0)
            {
                return false;
            }

            if (m_MissionList.m_aMission.ContainsKey(nMissionID))
            {
                return true;
            }
            return false;
        }
        public bool SetMissionState(int nMissionID, byte nStatus)
        {
            if (false == IsHaveMission(nMissionID))
            {
                return false;
            }
            if (nStatus == GetMissionState(nMissionID))
            {
                return false;
            }
            m_MissionList.m_aMission[nMissionID].SetStatus(nStatus);

            // 客户端任务 保存下状态
            if (IsClientMission(nMissionID))
            {
                if (null != Singleton<ObjManager>.GetInstance()
                    && null != Singleton<ObjManager>.GetInstance().MainPlayer)
                {
                    UInt64 PlayerGuid = Singleton<ObjManager>.GetInstance().MainPlayer.GUID;
                    UserConfigData.AddClientMission(PlayerGuid.ToString(), m_MissionList.m_aMission[nMissionID]);
                }
            }

            // 通知UI更新
            NotifyMissionUI(nMissionID, "state");

            // 通知日常任务界面
            NotifyDailyMissionUI(nMissionID, "state");

            NotifyGuildMissionUI(nMissionID, "");

            return true;
        }
        public byte GetMissionState(int nMissionID)
        {
            if (false == IsHaveMission(nMissionID))
            {
                return 0;
            }
            return m_MissionList.m_aMission[nMissionID].m_yStatus;
        }

        public byte GetMissionQuality(int nMissionID)
        {
            if (true == IsHaveMission(nMissionID))
            {
                return m_MissionList.m_aMission[nMissionID].m_yQuality;
            }
            return (byte)MISSION_QUALITY.MISSION_QUALITY_NONE;
        }

        // 任务更新UI
        public void NotifyMissionUI(int nMissionID, string strOpt)
        {
            if (null != MissionDialogAndLeftTabsLogic.Instance())
            {
                MissionDialogAndLeftTabsLogic.Instance().UpDateMissionFollow(nMissionID, strOpt);
            }

        }

        public bool IsClientMission(int nMissionID)
        {
            Tab_MissionBase MissionBase = TableManager.GetMissionBaseByID(nMissionID, 0);
            if (MissionBase == null)
            {
                return false;
            }
            if (nMissionID == 301 || nMissionID == 302)
            {
                return false;
            }
            int nType = MissionBase.LogicType;
            if (nType == (int)TableType.Table_Story || nType == (int)TableType.Table_CollectItem)
            {
                return true;
            }
            return false;
        }

        // 剧情任务 在客户端完成
        public bool SetStoryMissionState(int nMissionID, byte byState)
        {
            Tab_MissionBase misLine = TableManager.GetMissionBaseByID(nMissionID, 0);
            if (misLine == null)
            {
                return false;
            }

            if (TableType.Table_Story == (TableType)misLine.LogicType)
            {
                SetMissionParam(nMissionID, 0, 1);
                return SetMissionState(nMissionID, (byte)byState);
            }

            return false;
        }

        // 获取玩家身上的任务
        public List<int> GetAllMissionID()
        {
            List<int> nMissionIDList = new List<int>();

            // 遍历玩家任务列表
            foreach (KeyValuePair<int, CurOwnMission> kvp in m_MissionList.m_aMission)
            {
                if (kvp.Key < 0)
                    continue;
                nMissionIDList.Add(kvp.Key);
            }
            return nMissionIDList;
        }

        // 获取玩家身上的非日常任务
        public List<int> GetAllNotDailyMissionList()
        {
            List<int> nMissionIDList = new List<int>();

            // 遍历玩家任务列表
            foreach (KeyValuePair<int, CurOwnMission> kvp in m_MissionList.m_aMission)
            {
                if (kvp.Key < 0)
                    continue;

                Tab_MissionBase misBase = TableManager.GetMissionBaseByID(kvp.Key, 0);
				if (null == misBase 
				    || misBase.MissionType == (int)MISSIONTYPE.MISSION_DAILY 
				    || misBase.MissionType == (int)MISSIONTYPE.MISSION_GUILD)
                {
                    continue;
                }

                nMissionIDList.Add(kvp.Key);
            }
            return nMissionIDList;
        }

        // 遍历场景中前nNum个可接任务（按等级先大后小） 任务日志用
        public List<int> GetCanAcceptedMissionID(int nNum)
        {
            List<int> nMissionIDList = new List<int>();
            
            // 遍历表格 查任务
            Dictionary<int, List<Tab_SceneNpc> > AllNpc = TableManager.GetSceneNpc();
            foreach (int key in AllNpc.Keys)
            {
                Tab_SceneNpc NpcTable = AllNpc[key][0];
                if (null == NpcTable)
                {
                    continue;
                }

                Tab_RoleBaseAttr npcRoleBase = TableManager.GetRoleBaseAttrByID(NpcTable.DataID, 0);
                if (null == npcRoleBase || npcRoleBase.DialogID < 0)
                {
                    continue;
                }

                Tab_NpcDialog npcDialog = TableManager.GetNpcDialogByID(npcRoleBase.DialogID, 0);
                if (null == npcDialog)
                {
                    continue;
                }
                for (int i = 0; i < npcDialog.getMissionIDCount(); i++ )
                {
                    int nMissionID = npcDialog.GetMissionIDbyIndex(i);
                    if (nMissionID < 0)
                    {
                        continue;
                    }

                    Tab_MissionBase misBase = TableManager.GetMissionBaseByID(nMissionID, 0);
                    if (null == misBase)
                    {
                        continue;
                    }

                    if (false == CanAcceptMission(nMissionID)
                        || misBase.AcceptDataID != NpcTable.DataID)
                    {
                        continue;
                    }

                    // 有序添加
                    if (false == AddMissionIDByLevel(nMissionIDList, nMissionID))
                    {
                    }

                    if (nMissionIDList.Count > nNum)
                    {
                        nMissionIDList.RemoveAt(nNum);
                    }
                }
            }

            return nMissionIDList;
        }
        // 按任务限制等级排序 任务日志用
        bool AddMissionIDByLevel(List<int> MissionList, int nMissionID)
        {
            if (MissionList.Count < 0)
            {
                return false;
            }

            bool bFlag = false;
            for (int i = 0; i < MissionList.Count; i++)
            {
                Tab_MissionLimit TempMisLimit = TableManager.GetMissionLimitByID(nMissionID, 0);
                if (TempMisLimit == null)
                {
                    return false;
                }

                Tab_MissionLimit MisLimit = TableManager.GetMissionLimitByID(MissionList[i], 0);
                if (MisLimit == null)
                {
                    // 异常 怎么处理？
                    continue;
                }

                if (TempMisLimit.LowLevel > MisLimit.LowLevel)
                {
                    bFlag = true;
                    MissionList.Insert(i, nMissionID);
                    break;
                }
            }

            if (bFlag == false)
            {
                MissionList.Add(nMissionID);
            }

            return true;
        }

        // 任务追踪、任务日志寻路
        public void MissionPathFinder(int nMissionID)
        {
            if (nMissionID < 0)
            {
                return;
            }
            Tab_MissionDictionary MDLine = TableManager.GetMissionDictionaryByID(nMissionID, 0);
            if (MDLine == null)
            {
                return;
            }
            if (Singleton<ObjManager>.Instance.MainPlayer)
            {
                if (GameManager.gameManager.ActiveScene.IsCopyScene())
                {
                    Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{1604}");
                    return;
                }
            }

            if (MDLine.IsAcceptAutoFindPath) //自动寻路
            {
                
                //接受任务后自动寻路
                Vector3 targetPos = new Vector3();
                int nTargetScene = -1;
                int nTargetNpcDataID = -1;
                // 任务是否完成
                MissionState misState = (MissionState)GetMissionState(nMissionID);
                if (MissionState.Mission_None == misState
                    || MissionState.Mission_Failed == misState)
                {
                    nTargetScene = MDLine.AccepteNpcSceneID;
                    targetPos.x = MDLine.AccepteNpcPosX;
                    targetPos.z = MDLine.AccepteNpcPosZ;
                    nTargetNpcDataID = MDLine.AcceptNpcDataID;
                }
                else if (MissionState.Mission_Accepted == misState)
                {
                    // 日常任务处理
                    if ((int)TableType.Table_LevelUp == GetMissionLogicType(nMissionID))
                    {
                        OpenMissionUI();
                        return;
                    }

            
                    if (Mission_NewPlayerGuide(nMissionID)) // 特殊任务 新手指引
                    {
                        return;
                    }

                    if (OpenMissionWindow(nMissionID))
                    {
                        return;
                    }

                    nTargetScene = MDLine.TargetNpcSceneID;
                    targetPos.x = MDLine.TargetNpcPosX;
                    targetPos.z = MDLine.TargetNpcPosZ;
                    nTargetNpcDataID = MDLine.TargetNpcDataID;
                }
                else if (MissionState.Mission_Completed == misState)
                {
                    // 日常任务处理
                    if ((int)MISSIONTYPE.MISSION_DAILY == GetMissionType(nMissionID))
                    {                    
                        OpenDailyMissionUI();                 
                        return;
                    }
					else if((int)MISSIONTYPE.MISSION_GUILD == GetMissionType(nMissionID))
					{
						OpenGuildMissionWindow();               
						return;
					}
					else
					{
                        nTargetScene = MDLine.CompleteNpcSceneID;
                        targetPos.x = MDLine.CompleteNpcPosX;
                        targetPos.z = MDLine.CompleteNpcPosZ;
                        nTargetNpcDataID = MDLine.CompleteNpcDataID;
                    }
                }

                if (null != Singleton<ObjManager>.GetInstance()
                    && null != Singleton<ObjManager>.GetInstance().MainPlayer)
                {
                    Singleton<ObjManager>.GetInstance().MainPlayer.BreakAutoCombatState();

                    Singleton<ObjManager>.Instance.MainPlayer.m_playerHeadInfo.ToggleXunLu(true);
					ObjManager.Instance.MainPlayer.AutoXunLu=true;
                    // 自动寻路
                    AutoSearchPoint point = new AutoSearchPoint(nTargetScene, targetPos.x, targetPos.z);
                    if (GameManager.gameManager && GameManager.gameManager.AutoSearch)
                    {
                        GameManager.gameManager.AutoSearch.BuildPath(point);
                        Tab_RoleBaseAttr RoleBase = TableManager.GetRoleBaseAttrByID(nTargetNpcDataID, 0);
                        if (null != RoleBase && null != GameManager.gameManager.AutoSearch.Path)
                        {
                            GameManager.gameManager.AutoSearch.Path.AutoSearchTargetName = RoleBase.Name;
                        }
                    }

                    MoveOverMissionEvent(nMissionID);
                }
            }
        }

        // 任务寻路完成后 任务相关事件处理
        void MoveOverMissionEvent(int nMissionID)
        {
            
            // 收集任务特殊处理
            Tab_MissionBase missBase = TableManager.GetMissionBaseByID(nMissionID, 0);
            if (missBase == null)
            {
                return;
            }
            MissionState misState = (MissionState)GetMissionState(nMissionID);
            if (missBase.LogicType == (int)TableType.Table_CollectItem)
            {
                if (misState != MissionState.Mission_Accepted)
                {
                    return;
                }

                GameEvent gameEvent = new GameEvent();
                gameEvent.Reset();
                gameEvent.EventID = Games.GlobeDefine.GameDefine_Globe.EVENT_DEFINE.EVENT_MISSION_COLLECTITEM;
                //gameEvent.AddIntParam(nMissionID);
                //注意,要在MoveTo之后调用
                if (null != GameManager.gameManager.AutoSearch &&
                    null != GameManager.gameManager.AutoSearch.Path)
                    GameManager.gameManager.AutoSearch.Path.FinishCallBackEvent = gameEvent;
            }
        }
		int tem=0;
        // 服务器同步数据
        public void SyncMissionList(GC_MISSION_SYNC_MISSIONLIST data)
        {
            m_MissionList.CleanUp();

			for (int i = 0; i < data.missionIDCount; i++)
            {
				int nMissionID = (int)data.GetMissionID(i);
                if (true == AddMission(nMissionID, (byte)data.GetQualitytype(i)))
                {
					byte tem2=(byte)data.GetState(i);
                    m_MissionList.m_aMission[nMissionID].SetStatus(tem2);


                    for (int j = 0; j < CurOwnMission.MAX_MISSION_PARAM_NUM; j++)
                    {
						tem= data.nParamList[i * CurOwnMission.MAX_MISSION_PARAM_NUM + j];
						m_MissionList.m_aMission[nMissionID].m_nParam[j] =tem;
                    }
                }

                // 读取客户端存储的任务数据
                if (IsClientMission(nMissionID))
                {
                    Dictionary<string, List<CurOwnMission>> ClientMissionData = UserConfigData.GetClientMissionData();
                    UInt64 PlayerGuid = PlayerPreferenceData.LastRoleGUID;
                    if (ClientMissionData.ContainsKey(PlayerGuid.ToString()))
                    {
                        foreach (CurOwnMission oMission in ClientMissionData[PlayerGuid.ToString()])
                        {
                            if (nMissionID == oMission.m_nMissionID)
                            {
                                m_MissionList.m_aMission[nMissionID].SetStatus(oMission.m_yStatus);
                                m_MissionList.m_aMission[nMissionID].m_nParam[0] = oMission.m_nParam[0];
                            }
                        }
                    }
                }

                // 拼字任务特殊处理额……
                if (nMissionID == 105)
                {
                    m_MissionList.m_aMission[nMissionID].SetStatus((byte)MissionState.Mission_Completed);
                    m_MissionList.m_aMission[nMissionID].m_nParam[0] = 1;
                }
            }

            for (int nIndex = 0; nIndex < MissionList.MAX_CHAR_MISSION_FLAG_LEN; nIndex++)
            {
                m_MissionList.m_aMissionHaveDoneFlags[nIndex] = data.GetHavedoneFlag(nIndex);
            }

            GameManager.gameManager.PlayerDataPool.MissionSortList.Clear();
            // 如果UI存在，更新UI
            if (MissionDialogAndLeftTabsLogic.Instance())
            {
                MissionDialogAndLeftTabsLogic.Instance().InitMissionFollow();
            }
        }

        //MissionBoard用，Npc头顶任务特效
        public bool IsHaveMissionAccepted(Obj_NPC oNpc)
        {
            foreach (int nMissionID in oNpc.MissionList)
            {
                Tab_MissionBase MissionBase = TableManager.GetMissionBaseByID(nMissionID, 0);
                if (MissionBase == null)
                {
                    continue;
                }

                if (MissionBase.AcceptDataID != oNpc.BaseAttr.RoleBaseID)
                {
                    continue;
                }

                if (CanAcceptMission(nMissionID))
                {
                    return true;
                }
            }

            return false;
        }
        public MissionBoard.MissionBoardState GetMissionBoardState(Obj_NPC oNpc)
        {
            foreach (int nMissionID in oNpc.MissionList)
            {
                Tab_MissionBase MissionBase = TableManager.GetMissionBaseByID(nMissionID, 0);
                if ( MissionBase == null)
                {
                    continue;
                }

                if (false == IsHaveMission(nMissionID))
                {
                    continue;
                }

                byte yState = GetMissionState(nMissionID);
                if (MissionBase.CompleteDataID == oNpc.BaseAttr.RoleBaseID)
                {
                    if (yState == (byte)MissionState.Mission_Accepted)
                    {
                        return MissionBoard.MissionBoardState.MISSION_CANCOMPLETED;
                    }
                    else if (yState == (byte)MissionState.Mission_Completed)
                    {
                        return MissionBoard.MissionBoardState.MISSION_COMPLETED;
                    }
                }
                else if (MissionBase.AcceptDataID == oNpc.BaseAttr.RoleBaseID)
                {
                    return MissionBoard.MissionBoardState.MISSION_ACCEPTED;
                }
            }

            return MissionBoard.MissionBoardState.MISSION_NONE;
        }
        
        // 更新日常任务界面
        void NotifyDailyMissionUI(int nMissionID, string strOpt)
        {
            Tab_MissionBase MissionBase = TableManager.GetMissionBaseByID(nMissionID, 0);
            if (MissionBase != null && MissionBase.MissionType == (int)MISSIONTYPE.MISSION_DAILY)
            {
                Tab_DailyMission DailyMission = TableManager.GetDailyMissionByID(MissionBase.DalityMissionTabID, 0);
                if (DailyMission == null)
                {
                    return;
                }
                
                if (ActivityController.Instance())
                {
                    ActivityController.Instance().UpdateDailyMissionList();
                    ActivityController.Instance().UpdateMissionItemByKind(DailyMission.Type);
                }
            }
        }

        public void NotifyGuildMissionUI(int nMissionID, string strOpt)
        {
            Tab_MissionBase MissionBase = TableManager.GetMissionBaseByID(nMissionID, 0);
            if (MissionBase != null && MissionBase.MissionType == (int)MISSIONTYPE.MISSION_GUILD)
            {
                Tab_GuildMission Mission = TableManager.GetGuildMissionByID(MissionBase.GuildMissionTabID, 0);
                if (Mission == null)
                {
                    return;
                }

                if (GuildMissionLogic.Instance())
                {
                    if (strOpt == "Abandon")
                    {
                        GuildMissionLogic.Instance().UpdateGuildMissionBtn(2);
                        return;
                    }
                    if ((byte)MissionState.Mission_Completed == GetMissionState(nMissionID))
                    {
                        GuildMissionLogic.Instance().UpdateGuildMissionBtn(1);
                        return;
                    }
                    GuildMissionLogic.Instance().UpdateGuildMissionBtn(0);
                }
            }
        }

        public int GetMissionType(int nMissionID)
        {
            Tab_MissionBase MissionBase = TableManager.GetMissionBaseByID(nMissionID, 0);
            if (MissionBase != null)
            {
                return MissionBase.MissionType;
            }
            return (int)MISSIONTYPE.MISSION_INVALID;
        }

        public int GetMissionLogicType(int nMissionID)
        {
            Tab_MissionBase MissionBase = TableManager.GetMissionBaseByID(nMissionID, 0);
            if (MissionBase != null)
            {
                return MissionBase.LogicType;
            }
            return (int)TableType.Table_Invalid;
        }

        // 采集任务处理
        public void MissionCollectItem()
        {
            Dictionary<string, GameObject> otherObjPool = Singleton<ObjManager>.GetInstance().OtherGameObjPools;
            if (otherObjPool.Count <= 0)
            {
                return;
            }

            foreach (KeyValuePair<string, GameObject> objPair in otherObjPool)
            {
                if (objPair.Value.CompareTag("CollectItem"))
                {
                    // 距离判断
                    Vector3 userPos = new Vector3(0, 0, 0);
                    if (Singleton<ObjManager>.GetInstance().MainPlayer == null)
                    {
                        return; 
                    }
                    userPos = Singleton<ObjManager>.GetInstance().MainPlayer.Position;

                    Vector3 TargetPos = objPair.Value.transform.position;
                    TargetPos.y = userPos.y;
                    float dis = Mathf.Abs(Vector3.Distance(userPos, TargetPos));
                    if (dis > 2)
                    {
                        continue;
                    }
                    Singleton<CollectItem>.GetInstance().RemoveItem(objPair.Value);
					//播放采集动作，隐藏武器
					//if(Singleton<ObjManager>.GetInstance().MainPlayer.MountObj==null)
					//{
					//Singleton<ObjManager>.GetInstance().MainPlayer.AnimLogic.Play(1009);
					//Singleton<ObjManager>.GetInstance().MainPlayer.HideOrShowWeanpon();
						//====In MissionCollect
					//	Singleton<ObjManager>.GetInstance().MainPlayer.isMissionCollect = true;
					//}
//					if(Singleton<ObjManager>.GetInstance().MainPlayer.MountID!=-1)
//					Singleton<ObjManager>.GetInstance().MainPlayer.RideOrUnMount(Singleton<ObjManager>.GetInstance().MainPlayer.MountID);

				
                    return;
                }
            }
        }

        // 显示日常任务界面
        void OpenDailyMissionUI()
        {
			PlayerFrameLogic.Instance ().SwitchAllWhenPopUIShow (false);//关闭其它主界面
            UIManager.ShowUI(UIInfo.Activity, ShowDailyMissionOver);
        }

		void OpenGuildMissionWindow()
		{
			PlayerFrameLogic.Instance ().SwitchAllWhenPopUIShow (false);//关闭其它主界面
			UIManager.ShowUI(UIInfo.MasterAndGuildRoot, ShowGuildMissionOver);
		}

        // 显示任务界面
        void OpenMissionUI()
        {
			PlayerFrameLogic.Instance ().SwitchAllWhenPopUIShow (false);//关闭其它主界面
            UIManager.ShowUI(UIInfo.MissionLogRoot);
        }

        void OpenDailyLuckyUI()
        {
            if (MainUILogic.Instance() != null)
            {
				PlayerFrameLogic.Instance ().SwitchAllWhenPopUIShow (false);//关闭其它主界面
                UIManager.ShowUI(UIInfo.DailyDrawRoot);
            }
        }

        int GetDailyMissionType(int nMissionID)
        {
            Tab_MissionBase MissionBase = TableManager.GetMissionBaseByID(nMissionID, 0);
            if (MissionBase == null)
            {
                return (int)DailyMissionType.DailyMissionType_Invalid;
            }
            Tab_DailyMission DailyMission = TableManager.GetDailyMissionByID(MissionBase.DalityMissionTabID, 0);
            if (DailyMission == null)
            {
                return (int)DailyMissionType.DailyMissionType_Invalid;
            }
            return DailyMission.Type;
        }

        void ShowDailyMissionOver(bool bSuccess, object param)
        {
            if (bSuccess)
            {
                if (ActivityController.Instance())
                {
                    ActivityController.Instance().ChangeToDailyMissionTab();
                }
            }
        }

		void ShowGuildMissionOver(bool bSuccess, object param)
		{
			if (bSuccess)
			{
				if (GuildWindow.Instance())
				{
					GuildWindow.Instance().ChangeTabGuildMission();
				}
			}
		}

        // 特殊不可接任务
        public bool SpecialMission_CanAccept(int nMissionID)
        {
			return true;
            if (nMissionID == 236)
            {
                bool bRet = GameManager.gameManager.PlayerDataPool.CommonData.GetCommondFlag((int)USER_COMMONFLAG.CF_MISSION_DAILYMISSION_FLAG);
                if (bRet == true)
                {
                    return false;
                }
            }
            else if (nMissionID == 238)
            {
                bool bRet = GameManager.gameManager.PlayerDataPool.CommonData.GetCommondFlag((int)USER_COMMONFLAG.CF_ACTIVITY_SINGLEDAY_FLAG);
                bool bFlag = GameManager.gameManager.PlayerDataPool.CommonData.GetCommondFlag((int)USER_COMMONFLAG.CF_ACTIVITY_SINGLEDAYMISSION_FLAG);
                if (bRet == false || bFlag == true)
                {
                    return false;
                }
            }
            else if(nMissionID == 239 || nMissionID == 240)
            {
                bool bRet = GameManager.gameManager.PlayerDataPool.CommonData.GetCommondFlag((int)USER_COMMONFLAG.CF_ACTIVITY_THANKSGIVINGDAYFLAG);
                bool bFlag = GameManager.gameManager.PlayerDataPool.CommonData.GetCommondFlag((int)USER_COMMONFLAG.CF_ACTIVITY_THANKSMISSIONERYDAYFLAG);
                if (bRet == false || bFlag == true)
                {
                    return false;
                }
            }
            return true;
        }
    
        // 新手指引任务 点击指引
        bool Mission_NewPlayerGuide(int nMissionID)
        {
            bool bRet = false;
            string strUIName = "";
            int nIndex = -1;
            switch(nMissionID)
            {
	        case 601: // 宝石镶嵌
	            strUIName = "PlayerFrame";
				nIndex = (int)GameDefine_Globe.NEWOLAYERGUIDE.EQUIP_GEM;
	            break;
	        case 602: // 赠兽
	            strUIName = "PlayerFrame";
				nIndex = (int)GameDefine_Globe.NEWOLAYERGUIDE.PET_FIGHT;
	            break;
	        case 603: // 强化
	            strUIName = "PlayerFrame";
					nIndex = (int)GameDefine_Globe.NEWOLAYERGUIDE.EQUIP_INTENSIFY;
	            break;
			case 604: // 开启名将，对名将进行亲密和进化操作。
				strUIName = "PlayerFrame";
				nIndex = (int)GameDefine_Globe.NEWOLAYERGUIDE.MINGJIANG_QIMI;
				break;
			case 605: // 抽取伙伴。
				strUIName = "PlayerFrame";
				nIndex = (int)GameDefine_Globe.NEWOLAYERGUIDE.PET_GAIN;
				break;
			case 607: // 开启宝物系统，探索宝物一次。
				strUIName = "PlayerFrame";
				nIndex = (int)GameDefine_Globe.NEWOLAYERGUIDE.BAOWU;
				break;
			case 608: // 完成一次群雄逐鹿。
				strUIName = "FunctionButton";
				nIndex = (int)GameDefine_Globe.NEWOLAYERGUIDE.ACTIVITY_QUNXIONG;
				break;
			case 609: // 完成一次七星玄阵。
				strUIName = "FunctionButton";
				nIndex = (int)GameDefine_Globe.NEWOLAYERGUIDE.ACTIVITY_QIXINGXUANZHEN;
				break;
			case 610: // 完成一次虎牢关。
			case 613:
				strUIName = "FunctionButton";
				nIndex = (int)GameDefine_Globe.NEWOLAYERGUIDE.ACTIVITY_HULAOGUAN;
				break;
			case 611: // 完成武神塔2层。
			case 612:
				strUIName = "FunctionButton";
				nIndex = (int)GameDefine_Globe.NEWOLAYERGUIDE.ACTIVITY_WUSHENTA;
				break;
			case 614: // 完成护送美人副本。
				strUIName = "FunctionButton";
				nIndex = (int)GameDefine_Globe.NEWOLAYERGUIDE.ACTIVITY_HUSONGMEIREN;
				break;
			case 615: // 完成一次七星玄阵。
				strUIName = "FunctionButton";
				nIndex = (int)GameDefine_Globe.NEWOLAYERGUIDE.ACTIVITY_MISSION;
				break;
			case 616: // 完成一次七星玄阵。
				strUIName = "FunctionButton";
				nIndex = (int)GameDefine_Globe.NEWOLAYERGUIDE.ACTIVITY_GUOGUANZHANJIANG;
				break;
	        case 617: // 打星
	            strUIName = "PlayerFrame";
					nIndex = (int)GameDefine_Globe.NEWOLAYERGUIDE.EQUIP_START;
	            break;
			case 618: // 占星·财运占星1次
				strUIName = "FunctionButton";
				nIndex = (int)GameDefine_Globe.NEWOLAYERGUIDE.DIVI_MONEY;
				break;
			case 619: // 占星·宝物占星1次
				strUIName = "FunctionButton";
				nIndex = (int)GameDefine_Globe.NEWOLAYERGUIDE.DIVI_DRAW;
				break;
			case 620: // 挖掘一次经验墓穴
				strUIName = "PlayerFrame";
				nIndex = (int)GameDefine_Globe.NEWOLAYERGUIDE.RESTAURANT;
				break;
			case 621: // 进入活动·烽火连天
				strUIName = "FunctionButton";
				nIndex = (int)GameDefine_Globe.NEWOLAYERGUIDE.ACTIVITY_YEXIDAYIN;
				break;
			case 622: // 进入活动·烽火连天
				strUIName = "FunctionButton";
				nIndex = (int)GameDefine_Globe.NEWOLAYERGUIDE.ACTIVITY_FENGHUOLIANTIAN;
				break;
            } 

            if (strUIName != "" && nIndex > -1)
            {
                bRet = true;
                NewPlayerGuide.NewPlayerGuideOpt(strUIName, nIndex);
            }

            return bRet;
        }

        // 日常任务 显示对应界面
        bool OpenMissionWindow(int nMissionID)
        {
            bool bRet = true;

			switch(nMissionID)
			{
			case 615:
			case 145:
			case 149:
				// 日常任务
				OpenDailyMissionUI();
				break;
			case 500:
				// 帮会任务
				UIManager.ShowUI(UIInfo.MasterAndGuildRoot);
				break;
			case 502:
			case 411:
			case 603:
				// 装备吸收
				UIManager.ShowUI(UIInfo.EquipStren);
				break;
				// 乾坤袋
			case 506:
				UIManager.ShowUI(UIInfo.BackPackRoot, BackPackLogic.SwitchQianKunDaiView);
				break;
			case 410:
			case 409:
			case 438:
			case 439:
			case 440:
			case 445:
			case 446:
			case 447:
			case 66:
			case 134:
				// 活动界面
				UIManager.ShowUI(UIInfo.Activity, ShowMissionWindow, nMissionID);
				break;
			case 412:
				// 装备镶嵌
				UIManager.ShowUI(UIInfo.EquipStren, OpenEquipStar);
				break;
			case 413:
			case 606:
				// 装备打星
				UIManager.ShowUI(UIInfo.EquipStren, OpenEquipGem);
				break;
			case 418:
			case 450:
				// 宠物吸收
				UIManager.ShowUI(UIInfo.PartnerAndMountRoot, OpenFellow);
				break;
			case 448:
				// 宝物占星
				UIManager.ShowUI(UIInfo.DivinationRoot);
				break;
			case 449:
				// 封赏武将
				UIManager.ShowUI(UIInfo.Belle);
				break;
			default:
				bRet = false;
				break;
			}

            return bRet;
        }
        void OpenEquipStar(bool bSuccess, object param)
        {
            if (bSuccess == false)
            {
                return;
            }

            if (EquipStrengthenLogic.Instance()
                && EquipStrengthenLogic.Instance().m_TabController)
            {
                EquipStrengthenLogic.Instance().m_TabController.ChangeTab("Button2-Star");
                return;
            }
        }

		void OpenEquipGem(bool bSuccess, object param)
		{
			if (bSuccess == false)
			{
				return;
			}
			
			if (EquipStrengthenLogic.Instance()
			    && EquipStrengthenLogic.Instance().m_TabController)
			{
				EquipStrengthenLogic.Instance().m_TabController.ChangeTab("Button3-Gem");
				return;
			}
		}
		
		void OpenRoleView(bool bSuccess, object param)
		{
			if (RoleViewLogic.Instance() != null)
			{
                RoleViewLogic.Instance().m_delAfterStart = RoleViewLogic.Instance().GemBtClick;
            }
        }
        void OpenFellow(bool bSuccess, object param)
        {
            if (PartnerFrameLogic.Instance())
            {
                PartnerFrameLogic.Instance().SetStartDelegate(2);
            }
        }
        // 任务相关 打开相关界面
        void ShowMissionWindow(bool bSuccess, object param)
        {

            if (bSuccess == false)
            {
                return;
            }

            if (ActivityController.Instance() == null)
            {
                return;
            }

            int nMissionID = (int)param;

            string strTabName = "";
            switch (nMissionID)
            {
			case 409:
				strTabName = "Tab7";
				break;
			case 410:
				strTabName = "Tab3";
				break;
			}

            if (strTabName != "")
            {
                ActivityController.Instance().StrTabName = strTabName;
            }
        }
    }

}
