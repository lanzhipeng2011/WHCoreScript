/********************************************************************************
 *	文件名：	EffectLogic.cs
 *	全路径：	\Script\LogicCore\EffectLogic.cs
 *	创建人：	王迪
 *	创建时间：2013-11-21
 *
 *	功能说明：特效控制类
 *	修改记录：
*********************************************************************************/

using Games.GlobeDefine;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GCGame.Table;
using Games.LogicObj;
using Module.Log;
using System;
public class EffectLogic : MonoBehaviour
{
    public enum EffectType
    {
        TYPE_NORMAL = 0,
        TYPE_CHANGEMODEL = 1,
        TYPE_CHANGBLUE = 2,//材质变蓝
    }

    private class PlayEffectData
    {
        public PlayEffectData(string effectPath, GameObject parentObj, Vector3 effectPos, Vector3 effectRot, float duration, float delay = 0)
        {
            _effectPath = effectPath;
            _parentObj = parentObj;
            _effectPos = effectPos;
            _effectRot = effectRot;
            _duration = duration;
            _delay = delay;
        }

        public string _effectPath;
        public GameObject _parentObj;
        public Vector3 _effectPos;
        public Vector3 _effectRot;
        public float _duration;
        public float _delay;
    }

    private class AddEffectData
    {
        public AddEffectData(GameObject parentObj, Tab_Effect effectData, PlayEffectDelegate delPlayEffect, object param)
        {
            _parentObj = parentObj;
            _effectData = effectData;
            _delPlayEffect = delPlayEffect;
            _param = param;
        }

        public GameObject _parentObj;
        public Tab_Effect _effectData;
        public PlayEffectDelegate _delPlayEffect;
        public object _param;

    }
   
    /*
    // 缓存播放过的特效，在合适的时候清理（暂时在切场景)
    private static Dictionary<string, GameObject> m_effectObjCache = new Dictionary<string, GameObject>();
    public static void ReleaseEffectCahce()
    {
        m_effectObjCache.Clear();
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }
    */
    //Android 下使用
    private Obj_OtherPlayer m_ObjOtherPlayer;
    private bool m_IsValue = false;
    private bool m_IsOk = false;//

    private Dictionary<int, List<FXController>> m_dicEffectCache = new Dictionary<int, List<FXController>>();
    private Dictionary<int,int> m_EffectCountCache =new Dictionary<int, int>();//缓存同一个特效的数量
    public Dictionary<int, int> EffectCountCache
    {
        get { return m_EffectCountCache; }
    }

    public delegate void PlayEffectDelegate(GameObject effectObj, object param);

    private List<int> m_NeedPlayEffectIdCache =new List<int>();
    
    GameObject m_EffectGameObj;
    Obj m_EffectObj =null;
    private Dictionary<string, GameObject> m_effectBindPointCache = new Dictionary<string, GameObject>(); // 绑定点缓存

    public bool IsHaveBindPoint(string strPoint)
    {
        if (m_effectBindPointCache.ContainsKey(strPoint) && m_effectBindPointCache[strPoint] != null)
        {
            return true;
        }
        return false;
    }

    public const string m_NormalPonintName ="EffectPoint"; //通用点
    public EffectLogic()
    {

    }

#if UNITY_ANDROID

    //添加对NPC特效管理。如果发现该特效在20s内还没有被使用过，删除掉
    private float m_Time = 20f;
    private float m_WhileTime = 10f;
    private float m_TagTime = 0f;

    /// <summary>
    /// 每隔一定的时间删除一次缓存信息
    /// </summary>
    private void FixedUpdate()
    {
        if (m_dicEffectCache == null || m_EffectObj == null || !m_IsOk) return;
        if (m_EffectObj.ObjType != GameDefine_Globe.OBJ_TYPE.OBJ_NPC) return;

        if (m_TagTime.Equals(0f))
        {
            m_TagTime = Time.time;
            return;
        }

        if ((Time.time - m_TagTime) >= m_WhileTime)
        {
            m_TagTime = Time.time;
            DeleteNPCList();
        }
    }

