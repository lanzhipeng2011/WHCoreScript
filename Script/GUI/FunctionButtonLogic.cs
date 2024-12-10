using Games.AwardActivity;
using Games.GlobeDefine;
using Games.LogicObj;
using Games.UserCommonData;
using GCGame;
using GCGame.Table;
using Module.Log;
using System.Collections.Generic;
using UnityEngine;

public class FunctionButtonLogic : MonoBehaviour
{
    private static FunctionButtonLogic m_Instance = null;
    public static FunctionButtonLogic Instance()
    {
        return m_Instance;
    }

    private static bool m_bShowDetailButtons = false;

    public GameObject m_FirstChild;
    public GameObject m_FunctionButtonOffset;
    public UILabel LabelDoubleExpTimer;     // 双倍经验倒计时
    public GameObject m_PKNormalBt;
    public GameObject m_PKKillBt;
    public GameObject m_FunctionButtonGrid;
    public GameObject m_BtnOpenDetail;      // 打开详细界面
    public GameObject m_BtnCloseDetail;
    //public GameObject m_BtnCloseDetail;     // 关闭详细界面
    public GameObject m_BtnAutoBegin;
    public GameObject m_BtnAutoStop;
    public GameObject m_ExitFB;
    public GameObject m_CanHideButtonRoot;
    public GameObject m_RadarWindow;
    //public GameObject m_BtnOpenRadar;
    //public GameObject m_BtnCloseRadar;
    //public TweenPosition m_FunctionWindowTween;

    public GameObject m_TipsObject;

    public UILabel m_LabelTotalAimTips;
    public UILabel m_LabelTotalAimTips2;

    public UILabel m_LabelDaliyLuckyTip;
    public UIImageButton m_ButtonDailyLuckyDraw;

    // 奖励活动相关
    public UILabel AwardTipsText;
    //======领取奖励tips
    public GameObject RewardTips;
    //======领取占星奖励tips
    public GameObject ZhanxingTips;
    private float m_updateTimer = 1.0f;

    // 新手指引相关
    private int m_NewPlayerGuide_Step = -1;
    public int NewPlayerGuide_Step
    {
        get { return m_NewPlayerGuide_Step; }
        set { m_NewPlayerGuide_Step = value; }
    }
    private bool m_Direction = false;
    public GameObject m_ButtonAct;
    public UISprite m_ButtonActTip;

    public UIGrid m_TipsGrid;
    public GameObject m_ButtonMailTip;
    // world boss
    public GameObject m_ButtonWorldBoss;
    public GameObject m_ButtonZhenQiAssit;
    public UILabel m_ZhenQiAssistCount;
    // 摇钱树按钮
    public GameObject m_MoneyTreeButton;
    private GameObject m_NewButton = null;

    private string countNameStr = "ui_pub_029";

    //====tipsCount;
    private int tipsCount = 0;

    private int m_nExitTime = -1;
    public int ExitTime
    {
        get { return m_nExitTime; }
        set { m_nExitTime = value; }
    }

    public UILabel m_ExitTime;
    public GameObject m_AuotTeamCue;

    void Awake()
    {
        OnMailUpdate(MailData.MailUpdateType.UPDATE, 0);

        m_Instance = this;
        ShowFunctionButtons(false);
        //m_FunctionWindowTween.transform.localPosition = m_bShowRadarWindow ? m_FunctionWindowTween.from : m_FunctionWindowTween.to;
    }

    // Use this for initialization
    void Start()
    {
        MailData.delMailUpdate += OnMailUpdate;
        InitUITweenerWhenChangeScene();
        initAwardActivityTips();
        UpdateMoneyTreeButton();
        UpdateAutoFightBtnState();
        UpdateDaliyLuckNum();
        m_FirstChild.SetActive(true);
        m_ExitTime.text = "";
        if (GameManager.gameManager.ActiveScene.IsCopyScene())
        {
            m_ExitFB.SetActive(true);  //临时屏蔽掉
        }
        else
        {
            m_ExitFB.SetActive(false);
        }
        if (HuaShanPVPData.WorldBossOpen == 1)
            m_ButtonWorldBoss.SetActive(true);

        InitButtonActive();
        UpdateActionButtonTip();
        UpdateAutoTeamCue();
        UpdateRewardButtonTip();
        m_TipsGrid.repositionNow = true;
    }

    // Update is called once per frame
    void OnClickVIP(GameObject go)
    {
        UIManager.ShowUI(UIInfo.VipRoot);
    }
    void FixedUpdate()
    {
        UpdateDoubleExpTimer();
        UpdateRewardButtonTip();
        UpdateExitTime();
    }

