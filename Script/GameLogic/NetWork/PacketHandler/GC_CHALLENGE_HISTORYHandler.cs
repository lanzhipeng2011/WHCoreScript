//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_CHALLENGE_HISTORYHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_CHALLENGE_HISTORY packet = (GC_CHALLENGE_HISTORY )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 PVPData.UpdateChallengeHistory(packet);
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
