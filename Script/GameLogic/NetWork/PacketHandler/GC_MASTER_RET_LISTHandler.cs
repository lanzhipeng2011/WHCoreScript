//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_MASTER_RET_LISTHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_MASTER_RET_LIST packet = (GC_MASTER_RET_LIST)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;

            if (GameManager.gameManager.PlayerDataPool.MasterPreList != null)
            {
                GameManager.gameManager.PlayerDataPool.MasterPreList.UpdateData(packet);
            }

            if (GUIData.delMasterDataUpdate != null)
            {
                GUIData.delMasterDataUpdate();
            }

            //enter your logic
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
