//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_RET_LOCK_TITLEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_RET_LOCK_TITLE packet = (GC_RET_LOCK_TITLE)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            GameManager.gameManager.PlayerDataPool.IsLockPriorTitle = packet.IsLock == 1 ? true : false;
            if (TitleInvestitiveLogic.Instance() != null)
            {
                TitleInvestitiveLogic.Instance().UpdateLockButton();
            }
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
