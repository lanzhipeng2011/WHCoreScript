//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_WORLDBOSS_TEAMLISTHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_WORLDBOSS_TEAMLIST packet = (GC_WORLDBOSS_TEAMLIST )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 HuaShanPVPData.ShowWorldBossList(packet);
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