    void OnDestroy()
    {
        MailData.delMailUpdate -= OnMailUpdate;
        m_Instance = null;
    }

    public static float m_fTimeSecond = Time.realtimeSinceStartup;
    public void UpdateExitTime()
    {
        float ftimeSec = Time.realtimeSinceStartup;
        int nTimeData = (int)(ftimeSec - m_fTimeSecond);
        if (nTimeData > 0)
        {
            if (m_nExitTime > 0)
            {
                m_nExitTime = m_nExitTime - nTimeData;
                //m_ExitTime.text = "距离副本结束时间:" + m_nExitTime.ToString() + "秒";
                m_ExitTime.text = StrDictionary.GetClientDictionaryString("#{2835}", m_nExitTime);
                if (m_nExitTime <= 0)
                {
                    //                     CG_LEAVE_COPYSCENE packet = (CG_LEAVE_COPYSCENE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_LEAVE_COPYSCENE);
                    //                     packet.NoParam = 1;
                    //                     packet.SendPacket();
                    m_ExitTime.text = "";
                    m_nExitTime = 0;
                }

            }
            m_fTimeSecond = ftimeSec;
        }
    }
    public void PlayTween(bool nDirection)
    {
        //BeforeClickPlayerFrame(nDirection);
        gameObject.SetActive(!nDirection);
        //foreach(TweenAlpha tween in m_FoldTween)
        //{
        //    tween.Play(nDirection);
        //}

        m_Direction = nDirection;
    }

    /// <summary>
    /// 应对切换场景时UI异常消失 重新初始化Tween动画
    /// </summary>
    void InitUITweenerWhenChangeScene()
    {
        //curTween.Reset();
        //curTween.alpha = 1;
        //foreach(TweenAlpha tween in m_FoldTween)
        //{
        //    tween.Reset();
        //    tween.alpha = 1;
        //}
    }

    //public void BeforeClickPlayerFrame(bool nDirection)
    //{
    //    foreach (BoxCollider box in gameObject.GetComponentsInChildren<BoxCollider>())
    //    {
    //        UIImageButton button = box.gameObject.GetComponent<UIImageButton>();
    //        if (button != null)
    //        {
    //            button.isEnabled = !nDirection;
    //        }
    //        else
    //        {
    //            box.enabled = !nDirection;
    //        }
    //    }
    //}

    void ShowSceneMap()
    {
        if (MainUILogic.Instance() != null)
        {
            NewPlayerGuidLogic.CloseWindow();
            UIManager.ShowUI(UIInfo.SceneMapRoot);
        }
    }

    //=======显示可领奖励提示
    public void UpdateRewardButtonTip()
    {
        //		int onlineNum = GameManager.gameManager.PlayerDataPool.AwardActivityData.OnlineRewardTipNum;
        int onlineNum = GameManager.gameManager.PlayerDataPool.AwardActivityData.LeftTime;
        int dayNum = GameManager.gameManager.PlayerDataPool.AwardActivityData.LevelRewardTipNum;
        int dailyNum = GameManager.gameManager.PlayerDataPool.AwardActivityData.DailyRewardTipNum;
        int dl7DayNum = GameManager.gameManager.PlayerDataPool.AwardActivityData.DL7RewardTipNum;

        int DiviTCDTime = GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawFreeCDTime;
        int DiviMCDTime = GameManager.gameManager.PlayerDataPool.MoneyTreeData.CDTime;

        if (onlineNum == 0 || dayNum > 0 || dailyNum > 0 || dl7DayNum > 0)
        {
            RewardTips.SetActive(true);
        }
        else
        {
            RewardTips.SetActive(false);
        }


        if (DiviTCDTime <= 0 || DiviMCDTime <= 0)
        {
            if ((Singleton<ObjManager>.GetInstance().MainPlayer != null) && Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.Level >= 15)
                ZhanxingTips.SetActive(true);
            else
                ZhanxingTips.SetActive(false);
        }
        else
        {
            ZhanxingTips.SetActive(false);
        }

    }


