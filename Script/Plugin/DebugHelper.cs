/********************************************************************
	created:	2013/12/24
	created:	24:12:2013   14:13
	filename: 	DebugHelper.cs
	author:		王迪
	
	purpose:	用系统控件做一些辅助功能，测试性能
*********************************************************************/

using UnityEngine;
using System.Collections;
using Games.LogicObj;
using Games.AI_Logic;
using Games.Animation_Modle;
using System.Collections.Generic;
using Games.GlobeDefine;
using Games.Fellow;
using Module.Log;
public class DebugHelper : MonoBehaviour {
    private bool m_bUseOtherFun = false;

    //private int m_PlayerCount;

    //private bool m_IsSetNameHeight = false;
    private string m_strSetNameHeight = "";

    // Use this for initialization
    void Start()
    {
        _widthValue = Screen.width * _widthValue / 2000f;
        _heightValue = Screen.height * _heightValue / 1000f;
    }

    private static  GameObject helperInstance = null;
    public static void CreateDebugHelper()
    {
        if (!PlatformHelper.IsEnableDebugMode())
        {
            return;
        }
        if (null == helperInstance)
        {
            helperInstance = ResourceManager.InstantiateResource("Prefab/Logic/DebugHelper", "DebugHelper") as GameObject;
        }

        if(null == helperInstance)
        {
            LogModule.DebugLog("create debughelper fail");
        }
    }


    //private GameObject sceneObj = null;
    private GameObject uiRoot = null;
	public static bool m_bShowEffect = true;
    public static bool m_bShowDamageBoard = true;
    public static bool m_bEnableTestAccount = false;

    private float _widthValue = 150f;
    private float _heightValue = 40f;

	void OnGUI()
    {
        if (GUILayout.Button("door", GUILayout.Height(_heightValue), GUILayout.Width(_widthValue)))
        {
            m_bUseOtherFun = !m_bUseOtherFun;
        }

        if (m_bUseOtherFun)
        {                        
            if (GUILayout.Button("HideUI", GUILayout.Height(_heightValue), GUILayout.Width(_widthValue)))
            {
                if (null == uiRoot)
                {
                    uiRoot = GameObject.Find("UI Root");
                }
                uiRoot.SetActive(!uiRoot.activeSelf);
            }
			
			if (GUILayout.Button("DisConnect", GUILayout.Height(_heightValue), GUILayout.Width(_widthValue)))
			{
				NetWorkLogic.GetMe().DisconnectServer();
			}

            if (GUILayout.Button("PlayerSound", GUILayout.Height(_heightValue), GUILayout.Width(_widthValue)))
            {
                bool bEnable = SoundManager.m_EnableBGM;
                bEnable = !bEnable;
                SoundManager.m_EnableBGM = bEnable;
                SoundManager.m_EnableSFX = bEnable;
            }
            
            if (LoginData.m_bEnableTestAccount)
            {
                if (GUILayout.Button("Disable TestAccount", GUILayout.Height(_heightValue), GUILayout.Width(_widthValue)))
                {
                    LoginData.m_bEnableTestAccount = false;
                }

                LoginData.m_strTestAccount = GUI.TextField(new Rect(Screen.width - 200, 0, 200, _heightValue), LoginData.m_strTestAccount, 15);
            }
            else
            {
                if (GUILayout.Button("Enable TestAccount", GUILayout.Height(_heightValue), GUILayout.Width(_widthValue)))
                {
                    LoginData.m_bEnableTestAccount = true;
                }
            }

            if (GUILayout.Button("TerrainHeight", GUILayout.Height(_heightValue), GUILayout.Width(_widthValue)))
            {
                Obj_MainPlayer objMain = Singleton<ObjManager>.GetInstance().MainPlayer;
                if (null != objMain)
                {
                    if (null != GameManager.gameManager.ActiveScene &&
                        null != GameManager.gameManager.ActiveScene.TerrainData)
                    {
                        float height = GameManager.gameManager.ActiveScene.TerrainData.GetTerrianHeight(objMain.Position);
                        Debug.Log("Terrain Heigt: " + height);
                        return;
                    }
                }

                Debug.Log("Get Terrain Height Error");
            }

            m_strSetNameHeight = GUI.TextField(new Rect(Screen.width - 200, _heightValue, 200, _heightValue), m_strSetNameHeight, 15);
            if (GUILayout.Button("ChangeNameHeight", GUILayout.Height(_heightValue), GUILayout.Width(_widthValue)))
            {               
                if (Singleton<ObjManager>.Instance.MainPlayer)
                {
                    float fNewHeight;
                    bool bResult = float.TryParse(m_strSetNameHeight, out fNewHeight);
                    if (bResult)
                    {
                        Obj_Character target = Singleton<ObjManager>.Instance.MainPlayer.SelectTarget;
                        if (null != target)
                        {
                            BillBoard billboard = target.HeadInfoBoard.GetComponent<BillBoard>();
                            if (billboard != null)
                            {
                                billboard.fDeltaHeight = fNewHeight;
                            }
                        }

                        //m_IsSetNameHeight = false;
                    }

                }
            }

            if (GUILayout.Button("Unload unuse", GUILayout.Height(_heightValue), GUILayout.Width(_widthValue)))
			{
                //BundleManager.DoUnloadUnuseBundle();
			}
        }
    }
    
