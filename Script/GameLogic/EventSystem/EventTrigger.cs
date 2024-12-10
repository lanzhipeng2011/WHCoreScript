/********************************************************************************
 *	文件名：	EventTrigger.cs
 *	全路径：	\Script\Event\EventTrigger.cs
 *	创建人：	李嘉
 *	创建时间：2014-03-26
 *
 *	功能说明：事件触发器，在Update中检测条件，符合条件则触发某个事件
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using Games.GlobeDefine;

namespace Games.Events
{
    public class EventTrigger : MonoBehaviour
    {
        protected GameEvent m_TriggerEvent = new GameEvent();         //触发的事件

        protected bool m_bCanTrigger = true;                          //是否可以触发
        public bool CanTrigger
        {
            get { return m_bCanTrigger; }
            set { m_bCanTrigger = value; }
        }

        public float m_CoolDown = GlobeVar.INVALID_ID;
        private float m_fLastTriggerTime = 0.0f;

        // Update is called once per frame
        void FixedUpdate()
        {
            if (m_TriggerEvent.EventID == GlobeDefine.GameDefine_Globe.EVENT_DEFINE.EVENT_INVALID)
            {
                return;
            }

            UpdateCoolDown();
        }

        protected void UpdateCoolDown()
        {
            if (CanTrigger == false && m_CoolDown > 0)
            {
                if (Time.time - m_fLastTriggerTime > m_CoolDown)
                {
                    CanTrigger = true;
                }
            }
        }

        //触发事件
        protected virtual void TriggerEvent()
        {
            if (m_TriggerEvent.EventID == GlobeDefine.GameDefine_Globe.EVENT_DEFINE.EVENT_INVALID)
            {
                return;
            }

            //可以被触发并且事件ID合法
            if (CanTrigger)
            {
                Singleton<EventSystem>.GetInstance().PushEvent(m_TriggerEvent);
                //如果有CD，则置成不可触发，并记录触发时间
                if (m_CoolDown > 0)
                {
                    CanTrigger = false;
                    m_fLastTriggerTime = Time.time;
                }
            }
        }
    }
}
