/********************************************************************************
 *	文件名：	Obj_MainPlayer.cs
 *	全路径：	\Script\Obj\Obj_MainPlayer.cs
 *	创建人：	李嘉
 *	创建时间：2013-10-25
 *
 *	功能说明：游戏主角Obj逻辑类
 *	修改记录：
*********************************************************************************/

using System.Runtime.Serialization.Formatters;
using Games.AI_Logic;
using Games.ImpactModle;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Games.GlobeDefine;
using Games.Events;
using Games.Scene;
using GCGame;

using GCGame.Table;
using Games.SkillModle;
using Games.Animation_Modle;
using Games.Item;
using Module.Log;
using System;
using Games.Fellow;

namespace Games.LogicObj
{
    
	public partial class Obj_MainPlayer : Obj_OtherPlayer
	{
        // 加载模型相关
        private static int m_originalModelID = -1;
        public static int OriginalModelID { set { m_originalModelID = value; } get { return m_originalModelID; } }
        private static int m_changeModelID = -1;
        public static int ChangeModelID { set { m_changeModelID = value; } get { return m_changeModelID; } }

		public Obj_MainPlayer()
		{
			m_ObjType = GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER;
			m_BaseAttr = new BaseAttr();
		   
		}
		
		public override bool Init(Obj_Init_Data initData)
		{
            //主角进行Init的时候调用一次Unload方法
            Resources.UnloadUnusedAssets();
            LastHeartBeatTime = -1;
            return true;
		}
		void Awake()
        {
            if (null == m_ObjTransform)
            {
                m_ObjTransform = transform;
            }
		}

		ThirdPersonController m_Thirdcontroller = null;
		public ThirdPersonController Thirdcontroller
		{
			get { return m_Thirdcontroller; }
		}
        
        public override int Profession
        {
            get { return GameManager.gameManager.PlayerDataPool.Profession; }
            set { GameManager.gameManager.PlayerDataPool.Profession = value; }
        }
        public override System.UInt64 GuildGUID
        {
            get { return GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid; }
            set { GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid = value; }
        }
        public System.UInt64 LoverGUID
        {
            get { return GameManager.gameManager.PlayerDataPool.LoverGUID; }
            set { GameManager.gameManager.PlayerDataPool.LoverGUID = value; }
        }
        private UInt64 m_CurUseMountItemGuid;

        public float LastHeartBeatTime = -1;
		void Start()
		{
			if ((int)GameDefine_Globe.SCENE_DEFINE.SCENE_LOGIN != GameManager.gameManager.RunningScene)
			{
                InitAutoInfo();
				InitSpecialInfo();
				InitNameBoard();
                //更新人物头像显示
                OptHPChange();
                OptMPChange();
                OptXPChange();
                OptLevelChange();
                OptForceChange();
                OptStealthLevChange();
                OptNameChange();
                OptChangPKModle();
                OnExpChange();
                OnOffLineExpChange();
                ShowPlayerTitleInvestitive();
                InitSkillInfo();
                InitCangJingGeInfo();
                if (PlayerFrameLogic.Instance() != null) 
                {
                    PlayerFrameLogic.Instance().UpdateFunctionCD();
                }
                //在监狱的话 打开PK界面 放在更新公告之前
                if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_JIANYU)
                {
                    UIManager.ShowUI(UIInfo.PKSetInfo);
                }
                NoticeLogic.TryOpen();    
                //显示帮战的推送
                if (GameManager.gameManager.PlayerDataPool.WarPushMessaeg.Count >0)
			    {
                    if (GuildWarPushMessageLogic.Instance() == null)
                    {
                        UIManager.ShowUI(UIInfo.GuilWarPushMessage);
                    }
			    }
			}

            if (null != GameManager.gameManager)
            {
                m_playDataTool = GameManager.gameManager.PlayerDataPool;
            }
            

            //开始每分钟一次的循环
            StartCoroutine(UpdatePerMinute());  

            if (PlayerPreferenceData.LeftTabChoose == 1)
            {
                if (GameManager.gameManager.PlayerDataPool.TeamInfo.TeamID >= 0)
                {
                    if (null != TeamList.Instance())
                    {
                        TeamList.Instance().UpdateTeamMember();
                    }
                }
                if (MissionDialogAndLeftTabsLogic.Instance() != null)
                {
                    MissionDialogAndLeftTabsLogic.Instance().UpdateTeamInfo();
                }
            }

 
            if (null != PlayerFrameLogic.Instance())
            {
                PlayerFrameLogic.Instance().SetTeamCaptain(IsTeamLeader());
            }
            if (m_ObjTransform !=null)
		    {
                m_fLastSyncPos = m_ObjTransform.position;
		    }
#if UNITY_ANDROID
			PlayerFrameLogic.Instance().m_FirstChild.SetActive(true);
            if(SkillBarLogic.Instance()!=null)
			SkillBarLogic.Instance().m_FirstChild.SetActive(true);
#endif
            LastHeartBeatTime = -1;
		}

        //开始每秒一次的循环
       

        //开始每分钟一次循环
        IEnumerator UpdatePerMinute()
        {
            while (true)
            {
                yield return new WaitForSeconds(60);

                //针对挂机崩溃的情况，如果是挂机状态，则进行一分钟一次的系统回收
                if (IsOpenAutoCombat)
                {
                    Resources.UnloadUnusedAssets();
                }
            }
        }
      
		//更新Obj_MainPlayer逻辑数据

        float updateSecondStep = 0;
        void UpdateSecond() 
        {
            UpdateReliveEntryTime();
            //体能恢复倒计时
            StaminaTimerFunc();
            if (IsDie() != true)
            {
                //更新自动打怪
                UpdateAuto();
                UpdateTarget();
                //当前选中目标距离检测
                UpdateSelectTarget();
            }
        }
        void Update()
        {
            updateSecondStep += Time.deltaTime;
            if (updateSecondStep > 1) 
            {
                UpdateSecond();
                updateSecondStep = 0;
            }

            UpdateCameraController();
            UpdateTargetMove();
            UpdateQingGong();       //由于轻功可能改变玩家坐标，所以放在UpdateTargetMove之后进行   
			updateWeaponPoint ();

			//====
			updateBreakForActStand ();
			//======

        }

        PlayerData m_playDataTool = null;
		void FixedUpdate()
        {
            UpdateAnimation();
			UpdateAttckSY ();
			UpdateAttckCF (this.Profession);
			UpdateAttckYS ();
			UpdateAttckHT ();
			UpdateAttckXS ();
			//UpdateAttckPGCF (this.Profession);
			//UpdataTarget ();
            // 活动添加
            if (null != m_playDataTool)
            {
                m_playDataTool.Tick_Award();
                m_playDataTool.Tick_MoneyTreeAward();
                m_playDataTool.DailyLuckyDrawData.Tick_FreeCDTime();
                m_playDataTool.Tick_FellowGainCD();
            }

            //涉及逻辑更新函数
            UpdateStep();
            UpdateTeamFollow();

            //同步位置给Server其他玩家
            if (GameManager.gameManager.OnLineState)
            {
                //有父节点的时候，跟随父节点移动，不需要发送同步包
                if (BindParent < 0)
                {
                    SyncPosToServer();
                }
            }

            UpdateSkillCDTime();

            //多次冒血的
            UpdateShowMultiShowDamageBoard();
            //技能结束检测
            if (m_SkillCore != null)
            {
                m_SkillCore.CheckSkillShouldFinish();
            }
            //自动战斗 中断状态检测
            UpdateAutoCombatBreakState();
			UpdateAutoXunLuBreakState ();
        }

        //更新玩家脚本
        void UpdateStep()
        {
            if (IsMoving && null != m_Objanimation)
            {
                if (m_Objanimation.IsPlaying("Skill_Mohewuliang") == false && m_Objanimation.IsPlaying("Skill_Zhurongzhang_Loop") == false)
                {
                    if (GetEquipMountID() > 0) //有坐骑播放坐骑声音
                    {
                        Tab_MountBase MountBase = TableManager.GetMountBaseByID(GetEquipMountID(), 0);
                        if (null != MountBase)
                        {
                            Tab_CharMount MountTab = TableManager.GetCharMountByID(MountBase.ModelID, 0);
                            if (null != MountTab)
                            {
                                GameManager.gameManager.SoundManager.PlaySoundEffect(MountTab.SoundID);
                            }
                        }
                    }
                    else //播放玩家跑步声音
                    {
                        if (Profession == (int)CharacterDefine.PROFESSION.TIANSHAN) //天山特殊化
                        {
                            GameManager.gameManager.SoundManager.PlaySoundEffect(22);   //footsteps_tianshan
                        }
                        else
                        {
                            GameManager.gameManager.SoundManager.PlaySoundEffect(22);   //footsteps
                        }
                    }
                }
            }

        }

        public static float m_fTimeSecond = Time.realtimeSinceStartup;
        void UpdateReliveEntryTime()
        {
            float ftimeSec = Time.realtimeSinceStartup;
            int nTimeData = (int)(ftimeSec - m_fTimeSecond);
            if (nTimeData > 0)
            {
                if (ReliveEntryTime > 0)
                {
                    ReliveEntryTime = ReliveEntryTime - nTimeData;
                    if (ReliveEntryTime < 0)
                    {
                        ReliveEntryTime = 0;
                    }                    
                }
                m_fTimeSecond = ftimeSec;
            }
        }

