//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_RES_POWERUPHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_RES_POWERUP packet = (GC_RES_POWERUP )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 BePowerData.ReciveResPowerData(packet);
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
