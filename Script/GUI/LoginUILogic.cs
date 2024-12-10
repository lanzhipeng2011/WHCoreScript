//********************************************************************
// 文件名: LoginUILogic.cs
// 描述: 登录界面逻辑
// 作者: WangZhe
// 创建时间: 2013-11-1
//
// 修改历史:
// 2013-11-1 王喆创建
//********************************************************************

using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
using Games.Events;
using Games.LogicObj;
using GCGame;
using GCGame.Table;
using Module.Log;
using System.Collections.Generic;
using Games.Animation_Modle;
public class LoginUILogic : MonoBehaviour
{
    public GameObject Camera3D;
    public UpdateLoadingBar m_UpdateLoadingBar;
    public GameObject m_ResDownloadTip;
    public UILabel m_LabelResDownloadTip;

    public GameObject m_CameraUI;
    public GameObject m_CameraChooseRole;
    public GameObject m_CameraCreateRole;
    public GameObject m_ObjCreateRoleBack;

    public GameObject m_ModelChooseRole;
    public GameObject CreateRoleChar;
    private GameObject m_curController;
    private UpdateHelper m_curUpdateHelper;
    private bool m_bUpdating = false;
    private UpdateHelper.UpdateStep m_lastUpateStep = UpdateHelper.UpdateStep.NONE;
    
    public GameObject m_BackgroundTex;
    public Animation m_CameraAni;
    public List<BoxCollider> m_RoleCollider = new List<BoxCollider>();

    public GameObject m_OpeningScene;
    public GameObject[] CreateRoleModels;
    public GameObject m_XiaoYaoModel;
    public GameObject m_TianShanModel;
    public GameObject m_ShaoLinModel;
    public GameObject m_DaLiModel;
    public EffectLogic m_XiaoYaoEffect;
    public EffectLogic m_TianShanEffect;
    public EffectLogic m_ShaoLinEffect;
    public EffectLogic m_DaLiEffect;
    public GameObject m_XiaoYaoSpecialEffect;
    public MeshRenderer m_RoleCreateShader;

    public TweenPosition[] m_ArrayLoadingBackGround;
    public GameObject m_RoleProfessionEffect;

    public GameObject[] CreateRoleHideObjs;

    public static int m_LoginSelect = 0;    //正常登陆：0，账号:1,角色2

    private int m_RoleType = GlobeVar.INVALID_ID;
    private int m_RoleModelVisualID = GlobeVar.INVALID_ID;
    private int m_RoleWeaponID = GlobeVar.INVALID_ID;
    private int m_RoleWeaponEffectGem = GlobeVar.INVALID_ID;
    private bool m_RoleClick = false;
    private bool m_BeginChangeBlack = false;
    public  bool m_bDonwloadFile = false;
    

	public GameObject  m_chooseEffect;
	public GameObject  m_createEffect;
#if UNITY_ANDROID
    private bool m_bShowWarnning = false;
#endif

    private static LoginUILogic m_instance;
    public static LoginUILogic Instance()
    {
        return m_instance;
    }

    void Awake()
    {    
        m_instance = this;

#if !UNITY_EDITOR && UNITY_ANDROID 
        //关闭垂直同步
        QualitySettings.vSyncCount = 0;
#endif

        // 禁止屏幕自动黑屏
       // PlatformHelper.SetScreenCanAutoLock(false);
        // 限制帧率30帧
        Application.targetFrameRate = 30;


//#if UNITY_EDITOR
        LoginData.m_bEnableTestAccount = true;
        LoginData.m_strTestAccount = PlayerPreferenceData.LastAccount;
        if (string.IsNullOrEmpty(LoginData.m_strTestAccount))
        {
            LoginData.m_strTestAccount = Random.Range(0, 10000).ToString();
        }
//#endif

#if UNITY_ANDROID
        //if (PlatformHelper.GetChannelType() == PlatformHelper.ChannelType.TEST)
        //{
        //    LoginData.m_bEnableTestAccount = true;
        //    LoginData.m_strTestAccount = PlayerPreferenceData.LastAccount;
        //    if (string.IsNullOrEmpty(LoginData.m_strTestAccount))
        //    {
        //        LoginData.m_strTestAccount = Random.Range(0, 10000).ToString();
        //    }
        //}
        
        ////安卓下由于点登录的时候会卡住，所以屏蔽掉输帐号界面的动画
        //if (null != m_ArrayLoadingBackGround)
        //{
        //    for (int i = 0; i < m_ArrayLoadingBackGround.Length; ++i)
        //    {
        //        m_ArrayLoadingBackGround[i].enabled = false;
        //    }
        //}
#endif
    }
    void Start() 
    {
       

		PlayerPreferenceData.NewPlayerGuideClose = false;
		StartCoroutine(BeginInitGame());
	}

    void OnDestroy()
    {
        m_instance = null;
    }

    // 初始化游戏
	IEnumerator BeginInitGame()
	{

        // 第一次启动播放CG
        if (PlayerPreferenceData.IsAppFirstRun)
        {
            PlayerPreferenceData.IsAppFirstRun = false;
//#if !UNITY_EDITOR && !UNITY_STANDALONE_WIN
//            if(PlatformHelper.GetChannelType() != PlatformHelper.ChannelType.IOS_91)
//            {
//                Handheld.PlayFullScreenMovie("CG_TLBB.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput);
//                yield return new WaitForSeconds(0.1f);
//            }
//#endif
          //  UserConfigData.SetSystemDefault();
        }
       
        //PlatformHelper.SendUserAction(UserBehaviorDefine.AppStart);

        m_CameraUI.SetActive(true);

        m_UpdateLoadingBar.gameObject.SetActive(true);
        m_UpdateLoadingBar.SetLoadingPrecent(0);
        m_UpdateLoadingBar.SetStateString("");

        

		BeginCheckRes();

        yield return null;
	}
    public void OnApplicationQuit() 
    {
        NetWorkLogic.GetMe().DisconnectServer();
    }
	public void OnDoDownload()
    {
        m_bDonwloadFile = true;
       // m_ResDownloadTip.SetActive(false);
        m_curUpdateHelper.DownloadCurFileList();
    }

