//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;

namespace SPacket.SocketInstance
{
    public class GC_RET_FINALGUILDWARGROUPINFOHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_RET_FINALGUILDWARGROUPINFO packet = (GC_RET_FINALGUILDWARGROUPINFO) ipacket;
            if (null == packet) return (uint) PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            if (GuildWarInfoLogic.Instance())
            {
                GuildWarInfoLogic.Instance().UpdateWarGroupInfo(packet);
            }
            return (uint) PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
