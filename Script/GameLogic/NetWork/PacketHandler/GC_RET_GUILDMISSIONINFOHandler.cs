//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_RET_GUILDMISSIONINFOHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
     GC_RET_GUILDMISSIONINFO packet = (GC_RET_GUILDMISSIONINFO )ipacket;
     if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
     //enter your logic
     if (null != GameManager.gameManager.PlayerDataPool.GuildInfo)
     {
         GameManager.gameManager.PlayerDataPool.GuildInfo.UpdateGuildMissionInfo(packet);
     }
     if (null != GuildMissionLogic.Instance())
     {
         GuildMissionLogic.Instance().UpdateGuildMission();
     }

     return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
