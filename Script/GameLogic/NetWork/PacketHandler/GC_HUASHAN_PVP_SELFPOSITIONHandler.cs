//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_HUASHAN_PVP_SELFPOSITIONHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_HUASHAN_PVP_SELFPOSITION packet = (GC_HUASHAN_PVP_SELFPOSITION )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 //HuaShanPVPData.ShowSelfRegisterInfo(packet);
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
