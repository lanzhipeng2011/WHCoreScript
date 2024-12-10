//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_ASK_ACTIVENESSAWARD_RETHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_ASK_ACTIVENESSAWARD_RET packet = (GC_ASK_ACTIVENESSAWARD_RET )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            GameManager.gameManager.PlayerDataPool.HandlePacket(packet);
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
