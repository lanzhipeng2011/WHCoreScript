/********************************************************************
	创建时间:	2014/07/02 11:20
	全路径:		\Project\Client\Assets\MLDJ\Script\GUI\GuildWarInfoLogic.cs
	创建人:		luoy
	功能说明:	帮战信息界面
	修改记录:
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Games.GlobeDefine;
using Games.LogicObj;
using GCGame;
using GCGame.Table;
using Module.Log;
using UnityEngine;
using System.Collections;

public class GuildWarInfoLogic : MonoBehaviour
{
    public enum MAXCOUNT
    {
        MAXSCORECOUNT =16,//预赛排行榜最大数量
        MAXKILLCOUNT =3,//预赛个人击杀榜最大数量
        MAXFINALGROUPCOUNT =8,//晋级赛最大对战分组数
        MAXPRELINARYRANKNUM = 16,//海选最大的晋级数
    }


    public GameObject m_PreminaryRoot; //海选比赛界面
    public GameObject m_FinalRoot;//晋级比赛界面
    public GameObject m_NoWarRoot;//无比赛界面

    public GameObject m_SaiChengBiao; //帮战晋级分组表
    public GameObject m_PreminaryRankRoot;//海选积分榜

    public GameObject m_ShowFinalGroupInfoBt;//晋级比赛 显示分组的按钮
    //Item
    public GameObject m_FightGroupItem;//晋级赛对战分组
    public GameObject m_WinGroupItem;//晋级赛胜出队伍
    public GameObject m_PreliminaryScoreItem;//海选积分榜选项
    public GameObject m_PreliminaryKillItem;//海选界面击杀榜选项
    public GameObject m_PreliminaryRankItem;//海选信息榜
    //gird
    public GameObject m_GuidWarScoreRankGird;//帮战海选赛积分排行榜
    public GameObject m_GuidWarKillRankGird;//帮战海选赛击杀排行榜
    public GameObject m_FightGird;//晋级赛对战
    public GameObject m_WinGird;//晋级赛胜出
    public GameObject m_PreliminaryRankGird; //海选信息排行榜
    public UIGrid m_GroupFightGird; //分组界面对战列表的Grid
    public UIGrid m_GroupWinGird; //分组界面胜出列表的Grid
    public UILabel m_GuildWarScoreLable;             //帮战海选赛积分
    public UILabel m_GuildWarTimesLable;             //帮战海选赛剩余参战次数
    public UILabel m_GuildWarKillerNumLable;         //帮战海选赛击杀人数
    public UILabel m_MyGuildPointScoreLable;              //本帮据点总分
    public UILabel m_FightGuildPointScoreLable;           //对方帮会据点总分
    public UILabel m_MyGuildNameLabel;                  //本帮帮会名字
    public UILabel m_FightGuildNameLabel;               //对方帮会名字
    public GameObject m_PointHelpInfo;                  //据点争夺帮助信息
    public UILabel m_RoundInfoLable;                    //分组信息
    public GameObject[] m_PointGameObj = new GameObject[(int)GUILDWARPOINTTYPE.MAXPOINTNUM];
    private GuildWarPonitItemLogic[] m_PointItem =new GuildWarPonitItemLogic[(int)GUILDWARPOINTTYPE.MAXPOINTNUM];
    private GuildWarScoreRankItemLogic[] m_ScoreRankItemLogic =new GuildWarScoreRankItemLogic[(int)MAXCOUNT.MAXSCORECOUNT];
    private GuildWarKillRankItemLogic[] m_KillRankItemLoigc =new GuildWarKillRankItemLogic[(int)MAXCOUNT.MAXKILLCOUNT];
    private GuildWarFightGroupItemLogic[] m_FightGroupItemLogic = new GuildWarFightGroupItemLogic[(int)MAXCOUNT.MAXFINALGROUPCOUNT];
    private GuildWarWinGroupItemLogic[] m_WinGroupItemLogic = new GuildWarWinGroupItemLogic[(int)MAXCOUNT.MAXFINALGROUPCOUNT];
    private GuildWarRankItemLogic[] m_GuildWarRankItemLogic = new GuildWarRankItemLogic[(int)MAXCOUNT.MAXPRELINARYRANKNUM];
    private int m_curWarType =-1;//当前帮战类型
    private int m_curWarSchedule = -1;//当前进程(比赛中 围观中)
    public int CurWarType
    {
        get { return m_curWarType; }
        set { m_curWarType = value; }
    }
    private float m_fLastPointInfoUpdateTime = 0.0f;//上次据点信息更新时间
    private static GuildWarInfoLogic m_Instance = null;
    public static GuildWarInfoLogic Instance()
    {
        return m_Instance;
    }
    private bool m_bIsLoadItem = false;
    void Awake()
    {
        m_Instance = this;
    }
	// Use this for initialization
    private void Start()
    {
       
    }

    void OnDestroy()
    {
        m_Instance = null;
    }

    void OnEnable()
    {
        m_PreminaryRoot.SetActive(false);
        m_FinalRoot.SetActive(false);
        m_NoWarRoot.SetActive(false);
        m_SaiChengBiao.SetActive(false);
        m_PointHelpInfo.SetActive(false);
        if (m_bIsLoadItem ==false)
        {
            OnLoadItem();
            m_bIsLoadItem = true;
        }
        //请求当前帮战类型
        AskCurWarTypeInfo();
    }
    //请求当前帮战类型
    void AskCurWarTypeInfo()
    {
        CG_ASK_CURGUILDWARTYPE askPak = (CG_ASK_CURGUILDWARTYPE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_CURGUILDWARTYPE);
        askPak.SetNopara(-1);
        askPak.SendPacket();
    }

    void ShowNoWarUI() //显示没比赛的界面
    {
        m_PreminaryRoot.SetActive(false);
        m_FinalRoot.SetActive(false);
        m_NoWarRoot.SetActive(true);
    }

    void ShowPreliminaryUI() //显示海选比赛的界面
    {
        m_GuildWarScoreLable.text = "0";
        m_GuildWarKillerNumLable.text = "0";
        m_GuildWarTimesLable.text = "0/0";
        m_PreminaryRoot.SetActive(true);
        m_FinalRoot.SetActive(false);
        m_NoWarRoot.SetActive(false);
        AskPreliminaryGuildWarInfo();
    }

    void ShowFinalUI() //显示晋级赛和约战的比赛界面
    {
        m_PreminaryRoot.SetActive(false);
        m_NoWarRoot.SetActive(false);
        m_ShowFinalGroupInfoBt.SetActive(false);
        m_FinalRoot.SetActive(false);
        AskFinalWarInfo();
    }
    public void RetCurWarType(int nWarType,int nRetType) 
    {
        m_curWarType = nWarType;
        m_curWarSchedule = nRetType;
        switch ((GC_RET_CURGUILDWARTYPE.RETTYPE) nRetType)
        {
            case GC_RET_CURGUILDWARTYPE.RETTYPE.NOTYPE:
            case GC_RET_CURGUILDWARTYPE.RETTYPE.PRELIMINARY_NOJION:
            case GC_RET_CURGUILDWARTYPE.RETTYPE.FINAL_NOJION:
                ShowNoWarUI();
                break;
            case GC_RET_CURGUILDWARTYPE.RETTYPE.PRELIMINARY_JION:
                ShowPreliminaryUI();
                break;
            case GC_RET_CURGUILDWARTYPE.RETTYPE.FINAL_JION:
            case GC_RET_CURGUILDWARTYPE.RETTYPE.CHALLENGEWAR:
                ShowFinalUI();
                break;
        }
    }
	// Update is called once per frame
	void Update ()
	{
	    if (m_FinalRoot.activeInHierarchy)
	    {
            if (Time.time - m_fLastPointInfoUpdateTime > 1) //没间隔1s 更新一次
            {
                m_fLastPointInfoUpdateTime = Time.time;
                CG_ASK_FINALGUILDWARPOINTINFO infoPak = (CG_ASK_FINALGUILDWARPOINTINFO)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_FINALGUILDWARPOINTINFO);
                infoPak.SetWarType(m_curWarType);
                infoPak.SendPacket();
            }
	    }
	}

    public void UpdateGuildWarPreliminaryInfo(GC_RET_PRELIMINARY_WARINFO pak)
    {
        //个人信息
        m_GuildWarScoreLable.text =pak.MyGuildScore.ToString();
        m_GuildWarKillerNumLable.text =pak.MyKillNum.ToString();
        m_GuildWarTimesLable.text = String.Format("{0}/10", 10-pak.MyRemainTimes);
    }

    public void UpdateGuildWarPreliminaryRankInfo(GC_RET_RANK packet)
    {
        //帮战排行榜
        for (int i = 0; i < (int)MAXCOUNT.MAXSCORECOUNT; i++)
        {
            m_ScoreRankItemLogic[i].CleanUp(i);
            if (i >= 0 && i < packet.guildScoreCount)
            {
                GuildWarPreliminaryRank RankInfo = new GuildWarPreliminaryRank();
                RankInfo.CleanUp();
                RankInfo.SortNum = packet.GetGuildSortNum(i);
                RankInfo.Score = packet.GetGuildScore(i);
                RankInfo.GuildName = packet.GetGuilidName(i);
                m_ScoreRankItemLogic[i].InitInfo(RankInfo);
            }
        }
        for (int i = 0; i < (int)MAXCOUNT.MAXPRELINARYRANKNUM; i++)
        {
            m_GuildWarRankItemLogic[i].CleanUp();
            if (i >= 0 && i < packet.guildScoreCount)
            {
                m_GuildWarRankItemLogic[i].InitInfo(packet.GetGuilidName(i),packet.GetGuildScore(i));
            }
        }
        if (m_curWarSchedule != (int)GC_RET_CURGUILDWARTYPE.RETTYPE.PRELIMINARY_JION)
        {
            m_PreminaryRankRoot.SetActive(true);
        }
        else
        {
            m_PreminaryRankRoot.SetActive(false);
        }
       
    }
    public void UpdateGuildWarKillRankInfo(GC_RET_RANK packet)
    {
        //个人击杀排行榜
        for (int i = 0; i < (int)MAXCOUNT.MAXKILLCOUNT; i++)
        {
            m_KillRankItemLoigc[i].CleanUp(i);
            if( i >=0 && i < packet.killerSortNumCount)
            {
                GuildWarKillRank RankInfo = new GuildWarKillRank();
                RankInfo.CleanUp();
                RankInfo.SortNum = packet.GetKillerSortNum(i);
                RankInfo.KillerNum = packet.GetKillNum(i);
                RankInfo.KillerName = packet.GetKillerName(i);
                m_KillRankItemLoigc[i].InitInfo(RankInfo);
            }
        }
    }

    public void UpdateWarGroupInfo(GC_RET_FINALGUILDWARGROUPINFO packet)
    {
        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (_mainPlayer ==null)
        {
            return;
        }
        if (packet.GroupIndexCount<=0)
        {
            _mainPlayer.SendNoticMsg(false, "#{2586}");
            return;
        }
        //显示对战的帮会
        if (packet.CurRound>0)
        {
            int nNeedShowGroup =16/(int)(Mathf.Pow(2, packet.CurRound));
            for (int i = 0; i < (int)MAXCOUNT.MAXFINALGROUPCOUNT; i++)
            {
                if (null != m_FightGroupItemLogic[i])
                {
                    m_FightGroupItemLogic[i].CleanUp();
                    if (i < nNeedShowGroup)
                    {
                        m_FightGroupItemLogic[i].gameObject.SetActive(true);
                        if (i >= 0 && i < packet.GroupIndexCount)
                        {
                            GuildWarGroupInfo groupInfo = new GuildWarGroupInfo();
                            groupInfo.CleanUp();
                            groupInfo.GroupIndex = i;
                            groupInfo.GuildAScore = packet.GetGuildAScore(i);
                            groupInfo.GuildBScore = packet.GetGuildBScore(i);
                            groupInfo.GuildAName = packet.GetGuildAName(i);
                            groupInfo.GuildBName = packet.GetGuildBName(i);
                            groupInfo.WinType = packet.GetWinType(i);
                            m_FightGroupItemLogic[i].InitGroupInfo(groupInfo);
                        }
                    }
                }
            }

            if (packet.CurRound ==1)
            {
                m_GroupFightGird.maxPerLine = 1;
                m_GroupFightGird.cellWidth = 147;
                Vector3 FightGirdPos = m_FightGird.transform.localPosition;
				FightGirdPos.x = -757;
                m_FightGird.transform.localPosition = FightGirdPos;

                m_GroupWinGird.maxPerLine = 1;
				m_GroupWinGird.cellWidth = 147;
                Vector3 WinGirdPos = m_WinGird.transform.localPosition;
				WinGirdPos.x = -757;
                m_WinGird.transform.localPosition = WinGirdPos;
                m_RoundInfoLable.text = StrDictionary.GetClientDictionaryString("#{2597}", nNeedShowGroup * 2);
            }
            else if (packet.CurRound == 2)
            {
                m_GroupFightGird.maxPerLine = 1;
				m_GroupFightGird.cellWidth = 250;
                Vector3 FightGirdPos = m_FightGird.transform.localPosition;
				FightGirdPos.x = -610;
                m_FightGird.transform.localPosition = FightGirdPos;

                m_GroupWinGird.maxPerLine = 1;
				m_GroupWinGird.cellWidth = 250;
                Vector3 WinGirdPos = m_WinGird.transform.localPosition;
				WinGirdPos.x = -610;
                m_WinGird.transform.localPosition = WinGirdPos;
                m_RoundInfoLable.text = StrDictionary.GetClientDictionaryString("#{2597}", nNeedShowGroup * 2);
              
            }
            else
            {
                if (packet.CurRound == 3)
                {

                    m_GroupFightGird.maxPerLine = 1;
					m_GroupFightGird.cellWidth = 300;
                    Vector3 FightGirdPos = m_FightGird.transform.localPosition;
					FightGirdPos.x = -380;
                    m_FightGird.transform.localPosition = FightGirdPos;

                    m_GroupWinGird.maxPerLine = 1;
					m_GroupWinGird.cellWidth = 300;
                    Vector3 WinGirdPos = m_WinGird.transform.localPosition;
					WinGirdPos.x =-380;
                    m_WinGird.transform.localPosition = WinGirdPos;
                    m_RoundInfoLable.text = StrDictionary.GetClientDictionaryString("#{3001}");
                }
                else if (packet.CurRound == 4)
                {
                    m_GroupFightGird.maxPerLine = 1;
					m_GroupFightGird.cellWidth = 300;
                    Vector3 FightGirdPos = m_FightGird.transform.localPosition;
					FightGirdPos.x = -230;
                    m_FightGird.transform.localPosition = FightGirdPos;

                    m_GroupWinGird.maxPerLine = 1;
					m_GroupWinGird.cellWidth = 300;
                    Vector3 WinGirdPos = m_WinGird.transform.localPosition;
					WinGirdPos.x = -230;
                    m_WinGird.transform.localPosition = WinGirdPos;
                    m_RoundInfoLable.text = StrDictionary.GetClientDictionaryString("#{3002}");
                }
            }
            m_FightGird.GetComponent<UIGrid>().Reposition();
            //显示胜利的帮会
            for (int i = 0; i < (int)MAXCOUNT.MAXFINALGROUPCOUNT; i++)
            {
                m_WinGroupItemLogic[i].CleanUp();
                if (i < nNeedShowGroup)
                {
                    m_WinGroupItemLogic[i].gameObject.SetActive(true);
                    if (i >= 0 && i < packet.GroupIndexCount)
                    {
                        if (packet.GetWinType(i) == (int)GC_RET_FINALGUILDWARGROUPINFO.WINTYPE.AGUILDWIN)
                        {
                            m_WinGroupItemLogic[i].InitInfo(packet.GetGuildAName(i));
                        }
                        else if (packet.GetWinType(i) == (int)GC_RET_FINALGUILDWARGROUPINFO.WINTYPE.BGUILDWIN)
                        {
                            m_WinGroupItemLogic[i].InitInfo(packet.GetGuildBName(i));
                        }
                    }
                }
            }
            m_GroupWinGird.Reposition();
            m_SaiChengBiao.SetActive(true);
        }
    }

    public void UpdateWarPointInfo(GC_RET_FINALGUILDWARPOINTINFO packet)
    {
        for (int i = 0; i < packet.pointTypeCount; i++)
        {
            GuildWarPointInfo pointInfo =new GuildWarPointInfo();
            pointInfo.PointType = packet.GetPointType(i);
            pointInfo.PointScore = packet.GetPointScore(i);
            pointInfo.PointOwnGuildGuid = packet.GetPointOwnGuildGuid(i);
            pointInfo.IsFighting = (packet.GetIsFighting(i) == 1 ? true : false);
            pointInfo.MyGuildScore = packet.MyGuildScore;
            pointInfo.FightGuildScore = packet.FightGuildScore;
            if (i>=0 && i<(int)GUILDWARPOINTTYPE.MAXPOINTNUM)
            {
                if (m_PointItem[i] ==null && m_PointGameObj[i] !=null)
                {
                    m_PointItem[i] = m_PointGameObj[i].GetComponent<GuildWarPonitItemLogic>();
                }
                if (m_PointItem[i] !=null)
                {
                    m_PointItem[i].InitInfo(pointInfo);
                }
            }
        }
        m_MyGuildPointScoreLable.text = packet.MyGuildScore.ToString();
        m_FightGuildPointScoreLable.text = packet.FightGuildScore.ToString();
        m_MyGuildNameLabel.text = packet.MyGuildName;
        m_FightGuildNameLabel.text = packet.FightGuildBName;
        m_FinalRoot.SetActive(true);
        if (m_curWarSchedule == (int)GC_RET_CURGUILDWARTYPE.RETTYPE.FINAL_JION)
        {
            m_ShowFinalGroupInfoBt.SetActive(true);
        }
        
    }
    void OnLoadItem()
    {
        //海选积分榜
        Utils.CleanGrid(m_GuidWarScoreRankGird);
        for (int i = 0; i < (int)MAXCOUNT.MAXSCORECOUNT; i++)
        {
            GameObject newGuildListItem = Utils.BindObjToParent(m_PreliminaryScoreItem, m_GuidWarScoreRankGird, (1000+i).ToString());
            if (newGuildListItem)
            {
                m_ScoreRankItemLogic[i] = newGuildListItem.GetComponent<GuildWarScoreRankItemLogic>();
                if (null != m_ScoreRankItemLogic[i])
                    m_ScoreRankItemLogic[i].CleanUp(i);
            }
        }
        //Grid排序，防止列表异常
        m_GuidWarScoreRankGird.GetComponent<UIGrid>().Reposition();
        m_GuidWarScoreRankGird.GetComponent<UITopGrid>().Recenter(true);
        //海选击杀榜
        Utils.CleanGrid(m_GuidWarKillRankGird);
        for (int i = 0; i < (int)MAXCOUNT.MAXKILLCOUNT; i++)
        {
            GameObject newGuildListItem = Utils.BindObjToParent(m_PreliminaryKillItem, m_GuidWarKillRankGird, (1000 + i).ToString());
            if (newGuildListItem)
            {
                m_KillRankItemLoigc[i] = newGuildListItem.GetComponent<GuildWarKillRankItemLogic>();
                if (null != m_KillRankItemLoigc[i])
                    m_KillRankItemLoigc[i].CleanUp(i);
            }
        }
        //Grid排序，防止列表异常
        m_GuidWarKillRankGird.GetComponent<UIGrid>().Reposition();
        m_GuidWarKillRankGird.GetComponent<UITopGrid>().Recenter(true);

        //晋级赛对战分组
        Utils.CleanGrid(m_FightGird);
        for (int i = 0; i < (int) MAXCOUNT.MAXFINALGROUPCOUNT; i++)
        {
            GameObject newGuildListItem = Utils.BindObjToParent(m_FightGroupItem, m_FightGird, (1000 + i).ToString());
            if (newGuildListItem)
            {
                m_FightGroupItemLogic[i] = newGuildListItem.GetComponent<GuildWarFightGroupItemLogic>();
                m_FightGroupItemLogic[i].CleanUp();
            }
        }
        m_FightGird.GetComponent<UIGrid>().Reposition();
        //晋级胜出
        Utils.CleanGrid(m_WinGird);
        for (int i = 0; i < (int)MAXCOUNT.MAXFINALGROUPCOUNT; i++)
        {
            GameObject newGuildListItem = Utils.BindObjToParent(m_WinGroupItem, m_WinGird, (1000 + i).ToString());
            if (newGuildListItem)
            {
                m_WinGroupItemLogic[i] = newGuildListItem.GetComponent<GuildWarWinGroupItemLogic>();
                if (null != m_WinGroupItemLogic[i])
                    m_WinGroupItemLogic[i].CleanUp();
            }
        }
        m_WinGird.GetComponent<UIGrid>().Reposition();
        //海选排行榜信息
        Utils.CleanGrid(m_PreliminaryRankGird);
        for (int i = 0; i < (int)MAXCOUNT.MAXPRELINARYRANKNUM; i++)
        {
            GameObject newGuildListItem = Utils.BindObjToParent(m_PreliminaryRankItem, m_PreliminaryRankGird, (1000 + i).ToString());
            if (newGuildListItem)
            {
                m_GuildWarRankItemLogic[i] = newGuildListItem.GetComponent<GuildWarRankItemLogic>();
                if (null != m_GuildWarRankItemLogic[i])
                    m_GuildWarRankItemLogic[i].CleanUp();
            }
        }
        m_PreliminaryRankGird.GetComponent<UIGrid>().Reposition();
    }
    
    
    void ClickEnterPreliminaryGuildWar()
    {
        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (_mainPlayer)
        {
            CG_PRELIMINARY_APPLYGUILDWAR ApplyPak = (CG_PRELIMINARY_APPLYGUILDWAR)PacketDistributed.CreatePacket(MessageID.PACKET_CG_PRELIMINARY_APPLYGUILDWAR);
            ApplyPak.SetObjid(_mainPlayer.ServerID);
            ApplyPak.SendPacket();
        }
    }
    public void CloseWindow()
    {
        UIManager.CloseUI(UIInfo.MasterAndGuildRoot);
    }
    //请求帮战海选信息
    void AskPreliminaryGuildWarInfo()
    {
        //请求海选赛信息
        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (_mainPlayer)
        {
            CG_ASK_PRELIMINARY_WARINFO InfoPak = (CG_ASK_PRELIMINARY_WARINFO)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_PRELIMINARY_WARINFO);
            InfoPak.SetObjid(_mainPlayer.ServerID);
            InfoPak.SendPacket();
        }
        //请求海选赛击杀排行榜
        CG_ASK_RANK ScoreRankPak = (CG_ASK_RANK)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_RANK);
        ScoreRankPak.NType = (int)GameDefine_Globe.RANKTYPE.TYPE_PRELIMINARYGUILDWARRANK;
        ScoreRankPak.NPage = 0;
        ScoreRankPak.SendPacket();
        //请求海选赛击杀排行榜
        CG_ASK_RANK KillRankPak = (CG_ASK_RANK)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_RANK);
        KillRankPak.NType = (int)GameDefine_Globe.RANKTYPE.TYPE_PRELIMINARYGUILDWARKILLRANK;
        KillRankPak.NPage = 0;
        KillRankPak.SendPacket();
    }

    void AskFinalWarInfo()
    {
        if (m_curWarSchedule == (int)GC_RET_CURGUILDWARTYPE.RETTYPE.FINAL_JION)
        {
            CG_ASK_FINALGUILDWARPOINTINFO infoPak = (CG_ASK_FINALGUILDWARPOINTINFO)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_FINALGUILDWARPOINTINFO);
            infoPak.SetWarType(m_curWarType);
            infoPak.SendPacket();
        }
        else if (m_curWarSchedule == (int)GC_RET_CURGUILDWARTYPE.RETTYPE.CHALLENGEWAR)
        {
            CG_ASK_FINALGUILDWARPOINTINFO infoPak = (CG_ASK_FINALGUILDWARPOINTINFO)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_FINALGUILDWARPOINTINFO);
            infoPak.SetWarType(m_curWarType);
            infoPak.SendPacket();
        }
    }

    public static string GetWarPointNameByType(int ponitType)
    {
        string pointName = "";
        switch (ponitType)
        {
            case (int)GUILDWARPOINTTYPE.TIANSHU:
                pointName = StrDictionary.GetClientDictionaryString("#{2498}");
                break;
            case (int)GUILDWARPOINTTYPE.TIANXUAN:
                pointName = StrDictionary.GetClientDictionaryString("#{2499}");
                break;
            case (int)GUILDWARPOINTTYPE.TIANJI:
                pointName = StrDictionary.GetClientDictionaryString("#{2500}");
                break;
            case (int)GUILDWARPOINTTYPE.TIANQUAN:
                pointName = StrDictionary.GetClientDictionaryString("#{2501}");
                break;
            case (int)GUILDWARPOINTTYPE.YUHENG:
                pointName = StrDictionary.GetClientDictionaryString("#{2502}");
                break;
            case (int)GUILDWARPOINTTYPE.KAIYANG:
                pointName = StrDictionary.GetClientDictionaryString("#{2503}");
                break;
            case (int)GUILDWARPOINTTYPE.YAOGUANG:
                pointName = StrDictionary.GetClientDictionaryString("#{2504}");
                break;
        }
        return pointName;
    }

    void ClosGroupInfo()
    {
        m_SaiChengBiao.SetActive(false);
    }
    void ClosPreliminaryRankInfo()
    {
        m_PreminaryRankRoot.SetActive(false);
    }
    void ClickShowGroupInfo()
    {
        CG_ASK_FINALGUILDWARGROUPINFO infoPak =(CG_ASK_FINALGUILDWARGROUPINFO)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_FINALGUILDWARGROUPINFO);
        infoPak.SetNoParam(-1);
        infoPak.SendPacket();
    }
    void ClickShowPreliminaryRankInfo()
    {
        //请求海选赛击杀排行榜
        CG_ASK_RANK ScoreRankPak = (CG_ASK_RANK)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_RANK);
        ScoreRankPak.NType = (int)GameDefine_Globe.RANKTYPE.TYPE_PRELIMINARYGUILDWARRANK;
        ScoreRankPak.NPage = 0;
        ScoreRankPak.SendPacket();
    }
    void PressPointHelpBt()
    {
        m_PointHelpInfo.SetActive(true);
    }

    void OnRelePointHelpBt()
    {
        m_PointHelpInfo.SetActive(false);
    }
}
