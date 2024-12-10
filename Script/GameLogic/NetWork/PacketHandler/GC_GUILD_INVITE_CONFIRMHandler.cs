//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
 using Games.GlobeDefine;
 using Games.LogicObj;
using  GCGame.Table;
namespace SPacket.SocketInstance
 {
     public class GC_GUILD_INVITE_CONFIRMHandler : Ipacket
     {
         public uint Execute(PacketDistributed ipacket)
         {
        
             //�����û�������
             m_InviterGuid = GlobeVar.INVALID_GUID;
             m_InviterGuildGuid = GlobeVar.INVALID_GUID;

             //������Ϣ��
             GC_GUILD_INVITE_CONFIRM packet = (GC_GUILD_INVITE_CONFIRM)ipacket;
             if (null == packet) 
                 return (uint)PACKET_EXE.PACKET_EXE_ERROR;

             if (packet.InviterGuid == GlobeVar.INVALID_GUID)
             {
                 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
             }

             m_InviterGuid = packet.InviterGuid;
             m_InviterName = packet.InviterName;
             m_InviterGuildGuid = packet.InviterGuildGuid;

             Obj_MainPlayer mainPlayer = Singleton<ObjManager>.Instance.MainPlayer;
             if (null == mainPlayer)
             {
                return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
             }

            //����MessageBoxȷ������
            //"���XXX�������������XXXX���Ƿ�ͬ�⣿"
             string dicStr = StrDictionary.GetClientDictionaryString("#{2767}", packet.InviterName, packet.InviterGuidName);
             MessageBoxLogic.OpenOKCancelBox(dicStr, "", AgreeGuildInvite, DisagreeGuildInvite);
            
             return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;

         }

         //��Ϣ���ݴ����ݲ���
         private UInt64 m_InviterGuid;      //������GUID���յ���Ϣ����ʱ���ݴ棬���ȷ�Ϻ�ȡ��֮����ò�����
         private UInt64 m_InviterGuildGuid; 
         private string m_InviterName;
         public void AgreeGuildInvite()
         {
             CG_GUILD_INVITE_CONFIRM pak = (CG_GUILD_INVITE_CONFIRM)PacketDistributed.CreatePacket(MessageID.PACKET_CG_GUILD_INVITE_CONFIRM);
             pak.InviterGuid = m_InviterGuid;
             pak.InviterGuildGuid = m_InviterGuildGuid;
             pak.Agree = 1;
             pak.InviterName = m_InviterName;
             pak.SendPacket();

             m_InviterGuid = GlobeVar.INVALID_GUID;
             m_InviterGuildGuid = GlobeVar.INVALID_GUID;
         }

         public void DisagreeGuildInvite()
         {
             CG_GUILD_INVITE_CONFIRM pak = (CG_GUILD_INVITE_CONFIRM)PacketDistributed.CreatePacket(MessageID.PACKET_CG_GUILD_INVITE_CONFIRM);
             pak.InviterGuid = m_InviterGuid;
             pak.InviterGuildGuid = m_InviterGuildGuid;
             pak.Agree = 0;
             pak.InviterName = m_InviterName;
             pak.SendPacket();

             m_InviterGuid = GlobeVar.INVALID_GUID;
             m_InviterGuildGuid = GlobeVar.INVALID_GUID;
         }
     }
 }
