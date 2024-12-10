/********************************************************************************
 *	文件名：	AIController.cs
 *	全路径：	\Script\Player\AIController.cs
 *	创建人：	李嘉
 *	创建时间：2013-11-18
 *
 *	功能说明：  客户端AI控制器，负责Obj的AI装载
 *	           AI分为三种AI列表进行管理，分别是平时AI，战斗AI和死亡AI，通过特殊接口触发
 *	           三种AI的切换接口为：
 *	           初始化:Normal.AI
 *	           进入战斗:Normal.AI->Combat.AI
 *	           离开战斗:Combat.AI->Normal.AI
 *	           死亡：Combat.AI or Normal.AI->Dead.AI
 *	           复活:Dead.AI->Normal.AI	          
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Games.GlobeDefine;
using Games.LogicObj;
namespace Games.AI_Logic
{
    public class AIController : MonoBehaviour
    {
        //NPC非战斗AI
        private BaseAI m_NormalAI = null;
        public BaseAI NormalAI
        {
            get { return m_NormalAI; }
            set
            {
                if (null != m_NormalAI)
                {
                    m_NormalAI.Destroy();
                }

                m_NormalAI = value;
            }
        }
		//剧情AI
		private BaseAI m_JuQingAI = null;
		public BaseAI JuQingAI
		{
			get { return m_JuQingAI; }
			set
			{
				if (null != m_JuQingAI)
				{
					m_JuQingAI.Destroy();
				}
				
				m_JuQingAI = value;
			}
		}
        //NPC战斗AI
        private BaseAI m_CombatAI = null;
        public BaseAI CombatAI
        {
            get { return m_CombatAI; }
            set
            {
                if (null != m_CombatAI)
                {
                    m_CombatAI.Destroy();
                }

                m_CombatAI = value;
            }
        }

        //NPC死亡AI
        private BaseAI m_DeadAI = null;
        public BaseAI DeadAI
        {
            get { return m_DeadAI; }
            set
            {
                if (null != m_DeadAI)
                {
                    m_DeadAI.Destroy();
                }

                m_DeadAI = value;
            }
        }

        //当前NPC状态
        private BaseAI m_CurrentAIState = null;
        public BaseAI CurrentAIState
        {
            get { return m_CurrentAIState; }
            set { m_CurrentAIState = value; }
        }

        //标记位
        //private bool m_bAliveFlag = true;       //是否死亡标记位
        private bool m_bCombatFlag = false;     //是否战斗标记位
        public bool CombatFlag
        {
            get { return m_bCombatFlag; }
            set { m_bCombatFlag = value; }
        }
        //private bool m_bRestFlag = false;       //是否正在进行复位操作

        //更新间隔
        private float m_fLastUpdateTime = 0.0f;
        public static float m_fUpdateInterval = 0.5f;   //每次AI的更新间隔（秒）

        //////////////////////////////////////////////////////////////////////////
        //战斗AI相关
        //////////////////////////////////////////////////////////////////////////
        private Threat m_ThreadInfo;
        public Threat ThreadInfo
        {
            get { return m_ThreadInfo; }
            set { m_ThreadInfo = value; }
        }

        //初始化AI Controller操作
        void Awake()
        {
            //m_bAliveFlag = true;
            m_bCombatFlag = false;
            //m_bRestFlag = false;
            if (null == ThreadInfo)
            {
                ThreadInfo = new Threat();
            }
        }
        
        void FixedUpdate()
        {
            //是否达到更新间隔
            if (Time.time - m_fLastUpdateTime >= m_fUpdateInterval)
            {
                m_fLastUpdateTime = Time.time;
            }
            else
            {
                return;
            }

            if (null == m_CurrentAIState)
            {
                m_CurrentAIState = m_NormalAI;
            }

            if (null != m_CurrentAIState)
            {
                m_CurrentAIState.UpdateAI();
            }
        }

        //切换AI
        public void SwitchCurrentAI(BaseAI ai)
        {
            if (null != m_CurrentAIState)
            {
                m_CurrentAIState.OnDeactive();
            }

            m_CurrentAIState = ai;
            if (null != m_CurrentAIState)
            {
                //重置Threat
                if (m_CurrentAIState.AIStateType != CharacterDefine.AI_STATE_TYPE.AI_STATE_COMBAT)
                {
                    m_ThreadInfo.ResetAllThreat();
                }

                m_CurrentAIState.OnActive();
            }
        }

        //初始化
        //进入战斗
        public void EnterCombat()
        {
            m_bCombatFlag = true;
            SwitchCurrentAI(m_CombatAI);
        }
		public void EnterJuQing()
		{
			SwitchCurrentAI(m_JuQingAI);
		}
		//离开剧情
		public void LeaveJuQing()
		{

			SwitchCurrentAI(m_NormalAI);
		}
        //离开战斗
        public void LeaveCombat()
        {
            m_bCombatFlag = false;
            SwitchCurrentAI(m_NormalAI);
        }

        //复活
        public void OnAlive()
        {
            m_bCombatFlag = false;
            //m_bAliveFlag = true;
            SwitchCurrentAI(m_NormalAI);
        }

        //死亡
        public void OnDie()
        {
            m_bCombatFlag = false;
            //m_bAliveFlag = false;
            SwitchCurrentAI(m_DeadAI);
        }

        //Obj复位操作
        public void OnRest(bool bFlag)
        {
            //m_bRestFlag = bFlag;
            m_ThreadInfo.ResetAllThreat();
        }

        //根据AI类型添加
        public bool AddAIByStateType(BaseAI ai)
        {
            switch (ai.AIStateType)
            {
                case CharacterDefine.AI_STATE_TYPE.AI_STATE_NORMAL:
                    {
                        NormalAI = ai;
                        return true;
                    }
                case CharacterDefine.AI_STATE_TYPE.AI_STATE_COMBAT:
                    {
                        CombatAI = ai;
                        return true;
                    }
                case CharacterDefine.AI_STATE_TYPE.AI_STATE_DEAD:
                    {
                        DeadAI = ai;
                        return true;
                    }
			    case CharacterDefine.AI_STATE_TYPE.AI_STATE_JUQING:
			       {
				       JuQingAI = ai;
				       return true;
			       }
                default:
                    break;
            }

            return false;
        }
    }
}
