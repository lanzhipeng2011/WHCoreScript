//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class CG_ENTER_SCENE_OKHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 CG_ENTER_SCENE_OK packet = (CG_ENTER_SCENE_OK )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
