using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
using System.Collections.Generic;
using Games.LogicObj;
using GCGame;
using GCGame.Table;

public class HuaShanPVPData
{
#region opponent info
    public struct OpponentViewInfo
    {
        public OpponentViewInfo(
            ulong _guid,
            string _name,
            CharacterDefine.PROFESSION _profession,
            int _level,
            int _combat,
            int _pos, 
            int _hp,
            int _mp,
            int _atk,
            int _def,
            int _cri,
            int _spd,
            int _dge
            )
        {
            guid = _guid;
            name = _name;
            profession = _profession;
            level = _level;
            pos = _pos;
            hp = _hp;
            mp = _mp;
            atk = _atk;
            def = _def;
            cri = _cri;
            spd = _spd;
            dge = _dge;
			combat = _combat;
        }
		
        public void Clean()
        {
            guid = Games.GlobeDefine.GlobeVar.INVALID_GUID;
        }

        public ulong guid;
        public string name;
        public CharacterDefine.PROFESSION profession;
        public int level;
        public int combat;
        public int pos;
        public int hp;
        public int mp;
        public int atk;
        public int def;
        public int cri;
        public int spd;
        public int dge;
    }

    public static OpponentViewInfo OppoViewInfo = new OpponentViewInfo();

    public delegate void ShowOpponentViewInfoDelegate();
    public static ShowOpponentViewInfoDelegate delegateShowOpponentViewInfo;

    public delegate void ShowWaitForOpponetDelegate();
    public static ShowWaitForOpponetDelegate delegateShowWaitForOpponet;

    public static void ShowOppoentViewInfo( GC_HUASHAN_PVP_OPPONENTVIEW msg )
    {
        if (null != ActivityController.Instance())
            ActivityController.Instance().ChangeToHSPvPShowOppoentView();


	    OppoViewInfo.Clean();
	    OppoViewInfo.guid = msg.Guid;
	    OppoViewInfo.name = msg.Name;
	    OppoViewInfo.profession = (CharacterDefine.PROFESSION)msg.Profession;
	    OppoViewInfo.level = msg.Level;
	    OppoViewInfo.combat = msg.Combatnum;
	    OppoViewInfo.pos = msg.Pos;
	    OppoViewInfo.hp = msg.Hp;
	    OppoViewInfo.mp = msg.Mp;
	    OppoViewInfo.atk = msg.Attack;
	    OppoViewInfo.def = msg.Defense;
	    OppoViewInfo.cri = msg.Critical;
	    OppoViewInfo.spd = msg.Speed;
	    OppoViewInfo.dge = msg.Dodge;

        if (null != delegateShowOpponentViewInfo)
            delegateShowOpponentViewInfo();
    }

    public static int Resultwin { set; get; }
    public static int Rounder { set; get; }
    public static int huaShanPosition = -1;
    public static int HuaShanPosition 
    { 
        set{ huaShanPosition = value;}
        get { return huaShanPosition; }
    }

    public static int HuaShanState { set; get; }

    public delegate void ShowDefaultWindowMySelfDelegate();
    public static ShowDefaultWindowMySelfDelegate delegateShowDefaultWindowMySelf;

    public static string HSRoundTipPrefix()
    {
        if (Rounder == 1)
            return Utils.GetDicByID(2234);
        else if( Rounder == 2)
            return Utils.GetDicByID(2233);
        else
            return StrDictionary.GetClientDictionaryString("#{2232}", Rounder);
    }

    public static string HSRounderTip()
    {
        if (Rounder == 1)
            return Utils.GetDicByID(2234);
        else if (Rounder == 2)
            return Utils.GetDicByID(2233);
        else
            return StrDictionary.GetClientDictionaryString("#{1844}", Rounder);
    }

