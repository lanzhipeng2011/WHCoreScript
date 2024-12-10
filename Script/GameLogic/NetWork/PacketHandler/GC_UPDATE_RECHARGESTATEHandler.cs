//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_UPDATE_RECHARGESTATEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_UPDATE_RECHARGESTATE packet = (GC_UPDATE_RECHARGESTATE)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            RechargeData.RechareStateUpdate(packet.IsEnable > 0);
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