    private void DeleteNPCList()
    {
        if (m_dicEffectCache == null) return;
        //Debug.Log(this.name+"::::"+Time.time);
        List<int> tempID = new List<int>();
        FXController fxController;
        foreach (KeyValuePair<int, List<FXController>> keyValuePair in m_dicEffectCache)
        {
            if (keyValuePair.Value == null) continue;

            if (!GameManager.gameManager.IsLowAndroid)
            {
                //删除较多的特效效果
                if (keyValuePair.Value.Count > 1)
                {
                    for (int i = 1; i < keyValuePair.Value.Count; i++)
                    {
                        if (i >= keyValuePair.Value.Count) break;

                        if (keyValuePair.Value[i] != null)
                        {
                            fxController = keyValuePair.Value[i] as FXController;

                            if (fxController == null)
                            {
                                EffectDestroyed(keyValuePair.Key);
                                keyValuePair.Value[i] = null;
                                continue;
                            }

                            if (!fxController.gameObject.activeSelf)
                            {
                                ResourceManager.DestroyResource(fxController.gameObject);
                                EffectDestroyed(keyValuePair.Key);
                                keyValuePair.Value[i] = null;
                            }

                            fxController = null;
                        }
                    }
                }
            }

            for (int i = 0; i < keyValuePair.Value.Count; i++)
            {
                fxController = keyValuePair.Value[i];
                if (fxController != null)
                {
                    if (fxController.PlayerFinishTime > 0 && (Time.time - fxController.PlayerFinishTime) > m_Time)
                    {
                        tempID.Add(keyValuePair.Key);
                        fxController = null;
                        break;
                    }

                    fxController = null;
                }
            }
        }

        for (int i = 0; i < tempID.Count; i++)
        {
            if (m_dicEffectCache.ContainsKey(tempID[i]))
            {
                for (int j = 0; j < m_dicEffectCache[tempID[i]].Count; j++)
                {
                    if (m_dicEffectCache[tempID[i]][j] != null)
                    {
                        fxController = m_dicEffectCache[tempID[i]][j] as FXController;

                        if (fxController == null)
                        {
                            EffectDestroyed(tempID[i]);
                            m_dicEffectCache[tempID[i]][j] = null;

                            continue;
                        }

                        ResourceManager.DestroyResource(fxController.gameObject);
                        EffectDestroyed(tempID[i]);
                        m_dicEffectCache[tempID[i]][j] = null;

                        fxController = null;
                    }
                }

                m_dicEffectCache.Remove(tempID[i]);
            }
        }

        if (m_dicEffectCache.Count <= 0)
        {
            m_IsOk = false;
            m_dicEffectCache.Clear();
        }
    }

#endif


    public void InitEffect(GameObject effGameObj)
    {
        m_EffectGameObj = effGameObj;
        if (null != m_EffectGameObj && m_EffectGameObj.GetComponent<Obj>() != null)
        {
            m_EffectObj = m_EffectGameObj.GetComponent<Obj>();
        }

        InitEffectPointInfo();
        m_dicEffectCache.Clear();
    }

