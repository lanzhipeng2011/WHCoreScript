/********************************************************************************
 *	文件名：	NearbyPlayer.cs
 *	全路径：	\Script\Player\Team\NearbyPlayer.cs
 *	创建人：	李嘉
 *	创建时间：2014-02-10
 *
 *	功能说明：附近玩家基础数据
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System;
using System.Collections;
using Games.GlobeDefine;

public class NearbyPlayer 
{
    public NearbyPlayer()
    {
        CleanUp();
    }

    public void CleanUp()
    {
        m_Guid = GlobeVar.INVALID_GUID;
        m_szName = "";
        m_nLevel = 0;
        m_nProfession = 0;
        m_nCombatNum = 0;
    }

    public bool IsValid()
    {
        return (m_Guid != GlobeVar.INVALID_GUID);
    }

    private UInt64 m_Guid;               //玩家GUID
    public UInt64 Guid
    {
        get { return m_Guid; }
        set { m_Guid = value; }
    }

    private string m_szName;        //玩家名字
    public string Name
    {
        get { return m_szName; }
        set { m_szName = value; }
    }

    private int m_nLevel;               //玩家等级
    public int Level
    {
        get { return m_nLevel; }
        set { m_nLevel = value; }
    }

    private int m_nProfession;          //玩家职业
    public int Profession
    {
        get { return m_nProfession; }
        set { m_nProfession = value; }
    }
    
    private int m_nCombatNum;           //战斗力
    public int CombatNum
    {
        get { return m_nCombatNum; }
        set { m_nCombatNum = value; }
    }
}
