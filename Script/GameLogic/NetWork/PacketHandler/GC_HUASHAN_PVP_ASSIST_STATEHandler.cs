//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_HUASHAN_PVP_ASSIST_STATEHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_HUASHAN_PVP_ASSIST_STATE packet = (GC_HUASHAN_PVP_ASSIST_STATE )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 if (FunctionButtonLogic.Instance() != null)
     FunctionButtonLogic.Instance().ZhenQiAssistState( packet.State, packet.Times );
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
