/********************************************************************************
 *	文件名：	AI_NpcCombat.cs
 *	全路径：	\Script\Player\AI\AI_NpcCombat.cs
 *	创建时间：2013-12-4
 *
 *	功能说明：Npc战斗AI
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
using Games.LogicObj;

namespace Games.AI_Logic
{
    class AI_NpcCombat : AI_BaseCombat
    {
        public AI_NpcCombat()
        {
           
        }
        //private AIController m_BindNPCAIController = null;
        //private GameObject m_AttackObj = null;          //当前更新目标 

//        void Start()
//        {
//            //装载AI到AIController，进行统一管理
////             LoadAI();
////            
////             m_CombatNpc = this.gameObject.GetComponent<Obj_NPC>();
//        }

        public override void UpdateAI()
        {
//             base.UpdateAI();
//            
//             if (m_CombatNpc == null || m_CombatNpc.Controller ==null)
//             {
//                 return;
//             }
//           
//             if (m_CombatNpc.IsDie())
//             {
//                 return;
//             }
// //             GameObject firstThreatObj = m_BindNPCAIController.ThreadInfo.FindMaxThreatObj();
// //             if (firstThreatObj != m_AttackObj)
// //             {
// //                 m_AttackObj = firstThreatObj;
// //             }
// // 
// //             if (null != firstThreatObj)
// //             {
// //                 
// //             }
//             if (m_CombatNpc.Controller.CombatFlag == false)
//             {
//                 return;
//             }
//             Obj_MainPlayer MainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
//             if (MainPlayer ==null)
//             {
//                 return;
//             }
//             if (Reputation.IsFriend(m_CombatNpc))
//             {
//                 return;
//             }
//            
//             Vector3 BornPos = new Vector3(m_CombatNpc.BornPosX, m_CombatNpc.BornPosY, m_CombatNpc.BornPosZ);
//             Vector3 targetPos = new Vector3(m_CombatNpc.Position.x, m_CombatNpc.Position.y, m_CombatNpc.Position.z);
//             float Dis = Mathf.Abs(Vector3.Distance(BornPos, targetPos));
//             float PathRadius = m_CombatNpc.BaseAttr.PathRadius;
//             if (PathRadius > 0 && Dis - PathRadius > 0)//玩家远离 脱战
//             {
//                 m_CombatNpc.OnLevelCombat(null);
//                 Vector3 BornVec3 = new Vector3(m_CombatNpc.BornPosX, m_CombatNpc.BornPosY, m_CombatNpc.BornPosZ);
//                 m_CombatNpc.FaceTo(BornVec3);
//                 m_CombatNpc.MoveTo(BornVec3,null,0);
//               
//                 return;
//             }
//             //距离不够 先走过去
//        
//             float DiffDis = Mathf.Abs(Vector3.Distance(MainPlayer.Position, m_CombatNpc.Position));
// 
//             if (DiffDis-m_CombatNpc.BaseAttr.AttrRadius >0 &&  m_CombatNpc.IsTracing ==false )
//             {
//                 m_CombatNpc.ProcessTrace(MainPlayer);
//                 return;
// 
//             }
// 
//             if (DiffDis > m_CombatNpc.BaseAttr.AttrRadius )
//             {
//                 return;
//             } 
//             Threat NpcThreadInfo =m_CombatNpc.Controller.ThreadInfo;
//             if(NpcThreadInfo ==null)
//             {
//                 return;
//             }
// 			if(NpcThreadInfo.FindMaxThreatObj() ==null)
// 			{
// 			//	return;
// 			}
// 			//Obj MainPlayer = NpcThreadInfo.FindMaxThreatObj().GetComponent<Obj>();
// 
//             
//        //     m_CombatNpc.FaceTo(MainPlayer.Position);
//             
//             if (m_CombatNpc != null && MainPlayer !=null)
//             {
// 
//                 
//                  if (Time.time-m_UpdateTime < m_AttrSpeed)
//                 {
//                      return;
//                  }
//                 m_UpdateTime = Time.time;
//               
//                 //播放攻击动画
//                bool bAttack = m_CombatNpc.AnimLogic.Play((int)(CharacterDefine.CharacterAnimId.Attack));
//                 if (bAttack ==false) //攻击动作没做出来 不出伤害
//                 {
//                     return;
//                 }
//               
//          /*       MainPlayer.Objanimation.CrossFade("Skill_01");*/
// //                MainPlayer.AnimLogic.Play((int)(CharacterDefine.CharacterAnimId.Hit));
//                
//                
// 
//             }
        }

        void SeleSkill()
        {
            
        }
    }
}
