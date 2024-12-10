//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_ADDHATELISTHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_ADDHATELIST packet = (GC_ADDHATELIST)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic

            if (null != GameManager.gameManager.PlayerDataPool.HateList)
            {
                Relation _relation = new Relation();
                _relation.Guid = packet.Guid;
                _relation.Name = packet.Name;
                _relation.Level = packet.Level;
                _relation.Profession = packet.Prof;
                _relation.CombatNum = packet.Combat;
                _relation.State = packet.State;

                if (GameManager.gameManager.PlayerDataPool.HateList.AddRelation(_relation))
                {
                    if (Singleton<ObjManager>.Instance.MainPlayer != null)
                    {
                        Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{3064}");
                    }
                }
            }

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
