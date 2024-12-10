//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class CG_JUQINGITEM_PLAYEFFECTHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 CG_JUQINGITEM_PLAYEFFECT packet = (CG_JUQINGITEM_PLAYEFFECT )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
