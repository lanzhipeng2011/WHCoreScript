/********************************************************************
	
	
	purpose:	Bundle加载 注意，在Bundle加载时AssetBundle.Unload(false)不能直接使用
            需要使用CacheBundle
*********************************************************************/
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Games.GlobeDefine;
using Module.Log;
using System.IO;

public class BundleManager 
{

	public static string LocalLoadPath = Application.streamingAssetsPath;
	public static string FolderLoginUI = "Login";
	public static string FolderMainUI = "Main";
    public static string FolderCommonUI = "Common";

	public static string UIFontName = "font.data";
	public static string UIBaseName =  "baseui.data";
	public static string UISoundName = "uisound.data";
    public static string UILoginName = "login.data";
    public static string UIMainName = "main.data";
    public static string UICommonName = "common.data";

	public static string PathUICommon = "/UI/CommonData";
	public static string PathUIPrefab = "/UI/Prefab";
    public static string PathModelPrefab = "/Model";
    public static string PathEffectPrefab = "/Effect";
    public static string PathSoundPrefab = "/Sounds";
    public static string PathAnimationAsset = "/Animations";
    public static string PathTableData = "/Tables";
    public static string PathSceneData = "/Scene";

    public static string AppOutputPath = Application.streamingAssetsPath;
    public static string DevelopOutputPath = Application.dataPath + "/BundleAssets";
	public static string UpdateOutputPath = Application.dataPath + "/../Release/ResData/StreamingAssets";

    // 下载更新后存放的路径
    public static string LocalPathRoot
    {
        get
        {
            return Application.persistentDataPath + "/ResData";
        }
    }


	public static string m_loadUrlHead = "file://";

    // 各种常驻内存容器
    private static List<AssetBundle> m_BundleListFont = new List<AssetBundle>();
    private static List<AssetBundle> m_BundleListCommon = new List<AssetBundle>();
    private static List<AssetBundle> m_BundleListLoginUI = new List<AssetBundle>();
    private static List<AssetBundle> m_BundleListMainUI = new List<AssetBundle>();

    private static List<string> m_BundleUILoadingList = new List<string>();


    private static int m_cacheBundleSize = 0;           // 距离上次unload缓存的大小
    private static int m_cacheBundleAddCount = 0;       // 缓存次数
    private const int m_cacheBundleMax = 500 * 1000;    // 缓存大小上限     
    private const int m_cacheBundleAddMax = 10;         // 缓存次数上限

    // 缓存Bundle
    private static void CacheBundle(WWW wwwBundle)
    {
        if (null == wwwBundle)
        {
            return;
        }

        if (wwwBundle.assetBundle != null)
        {
            //m_cacheBundleSize += wwwBundle.bytesDownloaded;
            //m_cacheBundleAddCount++;
            LogModule.DebugLog("add bundle size" + m_cacheBundleSize + "  name: " + wwwBundle.assetBundle.name);
            wwwBundle.assetBundle.Unload(false);
        }
    }

    // 尝试清除缓存
    public static void TryUnloadUnuseBundle()
    {
        if (m_cacheBundleSize > m_cacheBundleMax || m_cacheBundleAddCount > m_cacheBundleAddMax)
        {
            LogModule.DebugLog("begin unload unuse ui");
            Resources.UnloadUnusedAssets();
            GC.Collect();
            m_cacheBundleSize = 0;
            m_cacheBundleAddCount = 0;
        }
    }

    // 清除缓存
    public static void DoUnloadUnuseBundle()
    {
        Resources.UnloadUnusedAssets();
        GC.Collect();
        m_cacheBundleSize = 0;
        m_cacheBundleAddCount = 0;
    }

 #region UI Bundle
    // 存储捆绑打包类型，当主资源请求加载时加载Bundle
    private static Dictionary<string, AssetBundle> m_BundleDicUIGroup = new Dictionary<string, AssetBundle>();
    private static Dictionary<string, AssetBundle> m_BundleDicUIRef = new Dictionary<string, AssetBundle>();

	public delegate void LoadBundleFinish(UIPathData uiData,GameObject retObj, object param1, object param2);

