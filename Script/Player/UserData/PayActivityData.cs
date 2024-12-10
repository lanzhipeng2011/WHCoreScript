/********************************************************************
	created:	2014/07/31
	created:	31:7:2014   16:43
	filename: 	PayActivityData.cs
	author:		Tangyi
	purpose:	充值活动数据
*********************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GCGame.Table;
using Module.Log;
using System;

public enum FLAGTYPE
{
    //此枚举与服务器枚举对应 需要同时修改
    FLAG_FIRSTTIME          = 0,        //首充礼包 是否领取
    FLAG_MONTHCARD          = 1,        //月卡礼包 是否领取
    FLAG_MONTHCARD_TODAY    = 2,        //月卡礼包 是否领取
    FLAG_GROWUP             = 3,        //成长礼包 是否领取额
    FLAG_GROWUP_40          = 4,        //成长礼包 40级是否领取
    FLAG_GROWUP_50          = 5,        //成长礼包 50级是否领取
    FLAG_GROWUP_60          = 6,        //成长礼包 60级是否领取
    FLAG_GROWUP_70          = 7,        //成长礼包 70级是否领取
    FLAG_GROWUP_80          = 8,        //成长礼包 80级是否领取
    FLAG_GROWUP_90          = 9,        //成长礼包 90级是否领取

    FLAG_NUM                = 16,
}

public struct PayActivityData 
{
    /// <summary>
    /// 首充标记
    /// </summary>
    /// <returns></returns>
    public bool IsFirstTimeFlag()
    {
        return m_Flags[(int)FLAGTYPE.FLAG_FIRSTTIME];
    }

    /// <summary>
    /// 月卡标记
    /// </summary>
    /// <returns></returns>
    public bool IsMonthCardFlag()
    {
        return m_Flags[(int)FLAGTYPE.FLAG_MONTHCARD];
    }

    /// <summary>
    /// 月卡今日是否已经领取标记
    /// </summary>
    /// <returns></returns>
    public bool IsMonthCardTodayFlag()
    {
        return m_Flags[(int)FLAGTYPE.FLAG_MONTHCARD_TODAY];
    }

    /// <summary>
    /// 成长标记
    /// </summary>
    /// <returns></returns>
    public bool IsGrowUpFlag()
    {
        return m_Flags[(int)FLAGTYPE.FLAG_GROWUP];
    }

    /// <summary>
    /// 40级成长礼包是否已领取
    /// </summary>
    /// <returns></returns>
    public bool IsGrowUp40Flag()
    {
        return m_Flags[(int)FLAGTYPE.FLAG_GROWUP_40];
    }

    /// <summary>
    /// 50级成长礼包是否已领取
    /// </summary>
    /// <returns></returns>
    public bool IsGrowUp50Flag()
    {
        return m_Flags[(int)FLAGTYPE.FLAG_GROWUP_50];
    }

    /// <summary>
    /// 60级成长礼包是否已领取
    /// </summary>
    /// <returns></returns>
    public bool IsGrowUp60Flag()
    {
        return m_Flags[(int)FLAGTYPE.FLAG_GROWUP_60];
    }

    /// <summary>
    /// 70级成长礼包是否已领取
    /// </summary>
    /// <returns></returns>
    public bool IsGrowUp70Flag()
    {
        return m_Flags[(int)FLAGTYPE.FLAG_GROWUP_70];
    }

    /// <summary>
    /// 80级成长礼包是否已领取
    /// </summary>
    /// <returns></returns>
    public bool IsGrowUp80Flag()
    {
        return m_Flags[(int)FLAGTYPE.FLAG_GROWUP_80];
    }

    /// <summary>
    /// 90级成长礼包是否已领取
    /// </summary>
    /// <returns></returns>
    public bool IsGrowUp90Flag()
    {
        return m_Flags[(int)FLAGTYPE.FLAG_GROWUP_90];
    }

    /// <summary>
    /// 首周是否完成
    /// </summary>
    /// <returns></returns>
    public bool IsFirstWeekOver()
    {
        if (m_StartServerDays >= 7 || m_StartServerDays < 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 月卡是否完成
    /// </summary>
    /// <returns></returns>
    public bool IsMonthCardOver()
    {
        if (m_Flags[(int)FLAGTYPE.FLAG_MONTHCARD] && m_MonthCardBeginTime != 0)
        {
            DateTime startTime = new DateTime(1970, 1, 1);
            //开始时间
            DateTime MonthCardBegin = new DateTime(startTime.Ticks + (long)m_MonthCardBeginTime * 10000000L, DateTimeKind.Unspecified);
            MonthCardBegin = MonthCardBegin.ToLocalTime();
            //当前时间
            DateTime NowData = new DateTime(startTime.Ticks + (long)GlobalData.ServerAnsiTime * 10000000L, DateTimeKind.Unspecified);
            NowData = NowData.ToLocalTime();
            //相差天数
            TimeSpan span = NowData.Subtract(MonthCardBegin);
            if (span.Days >= 29)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 成长基金是否完成
    /// </summary>
    /// <returns></returns>
    public bool IsGrowUpOver()
    {
        if (m_Flags[(int)FLAGTYPE.FLAG_GROWUP])
        {
            if (m_Flags[(int)FLAGTYPE.FLAG_GROWUP_40] &&
                m_Flags[(int)FLAGTYPE.FLAG_GROWUP_50] &&
                m_Flags[(int)FLAGTYPE.FLAG_GROWUP_60] &&
                m_Flags[(int)FLAGTYPE.FLAG_GROWUP_70] &&
                m_Flags[(int)FLAGTYPE.FLAG_GROWUP_80] &&
                m_Flags[(int)FLAGTYPE.FLAG_GROWUP_90])
            {
                return true;
            }
        }
        return false;
    }

    public float GetYBPrizeRate()
    {
        //UI开关关闭的时候 总显示1.0f
        if (GameManager.gameManager.PlayerDataPool.IsServerFlagOpen(SERVER_FLAGS_ENUM.FLAG_PAYACT) == false)
        {
            return 1.0f;
        }

        //首充最优先
        if (!IsFirstTimeFlag())
        {
            return 2.0f;
        }
        float result = 1.0f;
        //首周和不定时可以叠加显示
        if (!IsFirstWeekOver())
        {
            result = 2.0f;
        }
        //不定时返利
        if (m_RebateOpen)
        {
            result += m_RebateRate;
        }
        return result;
    }

    public void CleanUp()
    {
        m_Flags = new bool[(int)FLAGTYPE.FLAG_NUM];
        for (int i = 0; i < (int)FLAGTYPE.FLAG_NUM; i++)
        {
            m_Flags[i] = false;
        }
        m_MonthCardBeginTime = 0;
        m_MonthCardYBTotal = 0;
        m_StartServerDays = -1;
    }
    
    private bool[] m_Flags;
    //月卡开始时间
    private int m_MonthCardBeginTime;
    public int MonthCardBeginTime
    {
        get { return m_MonthCardBeginTime; }
        set { m_MonthCardBeginTime = value; }
    }
    //月卡累计领取元宝数
    private int m_MonthCardYBTotal;
    public int MonthCardYBTotal
    {
        get { return m_MonthCardYBTotal; }
        set { m_MonthCardYBTotal = value; }
    }
    //开服天数
    private int m_StartServerDays;
    public int StartServerDays
    {
        get { return m_StartServerDays; }
        set { m_StartServerDays = value; }
    }
    //不定时返利是否打开
    private bool m_RebateOpen;
    public bool RebateOpen
    {
        get { return m_RebateOpen; }
        set { m_RebateOpen = value; }
    }
    //不定时返利倍率
    private float m_RebateRate;
    public float RebateRate
    {
        get { return m_RebateRate; }
        set { m_RebateRate = value; }
    }

    public void HandlePacket(GC_SYNC_PAY_ACTIVITY_DATA packet)
    {
        //获取数据
        int flagCount = packet.flagsCount;
        for (int i = 0; i < flagCount && i < (int)FLAGTYPE.FLAG_NUM; i++)
        {
            m_Flags[i] = (packet.GetFlags(i)) > 0 ? true : false;
        }
        m_MonthCardBeginTime = packet.Monthcardbegin;
        m_MonthCardYBTotal = packet.Monthcardyb;
        m_StartServerDays = packet.Startserverdays;
        m_RebateOpen = (packet.Rebateopen == 1);
        m_RebateRate = (float)packet.Rebaterate / 10;

        if (RechargeController.Instance())
        {
            RechargeController.Instance().UpdateYBPrizeRate();
        }

        if (null != RechargeController.Instance())
        {
            RechargeController.Instance().UpdateRechargeList();
        }
    }

    public void HandlePacket(GC_ASK_PAY_ACTIVITY_PRIZE_RET packet)
    {

    }

    public void SendMonthCardPacket()
    {
        if (m_Flags[(int)FLAGTYPE.FLAG_MONTHCARD] == false)
        {
            return;
        }

        if (m_Flags[(int)FLAGTYPE.FLAG_MONTHCARD_TODAY])
        {
            return;
        }

        if (IsMonthCardOver())
        {
            return;
        }

        CG_ASK_PAY_ACTIVITY_PRIZE msg = (CG_ASK_PAY_ACTIVITY_PRIZE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_PAY_ACTIVITY_PRIZE);
        msg.SetPrizetype((int)CG_ASK_PAY_ACTIVITY_PRIZE.PrizeType.PRIZE_MONTHCARD);
        msg.SetPrizepram1(0);
        msg.SetPrizepram2(0);
        msg.SendPacket();
    }

    public void SendGrowUpPacket(int level)
    {
        if (m_Flags[(int)FLAGTYPE.FLAG_GROWUP] == false)
        {
            return;
        }

        if (IsGrowUpOver())
        {
            return;
        }

        if (Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.Level < level)
        {
            return;
        }

        CG_ASK_PAY_ACTIVITY_PRIZE msg = (CG_ASK_PAY_ACTIVITY_PRIZE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_PAY_ACTIVITY_PRIZE);
        msg.SetPrizetype((int)CG_ASK_PAY_ACTIVITY_PRIZE.PrizeType.PRIZE_GROWUP);
        msg.SetPrizepram1(level);
        msg.SetPrizepram2(0);
        msg.SendPacket();
    }
}