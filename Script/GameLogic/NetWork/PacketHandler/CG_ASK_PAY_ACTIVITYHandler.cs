//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class CG_ASK_PAY_ACTIVITYHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 CG_ASK_PAY_ACTIVITY packet = (CG_ASK_PAY_ACTIVITY )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
