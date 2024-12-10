/********************************************************************************
 *	文件名：	Obj_Character_Action.cs
 *	全路径：	\Script\Obj\Obj_Character_Action.cs
 *	创建人：	李嘉
 *	创建时间：2013-12-3
 *
 *	功能说明：游戏逻辑Obj_Character类的移动相关部分
 *	修改记录：
 *	李嘉 2014-02-19 将原来的obj基类上移作为有动作行为的obj_character，下层添加基类obj
*********************************************************************************/

using System.Security.Permissions;
using Games.SkillModle;
using GCGame;
using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
using Games.Events;
using Games.Animation_Modle;
using GCGame.Table;
using Module.Log;
namespace Games.LogicObj
{
    public partial class Obj_Character : Obj
    {

        #region Obj移动相关
        //obj的移动速度,无NavAgent时使用
        public float m_fMoveSpeed = 5.0f;

        //是否在移动中
        private bool m_bIsMoving;
        public bool IsMoving
        {
            get { return m_bIsMoving; }
            set 
			{
				m_bIsMoving = value; 
			}
        }

        private bool m_bIsTracing = false;//是否在追踪
        public bool IsTracing
        {
            get { return m_bIsTracing; }
            set { m_bIsTracing = value; }
        }

        //移动的目标点
        private Vector3 m_vecTargetPos;
        public Vector3 VecTargetPos
        {
            get { return m_vecTargetPos; }
            set
            {
                m_vecTargetPos = value;
                m_bIsMoving = true;
            }
        }

        //寻路代理
        private NavMeshAgent m_NavAgent = null;
        public UnityEngine.NavMeshAgent NavAgent
        {
            get { return m_NavAgent; }
            set { m_NavAgent = value; }
        }

        //初始化寻路代理
        public void InitNavAgent()
        {
            if (NavAgent == null)
            {
                NavAgent = gameObject.AddComponent<NavMeshAgent>();
            }
            //NavAgent = gameObject.GetComponent<NavMeshAgent>();

            //初始化自动寻路
            if (null != NavAgent && 0 != gameObject.transform.localScale.x)
            {
                NavAgent.enabled = true;
                //设置成0，否则gameobject之间会互相碰撞
                NavAgent.speed = BaseAttr.MoveSpeed;
                NavAgent.radius = 0.0f;
                NavAgent.height = 2.0f / gameObject.transform.localScale.x;
                NavAgent.acceleration = 10000.0f;
                NavAgent.angularSpeed = 30000.0f;
                NavAgent.autoRepath = false;
                NavAgent.autoBraking = true;
            }
        }

        private bool m_bIsMoveToNoFaceTo = false; //调用moveto时 是否禁用了朝向旋转
        public bool IsMoveToNoFaceTo
        {
            get { return m_bIsMoveToNoFaceTo; }
            set { m_bIsMoveToNoFaceTo = value; }
        }

        private UniformAcceleratedMotion m_AcceleratedMotion = null; //UniformAcceleratedMotion组件
        public UniformAcceleratedMotion AcceleratedMotion
        {
            get { return m_AcceleratedMotion; }
            set { m_AcceleratedMotion = value; }
        }
        
        //是否允许移动过程中根据目标点调整方向
        private bool m_EnableMovingRotation = true; 
        public void EnableMovingRotation(bool bEnable)
        {
            if (null != NavAgent)
            {
                if (bEnable)
                {
                    NavAgent.angularSpeed = 30000.0f;
                }
                else
                {
                    NavAgent.angularSpeed = 0.0f;
                }
            }

            m_EnableMovingRotation = bEnable;
        }

        //保存路径
        //private PathNodeCollection m_MovePathList = new PathNodeCollection();


        private GameObject m_MoveTarget = null;

        public GameObject MoveTarget
        {
            get { return m_MoveTarget; }
            set
            {
                if (value != m_MoveTarget)
                {
                    m_MoveTarget = value;
                    if (null != m_MoveTarget)
                    {
                        m_moveTargetTrans = m_MoveTarget.transform;
                    }
                    else
                    {
                        m_moveTargetTrans = null;
                    }
                }
            }
        }
        //停止距离
        private float m_fStopRange;
        public float StopRange
        {
            get { return m_fStopRange; }
            set { m_fStopRange = value; }
        }

        //移动结束后触发的事件
        protected GameEvent m_MoveOverEvent;
        public GameEvent MoveOverEvent
        {
            get { return m_MoveOverEvent; }
            set { m_MoveOverEvent = value; }
        }

        //防卡住措施
        private Vector3 m_LastPosition = new Vector3(0, 0, 0);
        private float m_LastPositionTime = 0.0f;

		public bool  m_IsMJ=false;
        void ResetMoveOverEvent()
        {
            m_MoveOverEvent.Reset();
        }

        //移动对外接口
        //设置好了目标点和停止距离之后会自动在Update中移动
        public void BeforeMoveTo(bool bIsAutoSearch)
        {

            if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER)
            {
                //寻路过程中 打断主角的自动战斗状态
                //  Singleton<ObjManager>.GetInstance().MainPlayer.BreakAutoCombatState();

                //如果不是自动寻路，而AutoSearchAgent还在工作的话，则清理自动寻路状态
                if (false == bIsAutoSearch &&
                    null != GameManager.gameManager.AutoSearch &&
                    true == GameManager.gameManager.AutoSearch.IsAutoSearching)
                {
                    GameManager.gameManager.AutoSearch.Stop();
                }
                else if (bIsAutoSearch) //如果是自动寻路 
                {
                    //打断自动挂机状态
                    Singleton<ObjManager>.GetInstance().MainPlayer.BreakAutoCombatState();
                }
            }
            //打断 移动可以打断的技能

            if (m_SkillCore !=null)
            {
                if (m_SkillCore.UsingSkillBaseInfo !=null &&
                    m_SkillCore.UsingSkillBaseInfo.IsMoveBreak ==1)
                {
                    m_SkillCore.BreakCurSkill();
                }
            }
            if (AnimLogic != null &&
                AnimLogic.CurAnimData !=null &&
                AnimLogic.CurAnimData.AnimID ==(int)CharacterDefine.CharacterAnimId.Hit)
            {
                m_AnimLogic.Stop();
            }
        }

        public void MoveTo(Vector3 targetPos, GameObject targetObj = null, float fStopRange = 2.0f, bool bIsAutoSearch = false)
        {

			if (m_bIsCanPGCF) 
			{
				return ;
			}
            m_fStopRange = fStopRange;
            //设定寻路目标
            if (null != targetObj)
            {
                MoveTarget = targetObj;
                m_vecTargetPos = targetObj.transform.localPosition;
            }
            else
            {
                MoveTarget = null;
                m_vecTargetPos = targetPos;
            }

			//如果已经足够近了，就不执行移动
			float fDis = Vector3.Distance(this.Position, m_vecTargetPos);
			float fDiffDis = fDis - fStopRange;

            //移动前的操作
            BeforeMoveTo(bIsAutoSearch);

            //如果已经足够近了，就不执行移动
//            float fDis = Vector3.Distance(this.Position, m_vecTargetPos);
//            float fDiffDis = fDis - fStopRange;

            if (fDiffDis <= 0)
            {

                StopMove();
                ResetMoveOverEvent();
			
                return;
            }

            //如果存在NavAgent，则由NavAgent进行移动
            if (null != NavAgent && NavAgent.enabled == true)
            {
                NavAgent.stoppingDistance = fStopRange;
				NavAgent.destination = m_vecTargetPos;

                if (IsMoveToNoFaceTo)
                {
                    EnableMovingRotation(false);
                }
            }
            m_bIsMoving = true;
        }

