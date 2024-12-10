/********************************************************************************
 *	文件名：	GameObjectPool.cs
 *	全路径：	\Script\GameManager\GameObjectPool.cs
 *	创建人：	李嘉
 *	创建时间：2014-04-15
 *
 *	功能说明：自增长池类
 *	         当池子中对象满的时候，会创建并且插入，如果发现有删除的对象则复用。
 *	         删除对象的时候池子中的数量并不会减少，而是将对象变为可复用。
 *	         对外只提供Create和Remove接口
 *	         池子的作用是避免过多的GC行为发生，在内存略微增长的前提下达到帧率的平滑稳定。
 *	         注意：如果对象的销毁函数（Destroy）中包含对class赋空的情况，则无法避免GC行为，此时不一定要使用该池
 *	               另外，千万不要在其他逻辑中将list中的GameObject状态置true和false，否则在取的时候会失效，此时宁可不用Pool
 *	修改记录：
 *	         LiJia 2014-09-17 将尺子改为自增长，但是最大可能只有POOL_MAX_CAPACITY定义的数量
*********************************************************************************/
using System.Collections.Generic;
using Games.GlobeDefine;
using UnityEngine;
using Module.Log;

public class GameObjectPool
{
    public delegate void CreatePoolObjDelegate(GameObject newObj, object param1, object param2);

    private class LoadBundleParam
    {
        public LoadBundleParam(CreatePoolObjDelegate delFun, object param1, object param2, UIPathData uiData, string objName)
        {
            m_delFun = delFun;
            m_userParam1 = param1;
            m_userParam2 = param2;
            m_uiData = uiData;
            m_objName = objName;
        }

        public CreatePoolObjDelegate m_delFun;
        public object m_userParam1;
        public object m_userParam2;
        public UIPathData m_uiData;
        public string m_objName;
    }

    private GameObjectPool()
    {
    }

    public GameObjectPool(string szPoolName, int nMaxSize = GlobeVar.INVALID_ID)
    {
        m_szPoolName = szPoolName;

        m_ActivePool = new Dictionary<string, List<GameObject>>();
        m_InActivePool = new Dictionary<string, List<GameObject>>();
        m_nMaxSize = nMaxSize;
    }

    private string m_szPoolName;                        //池子名称
    public string PoolName
    {
        get { return m_szPoolName; }
    }

    private Dictionary<string, List<GameObject>> m_ActivePool;      //活动对象队列
    private Dictionary<string, List<GameObject>> m_InActivePool;    //非活动对象队列
    private int m_nMaxGameObjectType = 64;                          //池子中索引的数量
    private int m_nMaxSize = 128;                                   //池子中每种类型的GameObject可容纳的子队列大小
    
    // 从Bundle加载一个可复用的ITEM
    public void CreateUIFromBundle(UIPathData uiData, string objName, CreatePoolObjDelegate delFun, object param1, object param2)
    {
        GameObject obj = ReUseElement(objName);
        if (null == obj)
        {
            if (m_nMaxGameObjectType > 0 && m_ActivePool.Count == m_nMaxGameObjectType)
            {
                if(null != delFun) delFun(null, param1, param2);
                return;
            }

            UIManager.LoadItem(uiData, OnLoadBundleItem, new LoadBundleParam(delFun, param1, param2, uiData, objName));
        }
        else
        {
            if(null != delFun) delFun(obj, param1, param2);
        }
    }

    private void OnLoadBundleItem(GameObject resObj, object param)
    {
        LoadBundleParam curParam = param as LoadBundleParam;
        if (null == curParam)
            return;

        GameObject newElement = GameObject.Instantiate(resObj) as GameObject;
        if (null != newElement)
        {
            newElement.name = curParam.m_objName;
            if (false == InsertElement(m_ActivePool, newElement))
            {
                //这里必须将newElement强制释放雕，否则会导致Pool管理失效，newElement无限增加
                //后面所有用到newElement的地方原理相同
                GameObject.Destroy(newElement);
            }

            if (null != curParam.m_delFun) 
                curParam.m_delFun(newElement, curParam.m_userParam1, curParam.m_userParam2);
        }
    }


    public void Remove(GameObject element)
    {
        if (null == element)
            return;

        if (RemoveElement(m_ActivePool, element))
        {
            //插入都未使用队列
            if (InsertElement(m_InActivePool, element))
            {
                element.transform.position = GlobeVar.INFINITY_FAR;
            }
            else
            {
                //插入失败，需要把element释放掉，否则会变成不受管理的GameObject
                GameObject.Destroy(element);
            }
        }
    }

    //复用一个已经无效的对象
    private GameObject ReUseElement(string strGameObjectName)
    {
        GameObject reuseElem = null;
        if (m_InActivePool.ContainsKey(strGameObjectName))
        {
            List<GameObject> list = m_InActivePool[strGameObjectName];
            if (null == list || list.Count <= 0)
                return null;

            reuseElem = list[0];
            if (reuseElem == null)
            {
                list.RemoveAt(0);
                return null;
            }

            list.Remove(reuseElem);

            if (InsertElement(m_ActivePool, reuseElem))
            {
                return reuseElem;
            }
            else
            {
                //插入失败，释放掉
                GameObject.Destroy(reuseElem);
            }
        }
        
        return null;
    }
        
    //内部接口
    //向池子中插入一个新的GameObject
    private bool InsertElement(Dictionary<string, List<GameObject>> pool, GameObject newElement)
    {
        if (null == newElement || null == pool)
        {
            return false;
        }

        //先判断池子中是否有这个Key,如果没有则先添加
        List<GameObject> list = null;
        if (false == pool.ContainsKey(newElement.name))
        {
            list = new List<GameObject>();
            pool.Add(newElement.name, list);
        }
        else
        {
            list = pool[newElement.name];
        }
                
        if (null != list)
        {
            //池子已达到最大
            if (m_nMaxSize > 0 && list.Count >= m_nMaxSize)
            {
                if (list.Count < GlobeVar.POOL_MAX_CAPACITY)
                {
                    m_nMaxSize++;
                }
                else
                {
                    LogModule.ErrorLog("Pool has reached capacity size, Element name:" + newElement.name);
                    return false;
                }
            }
            list.Add(newElement);
            return true;
        }

        return false;
    }

    //从池子中移除一个GameObject
    private bool RemoveElement(Dictionary<string, List<GameObject>> pool, GameObject element)
    {
        if (null == element || null == pool)
        {
            return false;
        }

        if (pool.ContainsKey(element.name))
        {
            List<GameObject> list = pool[element.name];
            if (null != list && list.Contains(element))
            {
                list.Remove(element);
                return true;
            }
        }

        return false;
    }

    //清理所有的GameObjectPool
    public void ClearAllPool()
    {
        if (null != m_ActivePool)
        {
            foreach(KeyValuePair<string, List<GameObject>> activeList in m_ActivePool)
            {
                if (null != activeList.Value)
                {
                    for(int i=0; i<activeList.Value.Count; ++i)
                    {
                        activeList.Value[i] = null;
                    }
                    activeList.Value.Clear();
                }
            }

            m_ActivePool.Clear();
        }

        if (null != m_InActivePool)
        {
            foreach (KeyValuePair<string, List<GameObject>> inactiveList in m_InActivePool)
            {
                if (null != inactiveList.Value)
                {
                    for (int i = 0; i < inactiveList.Value.Count; ++i)
                    {
                        inactiveList.Value[i] = null;
                    }
                    inactiveList.Value.Clear();
                }
            }

            m_InActivePool.Clear();
        }
    }
}
