//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
 using Games.Item;
using Games.SwordsMan;
namespace SPacket.SocketInstance
{
    public class GC_SWORDSMANPACK_RESIZEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_SWORDSMANPACK_RESIZE packet = (GC_SWORDSMANPACK_RESIZE)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;

            int packtype = packet.Packtype;
            int newSize = packet.Size;
            int nType = packet.Type;
            if ((int)SwordsManContainer.PACK_TYPE.TYPE_EQUIPPACK == packtype)
            {
                int curSize = GameManager.gameManager.PlayerDataPool.SwordsManEquipPack.ContainerSize;
                if (newSize > curSize)
                {
                    int addSize = newSize - curSize;
                    GameManager.gameManager.PlayerDataPool.SwordsManEquipPack.AddContainerSize(addSize);
                    if (SwordsManController.Instance() != null)
                    {
                        SwordsManController.Instance().UpdateSwordsManEquipPack();
                    }
                }
            }
            //enter your logic
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
