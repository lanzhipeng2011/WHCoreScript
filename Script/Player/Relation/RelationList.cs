/********************************************************************************
 *	文件名：	RelationList.cs
 *	全路径：	\Script\Player\Relation\RelationList.cs
 *	创建人：	李嘉
 *	创建时间：2014-02-14
 *
 *	功能说明：游戏玩家关系人数据列表
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Games.GlobeDefine;

public class RelationList
{
    public RelationList()
    {
        m_RelationDataList = new Dictionary<UInt64, Relation>();
    }

    private Dictionary<UInt64, Relation> m_RelationDataList;        //关系人数据列表
    public Dictionary<UInt64, Relation> RelationDataList
    {
        get { return m_RelationDataList; }
        set { m_RelationDataList = value; }
    }

    public void CleanUp()
    {
        if (null != m_RelationDataList)
        {
            m_RelationDataList.Clear();
        }        
    }
    //关系人列表接口

    //添加关系人
    public bool AddRelation(Relation _relation)
    {
        //非法ID，则返回
        if (null == _relation || _relation.Guid == GlobeVar.INVALID_GUID)
        {
            return false;
        }

        //如果发现已存在，则先remove
        bool bCanAdd = true;
        if (m_RelationDataList.ContainsKey(_relation.Guid))
        {
            bCanAdd = m_RelationDataList.Remove(_relation.Guid);
        }

        if (!bCanAdd)
        {
            return false;
        }

        //发现可添加，则加入关系人
        m_RelationDataList.Add(_relation.Guid, _relation);
        return true;  
    }

    //删除关系人
    public void DelRelation(UInt64 relationGuid)
    {
        //非法ID，则返回
        if (relationGuid == GlobeVar.INVALID_GUID)
        {
            return;
        }

        //如果发现已存在，则remove
        if (m_RelationDataList.ContainsKey(relationGuid))
        {
            m_RelationDataList.Remove(relationGuid);
        }
    }

    //获得关系人数量
    public int GetRelationNum()
    {
        return m_RelationDataList.Count;
    }

    //获得某种状态关系人数量
    public int GetStateRelationNum(int relationState)
    {
        int nCount = 0;
        foreach(KeyValuePair<UInt64, Relation> _relation in m_RelationDataList)
        {
            if (_relation.Value.State == relationState)
            {
                nCount++;
            }
        }

        return nCount;
    }
    
    //更新关系人，封装一层接口，方便调用函数时候辨认；
    public void UpdateRelation(Relation _relation)
    {
        AddRelation(_relation);
    }

    //更新关系人状态
    public void UpdateRelationState(UInt64 relationGuid, int state)
    {
        //非法ID，则返回
        if (relationGuid == GlobeVar.INVALID_GUID)
        {
            return;
        }

        //如果存在，则获取联系人并设置状态
        Relation _relation = null;
        if (true == m_RelationDataList.TryGetValue(relationGuid, out _relation))
        {
            _relation.State = state;
        }
    }

    //更新关系人实时状态
    public void UpdateRelationUserInfo(Relation _relation)
    {
        if (null == _relation || _relation.Guid == GlobeVar.INVALID_GUID)
        {
            return;
        }

        //更新信息
        if (m_RelationDataList.ContainsKey(_relation.Guid))
        {
            Relation relation = m_RelationDataList[_relation.Guid];
            if (null != relation)
            {
                //由于UserInfo只更新少数数据，所以不能直接
                relation.Level = _relation.Level;
                relation.Profession = _relation.Profession;
                relation.State = _relation.State;
                relation.CombatNum = _relation.CombatNum;
                relation.Name = _relation.Name;
            }
        }
    }
    
    //更新关系人所有数据
    //由于消息包不同，所以该方法会被重载
    public void RebuildRelationList(GC_SYC_FULL_FRIEND_LIST packet)
    {
        if (null == m_RelationDataList || null == packet)
        {
            return;
        }

        m_RelationDataList.Clear();
        for (int i = 0; i < packet.guidCount; ++i)
        {
            Relation _relation = new Relation();
            _relation.Guid = packet.GetGuid(i);
            _relation.Level = packet.GetLevel(i);
            _relation.Name = packet.GetName(i);
            _relation.Profession = packet.GetProf(i);
            _relation.CombatNum = packet.GetCombat(i);
            _relation.State = packet.GetState(i);

            if (_relation.IsValid())
            {
                m_RelationDataList.Add(_relation.Guid, _relation);
            }
        }
    }

    public void RebuildRelationList(GC_SYC_FULL_BLACK_LIST packet)
    {
        if (null == m_RelationDataList || null == packet)
        {
            return;
        }

        m_RelationDataList.Clear();
        for (int i = 0; i < packet.guidCount; ++i)
        {
            Relation _relation = new Relation();
            _relation.Guid = packet.GetGuid(i);
            _relation.Level = packet.GetLevel(i);
            _relation.Name = packet.GetName(i);
            _relation.Profession = packet.GetProf(i);
            _relation.CombatNum = packet.GetCombat(i);
            _relation.State = packet.GetState(i);

            if (_relation.IsValid())
            {
                m_RelationDataList.Add(_relation.Guid, _relation);
            }
        }
    }

    public void RebuildRelationList(GC_SYC_FULL_HATE_LIST packet)
    {
        if (null == m_RelationDataList || null == packet)
        {
            return;
        }

        m_RelationDataList.Clear();
        for (int i = 0; i < packet.GuidCount; ++i)
        {
            Relation _relation = new Relation();
            _relation.Guid = packet.GetGuid(i);
            _relation.Level = packet.GetLevel(i);
            _relation.Name = packet.GetName(i);
            _relation.Profession = packet.GetProf(i);
            _relation.CombatNum = packet.GetCombat(i);
            _relation.State = packet.GetState(i);

            if (_relation.IsValid())
            {
                m_RelationDataList.Add(_relation.Guid, _relation);
            }
        }
    }

    //按照某个状态整理关系人状态
    //多次排序之后，可以保证按照最后一次Sort排列，并保留部分前次排序结果
    public void SortByRelationState(int _state)
    {
        //临时队列
        Dictionary<UInt64, Relation> tempList = new Dictionary<UInt64, Relation>();
        foreach (KeyValuePair<UInt64, Relation> _relation in m_RelationDataList)
        {
            //如果发现状态不符合，则将关系暂时放入临时列表中
            if (_relation.Value.State != _state)
            {
                tempList.Add(_relation.Key, _relation.Value);
                m_RelationDataList.Remove(_relation.Key);
            }
        }

        foreach (KeyValuePair<UInt64, Relation> _tmpRelation in tempList)
        {
            //插入老数据
            m_RelationDataList.Add(_tmpRelation.Key, _tmpRelation.Value);
        }
    }

    //是否存在某个关系人
    public bool IsExist(UInt64 key)
    {
        return m_RelationDataList.ContainsKey(key);
    }
}
