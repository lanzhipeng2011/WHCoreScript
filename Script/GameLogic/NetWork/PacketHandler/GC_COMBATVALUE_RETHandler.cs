//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using UnityEngine;
namespace SPacket.SocketInstance
{
    public class GC_COMBATVALUE_RETHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_COMBATVALUE_RET packet = (GC_COMBATVALUE_RET )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic

            if (Singleton<ObjManager>.GetInstance().MainPlayer != null)
            {
                if (packet.ShowPowerRemind == 1)
                {
                    int nNewCombatValue = packet.CombatValue;
                    int nAddCombatValue = nNewCombatValue - Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.CombatValue;
                    if (nAddCombatValue > 0)
                    {
                        PowerRemindLogic.InitPowerInfo(nNewCombatValue, nAddCombatValue);  
                    }
                }

                Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.CombatValue = packet.CombatValue;

                if (BackPackLogic.Instance())
                {
                    //设置了客户端战斗力之后更新背包界面
                    BackPackLogic.Instance().UpdatePlayerInfo();
                }

                if (PVPPowerWindow.Instance())
                {
                    PVPPowerWindow.Instance().UpdateCombatValue();
                }
                if (RoleViewLogic.Instance() !=null)
                {
                    RoleViewLogic.Instance().OnCombatChange();
                }
            }
            else
            {
                GameManager.gameManager.PlayerDataPool.MainPlayerBaseAttr.CombatValue = packet.CombatValue;
            }
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
