/********************************************************************************
游戏场景逻辑，
负责本场景内的场景相关逻辑
每一个场景挂接一个，这样就保证在场景切换完成之后可以进行操作
*********************************************************************************/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Games.GlobeDefine;
using GCGame.Table;
using Module.Log;
using GCGame;
using Games.LogicObj;
namespace Games.Scene
{
    public enum Scene_Const_Value
    {
        MAX_SCENE_MUSIC_NUM = 3,
    }

    public class SceneLogic : MonoBehaviour
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        public void SetCameraMain()
        {
            if (Camera.main == null) return;

            float m_CameraXOffset = 7.8f;            //摄像机相对主角的X偏移
            float m_CameraYOffset = 8.0f;           //摄像机相对主角的Y偏移
            float m_CameraZOffset = -9.0f;          //摄像机相对主角的Z偏移
            float m_CameraXOffsetMax = 7.8f;            //摄像机相对主角的X偏移
            float m_CameraYOffsetMax = 8.0f;           //摄像机相对主角的Y偏移
            float m_CameraZOffsetMax = -9.0f;          //摄像机相对主角的Z偏移
            float m_CameraXOffsetMin = 4.0f;
            float m_CameraYOffsetMin = 4.0f;
            float m_CameraZOffsetMin = -5.0f;
            float m_Scale = 1.0f;
            float m_pinchSpeed = 100.0f;

            float m_CenterOffest = 0.6f;
            float m_CenterOffsetMax = 0.9f;
            float m_CenterOffsetMin = 0.6f;
            float m_PinchMax = 10.0f;

            Transform m_CameraTran = Camera.main.transform;

            Vector3 camInitPos = GameManager.gameManager.PlayerDataPool.EnterSceneCache.EnterScenePos;
            camInitPos = ActiveScene.GetTerrainPosition(camInitPos, false);


            //更新摄像机的角度
            m_CameraXOffset = (m_CameraXOffsetMax - m_CameraXOffsetMin) * m_Scale + m_CameraXOffsetMin;
            m_CameraYOffset = (m_CameraYOffsetMax - m_CameraYOffsetMin) * m_Scale + m_CameraYOffsetMin;
            m_CameraZOffset = (m_CameraZOffsetMax - m_CameraZOffsetMin) * m_Scale + m_CameraZOffsetMin;
            m_CenterOffest = (m_CenterOffsetMax - m_CenterOffsetMin) * (1 - m_Scale) + m_CenterOffsetMin;
            Vector3 dir = new Vector3(m_CameraXOffset, m_CameraYOffset, m_CameraZOffset);
            m_CameraTran.position = camInitPos + dir;

            Vector3 pos = camInitPos;
            pos.y += m_CenterOffest;
            Vector3 lookPos = pos - m_CameraTran.position;
            lookPos.Normalize();
            m_CameraTran.rotation = Quaternion.LookRotation(lookPos);
        }
#endif

