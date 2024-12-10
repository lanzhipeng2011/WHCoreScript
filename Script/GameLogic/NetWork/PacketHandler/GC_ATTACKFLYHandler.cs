//This code create by CodeEngine

using System;
using Games.GlobeDefine;
using Games.LogicObj;
using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
    public class GC_ATTACKFLYHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_ATTACKFLY packet = (GC_ATTACKFLY )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            Obj_Character _objChar = Singleton<ObjManager>.GetInstance().FindObjCharacterInScene(packet.ObjId);
            if (_objChar)
            {
                _objChar.CurObjAnimState = GameDefine_Globe.OBJ_ANIMSTATE.STATE_ATTACKFLY;
                _objChar.AttackFly(packet.Dis,packet.Hight,packet.FlyTime/1000.0f);
            }
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
 }
