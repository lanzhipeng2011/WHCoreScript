//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using Games.GlobeDefine;
namespace SPacket.SocketInstance
{
    public class GC_UPDATE_STAMINA_TIMEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_UPDATE_STAMINA_TIME packet = (GC_UPDATE_STAMINA_TIME)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            GameManager.gameManager.PlayerDataPool.StaminaCountDown = packet.RemainTime;
            if (LivingSkillLogic.Instance() != null)
            {
                LivingSkillLogic.Instance().UpdateCountDownLabel();
            }
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
