//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class CG_EQUIP_ENCHANCEHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 CG_EQUIP_ENCHANCE packet = (CG_EQUIP_ENCHANCE )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
