//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using Games.Item;

namespace SPacket.SocketInstance
 {
     public class GC_RET_OTHERROLE_DATAHandler : Ipacket
     {
         public uint Execute(PacketDistributed ipacket)
         {
            GC_RET_OTHERROLE_DATA packet = (GC_RET_OTHERROLE_DATA )ipacket;
            if (null == packet) 
                return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            GameManager.gameManager.OtherPlayerData.CleanUpData();
            GameManager.gameManager.OtherPlayerData.Name = packet.Rolename;
            GameManager.gameManager.OtherPlayerData.CombatValue = packet.Combatvalue;
            GameManager.gameManager.OtherPlayerData.Level = packet.Leve;
            GameManager.gameManager.OtherPlayerData.Profession = packet.Profession;
            GameManager.gameManager.OtherPlayerData.RoleGUID = packet.Roleguid;
            GameManager.gameManager.OtherPlayerData.CurHp = packet.Curhp;
            GameManager.gameManager.OtherPlayerData.MaxHP = packet.Maxhp;
            GameManager.gameManager.OtherPlayerData.CurMp = packet.Curmp;
            GameManager.gameManager.OtherPlayerData.MaxMp = packet.Maxmp;
            GameManager.gameManager.OtherPlayerData.CurExp = packet.Curexp;
            GameManager.gameManager.OtherPlayerData.MaxExp = packet.Maxexp;
            GameManager.gameManager.OtherPlayerData.PAttck = packet.Pattack;
            GameManager.gameManager.OtherPlayerData.MAttack = packet.Mattack;
            GameManager.gameManager.OtherPlayerData.Hit = packet.Hit;
            GameManager.gameManager.OtherPlayerData.Critical = packet.Critical;
            GameManager.gameManager.OtherPlayerData.PDefense = packet.Pdefense;
            GameManager.gameManager.OtherPlayerData.MDefense = packet.Mdefense;
            GameManager.gameManager.OtherPlayerData.DeCritical = packet.Decritical;
            GameManager.gameManager.OtherPlayerData.Doge = packet.Doge;
            GameManager.gameManager.OtherPlayerData.Strike = packet.Strike;
            GameManager.gameManager.OtherPlayerData.CriticalAdd = packet.Criticaladd;
            GameManager.gameManager.OtherPlayerData.Ductical = packet.Dutical;
            GameManager.gameManager.OtherPlayerData.CriticalMis = packet.Criticalmis;
            GameManager.gameManager.OtherPlayerData.ModuleVisualID = packet.ModelVisualID;
            GameManager.gameManager.OtherPlayerData.CurWeaponDataID = packet.WeaponDataID;
            
			//====TODO===???? ?????????
			GameManager.gameManager.OtherPlayerData.WeaponEffectGem = packet.WeaponEffectGem;
//			int WeaponEffectGem = 0;
//			switch(GameManager.gameManager.OtherPlayerData.Profession)
//			{
//			case 0:
//				WeaponEffectGem = 8301;
//				break;
//			case 1:
//				WeaponEffectGem = 8201;
//				break;
//			case 2:
//				WeaponEffectGem = 8901;
//				break;
//			case 3:
//				WeaponEffectGem = 8701;
//				break;
//			}
		//	GameManager.gameManager.OtherPlayerData.WeaponEffectGem = WeaponEffectGem;
			//====================

            GameItemContainer EquipContainer = GameManager.gameManager.OtherPlayerData.EquipPack;
            if (EquipContainer != null)
            {
                for (int i = 0; i < packet.itemguidCount; i++)
                {
                    int packindex = packet.GetPackindex(i);
                    //取得物品
                    GameItem item = EquipContainer.GetItem(packindex);
                    if (null == item)
                    {
                        continue;
                    }
                    item.DataID = packet.GetDataid(i);
                    item.Guid = packet.GetItemguid(i);
                    if (packet.GetBindflag(i) == 1)
                    {
                        item.BindFlag = true;
                    }
                    else
                    {
                        item.BindFlag = false;
                    }
                    item.StackCount = packet.GetStackcount(i);
                    item.CreateTime = packet.GetCreatetime(i);
                    item.EnchanceLevel = packet.GetEnchancelevel(i);
                    item.EnchanceExp = packet.GetEnchanceexp(i);
                    item.StarLevel = packet.GetStarlevel(i);
                    item.DynamicData[0] = packet.GetDynamicdata1(i);
                    item.DynamicData[1] = packet.GetDynamicdata2(i);
                    item.DynamicData[2] = packet.GetDynamicdata3(i);
                    item.DynamicData[3] = packet.GetDynamicdata4(i);
                    item.DynamicData[4] = packet.GetDynamicdata5(i);
                    item.DynamicData[5] = packet.GetDynamicdata6(i);
                    item.DynamicData[6] = packet.GetDynamicdata7(i);
                    item.DynamicData[7] = packet.GetDynamicdata8(i);
                    item.EnchanceTotalExp = packet.GetEnchancetotalexp(i);
                    item.StarTimes = packet.GetStartimes(i);
                }
            }
            int nGemCount = packet.gemidCount;
            for (int i = 0; i < (int)EquipPackSlot.Slot_NUM; i++)
            {
                for (int index = 0; index < (int)GemSlot.OPEN_NUM; index++)
                {
                    int totalIndex = i * (int)GemSlot.OPEN_NUM + index;
                    if (totalIndex >= 0 && totalIndex < nGemCount)
                    {
                        int nGemId = packet.GetGemid(totalIndex);
                        GameManager.gameManager.OtherPlayerData.GemData.SetGemId(i, index, nGemId);
                    }
                }
            }
			PlayerFrameLogic.Instance ().SwitchAllWhenPopUIShow (false);//???????
            UIManager.ShowUI(UIInfo.OtherRoleView, OtherRoleViewLogic.OnShowOtherRoleVirew);

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }
     }
 }