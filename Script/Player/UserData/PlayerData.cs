/********************************************************************************
 *	文件名：	PlayerData.cs
 *	全路径：	\Script\Player\UserData\PlayerData.cs
 *	创建人：	李嘉
 *	创建时间：2013-12-30
 *
 *	功能说明：主角游戏全程需要保留数据
 *	修改记录：
*********************************************************************************/

using System;
using System.Collections.Generic;
using Games.GlobeDefine;
using Games.ImpactModle;
using Games.LogicObj;
using Games.SkillModle;
using UnityEngine;
using System.Collections;
using Games.Item;
using Games.MountModule;
using Games.TitleInvestitive;
using Games.ChatHistory;
using Games.Fellow;
using Games.AwardActivity;
using Games.MoneyTree;
using Games.UserCommonData;
using Games.DailyMissionData;
using Games.DailyLuckyDraw;
using Games.SwordsMan;

public enum MONEYTYPE
{
    MONEYTYPE_INVALID = -1,
    MONEYTYPE_COIN = 0,     //金币
    MONEYTYPE_YUANBAO = 1,  //元宝
    MONEYTYPE_YUANBAO_BIND = 2,  //绑定元宝
    MONEYTYPE_TYPECOUNT,
}

public enum Consume_Type
{
    YUANBAO = 1,
    COIN = 2,
    CURRENCY = 3,
    EXP = 4,
    ITEM = 5,
}

public enum Consume_SubType
{
    YUANBAO_NORMAL = 1,
    YUANBAO_BIND = 2,
    COIN = 1,
}

//服务器标志、枚举同步
public enum SERVER_FLAGS_ENUM
{
    FLAG_START = 0,         //开始标记
    FLAG_VIP = FLAG_START,  //VIP开关
    FLAG_SNS,               //分享功能
    FLAG_ACTIVATION,        //激活界面
    FLAG_PAYACT,            //充值活动开关
    FLAG_TIMEFASHION,       //限时时装
	FLAG_MOUNTTAB = FLAG_START,          //元宝商店坐骑分页
    FLAG_FASHIONTAB,        //人物界面时装分页
    FLAG_CYFANS,            //畅游老玩家回馈奖励开关
    FLAG_WISHING,           //许愿池开关
    FLAG_LIGHTSKILL,        //轻功开关
    FLAG_DAILYLUCKYDRAW,    //每日幸运抽奖
    FLAG_NUM,               //当前使用数量
    FLAG_MAX = 30,          //最大使用数量
};

public class PlayerData
{
    public PlayerData()
    {
        CleanUp();
    }

