//This code create by CodeEngine

using System;
using Games.GlobeDefine;
using Games.LogicObj;
using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
    public class GC_CHANGE_PKLISTHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_CHANGE_PKLIST packet = (GC_CHANGE_PKLIST )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic

            GameManager.gameManager.PlayerDataPool.IsCanPKLegal = (packet.IsPKListEmpty == 1 ? true : false);
            //修改主角名字颜色
            Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
            if (_mainPlayer)
            {
                _mainPlayer.SetNameBoardColor();
            }
           
            int nObjId = packet.ObjId;//加入或者移出主角反击列表的玩家
            Obj _obj = Singleton<ObjManager>.GetInstance().FindObjInScene(nObjId);
            if (_obj ==null)
            {
                return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
            }
            if (_obj.ObjType ==GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER &&
                _obj.ObjType != GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER)
            {
                Obj_OtherPlayer _player = _obj.GetComponent<Obj_OtherPlayer>();
                if (_player)
                {
                    _player.IsInMainPlayerPKList = (packet.IsInPkList == 1 ? true : false);
                    _player.SetNameBoardColor();
                }
            }
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }
    }
 }
