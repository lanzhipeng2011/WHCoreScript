/********************************************************************************
 *	文件名：	PatrolPoint.cs
 *	全路径：	\Script\Player\AI\PatrolPoint.cs
 *	创建人：	李嘉
 *	创建时间：2013-11-19
 *
 *	功能说明： 路点基础数据，支持从路点表格PatrolPoint.txt中加载
 *	          
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GCGame.Table;
using Games.GlobeDefine;

namespace Games.AI_Logic
{
    public class PatrolPoint
    {
        //寻路基本信息
        public PatrolPoint()
        {
            ResetPatrolPoint();
        }

        //根据表格ID获取巡逻信息
        public bool InitPatrolPoint(int nTableID)
        {
            ResetPatrolPoint();

            Tab_PatrolPoint patrolTable = TableManager.GetPatrolPointByID(nTableID, 0);
            if (patrolTable != null)
            {
                m_nSceneResID = patrolTable.SceneID;
                //目前暂时用数字表示上限
                for (int i = 0; i < 8; ++i)
                {
                    if (GlobeVar.INVALID_ID != patrolTable.GetPXbyIndex(i) &&
                        GlobeVar.INVALID_ID != patrolTable.GetPYbyIndex(i))
                    {
                        m_listPatrolPoint.Add(new Vector2(patrolTable.GetPXbyIndex(i), patrolTable.GetPYbyIndex(i)));
                    }
                }
            }

            return true;
        }

        public void AddPatrolPoint(int nX, int nY)
        {
            AddPatrolPoint(new Vector2(nX, nY));
        }

        public void AddPatrolPoint(Vector2 point)
        {
            if (null != m_listPatrolPoint)
            {
                m_listPatrolPoint.Add(point);
            }
        }

        public void ResetPatrolPoint()
        {
            m_nSceneResID = GlobeVar.INVALID_ID;
            m_listPatrolPoint = new List<Vector2>();
        }

        private int m_nSceneResID;                      //路点所在场景资源ID
        public int SceneResID
        {
            get { return m_nSceneResID; }
            set { m_nSceneResID = value; }
        }

        private List<Vector2> m_listPatrolPoint;        //路点序列
        public List<Vector2> ListPatrolPoint
        {
            get { return m_listPatrolPoint; }
            set { m_listPatrolPoint = value; }
        }
    }

}