    public static void DealHuaShanPvPState(GC_HUASHAN_PVP_STATE msg)
    {
        HuaShanState = msg.State;
        int curPosition = -1;
        int curRounder = 0;
        if (msg.HasPosition)
        {
            curPosition = msg.Position;
        }

        if (msg.HasRounder)
        {
            curRounder = msg.Rounder;
        }

        Rounder = curRounder;
        HuaShanPosition = curPosition;


        switch ((GC_HUASHAN_PVP_STATE.HSPVPSTATE)msg.State)
        {
            case GC_HUASHAN_PVP_STATE.HSPVPSTATE.KICKED:
                //..增加通知
                if (delegateShowDefaultWindowMySelf != null)
                {
                    delegateShowDefaultWindowMySelf();
                }
                break;
            case GC_HUASHAN_PVP_STATE.HSPVPSTATE.CLOSED:
                //..增加通知
                if (delegateShowDefaultWindowMySelf != null)
                {
                    delegateShowDefaultWindowMySelf();
                }
                break;
            case GC_HUASHAN_PVP_STATE.HSPVPSTATE.REGISTER:
                //..增加通知
                if (delegateShowDefaultWindowMySelf != null)
                {
                    delegateShowDefaultWindowMySelf();
                }
                break;
            case GC_HUASHAN_PVP_STATE.HSPVPSTATE.REGOK:
                //..增加通知
                if (delegateShowDefaultWindowMySelf != null)
                {
                    delegateShowDefaultWindowMySelf();
                }
                break;
            case GC_HUASHAN_PVP_STATE.HSPVPSTATE.REGISTERED:
                //..增加通知
                if (delegateShowDefaultWindowMySelf != null)
                {
                    delegateShowDefaultWindowMySelf();
                }
                break;
            case GC_HUASHAN_PVP_STATE.HSPVPSTATE.MAKEEFF:
                //..增加通知
                if (delegateShowDefaultWindowMySelf != null)
                {
                    delegateShowDefaultWindowMySelf();
                }
                break;
            case GC_HUASHAN_PVP_STATE.HSPVPSTATE.WAITNEXTROUND:
                //..增加通知
                if (delegateShowDefaultWindowMySelf != null)
                {
                    delegateShowDefaultWindowMySelf();
                }
                break;
            case GC_HUASHAN_PVP_STATE.HSPVPSTATE.SEARCH:
                //..暂时不处理，使用通用的发送
                break;
            case GC_HUASHAN_PVP_STATE.HSPVPSTATE.OPPVIEW:
                //..暂时不处理，使用通用的发送
                break;
            case GC_HUASHAN_PVP_STATE.HSPVPSTATE.WIN:
            case GC_HUASHAN_PVP_STATE.HSPVPSTATE.LOSE:
                {
                    Resultwin = ((int)GC_HUASHAN_PVP_STATE.HSPVPSTATE.WIN == msg.State) ?
                           1 : 0;
                    Rounder = msg.Rounder;
                    //if (Resultwin == 0) SelfHSPos = -1;
                    ChallengeRewardLogic.ShowRewardUI(1);
                }
                break;
            case GC_HUASHAN_PVP_STATE.HSPVPSTATE.WAITFOR:
                {
                    Rounder = msg.Rounder;
                    GUIData.AddNotifyData2Client(false, "#{2344}");
                    if (null != delegateShowWaitForOpponet)
                        delegateShowWaitForOpponet();
                }
                break;
            case GC_HUASHAN_PVP_STATE.HSPVPSTATE.LUCKDOG:
                {
                    Rounder = msg.Rounder;
                    string roundTips = HSRoundTipPrefix();
                    GUIData.AddNotifyData2Client(false, "#{1663}", roundTips);
                    
                    OppoViewInfo.Clean();
                    if (null != delegateShowOpponentViewInfo)
                        delegateShowOpponentViewInfo();
                }
                break;
            case GC_HUASHAN_PVP_STATE.HSPVPSTATE.FINISH:
                if (delegateShowDefaultWindowMySelf != null)
                {
                    delegateShowDefaultWindowMySelf();
                }
                break;
            case GC_HUASHAN_PVP_STATE.HSPVPSTATE.START:
                //..增加通知
                if (delegateShowDefaultWindowMySelf != null)
                {
                    delegateShowDefaultWindowMySelf();
                }
                break;
        }
    }