    // 添加特效，如果添加过，并且isOnylDeactive为true，则可以利用返回的ID进行播放与停止操作
    public void AddEffect(GameObject parentObj, Tab_Effect effectData, PlayEffectDelegate delPlayEffect, object param)
    {
        if (null == effectData)
        {
            LogModule.ErrorLog("effectData is null");
            if (null != delPlayEffect) delPlayEffect(null, param);
            return;
        }
        if (effectData.Type == (int)EffectType.TYPE_CHANGBLUE)
        {
            if (m_EffectObj!= null)
            {
                m_EffectObj.SetMaterialColor(GlobeVar.BLUEMATERIAL_R, GlobeVar.BLUEMATERIAL_G, GlobeVar.BLUEMATERIAL_B);
            }
        }

        //已经存在 数量加1
        if (m_EffectCountCache.ContainsKey(effectData.EffectID))
        {
            m_EffectCountCache[effectData.EffectID] = m_EffectCountCache[effectData.EffectID]+1;
        }
        //不存在 添加进去 数量为1
        else
        {
            m_EffectCountCache.Add(effectData.EffectID,1);
        }

#if UNITY_ANDROID

        if (GameManager.gameManager.IsLowAndroid)
        {
            if (m_EffectObj != null)
            {
                if (m_EffectObj.ObjType != GameDefine_Globe.OBJ_TYPE.OBJ_NPC)
                {
                    if (effectData.IsOnlyDeactive && m_dicEffectCache.ContainsKey(effectData.EffectID) && m_dicEffectCache[effectData.EffectID].Count > 0)
                    {
                        FXController curFXController = m_dicEffectCache[effectData.EffectID][0];

                        if (curFXController != null)
                        {
                            m_dicEffectCache[effectData.EffectID].RemoveAt(0);
                            curFXController.Play(this);
                            if (null != delPlayEffect) delPlayEffect(curFXController.m_FirstChild, param);
                        }
                        else
                        {
                            m_dicEffectCache[effectData.EffectID][0] = null;
                            m_dicEffectCache[effectData.EffectID].RemoveAt(0);
                        }
                        
                        return;
                    }
                }
            }
        }
        else
        {
            if (m_dicEffectCache.ContainsKey(effectData.EffectID))
            {
                if (m_dicEffectCache[effectData.EffectID].Count > 0)
                {
                    FXController curFXController = m_dicEffectCache[effectData.EffectID][0] as FXController;
                    if (curFXController != null)
                    {
                        curFXController.Play(this);
                        if (null != delPlayEffect) delPlayEffect(curFXController.m_FirstChild, param);
                    }
                    else
                    {
                        m_dicEffectCache[effectData.EffectID][0] = null;
                        m_dicEffectCache[effectData.EffectID].RemoveAt(0);
                    }

                    return;
                }
            }
        }

#else
        if (effectData.IsOnlyDeactive && m_dicEffectCache.ContainsKey(effectData.EffectID) && m_dicEffectCache[effectData.EffectID].Count > 0)
                    {
                        FXController curFXController = m_dicEffectCache[effectData.EffectID][0];
                        m_dicEffectCache[effectData.EffectID].RemoveAt(0);
                        curFXController.Play(this);
                        if (null != delPlayEffect) delPlayEffect(curFXController.m_FirstChild, param);
                        return;
                    }
#endif

        string curName = effectData.Path.Substring(effectData.Path.LastIndexOf('/') + 1);
        AddEffectData curData = new AddEffectData(parentObj, effectData, delPlayEffect, param);
        m_NeedPlayEffectIdCache.Add(effectData.EffectID);
        GameObject resObj = ResourceManager.LoadResource(effectData.Path) as GameObject;
        if (null != resObj)
        {
            OnAddEffectDataLoad(effectData.Path, resObj, curData, null, null);
            return;
        }

        BundleManager.LoadEffectInQueue(curName, OnAddEffectDataLoad, curData);
      //  StartCoroutine(BundleManager.LoadEffect(curName, OnAddEffectDataLoad, curData));
    }

