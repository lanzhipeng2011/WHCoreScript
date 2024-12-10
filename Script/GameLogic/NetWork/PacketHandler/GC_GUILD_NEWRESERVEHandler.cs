//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_GUILD_NEWRESERVEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_GUILD_NEWRESERVE packet = (GC_GUILD_NEWRESERVE)ipacket;
            if (null == packet) 
                return (uint)PACKET_EXE.PACKET_EXE_ERROR;

            //enter your logic
            if (Singleton<ObjManager>.GetInstance().MainPlayer != null)
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.ShowGuildNewReserveFlag = true;
                if (null != PlayerFrameLogic.Instance())
                {
                    PlayerFrameLogic.Instance().UpdateRemainNum();
                }

                if (MenuBarLogic.Instance() != null)
                {
                    MenuBarLogic.Instance().UpdateGuildAndMasterReserveMember();
                }
            }

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
