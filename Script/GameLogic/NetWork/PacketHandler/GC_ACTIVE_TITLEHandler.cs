//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;

namespace SPacket.SocketInstance
{
    public class GC_ACTIVE_TITLEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_ACTIVE_TITLE packet = (GC_ACTIVE_TITLE )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            int nActiveTitle = packet.TitleIndex;
            GameManager.gameManager.PlayerDataPool.TitleInvestitive.ChangeTitle(nActiveTitle);
            if (Singleton<ObjManager>.Instance.MainPlayer)
            {
                Singleton<ObjManager>.Instance.MainPlayer.ShowPlayerTitleInvestitive();
            }            
            if (TitleInvestitiveLogic.Instance() != null)
            {
                TitleInvestitiveLogic.Instance().HandleActiveTitle();
            }
            
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