    void OnCancelDownload()
    {
        m_ResDownloadTip.SetActive(false);
        //m_UpdateLoadingBar.SetStateString("更新失败");
        m_UpdateLoadingBar.SetStateString(StrDictionary.GetClientDictionaryString("#{2843}"));
    }

    IEnumerator RequestiADFlag()
    {
        WWW wwwiADFlag = new WWW(DownloadHelper.AddTimestampToUrl(GameDefines.FlagiADUrl));
        yield return wwwiADFlag;
        if (string.IsNullOrEmpty(wwwiADFlag.error))
        {
            
            string iadFlag = wwwiADFlag.text;
            string[] strCol = iadFlag.Split('\t');
            int nFlag = 0;
            int nGameVersion = 0;
            int nProgramVersion = 0;
            LogModule.DebugLog("load iad flag success:" + iadFlag);
            if (strCol.Length >= 3 && int.TryParse(strCol[0], out nFlag) && int.TryParse(strCol[1], out nGameVersion) && int.TryParse(strCol[2], out nProgramVersion))
            {
                if (nFlag == 1 && nGameVersion == PlatformHelper.GetGameVersion() && nProgramVersion == PlatformHelper.GetProgramVersion())
                {
                    PlatformHelper.StartAD();
                }
            }
           
        }
    }

    IEnumerator RequestServerList()
    {
        //PlatformHelper.SendUserAction(UserBehaviorDefine.RequestServerList);
        // 请求服务器列表
        m_UpdateLoadingBar.SetLoadingPrecent(0);
        //m_UpdateLoadingBar.SetStateString("正在请求服务器列表");
        m_UpdateLoadingBar.SetStateString(StrDictionary.GetClientDictionaryString("#{2844}"));

        if (LoginData.serverListData.Count > 0)
        {
            // 读取服务器列表，保存服务器信息
            m_UpdateLoadingBar.SetLoadingPrecent(0.5f);
            //m_UpdateLoadingBar.SetStateString("请求列表成功，正在准备资源");
            m_UpdateLoadingBar.SetStateString(StrDictionary.GetClientDictionaryString("#{2846}"));
            yield return StartCoroutine(BeignLoadRes());
            yield break;
        }
        
        bool bLoadServerListSuccess = false;
        int nTryLoadServerListTime = 3;

        WWW wwwServerList = null;
        while (nTryLoadServerListTime > 0)
        {
            try
            {
                wwwServerList = new WWW(PlatformHelper.GetServerListUrl());
            }
            catch (System.Exception e) 
            {
                NGUIDebug.Log(e.ToString());
            }
            yield return wwwServerList;
            if(!string.IsNullOrEmpty(wwwServerList.error))
            {
                nTryLoadServerListTime--;
            }
            else
            {
                bLoadServerListSuccess = true;
                break;
            }
        }

        if (!bLoadServerListSuccess || null == wwwServerList)
        {
            //m_UpdateLoadingBar.SetStateString("服务器列表请求失败");
            m_UpdateLoadingBar.SetStateString(StrDictionary.GetClientDictionaryString("#{2845}"));
            LogModule.ErrorLog("update server list fail " + wwwServerList.error);
        }
        else
        {
            // 读取服务器列表，保存服务器信息
            m_UpdateLoadingBar.SetLoadingPrecent(0.5f);
            //m_UpdateLoadingBar.SetStateString("请求列表成功，正在准备资源");
            m_UpdateLoadingBar.SetStateString(StrDictionary.GetClientDictionaryString("#{2846}"));
            LoginData.serverListData.Clear();
            LoginData.serverPageData.Clear();

            string serverListTxt = wwwServerList.text;
            string[] rowDatas = serverListTxt.Split('\n');
            int jumpLineCount = 4; // IPList表格跳过前4行
            foreach (string curLine in rowDatas)
            {
                if (jumpLineCount > 0)
                {
                    jumpLineCount--;
                    continue;
                }
                if (string.IsNullOrEmpty(curLine))
                    continue;

                string[] strCols = curLine.Split('\t');
                bool bAddCurData = false;
                if (strCols.Length != 8)
                {
                    // 特殊判断，测试服务器,判定版本号
                    if (strCols.Length == 12)
                    {
                        int gameVersion = 0;
                        int programVersion = 0;
                        int publicVersion = 0;
                        int resVersion = 0;

                        if (int.TryParse(strCols[8], out gameVersion) &&
                            int.TryParse(strCols[9], out programVersion) &&
                            int.TryParse(strCols[10], out publicVersion) &&
                            int.TryParse(strCols[11], out resVersion))
                        {
                            if (gameVersion == PlatformHelper.GetGameVersion() &&
                                programVersion == PlatformHelper.GetProgramVersion() &&
                                resVersion == UserConfigData.ClientResVersion)
                            {
                                bAddCurData = true;
                            }
                        }
                        else
                        {
                            LogModule.ErrorLog("special server data col error");
                        }
                    }
                    else
                    {
                        LogModule.ErrorLog("server data col error");
                    }

                }
                else
                {
                    bAddCurData = true;
                }

                if (bAddCurData)
                {
                    LoginData.ServerListData newData = new LoginData.ServerListData(strCols[0], strCols[2], strCols[3], strCols[4], strCols[5], strCols[6]);
                    LoginData.serverListData.Insert(0, newData);
                    if (strCols[7].Length > 2)
                    {
                        LogModule.DebugLog(strCols[7]);
                        LoginData.serverPageData.Add(new LoginData.ServerPageData(LoginData.serverListData.Count-1, strCols[7]));
                    }
                }
            }

            yield return StartCoroutine(BeignLoadRes());
        }
    }

