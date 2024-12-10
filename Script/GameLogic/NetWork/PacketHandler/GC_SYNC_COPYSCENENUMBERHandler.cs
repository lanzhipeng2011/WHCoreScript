//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_SYNC_COPYSCENENUMBERHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_SYNC_COPYSCENENUMBER packet = (GC_SYNC_COPYSCENENUMBER )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 GameManager.gameManager.PlayerDataPool.CommonData.HandlePacket(packet);
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
