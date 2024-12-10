//This code create by CodeEngine

using System;
using Games.Item;
using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;

namespace SPacket.SocketInstance
{
    public class GC_SHOW_USEITEMREMINDHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_SHOW_USEITEMREMIND packet = (GC_SHOW_USEITEMREMIND) ipacket;
            if (null == packet) return (uint) PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            GameItem item = new GameItem();
            item.Guid = packet.ItemGUID;
            item.DataID = packet.ItemID;
//            UseItemRemindLogic.InitUseItemInfo(item);

			//====目前仅能用于坐骑弹窗TODO
			if(item.DataID == 59046)
			{
				EquipRemindLogic.InitEquipInfo(item);
			}
            return (uint) PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
