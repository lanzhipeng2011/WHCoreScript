//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class CG_DAILYMISSION_UPDATEHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 CG_DAILYMISSION_UPDATE packet = (CG_DAILYMISSION_UPDATE )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
