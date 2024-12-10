//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_GUILD_JOINHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_GUILD_JOIN packet = (GC_GUILD_JOIN )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic

            if (ChatInfoLogic.Instance() != null)
            {
                ChatInfoLogic.Instance().UpdateTeamAndGuildChannel();
                ChatInfoLogic.Instance().UpdateSpeakerList_Guild();
            }
            
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
