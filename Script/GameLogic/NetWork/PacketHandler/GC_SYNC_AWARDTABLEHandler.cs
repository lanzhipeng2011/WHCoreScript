//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using Games.AwardActivity;
namespace SPacket.SocketInstance
{
    public class GC_SYNC_AWARDTABLEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_SYNC_AWARDTABLE packet = (GC_SYNC_AWARDTABLE )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic

            for (int i = 0; i < packet.idCount; i++)
            {
                OnlineAwardLine DataLine = new OnlineAwardLine();
                DataLine.ID = packet.GetId(i);
                DataLine.LeftTime = packet.GetLefttime(i);
                DataLine.Exp = packet.GetExp(i);
                DataLine.Money = packet.GetMoney(i);
                DataLine.BindYuanbao = packet.GetBindyuanbao(i);
                DataLine.Item1DataID = packet.GetItem1dataid(i);
                DataLine.Item1Count = packet.GetItem1count(i);
                DataLine.Item2DataID = packet.GetItem2dataid(i);
                DataLine.Item2count = packet.GetItem2count(i);
                GameManager.gameManager.PlayerDataPool.AddOnlineAwardLine(DataLine);
            }
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
