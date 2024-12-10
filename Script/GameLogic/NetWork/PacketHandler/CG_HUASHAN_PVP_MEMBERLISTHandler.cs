//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class CG_HUASHAN_PVP_MEMBERLISTHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 CG_HUASHAN_PVP_MEMBERLIST packet = (CG_HUASHAN_PVP_MEMBERLIST )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
