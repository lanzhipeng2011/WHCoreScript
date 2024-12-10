/********************************************************************************
 *	文件名：	NearbyTeam.cs
 *	全路径：	\Script\Player\Team\NearbyTeam.cs
 *	创建人：	李嘉
 *	创建时间：2014-02-10
 *
 *	功能说明：附近队伍基础数据，显示队长数据
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System;
using System.Collections;
using Games.GlobeDefine;

public class NearbyTeam
{
    public NearbyTeam()
    {
        CleanUp();
    }

    public void CleanUp()
    {
        TeamID = GlobeVar.INVALID_ID;
        m_Guid = GlobeVar.INVALID_GUID;
        m_szName = "";
        m_nLevel = 0;
        m_nProfession = 0;
        m_nCombatNum = 0;
    }

    public bool IsValid()
    {
        return (m_Guid != GlobeVar.INVALID_GUID && m_TeamID != GlobeVar.INVALID_ID);
    }

    private int m_TeamID;                //队伍ID
    public int TeamID
    {
        get { return m_TeamID; }
        set { m_TeamID = value; }
    }
    private UInt64 m_Guid;               //队长GUID
    public UInt64 Guid
    {
        get { return m_Guid; }
        set { m_Guid = value; }
    }

    private string m_szName;        //队长名字
    public string Name
    {
        get { return m_szName; }
        set { m_szName = value; }
    }

    private int m_nLevel;               //队长等级
    public int Level
    {
        get { return m_nLevel; }
        set { m_nLevel = value; }
    }

    private int m_nProfession;          //队长职业
    public int Profession
    {
        get { return m_nProfession; }
        set { m_nProfession = value; }
    }

    private int m_nCombatNum;           //队长战斗力
    public int CombatNum
    {
        get { return m_nCombatNum; }
        set { m_nCombatNum = value; }
    }
}
