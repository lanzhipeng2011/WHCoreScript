//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using Games.GlobeDefine;
using Games.LogicObj;
using UnityEngine;
using GCGame.Table;

namespace SPacket.SocketInstance
{
    public class GC_CREATE_PLAYERHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_CREATE_PLAYER packet = (GC_CREATE_PLAYER)ipacket;
            if (null == packet)
                return (uint)PACKET_EXE.PACKET_EXE_ERROR;

            //enter your logic
            //判断是MainPlayer还是其他Obj
            PlayerData playerDataPool = GameManager.gameManager.PlayerDataPool;
            if (playerDataPool.EnterSceneCache.EnterSceneServerID == packet.ServerId &&
                playerDataPool.EnterSceneCache.EnterSceneSceneID == packet.SceneClass)
            {
                playerDataPool.MainPlayerBaseAttr.RoleBaseID = packet.DataId;
                playerDataPool.MainPlayerBaseAttr.RoleName = packet.Name;
                playerDataPool.MainPlayerBaseAttr.Force = packet.Curforce;
                playerDataPool.MainPlayerBaseAttr.Die = packet.IsDie == 1 ? true : false;
                //未存入BaseAttr而保存在Obj类中的时候，需要存入EnterSceneCache,然后在创建完Obj_MainPlayer的时候赋值，之后清空EnterSceneCache
                playerDataPool.EnterSceneCache.EnterSceneRoleBaseID = packet.DataId;
                playerDataPool.EnterSceneCache.Guid = packet.Guid;
                playerDataPool.EnterSceneCache.Profession = packet.CurProfession;
                playerDataPool.PkModle = packet.PKModle;
                playerDataPool.m_objMountParam.MountID = packet.MountID;
                playerDataPool.MainPlayerBaseAttr.MoveSpeed = (int)(packet.MoveSpeed / 100);
					playerDataPool.StealthLev = packet.StealthLev;
                if (packet.HasReliveTime)
                {
                    playerDataPool.ReliveEntryTime = packet.ReliveTime;
                }
                //临时代码，将OtherPlayer的ModelID赋值，从RoleBase表中读取
                Tab_RoleBaseAttr roleBaseAttr = TableManager.GetRoleBaseAttrByID(packet.DataId, 0);
                if (null != roleBaseAttr)
                {
                    playerDataPool.EnterSceneCache.EnterSceneCharModelID = roleBaseAttr.CharModelID;
                }
                playerDataPool.EnterSceneCache.ModelVisualID = packet.ModelVisualID;
                playerDataPool.EnterSceneCache.WeaponDataID = packet.WeaponDataID;
               
				//====TODO===临时逻辑 后期宝石走正常逻辑
				playerDataPool.EnterSceneCache.WeaponEffectGem = packet.WeaponEffectGem;
//				int WeaponEffectGem = 0;
//				switch(playerDataPool.EnterSceneCache.Profession)
//				{
//				case 0:
//					WeaponEffectGem = 8301;
//					break;
//				case 1:
//					WeaponEffectGem = 8201;
//					break;
//				case 2:
//					WeaponEffectGem = 8901;
//					break;
//				case 3:
//					WeaponEffectGem = 8701;
//					break;
//				}
		//		playerDataPool.EnterSceneCache.WeaponEffectGem = playerDataPool.EnterSceneCache.WeaponEffectGem ;
				//====================

				for (int i = 0; i < packet.stateTypeCount; i++)
				{
					if (packet.GetStateType(i) == 16)
					{
						playerDataPool.EnterSceneCache.GuildBusinessState = packet.GetStateValue(i);
					}
				}
                if (0 != packet.GuildGuid) 
                {
                    playerDataPool.GuildInfo.GuildGuid = packet.GuildGuid;
                }
                if (packet.HasVipCost)
                {
                    playerDataPool.VipCost = packet.VipCost;
                }
                if (packet.HasCombatValue)
                {
                    playerDataPool.PoolCombatValue = packet.CombatValue;
                }
                if (packet.HasBindparent)
                {
                    playerDataPool.MainBindParent = packet.Bindparent;
                }
                playerDataPool.MainBindChildren.Clear();
                for (int nindex = 0; nindex < GlobeVar.BIND_CHILDREN_MAX; ++nindex )
                {
                    if (nindex < packet.bindchildrenCount)
                    {
                        playerDataPool.MainBindChildren.Add(packet.GetBindchildren(nindex));
                    }
                    else
                    {
                        playerDataPool.MainBindChildren.Add(-1);
                    }
                }

                if (GameManager.gameManager.RunningScene == GameManager.gameManager.PlayerDataPool.EnterSceneCache.EnterSceneSceneID &&
                    GameManager.gameManager.PlayerDataPool.EnterSceneCache.EnterSceneServerID != -1 &&
                    GameManager.gameManager.PlayerDataPool.EnterSceneCache.EnterSceneRoleBaseID != -1)
                {
                    //创建MainPlayer
                    if ((int)GameDefine_Globe.SCENE_DEFINE.SCENE_LOGIN != GameManager.gameManager.RunningScene)
                    {
                      if ((int)GameDefine_Globe.SCENE_DEFINE.SCENE_YANMENGUANWAI == GameManager.gameManager.RunningScene) 
                        {
                            Tab_ItemVisual itemVisual = TableManager.GetItemVisualByID(29,0);
                            playerDataPool.EnterSceneCache.ModelVisualID = 29;
                            playerDataPool.EnterSceneCache.WeaponDataID = 29;
                        }
                        Singleton<ObjManager>.GetInstance().CreateMainPlayer();
                    }
                }

				int length = 0;
				if (playerDataPool.Profession == (int)CharacterDefine.PROFESSION.XIAOYAO) 
				{
					length=3;
					playerDataPool.AttackCount=3;//蒲扇普攻3段
				}
				if (playerDataPool.Profession == (int)CharacterDefine.PROFESSION.DALI||playerDataPool.Profession ==(int)CharacterDefine.PROFESSION.SHAOLIN) 
				{
					length=4;
					playerDataPool.AttackCount=4;//剑客,枪客普攻4段
				}
				if (playerDataPool.Profession==(int)CharacterDefine.PROFESSION.TIANSHAN) 
				{
					length=5;
					playerDataPool.AttackCount=5;//刺客普攻5段
				}
				for (int i=1;i<length;i++)
				{
					
					playerDataPool.OwnAutoSkillInfo[i].SkillId=playerDataPool.OwnAutoSkillInfo[0].SkillId+i;
					
				}

				//========
				if (PlayerFrameLogic.Instance() != null) 
				{
					PlayerFrameLogic.Instance().InitUI();
				}

            }
            else
            {
                //如果不是主角则是其他玩家
                //其他玩家创建的时候首先判断场景ID
                if (GameManager.gameManager.RunningScene != packet.SceneClass)
                {
                    return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
                }

                Obj_Init_Data initData = new Obj_Init_Data();
                initData.m_ServerID = packet.ServerId;
                initData.m_Guid = packet.Guid;
                initData.m_fX = ((float)packet.PosX) / 100;
                initData.m_fZ = ((float)packet.PosZ) / 100;
                initData.m_RoleBaseID = packet.DataId;
                //LogModule.DebugLog("Player GUID===================" + initData.m_Guid);
                //临时代码，将OtherPlayer的ModelID赋值，从RoleBase表中读取
                Tab_RoleBaseAttr roleBaseAttr = TableManager.GetRoleBaseAttrByID(initData.m_RoleBaseID, 0);
                if (null != roleBaseAttr)
                {
                    initData.m_CharModelID = roleBaseAttr.CharModelID;
                }
                initData.m_Force = packet.Curforce;
                initData.m_StrName = packet.Name;
                initData.m_nProfession = packet.CurProfession;
                initData.m_strTitleName = packet.Titlename;
                initData.m_CurTitleID = packet.CurTitleID;
                initData.m_isInMainPlayerPKList = (packet.IsInPkList == 1 ? true : false);
                initData.m_IsDie = packet.IsDie == 1 ? true : false;
                initData.m_MountID = packet.MountID;
                initData.m_PkModel = packet.PKModle;
                initData.m_MoveSpeed = ((float)packet.MoveSpeed) / 100;
                initData.m_fDir = (float)packet.Facedir / 100;
                initData.m_ModelVisualID = packet.ModelVisualID;
                initData.m_WeaponDataID = packet.WeaponDataID;
               
				//====TODO===临时逻辑 后期宝石走正常逻辑
				initData.m_WeaponEffectGem = packet.WeaponEffectGem;
//				int WeaponEffectGem = 0;
//				switch( initData.m_nProfession)
//				{
//				case 0:
//					WeaponEffectGem = 8301;
//					break;
//				case 1:
//					WeaponEffectGem = 8201;
//					break;
//				case 2:
//					WeaponEffectGem = 8901;
//					break;
//				case 3:
//					WeaponEffectGem = 8701;
//					break;
//				}
			//	initData.m_WeaponEffectGem = WeaponEffectGem;
				//====================

                initData.m_StealthLev = packet.StealthLev;
                initData.m_GuildGuid = packet.GuildGuid;
                initData.m_bIsWildEnemyForMainPlayer = (packet.IsEnemy2Self == 1);
				if ( packet.HasVipCost )
				{
					initData.m_nOtherVipCost = packet.VipCost;
				}
                if (packet.HasCombatValue)
                {
                    initData.m_nOtherCombatValue = packet.CombatValue;
                }
                if (packet.HasBindparent)
                {
                    initData.m_BindParent = packet.Bindparent;
                }
				for (int i = 0; i < packet.stateTypeCount; i++)
				{
					if (packet.GetStateType(i) == 16)
					{
						initData.m_nGuildBusinessState = packet.GetStateValue(i);
					}
				}
                initData.m_BindChildren.Clear();
                for (int nindex = 0; nindex < GlobeVar.BIND_CHILDREN_MAX; ++nindex)
                {
                    if (nindex < packet.bindchildrenCount)
                    {
                        initData.m_BindChildren.Add(packet.GetBindchildren(nindex));
                    }
                    else
                    {
                        initData.m_BindChildren.Add(-1);
                    }
                }
                Singleton<ObjManager>.GetInstance().NewCharacterObj(GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER, initData);
            }

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}