        //////////////////////////////////////////////////////////////////////////
        //向服务器同步相关
        //////////////////////////////////////////////////////////////////////////
        //同步位置信息间隔
        const float m_fSyncPosTimeInterval = 0.2f;
        float m_fLastSyncPosTime = 0.0f;
        Vector3 m_fLastSyncPos = new Vector3(0.0f, 0.0f, 0.0f);
        public UnityEngine.Vector3 LastSyncPos
        {
            get { return m_fLastSyncPos; }
            set { m_fLastSyncPos = value; }
        }
        //同步位置信息给Server
        void SyncPosToServer()
        {
            //轻功状态下不同步
            if (true == QingGongState)
            {
                return;
            }
			if (m_bIsCanPGCF || m_bIsCanCF)
			{
				return ;
			}
			if (SkillCore != null && SkillCore.UsingSkillBaseInfo != null) {
				if (SkillCore.UsingSkillBaseInfo.IsMove != 1) {
					return;
				}
			}
            if (Time.time - m_fLastSyncPosTime > m_fSyncPosTimeInterval)
            {
                m_fLastSyncPosTime = Time.time;

                float Dpos = Vector3.Distance(m_fLastSyncPos, m_ObjTransform.position);
                //检查上一次同步的坐标和当前坐标是否有差距
                if (Dpos < 0.1f)
                {
                    return;
                }
                else
                {
                    m_fLastSyncPos = m_ObjTransform.position;
                }
                //新Obj同步机制修改，改用CG_MOVE包
                CG_MOVE movPacket = (CG_MOVE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_MOVE);
                movPacket.Poscount = 1;
                movPacket.AddPosx((int)(m_ObjTransform.position.x * 100));
                movPacket.AddPosz((int)(m_ObjTransform.position.z * 100));
			//	Debug.LogError("x "+(m_ObjTransform.position.x * 100));
			//	Debug.LogError("ismoving"+IsMoving);
                if (IsMoving)
                {
                    movPacket.Ismoving =1;
                }
                else
                {
                    movPacket.Ismoving =0;
                }
				//movPacket.Ismoving =1;
                movPacket.SendPacket();

                if (null != CollectItemSliderLogic.Instance())
                {
                    CollectItemSliderLogic.Instance().CloseCollect();
                }

            }
        }

        public override BaseAttr BaseAttr
        {
            //屏蔽掉自己的 m_BaseAttr，而从GameManager中读取，保证切场景依然存在
            get { return GameManager.gameManager.PlayerDataPool.MainPlayerBaseAttr; }
            set { GameManager.gameManager.PlayerDataPool.MainPlayerBaseAttr = value; }
        }

		void InitNameBoard()
		{
            ResourceManager.LoadHeadInfoPrefab(UIInfo.PlayerHeadInfo, gameObject, "PlayerHeadInfo", OnLoadNameBoard);
            
		}

        void OnLoadNameBoard(GameObject objNameBoard)
        {
            m_HeadInfoBoard = objNameBoard;
            if (null != m_HeadInfoBoard)
            {
                m_playerHeadInfo = m_HeadInfoBoard.GetComponent<PlayerHeadInfo>();
                if (null != m_playerHeadInfo)
                {
                    m_NameBoard = m_playerHeadInfo.m_LabelName;
                }
                /*
                m_OriginalHeadInfo = m_HeadInfoBoard.transform.FindChild("NameBoardOffset").FindChild("OriginalHeadInfo").gameObject;
                m_NameBoard = m_OriginalHeadInfo.transform.FindChild("NameBoard").GetComponent<UILabel>();
                //m_DamageBoard = m_HeadInfoBoard.transform.FindChild("DamageBoard").GetComponent<DamageBoardManager>();
                m_TitleInvestitiveBoard = m_OriginalHeadInfo.transform.FindChild("TitleInvestitiveBoard").gameObject;
                //if (null != m_DamageBoard)
                //{
                //    m_DamageBoard.gameObject.SetActive(false);
                //}
                m_VipInfo = m_OriginalHeadInfo.transform.FindChild("VipSprite").GetComponent<UISprite>();
                m_ChatBubble = m_HeadInfoBoard.transform.FindChild("NameBoardOffset").FindChild("ChatBubble").gameObject;
                m_ChatBubble.SetActive(false);
                */
                ShowNameBoard();
                ShowPlayerTitleInvestitive();
                UpdateVipInfo();
            }
        }
        public override void UpdateVipInfo()
        {
            base.UpdateVipInfo();
            OnVipCostChange();
        }

        public void ShowPlayerTitleInvestitive()
        {
            if (null != m_playerHeadInfo)
            {
                m_playerHeadInfo.ShowTitleInvestitive(GameManager.gameManager.PlayerDataPool.TitleInvestitive.GetCurrentTitle());
            }
        }

        public void UpadatePlayerGBState()
        {
            if (null != m_playerHeadInfo)
            {
                m_playerHeadInfo.UpdateGuildBusinessIcon(GuildBusinessState);
            }
        }

        //玩家登陆接口
        public void OnPlayerLogin()
        {
        }
        
		//切换场景调用接口
        public void OnPlayerEnterScene()
		{
            //设置初始位置
            //这个之后会改为读取PlayerDataPool中的位置
            if (GameManager.gameManager.OnLineState)
            {
                Position = ActiveScene.GetTerrainPosition(GameManager.gameManager.PlayerDataPool.EnterSceneCache.EnterScenePos);
            }
            else
            {
                Position = DefaultPosition();
            }
            //做个藏经阁特例,用于做轻功. add by Yx
//            if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_WUSHENTA)
//            {
//                Vector3 pos = Position;
//                pos.y = 6;
//                Position = pos;
//            }
			//add pinch

            if(gameObject.GetComponent<PinchRecognizer>() == null)
            {
            gameObject.AddComponent<PinchRecognizer>();
            }
           
            //添加主角摇杆控制组件
            if (m_Thirdcontroller == null)
            {
                m_Thirdcontroller = gameObject.AddComponent<ThirdPersonController>();
            }
            //m_Thirdcontroller = gameObject.GetComponent<ThirdPersonController>();

            //添加主角摄像机控制组件
            if (m_CameraController == null)
            {
                m_CameraController = gameObject.AddComponent<CameraController>();
            }
            //m_CameraController = gameObject.GetComponent<CameraController>();

            //if (gameObject.GetComponent<AudioListener>() == null)
            //{
            gameObject.AddComponent<AudioListener>();
            //}
            //添加主角UI组件
            //if (gameObject.GetComponent<UIPanel>() == null)
            //{
            //    gameObject.AddComponent<UIPanel>();
            //}

            //添加主角AI组件
            if (Controller == null)
            {
                Controller = gameObject.AddComponent<AIController>();
            }
            //Controller = gameObject.GetComponent<AIController>();

            if (gameObject.GetComponent<AI_PlayerCombat>() == null)
            {
                gameObject.AddComponent<AI_PlayerCombat>();
            }
			
			if (gameObject.GetComponent<AI_JuQing>() == null)
			{
				gameObject.AddComponent<AI_JuQing>();
			}

            //初始化寻路代理
            InitNavAgent();

            if (AnimLogic == null)
            {
                AnimLogic = gameObject.AddComponent<AnimationLogic>();
            }
            //AnimLogic = gameObject.GetComponent<AnimationLogic>();

            if (ObjEffectLogic == null)
            {
                ObjEffectLogic = gameObject.AddComponent<EffectLogic>();
            }
            //ObjEffectLogic = gameObject.GetComponent<EffectLogic>();
            InitEffect();

            Controller.SwitchCurrentAI(Controller.NormalAI);

            //初始化RoleBaseID
            BaseAttr.RoleBaseID = GameManager.gameManager.PlayerDataPool.EnterSceneCache.EnterSceneRoleBaseID;

            //初始化ServerID
            ServerID = GameManager.gameManager.PlayerDataPool.EnterSceneCache.EnterSceneServerID;
            
           

            ReliveEntryTime = GameManager.gameManager.PlayerDataPool.ReliveEntryTime;
			//=======当场景id为剧情江夏时，添加指路箭头
			int sceneId = GameManager.gameManager.PlayerDataPool.EnterSceneCache.EnterSceneSceneID;
			if(sceneId == 14)
			{
				if (GuideArrow.instance != null) 
				{
					GuideArrow.instance.Show(ObjManager.Instance.MainPlayer.gameObject,Vector3.zero,new GameObject());
				}
			}
			//========end


		}
        public override void UpdateAttrBroadcastPackt(GC_BROADCAST_ATTR packet)
        {
            base.UpdateAttrBroadcastPackt(packet);
			if (packet.HasModelVisualID || packet.HasWeaponDataID || packet.HasWeaponEffectGem)
            {
                if (RoleViewLogic.Instance() != null)
                {
                    RoleViewLogic.Instance().UpdateFashionView();
                }
                if (BackPackLogic.Instance() != null)
                {
                    BackPackLogic.Instance().UpdatePlayerEquipView();
                }
                if (FashionLogic.Instance() != null && FashionLogic.Instance().gameObject.activeInHierarchy)
                {
                    FashionLogic.Instance().HandleUpdateAttr();
                }
            }
            if (packet.HasModelVisualID)
            {
                for (int i = 0; i < LoginData.loginRoleList.Count; i++)
                {
                    if (LoginData.loginRoleList[i].guid == GUID)
                    {
                        LoginData.loginRoleList[i].ModelVisualID = packet.ModelVisualID;
                        break;
                    }
                }
//                if (ModelVisualID != GlobeVar.INVALID_ID)
//                {
//                    PlayEffect(225);
//                }Z
            }
            if (packet.HasWeaponDataID)
            {
                for (int i = 0; i < LoginData.loginRoleList.Count; i++)
                {
                    if (LoginData.loginRoleList[i].guid == GUID)
                    {
                        LoginData.loginRoleList[i].WeaponID = packet.WeaponDataID;
                        break;
                    }
                }
            }
            if (packet.HasWeaponEffectGem)
            {
                for (int i = 0; i < LoginData.loginRoleList.Count; i++)
                {
                    if (LoginData.loginRoleList[i].guid == GUID)
                    {
                        LoginData.loginRoleList[i].WeaponEffectGem = packet.WeaponEffectGem;
                        break;
                    }
                }
            }
        }
        public void ChangeHeadPic()
        {
            Tab_RoleBaseAttr roleBaseTab = TableManager.GetRoleBaseAttrByID(BaseAttr.RoleBaseID, 0);
            Tab_CharModel charModel = TableManager.GetCharModelByID(ModelID, 0);
            if (null == charModel && null != roleBaseTab)
            {
                charModel = TableManager.GetCharModelByID(roleBaseTab.CharModelID, 0);
            }
            if (null != charModel)
            {
                BaseAttr.HeadPic = charModel.HeadPic;
                OptHeadPicChange();
            }

            DeltaHeight = charModel.HeadInfoHeight;
        }

