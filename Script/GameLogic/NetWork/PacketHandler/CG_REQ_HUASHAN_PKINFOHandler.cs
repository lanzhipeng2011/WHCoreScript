//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
    public class CG_REQ_HUASHAN_PKINFOHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            CG_REQ_HUASHAN_PKINFO packet = (CG_REQ_HUASHAN_PKINFO)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
 }
