//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
    public class GC_RET_PRELIMINARY_WARINFOHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_RET_PRELIMINARY_WARINFO packet = (GC_RET_PRELIMINARY_WARINFO )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            if (GuildWarInfoLogic.Instance())
            {
                GuildWarInfoLogic.Instance().UpdateGuildWarPreliminaryInfo(packet);
            }
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
 }
