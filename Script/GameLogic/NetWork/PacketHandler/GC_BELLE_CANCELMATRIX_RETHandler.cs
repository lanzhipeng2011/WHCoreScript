//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_BELLE_CANCELMATRIX_RETHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_BELLE_CANCELMATRIX_RET packet = (GC_BELLE_CANCELMATRIX_RET )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
			BelleData.UpdateUnActiveMatrixData(packet);
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
