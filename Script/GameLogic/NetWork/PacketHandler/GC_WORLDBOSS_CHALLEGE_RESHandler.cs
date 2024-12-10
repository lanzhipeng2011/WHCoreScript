//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_WORLDBOSS_CHALLEGE_RESHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_WORLDBOSS_CHALLEGE_RES packet = (GC_WORLDBOSS_CHALLEGE_RES )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
