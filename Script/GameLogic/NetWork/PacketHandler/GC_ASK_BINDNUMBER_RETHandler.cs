//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_ASK_BINDNUMBER_RETHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_ASK_BINDNUMBER_RET packet = (GC_ASK_BINDNUMBER_RET )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic

			int strTipID = 0;
			switch(packet.Result)
			{
			case 1:
				strTipID = 6011;
				break;
			case 2:
				strTipID = 6012;
				break;
			case 3:
				strTipID = 6013;
				break;
			}
			MessageBoxLogic.OpenOKBox(strTipID);

 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
