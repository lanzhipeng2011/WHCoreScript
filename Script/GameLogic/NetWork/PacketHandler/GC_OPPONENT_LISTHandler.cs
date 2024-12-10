//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_OPPONENT_LISTHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_OPPONENT_LIST packet = (GC_OPPONENT_LIST)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            PVPData.UpdateOpponentInfo(packet);
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
