//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class CG_PUT_ITEM_STORAGEPACKHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 CG_PUT_ITEM_STORAGEPACK packet = (CG_PUT_ITEM_STORAGEPACK )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
