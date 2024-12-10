//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class CG_COMBATVALUE_ASKHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 CG_COMBATVALUE_ASK packet = (CG_COMBATVALUE_ASK )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
