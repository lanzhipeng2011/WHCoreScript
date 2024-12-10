//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_UI_NEWPLAYERGUIDEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_UI_NEWPLAYERGUIDE packet = (GC_UI_NEWPLAYERGUIDE )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            string UIName = packet.UIName;
            int nIndex = packet.Index;
            NewPlayerGuide.NewPlayerGuideOpt(UIName, nIndex);
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
