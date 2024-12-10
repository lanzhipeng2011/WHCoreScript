/********************************************************************************
 *	文件名：	ActiveSceneManager.cs
 *	全路径：	\Script\Scene\ActiveSceneManager.cs
 *	创建人：	李嘉
 *	创建时间：2013-10-29
 *
 *	功能说明：游戏当前激活场景,负责游戏的场景数据保存和提供常用方法
 *	修改记录：
*********************************************************************************/

using System;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
using GCGame.Table;
using Module.Log;
using System.IO;
using GCGame;
using Games.LogicObj;
using Clojure;
namespace Games.Scene
{
    public class Scene_Init_Data
    {
        Scene_Init_Data()
        {
            m_bIsValid = false;
        }

        public bool m_bIsValid;         //该结构体是否合法，创建的时候不合法，必须手动赋值后再设置该项
        public int m_nCurSceneSrvID;    //场景服务器ID
    }

    public class ActiveScene
    {
        private GameObject m_MainCamera; //主摄像机
        /// <summary>
        /// 场景主摄像机
        /// </summary>
        public GameObject MainCamera 
        {
            get { return m_MainCamera; }
        }
		private GameObject m_Light; //灯光
		/// <summary>
		/// 场景灯光
		/// </summary>
		public GameObject Lighting 
		{
			get { return m_Light; }
		}
        private int m_nCurSceneServerID;  //当前场景的服务器ID（考虑到副本场景可能资源ID和场景服务器ID不一致的情况）
        public int CurSceneServerID
        {
            get { return m_nCurSceneServerID; }
            set { m_nCurSceneServerID = value; }
        }

        private GameObject m_UIRoot;      //当前场景UI根节点
        public GameObject UIRoot
        {
            get { return m_UIRoot; }
            set { m_UIRoot = value; }
        }

        public GameObject NameBoardRoot { set; get; }

        public GameObject FakeObjRoot { get; set; }
        public GameObject FakeObjTrans
        {
            get { return FakeObjRoot.transform.FindChild("TransformOff").gameObject; }
        }

        private GameObject[] m_SceneAudioSources;
        public GameObject[] SceneAudioSources
        {
            get { return m_SceneAudioSources; }
            set { m_SceneAudioSources = value; }
        }
        private GameObject m_Teleport;      // 传送点
        public GameObject Teleport
        {
            get { return m_Teleport; }
            set { m_Teleport = value; }
        }
        private GameObject m_TeleportCopyScene;      // 传送下一个副本
        public GameObject TeleportCopyScene
        {
            get { return m_TeleportCopyScene; }
            set { m_TeleportCopyScene = value; }
        }
        private GameObject m_DamageBoardRoot = null;
        public GameObject DamageBoardRoot
        {
            get { return m_DamageBoardRoot; }
        }
        public DamageBoardManager m_DamageBoardManager = null;
        public DamageBoardManager DamageBoardManager
        {
            get { return m_DamageBoardManager; }
        }

        private GameObject[] m_QingGongPointList;
        public GameObject[] QingGongPointList
        {
            get { return m_QingGongPointList; }
            set { m_QingGongPointList = value; }
        }

        private float m_ChangeTime = -1;
        private float m_SceneTimeScaleStart = -1;
        //地形文件
        private TerrainManager m_TerrainData = null;
        public TerrainManager TerrainData
        {
            get { return m_TerrainData; }
            set { m_TerrainData = value; }
        }

        //名字版池子
        private GameObjectPool m_NameBoardPool = null;
        public GameObjectPool NameBoardPool
        {
            get { return m_NameBoardPool; }
            set { m_NameBoardPool = value; }
        }

        //掉落包节点
        public GameObject DropItemBoardRoot { set; get; }
        //////////////////////////////////////////////////////////////////////////
        //地面特效部分
        //////////////////////////////////////////////////////////////////////////
        private GameObject m_MovingCircle = null;      //地表移动特效
        private GameObject m_SelectCircle = null;      //NPC选中特效
		public GameObject  m_sele1=null;
		public GameObject  m_sele2=null;
        private GameObject m_SelectObj = null;         //NPC脚下光圈的目标

