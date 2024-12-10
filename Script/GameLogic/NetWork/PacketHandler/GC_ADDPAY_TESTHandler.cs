//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_ADDPAY_TESTHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_ADDPAY_TEST packet = (GC_ADDPAY_TEST)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;

            if (RechargeController.Instance() != null)
            {
                PlatformHelper.OnChargeSuccess(RechargeController.Instance().PayOrderId);
            }
            GameManager.gameManager.PlayerDataPool.VipCost = packet.Vipcost;
            if (ObjManager.Instance.MainPlayer != null)
            {
                ObjManager.Instance.MainPlayer.VipCost = packet.Vipcost;
            }

            //enter your logic
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
