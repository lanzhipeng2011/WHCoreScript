//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_DELHATELISTHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_DELHATELIST packet = (GC_DELHATELIST)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic

            if (null != GameManager.gameManager.PlayerDataPool.HateList)
            {
                GameManager.gameManager.PlayerDataPool.HateList.DelRelation(packet.Guid);
            }


            if (null != GUIData.delFriendDataUpdate) GUIData.delFriendDataUpdate();
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