    public static string GetBundleLoadUrl(string subFolder, string localName)
    {
        if (false)
        {
            string localPath = LocalPathRoot + subFolder + "/" + localName;

            if (File.Exists(localPath))
            {
                return m_loadUrlHead + localPath;
            }
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        return Application.streamingAssetsPath + subFolder + "/" + localName;
#elif UNITY_EDITOR
        return m_loadUrlHead + BundleManager.DevelopOutputPath + subFolder + "/" + localName;
#else
        return BundleManager.m_loadUrlHead + Application.streamingAssetsPath + subFolder + "/" + localName;
#endif
    }


    public static IEnumerator LoadLoginUI()
    {
		yield return null;
	//	by dsy  暂时不用，最终发布的时候解开

//        if (m_BundleListLoginUI.Count == 0)
//        {
//            WWW www = new WWW(GetBundleLoadUrl(BundleManager.PathUICommon, BundleManager.UILoginName));
//            yield return www;
//            if (null != www.assetBundle)
//            {
//                www.assetBundle.LoadAll();
//                m_BundleListLoginUI.Add(www.assetBundle);
//            }
//        }
    }

    public static IEnumerator LoadFontUI()
    {

		//yield return null;
        // 加载字体  by dsy  暂时不用，最终发布的时候解开
        if (m_BundleListFont.Count == 0)
        {
            WWW www = new WWW(GetBundleLoadUrl(BundleManager.PathUICommon, BundleManager.UIFontName));
            yield return www;
            if (null != www.assetBundle)
            {
                www.assetBundle.LoadAll();
                m_BundleListFont.Add(www.assetBundle);
            }
        }
    }

    public static IEnumerator LoadCommonUI()
    {
	//	yield return null;
		//        /加载commonui  by dsy  暂时不用，最终发布的时候解开
        if (m_BundleListCommon.Count == 0)
        {
            WWW www = new WWW(GetBundleLoadUrl(BundleManager.PathUICommon, BundleManager.UICommonName));
            yield return www;
            if (null != www.assetBundle)
            {
                www.assetBundle.LoadAll();
                m_BundleListCommon.Add(www.assetBundle);
            }
        }
    }

    public static IEnumerator LoadMainUI()
    {
		yield return null;
		// 加载mainui  by dsy  暂时不用，最终发布的时候解开
//        if (m_BundleListMainUI.Count == 0)
//        {
//            WWW www = new WWW(GetBundleLoadUrl(BundleManager.PathUICommon, BundleManager.UIMainName));
//            yield return www;
//            if (null != www.assetBundle)
//            {
//                www.assetBundle.LoadAll();
//                m_BundleListMainUI.Add(www.assetBundle);
//            }
//        }
    }


    public static IEnumerator LoadUI(UIPathData uiData, LoadBundleFinish delFinish, object param1, object param2)
	{
		//yield return null;
		// by dsy  暂时不用，最终发布的时候解开
        if (m_BundleListFont.Count == 0)
        {
            WWW www = new WWW(GetBundleLoadUrl(BundleManager.PathUICommon, BundleManager.UIFontName));
            yield return www;
            if (null != www.assetBundle)
            {
                www.assetBundle.LoadAll();
                m_BundleListFont.Add(www.assetBundle); 
            }
        }

        // 加载通用资源
        if (m_BundleListCommon.Count == 0)
        {
            WWW www = new WWW(GetBundleLoadUrl(BundleManager.PathUICommon, BundleManager.UICommonName));
            yield return www;
            if (null != www.assetBundle)
            {
                www.assetBundle.LoadAll();
                m_BundleListCommon.Add(www.assetBundle);
            }
        }

        // 加载Login资源
        if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_LOGIN)
        {
            if (m_BundleListLoginUI.Count == 0)
            {
                WWW www = new WWW(GetBundleLoadUrl(BundleManager.PathUICommon, BundleManager.UILoginName));
                yield return www;
                if (null != www.assetBundle)
                {
                    www.assetBundle.LoadAll();
                    m_BundleListLoginUI.Add(www.assetBundle);
                }
            }
        }
        else
        {
            if (m_BundleListMainUI.Count == 0)
            {
                WWW www = new WWW(GetBundleLoadUrl(BundleManager.PathUICommon, BundleManager.UIMainName));
                yield return www;
                if (null != www.assetBundle)
                {
                    www.assetBundle.LoadAll();
                    m_BundleListMainUI.Add(www.assetBundle);
                }
            }
        }
      
        // 加载当前UI

        GameObject retObj = null;
        WWW wwwUI;
        if (uiData.uiGroupName != null)
        {
            // 分组缓存，不释放
            while (m_BundleUILoadingList.Contains(uiData.uiGroupName))
            {
                yield return null;
            }
            m_BundleUILoadingList.Add(uiData.uiGroupName);

            if (m_BundleDicUIGroup.ContainsKey(uiData.uiGroupName))
            {
                retObj = m_BundleDicUIGroup[uiData.uiGroupName].Load(uiData.name, typeof(GameObject)) as GameObject;
            }
            else
            {
                LogModule.DebugLog("load asset :" + uiData.name + "UI gropu:" + uiData.uiGroupName);
                wwwUI = new WWW(GetBundleLoadUrl(BundleManager.PathUIPrefab, uiData.uiGroupName + ".data"));
                yield return wwwUI;
                if (null != wwwUI.assetBundle)
                {
                    retObj = wwwUI.assetBundle.Load(uiData.name, typeof(GameObject)) as GameObject;
                    m_BundleDicUIGroup.Add(uiData.uiGroupName, wwwUI.assetBundle);
                }
                else
                {
                    LogModule.ErrorLog("load assetbundle none :" + uiData.uiGroupName);
                }
            }

            m_BundleUILoadingList.Remove(uiData.uiGroupName);
        }
        else if (uiData.uiType == UIPathData.UIType.TYPE_BASE || uiData.uiType == UIPathData.UIType.TYPE_ITEM || uiData.uiType == UIPathData.UIType.TYPE_MESSAGE)
        {
            // 分组缓存，不释放
//             while (m_BundleUILoadingList.Contains(uiData.name))
//             {
//                 yield return null;
//             }
//             m_BundleUILoadingList.Add(uiData.name);


            //LogModule.DebugLog("load asset :" + uiData.name + "UI gropu:" + uiData.uiGroupName);
            wwwUI = new WWW(GetBundleLoadUrl(BundleManager.PathUIPrefab, uiData.name + ".data"));
            yield return wwwUI;
            if (null != wwwUI.assetBundle)
            {
                retObj = wwwUI.assetBundle.Load(uiData.name, typeof(GameObject)) as GameObject;
                CacheBundle(wwwUI);
               
            }
            else
            {
                LogModule.ErrorLog("load assetbundle none :" + uiData.uiGroupName);
            }

            //m_BundleUILoadingList.Remove(uiData.name);
        }
        else
        {
            if (m_BundleDicUIRef.ContainsKey(uiData.name))
            {
                LogModule.ErrorLog("load ui data fail: already load a same bundle " + uiData.name);
            }
            else
            {
                //LogModule.DebugLog("load asset :" + uiData.name + "UI gropu:" + uiData.uiGroupName);
                wwwUI = new WWW(GetBundleLoadUrl(BundleManager.PathUIPrefab, uiData.name + ".data"));
                yield return wwwUI;
                if (null != wwwUI.assetBundle)
                {
                    retObj = wwwUI.assetBundle.Load(uiData.name, typeof(GameObject)) as GameObject;
                    CacheBundle(wwwUI);
                    //m_BundleDicUIRef.Add(uiData.name, wwwUI.assetBundle);
                }
                else
                {
                    LogModule.ErrorLog("load assetbundle none :" + uiData.uiGroupName);
                }
            }
        }
       
        if (null != delFinish) delFinish(uiData, retObj, param1, param2);
	}