        void Awake()
        {
            //设置当前场景
            if (null == GameManager.gameManager)
            {
                ResourceManager.InstantiateResource("Prefab/Logic/GameManager", "GameManagerObject");
            }

#if UNITY_ANDROID && !UNITY_EDITOR

            if (null != Camera.main)
            {
                SetCameraMain();
            }

#endif

            //if (null == PlatformListener.Instance())
            //{
            //    ResourceManager.InstantiateResource("Prefab/Logic/PlatformListener", "PlatformListener");
            //}

            //if (null == AndroidHelper.Instance())
            //{
            //    ResourceManager.InstantiateResource("Prefab/Logic/AndroidHelper", "AndroidHelper");
            //}

            //将当前场景SceneLogic放入GameManager暂存
            GameManager.gameManager.SceneLogic = this;
            
			 //by dys 摄像机不能变化
			ResourceManager.InstantiateResource("Prefab/Logic/FingerGestures", "FingerGestures");
            //GameManager.gameManager.ActiveScene.CurSceneID = GameManager.gameManager.RunningScene;
            //GameManager.gameManager.ActiveScene.UIRoot = GameObject.Find("UI Root");
            // 将Debug工具挂入场景，如发布正式版，注释掉此行代码
           // DebugHelper.CreateDebugHelper();
            Singleton<ObjManager>.GetInstance().OnEnterScene();

#if UNITY_ANDROID && !UNITY_EDITOR
            GameManager.gameManager.initDataCallback = GonoAwake;
            GameManager.gameManager.ActiveScene.Init();
            return;
#endif

            // 动态加载场景无法加载SHADER问题
//#if UNITY_EDITOR
            GameObject SceneObj = GameObject.Find("Scene");
            if (null != SceneObj && SceneObj.GetComponent<ShaderFix>() == null)
            {
                SceneObj.AddComponent<ShaderFix>();
            }

            TeleportPoint[] Teleports = GameObject.FindObjectsOfType<TeleportPoint>();
            foreach (TeleportPoint curTeleport in Teleports)
            {
                if (curTeleport.gameObject.GetComponent<ShaderFix>() == null)
                {
                    curTeleport.gameObject.AddComponent<ShaderFix>();
                }
            }
            
//#endif
            GameManager.gameManager.ActiveScene.Init();

            //为了防止低端机型在场景切换完成和人物创建完成之间会照到空白的地方，这里先调整一下摄像机的位置
            if (null != Camera.main)
            {
                Vector3 camInitPos = GameManager.gameManager.PlayerDataPool.EnterSceneCache.EnterScenePos;
                camInitPos = ActiveScene.GetTerrainPosition(camInitPos);
                camInitPos.y += 8.0f;
                Camera.main.transform.position = camInitPos;
            }
            //Init Scene Obj(NPC) data from tables
            //InitSceneObjData(GameManager.gameManager.RunningScene);

            //初始化场景通路图
            if (null != GameManager.gameManager.AutoSearch)
            {
                GameManager.gameManager.AutoSearch.InitMapConnectPath();
            }

            //场景中本地创建主角
            //if ((int)GameDefine_Globe.SCENE_DEFINE.SCENE_LOGIN != GameManager.gameManager.RunningScene)
            //{
            //    Singleton<ObjManager>.GetInstance().CreateMainPlayer();
            //}

            //设置是否可处理消息包
            NetWorkLogic.GetMe().CanProcessPacket = true;
        }

        /// <summary>
        /// 用于资源加载 回调
        /// </summary>
        private void GonoAwake()
        {
            GameManager.gameManager.initDataCallback -= GonoAwake;

            //初始化场景通路图
            if (null != GameManager.gameManager.AutoSearch)
            {
                GameManager.gameManager.AutoSearch.InitMapConnectPath();
            }

            //设置是否可处理消息包
            NetWorkLogic.GetMe().CanProcessPacket = true;
        }

        // Use this for initialization
        //场景内部初始化在此进行
        void Start()
        {
            if ((int)GameDefine_Globe.SCENE_DEFINE.SCENE_LOGIN != GameManager.gameManager.RunningScene)
            {
				// login scene will call play bg self
				PlaySceneMusic();
                Singleton<CollectItem>.GetInstance().InitCollectItem(GameManager.gameManager.RunningScene);
				Singleton<JuQingItemMgr>.GetInstance().InitJuqingItem(GameManager.gameManager.RunningScene);
			}

			//StartCoroutine(BundleManager.BundleQueueLoadTick());
        }

        public void PlaySceneMusic()
        {
            if (null == GameManager.gameManager.SoundManager)
            {
                return;
            }

            Tab_SceneClass tab = TableManager.GetSceneClassByID(GameManager.gameManager.RunningScene, 0);
            if (tab == null)
            {
                return;
            }

            if (tab.BGMusic < 0)
            {
                return;
            }

            Tab_Sounds soundsTab = TableManager.GetSoundsByID(tab.BGMusic, 0);
            if (soundsTab == null)
            {
                //  
                LogModule.DebugLog("sound name " + tab.BGMusic.ToString() + " is null");
                return;
            }

            GameManager.gameManager.SoundManager.PlayBGMusic(tab.BGMusic, soundsTab.FadeOutTime, soundsTab.FadeInTime);
        }

