//This code create by CodeEngine

using System;
using Games.LogicObj;
using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
    public class GC_UPDATE_HITPONITHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_UPDATE_HITPONIT packet = (GC_UPDATE_HITPONIT )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            //交给客户端 模拟处理
           // Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
           // if (_mainPlayer)
          //  {
           //     bool bIsCritical = (packet.IsCritical == 1 ? true : false);
          //      _mainPlayer.ChangeHit(packet.Hitponit, bIsCritical);
           // }
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }
    }
 }
