//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_QIANKUNDAI_PRODUCTHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_QIANKUNDAI_PRODUCT packet = (GC_QIANKUNDAI_PRODUCT)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            if (QianKunDaiLogic.Instance() != null && QianKunDaiLogic.Instance().gameObject.activeSelf)
            {
                QianKunDaiLogic.Instance().HandleQianKunDaiProduct(packet.ProductDataID);
            }
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
