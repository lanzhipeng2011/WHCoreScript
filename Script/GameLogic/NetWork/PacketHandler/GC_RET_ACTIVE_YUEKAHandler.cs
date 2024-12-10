//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_RET_ACTIVE_YUEKAHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_RET_ACTIVE_YUEKA packet = (GC_RET_ACTIVE_YUEKA )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
			for(int i = 0;i < packet.typeCount; i++)
			{
				switch(packet.typeList[i])
				{
				case 1:
					GameManager.gameManager.PlayerDataPool.WeekDay = (int)(packet.dayList[i]);
					break;
				case 2:
					GameManager.gameManager.PlayerDataPool.MonthDay =  (int)(packet.dayList[i]);
					break;
				}
			}


 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
