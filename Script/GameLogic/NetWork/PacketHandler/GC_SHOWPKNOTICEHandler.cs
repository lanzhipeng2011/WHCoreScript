//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;

namespace SPacket.SocketInstance
{
    public class GC_SHOWPKNOTICEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_SHOWPKNOTICE packet = (GC_SHOWPKNOTICE) ipacket;
            if (null == packet) return (uint) PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            if (PKNoticeLogic.Instance() !=null)
            {
                PKNoticeLogic.Instance().PlayPkNotice(9.0f);
            }
            return (uint) PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
