//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_MAIL_DELETEHandler : Ipacket
    {
       
        public uint Execute(PacketDistributed ipacket)
        {
            GC_MAIL_DELETE packet = (GC_MAIL_DELETE)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            MailData.DelMail(packet.MailGuid);
           
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
