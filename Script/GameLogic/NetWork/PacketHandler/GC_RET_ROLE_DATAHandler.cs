//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
using GCGame.Table;
namespace SPacket.SocketInstance
 {
    public class GC_RET_ROLE_DATAHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_RET_ROLE_DATA packet = (GC_RET_ROLE_DATA )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            GameManager.gameManager.PlayerDataPool.PoolCombatValue = packet.Combatvalue;
            //enter your logic
            if (RoleViewLogic.Instance() ==null)
            {
                return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
            }
            RoleViewLogic.Instance().Name = packet.Rolename;
            RoleViewLogic.Instance().CombatValue = packet.Combatvalue;
            RoleViewLogic.Instance().Level = packet.Leve;
            RoleViewLogic.Instance().Profession = packet.Profession;
            RoleViewLogic.Instance().RoleGUID = packet.Guid;
            RoleViewLogic.Instance().CurHp = packet.Curhp;
            RoleViewLogic.Instance().MaxHP = packet.Maxhp;
            RoleViewLogic.Instance().CurMp = packet.Curmp;
            RoleViewLogic.Instance().MaxMp = packet.Maxmp;
            RoleViewLogic.Instance().CurExp = packet.Curexp;
            RoleViewLogic.Instance().MaxExp = packet.Maxexp;
            RoleViewLogic.Instance().PAttck = packet.Pattack;
            RoleViewLogic.Instance().MAttack = packet.Mattack;
            RoleViewLogic.Instance().Hit = packet.Hit;
            RoleViewLogic.Instance().Critical = packet.Critical;
            RoleViewLogic.Instance().PDefense = packet.Pdefense;
            RoleViewLogic.Instance().MDefense = packet.Mdefense;
            RoleViewLogic.Instance().DeCritical = packet.Decritical;
            RoleViewLogic.Instance().Doge = packet.Doge;
            RoleViewLogic.Instance().Strike = packet.Strike;
            RoleViewLogic.Instance().CriticalAdd = packet.Criticaladd;
            RoleViewLogic.Instance().Ductical = packet.Dutical;
            RoleViewLogic.Instance().CriticalMis = packet.Criticalmis;
            RoleViewLogic.Instance().OffLineExp = packet.Curofflineexp;
            RoleViewLogic.Instance().m_ExpSprite.fillAmount = (float)RoleViewLogic.Instance().CurExp / (float)RoleViewLogic.Instance().MaxExp;
            if (PlayerFrameLogic.Instance() != null) 
            {
                PlayerFrameLogic.Instance().OnCombatValueChange(packet.Combatvalue);
            }
			//========
			if (BackPackLogic.Instance())
			{
				//ÉèÖÃÁË¿Í»§¶ËÕ½¶·Á¦Ö®ºó¸üÐÂ±³°ü½çÃæ
				BackPackLogic.Instance().UpdatePlayerInfo();
			}
            //
            Tab_OffLineExp curTabOffLine = TableManager.GetOffLineExpByID(packet.Leve, 0);
            if (null != curTabOffLine)
            {
                RoleViewLogic.Instance().OffLineMaxExp = curTabOffLine.OffLineExpLimit;
            }
            RoleViewLogic.Instance().UpdateAttrRightView();
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
      
        }
    }
 }
