using UnityEngine;
using System.Collections;
using Module.Log;
using GCGame.Table;
using Games.GlobeDefine;

public class LoadingWindow : MonoBehaviour {

    public ClipSlider m_ClipSlider;

    public UILabel m_xiaozhitiao;

	private static AssetBundle m_lastSceneBundle = null;
	public static Games.GlobeDefine.GameDefine_Globe.SCENE_DEFINE nextSceneID;

    public static void LoadScene(Games.GlobeDefine.GameDefine_Globe.SCENE_DEFINE secenID)
    {
		/*
        if (GameManager.gameManager.RunningScene == (int)secenID)
        {
            UIManager.CloseUI(UIInfo.MessageBox);
            Singleton<ObjManager>.Instance.CleanSceneObj();
            NetWorkLogic.GetMe().CanProcessPacket = true;
            NetWorkLogic.GetMe().ForceProcessPakcet = true;
            return;
        }
		*/
        if (secenID == Games.GlobeDefine.GameDefine_Globe.SCENE_DEFINE.SCENE_LOADINGSCENE)
        {
            LogModule.WarningLog("loading scene can not be loaded");
            return;
        }

        if (secenID == Games.GlobeDefine.GameDefine_Globe.SCENE_DEFINE.SCENE_YANMENGUANWAI)
        {
           // PlatformHelper.SendUserAction(UserBehaviorDefine.EnterFirstScene);
        }

        GameManager.gameManager.RunningScene = (int)Games.GlobeDefine.GameDefine_Globe.SCENE_DEFINE.SCENE_LOADINGSCENE;
        LoadingWindow.nextSceneID = secenID;
        Application.LoadLevel("LoadingScene");
       
    }

    

	// Use this for initialization
	void Start () {

        int nIndex = Random.Range(1, 10000);
        int nTotalCount = TableManager.GetXiaozhitiao().Count;
        if (nTotalCount >= 1)
        {
            nIndex = nIndex % nTotalCount;
            Tab_Xiaozhitiao xztTab = TableManager.GetXiaozhitiaoByID(nIndex, 0);
            if (xztTab != null)
            {
                m_xiaozhitiao.text = StrDictionary.GetClientString_WithNameSex(xztTab.Content);
            }
        }

        StartCoroutine(LoadScene());
	}

    void OnDestroy()
    {
        if (null != m_lastSceneBundle)
        {
            m_lastSceneBundle.Unload(false);
            m_lastSceneBundle = null;
        }
    }
	
    IEnumerator LoadScene()
    {
        m_ClipSlider.SetNextProgress(1f);

		BundleManager.ReleaseSingleBundle();
		BundleManager.CleanBundleLoadQueue();
        BundleManager.ReleaseUIRefBundle();
        BundleManager.DoUnloadUnuseBundle();

		if(null != m_lastSceneBundle)
		{
			m_lastSceneBundle.Unload(false);
			m_lastSceneBundle = null;
		}

        if (nextSceneID != (int)GameDefine_Globe.SCENE_DEFINE.SCENE_LOGIN)
        {
            m_ClipSlider.m_curSpeed = 0.2f;
            BundleManager.ReleaseLoginBundle();
            yield return Resources.UnloadUnusedAssets();
            System.GC.Collect();
			yield return StartCoroutine(BundleManager.LoadMainUI());   
           // yield return StartCoroutine(BundleManager.LoadUI(UIInfo.JoyStickRoot, null, null, null));
            DamageBoardManager.PreloadDamageBoard();            
        }
        else
        {
			BundleManager.ReleaseGroupBundle();
            yield return Resources.UnloadUnusedAssets();
            System.GC.Collect();
            yield return StartCoroutine(BundleManager.LoadLoginUI());   
        }
        if (nextSceneID == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_LOGIN)
        {
            GameManager.gameManager.RunningScene = (int)LoadingWindow.nextSceneID;
            AsyncOperation curAsync = Application.LoadLevelAsync((int)LoadingWindow.nextSceneID);
            while (!curAsync.isDone)
            {
                //SetProgress(curAsync.progress);

                m_ClipSlider.SetNextProgress(0.9f + 0.1f * curAsync.progress);
                //loadingSlider.value = curAsync.progress;
                yield return null;
            }
            m_ClipSlider.SetProgress(1f);
            //loadingSlider.value = 1;
        }
        else
        {
            m_ClipSlider.SetNextProgress(0.7f);
            yield return StartCoroutine(BundleManager.LoadScene(GameDefine_Globe.SceneName[(int)LoadingWindow.nextSceneID], LoadSceneFinish));
        }
       
    }
    


    void LoadSceneFinish(string sceneName, AssetBundle SceneBundle)
    {

        if (string.IsNullOrEmpty(sceneName))
        {
            LogModule.ErrorLog("load scene fail");
        }
        else
        {
			m_lastSceneBundle = SceneBundle;
            GameManager.gameManager.RunningScene= (int)LoadingWindow.nextSceneID;
            AsyncOperation curAsync = Application.LoadLevelAsync(sceneName);
        }
    }
}
