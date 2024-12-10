using System;
using Games.GlobeDefine;
using UnityEngine;
using System.Collections;

public struct GuildWarPreliminaryRank
{
    //public GuildWarPreliminaryRank()
    //{
    //    CleanUp();
    //}
    public bool IsVaild()
    {
        return (m_nSortNum != -1);
    }

    public void CleanUp()
    {
        m_nSortNum = -1;
        m_nScore = 0;
        m_GuildName = "";
    }
    private int m_nSortNum;
    public int SortNum
    {
        get { return m_nSortNum; }
        set { m_nSortNum = value; }
    }
    private int m_nScore;
    public int Score
    {
        get { return m_nScore; }
        set { m_nScore = value; }
    }
    private string m_GuildName;
    public string GuildName
    {
        get { return m_GuildName; }
        set { m_GuildName = value; }
    }
}
public struct GuildWarKillRank
{
    //public GuildWarKillRank()
    //{
    //    CleanUp();
    //}
    public bool IsVaild()
    {
        return (m_nSortNum != -1);
    }
    public void CleanUp()
    {
        m_nSortNum = -1;
        m_nKillerNum = 0;
        m_KillerName = "";
    }
    private int m_nSortNum;
    public int SortNum
    {
        get { return m_nSortNum; }
        set { m_nSortNum = value; }
    }
    private int m_nKillerNum;
    public int KillerNum
    {
        get { return m_nKillerNum; }
        set { m_nKillerNum = value; }
    }
   
    private string m_KillerName;
    public string KillerName
    {
        get { return m_KillerName; }
        set { m_KillerName = value; }
    }
}
//帮战分组信息
public struct GuildWarGroupInfo
{
    public bool IsVaild()
    {
        return (m_nGroupIndex != -1);
    }
    public void CleanUp()
    {
        m_nGroupIndex = -1;
        m_GuildAName = "";
        m_GuildBName = "";
        m_nGuildAScore = 0;
        m_nGuildBScore = 0;
        m_nWinType = -1;
    }
    private int 	m_nGroupIndex;//第几组
    public int GroupIndex
    {
        get { return m_nGroupIndex; }
        set { m_nGroupIndex = value; }
    }
    private string m_GuildAName;//A帮名字
    public string GuildAName
    {
        get { return m_GuildAName; }
        set { m_GuildAName = value; }
    }
    private string m_GuildBName;//B帮名字
    public string GuildBName
    {
        get { return m_GuildBName; }
        set { m_GuildBName = value; }
    }
    private int m_nGuildAScore;//A帮当前积分
    public int GuildAScore
    {
        get { return m_nGuildAScore; }
        set { m_nGuildAScore = value; }
    }
    private int m_nGuildBScore;//B帮当前积分
    public int GuildBScore
    {
        get { return m_nGuildBScore; }
        set { m_nGuildBScore = value; }
    }

    private int m_nWinType;
    public int WinType
    {
        get { return m_nWinType; }
        set { m_nWinType = value; }
    }
}
//帮战据点信息
public struct GuildWarPointInfo
{
    public bool IsVaild()
    {
        return (m_nPointType != -1);
    }
    public void CleanUp()
    {
        m_nPointType = -1;
        m_nPointScore = 0;
        m_PointOwnGuildGuid =GlobeVar.INVALID_GUID;
        m_bIsFighting = false;
        m_nMyGuildScore = 0;
        m_nFightGuildScore = 0;
    }
    private  int m_nPointType ;//据点类型
    public int PointType
    {
        get { return m_nPointType; }
        set { m_nPointType = value; }
    }
    private  int m_nPointScore;//据点分值
    public int PointScore
    {
        get { return m_nPointScore; }
        set { m_nPointScore = value; }
    }
    private UInt64 m_PointOwnGuildGuid;//占领帮会GUID
    public System.UInt64 PointOwnGuildGuid
    {
        get { return m_PointOwnGuildGuid; }
        set { m_PointOwnGuildGuid = value; }
    }
    private bool m_bIsFighting;//是否正在抢夺中
    public bool IsFighting
    {
        get { return m_bIsFighting; }
        set { m_bIsFighting = value; }
    }
    
    private int m_nMyGuildScore;//本帮当前积分
    public int MyGuildScore
    {
        get { return m_nMyGuildScore; }
        set { m_nMyGuildScore = value; }
    }
    private int m_nFightGuildScore;//对方帮会当前积分
    public int FightGuildScore
    {
        get { return m_nFightGuildScore; }
        set { m_nFightGuildScore = value; }
    }
}

public struct GuildWarPushMessageInfo
{
    public void CleanUp()
    {
        m_nMessageType =-1;
        m_nWarType =-1;
        m_nPointType = -1;
        m_ChallengeGuildName ="";
        m_fPushTime = 0.0f;
        m_ChallengeGuildGUID = GlobeVar.INVALID_GUID;
    }

    public bool IsVaild()
    {
        return (m_nMessageType != -1);
    }

    private int m_nMessageType;
    public int MessageType
    {
        get { return m_nMessageType; }
        set { m_nMessageType = value; }
    }
    private int m_nWarType;
    public int WarType
    {
        get { return m_nWarType; }
        set { m_nWarType = value; }
    }
    private int m_nPointType;
    public int PointType
    {
        get { return m_nPointType; }
        set { m_nPointType = value; }
    }
    private string m_ChallengeGuildName;
    public string ChallengeGuildName
    {
        get { return m_ChallengeGuildName; }
        set { m_ChallengeGuildName = value; }
    }

    private float m_fPushTime;
    public float PushTime
    {
        get { return m_fPushTime; }
        set { m_fPushTime = value; }
    }

    private UInt64 m_ChallengeGuildGUID;
    public System.UInt64 ChallengeGuildGUID
    {
        get { return m_ChallengeGuildGUID; }
        set { m_ChallengeGuildGUID = value; }
    }
}

public enum GUILDWARPOINTTYPE
{
    TIANSHU =0,//天枢
    TIANXUAN,//天璇
    TIANJI,//天玑
    TIANQUAN,//天权
    YUHENG,//玉衡
    KAIYANG,//开阳
    YAOGUANG,//摇光
    MAXPOINTNUM,
}

public enum GUILDWARTYPE
{
    INVALID = -1,
    PRELIMINARY =0,//预赛
    FINALS,//决赛
    CHALLENGE,//约战
}