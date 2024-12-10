/********************************************************************************
 *	文件名：	Guild.cs
 *	全路径：	\Script\Player\Guild\Guild.cs
 *	创建人：	李嘉
 *	创建时间：2014-04-22
 *
 *	功能说明：帮会基础数据
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Games.GlobeDefine;

public class Guild
{
    public Guild()
    {
        CleanUp();
    }

    public void CleanUp()
    {
        m_GuildGuid = GlobeVar.INVALID_GUID;
        m_GuildName = "";
        m_GuildLevel = 0;
        m_GuildChiefGuid = GlobeVar.INVALID_GUID;
        if (null == m_GuildMemberList)
        {
            m_GuildMemberList = new Dictionary<UInt64, GuildMember>();
        }

        m_GuildMemberList.Clear();
    }

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
    
    private UInt64 m_GuildChiefGuid;                //帮主Guid
    public System.UInt64 GuildChiefGuid
    {
        get { return m_GuildChiefGuid; }
        set { m_GuildChiefGuid = value; }
    }

    private int m_GuildExp;                         //帮会繁荣度
    public int GuildExp
    {
        get { return m_GuildExp; }
        set { m_GuildExp = value; }
    }

    private string m_GuildNotice;                   //帮会公告
    public string GuildNotice
    {
        get { return m_GuildNotice; }
        set { m_GuildNotice = value; }
    }

    private Dictionary<UInt64, GuildMember> m_GuildMemberList = null;    //帮会成员列表
    public Dictionary<UInt64, GuildMember> GuildMemberList
    {
        get { return m_GuildMemberList; }
    }

    private UInt64 m_PreserveGuildGuid;             //玩家当前加入的待审批帮会Guid,在申请帮会列表的时候同步
    public System.UInt64 PreserveGuildGuid
    {
        get { return m_PreserveGuildGuid; }
        set { m_PreserveGuildGuid = value; }
    }

    public void UpdateData(GC_GUILD_RET_INFO info)
    {
        //清空之前的数据
        CleanUp();

        //判断消息包数据合法性
        if (null == info || info.GuildGuid == GlobeVar.INVALID_GUID)
        {
            return;
        }

        //填充数据
        GuildGuid = info.GuildGuid;
        GuildLevel = info.GuildLevel;
        GuildName = info.GuildName;
        GuildChiefGuid = info.GuildChiefGuid;
        GuildExp = info.GuildExp;
        GuildNotice = info.GuildNotice;

        for (int i = 0; i < info.memberGuidCount; ++i )
        {
            GuildMember member = new GuildMember();
            //member.CleanUp();

            member.Guid = info.GetMemberGuid(i);

            if (info.memberNameCount > i)
            {
                member.MemberName = info.GetMemberName(i);
            }
            if (info.memberVIPCount > i)
            {
                member.VIP = info.GetMemberVIP(i);
            }
            if (info.memberLevelCount > i)
            {
                member.Level = info.GetMemberLevel(i);
            }
            if (info.memberJobCount > i)
            {
                member.Job = info.GetMemberJob(i);
            }
            if (info.memberLastLoginCount > i)
            {
                member.LastLogin = info.GetMemberLastLogin(i);
            }
            if (info.memberProfCount > i)
            {
                member.Profession = info.GetMemberProf(i);
            }
            if (info.memberStateCount > i)
            {
                member.State = info.GetMemberState(i);
            }
            if (info.memberContirbuteCount > i)
            {
                member.Contribute = info.GetMemberContirbute(i);
            }
            if (info.combatvalCount > i)
            {
                member.ComBatVal = info.GetCombatval(i);
            }
            if (member.IsValid())
            {
                GuildMemberList.Add(member.Guid, member);
            }
        }

        //按照VIP等级排序
        SortMemberListByVIP(m_GuildMemberList);
        //按照等级排序
        SortMemberListByLevel(m_GuildMemberList);
        //按照在线状态进行排序
        SortMemberListByOnLine(m_GuildMemberList);
        //按照职位排序
        SortMemberListByJob(m_GuildMemberList);

        if (ChatInfoLogic.Instance() != null)
        {
            ChatInfoLogic.Instance().UpdateSpeakerList_Guild();
            ChatInfoLogic.Instance().UpdateSpeakerList_Master();
        }
    }

    public int GetGuildFormalMemberCount()
    {
        int nNum = 0;
        foreach (KeyValuePair<ulong, GuildMember> member in m_GuildMemberList)
        {
            if (member.Value.IsValid() && member.Value.Job != (int)GameDefine_Globe.GUILD_JOB.RESERVE)
            {
                nNum++;
            }
        }

        return nNum;
    }

    public int GetGuildReserveMemberCount()
    {
        int nNum = 0;
        foreach (KeyValuePair<ulong, GuildMember> member in m_GuildMemberList)
        {
            if (member.Value.IsValid() && member.Value.Job == (int)GameDefine_Globe.GUILD_JOB.RESERVE)
            {
                nNum++;
            }
        }

        return nNum;
    }

    //根据会员Guid获得其职位
    public int GetMemberJob(UInt64 memberGuid)
    {
        int nJob = (int)GameDefine_Globe.GUILD_JOB.INVALID;
        foreach (KeyValuePair<ulong, GuildMember> member in m_GuildMemberList)
        {
            if (member.Value.IsValid() && member.Value.Guid == memberGuid)
            {
                nJob = member.Value.Job;
                break;
            }
        }

        return nJob;
    }

    //根据会员Guid获得其等级
    public int GetMemberLevel(UInt64 memberGuid)
    {
        int nLevel = (int)GameDefine_Globe.GUILD_JOB.INVALID;
        foreach (KeyValuePair<ulong, GuildMember> member in m_GuildMemberList)
        {
            if (member.Value.IsValid() && member.Value.Guid == memberGuid)
            {
                nLevel = member.Value.Level;
                break;
            }
        }

        return nLevel;
    }

    //根据会员Guid获得其贡献度
    public int GetMemberContribute(UInt64 memberGuid)
    {
        int nContribute = (int)GameDefine_Globe.GUILD_JOB.INVALID;
        foreach (KeyValuePair<ulong, GuildMember> member in m_GuildMemberList)
        {
            if (member.Value.IsValid() && member.Value.Guid == memberGuid)
            {
                nContribute = member.Value.Contribute;
                break;
            }
        }

        return nContribute;
    }

    //获得MainPlayer的帮会信息
    public GuildMember GetMainPlayerGuildInfo()
    {
        GuildMember mainPlayerGuildInfo;
        if (null != Singleton<ObjManager>.GetInstance().MainPlayer &&
            GuildMemberList.TryGetValue(Singleton<ObjManager>.GetInstance().MainPlayer.GUID, out mainPlayerGuildInfo))
        {
            return mainPlayerGuildInfo;
        }

        return null;
    }

    protected Dictionary<UInt64, GuildMember> SortMemberListByJob(Dictionary<UInt64, GuildMember> dic)
    {
        List<KeyValuePair<UInt64, GuildMember>> myList = new List<KeyValuePair<UInt64, GuildMember>>(dic);
        myList.Sort(delegate(KeyValuePair<UInt64, GuildMember> s1, KeyValuePair<UInt64, GuildMember> s2)
        {
            return s1.Value.Job.CompareTo(s2.Value.Job);
        });

        dic.Clear();
        foreach (KeyValuePair<UInt64, GuildMember> pair in myList)
        {
            dic.Add(pair.Key, pair.Value);
        }
        return dic;
    }

    protected Dictionary<UInt64, GuildMember> SortMemberListByOnLine(Dictionary<UInt64, GuildMember> dic)
    {
        List<KeyValuePair<UInt64, GuildMember>> myList = new List<KeyValuePair<UInt64, GuildMember>>(dic);
        myList.Sort(delegate(KeyValuePair<UInt64, GuildMember> s1, KeyValuePair<UInt64, GuildMember> s2)
        {
            return s2.Value.State.CompareTo(s1.Value.State);
        });

        dic.Clear();
        foreach (KeyValuePair<UInt64, GuildMember> pair in myList)
        {
            dic.Add(pair.Key, pair.Value);
        }
        return dic;
    }

    protected Dictionary<UInt64, GuildMember> SortMemberListByVIP(Dictionary<UInt64, GuildMember> dic)
    {
        List<KeyValuePair<UInt64, GuildMember>> myList = new List<KeyValuePair<UInt64, GuildMember>>(dic);
        myList.Sort(delegate(KeyValuePair<UInt64, GuildMember> s1, KeyValuePair<UInt64, GuildMember> s2)
        {
            return s2.Value.VIP.CompareTo(s1.Value.VIP);
        });

        dic.Clear();
        foreach (KeyValuePair<UInt64, GuildMember> pair in myList)
        {
            dic.Add(pair.Key, pair.Value);
        }
        return dic;
    }

    protected Dictionary<UInt64, GuildMember> SortMemberListByLevel(Dictionary<UInt64, GuildMember> dic)
    {
        List<KeyValuePair<UInt64, GuildMember>> myList = new List<KeyValuePair<UInt64, GuildMember>>(dic);
        myList.Sort(delegate(KeyValuePair<UInt64, GuildMember> s1, KeyValuePair<UInt64, GuildMember> s2)
        {
            return s2.Value.Level.CompareTo(s1.Value.Level);
        });

        dic.Clear();
        foreach (KeyValuePair<UInt64, GuildMember> pair in myList)
        {
            dic.Add(pair.Key, pair.Value);
        }
        return dic;
    }

    //帮会财富数据

    private string[] m_GBRobbedGuildName = new string[6];                  //抢劫信息
    public string GetGBRobbedGuildName(int index)
    {
        if (index >= 0 && index < m_GBRobbedGuildName.Length)
        {
            return m_GBRobbedGuildName[index];
        }
        return string.Empty;
    }

    private string[] m_GBRobbPlayerName = new string[6];                  //抢劫信息
    public string GetGBRobbPlayerName(int index)
    {
        if (index >= 0 && index < m_GBRobbPlayerName.Length)
        {
            return m_GBRobbPlayerName[index];
        }
        return string.Empty;
    }

    private bool m_GBIsAuto;                          //是否自动开启帮会跑商
    public bool GBIsAuto
    {
        get
        {
            return m_GBIsAuto;
        }
    }

    private int m_GBCanAssignTime;
    public int GBCanAssignTime
    {
        get
        {
            return m_GBCanAssignTime;
        }
    }

    private int m_GBOneWeekTime;
    public int GBOneWeekTime
    {
        get
        {
            return m_GBOneWeekTime;
        }
    }

    private int m_GBCanAcceptTime;
    public int GBCanAcceptTime
    {
        get
        {
            return m_GBCanAcceptTime;
        }
    }

    private int m_GuildWeath;
    public int GuildWeath
    {
        get
        {
            return m_GuildWeath;
        }
    }

    public void UpdataBusinessDate(GC_RET_GUILDBUSINESSINFO info)
    {
        //清空之前的数据
        CleanUpBusiness();

        //判断消息包数据合法性
        if (null == info)
        {
            return;
        }

        for (int i = 0; i < info.robGuildNameCount; i++)
        {
            m_GBRobbedGuildName[i] = info.GetRobGuildName(i);
            m_GBRobbPlayerName[i] = info.GetRobName(i);
        }
        m_GBIsAuto = (info.IsAutoAssign == 1);
        m_GBCanAssignTime = info.CurAssignTime;
        m_GBOneWeekTime = info.MaxAssignTime;
        m_GBCanAcceptTime = info.CurCanAcceptTime;
        m_GuildWeath = info.WealthNum;
    }

    private void CleanUpBusiness()
    {
        for (int i = 0; i < m_GBRobbedGuildName.Length; i++)
        {
            m_GBRobbedGuildName[i] = string.Empty;
            m_GBRobbPlayerName[i] = string.Empty;
        }
        m_GBIsAuto = false;
        m_GBCanAssignTime = 0;
        m_GBOneWeekTime = 0;
        m_GBCanAcceptTime = 0;
        m_GuildWeath = 0;
    }

    private int m_GMCurAssign;
    public int GMCurAssign
    {
        get { return m_GMCurAssign; }
    }

    private int m_GMMaxAssign;
    public int GMMaxAssign
    {
        get { return m_GMMaxAssign; }
    }

    private int m_GMCurPartake;
    public int GMCurPartake
    {
        get { return m_GMCurPartake; }
    }

    private int m_GMMaxPartake;
    public int GMMaxPartake
    {
        get { return m_GMMaxPartake; }
    }

    private int m_GMPartakeType;
    public int GMCanPartakeType
    {
        get { return m_GMPartakeType; }
    }

    private int m_GMMisionID = -1;
    public int CMMisionID
    {
        get { return m_GMMisionID; }
    }

    private int m_GMDoneNum = 0;
    public int GMDoneNum
    {
        get { return m_GMDoneNum; }
    }

    public void UpdateGuildMissionInfo(GC_RET_GUILDMISSIONINFO pak)
    {
        if (null == pak)
        {
            return;
        }

        m_GMCurAssign = pak.CurAssign;
        m_GMMaxAssign = pak.MaxAssign;
        m_GMCurPartake = pak.CurPartake;
        m_GMMaxPartake = pak.MaxPartake;
        m_GMPartakeType = pak.IsPartake;

        if (pak.HasCurMissionId)
        {
            m_GMMisionID = pak.CurMissionId;
        }
        else
        {
            m_GMMisionID = -1;
        }

        if (pak.HasCirNum)
        {
            m_GMDoneNum = pak.CirNum;
        }
        else
        {
            m_GMMisionID = 0;
        }
    }

    public bool CanGMAssign
    {
        get { return m_GMCurAssign > 0; }
    }

    public bool GuildLeftMissionTime
    {
        get { return m_GMCurPartake > 0; }
    }
   
}
