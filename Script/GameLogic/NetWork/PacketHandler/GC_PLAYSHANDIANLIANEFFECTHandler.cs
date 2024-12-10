//This code create by CodeEngine

using System;
using Games.LogicObj;
using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
using UnityEngine;

namespace SPacket.SocketInstance
 {
    public class GC_PLAYSHANDIANLIANEFFECTHandler : Ipacket
    {
      
        public uint Execute(PacketDistributed ipacket)
        {
            GC_PLAYSHANDIANLIANEFFECT packet = (GC_PLAYSHANDIANLIANEFFECT )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            Obj_Character _objChar = Singleton<ObjManager>.GetInstance().FindObjCharacterInScene(packet.ObjId);

            Obj_Character _objTarget = Singleton<ObjManager>.GetInstance().FindObjCharacterInScene(packet.TargetId);
            if (_objChar && _objTarget!=null && _objChar.ObjEffectLogic !=null)
            {
                _objChar.ObjEffectLogic.PlayEffect(43, OnPlayEffect, packet.TargetId);
            }
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }

        void OnPlayEffect(GameObject effectObject, object targetID)
        {
            if (effectObject != null && effectObject.GetComponent<LightingChain>() != null)
            {
                effectObject.GetComponent<LightingChain>().InitData((int)targetID);
            }
        }
    }
 }
