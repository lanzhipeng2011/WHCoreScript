//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_RESTAURANT_UPDATEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_RESTAURANT_UPDATE packet = (GC_RESTAURANT_UPDATE)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            RestaurantData.UpdatePlayerData(packet);
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
