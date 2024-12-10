//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_RESTAURANT_DESTUPDATEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_RESTAURANT_DESTUPDATE packet = (GC_RESTAURANT_DESTUPDATE)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            RestaurantData.UpdatePlayerDeskData(packet);
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