    public void UpdateActionButtonTip()
    {
        //默认不显示圆点，只要有一个条件满足的时候就显示圆点并返回。
        m_ButtonActTip.spriteName = "";

        //1日常
        {
            if (Utils.GetActivityAwardBonusLeft() > 0)
            {
                m_ButtonActTip.spriteName = countNameStr;
                return;
            }
        }

        //等级显示限制
        int nLevel = 0;
        if (Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            nLevel = Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.Level;
        }

        //2江湖名人录
        {
            if (PVPData.LeftFightTime > 0 && nLevel >= 20)
            {
                m_ButtonActTip.spriteName = countNameStr;
                return;
            }
        }

        if (nLevel < 20)
            return;

        //3聚贤庄11
        {
            int nCount = 0;
            int nCur = 0;
            int nMax = 0;
            Utils.GetCopySceneCountsAll((int)GameDefine_Globe.SCENE_DEFINE.SCENE_HULAOGUAN, ref nCur, ref nMax);
            nCount += nCur;
            if (nCount > 0)
            {
                m_ButtonActTip.spriteName = countNameStr;
                return;
            }
        }
        //5藏经阁14
        {
            int nCount = 0;
            int nCur = 0;
            int nMax = 0;
            Utils.GetCopySceneCounts((int)GameDefine_Globe.SCENE_DEFINE.SCENE_WUSHENTA, 1, 1, ref nCur, ref nMax);
            nCount += nCur;
            Utils.GetSweepCounts(ref nCur, ref nMax);
            nCount += nCur;

            if (nCount > 0)
            {
                m_ButtonActTip.spriteName = countNameStr;
                return;
            }
        }
        //6燕子坞19
        {
            int nCount = 0;
            int nCur = 0;
            int nMax = 0;
            Utils.GetCopySceneCountsAll((int)GameDefine_Globe.SCENE_DEFINE.SCENE_HUSONGMEIREN, ref nCur, ref nMax);
            nCount += nCur;

            if (nCount > 0)
            {
                m_ButtonActTip.spriteName = countNameStr;
                return;
            }
        }
        //7珍珑棋局28
        {
            int nCount = 0;
            int nCur = 0;
            int nMax = 0;
            Utils.GetCopySceneCountsAll((int)GameDefine_Globe.SCENE_DEFINE.SCENE_QIXINGXUANZHEN, ref nCur, ref nMax);
            nCount += nCur;

            if (nCount > 0)
            {
                m_ButtonActTip.spriteName = countNameStr;
                return;
            }
        }
        //8燕王古墓27
        {
            int nCount = 0;
            int nCur = 0;
            int nMax = 0;
            Utils.GetCopySceneCountsAll((int)GameDefine_Globe.SCENE_DEFINE.SCENE_YEXIDAYING, ref nCur, ref nMax);
            nCount += nCur;

            if (nCount > 0)
            {
                m_ButtonActTip.spriteName = countNameStr;
                return;
            }
        }
        // 9 天降奇珍
        {
            bool bFlag = GameManager.gameManager.PlayerDataPool.CommonData.GetCommondFlag((int)USER_COMMONFLAG.CF_GUILDACTIVITY_FLAG);
            if (bFlag)
            {
                m_ButtonActTip.spriteName = countNameStr;
                return;
            }
        }
        //13怒海锄奸7
        {
            int nCount = 0;
            int nCur = 0;
            int nMax = 0;
            Utils.GetCopySceneCounts((int)GameDefine_Globe.SCENE_DEFINE.SCENE_GUOGUANZHANJIANG, 1, 1, ref nCur, ref nMax);
            nCount += nCur;

            if (nCount > 0)
            {
                m_ButtonActTip.spriteName = countNameStr;
                return;
            }
        }
        //10少室山31
        {
            //            // 少室山等级要求
            //            if (Singleton<ObjManager>.Instance.MainPlayer.BaseAttr.Level >= 70)
            //            {
            int nCount = 0;
            int nCur = 0;
            int nMax = 0;
            Utils.GetCopySceneCounts((int)GameDefine_Globe.SCENE_DEFINE.SCENE_FENGHUOLIANTIAN, 2, 1, ref nCur, ref nMax);
            nCount += nCur;

            if (nCount > 0)
            {
                m_ButtonActTip.spriteName = countNameStr;
                return;
            }
            //            }
        }

    }
    void OnShopClick()
    {
        NewPlayerGuidLogic.CloseWindow();

        //UIManager.ShowUI(UIInfo.SysShop);
        //UIManager.ShowUI(UIInfo.YuanBaoShop);
        // 需要看元宝商店是否开启
        CG_ASK_YUANBAOSHOP_OPEN packet = (CG_ASK_YUANBAOSHOP_OPEN)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_YUANBAOSHOP_OPEN);
        packet.NoParam = 1;
        packet.SendPacket();
    }
    void OnChongZhiClick()
    {
        RechargeData.PayUI();
    }


    void UpdateDoubleExpTimer()
    {
        if (null == LabelDoubleExpTimer)
        {
            // label不小心被删时，防止抛异常
            return;
        }
        m_updateTimer -= Time.deltaTime;
        if (m_updateTimer <= 0)
        {
            m_updateTimer = 1.0f;
            int doubleExpLeftTime = AdditionData.doubleExpDisableTime - (int)Time.realtimeSinceStartup;
            if (doubleExpLeftTime > 0)
            {
                if (!LabelDoubleExpTimer.gameObject.activeSelf)
                {
                    LabelDoubleExpTimer.gameObject.SetActive(true);
                }
                Utils.SetTimeDiffToLabel(LabelDoubleExpTimer, doubleExpLeftTime);
            }
            else if (LabelDoubleExpTimer.gameObject.activeSelf)
            {
                LabelDoubleExpTimer.gameObject.SetActive(false);
            }
        }
    }



    public void initAwardActivityTips()
    {
        CleanUpAwardActivityTips();
        UpdateButtonAwardTips();
    }

    public void CleanUpAwardActivityTips()
    {
        if (AwardTipsText == null)
        {
            LogModule.DebugLog("AwardTips or AwardTipsText GameObject if null");
            return;
        }
        AwardTipsText.text = "";
        AwardTipsText.gameObject.SetActive(false);
    }

    public void UpdateButtonAwardTips()
    {
        if (AwardTipsText == null)
        {
            return;
        }
        if (m_LabelTotalAimTips == null)
        {
            return;
        }
        if (m_LabelTotalAimTips2 == null)
        {
            return;
        }
        AwardActivityData Data = GameManager.gameManager.PlayerDataPool.AwardActivityData;
        int TipCount = 0;
        int dayAward = 0;

        // 策划要求 加等级限制 5级
        if (Singleton<ObjManager>.Instance.MainPlayer)
        {
            int nLevel = Singleton<ObjManager>.Instance.MainPlayer.BaseAttr.Level;
            if (nLevel >= 1)                            //if ( nLevel >= 5)
            {
                // 每日奖励
                int nWeekDay = Data.WeekDay;
                bool bDayAwardFlag = Data.DayAwardFlag;
                if (nWeekDay > -1 && bDayAwardFlag == false) // 未领取
                {
                    dayAward += 1;
                }
                // 在线奖励
                int nOnlineAwardID = Data.OnlineAwardID;
                int nLeftTime = Data.LeftTime;
                if (nOnlineAwardID > -1 && nLeftTime <= 0)
                {
                    dayAward += 1;
                }
                // 首周奖励
                int nDays = Data.NewServerDays;
                bool bCanAward = Data.IsCanGetAward(nDays);
                if (nDays > -1 && bCanAward == true) // 未领取
                {
                    dayAward += 1;
                }
                // 在线奖励
                int nNewOnlineAwardID = Data.NewOnlineAwardID;
                int nNewLeftTime = Data.NewLeftTime;
                if (nNewOnlineAwardID > -1 && nNewLeftTime <= 0 && Data.NewOnlineAwardStart)
                {
                    dayAward += 1;
                }

                // 在线7天奖励
                int nNew7DayOnlineAwardID = Data.New7DayOnlineAwardID;
                int nNew7DayLeftTime = Data.New7DayLeftTime;
                if (nNew7DayOnlineAwardID > -1 && nNew7DayLeftTime <= 0 && Data.New7DayOnlineAwardStart)
                {
                    dayAward += 1;
                }
            }

            if (nLevel >= 15)
            {
                int nTurnID = GameManager.gameManager.PlayerDataPool.MoneyTreeData.MoneyTreeID;
                int nTime = GameManager.gameManager.PlayerDataPool.MoneyTreeData.CDTime;
                int nMaxFreeCount = Games.MoneyTree.MoneyTreeData.MaxFreeAwardNum;
                if (nTime <= 0 && nTurnID > GlobeVar.INVALID_ID && nTurnID < nMaxFreeCount)
                {
                    TipCount += 1;
                }
            }

            if (nLevel >= 16)
            {
                if (true == GameManager.gameManager.PlayerDataPool.IsServerFlagOpen(SERVER_FLAGS_ENUM.FLAG_SNS))
                {
                    bool bRet = GameManager.gameManager.PlayerDataPool.CommonData.GetCommondFlag((int)USER_COMMONFLAG.CF_SNS_DAILY_REWARD);
                    if (!bRet)
                    {
                        dayAward += 1;
                    }
                }
            }

            if (nLevel >= 10)
            {
                int nDrawFreeCDTime = GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawFreeCDTime;
                int nDrawFreeTimes = GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawFreeTimes;
                if (nDrawFreeCDTime <= 0 && nDrawFreeTimes > 0)
                {
                    TipCount += 1;
                }
            }

        }

        TipCount += dayAward;

        if (dayAward > 0)
        {
            // 提醒 为 1
            AwardTipsText.gameObject.SetActive(true);
            AwardTipsText.text = dayAward.ToString();
        }
        else
        {
            AwardTipsText.text = "";
            AwardTipsText.gameObject.SetActive(false);
        }

        if (TipCount > 0)
        {
            m_LabelTotalAimTips.text = TipCount.ToString();
            m_LabelTotalAimTips2.text = TipCount.ToString();
            m_LabelTotalAimTips.gameObject.SetActive(true);
            m_LabelTotalAimTips2.gameObject.SetActive(true);

            m_TipsObject.gameObject.SetActive(true);
        }
        else
        {
            m_LabelTotalAimTips.text = "";
            m_LabelTotalAimTips2.text = "";
            m_LabelTotalAimTips.gameObject.SetActive(false);
            m_LabelTotalAimTips2.gameObject.SetActive(false);

            m_TipsObject.gameObject.SetActive(false);
        }
        //		int onlineNum=GameManager.gameManager.PlayerDataPool.AwardActivityData.LeftTime;
        //		int dayNum = GameManager.gameManager.PlayerDataPool.AwardActivityData.LevelRewardTipNum;
        //		int dailyNum = GameManager.gameManager.PlayerDataPool.AwardActivityData.DailyRewardTipNum;
        //		int dl7DayNum = GameManager.gameManager.PlayerDataPool.AwardActivityData.DL7RewardTipNum;
        //		
        //		if(onlineNum == 0 ||  dayNum>0 || dailyNum > 0||dl7DayNum>0)
        //		{
        //			m_TipsObject.SetActive(true);
        //		}else{
        //			m_TipsObject.SetActive(false);
        //		}

        tipsCount = TipCount;

    }

    void ShowAwardUI()
    {
        if (MainUILogic.Instance() != null)
        {
            UIManager.ShowUI(UIInfo.AwardRoot);
        }
    }

    void OnLingJiangBtnFun()
    {
        if (MainUILogic.Instance() != null)
        {
            UIManager.ShowUI(UIInfo.RewardRoot);
        }
    }

    void OnActivityClick(GameObject value)
    {
        if (m_NewButton != null && m_NewButton == value)
        {
            StopNewButtonEffect();
        }

        UIManager.ShowUI(UIInfo.Activity);
    }

    void OnPKClick()
    {
        NewPlayerGuidLogic.CloseWindow();

        UIManager.ShowUI(UIInfo.PKSetInfo);
    }

    public void NewPlayerGuide(int nIndex)
    {
        if (m_Direction == true)
        {
            return;
        }

        if (nIndex < 0)
        {
            return;
        }

        NewPlayerGuidLogic.CloseWindow();

        m_NewPlayerGuide_Step = nIndex;

        switch (nIndex)
        {
            case (int)GameDefine_Globe.NEWOLAYERGUIDE.ACTIVITY_MISSION: // 日常任务
            case (int)GameDefine_Globe.NEWOLAYERGUIDE.ACTIVITY_QUNXIONG:// 武道之巅
            case (int)GameDefine_Globe.NEWOLAYERGUIDE.ACTIVITY_QIXINGXUANZHEN:// 藏经阁
            case (int)GameDefine_Globe.NEWOLAYERGUIDE.ACTIVITY_HULAOGUAN:// 燕子坞
            case (int)GameDefine_Globe.NEWOLAYERGUIDE.ACTIVITY_WUSHENTA:// 聚贤庄
            case (int)GameDefine_Globe.NEWOLAYERGUIDE.ACTIVITY_HUSONGMEIREN:// 珍珑棋局
            case (int)GameDefine_Globe.NEWOLAYERGUIDE.ACTIVITY_GUOGUANZHANJIANG:// 珍珑棋局
            case (int)GameDefine_Globe.NEWOLAYERGUIDE.ACTIVITY_FENGHUOLIANTIAN:// 珍珑棋局
            case (int)GameDefine_Globe.NEWOLAYERGUIDE.ACTIVITY_YEXIDAYIN:// 珍珑棋局
                {
                    NewPlayerGuidLogic.OpenWindow(m_ButtonAct, 106, 106, "", "left", 2, true, true);
                }
                break;
            case (int)GameDefine_Globe.NEWOLAYERGUIDE.DIVI_MONEY:
            case (int)GameDefine_Globe.NEWOLAYERGUIDE.DIVI_DRAW:
                {
                    if (false == m_CanHideButtonRoot.activeSelf)
                    {
                        NewPlayerGuidLogic.OpenWindow(m_BtnOpenDetail.gameObject, 106, 106, "", "left", 2, true, true);
                        break;
                    }
                    NewPlayerGuidLogic.OpenWindow(m_ButtonDailyLuckyDraw.gameObject, 106, 106, "", "left", 2, true, true);
                    break;
                }
            case (int)GameDefine_Globe.NEWOLAYERGUIDE.EXITDUNGEON: // 离开副本
                if (null != m_ExitFB && m_ExitFB.activeSelf)
                {
                    NewPlayerGuidLogic.OpenWindow(m_ExitFB, 106, 106, "", "left", 2, true, true);
                }
                break;
        }
    }

    void OnMailUpdate(MailData.MailUpdateType curUpdateType, System.UInt64 curKey)
    {
        foreach (KeyValuePair<ulong, MailData.UserMail> curMail in MailData.UserMailMap)
        {
            if (!curMail.Value.bReaded)
            {
                if (!m_ButtonMailTip.activeSelf)
                {
                    m_ButtonMailTip.SetActive(true);
                    m_TipsGrid.repositionNow = true;
                }
                return;
            }
        }

        m_ButtonMailTip.SetActive(false);
        m_TipsGrid.repositionNow = true;
    }

    void OnMailTipClick()
    {
        if (NewPlayerGuidLogic.Instance())
        {
            NewPlayerGuidLogic.CloseWindow();
        }
        m_ButtonMailTip.gameObject.SetActive(false);
        RelationLogic.OpenMailRecvWindow();
    }

    //每日幸运抽奖相关
    void OnDailyLuckyDrawClick()
    {
        if (Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.Level < 10)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{3187}");
            return;
        }
        if (MainUILogic.Instance() != null)
        {
            UIManager.ShowUI(UIInfo.DailyDrawRoot);
        }
    }

    // 
    void OnDivinationClick()
    {
        if (NewPlayerGuidLogic.Instance())
        {
            NewPlayerGuidLogic.CloseWindow();
        }
        if (Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.Level < 15)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{3187}");
            return;
        }
        if (MainUILogic.Instance() != null)
        {
            UIManager.ShowUI(UIInfo.DivinationRoot);
        }
    }

    public void UpdateMoneyTreeButton()
    {
        if (Singleton<ObjManager>.Instance.MainPlayer == null
            || Singleton<ObjManager>.Instance.MainPlayer.BaseAttr.Level < 5)
        {
            return;
        }

        if (m_MoneyTreeButton == null)
        {
            return;
        }
        int nMoneyTreeID = GameManager.gameManager.PlayerDataPool.MoneyTreeData.MoneyTreeID;
        if (nMoneyTreeID < 0 || nMoneyTreeID > 20)
        {
            m_MoneyTreeButton.SetActive(false);
        }
        else if (m_MoneyTreeButton.activeSelf == false)
        {
            m_MoneyTreeButton.SetActive(true);
        }
    }
    public void OnExitFBClick()
    {
        if ((int)GameDefine_Globe.NEWOLAYERGUIDE.EXITDUNGEON == m_NewPlayerGuide_Step)
        {
            NewPlayerGuidLogic.CloseWindow();
            m_NewPlayerGuide_Step = -1;
        }

        string str = StrDictionary.GetClientDictionaryString("#{1847}");
        if ((int)GameDefine_Globe.SCENE_DEFINE.SCENE_WUSHENTA == GameManager.gameManager.RunningScene)
        {
            str = StrDictionary.GetClientDictionaryString("#{2345}");
        }
        //天下无双
        if (23 == GameManager.gameManager.RunningScene)
        {
            str = StrDictionary.GetClientDictionaryString("#{6014}");
        }
        int sceneid = GameManager.gameManager.RunningScene;
        Tab_SceneClass scene = TableManager.GetSceneClassByID(sceneid, 0);
        if (scene != null)
        {
            if (scene.CopySceneID == -1)
            {
                str = StrDictionary.GetClientDictionaryString("#{2345}");
            }
        }
        MessageBoxLogic.OpenOKCancelBox(str, "", OnLeaveCopySceneOK, OnLEaveCopySceneNO);

    }
    public void OnLeaveCopySceneOK()
    {
        if (GameManager.gameManager.PlayerDataPool.CopySceneChange) //正在传送中
        {
            return;
        }
        GameManager.gameManager.PlayerDataPool.CopySceneChange = true;
        CG_LEAVE_COPYSCENE packet = (CG_LEAVE_COPYSCENE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_LEAVE_COPYSCENE);
        packet.NoParam = 1;
        packet.SendPacket();
    }
    public void OnLEaveCopySceneNO()
    {

    }

    public void OpenFunction(int nType)
    {
        switch (nType)
        {
            case (int)Games.UserCommonData.USER_COMMONFLAG.CF_ACTIVITYFUNCTION_OPENFLAG:
                {
                    if (!m_ButtonAct.activeSelf)
                    {
                        NewItemGetLogic.InitItemInfo(m_ButtonAct.GetComponent<UIImageButton>().target.spriteName,
                            m_ButtonAct,
                            NewItemGetLogic.NEWITEMTYPE.TYPE_FUNCTION, nType);
                        m_NewButton = m_ButtonAct;
                        m_ButtonAct.SetActive(true);
                    }
                }
                break;
        }
    }

    public void LevelUpButtonActive()
    {
        return;
        int nPlayerLevel = Singleton<ObjManager>.Instance.MainPlayer.BaseAttr.Level;
        if (nPlayerLevel >= (int)GameDefine_Globe.NEWBUTTON_LEVEL.ACTIVITY)
        {
            if (!m_ButtonAct.activeSelf)
            {
                NewItemGetLogic.InitItemInfo(m_ButtonAct.GetComponent<UIImageButton>().target.spriteName,
                    m_ButtonAct,
                    NewItemGetLogic.NEWITEMTYPE.TYPE_FUNCTION);
                m_NewButton = m_ButtonAct;
            }
        }
        else if (nPlayerLevel >= (int)GameDefine_Globe.NEWBUTTON_LEVEL.AUTOFIGHT)
        {
            // 改地方了WD
            /*
            if (!m_ButtonAuto.activeSelf)
            {
                NewItemGetLogic.InitItemInfo(m_ButtonAuto.GetComponent<UIImageButton>().target.spriteName,
                    m_ButtonAuto,
                    NewItemGetLogic.NEWITEMTYPE.TYPE_FUNCTION);
                m_NewButton = m_ButtonAuto;
            }
             * */
        }
    }

    public void InitButtonActive()
    {

        if (m_ButtonAct == null)
        {
            return;
        }

        bool bRet = GameManager.gameManager.PlayerDataPool.CommonData.GetCommondFlag((int)USER_COMMONFLAG.CF_ACTIVITYFUNCTION_OPENFLAG);
        m_ButtonAct.SetActive(bRet);
    }

    public void StopNewButtonEffect()
    {
        if (m_NewButton == null)
        {
            return;
        }

        Transform effectTrans = m_NewButton.transform.FindChild("EffectPoint");
        if (effectTrans == null)
        {
            return;
        }

        Transform spriteAniTrans = effectTrans.FindChild("SpriteAni");
        if (spriteAniTrans == null)
        {
            return;
        }

        spriteAniTrans.gameObject.SetActive(false);

        m_NewButton = null;
    }

    public void PlayNewButtonEffect()
    {
        if (m_NewButton == null)
        {
            return;
        }

        Transform effectTrans = m_NewButton.transform.FindChild("EffectPoint");
        if (effectTrans == null)
        {
            return;
        }

        Transform spriteAniTrans = effectTrans.FindChild("SpriteAni");
        if (spriteAniTrans == null)
        {
            return;
        }

        spriteAniTrans.gameObject.SetActive(true);
    }

    public void OnWorldBossClick()
    {
        //======龙虎榜按钮去掉显示条件改为全显示
        // if (GlobeVar.INVALID_ID != GameManager.gameManager.PlayerDataPool.TeamInfo.TeamID)
        //{
        HuaShanPVPData.IsClickWorldBossUI = 1;
        CG_WORLDBOSS_TEAMLIST_REQ packet = (CG_WORLDBOSS_TEAMLIST_REQ)PacketDistributed.CreatePacket(MessageID.PACKET_CG_WORLDBOSS_TEAMLIST_REQ);
        packet.Page = 1;
        packet.SendPacket();
        //}
        //else
        //{

        //    m_ButtonWorldBoss.SetActive(false);
        // }
    }

    public void OnWorldBossDead()
    {
        if (m_ButtonWorldBoss != null)
            m_ButtonWorldBoss.SetActive(false);
        HuaShanPVPData.WorldBossOpen = 0;
    }

    public void OnWorldBossStateChange(int state)
    {
        if (state != 2)
        {
            if (m_ButtonWorldBoss != null)
                m_ButtonWorldBoss.SetActive(false);
            HuaShanPVPData.WorldBossOpen = 0;
        }
        else
        {
            if (m_ButtonWorldBoss != null)
                m_ButtonWorldBoss.SetActive(true);
            HuaShanPVPData.WorldBossOpen = 1;

            m_TipsGrid.repositionNow = true;
        }
    }

    public void ZhenQiAssistState(int state, int times)
    {
        if (m_ZhenQiAssistCount != null)
        {
            if (state == 0)
                m_ZhenQiAssistCount.text = "";
            else
            {
                m_ZhenQiAssistCount.text = StrDictionary.GetClientDictionaryString("#{2143}", times);
            }
        }
        if (m_ButtonZhenQiAssit != null)
        {
            m_ButtonZhenQiAssit.SetActive(state == 1 ? true : false);
        }
    }

    public void ZhenQiAssistButtonClick()
    {
        if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_TIANXIAWUSHUANG)
        {
            CG_HUASHAN_ASSIST_REQ packet = (CG_HUASHAN_ASSIST_REQ)PacketDistributed.CreatePacket(MessageID.PACKET_CG_HUASHAN_ASSIST_REQ);
            packet.None = 0;
            packet.SendPacket();
        }
        else
        {
            ZhenQiAssistState(0, 0);
        }
    }



    // 直接调用自动战斗
    void OnDoAutoFightClick()
    {
        if (m_NewPlayerGuide_Step == 1)
        {
            NewPlayerGuidLogic.CloseWindow();
            m_NewPlayerGuide_Step = -1;
        }

        Obj_MainPlayer mainPalyer = Singleton<ObjManager>.Instance.MainPlayer;
        if (null == mainPalyer)
        {
            return;
        }
        mainPalyer.EnterAutoCombat();
        UpdateAutoFightBtnState();
    }

    void OnDoAutoStopFightClick()
    {
        if (m_NewPlayerGuide_Step == 3)
        {
            NewPlayerGuidLogic.CloseWindow();
            m_NewPlayerGuide_Step = -1;
        }
        Obj_MainPlayer mainPalyer = Singleton<ObjManager>.Instance.MainPlayer;
        if (null == mainPalyer)
        {
            return;
        }
        mainPalyer.LeveAutoCombat();
        UpdateAutoFightBtnState();
    }

    void ShowFunctionButtons(bool bShow)
    {
        /*
        m_FunctionButtonGrid.SetActive(bShow);
        //m_BtnCloseDetail.SetActive(bShow);
        m_BtnOpenDetail.SetActive(!bShow);
        m_bShowDetailButtons = bShow;
        */

        m_BtnCloseDetail.gameObject.SetActive(bShow);
        m_BtnOpenDetail.gameObject.SetActive(!bShow);
        m_CanHideButtonRoot.SetActive(bShow);
        m_RadarWindow.SetActive(!bShow);
        m_bShowDetailButtons = bShow;

        changeTipsObj(bShow);

        NewPlayerGuide(m_NewPlayerGuide_Step);
    }

    private void changeTipsObj(bool boo)
    {

        if (tipsCount > 0)
        {
            if (boo)
                m_TipsObject.gameObject.SetActive(false);
            else
                m_TipsObject.gameObject.SetActive(true);
        }
    }


    public void UpdateAutoFightBtnState()
    {
        Obj_MainPlayer mainPalyer = Singleton<ObjManager>.Instance.MainPlayer;
        if (null == mainPalyer)
        {
            return;
        }
        m_BtnAutoStop.SetActive(mainPalyer.IsOpenAutoCombat);
        m_BtnAutoBegin.SetActive(!mainPalyer.IsOpenAutoCombat);
    }

    public void UpdateDaliyLuckNum()
    {
        if (null != m_LabelDaliyLuckyTip)
        {
            Obj_MainPlayer mainPalyer = Singleton<ObjManager>.Instance.MainPlayer;
            if (null == mainPalyer)
            {
                return;
            }
            if (GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawFreeCDTime > 0 ||
                GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawFreeTimes == 0 ||
                mainPalyer.BaseAttr.Level < 10 ||
                (false == GameManager.gameManager.PlayerDataPool.IsServerFlagOpen(SERVER_FLAGS_ENUM.FLAG_DAILYLUCKYDRAW)))
            {
                m_LabelDaliyLuckyTip.gameObject.SetActive(false);
            }
            else
            {
                m_LabelDaliyLuckyTip.gameObject.SetActive(true);
            }
        }
    }
    public void UpdateDailyLuckyButton()
    {
        if (null != m_ButtonDailyLuckyDraw)
        {
            if (GameManager.gameManager.PlayerDataPool.IsServerFlagOpen(SERVER_FLAGS_ENUM.FLAG_DAILYLUCKYDRAW))
            {
                m_ButtonDailyLuckyDraw.gameObject.SetActive(true);
                UpdateDaliyLuckNum();
            }
            else
            {
                m_ButtonDailyLuckyDraw.gameObject.SetActive(false);
            }
        }
    }

    void OnShowDetailButtons()
    {
        ShowFunctionButtons(true);
    }

    void OnHideDetailButtions()
    {
        ShowFunctionButtons(false);
    }
    public void UpdateAutoTeamCue()
    {
        if (GameManager.gameManager.PlayerDataPool.AutoTeamState == true)
        {
            m_AuotTeamCue.SetActive(true);
        }
        else
        {
            m_AuotTeamCue.SetActive(false);
        }

    }
}
