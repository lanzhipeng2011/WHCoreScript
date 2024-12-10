/********************************************************************************
 *	文件名：	AutoMove.cs
 *	全路径：	\Script\Player\AutoSearch\AutoMove.cs
 *	创建人：	李嘉
 *	创建时间：2014-03-20
 *
 *	功能说明： 客户端非主角Obj接收服务器移动消息包后的模拟移动类
 *	          
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using Games.LogicObj;
using Games.GlobeDefine;
using Games.Scene;

//模拟移动点结构
struct AutoMovePos
{
    public int m_nPosSerial;                               //当前点序列号
    public float m_fDstPosX;                               //当前点目的地X
    public float m_fDstPosZ;                               //当前点目的地Z
}

public class AutoMove : MonoBehaviour
{
    private List<AutoMovePos> m_posList = null;             //移动路径列表
    private int m_nCurPosSerial = 0;                        //当前移动的路径序列
    private bool m_bIsMove = false;                         //是否正在移动

    private Obj_Character m_BindObj = null;                 //绑定的Obj_Character对象

    public void Awake()
    {
        m_posList = new List<AutoMovePos>();

        //必须是Obj_Character才有效
        m_BindObj = gameObject.GetComponent<Obj_Character>();
    }

    public void ResetAutoMove()
    {
        m_posList.Clear();
        m_nCurPosSerial = 0;
        m_bIsMove = false;
    }

    //根据服务器GC_MOVE包插入新的路经点
    public void InsertAutoMovePoint(GC_MOVE packet)
    {
        if (null == m_posList || null == m_BindObj)
        {
            return;
        }


        //校验ServerID
        if (packet.Serverid != m_BindObj.ServerID)
        {
            return;
        }

        for (int i = 0; i < packet.posserialCount; ++i)
        {
            AutoMovePos pos = new AutoMovePos();
            pos.m_nPosSerial = packet.posserialList[i];
            pos.m_fDstPosX = ((float)packet.posxList[i])/100;
            pos.m_fDstPosZ = ((float)packet.poszList[i])/100;

            //措施，当发现堆积点过多的时候，清空一下，只插入头一个点
            if (m_posList.Count >= 32)
            {
                ResetAutoMove();
            }
            m_posList.Add(pos);
        }
    }

    private bool IsReachPoint(AutoMovePos pos)
    {
        //先判断序列号
        if (pos.m_nPosSerial != m_nCurPosSerial)
        {
            return false;
        }

        Vector2 dstPos = new Vector2(pos.m_fDstPosX, pos.m_fDstPosZ);
        Vector2 curPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.z);
        if (Vector2.Distance(curPos, dstPos) < 0.5f)
        {
            return true;
        }

        return false;
    }

    //到达当前点，删除当前点，向下一个点移动
    private void ReachPoint()
    {
        if (null == m_posList || m_posList.Count <= 0)
        {
            return;
        }

#if UNITY_ANDROID

        BeginMove(m_posList[0]);
        //移除当前点
        m_posList.RemoveAt(0);

#else
        //移除当前点
        m_posList.RemoveAt(0);

        //判断剩余点
        if (m_posList.Count <= 0)
        {
            //已经无点，停止移动
            StopMove();
        }
        else
        {
            BeginMove(m_posList[0]);
        }

#endif
    }

    //移动开始
    private void BeginMove(AutoMovePos dstPos)
    {
        if (null == m_BindObj)
        {
            return;
        }

        //记录当前点序列号
        m_nCurPosSerial = dstPos.m_nPosSerial;

        //开始移动
        Vector3 dest = new Vector3(dstPos.m_fDstPosX, 0.0f, dstPos.m_fDstPosZ);
        //dest = ActiveScene.GetTerrainPosition(dest);
		//dest.y=GameManager.gameManager.ActiveScene.GetNavSampleHeight (dest);
		dest.y = this.gameObject.transform.position.y;
        if (null != Terrain.activeTerrain)
        {
            dest.y = Terrain.activeTerrain.SampleHeight(dest);
        }

        m_bIsMove = true;

        m_BindObj.MoveTo(dest, null, 0.0f);
    }

    //移动结束
    private void StopMove()
    {
        if (null != m_BindObj)
        {
            m_BindObj.StopMove();
        }

        ResetAutoMove();
    }

    //强制打断移动
    //策略是找到路径中和GC_STOP的Serial相同点，走完之前点，删除之后点，并且将该点的目的地变为GC_STOP中的坐标
    public void InterruptMove(GC_STOP packet)
    {
        if (null == m_BindObj || m_BindObj.ServerID != packet.Serverid)
        {
            return;
        }

        for (int i = 0; i < m_posList.Count; ++i)
        {
            if (m_posList[i].m_nPosSerial == packet.Posserial)
            {
                //如果不是最后一个点，则清除该点之后所有点
                if (i < m_posList.Count-1)
                {
                    m_posList.RemoveRange(i + 1, m_posList.Count - i - 1);
                }

                //修改i数值
                AutoMovePos autoPos = m_posList[i];
                autoPos.m_fDstPosX = ((float)packet.Posx) / 100;
                autoPos.m_fDstPosZ = ((float)packet.Posz) / 100;

                //删除原有数据，插入新数值
                m_posList.RemoveAt(i);
                m_posList.Add(autoPos);

                //如果是第一个点，则重新发起移动
                if (i == 0)
                {
                    BeginMove(m_posList[i]);
                }

                return;
            }
        }

        //如果没找到点，也强制打断移动
        StopMove();
    }

    //卡点检测
    const float m_fObstacleTestInterval = 1.0f;
    float m_fObstacleTestTime = 0.0f;
    private Vector3 m_LastPos = Vector3.zero;
    void ObstacleTest()
    {
        if (!m_bIsMove)
        {
            return;
        }

        if (Time.time - m_fObstacleTestTime > m_fObstacleTestInterval)
        {
            m_fObstacleTestTime = Time.time;
        }
        else
        {
            return;
        }

        //长时间卡在一个点，则直接SetPos过去，并且删除当前点
        if (Vector3.Distance(m_LastPos, gameObject.transform.position) < 0.1f)
        {
            Vector3 pos = new Vector3(m_posList[0].m_fDstPosX, 0, m_posList[0].m_fDstPosZ);
            if (GameManager.gameManager.ActiveScene.IsT4MScene())
            {
                pos.y = GameManager.gameManager.ActiveScene.GetTerrainHeight(pos);
            }
            else if (null != Terrain.activeTerrain)
            {
                pos.y = Terrain.activeTerrain.SampleHeight(pos);
            }
            gameObject.transform.position = pos;
            ReachPoint();
        }

        //记录上一次坐标
        m_LastPos = gameObject.transform.position;
    }
    
    void FixedUpdate()
    {
        //如果posList未初始化（可能是主角）或者无自动移动行为，返回
        if (null == m_posList || m_posList.Count <= 0)
        {
            return;
        }

        //防卡机检测
        ObstacleTest();

        //由于防卡机检测中可能将m_postList置空，所以这里要再次检测
        if (m_posList.Count <= 0)
        {
            return;
        }

        if (m_bIsMove)
        {
            //如果正在移动中,判断是否到达
            if (IsReachPoint(m_posList[0]))
            {
                ReachPoint();
                return;
            }
            else
            {
                BeginMove(m_posList[0]);
            }
        }        
        else
        {
            //如果未移动但是posList不为空
            BeginMove(m_posList[0]);
        }
    }
}
