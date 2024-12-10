/********************************************************************
	日期:	2014/03/01
	文件: 	Obj_MainPlayer_Auto.cs
	路径:	D:\work\code\mldj\Version\Main\Project\Client\Assets\MLDJ\Script\Obj\Obj_MainPlayer_Auto.cs
	作者:	YangXin
	描述:	人物挂机
	修改:	
*********************************************************************/
using System;
using UnityEngine;
using Games.GlobeDefine;
using System.Collections.Generic;
using Games.Item;
using GCGame.Table;

namespace Games.LogicObj
{
    public partial class Obj_MainPlayer : Obj_OtherPlayer
    {
        //是否开启挂机模式
        public bool IsOpenAutoCombat
        {
            get { return GameManager.gameManager.PlayerDataPool.IsOpenAutoCombat; }
            set { GameManager.gameManager.PlayerDataPool.IsOpenAutoCombat = value; }
        }
        //最近一次打断 自动战斗的时间
        public float BreakAutoCombatTime
        {
            get { return GameManager.gameManager.PlayerDataPool.BreakAutoCombatTime; }
            set { GameManager.gameManager.PlayerDataPool.BreakAutoCombatTime = value; }
        }
        //是否开始自动战斗
        public bool AutoComabat
        {
            get { return GameManager.gameManager.PlayerDataPool.AutoComabat; }
            set { GameManager.gameManager.PlayerDataPool.AutoComabat = value; }
        }
		public bool AutoXunLu
		{
			get { return GameManager.gameManager.PlayerDataPool.AutoXunlu; }
			set { GameManager.gameManager.PlayerDataPool.AutoXunlu = value; }
		}
		public void BreakAutoCombatState()
        {
            AutoComabat = false;
            BreakAutoCombatTime = Time.time;

        }
        public bool GetAutoCombatState()
        {
            if (Controller)
            {
                return Controller.CombatFlag;
            }
            return false;
        }
		public void EnterJuqing(Obj juqingitem)
		{
			if (Controller) 
			{
				Controller.EnterJuQing();
				Controller.CurrentAIState.SetTarget(juqingitem);
			}
		}
		public void LeveJuqing()
		{
			if (Controller) 
			{
				Controller.LeaveJuQing();
				Invoke("DelayMsg",1.0f);
	           
			}
		}
		public void DelayMsg()
		{
			Controller.EnterCombat();
		}
        public void EnterAutoCombat()
        {
            if (Controller)
            {
                m_playerHeadInfo.ToggleGuaJi(true);
                Controller.EnterCombat();
                AutoComabat = true;
                IsOpenAutoCombat = true;
				LeaveTeamFollow();
                UpdateSellItem();       //自动卖药
            }
        }

        public void LeveAutoCombat()
        {
            if (Controller)
            {
                m_playerHeadInfo.ToggleGuaJi(false);
                Controller.LeaveCombat();
                AutoComabat = false;
                IsOpenAutoCombat = false;
            }
         
        }
        //拾取物品,按品质算的,以位记录AUTOCOMBAT_PICKUP_TYPE
      
        protected int m_nAutoPickUp = 0;
        public int AutoPickUp
        {
            get { return m_nAutoPickUp; }
            set { m_nAutoPickUp = value; }
        }
        public void SetAutoPickUpFlag(int nIdx, bool bFlag)
        {
            if (nIdx >= 0 && nIdx < 32)
            {
                if (bFlag == true)
                {
                    m_nAutoPickUp |= 0x01 << nIdx;
                }
                else
                {
                    m_nAutoPickUp &= ~(0x01 << nIdx);
                }
            }
        }
        public bool GetAutoPickUpFlag(int nIdx)
        {
            if (nIdx >= 0 && nIdx < 32)
            {
                return ((m_nAutoPickUp&(0x01 << nIdx)) > 0);     
            }
            return false;
        }
        //自动组队
        protected bool m_bAutoTaem = false;
        public bool AutoTaem
        {
            get { return m_bAutoTaem; }
            set { m_bAutoTaem = value; }
        }
		//自动组队
		protected bool m_bAutoAcceptTaem = false;
		public bool AutoAcceptTaem
		{
			get { return m_bAutoAcceptTaem; }
			set { m_bAutoAcceptTaem = value; }
		}
        //自动回血回蓝 白百分比
        protected float m_fAutoHpPercent = 0.712f;
        public float AutoHpPercent
        {
            get { return m_fAutoHpPercent; }
            set { m_fAutoHpPercent = value; }
        }
        protected int m_nAutoHpID = -1;
        public int AutoHpID
        {
            get { return m_nAutoHpID; }
            set { m_nAutoHpID = value; GameViewModel.Get<MainPlayerViewModel>().AutoHpID.Value = value; }
        }

