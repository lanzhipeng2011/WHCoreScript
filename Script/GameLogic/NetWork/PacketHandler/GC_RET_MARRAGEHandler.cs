//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_RET_MARRAGEHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_RET_MARRAGE packet = (GC_RET_MARRAGE )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
			MarryRootLogic.Handler (packet);
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
