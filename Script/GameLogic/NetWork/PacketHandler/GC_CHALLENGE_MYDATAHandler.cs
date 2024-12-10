//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_CHALLENGE_MYDATAHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_CHALLENGE_MYDATA packet = (GC_CHALLENGE_MYDATA)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            PVPData.UpdateMyData(packet);
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