    public static int DuelResult { set; get; }
    public static void DealDuelState(GC_DUEL_STATE msg)
    {
        switch ((GC_DUEL_STATE.DUELSTATE)msg.State)
        {
            case GC_DUEL_STATE.DUELSTATE.WIN:
            case GC_DUEL_STATE.DUELSTATE.LOSE:
                {
                    DuelResult = ((int)GC_DUEL_STATE.DUELSTATE.WIN == msg.State) ?
                                       1 : 0;
                    ChallengeRewardLogic.ShowRewardUI(2);
                }
                break;
            default:
                break;
        }
    }

    // ...
    public static int ContinueSec { set; get; }
 
    public delegate void ShowSearchOpponentDelegate(int sec, int s);
    public static ShowSearchOpponentDelegate delegateShowSearchOpponent;
    public static void ShowSearchOpponent(GC_HUASHAN_PVP_ShowSearch msg)
    {
        ContinueSec = msg.ContinueSecond;
        Rounder = msg.Progress;
        if (null != ActivityController.Instance())
        {
            ActivityController.Instance().ChangeToHSPvPShowOppoentView();
            CallShowSearchOpponentDelegate( );
        }
        else
        {
            UIManager.ShowUI(UIInfo.Activity, OnActiveShowActiviController);
        }
    }

    static void OnActiveShowActiviController(bool bSuccess, object param)
    {
        ActivityController.Instance().ChangeToHuaShanTab();
        ActivityController.Instance().ChangeToHSPvPShowOppoentView();
        CallShowSearchOpponentDelegate( );
    }

    static void CallShowSearchOpponentDelegate( )
    {
        if (null != delegateShowSearchOpponent)
            delegateShowSearchOpponent(ContinueSec, Rounder);
    }


#endregion
#region Self
//     private static int SelfHSPos = -1;
//     public static int MySelfeHuaShanPvPPos { 
//         set{SelfHSPos = value;}
//         get{return SelfHSPos;} }
//     public delegate void ShowSelfRegisterInfoDelegate();
//     public static ShowSelfRegisterInfoDelegate delegateShowSelfRegisterInfo;
//     public static void ShowSelfRegisterInfo(GC_HUASHAN_PVP_SELFPOSITION msg)
//     {
//         MySelfeHuaShanPvPPos = msg.SelfPos;
//         if (null != delegateShowSelfRegisterInfo)
//             delegateShowSelfRegisterInfo();
//     }
#endregion

#region register Member
    public class RegisterMemberInfo
    {
        public RegisterMemberInfo(
            ulong _guid,
            string _name,
            CharacterDefine.PROFESSION _profession,
            int _level,
            int _combat,
            int _pos
            )
        {
            guid = _guid;
            name = _name;
            profession = _profession;
            level = _level;
            combat = _combat;
            pos = _pos;
        }
        public ulong guid;
        public string name;
        public CharacterDefine.PROFESSION profession;
        public int level;
        public int combat;
        public int pos;
    };

    public static List<RegisterMemberInfo> RegisterMemberList = new List<RegisterMemberInfo>();
    public delegate void ShowRegisterMemberListDelegate();
    public static ShowRegisterMemberListDelegate delegateShowRegisterMemberList;

    public static void ShowRegisterMemberList(GC_HUASHAN_PVP_MEMBERLIST msg)
    {
        RegisterMemberList.Clear();
        HuaShanPosition = msg.SelfPos;
        //
         if (msg.guidCount <= 0)
        {
            if (null != ActivityController.Instance())
                ActivityController.Instance().ChangeToHSPvPShowDefault();
        }
        else
        {
            if (null != ActivityController.Instance())
            {
                ActivityController.Instance().ChangeToHSPvPShowMemberList();
            }

            for (int i = 0; i < msg.guidCount; i++)
            {
                RegisterMemberList.Add(new RegisterMemberInfo(
                    msg.GetGuid(i), msg.GetName(i),
                    (CharacterDefine.PROFESSION)msg.GetProfession(i),
                    msg.GetLevel(i), msg.GetCombatnum(i), msg.GetPos(i)));
            }

            // ....
            if (null != delegateShowRegisterMemberList)
            {
                delegateShowRegisterMemberList();
            }
        }
    }
#endregion

#region mercenary
    public struct MercenaryInfo
    {
        public MercenaryInfo(ulong _guid, string _name, int _relationship, int _cost, int pro, int com)
        {
            guid = _guid;
            name = _name;
            relationship = _relationship;
            cost = _cost;
            profession = pro;
            combat = com;
        }
        public ulong guid;
        public string name;
        public int relationship;
        public int cost;
        public int profession;
        public int combat;
    }

