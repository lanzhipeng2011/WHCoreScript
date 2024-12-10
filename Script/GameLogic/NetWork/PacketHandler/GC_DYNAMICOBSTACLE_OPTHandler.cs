//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_DYNAMICOBSTACLE_OPTHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_DYNAMICOBSTACLE_OPT packet = (GC_DYNAMICOBSTACLE_OPT )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic

            DynamicObstacle.HandleObstacle(packet);
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
