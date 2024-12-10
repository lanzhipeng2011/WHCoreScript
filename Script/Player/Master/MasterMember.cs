/********************************************************************
	filename:	MasterMember.cs
	date:		2014-5-7  16-37
	author:		tangyi
	purpose:	师门成员数据结构
 *  
 *  modify log:   2014-5-28 Lijia: 客户端效率优化，把MasterMember从class改为struct
*********************************************************************/
using System;
using Games.GlobeDefine;

public struct MasterMember
{
    //public MasterMember()
    //{
    //    CleanUp();
    //}

    public void CleanUp()
    {
        m_Guid = GlobeVar.INVALID_GUID;
        m_MemberName = "";
        m_GuildName = "";
        m_nVIP = 0;
        m_nLevel = 0;
        m_nCombatValue = 0;
        m_nTorch = 0;
        m_nLastLogin = 0;
        m_nProfession = 0;
        m_nState = 0;
        m_nIsReserve = 0;
    }

    public bool IsValid()
    {
        return (m_Guid != GlobeVar.INVALID_GUID);
    }
    //是否待审批成员
    public bool IsReserveMember()
    {
        return (m_nIsReserve == 1);
    }

    //GUID
    private UInt64 m_Guid;               
    public UInt64 Guid
    {
        get { return m_Guid; }
        set { m_Guid = value; }
    }
    //名字
    private string m_MemberName;
    public string MemberName
    {
        get { return m_MemberName; }
        set { m_MemberName = value; }
    }
    //所在帮会名称
    private string m_GuildName;
    public string GuildName
    {
        get { return m_GuildName; }
        set { m_GuildName = value; }
    }
    //VIP等级
    private int m_nVIP;                 
    public int VIP
    {
        get { return m_nVIP; }
        set { m_nVIP = value; }
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
    private int m_nCombatValue;
    public int CombatValue
    {
        get { return m_nCombatValue; }
        set { m_nCombatValue = value; }
    }
    //薪火
    private int m_nTorch;
    public int Torch
    {
        get { return m_nTorch; }
        set { m_nTorch = value; }
    }
    //最后登录时间
    private int m_nLastLogin;           
    public int LastLogin
    {
        get { return m_nLastLogin; }
        set { m_nLastLogin = value; }
    }
    //会员状态
    private int m_nState;               
    public int State
    {
        get { return m_nState; }
        set { m_nState = value; }
    }
    //是否待审批
    private int m_nIsReserve;
    public int IsReserve
    {
        get { return m_nIsReserve; }
        set { m_nIsReserve = value; }
    }
}
