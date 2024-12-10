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

            //��ʱ���룬�ȷ��أ����Ե�ʱ���
            //return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;

            //�ж�ServerID
            if (packet.Serverid == GlobeVar.INVALID_ID)
            {
                return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
            }

            //���Ҹ�NPC
            Obj_Character obj = Singleton<ObjManager>.GetInstance().FindObjCharacterInScene(packet.Serverid);
            if (null == obj)
            {
                return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
            }
            
            //����ң���Ϣ����Ч
            if (obj.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER)
            {
                return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
            }
			if (obj.IsDie())//??
			{
				return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
			}

            //����һ�飬��Ϣ����Ч
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

            //����Obj�Ƿ��AutoMove���
            AutoMove autoMove = obj.gameObject.GetComponent<AutoMove>();
            if (null != autoMove)
            {
                autoMove.InsertAutoMovePoint(packet);
            }

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
