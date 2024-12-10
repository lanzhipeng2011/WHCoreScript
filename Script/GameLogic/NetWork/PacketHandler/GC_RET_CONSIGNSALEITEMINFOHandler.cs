//This code create by CodeEngine

using System;
using Games.ConsignSale;
using Games.LogicObj;
using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
    public class GC_RET_CONSIGNSALEITEMINFOHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_RET_CONSIGNSALEITEMINFO packet = (GC_RET_CONSIGNSALEITEMINFO )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
            if (_mainPlayer ==null)
            {
                return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
            }
            if (ConsignSaleLogic.Instance() !=null)
            {
				ConsignSaleLogic.Instance().BuyItemInfo.Clear();
                int nCurPage = 0;
                if (packet.dataidCount <=0 && packet.Curpage !=0)//查看下一页或者上一页 未找到更多的信息 则不清除当前信息
                {
                    nCurPage = packet.Curpage - 1;
                    _mainPlayer.SendNoticMsg(false, "#{1693}");
                }
                else
                {
                    nCurPage = packet.Curpage;
                    ConsignSaleLogic.Instance().BuyItemInfo.Clear();
                    for (int i = 0; i < packet.dataidCount; i++)
                    {
                        ConsignSaleSearchInfo searchInfo = new ConsignSaleSearchInfo();
                        searchInfo.CleanUp();
                        searchInfo.ItemInfo.DataID = packet.GetDataid(i);
                        searchInfo.ItemInfo.Guid = packet.GetGuid(i);
                        searchInfo.ItemInfo.BindFlag = (packet.GetBindflag(i) == 1 ? true : false);
                        searchInfo.ItemInfo.StackCount = packet.GetStackcount(i);
                        searchInfo.ItemInfo.EnchanceLevel = packet.GetEnchancelevel(i);
                        searchInfo.ItemInfo.StarLevel = packet.GetStarlevel(i);
                        searchInfo.ItemInfo.DynamicData[0] = packet.GetDynamicdata1(i);
                        searchInfo.ItemInfo.DynamicData[1] = packet.GetDynamicdata2(i);
                        searchInfo.ItemInfo.DynamicData[2] = packet.GetDynamicdata3(i);
                        searchInfo.ItemInfo.DynamicData[3] = packet.GetDynamicdata4(i);
                        searchInfo.ItemInfo.DynamicData[4] = packet.GetDynamicdata5(i);
                        searchInfo.ItemInfo.DynamicData[5] = packet.GetDynamicdata6(i);
                        searchInfo.ItemInfo.DynamicData[6] = packet.GetDynamicdata7(i);
                        searchInfo.ItemInfo.DynamicData[7] = packet.GetDynamicdata8(i);
                        searchInfo.ItemInfo.EnchanceExp = packet.GetEnchanceexp(i);
                        searchInfo.ItemInfo.EnchanceTotalExp = packet.GetEnchancetotalexp(i);
                        searchInfo.ItemInfo.StarTimes = packet.GetStartimes(i);
                        searchInfo.Price = packet.GetTotalprice(i);
                        searchInfo.OwnerName = packet.GetOwnername(i);
                        ConsignSaleLogic.Instance().BuyItemInfo.Add(searchInfo);
                    }
                }
                ConsignSaleLogic.Instance().CurBuyPage =nCurPage;
                ConsignSaleLogic.Instance().UpdateBuyInfo();
            }
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
 }
