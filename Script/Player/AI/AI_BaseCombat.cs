/********************************************************************************
 *	文件名：	AI_BaseCombat.cs
 *	全路径：	\Script\Player\AI\AI_BaseCombat.cs
 *	创建人：	李嘉
 *	创建时间：2013-11-18
 *
 *	功能说明： 客户端战斗AI
 *	          
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
using Games.LogicObj;

namespace Games.AI_Logic
{
    public class AI_BaseCombat : BaseAI
    {
        public AI_BaseCombat()
        {
            m_AIType = CharacterDefine.AI_TYPE.AI_TYPE_COMBAT;
            AIStateType = CharacterDefine.AI_STATE_TYPE.AI_STATE_COMBAT;
        }
      

        //void Start()
        //{
          
        //}

        public override void UpdateAI()
        {
            base.UpdateAI();
           
           

            
        }
    }
}
