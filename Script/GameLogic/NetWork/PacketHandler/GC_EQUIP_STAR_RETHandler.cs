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

            //ǿ������
            if (EquipStrengthenLogic.Instance() != null)
            {
                EquipStrengthenLogic.Instance().UpdateTab();
            }

            if (success == 1)
            {
                //���ǳɹ�
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1276}");
            }
            else if (success == 2)
            {
                //�Ǽ�����
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1274}");
            }
            else if (success == 3)
            {
                //����ʯ��������
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1275}");
            }
            else if (success == 4)
            {
                //��Ҳ���
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1019}");
            }
            else
            {
                //����ʧ��
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1277}");
            }

            //enter your logic
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
