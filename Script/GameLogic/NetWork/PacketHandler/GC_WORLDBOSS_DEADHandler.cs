//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_WORLDBOSS_DEADHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_WORLDBOSS_DEAD packet = (GC_WORLDBOSS_DEAD )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 if (FunctionButtonLogic.Instance() != null)
     FunctionButtonLogic.Instance().OnWorldBossDead();
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