    private void OnAddEffectDataLoad(string name, GameObject resObj, object param1, object param2, object param3)
    {
        AddEffectData curData = param1 as AddEffectData;
        if (curData == null)
        {
            return;
        }

        if (resObj == null)
        {
            LogModule.ErrorLog("add effect error:" + name);
            if (null != curData._delPlayEffect) curData._delPlayEffect(null, curData._param);
            return;
        }
        if (null == curData._effectData)
        {
            LogModule.ErrorLog("effectData is null");
            if (null != curData._delPlayEffect) curData._delPlayEffect(null, curData._param);
            return;
        }
        if (null ==curData._parentObj)
        {
            LogModule.DebugLog("curData._parentObj is null");
            if (null != curData._delPlayEffect) curData._delPlayEffect(null, curData._param);
            return;
        }
        if (m_NeedPlayEffectIdCache.Contains(curData._effectData.EffectID) == false)
        {
            if (null != curData._delPlayEffect) curData._delPlayEffect(null, curData._param);
            return;
        }

        GameObject fxObject = GameObject.Instantiate(resObj) as GameObject;
        if (null == fxObject)
        {
            if (null != curData._delPlayEffect) curData._delPlayEffect(null, curData._param);
            return;
        }

        GameObject newFxController = ResourceManager.InstantiateResource("Prefab/Effect/FXController") as GameObject;
        if (null == newFxController)
        {
            GameObject.Destroy(fxObject);
            LogModule.ErrorLog("can't find prefab fxcontroller in Prefab/Effect/FXController");
            return;
        }

        m_NeedPlayEffectIdCache.Remove(curData._effectData.EffectID);
        fxObject.SetActive(false);

        if (null != curData._parentObj)
        {
            newFxController.transform.parent = curData._parentObj.transform;
            newFxController.transform.localPosition = Vector3.zero;
            newFxController.transform.localRotation = Quaternion.Euler(0, 0, 0);
            newFxController.transform.localScale = Vector3.one;
        }

        fxObject.transform.parent = newFxController.transform;
		if(curData._effectData.EffectID!=2009)
             fxObject.transform.localScale = Vector3.one;
        fxObject.transform.localPosition = new Vector3(curData._effectData.X, curData._effectData.Y,
            curData._effectData.Z);
        fxObject.transform.localRotation =
            Quaternion.Euler(new Vector3(curData._effectData.RotationX, curData._effectData.RotationY,
                curData._effectData.RotationZ));
        //头结点 坐标特殊偏移修正
        if (curData._effectData.ParentName == "HeadPoint")
        {
            if (m_EffectObj != null)
            {
                Tab_CharModel _CharModelInfo = TableManager.GetCharModelByID(m_EffectObj.ModelID, 0);
                if (_CharModelInfo != null)
                {
                    if (_CharModelInfo.ModelType == (int) GameDefine_Globe.MODELTYPE.ANIMAL)
                    {
                        fxObject.transform.localPosition = new Vector3(0.0f, -0.8f, 0.0f);
                    }
                    else if (_CharModelInfo.ModelType == (int) GameDefine_Globe.MODELTYPE.HUMAN ||
                             _CharModelInfo.ModelType == (int) GameDefine_Globe.MODELTYPE.HUMAN_FAT ||
                            _CharModelInfo.ModelType == (int)GameDefine_Globe.MODELTYPE.HUMAN_DYQ)
                    {
                        fxObject.transform.localPosition = new Vector3(-0.5f, 0.0f, 0.0f);
                    }
                    else if (_CharModelInfo.ModelType == -1)
                    {
                        fxObject.transform.localPosition = new Vector3(0.0f, _CharModelInfo.HeadInfoHeight/2.0f + 0.5f,
                            0.0f);
                    }
                }
            }
        }
        //身体中心点结点 坐标特殊偏移修正
        if (curData._effectData.ParentName == "CenterPoint")
        {
            if (m_EffectObj != null)
            {
                Tab_CharModel _CharModelInfo = TableManager.GetCharModelByID(m_EffectObj.ModelID, 0);
                if (_CharModelInfo != null)
                {
                    if (_CharModelInfo.ModelType == -1)
                    {
                        fxObject.transform.localPosition = new Vector3(0.0f, _CharModelInfo.HeadInfoHeight/4.0f, 0.0f);
                    }
                }
            }
        }

        FXController controller = newFxController.GetComponent<FXController>();

        if (controller == null)
        {
            LogModule.ErrorLog("can not find fxcontroller on gameobject");
            GameObject.Destroy(newFxController);
            if (null != curData._delPlayEffect) curData._delPlayEffect(fxObject, curData._param);
            return;
        }

        controller.OnlyDeactivate = curData._effectData.IsOnlyDeactive;
        controller.FxType = FXController.FXType.TYPE_PARTICLE;
        controller.Delay = curData._effectData.DelayTime;
        controller.Duration = curData._effectData.Duration;
        controller.EffectGameObj = m_EffectGameObj;
        controller.IsFellowOwner = curData._effectData.IsFellowOwner;
        controller.EffectID = curData._effectData.EffectID;
        controller.Play(this);

#if UNITY_ANDROID

        if (m_EffectObj != null && m_dicEffectCache != null)
        {
            if (m_EffectObj.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_NPC)
            {
                //低配机
                if (GameManager.gameManager.IsLowAndroid) 
                {
                    if (m_dicEffectCache.ContainsKey(controller.EffectID))
                    {
                        if (m_dicEffectCache[controller.EffectID] == null)
                        {
                            m_dicEffectCache[controller.EffectID] = new List<FXController>();
                            m_dicEffectCache[controller.EffectID].Add(controller);
                        }
                    }
                }
                else
                {
                    //if (controller.OnlyDeactivate)
                    //{
                        if (!m_dicEffectCache.ContainsKey(controller.EffectID))
                        {
                            m_dicEffectCache[controller.EffectID] = new List<FXController>();
                            m_dicEffectCache[controller.EffectID].Add(controller);
                        }
                        else
                        {
                            if (m_dicEffectCache[controller.EffectID] == null)
                            {
                                m_dicEffectCache[controller.EffectID] = new List<FXController>();
                            }

                            m_dicEffectCache[controller.EffectID].Add(controller);
                        }
                    //}
                }
            }
        }

#endif
        if (null != curData._delPlayEffect) curData._delPlayEffect(fxObject, curData._param);
    }

