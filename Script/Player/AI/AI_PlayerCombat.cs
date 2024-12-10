/********************************************************************************
 *	文件名：	AI_PlayCombat.cs
 *	全路径：	\Script\Player\AI\AI_PlayCombat.cs
 *	创建时间：2013-12-4
 *
 *	功能说明：主角战斗AI
 *	修改记录：
*********************************************************************************/

using System.Collections.Generic;
using Games.SkillModle;
using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
using Games.LogicObj;
using GCGame.Table;
namespace Games.AI_Logic
{
    class AI_PlayerCombat : AI_BaseCombat
    {
        public AI_PlayerCombat()
        {
            for (int i = 0; i < (int)SKILLDEFINE.MAX_SKILLNUM; i++)
            {
                m_canSeleSkill[i] = new OwnSkillData();
                m_canSeleSkill[i].CleanUp();
            }
        }
        Obj_MainPlayer m_player = null;
        private int m_nLastUseSkill =0;
	
		private Tab_SceneClass _sceneClassInfo; 
		private bool m_isUseXp=true;
        void Start()
        {
            //装载AI到AIController，进行统一管理
            LoadAI();
			_sceneClassInfo= TableManager.GetSceneClassByID(GameManager.gameManager.RunningScene, 0);
			if (_sceneClassInfo != null)
			{
				if(_sceneClassInfo.IsCanUseXp ==1)
				{
					m_isUseXp=true;
				}
				else
				{
					m_isUseXp=false;
				}
			}
            m_player =gameObject.GetComponent<Obj_MainPlayer>();
            if (m_player)
            {

              //如果已经设定了自动挂机 则开启自动挂机
                if (m_player.IsOpenAutoCombat)
                {
                    if (m_player.Controller)
                    {
                        //先中断处理
                        m_player.BreakAutoCombatState();
                        m_player.Controller.EnterCombat();
                    }
                   
                }
            }
        }
        private OwnSkillData[] m_canSeleSkill = new OwnSkillData[(int)SKILLDEFINE.MAX_SKILLNUM];
        protected float m_fLastUseEndTime = 0.0f;
        //激活AI
        public override void OnActive()
        {
            base.OnActive();
            UpdateAI();
        }
        public override void UpdateAI()
        {
            base.UpdateAI();
            if (m_player == null || m_player.Controller == null)
            {
                return;
            }
            if (m_player.IsDie())
            {
                return;
            }
            if (m_player.Controller.CombatFlag == false)
            {
                return;
            }
            if (m_player.AutoComabat == false )
            {
                return;
            }
            //轻功状态下 不挂机
            if (m_player.QingGongState)
            {
                return ;
            }
            if (m_player.AcceleratedMotion != null && m_player.AcceleratedMotion.Going == true)
            {
                return ;
            }
            //剧情播放中不挂机
            if (m_player.IsInModelStory)
            {
                return ;
            }
             if (m_player.SkillCore.IsUsingSkill)
            {
                if (m_player.SkillCore.UsingSkillBaseInfo != null)
                {
                    //正在使用祝融掌 则检测下当前目标是否还存在 不存在则重新选取目标
                    if (m_player.SkillCore.UsingSkillBaseInfo.Id ==(int)SKILLBASEID.ZLZ)
                    {
                        if (m_player.SelectTarget ==null || m_player.SelectTarget.IsDie())
                        {
                            Obj_Character _NewAttackCharacter = GetCanAttackTar();
                            if (_NewAttackCharacter != null)
                            {
                                //设置新的选中目标
                                m_player.OnSelectTarget(_NewAttackCharacter.gameObject,false);
                                m_player.MoveTo(_NewAttackCharacter.transform.position, _NewAttackCharacter.gameObject,1.0f);
                            }
                        } 
                    }
                    
                    if ((m_player.SkillCore.UsingSkillBaseInfo.SkillClass&(int)SKILLCLASS.AUTOREPEAT)==0)
                    {
                        return;
                    }
					return ;
                }
            }
            else
            {
                if (m_fLastUseEndTime <=0.1f)
                {
                    m_fLastUseEndTime = Time.time;
                }
            }
//            //留个缓冲时间
//            if (Time.time -m_fLastUseEndTime <0.1f)
//            {
//                return;
//            }
            //有轻功点了出现了 向轻功点移动
            if (m_player.AutoMovetoQGPointId !=-1)
            {
                //打断挂机状态
                m_player.BreakAutoCombatState();
                if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_HUSONGMEIREN)
                {
                    //燕子坞轻功点移动
                    m_player.AutoFightFlyInYanZiWu();
                }
                m_player.AutoMovetoQGPointId = -1;
                return;
            }
            m_fLastUseEndTime = 0.0f;
            int skillId = SeleSkill();
            if (skillId == -1)
            {
                return;
            }
            Tab_SkillEx skillExInfo = TableManager.GetSkillExByID(skillId, 0);
            if (skillExInfo == null)
            {
                return;
            }
            Tab_SkillBase skillBaseInfo = TableManager.GetSkillBaseByID(skillExInfo.BaseId, 0);
            if (skillBaseInfo ==null)
            {
                return;
            }
			Obj_Character CanAttackCharacter = null;
			if (m_player.SelectTarget == null) 
			{
				CanAttackCharacter = GetCanAttackTar ();
			}
			else 
			{
				if(Reputation.IsEnemy(m_player.SelectTarget)||Reputation.IsNeutral(m_player.SelectTarget))//解决在Npc附近挂机也放技能的问题
			    	CanAttackCharacter=m_player.SelectTarget;	
			}
            //是否有攻击目标
            if (CanAttackCharacter == null)
            {
                return;
            }
            //设置为选中目标
            m_player.OnSelectTarget(CanAttackCharacter.gameObject, false);
            //距离不过 先跑过去
            float skillRadius = skillExInfo.Radius;
            float dis = Vector3.Distance(m_player.Position, CanAttackCharacter.Position);
          
