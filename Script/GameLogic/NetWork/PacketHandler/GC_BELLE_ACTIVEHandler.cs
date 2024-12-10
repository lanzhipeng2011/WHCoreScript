//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_BELLE_ACTIVEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_BELLE_ACTIVE packet = (GC_BELLE_ACTIVE)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            BelleData.AddBelle(packet.BelleID);
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