    public void InitEffectPointInfo()
    {
        m_IsValue = false;

        m_effectBindPointCache.Clear();
        m_EffectCountCache.Clear();
        //通用节点
        Transform effectPointBone = m_EffectGameObj.gameObject.transform.Find(m_NormalPonintName);
        if (effectPointBone != null)
        {
            m_effectBindPointCache.Add("EffectPoint", effectPointBone.gameObject);
        }
        if (m_EffectObj !=null)
        {
            Tab_CharModel _CharModelInfo = TableManager.GetCharModelByID(m_EffectObj.ModelID,0);
            if (_CharModelInfo != null && _CharModelInfo.ModelType !=-1)
            {
                Tab_EffectPoint _EffectPointInfo= TableManager.GetEffectPointByID(_CharModelInfo.ModelType, 0);
                if (_EffectPointInfo !=null)
                {
                    //身体中心点
                    effectPointBone = m_EffectGameObj.gameObject.transform.Find(_EffectPointInfo.CenterPointPath);
                    if (effectPointBone != null)
                    {
                        m_effectBindPointCache.Add("CenterPoint", effectPointBone.gameObject);
                    }
                    //头节点
                    effectPointBone = m_EffectGameObj.gameObject.transform.Find(_EffectPointInfo.HeadPointPath);
                    if (effectPointBone != null)
                    {
                        m_effectBindPointCache.Add("HeadPoint", effectPointBone.gameObject);
                    }
                    //左手（左前足）节点
                    /*for (int i = 0; i < m_EffectGameObj.transform.childCount;i++)
                    {
                        Debug.LogError(m_EffectGameObj.transform.GetChild(i).name);
                    }*/
                    effectPointBone = m_EffectGameObj.gameObject.transform.Find("Model/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 L Clavicle/Bip001 L UpperArm/Bip001 L Forearm/Bip001 L Hand/HH_weaponHandLf");
                    if (effectPointBone != null)
                    {
                        m_effectBindPointCache.Add("LHandPoint", effectPointBone.gameObject);
                    }
                    //右手（右前足）节点
                    effectPointBone = m_EffectGameObj.gameObject.transform.Find(_EffectPointInfo.RHandPointPath);
                    if (effectPointBone != null)
                    {
                        m_effectBindPointCache.Add("RHandPoint", effectPointBone.gameObject);
                    }
                    //左脚（左后足）节点
                    effectPointBone = m_EffectGameObj.gameObject.transform.Find(_EffectPointInfo.LFootPointPath);
                    if (effectPointBone != null)
                    {
                        m_effectBindPointCache.Add("LFootPoint", effectPointBone.gameObject);
                    }
                    //右脚（右后足）节点
                    effectPointBone = m_EffectGameObj.gameObject.transform.Find(_EffectPointInfo.RFootPointPath);
                    if (effectPointBone != null)
                    {
                        m_effectBindPointCache.Add("RFootPoint", effectPointBone.gameObject);
                    }
                }
            }
        }
    }
    
