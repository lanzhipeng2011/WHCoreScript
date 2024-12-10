/********************************************************************************
 *	文件名：	Event.cs
 *	全路径：	\Script\Event\EventSystem.cs
 *	创建人：	李嘉
 *	创建时间：2013-11-01
 *
 *	功能说明：事件处理函数对外接口
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Games.GlobeDefine;
using Games.LogicObj;

namespace Games.Events
{
    public class EventSystem : Singleton<EventSystem>
    {
        public EventSystem()
        {
            m_DelayEventQueue = new List<GameEvent>();
        }
        //消息队列
        List<GameEvent> m_DelayEventQueue;

        public bool Init()
        {
            return true;
        }

        //外部调用发送事件接口
        public void PushEvent(GameEvent _event)
        {
            if (_event.IsDelay)
            {
                //延迟时间，加入队列，等待处理
                m_DelayEventQueue.Add(_event);
            }
            else
            {
                //非延迟事件，马上处理
                _ProcessEvent(_event);
            }
        }

        public void UpdateDelayEventQueue()
        {
            for (int i = 0; i < m_DelayEventQueue.Count; ++i )
            {
                //延迟事件，等待延迟结束处理
                if (m_DelayEventQueue[i].IsDelayFinish())
                {
                    _ProcessEvent(m_DelayEventQueue[i]);
                }                
            }
        }

        //实际事件处理接口
        void _ProcessEvent(GameEvent _event)
        {
            //首先弹出这个Events
            m_DelayEventQueue.Remove(_event);
            switch (_event.EventID)
            {
                case GameDefine_Globe.EVENT_DEFINE.EVENT_ENTERGAME:
                    {
                        //举个例子，之后处理函数过多的时候可以考虑封装
                        int nLevel = _event.GetIntParam(0);
                        LoadingWindow.LoadScene((GameDefine_Globe.SCENE_DEFINE)nLevel);
  
                    }
                    break;
                case GameDefine_Globe.EVENT_DEFINE.EVENT_MISSION_COLLECTITEM:
                    {
                        //int nMissionID = _event.GetIntParam(0);
                        GameManager.gameManager.MissionManager.MissionCollectItem();
                    }
                    break;
                case GameDefine_Globe.EVENT_DEFINE.EVENT_CHANGESCENE:
                    {
                        int nLevel = _event.GetIntParam(0);
                        if (Singleton<ObjManager>.Instance.MainPlayer != null)
                        {
                            Singleton<ObjManager>.Instance.MainPlayer.OnPlayerLeaveScene();
                        }
                        LoadingWindow.LoadScene((GameDefine_Globe.SCENE_DEFINE)nLevel);

                        //处理消息包标记位
                        //NetWorkLogic.GetMe().CanProcessPacket = false;
                        //Application.LoadLevel(nLevel);
                    }
                    break;
                case GameDefine_Globe.EVENT_DEFINE.EVENT_QINGGONG:
                    {
//                         if (Singleton<ObjManager>.Instance.MainPlayer != null)
//                         {
//                             Singleton<ObjManager>.Instance.MainPlayer.BeginQingGong(_event);
//                         }
                        int nServerID = _event.GetIntParam(2);
                        Obj obj = Singleton<ObjManager>.Instance.FindObjInScene(nServerID);
                        if (obj)
                        {
                            if (obj.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER
                                || obj.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER)
                            {
                                Obj_OtherPlayer OtherPlayer = obj as Obj_OtherPlayer;
                                if (OtherPlayer)
                                {
                                    OtherPlayer.BeginQingGong(_event);
                                }
                            }
                        }
                    }
                    break;
                case GameDefine_Globe.EVENT_DEFINE.EVENT_COLLECTITEM_RESEED:
                    {
                        int nSceneID = _event.GetIntParam(0);
                        int nSceneIndex = _event.GetIntParam(1);
                        int nItemIndex = _event.GetIntParam(2);
                        if (Singleton<CollectItem>.GetInstance() != null)
                        {
                            Singleton<CollectItem>.GetInstance().ReSeedItems(nSceneID, nSceneIndex, nItemIndex);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
