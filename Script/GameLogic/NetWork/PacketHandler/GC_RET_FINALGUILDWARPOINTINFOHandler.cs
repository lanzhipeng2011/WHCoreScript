//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;

namespace SPacket.SocketInstance
{
    public class GC_RET_FINALGUILDWARPOINTINFOHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_RET_FINALGUILDWARPOINTINFO packet = (GC_RET_FINALGUILDWARPOINTINFO) ipacket;
            if (null == packet) return (uint) PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            if (GuildWarInfoLogic.Instance())
            {
                GuildWarInfoLogic.Instance().UpdateWarPointInfo(packet);
            }
            return (uint) PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
