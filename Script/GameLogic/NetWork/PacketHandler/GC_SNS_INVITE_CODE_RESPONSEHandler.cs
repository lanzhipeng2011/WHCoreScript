//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_SNS_INVITE_CODE_RESPONSEHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_SNS_INVITE_CODE_RESPONSE packet = (GC_SNS_INVITE_CODE_RESPONSE )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
			if (packet.Result == (int)GC_SNS_INVITE_CODE_RESPONSE.RETTYPE.OK_ICODE) 
			{
				UIManager.CloseUI(UIInfo.SNSShareCodeRoot);
			}
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
