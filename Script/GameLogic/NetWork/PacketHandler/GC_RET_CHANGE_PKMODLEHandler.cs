//This code create by CodeEngine

using System;
using Games.GlobeDefine;
using Games.LogicObj;
using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
    public class GC_RET_CHANGE_PKMODLEHandler : Ipacket
    {  
        public uint Execute(PacketDistributed ipacket)
        {
            GC_RET_CHANGE_PKMODLE packet = (GC_RET_CHANGE_PKMODLE )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            Obj _obj = Singleton<ObjManager>.GetInstance().FindObjInScene(packet.ObjId);
            if (_obj == null)
            {
                return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
            }
            if (_obj.ObjType != GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER &&
                _obj.ObjType != GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER)
            {
                return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
            }
            Obj_OtherPlayer _player = _obj.GetComponent<Obj_OtherPlayer>();
            _player.PkModle = packet.PKModle;
            _player.OptChangPKModle();

            if (PKInfoSetLogic.Instance() != null)
            {
                PKInfoSetLogic.Instance().SwitchBtnState();
            }

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
 }