        protected float m_fAutoMpPercent = 0.712f;
        public float AutoMpPercent
        {
            get { return m_fAutoMpPercent; }
            set { m_fAutoMpPercent = value; }
        }
        protected int m_nAutoMpID = -1;
        public int AutoMpID
        {
            get { return m_nAutoMpID; }
            set { m_nAutoMpID = value; GameViewModel.Get<MainPlayerViewModel>().AutoMpID.Value = value; }
        }

        protected bool m_bAutoBuyDrug = false;
        public bool AutoBuyDrug
        {
            get { return m_bAutoBuyDrug; }
            set { m_bAutoBuyDrug = value; }
        }

        protected bool m_bIsSelectDrug = false;
        public bool AutoIsSelectDrug
        {
            get { return m_bIsSelectDrug; }
            set { m_bIsSelectDrug = value; }
        }

        protected UInt64 m_nAutoEquipGuid = GlobeVar.INVALID_GUID;
        public UInt64 AutoEquipGuid
        {
            get { return m_nAutoEquipGuid; }
            set { m_nAutoEquipGuid = value; }
        }
        //搜索范围
//         protected int m_nAutoRadius = 50;
//         public int AutoRadius
//         {
//             get { return m_nAutoRadius; }
//             set { m_nAutoRadius = value; }
//         }
         //公告控制
        protected int m_nAutoNotice = 0;
        public int AutoNotice
        {
            get { return m_nAutoNotice; }
            set { m_nAutoNotice = value; }
        }

        private int m_nAutoMovetoQGPointId =-1; //需要自动走到的轻功点ID
        public int AutoMovetoQGPointId
        {
            get { return m_nAutoMovetoQGPointId; }
            set { m_nAutoMovetoQGPointId = value; }
        }
        private UInt64 m_nHpDrugGUID = GlobeVar.INVALID_GUID;     //血药位置       
        public UInt64 HpDrugGUID
        {
            get { return m_nHpDrugGUID; }
            set { m_nHpDrugGUID = value; }
        }