    public static void ReleaseLoginBundle()
    {
        for (int i = 0; i < m_BundleListLoginUI.Count; ++i )
        {
            m_BundleListLoginUI[i].Unload(true);
        }
        m_BundleListLoginUI.Clear();
    }

    public static GameObject GetGroupUIItem(UIPathData data)
    {
        if (null == data.uiGroupName)
        {
            return null;
        }
        if (m_BundleDicUIGroup.ContainsKey(data.uiGroupName))
        {
            return m_BundleDicUIGroup[data.uiGroupName].Load(data.name) as GameObject;
        }

        return null;
    }


    public static void ReleaseGroupBundle()
    {
        foreach (KeyValuePair<string, AssetBundle> loginBundle in m_BundleDicUIGroup)
        {
            loginBundle.Value.Unload(true);
        }
        m_BundleUILoadingList.Clear();
        m_BundleDicUIGroup.Clear();
    }

    public static void ReleaseUIRefBundle()
    {
        foreach (KeyValuePair<string, AssetBundle> refBundle in m_BundleDicUIRef)
        {
            refBundle.Value.Unload(true);
        }
        m_BundleDicUIRef.Clear();
    }

    // 当UI销毁时调用，如果未调用可能引起资源泄露
    public static void OnUIDestroy(string uiName)
    {
        if (m_BundleDicUIRef.ContainsKey(uiName))
        {
            LogModule.DebugLog("remove ui " + uiName);
            m_BundleDicUIRef[uiName].Unload(true);
            m_BundleDicUIRef.Remove(uiName);
        }

        TryUnloadUnuseBundle();
    }

#endregion
    // 单体资源缓存，如果有缓存，则不从文件加载
    private static Dictionary<string, GameObject> m_dicSingleBundleCache = new Dictionary<string, GameObject>();
    // 单体资源引用计数，如果超过1，则将此资源放入缓存
    private static Dictionary<string, int> m_dicSingleBundleRef = new Dictionary<string, int>();

