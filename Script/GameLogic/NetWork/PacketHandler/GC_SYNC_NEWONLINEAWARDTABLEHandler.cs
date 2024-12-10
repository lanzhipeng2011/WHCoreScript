//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using Games.AwardActivity;
namespace SPacket.SocketInstance
 {
     public class GC_SYNC_NEWONLINEAWARDTABLEHandler : Ipacket
     {
         public uint Execute(PacketDistributed ipacket)
         {
             GC_SYNC_NEWONLINEAWARDTABLE packet = (GC_SYNC_NEWONLINEAWARDTABLE )ipacket;
             if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
             //enter your logic
             bool isShow = packet.IsShow > 0;
             GameManager.gameManager.PlayerDataPool.ShouNowOnlineAwardWindow = isShow;
             if (isShow)
             {
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
                     GameManager.gameManager.PlayerDataPool.AddNewOnlineAwardLine(DataLine);

                     if (packet.HasStartDate)
                     {
                         GameManager.gameManager.PlayerDataPool.m_sNewOnlineDateTime.StartDate = packet.StartDate;
                     }
                     if (packet.HasEndDate)
                     {
                         GameManager.gameManager.PlayerDataPool.m_sNewOnlineDateTime.EndDate = packet.EndDate;
                     }
                     if (packet.HasStartTime)
                     {
                         GameManager.gameManager.PlayerDataPool.m_sNewOnlineDateTime.StartTime = packet.StartTime;
                     }
                     if (packet.HasEndTime)
                     {
                         GameManager.gameManager.PlayerDataPool.m_sNewOnlineDateTime.EndTime = packet.EndTime;
                     }
                 }
             }
             
             return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }
     }
 }
