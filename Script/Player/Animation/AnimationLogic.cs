using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Games.LogicObj;
using UnityEngine;
using System.Collections;
using GCGame.Table;
using Games.GlobeDefine;
using System.Collections.Generic;
using Module.Log;
using Games.SkillModle;
namespace Games.Animation_Modle
{
    public class AnimationLogic : MonoBehaviour
    {

		private float hurttime = 0;
		private int   hurttimes = 0;
		private Animation m_animation;
        public UnityEngine.Animation Animation
        {
            get { return m_animation; }

        }

        private string m_animResFilePath = "";       //动画资源路径
        public string AnimResFilePath
        {
            get { return m_animResFilePath; }
            set { m_animResFilePath = value; }
        }
        private GameObject m_animGameObj ;
        private EffectLogic m_ObjEffectLogic = null;
        private Obj_Character m_ObjChar = null;
        private AnimationState m_curAnimState;
        public UnityEngine.AnimationState CurAnimState
        {
            get { return m_curAnimState; }
            set { m_curAnimState = value; }
        }
        private Tab_Animation m_curAnimData;
        public GCGame.Table.Tab_Animation CurAnimData
        {
            get { return m_curAnimData; }
            set { m_curAnimData = value; }
        }
        private Dictionary<int, Tab_Animation> animStateCache;
        private LinkedList<int> m_NeedCheckEndCallBack = new LinkedList<int>();

//#if UNITY_ANDROID
//        public bool IsOtherPlayer = false;
//        public Obj_OtherPlayer OtherPlayer = null;
//#endif

        public void InitState(GameObject animObj)
        {
            if (null == animObj)
                return;

            Transform animParent = animObj.transform.parent;
            m_animation = animObj.animation;
            if (null != animParent)
            {
                m_animGameObj = animParent.gameObject;
                if (m_animGameObj != null)
                {
                    m_ObjEffectLogic = m_animGameObj.GetComponent<EffectLogic>();
                    m_ObjChar = m_animGameObj.GetComponent<Obj_Character>();
                }
                animStateCache = new Dictionary<int, Tab_Animation>();
            }
        }

        public Animation GetAnimation()
        {
            return m_animation;
        }
        
        protected float m_fLastLoopTime = 0.0f;
        public float LastLoopTime
        {
            get { return m_fLastLoopTime; }

        }

        protected int m_nLoopTimes = 0;
        public int LoopTimes
        {
            get { return m_nLoopTimes; }

        }

