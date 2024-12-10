/********************************************************************************
 *	文件名：	AutoSearchAgent.cs
 *	全路径：	\Script\Player\AutoSearch\AutoSearchAgent.cs
 *	创建人：	李嘉
 *	创建时间：2014-01-02
 *
 *	功能说明：自动寻路代理，只要建立一个路径，就会自动控制主角进行自动寻路
 *	         寻路的具体路径保存在AutoSearchPath中
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
using Games.Events;
using Games.LogicObj;
using System.Collections.Generic;
using GCGame.Table;
using Games.Scene;

public struct MapConnectPath
{
    //起始点
    private int m_SrcSceneId;
    public int SrcSceneId
    {
        get { return m_SrcSceneId; }
        set { m_SrcSceneId = value; }
    }

    //结束点
    private int m_DstSceneId;
    public int DstSceneId
    {
        get { return m_DstSceneId; }
        set { m_DstSceneId = value; }
    }

    //传送点坐标X
    private float m_fTelePosX;
    public float TelePosX
    {
        get { return m_fTelePosX; }
        set { m_fTelePosX = value; }
    }

    //传送点坐标Y
    private float m_fTelePosZ;
    public float TelePosZ
    {
        get { return m_fTelePosZ; }
        set { m_fTelePosZ = value; }
    }
}

public class AutoSearchAgent : MonoBehaviour 
{
    // 自动寻路路径
    private AutoSearchPath m_Path;
    public AutoSearchPath Path
    {
        get { return m_Path; }
        set { m_Path = value; }
    }

    //是否自动寻路中标记位
    private bool m_bIsAutoSearching;
    public bool IsAutoSearching
    {
        get { return m_bIsAutoSearching; }
        set { m_bIsAutoSearching = value; }
    }

    //更新时间间隔
    private float m_fUpdateInterval = 0.50f;       //更新时间间隔，单位为秒 
    private float m_fLastUpdateTime = 0;        //上一次更新时间
    private int m_bNotEnamyNpcFlag = 0;    // 非敌对NPC标记 避免NPC多余遍历

    //寻路最短距离的最大值
    const int m_nMinDistanceMaxValue = 65535;

    private AutoSearchPoint m_EndPointCache;

    //场景通路图，可以在游戏开始后初始化
    private List<MapConnectPath> m_ConnectPath = null;
    //初始化场景通路图
    public void InitMapConnectPath()
    {
        if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_LOGIN)
        {
            return;
        }

        //说明已经初始化过，无需再次调用Init函数，整个游戏过程中初始化一次即可
        if (null != m_ConnectPath)
        {
            return;
        }
        //生成完整通路图
        else
        {
            m_ConnectPath = new List<MapConnectPath>();
        }

        foreach (int key in TableManager.GetMapConnection().Keys)
        {
            List<Tab_MapConnection> tableList = TableManager.GetMapConnectionByID(key);
            if (null != tableList)
            {
                foreach (Tab_MapConnection conect in tableList)
                {
                    MapConnectPath path = new MapConnectPath();
                    path.SrcSceneId = conect.SourceSceneID;
                    path.DstSceneId = conect.DestSceneID;
                    path.TelePosX = conect.TelePosX;
                    path.TelePosZ = conect.TelePosZ;
                    m_ConnectPath.Add(path);
                }
            }
        }
    }

    //初始化
    void Awake()
    {
        m_Path = new AutoSearchPath();
        m_Path.ResetPath();

        m_bIsAutoSearching = false;

        m_EndPointCache = new AutoSearchPoint();
        //InitMapConnectPath();
    }
    	
	// Update is called once per frame
	void Update () 
    {
        //未寻路，不更新
	    if (!m_bIsAutoSearching)
	    {
            return;
	    }

        Obj_MainPlayer mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;

        if (null == mainPlayer)
        {
            return;
        }
        //判断是否到了更新间隔
        if (Time.time - m_fLastUpdateTime < m_fUpdateInterval)
        {
            return;
        }
        else
        {
            m_fLastUpdateTime = Time.time;
        }        
	    
        //寻路中首先判断是否结束
        if (m_Path.IsFinish(mainPlayer.transform.position))
        {
            Finish();
        }
        else if (m_Path.IsReachPoint(mainPlayer.transform.position))
        {
            GotoNextPoint();
        }
        
        //如果此时未移动，则前往下一个点
        if (!mainPlayer.IsMoving)
        {
            FindEnamyTick(); // 开始就找一下
            BeginMove();
        }
        else
        {
            FindEnamyTick();
        }
	}

    // 寻路找怪处理
	private Obj_NPC  objNpc;
    void FindEnamyTick()
    {
        if (m_bNotEnamyNpcFlag != 0)
        {
            return;
        }
        //如果有目标名字则寻找该目标，进行交互
        if (m_Path.AutoSearchTargetName != "")
        {
            Obj_Character obj = Singleton<ObjManager>.GetInstance().FindObjCharacterInSceneByName(m_Path.AutoSearchTargetName);
            if (obj && obj.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_NPC)
            {
                 objNpc = obj as Obj_NPC;
                //根据目标NPC的势力确认是对话还是攻击
                if (Reputation.IsEnemy(objNpc) || Reputation.IsNeutral(objNpc))
                {
                    
                    // Singleton<ObjManager>.GetInstance().MainPlayer.OnEnterCombat(objNpc.GetComponent<Obj_Character>());
					Invoke("StartAutoCombat",1.5f);
                     m_bNotEnamyNpcFlag = 1;
                }
                else
                {
                    m_bNotEnamyNpcFlag = 2;
                    return;
                }
            }
        }
    }
	public void StartAutoCombat()
	{

		//如果是地方NPC，则开始攻击
	    if(objNpc!=null)
			Singleton<ObjManager>.GetInstance().MainPlayer.OnSelectTarget(objNpc.gameObject);
		//Singleton<ObjManager>.GetInstance().MainPlayer.CurUseSkillId=-1;
		ObjManager.Instance.MainPlayer.m_playerHeadInfo.ToggleXunLu(false);
		ObjManager.Instance.MainPlayer.AutoXunLu=false;
		Singleton<ObjManager>.GetInstance().MainPlayer.EnterAutoCombat();
		if(SGAutoFightBtn.Instance!=null)
		{
			SGAutoFightBtn.Instance.UpdateAutoFightBtnState();
		}
		Singleton<ObjManager>.GetInstance().MainPlayer.StopMove();
		m_Path.AutoSearchPosCache.Clear();
		m_Path.ResetPath();
		m_bIsAutoSearching = false;
		m_EndPointCache.Clean();

	}

    //创建一条路径，并且在Update中判断是否抵达
    public void BuildPath(AutoSearchPoint endPoint)
    {
        // 同一个点，直接跳过
        if (m_EndPointCache.SceneID == endPoint.SceneID
            && m_EndPointCache.PosX == endPoint.PosX
            && m_EndPointCache.PosZ == endPoint.PosZ)
        {
            return;
        }

        //停止正在进行的移动
        if (null != Singleton<ObjManager>.GetInstance().MainPlayer
            && true == Singleton<ObjManager>.GetInstance().MainPlayer.IsMoving)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.StopMove();
        }

        m_EndPointCache = endPoint;
        m_Path.ResetPath();
        m_bNotEnamyNpcFlag = 0;
        //判断主角是否存在
        Obj_MainPlayer mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (null == mainPlayer)
        {
            return;
        }

        //如果是当前场景，则直接生成单点路径，进行移动
        if (GameManager.gameManager.RunningScene == endPoint.SceneID)
        {
            m_Path.AddPathPoint(endPoint);
            //设置开始自动寻路
            m_bIsAutoSearching = true;
            return;
        }

        //根据当前点和目的地点来确定路径
        AutoSearchPoint startPoint = AutoSearchPoint.MakePoint(mainPlayer.gameObject);
        if (true == FindPath(startPoint, endPoint))
        {
            m_bIsAutoSearching = true;
            return;
        }
    }

    //获得两个场景的通路长度
    int GetDisBySceneID(int srcId, int dstId)
    {
        foreach (MapConnectPath path in m_ConnectPath)
        {
            if (path.SrcSceneId == srcId && path.DstSceneId == dstId)
            {
                return 1;
            }
        }

        return m_nMinDistanceMaxValue;
    }

    class PathNodeInfo
    {
        public PathNodeInfo()
        {
            sceneId = -1;
            dis = m_nMinDistanceMaxValue;
            prevNode = -1;
        }
        public int sceneId;
        public int dis;
        public int prevNode;
    }

    //搜索路径算法，根据DJ算法生成最短路径
    bool FindPath(AutoSearchPoint startPoint, AutoSearchPoint endPoint)
    {
        List<int> path = new List<int>();       //最后生成的最短路径图

        //统计所有节点，计算srcid的逆临街表，放在nodeMap中 将所有点放到nodeMap中
        Dictionary<int, PathNodeInfo> nodeMap = new Dictionary<int, PathNodeInfo>();
        foreach (MapConnectPath connectPath in m_ConnectPath)
        {
            if (false == nodeMap.ContainsKey(connectPath.SrcSceneId))
            {
                // 创建路径节点
                PathNodeInfo info = new PathNodeInfo();
                info.sceneId = connectPath.SrcSceneId;
                // 计算从起始点到这个点的距离
                info.dis = GetDisBySceneID(startPoint.SceneID, info.sceneId);
                info.prevNode = startPoint.SceneID;
                nodeMap.Add(info.sceneId, info);
            }
            
            if (false == nodeMap.ContainsKey(connectPath.DstSceneId))
            {
                PathNodeInfo info = new PathNodeInfo();
                info.sceneId = connectPath.DstSceneId;
                info.dis = GetDisBySceneID(startPoint.SceneID, info.sceneId);
                info.prevNode = startPoint.SceneID;
                nodeMap.Add(info.sceneId, info);
            }
        }
        
        List<int> openList = new List<int>(); // 开集集合存储还未确定到达起点的最短路径的节点，初始化包含所有点集
        List<int> closeList = new List<int>(); // 闭集集合存储已经确定了到达起点的最短路径的节点，初始化为空集合

        //放入开始点
        openList.Add(startPoint.SceneID);

        while (openList.Count > 0)
        {
            int minPt = openList[0];
            int minDis = m_nMinDistanceMaxValue;

            foreach (int openPoint in openList)
            {
                if (!nodeMap.ContainsKey(openPoint))
                {
                    continue;
                }

                if (nodeMap[openPoint].dis < minDis)
                {
                    minPt = openPoint;
                    minDis = nodeMap[openPoint].dis;
                }
            }

            int minDisNode = minPt;
            openList.Remove(minDisNode);
            closeList.Add(minDisNode);

            //展开该节点,更新所有邻接表的距离，并把下一个（或几个）最短路径上的点放入openList。
            foreach (MapConnectPath connectPath in m_ConnectPath)
            {
                if (connectPath.SrcSceneId == minDisNode)
                {
                    int newDis = nodeMap[minDisNode].dis + GetDisBySceneID(minDisNode, connectPath.DstSceneId);
                    if (newDis < nodeMap[connectPath.DstSceneId].dis)
                    {
                        nodeMap[connectPath.DstSceneId].dis = newDis;
                        nodeMap[connectPath.DstSceneId].prevNode = minDisNode;
                    }

                    if (!openList.Contains(connectPath.DstSceneId) && !closeList.Contains(connectPath.DstSceneId))
                    {
                        openList.Add(connectPath.DstSceneId);
                    }
                }
            }
        }


        if (!nodeMap.ContainsKey(endPoint.SceneID))
        {
            return false;
        }

        //这里生成最短路径和下一个场景号，这里的邻接表已经记录了最短路径信息
        if (nodeMap[endPoint.SceneID].dis < m_nMinDistanceMaxValue)
        {
            path.Insert(0, endPoint.SceneID);
            int backId = nodeMap[endPoint.SceneID].prevNode;
            while (backId != -1)
            {
                if (backId == startPoint.SceneID)
                {
                    //int nextScn = path[0];
                    path.Insert(0, backId);
                    break;
                }

                path.Insert(0, backId);
                backId = nodeMap[backId].prevNode;
            }
        }

        if (path.Count > 1)
        {
            //生成路径
            int beginScene = path[0];
            int endScene = -1;
            for (int i = 1; i < path.Count; ++i )
            {
                endScene = path[i];
                foreach (MapConnectPath connectPath in m_ConnectPath)
                {
                    if (connectPath.SrcSceneId == beginScene && connectPath.DstSceneId == endScene)
                    {
                        AutoSearchPoint point = new AutoSearchPoint(connectPath.SrcSceneId, connectPath.TelePosX, connectPath.TelePosZ);
                        m_Path.AutoSearchPosCache.Add(point);

                        beginScene = endScene;
                    }
                }
            }

            //最后加入目标点
            m_Path.AutoSearchPosCache.Add(endPoint);
            return true;
        }

        return false;
    }

    //停止自动寻路
    public void Stop()
    {
        m_Path.ResetPath();
        m_bIsAutoSearching = false;
        m_EndPointCache.Clean();
        if (SceneMapLogic.Instance() != null)
        {
            SceneMapLogic.Instance().StopPlayClickEffect();
        }
    }

    //向最新点移动
    void BeginMove()
    {
        if (m_Path.AutoSearchPosCache.Count > 0)
        {
            //首先检测场景是否对应，如果不对则直接返回
            if (GameManager.gameManager.RunningScene != m_Path.AutoSearchPosCache[0].SceneID)
            {
                return;
            }

            Vector3 pos = new Vector3(m_Path.AutoSearchPosCache[0].PosX, 0, m_Path.AutoSearchPosCache[0].PosZ);
			//pos.y=this.gameObject.transform.position.y;
          //  pos = ActiveScene.GetTerrainPosition(pos);
		pos.y=GameManager.gameManager.ActiveScene.GetNavSampleHeight (pos);
            if (Singleton<ObjManager>.GetInstance().MainPlayer.IsCanOperate_Move())
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.MoveTo(pos, null, 1.0f, true);
            }
        }
    }

    //到达当前点
    void GotoNextPoint()
    {
        //移除第一个点
        m_Path.AutoSearchPosCache.RemoveAt(0);

        //主角向下一个点进发
        BeginMove();
    }

    //自动寻路结束
    void Finish()
    {
        Singleton<ObjManager>.Instance.MainPlayer.m_playerHeadInfo.ToggleXunLu(false);
		ObjManager.Instance.MainPlayer.AutoXunLu=false;
        //如果有结束后的回调事件则回调
        if (null != m_Path.FinishCallBackEvent && GameDefine_Globe.EVENT_DEFINE.EVENT_INVALID != m_Path.FinishCallBackEvent.EventID)
        {
            Singleton<EventSystem>.GetInstance().PushEvent(m_Path.FinishCallBackEvent);
        }

        //如果有目标名字则寻找该目标，进行交互
        if (m_Path.AutoSearchTargetName != "")
        {
            Obj_Character obj = Singleton<ObjManager>.GetInstance().FindObjCharacterInSceneByName(m_Path.AutoSearchTargetName);
            if (obj && obj.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_NPC)
            {
                Obj_NPC objNpc = obj as Obj_NPC;
                //根据目标NPC的势力确认是对话还是攻击
                Singleton<ObjManager>.GetInstance().MainPlayer.OnSelectTarget(objNpc.gameObject);
                if (Reputation.IsEnemy(objNpc) || Reputation.IsNeutral(objNpc))
                {
                    //如果是地方NPC，则开始攻击
                    Singleton<ObjManager>.GetInstance().MainPlayer.OnEnterCombat(objNpc.GetComponent<Obj_Character>());

                }
                else
                {
                    //如果是友方NPC，则开始对话
                    Singleton<DialogCore>.GetInstance().Show(objNpc);
                    objNpc.FaceTo(Singleton<ObjManager>.GetInstance().MainPlayer.ObjTransform.position);
                }
            }
        }

        m_EndPointCache.Clean();
        m_Path.ResetPath();
        m_bIsAutoSearching = false;
        if (SceneMapLogic.Instance() != null)
        {
            SceneMapLogic.Instance().StopPlayClickEffect();
        }
    }

    //当自动寻路过程中遇到传送点
    public void ProcessTelepoint(TeleportPoint telePoint)
    {
        //传送点数据异常，返回
        if (null == telePoint || null == telePoint.gameObject)
        {
            return;
        }

        //非自动寻路状态，返回
        if (false == m_bIsAutoSearching)
        {
            return;
        }

        //自动寻路状态，并且路径中有数值
        if (m_Path.AutoSearchPosCache.Count > 0)
        {
            //判断是不是本次跨场景寻路所需传送点
            Vector2 autoSearchPoint = new Vector2(m_Path.AutoSearchPosCache[0].PosX, m_Path.AutoSearchPosCache[0].PosZ);
            Vector2 telePortPoint = new Vector2(telePoint.gameObject.transform.position.x, telePoint.gameObject.transform.position.z);
            //距离校验，因为自动寻路点为2D，所以转化为2D坐标验证
            if (Vector2.Distance(autoSearchPoint, telePortPoint) <= telePoint.ActiveRadius + 2.0f)
            {
                m_Path.AutoSearchPosCache.RemoveAt(0);

                //如果此时路径点中无数据，则结束自动寻路
                if (m_Path.AutoSearchPosCache.Count == 0)
                {
                    Finish();
                }
            }
        }
    }
}
