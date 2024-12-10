//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_RESTAURANT_LEVELUPDATEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_RESTAURANT_LEVELUPDATE packet = (GC_RESTAURANT_LEVELUPDATE)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            RestaurantData.UpdateRestaurantLevel(packet);
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
