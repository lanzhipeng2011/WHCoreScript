//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
using GCGame.Table;
namespace SPacket.SocketInstance
 {
 public class GC_RET_COPYSCENE_SWEEPHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_RET_COPYSCENE_SWEEP packet = (GC_RET_COPYSCENE_SWEEP )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;

			if (packet.HasResult)
			{
				for(int i=0; i<packet.SweepTypeCount; i++)
				{
//					GUIData.AddNotifyData(packet.Notice, IsFilterRepeat);

					Tab_CommonItem  item=TableManager.GetCommonItemByID(packet.SweepTypeList[i],0);
					int numbers = packet.SweepCountList[i];
					string str = item.Name +"  " +numbers.ToString();
					GUIData.AddNotifyData2Client(false,str);
				}
			}
 //enter your logic
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