        private GameObject m_GuideArrow;
        public UnityEngine.GameObject SelectObj
        {
            get { return m_SelectObj; }
            set { m_SelectObj = value; }
        }
        //设置移动特效状态和位置
        public void ActiveMovingCircle(Vector3 pos)
        {
            if (null == m_MovingCircle)
            {
                return;
            }
            Singleton<ObjManager>.Instance.MainPlayer.m_playerHeadInfo.ToggleXunLu(false);
			ObjManager.Instance.MainPlayer.AutoXunLu=false;
            if (!m_MovingCircle.activeSelf)
            {
                m_MovingCircle.SetActive(true);
            }

            for (int i = 0; i < m_MovingCircle.transform.childCount; ++i)
            {
                GameObject child = m_MovingCircle.transform.GetChild(i).gameObject;
                if (null != child)
                {
                    ParticleSystem particle = child.GetComponent<ParticleSystem>();
                    if (null != particle)
                    {
                        particle.time = 0.0f;
                    }
                }
            }
            
            ////由于特效需要显示在最上层，所以判断点击点和地表哪个更高，取最高值
            //float height = 0;
            //if (GameManager.gameManager.ActiveScene.IsT4MScene())
            //{
            //    height = GameManager.gameManager.ActiveScene.GetTerrainHeight(pos);
            //}
            //else
            //{
            //    height = UnityEngine.Terrain.activeTerrain.SampleHeight(pos);
            //}
            //if (height > pos.y)
            //{
            //    pos.y = height;
            //}
            m_MovingCircle.transform.position = pos + new Vector3(0,0.3f,0); 
        }

        public void DeactiveMovingCircle()
        {
            if (null != m_MovingCircle)
            {
                m_MovingCircle.SetActive(false);
            }
        }
        
        //设置选择特效状态和位置
		public void ActiveSelectCircle(GameObject obj,Obj_Character  chara)
        {
            //在开启显示服务器主角位置的情况下，选中特效作为位置显示使用，不参与选中逻辑
            if (GameManager.gameManager.ShowMainPlayerServerTrace)
            {
                return;
            }

            if (null == m_SelectCircle || null == obj)
            {
                return;
            }
		//	m_SelectCircle.SetActive(true);

			Tab_RoleBaseAttr roleBaseTab = TableManager.GetRoleBaseAttrByID(chara.BaseAttr.RoleBaseID, 0);
			if (roleBaseTab != null) {
						if (roleBaseTab.CombatNPC > 0) {
								
								m_sele1.SetActive (false);
								
								m_sele2.SetActive (true);		
						} else {
					m_sele1.SetActive (true);
					
					m_sele2.SetActive (false);		
				}
			} else {

			
						CharacterDefine.REPUTATION_TYPE nType = Reputation.GetObjReputionType (chara);
						//技能针对敌对目标 过滤掉非敌对的目标
						if (nType == CharacterDefine.REPUTATION_TYPE.REPUTATION_HOSTILE )
				         {
					m_sele1.SetActive (false);
					
					m_sele2.SetActive (true);		
				}
				else
				{
				
					m_sele1.SetActive (true);
					
					m_sele2.SetActive (false);		
					
				}
			}
			//激活
          
            m_SelectObj = obj;

           // m_SelectCircle.transform.position = obj.transform.position;
			Vector3 posV3 = new Vector3 (obj.transform.position.x, obj.transform.position.y + 0.1f, obj.transform.position.z);
			m_SelectCircle.transform.position = posV3;

            //float height = 0;
            //if (GameManager.gameManager.ActiveScene.IsT4MScene())
            //{
            //    height = GameManager.gameManager.ActiveScene.GetTerrainHeight(obj.transform.position);
            //}
            //else
            //{
            //    height = UnityEngine.Terrain.activeTerrain.SampleHeight(obj.transform.position);
            //}
            ////由于特效需要显示在最上层，所以判断点击点和地表哪个更高，取最高值
            //if (height > obj.transform.position.y)
            //{
            //    Vector3 pos = m_SelectCircle.transform.position;
            //    pos.y = height;
            //    m_SelectCircle.transform.position = pos;
            //}
        }

