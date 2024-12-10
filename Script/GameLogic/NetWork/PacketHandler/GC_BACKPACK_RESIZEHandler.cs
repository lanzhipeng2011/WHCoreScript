//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
 using Games.Item;
using Games.SwordsMan;
namespace SPacket.SocketInstance
{
    public class GC_BACKPACK_RESIZEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_BACKPACK_RESIZE packet = (GC_BACKPACK_RESIZE)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;

            int packtype = packet.Packtype;
            int newSize = packet.Size;
            int nType = packet.Type;
            if (packtype == (int)GameItemContainer.Type.TYPE_BACKPACK)
            {
                int curSize = GameManager.gameManager.PlayerDataPool.BackPack.ContainerSize;
                if (newSize > curSize)
                {
                    int addSize = newSize - curSize;
                    GameManager.gameManager.PlayerDataPool.BackPack.AddContainerSize(addSize);
                    //如果背包界面开着 刷新一下
                    if (BackPackLogic.Instance())
                    {
                        BackPackLogic.Instance().UpdateBackPack();
                    }
                    if (nType == 1)
                    {
                        //type为1时才需要屏幕提示
                        Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1368}");
                    }
                }
            }
            else if (packtype == (int)GameItemContainer.Type.TYPE_STORAGEPACK)
            {
                int curSize = GameManager.gameManager.PlayerDataPool.StoragePack.ContainerSize;
                if (newSize > curSize)
                {
                    int addSize = newSize - curSize;
                    GameManager.gameManager.PlayerDataPool.StoragePack.AddContainerSize(addSize);
                    if (CangKuLogic.Instance())
                    {
                        CangKuLogic.Instance().UpdateCangKu();
                    }
                }
            }
            //enter your logic
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
