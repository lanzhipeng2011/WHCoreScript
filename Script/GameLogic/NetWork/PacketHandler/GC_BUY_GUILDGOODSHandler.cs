//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_BUY_GUILDGOODSHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_BUY_GUILDGOODS packet = (GC_BUY_GUILDGOODS)ipacket;
            if (null == packet) 
                return (uint)PACKET_EXE.PACKET_EXE_ERROR;

            //进行提示
            if (null != Singleton<ObjManager>.GetInstance().MainPlayer)
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2404}");
            }

            //更新剩余帮贡
            GuildMember member = GameManager.gameManager.PlayerDataPool.GuildInfo.GetMainPlayerGuildInfo();
            if (null != member)
            {
                member.Contribute -= packet.Cost;
                if (member.Contribute < 0)
                {
                    member.Contribute = 0;
                }
            }

            //更新帮会商店界面
            if (null != GuildWindow.Instance())
            {
                GuildWindow.Instance().UpdateGuildShopContribute();
            }

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