        public void DeactiveSelectCircle()
        {
            //在开启显示服务器主角位置的情况下，选中特效作为位置显示使用，不参与选中逻辑
            if (GameManager.gameManager.ShowMainPlayerServerTrace)
            {
                return;
            }
		
            if (null != m_SelectCircle)
            {
               // m_SelectCircle.SetActive(false);
				m_sele1.SetActive(false);
				m_sele2.SetActive(false);
			}

            m_SelectObj = null;
        }

        private void UpdateSelectCircle()
        {
            //在开启显示服务器主角位置的情况下，选中特效作为位置显示使用，不参与选中逻辑
            if (GameManager.gameManager.ShowMainPlayerServerTrace)
            {
                return;
            }

       

            if (null != m_SelectObj && null != m_SelectCircle)
            {
                //m_SelectCircle.transform.position = m_SelectObj.transform.position;
				Vector3 posV3 = new Vector3 (m_SelectObj.transform.position.x, m_SelectObj.transform.position.y + 0.1f, m_SelectObj.transform.position.z);
				m_SelectCircle.transform.position = posV3;
            }
        }

        //主角服务器位置测试选项
        public void ShowMainPlayerServerPosition(float fX, float fZ)
        {
            if (false == GameManager.gameManager.ShowMainPlayerServerTrace)
            {
                return;
            }
            
            if (null != m_SelectCircle)
            {
               // m_SelectCircle.SetActive(true);
                Vector3 pos = new Vector3(fX, 0, fZ);
                pos.y = Singleton<ObjManager>.GetInstance().MainPlayer.ObjTransform.position.y;

                //m_SelectCircle.transform.position = pos;
				Vector3 posV3 = new Vector3 (pos.x, pos.y + 0.1f, pos.z);
				m_SelectCircle.transform.position = posV3;
            }
        }

