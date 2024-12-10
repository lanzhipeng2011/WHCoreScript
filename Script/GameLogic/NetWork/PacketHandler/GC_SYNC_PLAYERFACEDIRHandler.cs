//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_SYNC_PLAYERFACEDIRHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_SYNC_PLAYERFACEDIR packet = (GC_SYNC_PLAYERFACEDIR )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
