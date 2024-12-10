//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using GCGame.Table;
using UnityEngine;
using Games.LogicObj;
namespace SPacket.SocketInstance
{
    public class GC_PLAY_EFFECTHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_PLAY_EFFECT packet = (GC_PLAY_EFFECT)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;

            Obj_Character _objChar = Singleton<ObjManager>.GetInstance().FindObjCharacterInScene(packet.ObjID);
            
            if (null != _objChar)
            {
                _objChar.PlayEffect(packet.EffectID);
            }
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
