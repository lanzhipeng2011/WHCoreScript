//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_SYC_FRIEND_INFOHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_SYC_FRIEND_INFO packet = (GC_SYC_FRIEND_INFO)ipacket;
            if (null == packet) 
                return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            
            if (null != GameManager.gameManager.PlayerDataPool.FriendList)
            {
                Relation _relation = new Relation();
                _relation.Guid = packet.Guid;
                _relation.Name = packet.Name;
                _relation.Level = packet.Level;
                _relation.Profession = packet.Prof;
                _relation.CombatNum = packet.Combat;
                _relation.State = packet.State;

                GameManager.gameManager.PlayerDataPool.FriendList.UpdateRelation(_relation);
            }

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
