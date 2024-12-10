//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_ONLINEAWARD_DATAHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_ONLINEAWARD_DATA packet = (GC_ONLINEAWARD_DATA )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            GameManager.gameManager.PlayerDataPool.HandlePacket(packet);

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
