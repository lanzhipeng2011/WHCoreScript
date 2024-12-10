//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_GUILDACTIVITY_BOSSDATAHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_GUILDACTIVITY_BOSSDATA packet = (GC_GUILDACTIVITY_BOSSDATA )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            GameManager.gameManager.PlayerDataPool.HandleGuildActivityBossData(packet);
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