        public bool Init()
        {
            m_UIRoot = GameObject.Find("UI Root");
            if (null == m_UIRoot)
            {
                LogModule.WarningLog("can not find uiroot in curscene");
            }
            else
            {
                if (null == m_UIRoot.GetComponent<UIManager>())
                {
                    m_UIRoot.AddComponent<UIManager>();
                }
            }
            NameBoardRoot = GameObject.Find("CharacterRoot");
            if (NameBoardRoot == null)
            {
                NameBoardRoot = ResourceManager.InstantiateResource("Prefab/HeadInfo/NameBoardRoot") as GameObject;
            }

            FakeObjRoot = GameObject.Find("FakeObjRoot");
            if (FakeObjRoot == null)
            {
                FakeObjRoot = ResourceManager.InstantiateResource("Prefab/HeadInfo/FakeObjRoot") as GameObject;
                FakeObjRoot.SetActive(false);
            }

            DropItemBoardRoot = GameObject.Find("DropItemBoardRoot");
            if (DropItemBoardRoot == null)
            {
                DropItemBoardRoot = ResourceManager.InstantiateResource("Prefab/HeadInfo/DropItemBoardRoot") as GameObject;
            }

            //初始化特效
            m_MovingCircle = GameObject.Find("MovingCircle");
            if (null == m_MovingCircle)
            {
                m_MovingCircle = ResourceManager.InstantiateResource("Prefab/Effect/MovingCircle") as GameObject;
                if (null != m_MovingCircle)
                {
                    m_MovingCircle.transform.position = Vector3.zero;
                    m_MovingCircle.transform.rotation = Quaternion.Euler(Vector3.zero);
                    m_MovingCircle.SetActive(false);
                }
            }
            m_GuideArrow = GameObject.Find("GuideArrow");
            if (m_GuideArrow == null) 
            {
                m_GuideArrow = ResourceManager.InstantiateResource("Prefab/Effect/GuideArrow") as GameObject;
            }
            if (m_GuideArrow != null) 
            {
                m_GuideArrow.SetActive(false);
            }
              

            m_SelectCircle = GameObject.Find("SelectCircle");
            if (null == m_SelectCircle)
            {
                m_SelectCircle = ResourceManager.InstantiateResource("Prefab/Effect/SGSelectCircle") as GameObject;
                if (null != m_SelectCircle)
                {
                    m_SelectCircle.transform.position = Vector3.zero;
                    m_SelectCircle.transform.rotation = Quaternion.Euler(Vector3.zero);
					m_sele1=m_SelectCircle.transform.FindChild ("guangquan1").gameObject;
					m_sele2=m_SelectCircle.transform.FindChild ("guangquan2").gameObject;
					m_SelectCircle.SetActive(true);
                }
            }

            m_Teleport = GameObject.Find("Teleport");
            if (m_Teleport != null)
            {
                if (IsCopyScene())
                {
                    m_Teleport.SetActive(false);
                }
            }
            m_TeleportCopyScene = GameObject.Find("TeleportCopyScene");
            if (m_TeleportCopyScene)
            {
                if (IsCopyScene())
                {
                    m_TeleportCopyScene.SetActive(false);
                }
            }
            if (null == m_DamageBoardRoot)
            {
                m_DamageBoardRoot = new GameObject("DamageBoard Root");
                if (null != m_DamageBoardRoot)
                {
                    UIPanel panel = m_DamageBoardRoot.AddComponent<UIPanel>();
                    if (null != panel)
                    {
                        panel.depth = 20;
                    }
                    m_DamageBoardRoot.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

                    m_DamageBoardManager = m_DamageBoardRoot.AddComponent<DamageBoardManager>();
                }
            }

            m_QingGongPointList = GameObject.FindGameObjectsWithTag("QingGongPoint");
//             if (m_QingGongPoint != null)
//             {
//                 if (IsCopyScene())
//                 {
//                     m_QingGongPoint.SetActive(false);
//                 }
//             }
            for (int i = 0; i < m_QingGongPointList.Length; ++i)
            {
                if (IsCopyScene() && m_QingGongPointList[i] != null)
                {
                    m_QingGongPointList[i].SetActive(false);
                }
            }

            //声音资源
            m_SceneAudioSources = GameObject.FindGameObjectsWithTag("SceneSoundEffect");
            for (int i = 0; i < m_SceneAudioSources.Length; ++i)
            {
                if (m_SceneAudioSources[i] != null && PlayerPreferenceData.SystemSoundEffect == 0)
                {
                    m_SceneAudioSources[i].SetActive(false);
                }
            }

            //初始化Terrain数据
            #region
            if (GameManager.gameManager.RunningScene != (int)Games.GlobeDefine.GameDefine_Globe.SCENE_DEFINE.SCENE_LOGIN)
            {
                if (null != m_TerrainData)
                {
                    m_TerrainData = null;
                }

                Tab_SceneClass sceneClass = TableManager.GetSceneClassByID(GameManager.gameManager.RunningScene, 0);
                if (null != sceneClass)
                {
                    //PC上Raw文件路径为Assets/MLDJ/Scene/TerrainRaw/+场景名称.raw
                    //iPhone上文件路径为Application.dataPath/TerrainRaw/+场景名称.raw
                    string streamingAssetPath = Utils.GetStreamingAssetPath();
                    if ("" == streamingAssetPath)
                    {
                        return false;
                    }

                    string strTerrainRawFilePath = streamingAssetPath + "/TerrainRaw/" + sceneClass.ResName + ".raw";

#if UNITY_ANDROID && !UNITY_EDITOR
                        m_TerrainData = new TerrainManager(); // 可优化区
                        //场景如果分32*32个块，那么顶点应该是33*33
                        if (false == m_TerrainData.InitTerrianData(strTerrainRawFilePath,
                                                                    sceneClass.TerrainHeightMapLength + 1,
                                                                    sceneClass.TerrainHeightMapWidth + 1,
                                                                    sceneClass.TerrainHeightMax,
                                                                    sceneClass.Length + 1,
                                                                    sceneClass.Width + 1))
                        {
                            LogModule.DebugLog("init Scene TerrainData Failed");
                        }
#else
                    if (File.Exists(strTerrainRawFilePath))
                    {
                        m_TerrainData = new TerrainManager(); // 可优化区
                        //场景如果分32*32个块，那么顶点应该是33*33
                        if (false == m_TerrainData.InitTerrianData(strTerrainRawFilePath,
                                                                    sceneClass.TerrainHeightMapLength + 1,
                                                                    sceneClass.TerrainHeightMapWidth + 1,
                                                                    sceneClass.TerrainHeightMax,
                                                                    sceneClass.Length + 1,
                                                                    sceneClass.Width + 1))
                        {
                            LogModule.DebugLog("init Scene TerrainData Failed");
                        }
                    }
#endif

                }
            }

            #endregion

            //初始化名字版池子
            if (null == m_NameBoardPool)
            {
                m_NameBoardPool = new GameObjectPool("HeadInfo", 128);
            }

            if (null != m_NameBoardPool)
            {
                m_NameBoardPool.ClearAllPool();
            }

            if (m_MainCamera == null)
            {
                m_MainCamera = GameObject.Find("Main Camera");
            }
		//	if (m_Light == null)
			{
				m_Light = GameObject.Find("Directional light");
				if(m_Light!=null)
				{
					m_Light.GetComponent<Light>().enabled=false;
//					m_Light.GetComponent<Light>().shadowStrength=0.8f;
//					m_Light.GetComponent<Light>().color=new Color(0.1f,0.1f,0.1f);
					//m_Light.GetComponent<Light>().shadows.
				}
			}


#if  UNITY_ANDROID && !UNITY_EDITOR
            //Android默认下关闭Bloom效果
            if (m_MainCamera != null)
            {
                var fastBloom = m_MainCamera.GetComponent<FastBloom>();
                if (fastBloom != null)
                {
                    fastBloom.enabled = false;
                    fastBloom = null;
                }
            }
#endif
            return true;
        }

