//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using Games.GlobeDefine;
using Games.LogicObj;

namespace SPacket.SocketInstance
{
    public class GC_STOPHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_STOP packet = (GC_STOP)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;

            //临时代码，先返回，调试的时候打开
            //return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;

            //判断ServerID
            if (packet.Serverid == GlobeVar.INVALID_ID)
            {
                return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
            }

            //查找该NPC
            Obj_Character obj = Singleton<ObjManager>.GetInstance().FindObjCharacterInScene(packet.Serverid);
            if (null == obj)
            {
                return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
            }

            //查找Obj是否绑定AutoMove组件
            AutoMove autoMove = obj.gameObject.GetComponent<AutoMove>();
            if (null != autoMove)
            {
                autoMove.InterruptMove(packet);
            }

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
