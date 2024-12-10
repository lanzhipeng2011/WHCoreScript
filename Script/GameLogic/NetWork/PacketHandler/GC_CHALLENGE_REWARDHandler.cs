//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_CHALLENGE_REWARDHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_CHALLENGE_REWARD packet = (GC_CHALLENGE_REWARD )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 PVPData.UpdateChallengeReward(packet);
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
