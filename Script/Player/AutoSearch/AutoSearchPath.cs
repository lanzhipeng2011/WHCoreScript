/********************************************************************************
 *	文件名：	AutoSearchPath.cs
 *	全路径：	\Script\Player\AutoSearch\AutoSearchPath.cs
 *	创建人：	李嘉
 *	创建时间：2014-01-02
 *
 *	功能说明：自动寻路路径类，只保存数据，而不进行逻辑控制
 *	         保存了路径中所有的点在List中
 *	         保存了自动寻路结束后的回调事件和目标名称
 *	         索引可能会重复，但是路径一定按照插入的顺序排列
 *	         所以当完成一个点的时候，可以直接将List中的头元素删除
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Games.Events;

public class AutoSearchPath
{
    public AutoSearchPath()
    {
        m_AutoSearchPointCache = new List<AutoSearchPoint>();
        m_FinishCallBackEvent = null;
        m_AutoSearchTargetName = "";
    }

    //自动寻路点缓存
    private List<AutoSearchPoint> m_AutoSearchPointCache;
    public List<AutoSearchPoint> AutoSearchPosCache
    {
        get { return m_AutoSearchPointCache; }
    }

    //自动寻路目标名字
    private string m_AutoSearchTargetName;
    public string AutoSearchTargetName
    {
        get { return m_AutoSearchTargetName; }
        set { m_AutoSearchTargetName = value; }
    }

    //自动寻路结束事件
    //当到达最后一个点之后触发
    private GameEvent m_FinishCallBackEvent;
    public GameEvent FinishCallBackEvent
    {
        get { return m_FinishCallBackEvent; }
        set { m_FinishCallBackEvent = value; }
    }

    public void AddPathPoint(AutoSearchPoint point)
    {
        if (null != m_AutoSearchPointCache)
        {
            m_AutoSearchPointCache.Add(point);
        }
    }

    public void ResetPath()
    {
        m_AutoSearchPointCache.Clear();
        if (null != m_FinishCallBackEvent)
        {
            m_FinishCallBackEvent.Reset();
        }
        if (m_AutoSearchTargetName!="")
        {
            m_AutoSearchTargetName = "";
        }
    }

    //找到场景sceneId中第一个需要达到的点
    public AutoSearchPoint GetPathPosition(int sceneId)
    {
        for (int i = 0; i < m_AutoSearchPointCache.Count; ++i)
        {
            if (m_AutoSearchPointCache[i].SceneID == sceneId)
            {
                return m_AutoSearchPointCache[i];
            }
        }

        return null;
    }

    //是否已经抵达路径点
    public bool IsFinish(Vector3 pos)
    {
        //路径数量大于1，则认为未到达
        if (m_AutoSearchPointCache.Count > 1)
        {
            return false;
        }
        //路径为空，认为已经到达
        else if (m_AutoSearchPointCache.Count <= 0)
        {
            return true;
        }

        //如果目前路径中确实只有一个点，则判断是否已经达到要求
        return IsReachPoint(pos);
    }

    //是否到达寻路路径中的下一个目标点
    //按照设计就是当前索引为0的点
    public bool IsReachPoint(Vector3 pos)
    {
        if (m_AutoSearchPointCache.Count <= 0)
        {
            return false;
        }

        //先判断场景ID
        if (m_AutoSearchPointCache[0].SceneID == GameManager.gameManager.RunningScene)
        {
            //只用做2D判断即可
            pos.y = 0.0f;
            Vector3 destPos = new Vector3(m_AutoSearchPointCache[0].PosX, 0.0f, m_AutoSearchPointCache[0].PosZ);
            //判断距离
            if (Vector3.Distance(pos, destPos) < 1.0f)
            {
                return true;
            }
        }

        return false;
    }

    //得到下一个需要寻路的点
    //返回null表示路径结束
    public AutoSearchPoint GetNext()
    {
        if (m_AutoSearchPointCache.Count > 0)
        {
            return m_AutoSearchPointCache[0];
        }

        return null;
    }
}
