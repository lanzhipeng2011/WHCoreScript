using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
using System.Collections.Generic;
using Games.LogicObj;

public class PVPData
{
	public struct OpponentInfo
	{
		public OpponentInfo(ulong _guid, string _name, CharacterDefine.PROFESSION _profession, int _level,
            int _power, int _range, int _winSpirit, int _loseSpirit, int _winReputation, int _loseReputation)
		{
			guid 		= _guid;
			name 		= _name;
			profession 	= _profession;
			level 		= _level;
			fightPower 	= _power;
			range 		= _range;
            winSpirit   = _winSpirit;
            loseSpirit  = _loseSpirit;
            winReputation = _winReputation;
            loseReputation = _loseReputation;
		}

		public bool IsValid( )
		{
			return (guid != Games.GlobeDefine.GlobeVar.INVALID_GUID);
		}

		public void Clear()
		{
			guid = Games.GlobeDefine.GlobeVar.INVALID_GUID;
		}

		public ulong 						guid;
		public string 						name;
		public CharacterDefine.PROFESSION 	profession;
		public int 							level;
        public int 							fightPower;
		public int 							winSpirit;
        public int 							loseSpirit;
        public int                          winReputation;
        public int                          loseReputation;
		public int 							range;
	}

	public static int MyPVPRange 		{set;get;}
	public static int LeftFightTime 	{set;get;}
	public static int Power 			{set;get;} // zhenqi

	public static Dictionary<ulong, OpponentInfo> OpponentMap = new Dictionary<ulong, OpponentInfo>();

    public delegate void UpdateOpponentDelegate();
    public static UpdateOpponentDelegate delUpdateOpponent;
    public static int NeedCostYuanBao { set; get; }
	public static void UpdateOpponentInfo(GC_OPPONENT_LIST data)
    {
        OpponentMap.Clear();
        if (data.HasNeedCost)
        {
            NeedCostYuanBao = data.NeedCost;
        }
        else
        {
            NeedCostYuanBao = 0;
        }
        for (int i = 0; i < data.userGuidCount; i++)
        {

			OpponentMap.Add((ulong)i, 
                new OpponentInfo(data.GetUserGuid(i), data.GetName(i), 
                    (CharacterDefine.PROFESSION)data.GetProfession(i), 
                    data.GetLevel(i), data.GetCombatNum(i), 
                    data.GetRankPos(i), 
                    data.GetWinSpirit(i), data.GetLoseSpirit(i), data.GetWinReputataion(i), data.GetLoseReputataion(i)));
        }

        if (null != delUpdateOpponent) delUpdateOpponent();
	}

    public delegate void UpdateMyDataDelegate();
    public static UpdateMyDataDelegate delUpdateMyData;
    public static void UpdateMyData(GC_CHALLENGE_MYDATA data)
    {
        MyPVPRange = data.RankPos;
        Power = data.SpiritVal;
        LeftFightTime = data.ChallengeTimes;
        if (null != delUpdateMyData) delUpdateMyData();
        if (FunctionButtonLogic.Instance())
        {
            FunctionButtonLogic.Instance().UpdateActionButtonTip();
        }
		if (SkillRootLogic.Instance())
		{
			SkillRootLogic.Instance().UpdateSkillInfo();
		}
	
    }

    //-----------------------------------------------------------------------------------
    public struct HistroyData
    {
        public HistroyData(ulong _guid, int _pos, uint _occurTime, int _lose, int _active,string name )
        {
            guid = _guid;
            rankPos = _pos;
            occurTime = _occurTime;
            isLose = _lose;
            isActive = _active;
            opponentName = name;
        }

