//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
using GCGame.Table;
namespace SPacket.SocketInstance
{
    public class GC_EQUIP_STAR_RETHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_EQUIP_STAR_RET packet = (GC_EQUIP_STAR_RET)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;

            int success = packet.Success;
//            int packType = packet.Packtype;
//            UInt64 equipGuid = packet.Equipguid;

            //强化界面
            if (EquipStrengthenLogic.Instance() != null)
            {
                EquipStrengthenLogic.Instance().UpdateTab();
            }

            if (success == 1)
            {
                //打星成功
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1276}");
            }
            else if (success == 2)
            {
                //星级已满
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1274}");
            }
            else if (success == 3)
            {
                //打星石数量不足
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1275}");
            }
            else if (success == 4)
            {
                //金币不足
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1019}");
            }
            else
            {
                //打星失败
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1277}");
            }

            //enter your logic
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
