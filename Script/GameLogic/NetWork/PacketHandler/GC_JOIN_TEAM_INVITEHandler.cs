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
             //先重置缓存数据
             m_InviterGuid = GlobeVar.INVALID_GUID;

             //处理消息包
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
                     //挂机中
                     AgreeTeamInvite();
                 }
                 else
                 {
                     //发送MessageBox确认邀请
                     //"玩家XXX邀请您加入队伍，是否同意？"
                     string dicStr = StrDictionary.GetClientDictionaryString("#{1173}", packet.InviterName);
                     MessageBoxLogic.OpenOKCancelBox(dicStr, "", AgreeTeamInvite, DisagreeTeamInvite);
                 }  
             }
             return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }
         //消息包暂存数据部分
         private UInt64 m_InviterGuid;      //邀请者GUID，收到消息包的时候暂存，玩家确认和取消之后调用并重置

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
