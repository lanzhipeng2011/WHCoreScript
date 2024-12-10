//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
using Games.SkillModle;
namespace SPacket.SocketInstance
{
    public class GC_SHOW_SKILLNAMEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_SHOW_SKILLNAME packet = (GC_SHOW_SKILLNAME)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;

            int nSkillId = packet.Skillid;
            int nSenderId = packet.Senderid;
            string szSkillName = "";
            if (packet.HasSkillname)
            {
                szSkillName = packet.Skillname;
            }

            SkillCore.ShowSkillName(nSkillId, nSenderId, szSkillName);
            

            //enter your logic
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