        public void OnPlayerLeaveScene()
        {
        }

		//////////////////////////////////////////////////////////////////////////
		//摄像机相关
		//////////////////////////////////////////////////////////////////////////
        private CameraController m_CameraController = null;
        public CameraController CameraController
        {
            get { return m_CameraController; }
            set { m_CameraController = value; }
        }
		

		void UpdateCameraController()
		{
			if (m_CameraController)
			{
				m_CameraController.UpdateCamera();
			}
		}
		//////////////////////////////////////////////////////////////////////////
		/// 移动和动画相关
		//////////////////////////////////////////////////////////////////////////
		protected override void OnMoveOver()
		{
			//移动结束，如果发现存在目标NPC，则进行移动结束后的操作
			//目前暂定两种：
			//1.友方NPC开始对话操作
			//2.敌方NPC开始攻击操作
			if (null != MoveTarget)
			{
				Obj_NPC objNpc = MoveTarget.GetComponent<Obj_NPC>();
				if (objNpc)
				{
					if (Reputation.IsEnemy(objNpc) || Reputation.IsNeutral(objNpc))
					{
						//如果是地方NPC，则开始攻击
					    if (m_SkillCore !=null )
					    {
							//if(CurUseSkillId==-1)
							//CurUseSkillId=-1;
                            OnEnterCombat(MoveTarget.GetComponent<Obj_Character>());
							//EnterAutoCombat();
						}
					}
					else
					{
					  //如果是友方NPC，则开始对话

						Singleton<DialogCore>.GetInstance().Show(objNpc);
						Tab_RoleBaseAttr tab=TableManager.GetRoleBaseAttrByID(objNpc.BaseAttr.RoleBaseID,0);
						if((tab!=null)&&(tab.IsZA==1))
						{
							MoveTarget = null;
							return ;
						}
                        objNpc.FaceTo(m_ObjTransform.position);
					}
				//	UpdateTargetFrame(objNpc);
				}

                MoveTarget = null;
			}
			//处理移动后事件
			if (m_MoveOverEvent.EventID != GameDefine_Globe.EVENT_DEFINE.EVENT_INVALID)
			{
				Singleton<EventSystem>.GetInstance().PushEvent(m_MoveOverEvent);
			}	
	   
            //移动圈消失
            //if (null != GameManager.gameManager.ActiveScene)
            //{
            //    GameManager.gameManager.ActiveScene.DeactiveMovingCircle();
            //}
		}

	    
		//////////////////////////////////////////////////////////////////////////
		//目标选择逻辑
		//////////////////////////////////////////////////////////////////////////
		private Obj_Character m_selectTarget = null;      //选择的目标
        public Obj_Character SelectTarget
        {
            get { return m_selectTarget; }
            set { m_selectTarget = value; }
        }
		private bool m_onSelectForClick = false;//标记从点击选择的目标
		public bool OnSelectForClick
		{
			get { return m_onSelectForClick; }
			set { m_onSelectForClick = value; }
		}

		public void OnSelectTargetForClick(GameObject targetObj, bool isMoveAgainSelect = true)
		{
			m_onSelectForClick = true;
			OnSelectTarget (targetObj,isMoveAgainSelect);
		}