    public void CleanUp()
    {
        m_MainPlayerBaseAttr = new BaseAttr();
        m_oBackPack = new GameItemContainer(GameItemContainer.SIZE_BACKPACK, GameItemContainer.Type.TYPE_BACKPACK);
        m_oEquipPack = new GameItemContainer(GameItemContainer.SIZE_EQUIPPACK, GameItemContainer.Type.TYPE_EQUIPPACK);
        m_oBuyBackPack = new GameItemContainer(GameItemContainer.MAXSIZE_BUYBACKPACK, GameItemContainer.Type.TYPE_BUYBACKPACK);
        m_oStoragePack = new GameItemContainer(GameItemContainer.SIZE_STORAGEPACK, GameItemContainer.Type.TYPE_STORAGEPACK);
        m_oGuildPack = new GameItemContainer(GameItemContainer.SIZE_GUILDPACK, GameItemContainer.Type.TYPE_GUILDSHOP);
        //清理玩家切场景缓存数据
        m_EnterSceneCache = new PlayerEnterSceneCache();

        //玩家组队信息
        m_TeamInfo = new Team();
        if (null != m_TeamInfo)
        {
            m_TeamInfo.CleanUp();
        }

        m_GuildInfo = new Guild();
        m_GuildList = new GuildList();

        m_objMountParam = new MountParam();
        m_objMountParam.CleanUp();
        // 玩家称号
        m_TitleInvestitive = new GameTitleInvestitive();
        m_IsLockPriorTitle = false;
        // 聊天记录
        m_ChatHistory = new GameChatHistory();
        // 最近私聊对象记录
        m_TellChatSpeakers = new LastSpeakerRecord();

        //初始化玩家关系数据
        m_FriendList = new RelationList();
        m_BlackList = new RelationList();
        m_HateList = new RelationList();

        //伙伴
        m_FellowContainer = new FellowContainer();
        m_FellowPlayerEffect = false;
        m_ActiveFellowSkill = new List<int>();
        m_FellowGainCount_Free = 0;
        m_FellowGainCount_Coin = 0;
        m_FellowGainCount_YuanBao = 0;
        m_FellowGainCD_Coin = 0;
        m_FellowTickTime = 0f;

        // 奖励活动
        m_AwardActivityData = new AwardActivityData();
        m_AwardActivityData.CleanUp();

        m_MoneyTreeData = new MoneyTreeData();  // 摇钱树
        m_MoneyTreeData.CleanUp();

        m_OwnSkillInfo = new OwnSkillData[(int)SKILLDEFINE.MAX_SKILLNUM];
        for (int i = 0; i < (int)SKILLDEFINE.MAX_SKILLNUM; i++)
        {
            m_OwnSkillInfo[i] = new OwnSkillData();
            m_OwnSkillInfo[i].CleanUp();
        }

		m_OwnAutoSkillInfo = new OwnSkillData[(int)SKILLDEFINE.MAX_SKILLNUM];
		for (int i = 0; i < (int)SKILLDEFINE.MAX_SKILLNUM; i++)
		{
			m_OwnAutoSkillInfo[i] = new OwnSkillData();
			m_OwnAutoSkillInfo[i].CleanUp();
		}
        m_nSkillPublicTime = 0;
        m_bIsCanPKLegal = false;
        m_Money = new MONEY();
        m_nProfession = -1;
        m_nVipCost = -1;
        m_ClientImpactInfo = new List<ClientImpactInfo>();

        m_CommonData = new UserCommonData(); // 玩家通用数据
        m_DailyMissionData = new DailyMissionData();    // 日常任务数据

        m_FashionDeadline = new int[GlobeVar.MAX_FASHION_SIZE];
        for (int i = 0; i < GlobeVar.MAX_FASHION_SIZE; i++)
        {
            m_FashionDeadline[i] = 0;
        }
        m_CurFashionID = GlobeVar.INVALID_ID;
        m_bShowFashion = false;
        m_ModelVisualID = GlobeVar.INVALID_ID;

        //每日幸运抽奖
        m_DailyLuckyDrawData = new DailyLuckyDrawData();

        //宝石
        m_GemData = new GemData();
        m_GemData.CleanUp();

        //师门
        m_MasterInfo = new Master();
        m_MasterPreList = new MasterPreviewList();
        m_TorchValue = 0;
        m_MasterSkillName = new Dictionary<int, string>();
        m_nPkModle = -1;
        //体能
        m_StaminaCountDown = GlobeVar.INVALID_ID;
        //隐身级别
        m_nStealthLev = 0;
        //是否开启挂机模式
        m_IsOpenAutoCombat = false;
        //最近一次打断 自动战斗的时间
        m_fBreakAutoCombatTime = 0.0f;
        //是否开始自动战斗
        m_bAutoComabat = false;
		m_bAutoXunlu = false;
        m_oSwordsManBackPack = new SwordsManContainer(SwordsManContainer.SWORDSMAN_BACKPACK_SIZE, SwordsManContainer.PACK_TYPE.TYPE_BACKPACK);
        m_oSwordsManEquipPack = new SwordsManContainer(SwordsManContainer.SWORDSMAN_EQUIPPACK_SIZE, SwordsManContainer.PACK_TYPE.TYPE_EQUIPPACK);
        m_nSwordsManScore = 0;
        m_nSwordsManVisitState = 0;
        m_nSwordsManCombat = 0;

        m_OnlineAwardTable = new Dictionary<int, OnlineAwardLine>();
        m_NewOnlineAwardTable = new Dictionary<int, OnlineAwardLine>();
        m_WarPushMessaeg = new List<GuildWarPushMessageInfo>();
        m_LoverGUID = GlobeVar.INVALID_GUID;
        //上次寄售行吆喝时间
        m_fLastConsignShareTime = 0.0f;
        m_PayActivity.CleanUp();

        m_RollNoticeList = new List<string>();

        m_bForthSkillFlag = false;

        m_MainBindChildren = new List<int>();
        m_MainBindParent = -1;
        AutoTeamState = false;

        m_MissionSortList = new List<int>();
        m_MissionGridLastPos = Vector3.zero;
        m_BossData = new GuildActivityBossData();
        m_bIsClickChargeActivitySY = true;
        m_bIsClickChargeActivityCZ = true;
        m_bIsClickChargeActivitySZ = true;

		m_bIsFirstYeXiDaYing = true;
		m_AttackCount = 0;
		m_usingskill = 0;
    }

	private int m_usingskill;//是否收到技能回信
	public int Usingskill
	{
		get { return m_usingskill; }
		set { m_usingskill = value; }
	}
	private int m_AttackCount;//普攻段數
	public int AttackCount
	{
		get { return m_AttackCount; }
		set { m_AttackCount = value; }
	}


    private OwnSkillData[] m_OwnSkillInfo;
    public OwnSkillData[] OwnSkillInfo
    {
        get { return m_OwnSkillInfo; }
        set { m_OwnSkillInfo = value; }
    }
	private OwnSkillData[] m_OwnAutoSkillInfo;
	public OwnSkillData[] OwnAutoSkillInfo
	{
		get { return m_OwnAutoSkillInfo; }
		set { m_OwnAutoSkillInfo = value; }
	}
    private int m_nSkillPublicTime; //技能公共CD 单位：毫秒
    public int SkillPublicTime
    {
        get { return m_nSkillPublicTime; }
        set { m_nSkillPublicTime = value; }
    }
    public void CleanUpOwnSkillInfo()
    {
        for (int i = 0; i < OwnSkillInfo.Length; i++)
        {
            OwnSkillInfo[i].CleanUp();
			OwnAutoSkillInfo[i].CleanUp();
        }
    }
    private bool m_bIsCanPKLegal;
    public bool IsCanPKLegal
    {
        get { return m_bIsCanPKLegal; }
        set { m_bIsCanPKLegal = value; }
    }
    private int m_nPkModle;
    public int PkModle
    {
        get { return m_nPkModle; }
        set { m_nPkModle = value; }
    }
    //是否开启挂机模式
    private bool m_IsOpenAutoCombat;
    public bool IsOpenAutoCombat
    {
        get { return m_IsOpenAutoCombat; }
        set { m_IsOpenAutoCombat = value; }
    }
    //最近一次打断 自动战斗的时间
    private float m_fBreakAutoCombatTime;
    public float BreakAutoCombatTime
    {
        get { return m_fBreakAutoCombatTime; }
        set { m_fBreakAutoCombatTime = value; }
    }
    //是否开始自动战斗
    protected bool m_bAutoComabat;
    public bool AutoComabat
    {
        get { return m_bAutoComabat; }
        set { m_bAutoComabat = value; }
    }
	//是否开始自动寻路
	protected bool m_bAutoXunlu;
	public bool AutoXunlu
	{
		get { return m_bAutoXunlu; }
		set { m_bAutoXunlu = value; }
	}
	private int m_nStealthLev;
    public int StealthLev
    {
        get { return m_nStealthLev; }
        set { m_nStealthLev = value; }
    }
    //职业
    private int m_nProfession;
    public int Profession
    {
        get { return m_nProfession; }
        set { m_nProfession = value; }
    }
    //上次寄售行吆喝时间
    private float m_fLastConsignShareTime = 0.0f;
    public float LastConsignShareTime
    {
        get { return m_fLastConsignShareTime; }
        set { m_fLastConsignShareTime = value; }
    }
    //VIP
    private int m_nVipCost;
    public int VipCost
    {
        get { return m_nVipCost; }
        set { m_nVipCost = value; }
    }
    private int m_nPoolCombatValue;
    public int PoolCombatValue
    {
        get { return m_nPoolCombatValue; }
        set { m_nPoolCombatValue = value; }
    }
    private List<ClientImpactInfo> m_ClientImpactInfo;
    public List<ClientImpactInfo> ClientImpactInfo
    {
        get { return m_ClientImpactInfo; }
        set { m_ClientImpactInfo = value; }
    }
    private int m_MainBindParent;
    public int MainBindParent
    {
        get { return m_MainBindParent; }
        set { m_MainBindParent = value; }
    }
    private List<int> m_MainBindChildren;
    public List<int> MainBindChildren
    {
        get { return m_MainBindChildren; }
        set { m_MainBindChildren = value; }
    }
    //主角基础属性
    //Obj_MainPlayer已经将自己的BaseAttr屏蔽掉，而直接从这里读取
    private BaseAttr m_MainPlayerBaseAttr;
    public BaseAttr MainPlayerBaseAttr
    {
        get { return m_MainPlayerBaseAttr; }
        set { m_MainPlayerBaseAttr = value; }
    }

