//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_UPDATE_ACTIVE_FELLOWSKILLHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_UPDATE_ACTIVE_FELLOWSKILL packet = (GC_UPDATE_ACTIVE_FELLOWSKILL)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;

            GameManager.gameManager.PlayerDataPool.ActiveFellowSkill.Clear();
            int count = packet.skillIdCount;
            for (int index = 0; index < count; index++ )
            {
                int skillId = packet.GetSkillId(index);
                if (skillId > 0)
                {
                    GameManager.gameManager.PlayerDataPool.ActiveFellowSkill.Add(skillId);
                }
            }

            if (PartnerFrameLogic.Instance() != null)
            {
                PartnerFrameLogic.Instance().UpdateSkillRemain();
            }

            if (PartnerFrameLogic_Skill.Instance() != null)
            {
                PartnerFrameLogic_Skill.Instance().InitPartnerSkillList();
            }

            //enter your logic
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
