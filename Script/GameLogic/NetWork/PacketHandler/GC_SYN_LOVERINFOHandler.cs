//This code create by CodeEngine

using System;
using System.Collections.Generic;
using Games.GlobeDefine;
using Games.LogicObj;
using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;

namespace SPacket.SocketInstance
{
    public class GC_SYN_LOVERINFOHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_SYN_LOVERINFO packet = (GC_SYN_LOVERINFO) ipacket;
            if (null == packet) return (uint) PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic

            GameManager.gameManager.PlayerDataPool.LoverGUID = packet.LoverGuid;
            //结婚对象 在可见范围内 则修改名字颜色
            if (packet.LoverGuid !=GlobeVar.INVALID_GUID)
            {
                Obj_OtherPlayer _objChar = Singleton<ObjManager>.GetInstance().FindOtherPlayerInScene(packet.LoverGuid);
                if (null != _objChar && _objChar.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER)
                {
                    _objChar.SetNameBoardColor();
                }
            }
            else //离婚了 就重置附近玩家的名字
            {
                Dictionary<string, Obj> targets = Singleton<ObjManager>.GetInstance().ObjPools;
                foreach (Obj targetObj in targets.Values)
                {
                    if (targetObj != null && targetObj.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER)
                    {
                        Obj_OtherPlayer _Player = targetObj as Obj_OtherPlayer;
                        if (_Player)
                        {
                            _Player.SetNameBoardColor();
                        }
                    }
                }
            }
           
            return (uint) PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
