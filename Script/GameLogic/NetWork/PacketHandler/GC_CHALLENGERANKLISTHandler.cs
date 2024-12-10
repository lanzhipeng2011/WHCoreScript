//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_CHALLENGERANKLISTHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_CHALLENGERANKLIST packet = (GC_CHALLENGERANKLIST )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 PVPData.UpdatePvPRankList(packet);
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
