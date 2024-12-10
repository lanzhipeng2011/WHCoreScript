//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_MASTERSHOP_BUYHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_MASTERSHOP_BUY packet = (GC_MASTERSHOP_BUY)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic

            //进行提示
            if (null != Singleton<ObjManager>.GetInstance().MainPlayer)
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2404}");
            }

            //更新界面
            if (GUIData.delMasterDataUpdate != null)
            {
                GUIData.delMasterDataUpdate();
            }

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
