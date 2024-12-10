//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
     public class GC_DAILYLUCKYDRAW_UPDATEHandler : Ipacket
     {
         public uint Execute(PacketDistributed ipacket)
         {
             GC_DAILYLUCKYDRAW_UPDATE packet = (GC_DAILYLUCKYDRAW_UPDATE )ipacket;
             if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
             //enter your logic
             GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.HandlePacket(packet);
             return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }
     }
 }
