//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class CG_MONEYTREE_ASKAWARDHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 CG_MONEYTREE_ASKAWARD packet = (CG_MONEYTREE_ASKAWARD )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