    IEnumerator BeignLoadRes()
    {
        NGUILogHelpler.Log("BeignLoadRes", "PlatformHelper");
        //PlatformHelper.SendUserAction(UserBehaviorDefine.BeginLoadingRes);
        // 按照ID大小从小到大排列
        // LoginData.serverListData.Sort();
        // Debug.Log("begin loading");
        // 载入资源

        //m_CameraCreateRole.SetActive(true);

        m_UpdateLoadingBar.SetLoadingPrecent(0.85f);
        //m_UpdateLoadingBar.SetStateString("载入字体资源");
        m_UpdateLoadingBar.SetStateString(StrDictionary.GetClientDictionaryString("#{2847}"));

        //yield return StartCoroutine(BundleManager.LoadFontUI());
        m_UpdateLoadingBar.SetLoadingPrecent(0.9f);
        //m_UpdateLoadingBar.SetStateString("载入通用资源");
        m_UpdateLoadingBar.SetStateString(StrDictionary.GetClientDictionaryString("#{2848}"));

       // yield return StartCoroutine(BundleManager.LoadCommonUI());
        m_UpdateLoadingBar.SetLoadingPrecent(0.95f);
       // yield return StartCoroutine(BundleManager.LoadLoginUI());
        m_UpdateLoadingBar.SetLoadingPrecent(0.99f);
        //m_UpdateLoadingBar.SetStateString("正在准备资源，即将进入游戏");
        m_UpdateLoadingBar.SetStateString(StrDictionary.GetClientDictionaryString("#{2849}"));

        yield return new WaitForSeconds(1.0f);
        
        GameManager.gameManager.InitGame();
        
        //m_UpdateLoadingBar.gameObject.SetActive(false);
        //m_BackgroundTex.SetActive(true);
        PlatformHelper.OnLoadGame();
        // 初始化UI
        PlatformHelper.OnInitPlatform((code,msg)=>{ InitUI();});
        VoiceManager.Instance.Init();
    }

   
     void InitUI()
    {


       

        //GameManager.gameManager.SceneLogic.PlaySceneMusic();
        if (m_LoginSelect == 1)
        {
            LoginData.accountData.CleanData();
            PlatformHelper.UserLogin();
            EnterServerChoose();
        }
        else if (m_LoginSelect == 2)
        {
            EnterChooseRole();
        }
        else
        {
            //PlatformHelper.SendUserAction(UserBehaviorDefine.ServerChoose_Show);
            //LogModule.DebugLog(TableManager.GetPublicConfigByID(GameDefines.PublicResVersionKey, 0).IntValue);
            EnterServerChoose();
            PlatformHelper.UserLogin();

        }
        UIManager.ShowUI(UIInfo.CentreNotice);

        m_LoginSelect = 0;
    }

