//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class CG_REQ_TEAM_INVITEHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 CG_REQ_TEAM_INVITE packet = (CG_REQ_TEAM_INVITE )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
