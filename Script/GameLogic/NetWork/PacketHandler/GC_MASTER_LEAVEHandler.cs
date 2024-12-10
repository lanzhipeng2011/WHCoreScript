//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_MASTER_LEAVEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_MASTER_LEAVE packet = (GC_MASTER_LEAVE)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;

            int leavlType = packet.LevelType;
            if (leavlType == 1)
            {
                //主动离开
                if (MasterWindow.Instance())
                {
                    UIManager.CloseUI(UIInfo.MasterAndGuildRoot);
                }
            }
            else if (leavlType == 2)
            {
                //被踢出师门
                if (MasterWindow.Instance())
                {
                    UIManager.CloseUI(UIInfo.MasterAndGuildRoot);
                }
            }
            else if (leavlType == 3)
            {
                //师门解散
                if (MasterWindow.Instance())
                {
                    UIManager.CloseUI(UIInfo.MasterAndGuildRoot);
                }
            }
            else if (leavlType == 4)
            {
                //申请被拒绝
            }

            //清空客户端缓存数据
            GameManager.gameManager.PlayerDataPool.MasterInfo.CleanUp();

            //enter your logic
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
