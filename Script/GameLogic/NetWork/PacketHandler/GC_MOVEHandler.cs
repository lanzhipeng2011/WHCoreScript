//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using Games.GlobeDefine;
using Games.LogicObj;
namespace SPacket.SocketInstance
{
    public class GC_MOVEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_MOVE packet = (GC_MOVE)ipacket;
            if (null == packet) 
                return (uint)PACKET_EXE.PACKET_EXE_ERROR;

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
            
            //主玩家，消息包无效
            if (obj.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER)
            {
                return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
            }
			if (obj.IsDie())//??
			{
				return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
			}

            //主玩家伙伴，消息包无效
            if (obj.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_FELLOW)
            {
                Obj_Fellow objFellow = obj as Obj_Fellow;
                if (objFellow.OwnerObjId == Singleton<ObjManager>.GetInstance().MainPlayer.ServerID)
                {
                    return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
                }
            }

            if (obj.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER)
            {
                Obj_OtherPlayer objOther = obj as Obj_OtherPlayer;
                if (null != objOther && objOther.QingGongState == true)
                {
                    return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
                }
            }

            //查找Obj是否绑定AutoMove组件
            AutoMove autoMove = obj.gameObject.GetComponent<AutoMove>();
            if (null != autoMove)
            {
                autoMove.InsertAutoMovePoint(packet);
            }

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
