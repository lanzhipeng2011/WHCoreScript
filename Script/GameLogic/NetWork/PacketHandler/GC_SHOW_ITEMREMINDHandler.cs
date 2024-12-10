//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_SHOW_ITEMREMINDHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_SHOW_ITEMREMIND packet = (GC_SHOW_ITEMREMIND)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            ItemRemindLogic.InitItemInfo(packet.DataID);
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
