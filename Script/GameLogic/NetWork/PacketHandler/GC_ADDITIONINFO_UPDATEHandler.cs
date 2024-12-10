//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_ADDITIONINFO_UPDATEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_ADDITIONINFO_UPDATE packet = (GC_ADDITIONINFO_UPDATE)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            AdditionData.UpdateAdditionData(packet);
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