        private UInt64 m_nMpDrugGUID = GlobeVar.INVALID_GUID;       //魔药位置
        public UInt64 MpDrugGUID
        {
            get { return m_nMpDrugGUID; }
            set { m_nMpDrugGUID = value; }
        }
		public void InitSpecialInfo()
		{
			Dictionary<string, PlayerSpecialData> curPlayer= UserConfigData.GetPlayerSpecialList();
			UInt64 PlayerGuid = PlayerPreferenceData.LastRoleGUID;
			if (curPlayer.ContainsKey(PlayerGuid.ToString()))
			{
				PlayerSpecialData  oPlayer = curPlayer[PlayerGuid.ToString()];
				if (oPlayer != null)
				{
			//		GameManager.gameManager.PlayerDataPool.IsFirstYeXiDaYing=true;
				}
					
			}
		}
		public void InitAutoInfo()
			{
				//先初始化
            SetAutoPickUpFlag((int)GameDefine_Globe.AUTOCOMBAT_PICKUP_TYPE.PICKUP_EQUIP1, false);
            SetAutoPickUpFlag((int)GameDefine_Globe.AUTOCOMBAT_PICKUP_TYPE.PICKUP_EQUIP2, false);
            SetAutoPickUpFlag((int)GameDefine_Globe.AUTOCOMBAT_PICKUP_TYPE.PICKUP_EQUIP3, false);
            SetAutoPickUpFlag((int)GameDefine_Globe.AUTOCOMBAT_PICKUP_TYPE.PICKUP_EQUIP4, false);
            SetAutoPickUpFlag((int)GameDefine_Globe.AUTOCOMBAT_PICKUP_TYPE.PICKUP_EQUIP5, false);
            SetAutoPickUpFlag((int)GameDefine_Globe.AUTOCOMBAT_PICKUP_TYPE.PICKUP_STUFF, false);
            SetAutoPickUpFlag((int)GameDefine_Globe.AUTOCOMBAT_PICKUP_TYPE.PICKUP_OTHER, false);

            AutoHpID = (int)GameDefine_Globe.AUTOCOMBAT_DRUG_ID.AUTO_DRUG_START_HP;
            AutoMpID = (int)GameDefine_Globe.AUTOCOMBAT_DRUG_ID.AUTO_DRUG_START_MP;

            //读取注册表
            Dictionary<string, PlayerAuto> curPlayerAutoList = UserConfigData.GetPlayerAutoList();
            UInt64 PlayerGuid = PlayerPreferenceData.LastRoleGUID;
            if (curPlayerAutoList.ContainsKey(PlayerGuid.ToString()))
            {
                PlayerAuto oPlayerAuto = curPlayerAutoList[PlayerGuid.ToString()];
                if (oPlayerAuto != null)
                {
                    //AutoComabat = oPlayerAuto.AutoFightOpenToggle == 1? true : false; 
                    AutoPickUp = oPlayerAuto.AutoPickUp;
                    AutoTaem = oPlayerAuto.AutoTaem == 1? true : false; 
					AutoAcceptTaem = oPlayerAuto.AutoAcceptTeam == 1? true : false; 
                    AutoHpPercent = (float)oPlayerAuto.AutoHpPercent/100;
                    AutoMpPercent = (float)oPlayerAuto.AutoMpPercent/100;
                    AutoBuyDrug = oPlayerAuto.AutoBuyDrug == 1 ? true : false; 
                    //AutoRadius = oPlayerAuto.AutoRadius; 
                    AutoNotice = oPlayerAuto.AutoNotice;

                    AutoHpID = oPlayerAuto.AutoHpID;
                    AutoMpID = oPlayerAuto.AutoMpID;
                    GameManager.gameManager.PlayerDataPool.HpItemCDTime = 0;
                    GameManager.gameManager.PlayerDataPool.MpItemCDTime = 0;
                    
                    AutoIsSelectDrug = oPlayerAuto.AutoIsSelectDrug == 1 ? true : false;
                    AutoEquipGuid = oPlayerAuto.AutoEquipGuid;
					int startindex=GameManager.gameManager.PlayerDataPool.AttackCount;
					for(int i=0;i<10;i++)
					{
						GameManager.gameManager.PlayerDataPool.OwnAutoSkillInfo[startindex+i].CanAutoCombat=(oPlayerAuto.AutoSkills[i]==1?true:false);
					}
                    UpdateSelectDrug();
                }               
            }
          
            if (PlayerFrameLogic.Instance())
            {
               // PlayerFrameLogic.Instance().InitAutoFight();
            }
            if (null != FunctionButtonLogic.Instance())
            {
                FunctionButtonLogic.Instance().UpdateAutoFightBtnState();
            }
            m_nCopySceneExitTime = -1;
        }
        public void UpdateSelectDrug()
        {
            GameItemContainer BackPack = GameManager.gameManager.PlayerDataPool.BackPack;
            if (AutoIsSelectDrug == false)
            {
                //做自动选药处理
                bool isFind = false;
                for (int i = (int)GameDefine_Globe.AUTOCOMBAT_DRUG_ID.AUTO_DRUG_END_DYHP; i >= (int)GameDefine_Globe.AUTOCOMBAT_DRUG_ID.AUTO_DRUG_START_DYHP; --i)
                {
                    Tab_CommonItem commonItem = TableManager.GetCommonItemByID(i, 0);
                    if (commonItem != null)
                    {
                        if (BaseAttr.Level >= commonItem.MinLevelRequire)
                        {
                            for (int index = 0; index < BackPack.ContainerSize; ++index)
                            {
                                GameItem eItemEx = BackPack.GetItem(index);
                                if (eItemEx != null && eItemEx.DataID == i) //身上有这个药
                                {
                                    AutoHpID = i;
                                    isFind = true;
                                    break;
                                }
                            }
                            if (isFind)
                            {
                                break;
                            }
                        }
                    }
                }
                if (!isFind)    
                {                    
                    for (int i = (int)GameDefine_Globe.AUTOCOMBAT_DRUG_ID.AUTO_DRUG_END_HP; i >= (int)GameDefine_Globe.AUTOCOMBAT_DRUG_ID.AUTO_DRUG_START_HP; --i)
                    {
                        Tab_CommonItem commonItem = TableManager.GetCommonItemByID(i, 0);
                        if (commonItem != null)
                        {
                            if (BaseAttr.Level >= commonItem.MinLevelRequire)
                            {
                                for (int index = 0; index < BackPack.ContainerSize; ++index)
                                {
                                    GameItem eItemEx = BackPack.GetItem(index);
                                    if (eItemEx != null && eItemEx.DataID == i) //身上有这个药
                                    {
                                        AutoHpID = i;
                                        isFind = true;
                                        break;
                                    }
                                }
                                if (isFind)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                
                isFind = false;
                for (int i = (int)GameDefine_Globe.AUTOCOMBAT_DRUG_ID.AUTO_DRUG_END_DYMP; i >= (int)GameDefine_Globe.AUTOCOMBAT_DRUG_ID.AUTO_DRUG_START_DYMP; --i)
                {
                    Tab_CommonItem commonItem = TableManager.GetCommonItemByID(i, 0);
                    if (commonItem != null)
                    {
                        if (BaseAttr.Level >= commonItem.MinLevelRequire)
                        {
                            for (int index = 0; index < BackPack.ContainerSize; ++index)
                            {
                                GameItem eItemEx = BackPack.GetItem(index);
                                if (eItemEx != null && eItemEx.DataID == i) //身上有这个药
                                {
                                    AutoMpID = i;
                                    isFind = true;
                                    break;
                                }
                            }
                            if (isFind)
                            {
                                break;
                            }
                        }
                    }
                }
                if (!isFind)
                {
                    for (int i = (int)GameDefine_Globe.AUTOCOMBAT_DRUG_ID.AUTO_DRUG_END_MP; i >= (int)GameDefine_Globe.AUTOCOMBAT_DRUG_ID.AUTO_DRUG_START_MP; --i)
                    {
                        Tab_CommonItem commonItem = TableManager.GetCommonItemByID(i, 0);
                        if (commonItem != null)
                        {
                            if (BaseAttr.Level >= commonItem.MinLevelRequire)
                            {
                                for (int index = 0; index < BackPack.ContainerSize; ++index)
                                {
                                    GameItem eItemEx = BackPack.GetItem(index);
                                    if (eItemEx != null && eItemEx.DataID == i) //身上有这个药
                                    {
                                        AutoMpID = i;
                                        isFind = true;
                                        break;
                                    }
                                }
                                if (isFind)
                                {
                                    break;
                                }
                            }
                        }
                    }                    
                }
            }
        }
        private int m_nCopySceneExitTime = -1;
        public int ExitTime
        {
            get { return m_nCopySceneExitTime; }
            set { m_nCopySceneExitTime = value; }
        }
        public static float m_fAutoTimeSecond = Time.realtimeSinceStartup;
        public void UpdateAuto()
        {
            float ftimeSec = Time.realtimeSinceStartup;
            int nTimeData = (int)(ftimeSec - m_fAutoTimeSecond);
            if (nTimeData > 0)
            {
                m_fAutoTimeSecond = ftimeSec;
                if (ExitTime > 0)
                {
                    ExitTime = ExitTime - nTimeData;

                    if (ExitTime <= 0)
                    {
                        if (GameManager.gameManager.PlayerDataPool.CopySceneChange) //正在传送中
                        {
                            return;
                        }
                        GameManager.gameManager.PlayerDataPool.CopySceneChange = true;
                        CG_LEAVE_COPYSCENE packet = (CG_LEAVE_COPYSCENE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_LEAVE_COPYSCENE);
                        packet.NoParam = 1;
                        packet.SendPacket();
                    }
                    else
                    {
                        SendNoticMsg(false, "#{2227}", ExitTime);
                    }                    
                }
                if (GameManager.gameManager.PlayerDataPool.CJGSweepCDTime > 0)
                {
                    GameManager.gameManager.PlayerDataPool.CJGSweepCDTime = GameManager.gameManager.PlayerDataPool.CJGSweepCDTime - nTimeData;
                    if (GameManager.gameManager.PlayerDataPool.CJGSweepCDTime < 0)
                    {
                        GameManager.gameManager.PlayerDataPool.CJGSweepCDTime = 0;
                    }        
                }
                if (GameManager.gameManager.PlayerDataPool.HpItemCDTime > 0)
                {
                    GameManager.gameManager.PlayerDataPool.HpItemCDTime = GameManager.gameManager.PlayerDataPool.HpItemCDTime - nTimeData * 1000.0f;
                    if (GameManager.gameManager.PlayerDataPool.HpItemCDTime < 0) GameManager.gameManager.PlayerDataPool.HpItemCDTime = 0;
                }
                if (GameManager.gameManager.PlayerDataPool.MpItemCDTime > 0)
                {
                    GameManager.gameManager.PlayerDataPool.MpItemCDTime = GameManager.gameManager.PlayerDataPool.MpItemCDTime - nTimeData * 1000.0f;
                    if (GameManager.gameManager.PlayerDataPool.MpItemCDTime < 0) GameManager.gameManager.PlayerDataPool.MpItemCDTime = 0;
                }
            }
            //自动吃药
            if (IsDie() == false)
            {
				//by dsy 先注掉
                AutoUseHPMPDrug();
                OptMPChange();
            }

            //藏经阁扫荡CD时间

        }
		public void UpdateAutoXunLuBreakState()
		{
			if(AutoXunLu==true)
			{
				m_playerHeadInfo.ToggleXunLu(true);
			}

		}
		public void UpdateAutoCombatBreakState()
        {
            if (IsOpenAutoCombat && AutoComabat==false)
            {
                if (IsMoving)
                {
                    BreakAutoCombatTime = Time.time;
                }
                //空闲10s后 自动进入挂机状态
                if (Time.time -BreakAutoCombatTime >=8)
                {
                    m_playerHeadInfo.ToggleGuaJi(true);
                    AutoComabat = true;
                }
            }
            else
            {
                BreakAutoCombatTime = 0;
            }
        }
        public void ServerAutoInfo()
        {
            //List<LoginData.PlayerRoleData> curList = new List<LoginData.PlayerRoleData>();
			bool [] autoskills = new bool[10];
			int startindex = GameManager.gameManager.PlayerDataPool.AttackCount;
			for (int i=0; i<10; i++) 
			{
				autoskills[i]=ObjManager.GetInstance().MainPlayer.OwnAutoSkillInfo[startindex+i].CanAutoCombat;
			}
            PlayerAuto AutoData = new PlayerAuto(AutoPickUp, AutoTaem, AutoAcceptTaem,
                (int)(AutoHpPercent * 100), 
                (int)(AutoMpPercent * 100),
                AutoBuyDrug,
                AutoNotice, 
                AutoHpID,
                AutoMpID, 
                AutoIsSelectDrug, 
                AutoEquipGuid,autoskills);

            UserConfigData.AddPlayerAuto(GUID.ToString(), AutoData);
        }
        // 自动吃药
        public bool AutoPercent(int nType)
        {
            bool bSucceed = false;
            if (nType != (int)MedicSubClass.HP && nType != (int)MedicSubClass.MP)
            {
                return false;
            }
            GameItemContainer BackPack = GameManager.gameManager.PlayerDataPool.BackPack;
            GameItem eItem = null;
            int nType2 = nType; 
            if (nType == (int)MedicSubClass.HP)
            {
                eItem = BackPack.GetItemByGuid(m_nHpDrugGUID);
                nType2 = (int)MedicSubClass.HP_DY;
            }
            else if (nType == (int)MedicSubClass.MP)      
            {
                eItem = BackPack.GetItemByGuid(m_nMpDrugGUID);
                nType2 = (int)MedicSubClass.MP_DY;
            }

            if (eItem != null && eItem.IsValid())
            {
                Tab_CommonItem line = TableManager.GetCommonItemByID(eItem.DataID, 0);
                if (null != line)
                {
                    if (line.ClassID == (int)ItemClass.MEDIC
                        && (line.SubClassID == nType || line.SubClassID == nType2)
                        && (AutoHpID == line.Id || AutoMpID == line.Id))
                    {
                        bSucceed = true;
                        AutoUseDrug(eItem);
                    }
                }
            }
            if (bSucceed == false)
            {
                for (int index = 0; index < BackPack.ContainerSize; ++index)
                {
                    GameItem eItemEx = BackPack.GetItem(index);
                    if (eItemEx != null && eItemEx.IsValid())
                    {
                        Tab_CommonItem line = TableManager.GetCommonItemByID(eItemEx.DataID, 0);
                        if (null != line)
                        {
                            if (line.ClassID == (int)ItemClass.MEDIC
                                && (line.SubClassID == nType || line.SubClassID == nType2)
                                 && (AutoHpID == line.Id || AutoMpID == line.Id))
                            {
                                bSucceed = true;
                                if (AutoUseDrug(eItemEx))
                                {
                                    //BackPackItemLogic.Instance().UseItem(eItemEx);
                                    if (nType == (int)MedicSubClass.HP)
                                    {
                                        m_nHpDrugGUID = eItemEx.Guid;
                                    }
                                    else
                                    {
                                        m_nMpDrugGUID = eItemEx.Guid;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return bSucceed;
        }        
        public bool AutoUseDrug(GameItem item)
        {
            if (null == item)
                return false;

            Tab_CommonItem tabItem = TableManager.GetCommonItemByID(item.DataID, 0);
            if (null != tabItem)
            {
                int canuse = tabItem.CanUse;
                if (canuse == 1)
                {
                    CG_USE_ITEM useitem = (CG_USE_ITEM)PacketDistributed.CreatePacket(MessageID.PACKET_CG_USE_ITEM);
                    useitem.SetItemguid(item.Guid);
                    useitem.SendPacket();
                    return true;
                }
            }
            return false;
        }
        // 自动买药
        public bool OnAutoBuyDrug(int nId)
        {
            bool IsBuy = false;
            int ShopId = 3;
            int itemIdex = -1;
            Tab_SystemShop curShop = TableManager.GetSystemShopByID(ShopId, 0);
            if (curShop != null)
            {
                for (int i = 0; i < curShop.Pnum; i++)
                {
                    if (nId == curShop.GetPidbyIndex(i))
                    {
                        IsBuy = true;
                        itemIdex = i;
                        break;
                    }

                }
                if (IsBuy)  //可以买
                {
                    CG_SYSTEMSHOP_BUY buyPacket = (CG_SYSTEMSHOP_BUY)PacketDistributed.CreatePacket(MessageID.PACKET_CG_SYSTEMSHOP_BUY);
                    buyPacket.SetBuyNum(1);
                    buyPacket.SetShopId(ShopId);
					buyPacket.SetMercIndex((uint)itemIdex);
                    buyPacket.SendPacket();

                }
            }
            return true;
        }
        //////////////////////////////////////////////////////////////////////////
        //系统设置
//         protected int m_nSystemNameBoard = 0;   //名字板
//         public int SystemNameBoard
//         {
//             get { return m_nSystemNameBoard; }
//             set { m_nSystemNameBoard = value; }
//         }
//         protected int m_nSystemMusic = 0;   //声音
//         public int SystemMusic
//         {
//             get { return m_nSystemMusic; }
//             set { m_nSystemMusic = value; }
//         }
//         protected int m_nSystemSoundEffect = 0;   //声效
//         public int SystemSoundEffect
//         {
//             get { return m_nSystemSoundEffect; }
//             set { m_nSystemSoundEffect = value; }
//         }
//         protected int m_nTableau = 0;   //图像
//         public int SystemTableau
//         {
//             get { return m_nTableau; }
//             set { m_nTableau = value; }
//         }
//         public void InitSystemInfo()
//         {
//             SystemNameBoard = PlayerPreferenceData.SystemNameBoard;
//             SystemMusic = PlayerPreferenceData.SystemMusic;
//             SystemSoundEffect = PlayerPreferenceData.SystemSoundEffect;
//             SystemTableau = PlayerPreferenceData.SystemTableau;
// 
//             GameManager.gameManager.SoundManager.EnableBGM = !(SystemMusic == 1 ? true : false);
//             GameManager.gameManager.SoundManager.EnableSFX = !(SystemSoundEffect == 1 ? true : false);
// 
//          
        public void AutoFightInYanziwu()
        {
            if (IsOpenAutoCombat == false
                || GameManager.gameManager.RunningScene != (int) GameDefine_Globe.SCENE_DEFINE.SCENE_HUSONGMEIREN)
            {
                return;
            }

            Vector3 targetPos = new Vector3();
            GameObject[] m_QinggongList = GameManager.gameManager.ActiveScene.QingGongPointList;
            if (m_QinggongList.Length < 0 || m_QinggongList.Length > 3)
            {
                return;
            }
            Vector2[] PointVector2 = new Vector2[3];
            PointVector2[0] = new Vector2(56.2f, 27.4f); //轻功点1 起跳点
            PointVector2[2] = new Vector2(41.5f, 50.9f);//轻功点2 起跳点
            PointVector2[1] = new Vector2(3.8f, 41.7f);//轻功点3 起跳点
            if (m_QinggongList[1].activeInHierarchy) //轻功点3开启表示 向区域四中心移动
            {
                targetPos.x = 18.5f;
                targetPos.z = 19.6f;
            }
            else if (m_QinggongList[2].activeInHierarchy) //轻功点2  向区域三中心移动
            {
                targetPos.x = 17.8f;
                targetPos.z = 51.2f;
            }
            else if (m_QinggongList[0].activeInHierarchy) //轻功点1  向区域二中心移动
            {
                targetPos.x = 54.5f;
                targetPos.z = 47.2f;
            }
            else //向区域一中心移动
            {
                targetPos.x = 55.0f;
                targetPos.z = 17.0f;
            }
            //Vector3 _vec3Tar = new Vector3(targetPos.x, Position.y, targetPos.z);
            //float fDis = Vector3.Distance(_vec3Tar, Position);
            //if (fDis > 16) //距离目标中心点过远 则改为向最近开启过的轻功点移动
            //{
            //    float fMinDis = 9999999.0f;
            //    int nSelePointId = -1;
            //    float fPointDis = -1;
            //    for (int i = 0; i < m_QinggongList.Length; i++)
            //    {
            //        if (m_QinggongList[i].activeInHierarchy)
            //        {
            //            if (Position.z > PointVector2[i].y)
            //            {
            //                continue; //不往回走
            //            }
            //            fPointDis = Vector3.Distance(new Vector3(PointVector2[i].x, Position.y, PointVector2[i].y), Position);
            //            if (fPointDis < fMinDis)
            //            {
            //                fMinDis = fPointDis;
            //                targetPos.x = PointVector2[i].x;
            //                targetPos.z = PointVector2[i].y;
            //            }
            //        }
            //    }
            //}
            AutoSearchPoint point = new AutoSearchPoint(GameManager.gameManager.RunningScene, targetPos.x, targetPos.z);

            if (GameManager.gameManager && GameManager.gameManager.AutoSearch)
            {
                BreakAutoCombatState();
                GameManager.gameManager.AutoSearch.BuildPath(point);
            }
        }

        public void AutoFightFlyInYanZiWu()
        {
            if (IsOpenAutoCombat == false)
            {
                return;
            }
            if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_HUSONGMEIREN)
            {
                GameObject[] m_QinggongList = GameManager.gameManager.ActiveScene.QingGongPointList;
                if (m_QinggongList.Length < 0 || m_QinggongList.Length >3)
                {
                    return;
                }
                Vector3 targetPos = new Vector3();
                if (m_QinggongList[1].activeInHierarchy)//轻功点3
                {
                    targetPos.x = 3.8f;
                    targetPos.z = 41.7f;
                }
                else if (m_QinggongList[2].activeInHierarchy)//轻功点2
                {
                    targetPos.x = 41.5f;
                    targetPos.z = 50.9f;
                }
                else if (m_QinggongList[0].activeInHierarchy)//轻功点1
                {
                    targetPos.x = 56.2f;
                    targetPos.z = 27.4f;
                }
                AutoSearchPoint point = new AutoSearchPoint(GameManager.gameManager.RunningScene, targetPos.x, targetPos.z);
                
                if (GameManager.gameManager && GameManager.gameManager.AutoSearch)
                {
                    GameManager.gameManager.AutoSearch.BuildPath(point);
                }
            }
        }
        public void UpdateSellItem()
        {
            if (BaseAttr.Level < GlobeVar.MAX_AUTOEQUIT_LIVE)
            {
                return;
            }
            if (IsOpenAutoCombat == false)
            {
                return;
            }
            GameItemContainer BackPack = GameManager.gameManager.PlayerDataPool.BackPack;
            List<ulong> selllist = new List<ulong>();
            
            
            for (int index = 0; index < BackPack.ContainerSize; ++index)
            {
                GameItem eItem = BackPack.GetItem(index);
                if (eItem!= null) //身上有这个药
                {
                    Tab_CommonItem line = TableManager.GetCommonItemByID(eItem.DataID, 0);
                    if (line != null)
                    {
                        //计算品级及拾取规则                           
                        if (line.ClassID == (int)ItemClass.EQUIP && GetAutoPickUpFlag(line.Quality)
                             /* && eItem.EnchanceLevel <= 0 && eItem.EnchanceExp <= 0 && eItem.StarLevel <= 0*/
                             && EquipStrengthenLogic.IsAutoEnchanceMaterial(eItem)
                            && AutoEquipGuid != GlobeVar.INVALID_GUID)
                        {
                            selllist.Add(eItem.Guid);  
                        }                       
                    }
                }                                                   
            }
            //做自动强化物品            
             if (selllist.Count > 0)
             {
                 //SysShopController.SellItem((int)GameItemContainer.Type.TYPE_BACKPACK, selllist);
                 CG_EQUIP_ENCHANCE equipEnchance = (CG_EQUIP_ENCHANCE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_EQUIP_ENCHANCE);
                 //发送消息包
                 equipEnchance.SetPacktype(1);
                 equipEnchance.SetEquipguid(AutoEquipGuid);
                 for (int i = 0; i < selllist.Count; ++i)
                 {
                     equipEnchance.AddMaterialguid(selllist[i]);
                 }                 
                 equipEnchance.SendPacket();
             }           
        }
        public void UpdateSellItem(int index)
        {
            if (BaseAttr.Level < GlobeVar.MAX_AUTOEQUIT_LIVE)
            {
                return;
            }    
            if (IsOpenAutoCombat == false)
            {
                return;
            }
            GameItemContainer BackPack = GameManager.gameManager.PlayerDataPool.BackPack;
            //List<ulong> selllist = new List<ulong>();
            //做自动强化物品
            GameItem eItem = BackPack.GetItem(index);
            if (eItem != null) //身上有这个药
            {
                Tab_CommonItem line = TableManager.GetCommonItemByID(eItem.DataID, 0);
                if (line != null)
                {
                    //计算品级及拾取规则                           
                    if (line.ClassID == (int)ItemClass.EQUIP && GetAutoPickUpFlag(line.Quality)
                       /* && eItem.EnchanceLevel <= 0 && eItem.EnchanceExp <= 0 && eItem.StarLevel <= 0*/
                         && EquipStrengthenLogic.IsAutoEnchanceMaterial(eItem)
                            && AutoEquipGuid != GlobeVar.INVALID_GUID)
                    {
                        //selllist.Add(eItem.Guid);
                        CG_EQUIP_ENCHANCE equipEnchance = (CG_EQUIP_ENCHANCE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_EQUIP_ENCHANCE);
                        equipEnchance.SetPacktype(1);
                        equipEnchance.SetEquipguid(AutoEquipGuid);
                        equipEnchance.AddMaterialguid(eItem.Guid);
                        equipEnchance.SendPacket();
                    }
                }            
            }
//             if (selllist.Count > 0)
//             {
//                 SysShopController.SellItem((int)GameItemContainer.Type.TYPE_BACKPACK, selllist);
//             }
        }
        public void UpdateSelectEquip()
        {
            if (AutoEquipGuid == GlobeVar.INVALID_GUID)
            {
                GameItemContainer EquipPack = GameManager.gameManager.PlayerDataPool.EquipPack;
                if (EquipPack != null)
                {
                    for (int index = 0; index < EquipPack.ContainerSize; index++)
                    {
                        GameItem equip = EquipPack.GetItem(index);
                        if (equip != null && equip.IsValid() && equip.IsBelt() == false)
                        {
                            AutoEquipGuid = equip.Guid;
                            SetAutoPickUpFlag((int)GameDefine_Globe.AUTOCOMBAT_PICKUP_TYPE.PICKUP_EQUIP1, true);
                            SetAutoPickUpFlag((int)GameDefine_Globe.AUTOCOMBAT_PICKUP_TYPE.PICKUP_EQUIP2, true);
                            ServerAutoInfo();
                            UpdateSellItem();
                            return;
                        }
                    }
                }                
            }           
        }
        public bool IsAutoSellItem(int nDataId)
        {
            if (IsOpenAutoCombat == false)
            {
                return false;
            }
            Tab_CommonItem line = TableManager.GetCommonItemByID(nDataId, 0);
            if (line != null)
            {
                //计算品级及拾取规则                           
                if ( line.ClassID == (int)ItemClass.EQUIP 
                    && GetAutoPickUpFlag(line.Quality) )
                {
                    return true;
                }
            }
            return false;
        }
        private int m_nAutoSceneId = -1;
        private Vector3 m_AutoPos = new Vector3(0, 0, 0);
        public void UpdateAutoAnteMortem()
        {
            if (IsOpenAutoCombat == false)
            {
                return ;
            }
            if (IsDie())
            {
                 m_nAutoSceneId = GameManager.gameManager.RunningScene;
                 m_AutoPos = m_ObjTransform.position;
            }
            else
            {
                if (m_nAutoSceneId == GameManager.gameManager.RunningScene)
                {
                    AutoSearchPoint point = new AutoSearchPoint(m_nAutoSceneId, m_AutoPos.x, m_AutoPos.z);
                    if (GameManager.gameManager && GameManager.gameManager.AutoSearch)
                    {
                        GameManager.gameManager.AutoSearch.BuildPath(point);
                    }
                    
                }
            }
        }
    }
}
