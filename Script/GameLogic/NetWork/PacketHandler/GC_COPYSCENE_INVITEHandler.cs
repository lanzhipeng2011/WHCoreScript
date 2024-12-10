//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using Games.LogicObj;
using GCGame.Table;
using Games.GlobeDefine;
namespace SPacket.SocketInstance
 {
     public class GC_COPYSCENE_INVITEHandler : Ipacket
     {
         public uint Execute(PacketDistributed ipacket)
         {
             GC_COPYSCENE_INVITE packet = (GC_COPYSCENE_INVITE )ipacket;
             if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
             //enter your logic
             Obj_MainPlayer MainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
             if (null == MainPlayer) return (uint)PACKET_EXE.PACKET_EXE_ERROR;

             m_nSceneId = packet.SceneID;
             m_nDifficult = packet.Difficult;
             string szName = packet.InviterName;
              
             Tab_SceneClass tabSceneClass = TableManager.GetSceneClassByID(m_nSceneId, 0);
             if (null != tabSceneClass)
             {
                 if (m_nDifficult == 1)
                 {
                     MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{1974}", szName,  tabSceneClass.Name), "", AgreeCopySceneInvite, DisagreeCopySceneInvite, 20);

                 }
                 else if (m_nDifficult == 2)
                 {
                     MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{2015}", szName,  tabSceneClass.Name), "", AgreeCopySceneInvite, DisagreeCopySceneInvite, 20);

                 }
                 else if (m_nDifficult == 3)
                 {
                     MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{2016}", szName, tabSceneClass.Name), "", AgreeCopySceneInvite, DisagreeCopySceneInvite, 20);
                 }
             } 
             return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }
         int m_nSceneId =  GlobeVar.INVALID_ID;
         int m_nDifficult =  GlobeVar.INVALID_ID;
         public void AgreeCopySceneInvite()
         {
             CG_COPYSCENE_INVITE_RET pak = (CG_COPYSCENE_INVITE_RET)PacketDistributed.CreatePacket(MessageID.PACKET_CG_COPYSCENE_INVITE_RET);
             pak.SceneID = m_nSceneId;
             pak.InviteResult = (int)CG_COPYSCENE_INVITE_RET.InviteResultType.RESULTTYPE_YES;
             pak.Difficult = m_nDifficult;
             pak.SendPacket();

             m_nSceneId = GlobeVar.INVALID_ID;
             m_nDifficult = GlobeVar.INVALID_ID;
         }

         public void DisagreeCopySceneInvite()
         {
             CG_COPYSCENE_INVITE_RET pak = (CG_COPYSCENE_INVITE_RET)PacketDistributed.CreatePacket(MessageID.PACKET_CG_COPYSCENE_INVITE_RET);
             pak.SceneID = m_nSceneId;
             pak.InviteResult = (int)CG_COPYSCENE_INVITE_RET.InviteResultType.RESULTTYPE_NO;
             pak.Difficult = m_nDifficult;
             pak.SendPacket();

             m_nSceneId = GlobeVar.INVALID_ID;
             m_nDifficult = GlobeVar.INVALID_ID;
         }
     }
 }