        private delegate void LoadAnimationDelegate(string animPath, AnimationClip curAnimClip, object param1, object param2);
		private bool isCallBack=true;
		protected void CheckForNeedCallBack()
		{
			if (m_animation ==null)
			{
				return;
			}
			if (m_animGameObj ==null)
			{
				return;
			}
			if (m_NeedCheckEndCallBack == null || m_NeedCheckEndCallBack.Count <= 0)
			{
				return;
				
			}
			if (m_curAnimData ==null)
			{
				return;
			}
			LinkedList<int> NeedRemoveAnim = new LinkedList<int>();
			int[] TmpNeedCheckArry = new int[m_NeedCheckEndCallBack.Count];
			m_NeedCheckEndCallBack.CopyTo(TmpNeedCheckArry, 0);
			for (int AnimIndex = 0; AnimIndex < TmpNeedCheckArry.Length; AnimIndex++)
			{
				int AnimId = TmpNeedCheckArry[AnimIndex];
				if (m_NeedCheckEndCallBack.Contains(AnimId) == false)
				{
					continue;
				}
				Tab_Animation curAnimData = GetAnimInfoById(AnimId);
				if (curAnimData != null && m_animGameObj != null)
				{
					if (curAnimData.WrapMode != 2 && m_animation.IsPlaying(curAnimData.AinmName) == false)
					{
						if (m_ObjChar !=null)
						{
							if (m_ObjEffectLogic !=null)
							{
								FinishAnimEffect(curAnimData.AnimID);
							}
							m_ObjChar.OnAnimationFinish(curAnimData.AnimID);
						}
						NeedRemoveAnim.AddFirst(curAnimData.AnimID);
					}
					else if (curAnimData.WrapMode == 2 && m_fLastLoopTime > 0)
					{
						if (curAnimData.SPEED != 0 && m_animation[curAnimData.AinmName] !=null)
						{
							float LoopOnecAnimTime = m_animation[curAnimData.AinmName].length / m_curAnimData.SPEED;
							if (Time.time - m_fLastLoopTime >= (m_nLoopTimes + 1) * LoopOnecAnimTime)
							{
								m_nLoopTimes++;
								if (m_ObjChar!=null)
								{
									m_ObjChar.OnAnimationFinish(curAnimData.AnimID);
								

								}
							}
						}
						if (m_nLoopTimes >= curAnimData.LoopTime)
						{
							Stop();
							m_nLoopTimes = 0;
							m_fLastLoopTime = 0.0f;
							if (m_curAnimData.LoopOverAnimId != -1)
							{
								Play(m_curAnimData.LoopOverAnimId);
							
							}
						}
					}
				}
			}
			if (NeedRemoveAnim != null && NeedRemoveAnim.Count > 0)
			{                
				foreach (int animId in NeedRemoveAnim)
				{
					if (m_NeedCheckEndCallBack.Contains(animId))
					{
						m_NeedCheckEndCallBack.Remove(animId);
					}
				}
			}
		}
//        protected void CheckForNeedCallBack()
//        {
//			//添加目标判断是，如果没有目标则停止动作  by dsy
//			Obj_MainPlayer obj = ObjManager.GetInstance ().MainPlayer;
//
//            if (m_animation ==null)
//            {
//                return;
//            }
//            if (m_animGameObj ==null)
//            {
//                return;
//            }
//            if (m_NeedCheckEndCallBack == null || m_NeedCheckEndCallBack.Count <= 0)
//            {
//                return;
//
//            }
//            if (m_curAnimData ==null)
//            {
//                return;
//            }
//            LinkedList<int> NeedRemoveAnim = new LinkedList<int>();
//            int[] TmpNeedCheckArry = new int[m_NeedCheckEndCallBack.Count];
//            m_NeedCheckEndCallBack.CopyTo(TmpNeedCheckArry, 0);
//            for (int AnimIndex = 0; AnimIndex < TmpNeedCheckArry.Length; AnimIndex++)
//            {
//                int AnimId = TmpNeedCheckArry[AnimIndex];
//                if (m_NeedCheckEndCallBack.Contains(AnimId) == false)
//                {
//                    continue;
//                }
////				if (obj != null) 
////				{
////					if(obj.SelectTarget==null)
////					{
////						continue ;
////					}
////				}
//                Tab_Animation curAnimData = GetAnimInfoById(AnimId);
//
//                if (curAnimData != null && m_animGameObj != null)
//                {
//					float hurtpoint=curAnimData.HurtPoint;//by dsy 加入受伤播放时间点
//					int temhurttime=curAnimData.HurtTimes;
//					//  多次伤害
//					if (temhurttime>1&&curAnimData.IsHurt==true&&curAnimData.WrapMode != 2 &&m_animation.IsPlaying(curAnimData.AinmName) == true)
//					{
//						if (curAnimData.SPEED != 0 && m_animation[curAnimData.AinmName] !=null)
//						{
//							float perOnecHurtTime = m_animation[curAnimData.AinmName].length / temhurttime;
//							if (Time.time - hurttime >= (hurttimes + 1) * perOnecHurtTime)
//							{
//								hurttimes++;
//								if (m_ObjChar!=null)
//								{
//									m_ObjChar.OnAnimationCallBack(curAnimData.AnimID);
//								}
//							}
//						}
//						if (hurttimes >= temhurttime)
//						{
//						
//							hurttimes = 0;
//							hurttime = 0.0f;
//
//						}
//					}
//					//单次伤害
//					if (curAnimData.IsHurt==true&&isCallBack&&curAnimData.WrapMode != 2 &&m_animation.IsPlaying(curAnimData.AinmName) == true&& m_animation[curAnimData.AinmName].normalizedTime>hurtpoint)
//					{
//						if (m_ObjChar !=null)
//						{
//							//在回调中发送技能开始试用消息  by dsy
//							m_ObjChar.OnAnimationCallBack(curAnimData.AnimID);
//							isCallBack=false;
//							
//						}
//					}
//					if (curAnimData.WrapMode != 2 && m_animation.IsPlaying(curAnimData.AinmName) == false)
//                    {
//                        if (m_ObjChar !=null)
//                        {
//                            if (m_ObjEffectLogic !=null)
//                            {
//                               FinishAnimEffect(curAnimData.AnimID);
//                            }
//                            m_ObjChar.OnAnimationFinish(curAnimData.AnimID);
//                        }
//					
//                        NeedRemoveAnim.AddFirst(curAnimData.AnimID);
//                    }
//                    else if (curAnimData.WrapMode == 2 && m_fLastLoopTime > 0)
//                    {
//                        if (curAnimData.SPEED != 0 && m_animation[curAnimData.AinmName] !=null)
//                        {
//                            float LoopOnecAnimTime = m_animation[curAnimData.AinmName].length / m_curAnimData.SPEED;
//                            if (Time.time - m_fLastLoopTime >= (m_nLoopTimes + 1) * LoopOnecAnimTime)
//                            {
//                                m_nLoopTimes++;
//
//								//循环的伤害
//								//  多次伤害
//								if (curAnimData.IsHurt==true)
//								{
//
//										if (m_ObjChar!=null)
//										{
//											m_ObjChar.OnAnimationCallBack(curAnimData.AnimID);
//										}
//									LogModule.DebugLog("m_nLoopTimes   "+m_nLoopTimes);
//
//							
//
//								}
//                                if (m_ObjChar!=null)
//                                {
//                                    m_ObjChar.OnAnimationFinish(curAnimData.AnimID);
//                                }
//
//
//                            }
//                        }
//                        if (m_nLoopTimes >= curAnimData.LoopTime)
//                        {
//                            Stop();
//                            m_nLoopTimes = 0;
//                            m_fLastLoopTime = 0.0f;
//							if (curAnimData.LoopOverAnimId != -1)
//                            {
//								Play(curAnimData.LoopOverAnimId);
//                            }
//                        }
//                    }
//                }
//            }
//            if (NeedRemoveAnim != null && NeedRemoveAnim.Count > 0)
//            {                
//                foreach (int animId in NeedRemoveAnim)
//                {
//                    if (m_NeedCheckEndCallBack.Contains(animId))
//                    {
//                        m_NeedCheckEndCallBack.Remove(animId);
//                    }
//                }
//            }
//        }

