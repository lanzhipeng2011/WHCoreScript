//This code create by CodeEngine

using System;
using Games.GlobeDefine;
using GCGame.Table;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using Games.LogicObj;
namespace SPacket.SocketInstance
 {
     public class GC_JOIN_TEAM_INVITEHandler : Ipacket
     {
         public uint Execute(PacketDistributed ipacket)
         {
             //�����û�������
             m_InviterGuid = GlobeVar.INVALID_GUID;

             //������Ϣ��
             GC_JOIN_TEAM_INVITE packet = (GC_JOIN_TEAM_INVITE)ipacket;
             if (null == packet) 
                 return (uint)PACKET_EXE.PACKET_EXE_ERROR;

             if (packet.InviterGuid == GlobeVar.INVALID_GUID)
             {
                 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
             }

             m_InviterGuid = packet.InviterGuid;
             Obj_MainPlayer mainPlayer = Singleton<ObjManager>.Instance.MainPlayer;
             if (null != mainPlayer)
             {

                 if (mainPlayer.GetAutoCombatState() && mainPlayer.AutoTaem)
                 {
                     //�һ���
                     AgreeTeamInvite();
                 }
                 else
                 {
                     //����MessageBoxȷ������
                     //"���XXX������������飬�Ƿ�ͬ�⣿"
                     string dicStr = StrDictionary.GetClientDictionaryString("#{1173}", packet.InviterName);
                     MessageBoxLogic.OpenOKCancelBox(dicStr, "", AgreeTeamInvite, DisagreeTeamInvite);
                 }  
             }
             return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }
         //��Ϣ���ݴ����ݲ���
         private UInt64 m_InviterGuid;      //������GUID���յ���Ϣ����ʱ���ݴ棬���ȷ�Ϻ�ȡ��֮����ò�����

         public void AgreeTeamInvite()
         {
             CG_JOIN_TEAM_INVITE_RESULT pak = (CG_JOIN_TEAM_INVITE_RESULT)PacketDistributed.CreatePacket(MessageID.PACKET_CG_JOIN_TEAM_INVITE_RESULT);
             pak.InviterGuid = m_InviterGuid;
             pak.Result = 1;
             pak.SendPacket();

             m_InviterGuid = GlobeVar.INVALID_GUID;
         }

         public void DisagreeTeamInvite()
         {
             CG_JOIN_TEAM_INVITE_RESULT pak = (CG_JOIN_TEAM_INVITE_RESULT)PacketDistributed.CreatePacket(MessageID.PACKET_CG_JOIN_TEAM_INVITE_RESULT);
             pak.InviterGuid = m_InviterGuid;
             pak.Result = 0;
             pak.SendPacket();

             m_InviterGuid = GlobeVar.INVALID_GUID;
         }
     }
 }