    void Update()
    {
        if (m_bUpdating  && null != m_curUpdateHelper)
        {
            if (m_curUpdateHelper.CurUpdateStep != m_lastUpateStep)
            {
                switch (m_curUpdateHelper.CurUpdateStep)
                {
                    case UpdateHelper.UpdateStep.CheckVersion:
                        m_UpdateLoadingBar.SetLoadingPrecent(0);
                        m_UpdateLoadingBar.SetStateString("正在检查资源版本信息");
                        //m_UpdateLoadingBar.SetStateString(StrDictionary.GetClientDictionaryString("#{2850}"));
                        break;
                    case UpdateHelper.UpdateStep.GetFileList:
                        m_UpdateLoadingBar.SetLoadingPrecent(0.1f);
                        m_UpdateLoadingBar.SetStateString("正在获取文件列表");
                        //m_UpdateLoadingBar.SetStateString(StrDictionary.GetClientDictionaryString("#{2851}"));
                        break;
                    case UpdateHelper.UpdateStep.CompareRes:
                        m_UpdateLoadingBar.SetLoadingPrecent(0.2f);
                        m_UpdateLoadingBar.SetStateString("正在比对资源");
                        //m_UpdateLoadingBar.SetStateString(StrDictionary.GetClientDictionaryString("#{2852}"));

                        break;
                    case UpdateHelper.UpdateStep.AskIsDonwload:
                        m_UpdateLoadingBar.SetLoadingPrecent(0.3f);
                        m_UpdateLoadingBar.SetStateString("正在下载资源");
                        //m_UpdateLoadingBar.SetStateString(StrDictionary.GetClientDictionaryString("#{2853}"));

                       // m_ResDownloadTip.SetActive(true);

						//m_LabelResDownloadTip.text = "需要下载" + ((int)(m_curUpdateHelper.DownloadTotalSize / 1024.0f) + 1).ToString()+ "K, 是否下载?";                   
                        break;
                    case UpdateHelper.UpdateStep.DownloadRes:
                        break;
                    case UpdateHelper.UpdateStep.CheckRes:
                        m_UpdateLoadingBar.SetLoadingPrecent(0.9f);
                        m_UpdateLoadingBar.SetStateString("正在验证资源");
                        //m_UpdateLoadingBar.SetStateString(StrDictionary.GetClientDictionaryString("#{2855}"));
                        break;
                    case UpdateHelper.UpdateStep.CopyRes:
                        m_UpdateLoadingBar.SetLoadingPrecent(0.95f);
                        m_UpdateLoadingBar.SetStateString("正在准备资源");
                        //m_UpdateLoadingBar.SetStateString(StrDictionary.GetClientDictionaryString("#{2856}"));
                        break;
                    case UpdateHelper.UpdateStep.CleanCache:
                        m_UpdateLoadingBar.SetLoadingPrecent(0.98f);
                        m_UpdateLoadingBar.SetStateString("正在清理缓存");
                        //m_UpdateLoadingBar.SetStateString(StrDictionary.GetClientDictionaryString("#{2857}"));
                        break;
                    case UpdateHelper.UpdateStep.FINISH:
                        if (m_curUpdateHelper.CurUpdateResult == UpdateHelper.UpdateResult.Success)
                        {
                            m_UpdateLoadingBar.SetLoadingPrecent(1);
                            m_UpdateLoadingBar.SetStateString("资源更新完成");
                            //m_UpdateLoadingBar.SetStateString(StrDictionary.GetClientDictionaryString("#{2858}"));
                            FinishCheckRes(true);
                            //StartCoroutine(FinishCheckRes());
                        }
                        else
                        {
                            FinishCheckRes(false);
                            m_UpdateLoadingBar.SetStateString("更新失败");
                            //m_UpdateLoadingBar.SetStateString(StrDictionary.GetClientDictionaryString("#{2859}"));
                        }
                        
                        break;
                }
                //Debug.LogError(m_curUpdateHelper.CurUpdateStep.ToString());
                m_lastUpateStep = m_curUpdateHelper.CurUpdateStep;
            }

            if (m_lastUpateStep == UpdateHelper.UpdateStep.DownloadRes)
            {
                m_UpdateLoadingBar.SetLoadingPrecent(0.3f + 0.6f * ((float)m_curUpdateHelper.CurDownloadSize/(float)m_curUpdateHelper.DownloadTotalSize));
                //m_UpdateLoadingBar.SetStateString("正在下载资源 " + m_curUpdateHelper.CurDownloadSize / 1024 + "KB/" + m_curUpdateHelper.DownloadTotalSize / 1024 + " KB");
				m_UpdateLoadingBar.SetStateString("正在下载" + string.Format("{0}K/{1}K", m_curUpdateHelper.CurDownloadSize / 1024, m_curUpdateHelper.DownloadTotalSize/1024));
           
            } 
        }

//        if (m_CameraAni.IsPlaying("shexiangji_huangdong"))
//        {
//            if (!m_BeginChangeBlack)
//            {
//                m_BeginChangeBlack = true;
//            }
//        }
//
//        if (m_BeginChangeBlack)
//        {
//            if (!m_RoleClick)
//            {
//                if (m_RoleCreateShader.material.color.a < 1)
//                {
//                    m_RoleCreateShader.material.color += new Color(0, 0, 0, 0.02f);
//                }
//            }
//            else
//            {
//                if (m_RoleCreateShader.material.color.a > 0.12f)
//                {
//                    m_RoleCreateShader.material.color -= new Color(0, 0, 0, 0.02f);
//                }
//            }
//        }        
    }

    public void EnterGame()
    {

    }

	public void BeginCheckRes()
	{
        NGUILogHelpler.Log("BeginCheckRes", "PlatformHelper");
		bool isupdate = false;
		#if !UNITY_EDITOR
		isupdate=true;
		#endif

		if (false)
		{
			m_bDonwloadFile = false;
			//UIManager.CloseUI(UIInfo.ServerChoose);
			m_BackgroundTex.SetActive(false);
			m_UpdateLoadingBar.gameObject.SetActive(true);
			m_UpdateLoadingBar.SetLoadingPrecent(0);
			m_UpdateLoadingBar.SetStateString("");
			
			m_curUpdateHelper = gameObject.GetComponent<UpdateHelper>();
			if (null == m_curUpdateHelper)
			{
				m_curUpdateHelper = gameObject.AddComponent<UpdateHelper>();
			}
			
			if (null == m_curUpdateHelper)
			{
				LogModule.ErrorLog("add componet update helper fail");
			}
			else
			{
				
				m_curUpdateHelper.StartCheckRes(PlatformHelper.GetResDonwloadUrl());
				m_bUpdating = true;
			}
		}
		else
		{
			FinishCheckRes(true);
		}
	}
	
	public void LoginSuccess ()
	{
      
		m_CameraCreateRole.SetActive(false);
		//PlatformHelper.ShowAD(false);
		m_UpdateLoadingBar.gameObject.SetActive(false);
		if (m_bDonwloadFile)
		{
			// 重新加载表格
			GameManager.gameManager.TableManager.InitTable();
		}
		
		if (LoginData.loginRoleList.Count > 0)
		{
			LoginUILogic.Instance().EnterChooseRole();
		}
		else
		{
			LoginUILogic.Instance().EnterCreateRole();
		}
		

		PlatformHelper.OnAccountLogin(PlayerPreferenceData.LastServer.ToString(), LoginData.accountData.m_userID);
	}
	
