//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class CG_BUY_FASHIONHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 CG_BUY_FASHION packet = (CG_BUY_FASHION )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
