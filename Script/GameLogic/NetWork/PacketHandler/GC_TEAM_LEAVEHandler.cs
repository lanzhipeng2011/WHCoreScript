//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using Games.LogicObj;

namespace SPacket.SocketInstance
{
    public class GC_TEAM_LEAVEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_TEAM_LEAVE packet = (GC_TEAM_LEAVE)ipacket;
            if (null == packet) 
                return (uint)PACKET_EXE.PACKET_EXE_ERROR;

            //直接调用MainPlayer的离队操作

            if (null != Singleton<ObjManager>.GetInstance().MainPlayer)
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.LeaveTeam();
            }
            
            //更新组队界面（如果未打开，则在UpdateTeamInfo中会不处理）
            if (null != GUIData.delFriendDataUpdate) GUIData.delFriendDataUpdate();

            if (ChatInfoLogic.Instance() != null)
            {
                ChatInfoLogic.Instance().UpdateTeamAndGuildChannel();
            }

			if (PlayerPreferenceData.LeftTabChoose == 1)
			{
				if (MissionDialogAndLeftTabsLogic.Instance() != null)
				{
					MissionDialogAndLeftTabsLogic.Instance().UpdateTeamInfo();
				}
			}
			if (null != RelationLogic.Instance ()) 
			{
				RelationLogic.OpenTeamWindow (RelationTeamWindow.TeamTab.TeamTab_NearPlayer);
			}

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