	public void FinishCheckRes(bool bSuccess)
    {
		if(bSuccess)
		{
            //if (PlatformHelper.GetChannelType() == PlatformHelper.ChannelType.IOS_APPSTORE)
            //{
            //    StartCoroutine(RequestiADFlag());
            //}
			
			StartCoroutine(RequestServerList());
		}

	}
	public void EnterServerChoose()
    {
       
        UIManager.ShowUI(UIInfo.ServerChoose, OnShowServerChoose);
        UIManager.CloseUI(UIInfo.QueueWindow);
    }

    void OnShowServerChoose(bool bSucces, object param)
    {
        ShowFirstUI();
        
    }


	//====添加临时账号切换
	private int tempId;

    public void EnterAccount(string inPutId = "")  
    {
       NGUILogHelpler.Log("befor SetAccountData", "PlatformHelper");
       PlatformHelper.SetAccountData(LoginData.accountData);

       NetManager.SendUserLogin(LoginData.Ret_Login, false);

    }

    public void EnterChooseRole()//角色选择
    {
        //PlaneBack.SetActive(true);

		if (m_chooseEffect != null)
			m_chooseEffect.gameObject.SetActive (true);
		if (m_createEffect != null)
			m_createEffect.gameObject.SetActive (false);
        UIManager.CloseUI(UIInfo.QueueWindow);
		UIManager.ShowUI(UIInfo.RoleChoose,OnShowServerChoose);
	}
	
	void OnShowChooseRole(bool bSucces, object param)
	{
		m_UpdateLoadingBar.gameObject.SetActive(false);
		
	}

	public void EnterCreateRole()
    {
       // PlatformHelper.SendUserAction(UserBehaviorDefine.RoleCreateShow);
		if (m_chooseEffect != null)
						m_chooseEffect.gameObject.SetActive (false);
		if (m_createEffect != null)
			m_createEffect.gameObject.SetActive (true);
        UIManager.ShowUI(UIInfo.RoleCreate);
    }


    public float PlayCameraAni(string name)
    {
        m_CameraAni.Play(name);
        return m_CameraAni[name].length;
    }

    public bool IsPlayingCameraAni()
    {
        return m_CameraAni.isPlaying && !m_CameraAni.IsPlaying("shexiangji_huangdong");
    }


    public void CreateRoleClick(string strChooseRole)
    {
        for (int i = 0; i < m_RoleCollider.Count; ++i)
        {
            m_RoleCollider[i].enabled = false;
        }

        m_OpeningScene.SetActive(false);
        m_ObjCreateRoleBack.SetActive(true);
        m_RoleClick = true;

        for(int i=0; i<CreateRoleHideObjs.Length; i++)
        {
            CreateRoleHideObjs[i].SetActive(true);
        }
        if (strChooseRole.Contains("XiaoYao"))
        {
            m_XiaoYaoModel.SetActive(true);
            m_TianShanModel.SetActive(false);
            m_ShaoLinModel.SetActive(false);
            m_DaLiModel.SetActive(false);
            m_XiaoYaoModel.GetComponent<Animation>().Play("Xiaoyao_Selected");
            GameManager.gameManager.SoundManager.PlaySoundEffect(116);  //login_xiaoyao
            m_XiaoYaoModel.GetComponent<Animation>().PlayQueued("Xiaoyao_Selected_Loop");

            m_XiaoYaoEffect.ClearEffect();
            m_TianShanEffect.ClearEffect();
            m_ShaoLinEffect.ClearEffect();
            m_DaLiEffect.ClearEffect();
            m_XiaoYaoEffect.InitEffect(m_XiaoYaoEffect.gameObject);
            m_XiaoYaoEffect.PlayEffect(236);
            m_XiaoYaoSpecialEffect.SetActive(true);
        }
        else if (strChooseRole.Contains("Tianshan"))
        {
            m_XiaoYaoModel.SetActive(false);
            m_TianShanModel.SetActive(true);
            m_ShaoLinModel.SetActive(false);
            m_DaLiModel.SetActive(false);
            m_TianShanModel.GetComponent<Animation>().Play("Tianshan_Selected");
            GameManager.gameManager.SoundManager.PlaySoundEffect(114);  //login_tianshan
            m_TianShanModel.GetComponent<Animation>().PlayQueued("Tianshan_Selected_Loop");

            m_XiaoYaoEffect.ClearEffect();
            m_TianShanEffect.ClearEffect();
            m_ShaoLinEffect.ClearEffect();
            m_DaLiEffect.ClearEffect();
            m_TianShanEffect.InitEffect(m_TianShanEffect.gameObject);
            m_TianShanEffect.PlayEffect(232);
            m_TianShanEffect.PlayEffect(233);
            m_XiaoYaoSpecialEffect.SetActive(false);
        }
        else if (strChooseRole.Contains("ShaoLin"))
        {
            m_XiaoYaoModel.SetActive(false);
            m_TianShanModel.SetActive(false);
            m_ShaoLinModel.SetActive(true);
            m_DaLiModel.SetActive(false);
            m_ShaoLinModel.GetComponent<Animation>().Play("Shaolin_Selected");
            GameManager.gameManager.SoundManager.PlaySoundEffect(113);  //login_shaolin
            m_ShaoLinModel.GetComponent<Animation>().PlayQueued("Shaolin_Selected_Loop");

            m_XiaoYaoEffect.ClearEffect();
            m_TianShanEffect.ClearEffect();
            m_ShaoLinEffect.ClearEffect();
            m_DaLiEffect.ClearEffect();
            m_ShaoLinEffect.InitEffect(m_ShaoLinEffect.gameObject);
            m_ShaoLinEffect.PlayEffect(230);
            m_ShaoLinEffect.PlayEffect(231);
            m_XiaoYaoSpecialEffect.SetActive(false);
        }
        else if (strChooseRole.Contains("DaLi"))
        {
            m_XiaoYaoModel.SetActive(false);
            m_TianShanModel.SetActive(false);
            m_ShaoLinModel.SetActive(false);
            m_DaLiModel.SetActive(true);
            m_DaLiModel.GetComponent<Animation>().Play("Dali_Selected");
            GameManager.gameManager.SoundManager.PlaySoundEffect(115);  //login_dali
            m_DaLiModel.GetComponent<Animation>().PlayQueued("Dali_Selected_Loop");

            m_XiaoYaoEffect.ClearEffect();
            m_TianShanEffect.ClearEffect();
            m_ShaoLinEffect.ClearEffect();
            m_DaLiEffect.ClearEffect();
            m_DaLiEffect.InitEffect(m_DaLiEffect.gameObject);
            m_DaLiEffect.PlayEffect(234);
            m_DaLiEffect.PlayEffect(235);
            m_XiaoYaoSpecialEffect.SetActive(false);
        }
    }

