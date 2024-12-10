//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_HUASHAN_PVP_STATEHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_HUASHAN_PVP_STATE packet = (GC_HUASHAN_PVP_STATE )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 HuaShanPVPData.DealHuaShanPvPState(packet);
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
