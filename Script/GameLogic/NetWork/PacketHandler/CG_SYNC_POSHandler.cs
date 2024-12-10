//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class CG_SYNC_POSHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            CG_SYNC_POS packet = (CG_SYNC_POS)ipacket;
            if (null == packet) 
                return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
