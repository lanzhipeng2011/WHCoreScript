//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;

namespace SPacket.SocketInstance
{
    public class GC_RET_CURGUILDWARTYPEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_RET_CURGUILDWARTYPE packet = (GC_RET_CURGUILDWARTYPE) ipacket;
            if (null == packet) return (uint) PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            if (GuildWindow.Instance() !=null)
            {
                if (GuildWarInfoLogic.Instance())
                {
                    GuildWarInfoLogic.Instance().RetCurWarType(packet.WarType, packet.RetType);
                }
            }
            
            return (uint) PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
