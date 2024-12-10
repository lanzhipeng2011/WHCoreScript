//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_DEBUG_MY_POSHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_DEBUG_MY_POS packet = (GC_DEBUG_MY_POS)ipacket;
            if (null == packet) return 
                (uint)PACKET_EXE.PACKET_EXE_ERROR;

            //enter your logic
            //第一次打开
            if (false == GameManager.gameManager.ShowMainPlayerServerTrace)
            {
                GameManager.gameManager.ShowMainPlayerServerTrace = true;
            }

            if (GameManager.gameManager.ShowMainPlayerServerTrace && 
                null != GameManager.gameManager.ActiveScene)
            {
                GameManager.gameManager.ActiveScene.ShowMainPlayerServerPosition(((float)packet.PosX) / 100, ((float)packet.PosZ) / 100);
            }

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
