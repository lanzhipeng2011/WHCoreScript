//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using Games.GlobeDefine;
namespace SPacket.SocketInstance
{
    public class GC_SYNC_FASHIONHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_SYNC_FASHION packet = (GC_SYNC_FASHION )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            if (packet.DeadlineCount <= GlobeVar.MAX_FASHION_SIZE)
            {
                for (int i = 0; i < packet.DeadlineCount; i++)
                {
                    GameManager.gameManager.PlayerDataPool.FashionDeadline[i] = packet.DeadlineList[i];
                }
                GameManager.gameManager.PlayerDataPool.CurFashionID = packet.CurFashionID;
                GameManager.gameManager.PlayerDataPool.ShowFashion = packet.IsShowFashion == 1;
            }
            
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
