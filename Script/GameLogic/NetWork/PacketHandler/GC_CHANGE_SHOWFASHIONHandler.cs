//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_CHANGE_SHOWFASHIONHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_CHANGE_SHOWFASHION packet = (GC_CHANGE_SHOWFASHION )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic

            GameManager.gameManager.PlayerDataPool.ShowFashion = packet.ShowFashion == 1 ? true : false;

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
