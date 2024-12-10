//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;

namespace SPacket.SocketInstance
{
    public class GC_RET_BLACKMARKETITEMINFOHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_RET_BLACKMARKETITEMINFO packet = (GC_RET_BLACKMARKETITEMINFO) ipacket;
            if (null == packet) return (uint) PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            if (BlackMarketLogic.Instance() !=null)
            {
                BlackMarketLogic.Instance().UpdateGoodInfo(packet);
            }
            return (uint) PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
