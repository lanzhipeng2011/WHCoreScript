//This code create by CodeEngine

using System;
using Games.LogicObj;
using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
    public class GC_BROADCAST_ATTRHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_BROADCAST_ATTR packet = (GC_BROADCAST_ATTR )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            int objId = packet.ObjId;
            Obj_Character TargetObj = Singleton<ObjManager>.GetInstance().FindObjCharacterInScene(objId);
            if (TargetObj ==null)
            {
                return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;  
            }

            TargetObj.UpdateAttrBroadcastPackt(packet);
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
 }
