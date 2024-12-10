//This code create by CodeEngine

using System;
using Games.GlobeDefine;
using Games.LogicObj;
using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
    public class CG_CHANGE_PKMODLEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {  
            CG_CHANGE_PKMODLE packet = (CG_CHANGE_PKMODLE )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
          
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }  
 }