    public static int MercenarySceneClass { set; get; }
    public static int MercenaryTimesLeft { set; get; }
    public static List<MercenaryInfo> MercenaryList = new List<MercenaryInfo>();
    public delegate void ShowMercenaryListDelegate();
    public static ShowMercenaryListDelegate delegateShowMercenaryList;

    public static void ShowMercenaryList(GC_MERCENARY_LIST_RES msg)
    {
        MercenaryList.Clear();

        HuaShanPVPData.MercenaryTimesLeft = msg.Lefttimes;
        //...
        for (int i = 0; i < msg.guidCount; i++)
        {
            MercenaryList.Add(new MercenaryInfo(
                msg.GetGuid(i), msg.GetName(i),
                msg.GetSource(i), msg.GetCost(i),
                msg.GetProfession(i), msg.GetCombat(i)));
        }

        if (MercenaryWindow.Instance() != null)
        {
            CallShowMercenaryList();
            CallShowMercenaryLeftTime();
        }
        else
            UIManager.ShowUI(UIInfo.MercenaryWindowRoot, OnShowMercenaryMemberRoot);
    }
    public static void OnShowMercenaryMemberRoot(bool bSuccess, object param)
    {
        CallShowMercenaryList();
        CallShowMercenaryLeftTime();
    }
    public static void CallShowMercenaryList()
    {
        if (null != delegateShowMercenaryList)
            delegateShowMercenaryList();
    }

    //..
    public delegate void ShowMercenaryLeftTimesDelegate();
    public static ShowMercenaryLeftTimesDelegate delegateShowMercenaryLeftTimes;
    public static void SetMercenaryLeftTimes(GC_MERCENARY_LEFTTIMES msg)
    {
        HuaShanPVPData.MercenaryTimesLeft = msg.Lefttimes;
        CallShowMercenaryLeftTime();
    }

    public static void CallShowMercenaryLeftTime()
    {
        if (null != delegateShowMercenaryLeftTimes)
            delegateShowMercenaryLeftTimes();
    }

    public static List<MercenaryInfo> MercenaryEmployList = new List<MercenaryInfo>();
    public delegate void ShowMercenaryEmployedDelegate();
    public static ShowMercenaryEmployedDelegate delegateShowMercenaryEmployed;
    public static void ShowMercenaryEmployed(GC_MERCENARY_EMPLOYLIST msg)
    {
        MercenaryEmployList.Clear();

        //...
        for (int i = 0; i < msg.guidCount; i++)
        {
            if (msg.GetGuid(i) != GlobeVar.INVALID_GUID)
            {
                MercenaryEmployList.Add(new MercenaryInfo(
                    msg.GetGuid(i), msg.GetName(i), 
                    msg.GetSource(i), msg.GetCost(i),
                      msg.GetProfession(i), msg.GetCombat(i)));
            }
        }

        if (null != delegateShowMercenaryEmployed)
            delegateShowMercenaryEmployed();

    }
#endregion

#region  world boss
    public struct WorldBossTeamInfo
    {
        public WorldBossTeamInfo(int _id, string _name, int _score, int _pos, int _cd, int _per)
        {
            id = _id;
            name = _name;
            score = _score;
            pos = _pos;
            cd = _cd;
            per = _per;
        }

        public int id;
        public string name;
        public int score;
        public int pos;
        public int cd;
        public int per;
    }

