//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
using ProtoCmd;

namespace SPacket.SocketInstance
 {
 public class GC_PAY_ORDERHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_PAY_ORDER packet = (GC_PAY_ORDER )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic

			PayDatas payD = CommonSDKPlaform.Instance.payDatas;

			payD.rmb = packet.Amount;
			payD.ext_data = packet.Sign;
			payD.out_order = packet.Plat_order;
			payD.notice_url = packet.Notice_url;
			payD.inner_order = packet.Game_order;
			payD.goods_id = packet.Goods_id;
			payD.subTime = packet.Create_time;
			payD.description = packet.Goods_desc;
			payD.productName = packet.Goods_name;
			payD.productRealPrice = (int)packet.Amount;
			payD.productIdealPrice = (int)packet.Amount;

			PlatformHelper.OnChargeRequest ("", "", 0f, "", 0f, "");


 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
