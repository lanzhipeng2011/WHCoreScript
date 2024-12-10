//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_RET_FANKUIHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_RET_FANKUI packet = (GC_RET_FANKUI )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
			MessageBoxLogic.OpenOKBox(6018);
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