        public void AnimationUpdate()
        {
            if (m_NeedCheckEndCallBack != null && m_NeedCheckEndCallBack.Count > 0)
            {
                CheckForNeedCallBack();
            }
        }

        public bool Play(int animId)
        {
            Tab_Animation newAnimation = GetAnimInfoById(animId);
            if (newAnimation == null)
            {
                return false;
            }
            if (m_animation ==null)
            {
                return false;
            }
//  by dsy  安卓下卡贞的情况

//#if UNITY_ANDROID

//            if (IsOtherPlayer && OtherPlayer != null)
//            {
//                if (OtherPlayer != null)
//                {
//                    if (OtherPlayer.OtherPlayerMoveTag == string.Empty)
//                    {
//                        OtherPlayer.OtherPlayerMoveTag = newAnimation.AinmName;
//                    }
//                    else
//                    {
//                        if (OtherPlayer.OtherPlayerMoveTag == "Run" && newAnimation.AinmName == "Stand")
//                        {
//                            newAnimation = null;
//                            return true;
//                        }
//                    }
//                }
//            }
//#endif

            if (animStateCache == null)
            {
                LogModule.WarningLog("can't play animation, animationlogic shoud initstate before use it.");
                return false;
            }
            if (m_curAnimData != null && m_animation.IsPlaying(m_curAnimData.AinmName) && m_curAnimData.IsCanBreak == false)
            {
                return false;
            }
            if (m_animation.IsPlaying(newAnimation.AinmName) &&
                m_curAnimData != null &&
                m_curAnimData.AnimID == animId)
            {
                return false;
            }
          
            if (animStateCache.ContainsKey(animId))
            {
                m_curAnimData = animStateCache[animId];
            }
            else
            {
                m_curAnimData = GetAnimInfoById(animId);
                animStateCache.Add(animId, m_curAnimData);
            }
           
            if (m_curAnimData == null)
            {
                return false;
            }

			//===TODO  判断各职业 站立、战斗站立、死亡、跑时重置对应的挂点位置
			Obj_MainPlayer mainPlayer = this.gameObject.GetComponent<Obj_MainPlayer> ();
			if(mainPlayer != null)
			{
				string strWeaponPoint = "Model/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/HH_weaponHandRt";
				Transform weaponParent = this.gameObject.transform.FindChild(strWeaponPoint);
				Transform trans = weaponParent;
				if(trans != null)
				{
					if(	mainPlayer.Profession ==  (int)CharacterDefine.PROFESSION.DALI)
					{
						if(animId== (int)CharacterDefine.CharacterAnimId.Stand)
						{
							trans.localPosition = new Vector3(-0.0004440765f,0.0005200184f,-0.0003863251f);
							trans.localRotation = Quaternion.Euler( new Vector3(358.962f,321.4359f,89.58354f));
						}else if(animId== (int)CharacterDefine.CharacterAnimId.AttackStand)
						{
							trans.localPosition = new Vector3(-0.0008901468f,0.0005259991f,-2.934569e-05f);
							trans.localRotation = Quaternion.Euler(new Vector3(354.5297f,317.4298f,89.65583f));
						}else if(animId== (int)CharacterDefine.CharacterAnimId.Walk)
						{
							trans.localPosition = new Vector3(-0.0005550765f,0.0005200184f,-0.0003863251f);
							trans.localRotation = Quaternion.Euler(new Vector3(358.9578f,320.855f,89.59406f));
						}else if(animId== (int)CharacterDefine.CharacterAnimId.Die)
						{
							trans.localPosition = new Vector3(-0.0008901468f,0.0005259991f,-2.934569e-05f);
							trans.localRotation = Quaternion.Euler(new Vector3(355.0468f,317.4303f,89.65639f));
						}
					}
//					else if(mainPlayer.Profession ==  (int)CharacterDefine.PROFESSION.XIAOYAO)
//					{
//						if(animId== (int)CharacterDefine.CharacterAnimId.Stand)
//						{
//							trans.localPosition = new Vector3(0.002594349f,-0.001930365f,-0.001158958f);
//							trans.localRotation = Quaternion.Euler( new Vector3(6.915289f,132.4426f,3.42487f));
//						}else if(animId== (int)CharacterDefine.CharacterAnimId.AttackStand)
//						{
//							
//						}else if(animId== (int)CharacterDefine.CharacterAnimId.Walk)
//						{
//							trans.localPosition = new Vector3(0.001359341f,0.0004414589f,-0.001669887f);
//							trans.localRotation = Quaternion.Euler( new Vector3(308.7508f,109.3115f,32.15303f));
//						}else if(animId== (int)CharacterDefine.CharacterAnimId.Die)
//						{
//							trans.localPosition = new Vector3(0.002594349f,-0.001930365f,-0.001158958f);
//							trans.localRotation = Quaternion.Euler( new Vector3(6.915289f,132.4426f,3.42487f));
//						}
//					}
				}
			}
			//====  判断各职业 站立、战斗站立、死亡、跑时重置对应的挂点位置   end

            //如果为空,则尝试加载
            if (null == m_animation[m_curAnimData.AinmName])
            {
                //检查动作名和动作Obj
                if (m_curAnimData.AinmName.Length <= 0)
                {
                    return false;
                }
                //检查脚本
                if (m_animResFilePath.Length <= 0)
                {
                    return false;
                }
                string fullPath = string.Format("{0}/{1}", m_animResFilePath, m_curAnimData.AinmName);
                AnimationClip clip = ResourceManager.LoadResource(fullPath) as AnimationClip;
                //AnimationClip clip = (AnimationClip)ResourceManager.CreateUnityObject(fullPath, strAnim);
                if (null != clip)
                {
                    OnLoadAnimation(fullPath, clip);
                }
                else
                {
					//LogModule.DebugLog(fullPath);
                    LogModule.DebugLog("animation load fail");
                }
            }
            else
            {
               
                OnLoadAnimation(m_curAnimData.AinmName, m_animation.GetClip(m_curAnimData.AinmName));
            }

            return true;
        }

