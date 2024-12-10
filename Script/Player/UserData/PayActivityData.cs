/********************************************************************
	created:	2014/07/31
	created:	31:7:2014   16:43
	filename: 	PayActivityData.cs
	author:		Tangyi
	purpose:	��ֵ�����
*********************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GCGame.Table;
using Module.Log;
using System;

public enum FLAGTYPE
{
    //��ö���������ö�ٶ�Ӧ ��Ҫͬʱ�޸�
    FLAG_FIRSTTIME          = 0,        //�׳���� �Ƿ���ȡ
    FLAG_MONTHCARD          = 1,        //�¿���� �Ƿ���ȡ
    FLAG_MONTHCARD_TODAY    = 2,        //�¿���� �Ƿ���ȡ
    FLAG_GROWUP             = 3,        //�ɳ���� �Ƿ���ȡ��
    FLAG_GROWUP_40          = 4,        //�ɳ���� 40���Ƿ���ȡ
    FLAG_GROWUP_50          = 5,        //�ɳ���� 50���Ƿ���ȡ
    FLAG_GROWUP_60          = 6,        //�ɳ���� 60���Ƿ���ȡ
    FLAG_GROWUP_70          = 7,        //�ɳ���� 70���Ƿ���ȡ
    FLAG_GROWUP_80          = 8,        //�ɳ���� 80���Ƿ���ȡ
    FLAG_GROWUP_90          = 9,        //�ɳ���� 90���Ƿ���ȡ

    FLAG_NUM                = 16,
}

public struct PayActivityData 
{
    /// <summary>
    /// �׳���
    /// </summary>
    /// <returns></returns>
    public bool IsFirstTimeFlag()
    {
        return m_Flags[(int)FLAGTYPE.FLAG_FIRSTTIME];
    }

    /// <summary>
    /// �¿����
    /// </summary>
    /// <returns></returns>
    public bool IsMonthCardFlag()
    {
        return m_Flags[(int)FLAGTYPE.FLAG_MONTHCARD];
    }

    /// <summary>
    /// �¿������Ƿ��Ѿ���ȡ���
    /// </summary>
    /// <returns></returns>
    public bool IsMonthCardTodayFlag()
    {
        return m_Flags[(int)FLAGTYPE.FLAG_MONTHCARD_TODAY];
    }

    /// <summary>
    /// �ɳ����
    /// </summary>
    /// <returns></returns>
    public bool IsGrowUpFlag()
    {
        return m_Flags[(int)FLAGTYPE.FLAG_GROWUP];
    }

    /// <summary>
    /// 40���ɳ�����Ƿ�����ȡ
    /// </summary>
    /// <returns></returns>
    public bool IsGrowUp40Flag()
    {
        return m_Flags[(int)FLAGTYPE.FLAG_GROWUP_40];
    }

    /// <summary>
    /// 50���ɳ�����Ƿ�����ȡ
    /// </summary>
    /// <returns></returns>
    public bool IsGrowUp50Flag()
    {
        return m_Flags[(int)FLAGTYPE.FLAG_GROWUP_50];
    }

    /// <summary>
    /// 60���ɳ�����Ƿ�����ȡ
    /// </summary>
    /// <returns></returns>
    public bool IsGrowUp60Flag()
    {
        return m_Flags[(int)FLAGTYPE.FLAG_GROWUP_60];
    }

    /// <summary>
    /// 70���ɳ�����Ƿ�����ȡ
    /// </summary>
    /// <returns></returns>
    public bool IsGrowUp70Flag()
    {
        return m_Flags[(int)FLAGTYPE.FLAG_GROWUP_70];
    }

    /// <summary>
    /// 80���ɳ�����Ƿ�����ȡ
    /// </summary>
    /// <returns></returns>
    public bool IsGrowUp80Flag()
    {
        return m_Flags[(int)FLAGTYPE.FLAG_GROWUP_80];
    }

    /// <summary>
    /// 90���ɳ�����Ƿ�����ȡ
    /// </summary>
    /// <returns></returns>
    public bool IsGrowUp90Flag()
    {
        return m_Flags[(int)FLAGTYPE.FLAG_GROWUP_90];
    }

    /// <summary>
    /// �����Ƿ����
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
    /// �¿��Ƿ����
    /// </summary>
    /// <returns></returns>
    public bool IsMonthCardOver()
    {
        if (m_Flags[(int)FLAGTYPE.FLAG_MONTHCARD] && m_MonthCardBeginTime != 0)
        {
            DateTime startTime = new DateTime(1970, 1, 1);
            //��ʼʱ��
            DateTime MonthCardBegin = new DateTime(startTime.Ticks + (long)m_MonthCardBeginTime * 10000000L, DateTimeKind.Unspecified);
            MonthCardBegin = MonthCardBegin.ToLocalTime();
            //��ǰʱ��
            DateTime NowData = new DateTime(startTime.Ticks + (long)GlobalData.ServerAnsiTime * 10000000L, DateTimeKind.Unspecified);
            NowData = NowData.ToLocalTime();
            //�������
            TimeSpan span = NowData.Subtract(MonthCardBegin);
            if (span.Days >= 29)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// �ɳ������Ƿ����
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
        //UI���عرյ�ʱ�� ����ʾ1.0f
        if (GameManager.gameManager.PlayerDataPool.IsServerFlagOpen(SERVER_FLAGS_ENUM.FLAG_PAYACT) == false)
        {
            return 1.0f;
        }

        //�׳�������
        if (!IsFirstTimeFlag())
        {
            return 2.0f;
        }
        float result = 1.0f;
        //���ܺͲ���ʱ���Ե�����ʾ
        if (!IsFirstWeekOver())
        {
            result = 2.0f;
        }
        //����ʱ����
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
    //�¿���ʼʱ��
    private int m_MonthCardBeginTime;
    public int MonthCardBeginTime
    {
        get { return m_MonthCardBeginTime; }
        set { m_MonthCardBeginTime = value; }
    }
    //�¿��ۼ���ȡԪ����
    private int m_MonthCardYBTotal;
    public int MonthCardYBTotal
    {
        get { return m_MonthCardYBTotal; }
        set { m_MonthCardYBTotal = value; }
    }
    //��������
    private int m_StartServerDays;
    public int StartServerDays
    {
        get { return m_StartServerDays; }
        set { m_StartServerDays = value; }
    }
    //����ʱ�����Ƿ��
    private bool m_RebateOpen;
    public bool RebateOpen
    {
        get { return m_RebateOpen; }
        set { m_RebateOpen = value; }
    }
    //����ʱ��������
    private float m_RebateRate;
    public float RebateRate
    {
        get { return m_RebateRate; }
        set { m_RebateRate = value; }
    }

    public void HandlePacket(GC_SYNC_PAY_ACTIVITY_DATA packet)
    {
        //��ȡ����
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