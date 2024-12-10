//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_EQUIP_FELLOW_SKILL_RETHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_EQUIP_FELLOW_SKILL_RET packet = (GC_EQUIP_FELLOW_SKILL_RET )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
