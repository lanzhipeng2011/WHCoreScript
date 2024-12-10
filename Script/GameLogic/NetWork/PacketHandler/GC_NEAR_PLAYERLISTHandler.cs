//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_NEAR_PLAYERLISTHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_NEAR_PLAYERLIST packet = (GC_NEAR_PLAYERLIST)ipacket;
            if (null == packet) 
                return (uint)PACKET_EXE.PACKET_EXE_ERROR;

            //enter your logic
            if (null != GUIData.delNearbyPlayerUpdate) GUIData.delNearbyPlayerUpdate(packet);
            //RelationLogic.Instance().UpdateNearbyPlayer(packet);

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
