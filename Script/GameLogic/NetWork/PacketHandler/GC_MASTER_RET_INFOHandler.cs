//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_MASTER_RET_INFOHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_MASTER_RET_INFO packet = (GC_MASTER_RET_INFO)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;

            bool bReShowTab = false;
            bool bReserveMember = false;
            if (GameManager.gameManager.PlayerDataPool.MasterInfo.IsValid() == false)
            {
                bReShowTab = true;
            }
            else
            {
                if (GameManager.gameManager.PlayerDataPool.IsMasterReserveMember())
                {
                    bReserveMember = true;
                }
            }

            if (GameManager.gameManager.PlayerDataPool.MasterInfo != null)
            {
                GameManager.gameManager.PlayerDataPool.MasterInfo.UpdateData(packet);
            }

            //之前是待审批成员 这次更新之后不是了 刷新界面
            if (bReserveMember && GameManager.gameManager.PlayerDataPool.IsMasterReserveMember() == false)
            {
                bReShowTab = true;
            }

            if (bReShowTab)
            {
                if (MasterWindow.Instance())
                {
                    MasterWindow.Instance().ShowTab();
                }
            }

            if (GUIData.delMasterDataUpdate != null)
            {
                GUIData.delMasterDataUpdate();
            }

            if (MenuBarLogic.Instance())
            {
                MenuBarLogic.Instance().UpdateGuildAndMasterReserveMember();
            }
            if (SkillRootLogic.Instance())
            {
                SkillRootLogic.Instance().UpdateSkillInfo();
            }
            //enter your logic
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
