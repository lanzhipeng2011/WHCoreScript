//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_MERCENARY_LEFTTIMESHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_MERCENARY_LEFTTIMES packet = (GC_MERCENARY_LEFTTIMES )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 HuaShanPVPData.SetMercenaryLeftTimes(packet);
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
