//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_FINDPLAYERHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_FINDPLAYER packet = (GC_FINDPLAYER)ipacket;
            if (null == packet) 
                return (uint)PACKET_EXE.PACKET_EXE_ERROR;

            //enter your logic
            if (null != GUIData.delPlayerFindResult) GUIData.delPlayerFindResult(packet);
            //RelationLogic.Instance().UpdatePlayerFindResult(packet);

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
