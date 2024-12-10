//********************************************************************
// 文件名: ObjAnimModel.cs
// 描述: 通用非obj类型动作
// 作者: HeWenpeng
// 创建时间: 2014-1-2
//
// 修改历史:
//********************************************************************

using UnityEngine;
using System.Collections;
using Games.Animation_Modle;
using Module.Log;
namespace Games.ObjAnimModule
{
    public class ObjAnimModel : MonoBehaviour
    {
        protected Animation m_ObjAnimation;
        public UnityEngine.Animation ObjAnimation
        {
            get { return m_ObjAnimation; }

        }

        protected AnimationLogic m_AnimLogic;
        public AnimationLogic AnimLogic
        {
            get { return m_AnimLogic; }
            set { m_AnimLogic = value; }
        }

        private string m_AnimationFilePath = "";
        public string AnimationFilePath
        {
            get { return m_AnimationFilePath; }
            set { m_AnimationFilePath = value; }
        }

        public void InitAnimation()
        {
            if (m_AnimLogic == null)
            {
              m_AnimLogic = gameObject.AddComponent<AnimationLogic>();
            }

            if (null == m_AnimLogic)
                return;

            //首先保存该ObjAction的路径
            m_AnimLogic.AnimResFilePath = m_AnimationFilePath;

            Transform modelTransform = gameObject.transform.FindChild("Model");
            if (modelTransform)
            {
                m_ObjAnimation = modelTransform.gameObject.GetComponent<Animation>();
            }

            if (m_ObjAnimation)
            {
                m_AnimLogic.InitState(m_ObjAnimation.gameObject);
            }
            else
            {
                LogModule.DebugLog("The Mount doesn't have animations. Moving her might look weird.");
            }
        }
    }
}