    // 播放特效,只接受type= 0的特效
    public void PlayEffect( int effectID, PlayEffectDelegate delPlayEffect = null, object param = null)
    {
        if (m_ObjOtherPlayer == null && !m_IsValue)
        {
            m_ObjOtherPlayer = this.GetComponent<Obj_OtherPlayer>();
            m_IsValue = true;
        }

        if (m_ObjOtherPlayer != null)
        {
            if (m_ObjOtherPlayer.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER
                && !m_ObjOtherPlayer.IsVisibleChar())
            {
                return;
            }
        }

#if UNITY_ANDROID

        if (m_EffectObj != null)
        {
            if (m_EffectObj.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_NPC)
            {
                //只在低配机下运行; 如果NPC超过30时 同时其他玩家超过1个时，默认关闭NPC特效
                if (GameManager.gameManager.IsLowAndroid)
                {
                    if (Singleton<ObjManager>.GetInstance() != null)
                    {
                        if (Singleton<ObjManager>.GetInstance().GetNPCNum() > 30)
                        {
                            return;
                        }

                        if (Singleton<ObjManager>.GetInstance().MainPlayer != null)
                        {
                            if (Singleton<ObjManager>.GetInstance().MainPlayer.AutoComabat
                                && Singleton<ObjManager>.GetInstance().MainPlayer.IsOpenAutoCombat)
                            {
                                return;
                            }
                        }
                    }
                }

                //m_IsOk = true;
            }
        }
#endif

        //设置了 取消技能特效
        if (PlayerPreferenceData.SystemSkillEffectEnable == false && m_EffectObj !=null )
        {
            if (m_EffectObj.ObjType ==GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER || //主角
                m_EffectObj.ObjType ==GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER || //其他玩家
                m_EffectObj.ObjType ==GameDefine_Globe.OBJ_TYPE.OBJ_NPC || //NPC
                m_EffectObj.ObjType ==GameDefine_Globe.OBJ_TYPE.OBJ_FELLOW || //伙伴
                m_EffectObj.ObjType ==GameDefine_Globe.OBJ_TYPE.OBJ_ZOMBIE_PLAYER|| //僵尸玩家
                m_EffectObj.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_DROP_ITEM ||   //掉落包
                m_EffectObj.ObjType ==GameDefine_Globe.OBJ_TYPE.OBJ_SNARE //陷阱
            ) 
            {
                return;
            }
        }

        //如果做了屏幕内特效优化，则也不显示
        //if (null !=  m_EffectObj)
        //{
        //    if (m_EffectObj.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER || //其他玩家
        //        m_EffectObj.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_NPC || //NPC
        //        m_EffectObj.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_FELLOW) //伙伴
        //    {
        //        Obj_Character objChar = m_EffectObj as Obj_Character;
        //        if (null != objChar && objChar.ModelInViewPort == false)
        //        {
        //            return;
        //        }
        //    }
        //}

        if (effectID < 0)
        {
            if (null != delPlayEffect)
            {
                delPlayEffect(null, param);
            }             
            return;
        }

        if (m_EffectGameObj == null)
        {
            LogModule.WarningLog("effect can not play before init");
            if (null != delPlayEffect)
            {
                delPlayEffect(null, param);
            }
            return;
        }


        Tab_Effect EffectInfo = TableManager.GetEffectByID(effectID, 0);
        if (EffectInfo ==null)
        {
            if (null != delPlayEffect)
            {
                delPlayEffect(null, param);
            }
            return;
        }

        //加个数量限制
        //如果表格配置为-1则不限制，此时不用判断数量
        if (EffectInfo.MaxOwnNum>= 0 && GetEffectCountById(effectID) >= EffectInfo.MaxOwnNum)
        {
            return;
        }

#if UNITY_ANDROID
        //只在低配机下运行
        if (m_EffectObj != null)
        {
            if (m_EffectObj.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_NPC)
            {
                if (m_dicEffectCache != null && GameManager.gameManager.IsLowAndroid)
                {
                    if (m_dicEffectCache.ContainsKey(EffectInfo.EffectID))
                    {
                        if (m_dicEffectCache[EffectInfo.EffectID] == null) return;
                        if (m_dicEffectCache[EffectInfo.EffectID].Count <= 0) return;

                        FXController curFXController = m_dicEffectCache[EffectInfo.EffectID][0];

                        if (curFXController == null)
                        {
                            m_dicEffectCache[EffectInfo.EffectID][0] = null;
                            m_dicEffectCache[EffectInfo.EffectID] = null;
                            m_dicEffectCache.Remove(EffectInfo.EffectID);

                            EffectDestroyed(EffectInfo.EffectID);
                            return;
                        }

                        if (curFXController.gameObject.activeSelf)
                        {
                            curFXController = null;
                            return;
                        }

                        curFXController.Play(this);
                        if (null != delPlayEffect) delPlayEffect(curFXController.m_FirstChild, param);
                        curFXController = null;

                        return;
                    }
                    else
                    {
                        m_dicEffectCache.Add(EffectInfo.EffectID, null);
                    }
                }
            }
        }

#endif

        LoadEffect(EffectInfo, delPlayEffect, param);
    }

