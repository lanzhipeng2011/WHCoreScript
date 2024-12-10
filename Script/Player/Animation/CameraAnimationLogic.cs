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
public class CameraAnimationLogic : MonoBehaviour
{
		private Animation m_animation;
		private GameObject m_animGameObj ;
		public UnityEngine.Animation Animation
		{
			get { return m_animation; }
			
		}
		private string m_animResFilePath = "Animation/camera";       //动画资源路径
		public string AnimResFilePath
		{
			get { return m_animResFilePath; }
			set { m_animResFilePath = value; }
		}
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
		void Start()
		{
			InitState (this.gameObject);
			//Play (1200);
		}
		public void InitState(GameObject animObj)
		{
			if (null == animObj)
				return;
			
			Transform animParent = animObj.transform.parent;
			m_animation = animObj.animation;
		//	if (null != animParent)
			{

				animStateCache = new Dictionary<int, Tab_Animation>();
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
					LogModule.DebugLog("animation load fail");
				}
			}
			else
			{
				
				OnLoadAnimation(m_curAnimData.AinmName, m_animation.GetClip(m_curAnimData.AinmName));
			}
			
			return true;
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

			m_animation.CrossFade(m_curAnimData.AinmName, _animTransitTime, PlayMode.StopAll);
			if (m_curAnimData.SoundID >= 0 && null != GameManager.gameManager.SoundManager)
			{

				{
					GameManager.gameManager.SoundManager.PlaySoundEffect(m_curAnimData.SoundID);
				}
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
		public Tab_Animation GetAnimInfoById(int AnimaId)
		{
			Tab_Animation animInfo = TableManager.GetAnimationByID(AnimaId, 0);
			return animInfo;
			
		}
	}
}

