/********************************************************************************
 *	文件名：	GuildMember.cs
 *	全路径：	\Script\Player\Guild\GuildMember.cs
 *	创建人：	李嘉
 *	创建时间：2014-04-22
 *
 *	功能说明：帮会队员基础数据
 *	修改记录：
 *  2014-5-28 Lijia: 客户端效率优化，把GuildMember从class改为struct
*********************************************************************************/
using System;
using Games.GlobeDefine;

public class GuildMember
{
    public GuildMember()
    {
        CleanUp();
    }

    public void CleanUp()
    {
        m_Guid = GlobeVar.INVALID_GUID;
        m_MemberName = "";
        m_nVIP = 0;
        m_nJob = GlobeVar.INVALID_ID;
        m_nLevel = 0;
        m_nLastLogin = 0;
        m_nProfession = 0;
        m_nState = 0;
        m_nComBatVal = 0;
    }

    public bool IsValid()
    {
        return (m_Guid != GlobeVar.INVALID_GUID);
    }

    private UInt64 m_Guid;               //GUID
    public UInt64 Guid
    {
        get { return m_Guid; }
        set { m_Guid = value; }
    }

    private string m_MemberName;        //名字
    public string MemberName
    {
        get { return m_MemberName; }
        set { m_MemberName = value; }
    }

    private int m_nVIP;                 //VIP等级
    public int VIP
    {
        get { return m_nVIP; }
        set { m_nVIP = value; }
    }

    private int m_nLevel;               //等级
    public int Level
    {
        get { return m_nLevel; }
        set { m_nLevel = value; }
    }

    private int m_nProfession;          //职业
    public int Profession
    {
        get { return m_nProfession; }
        set { m_nProfession = value; }
    }

    private int m_nContribute;          //贡献度
    public int Contribute
    {
        get { return m_nContribute; }
        set { m_nContribute = value; }
    }

    private int m_nJob;                 //职位
    public int Job
    {
        get { return m_nJob; }
        set { m_nJob = value; }
    }

    private int m_nLastLogin;           //最后登录时间
    public int LastLogin
    {
        get { return m_nLastLogin; }
        set { m_nLastLogin = value; }
    }

    private int m_nState;               //会员状态
    public int State
    {
        get { return m_nState; }
        set { m_nState = value; }
    }
    private int m_nComBatVal;               //战力
    public int ComBatVal
    {
        get { return m_nComBatVal; }
        set { m_nComBatVal = value; }
    }
}
