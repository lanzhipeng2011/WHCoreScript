//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_WORLDBOSS_SOMECHALL_MEHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_WORLDBOSS_SOMECHALL_ME packet = (GC_WORLDBOSS_SOMECHALL_ME )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 HuaShanPVPData.ShowWorldBossChallengeBox(packet);
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
