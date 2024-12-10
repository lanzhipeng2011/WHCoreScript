//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
     public class GC_SYNC_NEW7DAYONLINEAWARDTABLEHandler : Ipacket
     {
         public uint Execute(PacketDistributed ipacket)
         {
             GC_SYNC_NEW7DAYONLINEAWARDTABLE packet = (GC_SYNC_NEW7DAYONLINEAWARDTABLE )ipacket;
             if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
             //enter your logic
             GameManager.gameManager.PlayerDataPool.HandlePacket(packet);
             return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }
     }
 }
