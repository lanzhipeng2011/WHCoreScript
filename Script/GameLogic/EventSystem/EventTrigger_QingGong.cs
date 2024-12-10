/********************************************************************************
 *	文件名：	EventTrigger_QingGong.cs
 *	全路径：	\Script\Event\EventTrigger_QingGong.cs
 *	创建人：	李嘉
 *	创建时间：2014-03-26
 *
 *	功能说明：客户端轻功专用触发器
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Games.LogicObj;
using Games.GlobeDefine;

namespace Games.Events
{
    public class EventTrigger_QingGong : EventTrigger
    {
        public int m_ID;                //轻功点ID
        public float m_NextPosX;        //轻功目标点X
        public float m_NectPosY;        //轻功目标点Y
        public float m_NextPosZ;        //轻功目标点Z
        public float m_Speed;           //轻功移动速度
        public int m_State;             //轻功轨迹状态（QINGGONG_TRAIL_TYPE），0-抛物线 1-靠左倾斜沿墙壁 2-靠右倾斜沿墙壁
        public float m_TriggerRadius;   //触发事件半径
        public float m_MaxHeight;       //飞行过程中的最大高度（目前只有抛物线轨迹有效）
        public int nSoundEffectID = -1;   //音效

        private Transform m_PointTransform = null;
        private float m_fLastSysPosTime = 0.0f;
        void Awake()
        {
            m_PointTransform = gameObject.transform;
        }

        // Update is called once per frame
        void Update()
        {
            if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
            {
                return;
            }

            UpdateCoolDown();

            QingGongTrigger();
        }

        // 轻功玩家 触发事件
        void QingGongTrigger()
        {
            Dictionary<string, Obj> ObjPools = Singleton<ObjManager>.GetInstance().ObjPools;
            foreach (KeyValuePair<string, Obj> objPair in ObjPools)
            {
                if (objPair.Value.ObjType != GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER
                    && objPair.Value.ObjType != GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER)
                {
                    continue;
                }
                if (Vector3.Distance(m_PointTransform.position, objPair.Value.ObjTransform.position) < m_TriggerRadius)
                {
                    m_TriggerEvent.Reset();
                    m_TriggerEvent.EventID = GlobeDefine.GameDefine_Globe.EVENT_DEFINE.EVENT_QINGGONG;

                    //初始化轻功数值
                    m_TriggerEvent.AddFloatParam(m_NextPosX);
                    m_TriggerEvent.AddFloatParam(m_NectPosY);
                    m_TriggerEvent.AddFloatParam(m_NextPosZ);
                    m_TriggerEvent.AddFloatParam(m_Speed);
                    m_TriggerEvent.AddFloatParam(m_MaxHeight);
                    m_TriggerEvent.AddIntParam(m_State);
                    m_TriggerEvent.AddIntParam(m_ID);
                    m_TriggerEvent.AddIntParam(objPair.Value.ServerID);
                    TriggerEvent();

                    if (nSoundEffectID >= 0 && null != GameManager.gameManager.SoundManager)
                    {
                        GameManager.gameManager.SoundManager.PlaySoundEffect(nSoundEffectID);
                    }

                    //如果是主角，则强制广播两次，一起轻功起始点，一次结束点
                    if (objPair.Value.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER &&
                        Time.time -m_fLastSysPosTime >2.0f)//同一个轻功点的同步 设置一个间隔
                    {
                        //起始点
                        Singleton<ObjManager>.GetInstance().MainPlayer.SycQingGongPos(m_PointTransform.position);
                        //触发了轻功点 清除掉需要自动走向的轻功点
                        Singleton<ObjManager>.GetInstance().MainPlayer.AutoMovetoQGPointId = -1;
                        //停止自动寻路
                        if (null != GameManager.gameManager.AutoSearch &&
                            GameManager.gameManager.AutoSearch.IsAutoSearching)
                        {
                            GameManager.gameManager.AutoSearch.Stop();
                        }
                       
                        //结束点
                        Vector3 endPoint = new Vector3(m_NextPosX, 0, m_NextPosZ);
                        Singleton<ObjManager>.GetInstance().MainPlayer.SycQingGongPos(endPoint);
                        m_fLastSysPosTime = Time.time;
                    }
                }
            }
        }
    }
}
