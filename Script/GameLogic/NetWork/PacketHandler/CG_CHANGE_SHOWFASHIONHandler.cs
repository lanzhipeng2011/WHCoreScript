//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class CG_CHANGE_SHOWFASHIONHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 CG_CHANGE_SHOWFASHION packet = (CG_CHANGE_SHOWFASHION )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
