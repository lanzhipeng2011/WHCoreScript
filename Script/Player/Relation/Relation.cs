/********************************************************************************
 *	文件名：	Relation.cs
 *	全路径：	\Script\Player\Relation\Relation.cs
 *	创建人：	李嘉
 *	创建时间：2014-02-14
 *
 *	功能说明：游戏玩家关系人数据
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System;
using System.Collections;
using Games.GlobeDefine;

public class Relation
{
    //Guid
    private UInt64 m_Guid;
    public UInt64 Guid
    {
        get { return m_Guid; }
        set { m_Guid = value; }
    }

    //角色名字
    private string m_szName;
    public string Name
    {
        get { return m_szName; }
        set { m_szName = value; }
    }

    //等级
    private int m_nLevel;
    public int Level
    {
        get { return m_nLevel; }
        set { m_nLevel = value; }
    }

    //职业
    private int m_nProfession;
    public int Profession
    {
        get { return m_nProfession; }
        set { m_nProfession = value; }
    }

    //战斗力
    private int m_nCombatNum;
    public int CombatNum
    {
        get { return m_nCombatNum; }
        set { m_nCombatNum = value; }
    }

    //状态
    private int m_nState = (int)CharacterDefine.RELATION_TYPE.OFFLINE;
    public int State
    {
        get { return m_nState; }
        set { m_nState = value; }
    }

    //合法性判断
    public bool IsValid()
    {
        return m_Guid != GlobeVar.INVALID_GUID;
    }
}
