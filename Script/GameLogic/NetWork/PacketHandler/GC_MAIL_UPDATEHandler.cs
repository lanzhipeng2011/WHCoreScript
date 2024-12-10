//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_MAIL_UPDATEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_MAIL_UPDATE packet = (GC_MAIL_UPDATE)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            MailData.UpdateMailData(packet);
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
