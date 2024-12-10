//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
using Games.Item;
namespace SPacket.SocketInstance
{
    public class GC_UPDATE_GEM_INFOHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_UPDATE_GEM_INFO packet = (GC_UPDATE_GEM_INFO)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;

            int gemCount = packet.gemidCount;
            
            for (int i = 0; i < (int)EquipPackSlot.Slot_NUM; i++)
            {
                for (int index = 0; index < (int)GemSlot.OPEN_NUM; index++)
                {
                    int totalIndex = i * (int)GemSlot.OPEN_NUM + index;
                    if (totalIndex >= 0 && totalIndex < gemCount)
                    {
                        int gemId = packet.GetGemid(totalIndex);
                        GameManager.gameManager.PlayerDataPool.GemData.SetGemId(i, index, gemId);
                    }
                }
            }

            //if (GemLogic.Instance())
            //{
            //    GemLogic.Instance().UpdateGemSlot();
            //}

            //enter your logic
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
