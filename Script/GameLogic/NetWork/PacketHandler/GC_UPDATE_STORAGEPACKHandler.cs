//This code create by CodeEngine

using System;
using Games.Item;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_UPDATE_STORAGEPACKHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_UPDATE_STORAGEPACK packet = (GC_UPDATE_STORAGEPACK)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic

            int Count = packet.packindexCount;
            GameItemContainer Container = GameManager.gameManager.PlayerDataPool.StoragePack;
            for (int i = 0; i < Count; i++)
            {
                int packindex = packet.GetPackindex(i);
                GameItem item = Container.GetItem(packindex);
                if (item != null)
                {
                    item.DataID = packet.GetDataid(i);
                    item.Guid = packet.GetGuid(i);
                    item.BindFlag = (packet.GetBindflag(i) == 1);
                    item.EnchanceLevel = packet.GetEnchancelevel(i);
                    item.EnchanceExp = packet.GetEnchanceexp(i);
                    item.EnchanceTotalExp = packet.GetEnchancetotalexp(i);
                    item.StarLevel = packet.GetStarlevel(i);
                    item.StarTimes = packet.GetStartimes(i);
                    item.StackCount = packet.GetStackcount(i);
                    item.CreateTime = packet.GetCreatetime(i);
                    item.DynamicData[0] = packet.GetDynamicdata1(i);
                    item.DynamicData[1] = packet.GetDynamicdata2(i);
                    item.DynamicData[2] = packet.GetDynamicdata3(i);
                    item.DynamicData[3] = packet.GetDynamicdata4(i);
                    item.DynamicData[4] = packet.GetDynamicdata5(i);
                    item.DynamicData[5] = packet.GetDynamicdata6(i);
                    item.DynamicData[6] = packet.GetDynamicdata7(i);
                    item.DynamicData[7] = packet.GetDynamicdata8(i);
                }
            }

            if (CangKuLogic.Instance() != null)
            {
                CangKuLogic.Instance().UpdateCangKu();
            }

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
