//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using Games.GlobeDefine;
using GCGame.Table;
namespace SPacket.SocketInstance
{
    public class GC_NOTICE_ADDED_FRIENDHandler : Ipacket
    {
        private UInt64 m_guid;
        public uint Execute(PacketDistributed ipacket)
        {
            GC_NOTICE_ADDED_FRIEND packet = (GC_NOTICE_ADDED_FRIEND)ipacket;
            if (null == packet) 
                return (uint)PACKET_EXE.PACKET_EXE_ERROR;

            m_guid = packet.Guid;
            if (m_guid == GlobeVar.INVALID_GUID)
            {
                return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
            }

            //发送MessageBox确认邀请
            //XXX将您加为好友，是否回关注？
            string text = packet.Name + StrDictionary.GetClientDictionaryString("#{1072}");
            MessageBoxLogic.OpenOKCancelBox(text, "", AgreeAddFriend, DisAgreeAddFriend);

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        private void AgreeAddFriend()
        {
            if (m_guid == GlobeVar.INVALID_GUID)
            {
                return;
            }

            if (null != Singleton<ObjManager>.GetInstance().MainPlayer)
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.ReqAddFriend(m_guid);
            }
        }

        private void DisAgreeAddFriend()
        {
        }
    }
}