    public static void ReleaseSingleBundle()
    {
        m_dicSingleBundleCache.Clear();
    }

    static void ChangeShader(Transform obj)
    {

        if (obj.renderer != null && obj.renderer.material != null)
        {
            Material sm = obj.renderer.material;
            var shaderName = sm.shader.name;
            if (!string.IsNullOrEmpty(shaderName))
            {
                var newShader = Shader.Find(shaderName);
                if (newShader != null)
                {
                    sm.shader = newShader;
                }
                else
                {
                    LogModule.WarningLog("unable to refresh shader: " + shaderName + " in material " + sm.name);
                }
            }
        }

        for (int i = 0; i < obj.childCount; i++)
        {
            ChangeShader(obj.transform.GetChild(i));
        }
    }

    public delegate void LoadSingleFinish(string modelName, GameObject resObj, object param1, object param2, object param3 = null);

    private static bool LoadFromCache(string bundlePath, string bundleName, LoadSingleFinish delFinish, object param1, object param2, object param3)
    {
        if (m_dicSingleBundleCache.ContainsKey(bundlePath))
        {
            if (null != delFinish) delFinish(bundleName, m_dicSingleBundleCache[bundlePath], param1, param2, param3);
            return true;
        }

        if (m_dicSingleBundleRef.ContainsKey(bundlePath))
        {
            m_dicSingleBundleRef[bundlePath]++;
        }
        else
        {
            m_dicSingleBundleRef.Add(bundlePath, 1);
        }

        return false;
    }

    private static void ProcessLoad(WWW www, string bundlePath, string bundleName, LoadSingleFinish delFinish, object param1, object param2, object param3)
    {
        GameObject retObj = null;
        if (null != www.assetBundle)
        {
            retObj = www.assetBundle.mainAsset as GameObject;
#if UNITY_EDITOR
            ChangeShader(retObj.transform);
#endif
            CacheBundle(www);
            if (m_dicSingleBundleRef.ContainsKey(bundlePath) && m_dicSingleBundleRef[bundlePath] > 1)
            {
                m_dicSingleBundleCache.Add(bundlePath, retObj);
                m_dicSingleBundleRef.Remove(bundlePath);
            }
        }
        else
        {
            LogModule.ErrorLog("load single assetbundle none :" + bundleName);
        }


        if (null != delFinish) delFinish(bundleName, retObj, param1, param2, param3);
    }

    // 加载声音，不缓存
    private static void ProcessLoadSound(WWW www, string bundlePath, string bundleName, LoadSoundFinish delFinish, object param1, object param2, object param3)
    {
        AudioClip retObj = null;
        if (null != www.assetBundle)
        {
            retObj = www.assetBundle.mainAsset as AudioClip;
            CacheBundle(www);
        }
        else
        {
            LogModule.ErrorLog("load single assetbundle none :" + bundleName);
        }

        if (null != delFinish) delFinish(bundleName, retObj, param1, param2, param3);
    }

    // 加载动作，不缓存
    private static void ProcessLoadAnimation(WWW www, string bundlePath, string bundleName, LoadAnimationFinish delFinish)
    {
        AnimationClip retObj = null;
        if (null != www.assetBundle)
        {
            retObj = www.assetBundle.mainAsset as AnimationClip;
            CacheBundle(www);
        }
        else
        {
            LogModule.ErrorLog("load single assetbundle none :" + bundleName);
        }

        if (null != delFinish) delFinish(bundleName, retObj);
    }

