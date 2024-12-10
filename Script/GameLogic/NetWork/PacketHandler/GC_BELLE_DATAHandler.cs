//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_BELLE_DATAHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_BELLE_DATA packet = (GC_BELLE_DATA)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            BelleData.UpdateBelleData(packet);
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
