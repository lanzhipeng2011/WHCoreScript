
//This code create by CodeEngine

using System;
using System.Diagnostics;
using Games.GlobeDefine;
using GCGame.Table;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using Games.LogicObj;
using Games.GlobeDefine;
namespace SPacket.SocketInstance
 {
    public class GC_SYN_SKILLINFOHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_SYN_SKILLINFO packet = (GC_SYN_SKILLINFO )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            PlayerData playerDataPool = GameManager.gameManager.PlayerDataPool;
            //先清除
            for (int skillIndex = 0; skillIndex < playerDataPool.OwnSkillInfo.Length; skillIndex++)
            {
                playerDataPool.OwnSkillInfo[skillIndex].CleanUp();
            }
            for (int i=0;i<packet.skillidCount;i++)
            {
                int nIndex = packet.GetSkillindex(i);
                if (nIndex >=0 && nIndex<playerDataPool.OwnSkillInfo.Length)
                {
                    playerDataPool.OwnSkillInfo[nIndex].SkillId = packet.GetSkillid(i);
					playerDataPool.OwnAutoSkillInfo[nIndex].SkillId = packet.GetSkillid(i);
                    //冷却时间
					//==When players log in or skills upgrading skills does not reset the cooldown cd
                    //playerDataPool.OwnSkillInfo[nIndex].CDTime = packet.GetCDTime(i);
                }
            }
			Obj_MainPlayer main = Singleton<ObjManager>.GetInstance().MainPlayer;
		

			if ( main!=null)
            {
				main.UpdateSkillInfo();
            }
            if (PVPPowerWindow.Instance())
            {
                PVPPowerWindow.Instance().UpdateSkillList();
            }
            if (SkillRootLogic.Instance())
            {
               if (packet.HasIsSkillLevelUp && packet.IsSkillLevelUp ==1)
                {
                    if (null != GameManager.gameManager)
                    {
                        GameManager.gameManager.SoundManager.PlaySoundEffect(141);  //skill_levelup
                    }

					//===Skills upgrading should plan requires shielding effects
                    //SkillRootLogic.Instance().PlaySkillLevelUpEffect();
                }
               
                SkillRootLogic.Instance().UpdateSkillInfo();

				//=====pop tips skill level up
				//SkillRootLogic.Instance().CurClickBtItem;
				Tab_SkillLevelUp _levelUp = TableManager.GetSkillLevelUpByID(SkillRootLogic.Instance().CurClickBtItem.SkillID, 0);
				Tab_SkillEx _skillEx = TableManager.GetSkillExByID(SkillRootLogic.Instance().CurClickBtItem.SkillID, 0);
				Tab_SkillBase _skillBase = TableManager.GetSkillBaseByID(_skillEx.BaseId, 0);
				if(_levelUp.NextSkillId == -1)
				{
					Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1809}",_skillBase.Name);
				}else{
					Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1808}",_skillBase.Name,_skillEx.Level);
				}
            }
            if (GUIData.delMasterDataUpdate != null)
            {
                GUIData.delMasterDataUpdate();
            }
            
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
 }
