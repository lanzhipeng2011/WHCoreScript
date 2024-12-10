//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
using Games.GlobeDefine;
namespace SPacket.SocketInstance
 {
 public class GC_COUNTDOWNHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_COUNTDOWN packet = (GC_COUNTDOWN )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 if (GameManager.gameManager.RunningScene != (int)GameDefine_Globe.SCENE_DEFINE.SCENE_YANMENGUANWAI) 
 {
     CountDownLogic.ShowCountDown(packet.CountDownSecond);
  }

 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
