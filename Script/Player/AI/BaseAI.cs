/********************************************************************************
 *	文件名：	BaseAI.cs
 *	全路径：	\Script\Player\BaseAI.cs
 *	创建人：	李嘉
 *	创建时间：2013-11-18
 *
 *	功能说明： 客户端AI基础类，提供接口
 *	          
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
using Games.LogicObj;
namespace Games.AI_Logic
{
    public class BaseAI : MonoBehaviour
    {
        //AI的具体类型
        protected CharacterDefine.AI_TYPE m_AIType;
        public CharacterDefine.AI_TYPE AIType
        {
            get { return m_AIType; }
            set { m_AIType = value; }
        }
		public Obj  m_target=null;
        public CharacterDefine.AI_STATE_TYPE AIStateType;
        
        public BaseAI()
        {
            m_AIType = CharacterDefine.AI_TYPE.AI_TYPE_INVALID;
        }

        //初始化AI
        void Awake()
        {
        }

        //将AI装载到OBJ的AIController中
        public void LoadAI()
        {
            AIController aiController = this.gameObject.GetComponent<AIController>();
            if (aiController)
            {
                aiController.AddAIByStateType(this);
            }
        }
		public void SetTarget(Obj  obj)
		{
			m_target = obj;
		}
        //销毁AI
        public virtual void Destroy()
        {
        }

        //激活AI
        public virtual void OnActive()
        {
        }

        //关闭AI
        public virtual void OnDeactive()
        {
        }

        //更新AI
        public virtual void UpdateAI()
        {
        }
    }
}
