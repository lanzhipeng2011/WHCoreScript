//This code create by CodeEngine

using System;
 using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_SYNC_MASTER_SKILL_NAMEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_SYNC_MASTER_SKILL_NAME packet = (GC_SYNC_MASTER_SKILL_NAME)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;

            GameManager.gameManager.PlayerDataPool.MasterSkillName.Clear();
            if (packet.SkillId1 >= 0)
            {
                GameManager.gameManager.PlayerDataPool.MasterSkillName.Add(packet.SkillId1, packet.SkillName1);
            }
            if (packet.SkillId2 >= 0)
            {
                GameManager.gameManager.PlayerDataPool.MasterSkillName.Add(packet.SkillId2, packet.SkillName2);
            }
            if (packet.SkillId3 >= 0)
            {
                GameManager.gameManager.PlayerDataPool.MasterSkillName.Add(packet.SkillId3, packet.SkillName3);
            }

            //enter your logic
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
