//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class CG_STORAGEPACK_UNLOCKHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 CG_STORAGEPACK_UNLOCK packet = (CG_STORAGEPACK_UNLOCK )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