        bool IsScriptScene  = false;
        public void OnLoad(int sceneID) 
        {
            if (RT.MainContext == null) 
            {
                RT.InitRuntime();
                GameTrigger.Instance.Load();
            }
            IsScriptScene = false;
            string scriptName =  TableManager.GetSceneClassByID(sceneID)[0].SceneScript;
            if (scriptName != "" && scriptName != null && scriptName!="0")
            {
                IsScriptScene = true;
                RT.LoadASTTree(scriptName, ((TextAsset)Resources.Load("Tables/Triggers/" + scriptName)).text);              
                RT.EvaluateTree(RT.GetASTTree(scriptName));
				//GameTrigger.Instance.CallEvent("GameLoad");
			}
		}

        
        public void MainPlayerCreateOver()
        {
            if (IsScriptScene)
            {
                GameTrigger.Instance.CallEvent("GameLoad");
            }
        }
        public void Update()
        {
           
            GameTrigger.Instance.Update();
            
            UpdateSelectCircle();
            //UpdateMovingCircle();
            UpdateSceneTimeScale();
        }

        public void RelaseActiveSceneData()
        {
            m_nCurSceneServerID = GlobeVar.INVALID_ID;
            m_UIRoot = null;
            NameBoardRoot = null;
            m_TerrainData = null;
            m_MovingCircle = null;
            m_SelectCircle = null;
            m_Teleport = null;
            m_TeleportCopyScene = null;
            m_QingGongPointList = null;
            m_NameBoardPool = null;
            m_DamageBoardRoot = null;
            m_DamageBoardManager = null;
            m_SceneAudioSources = null;
        }

        public void SceneTimeScale(int nTimeScaleType)
        {
            if (nTimeScaleType == 0)
            {
                Time.timeScale = 0.2f;
                m_ChangeTime = 0.6f;
                m_SceneTimeScaleStart = Time.fixedTime;
            }
            else if (nTimeScaleType == 1) //todo 临时实验代码
            {
//                 Time.timeScale = 0.5f;
//                 m_ChangeTime = 0.1f;
//                 m_SceneTimeScaleStart = Time.fixedTime;
            }
        }

