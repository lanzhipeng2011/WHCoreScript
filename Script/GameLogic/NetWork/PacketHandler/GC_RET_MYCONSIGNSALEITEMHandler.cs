//This code create by CodeEngine

using System;
using Games.ConsignSale;
using Games.LogicObj;
using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
    public class GC_RET_MYCONSIGNSALEITEMHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_RET_MYCONSIGNSALEITEM packet = (GC_RET_MYCONSIGNSALEITEM )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
            if (_mainPlayer == null)
            {
                return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
            }
            int nCurPage = 0;
            if (ConsignSaleLogic.Instance() != null)
            {
                if (packet.dataidCount <= 0 && packet.Curpage != 0)//查看下一页 未找到更多的信息 则不清除当前信息
                {
                    nCurPage = packet.Curpage - 1;
                    _mainPlayer.SendNoticMsg(false, "#{1693}");
                }
                else
                {
                    nCurPage = packet.Curpage;
                    ConsignSaleLogic.Instance().SaleItemInfo.Clear();
                    for (int i = 0; i < packet.dataidCount; i++)
                    {
                        MyConsignSaleItemInfo searchInfo = new MyConsignSaleItemInfo();
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
                        searchInfo.RemainTime = packet.GetRemainTime(i);
                        searchInfo.ItemInfo.EnchanceExp = packet.GetEnchanceexp(i);
                        searchInfo.ItemInfo.EnchanceTotalExp = packet.GetEnchancetotalexp(i);
                        searchInfo.ItemInfo.StarTimes = packet.GetStartimes(i);
                        searchInfo.Price = packet.GetTotalprice(i);
                        ConsignSaleLogic.Instance().SaleItemInfo.Add(searchInfo);
                    }
                }
                ConsignSaleLogic.Instance().CurSalePage = nCurPage;
                ConsignSaleLogic.Instance().CurSaleCount= packet.Cursalecount;
                ConsignSaleLogic.Instance().MaxSaleCount= packet.Maxsalecount;
                ConsignSaleLogic.Instance().UpdateSaleInfo();
                //if (ConsignSaleLogic.Instance().m_SaleBag.gameObject.activeInHierarchy)//如果背包打开 则更新背包
                {
                    ConsignSaleLogic.Instance().m_SaleBag.gameObject.GetComponent<ConsignSaleBag>().UpdateBackPack();
                }
            }
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
 }