        public void StopMove()
        {
            if (null != NavAgent && NavAgent.enabled == true&&NavAgent.gameObject.activeSelf)
            {
                NavAgent.Stop();
                //如果在moveto时 禁用了旋转 移动结束后 重新置为打开
                if (IsMoveToNoFaceTo)
                {
                    EnableMovingRotation(true);
                    IsMoveToNoFaceTo = false;
                }
            }

            if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER)
            {
                Obj_MainPlayer MainPlayer = this as Obj_MainPlayer;
                if (MainPlayer.Thirdcontroller != null && MainPlayer.Thirdcontroller.IsMoving)
                {
                    CurObjAnimState = GameDefine_Globe.OBJ_ANIMSTATE.STATE_WALK;
                }
                else
                {
                    CurObjAnimState = GameDefine_Globe.OBJ_ANIMSTATE.STATE_NORMOR;
                }
                //取消地面移动特效
                if (null != GameManager.gameManager.ActiveScene)
                {
                    GameManager.gameManager.ActiveScene.DeactiveMovingCircle();
                }
            }
            else
            {
//#if UNITY_ANDROID
//                if (ObjType != GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER)
//                {
//#endif
                    CurObjAnimState = GameDefine_Globe.OBJ_ANIMSTATE.STATE_NORMOR;

//#if UNITY_ANDROID
//                }
//#endif
            }

            //正常结束处理
            m_bIsMoving = false;
            m_bIsTracing = false;
            float fDis = Vector3.Distance(this.Position, m_vecTargetPos);
            if (m_fStopRange - fDis >= 0)
            {
				if(MoveTarget)
				{
				if(MoveTarget.CompareTag("JuqingItem"))
				{
					Obj_JuqingItem item=MoveTarget.GetComponent<Obj_JuqingItem>();
					if(item.IsActive==true)
					{
					if(Singleton<ObjManager>.GetInstance().MainPlayer.MountObj==null)
					{
						Singleton<ObjManager>.GetInstance().MainPlayer.AnimLogic.Play(1009);
						Singleton<ObjManager>.GetInstance().MainPlayer.HideOrShowWeanpon();
								item.StopEffect(301);
					    item.DelaysendMsg();
						CG_JUQINGITEM_PLAYEFFECT  msg = (CG_JUQINGITEM_PLAYEFFECT)PacketDistributed.CreatePacket(MessageID.PACKET_CG_JUQINGITEM_PLAYEFFECT);
						msg.Itemid=item.ID;
						msg.Sceneclassid=GameManager.gameManager.RunningScene;
						msg.Sceneinstid=0;
						msg.Juqingitemname=item.gameObject.name;
						msg.SendPacket();
						Singleton<ObjManager>.GetInstance().MainPlayer.LeveJuqing();
						
					}
					}
				}
				}
                OnMoveOver();
            }

            //   MoveTarget = null;
            ResetMoveOverEvent();