			Tab_RoleBaseAttr  role=TableManager.GetRoleBaseAttrByID(
				CanAttackCharacter.BaseAttr.RoleBaseID,0);
			float roleradio=0.0f;
			if(role!=null)
				roleradio=role.SelectRadius;

			float diffDistance = dis - skillRadius-roleradio;
            m_player.CurUseSkillId = skillId;
            m_nLastUseSkill = skillId;
            //需要向目标移动 则想目标移动
			if (diffDistance > 0 && skillBaseInfo.IsMoveToTarInAutoCombat ==1)
            {
                //move
                if (m_player.IsMoving == false && m_player.IsCanOperate_Move())
                {
                   // m_player.MoveTo(CanAttackCharacter.Position, CanAttackCharacter.gameObject, skillRadius - 1.0f);


//					if(m_player.Profession==(int)CharacterDefine.PROFESSION.XIAOYAO||skillExInfo.CheckTime<=0)
//					{
//						if(diffDistance>=0)
//						{
//							m_player.MoveTo(CanAttackCharacter.Position, CanAttackCharacter.gameObject,skillRadius +roleradio- 0.5f);
//							
//							return ;
//						}
//					}
//					else
					{
//						if(diffDistance>=3)
//						{
//							m_player.MoveTo(CanAttackCharacter.Position, CanAttackCharacter.gameObject,  skillRadius +roleradio- 0.5f+3.0f);
//							
//							return ;
//						}
						if (diffDistance >0)
						{
							
							m_player.FaceTo(CanAttackCharacter.Position);
							m_player.MoveTo(CanAttackCharacter.Position, CanAttackCharacter.gameObject,  skillRadius +roleradio- 0.5f);
							//AttackFly(10,5,1.0f);
							//m_player.AttackPGCF(CanAttackCharacter);
							//MoveTo(Target.Position, Target.gameObject, skillRadius +roleradio- 0.5f);
						}
						return;

					}
				
					return;	
				
               
                }
				return ;
            }

            m_player.UseSkillOpt(skillId,null);
        }

