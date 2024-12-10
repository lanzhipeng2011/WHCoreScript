
/********************************************************************************
 *	文件名：	Obj_NPC.cs
 *	全路径：	\Script\Obj\Obj_NPC.cs
 *	创建人：	李嘉
 *	创建时间：2013-10-25
 *
 *	功能说明：游戏NPC的Obj逻辑类
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Games.GlobeDefine;
using Games.AI_Logic;
using GCGame.Table;
using Games.Scene;
using GCGame;

using Games.Animation_Modle;
namespace Games.LogicObj
{
	public class Obj_NPC : Obj_Character
	{
		protected GameObject m_MissionBoard;       // 头顶任务标记
	    private bool m_bIsBornCreate;//是否是刚刷出来的
	    private bool m_IsNameBoard = false;//标记是否已经显示了名字版(初始化时)
        
		public Obj_NPC()
		{
			m_ObjType = GameDefine_Globe.OBJ_TYPE.OBJ_NPC;
			m_listMissionID = new List<int>();
		}

	   
		public override bool Init(Obj_Init_Data initData)
        {
            if (null == m_ObjTransform)
            {
                m_ObjTransform = transform;
            }
			Vector3 newpos = new Vector3 (initData.m_fX, 0, initData.m_fZ);
			newpos = ActiveScene.GetTerrainPosition(newpos);
            m_ObjTransform.position = newpos;
          
           

            BaseAttr.RoleBaseID = initData.m_RoleBaseID;
            BaseAttr.MoveSpeed = initData.m_MoveSpeed;
            Tab_RoleBaseAttr roleBaseTab = TableManager.GetRoleBaseAttrByID(BaseAttr.RoleBaseID, 0);
            if (roleBaseTab != null)
            {
                //初始化CharModelID，并读取部分客户端信息
                ModelID = roleBaseTab.CharModelID;
                Tab_CharModel charModel = TableManager.GetCharModelByID(ModelID, 0);
                if (null != charModel)
                {
                    BaseAttr.HeadPic = charModel.HeadPic;
                    //设置动作路径
                    AnimationFilePath = charModel.AnimPath;
                    //设置名字版高度
                  //  DeltaHeight = charModel.HeadInfoHeight;
		
					if (null != roleBaseTab&&(BaseAttr.RoleBaseID==20201||BaseAttr.RoleBaseID==40205||BaseAttr.RoleBaseID==20206 ||BaseAttr.RoleBaseID==30201||BaseAttr.RoleBaseID==40206||BaseAttr.RoleBaseID==100204||BaseAttr.RoleBaseID==100205))
					{
						//transform.localScale = new Vector3(charModel.Scale*1.6f, charModel.Scale*1.6f, charModel.Scale*1.6f);
						transform.localScale = new Vector3(charModel.Scale, charModel.Scale, charModel.Scale);
						DeltaHeight = charModel.HeadInfoHeight*1.5f;

					}
					else
					{
					DeltaHeight = charModel.HeadInfoHeight;
                    transform.localScale = new Vector3(charModel.Scale, charModel.Scale, charModel.Scale);
					}
                }
            }

            //服务器发过来的信息               
            this.ServerID = initData.m_ServerID;
            this.BornPosX = initData.m_fX;
            this.BornPosY = gameObject.transform.position.y;
            this.BornPosZ = initData.m_fZ;
            BaseAttr.RoleName = initData.m_StrName;
		    BaseAttr.Force = initData.m_Force;
            BaseAttr.Die = initData.m_IsDie;
            m_ObjTransform.rotation =Utils.DirServerToClient(initData.m_fDir);
		    StealthLev = initData.m_StealthLev;
		    m_bIsBornCreate = initData.m_bNpcBornCreate;
            OptStealthLevChange();
            // NPC挂任务
            AddDialogMission();

            //组件在初始化数据后进行
            
            //初始化寻路代理
			if(roleBaseTab.IsZA==0)
                InitNavAgent();

            //初始化动画，需要在AnimationFilePath被赋值后进行
            AnimLogic = gameObject.GetComponent<AnimationLogic>();
            if (AnimLogic == null)
            {
                AnimLogic = gameObject.AddComponent<AnimationLogic>();
            }

            //初始化特效
            ObjEffectLogic = gameObject.GetComponent<EffectLogic>();
            if (ObjEffectLogic == null)
            {
                ObjEffectLogic = gameObject.AddComponent<EffectLogic>();

                InitEffect();
            }
			//加箭头指引
			if (roleBaseTab.IsZA == 1)
								PlayEffect (305);//
            //初始化AutoMove功能模块
            if (gameObject.GetComponent<AutoMove>() == null)
            {
                gameObject.AddComponent<AutoMove>();
            }
            
            if (IsDie())
            {
                OnCorpse();
            }
            else
            {
                if (Objanimation!=null)
                {
                    Objanimation.Stop();
                }
                CurObjAnimState = GameDefine_Globe.OBJ_ANIMSTATE.STATE_NORMOR;
            }

            //创新跟玩家一样的模型
            m_ModelVisualID = initData.m_ModelVisualID;
            m_nProfession = initData.m_nProfession;
            m_WeaponDataID = initData.m_WeaponDataID;
            m_WeaponEffectGem = initData.m_WeaponDataID;
            if (initData.m_ModelVisualID != GlobeVar.INVALID_ID && initData.m_nProfession != -1)
            {
                ReloadPlayerModelVisual(initData.m_ModelVisualID, initData.m_nProfession);
            }
            
            //初始化名字版
            //InitNameBoard();
            if (!m_IsNameBoard)
            {
                InitNameBoard();
            }
            else
            {
                if (m_MissionBoard != null)
                {
                    //m_MissionBoard.SetActive(true);
                    ShowNameBoard();
                }
            }

            m_mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
            
            // 帮会活动NPC判断
            m_bIsGuildActivityBoss = IsGuildBoss();

			return base.Init(initData);
		}
		private float m_fBornPosX = 0.0f;
		public float BornPosX
		{
			get { return m_fBornPosX; }
			set { m_fBornPosX = value; }
		}

		private float m_fBornPosY = 0.0f;
		public float BornPosY
		{
			get { return m_fBornPosY; }
			set { m_fBornPosY = value; }
		}

		private float m_fBornPosZ = 0.0f;
		public float BornPosZ
		{
			get { return m_fBornPosZ; }
			set { m_fBornPosZ = value; }
		}
        private int m_nProfession;      // 职业 （创建玩家用的和模仿玩家NPC模型）
        public int Profession
        {
            get { return m_nProfession; }
            set { m_nProfession = value; }
        }
        private int m_WeaponDataID;     // 当前武器
        public int WeaponDataID
        {
            get { return m_WeaponDataID; }
            set { m_WeaponDataID = value; }
        }
        private int m_ModelVisualID;    // 当前模型外观ID
        public int ModelVisualID
        {
            get { return m_ModelVisualID; }
            set { m_ModelVisualID = value; }
        }
        private int m_WeaponEffectGem;  // 武器特效宝石
        public int WeaponEffectGem
        {
            get { return m_WeaponEffectGem; }
            set { m_WeaponEffectGem = value; }
        }

        /// <summary>
        /// 停止NPC当前播放的特效
        /// </summary>
	    public void StopNPCEffect()
	    {
	        if (ObjEffectLogic != null)
	        {
                ObjEffectLogic.NPCStopEffect();
	        }
	    }

        /// <summary>
        /// NPC复活后，重新值下溶解相关值
        /// </summary>
        public void PlayUnDissolve()
        {
            if (BaseAttr == null) return;

            m_bIsPlayDissolve = false;
            m_bIsPlayUnDissolve = false;
            Tab_RoleBaseAttr _roleBaseAttr = TableManager.GetRoleBaseAttrByID(BaseAttr.RoleBaseID, 0);
            if (_roleBaseAttr != null)
            {
                //溶解特效
                if (_roleBaseAttr.RampType == 2 || _roleBaseAttr.RampType == 3)
                {
                    ShowDissolveNPC();
                }

                _roleBaseAttr = null;
            }
        }

	    public override bool OnDie()
		{
            if (IsDie())
            {
                return false;
            }
            base.OnDie();
            if (m_NameBoard)
            {
                m_NameBoard.color = CharacterDefine.NPC_COLOR_DIE;
            }
            //死亡特效
		    Tab_RoleBaseAttr _roleBaseAttr = TableManager.GetRoleBaseAttrByID(BaseAttr.RoleBaseID,0);
		    if (_roleBaseAttr !=null )
		    {
				//float fSpeed = 1.0f/(_roleBaseAttr.CorpseTime/1000.0f -0.5f);
				//PlayDissolve(fSpeed, 0.5f);
                //溶解特效
                if (_roleBaseAttr.RampType == 2 || _roleBaseAttr.RampType == 3)
                {
                    float fSpeed = 1.0f/(_roleBaseAttr.CorpseTime/1000.0f -0.5f);
                    PlayDissolve(fSpeed, 0.5f);
		        }
                //死亡特效
		        if (_roleBaseAttr.DieEffectID!= -1)
                {
                    PlayEffect(_roleBaseAttr.DieEffectID);
                }
		    }
		    return true;
		}
        // 重载模型回调
        public override void OnReloadModle()
        {
           base.OnReloadModle();
           if (m_bIsBornCreate)
           {
                Tab_RoleBaseAttr _roleBaseAttr = TableManager.GetRoleBaseAttrByID(BaseAttr.RoleBaseID, 0);
                if (_roleBaseAttr != null )
                {
                    //反溶解特效
                    if (_roleBaseAttr.RampType ==1 || _roleBaseAttr.RampType ==3)
                    {
                        PlayUnDissolve(0.5f,0.0f);
                    }
                    //死亡特效
                    if (_roleBaseAttr.BornEffctID != -1)
                    {
                        PlayEffect(_roleBaseAttr.BornEffctID);
                    }
                }
                m_bIsBornCreate = false;
           }
           //雁门关的BOSS 刷新身上加上一个特效
           if (BaseAttr.RoleBaseID == 58)
           {
               PlayEffect(170);
           }
        }

        //进入可视区域
        void OnBecameVisible()
        {
            if (!IsNeedBecameVisible())
                return;

            //设置是否在视口内标记位，为其他系统优化判断标识
            ModelInViewPort = true;
            
            //显示名字版
            if (null != m_HeadInfoBoard)
            {
                m_HeadInfoBoard.SetActive(true);
            }

            //显示模型
            if (null != ModelNode && IsVisibleChar())
            {
                ModelNode.SetActive(true);
                OnSwithObjAnimState(CurObjAnimState);
            }

            //如果已经死亡，则切换到死亡状态
            if (IsDie())
            {
                OnCorpse();
            }
        }

        //离开可视区域
        void OnBecameInvisible()
        {
            if (!IsNeedBecameVisible())
                return;

            //设置是否在视口内标记位，为其他系统优化判断标识
            ModelInViewPort = false;

            //隐藏名字版
            if (null != m_HeadInfoBoard)
            {
                m_HeadInfoBoard.SetActive(false);
            }

            //隐藏模型
            if (null != ModelNode)
            {
                ModelNode.SetActive(false);
            }
        }

		void FixedUpdate()
        {
            UpdateTargetMove();
            UpdateAnimation();
            UpdateAttckFly();//击飞
            if (m_bIsPlayDissolve || m_bIsPlayUnDissolve)
            {
                UpdateDissolve();
            }
			UpdateShanBai ();
            //多次冒血的
            UpdateShowMultiShowDamageBoard();
            //技能结束检测
            if (m_SkillCore != null)
            {
                m_SkillCore.CheckSkillShouldFinish();
            }
		    AutoShowNPCDialogNear();
        }
		//===开放npc与主角距离用于隐藏头像
	    public float m_fLastDis2MainPlayer = 0.0f; //Npc上次离主角的距离
        private Obj_MainPlayer m_mainPlayer = null;
        void AutoShowNPCDialogNear()
	    {
            if (m_mainPlayer == null)
	        {
	            return;
	        }
            //当前距离NPC的距离
            float nDis2MainPlayer = Vector3.Distance(Position, m_mainPlayer.Position);
			int BussinessAccID = TableManager.GetSceneNpcByID (GlobeVar.GUILDBUSINESS_ACCEPTNPC, 0).DataID;
            //是黑市商人
	        if (BaseAttr.RoleBaseID == GlobeVar.BLACKMAKETDATAID_ERHAI ||
	            BaseAttr.RoleBaseID == GlobeVar.BLACKMAKETDATAID_SHUZHOU)
	        {
                if (nDis2MainPlayer <= 2.0 && m_fLastDis2MainPlayer > 2.0) //来到离黑市商人2码内的距离 自动弹出对话框
	            {
                    if (m_mainPlayer.IsMoving) //等停下来再进行操作 
	                {
	                    return;
	                }
                    if (OptionDialogLogic.Instance() == null) //没有弹出对话(有选项)
                    {
                        Singleton<DialogCore>.GetInstance().Show(this);
                    } 
	            }
	        }
            //是帮会NPC
			else if (BaseAttr.RoleBaseID == BussinessAccID)
            {
                if (nDis2MainPlayer <= 2.0 && m_fLastDis2MainPlayer > 2.0) //来到离帮会NPC2码内的距离 自动弹出对话框
                {
                    if (m_mainPlayer.IsMoving) //等停下来再进行操作 
                    {
                        return;
                    }
                    if (MissionInfoController.Instance() == null) //没有弹出对话(无选项)
                    {
                        Singleton<DialogCore>.GetInstance().Show(this);
                    }
                }
            }
            //是仓库NPC
            else if (BaseAttr.RoleBaseID == GlobeVar.CANGKU1_SHUZHOU ||
                BaseAttr.RoleBaseID == GlobeVar.CANGKU2_SHUZHOU)
            {
                if (nDis2MainPlayer <= 2.0 && m_fLastDis2MainPlayer > 2.0) //来到离黑市商人2码内的距离 自动弹出对话框
                {
                    if (m_mainPlayer.IsMoving) //等停下来再进行操作 
                    {
                        return;
                    }
                    if (OptionDialogLogic.Instance() == null) //没有弹出对话(有选项)
                    {
                        Singleton<DialogCore>.GetInstance().Show(this);
                    }
                }
            }
            //奖励npc
            else if (BaseAttr.RoleBaseID == GlobeVar.AWARD_NPCID &&
                GameManager.gameManager.PlayerDataPool.IsServerFlagOpen(SERVER_FLAGS_ENUM.FLAG_CYFANS) )
            {
                if (nDis2MainPlayer <= 2.0 && m_fLastDis2MainPlayer > 2.0)
                {
                    if (m_mainPlayer.IsMoving) //等停下来再进行操作 
                    {
                        return;
                    }
                    if (OptionDialogLogic.Instance() == null) //没有弹出对话(有选项)
                    {
                        Singleton<DialogCore>.GetInstance().Show(this);
                    }
                }
            }

            m_fLastDis2MainPlayer = nDis2MainPlayer;
	    }
		void InitNameBoard()
		{
            ResourceManager.LoadHeadInfoPrefab(UIInfo.NPCHeadInfo, gameObject, "NPCHeadInfo", OnLoadNameBoard);                    
		}

        void OnLoadNameBoard(GameObject objNameBoard)
        {
            if (null != objNameBoard)
            {
                m_HeadInfoBoard = objNameBoard;
                Transform HeadInfoTransfom = m_HeadInfoBoard.transform;
                m_NameBoard = HeadInfoTransfom.FindChild("NameBoardOffset").FindChild("NameBoard").GetComponent<UILabel>();
                //m_DamageBoard = HeadInfoTransfom.FindChild("DamageBoard").GetComponent<DamageBoardManager>();
                m_MissionBoard = HeadInfoTransfom.FindChild("MissionBoard").gameObject;
                if (null != m_MissionBoard)
                {
                    if (MissionList.Count > 0)
                    {
                        m_MissionBoard.SetActive(true);

                    }
                    else
                    {
                        m_MissionBoard.SetActive(false);
                    }
                }
                //if (null != m_DamageBoard)
                //{
                //    m_DamageBoard.gameObject.SetActive(false);
                //}

                ShowNameBoard();
                m_IsNameBoard = true;
            }    
        }

		//////////////////////////////////////////////////////////////////////////
		//NPC对话相关处理
		//////////////////////////////////////////////////////////////////////////
		private int m_nDefaultDialogID = -1;        //NPC对话索引列表
		public int DefaultDialogID
		{
			get { return m_nDefaultDialogID; }
			set { m_nDefaultDialogID = value; }
		}

		private List<int> m_listMissionID;        //NPC任务脚本
		public List<int> MissionList
		{
			get { return m_listMissionID; }
		}
		void AddMissionToList(int nMissionID)
		{
			m_listMissionID.Add(nMissionID);
		}
		void ClearMissionList()
		{
			m_listMissionID.Clear();
		}
        public bool IsHaveMission(int nMissionID)
        {
            if (m_listMissionID.Contains(nMissionID))
            {
                return true;
            }
            
            return false;
        }
		protected override void OnMoveOver()
		{
			//怪物向NPC 移动 到达玩家 开始攻击
			
		}

		public override  void OnAnimationFinish(int animationID)
		{
			base.OnAnimationFinish(animationID);
		}

		public override void OnEnterCombat(Obj_Character Attacker)
		{
			if (IsDie())
			{
				return;
			}

			base.OnEnterCombat(Attacker);
			if (Controller == null)
			{
				return;
			}
			if (Controller.CurrentAIState.AIType != CharacterDefine.AI_TYPE.AI_TYPE_COMBAT)
			{
			   Controller.EnterCombat();
			}

			if (Attacker)
			{
				Controller.ThreadInfo.AddThreat(Attacker.gameObject, 10);
			}
		}

		public override void OnLevelCombat(Obj_Character Attacker )
		{
			base.OnLevelCombat(Attacker);
            IsTracing = false;
			if (Controller == null)
			{
				return;
			}
			if (Controller.CurrentAIState.AIType == CharacterDefine.AI_TYPE.AI_TYPE_COMBAT)
			{
				Controller.LeaveCombat();
			}
			if (Attacker)
			{
				Controller.ThreadInfo.ResetThreat(Attacker.gameObject);
			}
		}

        public void AddDialogMission()
        {
            Tab_RoleBaseAttr RoleBase = TableManager.GetRoleBaseAttrByID(BaseAttr.RoleBaseID, 0);
            if (RoleBase == null)
                return;

            Tab_NpcDialog DialogLine = TableManager.GetNpcDialogByID(RoleBase.DialogID, 0);
            if (DialogLine != null)
            {
                m_nDefaultDialogID = RoleBase.DialogID;
                for (int i = 0; i < DialogLine.getMissionIDCount(); ++i)
                {
                    int nMissionID = DialogLine.GetMissionIDbyIndex(i);
                    if (nMissionID >= 0)
                    {
                        m_listMissionID.Add(nMissionID);
                    }
                }
            }
        }
        public override void OptNameChange()//名字变化后的操作
        {
           ShowNameBoard();
        }

        public override void OptForceChange()//势力变化后的操作
        {
            base.OptForceChange();
        }
        
        public override Color GetNameBoardColor()
        {
            Color curColr = Color.white;

            if (Reputation.IsFriend(this))
            {
                curColr = CharacterDefine.NPC_COLOR_FRIEND;
            }
            else if (Reputation.IsNeutral(this))
            {
                curColr = CharacterDefine.NPC_COLOR_NEUTRAL;
            }
            else if (Reputation.IsEnemy(this))
            {
                curColr = CharacterDefine.NPC_COLOR_ENEMY;
            }

            return curColr;
        }

        public bool IsNeedBecameVisible()
        {
            if (BaseAttr.RoleBaseID == GlobeVar.PARADE_BUSID)
            {
                return false;
            }
            return true;
        }
        public override void OnBindOpt(Obj_Character obj)
        {
            base.OnBindOpt(obj);

            //婚车特殊摆设
            if (BaseAttr.RoleBaseID == GlobeVar.PARADE_BUSID && obj != null)
            {
                Transform playermodel = obj.transform.FindChild("Model");
                Transform ridepoint = gameObject.transform.FindChild("Model/All/Bip01/Bip01 Pelvis/Bip01 Spine/Ride_Point01");
                Transform seatpoint = gameObject.transform.FindChild("Model/All/Bone02/Ride_Point02");
                if (playermodel == null || ridepoint == null || seatpoint == null)
                {
                    return;
                }
                if (GetBindChildIndex(obj.ServerID) == 0 && ridepoint.childCount < 1)
                {
                    obj.gameObject.transform.localPosition = new Vector3(0.0f, 0.78f, -0.47f);
                    GameObject fakeobj = (GameObject)Instantiate(playermodel.gameObject);
                    if (fakeobj != null)
                    {
                        fakeobj.SetActive(true);
                        fakeobj.transform.parent = ridepoint;
                        fakeobj.transform.localPosition = Vector3.zero;
                        fakeobj.transform.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
                        fakeobj.transform.localScale = Vector3.one;

                        AnimationLogic al = ridepoint.gameObject.AddComponent<AnimationLogic>();
                        if (al != null)
                        {
                            al.InitState(fakeobj);

                            Tab_RoleBaseAttr role = TableManager.GetRoleBaseAttrByID(obj.BaseAttr.RoleBaseID, 0);
                            if (role != null)
                            {
                                Tab_CharModel mo = TableManager.GetCharModelByID(role.CharModelID, 0);
                                if (mo != null)
                                {
                                    al.AnimResFilePath = mo.AnimPath;
                                }
                            }

                            int animId = -1;
                            //sl/ts/dl/xy
                            if (obj.BaseAttr.RoleBaseID == 0)
                                animId = 153;
                            else if (obj.BaseAttr.RoleBaseID == 1)
                                animId = 155;
                            else if (obj.BaseAttr.RoleBaseID == 2)
                                animId = 159;
                            else if (obj.BaseAttr.RoleBaseID == 3)
                                animId = 157;

                            if (animId > 0)
                            {
                                al.Play(animId);
                            }
                        }
                    }
                    return;
                }
                if (GetBindChildIndex(obj.ServerID) == 1 && seatpoint.childCount < 1)
                {
                    obj.gameObject.transform.localPosition = new Vector3(0.0f, 0.47f, -1.83f);
                    GameObject fakeobj = (GameObject)Instantiate(playermodel.gameObject);
                    if (fakeobj != null)
                    {
                        fakeobj.SetActive(true);
                        fakeobj.transform.parent = seatpoint;
                        fakeobj.transform.localPosition = Vector3.zero;
                        fakeobj.transform.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
                        fakeobj.transform.localScale = Vector3.one;

                        AnimationLogic al = seatpoint.gameObject.AddComponent<AnimationLogic>();
                        if (al != null)
                        {
                            al.InitState(fakeobj);

                            Tab_RoleBaseAttr role = TableManager.GetRoleBaseAttrByID(obj.BaseAttr.RoleBaseID, 0);
                            if (role != null)
                            {
                                Tab_CharModel mo = TableManager.GetCharModelByID(role.CharModelID, 0);
                                if (mo != null)
                                {
                                    al.AnimResFilePath = mo.AnimPath;
                                }
                            }

                            int animId = -1;
                            //sl/ts/dl/xy
                            if (obj.BaseAttr.RoleBaseID == 0)
                                animId = 146;
                            else if (obj.BaseAttr.RoleBaseID == 1)
                                animId = 148;
                            else if (obj.BaseAttr.RoleBaseID == 2)
                                animId = 152;
                            else if (obj.BaseAttr.RoleBaseID == 3)
                                animId = 150;
                            if (animId > 0)
                            {
                                al.Play(animId);
                            }
                        }
                    }
                    return;
                }
            }
        }

        private bool m_bIsGuildActivityBoss = false;    // 帮会活动Boss;
        public bool IsGuildActivityBoss
        {
            get { return m_bIsGuildActivityBoss; }
            set { m_bIsGuildActivityBoss = value; }
        }

        bool IsGuildBoss()
        {
            Dictionary<int, List<Tab_GuildActivityBoss> > GuildBossTab = TableManager.GetGuildActivityBoss();
            if (GuildBossTab == null)
            {
                return false;
            }
            for (int i = 1; i <= GuildBossTab.Count; i++)
            {
                Tab_GuildActivityBoss Tab = TableManager.GetGuildActivityBossByID(i, 0);
                if (Tab == null)
                {
                    continue;
                }

                if (BaseAttr.RoleBaseID == Tab.BossID)
                {
                    return true;
                }
            }
            return false;
        }
	}
}