        void UpdateSceneTimeScale()
        {
            if (m_ChangeTime > 0)
            {
                if (Time.fixedTime - m_SceneTimeScaleStart >= m_ChangeTime)
                {
                    m_SceneTimeScaleStart = -1;
                    m_ChangeTime = -1;
                    Time.timeScale = 1;
                }
            }
        }

        //判断是否为T4M场景，只要判断有没有高度图raw文件即可
        public bool IsT4MScene()
        {
            return (m_TerrainData != null);
        }

        public float GetTerrainHeight(Vector3 pos)
        {
            if (null != m_TerrainData)
            {
                return m_TerrainData.GetTerrianHeight(pos);
            }

            return 0;
        }

        //根据给定点获得在导航网格上的高度
        public float GetNavSampleHeight(Vector3 pos)
        {
            NavMeshHit hit;
	
//			if (GameManager.gameManager.RunningScene == 1||GameManager.gameManager.RunningScene == 3||GameManager.gameManager.RunningScene == 5)
//			{
//				//pos.y = pos.y+15 ;
//			}
			if (GameManager.gameManager.RunningScene == 8) {
				pos.y = pos.y + 10;
			} else 
			{
				
				pos.y = pos.y + 30;
			}
//
//		

            if (NavMesh.SamplePosition(pos,out hit, 40.0f, -1))
            {
                return hit.position.y;
            }            

            return 0.0f;
        }

        public float GetTerrainHeightSample(Vector3 pos)
        {
            if (null != m_TerrainData)
            {
                return m_TerrainData.GetTerrianHeightSample(pos);
            }
            return 0;
        }

        public bool IsCopyScene()
        {
            Tab_SceneClass tabSceneClass = TableManager.GetSceneClassByID(GameManager.gameManager.RunningScene, 0);
            if (tabSceneClass != null)
            {
                if (tabSceneClass.Type == (int)GameDefine_Globe.SCENE_TYPE.SCENETYPE_COPYSCENE)
                {
                    return true;
                }
            }
            return false;
        }

        public void SetSceneSoundEffect(bool bIsActive)
        {
            for (int i = 0; i < m_SceneAudioSources.Length; ++i)
            {
                if (m_SceneAudioSources[i] != null)
                {
                    m_SceneAudioSources[i].SetActive(bIsActive);
                }
            }
        }