    //////////////////////////////////////////////////////////////////////////
    //收到EnterScene包后的数据缓存
    //////////////////////////////////////////////////////////////////////////
    PlayerEnterSceneCache m_EnterSceneCache;
    public PlayerEnterSceneCache EnterSceneCache
    {
        get { return m_EnterSceneCache; }
        set { m_EnterSceneCache = value; }
    }

    //////////////////////////////////////////////////////////////////////////
    //背包
    //////////////////////////////////////////////////////////////////////////
    //取得背包
    public GameItemContainer GetItemContainer(GameItemContainer.Type type)
    {
        switch (type)
        {
            case GameItemContainer.Type.TYPE_BACKPACK: return m_oBackPack;
            case GameItemContainer.Type.TYPE_EQUIPPACK: return m_oEquipPack;
            case GameItemContainer.Type.TYPE_BUYBACKPACK: return m_oBuyBackPack;
            case GameItemContainer.Type.TYPE_STORAGEPACK: return m_oStoragePack;
            case GameItemContainer.Type.TYPE_GUILDSHOP: return m_oGuildPack;
        }
        return null;
    }

    private GameItemContainer m_oBackPack;
    public GameItemContainer BackPack
    {
        get { return m_oBackPack; }
        set { m_oBackPack = value; }
    }

    //////////////////////////////////////////////////////////////////////////
    //装备槽位
    //////////////////////////////////////////////////////////////////////////
    private GameItemContainer m_oEquipPack;
    public GameItemContainer EquipPack
    {
        get { return m_oEquipPack; }
        set { m_oEquipPack = value; }
    }

    //////////////////////////////////////////////////////////////////////////
    //回购槽位
    //////////////////////////////////////////////////////////////////////////
    private GameItemContainer m_oBuyBackPack;
    public GameItemContainer BuyBackPack
    {
        get { return m_oBuyBackPack; }
        set { m_oBuyBackPack = value; }
    }

    private GameItemContainer m_oStoragePack;
    public Games.Item.GameItemContainer StoragePack
    {
        get { return m_oStoragePack; }
        set { m_oStoragePack = value; }
    }

    //////////////////////////////////////////////////////////////////////////
    //帮会商店
    //////////////////////////////////////////////////////////////////////////
    private GameItemContainer m_oGuildPack;
    public GameItemContainer GuildPack
    {
        get { return m_oGuildPack; }
        set { m_oGuildPack = value; }
    }


    //////////////////////////////////////////////////////////////////////////
    //坐骑数据
    //////////////////////////////////////////////////////////////////////////
    public MountParam m_objMountParam;
    //public MountParam ObjMountParam
    //{
    //    get { return m_objMountParam; }
    //    set { m_objMountParam = value; }
    //}

    //////////////////////////////////////////////////////////////////////////
    //组队数据
    //////////////////////////////////////////////////////////////////////////
    private Team m_TeamInfo;
    public Team TeamInfo
    {
        get { return m_TeamInfo; }
        set { m_TeamInfo = value; }
    }

    public bool IsHaveTeam()
    {
        return (null != m_TeamInfo && m_TeamInfo.TeamID != GlobeVar.INVALID_ID);
    }
    //某个Guid是否为队伍成员
    public bool IsTeamMem(UInt64 guid)
    {
        if (GlobeVar.INVALID_ID == m_TeamInfo.TeamID ||
            GlobeVar.INVALID_GUID == guid)
        {
            return false;
        }
        for (int i = 0; i < GlobeVar.MAX_TEAM_MEMBER; i++)
        {
            if (null != m_TeamInfo.GetTeamMember(i) &&
                guid == m_TeamInfo.GetTeamMember(i).Guid)
            {
                return true;
            }
        }
        return false;
    }

