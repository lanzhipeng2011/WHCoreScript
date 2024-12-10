//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
    public class GC_RET_HUASHAN_PKINFOHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_RET_HUASHAN_PKINFO packet = (GC_RET_HUASHAN_PKINFO)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            HuaShanPVPData.ShowHuaShanPkInfoList(packet);
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
 }
