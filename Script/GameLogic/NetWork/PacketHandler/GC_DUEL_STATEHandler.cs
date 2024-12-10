//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_DUEL_STATEHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_DUEL_STATE packet = (GC_DUEL_STATE )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 HuaShanPVPData.DealDuelState(packet);
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