        //有衰减的音效播放，目前其他玩家和NPC的技能音效上.
        void PlaySoundAtPos(GameDefine_Globe.OBJ_TYPE ObjType, int nSoundID, Vector3 playingPos)
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
                    GameManager.gameManager.SoundManager.PlaySoundEffectAtPos(nSoundID, playingPos, Singleton<ObjManager>.Instance.MainPlayer.Position);
                }
                else
                {
                    GameManager.gameManager.SoundManager.PlaySoundEffect(nSoundID);
                }
            }
        }

        void OnLoadAnimation(string animPath, AnimationClip curAnimClip)
        {
            if (null == curAnimClip)
            {
                LogModule.ErrorLog("LoadAnimation Failed, ResName:" + animPath);
                return;
            }
            if (m_animation[m_curAnimData.AinmName] ==null)
            {
                //添加
                m_animation.AddClip(curAnimClip, m_curAnimData.AinmName);
            }
            //再次判断是否为空
            if (null == m_animation[m_curAnimData.AinmName])
            {
                LogModule.ErrorLog("animation is null " + animPath);
                return;
            }
			//开始记录伤害时间
			if (m_curAnimData.HurtTimes>1 )
			{
				if (m_curAnimState == null || m_curAnimState.name != m_curAnimData.AinmName)
				{
					hurttimes = 0;
					hurttime = Time.time;
				}
			}
			//第一次播放循环动作 记录下播放时间
			if (m_curAnimData.WrapMode == 2
			    && m_curAnimData.LoopTime > 0
			    )
            {
                if (m_curAnimState == null || m_curAnimState.name != m_curAnimData.AinmName)
                {
                    m_fLastLoopTime = Time.time;
                }
            }
            m_curAnimState = m_animation[m_curAnimData.AinmName];
            m_curAnimState.speed = m_curAnimData.SPEED;
            m_curAnimState.wrapMode = (WrapMode)m_curAnimData.WrapMode;
            m_curAnimState.layer = m_curAnimData.Layer;
            bool isNotifyFinish = m_curAnimData.IsCallEnd;
            float _animTransitTime = (m_curAnimState.length <= m_curAnimData.TransitTime ? 0 : m_curAnimData.TransitTime);
            if (_animTransitTime <= 0)
            {
                _animTransitTime = 0;
            }

			if(m_curAnimData.AnimID==2001)
			{
				int s=0;
			}
			isCallBack=true;

			//===
			if(m_ObjChar != null)
				m_ObjChar.AnimInfoNextAnimId = m_curAnimData.AnimID;

            m_animation.CrossFade(m_curAnimData.AinmName, _animTransitTime, PlayMode.StopAll);
            if (m_curAnimData.SoundID >= 0 && null != GameManager.gameManager.SoundManager)
            {
                if (m_ObjChar != null)
                {
                    PlaySoundAtPos(m_ObjChar.ObjType,m_curAnimData.SoundID,m_ObjChar.Position);
                }
                else
                {
                     GameManager.gameManager.SoundManager.PlaySoundEffect(m_curAnimData.SoundID);
                }
            }

            if (m_ObjEffectLogic != null)
            {
                for (int i = 0; i < m_curAnimData.getStartEffectCount(); i++)
                {
                    int StartEffectId = m_curAnimData.GetStartEffectbyIndex(i);
                    if (StartEffectId != -1)
                    {
                        if (DebugHelper.m_bShowEffect)
                            m_ObjEffectLogic.PlayEffect(StartEffectId);
                    }
                }

            }
            if (isNotifyFinish)
            {
                m_NeedCheckEndCallBack.AddFirst(m_curAnimData.AnimID);
            }

        }

        public void Stop()
        {
            if (m_curAnimData ==null)
            {
                return;
            }
            if (m_animation != null)
            {
                //击飞不能被打断
                if (m_curAnimData.AnimID ==(int)(CharacterDefine.CharacterAnimId.Knockback_02))
                {
                    return;
                }
				//击飞不能被打断
				if (m_curAnimData.AnimID ==(int)(CharacterDefine.CharacterAnimId.Dead))
				{
					return;
				}
                if (m_NeedCheckEndCallBack.Contains(m_curAnimData.AnimID))
                {
                    m_NeedCheckEndCallBack.Remove(m_curAnimData.AnimID);
                }
                //停止特效
                StopAnimEffect();
                //停止寒霜怒雪的音效
                if (m_curAnimData.SoundID == 85)   //sk5hsnx2_xiaoyao
                {
                    GameManager.gameManager.SoundManager.StopSoundEffect(m_curAnimData.SoundID);
                }
                if (m_ObjChar !=null)
                {
                    m_ObjChar.OnAnimationStop(m_curAnimData.AnimID);
                }
                //如果是中断循环动作  清除循环信息
                if (m_curAnimData.WrapMode == (int)WrapMode.Loop)
                {
                    m_fLastLoopTime = 0;
                    m_nLoopTimes = 0;
                }
                m_animation.Stop(m_curAnimData.AinmName);
            }
        }

        public WrapMode CurAnimWrapMode
        {
            set
            {
                if (null != m_curAnimState) m_curAnimState.wrapMode = value;
            }
            get
            {
                return ((m_curAnimState == null) ? WrapMode.Default : m_curAnimState.wrapMode);
            }
        }

      
        /*
        private bool CheckAnimation(Tab_Animation curTabAnimation, CheckAnimationDelegate defFun)
        {
            if (null == m_animation)
            {
                return false;
            }

            //如果为空,则尝试加载
            if (null == m_animation[curTabAnimation.AinmName])
            {
                LoadAnimation(curTabAnimation.AinmName);
            }

            //再次判断是否为空
            if (null == m_animation[curTabAnimation.AinmName])
            {
                return false;
            }

            return true;
        }
        */
        public Tab_Animation GetAnimInfoById(int AnimaId)
        {
            Tab_Animation animInfo = TableManager.GetAnimationByID(AnimaId, 0);
            return animInfo;

        }
       public void FinishAnimEffect(int finishedAnimID)
        {
            if (m_ObjEffectLogic == null)
            {
                return;
            }
            Tab_Animation curAnim = TableManager.GetAnimationByID(finishedAnimID, 0);
            if (curAnim == null)
            {
                LogModule.WarningLog("cur animation id is not exit " + finishedAnimID.ToString());
                return;
            }
            for (int i = 0; i < curAnim.getStartEffectCount(); i++)
            {
                int effectId = curAnim.GetStartEffectbyIndex(i);
                if (effectId != -1)
                {
                    if (curAnim.GetIsStartEffectAutoEndbyIndex(i) == false)
                    {
                        m_ObjEffectLogic.StopEffect(effectId);
                    }
                }

            }
            if (curAnim.EndEffect >= 0)
            {
				if (DebugHelper.m_bShowEffect)
                    m_ObjEffectLogic.PlayEffect(curAnim.EndEffect);
            }
        }

        public void StopAnimEffect()
        {
            if (m_ObjEffectLogic == null)
            {
                return;
            }
            if (m_curAnimData == null)
            {
                LogModule.WarningLog("cur animation id is not exit " + m_curAnimData.AnimID.ToString());
                return;
            }
            for (int i = 0; i < m_curAnimData.getStartEffectCount(); i++)
            {
                if (m_curAnimData.AnimID == 24)
                {
                    break;
                }
                int effectId = m_curAnimData.GetStartEffectbyIndex(i);
                if (effectId !=-1)
                {
                    m_ObjEffectLogic.StopEffect(effectId);
                }
            }
            if (m_curAnimData.EndEffect != -1)
            {
                m_ObjEffectLogic.StopEffect(m_curAnimData.EndEffect);
            }
        }
    }
}

