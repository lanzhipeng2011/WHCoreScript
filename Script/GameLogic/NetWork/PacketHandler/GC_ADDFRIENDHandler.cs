//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_ADDFRIENDHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_ADDFRIEND packet = (GC_ADDFRIEND)ipacket;
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

                if (GameManager.gameManager.PlayerDataPool.FriendList.AddRelation(_relation))
                {
                    if (Singleton<ObjManager>.Instance.MainPlayer != null)
                    {
                        Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2393}");
                    }
                }
            }

            //更新好友界面（如果未打开，则UpdateFriendList不处理）
            if (null != GUIData.delFriendDataUpdate) GUIData.delFriendDataUpdate();

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
