//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
 {
     public class GC_MISSION_STATEHandler : Ipacket
     {
         public uint Execute(PacketDistributed ipacket)
         {
             GC_MISSION_STATE packet = (GC_MISSION_STATE )ipacket;
             if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
             //enter your logic
             int nMissionID = packet.MissionID;
             int nState = packet.State;
             GameManager.gameManager.MissionManager.SetMissionState(nMissionID, (byte)nState);
             return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }
     }
 }