    /*
    // 加载模型
    // 此方法暂弃用，用LoadModelInQueue代替
    public static IEnumerator LoadModel(string modelName, LoadSingleFinish delFinish, object param1, object param2, object param3)
    {
        string loadPath = GetBundleLoadUrl(BundleManager.PathModelPrefab, modelName + ".data");
        if (!LoadFromCache(loadPath, modelName, delFinish, param1, param2, param3))
        {
            WWW www = new WWW(loadPath);
            yield return www;
            ProcessLoad(www, loadPath, modelName, delFinish, param1, param2, param3);
        }
    }
    
    // 加载特效
    // 此方法暂弃用，用LoadEffectInQueue代替
    private static bool m_bLoadEffectCommonShader = false;
    public static IEnumerator LoadEffect(string modelName, LoadSingleFinish delFinish, object param1 = null, object param2 = null)
    {
        if (!m_bLoadEffectCommonShader)
        {
            string shaderPath = GetBundleLoadUrl(BundleManager.PathEffectPrefab, "effect_shader_common.data");
            WWW www = new WWW(shaderPath);
            yield return www;
            if (null != www.assetBundle)
            {
                www.assetBundle.LoadAll();
            }
            else
            {
                LogModule.ErrorLog("can not find effect shader");
            }

            m_bLoadEffectCommonShader = true;
        }

        string loadPath = GetBundleLoadUrl(BundleManager.PathEffectPrefab, modelName + ".data");
        if (!LoadFromCache(loadPath, modelName, delFinish, param1, param2, null))
        {
            WWW www = new WWW(loadPath);
            yield return www;
            ProcessLoad(www, loadPath, modelName, delFinish, param1, param2, null);
        }
        
    }
     */ 

    public delegate void LoadSoundFinish(string modelName, AudioClip audioClip, object param1, object param2, object param3 = null);
    // 加载声音
    public static IEnumerator LoadSound(string soundPath, LoadSoundFinish delFinish, object param1 = null, object param2 = null, object param3 = null)
    {
        // 表里路径包含了Sounds文件夹名称
        string loadPath = GetBundleLoadUrl("", soundPath + ".data");
        WWW www = new WWW(loadPath);
        yield return www;
        ProcessLoadSound(www, loadPath, soundPath, delFinish, param1, param2, param3);
    }

    // 加载场景
    public delegate void LoadAnimationFinish(string animPath, AnimationClip animClip);
    public static IEnumerator LoadAnimation(string animPath, LoadAnimationFinish delFinish)
    {
        string loadPath = GetBundleLoadUrl(BundleManager.PathAnimationAsset, animPath + ".data");
        WWW www = new WWW(loadPath);
        yield return www;
        ProcessLoadAnimation(www, loadPath, animPath, delFinish);
    }

    public delegate void LoadSceneFinish(string sceneName, AssetBundle sceneBundle);
    public static IEnumerator LoadScene(string sceneName, LoadSceneFinish delFinish)
    {

       string loadPath = GetBundleLoadUrl(BundleManager.PathSceneData, sceneName + ".data");
		//string loadPath = m_loadUrlHeadApplication.dataPath + "/MLDJ/BundleData/Scene" + sceneName;
		GameManager.gameManager.RunningScene= (int)LoadingWindow.nextSceneID;
		Application.LoadLevel (sceneName);
       // WWW www = new WWW(loadPath);
        yield return null;
//        if (null != www.assetBundle)
//        {
//            if (null != delFinish) delFinish(sceneName, www.assetBundle);
//        }
//        else
//        {
//            if (null != delFinish) delFinish(null, null);
//        }


    }

	#region ModelLoadList
	
	class ModelLoadData
	{
        public enum LoadType
        {
            MODEL,
            EFFECT,
        }
        public ModelLoadData()
        {
        }
		public ModelLoadData(LoadType loadType, string modelName, LoadSingleFinish delFinish, object param1, object param2, object param3)
		{
			m_modelName = modelName;
			m_delFinish = delFinish;
			m_param1 = param1;
			m_param2 = param2;
			m_param3 = param3;
            m_loadType = loadType;
		}

        public void ResetData(LoadType loadType, string modelName, LoadSingleFinish delFinish, object param1, object param2, object param3)
		{
			m_modelName = modelName;
			m_delFinish = delFinish;
			m_param1 = param1;
			m_param2 = param2;
			m_param3 = param3;
            m_loadType = loadType;
		}

        public string GetLoadUrl()
        {
            switch (m_loadType)
            {
                case LoadType.MODEL:
                    return GetBundleLoadUrl(BundleManager.PathModelPrefab, m_modelName + ".data");
                case LoadType.EFFECT:
                    return GetBundleLoadUrl(BundleManager.PathEffectPrefab, m_modelName + ".data");
                default:
                    LogModule.ErrorLog("load un define single bundle");
                    return "";
            }
        }
		
