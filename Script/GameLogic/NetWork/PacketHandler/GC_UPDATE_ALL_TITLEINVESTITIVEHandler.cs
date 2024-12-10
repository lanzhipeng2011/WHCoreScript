//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using Games.LogicObj;

namespace SPacket.SocketInstance
 {
     public class GC_UPDATE_ALL_TITLEINVESTITIVEHandler : Ipacket
     {
         public uint Execute(PacketDistributed ipacket)
         {
             GC_UPDATE_ALL_TITLEINVESTITIVE packet = (GC_UPDATE_ALL_TITLEINVESTITIVE )ipacket;
             if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
             //enter your logic
             GameManager.gameManager.PlayerDataPool.TitleInvestitive.ReadAllTitleInvestitive(packet);
             GameManager.gameManager.PlayerDataPool.IsLockPriorTitle = packet.IsLockTitle == 1 ? true : false;
             return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }
     }
 }
