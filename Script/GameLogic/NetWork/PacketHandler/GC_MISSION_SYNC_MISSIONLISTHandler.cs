//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_MISSION_SYNC_MISSIONLISTHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_MISSION_SYNC_MISSIONLIST packet = (GC_MISSION_SYNC_MISSIONLIST )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
           GameManager.gameManager.MissionManager.SyncMissionList(packet);
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
