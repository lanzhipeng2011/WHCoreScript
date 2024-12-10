//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_MASTER_LEAVEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_MASTER_LEAVE packet = (GC_MASTER_LEAVE)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;

            int leavlType = packet.LevelType;
            if (leavlType == 1)
            {
                //�����뿪
                if (MasterWindow.Instance())
                {
                    UIManager.CloseUI(UIInfo.MasterAndGuildRoot);
                }
            }
            else if (leavlType == 2)
            {
                //���߳�ʦ��
                if (MasterWindow.Instance())
                {
                    UIManager.CloseUI(UIInfo.MasterAndGuildRoot);
                }
            }
            else if (leavlType == 3)
            {
                //ʦ�Ž�ɢ
                if (MasterWindow.Instance())
                {
                    UIManager.CloseUI(UIInfo.MasterAndGuildRoot);
                }
            }
            else if (leavlType == 4)
            {
                //���뱻�ܾ�
            }

            //��տͻ��˻�������
            GameManager.gameManager.PlayerDataPool.MasterInfo.CleanUp();

            //enter your logic
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