		public string m_modelName;
		public LoadSingleFinish m_delFinish;
		public object m_param1;
		public object m_param2;
		public object m_param3;
        public LoadType m_loadType;
	}

    private static Queue<ModelLoadData> m_loadBundleQueue = new Queue<ModelLoadData>();
	
	public static void CleanBundleLoadQueue()
	{
        m_loadBundleQueue.Clear();
	}

    public static void LoadEffectInQueue(string modelName, LoadSingleFinish delFinish, object param1 = null, object param2 = null)
    {
        ModelLoadData curData = new ModelLoadData(ModelLoadData.LoadType.EFFECT, modelName, delFinish, param1, param2, null);
        if (m_dicSingleBundleCache.ContainsKey(curData.GetLoadUrl()))
        {
            if (null != delFinish) delFinish(modelName, m_dicSingleBundleCache[curData.GetLoadUrl()], param1, param2, null);
            return;
        }
        object model = Resources.Load( modelName);
        if (model != null)
        {
            m_dicSingleBundleCache.Add(curData.GetLoadUrl(), (GameObject)model);
            delFinish(modelName, m_dicSingleBundleCache[curData.GetLoadUrl()], param1, param2);
            return;
        }
        m_loadBundleQueue.Enqueue(curData);
    }


	public static void LoadModelInQueue(string modelName, LoadSingleFinish delFinish, object param1, object param2, object param3)
	{

       
        ModelLoadData curData = new ModelLoadData(ModelLoadData.LoadType.MODEL, modelName, delFinish, param1, param2, param3);
        if (m_dicSingleBundleCache.ContainsKey(curData.GetLoadUrl()))
        {
            if (null != delFinish) delFinish(modelName, m_dicSingleBundleCache[curData.GetLoadUrl()], param1, param2, param3);
            return;
        }

         object model = Resources.Load("Model/"+modelName);
        if (model != null) 
        {
            m_dicSingleBundleCache.Add(curData.GetLoadUrl(),(GameObject)model);
            delFinish(modelName, m_dicSingleBundleCache[curData.GetLoadUrl()], param1, param2, param3);
            return;
        }
        m_loadBundleQueue.Enqueue(curData);
	}

	private static bool m_bLoadModelCommonShader = false;
	public static void BundleQueueLoadTick(MonoBehaviour bundleLoader)
	{
		if (null == bundleLoader)
		{
			return;
		}

		while (m_loadBundleQueue.Count > 0)
		{				
			ModelLoadData curData = m_loadBundleQueue.Dequeue();
			if (null != curData)
			{
				bundleLoader.StartCoroutine(LoadOneBundleFromQueue(curData));
			}
		}

        TryUnloadUnuseBundle();
	}

	private static IEnumerator LoadOneBundleFromQueue(ModelLoadData curData)
	{			
		yield return null;
		//by dsy  by dsy  暂时不用，最终发布的时候解开
//			if (m_bLoadModelCommonShader==false)
//			{
//				string modelShaderPath = GetBundleLoadUrl(BundleManager.PathModelPrefab, "model_shader_common.data");
//				WWW wwwModelShader = new WWW(modelShaderPath);
//                m_bLoadModelCommonShader = true;
//				yield return wwwModelShader;
//                if (null != wwwModelShader.assetBundle )
//				{
//					wwwModelShader.assetBundle.LoadAll();
//                  
//				}
//				else
//				{
//					LogModule.ErrorLog("can not find model shader");
//				}
//
//                string effectShaderPath = GetBundleLoadUrl(BundleManager.PathEffectPrefab, "effect_shader_common.data");
//                WWW wwwEffectShader = new WWW(effectShaderPath);
//                yield return wwwEffectShader;
//                if (null != wwwEffectShader.assetBundle)
//                {
//                    wwwEffectShader.assetBundle.LoadAll();
//                }
//                else
//                {
//                    LogModule.ErrorLog("can not find effect shader");
//                }
//		
//				
//			}		
//			
//			string bundlePath =curData.GetLoadUrl();
//			if(!LoadFromCache(bundlePath, curData.m_modelName, curData.m_delFinish, curData.m_param1, curData.m_param2, curData.m_param3))
//			{
//				WWW www = new WWW(bundlePath);
//				yield return www;
//				ProcessLoad(www, bundlePath, curData.m_modelName, curData.m_delFinish, curData.m_param1, curData.m_param2, curData.m_param3);
//			}

	}
	#endregion
}
