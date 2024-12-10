//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
    public class GC_RET_PKINFOHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_RET_PKINFO packet = (GC_RET_PKINFO )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            if (PKInfoSetLogic.Instance() == null)
            {
                return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
            }
            PKInfoSetLogic.Instance().PKModle = packet.PKModle;
            PKInfoSetLogic.Instance().PKCDTime = packet.PKCDTime;
            PKInfoSetLogic.Instance().PKValue = packet.PKValue;
            PKInfoSetLogic.Instance().UpdatePKInfo();
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }
     }
 }