    /// <summary>
    /// 创建测试用Obj
    /// </summary>
    /// <param name="szRootPrefab"></param>     Root的Prefab路径
    /// <param name="szPrefab"></param>         具体模型的Prefab路径
    /// <param name="szAnimationPath"></param>  如果需要动态加载动作，则输入路径
    void CreateTestObject(string szRootPrefab, string szPrefab, string szAnimationPath)
    {
        //Obj_Character mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer as Obj_Character;
        //if (null != mainPlayer)
        //{
        //    Vector3 newPos = mainPlayer.transform.position + new Vector3(Random.Range(-1.5f, 1.5f), 0, Random.Range(-1.5f, 1.5f));
        //    Vector3 rotate = new Vector3(0.0f, Random.Range(-180.0f, 180.0f), 0.0f);

        //    m_MainPlayer[m_PlayerCount] = ResourceManager.InstantiateResource(szRootPrefab, "Test Player" + m_PlayerCount) as GameObject;
        //    if (m_MainPlayer[m_PlayerCount])
        //    {
        //        Singleton<ObjManager>.GetInstance().ReloadModel(m_MainPlayer[m_PlayerCount], szPrefab);
        //        m_MainPlayer[m_PlayerCount].transform.Translate(newPos);
        //        m_MainPlayer[m_PlayerCount].transform.Rotate(rotate);

        //        Obj_NPC objScript = m_MainPlayer[m_PlayerCount].AddComponent<Obj_NPC>();
        //        if (null == objScript)
        //        {
        //            return;
        //        }

        //        if (m_MainPlayer[m_PlayerCount].GetComponent<AIController>() == null)
        //        {
        //            m_MainPlayer[m_PlayerCount].AddComponent<AIController>();
        //        }
        //        objScript.Controller = (AIController)m_MainPlayer[m_PlayerCount].GetComponent<AIController>();

        //        if (m_MainPlayer[m_PlayerCount].GetComponent<AI_Patrol>() == null)
        //        {
        //            m_MainPlayer[m_PlayerCount].AddComponent<AI_Patrol>();
        //        }
        //        objScript.Controller.NormalAI = m_MainPlayer[m_PlayerCount].GetComponent<AI_Patrol>();

        //        if (m_MainPlayer[m_PlayerCount].GetComponent<AI_NpcCombat>() == null)
        //        {
        //            m_MainPlayer[m_PlayerCount].AddComponent<AI_NpcCombat>();
        //        }
        //        objScript.Controller.CombatAI = m_MainPlayer[m_PlayerCount].GetComponent<AI_NpcCombat>();

        //        if (m_MainPlayer[m_PlayerCount].GetComponent<NavMeshAgent>() == null)
        //        {
        //            m_MainPlayer[m_PlayerCount].AddComponent<NavMeshAgent>();
        //        }
        //        NavMeshAgent naviAgent = m_MainPlayer[m_PlayerCount].GetComponent<NavMeshAgent>();
        //        if (naviAgent && 0 != m_MainPlayer[m_PlayerCount].transform.localScale.x)
        //        {
        //            //设置成0，否则gameobject之间会互相碰撞
        //            naviAgent.radius = 0.0f;
        //            naviAgent.height = 2.0f / gameObject.transform.localScale.x;
        //        }

        //        if (m_MainPlayer[m_PlayerCount].GetComponent<AnimationLogic>() == null)
        //        {
        //            objScript.AnimLogic = m_MainPlayer[m_PlayerCount].AddComponent<AnimationLogic>();
        //        }
        //        objScript.AnimationFilePath = szAnimationPath;
        //        objScript.InitAnimation();
        //        objScript.AnimLogic.Play(1);

        //        if (m_MainPlayer[m_PlayerCount].GetComponent<EffectLogic>() == null)
        //        {
        //            m_MainPlayer[m_PlayerCount].AddComponent<EffectLogic>();
        //        }
        //        objScript.InitEffect();

        //        objScript.Controller.SwitchCurrentAI(objScript.Controller.NormalAI);
        //    }

        //    m_PlayerCount++;
        //    LogModule.DebugLog("Cur Main Player is: " + m_PlayerCount);
        //}
    }
}