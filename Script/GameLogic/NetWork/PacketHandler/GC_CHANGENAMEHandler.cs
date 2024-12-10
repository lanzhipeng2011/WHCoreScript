//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;

namespace SPacket.SocketInstance
 {
    public class GC_CHANGENAMEHandler : Ipacket
     {
         public uint Execute(PacketDistributed ipacket)
         {
             GC_CHANGENAME packet = (GC_CHANGENAME)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            ChangeNameLogic.ShowChangeName((ChangeNameLogic.ChangeNameType)packet.Nametype);                      
            //enter your logic
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }
     }
 }