        public void OnSelectTarget(GameObject targetObj, bool isMoveAgainSelect = true)
		{
//
			//如果targetObj为空，则进行取消选择逻辑
			//如果之前已经选择，则移动过去
			if (null != m_selectTarget && m_selectTarget.gameObject == targetObj)
			{
				if (isMoveAgainSelect)
				{
				
					Tab_RoleBaseAttr  role=TableManager.GetRoleBaseAttrByID(
						m_selectTarget.BaseAttr.RoleBaseID,0);
					//屏蔽移动逻辑修改为npc过去，怪不过去
					if(role!=null)
					{
				      if (  Reputation.GetObjReputionType(targetObj.GetComponent<Obj_Character>()) == CharacterDefine.REPUTATION_TYPE.REPUTATION_FRIEND)
					  {
							Singleton<ObjManager>.GetInstance().MainPlayer.MoveTo(targetObj.transform.position, targetObj, role.SelectRadius+1.0f);
					  }
					}
					//屏蔽移动逻辑修改为只选中不移动
//					if(role!=null)
//						Singleton<ObjManager>.GetInstance().MainPlayer.MoveTo(targetObj.transform.position, targetObj, role.SelectRadius+1.0f);
					//修改奔跑中有目标了突然转身的情况
					if(IsMoving==false)
					Singleton<ObjManager>.GetInstance().MainPlayer.FaceTo(targetObj.transform.position);
				}
				return;
			}
			//如果选择的目标在播放技能范围的特效 切换目标时得 修改特效播放的对象
			if (CurPressSkillId != -1)
			{
				Tab_SkillEx _skillEx = TableManager.GetSkillExByID(CurPressSkillId, 0);
				if (_skillEx != null)
				{
					if (_skillEx.RangeEffectType != -1 && _skillEx.RangeEffectTarType ==(int)SKILLRANGEEFFECTTAR.SELECTTARGET)
					{
						m_selectTarget.StopSkillRangeEffect();
					}
				}
			}
			//发包给服务器
			int SelObjId =-1;
			if (targetObj !=null)
			{
				Obj npcScript = targetObj.GetComponent<Obj>();
				if (npcScript)
				{
					SelObjId = npcScript.ServerID;
				}
			}
			CG_ASK_SELOBJ_INFO selPacket = (CG_ASK_SELOBJ_INFO)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_SELOBJ_INFO);
			selPacket.SetObjId(ServerID);
			selPacket.SetSeleobjId(SelObjId);
			selPacket.SendPacket();
		}

		public void UpdateSelectTarget()
		{
			//更新目标选取策略
            //距离为11
			if (null == m_selectTarget)
			{
				return;
			}

            float fMaxViewDis = 15.0f;
            //判断目标类型改变fMaxViewDis
            //如果是远程职业，距离为8，近战为4
            //if (Profession == (int)CharacterDefine.PROFESSION.XIAOYAO ||
            //    Profession == (int)CharacterDefine.PROFESSION.DALI)
            //{
            //    fMaxViewDis = 8.0f;
            //}
            //else
            //{
            //    fMaxViewDis = 4.0f;
            //}

            //再次根据目标选择，是否为其他玩家
            if (m_selectTarget.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER)
            {
                fMaxViewDis = 17.0f;
            }
           
			float distance = Vector3.Distance(m_selectTarget.gameObject.transform.position, m_ObjTransform.position);
			//按照之前的估计，一个屏幕的宽度大概为场景宽度的1/3~1/2，所以当玩家离NPC的距离为场景宽度的1/6的时候，进行取消选择逻辑
            if (distance > fMaxViewDis || IsDie())
			{
				OnSelectTarget(null);
				m_selectedId.Clear();
				return;
			}
		}

        //////////////////////////////////////////////////////////////////////////
        // 坐骑相关
        //////////////////////////////////////////////////////////////////////////
        public int MountID
        {
            get { return GameManager.gameManager.PlayerDataPool.m_objMountParam.MountID; }
            set { GameManager.gameManager.PlayerDataPool.m_objMountParam.MountID = value; }
        }

        public override void RideOrUnMount(int nMountID)
        {
            base.RideOrUnMount(nMountID);
            GameManager.gameManager.PlayerDataPool.m_objMountParam.MountID = nMountID;
        }
        // 是否装备坐骑
        public int GetEquipMountID()
        {
            return GameManager.gameManager.PlayerDataPool.m_objMountParam.MountID;
        }

        //////////////////////////////////////////////////////////////////////////
        // 伙伴相关
        //////////////////////////////////////////////////////////////////////////
        //当前召出伙伴服务器objid
        private int m_nCurFellowObjId = -1;
        public int CurFellowObjId
        {
            get { return m_nCurFellowObjId; }
            set { m_nCurFellowObjId = value; }
        }

        //当前召出伙伴
        public Obj_Fellow GetCurFellow()
        {
            if (m_nCurFellowObjId >= 0)
            {
                Obj_Character charobj = Singleton<ObjManager>.GetInstance().FindObjCharacterInScene(m_nCurFellowObjId);
                if (charobj != null && charobj.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_FELLOW)
                {
                    return charobj as Obj_Fellow;
                }
            }
            return null;
        }

        //private float m_fAutoHPDrugSecond = -16.0f;
        //private float m_fBuyHPDrugSecond = Time.realtimeSinceStartup;
        //private float m_fAutoMPDrugSecond = -16.0f;
        //private float m_fBuyMPDrugSecond = Time.realtimeSinceStartup;
	    public void AutoUseHPMPDrug()
	    {
            //设置挂机,自动吃药 tt198507不挂机也可以自动吃药
            if (/*GetAutoCombatState() &&*/ /*InCombat &&*/ AutoHpPercent * 0.99f * BaseAttr.MaxHP > BaseAttr.HP && IsDie() == false
                 && !((int)GameDefine_Globe.SCENE_DEFINE.SCENE_TIANXIAWUSHUANG == GameManager.gameManager.RunningScene
                    || (int)GameDefine_Globe.SCENE_DEFINE.SCENE_RICHANGJUEDOU == GameManager.gameManager.RunningScene
                    || (int)GameDefine_Globe.SCENE_DEFINE.SCENE_GUILDWAR == GameManager.gameManager.RunningScene
                    || (int)GameDefine_Globe.SCENE_DEFINE.SCENE_WORLDBOSS == GameManager.gameManager.RunningScene)
               )
            {
                if (GameManager.gameManager.PlayerDataPool.HpItemCDTime <= 0)    //不可以连续发包
                {                    
                    if (AutoPercent((int)MedicSubClass.HP) == false)
                    {
                        //没药了.]
                        //                         if (GetAutoCombatState() && Time.realtimeSinceStartup - m_fBuyHPDrugSecond > 5 && AutoBuyDrug == true)
                        //                         {
                        //                             m_fBuyHPDrugSecond = Time.realtimeSinceStartup;
                        //                             OnAutoBuyDrug(AutoHpID);
                        //                         }
                        UpdateSelectDrug(); //重新选择药
                        AutoPercent((int)MedicSubClass.HP);                    
                        
                    }
                  
                }
            }

            //设置挂机,自动吃药 tt198507不挂机也可以自动吃药
            if (/*GetAutoCombatState() &&*/ /*InCombat &&*/ AutoMpPercent * 0.99f * BaseAttr.MaxMP > BaseAttr.MP && IsDie() == false
                && !((int)GameDefine_Globe.SCENE_DEFINE.SCENE_TIANXIAWUSHUANG == GameManager.gameManager.RunningScene
                    || (int)GameDefine_Globe.SCENE_DEFINE.SCENE_RICHANGJUEDOU == GameManager.gameManager.RunningScene
                    || (int)GameDefine_Globe.SCENE_DEFINE.SCENE_GUILDWAR == GameManager.gameManager.RunningScene
                    || (int)GameDefine_Globe.SCENE_DEFINE.SCENE_WORLDBOSS == GameManager.gameManager.RunningScene)
               )
            {
                if (GameManager.gameManager.PlayerDataPool.MpItemCDTime <= 0)    //不可以连续发包
                {                   
                    if (AutoPercent((int)MedicSubClass.MP) == false)
                    {
                        //没药了.
                        //                         if ( GetAutoCombatState() && Time.realtimeSinceStartup - m_fBuyMPDrugSecond > 5 && AutoBuyDrug == true )
                        //                         {
                        //                             m_fBuyMPDrugSecond = Time.realtimeSinceStartup;
                        //                             OnAutoBuyDrug(AutoMpID);
                        //                         }
                       UpdateSelectDrug(); //重新选择药
                       AutoPercent((int)MedicSubClass.MP);
                    }
                }
            }
	    }
       
		public bool isUp = false;

        public override void OptHPChange() //血量变化后的操作
        {
            //更新血条
            if (PlayerFrameLogic.Instance() != null)
            {
				if(isUp)
				{
					PlayEffect(51);
					isUp = false;
				}

                PlayerFrameLogic.Instance().ChangeHP(BaseAttr.HP, BaseAttr.MaxHP);
            }
        }
       
        public override void OptMPChange()//法力变化后的操作
        {
            if (PlayerFrameLogic.Instance() != null)
            {
                PlayerFrameLogic.Instance().ChangeMP(BaseAttr.MP, BaseAttr.MaxMP);
            }
        }
        public override void OptXPChange()//XP变化后的操作
        {
			//添加地图是否允许使用xp技能判断
			Tab_SceneClass _sceneClassInfo = TableManager.GetSceneClassByID(GameManager.gameManager.RunningScene, 0);
			if (SkillBarLogic.Instance() != null)
             {
				if(_sceneClassInfo.IsCanUseXp ==1)
                	SkillBarLogic.Instance().ChangeXPEnergy(BaseAttr.XP, BaseAttr.MaxXP);
				else
					SkillBarLogic.Instance().ChangeXPEnergy(0,1);
            }
           if (PlayerFrameLogic.Instance() != null)
           {
                PlayerFrameLogic.Instance().ChangeXPEnergy(BaseAttr.XP, BaseAttr.MaxXP);
            }
            
        }

        public void OnVipCostChange()
        {
            if (PlayerFrameLogic.Instance() != null)
            {
                PlayerFrameLogic.Instance().OnVipCostChange(VipCost);
            }
            if (RechargeBarLogic.Instance() != null)
            {
                RechargeBarLogic.Instance().OnVipCostChange(VipCost);
            }
            if (DungeonWindow.Instance() != null)
            {
                DungeonWindow.Instance().UpdateTabInfo();
            }
//             if (VipData.GetVipLv() == GlobeVar.USE_AUTOFIGHT_VIPLEVEL)
//             {
//                 //vip2的时候选择自动强化
//                 UpdateSelectEquip();
//             }
        }

        private int m_lastLevel = -1;
        public override void OptLevelChange()//等级变化后的操作
        {
            if (m_lastLevel > 0)
            {
                if (m_lastLevel != BaseAttr.Level)
                {
                    PlayEffect(52);
                    LevelUpController.ShowTipByID(1433);
                    GameManager.gameManager.SoundManager.PlaySoundEffect(106);  //upgrading
                    m_lastLevel = BaseAttr.Level;

                    //如果背包界面开着 刷新下背包物品显示（根据是否满足级别会有标红）
                    if (BackPackLogic.Instance())
                    {
                        BackPackLogic.Instance().UpdateBackPack();
                    }
                    LevelUpButtonActive();
                    if (m_lastLevel >= GlobeVar.MAX_AUTOEQUIT_LIVE && VipData.GetVipLv() >= GlobeVar.USE_AUTOFIGHT_VIPLEVEL)
                    {
                        //vip2的时候选择自动强化
                        UpdateSelectEquip();
                    }
                }
            }
            else
            {
                m_lastLevel = BaseAttr.Level;
                InitLevelButtonActive();
            }

            if (null != PlayerFrameLogic.Instance())
            {
                PlayerFrameLogic.Instance().ChangeLev(BaseAttr.Level);
            }
            if (ExpLogic.Instance() != null)
            {
                ExpLogic.Instance().OnLevelChange();
            }
            if (FunctionButtonLogic.Instance())
            {
                FunctionButtonLogic.Instance().UpdateActionButtonTip();
                FunctionButtonLogic.Instance().UpdateDaliyLuckNum();
                FunctionButtonLogic.Instance().UpdateButtonAwardTips();
            }
            if (MoneyTreeButtonLogic.Instance())
            {
                MoneyTreeButtonLogic.Instance().UpdateTimerText();
            }

            // 升级的时候给SDK同步一下当前角色信息
            LoginData.PlayerRoleData curData = LoginData.GetPlayerRoleData(PlayerPreferenceData.LastRoleGUID);
            if (null != curData)
            {
                PlatformHelper.UpdateRoleInfo(curData.level);
            }
           
        }

        public override void OptHeadPicChange()//头像变化后的操作
        {
            if (null != PlayerFrameLogic.Instance())
            {
                PlayerFrameLogic.Instance().ChangeHeadPic(BaseAttr.HeadPic);
            }
            
        }

        public override void OptNameChange()//名字变化后的操作
        {
            
            if (null != PlayerFrameLogic.Instance())
            {
                PlayerFrameLogic.Instance().ChangeName(BaseAttr.RoleName);
            }

            ShowNameBoard();

            //更新登陆界面信息
            for (int i = 0; i < LoginData.loginRoleList.Count; i++)
            {
                if (LoginData.loginRoleList[i].guid == GUID)
                {
                    LoginData.loginRoleList[i].name = BaseAttr.RoleName;
                    UserConfigData.AddRoleInfo();
                    break;
                }
            }
        }

        public override void OnExpChange()
        {
            if (ExpLogic.Instance() != null)
            {
                ExpLogic.Instance().UpdateExp();
            }
        }

        public void OnOffLineExpChange()
        {
            if (ExpLogic.Instance() != null)
            {
                ExpLogic.Instance().UpdateOffLineExp();
            }
        }

        public override void OptForceChange()//势力变化后的操作
        {
            base.OptForceChange();
            //重置周围玩家名字颜色
            Dictionary<string, Obj> targets = Singleton<ObjManager>.GetInstance().ObjPools;
            foreach (Obj targetObj in targets.Values)
            {
                if (targetObj != null && targetObj.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER)
                {
                    Obj_OtherPlayer _Player = targetObj as Obj_OtherPlayer;
                    if (_Player)
                    {
                        _Player.SetNameBoardColor();
                    }
                }
            }
        }

        public void AskCombatValue(bool bPowerRemind)
        {
            CG_COMBATVALUE_ASK packet = (CG_COMBATVALUE_ASK)PacketDistributed.CreatePacket(MessageID.PACKET_CG_COMBATVALUE_ASK);
            packet.ShowPowerRemind = bPowerRemind ? 1 : 0;
            packet.SendPacket();
        }

        //死亡相关
        private int m_nReliveEntryTime = 0;//记录复活剩余秒
        public int ReliveEntryTime
        {
            get { return m_nReliveEntryTime; }
            set { m_nReliveEntryTime = value; }
        }
        public override bool OnCorpse()
        {
            base.OnCorpse();
            //BaseAttr.Die = true;
            // 死亡 弹出复活UI
            if ((int)GameDefine_Globe.SCENE_DEFINE.SCENE_RICHANGJUEDOU == GameManager.gameManager.RunningScene 
			    || (int)GameDefine_Globe.SCENE_DEFINE.SCENE_TIANXIAWUSHUANG == GameManager.gameManager.RunningScene
			    || (int)GameDefine_Globe.SCENE_DEFINE.SCENE_WUSHENTA == GameManager.gameManager.RunningScene )
            {
                return true;
            }
            UIManager.ShowUI(UIInfo.Relive);
			//UIManager.CloseUI(UIInfo.MissionDialogAndLeftTabsRoot);
            //切换到死亡状态
            //CurObjAnimState = GameDefine_Globe.OBJ_ANIMSTATE.STATE_DEATH;
            return true;
        }


		//Obj死亡时候调用
		
		public override bool OnDie()
		{
			if (IsDie())
			{
				return false;
			}
			base.OnDie();
			//BaseAttr.Die = true;
			// 死亡 弹出复活UI
			
			// 玩家死亡停止自动寻路
			if (null != GameManager.gameManager.AutoSearch)
			{
				GameManager.gameManager.AutoSearch.Stop();
			}
			
			if (null != NavAgent)
			{
				Destroy(NavAgent);
			}

			if ((int)GameDefine_Globe.SCENE_DEFINE.SCENE_RICHANGJUEDOU == GameManager.gameManager.RunningScene
			    || (int)GameDefine_Globe.SCENE_DEFINE.SCENE_TIANXIAWUSHUANG == GameManager.gameManager.RunningScene
			    || (int)GameDefine_Globe.SCENE_DEFINE.SCENE_WUSHENTA == GameManager.gameManager.RunningScene)
            {
                return true;
            }

			UIManager.ShowUI(UIInfo.Relive);
			//UIManager.CloseUI(UIInfo.MissionDialogAndLeftTabsRoot);
			JoyStickLogic.Instance().ReleaseJoyStick();
			ProcessInput.Instance().SceneTouchFingerID = -1;
			LeveAutoCombat ();
			if(SGAutoFightBtn.Instance!=null)
			{
				SGAutoFightBtn.Instance.UpdateAutoFightBtnState();
			}
			//JoyStickLogic.Instance().CloseWindow();
			UpdateAutoAnteMortem();
			return true;
		}
		public override bool OnRelife()
		{
			base.OnRelife();
			GameManager.gameManager.PlayerDataPool.Usingskill = 0;
			//BaseAttr.Die = false;
			// 复活 关闭复活UI
			UIManager.CloseUI(UIInfo.Relive);
			//UIManager.ShowUI(UIInfo.MissionDialogAndLeftTabsRoot);
			//JoyStickLogic.Instance().OpenWindow();
			//CurObjAnimState = GameDefine_Globe.OBJ_ANIMSTATE.STATE_NORMOR;

			UpdateAutoAnteMortem();
			return true;
		}
		public override void OptChangPKModle()
		{
			base.OptChangPKModle();
			//重置周围玩家名字颜色
			Dictionary<string, Obj> targets = Singleton<ObjManager>.GetInstance().ObjPools;
			foreach (Obj targetObj in targets.Values)
			{
				if (targetObj != null && targetObj.ObjType ==GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER)
				{
                    Obj_OtherPlayer _Player = targetObj as Obj_OtherPlayer;
                    if (_Player)
                    {
                        _Player.SetNameBoardColor();
                    }
                }
            }
            //显示不同状态的按钮
            if (FunctionButtonLogic.Instance() !=null)
            {
                FunctionButtonLogic.Instance().m_PKKillBt.SetActive(false);
                FunctionButtonLogic.Instance().m_PKNormalBt.SetActive(false);
                if (PkModle ==(int)CharacterDefine.PKMODLE.NORMAL)
                {
                    FunctionButtonLogic.Instance().m_PKNormalBt.SetActive(true);
                }
                else if (PkModle == (int)CharacterDefine.PKMODLE.KILL)
                {
                    FunctionButtonLogic.Instance().m_PKKillBt.SetActive(true);
                }
            }
        }

        public override Color GetNameBoardColor()
        {
            string strColor = "FFFFFF";

            if (IsCanPKLegal)
            {
                strColor = "003300";
            }
            else
            {
                strColor = "06FBED";
            }

            return GCGame.Utils.GetColorByString(strColor);
        }

        private bool m_bIsInModelStory = false;
        public bool IsInModelStory
        {
            get { return m_bIsInModelStory; }
            set { m_bIsInModelStory = value; }
        }

        private bool m_bIsNoMove = false;
        public bool IsNoMove
        {
            get { return m_bIsNoMove; }
            set { m_bIsNoMove = value; }
        }

        public void SendNoticMsg(bool IsFilterRepeat, string strMsg, params object[] args)
	    {
            GUIData.AddNotifyData2Client(IsFilterRepeat,strMsg, args);
	    }

        //玩家是否接受外部移动指令
        public bool IsCanOperate_Move()
        {
            if (QingGongState)
            {
                return false;
            }
            if (IsDie())
            {
                return false;
            }
            if (CurObjAnimState ==GameDefine_Globe.OBJ_ANIMSTATE.STATE_ATTACKDOWN)
            {
                return false;
            }

            if (AcceleratedMotion != null && AcceleratedMotion.Going == true)
            {
                return false;
            }

            if (m_SkillCore != null &&
                m_SkillCore.IsUsingSkill &&
                m_SkillCore.UsingSkillBaseInfo != null &&
                m_SkillCore.UsingSkillExInfo != null)
            {

                //使用的技能中 不能移动释放且不能被移动打断 则不让移动
                if (m_SkillCore.UsingSkillBaseInfo.IsMove == 0 &&
                    m_SkillCore.UsingSkillBaseInfo.IsMoveBreak == 0)
                {
                    return false;
                }

                if ((m_SkillCore.UsingSkillBaseInfo.SkillClass & (int) SKILLCLASS.AUTOREPEAT) != 0)
                {
                    float _ElapseTime = Time.time - m_fLastClickAttackBtTime;
                    if (_ElapseTime < 1.5f)
                    {
                        return false;
                    }
                }

            }
            //有绑定父节点
            if (BindParent > 0)
            {
                return false;
            }
            if (m_bIsInModelStory)
            {
                return false;
            }
            if (m_bIsNoMove)
            {
                return false;
            }
            if (IsHaveNoMoveBuff()) //有禁止移动的BUFF
            {
                return false;
            }
            return true;
        }

        //玩家轻功部分处理
        public override void BeginQingGong(GameEvent _event)
        {
            ReqHideFellow();
            base.BeginQingGong(_event);
            ProcessQingGongStart();
        }

        public override void EndQingGong()
        {
            base.EndQingGong();
            ProcessQingGongOver();
            ReqShowFellow();
        }

        //玩家轻功开始之后，强制更新一下轻功点给服务器
        public void SycQingGongPos(Vector3 pos)
        {
            //新Obj同步机制修改，改用CG_MOVE包
            CG_MOVE movPacket = (CG_MOVE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_MOVE);
            movPacket.Poscount = 1;
            movPacket.AddPosx((int)(pos.x * 100));
            movPacket.AddPosz((int)(pos.z * 100));
            if (IsMoving)
            {
                movPacket.Ismoving = 1;
            }
            else
            {
                movPacket.Ismoving = 0;
            }
            movPacket.SendPacket();
        }

        private int m_ModelStoryID = GlobeVar.INVALID_ID;
        public int ModelStoryID
        {
            get { return m_ModelStoryID; }
            set { m_ModelStoryID = value; }
        }
        void ProcessQingGongStart()
        {
            if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_WUSHENTA )
            {
                IsInModelStory = true;
            }
        }
        void ProcessQingGongOver()
        {
            if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_YANMENGUANWAI &&
                QingGongPointID == GlobeVar.YanmMenGuan_QingGongID_Over)
            {
                if (m_ModelStoryID == GlobeVar.YanMenGuan_ModelStoryID)
                {
                    StoryDialogLogic.ShowStory(GlobeVar.YanMenGuan_BossStory1ID);
                }
            }
            if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_WUSHENTA)
            {
                IsInModelStory = false;
            }
        }

        //切磋
        public UInt64 DuelTargetGuid { set; get; }
        //
        public void ReqDuel(UInt64 targetGuid)
        {
            //向服务器发送邀请某人加入队伍消息
            CG_DUEL_REQUEST msg = (CG_DUEL_REQUEST)PacketDistributed.CreatePacket(MessageID.PACKET_CG_DUEL_REQUEST);
            msg.Guid = targetGuid;
            msg.SendPacket();
        }

        public void DuelWithMe(UInt64 targetGuid, string name)
        {
            DuelTargetGuid = targetGuid;
            string text = StrDictionary.GetClientDictionaryString("{#1666}", name);
            string title = Utils.GetDicByID(1657);
            MessageBoxLogic.OpenOKCancelBox(text, title, AgreeDuelWithOther, RefuseDuelWithOther);
        }

        public void AgreeDuelWithOther() { DecideDuelWithOrNot(1);  }
        public void RefuseDuelWithOther() { DecideDuelWithOrNot(0); }

        public void DecideDuelWithOrNot(int agree)
        {
            CG_DUEL_RESPONSE msg = (CG_DUEL_RESPONSE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_DUEL_RESPONSE);
            msg.Guid = DuelTargetGuid;
            msg.Agree = agree;
            msg.SendPacket();  
        }

        //----
        private int m_SpcialAnimationID = -1;
        private Obj_Client m_SpcicalClient1;
        private Obj_Client m_SpcicalClient2;

        public void OnStartPlayStory(int storyID)
        {
            switch (storyID)
            {
                case 16:
                    Singleton<ObjManager>.Instance.CreateModelStoryObj(170, "JXZFastMove1", ProcessCharFastMoveToMainPlayerAsycLoadOver);
                    break;
                case 17:
                    Singleton<ObjManager>.Instance.CreateModelStoryObj(171, "JXZFastMove1", ProcessCharFastMoveToMainPlayerAsycLoadOver);
                    Singleton<ObjManager>.Instance.CreateModelStoryObj(172, "JXZFastMove1", ProcessCharFastMoveToMainPlayerAsycLoadOver);
                    break;
                case 18: // 45 ,45
                    Singleton<ObjManager>.Instance.CreateModelStoryObj(173, "JXZFastMove1", ProcessCharFastMoveToMainPlayerAsycLoadOver);
                    break;
                case 19: // 36
                    m_SpcialAnimationID = 2;
                    Singleton<ObjManager>.Instance.CreateModelStoryObj(174, "JXZPlaySPAni1", ProcessCharFastMoveToMainPlayerAsycLoadOver);
                    break;
                case 20: // 37
                    m_SpcialAnimationID = 2;
                    Singleton<ObjManager>.Instance.CreateModelStoryObj(175, "JXZPlaySPAni2", ProcessCharFastMoveToMainPlayerAsycLoadOver);
                    break;
            }
        }

        public void RemoveAllSpicalClient()
        {
              RemoveSpicalClient(1);
              RemoveSpicalClient(2);
        }

        public void OnPlayStoryOver(int storyID)
        {
            switch (storyID)
            {
                case 16:
                    RemoveSpicalClient(1);
                    break;
                case 17: // 45 ,45
                    RemoveSpicalClient(1);
                    RemoveSpicalClient(2);
                    break;
                case 18: // 36
                    RemoveSpicalClient(1);
                    break;
                case 19: // 37
                    RemoveSpicalClient(1);
                    break;
                case 20: // 46
                    RemoveSpicalClient(1);
                    break;
                case GlobeVar.YanMenGuan_BeforeQingGongStoryID:
                    m_bIsInModelStory = true;
                    break;
                case GlobeVar.YanMenGuan_BossStory1ID:
                    if (!m_bIsInModelStory)
                    {
                        m_bIsInModelStory = true;
                    }
                    Singleton<ObjManager>.Instance.CreateModelStoryObj(363, "play-effect", ProcessQingGongCharAsycLoadOver);
                    Singleton<ObjManager>.Instance.CreateModelStoryObj(362, "non-effect", ProcessQingGongBossAsycLoadOver);
                    break;

            }
            if (StoryDialogLogic.Instance() != null)
            {
                if (StoryDialogLogic.IsNeedSilentMode(storyID))
                {
                    m_bIsInModelStory = false;
                }
            }
			CameraController  cam=ObjManager.Instance.MainPlayer.CameraController;
			if(cam!=null)
			{
				cam.EndStory();
			}
			
		}
		
		private void RemoveSpicalClient(int idx)
        {
            if (idx == 1 && null != m_SpcicalClient1)
            {
                Singleton<ObjManager>.Instance.ReomoveObjInScene(m_SpcicalClient1.gameObject);
                m_SpcicalClient1 = null;
            }
            else if (idx == 2 && null != m_SpcicalClient2)
            {
                Singleton<ObjManager>.Instance.ReomoveObjInScene(m_SpcicalClient2.gameObject);
                m_SpcicalClient2 = null;
            }
        }

        public void ProcessQingGongCharAsycLoadOver(object param1, object param2)
        {
            if (null == param1)
            {
                return;
            }

            Obj_Client curCharacter = (Obj_Client)param1;
            if (null == curCharacter)
                return;

            Transform transShadow = curCharacter.gameObject.transform.FindChild("Shadow");
            if (transShadow != null)
            {
                transShadow.gameObject.SetActive(false);
            }

            Vector3 charPosition = new Vector3(9.1f, 0, 55.6f);
            curCharacter.Position = ActiveScene.GetTerrainPosition(charPosition);
            curCharacter.Rotation = Quaternion.Euler(new Vector3(0, 90, 0));

            if (null != curCharacter.AnimLogic)
                curCharacter.AnimLogic.Play(GlobeVar.YanMenGuan_QiaoFengAni);

            if (null != GameManager.gameManager.SoundManager)
                GameManager.gameManager.SoundManager.PlaySoundEffect(107);   //ymgw_hyr
        }

        public void ProcessQingGongBossAsycLoadOver(object param1, object param2)
        {
            if (null == param1)
            {
                return;
            }

            Obj_Client curBoss = (Obj_Client)param1;
            if (null == curBoss)
                return;

            Transform transShadow = curBoss.gameObject.transform.FindChild("Shadow");
            if (transShadow != null)
            {
                transShadow.gameObject.SetActive(false);
            }

            Vector3 bossPosition = new Vector3(9.2f, 0, 55.5f);
            curBoss.Position = ActiveScene.GetTerrainPosition(bossPosition);
            curBoss.Rotation = Quaternion.Euler(new Vector3(0, 90, 0));

            if (null != curBoss.AnimLogic)
                curBoss.AnimLogic.Play(GlobeVar.YanMenGuan_BossAni);
        }

        public void ProcessCharFastMoveToMainPlayerAsycLoadOver(object param1, object param2)
        {
            if (null == param1)
            {
                return;
            }
            if (null == m_SpcicalClient1)
            {
                m_SpcicalClient1 = (Obj_Client)param1;

                if (null != m_SpcicalClient1)
                    FastMoveToMainPlayer(m_SpcicalClient1, 8.0f);
            }
            else if (null == m_SpcicalClient2)
            {
                m_SpcicalClient2 = (Obj_Client)param1;

                if (null != m_SpcicalClient2)
                    FastMoveToMainPlayer(m_SpcicalClient2, -8.0f);
            }
            
        }
        public void FastMoveToMainPlayer(Obj_Client oclient, float diffZ)
        {
            Vector3 pos = new Vector3(Position.x, Position.y, Position.z + diffZ);
            float fastMoveSpeed = Vector3.Distance(pos, Position) / 0.1f;
            oclient.Position = pos;
            oclient.FaceTo(Position);
            oclient.BaseAttr.MoveSpeed = fastMoveSpeed;
            oclient.MoveTo(Position);
        }

        public void ProcessCharPlayAnimationAsycLoadOver(object param1, object param2)
        {
            if (null == param1)
            {
                return;
            }
            if (null == m_SpcicalClient1)
            {
                m_SpcicalClient1 = (Obj_Client)param1;
                m_SpcicalClient1.Position = Position;
                if (m_SpcialAnimationID >= 0 )
                {
                    m_SpcicalClient1.PlayEffect(m_SpcialAnimationID);
                    m_SpcialAnimationID = -1;
                }
            }
        }

        public int CurFashionID
        {
            get { return GameManager.gameManager.PlayerDataPool.CurFashionID; }
            set { GameManager.gameManager.PlayerDataPool.CurFashionID = value; }
        }

        public override int ModelVisualID
        {
            get { return GameManager.gameManager.PlayerDataPool.ModelVisualID; }
            set { GameManager.gameManager.PlayerDataPool.ModelVisualID = value; }
        }

        public void InitYanMenGuanWaiVisual()
        {
            ModelVisualID = 18;
            if (Profession == (int)CharacterDefine.PROFESSION.SHAOLIN)
            {
                CurWeaponDataID = 52190;
                WeaponEffectGem = 8309;
            }
            else if (Profession == (int)CharacterDefine.PROFESSION.TIANSHAN)
            {
                CurWeaponDataID = 52188;
                WeaponEffectGem = 8909;
            }
            else if (Profession == (int)CharacterDefine.PROFESSION.DALI)
            {
                CurWeaponDataID = 52191;
                WeaponEffectGem = 8709;
            }
            else if (Profession == (int)CharacterDefine.PROFESSION.XIAOYAO)
            {
                CurWeaponDataID = 52189;
                WeaponEffectGem = 8409;
            }
        }

        private int m_nCopySceneId = -1;
        private int m_nCopySceneSingle = -1;
        private int m_nCopySceneDifficult = -1;
        public void SendOpenScene(int nSceneId, int nSingle, int nDifficult)
        {
            m_nCopySceneId = nSceneId;
            m_nCopySceneSingle = nSingle;
            m_nCopySceneDifficult = nDifficult;
            string dicStr = "";
            if (GameManager.gameManager.ActiveScene.IsCopyScene())
            {
                dicStr = StrDictionary.GetClientDictionaryString("#{1853}");
            }
            else
            {   
                dicStr = StrDictionary.GetClientDictionaryString("#{1854}");
            }
             MessageBoxLogic.OpenOKCancelBox(dicStr, "", OnOpenCopySceneOK, OnOpenCopySceneNO);
        }
        public void OnOpenCopySceneOK()
        {
            // 组队副本 又有队伍 还不是队长
            if (m_nCopySceneSingle == 2 && 
                GameManager.gameManager.PlayerDataPool.IsHaveTeam() && 
                !IsTeamLeader())
            {
                SendNoticMsg(false,"#{1530}");
                return;
            }
			GameManager.copyscenedifficult = m_nCopySceneDifficult;
            CG_OPEN_COPYSCENE packet = (CG_OPEN_COPYSCENE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_OPEN_COPYSCENE);
            packet.SceneID = m_nCopySceneId;
            packet.Type = m_nCopySceneSingle;
            packet.Difficult = m_nCopySceneDifficult;
			packet.EnterType = 1;

            packet.SendPacket();
        }
        public void OnOpenCopySceneNO()
        {

        }

        // 随玩家等级开放按钮
        void LevelUpButtonActive()
        {
            if (MenuBarLogic.Instance() != null)
            {
                MenuBarLogic.Instance().LevelUpButtonActive();
            }
            if (FunctionButtonLogic.Instance() != null)
            {
                FunctionButtonLogic.Instance().LevelUpButtonActive();
            }
        }

        void InitLevelButtonActive()
        {
            if (MenuBarLogic.Instance() != null)
            {
                MenuBarLogic.Instance().InitButtonActive();
            }
            if (FunctionButtonLogic.Instance() != null)
            {
                FunctionButtonLogic.Instance().InitButtonActive();
            }
        }

        // 体能恢复倒计时
        private float m_StaminaCoutDownTimer = GlobeVar.INVALID_ID;

        public int GetStaminaFull()
        {
            return (int)GlobeVar.MAX_STAMINA + BaseAttr.Level;
        }

        void StaminaTimerFunc()
        {
            if (m_StaminaCoutDownTimer != GlobeVar.INVALID_ID)
            {
                // 计时中
                if (GameManager.gameManager.PlayerDataPool.StaminaCountDown == GlobeVar.INVALID_ID)
                {
                    m_StaminaCoutDownTimer = GlobeVar.INVALID_ID;
                }
                if (Time.fixedTime - m_StaminaCoutDownTimer >= 1)
                {
                    if(GameManager.gameManager.PlayerDataPool.StaminaCountDown > 0)
                    {
                        GameManager.gameManager.PlayerDataPool.StaminaCountDown -= 1;

                        if (LivingSkillLogic.Instance() != null)
                        {
                            LivingSkillLogic.Instance().UpdateCountDownLabel();
                        }
                    }
                    m_StaminaCoutDownTimer = Time.fixedTime;
                }
            }
            else
            {
                // 未计时
                if (GameManager.gameManager.PlayerDataPool.StaminaCountDown != GlobeVar.INVALID_ID)
                {
                    m_StaminaCoutDownTimer = Time.fixedTime;
                }
            }
        }

        public void ReqViewOtherPlayer(UInt64 targetGuid, OtherRoleViewLogic.OPEN_TYPE oPenType)
        {
            //向服务器发送查看属性消息
            CG_ASK_OTHERROLE_DATA askPak = (CG_ASK_OTHERROLE_DATA)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_OTHERROLE_DATA);
            askPak.SetGuid(targetGuid);
            askPak.SendPacket();
            OtherRoleViewLogic.SetOpenType(oPenType);
        }

        public void ReqHideFellow()
        {
            //请求隐藏伙伴
            if (CurFellowObjId == -1)
            {
                return;
            }
            CG_ASK_HIDE_FELLOW askPak = (CG_ASK_HIDE_FELLOW)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_HIDE_FELLOW);
            askPak.SetType(1);
            askPak.SendPacket();
        }

        public void ReqShowFellow()
        {
            //请求显示伙伴
            CG_ASK_SHOW_FELLOW askPak = (CG_ASK_SHOW_FELLOW)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_SHOW_FELLOW);
            askPak.SetPosx((int)(this.Position.x*100));
            askPak.SetPosz((int)(this.Position.z*100));
            askPak.SetType(1);
            askPak.SendPacket();
        }

        public bool IsInJianYu()
        {
            return GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_JIANYU;
        }

        //跑商状态判断
        public bool IsInGuildBusiness()
        {
            return (GuildBusinessState == 1 || GuildBusinessState == 2);
        }

        public bool IsGBCanAccept()
        {
            Guild guildInfo = GameManager.gameManager.PlayerDataPool.GuildInfo;
            Tab_GuildBusiness tab = TableManager.GetGuildBusinessByID(guildInfo.GuildLevel, 0);
            if (tab != null)
            {
                int gbGotTime = GameManager.gameManager.PlayerDataPool.CommonData.GetCommonData((int)Games.UserCommonData.USER_COMMONDATA.CD_GUILDBUSINESS_GOTTEN_NUM);
                if ((guildInfo.GBCanAcceptTime <= 0))
                {
                    SendNoticMsg(false, "#{3935}");
                    return false;
                }
                if (gbGotTime >= tab.MemTimes)
                {
                    SendNoticMsg(false, "#{3922}");
                    return false;
                }
                if (BaseAttr.Level < 47)
                {
                    SendNoticMsg(false, "#{3919}");
                    return false;
                }
                return true;
            }
            return false;
        }

        public void UseItem(GameItem item)
        {
            if (false == item.IsMountItem())
            {
                CG_USE_ITEM useitem = (CG_USE_ITEM)PacketDistributed.CreatePacket(MessageID.PACKET_CG_USE_ITEM);
                useitem.SetItemguid(item.Guid);
                useitem.SendPacket();
                return;
            }
            UseMountItem(item);
        }

        void UseMountItem(GameItem item)
        {
            int nMountID = item.GetMountId();
            if (nMountID < 0 || nMountID >= GameManager.gameManager.PlayerDataPool.m_objMountParam.MountCollect.Length)
            {
                CG_USE_ITEM useitem = (CG_USE_ITEM)PacketDistributed.CreatePacket(MessageID.PACKET_CG_USE_ITEM);
                useitem.SetItemguid(item.Guid);
                useitem.SendPacket();
                return;
            }

            if (0 == GameManager.gameManager.PlayerDataPool.m_objMountParam.MountCollect[nMountID])
            {
                CG_USE_ITEM useitem = (CG_USE_ITEM)PacketDistributed.CreatePacket(MessageID.PACKET_CG_USE_ITEM);
                useitem.SetItemguid(item.Guid);
                useitem.SendPacket();
                return;
            }

            m_CurUseMountItemGuid = item.Guid;
            string strTip = "";
            if (GameManager.gameManager.PlayerDataPool.m_objMountParam.GetMoountLeftTime(nMountID) < 0 )
            {
                //已经拥有永久坐骑
                strTip = StrDictionary.GetClientDictionaryString("#{2969}", item.GetName());
            }
            else
            {
                if (item.IsTimeLimitItem())
                {
                    float fItemLeftDays = (float)item.GetLeftTime()/(float)86400;
                    int nLeftDays = Mathf.RoundToInt(fItemLeftDays);
                    strTip = StrDictionary.GetClientDictionaryString("#{2967}", item.GetName(), nLeftDays);
                }
                else
                {
                    strTip = StrDictionary.GetClientDictionaryString("#{2968}");
                }
            }
            string strTitle = StrDictionary.GetClientDictionaryString("#{1000}");
            MessageBoxLogic.OpenOKCancelBox(strTip, strTitle, OnUseMountItemOk, null);
        }
           
        void OnUseMountItemOk()
        {
            CG_USE_ITEM useitem = (CG_USE_ITEM)PacketDistributed.CreatePacket(MessageID.PACKET_CG_USE_ITEM);
            useitem.SetItemguid(m_CurUseMountItemGuid);
            useitem.SendPacket();
            m_CurUseMountItemGuid = GlobeVar.INVALID_GUID;
        }

        public bool CheckUseItem(GameItem item)
        {
            int canuse = TableManager.GetCommonItemByID(item.DataID, 0).CanUse;
            if (canuse == 1)
            {
                return true;
            }
            return false;
        }

        public void EquipItem(GameItem item)
        {
            CG_EQUIP_ITEM equipitem = (CG_EQUIP_ITEM)PacketDistributed.CreatePacket(MessageID.PACKET_CG_EQUIP_ITEM);
            equipitem.SetEquipguid(item.Guid);
            equipitem.SendPacket();

            if (EquipRemindLogic.Instance() != null && EquipRemindLogic.Instance().m_EquipRemind.activeInHierarchy)
            {
                if (EquipRemindLogic.Instance().GetCurEquipGuid() == item.Guid)
                {
                    EquipRemindLogic.Instance().CloseCurEquip();
                }
            }
        }

        public bool CheckEquipItem(GameItem item)
        {
            return true;
        }

        public void UnEquipItem(GameItem item)
        {
            //背包是否还有空间
            if (GameManager.gameManager.PlayerDataPool.BackPack.GetCanContainerSize() <= 0)
            {
                Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2102}");
                return;
            }

            CG_UNEQUIP_ITEM equipitem = (CG_UNEQUIP_ITEM)PacketDistributed.CreatePacket(MessageID.PACKET_CG_UNEQUIP_ITEM);
            equipitem.SetEquipguid(item.Guid);
            equipitem.SendPacket();

            if (BackPackLogic.Instance() != null)
            {
                BackPackLogic.Instance().TakeOffGuid = item.Guid;
            }

            if (RoleViewLogic.Instance() != null)
            {
                RoleViewLogic.Instance().TakeOffGuid = item.Guid;
            }
        }

        public bool CheckUnEquipItem(GameItem item)
        {
            return true;
        }

        public void ThrowItem(GameItem item)
        {
            CG_THROW_ITEM equipitem = (CG_THROW_ITEM)PacketDistributed.CreatePacket(MessageID.PACKET_CG_THROW_ITEM);
            equipitem.SetItemguid(item.Guid);
            equipitem.SendPacket();
        }

        public bool CheckThrowItem(GameItem item)
        {
            return true;
        }

        //宝石孔是否满足级别需求
        public bool CheckLevelForGemSlot(int slotindex)
        {
            Tab_GemOpenLimit line = TableManager.GetGemOpenLimitByID(slotindex + 1, 0);
            if (line != null)
            {
                if (line.OpenLevel > BaseAttr.Level)
                {
                    return false;
                }
            }
            return true;
        }

        //此装备位是否已有相同属性宝石
        public bool IsSameGemForEquipSlot(int gemId, int equipSlot)
        {
            Tab_GemAttr gemLine = TableManager.GetGemAttrByID(gemId, 0);
            if (gemLine == null)
            {
                return false;
            }

            for (int i = 0; i < (int)GemSlot.OPEN_NUM; i++)
            {
                int otherGemId = GameManager.gameManager.PlayerDataPool.GemData.GetGemId(equipSlot, i);
                if (otherGemId > 0)
                {
                    Tab_GemAttr otherGemLine = TableManager.GetGemAttrByID(otherGemId, 0);
                    if (otherGemLine != null)
                    {
                        if (gemLine.AttrClass == otherGemLine.AttrClass)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        public void InitCangJingGeInfo()
        {
            GameManager.gameManager.PlayerDataPool.StartSweep = false;
            GameManager.gameManager.PlayerDataPool.CangJIngGeTier = 0;
            GameManager.gameManager.PlayerDataPool.CangJIngGeSecond = Time.realtimeSinceStartup;
        }

        public void CangKuPutIn(GameItem item)
        {
            CG_PUT_ITEM_STORAGEPACK pak = (CG_PUT_ITEM_STORAGEPACK)PacketDistributed.CreatePacket(MessageID.PACKET_CG_PUT_ITEM_STORAGEPACK);
            pak.SetGuid(item.Guid);
            pak.SetPage(0);
            pak.SendPacket();
        }

        public void CangKuTakeOut(GameItem item)
        {
            CG_TAKE_ITEM_STORAGEPACK pak = (CG_TAKE_ITEM_STORAGEPACK)PacketDistributed.CreatePacket(MessageID.PACKET_CG_TAKE_ITEM_STORAGEPACK);
            pak.SetGuid(item.Guid);
            pak.SetPage(0);
            pak.SendPacket();
        }

        public int GetTotalEquipCombatValue()
        {
            int totalCombatValue = 0;
            GameItemContainer itempack = GameManager.gameManager.PlayerDataPool.EquipPack;
            for (int index = 0; index < itempack.ContainerSize; index++)
            {
                GameItem equip = itempack.GetItem(index);
                if (equip != null && equip.IsValid())
                {
                    totalCombatValue += equip.GetCombatValue();
                }
            }
            return totalCombatValue;
        }

        public int GetTotalGemCombatValue()
        {
            int totalCombatValue = 0;
            for (int i = 0; i < (int)EquipPackSlot.Slot_NUM; i++)
            {
                for (int index = 0; index < (int)GemSlot.OPEN_NUM; index++)
                {
                    int gemId = GameManager.gameManager.PlayerDataPool.GemData.GetGemId(i, index);
                    if (gemId > 0)
                    {
                        totalCombatValue += GemData.GetGemCombatValue(gemId);
                    }
                }
            }
            return totalCombatValue;
        }

        public int GetTotalFellowCombatValue()
        {
            int totalCombatValue = 0;
            FellowContainer fellowpack = GameManager.gameManager.PlayerDataPool.FellowContainer;
            for (int index = 0; index < fellowpack.ContainerSize; index++)
            {
                Fellow.Fellow fellow = fellowpack.GetFellowByIndex(index);
                if (fellow != null && fellow.IsValid())
                {
                    if (fellow.Called)
                    {
                        totalCombatValue += fellow.GetCombatValue();
                    }
                }
            }
            return totalCombatValue;
        }   

		public   void   updateWeaponPoint()//GameDefine_Globe.OBJ_ANIMSTATE ObjState
		{

			Obj_MainPlayer MainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
			if(MainPlayer.GetEquipMountID() > 0)
			{
				Tab_WeaponModel weaponModel =	reloadWeaponPath();
				if(weaponModel == null)
					return;
				changeWeaponPath(weaponModel.ResPath,3);
			}else if (m_Objanimation != null)
			{
				if (this.Profession == (int)CharacterDefine.PROFESSION.SHAOLIN) 
				{
 					if(AnimInfoNextAnimId == (int)CharacterDefine.CharacterAnimId.Stand)
					{
						Tab_WeaponModel weaponModel =	reloadWeaponPath();
						if(weaponModel == null)
							return;
						changeWeaponPath(weaponModel.ResPath,1);
					}else{
						Tab_WeaponModel weaponModel =	reloadWeaponPath();
						if(weaponModel == null)
							return;
						changeWeaponPath(weaponModel.ResPath,2);
					}

				}else if(this.Profession == (int)CharacterDefine.PROFESSION.XIAOYAO)
				{

					if(AnimInfoNextAnimId == (int)CharacterDefine.CharacterAnimId.Stand || AnimInfoNextAnimId == (int) CharacterDefine.CharacterAnimId.Walk)
					{
						Tab_WeaponModel weaponModel =	reloadWeaponPath();
						if(weaponModel == null)
							return;
						changeWeaponPath(weaponModel.ResPath,1);
					}else{
						Tab_WeaponModel weaponModel =	reloadWeaponPath();
						if(weaponModel == null)
							return;
						changeWeaponPath(weaponModel.ResPath,2);
					}
				}
			}
		}

		public Tab_WeaponModel reloadWeaponPath()
		{
			// 重载武器
			bool defaultVisual = false;
			Tab_ItemVisual WeaponVisual = null;
			Tab_EquipAttr tabEquipAttr = TableManager.GetEquipAttrByID(this.CurWeaponDataID, 0);
			if (tabEquipAttr != null)
			{
				Tab_ItemVisual tabWeaponVisual = TableManager.GetItemVisualByID(tabEquipAttr.ModelId, 0);
				if (tabWeaponVisual != null)
				{
					WeaponVisual = tabWeaponVisual;
				}
				else
				{
					defaultVisual = true;
				}
			}
			else
			{
				defaultVisual = true;
			}
			
			if (defaultVisual)
			{
				Tab_ItemVisual tabDefaultVisual = TableManager.GetItemVisualByID(this.CurWeaponDataID, 0);//GlobeVar.DEFAULT_VISUAL_ID
				if (tabDefaultVisual == null)
				{
					return null;
				}
				
				WeaponVisual = tabDefaultVisual;
			}
			
			if (WeaponVisual == null)
			{
				return null;
			}
			
			int nWeaponModelID = Singleton<ObjManager>.Instance.MainPlayer.GetWeaponModelID(WeaponVisual);
			
			Tab_WeaponModel tabWeaponModel = TableManager.GetWeaponModelByID(nWeaponModelID, 0);
			if (tabWeaponModel == null)
			{
				return null;
			}
			
			if (Singleton<ObjManager>.Instance.MainPlayer == null)
			{
				return null;
			}

			return tabWeaponModel;
		}

		public void changeWeaponPath(string weaponUrl,int tempIndex)
		{
			//GameObject  hbpoint=this.gameObject.transform.FindChild("Modle/Bip001/Bip001 Pelvis/HH_weaponback");
			string weaponPath = "";
			if(this.Profession == (int)CharacterDefine.PROFESSION.SHAOLIN || this.Profession == (int)CharacterDefine.PROFESSION.XIAOYAO)
			{
				weaponPath = "Model/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/HH_weaponback";
			}
			//else if(this.Profession == (int)CharacterDefine.PROFESSION.XIAOYAO)
			//{
			//	weaponPath = "Model/Bip001/Bip001 Pelvis/HH_weaponback";
			//}
			if(weaponPath == "")
				return;

			//string weaponPath = "Model/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/HH_weaponback";
			string weaponPath2 = "Model/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/HH_weaponHandRt";
			string weaponPath3 = "/" + weaponUrl + "(Clone)";

			string weaponBf = "";
			string weaponAf = "";

			string rideStr = "Model/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/ride/MountPlayer/";

			switch(tempIndex)
			{
			case 1:
				weaponBf = weaponPath2 +weaponPath3;
				weaponAf = weaponPath;
				break;
			case 2:
				weaponBf = weaponPath +weaponPath3;
				weaponAf = weaponPath2;
				break;
			case 3:
				weaponBf = rideStr + weaponPath2 +weaponPath3;
				weaponAf = rideStr +weaponPath;
				break;
			}

			if(weaponBf == "")
				return;
			if(weaponAf == "")
				return;

			GameObject  rtpoint;
			if( this.gameObject.transform.FindChild(weaponBf) != null)
			{
				rtpoint = this.gameObject.transform.FindChild(weaponBf).gameObject;

				GameObject  hbpoint = this.gameObject.transform.FindChild(weaponAf).gameObject;

				foreach(Transform tr in hbpoint.transform)
				{
					Destroy(tr.gameObject);
				}
				rtpoint.transform.parent = hbpoint.transform;
				rtpoint.transform.localPosition = Vector3.zero;
				rtpoint.transform.localRotation = Quaternion.Euler(Vector3.zero);
			}
		}

		private int breakStandTime = 50;
		private void updateBreakForActStand()
		{
			if (m_Objanimation != null)
			{
				//if (this.Profession == (int)CharacterDefine.PROFESSION.SHAOLIN) 
				//{
					if(AnimInfoNextAnimId == (int)CharacterDefine.CharacterAnimId.ActStand)
					{
						if(breakStandTime>0)
						{
							breakStandTime--;
						}else{
							CurObjAnimState = GameDefine_Globe.OBJ_ANIMSTATE.STATE_NORMOR;
						}
					}else{
						breakStandTime = 50;
					}
				//}
			}
		}
		public bool isHideWeapon=false;
		//public bool isMissionCollect = false;
		public void HideOrShowWeanpon()
		{
			string weaponPath1 = "";
			string weaponPath2 = "";
			if (Profession == (int)CharacterDefine.PROFESSION.TIANSHAN)
			{
			
				weaponPath1="Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 L Clavicle/Bip001 L UpperArm/Bip001 L Forearm/Bip001 L Hand/HH_weaponHandLf";
				weaponPath2="Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/HH_weaponHandRt";
			
			}
			if (Profession == (int)CharacterDefine.PROFESSION.SHAOLIN)
			{
				
				weaponPath1= "Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/HH_weaponHandRt";
					
			}
			if (Profession == (int)CharacterDefine.PROFESSION.DALI)
			{
				
				weaponPath1= "Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/HH_weaponHandRt";
				
			}
			if (Profession == (int)CharacterDefine.PROFESSION.XIAOYAO)
			{
				
				weaponPath1=  "Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/HH_weaponHandRt";
				
			}
			Transform modelTrans = transform.FindChild("Model");
			if (modelTrans == null)
			{
				return;
			}
			

			Transform weaponParent = modelTrans.FindChild(weaponPath1);
			if (weaponParent == null)
			{
				return;
			}

			weaponParent.gameObject.SetActive (isHideWeapon);
			if (weaponPath2 != "")
			{
				
				Transform weaponParent2 = modelTrans.FindChild(weaponPath2);
				if (weaponParent2 == null)
				{
					return;
				}
				
				weaponParent2.gameObject.SetActive (isHideWeapon);
			}
			isHideWeapon=!isHideWeapon;

			//else if (m_RoleType == (int)CharacterDefine.PROFESSION.XIAOYAO)
			//{
			//    Singleton<ObjManager>.GetInstance().ReloadWeapon(m_ModelChooseRole,
			//        tabWeaponModel.ResPath,
			//         OnLoadWeapon,
			//	   	"Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/HH_weaponHandLf");//Weapon_L
			// }
//			else
//			{
//				//剑客初始也要把剑背在背上
//				
//				if(m_RoleType == (int)CharacterDefine.PROFESSION.SHAOLIN||m_RoleType== (int)CharacterDefine.PROFESSION.XIAOYAO)
//				{
//					Singleton<ObjManager>.GetInstance().ReloadWeapon(m_ModelChooseRole,
//					                                                 tabWeaponModel.ResPath,
//					                                                 OnLoadWeapon,
//					                                                 "Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/HH_weaponback");//Weapon_R
//					
//				}
//				else
//					
//					Singleton<ObjManager>.GetInstance().ReloadWeapon(m_ModelChooseRole,
//					                                                 tabWeaponModel.ResPath,
//					                                                 OnLoadWeapon,
//					                                                 "Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/HH_weaponHandRt");//Weapon_R
//			}

		}

	}

}