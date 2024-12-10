//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_GUILD_CREATEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_GUILD_CREATE packet = (GC_GUILD_CREATE)ipacket;
            if (null == packet) return 
                (uint)PACKET_EXE.PACKET_EXE_ERROR;

            //enter your logic
            GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid = packet.GuildGuid;

            //֪ͨ��Ҵ������ɹ�
            if (Singleton<ObjManager>.GetInstance().MainPlayer)
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1779}");      //��ᴴ���ɹ�
            }

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
