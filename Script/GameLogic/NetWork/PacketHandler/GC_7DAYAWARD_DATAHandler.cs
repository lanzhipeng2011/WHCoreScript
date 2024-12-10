//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_7DAYAWARD_DATAHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_7DAYAWARD_DATA packet = (GC_7DAYAWARD_DATA )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
			GameManager.gameManager.PlayerDataPool.HandlePacket(packet);
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
