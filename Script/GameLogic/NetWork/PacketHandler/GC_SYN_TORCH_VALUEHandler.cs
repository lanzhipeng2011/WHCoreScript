//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_SYN_TORCH_VALUEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_SYN_TORCH_VALUE packet = (GC_SYN_TORCH_VALUE)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;

            if (packet.Torchvalue >= 0)
            {
                //更新客户端薪火值
                GameManager.gameManager.PlayerDataPool.TorchValue = packet.Torchvalue;
                //刷新界面
                if (SkillRootLogic.Instance() != null)
                {
                    SkillRootLogic.Instance().m_CurTorchLabel.text = GameManager.gameManager.PlayerDataPool.TorchValue.ToString();
                }
                if (MasterWindow.Instance() != null)
                {
                    MasterWindow.Instance().m_TorchValue.text = GameManager.gameManager.PlayerDataPool.TorchValue.ToString();
                }
            }

            //enter your logic
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
