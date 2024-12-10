//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_RET_ACTIVE_LEICHONGHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_RET_ACTIVE_LEICHONG packet = (GC_RET_ACTIVE_LEICHONG )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic

			ActivityRechargeData.UpdateLeiChongData (packet);

 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
