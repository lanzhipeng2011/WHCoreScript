/********************************************************************************
 *	文件名：	Obj_OtherPlayer.cs
 *	全路径：	\Script\Obj\Obj_OtherPlayer.cs
 *	创建人：	李嘉
 *	创建时间：2013-10-25
 *
 *	功能说明：游戏其他玩家Obj逻辑类
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
using GCGame;
using GCGame.Table;
using Games.Animation_Modle;
using Games.ObjAnimModule;
using System;
using Module.Log;
using Games.Item;
using Games.Scene;
using System.Collections.Generic;
using Games.Events;
using Games.ChatHistory;

namespace Games.LogicObj
{
    public class Obj_OtherPlayer : Obj_Character
    {
        public Obj_OtherPlayer()
        {
            m_ObjType = GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER;
        }

//#if UNITY_ANDROID
//        public string OtherPlayerMoveTag = string.Empty;
//#endif

        //GUID
        private UInt64 m_GUID = GlobeVar.INVALID_GUID;
        public UInt64 GUID
        {
            get { return m_GUID; }
            set { m_GUID = value; }
        }

        private UInt64 m_GuildGUID = GlobeVar.INVALID_GUID;
        public virtual System.UInt64 GuildGUID
        {
            get { return m_GuildGUID; }
            set { m_GuildGUID = value; }
        }

        //跑商状态
        private int m_GuildBusinessState = GlobeVar.INVALID_ID;
        public int GuildBusinessState
        {
            get { return m_GuildBusinessState; }
            set { m_GuildBusinessState = value; }
        }

        public override void OnReloadModle()
        {
            base.OnReloadModle();
            if (m_GuildBusinessState != -1)
            {
                this.UpdateGBStateEffect(m_GuildBusinessState);
            }
        }

        public void UpdateGBStateEffect(int nState)
        {
            if (this.m_playerHeadInfo != null)
            {
                this.m_playerHeadInfo.UpdateGuildBusinessIcon(nState);
            }
            switch (nState)
            {
                case 0:
                    base.StopEffect(0xef, true);
                    base.PlayEffect(0xee, null, null);
                    break;

                case 1:
                    base.StopEffect(0xee, true);
                    base.PlayEffect(0xef, null, null);
                    break;

                default:
                    base.StopEffect(0xee, true);
                    base.StopEffect(0xef, true);
                    break;
            }
        }

        private int m_fellowID;
        public int FellowID
        {
            get { return m_fellowID; }
            set { m_fellowID = value; }
        }

        private bool m_bIsWildEnemyForMainPlayer =false;
        public bool IsWildEnemyForMainPlayer
        {
            get { return m_bIsWildEnemyForMainPlayer; }
            set { m_bIsWildEnemyForMainPlayer = value; }
        }
        public override bool Init(Obj_Init_Data initData)
        {
            if (null == m_ObjTransform)
            {
                m_ObjTransform = transform;
            }

            m_ObjTransform.position = ActiveScene.GetTerrainPosition(new Vector3(initData.m_fX, 0, initData.m_fZ));
            ServerID = initData.m_ServerID;
            
            BaseAttr.RoleBaseID = initData.m_RoleBaseID;
            BaseAttr.MoveSpeed = initData.m_MoveSpeed;
            Profession = initData.m_nProfession;

            Tab_ItemVisual tabItemVisual = TableManager.GetItemVisualByID(initData.m_ModelVisualID, 0);
            if (tabItemVisual == null)
            {
                tabItemVisual = TableManager.GetItemVisualByID(GlobeVar.DEFAULT_VISUAL_ID, 0);
                if (tabItemVisual == null)
                {
                    return false;
                }
            }
            int nCharmodelID = GetCharModelID(tabItemVisual);
            //如果tabItemVisual取出来charModel为空，则给默认charModel
            //这个问题会出现在无职业的ZombiePlayer上
            if (nCharmodelID < 0)
            {
                nCharmodelID = initData.m_CharModelID;
            }
            Tab_CharModel charModel = TableManager.GetCharModelByID(nCharmodelID, 0);
            if (charModel == null)
            {
                return false;
            }
            ModelID = nCharmodelID;

            if (null != charModel)
			{
				//初始化动作路径
                AnimationFilePath = charModel.AnimPath;
				BaseAttr.RoleBaseID = initData.m_RoleBaseID;
                BaseAttr.HeadPic = charModel.HeadPic;
			}

			BaseAttr.RoleName = initData.m_StrName;
			GUID = initData.m_Guid;
            DeltaHeight = charModel.HeadInfoHeight;
            BaseAttr.Die = initData.m_IsDie;
            BaseAttr.Force = initData.m_Force;
            PkModle = initData.m_PkModel;
            
			VipCost = initData.m_nOtherVipCost;
            GuildGUID = initData.m_GuildGuid;
            OtherCombatValue = initData.m_nOtherCombatValue;
            IsWildEnemyForMainPlayer = initData.m_bIsWildEnemyForMainPlayer;
			//初始化动作特效
            if (AnimLogic == null)
            {
                AnimLogic = gameObject.AddComponent<AnimationLogic>();

//#if UNITY_ANDROID
//                if (AnimLogic != null)
//                {
//                    AnimLogic.IsOtherPlayer = true;
//                    AnimLogic.OtherPlayer = this;
//                }
//#endif
            }
            //AnimLogic = gameObject.AddComponent<AnimationLogic>();

            //初始化特效
            if (ObjEffectLogic == null)
            {
                ObjEffectLogic = gameObject.AddComponent<EffectLogic>();
            }

            //ObjEffectLogic = gameObject.AddComponent<EffectLogic>();
            InitEffect();
            //初始化寻路代理
            InitNavAgent();

            //初始化AutoMove功能模块
            //if (gameObject.GetComponent<AutoMove>() == null)
            //{
            gameObject.AddComponent<AutoMove>();
            //}

            if (IsDie())
            {
                OnCorpse();
            }
            else
            {
                if (Objanimation != null)
                {
                    Objanimation.Stop();
                }
                CurObjAnimState = GameDefine_Globe.OBJ_ANIMSTATE.STATE_NORMOR;
            }

            m_strTitleInvestitive = initData.m_strTitleName;
            m_CurTitleID = initData.m_CurTitleID;
            m_bIsInMainPlayerPKList = initData.m_isInMainPlayerPKList;
            PkModle = initData.m_PkModel;
            OptChangPKModle();
            StealthLev = initData.m_StealthLev;
            OptStealthLevChange();
            MountID = initData.m_MountID;

          //  m_ObjTransform.localScale = new Vector3(charModel.Scale, charModel.Scale, charModel.Scale);
            m_ObjTransform.localRotation = Utils.DirServerToClient(initData.m_fDir);
            ModelVisualID = initData.m_ModelVisualID;
            CurWeaponDataID = initData.m_WeaponDataID;
            WeaponEffectGem = initData.m_WeaponEffectGem;
			GuildBusinessState = initData.m_nGuildBusinessState;

            InitNameBoard();

            //init bind(no base.init)
            InitBindFromData(initData);
            
            return true;
        }

        //进入可视区域
        void OnBecameVisible()
        {
            //设置是否在视口内标记位，为其他系统优化判断标识
            ModelInViewPort = true;
            
            //显示名字版
            if (null != m_HeadInfoBoard)
            {
                m_HeadInfoBoard.SetActive(true);
            }
			//如果已经死亡，则切换到死亡状态
			if (IsDie())
			{
				OnCorpse();
			}

            //显示模型
            if (null != ModelNode && IsVisibleChar())
            {
                ModelNode.SetActive(true);
                OnSwithObjAnimState(CurObjAnimState);
            }

//            //如果已经死亡，则切换到死亡状态
//            if (IsDie())
//            {
//                OnCorpse();
//            }
        }

        //离开可视区域
        void OnBecameInvisible()
        {
            //设置是否在视口内标记位，为其他系统优化判断标识
            ModelInViewPort = false;

            //隐藏名字版
            if (null != m_HeadInfoBoard)
            {
                m_HeadInfoBoard.SetActive(false);
            }

            // 隐藏模型
            if (null != ModelNode)
            {
                ModelNode.SetActive(false);
            }
        }

        void FixedUpdate()
        {
//#if UNITY_ANDROID

//            OtherPlayerMoveTag = string.Empty;
//#endif
			UpdateAttckCF(this.Profession);
			UpdateAttckSY ();
			UpdateAttckYS ();
			UpdateAttckHT ();
			UpdateAttckXS ();
			UpdateTargetMove();
            UpdateAnimation();
            UpdateQingGong();       //由于轻功可能改变玩家坐标，所以放在UpdateTargetMove之后进行
			//UpdateAttckPGCF (this.Profession);
            //多次冒血的
            UpdateShowMultiShowDamageBoard();
            //技能结束检测
            if (m_SkillCore != null)
            {
                m_SkillCore.CheckSkillShouldFinish();
            }

			updateWeaponPoint ();

			//======
			updateBreakForActStand ();
			//=====

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

                ShowNameBoard();
                ShowTitleInvestitive();
                UpdateVipInfo();


                //跑商
				UpdateGBNameBoard();
            }
        }

        public void UpdateGBNameBoard()
        {
            if (null != m_playerHeadInfo)
            {
                m_playerHeadInfo.UpdateGuildBusinessIcon(m_GuildBusinessState);
            }
        }

        public virtual void UpdateCombatValue()
        {
            int nNeedEffect = -1;
            if (m_nOtherCombatValue >= GlobeVar.COMBAT_LEVEL_3)
            {
                nNeedEffect = GlobeVar.COMBAT_EFFECT_3;
            }
            else if (m_nOtherCombatValue >= GlobeVar.COMBAT_LEVEL_2)
            {
                nNeedEffect = GlobeVar.COMBAT_EFFECT_2;
            }
            else if (m_nOtherCombatValue >= GlobeVar.COMBAT_LEVEL_1)
            {
                nNeedEffect = GlobeVar.COMBAT_EFFECT_1;
            }
            if ( nNeedEffect > 0 )
            {
                Tab_Effect EffectInfo = TableManager.GetEffectByID(nNeedEffect, 0);
                if (EffectInfo != null && IsHaveBindPoint(EffectInfo.ParentName) && GetEffectCountById(nNeedEffect) < 1)
                {
                    StopEffect(GlobeVar.COMBAT_EFFECT_3);
                    StopEffect(GlobeVar.COMBAT_EFFECT_2);
                    StopEffect(GlobeVar.COMBAT_EFFECT_1);
                    PlayEffect(nNeedEffect);
                }
            }
            else
            {
                StopEffect(GlobeVar.COMBAT_EFFECT_3);
                StopEffect(GlobeVar.COMBAT_EFFECT_2);
                StopEffect(GlobeVar.COMBAT_EFFECT_1);
            }
        }
        public virtual void UpdateVipInfo()
        {
            if (null != m_playerHeadInfo)
            {
                m_playerHeadInfo.UpdateVipInfo(m_nVipCost);
            } 
        }

        public PlayerHeadInfo m_playerHeadInfo;
        
        private string m_strTitleInvestitive;
        public string StrTitleInvestitive
        {
            get { return m_strTitleInvestitive; }
            set { m_strTitleInvestitive = value; }
        }

        private int m_CurTitleID;
        public int CurTitleID
        {
            get { return m_CurTitleID; }
            set { m_CurTitleID = value; }
        }

        public void ShowTitleInvestitive()
        {
            if (null != m_playerHeadInfo)
            {
                m_playerHeadInfo.ShowTitleInvestitiveByTable(m_CurTitleID, m_strTitleInvestitive);
            }      
        }

        //职业
        private int m_nProfession = -1;
        public virtual int Profession
        {
            get { return m_nProfession; }
            set { m_nProfession = value; }
        }
        //VIP
        private int m_nVipCost = -1;
        public virtual int VipCost
        {
            get { return m_nVipCost; }
            set { m_nVipCost = value; UpdateVipInfo(); }
        }

       
        private int m_nOtherCombatValue = -1;
        public virtual int OtherCombatValue
        {
            get { return m_nOtherCombatValue; }
            set { m_nOtherCombatValue = value;}
        }
        //PK模式
        private int m_nPkModle =-1;
        public virtual int PkModle
        {
            get { return m_nPkModle; }
            set { m_nPkModle = value; }
        }
        //是否在主角的反击列表中
        private bool m_bIsInMainPlayerPKList =false;
        public bool IsInMainPlayerPKList
        {
            get { return m_bIsInMainPlayerPKList; }
            set { m_bIsInMainPlayerPKList = value; }
        }
        
        public override Color GetNameBoardColor()
        {
            string strColor = "FFFFFF";
            CharacterDefine.REPUTATION_TYPE type = Reputation.GetObjReputionType(this);
            if (type == CharacterDefine.REPUTATION_TYPE.REPUTATION_FRIEND)
            {
                //陌生人
                strColor = "FFFFFF";
            }
            else if (type == CharacterDefine.REPUTATION_TYPE.REPUTATION_NEUTRAL)
            {
                strColor = "3C00FF";
            }
            else if (type == CharacterDefine.REPUTATION_TYPE.REPUTATION_HOSTILE)
            {
                strColor = "FF0000";
            }

            //对方开了杀戮模式 且尚未攻击过主角 紫名
            if (type == CharacterDefine.REPUTATION_TYPE.REPUTATION_FRIEND &&
                PkModle == (int)CharacterDefine.PKMODLE.KILL && 
                IsInMainPlayerPKList == false)
            {
                //同队伍和同帮会 不能互相PK 不受PK模式影响 不是同一个帮会且不是同一个队伍的 才受此影响
                if ((GameManager.gameManager.PlayerDataPool.IsHaveGuild()==false||GameManager.gameManager.PlayerDataPool.IsGuildMember(this)==false) && 
                    (GameManager.gameManager.PlayerDataPool.IsHaveTeam()==false||GameManager.gameManager.PlayerDataPool.IsTeamMem(GUID)==false) &&
                    GUID != GameManager.gameManager.PlayerDataPool.LoverGUID && //是伴侣
                    GameManager.gameManager.ActiveScene.IsCopyScene() == false //不是副本
                    )
                {
                    strColor = "FF00FF";
                }
            }
           
            return GCGame.Utils.GetColorByString(strColor);
        }

        public virtual  void OptChangPKModle()
        {
            SetNameBoardColor();
        }

        public override void UpdateAttrBroadcastPackt(GC_BROADCAST_ATTR packet)
        {
            base.UpdateAttrBroadcastPackt(packet);
            if (packet.HasCurProfession)
            {
                Profession = packet.CurProfession;
            }
            if (packet.HasModelVisualID)
            {
                ModelVisualID = packet.ModelVisualID;
            }
            if (packet.HasWeaponDataID)
            {
                CurWeaponDataID = packet.WeaponDataID;
            }
            if (packet.HasWeaponEffectGem)
            {
                WeaponEffectGem = packet.WeaponEffectGem;
            }

            if (packet.HasModelVisualID)
            {
                if (SkillCore.IsUsingSkill)
                {
                    m_UpdateModelWait = true;
                }
                else
                {
                    ReloadPlayerModelVisual();
                }
            }
            else if (packet.HasWeaponDataID)
            {
                if (SkillCore.IsUsingSkill)
                {
                    m_UpdateWeaponWait = true;
                }
                else
                {
                    RealoadPlayerWeaponVisual();
                }
            }
            else if (packet.HasWeaponEffectGem)
            {
                if (SkillCore.IsUsingSkill)
                {
                    m_UpdateWeaponGemWait = true;
                }
                else
                {
                    ReloadWeaponEffectGem();
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////
        // 坐骑相关
        //////////////////////////////////////////////////////////////////////////
        private Obj_Mount m_MountObj = null;
        public Obj_Mount MountObj
        {
            get { return m_MountObj; }
            set { m_MountObj = value; }
        }

        private int m_MountID = -1;
        virtual public int MountID
        {
            get { return m_MountID; }
            set { 
                m_MountID = value;
                //RideOrUnMount(m_MountID);
            }
        }

        private bool m_bIsNeedUnMount = false;
        public bool IsNeedUnMount
        {
            get { return m_bIsNeedUnMount; }
            set { m_bIsNeedUnMount = value; }
        }
        public float GetMountNameBoardHeight()
        {
            if (m_MountID == -1)
            {
                return 0;
            }

            Tab_MountBase MountBase = TableManager.GetMountBaseByID(m_MountID, 0);
            if (null == MountBase)
            {
                return 0;
            }
            Tab_CharMount MountTable = TableManager.GetCharMountByID(MountBase.ModelID, 0);
            if (MountTable == null)
            {
                return 0;
            }

            float fFixHeight = 0;
            if (Profession >= 0 && Profession < MountTable.getHeadInfoAddHeightCount())
            {
                fFixHeight = MountTable.GetHeadInfoAddHeightbyIndex(Profession);
            }
            return fFixHeight;
        }

        // 上马下马 统一接口
        public virtual void RideOrUnMount(int nMountID)
        {
//#if UNITY_ANDROID

            if (!m_bVisible && m_ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER)
            {
                MountID = nMountID;
                return;
            }
//#endif

            if (nMountID >=0)
            {
                RideMount(nMountID);
            }
            else
            {
                if (m_MountObj == null)
                {
                    return;
                }
                UnMount();
            }
        }
        // 骑马
        private void RideMount(int nMountID)
        {
            m_AnimLogic.Stop();

            MountID = nMountID;
            Obj_Mount.RideMount(this, nMountID);
        }

        // 下马
        private void UnMount()
        {
            m_bIsNeedUnMount = true;
            Obj_Mount.UnMount(this);
        }

        public override void OptNameChange()//名字变化后的操作
        {
            ShowNameBoard();
        }

        public override void OptForceChange()//势力变化后的操作
        {
           base.OptForceChange();
        }
        public override bool OnCorpse()
        {
            base.OnCorpse();
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
            return true;
        }
        public override bool OnRelife()
        {
            base.OnRelife();
            
            return true;
        }

        // 纸娃娃系统换装
        private int m_ModelVisualID = GlobeVar.INVALID_ID;
        public virtual int ModelVisualID
        {
            get { return m_ModelVisualID; }
            set { m_ModelVisualID = value; }
        }

        private int m_CurWeaponDataID = GlobeVar.INVALID_ID;
        public virtual int CurWeaponDataID
        {
            get { return m_CurWeaponDataID; }
            set { m_CurWeaponDataID = value; }
        }

        private int m_WeaponEffectGem = GlobeVar.INVALID_ID;
        public int WeaponEffectGem
        {
            get { return m_WeaponEffectGem; }
            set { m_WeaponEffectGem = value; }
        }

        public void ReloadWeaponEffectGem()
        {
            List<string> strWeaponPointList = new List<string>();
            if (Profession == (int)CharacterDefine.PROFESSION.TIANSHAN)
            {
                //strWeaponPointList.Add("Weapon_L");
				strWeaponPointList.Add("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 L Clavicle/Bip001 L UpperArm/Bip001 L Forearm/Bip001 L Hand/HH_weaponHandLf");
                //strWeaponPointList.Add("Weapon_R");
				strWeaponPointList.Add("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/HH_weaponHandRt");
            }
           // else if (Profession == (int)CharacterDefine.PROFESSION.XIAOYAO)
            //{
                //strWeaponPointList.Add("Weapon_L");
			//	strWeaponPointList.Add("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 L Clavicle/Bip001 L UpperArm/Bip001 L Forearm/Bip001 L Hand/HH_weaponHandLf");
            //}
            else
            {
                //strWeaponPointList.Add("Weapon_R");
				strWeaponPointList.Add("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/HH_weaponHandRt");
            }

            for (int i = 0; i < strWeaponPointList.Count; ++i)
            {
                GameObject playerGameObject = null;
                if (MountObj == null)
                {
                    playerGameObject = gameObject;
                }
                else
                {
                    if (MountObj.MountPlayer == null)
                    {
                        return;
                    }
                    playerGameObject = MountObj.MountPlayer.gameObject;
                }

                if (playerGameObject == null)
                {
                    continue;
                }

                Transform modelTrans = playerGameObject.transform.FindChild("Model");
                if (modelTrans == null)
                {
                    return;
                }

               // Transform modelallTrans = modelTrans.FindChild("all");
               // if (modelallTrans == null)
               // {
                //    return;
               // }

				Transform weaponParent = modelTrans.FindChild(strWeaponPointList[i]);//modelallTrans
                if (weaponParent == null)
                {
                    return;
                }

				foreach(Transform tr in weaponParent)
				{
					Destroy(tr.gameObject);
				}

                Transform effectParent;
                //if (Profession == (int)CharacterDefine.PROFESSION.XIAOYAO)
               // {
                //    effectParent = weaponParent.FindChild("Weapon_R");
                //    if (effectParent == null)
                //    {
                  //      return;
                 //   }
               // }
               // else
                //{
                effectParent = weaponParent;
               // }

                EffectLogic effectLogic = effectParent.gameObject.GetComponent<EffectLogic>();
                if (null == effectLogic)
                {
                    effectLogic = effectParent.gameObject.AddComponent<EffectLogic>();
                    effectLogic.InitEffect(effectParent.gameObject);
                }
                if (null != effectLogic)
                {
                    effectLogic.ClearEffect();
                    Tab_GemAttr tabGemAttr = TableManager.GetGemAttrByID(WeaponEffectGem, 0);
                    if (tabGemAttr != null)
                    {
                        Tab_Effect tabEffect = TableManager.GetEffectByID(tabGemAttr.EffectID, 0);
                        if (tabEffect != null)
                        {
                            effectLogic.PlayEffect(tabGemAttr.EffectID);
                        }
                    }
                }
            }
        }

        // 放技能时换装需要等待
        private bool m_UpdateModelWait = false;
        private bool m_UpdateWeaponWait = false;
        private bool m_UpdateWeaponGemWait = false;

        public void UpdateVisualAfterSkill()
        {
            if (m_UpdateModelWait)
            {
                ReloadPlayerModelVisual();
            }

            if (m_UpdateWeaponWait)
            {
                RealoadPlayerWeaponVisual();
            }

            if (m_UpdateWeaponGemWait)
            {
                ReloadWeaponEffectGem();
            }

            m_UpdateModelWait = false;
            m_UpdateWeaponWait = false;
            m_UpdateWeaponGemWait = false;
        }

        //玩家轻功部分处理
        private bool m_bQingGongState = false;
        public bool QingGongState
        {
            get { return m_bQingGongState; }
            set { m_bQingGongState = value; }
        }

        private Vector3 m_QingGongSrc = Vector3.zero;
        private Vector3 m_QingGongDst = Vector3.zero;
        private int m_nQingGongType = GlobeVar.INVALID_ID;
        private int m_nQingGongPointID = GlobeVar.INVALID_ID;
        public int QingGongPointID
        {
            get { return m_nQingGongPointID; }
        }
        private float m_fQingGongMaxHeight = 0;
        private float m_fQingGongTime = 0;
        private float m_fQingGongBeginTime = 0;
        public virtual void BeginQingGong(GameEvent _event)
        {
            if (_event.EventID != GameDefine_Globe.EVENT_DEFINE.EVENT_QINGGONG)
            {
                return;
            }

            //得到目标点坐标
            Vector3 dstPos = new Vector3(_event.GetFloatParam(0), _event.GetFloatParam(1), _event.GetFloatParam(2));
            //dstPos.y = Terrain.activeTerrain.SampleHeight(dstPos);

            m_bQingGongState = true;
            m_QingGongDst = dstPos;
            m_QingGongSrc = transform.position;

            //忽略阻挡前进
            if (null != NavAgent && NavAgent.enabled)
            {
                NavAgent.enabled = false;
            }

            m_fMoveSpeed = _event.GetFloatParam(3);
            m_nQingGongType = _event.GetIntParam(0);
            m_fQingGongMaxHeight = _event.GetFloatParam(4);

            m_nQingGongPointID = _event.GetIntParam(1);

            //记录朝向，注意抛物线轨迹由UpdateQingGong进行位移操作，不使用系统MoveTo
            FaceTo(m_QingGongDst);

            //根据不同类型模拟线路和调整玩家角度
            if (m_nQingGongType == (int)GameDefine_Globe.QINGGONG_TRAIL_TYPE.PARABOLA)
            {
                //如果是抛物线移动
                CurObjAnimState = GameDefine_Globe.OBJ_ANIMSTATE.STATE_JUMP;

                //根据起始点和目标点计算本次轻功运动时间
                float fTreckDistance = Vector3.Distance(m_QingGongSrc, m_QingGongDst);
                if (m_fMoveSpeed > 0)
                {
                    m_fQingGongTime = fTreckDistance / m_fMoveSpeed;
                }

                //记录轻功开始时间
                m_fQingGongBeginTime = Time.time;
            }
            else if (m_nQingGongType == (int)GameDefine_Globe.QINGGONG_TRAIL_TYPE.TURN_LEFT)
            {
                //如果是线性移动
                CurObjAnimState = GameDefine_Globe.OBJ_ANIMSTATE.STATE_FASTRUN_LEFT;

                //直线移动，使用底层MoveTo即可
                MoveTo(m_QingGongDst, null, 0.0f);
            }
            else if (m_nQingGongType == (int)GameDefine_Globe.QINGGONG_TRAIL_TYPE.TURN_RIGHT)
            {
                //如果是线性移动
                CurObjAnimState = GameDefine_Globe.OBJ_ANIMSTATE.STATE_FASTRUN_RIGHT;

                //直线移动，使用底层MoveTo即可
                MoveTo(m_QingGongDst, null, 0.0f);
            }

            //播放轻功特效
            if (null != ObjEffectLogic)
            {
                ObjEffectLogic.PlayEffect(62);
            }
        }

        public virtual void EndQingGong()
        {
            StopMove();
            m_bQingGongState = false;

            //恢复数据
            if (NavAgent)
            {
                NavAgent.enabled = true;
            }

            m_fMoveSpeed = BaseAttr.MoveSpeed;

            //重置参数
            m_fQingGongMaxHeight = 0;
            m_fQingGongTime = 0;
            m_fQingGongBeginTime = 0;

            m_QingGongSrc = Vector3.zero;
            m_QingGongDst = Vector3.zero;
        }

        public virtual void UpdateQingGong()
        {
            if (false == m_bQingGongState)
            {
                return;
            }

            //到达目的地，轻功结束
            if (Vector3.Distance(m_ObjTransform.position, m_QingGongDst) < 0.4f)
            {
                EndQingGong();
            }

            //如果是抛物线轨迹，根据最大高度进行抛物线轨迹更新
            if (m_nQingGongType == (int)GameDefine_Globe.QINGGONG_TRAIL_TYPE.PARABOLA)
            {
                //计算从轻功开始到结束的流逝时间
                float fElapseTime = Time.time - m_fQingGongBeginTime;

                //如果轻功持续时间结束，也结束轻功
                //if (fElapseTime >= m_fQingGongTime)
                //{
                //    EndQingGong();
                //}

                //更新运动轨迹
                Vector3 fMoveDirection = (m_QingGongDst - m_QingGongSrc).normalized;

                //当前点
                Vector3 curPos = m_QingGongSrc + fMoveDirection * m_fMoveSpeed * fElapseTime;

                if (m_fQingGongMaxHeight > 0 && m_fQingGongTime > 0)
                {
                    //获得当前时间在轻功总行程中的路径比例
                    float fRate = fElapseTime / m_fQingGongTime;

                    float fHeightRefix = 0.0f;
                    //抛物线分前半段和后半段，分别处于上升和下降
                    if (fRate < 0.5f)
                    {
                        fHeightRefix = m_fQingGongMaxHeight * (fRate / 0.5f);
                    }
                    else
                    {
                        fHeightRefix = m_fQingGongMaxHeight * ((1 - fRate) / 0.5f);
                    }

                    //修正MainPlayer的高度
                    //Vector3 pos = transform.position;
                    curPos.y = curPos.y + fHeightRefix;
                    m_ObjTransform.position = curPos;
                }
            }
        }

        public void ShowChatBubble(ChatHistoryItem history)
        {
            if (null != m_playerHeadInfo)
            {
                m_playerHeadInfo.ShowChatBubble(history);
            }
        }

        public virtual void SetVisible(bool bVisible)
        {
            base.SetVisible(bVisible);
            OnSwithObjAnimState(CurObjAnimState);

            Obj_Character curFellow = Singleton<ObjManager>.Instance.FindObjInScene(m_fellowID) as Obj_Character;
            if (null == curFellow)
            {
                return;
            }

            curFellow.SetVisible(bVisible);
        }

		//==============
		public   void   updateWeaponPoint()//GameDefine_Globe.OBJ_ANIMSTATE ObjState
		{
			if (m_Objanimation != null)
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
		
		private Tab_WeaponModel reloadWeaponPath()
		{
			// 重载武器
			bool defaultVisual = false;
			Tab_EquipAttr tabEquipAttr = TableManager.GetEquipAttrByID(this.CurWeaponDataID, 0);
			Tab_ItemVisual tabItemVisual = null;
			
			//Tab_EquipAttr tabEquipAttr = TableManager.GetEquipAttrByID(nWeaponDataID, 0);
			if (tabEquipAttr == null)
			{
				defaultVisual = true;
			}
			else
			{
				tabItemVisual = TableManager.GetItemVisualByID(tabEquipAttr.ModelId, 0);
				if (tabItemVisual == null)
				{
					defaultVisual = true;
				}
			}
			
			if (defaultVisual)
			{
				tabItemVisual = TableManager.GetItemVisualByID(this.CurWeaponDataID, 0);//GlobeVar.DEFAULT_VISUAL_ID
				if (tabItemVisual == null)
				{
					return null;
				}
			}

			int nProfession = GlobeVar.INVALID_ID;
			if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER ||
			    ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER ||
			    ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_ZOMBIE_PLAYER)
			{
				Obj_OtherPlayer objPlayer = this as Obj_OtherPlayer;
				nProfession = objPlayer.Profession;
			}
			int nWeaponModelID = GetWeaponModelID(tabItemVisual, nProfession);
			
			Tab_WeaponModel tabWeaponModel = TableManager.GetWeaponModelByID(nWeaponModelID, 0);
			if (tabWeaponModel == null)
			{
				return null;
			}
			
			return tabWeaponModel;
		}
		
		private void changeWeaponPath(string weaponUrl,int tempIndex)
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
			}
			
			if(weaponBf == "")
				return;
			
			GameObject  rtpoint;
			if( this.gameObject.transform.FindChild(weaponBf) != null)
			{
				rtpoint = this.gameObject.transform.FindChild(weaponBf).gameObject;
				
				GameObject  hbpoint = this.gameObject.transform.FindChild(weaponAf).gameObject;
				rtpoint.transform.parent = hbpoint.transform;
				rtpoint.transform.localPosition = Vector3.zero;
				rtpoint.transform.localRotation = Quaternion.Euler(Vector3.zero);
			}
		}
		//==========end=========
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

    }
}
