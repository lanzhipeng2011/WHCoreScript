//This code create by CodeEngine

using System;
using Games.LogicObj;
using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;

namespace SPacket.SocketInstance
{
    public class GC_RET_ISWILDENEMY2USERHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_RET_ISWILDENEMY2USER packet = (GC_RET_ISWILDENEMY2USER) ipacket;
            if (null == packet) return (uint) PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            Obj_OtherPlayer _otherPlayer = Singleton<ObjManager>.GetInstance().FindOtherPlayerInScene(packet.UserGuid);
            if (_otherPlayer !=null)
            {
                _otherPlayer.IsWildEnemyForMainPlayer = (packet.IsEnemy == 1 ? true : false);
                _otherPlayer.SetNameBoardColor();
            }
            return (uint) PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
