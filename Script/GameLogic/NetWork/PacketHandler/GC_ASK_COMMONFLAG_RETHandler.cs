//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_ASK_COMMONFLAG_RETHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_ASK_COMMONFLAG_RET packet = (GC_ASK_COMMONFLAG_RET )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            GameManager.gameManager.PlayerDataPool.CommonData.HandlePacket(packet);
            if (null != AwardLogic.Instance()) AwardLogic.Instance().UpdateTip();
            if (null != FunctionButtonLogic.Instance())
            {
                FunctionButtonLogic.Instance().UpdateButtonAwardTips();
            }
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
