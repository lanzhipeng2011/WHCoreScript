//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_WORLDBOSS_OPENHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_WORLDBOSS_OPEN packet = (GC_WORLDBOSS_OPEN )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 HuaShanPVPData.WorldBossOpen = 0;
 if (packet.State == 2)
     HuaShanPVPData.WorldBossOpen = 1;
 if (FunctionButtonLogic.Instance() != null)
     FunctionButtonLogic.Instance().OnWorldBossStateChange(packet.State);
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
