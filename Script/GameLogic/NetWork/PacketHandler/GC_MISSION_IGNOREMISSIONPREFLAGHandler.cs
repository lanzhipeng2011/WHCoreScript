//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_MISSION_IGNOREMISSIONPREFLAGHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_MISSION_IGNOREMISSIONPREFLAG packet = (GC_MISSION_IGNOREMISSIONPREFLAG )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            GameManager.gameManager.MissionManager.IgnoreMissionPreFlag = packet.Flag;
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
