//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_PUT_GEM_RETHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_PUT_GEM_RET packet = (GC_PUT_GEM_RET)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;

            
            
			//modify by wmy
			int EquipPos = packet.Equipslot;
			int Index = packet.Index;
			int GemDateId = packet.Dataid;
            GameManager.gameManager.PlayerDataPool.GemData.SetGemId(EquipPos, Index, GemDateId);
            if (EquipStrengthenLogic.Instance() != null)
            {
                EquipStrengthenLogic.Instance().UpdateGemInfo();
            }

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
