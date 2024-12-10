//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_BELLE_REST_RETHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_BELLE_REST_RET packet = (GC_BELLE_REST_RET)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            BelleData.UpdateRestData(packet);
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
