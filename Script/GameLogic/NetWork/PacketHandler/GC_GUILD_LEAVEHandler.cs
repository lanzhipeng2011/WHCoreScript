//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_GUILD_LEAVEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_GUILD_LEAVE packet = (GC_GUILD_LEAVE )ipacket;
            if (null == packet) 
                return (uint)PACKET_EXE.PACKET_EXE_ERROR;

            //离开帮会，清空个人帮会信息
            GameManager.gameManager.PlayerDataPool.GuildInfo.CleanUp();
            //更新帮会界面（如果未打开，则不处理）
            if (null != GUIData.delGuildDataUpdate)
                GUIData.delGuildDataUpdate();

            if (ChatInfoLogic.Instance() != null)
            {
                ChatInfoLogic.Instance().UpdateTeamAndGuildChannel();
            }

            if (null != PopMenuLogic.Instance())
            {
                PopMenuLogic.ClosePop();
            }

            UIManager.CloseUI(UIInfo.MasterAndGuildRoot);
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
