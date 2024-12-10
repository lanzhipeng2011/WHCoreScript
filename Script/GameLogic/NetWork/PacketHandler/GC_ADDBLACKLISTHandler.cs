//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_ADDBLACKLISTHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_ADDBLACKLIST packet = (GC_ADDBLACKLIST)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic

            if (null != GameManager.gameManager.PlayerDataPool.BlackList)
            {
                Relation _relation = new Relation();
                _relation.Guid = packet.Guid;
                _relation.Name = packet.Name;
                _relation.Level = packet.Level;
                _relation.Profession = packet.Prof;
                _relation.CombatNum = packet.Combat;
                _relation.State = packet.State;

                if (GameManager.gameManager.PlayerDataPool.BlackList.AddRelation(_relation))
                {
                    if (Singleton<ObjManager>.Instance.MainPlayer != null)
                    {
                        Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false,"#{2357}");
                    }
                }
            }

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
