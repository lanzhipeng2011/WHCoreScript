//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using Games.GlobeDefine;
using GCGame.Table;
using Games.LogicObj;
namespace SPacket.SocketInstance
{
    public class GC_JOIN_TEAM_REQUESTHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_JOIN_TEAM_REQUEST packet = (GC_JOIN_TEAM_REQUEST)ipacket;
            if (null == packet)
                return (uint)PACKET_EXE.PACKET_EXE_ERROR;

            //enter your logic
            if (packet.RequesterGuid == GlobeVar.INVALID_GUID)
            {
                return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
            }

            m_RequesterGuid = packet.RequesterGuid;
			Obj_MainPlayer mainPlayer = Singleton<ObjManager>.Instance.MainPlayer;
			if (null != mainPlayer)
			{
				
				if (mainPlayer.GetAutoCombatState() && mainPlayer.AutoAcceptTaem)
				{
					//挂机中
					AgreeTeamJoin();
				}
				else
				{
					//发送MessageBox确认邀请
					//"玩家XXX申请加入队伍，是否同意？"
		
					string dicStr = StrDictionary.GetClientDictionaryString("#{1174}", packet.RequesterName);
					MessageBoxLogic.OpenOKCancelBox(dicStr, "", AgreeTeamJoin, DisagreeTeamJoin);

				}  
			}
           
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;

        }

        private UInt64 m_RequesterGuid = GlobeVar.INVALID_GUID;

        public void AgreeTeamJoin()
        {
            CG_JOIN_TEAM_REQUEST_RESULT pak = (CG_JOIN_TEAM_REQUEST_RESULT)PacketDistributed.CreatePacket(MessageID.PACKET_CG_JOIN_TEAM_REQUEST_RESULT);
            pak.RequesterGuid = m_RequesterGuid;
            pak.Result = 1;
            pak.SendPacket();

            m_RequesterGuid = GlobeVar.INVALID_GUID;
        }

        public void DisagreeTeamJoin()
        {
            CG_JOIN_TEAM_REQUEST_RESULT pak = (CG_JOIN_TEAM_REQUEST_RESULT)PacketDistributed.CreatePacket(MessageID.PACKET_CG_JOIN_TEAM_REQUEST_RESULT);
            pak.RequesterGuid = m_RequesterGuid;
            pak.Result = 0;
            pak.SendPacket();

            m_RequesterGuid = GlobeVar.INVALID_GUID;
        }
    }
}
