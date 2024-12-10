//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
     public class GC_UPDATENOTICEDATAHandler : Ipacket
     {
         public uint Execute(PacketDistributed ipacket)
         {
             GC_UPDATENOTICEDATA packet = (GC_UPDATENOTICEDATA )ipacket;
             if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
             //enter your logic
             NoticeWindow.ClearNotice();
             for (int i = 0; i < packet.dataCount && i < packet.newsCount; i++)
             {
                 NoticeWindow.addNotice(packet.GetNews(i),packet.GetData(i));
             }
             if (packet.HasIsShow)
             {
                 if (packet.IsShow == 1)
                 {
                     NoticeLogic.IsReceiveData = true;
                 }                
             }

            
             return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }
     }
 }
