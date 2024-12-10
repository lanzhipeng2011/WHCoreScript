//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_SEND_FASHIONINFOHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_SEND_FASHIONINFO packet = (GC_SEND_FASHIONINFO )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            GameManager.gameManager.PlayerDataPool.FashionDeadline[packet.FashionID] = packet.Deadline;
            if (FashionLogic.Instance() != null)
            {
                FashionLogic.Instance().HandleSendFashionInfo(packet.FashionID);
            }
            
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
