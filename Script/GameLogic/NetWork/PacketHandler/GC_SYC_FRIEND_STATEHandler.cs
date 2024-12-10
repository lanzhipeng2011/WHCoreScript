//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_SYC_FRIEND_STATEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_SYC_FRIEND_STATE packet = (GC_SYC_FRIEND_STATE)ipacket;
            if (null == packet) 
                return (uint)PACKET_EXE.PACKET_EXE_ERROR;

            if (null != GameManager.gameManager.PlayerDataPool.FriendList)
            {
                GameManager.gameManager.PlayerDataPool.FriendList.UpdateRelationState(packet.Guid, packet.State);
            }

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
