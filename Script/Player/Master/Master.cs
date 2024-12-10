/********************************************************************
	filename:	Master.cs
	date:		2014-5-7  11-20
	author:		tangyi
	purpose:	师门数据结构
*********************************************************************/
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Games.GlobeDefine;

public class Master
{
    public Master()
    {
        CleanUp();
    }

    public bool IsValid()
    {
        return (m_MasterGuid != GlobeVar.INVALID_GUID);
    }

    public bool IsMasterChief()
    {
        return (m_MasterChiefGuid != GlobeVar.INVALID_GUID);
    }

    public void CleanUp()
    {
        m_MasterGuid = GlobeVar.INVALID_GUID;
        m_MasterName = "";
        m_MasterChiefGuid = GlobeVar.INVALID_GUID;
        m_MasterTorch = 0;
        m_MasterNotice = "";
        m_MasterCreateTime = 0;
        m_MasterOnlineNum = 0;
        m_MasterMemberNum = 0;
        m_MasterChiefName = "";
        if (m_MasterSkillList == null)
        {
            m_MasterSkillList = new Dictionary<int, string>();
        }
        m_MasterSkillList.Clear();
        if (null == m_MasterMemberList)
        {
            m_MasterMemberList = new Dictionary<UInt64, MasterMember>();
        }
        m_MasterMemberList.Clear();
        if (null == m_MasterReserveMemberList)
        {
            m_MasterReserveMemberList = new Dictionary<UInt64, MasterMember>();
        }
        m_MasterReserveMemberList.Clear();
    }
    //师门GUID
    private UInt64 m_MasterGuid;
    public System.UInt64 MasterGuid
    {
        get { return m_MasterGuid; }
        set { m_MasterGuid = value; }
    }
    //师门名称
    private string m_MasterName;
    public string MasterName
    {
        get { return m_MasterName; }
        set { m_MasterName = value; }
    }
    //掌门Guid
    private UInt64 m_MasterChiefGuid;
    public System.UInt64 MasterChiefGuid
    {
        get { return m_MasterChiefGuid; }
        set { m_MasterChiefGuid = value; }
    }
    //掌门名称
    private string m_MasterChiefName;
    public string MasterChiefName
    {
        get { return m_MasterChiefName; }
        set { m_MasterChiefName = value; }
    }
    //师门薪火
    private int m_MasterTorch;
    public int MasterTorch
    {
        get { return m_MasterTorch; }
        set { m_MasterTorch = value; }
    }
    //师门公告
    private string m_MasterNotice;
    public string MasterNotice
    {
        get { return m_MasterNotice; }
        set { m_MasterNotice = value; }
    }
    //创建时间
    private int m_MasterCreateTime;
    public int MasterCreateTime
    {
        get { return m_MasterCreateTime; }
        set { m_MasterCreateTime = value; }
    }
    //在线玩家数量
    private int m_MasterOnlineNum;
    public int MasterOnlineNum
    {
        get { return m_MasterOnlineNum; }
        set { m_MasterOnlineNum = value; }
    }
    //玩家数量（在线+不在线）
    private int m_MasterMemberNum;
    public int MasterMemberNum
    {
        get { return m_MasterMemberNum; }
        set { m_MasterMemberNum = value; }
    }
    //师门技能
    private Dictionary<int, string> m_MasterSkillList = null;
    public Dictionary<int, string> MasterSkillList
    {
        get { return m_MasterSkillList; }
    }
    //师门成员列表
    private Dictionary<UInt64, MasterMember> m_MasterMemberList = null;
    public Dictionary<UInt64, MasterMember> MasterMemberList
    {
        get { return m_MasterMemberList; }
    }
    //师门待审批成员列表
    private Dictionary<UInt64, MasterMember> m_MasterReserveMemberList = null;
    public Dictionary<UInt64, MasterMember> MasterReserveMemberList
    {
        get { return m_MasterReserveMemberList; }
    }
    //师门已经激活技能数量
    public int GetActiveSkillNum()
    {
        int num = 0;
        foreach (KeyValuePair<int, string> skill in MasterSkillList)
        {
            if (skill.Key > 0)
            {
                num = num + 1;
            }
        }
        return num;
    }
    public void UpdateData(GC_MASTER_RET_INFO info)
    {
        //清空之前的数据
        CleanUp();

        //判断消息包数据合法性
        if (info == null || info.MasterGuid == GlobeVar.INVALID_GUID)
        {
            return;
        }

        //填充数据
        MasterGuid = info.MasterGuid;
        MasterName = info.MasterName;
        MasterChiefGuid = info.MasterChiefGuid;
        MasterTorch = info.MasterTorch;
        MasterNotice = info.MasterNotice;
        MasterOnlineNum = info.MemberNum;
        m_MasterCreateTime = info.CreateTime;
        //技能
        if (info.Masterskill1 >= 0)
        {
            m_MasterSkillList.Add(info.Masterskill1, info.Masterskillname1);
        }
        if (info.Masterskill2 >= 0)
        {
            m_MasterSkillList.Add(info.Masterskill2, info.Masterskillname2);
        }
        if (info.Masterskill3 >= 0)
        {
            m_MasterSkillList.Add(info.Masterskill3, info.Masterskillname3);
        }
        if (info.Masterskill4 >= 0)
        {
            m_MasterSkillList.Add(info.Masterskill4, info.Masterskillname4);
        }
        if (info.Masterskill5 >= 0)
        {
            m_MasterSkillList.Add(info.Masterskill5, info.Masterskillname5);
        }
        if (info.Masterskill6 >= 0)
        {
            m_MasterSkillList.Add(info.Masterskill6, info.Masterskillname6);
        }
        //成员
        for (int i = 0; i < info.memberGuidCount; ++i)
        {
            MasterMember member = new MasterMember();
            member.CleanUp();

            member.Guid = info.GetMemberGuid(i);

            if (info.memberNameCount > i)
            {
                member.MemberName = info.GetMemberName(i);
            }
            if (info.memberVIPCount > i)
            {
                member.VIP = info.GetMemberVIP(i);
            }
            if (info.memberGuildNameCount > i)
            {
                member.GuildName = info.GetMemberGuildName(i);
            }
            if (info.memberLevelCount > i)
            {
                member.Level = info.GetMemberLevel(i);
            }
            if (info.memberCombatValueCount > i)
            {
                member.CombatValue = info.GetMemberCombatValue(i);
            }
            if (info.memberTorchCount > i)
            {
                member.Torch = info.GetMemberTorch(i);
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
            if (info.memberIsReserveCount > i)
            {
                member.IsReserve = info.GetMemberIsReserve(i);
            }

            if (member.Guid == m_MasterChiefGuid)
            {
                //掌门姓名
                m_MasterChiefName = member.MemberName;
            }

            if (member.IsValid())
            {
                if (member.IsReserveMember())
                {
                    MasterReserveMemberList.Add(member.Guid, member);
                }
                else
                {
                    MasterMemberList.Add(member.Guid, member);
                }
            }
        }
        m_MasterMemberNum = MasterMemberList.Count;

        //按照薪火排序
        SortMemberListByTorch(m_MasterMemberList);
    }

    protected Dictionary<UInt64, MasterMember> SortMemberListByTorch(Dictionary<UInt64, MasterMember> dic)
    {
        List<KeyValuePair<UInt64, MasterMember>> myList = new List<KeyValuePair<UInt64, MasterMember>>(dic);
        myList.Sort(delegate(KeyValuePair<UInt64, MasterMember> s2, KeyValuePair<UInt64, MasterMember> s1)
        {
            //在线排前面
            if (s1.Value.State == 1 && s2.Value.State != 1)
            {
                return 1;
            }
            else if (s1.Value.State != 1 && s2.Value.State == 1)
            {
                return -1;
            }
            else
            {
                //掌门排前面
                if (s1.Value.Guid == m_MasterChiefGuid)
                {
                    return 1;
                }
                else if (s2.Value.Guid == m_MasterChiefGuid)
                {
                    return -1;
                }
                else
                {
                    //战力高的排前面
                    return s1.Value.CombatValue.CompareTo(s2.Value.CombatValue);
                }
            }
        });

        dic.Clear();
        foreach (KeyValuePair<UInt64, MasterMember> pair in myList)
        {
            dic.Add(pair.Key, pair.Value);
        }
        return dic;
    }
}
