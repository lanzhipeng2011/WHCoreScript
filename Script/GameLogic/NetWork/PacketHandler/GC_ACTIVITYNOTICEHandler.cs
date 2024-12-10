//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_ACTIVITYNOTICEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_ACTIVITYNOTICE packet = (GC_ACTIVITYNOTICE )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            if (RollNotice.Instance())
            {
                if (packet.HasStrNotice)
                {
                    RollNotice.Instance().AddRollNotice(packet.StrNotice);
                }
            }         
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
