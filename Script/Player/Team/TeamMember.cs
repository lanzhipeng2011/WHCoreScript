/********************************************************************************
 *	文件名：	TeamMember.cs
 *	全路径：	\Script\Player\Team\TeamMember.cs
 *	创建人：	李嘉
 *	创建时间：2014-01-09
 *
 *	功能说明：组队队员基础数据
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using System;
using Games.GlobeDefine;

public class TeamMember
{
    public TeamMember()
    {
        CleanUp();
    }

    public void CleanUp()
    {
        m_Guid = GlobeVar.INVALID_GUID;
        m_MemberName = "";
        m_nLevel = 0;
        m_nProfession = 0;
        m_nHP = 0;
        m_nMaxHP = 0;
        m_nTeamJob = -1;
        m_nCombatNum = 0;
        m_nSceneClassID = GlobeVar.INVALID_ID;
        m_nSceneInstID = GlobeVar.INVALID_ID;
        m_ScenePos = Vector3.zero;
    }

    public bool IsValid()
    {
        return (m_Guid != GlobeVar.INVALID_GUID);
    }

    private UInt64 m_Guid;               //队员GUID
    public UInt64 Guid
    {
        get { return m_Guid; }
        set { m_Guid = value; }
    }

    private string m_MemberName;        //队员名字
    public string MemberName
    {
        get { return m_MemberName; }
        set { m_MemberName = value; }
    }

    private int m_nLevel;               //队员等级
    public int Level
    {
        get { return m_nLevel; }
        set { m_nLevel = value; }
    }

    private int m_nProfession;          //队员职业
    public int Profession
    {
        get { return m_nProfession; }
        set { m_nProfession = value; }
    }

    private int m_nHP;                  //队员当前生命值
    public int HP
    {
        get { return m_nHP; }
        set { m_nHP = value; }
    }

    private int m_nMaxHP;               //队员最大生命值
    public int MaxHP
    {
        get { return m_nMaxHP; }
        set { m_nMaxHP = value; }
    }

    private int m_nTeamJob;             //队员职位，0为队长，1为队员
    public int TeamJob
    {
        get { return m_nTeamJob; }
        set { m_nTeamJob = value; }
    }

    private int m_nCombatNum;           //战斗力
    public int CombatNum
    {
        get { return m_nCombatNum; }
        set { m_nCombatNum = value; }
    }

    private int m_nSceneClassID;
    public int SceneClassID
    {
        get { return m_nSceneClassID; }
        set { m_nSceneClassID = value; }
    }

    private int m_nSceneInstID;
    public int SceneInstID
    {
        get { return m_nSceneInstID; }
        set { m_nSceneInstID = value; }
    }

    private Vector3 m_ScenePos;
    public UnityEngine.Vector3 ScenePos
    {
        get { return m_ScenePos; }
        set { m_ScenePos = value; }
    }


}
