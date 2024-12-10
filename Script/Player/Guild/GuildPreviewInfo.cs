/********************************************************************************
 *	文件名：	GuildPreviewInfo.cs
 *	全路径：	\Script\Player\Guild\GuildPreviewInfo.cs
 *	创建人：	李嘉
 *	创建时间：2014-04-22
 *
 *	功能说明：帮会列表中所需要的数据
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Games.GlobeDefine;

public class GuildPreviewInfo
{
    private UInt64 m_GuildGuid;                     //帮会GUID
    public System.UInt64 GuildGuid
    {
        get { return m_GuildGuid; }
        set { m_GuildGuid = value; }
    }

    private string m_GuildName;                     //帮会GUID
    public string GuildName
    {
        get { return m_GuildName; }
        set { m_GuildName = value; }
    }

    private int m_GuildLevel;                       //帮会等级
    public int GuildLevel
    {
        get { return m_GuildLevel; }
        set { m_GuildLevel = value; }
    }
    
    private string m_GuildChiefName;                //帮主姓名
    public string GuildChiefName
    {
        get { return m_GuildChiefName; }
        set { m_GuildChiefName = value; }
    }

    private int m_GuildCurMemberNum;                //帮会当前人数
    public int GuildCurMemberNum
    {
        get { return m_GuildCurMemberNum; }
        set { m_GuildCurMemberNum = value; }
    }

    private int m_GuildCombatValue;                //帮会当前人数
    public int GuildCombatValue
    {
        get { return m_GuildCombatValue; }
        set { m_GuildCombatValue = value; }
    }

    public int GuildMaxMemberNum
    {
        get { return GlobeVar.GetGuildMemberMax(m_GuildLevel); }
    }
    private bool m_bIsEnemyGuild = false;
	public bool IsEnemyGuild
	{
		get { return m_bIsEnemyGuild; }
		set { m_bIsEnemyGuild = value; }
	}

    private int m_GuildCurApplyNum;
    public int GuildCurApplyNum
    {
        get { return m_GuildCurApplyNum; }
        set { m_GuildCurApplyNum = value; }
    }

    private int m_GuildMaxApplyNum = 100;
    public int GuildMaxApplyNum
    {
        get { return m_GuildMaxApplyNum; }
    }
}