        // Update is called once per frame
        void Update()
        {
            //如果主角为空，并且数据正确的话，创建主角
            if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
            {
                //判断服务器数据是否已经传输完毕
                if (GameManager.gameManager.OnLineState)
                {
                    //if (GameManager.gameManager.RunningScene == GameManager.gameManager.PlayerDataPool.EnterSceneCache.EnterSceneSceneID &&
                    //    GameManager.gameManager.PlayerDataPool.EnterSceneCache.EnterSceneServerID != -1 &&
                    //    GameManager.gameManager.PlayerDataPool.EnterSceneCache.EnterSceneRoleBaseID != -1)
                    //{
                    //    //创建MainPlayer
                    //    if ((int)GameDefine_Globe.SCENE_DEFINE.SCENE_LOGIN != GameManager.gameManager.RunningScene)
                    //    {
                    //        Singleton<ObjManager>.GetInstance().CreateMainPlayer();
                    //    }
                    //}
                }
                else
                {
                    //测试代码，保证单机下可以创建主角
                    if ((int)GameDefine_Globe.SCENE_DEFINE.SCENE_LOGIN != GameManager.gameManager.RunningScene)
                    {
                        GameManager.gameManager.PlayerDataPool.EnterSceneCache.EnterSceneCharModelID = (int)CharacterDefine.PROFESSION.XIAOYAO;
                        Singleton<ObjManager>.GetInstance().CreateMainPlayer();
                    }
                }
                
                //如果主角已经创建，清理服务器数据，防止重复建立，并且准备下次切换场景使用
                if (null != Singleton<ObjManager>.GetInstance().MainPlayer)
                {
                    GameManager.gameManager.PlayerDataPool.EnterSceneCache.ClearEnterSceneInfo();
                }
            }

#if UNITY_ANDROID
            if (Time.time - m_npcTime > 0)
            {
                Singleton<ObjManager>.GetInstance().ShowNPC();
                m_npcTime = Time.time + GameManager.gameManager.m_NPCRefreshTime;
            }

            if (_wheelTime == 0) _wheelTime = Time.time;

            if (Time.time - _wheelTime >= m_cacheTime)
            {
                Singleton<ObjManager>.GetInstance().DeleteNPCGameObject();
                _wheelTime = Time.time;
            }
#endif
        }

        private float m_cacheTime = 20.0f;
        private float _wheelTime;
        private float m_npcTime = 0.0f;

        void FixedUpdate()
        {
			BundleManager.BundleQueueLoadTick(this);
        }

        void Destroy()
        {
            if (null != GameManager.gameManager)
            {
                if (null != GameManager.gameManager.ActiveScene)
                {
                    GameManager.gameManager.ActiveScene.RelaseActiveSceneData();
                }

                //清空当前场景SceneLogic对象
                GameManager.gameManager.SceneLogic = null;
            }
        }
        void OnApplicationPause(bool paused)
        {
            if (!paused)
            {
                //程序从后台进入前台时
                PushNotification.CleanNotification();

                Obj_MainPlayer mainPlayer = Singleton<ObjManager>.Instance.MainPlayer;
                if (null != mainPlayer)
                {
                    mainPlayer.LastHeartBeatTime = UnityEngine.Time.time;
                }
            }
            if (GlobeVar.s_FirstInitGame)
            {
                // 这时候程序还没有初始化
                return;
            }
            //程序进入后台时
            if (paused)
            {
                PushNotification.CleanNotification();
                PushNotification.NotificationMessage2Clinet();
                LogModule.DebugLog("OnApplicationPause:NotificationMessage2Server");
                PushNotification.NotificationMessage2Server();
            }
           
        }
    }
}
