//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;

namespace SPacket.SocketInstance
{
    public class GC_CLOSE_BLACKMARKETHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_CLOSE_BLACKMARKET packet = (GC_CLOSE_BLACKMARKET) ipacket;
            if (null == packet) return (uint) PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            if (BlackMarketLogic.Instance() != null)
            {
                BlackMarketLogic.Instance().CloseWindow();
            }
            return (uint) PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
