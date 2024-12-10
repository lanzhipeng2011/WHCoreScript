//This code create by CodeEngine

using System;
using Games.LogicObj;
using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
    public class GC_REMOVEEFFECTHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_REMOVEEFFECT packet = (GC_REMOVEEFFECT )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
             //enter your logic
            Obj_Character _objChar = Singleton<ObjManager>.GetInstance().FindObjCharacterInScene(packet.ObjId);
            if (_objChar)
            {
                if (_objChar.ObjEffectLogic !=null)
                {
                    _objChar.ObjEffectLogic.StopEffect(packet.EffectId);
                }
            }
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
 }
