//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;

namespace SPacket.SocketInstance
 {
    public class GC_OPEN_SHAREWINDOWHandler : Ipacket
     {
         public uint Execute(PacketDistributed ipacket)
         {
             GC_OPEN_SHAREWINDOW packet = (GC_OPEN_SHAREWINDOW)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
             //enter your logic
            ShareRootWindow.ShowShareWindow((ShareType)packet.Sharetype, OpenType.OpenType_Share);
             return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }
     }
 }
