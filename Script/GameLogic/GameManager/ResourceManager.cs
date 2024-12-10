/********************************************************************************
 *	文件名：	ResourceLoader.cs
 *	全路径：	\Script\GameLogic\GameManager\ResourceLoader.cs
 *	创建人：	李嘉
 *	创建时间：2014-01-20
 *
 *	功能说明：游戏的资源加载器，各种资源加载接口都在此
 *	         包括同步和异步加载等
 *	         并附加资源加载统计
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Games.LogicObj;
using Module.Log;

public class ResourceManager : MonoBehaviour
{
    private static Dictionary<string, int> m_ResourceInstantCounter = null;
    /// <summary>
    /// 资源动态加载记数，只需传入资源路径即可
    /// </summary>
    /// <param name="szKey"></param>
    private static void IncreaseInstantResource(string szKey)
    {
        if (null == m_ResourceInstantCounter)
        {
            m_ResourceInstantCounter = new Dictionary<string, int>();
        }

        if (true == m_ResourceInstantCounter.ContainsKey(szKey))
        {
            m_ResourceInstantCounter[szKey] += 1;
        }
        else
        {
            m_ResourceInstantCounter.Add(szKey, 1);
        }
    }
    
    /// <summary>
    /// 资源记数-1，如果发现为0则从统计表中删除
    /// </summary>
    /// <param name="szKey"></param>
    private static void DecreaseInstantResource(string szKey)
    {
        if (null == szKey || szKey.Length <= 0)
        {
            return;
        }

        if (null != m_ResourceInstantCounter && m_ResourceInstantCounter.ContainsKey(szKey))
        {
            m_ResourceInstantCounter[szKey]--;
            if (m_ResourceInstantCounter[szKey] <= 0)
            {
                m_ResourceInstantCounter.Remove(szKey);
            }
        }
    }
    
    /// <summary>
    /// 重置资源动态加载计数器
    /// </summary>
    public static void ResetResourceLoadCounter()
    {
        if (null != m_ResourceInstantCounter)
        {
            m_ResourceInstantCounter.Clear();
        }
    }

    private static List<string> m_ResourceLoadCounter = null;       //只调用Resource.Load但是没有Instant的资源
    private static void IncreaseResourceLoadCount(string szKey)
    {
        if (null == m_ResourceLoadCounter)
        {
            m_ResourceLoadCounter = new List<string>();
        }

        if (!m_ResourceLoadCounter.Contains(szKey))
        {
            m_ResourceLoadCounter.Add(szKey);
        }
    }

    /// <summary>
    /// 向内存中加载一个资源
    /// 因为只会创建内存数据，所以不会调用instantiate方法
    /// 此方法不支持异步加载
    /// 但是失败会输出日志
    /// </summary>
    /// <param name="resPath"></param>
    /// <returns></returns>
    public static UnityEngine.Object LoadResource(string resPath, System.Type  systemTypeInstance = null)
    {
        UnityEngine.Object resObject = null;
        if (null == systemTypeInstance)
        {
            resObject = Resources.Load(resPath);
        }
        else
        {
            resObject = Resources.Load(resPath, systemTypeInstance);
        }

        if (null != resObject)
        {
            IncreaseResourceLoadCount(resPath);
        }

        return resObject;
    }

    /// <summary>
    /// 销毁GameObject,记数-1，不会自动置空
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="bImmediate"></param>
    public static void DestroyResource(GameObject obj, bool bImmediate = false)
    {
        if (null != obj)
        {
            string szName = obj.name;
            if (false == bImmediate)
            {
                GameObject.Destroy(obj);

                //清理一次无用资源
                //Resources.UnloadUnusedAssets();
            }
            else
            {
                GameObject.DestroyImmediate(obj);
            }

            //减少记数
            DecreaseInstantResource(szName);
        }
    }

    /// <summary>
    /// 销毁GameObject,记数-1，会自动置空
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="bImmediate"></param>
    public static void DestroyResource(ref GameObject obj, bool bImmediate = false)
    {
        if (null != obj)
        {
            string szName = obj.name;
            if (false == bImmediate)
            {
                GameObject.Destroy(obj);

                //清理一次无用资源
                //Resources.UnloadUnusedAssets();
            }
            else
            {
                GameObject.DestroyImmediate(obj);
            }
            obj = null;

            //减少记数
            DecreaseInstantResource(szName);
        }
    }

    //根据资源动态创建一个GameObject
    /// <summary>
    /// 在场景中创建一个GameObject
    /// 加载成功或失败都会输出日志
    /// 此方法不支持异步加载
    /// </summary>
    /// <param name="resPath">资源路径</param>
    /// <param name="szKey">GameObject的名称，主要用作ObjPools的Key值</param>
    /// 成功和失败都会输入日志，但是成功之后会对统计记数递增
    /// <returns></returns>
    public static UnityEngine.Object InstantiateResource(string resPath, string szKey = "", System.Type systemTypeInstance = null)
    {
        UnityEngine.Object resObject = null;
        if (null == systemTypeInstance)
        {
            resObject = Resources.Load(resPath);
        }
        else
        {
            resObject = Resources.Load(resPath, systemTypeInstance);
        }

        if (null != resObject)
        {
            UnityEngine.Object modelObject = (GameObject)GameObject.Instantiate(resObject);
            if (null == modelObject)
            {
                return null;
            }
            else
            {
                IncreaseInstantResource(szKey);

                //如果没有传入名字，则使用Object的默认名字
                if (szKey.Length > 0)
                {
                    modelObject.name = szKey;
                }
            }

            return modelObject;
        }

        return null;
    }

    //加载BackCamera的Prefab
    public static void LoadBackCamera()
    {
        UIManager.LoadItem(UIInfo.BackCamera, OnLoadBackCameraOver);
    }
    
    private static void OnLoadBackCameraOver(GameObject resObj, object param)
    {
        if (null == resObj)
        {
            return;
        }

        GameObject newElement = GameObject.Instantiate(resObj) as GameObject;
    }

    /// <summary>
    /// 加载UI
    /// </summary>
    /// <param name="strUIName">UI名字</param>
    /// <param name="nLevel">UI级别</param>
    /// <returns></returns>
    public static GameObject LoadUIPrefab(string strUIName, int nUILevel)
    {
        if (null == GameManager.gameManager)
        {
            LogModule.ErrorLog("can not find gamemanager");
            return null;
        }

        if (null == GameManager.gameManager.ActiveScene)
        {
            LogModule.ErrorLog("can not find ActiveScene");
            return null;
        }

        GameObject uiRoot = GameManager.gameManager.ActiveScene.UIRoot;
        if (null == uiRoot)
        {
            LogModule.ErrorLog("can not find uiroot");
            return null;
        }
        string strPath = "Prefab/UI/" + strUIName;
        GameObject newObj = InstantiateResource(strPath, strUIName) as GameObject;
        if (newObj)
        {
            newObj.transform.parent = uiRoot.transform;
            newObj.transform.localPosition = Vector3.zero;
            newObj.transform.localScale = Vector3.one;
            if (MainUILogic.Instance() != null)
            {
                MainUILogic.Instance().DicUI.Add(strUIName, newObj);
                MainUILogic.Instance().DicUILevel.Add(strUIName, nUILevel);
            }
        }

        return newObj;
    }

    public delegate void LoadHeadInfoDelegate(GameObject objHeadInfo);
    /// <summary>
    /// 加载头顶信息Prefab
    /// </summary>
    /// <param name="nParent">父节点</param>
    /// <param name="strPrefabName">Prefab名字</param>
    /// <returns></returns>
    public static void LoadHeadInfoPrefab(UIPathData uiData, GameObject nParent, string strPrefabName, LoadHeadInfoDelegate delFun)
    {
        if (null == GameManager.gameManager.ActiveScene ||
            null == GameManager.gameManager.ActiveScene.NameBoardPool)
        {
            LogModule.ErrorLog("scene is not init when load headinfo");
            return;
        }

        GameManager.gameManager.ActiveScene.NameBoardPool.CreateUIFromBundle(uiData, strPrefabName, OnLoadHeadInfo, nParent, delFun);
    }

    static void OnLoadHeadInfo(GameObject resObj, object parent, object fun)
    {
        if (null != resObj)
        {
            if (null != GameManager.gameManager.ActiveScene.NameBoardRoot)
            {
                resObj.transform.parent = GameManager.gameManager.ActiveScene.NameBoardRoot.transform;
            }

            resObj.transform.localPosition = Vector3.zero;
            resObj.transform.localRotation = Quaternion.LookRotation(Vector3.forward);
            BillBoard billboard = resObj.GetComponent<BillBoard>();
            if (null == billboard)
            {
                billboard = resObj.AddComponent<BillBoard>();
                if (null != billboard)
                {
                    billboard.BindObj = parent as GameObject;
                }
            }
            else
            {
                billboard.BindObj = parent as GameObject;
                billboard.enabled = true;
            }

            //由于会复用，所以需要重新设置名字版的高度修正
            if (null != billboard && null != billboard.BindObj)
            {
                Obj_OtherPlayer objOtherPlayer = billboard.BindObj.GetComponent<Obj_OtherPlayer>();
                if (null != objOtherPlayer)
                {
                    billboard.fDeltaHeight = objOtherPlayer.DeltaHeight + objOtherPlayer.GetMountNameBoardHeight();
                }
                else
                {
                    Obj_Character objCharacter = billboard.BindObj.GetComponent<Obj_Character>();
                    if (null != objCharacter)
                    {
                        billboard.fDeltaHeight = objCharacter.DeltaHeight;
                    }
                }                
            }

            LoadHeadInfoDelegate delFun = fun as LoadHeadInfoDelegate;
            if (null != delFun) delFun(resObj);
        }
        else
        {
            LogModule.ErrorLog("load headinfo fail");
        }
    }

    public static void UnLoadHeadInfoPrefab(GameObject headInfo)
    {
        if (null == headInfo)
        {
            return;
        }

        BillBoard billboard = headInfo.GetComponent<BillBoard>();
        if (null != billboard)
        {
            billboard.BindObj = null;
            billboard.enabled = false;
        }

        //在池子中置为未使用
        if (null != GameManager.gameManager.ActiveScene &&
            null != GameManager.gameManager.ActiveScene.NameBoardPool)
        {
            GameManager.gameManager.ActiveScene.NameBoardPool.Remove(headInfo);
        }
    }
    
    public static GameObject LoadTitleInvestitiveItem(GameObject TitleInvestitiveGrid, int nTitleInvestitiveItemIndex)
    {
        string strPath = "Prefab/UI/TitleInvestitiveItem";
        string strPrefabName = "";
        if (nTitleInvestitiveItemIndex < 10)
        {
            strPrefabName = "TitleInvestitiveItem" + "0" + nTitleInvestitiveItemIndex.ToString();
        }
        else
        {
            strPrefabName = "TitleInvestitiveItem" + nTitleInvestitiveItemIndex.ToString();
        }
        GameObject TitleInvestitiveItem = InstantiateResource(strPath, strPrefabName) as GameObject;
        if (null != TitleInvestitiveItem)
        {
            if (null != TitleInvestitiveGrid)
                TitleInvestitiveItem.transform.parent = TitleInvestitiveGrid.transform;
            TitleInvestitiveItem.transform.localPosition = Vector3.zero;
            TitleInvestitiveItem.transform.localScale = Vector3.one;
            TitleInvestitiveItem.name = strPrefabName;
        }
        return TitleInvestitiveItem;
    }

    public static GameObject LoadLastSpeakerItem(GameObject LastSpeakerGrid, int nLastSpeakerItemIndex)
    {
        string strPath = "Prefab/UI/LastSpeakerItem";
        string strPrefabName = "";
        if (nLastSpeakerItemIndex < 10)
        {
            strPrefabName = "LastSpeakerItem" + "0" + nLastSpeakerItemIndex.ToString();
        }
        else
        {
            strPrefabName = "LastSpeakerItem" + nLastSpeakerItemIndex.ToString();
        }
        GameObject LastSpeakerItem = InstantiateResource(strPath, strPrefabName) as GameObject;
        if (null != LastSpeakerItem)
        {
            if (null != LastSpeakerGrid)
                LastSpeakerItem.transform.parent = LastSpeakerGrid.transform;
            LastSpeakerItem.transform.localPosition = Vector3.zero;
            LastSpeakerItem.transform.localScale = Vector3.one;
            LastSpeakerItem.name = strPrefabName;
        }
        return LastSpeakerItem;
    }

    public static GameObject LoadChatLink(GameObject LinkRoot)
    {
        string strPath = "Prefab/UI/ChatLink";
        string strPrefabName = "ChatLink";
        GameObject ChatLink = InstantiateResource(strPath, strPrefabName) as GameObject;
        if (null != ChatLink)
        {
            if (null != LinkRoot)
                ChatLink.transform.parent = LinkRoot.transform;
            ChatLink.transform.localPosition = Vector3.zero;
            ChatLink.transform.localScale = Vector3.one;
            ChatLink.name = strPrefabName;
        }
        return ChatLink;
    }

    public static GameObject LoadEmotionItem(GameObject ChatInfoEmotion)
    {
        string strPath = "Prefab/UI/EmotionItem";
        string strPrefabName = "EmotionItem";
        GameObject EmotionItem = InstantiateResource(strPath, strPrefabName) as GameObject;
        if (null != EmotionItem)
        {
            if (null != ChatInfoEmotion)
                EmotionItem.transform.parent = ChatInfoEmotion.transform;
            EmotionItem.transform.localPosition = Vector3.zero;
            EmotionItem.transform.localScale = Vector3.one;
            EmotionItem.name = strPrefabName;
        }
        return EmotionItem;
    }

    public static GameObject LoadChatVIPIcon(GameObject ChatVIPIcon)
    {
        string strPath = "Prefab/UI/SGChatVIPIcon";
        string strPrefabName = "SGChatVIPIcon";
        GameObject EmotionItem = InstantiateResource(strPath, strPrefabName) as GameObject;
        if (null != EmotionItem)
        {
            if (null != ChatVIPIcon)
                EmotionItem.transform.parent = ChatVIPIcon.transform;
            EmotionItem.transform.localPosition = Vector3.zero;
            EmotionItem.transform.localScale = Vector3.one;
            EmotionItem.name = strPrefabName;
        }
        return EmotionItem;
    }

    public static GameObject LoadMessageIcon(GameObject detailBandRoot)
    {
        string strPath = "Prefab/UI/MessageIcon";
        string strPrefabName = "MessageIcon";
        GameObject MessageIcon = InstantiateResource(strPath, strPrefabName) as GameObject;
        if (null != MessageIcon)
        {
            if (null != detailBandRoot)
                MessageIcon.transform.parent = detailBandRoot.transform;
            MessageIcon.transform.localPosition = Vector3.zero;
            MessageIcon.transform.localScale = Vector3.one;
            MessageIcon.name = strPrefabName;
        }
        return MessageIcon;
    }

    public static GameObject LoadFastReplyItem(GameObject FastReplyGrid, int nFastReplyItemIndex)
    {
        string strPath = "Prefab/UI/FastReplyItem";
        string strPrefabName = "";
        if (nFastReplyItemIndex < 10)
        {
            strPrefabName = "FastReplyItem" + "0" + nFastReplyItemIndex.ToString();
        }
        else
        {
            strPrefabName = "FastReplyItem" + nFastReplyItemIndex.ToString();
        }
        GameObject FastReplyItem = InstantiateResource(strPath, strPrefabName) as GameObject;
        if (null != FastReplyItem)
        {
            if (null != FastReplyGrid)
                FastReplyItem.transform.parent = FastReplyGrid.transform;
            FastReplyItem.transform.localPosition = Vector3.zero;
            FastReplyItem.transform.localScale = Vector3.one;
            FastReplyItem.name = strPrefabName;
        }
        return FastReplyItem;
    }


    public static GameObject LoadEmotionButton(GameObject EmotionGrid, int nEmotionButtonIndex)
    {
        string strPath = "Prefab/UI/EmotionButton";
        string strPrefabName = "";
        if (nEmotionButtonIndex < 10)
        {
            strPrefabName = "EmotionButton" + "0" + nEmotionButtonIndex.ToString();
        }
        else
        {
            strPrefabName = "EmotionButton" + nEmotionButtonIndex.ToString();
        }
        GameObject EmotionButton = InstantiateResource(strPath, strPrefabName) as GameObject;
        if (null != EmotionButton)
        {
            if (null != EmotionGrid)
                EmotionButton.transform.parent = EmotionGrid.transform;
            EmotionButton.transform.localPosition = Vector3.zero;
            EmotionButton.transform.localScale = Vector3.one;
            EmotionButton.name = strPrefabName;
        }
        return EmotionButton;
    }
}