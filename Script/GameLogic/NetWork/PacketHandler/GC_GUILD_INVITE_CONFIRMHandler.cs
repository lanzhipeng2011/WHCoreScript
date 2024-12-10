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
        
             //先重置缓存数据
             m_InviterGuid = GlobeVar.INVALID_GUID;
             m_InviterGuildGuid = GlobeVar.INVALID_GUID;

             //处理消息包
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

            //发送MessageBox确认邀请
            //"玩家XXX邀请您加入帮派XXXX，是否同意？"
             string dicStr = StrDictionary.GetClientDictionaryString("#{2767}", packet.InviterName, packet.InviterGuidName);
             MessageBoxLogic.OpenOKCancelBox(dicStr, "", AgreeGuildInvite, DisagreeGuildInvite);
            
             return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;

         }

         //消息包暂存数据部分
         private UInt64 m_InviterGuid;      //邀请者GUID，收到消息包的时候暂存，玩家确认和取消之后调用并重置
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