    public void PlayRoleCreateOtherAni(string strChooseRole)
    {
        if (strChooseRole.Contains("XiaoYao"))
        {
            if (!m_XiaoYaoModel.GetComponent<Animation>().IsPlaying("Xiaoyao_Selected"))
            {
                m_XiaoYaoModel.GetComponent<Animation>().Play("Xiaoyao_Selected_Click");
                GameManager.gameManager.SoundManager.PlaySoundEffect(127);
                m_XiaoYaoModel.GetComponent<Animation>().PlayQueued("Xiaoyao_Selected_Loop");
            }            
        }
        else if (strChooseRole.Contains("Tianshan"))
        {
            if (!m_TianShanModel.GetComponent<Animation>().IsPlaying("Tianshan_Selected"))
            {
                m_TianShanModel.GetComponent<Animation>().Play("Tianshan_Selected_Click");
                GameManager.gameManager.SoundManager.PlaySoundEffect(125);
                m_TianShanModel.GetComponent<Animation>().PlayQueued("Tianshan_Selected_Loop");
            }
        }
        else if (strChooseRole.Contains("ShaoLin"))
        {
            if (!m_ShaoLinModel.GetComponent<Animation>().IsPlaying("Shaolin_Selected"))
            {
                m_ShaoLinModel.GetComponent<Animation>().Play("Shaolin_Selected_Click");
                GameManager.gameManager.SoundManager.PlaySoundEffect(124);
                m_ShaoLinModel.GetComponent<Animation>().PlayQueued("Shaolin_Selected_Loop");
            }
        }
        else if (strChooseRole.Contains("DaLi"))
        {
            if (!m_DaLiModel.GetComponent<Animation>().IsPlaying("Dali_Selected"))
            {
                m_DaLiModel.GetComponent<Animation>().Play("Dali_Selected_Click");
                GameManager.gameManager.SoundManager.PlaySoundEffect(126);
                m_DaLiModel.GetComponent<Animation>().PlayQueued("Dali_Selected_Loop");
            }
        }
    }

    public void CreateRoleReChoose()
    {
        for (int i = 0; i < m_RoleCollider.Count; ++i)
        {
            m_RoleCollider[i].enabled = true;
        }

        for (int i = 0; i < CreateRoleHideObjs.Length; i++)
        {
            CreateRoleHideObjs[i].SetActive(false);
        }

        m_OpeningScene.SetActive(true);
        m_ObjCreateRoleBack.SetActive(false);
        m_RoleClick = false;

        m_XiaoYaoModel.SetActive(false);
        m_TianShanModel.SetActive(false);
        m_ShaoLinModel.SetActive(false);
        m_DaLiModel.SetActive(false);
        m_XiaoYaoModel.SetActive(true);
        m_TianShanModel.SetActive(true);
        m_ShaoLinModel.SetActive(true);
        m_DaLiModel.SetActive(true);

        m_XiaoYaoEffect.ClearEffect();
        m_TianShanEffect.ClearEffect();
        m_ShaoLinEffect.ClearEffect();
        m_DaLiEffect.ClearEffect();
        m_XiaoYaoSpecialEffect.SetActive(false);
    }

    public void ShowChooseRoleCamera()
    {
        m_ModelChooseRole.SetActive(true);
        CreateRoleChar.SetActive(false);
        m_BackgroundTex.SetActive(false);
        m_CameraCreateRole.SetActive(false);
        m_CameraChooseRole.SetActive(true);
    }

    public void ShowCreateRoleCamera()
    {
        m_ModelChooseRole.SetActive(false);
        CreateRoleChar.SetActive(true);
        m_BackgroundTex.SetActive(false);
        m_CameraCreateRole.SetActive(false);
        m_CameraChooseRole.SetActive(true);
    }

    public void ShowFirstUI()
    {
        m_CameraChooseRole.SetActive(false);
		m_UpdateLoadingBar.gameObject.SetActive(false);
        m_BackgroundTex.SetActive(true);
       
    }