        public ulong            guid;
        public int              rankPos;
        public uint             occurTime;
        public int              isLose;
        public int              isActive;
        public string           opponentName;
    }
    public static List<HistroyData> ChallengeHistory = new List<HistroyData>();
    public delegate void UpdatePvPRecordListDelegate();
    public static UpdatePvPRecordListDelegate delegateUpdatePvPRecordList;
    public static void UpdateChallengeHistory(GC_CHALLENGE_HISTORY data)
    {
        ChallengeHistory.Clear();
        for (int i = 0; i < data.userGuidCount; i++)
        {
            ChallengeHistory.Add(new HistroyData(data.GetUserGuid(i), data.GetRankPos(i), data.GetOccurTime(i), data.GetIsLose(i), data.GetIsActive(i), data.GetName(i)));
        }
        if (delegateUpdatePvPRecordList != null)
            delegateUpdatePvPRecordList();
    }

    //------------------------------------------------------
    public static int ChallengeIsLose { set; get; }
    public static int ChallengeSpirit { set; get; }
    public static int ChallengeReputation { set; get; }
    public static bool ChallengeAutoShow { set; get; }
    public static void UpdateChallengeReward(GC_CHALLENGE_REWARD data)
    {
        Obj_MainPlayer mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (mainPlayer != null && data.IsLose > 0)
        {
            int nPlayerMaxHP = mainPlayer.BaseAttr.MaxHP;
            if (PlayerFrameLogic.Instance() != null)
            {
                PlayerFrameLogic.Instance().ChangeHP(0, nPlayerMaxHP);
            }
        }

       // TT7092 
       // if (data.IsLose != 1)
        ChallengeAutoShow = false;
        if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_WUDAOZHIDIAN)
        {
            if (false == GameManager.gameManager.MissionManager.IsHaveMission(38))
            {
                ChallengeAutoShow = true;
            }
        }
 
        ChallengeIsLose = data.IsLose;
        ChallengeSpirit = data.SpiritVal;
        ChallengeReputation = 0;
        if (data.HasReputationVal)
        {
            ChallengeReputation = data.ReputationVal;
        }


        ChallengeRewardLogic.ShowRewardUI( 0 );
    }

    public static void CheckAutoShowChallengeUI()
    {
        if (PVPData.ChallengeAutoShow == true)
        {
            PVPData.ChallengeAutoShow = false;
            if (null != ActivityController.Instance())
            {
                ActivityController.Instance().ChangeToPvP();
            }
            else
            {
                UIManager.ShowUI(UIInfo.Activity, OnActiveShowActiviController);
            }
        }
    }

    static void OnActiveShowActiviController(bool bSuccess, object param)
    {
        ActivityController.Instance().ChangeToPvP();
    }

    //------------------------------------------------------
#region PvP排行榜
    public struct PvPRankListItemInfo
    {
        public PvPRankListItemInfo(ulong _id, string _name, int _pos, int _com, int _level, int _profession, int _zhenqi)
        {
            id = _id;
            name = _name;
            pos = _pos;
            com = _com;
            level = _level;
            profession = _profession;
            zhenqi = _zhenqi;
			score = 0;
        }
	
        public ulong id;
        public string name;
        public int score;
        public int pos;
        public int com;
        public int level;
        public int profession;
        public int zhenqi;
    }

    public static int PvPRankCurPage { set; get; }
    public static int PvPRankTotalPage { set; get; }
    public static List<PVPData.PvPRankListItemInfo> PvPRankList = new List<PVPData.PvPRankListItemInfo>();
    public delegate void UpdatePvPRankListDelegate();
    public static UpdatePvPRankListDelegate delegateUpdatePvPRankList;
    public static void UpdatePvPRankList(GC_CHALLENGERANKLIST packet)
    {
        PvPRankList.Clear();
        PvPRankCurPage = packet.Page;
        PvPRankTotalPage = packet.Totalpage;
        for (int i = 0; i < packet.userGuidCount; i++)
        {
            PvPRankList.Add(new PvPRankListItemInfo(
                packet.GetUserGuid(i), packet.GetName(i), packet.GetPos(i),
                packet.GetCombatNum(i), packet.GetLevel(i), packet.GetProfession(i), packet.GetZhenqi(i)));
        }
        if( delegateUpdatePvPRankList != null) delegateUpdatePvPRankList();
    }
