//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_ASK_LOUDSPEAKER_CONTENTHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_ASK_LOUDSPEAKER_CONTENT packet = (GC_ASK_LOUDSPEAKER_CONTENT )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic

            if (LoudSpeakerLogic.Instance() != null)
            {
                LoudSpeakerLogic.Instance().SendLoudSpeakerInfo();
            }
            
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
