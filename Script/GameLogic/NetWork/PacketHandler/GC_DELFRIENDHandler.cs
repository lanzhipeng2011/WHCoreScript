//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_DELFRIENDHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_DELFRIEND packet = (GC_DELFRIEND)ipacket;
            if (null == packet) 
                return (uint)PACKET_EXE.PACKET_EXE_ERROR;

            if (null != GameManager.gameManager.PlayerDataPool.FriendList)
            {
                GameManager.gameManager.PlayerDataPool.FriendList.DelRelation(packet.Guid);
            }

            //更新好友界面（如果未打开，则UpdateFriendList不处理）
            if (null != GUIData.delFriendDataUpdate) GUIData.delFriendDataUpdate();

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
