//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_POWERUP_LISTHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_POWERUP_LIST packet = (GC_POWERUP_LIST )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 BePowerData.ShowBePowerWithData(packet);
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
