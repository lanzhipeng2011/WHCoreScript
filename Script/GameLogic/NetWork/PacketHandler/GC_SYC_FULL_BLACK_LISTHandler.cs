//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_SYC_FULL_BLACK_LISTHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_SYC_FULL_BLACK_LIST packet = (GC_SYC_FULL_BLACK_LIST)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic

            if (null != GameManager.gameManager.PlayerDataPool.BlackList)
            {
                GameManager.gameManager.PlayerDataPool.BlackList.RebuildRelationList(packet);
            }

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