        public Obj_Character GetCanAttackTar()
        {
            //8码范围内 有可攻击的目标 
            Dictionary<string, Obj> targets = Singleton<ObjManager>.GetInstance().ObjPools;
            
            float fMinDis = 8.0f;
            if (GameManager.gameManager.ActiveScene.IsCopyScene())                
            {
                fMinDis = 100.0f;
            }
            Obj_Character selCharacter = null;
            foreach (Obj targetObj in targets.Values)
            {
                if (targetObj != null )
                {
                    Obj_Character targeObjCharacter= targetObj as Obj_Character;
                    if (targeObjCharacter == null )
                    {
                        continue;
                    }
                    if (targeObjCharacter.IsDie())
                    {
                        continue;
                    }
                    if (Reputation.IsEnemy(targeObjCharacter) ==false &&
                        Reputation.IsNeutral(targeObjCharacter) ==false)
                    {
                        continue;
                    }
                    float distance = Vector3.Distance(m_player.Position, targeObjCharacter.Position);
                    //选择最近的一个目标
                    if (distance < fMinDis)
                    {
                        fMinDis = distance;
                        selCharacter = targeObjCharacter;
                    }
                }
            }
//            if (selCharacter == null)
//            {
//                if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_ZHENLONGQIJU)
//                {
//                    Vector3 pos = new Vector3(18, m_player.gameObject.transform.position.y, 16);
//                    m_player.MoveTo(pos, null, 0.0f);
//
//                }
//                if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_YANGWANGGUMU)
//                {
//                    Vector3 pos = new Vector3(20, m_player.gameObject.transform.position.y, 21);
//                    m_player.MoveTo(pos, null, 0.0f);
//                }
//
//                if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_YANZIWU)
//                {
//                    Singleton<ObjManager>.Instance.MainPlayer.AutoFightInYanziwu();
//                }
//                
//            }
            return selCharacter;
        }

        int SeleSkill()
        {

			for (int i = 0; i < (int)SKILLDEFINE.MAX_SKILLNUM; i++)
			{
				m_canSeleSkill[i].CleanUp();
			}
			//默认使用普攻
			int UseSkillId = m_player.OwnSkillInfo[0].SkillId;
			int nLastSkillIndex = -1;
			for (int i = 0; i < m_player.OwnSkillInfo.Length; i++)
			{
				if (m_player.OwnSkillInfo[i].SkillId == m_nLastUseSkill)
				{
					nLastSkillIndex =i;
					break;
				}
			}
			if (nLastSkillIndex !=-1)
			{
				int validSelIndex = 0;
				//筛选出可以用的技能
				for (int skillIndex = nLastSkillIndex; skillIndex < m_player.OwnSkillInfo.Length; skillIndex++)
				{
					int skillId = m_player.OwnSkillInfo[skillIndex].SkillId;
					if (skillId !=m_nLastUseSkill && skillId!=-1)
					{
						//XP 检验 能量
						Tab_SkillEx _skillEx = TableManager.GetSkillExByID(skillId, 0);
						if (_skillEx != null)
						{
							Tab_SkillBase _skillBase = TableManager.GetSkillBaseByID(_skillEx.BaseId, 0);
							if (_skillBase != null)
							{
								if ((_skillBase.SkillClass & (int) SKILLCLASS.XP) != 0 &&
								    m_player.BaseAttr.XP < m_player.BaseAttr.MaxXP)
								{
									//XP能量不足 无法释放
									continue;
								}
								if ((_skillBase.SkillClass & (int) SKILLCLASS.XP) != 0 &&
								   ( m_isUseXp==false))
								{
									//XP能量不足 无法释放
									continue;
								}
								if (m_player.OwnSkillInfo[skillIndex].CDTime <= 0 &&
								    m_player.OwnSkillInfo[skillIndex].PriorityAutoCombat != -1&& m_player.OwnAutoSkillInfo[skillIndex].CanAutoCombat) //-1表示 挂机中不可以使用
								{
									m_canSeleSkill[validSelIndex++] = m_player.OwnSkillInfo[skillIndex];
								}
							}
						}
					}
				}
				//从可选技能中挑出优先级最高的那个
				int nMaxPriority = -1;
				for (int nIndex = 0; nIndex < m_canSeleSkill.Length; nIndex++)
				{
					if (m_canSeleSkill[nIndex].IsValid())
					{
						if (m_canSeleSkill[nIndex].PriorityAutoCombat >nMaxPriority)
						{
							nMaxPriority = m_canSeleSkill[nIndex].PriorityAutoCombat;
							UseSkillId = m_canSeleSkill[nIndex].SkillId;
						}
					}
				}
			}
			return UseSkillId;
        }
        
    }
}
