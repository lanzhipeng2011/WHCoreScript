//This code create by CodeEngine

using System;
using Games.LogicObj;
using Games.SkillModle;
using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
using GCGame.Table;
namespace SPacket.SocketInstance
 {
    public class GC_SKILL_FINISHHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_SKILL_FINISH packet = (GC_SKILL_FINISH )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            
            int _objId = packet.ObjId;

            Obj_Character Sender = Singleton<ObjManager>.GetInstance().FindObjCharacterInScene(_objId);
            if (Sender ==null)
            {
                return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
            }
            SkillCore _skillCore = Sender.SkillCore;
            if (_skillCore ==null)
            {
                return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
            }
			Tab_SkillBase skillbase = _skillCore.UsingSkillBaseInfo;
			if(skillbase != null)
			if (skillbase.IsMove == 1) 
			{
				return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
			}
            if (packet.FinsjType ==(int)SKILLFINISHREASON.BREAK)
            {
                _skillCore.BreakCurSkill();
            }
            else if (packet.FinsjType ==(int)SKILLFINISHREASON.FINISH)
            {
                _skillCore.SkillFinsh();
            }
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
 }
