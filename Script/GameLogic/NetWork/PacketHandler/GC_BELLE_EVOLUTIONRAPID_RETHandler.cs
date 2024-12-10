//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_BELLE_EVOLUTIONRAPID_RETHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_BELLE_EVOLUTIONRAPID_RET packet = (GC_BELLE_EVOLUTIONRAPID_RET)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            BelleData.UpdateEvolutionRapidData(packet);
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
