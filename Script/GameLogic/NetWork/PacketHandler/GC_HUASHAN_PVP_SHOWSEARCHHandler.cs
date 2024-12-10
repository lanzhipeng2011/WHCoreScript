//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_HUASHAN_PVP_SHOWSEARCHHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_HUASHAN_PVP_ShowSearch packet = (GC_HUASHAN_PVP_ShowSearch )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 HuaShanPVPData.ShowSearchOpponent(packet);
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