    public static int WorldBossCurPage { set; get; }
    public static int WorldBossTotalPage { set; get; }
    public static int WorldBossOpen { set; get; }
    public static int IsClickWorldBossUI { set; get; }

    public static List<WorldBossTeamInfo>WorldBossList = new List<WorldBossTeamInfo>();
    public delegate void ShowWorldBossListDelegate();
    public static ShowWorldBossListDelegate delegateShowWorldBossList;

    public static void ShowWorldBossList(GC_WORLDBOSS_TEAMLIST msg)
    {
       WorldBossList.Clear();

       //...
       WorldBossCurPage = msg.Curpage;
       WorldBossTotalPage = msg.Totalpage;
       for (int i = 0; i < msg.teamIdCount; i++)
       {
           WorldBossList.Add(new WorldBossTeamInfo(
                msg.GetTeamId(i), msg.GetLeadername(i),
                msg.GetScore(i), msg.GetPos(i), msg.GetCd(i), msg.GetPer(i)));
       }

       if (WorldBossWindow.Instance() != null)
       {
           CallShowWorldBossList();
       }
       else
       {
           if (IsClickWorldBossUI == 1)
           {
               IsClickWorldBossUI = 0;
               UIManager.ShowUI(UIInfo.WorldBossWindowRoot, OnShowWorldBossMemberRoot);
           }
       }
    }

    public static void OnShowWorldBossMemberRoot(bool bSuccess, object param)
    {
        CallShowWorldBossList();
    }
    public static void CallShowWorldBossList()
    {
        if (null != delegateShowWorldBossList)
            delegateShowWorldBossList();
    }

    public static void ShowWorldBossChallengeBox(GC_WORLDBOSS_SOMECHALL_ME msg)
    {
        string str = StrDictionary.GetClientDictionaryString("#{2909}", msg.TeamName);
        MessageBoxLogic.OpenOKCancelBox(str, "", SendAgreeWorldBossChallenge, SendDisagreeWorldBossChallenge);
    }

    public static void SendAgreeWorldBossChallenge()
    {
        CG_WORLDBOSS_CHALL_RESPONSE packet = (CG_WORLDBOSS_CHALL_RESPONSE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_WORLDBOSS_CHALL_RESPONSE);
        packet.SetAgree(1);
        packet.SendPacket();
    }

    public static void SendDisagreeWorldBossChallenge()
    {
        CG_WORLDBOSS_CHALL_RESPONSE packet = (CG_WORLDBOSS_CHALL_RESPONSE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_WORLDBOSS_CHALL_RESPONSE);
        packet.SetAgree(0);
        packet.SendPacket();
    }
#endregion

    public class MemberPKInfo
    {
        public MemberPKInfo(
        string fristname,
        string secondname,
        string winnername)
        {
            m_fristname = fristname;
            m_secondname = secondname;
            m_winnername = winnername;
        }
        public string m_fristname;
        public string m_secondname;
        public string m_winnername;
    };

    public static List<MemberPKInfo> HuaShanPKInfoList = new List<MemberPKInfo>();
    public delegate void ShowPkInfoDelegate();
    public static ShowPkInfoDelegate delegateShowPkInfo;

    public static void ShowHuaShanPkInfoList(GC_RET_HUASHAN_PKINFO msg)
    {
        HuaShanPKInfoList.Clear();
        Rounder = msg.Rounder;
        //
        for (int i = 0; i < msg.fristnameCount; i++)
        {
            HuaShanPKInfoList.Add(new MemberPKInfo(
                msg.GetFristname(i), msg.GetSecondname(i), msg.GetWinnername(i)));
        }
        // ....
        if (null != ActivityController.Instance() && ActivityController.Instance().IsShowHuaShanTab())
        {
            if (null != ActivityController.Instance())
            {
                ActivityController.Instance().ChangeToHSPvPShowPKInfo();
            }
            if (null != delegateShowPkInfo)
            {
                delegateShowPkInfo();
            }
        }
    }
}