    //////////////////////////////////////////////////////////////////////////
    //帮会数据
    //////////////////////////////////////////////////////////////////////////
    private Guild m_GuildInfo;                  //帮会数据
    public Guild GuildInfo
    {
        get { return m_GuildInfo; }
        set { m_GuildInfo = value; }
    }

    public bool IsHaveGuild()
    {
        return (null != m_GuildInfo && m_GuildInfo.GuildGuid != GlobeVar.INVALID_GUID);
    }

    public bool IsReserveGuildMember()
    {
        if (null == m_GuildInfo || m_GuildInfo.GuildGuid == GlobeVar.INVALID_GUID)
        {
            return false;
        }

        UInt64 mainPlayerGuid = Singleton<ObjManager>.GetInstance().MainPlayer.GUID;
        GuildMember member;
        if (m_GuildInfo.GuildMemberList.TryGetValue(mainPlayerGuid, out member))
        {
            if (member.Job == (int)GameDefine_Globe.GUILD_JOB.RESERVE)
            {
                return true;
            }
        }

        return false;
    }
    public bool IsGuildMember(Obj_OtherPlayer _otherPlayer)
    {
        if (_otherPlayer == null)
        {
            return false;
        }
        if (null == m_GuildInfo || m_GuildInfo.GuildGuid == GlobeVar.INVALID_GUID)
        {
            return false;
        }
        if (_otherPlayer.GUID == GlobeVar.INVALID_GUID)
        {
            return false;
        }
        GuildMember member;
        if ((m_GuildInfo.GuildMemberList.TryGetValue(_otherPlayer.GUID, out member) && m_GuildInfo.GetMemberJob(_otherPlayer.GUID) != (int)GameDefine_Globe.GUILD_JOB.RESERVE) //不是帮会审批成员
            || m_GuildInfo.GuildGuid == _otherPlayer.GuildGUID)
        {
            return true;
        }

        return false;
    }

    public bool IsGuildChief()
    {
        return (null != Singleton<ObjManager>.GetInstance().MainPlayer && m_GuildInfo.GuildChiefGuid == Singleton<ObjManager>.GetInstance().MainPlayer.GUID);
    }

