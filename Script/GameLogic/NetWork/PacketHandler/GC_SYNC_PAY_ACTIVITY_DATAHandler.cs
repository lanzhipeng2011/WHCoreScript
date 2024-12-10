//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_SYNC_PAY_ACTIVITY_DATAHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_SYNC_PAY_ACTIVITY_DATA packet = (GC_SYNC_PAY_ACTIVITY_DATA)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;

            if (GameManager.gameManager.PlayerDataPool != null)
            {
                GameManager.gameManager.PlayerDataPool.HandlePacket(packet);
            }

            if (ChargeActivityLogic.Instance())
            {
                ChargeActivityLogic.Instance().UpdateCurTab();
            }

            //enter your logic
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
