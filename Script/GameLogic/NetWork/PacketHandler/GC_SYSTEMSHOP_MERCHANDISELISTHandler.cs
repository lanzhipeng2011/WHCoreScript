//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_SYSTEMSHOP_MERCHANDISELISTHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_SYSTEMSHOP_MERCHANDISELIST packet = (GC_SYSTEMSHOP_MERCHANDISELIST )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
