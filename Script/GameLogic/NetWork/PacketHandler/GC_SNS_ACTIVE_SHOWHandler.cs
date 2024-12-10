//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_SNS_ACTIVE_SHOWHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_SNS_ACTIVE_SHOW packet = (GC_SNS_ACTIVE_SHOW )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 PVPData.OpenSNSWindows();
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }


 }
 }