    public void ShowModel(int nType, int nModelVisualID, int nWeaponID, int nWeaponEffectGem)
    {
        m_RoleType = nType;
        m_RoleModelVisualID = nModelVisualID;
        m_RoleWeaponID = nWeaponID;
        m_RoleWeaponEffectGem = nWeaponEffectGem;

        Tab_ItemVisual tabItemVisual = TableManager.GetItemVisualByID(m_RoleModelVisualID, 0);
        if (tabItemVisual == null)
        {
            tabItemVisual = TableManager.GetItemVisualByID(GlobeVar.DEFAULT_VISUAL_ID, 0);
            if (tabItemVisual == null)
            {
                return;
            }
        }

        int nCharModelID = GlobeVar.INVALID_ID;
        if (m_RoleType == (int)CharacterDefine.PROFESSION.SHAOLIN)
        {
            nCharModelID = tabItemVisual.CharModelIDShaoLin;
        }
        else if (m_RoleType == (int)CharacterDefine.PROFESSION.TIANSHAN)
        {
            nCharModelID = tabItemVisual.CharModelIDTianShan;
        }
        else if (m_RoleType == (int)CharacterDefine.PROFESSION.DALI)
        {
            nCharModelID = tabItemVisual.CharModelIDDaLi;
        }
        else if (m_RoleType == (int)CharacterDefine.PROFESSION.XIAOYAO)
        {
            nCharModelID = tabItemVisual.CharModelIDXiaoYao;
        }

        Tab_CharModel tabCharModel = TableManager.GetCharModelByID(nCharModelID, 0);
        if (tabCharModel == null)
        {
            return;
        }

        Singleton<ObjManager>.Instance.ReloadModel(m_ModelChooseRole, tabCharModel.ResPath, OnLoadModel, tabCharModel.AnimPath);
    }

    public void OnLoadModel(string modelName, GameObject resObj, object param1, object param2, object param3 = null)
    {
        if (null == resObj)
        {
            return;
        }

        if (null == param1 || null == param2)
        {
            return;
        }

        GameObject objRoot = (GameObject)param1;
        string animationPath = (string)param2;

        if(null == objRoot)
        {
            return;
        }

        GameObject model = (GameObject)GameObject.Instantiate(resObj);
        if (null == model)
        {
            return;
        }
        else
        {
            model.name = "Model";
        }

        Obj_Character curCharacter = objRoot.GetComponent<Obj_Character>();
        if (curCharacter == null)
        {
            curCharacter = objRoot.AddComponent<Obj_Character>();
        }

        AnimationLogic curAnimLogic = objRoot.GetComponent<AnimationLogic>();
        if (curAnimLogic == null)
        {
            curAnimLogic = objRoot.AddComponent<AnimationLogic>();
        }
        curCharacter.AnimLogic = curAnimLogic;

        Singleton<ObjManager>.Instance.ReloadModel(objRoot, model, animationPath);
		if (m_RoleType == (int)CharacterDefine.PROFESSION.SHAOLIN) 
		{
            objRoot.transform.localPosition = new Vector3(-0.78f, 6.85f, -0.88f);
		}
		else
		{
            objRoot.transform.localPosition = new Vector3(-0.98f, 6.850f, -0.88f);
		}
        objRoot.transform.localRotation = Quaternion.Euler(new Vector3(0, 226f, 0));
       // curAnimLogic.Play(0);
		objRoot.transform.localScale = new Vector3 (0.9f, 0.9f, 0.9f);
        ShowWeapon();

//         GameObject charRoot = param1 as GameObject;
//         GameObject newObj = GameObject.Instantiate(resObj) as GameObject;
//         newObj.name = "Model";
//         Singleton<ObjManager>.Instance.ReloadModel(charRoot, newObj);
// 
//         Tab_CharModel curTabCharModel = param3 as Tab_CharModel;
// 
    }

    public void ShowWeapon()
    {
        bool defaultVisual = false;
        Tab_ItemVisual tabItemVisual = null;

        Tab_EquipAttr tabEquipAttr = TableManager.GetEquipAttrByID(m_RoleWeaponID, 0);
        if (tabEquipAttr == null)
        {
            defaultVisual = true;
        }
        else
        {
            tabItemVisual = TableManager.GetItemVisualByID(tabEquipAttr.ModelId, 0);
            if (tabItemVisual == null)
            {
                defaultVisual = true;
            }
        }

        if (defaultVisual)
        {
			tabItemVisual = TableManager.GetItemVisualByID(m_RoleWeaponID, 0);//GlobeVar.DEFAULT_VISUAL_ID
			if (tabItemVisual == null)
            {
                return;
            }
        }

        int nWeaponModelID = GlobeVar.INVALID_ID;
        if (m_RoleType == (int)CharacterDefine.PROFESSION.SHAOLIN)
        {
            nWeaponModelID = tabItemVisual.WeaponModelIDShaoLin;
        }
        else if (m_RoleType == (int)CharacterDefine.PROFESSION.TIANSHAN)
        {
            nWeaponModelID = tabItemVisual.WeaponModelIDTianShan;
        }
        else if (m_RoleType == (int)CharacterDefine.PROFESSION.DALI)
        {
            nWeaponModelID = tabItemVisual.WeaponModelIDDaLi;
        }
        else if (m_RoleType == (int)CharacterDefine.PROFESSION.XIAOYAO)
        {
            nWeaponModelID = tabItemVisual.WeaponModelIDXiaoYao;
        }

        Tab_WeaponModel tabWeaponModel = TableManager.GetWeaponModelByID(nWeaponModelID, 0);
        if (tabWeaponModel == null)
        {
            return;
        }

        if (m_RoleType == (int)CharacterDefine.PROFESSION.TIANSHAN)
        {
            string resWeaponLeft = tabWeaponModel.ResPath + "_L";
            string resWeaponRight = tabWeaponModel.ResPath + "_R";

            Singleton<ObjManager>.GetInstance().ReloadWeapon(m_ModelChooseRole,
                resWeaponLeft,
                OnLoadWeapon,
                "Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 L Clavicle/Bip001 L UpperArm/Bip001 L Forearm/Bip001 L Hand/HH_weaponHandLf");//"Weapon_L");

            Singleton<ObjManager>.GetInstance().ReloadWeapon(m_ModelChooseRole,
                resWeaponRight,
                OnLoadWeapon,
               	"Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/HH_weaponHandRt"); //"Weapon_R");
        }
        //else if (m_RoleType == (int)CharacterDefine.PROFESSION.XIAOYAO)
        //{
        //    Singleton<ObjManager>.GetInstance().ReloadWeapon(m_ModelChooseRole,
        //        tabWeaponModel.ResPath,
       //         OnLoadWeapon,
		//	   	"Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/HH_weaponHandLf");//Weapon_L
       // }
        else
        {
			//剑客初始也要把剑背在背上

			if(m_RoleType == (int)CharacterDefine.PROFESSION.SHAOLIN||m_RoleType== (int)CharacterDefine.PROFESSION.XIAOYAO)
			{
				Singleton<ObjManager>.GetInstance().ReloadWeapon(m_ModelChooseRole,
				                                                 tabWeaponModel.ResPath,
				                                                 OnLoadWeapon,
				                                                 "Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/HH_weaponback");//Weapon_R

			}
			else

            Singleton<ObjManager>.GetInstance().ReloadWeapon(m_ModelChooseRole,
                tabWeaponModel.ResPath,
                OnLoadWeapon,
                "Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/HH_weaponHandRt");//Weapon_R
        }
    }

