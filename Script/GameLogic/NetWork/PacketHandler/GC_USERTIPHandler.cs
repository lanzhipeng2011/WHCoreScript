//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_USERTIPHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_USERTIP packet = (GC_USERTIP)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            if (packet.Tip == (int)GC_USERTIP.TipType.TIP_YUANBAO_NOTENOUGH)
            {
                MessageBoxLogic.OpenOKBox(1406, 1000);
            }
           
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
