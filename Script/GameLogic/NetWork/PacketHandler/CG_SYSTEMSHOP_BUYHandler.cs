//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class CG_SYSTEMSHOP_BUYHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 CG_SYSTEMSHOP_BUY packet = (CG_SYSTEMSHOP_BUY )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
