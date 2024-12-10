//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_ABANDONMISSION_RETHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_ABANDONMISSION_RET packet = (GC_ABANDONMISSION_RET )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            int nMissionID = packet.MissionID;
            int nRet = packet.Ret;
            if (1 == nRet)
            {
                GameManager.gameManager.MissionManager.AbandonMissionOver(nMissionID);
            }
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
