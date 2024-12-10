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

            //���º��ѽ��棨���δ�򿪣���UpdateFriendList������
            if (null != GUIData.delFriendDataUpdate) GUIData.delFriendDataUpdate();

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
