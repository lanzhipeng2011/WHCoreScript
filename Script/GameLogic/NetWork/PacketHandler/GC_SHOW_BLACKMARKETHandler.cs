//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_SHOW_BLACKMARKETHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_SHOW_BLACKMARKET packet = (GC_SHOW_BLACKMARKET )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
