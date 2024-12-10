//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
     public class GC_DAILYLUCKYDRAW_GAINBONUSHandler : Ipacket
     {
         public uint Execute(PacketDistributed ipacket)
         {
             GC_DAILYLUCKYDRAW_GAINBONUS packet = (GC_DAILYLUCKYDRAW_GAINBONUS )ipacket;
             if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
             //enter your logic
             GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.HandlePacket(packet);

             return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
     }
 }