#endregion

    public struct RankDataItem
    {
        public RankDataItem(string s1 = "", string s2 = "", string s3 = "", string s4 = "", string s5 = "", string s6 = "" )
        {
            str1 = s1;
            str2 = s2;
            str3 = s3;
            str4 = s4;
            str5 = s5;
            str6 = s6;
        }

        public string str1;
        public string str2;
        public string str3;
        public string str4;
        public string str5;
        public string str6;
    }

    public static List<PVPData.RankDataItem> RankDataList = new List<PVPData.RankDataItem>();
    public static string meRank { set; get; }
    public static int RankTotalPage { set; get; }
    public static int RankCurPage { set; get; }
    public static int RankType { set; get; }
    public static bool RankIsPage { set; get; }

    public static void ClearRankData()
    {
        RankDataList.Clear();
        meRank = "";
        RankTotalPage = 1;
        RankCurPage = 0;
        RankIsPage = false;
        RankType = (int)GameDefine_Globe.RANKTYPE.TYPE_CANGJINGGE;
    }

    public static void AddRankDataItem(string s1 = "", string s2 = "", string s3 = "", string s4 = "", string s5 = "", string s6 = "")
    {
        PVPData.RankDataList.Add(new RankDataItem(s1, s2, s3, s4, s5, s6));
    }

    public static void ShowRank()
    {
        UIManager.ShowUI(UIInfo.RankRoot, OnOpenRankWindow);
    }

    static void OnOpenRankWindow(bool bSuccess, object param)
    {
        if (RankWindow.Instance() != null)
        {
            switch(RankType)
            {
                case (int)GameDefine_Globe.RANKTYPE.TYPE_USERLEVELRANK: //等级
                    RankWindow.Instance().isRankDataReturn = true;
                    RankWindow.Instance().m_TabController.ChangeTab("Tab1");
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_USERCOMBATRANK://战斗力
                    RankWindow.Instance().isRankDataReturn = true;
                    RankWindow.Instance().m_TabController.ChangeTab("Tab2");
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_ATTACK://攻击
                    RankWindow.Instance().isRankDataReturn = true;
                    RankWindow.Instance().m_TabController.ChangeTab("Tab3");
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_USERHPRANK://血量
                    RankWindow.Instance().isRankDataReturn = true;
                    RankWindow.Instance().m_TabController.ChangeTab("Tab4");
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_USERCOIN: //金币
                    RankWindow.Instance().isRankDataReturn = true;
                    RankWindow.Instance().m_TabController.ChangeTab("Tab5");
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_CHONGZHI://充值
                    RankWindow.Instance().isRankDataReturn = true;
                    RankWindow.Instance().m_TabController.ChangeTab("Tab6");
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_USERCOSTYUANBAO://消费
                    RankWindow.Instance().isRankDataReturn = true;
                    RankWindow.Instance().m_TabController.ChangeTab("Tab7");
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_HUASHANPOS://华山-排名
                    RankWindow.Instance().isRankDataReturn = true;
                    RankWindow.Instance().m_TabController.ChangeTab("Tab8");
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_HUASHANJINYAODAI://金腰带
                    RankWindow.Instance().isRankDataReturn = true;
                    RankWindow.Instance().m_TabController.ChangeTab("Tab9");
                    //RankWindow.Instance().m_ProfessionReputationController.ChangeTab("Tab1");
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_HUASHANZHANJI://华山战绩
                    RankWindow.Instance().isRankDataReturn = true;
                    RankWindow.Instance().m_TabController.ChangeTab("Tab10");
                    //RankWindow.Instance().m_ProfessionReputationController.ChangeTab("Tab2");
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_CANGJINGGE://藏经阁
                    RankWindow.Instance().isRankDataReturn = true;
                    RankWindow.Instance().m_TabController.ChangeTab("Tab11");
                   // RankWindow.Instance().m_ProfessionReputationController.ChangeTab("Tab3");
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_GUILDCOMBAT://帮会实力
                    RankWindow.Instance().isRankDataReturn = true;
                    RankWindow.Instance().m_TabController.ChangeTab("Tab12");
                    //RankWindow.Instance().m_ProfessionReputationController.ChangeTab("Tab4");
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_MASTER://宗师
                    RankWindow.Instance().isRankDataReturn = true;
                    RankWindow.Instance().m_TabController.ChangeTab("Tab13");
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_SHAOLINREPUTATION://少林
                    RankWindow.Instance().isRankDataReturn = true;
                    RankWindow.Instance().m_TabController.ChangeTab("Tab14");
                    RankWindow.Instance().m_ProfessionReputationController.ChangeTab("Tab1");
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_TIANSHANREPUTATION://天山
                    RankWindow.Instance().isRankDataReturn = true;
                    RankWindow.Instance().m_TabController.ChangeTab("Tab14");
                    RankWindow.Instance().m_ProfessionReputationController.ChangeTab("Tab2");
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_DALIREPUTATION://大理
                    RankWindow.Instance().isRankDataReturn = true;
                    RankWindow.Instance().m_TabController.ChangeTab("Tab14");
                    RankWindow.Instance().m_ProfessionReputationController.ChangeTab("Tab3");
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_XIAOYAOREPUTATION://逍遥 
                    RankWindow.Instance().isRankDataReturn = true;
                    RankWindow.Instance().m_TabController.ChangeTab("Tab14");
                    RankWindow.Instance().m_ProfessionReputationController.ChangeTab("Tab4");
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_MASTERAVTIVECOMBAT://师门战斗力
                    RankWindow.Instance().isRankDataReturn = true;
                    RankWindow.Instance().m_TabController.ChangeTab("Tab15");
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_SHAOSHISHAN://少室山
                    RankWindow.Instance().isRankDataReturn = true;
                    RankWindow.Instance().m_TabController.ChangeTab("Tab16");
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_TOTALONLINETIME://在线时间排行榜
                    RankWindow.Instance().isRankDataReturn = true;
                    RankWindow.Instance().m_TabController.ChangeTab("Tab17");
                    break;
                 default:
                    return;
            }

            RankWindow.Instance().SetTotalPage(PVPData.RankTotalPage, PVPData.RankCurPage);
            int iCurCount = 0;
            foreach (PVPData.RankDataItem data in PVPData.RankDataList)
            {
                if( iCurCount < RankWindow.Instance().m_RankItem.Length)
                {
                    RankWindow.Instance().m_RankItem[iCurCount].gameObject.SetActive(true);
                    RankWindow.Instance().m_RankItem[iCurCount].SendData(
                        data.str1, data.str2, data.str3, data.str4, data.str5, data.str6);
                    iCurCount++;
                }
            }

            RankWindow.Instance().m_MeRank.text = meRank;
            RankWindow.Instance().isRankDataReturn = false;
            if (RankWindow.Instance().m_ItemTopGrid != null)
            {
                RankWindow.Instance().m_ItemTopGrid.IsResetOnEnable = true;
                RankWindow.Instance().m_ItemTopGrid.recenterTopNow = true;
            }
        }
    }

    public static void OpenSNSWindows( )
    {
        if (false == GameManager.gameManager.PlayerDataPool.IsServerFlagOpen(SERVER_FLAGS_ENUM.FLAG_SNS) ||
            false == PlatformHelper.IsEnableShareCenter())
        {
            return;
        }
        UIManager.ShowUI(UIInfo.AwardRoot, ShowSNSWindow);
    }

    public static void ShowSNSWindow(bool bSuccess, object param)
    {
        if (bSuccess)
        {
            if (AwardLogic.Instance() != null)
            {
                //AwardLogic.Instance().ShowSNSWindow();
                AwardLogic.Instance().SetAfterStartDelegateSNS();
            }
        }
    }

    public static int WorldBossChallengeResult { set; get; }
    public static void ShowWorldBossChallengeResult(GC_WORLDBOSS_CHALLEGE_RES packet)
    {
        WorldBossChallengeResult = packet.Result;
        ChallengeRewardLogic.ShowRewardUI(3);
    }
}