    public void OnLoadWeapon(string modelName, GameObject resObj, object param1, object param2, object param3 = null)
    {
        if (param1 == null || param2 == null)
        {
            return;
        }

        GameObject objRoot = (GameObject)param1;
        string strWeaponPoint = (string)param2;

        if(null == objRoot)
        {
            return;
        }
        GameObject weaponModel = (GameObject)GameObject.Instantiate(resObj);
        if (null == weaponModel)
        {
            return;
        }

        Transform modelTrans = objRoot.transform.FindChild("Model");
        if (modelTrans == null)
        {
            return;
        }

        //Transform modelallTrans = modelTrans.FindChild("all");
       // if (modelallTrans == null)
       // {
       //     return;
       // }

        //Transform weaponParent = modelallTrans.FindChild(strWeaponPoint);
		Transform weaponParent = modelTrans.FindChild(strWeaponPoint);
        if (weaponParent == null)
        {
            return;
        }

		foreach(Transform tr in weaponParent)
		{
			Destroy(tr.gameObject);
		}

        Transform effectParent;
       // if (m_RoleType == (int)CharacterDefine.PROFESSION.XIAOYAO)
       // {
       //    effectParent = weaponParent.FindChild("Weapon_R");
       //     if (effectParent == null)
       //   {
       //         return;
       //     }
        //}
        //else
        //{
         effectParent = weaponParent;
        //}

        for (int i = 0; i < effectParent.childCount; ++i)
        {
            Transform child = effectParent.GetChild(i);
            if (null != child && null != child.gameObject && child.gameObject.name != "EffectPoint")
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        Quaternion weaponRotation = weaponModel.transform.localRotation;
        weaponModel.transform.parent = weaponParent;
        weaponModel.transform.localPosition = Vector3.zero;

		//Tab_WeaponModel weapModelForScale = TableManager.GetWeaponModelByID (nWeaponModelID,0);
		//Vector3 scaleV3 = weapModelForScale == null ? Vector3.one :  new Vector3(weapModelForScale.Scale,weapModelForScale.Scale,weapModelForScale.Scale);
		
		weaponModel.transform.localScale = Vector3.one;// scaleV3; //Vector3.one;
		//weaponModel.transform.localScale = Vector3.one;
        weaponModel.transform.localRotation = weaponRotation;
        weaponModel.layer = objRoot.layer;

        // 加载武器宝石特效
        EffectLogic effectLogic = effectParent.gameObject.GetComponent<EffectLogic>();
        if (null == effectLogic)
        {
            effectLogic = effectParent.gameObject.AddComponent<EffectLogic>();
            effectLogic.InitEffect(effectParent.gameObject);
        }
        if (null != effectLogic)
        {
            Tab_GemAttr tabGemAttr = TableManager.GetGemAttrByID(m_RoleWeaponEffectGem, 0);
            if (tabGemAttr != null)
            {
                Tab_Effect tabEffect = TableManager.GetEffectByID(tabGemAttr.EffectID, 0);
                if (tabEffect != null)
                {
					effectLogic.PlayEffect(tabGemAttr.EffectID,null,weaponModel);
                }
            }
        }
    }

    public static void RequestRandomName()
    {
        CG_REQ_RANDOMNAME packet = (CG_REQ_RANDOMNAME)PacketDistributed.CreatePacket(MessageID.PACKET_CG_REQ_RANDOMNAME);
        packet.SetNone(0);
        packet.SendPacket();
    }

    public void ShowProfessionEffect(bool bShow)
    {
        m_RoleProfessionEffect.SetActive(bShow);
    }
	public static void DoUpdateApp()
	{
#if UNITY_ANDROID
		LoginUILogic.Instance().StartCoroutine(RequstUpdateAppUrl(PlatformHelper.GetUpdateAppUrl()));
#else
		Application.OpenURL(PlatformHelper.GetUpdateAppUrl());
#endif
	}

#if UNITY_ANDROID
	static IEnumerator RequstUpdateAppUrl(string url)
	{
		WWW wwwData = new WWW(DownloadHelper.AddTimestampToUrl(url));
		yield return wwwData;
		
		if (!string.IsNullOrEmpty(wwwData.text))
		{
			AndroidHelper.doSdk("showUpdate",wwwData.text);
		}
	}
#endif
}