            //重置移动卡住校验
            m_LastPosition = Vector3.zero;
            m_LastPositionTime = 0;
        }

        //朝向屏幕
        public void FaceToScreen()
        {
            gameObject.transform.Rotate(new Vector3(0, 145, 0));
        }

        //NPC面向某一点
        public void FaceTo(Vector3 facePos)
        {
            if (!m_EnableMovingRotation)
            {
                return;
            }

            //facePos.y = 0;
            Vector3 lookRot = facePos - m_ObjTransform.position;
            lookRot.y = 0;
            if (lookRot == Vector3.zero)
            {
                return;
            }
            if (m_SkillCore == null || m_SkillCore.IsUsingSkill == false)//正在使用技能的时候不转向
            {
                m_ObjTransform.rotation = Quaternion.LookRotation(lookRot);
            }
        }

        //Obj向自己的目标点移动
        Transform m_moveTargetTrans = null;

        protected void UpdateTargetMove()
        {
//#if UNITY_ANDROID
            //OtherPlayerMove();
//#else
            if (m_bIsMoving && IsDie() == false)
            {
                CurObjAnimState = GameDefine_Globe.OBJ_ANIMSTATE.STATE_WALK;

                if (null != MoveTarget && null != m_moveTargetTrans)
                {
                    m_vecTargetPos = m_moveTargetTrans.localPosition;
                }

                MoveToPosition(m_vecTargetPos, m_fStopRange);
            }
//#endif
        }

        //移动结束后的操作 
        protected virtual void OnMoveOver()
        {
        }

        //防卡死校验间隔
        static float s_MovingCheckInterval = 0.5f;
        //临时解决 记录上次距离移动目标的距离
        public void MoveToPosition(Vector3 targetPos, float fStopRange)
        {

            //获得当前坐标
            Vector3 vecPos = m_ObjTransform.position;

            //由于移动其实是在2D平面进行距离判定，所以直接将y置0即可
            float fDistance = Vector2.Distance(new Vector2(vecPos.x, vecPos.z), new Vector2(targetPos.x, targetPos.z));

            //阻挡卡怪问题 上次移动校验和本次移动坐标相同 停止移动
            if (Time.time - m_LastPositionTime > s_MovingCheckInterval)
            {
                //发现两次校验间隔移动距离过小，则停止移动
                if (Vector3.Distance(m_LastPosition, vecPos) <= 0.2f)
                {
                    //如果是其他玩家 则将玩家之间挪到目标点
                    if (ObjType ==GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER)
                    {
                        m_ObjTransform.position = targetPos;
                    }
                    StopMove();
                    return;
                }
                m_LastPositionTime = Time.time;
                m_LastPosition = vecPos;
            }

//#if UNITY_ANDROID

//            if (ObjType != GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER)
// 	          {
//#endif
               // 判断是否停止移动
                if (fDistance - fStopRange <= 0)
                {
                    StopMove();
                    return;
                }

                //如果距离小于移动距离则移动到目标点
                if (fDistance - m_fMoveSpeed * Time.deltaTime <= 0)
                {
                    m_ObjTransform.position = targetPos;
                    StopMove();
                    return;
                }

//#if UNITY_ANDROID
//            }
//#endif

            //如果没有使用NavAgent进行移动，则游戏内部逻辑控制移动
            if (null == NavAgent || NavAgent.enabled == false)
            {
                Vector3 vecMovDirction = targetPos - vecPos;
                vecMovDirction = vecMovDirction.normalized;
                vecMovDirction *= m_fMoveSpeed;
                vecMovDirction *= Time.deltaTime;

                Vector3 pos = vecPos + vecMovDirction;
                //pos.y = Terrain.activeTerrain.SampleHeight(pos);
                m_ObjTransform.position = pos;

                //更新朝向
                FaceTo(pos);
            }
        }

        private int m_Tag = 0;
        private void OtherPlayerMove()
        {
            if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER)
            {
                if (m_bIsMoving && IsDie() == false)
                {
                    if (null != MoveTarget && null != m_moveTargetTrans)
                    {
                        m_vecTargetPos = m_moveTargetTrans.localPosition;
                    }

                    m_Tag = 0;
                    if (!OtherPlayerIsMove(m_vecTargetPos, m_fStopRange))
                    {
                        return;
                    }

                    CurObjAnimState = GameDefine_Globe.OBJ_ANIMSTATE.STATE_WALK;
                    MoveToPosition(m_vecTargetPos, m_fStopRange);
                }
                else
                {
                    if (m_Tag <= 0)
                    {
                        CurObjAnimState = GameDefine_Globe.OBJ_ANIMSTATE.STATE_NORMOR;
                        m_Tag++;
                    }
                }
            }
            else
            {
                if (m_bIsMoving && IsDie() == false)
                {
                    CurObjAnimState = GameDefine_Globe.OBJ_ANIMSTATE.STATE_WALK;

                    if (null != MoveTarget && null != m_moveTargetTrans)
                    {
                        m_vecTargetPos = m_moveTargetTrans.localPosition;
                    }

                    MoveToPosition(m_vecTargetPos, m_fStopRange);
                }
            }
        }

        private bool OtherPlayerIsMove(Vector3 targetPos, float fStopRange)
        {
            //获得当前坐标
            Vector3 vecPos = m_ObjTransform.position;

            //由于移动其实是在2D平面进行距离判定，所以直接将y置0即可
            float fDistance = Vector2.Distance(new Vector2(vecPos.x, vecPos.z), new Vector2(targetPos.x, targetPos.z));

            //判断是否停止移动
            if (fDistance - fStopRange <= 0)
            {
                StopMove();
                return false;
            }

            return true;
        }

        #endregion

        #region Obj动作相关
        private GameDefine_Globe.OBJ_ANIMSTATE m_CurObjAnimState;
        //!!注意 访问 m_CurObjAnimState 时 请使用属性CurObjAnimState访问
        public GameDefine_Globe.OBJ_ANIMSTATE CurObjAnimState
        {
            get { return m_CurObjAnimState; }
            set
            {
                OnSwithObjAnimState(value);
            }
        }
        //Obj动作文件路径,涉及到在运行中是否需要动态加载动作
        private string m_AnimationFilePath = "";
        public string AnimationFilePath
        {
            get { return m_AnimationFilePath; }
            set { m_AnimationFilePath = value; }
        }

        public float walkMaxAnimationSpeed = 0.75f;

        protected Animation m_Objanimation; //!!!使用前记得判空
        public UnityEngine.Animation Objanimation//!!!使用前记得判空
        {
            get { return m_Objanimation; }

        }

        protected AnimationLogic m_AnimLogic = null;
        public AnimationLogic AnimLogic
        {
            get { return m_AnimLogic; }
            set { m_AnimLogic = value; }
        }

        //初始化动作接口，目前是硬代码，之后会根据配表之类的实现
        public void InitAnimation()
        {
            if (null == m_AnimLogic)
            {
                return;
            }

            //首先保存该ObjAction的路径
            m_AnimLogic.AnimResFilePath = m_AnimationFilePath;

            //由于目前Obj采用的都是Root作为根节点，而独立挂Model节点，所以动作组件采取搜索子节点方式
            Transform modelTransform = gameObject.transform.FindChild("Model");
            if (modelTransform)
            {
                m_Objanimation = modelTransform.gameObject.GetComponent<Animation>();
            }

            if (m_Objanimation)
            {
                m_AnimLogic.InitState(m_Objanimation.gameObject);
            }
            else
            {
                LogModule.DebugLog("The character you would like to control doesn't have animations. Moving her might look weird.");
            }
        }

        protected void UpdateAnimation()
        {
            if (m_AnimLogic == null)
            {
                return;
            }
            if (m_Objanimation == null)
            {
                return;
            }
            m_AnimLogic.AnimationUpdate();
            //没做其他动作了 切换回待机动作
//			if (m_SkillCore.IsUsingSkill == true) 
//			{
//				return ;
//			}
            if (m_Objanimation.isPlaying == false &&
                IsDie() == false)
            {
                CurObjAnimState = GameDefine_Globe.OBJ_ANIMSTATE.STATE_NORMOR;
            }
        }

        private float m_fLastPlayHitSoundTime = 0; //上次玩家受击音效播放的时间
     //   private float m_fLastPlayDamageSoundTime = 0; //上次玩家受伤害音效播放的时间
		public  virtual void   OnSwithObjAnimState(GameDefine_Globe.OBJ_ANIMSTATE ObjState)
        {



            if (IsDie() && 
                ObjState != GameDefine_Globe.OBJ_ANIMSTATE.STATE_DEATH  && //死亡状态
                ObjState != GameDefine_Globe.OBJ_ANIMSTATE.STATE_CORPSE && //尸体状态
                ObjState != GameDefine_Globe.OBJ_ANIMSTATE.STATE_ATTACKFLY) //死亡击飞状态
            {
                return;
            }
            m_CurObjAnimState = ObjState; //!!此处勿修改
            if (m_Objanimation != null) {
				switch (CurObjAnimState) {
				case GameDefine_Globe.OBJ_ANIMSTATE.STATE_NORMOR:
					ProcessIdleAnimState ();
					break;
				case GameDefine_Globe.OBJ_ANIMSTATE.STATE_WALK:
					ProcessWalkAnimState ();
					break;
				case GameDefine_Globe.OBJ_ANIMSTATE.STATE_DEATH:
					ProcessDeathAnimState ();
					break;
				case GameDefine_Globe.OBJ_ANIMSTATE.STATE_HIT:
				case GameDefine_Globe.OBJ_ANIMSTATE.STATE_HITBYSHAOLIN:
				case GameDefine_Globe.OBJ_ANIMSTATE.STATE_HITBYTIANSHAN:
				case GameDefine_Globe.OBJ_ANIMSTATE.STATE_HITBYDALI:
				case GameDefine_Globe.OBJ_ANIMSTATE.STATE_HITBYXIAOYAO:
					ProcessHitAnimState ();
					break;
				case GameDefine_Globe.OBJ_ANIMSTATE.STATE_ATTACKDOWN:
//					m_AnimLogic.Stop ();
//					m_AnimLogic.Play ((int)(CharacterDefine.CharacterAnimId.Knockback_01));
					ProcessHitAnimState ();
                        //受击特效
					PlayEffect ((int)GameDefine_Globe.EffectID.BEHIT);
					break;
				case GameDefine_Globe.OBJ_ANIMSTATE.STATE_ATTACKFLY:
					AnimLogic.Stop ();
					AnimLogic.Play ((int)CharacterDefine.CharacterAnimId.Knockback_02);
					break;
				case GameDefine_Globe.OBJ_ANIMSTATE.STATE_JUMP:
					{
						AnimLogic.Stop ();
						AnimLogic.Play ((int)CharacterDefine.CharacterAnimId.Jump01);
					}
					break;
				case GameDefine_Globe.OBJ_ANIMSTATE.STATE_JUMP_END:
					{
						AnimLogic.Stop ();
						AnimLogic.Play ((int)CharacterDefine.CharacterAnimId.JumpEnd_Stand);
					}
					break;
				case GameDefine_Globe.OBJ_ANIMSTATE.STATE_FASTRUN_LEFT:
					{
						AnimLogic.Stop ();
						AnimLogic.Play ((int)CharacterDefine.CharacterAnimId.Fastrun_Left);
					}
					break;
				case GameDefine_Globe.OBJ_ANIMSTATE.STATE_FASTRUN_RIGHT:
					{
						AnimLogic.Stop ();
						AnimLogic.Play ((int)CharacterDefine.CharacterAnimId.Fastrun_Right);
					}
					break;
				case GameDefine_Globe.OBJ_ANIMSTATE.STATE_CORPSE:
					{
						AnimLogic.Stop ();
						AnimLogic.Play (1011);
					}
					break;
				default:
					break;
				}
				if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER) {
					Obj_MainPlayer mainPlayer = this as Obj_MainPlayer;
					if (mainPlayer.Profession == (int)CharacterDefine.PROFESSION.XIAOYAO) {
						//mainPlayer.weap
					}
				}
			 }
			}
			//待机状态处理
			void ProcessIdleAnimState()
           {
            if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER)
            {
                // 玩家装备了坐骑
                Obj_MainPlayer MainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
                if (MainPlayer.IsDie() == false && MainPlayer.GetEquipMountID() > 0 && MainPlayer.MountObj)
                {
                    m_AnimLogic.Play((int)CharacterDefine.CharacterAnimId.Stand);
                    MainPlayer.MountObj.PlayMountPlayerAnima((int)CharacterDefine.CharacterAnimId.RIDE_STAND);
                }
                else
                    m_AnimLogic.Play((int)(CharacterDefine.CharacterAnimId.Stand));
            }
            else
            {
                if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER)
                {
                    Obj_OtherPlayer OtherPlayer = this as Obj_OtherPlayer;
                    if (OtherPlayer && OtherPlayer.IsDie() == false && OtherPlayer.MountID > 0 && OtherPlayer.MountObj)
                    {
                        m_AnimLogic.Play((int)CharacterDefine.CharacterAnimId.Stand);
                        OtherPlayer.MountObj.PlayMountPlayerAnima((int)CharacterDefine.CharacterAnimId.RIDE_STAND);
                    }
                    else
                        m_AnimLogic.Play((int)(CharacterDefine.CharacterAnimId.Stand));
                }
                else
                {
                    //如果是其他Obj，就需要根据DataID区分是否为战斗型的NPC
                    //非战斗NPC默认播放Stand动作
                    //战斗NPC播放AttackStand动作
                    Tab_RoleBaseAttr roleBaseTab = TableManager.GetRoleBaseAttrByID(BaseAttr.RoleBaseID, 0);
				
					if(roleBaseTab!=null&&roleBaseTab.IsZA==1)
					{
						return ;
					}
					//roleBaseTab.CombatNPC
					if (null != roleBaseTab &&(BaseAttr.RoleBaseID==20201||BaseAttr.RoleBaseID==40205||BaseAttr.RoleBaseID==20206 ||
					                           BaseAttr.RoleBaseID==30201||BaseAttr.RoleBaseID==40206|BaseAttr.RoleBaseID==40206||BaseAttr.RoleBaseID==100204||
					                           BaseAttr.RoleBaseID==100205|| BaseAttr.RoleBaseID==20208||
					                           BaseAttr.RoleBaseID==160209|| BaseAttr.RoleBaseID==160210||BaseAttr.RoleBaseID==160211||BaseAttr.RoleBaseID==160212||
					                           BaseAttr.RoleBaseID==160213||BaseAttr.RoleBaseID==160214||BaseAttr.RoleBaseID==160309||BaseAttr.RoleBaseID==160310||
					                           BaseAttr.RoleBaseID==160311||BaseAttr.RoleBaseID==160312||BaseAttr.RoleBaseID==160313||BaseAttr.RoleBaseID==160314||
					                           BaseAttr.RoleBaseID==210303
					                           ))
					{
						//是战斗NPC，播放AttackStand
						m_AnimLogic.Play((int)(CharacterDefine.CharacterAnimId.ActStand));
                    }
                    else
                    {
                        m_AnimLogic.Play((int)(CharacterDefine.CharacterAnimId.Stand));
                    }
                }
            }
        }
        //行走状态处理
        void ProcessWalkAnimState()
        {
            //移动时 还在播受击动作或者击倒动作 则停止播放
            if (AnimLogic != null &&
                AnimLogic.CurAnimData != null)
            {
                if (AnimLogic.CurAnimData.AnimID == (int)CharacterDefine.CharacterAnimId.Hit ||
                    AnimLogic.CurAnimData.AnimID == (int)CharacterDefine.CharacterAnimId.Knockback_01)
                {
                    m_AnimLogic.Stop();
                }
            }
            if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER)
            {
                // 玩家装备了坐骑
                Obj_MainPlayer MainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
                if (MainPlayer.IsDie() == false && MainPlayer.GetEquipMountID() > 0 && MainPlayer.MountObj)
                {
                    m_AnimLogic.Play((int)CharacterDefine.CharacterAnimId.Walk);
                    MainPlayer.MountObj.PlayMountPlayerAnima((int)CharacterDefine.CharacterAnimId.RIDE_RUN);
                }
                else
                    m_AnimLogic.Play((int)(CharacterDefine.CharacterAnimId.Walk));
            }
            else
            {
                if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER)
                {
                    Obj_OtherPlayer OtherPlayer = this.GetComponent<Obj_OtherPlayer>();
                    if (OtherPlayer && OtherPlayer.IsDie() == false && OtherPlayer.MountID > 0 && OtherPlayer.MountObj)
                    {
                        m_AnimLogic.Play((int)CharacterDefine.CharacterAnimId.Walk);
                        OtherPlayer.MountObj.PlayMountPlayerAnima((int)CharacterDefine.CharacterAnimId.RIDE_RUN);
                    }
                    else
                        m_AnimLogic.Play((int)(CharacterDefine.CharacterAnimId.Walk));
                }
                else
                    m_AnimLogic.Play((int)(CharacterDefine.CharacterAnimId.Walk));
            }
        }

        //有衰减的音效播放，目前用在NPC的受击、死亡音效上
       public void PlaySoundAtPos(GameDefine_Globe.OBJ_TYPE ObjType, int nSoundID, Vector3 playingPos)
        {
            if (GameManager.gameManager.SoundManager == null)
            {
                return;
            }

            if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER)
            {
                GameManager.gameManager.SoundManager.PlaySoundEffect(nSoundID);
            }
            else
            {
                if (Singleton<ObjManager>.Instance.MainPlayer != null)
                {
                    GameManager.gameManager.SoundManager.PlaySoundEffectAtPos2(nSoundID, playingPos, Singleton<ObjManager>.Instance.MainPlayer.Position);
                }
                else
                {
                    GameManager.gameManager.SoundManager.PlaySoundEffect(nSoundID);
                }
            }
        }

        //行走状态处理
        void ProcessDeathAnimState()
        {
			//死亡动画不在在这里播放了，注释掉。dsy
            if (AnimLogic != null && m_Objanimation !=null)
            {
                //是否在播放动作
                if (m_Objanimation.isPlaying &&  AnimLogic.CurAnimData != null)
                {
                    //击飞是不播死亡动画
                    if (AnimLogic.CurAnimData.AnimID != (int)CharacterDefine.CharacterAnimId.Knockback_01 &&
                        AnimLogic.CurAnimData.AnimID != (int)CharacterDefine.CharacterAnimId.Knockback_02)
                    {
                        m_AnimLogic.Stop();
                        m_AnimLogic.Play((int)(CharacterDefine.CharacterAnimId.Die));
                    } 
                }
                else
                {
                    m_AnimLogic.Stop();
                    m_AnimLogic.Play((int)(CharacterDefine.CharacterAnimId.Die));
                }
            }
		
			Obj_OtherPlayer obj = this as Obj_OtherPlayer;
			if (obj != null) {
				CharacterDefine.PROFESSION  profession =(CharacterDefine.PROFESSION ) obj.Profession;
						int soundid = -1;
						switch (profession) {
						case CharacterDefine.PROFESSION.SHAOLIN:
								soundid = 17;
								break;
						case CharacterDefine.PROFESSION.TIANSHAN:
								soundid = 18;
								break;
						case CharacterDefine.PROFESSION.DALI:
								soundid = 11;
								break;
						case CharacterDefine.PROFESSION.XIAOYAO:
								soundid = 19;
								break;
						}
						PlaySoundAtPos (ObjType, soundid, Position);
				}
            Tab_RoleBaseAttr roleBaseAttr = TableManager.GetRoleBaseAttrByID(BaseAttr.RoleBaseID, 0);
            if (roleBaseAttr != null)
            {
                Tab_CharModel charModel = TableManager.GetCharModelByID(roleBaseAttr.CharModelID, 0);
                if (charModel != null)
                {
                    int nDeadSoundCount = charModel.getDeadSoundCount();
                    int nRandNum = Random.Range(0, nDeadSoundCount - 1);
                    if (charModel.GetDeadSoundbyIndex(nRandNum) >=0 )
                    {
                        PlaySoundAtPos(ObjType, charModel.GetDeadSoundbyIndex(nRandNum), Position);
                    }
                }
            }
        }
        //受击状态处理
        void ProcessHitAnimState()
        {
            if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_NPC)
            {
				//PlayDissolve(0.1f, 0.5f);
                Tab_RoleBaseAttr _roleBaseAttr = TableManager.GetRoleBaseAttrByID(BaseAttr.RoleBaseID, 0);
                if (_roleBaseAttr != null && _roleBaseAttr.NpcType == (int)GameDefine_Globe.NPC_TYPE.BOSS)
                {
                    if (null != m_AnimLogic)
                        m_AnimLogic.Play((int)(CharacterDefine.CharacterAnimId.PlayerHit));
                }
                else
                {
                    if (null != m_AnimLogic)
						m_AnimLogic.Play(6004);
                }
            }
            else if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER ||
                    ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER ||
                    ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_ZOMBIE_PLAYER ||
                    ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_FELLOW)
            {
                bool isPlay = true;
                Obj_OtherPlayer _otherPlayer = null;
                if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER ||
                    ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER)
                {
                    _otherPlayer = this as Obj_OtherPlayer;
                }
                if (_otherPlayer ==null)
                {
//                    if (null != m_AnimLogic)
//                        isPlay = m_AnimLogic.Play((int)(CharacterDefine.CharacterAnimId.PlayerHit));
                }
                else if (_otherPlayer !=null && _otherPlayer.MountID ==-1) //骑马就不要播受击动作了
                {
//                    if (null != m_AnimLogic)
//                        isPlay = m_AnimLogic.Play((int)(CharacterDefine.CharacterAnimId.PlayerHit));
                }
                //玩家播放受击音效
                if (isPlay)
                {
                    if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER ||
                        ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER ||
                        ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_ZOMBIE_PLAYER)
                    {
                        
                        //受击音效不要太频繁 加个时间间隔
                        if (_otherPlayer != null && Time.time - m_fLastPlayHitSoundTime >= 2)
                        {
                            m_fLastPlayHitSoundTime = Time.time;
                            if (_otherPlayer.Profession == (int)CharacterDefine.PROFESSION.SHAOLIN)
                            {
                                PlaySoundAtPos(ObjType, 5, Position);  //attack1_shaolin
                            }
                            else if (_otherPlayer.Profession == (int)CharacterDefine.PROFESSION.TIANSHAN)
                            {
                                PlaySoundAtPos(ObjType, 6, Position);  //attack1_tianshan
                            }
                            else if (_otherPlayer.Profession == (int)CharacterDefine.PROFESSION.DALI)
                            {
                                PlaySoundAtPos(ObjType, 1, Position);  //attack1_dali
                            }
                            else if (_otherPlayer.Profession == (int)CharacterDefine.PROFESSION.XIAOYAO)
                            {
                                PlaySoundAtPos(ObjType, 7, Position);  //attack1_xiaoyao
                            }
                        }
                        //受伤害音效不要太频繁 加个时间间隔
                    //    if (_otherPlayer != null && Time.time - m_fLastPlayDamageSoundTime >= 1.5f)
                        {
                       //     m_fLastPlayDamageSoundTime = Time.time;
                            PlaySoundAtPos(ObjType, 10, Position);  //common_hurt
                        }
                    }
                }
            }
            //击中效果跟技能音效分离的处理
            //音量随机
            //float volumeFactor = Random.Range(0.5f, 1.0f);
            if (CurObjAnimState == GameDefine_Globe.OBJ_ANIMSTATE.STATE_HITBYSHAOLIN)
            {
                PlaySoundAtPos(ObjType, 4, Position);   //attack1_gun
            }
            else if (CurObjAnimState == GameDefine_Globe.OBJ_ANIMSTATE.STATE_HITBYTIANSHAN)
            {
                PlaySoundAtPos(ObjType, 2, Position);   //attack1_dao
            }
            else if (CurObjAnimState == GameDefine_Globe.OBJ_ANIMSTATE.STATE_HITBYDALI)
            {
                PlaySoundAtPos(ObjType, 0, Position);   //attack_jian
            }
            else if (CurObjAnimState == GameDefine_Globe.OBJ_ANIMSTATE.STATE_HITBYXIAOYAO)
            {
                PlaySoundAtPos(ObjType, 3, Position);   //attack1_fu
            }
            //受击特效 主角自己不播放
            if (ObjType !=GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER)
            {
			   PlayEffect((int)GameDefine_Globe.EffectID.BEHIT);
            }
        }
		//隐身
		public float m_fXSLastTime = 8.0f;
		private float m_fYScurTime = 0.0f;
		public bool  m_bIsYS = false;
		GameObject obj3=null;
		GameObject obj4=null;
		GameObject obj5=null;
		public void AttackYS()
		{
			if (m_bIsYS)
			{
				return;
			}
			
			m_bIsYS =true;
			m_fYScurTime = Time.time;

			if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER) 
			{
								StealthLev = 1;
								OptStealthLevChange ();
			}
			else 
			{
			          Obj_OtherPlayer  oth=this as Obj_OtherPlayer;
				    if(oth!=null)
					oth.RideOrUnMount(-1);
					if(obj3==null)
				                 {
						              obj3=this.gameObject.transform.FindChild("Model").gameObject;
				      
				                 }
				                  if(obj4==null)
					               obj4=this.gameObject.transform.FindChild("EffectPoint").gameObject;
			                 	if(obj5==null)
					                obj5=this.gameObject.transform.FindChild("shadow").gameObject;

								if(obj3!=null)
								obj3.SetActive(false);
								if(obj4!=null)
								obj4.SetActive(false);
				                if(obj5!=null)
					                obj5.SetActive(false);
				                CloseNameBoard();
				                
				
				
			}
			StopMove ();
			//更新运动轨迹

		}
		
		public  void UpdateAttckYS()
		{
			
			if (m_bIsYS ==false)
			{
				return;
			}


			if((Time.time-m_fYScurTime)>=m_fXSLastTime)
			{
			
				if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER)
				{
					StealthLev = 0;
					OptStealthLevChange ();
				}
				else
				{
					if(obj3!=null)
						obj3.SetActive(true);
					if(obj4!=null)
						obj4.SetActive(true);
					if(obj5!=null)
						obj5.SetActive(true);
					ShowNameBoard();
				}

				m_bIsYS=false;

			}

			
		}

		//消失
		private float m_fXSLastTime2 = 0.467f;
		private float m_fXSLastTime3 = 1.667f;
		private float m_fXScurTime = 0.0f;
		private bool  m_bIsXS = false;
		public void AttackXS()
		{
			if (m_bIsXS)
			{
				return;
			}
			
			m_bIsXS =true;
			m_fXScurTime = Time.time;

			StopMove ();
			//更新运动轨迹
			
		}
		GameObject obj2=null;
		public  void UpdateAttckXS()
		{
			
			if (m_bIsXS ==false)
			{
				return;
			}
			
			
			if((Time.time-m_fXScurTime)>=m_fXSLastTime2)
			{
				if(obj2==null)
				    obj2=this.gameObject.transform.FindChild("Model").gameObject;
		
				if((Time.time-m_fXScurTime)<=m_fXSLastTime3)
				{
					if(obj2)
					obj2.SetActive(false);	
				}
				else
				{
					if(obj2)
					{
						obj2.SetActive(true);
					   m_bIsXS=false;
						m_AnimLogic.Play(3016);
					}
				}

				
			}
			
			
		}
		//后退
		private float m_fHTLastTime = 1.1f;
		private float m_fHTcurTime = 0.0f;
		private bool  m_bIsCanHT = false;
		private Vector3  m_vecHTTarget=Vector3.zero;
		private float m_fHTSpeed=1;
		private Vector3  m_vecHTTar2=Vector3.zero;
		public void AttackHT()
		{
			if (m_bIsCanHT)
			{
				return;
			}
			//Invoke()
			m_bIsCanHT =true;
			//StopMove ();
			//更新运动轨迹
			m_vecHTTarget = m_ObjTransform.position +  -m_ObjTransform.forward*3;

			m_fHTcurTime = Time.time;
			NavAgent.enabled = false;

			
		}
		
		public  void UpdateAttckHT()
		{
			
			if (m_bIsCanHT ==false)
			{
				return;
			}
			
			if ((Time.time - m_fHTcurTime)<m_fHTLastTime)
			{
				return ;
			}
			if (NavAgent.enabled == false) 
			{
						NavAgent.enabled = true;
				NavAgent.SetDestination( m_vecHTTarget);
				
				NavAgent.speed = m_fHTSpeed;
				}
			if (NavAgent.pathPending==false) 
			{
				m_vecHTTar2 = NavAgent.pathEndPosition ;
				NavAgent.enabled=false;
				m_ObjTransform.position = m_vecHTTar2;
				//IsMoving=false;
				NavAgent.speed = BaseAttr.MoveSpeed;
				NavAgent.enabled=true;
				m_bIsCanHT = false;
			}
			
		}

		//瞬移
		private float m_fSXLastTime = 1.0f;
		private float m_fSXcurTime = 0.0f;
		private bool  m_bIsCanSY = false;
		private Vector3  m_vecTarget=Vector3.zero;
		private float m_fSYSpeed=300;
		private Vector3  m_vecTar2=Vector3.zero;
		public void AttackSY()
		{
			if (m_bIsCanSY)
			{
				return;
			}
		
			m_bIsCanSY =true;
			//StopMove ();
			//更新运动轨迹
			m_vecTarget = m_ObjTransform.position +  m_ObjTransform.forward*5;
			NavAgent.SetDestination( m_vecTarget);
		
			NavAgent.speed = m_fSYSpeed;
			m_fSXcurTime = Time.time;
			GameObject obj=this.gameObject.transform.FindChild("Model").gameObject;
			if(obj)
				obj.SetActive(false);

		}

		public  void UpdateAttckSY()
		{
		
			if (m_bIsCanSY ==false)
			{
				return;
			}
			
		
			if (NavAgent.pathPending==false) 
			{
				if(NavAgent.remainingDistance<0.1f)
				{
				GameObject obj=this.gameObject.transform.FindChild("Model").gameObject;
				if(obj)
					obj.SetActive(true);	
				NavAgent.speed = BaseAttr.MoveSpeed;
				CG_MOVE movPacket = (CG_MOVE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_MOVE);
				movPacket.Poscount = 1;
				movPacket.AddPosx((int)(m_ObjTransform.position.x * 100));
				movPacket.AddPosz((int)(m_ObjTransform.position.z * 100));

				movPacket.Ismoving =1;
				m_bIsCanSY = false;
				}
			}
			
		}
        //普攻前的冲锋
		private float m_fAttackPGCFSpeed = 12.0f;
		private float m_fAttackPGCFLastTime = 3.0f;
		private float m_fAttackPGCFMaxHight = 0.0f;
		private float m_fAttckPGCFBeginTime =0.0f;
		private Vector3 m_VecAttcakPGCFSrc = new Vector3(0, 0, 0);
		private Vector3 m_VecAttcakPGCFDst = new Vector3(0, 0, 0);
		public bool m_bIsCanPGCF = false;
		private Obj_Character  m_cfPGTarget = null;
		CameraController camController =null;
		public void AttackPGCF(Obj_Character  vtarget)
		{
			if (m_bIsCanPGCF)
			{
				return;
			}
			
			//StopMove ();
			m_bIsCanPGCF = true;
			m_AnimLogic.Stop();
			//m_AnimLogic.Play(2018);
			m_fAttckPGCFBeginTime = Time.time;
			PlayEffect (41);
			camController= Singleton<ObjManager>.GetInstance().MainPlayer.GetComponent<CameraController>();
			if ( camController!=null)
			{
				//camController.InitCameraRock(4);
				camController.InitCameraTrack(this.Position,vtarget.Position);
			}
			//更新运动轨迹
			if(NavAgent!=null&&vtarget!=null)
			{
				NavAgent.enabled=false;
				m_VecAttcakPGCFSrc=m_ObjTransform.position;
				m_VecAttcakPGCFDst=vtarget.gameObject.transform.position;
				//NavAgent.destination = m_VecAttcakCFDst;
				//NavAgent.speed = m_fAttackPGCFSpeed;//
				m_cfPGTarget=vtarget;
				//IsMoving=true;
			}
			//NavAgent.destination = vtarget;
		}
		
		public  void UpdateAttckPGCF(int profess)
		{
			float fStopRange = 1.0f;
			if (m_bIsCanPGCF ==false)
			{
				//	NavAgent.speed =BaseAttr.MoveSpeed;
				return;
			}
			//LogModule.DebugLog (m_AnimLogic.CurAnimData.AinmName);
			//到达目的地 结束
			if (Vector3.Distance(m_ObjTransform.position, m_VecAttcakPGCFDst) < 1.5f)
			{
				//m_ObjTransform.position = m_VecAttcakCFDst;
//				if(profess==0)
//				{
//					m_AnimLogic.Stop();
//					m_AnimLogic.Play(2014);
//				}
				NavAgent.enabled=true;
				EndAttckPGCF();
				CG_MOVE movPacket = (CG_MOVE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_MOVE);
				movPacket.Poscount = 1;
				movPacket.AddPosx((int)(m_ObjTransform.position.x * 100));
				movPacket.AddPosz((int)(m_ObjTransform.position.z * 100));
			
        	    movPacket.Ismoving =1;
				movPacket.SendPacket();
				OnEnterCombat(m_cfPGTarget);
				StopEffect(41);
				camController.ResetCameraToMainPlayer();
				//IsMoving=false;
				return;
			}
			//更新运动轨迹
			Vector3 fMoveDirection = (m_VecAttcakPGCFDst - m_VecAttcakPGCFSrc).normalized;
			//当前点
			Vector3 curPos = m_ObjTransform.position  + fMoveDirection *m_fAttackPGCFSpeed*Time.deltaTime ;
			m_ObjTransform.position = curPos;
			//			  m_ObjTransform.position = curPos;
			//			if (NavAgent.remainingDistance <= 1||(Time.time-m_fAttckCFBeginTime)>m_fAttackCFLastTime)
			//			{
			//			
			//				int anid=m_AnimLogic.CurAnimData.LoopOverAnimId;
			//				///m_AnimLogic.Stop();
			//				//m_AnimLogic.Play(2014);
			//			}
			//			
			
		}
		public void EndAttckPGCF()
		{
			m_bIsCanPGCF = false;
			//NavAgent.speed =BaseAttr.MoveSpeed;
			NavAgent.enabled = true;
			//NavAgent.destination = null;
		}

		//冲锋
		private float m_fAttackCFSpeed = 12.0f;
		private float m_fAttackCFLastTime = 3.0f;
		private float m_fAttackCFMaxHight = 0.0f;
		private float m_fAttckCFBeginTime =0.0f;
		private Vector3 m_VecAttcakCFSrc = new Vector3(0, 0, 0);
		private Vector3 m_VecAttcakCFDst = new Vector3(0, 0, 0);
		public bool m_bIsCanCF = false;
		private Obj_Character  m_cfTarget = null;
		public void AttackCF(Obj_Character  vtarget)
		{
			if (m_bIsCanCF)
			{
				return;
			}
			
			//StopMove ();
			m_bIsCanCF = true;
			m_fAttckCFBeginTime = Time.time;
			//更新运动轨迹
			if(NavAgent!=null&&vtarget!=null)
			{
				NavAgent.enabled=false;
				m_VecAttcakCFSrc=m_ObjTransform.position;
				m_VecAttcakCFDst=vtarget.gameObject.transform.position;
				//NavAgent.destination = m_VecAttcakCFDst;
				//NavAgent.speed = m_fAttackCFSpeed;//
				m_cfTarget=vtarget;
			}
			PlayEffect (41);
			//NavAgent.destination = vtarget;
		}
		
		public  void UpdateAttckCF(int profess)
		{
			float fStopRange = 1.0f;
			if (m_bIsCanCF ==false)
			{
			//	NavAgent.speed =BaseAttr.MoveSpeed;
				return;
			}
		//	LogModule.DebugLog (m_AnimLogic.CurAnimData.AinmName);
			//到达目的地 结束
			if (Vector3.Distance(m_ObjTransform.position, m_VecAttcakCFDst) < 2.5f)
			{
				//m_ObjTransform.position = m_VecAttcakCFDst;
		       if(profess==0)
				{
				m_AnimLogic.Stop();
				m_AnimLogic.Play(2014);
				}
				NavAgent.enabled=true;
				CG_MOVE movPacket = (CG_MOVE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_MOVE);
				movPacket.Poscount = 1;
				movPacket.AddPosx((int)(m_ObjTransform.position.x * 100));
				movPacket.AddPosz((int)(m_ObjTransform.position.z * 100));
			
				movPacket.Ismoving =1;
				//IsMoving=true;
				EndAttckCF();
				StopEffect(41);
				return;
			}
			//更新运动轨迹
			Vector3 fMoveDirection = (m_VecAttcakCFDst - m_VecAttcakCFSrc).normalized;
			//当前点
			Vector3 curPos = m_ObjTransform.position  + fMoveDirection *m_fAttackCFSpeed*Time.deltaTime ;
			m_ObjTransform.position = curPos;
			//			  m_ObjTransform.position = curPos;
//			if (NavAgent.remainingDistance <= 1||(Time.time-m_fAttckCFBeginTime)>m_fAttackCFLastTime)
//			{
//			
//				int anid=m_AnimLogic.CurAnimData.LoopOverAnimId;
//				///m_AnimLogic.Stop();
//				//m_AnimLogic.Play(2014);
//			}
//			
			
		}
		public void EndAttckCF()
		{
			m_bIsCanCF = false;
			NavAgent.speed =BaseAttr.MoveSpeed;
			//NavAgent.destination = null;
		}

        //击飞
        private float m_fAttackFlySpeed = 0.0f;
        private float m_fAttackFlyTime = 0.0f;
        private float m_fAttackFlyMaxHight = 0.0f;
        private float m_fAttckFlyBeginTime =0.0f;
        private Vector3 m_VecAttcakFlySrc = new Vector3(0, 0, 0);
        private Vector3 m_VecAttcakFlyDst = new Vector3(0, 0, 0);
        private bool m_bIsCanAttckFly = false;
        public void AttackFly(int nDis, int nHight, float fTime)
        {
            if (m_bIsCanAttckFly)
            {
                return;
            }
            if (fTime <= 0)
            {
                return;
            }
            m_fAttackFlySpeed =nDis/fTime; //速度
            m_fAttackFlyTime =fTime;
            m_fAttackFlyMaxHight =nHight;
            Vector3 vecDirect = new Vector3(0, 0, 0);
            float fDir = Utils.DirClientToServer(Rotation);
            fDir = Utils.NormaliseDirection(fDir);
            vecDirect.x = 0 - Mathf.Cos(fDir);
            vecDirect.z = 0 - Mathf.Sin(fDir);
            vecDirect = vecDirect.normalized;//移动方向
            m_VecAttcakFlySrc =m_ObjTransform.position;//起始点
            m_VecAttcakFlyDst =m_ObjTransform.position + vecDirect * m_fAttackFlySpeed * m_fAttackFlyTime; //目的点
            //忽略阻挡前进
            if (null != NavAgent && NavAgent.enabled)
            {
                NavAgent.enabled = false;
            }
            m_fAttckFlyBeginTime =Time.time;
            m_bIsCanAttckFly =true;
            PlayEffect(121); //击飞特效
        }

        public  void UpdateAttckFly()
        {
            if (m_bIsCanAttckFly ==false)
            {
                return;
            }
            //计算从轻功开始到结束的流逝时间
            float fElapseTime = Time.time - m_fAttckFlyBeginTime;
            //到达目的地 结束
            if (Vector3.Distance(m_ObjTransform.position, m_VecAttcakFlyDst) < 0.4f)
            {
                m_ObjTransform.position = m_VecAttcakFlyDst;
                m_bIsCanAttckFly = false;
                return;
            }
            //更新运动轨迹
            Vector3 fMoveDirection = (m_VecAttcakFlyDst - m_VecAttcakFlySrc).normalized;
            //当前点
            Vector3 curPos = m_VecAttcakFlySrc + fMoveDirection * m_fAttackFlySpeed * fElapseTime;
            //击飞动作 本身带有高度变化 这里就不再做Y轴的位移了 美术说会有卡顿的感觉
            //if (m_fAttackFlyMaxHight > 0 && m_fAttackFlyTime > 0)
            //{
            //    //获得当前时间在总行程中的路径比例
            //    float fRate = fElapseTime/m_fAttackFlyTime;

            //    float fHeightRefix = 0.0f;
            //    //抛物线分前半段和后半段，分别处于上升和下降
            //    if (fRate < 0.5f)
            //    {
            //        fHeightRefix = m_fAttackFlyMaxHight*(fRate/0.5f);
            //    }
            //    else
            //    {
            //        fHeightRefix = m_fAttackFlyMaxHight * ((1 - fRate) / 0.5f);
            //    }
            //    //修正当前的位置                
            //    curPos.y = curPos.y + fHeightRefix;
            //    if (curPos.y < m_VecAttcakFlySrc.y)
            //    {
            //        curPos =m_VecAttcakFlyDst;
            //    }
              
            //}
            m_ObjTransform.position = curPos;
        }
		//float time=0;
		//int inx=0;
		Obj_MainPlayer obj;

		virtual public void OnAnimationCallBack(int animationID)
		{

			Tab_Animation animInfo = TableManager.GetAnimationByID(animationID, 0);
			if (IsDie())
			{
				return;
			}
			if (animInfo == null)
			{
				return;
			}
			if (m_AnimLogic == null)
			{
				return;
			}
				
			
			if (animInfo.NextAnimId != -1)
			{
				//       m_AnimLogic.Stop();
				bool isplay= m_AnimLogic.Animation.IsPlaying(animInfo.AinmName);
				if(isplay==false)
					return ;
				obj = this as Obj_MainPlayer;
				if(obj!=null)
				{
					int skillid=obj.CurUseSkillId;
					if(skillid==-1)
					{
					 //  skillid=obj.OwnSkillInfo[0].SkillId;
					}
			
					
				}
			}
			obj = this as Obj_MainPlayer;
			if (obj == null)
				return;
			//if (obj.CurUseSkillId == -1) 
			{
				if (animInfo.NextAnimId == -1)
				{
					bool isplay= m_AnimLogic.Animation.IsPlaying(animInfo.AinmName);
					if(isplay==false)
						return ;
					
					if(obj!=null)
					{
						int skillid=obj.CurUseSkillId;
						if(skillid==-1)
						{
						//	skillid=obj.OwnSkillInfo[0].SkillId;
						}
	
						
					}
					
				}
			}
			
		}
		virtual public void OnAnimationFinish(int animationID)
        {

			//LogModule.DebugLog ("当前的技能ID"+animationID);
            Tab_Animation animInfo = TableManager.GetAnimationByID(animationID, 0);
            if (IsDie())
            {
                return;
            }
            if (animInfo == null)
            {
                return;
            }
            if (m_AnimLogic == null)
            {
                return;
            }

		
            if (animInfo.NextAnimId != -1)
            {


              	m_AnimLogic.Play(animInfo.NextAnimId);
				//特殊技能处理，播该动作隐身

				AnimInfoNextAnimId = animInfo.NextAnimId;
			}


		}

		private int m_AnimInfoNextAnimId; //指定当前动作ID用于判断该动作时各职业武器挂点
		public int AnimInfoNextAnimId
		{
			get { return m_AnimInfoNextAnimId; }
			set { m_AnimInfoNextAnimId = value; }
		}
		
		virtual public void OnAnimationStop(int aninationID)
        {
            // did nothing
			//AnimInfoNextAnimId =-1;
        }
		//by dsy 检查是否是冲锋
		public bool CheckChongFeng(int skillid)
		{
			//添加目标判断是，如果没有目标则停止动作  by dsy

			Tab_SkillEx _skillExinfo = TableManager.GetSkillExByID(skillid, 0);
			if (_skillExinfo == null)
				return false;
			Tab_SkillBase _skillbase = TableManager.GetSkillBaseByID (_skillExinfo.BaseId, 0);
			if (_skillbase != null && ((_skillbase.SkillClass & (int)SKILLCLASS.CHONGFENG) != 0)) 
			{
				LogModule.DebugLog ("check chong feng" + skillid);
				return true;
			}
			else
			{
				return false;
			}
			
		}
		public bool IsMoveNavAgent(Vector3 targetVector3)
		{
			if (this.m_NavAgent == null)
			{
				return false;
			}
			return this.m_NavAgent.CalculatePath(targetVector3, this.m_NavAgent.path);
		}
		

		// by dys 检查是否是普攻
		public bool CheckPG(SkillCore skillcore)
		{
			if (skillcore == null)
				return false;
			if (skillcore.UsingSkillBaseInfo == null)
				return false;
			int baseid = skillcore.UsingSkillBaseInfo.Id;
			if(baseid==2001||baseid==3001||baseid==4001||baseid==5001)
			{
				return true;

			}
			return false;
		}
		//设置慢镜头
		public void SetMJ()
		{
			m_IsMJ = !m_IsMJ;
		}
        #endregion
    }
}