//This code create by CodeEngine

using System;
using System.Collections.Generic;
using Games.GlobeDefine;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using Games.LogicObj;

namespace SPacket.SocketInstance
{
    public class GC_TEAM_SYNC_TEAMINFOHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_TEAM_SYNC_TEAMINFO packet = (GC_TEAM_SYNC_TEAMINFO)ipacket;
            if (null == packet) 
                return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            
            //更新数据
            GameManager.gameManager.PlayerDataPool.TeamInfo.UpdateTeamInfo(packet);   
         
            //更新UI
            if (null != TeamList.Instance())
            {
                TeamList.Instance().UpdateTeamMember();
            }
            if (PlayerPreferenceData.LeftTabChoose == 1)
            {
                if (MissionDialogAndLeftTabsLogic.Instance() != null)
                {
                    MissionDialogAndLeftTabsLogic.Instance().UpdateTeamInfo();
                }
            }
            //更新队长头像
            if (null != PlayerFrameLogic.Instance() &&
                null != Singleton<ObjManager>.GetInstance().MainPlayer)
            {
                PlayerFrameLogic.Instance().SetTeamCaptain(Singleton<ObjManager>.GetInstance().MainPlayer.IsTeamLeader());
            }
            
            //更新组队界面（如果未打开，则在UpdateTeamInfo中会不处理）
            if (null != GUIData.delTeamDataUpdate) 
                GUIData.delTeamDataUpdate();

            if (ChatInfoLogic.Instance() != null)
            {
                ChatInfoLogic.Instance().UpdateTeamAndGuildChannel();
                ChatInfoLogic.Instance().UpdateSpeakerList_Team();
            }

            if (MissionDialogAndLeftTabsLogic.Instance() != null)
            {
                MissionDialogAndLeftTabsLogic.Instance().HandleSyncTeamInfo();
            }

			// ????????
			if (null != Singleton<ObjManager>.GetInstance ().MainPlayer) 
			{
				if(Singleton<ObjManager>.GetInstance ().MainPlayer.IsTeamFollowState())
				{
					if(Singleton<ObjManager>.GetInstance ().MainPlayer.IsTeamLeaderChange() 
					   || (Singleton<ObjManager>.GetInstance().FindOtherPlayerInScene(GameManager.gameManager.PlayerDataPool.TeamInfo.GetTeamMember(0).Guid) == null))
						Singleton<ObjManager>.GetInstance ().MainPlayer.LeaveTeamFollow();
				}
			}

            // 组队信息更改(新加入队伍 或者 离开队伍)重置下周围玩家的名字版 
            
            Dictionary<string, Obj> targets = Singleton<ObjManager>.GetInstance().ObjPools;
            foreach (Obj targetObj in targets.Values)
            {
                if (targetObj != null && targetObj.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER)
                {
                    Obj_OtherPlayer _Player = targetObj as Obj_OtherPlayer;
                    if (_Player)
                    {
                        _Player.SetNameBoardColor();
                    }
                }
            }
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
