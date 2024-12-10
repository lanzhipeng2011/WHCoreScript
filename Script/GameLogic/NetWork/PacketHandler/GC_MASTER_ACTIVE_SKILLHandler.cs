//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_MASTER_ACTIVE_SKILLHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_MASTER_ACTIVE_SKILL packet = (GC_MASTER_ACTIVE_SKILL )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
