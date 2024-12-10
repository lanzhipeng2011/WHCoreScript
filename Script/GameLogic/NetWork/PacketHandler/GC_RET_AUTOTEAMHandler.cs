//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using GCGame.Table;
using Games.GlobeDefine;
namespace SPacket.SocketInstance
{
    public class GC_RET_AUTOTEAMHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_RET_AUTOTEAM packet = (GC_RET_AUTOTEAM)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            GameManager.gameManager.PlayerDataPool.AutoTeamState = packet.Result == (int)GC_RET_AUTOTEAM.RESULTTYPE.TYPE_WAIT;
            if (packet.HasSceneid)
            {
                GameManager.gameManager.PlayerDataPool.AutoTeamCopySceneId = packet.Sceneid;
            }
            if (packet.HasDifficult)
            {
                GameManager.gameManager.PlayerDataPool.AutoTeamCopySceneDifficult = packet.Difficult;
            }
            if (DungeonWindow.Instance() != null)
            {
                DungeonWindow.Instance().OnButtonAutoTeamLabel();
            }
            if (FunctionButtonLogic.Instance() != null)
           {
               FunctionButtonLogic.Instance().UpdateAutoTeamCue();
           }
            if (ActivityController.Instance() != null)
            {
                ActivityController.Instance().UpdateAutoTeam();
            }
            if (packet.Result == (int)GC_RET_AUTOTEAM.RESULTTYPE.TYPE_FINISH)
            {
                if (packet.HasSceneid)
                {
                    m_nSceneID = packet.Sceneid;
                }
                if (packet.HasDifficult)
                {
                    m_nDifficult = packet.Difficult;
                }
                Tab_SceneClass pSceneClass = TableManager.GetSceneClassByID(m_nSceneID, 0);
                if (pSceneClass != null)
                {
                    string strNaDu = StrDictionary.GetClientDictionaryString("#{" + CharacterDefine.COPYSCENE_DIFFICULTY[m_nDifficult - 1].ToString() + "}");
                    string dicStr = StrDictionary.GetClientDictionaryString("#{2958}", strNaDu, pSceneClass.Name );
                    MessageBoxLogic.OpenOKCancelBox(dicStr, "", OpenCopySceneOK, OpenCopySceneNO);
                }
            }
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
        //消息包暂存数据部分
        private int m_nSceneID = -1;      //副本ID
        private int m_nDifficult = -1;
        public void OpenCopySceneOK()
        {
            if (Singleton<ObjManager>.GetInstance().MainPlayer == null)
            {
                return;
            }
            // 组队副本 又有队伍 还不是队长
            if ( GameManager.gameManager.PlayerDataPool.IsHaveTeam() && Singleton<ObjManager>.GetInstance().MainPlayer.IsTeamLeader())
            {
                CG_OPEN_COPYSCENE packet = (CG_OPEN_COPYSCENE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_OPEN_COPYSCENE);
                packet.SceneID = m_nSceneID;
                packet.Type = 2;
                packet.Difficult = m_nDifficult;
                packet.SendPacket();
            }
           
        }
        public void OpenCopySceneNO()
        {

        }
    }
}