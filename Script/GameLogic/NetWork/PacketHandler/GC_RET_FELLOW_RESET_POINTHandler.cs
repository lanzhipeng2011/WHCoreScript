//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_RET_FELLOW_RESET_POINTHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_RET_FELLOW_RESET_POINT packet = (GC_RET_FELLOW_RESET_POINT )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
