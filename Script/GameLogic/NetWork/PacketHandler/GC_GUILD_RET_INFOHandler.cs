//This code create by CodeEngine
using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_GUILD_RET_INFOHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_GUILD_RET_INFO packet = (GC_GUILD_RET_INFO)ipacket;
            if (null == packet) return 
                (uint)PACKET_EXE.PACKET_EXE_ERROR;

            if (null != GameManager.gameManager.PlayerDataPool.GuildInfo)
            {
                GameManager.gameManager.PlayerDataPool.GuildInfo.UpdateData(packet);
            }

            //更新帮会界面（如果未打开，则不处理）
            if (null != GUIData.delGuildDataUpdate)
                GUIData.delGuildDataUpdate();

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
