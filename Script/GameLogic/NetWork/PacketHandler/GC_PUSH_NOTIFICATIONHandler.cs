//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
     public class GC_PUSH_NOTIFICATIONHandler : Ipacket
     {
         public uint Execute(PacketDistributed ipacket)
         {
             GC_PUSH_NOTIFICATION packet = (GC_PUSH_NOTIFICATION )ipacket;
             if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
             //enter your logic
             //LogModule.DebugLog("GC_PUSH_NOTIFICATIONHandler.strat");
             for (int i = 0; i < packet.newsCount && i < packet.dataCount && i < packet.RepeatCount; i++ )
             {
//                  LogModule.DebugLog("GC_PUSH_NOTIFICATIONHandler:addPushNotificationInfo."
//                      + "packet.GetNews:" + packet.GetNews(i).ToString() + "packet.GetData:" + packet.GetData(i).ToString() + "packet.GetRepeat:" + packet.GetRepeat(i).ToString());
                 PushNotification.addPushNotificationInfo(packet.GetNews(i), packet.GetData(i), packet.GetRepeat(i));
             }      
            
             return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }
     }
 }