        public static Vector3 GetTrrainPositionByAndroid(Vector3 orgPosition)
        {
            Vector3 newPosition = orgPosition;

            return newPosition;
        }
		public static bool  IsInActive(Vector3 orgPosition)
		{

			Vector3 m_TestRayOrigin = new Vector3(orgPosition.x, 50, orgPosition.z);         //进行射线检测的射线原点
			 Vector3 m_TestRayDirection = new Vector3(0, -100.0f, 0);     //进行射线检测的射线方向
			string m_szServerObstacleTag = "ServerObstacleTestPoint";    //为服务器行走面的物理体的Tag

			Ray ray = new Ray(m_TestRayOrigin, m_TestRayDirection);
			RaycastHit hit;
			//	Debug.DrawRay (m_TestRayOrigin, m_TestRayDirection);
			if (Physics.Raycast (ray, out hit))
			{
								//如果检测点是
				if (hit.collider.gameObject.CompareTag (m_szServerObstacleTag))
				{
					return true;
			    }
				return false;
			}
			return false;
		}
		public static Vector3 GetActivePos(Vector3 orgPosition)
		{
			Vector3 newPosition = orgPosition;
			if(GameManager.gameManager.RunningScene==5||GameManager.gameManager.RunningScene==6||GameManager.gameManager.RunningScene==11)
			{
				return newPosition;
			}
			if(IsInActive(newPosition))
			{
				return newPosition;
			}
			Vector3 newpos = new Vector3 (newPosition.x, 0, newPosition.z);
			Vector3  [] possz=new Vector3[9];
			int k = 0;
			for (int i=-1; i<=1; i++)
			{
				for(int j=-1;j<=1;j++)
				{
					possz[k].x=newpos.x+i;
					possz[k].y=0;
					possz[k].z=newpos.z+j;
					if(IsInActive(possz[k]))
					{
							return possz[k];
					}
					k++;
				}
			}

			return orgPosition;

		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgPosition"></param>
        /// <param name="isTag">标记Android下，是否读取默认配置中的值。只在场景首次加载时使用</param>
        /// <returns></returns>
        static public Vector3 GetTerrainPosition(Vector3 orgPosition, bool isTag = true)
        {
            if (null == GameManager.gameManager)
            {
                return orgPosition;
            }
            //<!--<Rectange Color="#017bcd" Scale="120,35,10" Position="0,0,0"></Rectange>-->
            Vector3 newPosition = orgPosition;

			Vector3 newpos = new Vector3 (orgPosition.x, 0, orgPosition.z);
			//newpos = GetActivePos (newpos);
			newpos.y = GameManager.gameManager.ActiveScene.GetNavSampleHeight(newpos);
			newPosition= newpos;
	   
			//            if (GameManager.gameManager.RunningScene == 5) 
//            {
//              newPosition.y = 20.0f;
//            }
//            if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_YANZIWU)
//            {
//                //暂时添加
//                newPosition.y = 17.5f;
//            }
//            else if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_JIANHUGONG)
//            {
//                //暂时添加
//                newPosition.y = 15.5f;
//            }
//            else if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_ERHAI)
//            {
//                newPosition.y = 10.51f;
//            }
//            else if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_YANGWANGGUMU)
//            {
//                newPosition.y = 20.5f;
//            }
//            else if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_JIANYU)
//            {
//                newPosition.y = 15.5f;
//            }
//            else if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_GUILDWAR)
//            {
//                newPosition.y = 20.0f;
//            }
//            else if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_WUDAOZHIDIAN ||
//                     GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_HUASHANLUNJIAN ||
//                     GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_YUQINGGONG ||
//                     GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_RICHANGJUEDOU)
//            {
//                newPosition.y = 22.7f;
//            }
//            else if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_HUTOUZHANCHUAN)
//            {
//                newPosition.y = 7.3f;
//            }
//            else if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_WUCHANG)
//            {
//                newPosition.y = 17.0f;
//            }
//            else if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_FBSHAOSHISHAN)
//            {
//                newPosition.y = 6.2f;
//            }
//            else
//            {
//                if (isTag)
//                {
//                    if (null != GameManager.gameManager.ActiveScene &&
//                    GameManager.gameManager.ActiveScene.IsT4MScene())
//                    {
//                        newPosition.y = GameManager.gameManager.ActiveScene.GetTerrainHeight(newPosition);
//                    }
//                    else
//                    {
//                        if (null != Terrain.activeTerrain)
//                        {
//                            newPosition.y = Terrain.activeTerrain.SampleHeight(newPosition);
//                        }
//                    }
//                }
//                else
//                {
//                    if (GameManager.gameManager.RunningScene < GameDefine_Globe.SceneHeight.Length
//                        && GameManager.gameManager.RunningScene >= 0)
//                    {
//                        newPosition.y = GameDefine_Globe.SceneHeight[GameManager.gameManager.RunningScene];
//                    }
//                    else
//                    {
//                        newPosition.y = 0.0f;
//                    }
//                }
//            }

            return newPosition;
        }

        public void InitFakeObjRoot(GameObject topleft, GameObject bottomright)
        {
            CameraFakeObjct fakeObj = FakeObjRoot.GetComponent<CameraFakeObjct>();
            if (null != fakeObj)
            {
                fakeObj.m_TopLeft = topleft;
                fakeObj.m_BottomRight = bottomright;
                fakeObj.Init();
            }
        }

        public void ShowFakeObj()
        {
            FakeObjRoot.SetActive(true);
        }

        public void HideFakeObj()
        {
            if (FakeObjRoot != null)
            FakeObjRoot.SetActive(false);
        }
    }
}