    public bool IsGuildViceChief(UInt64 guid)
    {
        if (null == m_GuildInfo || m_GuildInfo.GuildGuid == GlobeVar.INVALID_GUID)
        {
            return false;
        }
        if (guid == GlobeVar.INVALID_GUID)
        {
            return false;
        }
        GuildMember member;
        if (m_GuildInfo.GuildMemberList.TryGetValue(guid, out member))
        {
            if (member.Job == (int)GameDefine_Globe.GUILD_JOB.VICE_CHIEF)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    private GuildList m_GuildList;              //帮会列表
    public GuildList guildList
    {
        get { return m_GuildList; }
        set { m_GuildList = value; }
    }
    //帮战推送消息
    private List<GuildWarPushMessageInfo> m_WarPushMessaeg;
    public List<GuildWarPushMessageInfo> WarPushMessaeg
    {
        get { return m_WarPushMessaeg; }
        set { m_WarPushMessaeg = value; }
    }
    //////////////////////////////////////////////////////////////////////////
    //称号数据
    //////////////////////////////////////////////////////////////////////////
    private GameTitleInvestitive m_TitleInvestitive;
    public GameTitleInvestitive TitleInvestitive
    {
        get { return m_TitleInvestitive; }
        set { m_TitleInvestitive = value; }
    }

    private bool m_IsLockPriorTitle;
    public bool IsLockPriorTitle
    {
        get { return m_IsLockPriorTitle; }
        set { m_IsLockPriorTitle = value; }
    }

    //////////////////////////////////////////////////////////////////////////
    //聊天记录
    //////////////////////////////////////////////////////////////////////////
    private GameChatHistory m_ChatHistory;
    public GameChatHistory ChatHistory
    {
        get { return m_ChatHistory; }
        set { m_ChatHistory = value; }
    }

    private ChatInfoLogic.CHANNEL_TYPE m_eChooseChannel = ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_WORLD;
    public ChatInfoLogic.CHANNEL_TYPE ChooseChannel
    {
        get { return m_eChooseChannel; }
        set { m_eChooseChannel = value; }
    }

    private UInt64 m_LastTellGUID = GlobeVar.INVALID_GUID;
    public UInt64 LastTellGUID
    {
        get { return m_LastTellGUID; }
        set { m_LastTellGUID = value; }
    }

    private string m_LastTellName = "";
    public string LastTellName
    {
        get { return m_LastTellName; }
        set { m_LastTellName = value; }
    }

    private LastSpeakerRecord m_TellChatSpeakers;
    public LastSpeakerRecord TellChatSpeakers
    {
        get { return m_TellChatSpeakers; }
        set { m_TellChatSpeakers = value; }
    }

    //////////////////////////////////////////////////////////////////////////
    //玩家关系
    //////////////////////////////////////////////////////////////////////////
    //玩家好友列表
    private RelationList m_FriendList;
    public RelationList FriendList
    {
        get { return m_FriendList; }
        set { m_FriendList = value; }
    }

    //玩家黑名单
    private RelationList m_BlackList;
    public RelationList BlackList
    {
        get { return m_BlackList; }
        set { m_BlackList = value; }
    }

    //仇人列表
    private RelationList m_HateList;
    public RelationList HateList
    {
        get { return m_HateList; }
        set { m_HateList = value; }
    }

    //伙伴数据
    private FellowContainer m_FellowContainer;
    public FellowContainer FellowContainer
    {
        get { return m_FellowContainer; }
        set { m_FellowContainer = value; }
    }
    //伙伴特效
    private bool m_FellowPlayerEffect;
    public bool FellowPlayerEffect
    {
        get { return m_FellowPlayerEffect; }
        set { m_FellowPlayerEffect = value; }
    }
    //已经激活的伙伴技能
    private List<int> m_ActiveFellowSkill;
    public List<int> ActiveFellowSkill
    {
        get { return m_ActiveFellowSkill; }
        set { m_ActiveFellowSkill = value; }
    }
    //当天抽取伙伴次数
    private int m_FellowGainCount_Free;
    public int FellowGainCount_Free
    {
        get { return m_FellowGainCount_Free; }
        set { m_FellowGainCount_Free = value; }
    }
    private int m_FellowGainCount_Coin;
    public int FellowGainCount_Coin
    {
        get { return m_FellowGainCount_Coin; }
        set { m_FellowGainCount_Coin = value; }
    }
    private int m_FellowGainCount_YuanBao;
    public int FellowGainCount_YuanBao
    {
        get { return m_FellowGainCount_YuanBao; }
        set { m_FellowGainCount_YuanBao = value; }
    }
    //伙伴抽取CD
    private int m_FellowGainCD_Coin;
    public int FellowGainCD_Coin
    {
        get { return m_FellowGainCD_Coin; }
        set { m_FellowGainCD_Coin = value; }
    }
    private float m_FellowTickTime;
    public void Tick_FellowGainCD()
    {
        if (FellowGainCD_Coin > 0)
        {
            m_FellowTickTime += Time.fixedDeltaTime;
            //一秒触发一次
            if (m_FellowTickTime > 1)
            {
                m_FellowTickTime = 0f;
                FellowGainCD_Coin--;
                if (PartnerFrameLogic_Gamble.Instance())
                {
                    PartnerFrameLogic_Gamble.Instance().SetGainCDTime(FellowGainCD_Coin);
                }
            }
        }
    }

	//===========
	//周卡月卡数据
	private int m_WeekDay;
	public int WeekDay
	{
		get {return m_WeekDay;}
		set {m_WeekDay = value;}
	}
	private int m_MonthDay;
	public int MonthDay
	{
		get {return m_MonthDay;}
		set {m_MonthDay = value;}
	}






	//============


    // 奖励活动数据
    private AwardActivityData m_AwardActivityData;
    public AwardActivityData AwardActivityData
    {
        get { return m_AwardActivityData; }
    }
    public void Tick_Award()
    {
        m_AwardActivityData.Tick_Award();
    }
    public void HandlePacket(GC_NEWSERVERAWARD_DATA packet)
    {
        m_AwardActivityData.HandlePacket(packet);
    }
	public void HandlePacket(GC_DAILYAWARD_DATA packet)
	{
		m_AwardActivityData.HandlePacket(packet);
	}
    public void HandlePacket(GC_DAYAWARD_DATA packet)
    {
        m_AwardActivityData.HandlePacket(packet);
    }
    public void HandlePacket(GC_ONLINEAWARD_DATA packet)
    {
        m_AwardActivityData.HandlePacket(packet);
    }
	public void HandlePacket(GC_LEVELAWARD_DATA packet)
	{
		m_AwardActivityData.HandlePacket(packet);
	}
	public void HandlePacket(GC_7DAYAWARD_DATA packet)
	{
		m_AwardActivityData.HandlePacket(packet);
	}
    public void HandlePacket(GC_ASK_ACTIVENESSAWARD_RET packet)
    {
        m_AwardActivityData.HandlePacket(packet);
    }
    public void HandlePacket(GC_SYNC_ACTIVENESSAWARD packet)
    {
        m_AwardActivityData.HandlePacket(packet);
    }
    public void HandlePacket(GC_SYNC_ACTIVENESS packet)
    {
        m_AwardActivityData.HandlePacket(packet);
    }
    public void HandlePacket(GC_NEWONLINEAWARD_DATA packet)
    {
        m_AwardActivityData.HandlePacket(packet);
    }
    public void HandlePacket(GC_SYNC_PAY_ACTIVITY_DATA packet)
    {
        m_PayActivity.HandlePacket(packet);
    }
    public void HandlePacket(GC_NEW7DAYONLINEAWARD_DATA packet)
    {
        m_AwardActivityData.HandlePacket(packet);
    }
    public void HandlePacket(GC_SYNC_NEW7DAYONLINEAWARDTABLE packet)
    {
        m_AwardActivityData.HandlePacket(packet);
    }
    // 摇钱树数据
    private MoneyTreeData m_MoneyTreeData;
    public MoneyTreeData MoneyTreeData
    {
        get { return m_MoneyTreeData; }
    }
    public void Tick_MoneyTreeAward()
    {
        m_MoneyTreeData.Tick_MoneyTreeAward();
    }
    public void HandlePacket(GC_MONEYTREE_DATA packet)
    {
        m_MoneyTreeData.HandlePacket(packet);
    }

    private int m_nReliveEntryTime = 0;//记录复活剩余秒
    public int ReliveEntryTime
    {
        get { return m_nReliveEntryTime; }
        set { m_nReliveEntryTime = value; }
    }

    // 通用存储数据
    private UserCommonData m_CommonData;
    public UserCommonData CommonData
    {
        get { return m_CommonData; }
        set { m_CommonData = value; }
    }

    // 日常任务
    private DailyMissionData m_DailyMissionData;
    public DailyMissionData DailyMissionData
    {
        get { return m_DailyMissionData; }
        set { m_DailyMissionData = value; }
    }
    //伴侣 GUID
    private UInt64 m_LoverGUID = GlobeVar.INVALID_GUID;
    public System.UInt64 LoverGUID
    {
        get { return m_LoverGUID; }
        set { m_LoverGUID = value; }
    }

    //经济系统
    public class MONEY
    {
        public Int32 GetMoney_Coin() { return GetMoneyByType(MONEYTYPE.MONEYTYPE_COIN); }
        public Int32 GetMoney_YuanBao() { return GetMoneyByType(MONEYTYPE.MONEYTYPE_YUANBAO); }
        public Int32 GetMoney_YuanBaoBind() { return GetMoneyByType(MONEYTYPE.MONEYTYPE_YUANBAO_BIND); }
        public void SetMoney(MONEYTYPE nType, int nValue)
        {
            if (nType > MONEYTYPE.MONEYTYPE_INVALID && nType < MONEYTYPE.MONEYTYPE_TYPECOUNT)
            {
                m_nMoney[(int)nType] = nValue;
            }
        }

        private Int32 GetMoneyByType(MONEYTYPE nType)
        {
            if (nType > MONEYTYPE.MONEYTYPE_INVALID && nType < MONEYTYPE.MONEYTYPE_TYPECOUNT)
            {
                return m_nMoney[(int)nType];
            }

            return 0;
        }

        private Int32[] m_nMoney;

        public MONEY()
        {
            m_nMoney = new Int32[(int)MONEYTYPE.MONEYTYPE_TYPECOUNT];
            for (int i = 0; i < (int)MONEYTYPE.MONEYTYPE_TYPECOUNT; ++i)
            {
                m_nMoney[i] = 0;
            }
        }
    }

    public MONEY m_Money;
    public MONEY Money
    {
        get { return m_Money; }
        set { m_Money = value; }
    }

    //////////////////////////////////////////////////////////////////////////
    //时装
    //////////////////////////////////////////////////////////////////////////
    private int[] m_FashionDeadline;
    public int[] FashionDeadline
    {
        get { return m_FashionDeadline; }
        set { m_FashionDeadline = value; }
    }

    private int m_CurFashionID;
    public int CurFashionID
    {
        get { return m_CurFashionID; }
        set { m_CurFashionID = value; }
    }

    private bool m_bShowFashion;
    public bool ShowFashion
    {
        get { return m_bShowFashion; }
        set { m_bShowFashion = value; }
    }

    private int m_ModelVisualID;
    public int ModelVisualID
    {
        get { return m_ModelVisualID; }
        set { m_ModelVisualID = value; }
    }

    public void ClearFashionData()
    {
        m_FashionDeadline = new int[GlobeVar.MAX_FASHION_SIZE];
        for (int i = 0; i < GlobeVar.MAX_FASHION_SIZE; i++)
        {
            m_FashionDeadline[i] = 0;
        }
        m_CurFashionID = GlobeVar.INVALID_ID;
        m_bShowFashion = false;
        m_ModelVisualID = GlobeVar.INVALID_ID;
    }

    //每日幸运抽奖数据
    private DailyLuckyDrawData m_DailyLuckyDrawData;
    public DailyLuckyDrawData DailyLuckyDrawData
    {
        get { return m_DailyLuckyDrawData; }
        set { m_DailyLuckyDrawData = value; }
    }

    //宝石信息
    private GemData m_GemData;
    public GemData GemData
    {
        get { return m_GemData; }
        set { m_GemData = value; }
    }

    //师门
    private Master m_MasterInfo;
    public Master MasterInfo
    {
        get { return m_MasterInfo; }
        set { m_MasterInfo = value; }
    }
    private MasterPreviewList m_MasterPreList;
    public MasterPreviewList MasterPreList
    {
        get { return m_MasterPreList; }
        set { m_MasterPreList = value; }
    }
    private int m_TorchValue;
    public int TorchValue
    {
        get { return m_TorchValue; }
        set { m_TorchValue = value; }
    }
    private Dictionary<int, string> m_MasterSkillName;
    public Dictionary<int, string> MasterSkillName
    {
        get { return m_MasterSkillName; }
        set { m_MasterSkillName = value; }
    }
    //是否有师门
    public bool IsHaveMaster()
    {
        if (m_MasterInfo != null)
        {
            return (m_MasterInfo.MasterGuid != GlobeVar.INVALID_GUID);
        }
        return false;
    }
    //是否掌门
    public bool IsMasterChief()
    {
        if (m_MasterInfo != null && Singleton<ObjManager>.GetInstance().MainPlayer != null)
        {
            return (m_MasterInfo.MasterChiefGuid == Singleton<ObjManager>.GetInstance().MainPlayer.GUID);
        }
        return false;
    }
    //是否待审批师门成员
    public bool IsMasterReserveMember()
    {
		if (m_MasterInfo != null &&
		    m_MasterInfo.MasterReserveMemberList.Count != 0
            )
        {
			if(m_MasterInfo.MasterReserveMemberList.ContainsKey(Singleton<ObjManager>.GetInstance().MainPlayer.GUID) == true)
            	return true;
			else
				return false;
        }
        return false;
    }
    //取得师门技能名称
    public string GetMasterSkillName(int skillId)
    {
        if (skillId >= 0)
        {
            string skillname;
            if (MasterSkillName.TryGetValue(skillId, out skillname))
            {
                return skillname;
            }
        }
        return "";
    }
    //师门激活技能数量
    public int GetMasterSkillActiveNum()
    {
        return m_MasterInfo.GetActiveSkillNum();
    }

    //体能
    private int m_StaminaCountDown;
    public int StaminaCountDown
    {
        get { return m_StaminaCountDown; }
        set { m_StaminaCountDown = value; }
    }

    // 滚动公告
    private List<string> m_RollNoticeList;
    public List<string> RollNoticeList
    {
        get { return m_RollNoticeList; }
        set { m_RollNoticeList = value; }
    }
    //////////////////////////////////////////////////////////////////////////
    //侠客
    //////////////////////////////////////////////////////////////////////////
    //取得侠客


    private SwordsManContainer m_oSwordsManBackPack;
    public SwordsManContainer SwordsManBackPack
    {
        get { return m_oSwordsManBackPack; }
        set { m_oSwordsManBackPack = value; }
    }

    private SwordsManContainer m_oSwordsManEquipPack;
    public SwordsManContainer SwordsManEquipPack
    {
        get { return m_oSwordsManEquipPack; }
        set { m_oSwordsManEquipPack = value; }
    }

    public SwordsManContainer GetSwordsManContainer(SwordsManContainer.PACK_TYPE type)
    {
        switch (type)
        {
            case SwordsManContainer.PACK_TYPE.TYPE_BACKPACK: return m_oSwordsManBackPack;
            case SwordsManContainer.PACK_TYPE.TYPE_EQUIPPACK: return m_oSwordsManEquipPack;
        }
        return null;
    }

    private int m_nSwordsManVisitState;
    public int SwordsManVisitState
    {
        get { return m_nSwordsManVisitState; }
        set { m_nSwordsManVisitState = value; }
    }

    private int m_nSwordsManScore;
    public int SwordsManScore
    {
        get { return m_nSwordsManScore; }
        set { m_nSwordsManScore = value; }
    }

    private int m_nSwordsManCombat;
    public int SwordsManCombat
    {
        set { m_nSwordsManCombat = value; }
        get { return m_nSwordsManCombat; }
    }

    private int m_nReputation;
    public int Reputation
    {
        get { return m_nReputation; }
        set { m_nReputation = value; }
    }

    // 在线奖励 表格数据--用于动态加载    
    private Dictionary<int, OnlineAwardLine> m_OnlineAwardTable;
    public Dictionary<int, OnlineAwardLine> OnlineAwardTable
    {
        get { return m_OnlineAwardTable; }
        set { m_OnlineAwardTable = value; }
    }
    public void AddOnlineAwardLine(OnlineAwardLine DataLine)
    {
        if (m_OnlineAwardTable.ContainsKey(DataLine.ID))
        {
            m_OnlineAwardTable[DataLine.ID] = DataLine;
        }
        else
        {
            m_OnlineAwardTable.Add(DataLine.ID, DataLine);
        }
    }
    // 开服奖励 表格数据--用于动态加载
    private bool m_IsShouNowOnlineAwardWindow = false;
    public bool ShouNowOnlineAwardWindow
    {
        get { return m_IsShouNowOnlineAwardWindow; }
        set { m_IsShouNowOnlineAwardWindow = value; }
    }
    private Dictionary<int, OnlineAwardLine> m_NewOnlineAwardTable;
    public Dictionary<int, OnlineAwardLine> NewOnlineAwardTable
    {
        get { return m_NewOnlineAwardTable; }
        set { m_NewOnlineAwardTable = value; }
    }
    public void AddNewOnlineAwardLine(OnlineAwardLine DataLine)
    {
        if (m_NewOnlineAwardTable.ContainsKey(DataLine.ID))
        {
            m_NewOnlineAwardTable[DataLine.ID] = DataLine;
        }
        else
        {
            m_NewOnlineAwardTable.Add(DataLine.ID, DataLine);
        }
    }
    public NewOnlineDateTime m_sNewOnlineDateTime;
    private float m_fHpItemCDTime = 0; //记录血CD时间 PlayerFrameLogic.cs
    public float HpItemCDTime
    {
        get { return m_fHpItemCDTime; }
        set { m_fHpItemCDTime = value; GameViewModel.Get<MainPlayerViewModel>().HpItemCDTime.Value = value; }
    }
    private float m_fMpItemCDTime = 0; //记录蓝CD时间 PlayerFrameLogic.cs
    public float MpItemCDTime
    {
        get { return m_fMpItemCDTime; }
        set { m_fMpItemCDTime = value; GameViewModel.Get<MainPlayerViewModel>().MpItemCDTime.Value = value; }
    }
    private bool m_bStartSweep = false;
    public bool StartSweep
    {
        get { return m_bStartSweep; }
        set { m_bStartSweep = value; }
    }
    private int m_nCangJingGeTier = 0;
    public int CangJIngGeTier
    {
        get { return m_nCangJingGeTier; }
        set { m_nCangJingGeTier = value; }
    }
    private float m_CangJIngGeSecond = Time.realtimeSinceStartup;
    public float CangJIngGeSecond
    {
        get { return m_CangJIngGeSecond; }
        set { m_CangJIngGeSecond = value; }
    }
    private int m_nCJGSweepCDTime = 0;//记录藏经阁扫荡剩余秒
    public int CJGSweepCDTime
    {
        get { return m_nCJGSweepCDTime; }
        set { m_nCJGSweepCDTime = value; }
    }
    private float m_fCopySceneChange = 0;//判断副本是否发送过传送信息.
    public bool CopySceneChange
    {
        get
        {
            if (Time.realtimeSinceStartup - m_fCopySceneChange > 10.0f)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        set
        {
            if (value)
            {
                m_fCopySceneChange = Time.realtimeSinceStartup;
            }
            else
            {
                m_fCopySceneChange = 0;
            }
        }
    }
    //充值活动
    private PayActivityData m_PayActivity;
    public PayActivityData PayActivity
    {
        get { return m_PayActivity; }
        set { m_PayActivity = value; }
    }
    private bool m_bAutoTeamState = false;//记录自动组队状态
    public bool AutoTeamState
    {
        get { return m_bAutoTeamState; }
        set { m_bAutoTeamState = value; }
    }
    private int m_bAutoTeamCopySceneId = -1;//记录自动组队状态 场景ID
    public int AutoTeamCopySceneId
    {
        get { return m_bAutoTeamCopySceneId; }
        set { m_bAutoTeamCopySceneId = value; }
    }
    private int m_bAutoTeamCopySceneDifficult = 1;//记录自动组队状态  难度
    public int AutoTeamCopySceneDifficult
    {
        get { return m_bAutoTeamCopySceneDifficult; }
        set { m_bAutoTeamCopySceneDifficult = value; }
    }
    // 4技能新手指引，切场景需要 加个标记
    private bool m_bForthSkillFlag = false;
    public bool ForthSkillFlag
    {
        get { return m_bForthSkillFlag; }
        set { m_bForthSkillFlag = value; }
    }

    private bool m_bOpenChongZhiRank = false;
    public bool OpenChongZhiRank
    {
        get { return m_bOpenChongZhiRank; }
        set { m_bOpenChongZhiRank = value; }
    }

    private int m_nChongZhiStartTime = 0;
    public int ChongZhiStartTime
    {
        get { return m_nChongZhiStartTime; }
        set { m_nChongZhiStartTime = value; }
    }

    private int m_nChongZhiEndTime = 0;
    public int ChongZhiEndTime
    {
        get { return m_nChongZhiEndTime; }
        set { m_nChongZhiEndTime = value; }
    }

    // 策划要求 任务追踪界面排序
    private List<int> m_MissionSortList;
    public List<int> MissionSortList
    {
        get { return m_MissionSortList; }
        set { m_MissionSortList = value; }
    }
    private Vector3 m_MissionGridLastPos; // 任务追踪 滑动位置记录
    public UnityEngine.Vector3 MissionGridLastPos
    {
        get { return m_MissionGridLastPos; }
        set { m_MissionGridLastPos = value; }
    }
    //服务器标记
    private int m_ServerFlags;
    public void HandlerServerFlags(GC_SERVERFLAGS packet)
    {
        m_ServerFlags = packet.Flags;
        if (RechargeBarLogic.Instance() != null)
        {
            RechargeBarLogic.Instance().UpdateChargeActivity();
        }
        if (FunctionButtonLogic.Instance() != null)
        {
            FunctionButtonLogic.Instance().UpdateDailyLuckyButton();
        }

    }
    //public bool IsVipOpen()
    //{
    //    if ((m_ServerFlags & (1 << (int)SERVER_FLAGS_ENUM.FLAG_VIP)) == 0)
    //    {
    //        return false;
    //    }
    //    return true;
    //}

    //public bool IsChargeActivityOpen()
    //{
    //    if ((m_ServerFlags & (1 << (int)SERVER_FLAGS_ENUM.FLAG_PAYACT)) == 0)
    //    {
    //        return false;
    //    }
    //    return true;
    //}

    //服务器控制的客户端功能开关是否开启
    public bool IsServerFlagOpen(SERVER_FLAGS_ENUM eFlag)
    {
        if (eFlag >= SERVER_FLAGS_ENUM.FLAG_START && eFlag < SERVER_FLAGS_ENUM.FLAG_NUM)
        {
            if ((m_ServerFlags & (1 << (int)eFlag)) == 0)
            {
                return false;
            }

            return true;
        }

        return false;
    }

    // 帮会日常活动 Boss 坐标点
     private GuildActivityBossData m_BossData;
     public GuildActivityBossData BossData
     {
         get { return m_BossData; }
     }
     public void HandleGuildActivityBossData(GC_GUILDACTIVITY_BOSSDATA packet)
     {
         m_BossData.SetBossData(packet.Sceneclassid, packet.Sceneinstid, packet.PosX/100.0f, packet.PosZ/100.0f);
     }

     //每日首次登陆触发
     public void OnTodayFirstLogin()
     {
         //首充界面提醒
         GameManager.gameManager.PlayerDataPool.IsClickChargeActivitySY = false;
         GameManager.gameManager.PlayerDataPool.IsClickChargeActivityCZ = false;

         if (VipData.GetVipLv() == 0 && GameManager.gameManager.PlayerDataPool.IsServerFlagOpen(SERVER_FLAGS_ENUM.FLAG_VIP) )
         {
             GameManager.gameManager.PlayerDataPool.IsShowVipTip = true;
         }
         else
         {
             GameManager.gameManager.PlayerDataPool.IsShowVipTip = false;
         }
     }

     //招财进宝
     private bool m_bIsClickChargeActivitySY = true;
     public bool IsClickChargeActivitySY
     {
         get { return m_bIsClickChargeActivitySY; }
         set { m_bIsClickChargeActivitySY = value; }
     }
     //成长基金
     private bool m_bIsClickChargeActivityCZ = true;
     public bool IsClickChargeActivityCZ
     {
         get { return m_bIsClickChargeActivityCZ; }
         set { m_bIsClickChargeActivityCZ = value; }
     }
     //首周礼包
     private bool m_bIsClickChargeActivitySZ = true;
     public bool IsClickChargeActivitySZ
     {
         get { return m_bIsClickChargeActivitySZ; }
         set { m_bIsClickChargeActivitySZ = value; }
     }
     private bool m_bIsShowVipTip = false;
     public bool IsShowVipTip
     {
         get { return m_bIsShowVipTip; }
         set { m_bIsShowVipTip = value; }
     }
	//是否第一次进入夜袭大营
	private bool m_bIsFirstYeXiDaYing = false;
	public bool IsFirstYeXiDaYing
	{
		get { return m_bIsFirstYeXiDaYing; }
		set { m_bIsFirstYeXiDaYing = value; }
	}
}
