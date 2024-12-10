//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class CG_FELLOW_RESET_POINTHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 CG_FELLOW_RESET_POINT packet = (CG_FELLOW_RESET_POINT )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
