//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
 {
     public class GC_MISSION_PARAMHandler : Ipacket
     {
         public uint Execute(PacketDistributed ipacket)
         {
             GC_MISSION_PARAM packet = (GC_MISSION_PARAM )ipacket;
             if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
             //enter your logic
             int nMission = packet.MissionID;
             int nParamIndex = packet.ParamIndex;
             int nParam = packet.Param;
             GameManager.gameManager.MissionManager.SetMissionParam(nMission, nParamIndex, nParam);
             return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }
     }
 }
