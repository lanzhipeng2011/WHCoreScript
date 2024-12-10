//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_SERVERFLAGSHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_SERVERFLAGS packet = (GC_SERVERFLAGS )ipacket;
            if (null == packet)
            {
                return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            }
            GameManager.gameManager.PlayerDataPool.HandlerServerFlags(packet);
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
