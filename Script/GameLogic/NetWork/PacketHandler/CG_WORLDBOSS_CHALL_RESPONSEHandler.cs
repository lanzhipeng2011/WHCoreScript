//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class CG_WORLDBOSS_CHALL_RESPONSEHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 CG_WORLDBOSS_CHALL_RESPONSE packet = (CG_WORLDBOSS_CHALL_RESPONSE )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
