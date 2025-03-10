//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
 {
     public class GC_ACCEPTMISSION_RETHandler : Ipacket
     {
         public uint Execute(PacketDistributed ipacket)
         {
             GC_ACCEPTMISSION_RET packet = (GC_ACCEPTMISSION_RET )ipacket;
             if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
             //enter your logic
			int nMissionID = (int)packet.MissionID;
             byte yQualityType = (byte)packet.QualityType;
             int nRet = packet.Ret;
             if (1== nRet)
             {
				GameManager.gameManager.MissionManager.AddMissionToUser(nMissionID, yQualityType);
				GameManager.gameManager.MissionManager.NotifyGuildMissionUI(nMissionID, "");
             }
             return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }
     }
 }
