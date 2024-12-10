/********************************************************************************
 *	文件名：	Obj_MainPlayer_Team.cs
 *	全路径：	\Script\Obj\Obj_MainPlayer_Team.cs
 *	创建人：	李嘉
 *	创建时间：2013-10-25
 *
 *	功能说明：游戏主角Obj的组队逻辑部分
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using Games.LogicObj;
using Games.GlobeDefine;
using System;
using System.Collections.Generic;

namespace Games.LogicObj
{
    public partial class Obj_MainPlayer : Obj_OtherPlayer
    {
        //邀请某个玩家入队
        public void ReqInviteTeam(UInt64 playerGuid)
        {
            //检查被邀请者GUID是否为空(由于可以创建单人队伍，所以发空guid说明是单人组队，可以认为合法)
            //if (GlobeVar.INVALID_GUID == playerGuid)
            //{
            //    return;
            //}

            //判断队伍是否已满
            if (GameManager.gameManager.PlayerDataPool.IsHaveTeam())
            {
                if (GameManager.gameManager.PlayerDataPool.TeamInfo.IsFull())
                {
                    SendNoticMsg(false, "#{1145}");
                }
            }

            //非单人组队，增加提示“邀请已发送”
            if (playerGuid != GlobeVar.INVALID_GUID)
            {
                SendNoticMsg(false, "#{3170}");
            }

            //向服务器发送邀请某人加入队伍消息
            CG_REQ_TEAM_INVITE msg = (CG_REQ_TEAM_INVITE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_REQ_TEAM_INVITE);
            msg.Guid = playerGuid;
            msg.SendPacket();
        }

        //申请加入队伍
        public void ReqJoinTeam(UInt64 teamMemberGuid)
        {
            //有队伍则不得申请
            if (GameManager.gameManager.PlayerDataPool.IsHaveTeam())
            {
                SendNoticMsg(false, "#{2179}");
                return;
            }

            //申请加入队伍的队员GUID是否为非法
            if (GlobeVar.INVALID_GUID == teamMemberGuid)
            {
                return;
            }

            //向服务器发送申请加入队伍消息
			SendNoticMsg(false, "#{2906}");
            CG_REQ_TEAM_JOIN msg = (CG_REQ_TEAM_JOIN)PacketDistributed.CreatePacket(MessageID.PACKET_CG_REQ_TEAM_JOIN);
            msg.TeamMemberGuid = teamMemberGuid;
            msg.SendPacket();
        }

        //申请离开队伍
        public void ReqLeaveTeam()
        {
            //向服务器发送申请离队消息
            CG_REQ_TEAM_LEAVE msg = (CG_REQ_TEAM_LEAVE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_REQ_TEAM_LEAVE);
            msg.SetTeamID(GameManager.gameManager.PlayerDataPool.TeamInfo.TeamID);
            msg.SendPacket();
        }

        //申请踢人
        public void ReqKickTeamMember(UInt64 teamMemberGuid)
        {
            if (GlobeVar.INVALID_GUID == teamMemberGuid)
            {
                return;
            }

            CG_REQ_TEAM_KICK_MEMBER msg = (CG_REQ_TEAM_KICK_MEMBER)PacketDistributed.CreatePacket(MessageID.PACKET_CG_REQ_TEAM_KICK_MEMBER);
            msg.SetTeamMemberGuid(teamMemberGuid);
            msg.SendPacket();
        }

        //申请变换队长
        public void ReqChangeTeamLeader(UInt64 teamMemberGuid)
        {
            if (GlobeVar.INVALID_GUID == teamMemberGuid)
            {
                return;
            }

            CG_REQ_TEAM_CHANGE_LEADER msg = (CG_REQ_TEAM_CHANGE_LEADER)PacketDistributed.CreatePacket(MessageID.PACKET_CG_REQ_TEAM_CHANGE_LEADER);
            msg.SetTeamMemberGuid(teamMemberGuid);
            msg.SendPacket();
        }
        
        //更新某个组员信息，第三位为是否只更新HP标记
        public void UpdateTeamMemberInfo(int nIndex, TeamMember member, bool bJustUpdateHP)
        {
            //下标判断
            if (nIndex < 0 || nIndex >= GlobeVar.MAX_TEAM_MEMBER)
            {
                return;
            }

            //更新数据
            GameManager.gameManager.PlayerDataPool.TeamInfo.UpdateMember(nIndex, member, bJustUpdateHP);

            //更新UI数据
            TeamList.Instance().UpdateTeamMember();
        }

        //收到离开队伍消息
        public void LeaveTeam()
        {
			SendNoticMsg(false, "#{2469}");
            //清理队伍数据
            GameManager.gameManager.PlayerDataPool.TeamInfo.CleanUp();

            //通知UI清理队伍数据
			if (null != TeamList.Instance ()) 
			{
				TeamList.Instance().ClearTeamListUI();
			}
            
			if (null != MissionDialogAndLeftTabsLogic.Instance ()) 
			{
				MissionDialogAndLeftTabsLogic.Instance ().UpdateTeamInfo ();
			}
            //更新队长头像
            if (null != PlayerFrameLogic.Instance())
            {
                PlayerFrameLogic.Instance().SetTeamCaptain(false);
            }

            //退出组队跟随状态
            LeaveTeamFollow();
        }

        //MainPlayer是否为队长
        public bool IsTeamLeader()
        {
            if (GlobeVar.INVALID_ID == GameManager.gameManager.PlayerDataPool.TeamInfo.TeamID)
            {
                return false;
            }

            if (GUID == GameManager.gameManager.PlayerDataPool.TeamInfo.GetTeamMember(0).Guid)
            {
                return true;
            }

            return false;
        }

        //否个Guid是否为队长
        public bool IsTeamLeader(UInt64 guid)
        {
            if (GlobeVar.INVALID_ID == GameManager.gameManager.PlayerDataPool.TeamInfo.TeamID ||
                GlobeVar.INVALID_GUID == guid)
            {
                return false;
            }

            if (guid == GameManager.gameManager.PlayerDataPool.TeamInfo.GetTeamMember(0).Guid)
            {
                return true;
            }

            return false;
        }

        //只有MainPlayer有组队跟随功能
        //所以做到客户端
        //开始组队跟随状态
        private int m_nFollowServerID = GlobeVar.INVALID_ID;
		private ulong m_nFollowPlayerId = GlobeVar.INVALID_GUID;
        public int FollowServerID
        {
            get { return m_nFollowServerID; }
            set { 
                    m_nFollowServerID = value;
                    m_followObjChar = Singleton<ObjManager>.GetInstance().FindObjCharacterInScene(m_nFollowServerID);
                }
        }

        

        public void EnterTeamFollow()
        {
            // 副本，无法进入组队跟随
            if (GameManager.gameManager.ActiveScene.IsCopyScene())
            {
                return;
            }

            //没有队伍，无法进入组队跟随状态
            if (GlobeVar.INVALID_ID == GameManager.gameManager.PlayerDataPool.TeamInfo.TeamID)
            {
                return;
            }

            //如果自己是队长，也无法进入组队跟随
            if (IsTeamLeader())
            {
                return;
            }

            //根据队长的Guid找到ServerID
            TeamMember teamLeader = GameManager.gameManager.PlayerDataPool.TeamInfo.teamMember[0];
            //如果队伍信息非法或者自己是队长
            if (teamLeader.IsValid())
            {
                //遍历
                foreach (KeyValuePair<string, Obj> obj in Singleton<ObjManager>.GetInstance().ObjPools)
                {
                    //只能跟随其他玩家
                    if (obj.Value.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER)
                    {
                        Obj_OtherPlayer player = obj.Value as Obj_OtherPlayer;
                        if (null != player && player.GUID == teamLeader.Guid)
                        {
                            m_nFollowServerID = player.ServerID;
							m_nFollowPlayerId = player.GUID;
                            m_followObjChar = Singleton<ObjManager>.GetInstance().FindObjCharacterInScene(m_nFollowServerID);
                            break;
                        }
                    }
                }
            }
        }

        //结束组队跟随状态
        public void LeaveTeamFollow()
        {
			if (m_nFollowServerID != GlobeVar.INVALID_ID) 
			{
				m_nFollowServerID = GlobeVar.INVALID_ID;
				m_nFollowPlayerId = GlobeVar.INVALID_GUID;
				m_followObjChar = Singleton<ObjManager>.GetInstance().FindObjCharacterInScene(m_nFollowServerID);
				//
				if (null != MissionDialogAndLeftTabsLogic.Instance ()) 
				{
					MissionDialogAndLeftTabsLogic.Instance ().LeaveTeamFollow ();
				}	

				SendNoticMsg(false, "#{4742}");
			}
        }

        //是否为组队跟随状态
        public bool IsTeamFollowState()
        {
			return GlobeVar.INVALID_GUID != m_nFollowPlayerId;
        }

		public bool IsTeamLeaderChange()
		{
			return !IsTeamLeader(m_nFollowPlayerId);
		}

        //组队跟随状态更新

        private Obj_Character m_followObjChar = null;
        public void UpdateTeamFollow()
        {
            if (GlobeVar.INVALID_ID == m_nFollowServerID)
            {
                return;
            }

            if (null != m_followObjChar)
            {
                MoveTo(m_followObjChar.transform.position, null, 1.0f);
            }
        }

		public bool MemberInSence(System.UInt64 memberId)
		{
			foreach (KeyValuePair<string, Obj> obj in Singleton<ObjManager>.GetInstance().ObjPools)
			{
				//只能跟随其他玩家
				if (obj.Value.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER)
				{
					Obj_OtherPlayer player = obj.Value as Obj_OtherPlayer;
					if (null != player && player.GUID ==memberId)
					{
						return true;
					}
				}
			}
			return false;
		}
    }
}
