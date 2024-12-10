//This code create by CodeEngine

using System;
using Games.LogicObj;
using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
    public class GC_SKILL_STUDYHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_SKILL_STUDY packet = (GC_SKILL_STUDY )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
         
            Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
            if (_mainPlayer)
            {
                _mainPlayer.StudySkillOpt(packet.SkillId, packet.Skillindex);
            }

            if (GUIData.delMasterDataUpdate != null)
            {
                GUIData.delMasterDataUpdate();
            }
            
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
 }