    public bool IsHaveChangeColorEffct()
    {
        if (m_EffectCountCache ==null)
        {
            return false;
        }
        foreach (KeyValuePair<int, int> pair in m_EffectCountCache)
        {
            if (pair.Value >0)
            {
                Tab_Effect startEffect = TableManager.GetEffectByID(pair.Key, 0);
                if (null != startEffect && startEffect.Type == (int) EffectType.TYPE_CHANGBLUE)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public int GetEffectCountById(int effectID)
    {
        if (m_EffectCountCache == null)
        {
            return 0;
        }
        if (m_EffectCountCache.ContainsKey(effectID) ==false)
        {
            return 0;
        }
        return m_EffectCountCache[effectID];
    }

    /// <summary>
    /// 停止NPC所有特效
    /// </summary>
    public void NPCStopEffect()
    {
        m_IsOk = false;

        if (m_dicEffectCache == null || m_EffectObj == null) return;
        if (m_EffectObj.ObjType != GameDefine_Globe.OBJ_TYPE.OBJ_NPC) return;

        if (m_EffectObj != null)
        {
            m_EffectObj.SetMaterialInitColor();
        }

        int iTag = 0;
        FXController fx;
        foreach (KeyValuePair<int, List<FXController>> keyValuePair in m_dicEffectCache)
        {
            if (keyValuePair.Value == null) continue;
            iTag = keyValuePair.Value.Count;
            int j = 0;
            for (int i = 0; i < iTag; i++)
            {
                if (j >= keyValuePair.Value.Count) break;

                fx = keyValuePair.Value[j] as FXController;
                if (fx != null)
                {
                    if (j == 0 && fx.gameObject.activeSelf)
                    {
                        fx.gameObject.SetActive(false);
                        fx = null;
                        j++;
                    }
                    else
                    {
                        ResourceManager.DestroyResource(fx.gameObject);
                        keyValuePair.Value[j] = null;
                        keyValuePair.Value.RemoveAt(j);
                    }
                }
                fx = null;
            }
        }

        fx = null;
    }

    public void StopEffect(int effectID, bool bStopAll = true)
    {
        FXController[] controllers = null;
        if (m_NeedPlayEffectIdCache.Contains(effectID))
        {
            m_NeedPlayEffectIdCache.Remove(effectID);
        }
        //要停止的特效是否在播放 没有的话 则不要做后面的操作
        //屏蔽：防止部分查不到数量，但是实际存在需要stop特效的情况
        //if (GetEffectCountById(effectID) <=0)
        //{
        //    return;
        //}
        //材质变色材料停止的时候 置回初始颜色
        if (m_EffectObj!= null)
        {
            Tab_Effect startEffect = TableManager.GetEffectByID(effectID, 0);
            if (null != startEffect && startEffect.Type == (int)EffectType.TYPE_CHANGBLUE)
            {
                m_EffectObj.SetMaterialInitColor();
            }
        }

        foreach (KeyValuePair<string, GameObject> pair in m_effectBindPointCache)
        {
			if(pair.Value==null)
			{
				continue;
			}
            controllers = pair.Value.GetComponentsInChildren<FXController>();
            if (null == controllers)
            {
                continue;
            }
            foreach (FXController curController in controllers)
            {
                //处于播放完的隐藏状态 不需要stop
                if (curController.Remaindelay < 0 &&
                    curController.Remainduration < 0 &&
                    curController.gameObject.activeSelf == false )
                {
                    continue;
                }
                if (curController.EffectID == effectID)
                {
                    curController.Stop();
                    if (!bStopAll)
                    {
                        return;
                    }
                }
            }
        }
    }

    public void ClearEffect()
    {
        FXController[] controllers = null;
        foreach (KeyValuePair<string, GameObject> pair in m_effectBindPointCache)
        {
            controllers = pair.Value.GetComponentsInChildren<FXController>();
            if (null == controllers)
            {
                continue;
            }
            foreach (FXController curController in controllers)
            {
                curController.Stop();
            }
        }
    }

    public bool ContainsEffect(int effectID)
    {
        return false;
        //return m_fxCache.ContainsKey(effectID);
    }
    
    public void EffectDeactive(FXController curController)
    {

#if UNITY_ANDROID
        if (m_EffectObj != null)
        {
            if (m_EffectObj.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_NPC)
            {
                //技能升级特效播放完的回调
                OnEffectOver(curController.EffectID);

                return;
            }
        }
#endif

        if (m_dicEffectCache.ContainsKey(curController.EffectID) && null != m_dicEffectCache[curController.EffectID])
        {
            m_dicEffectCache[curController.EffectID].Add(curController);
        }
        else
        {
            m_dicEffectCache.Add(curController.EffectID, new List<FXController>());
            m_dicEffectCache[curController.EffectID].Add(curController);
        }

        OnEffectOver(curController.EffectID);
    }

    public void EffectDestroyed(int effectID)
    {
        //m_fxCache.Remove(effectID);
        OnEffectOver(effectID);
    }

    void OnEffectOver(int effectID)
    {
        //特效停止 数量减1
        if (m_EffectCountCache.ContainsKey(effectID))
        {
            m_EffectCountCache[effectID] = m_EffectCountCache[effectID] - 1;
            if (m_EffectCountCache[effectID] <= 0)
            {
                m_EffectCountCache.Remove(effectID);
            }
        }

        //技能升级特效播放完的回调
        if (effectID ==GlobeVar.SKILLLEVUPEFFECTID)
        {
            if (SkillRootLogic.Instance() !=null)
            {
                SkillRootLogic.Instance().SkillLevelUpEffectOver();
            }
        }
    }

    void LoadEffect(Tab_Effect effectData, PlayEffectDelegate delPlayEffect, object param)
    {
        if (effectData == null)
        {
            LogModule.WarningLog("cur effect is not in table");
            if (null != delPlayEffect) delPlayEffect(null, param);
            return;
        }
       
        if (effectData.ParentName == "0")
        {
            AddEffect(null, effectData, delPlayEffect, param);
            return;
        }
        
        GameObject effectParentObj =null;

		//=========
//		string modelNames = param as String;
		GameObject model = null;
		string modelNames = "";
		if(param != null && param is GameObject)
		{
			model = (GameObject)param;
			modelNames = model.name;
		}

		if(modelNames != null && modelNames != "")
		{
			string[] strArr = modelNames.Split ('_');
			if(strArr[1] == "weapon"||strArr[1]=="shizhuangweapon")
			{
				//通用节点
				Transform effectPointBone = m_EffectGameObj.gameObject.transform.Find("EffectPoint");
				if (effectPointBone != null)
				{
					//如果挂点不为空则直接查找该值赋值 不再添加
					//                m_effectBindPointCache.Add("EffectPoint", effectPointBone.gameObject);
					effectParentObj =m_effectBindPointCache["EffectPoint"];
				}else{
					//========播放武器特效
//					string modelName = param as String;
					effectPointBone = model.transform;//m_EffectGameObj.gameObject.transform.Find(modelName +"(Clone)");
					if(m_effectBindPointCache.ContainsKey(effectData.ParentName))
					{
						m_effectBindPointCache[effectData.ParentName] = effectPointBone.gameObject;
					}else{
						m_effectBindPointCache.Add("EffectPoint", effectPointBone.gameObject);
					}
					
					effectParentObj =m_effectBindPointCache["EffectPoint"];
				}
			}
		}else if (m_effectBindPointCache.ContainsKey(effectData.ParentName) && m_effectBindPointCache[effectData.ParentName]!=null)
        {
            effectParentObj =m_effectBindPointCache[effectData.ParentName];
        }
        else if (m_effectBindPointCache.ContainsKey("EffectPoint")) //容错 实在找不到 则使用通用的挂点
        {
            effectParentObj =m_effectBindPointCache["EffectPoint"];
        }
        else//容错 通用的挂点 找不到 加一下
        {
			//通用节点
			Transform effectPointBone = m_EffectGameObj.gameObject.transform.Find("EffectPoint");
			if (effectPointBone != null)
			{
				m_effectBindPointCache.Add("EffectPoint", effectPointBone.gameObject);
			}
        }


        if (effectParentObj !=null)
        {
            AddEffect(effectParentObj, effectData, delPlayEffect, param); 
        }
        else
        {
            LogModule.ErrorLog("can not find cur effect bind point : [effectid]" + effectData.EffectID + "[pointname]" + effectData.ParentName);
            if (null != delPlayEffect) delPlayEffect(null, param);
            return;
        }
    }
}
