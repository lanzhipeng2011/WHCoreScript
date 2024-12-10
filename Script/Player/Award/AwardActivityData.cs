//********************************************************************
// 文件名: AwardActivityData.cs
// 描述: 奖励活动数据
// 作者: HeWenpeng
// 创建时间: 2014-2-24
// 功能说明：包括新服奖励（首周）、每日登录奖励、在线奖励的活动数据
// 修改历史:
//         2014-4-18: 添加 活跃度领奖数据
//         2014-5-28 Lijia: 客户端效率优化，把AwardActivityData从class改为struct
//********************************************************************

using UnityEngine;
using System.Collections;
using GCGame.Table;
using System.Collections.Generic;

namespace Games.AwardActivity
{
    public enum AwardActivityType
    {
        AWARD_NONE,
        AWARD_NEWSERVER,
        AWARD_DAY,
        AWARD_ONLINE,
        AWARD_NEWONLINE,
        AWARD_NEW7DAYONLINE,
    }

    public struct OnlineAwardLine
    {
        public int ID;
        public int LeftTime;
        public int Exp;
        public int Money;
        public int BindYuanbao;
        public int Item1DataID;
        public int Item1Count;
        public int Item2DataID;
        public int Item2count;

        void CleanUp()
        {
            ID = -1;
            LeftTime = -1;
            Exp = -1;
            Money = -1;
            BindYuanbao = -1;
            Item1DataID = -1;
            Item1Count = -1;
            Item2DataID = -1;
            Item2count = -1;
        }
    }

    public struct AwardActivityData
    {
        // 新服奖励
        public const int MaxNewServerDays = 7;
        private int m_nNewServerDays;
        public int NewServerDays
        {
            get {return m_nNewServerDays; }
            set { m_nNewServerDays = value; }
        }

        private bool[] m_bNewServerAwardFlag;
        public bool GetNewServerAwardFlag(int nDay)
        {
            if (nDay >= 0 && nDay < MaxNewServerDays)
            {
                return m_bNewServerAwardFlag[nDay];
            }
            return false;
        }
        public bool IsCanGetAward(int nDay)
        {
            int nMaxRecordCount = TableManager.GetNewServerAward().Count;
            if (nDay < 0 || nDay >= nMaxRecordCount)
            {
                return false;
            }
            if (nDay == nMaxRecordCount - 1)
            {
                // 第7天的包 特殊处理，前面须领够4次，才能领包
                int nNeedCount = 0;
                for (int nIndex = 0; nIndex < nDay; nIndex++)
                {
                    if (GetNewServerAwardFlag(nIndex))
                    {
                        nNeedCount++;
                    }
                }
                if (nNeedCount >= 4 && false == GetNewServerAwardFlag(nDay))
                {
                    return true;
                }
            }
            else if (false == GetNewServerAwardFlag(nDay))
            {
                return true;
            }
            return false;
        }

        // 每日登录
        public const int MaxDayAwardDays = 7;
        private int m_nWeekDay;
        public int WeekDay
        {
            get { return m_nWeekDay; }
            set { m_nWeekDay = value; }
        }
        private bool m_bDayAwardFlag;
        public bool DayAwardFlag
        {
            get { return m_bDayAwardFlag; }
            set { m_bDayAwardFlag = value; }
        }

        // 在线奖励
        public const int MaxOnlineAwardCount = 7;
        private int m_nOnlineAwardID;
        public int OnlineAwardID
        {
            get { return m_nOnlineAwardID; }
            set { m_nOnlineAwardID = value; }
        }
		private int m_nLeftTime;
        public int LeftTime
        {
            get { return m_nLeftTime; }
            set { 
                m_nLeftTime = value; 
                if (AwardLogic.Instance())
                {
                    AwardLogic.Instance().LeftTime = m_nLeftTime;
                }
            }
        }
		//==========
//		private List<int> m_OnlineRewardIDList;
//		public List<int> OnlineRewardIDList
//		{
//			get { return m_OnlineRewardIDList; }
//			set { m_OnlineRewardIDList = value; }
//		}
//		private List<int> m_OnlineRewardStateList;
//		public List<int> OnlineRewardStateList
//		{
//			get { return m_OnlineRewardStateList; }
//			set { m_OnlineRewardStateList = value; }
//		}
		private int m_OnlineRewardTipNum;
		public int OnlineRewardTipNum
		{
			get { return m_OnlineRewardTipNum; }
			set { m_OnlineRewardTipNum = value; }
		}
		private List<int> m_LevelRewardIDList;
		public List<int> LevelRewardIDList
		{
			get { return m_LevelRewardIDList; }
			set { m_LevelRewardIDList = value; }
		}
		private List<int> m_LevelRewardStateList;
		public List<int> LevelRewardStateList
		{
			get { return m_LevelRewardStateList; }
			set { m_LevelRewardStateList = value; }
		}
		private int m_LevelRewardTipNum;
		public int LevelRewardTipNum
		{
			get { return m_LevelRewardTipNum; }
			set { m_LevelRewardTipNum = value; }
		}
		//==========
		private List<int> m_DL7RewardIDList;
		public List<int> DL7RewardIDList
		{
			get { return m_DL7RewardIDList; }
			set { m_DL7RewardIDList = value; }
		}
		private List<int> m_DL7RewardStateList;
		public List<int> DL7RewardStateList
		{
			get { return m_DL7RewardStateList; }
			set { m_DL7RewardStateList = value; }
		}
		private int m_DL7RewardTipNum;
		public int DL7RewardTipNum
		{
			get { return m_DL7RewardTipNum; }
			set { m_DL7RewardTipNum = value; }
		}


		//=============
		private int m_Monthcount;
		public int Monthcount
		{
			get { return m_Monthcount; }
			set { m_Monthcount = value; }
		}
		private int m_Monthmax;
		public int Monthmax
		{
			get { return m_Monthmax; }
			set { m_Monthmax = value; }
		}
		private int m_Flag;
		public int Flag
		{
			get { return m_Flag; }
			set { m_Flag = value; }
		}
		private List<int> m_DailyFlagList;
		public List<int> DailyFlagList
		{
			get { return m_DailyFlagList; }
			set { m_DailyFlagList = value; }
		}

		private int m_DailyRewardTipNum;
		public int DailyRewardTipNum
		{
			get { return m_DailyRewardTipNum; }
			set { m_DailyRewardTipNum = value; }
		}
		//==========


		//============
        private float m_CurTimeCount; // 计时器
        // 新开服在线奖励
        public const int MaxNewOnlineAwardCount = 7;
        private int m_nNewOnlineAwardID;
        public int NewOnlineAwardID
        {
            get { return m_nNewOnlineAwardID; }
            set { m_nNewOnlineAwardID = value; }
        }
        private int m_nNewLeftTime;
        public int NewLeftTime
        {
            get { return m_nNewLeftTime; }
            set
            {
                m_nNewLeftTime = value;
                if (AwardLogic.Instance())
                {
                    AwardLogic.Instance().NewLeftTime = m_nNewLeftTime;
                }
            }
        }
        public bool m_bNewOnlineAwardStart;
        public bool NewOnlineAwardStart
        {
            get { return m_bNewOnlineAwardStart; }
            set { m_bNewOnlineAwardStart = value; }
        }
        // 活跃度奖励
        private int m_nActiveness;  // 活跃度
        public int Activeness
        {
            get { return m_nActiveness; }
            set { 
                m_nActiveness = value;
                if (ActivityController.Instance())
                {
                    ActivityController.Instance().UpDateActiveness(m_nActiveness);
                }
                if (FunctionButtonLogic.Instance())
                {
                    FunctionButtonLogic.Instance().UpdateActionButtonTip();
                }
            }
        }
        public const int Activeness_Max_Count = 20;
        private bool[] m_bActivenessAwardFlag;

        //public AwardActivityData()
        //{
        //    CleanUp();
        //}

        public void CleanUp()
        {
            if (null == m_bNewServerAwardFlag)
            {
                m_bNewServerAwardFlag = new bool[MaxNewServerDays];
            }
            if (null == m_bActivenessAwardFlag)
            {
                m_bActivenessAwardFlag = new bool[Activeness_Max_Count];
            }
            // 新服奖励
            m_nNewServerDays = -1;
            for (int i = 0; i < MaxNewServerDays; i++)
            {
                m_bNewServerAwardFlag[i] = false;
            }

            // 每日登录
            m_nWeekDay = -1;
            m_bDayAwardFlag = false;

            // 在线奖励
            m_nOnlineAwardID = -1;
            m_nLeftTime = 0;
            m_CurTimeCount = 0;
            m_nNewOnlineAwardID = -1;
            m_bNewOnlineAwardStart = false;
            m_nNewLeftTime = 0;
            // 活跃度奖励
            m_nActiveness = 0;
            for (int i = 0; i < Activeness_Max_Count; i++)
            {
                m_bActivenessAwardFlag[i] = false;
            }
            // 在线7天奖励
            m_nNew7DayOnlineAwardID = -1;
            m_bNew7DayOnlineAwardStart = false;
            m_IsShowNew7DayOnlineAwardWindow = false;
            m_nNew7DayLeftTime = 0;
            m_New7DayOnlineAwardTable = new Dictionary<int, OnlineAwardLine>();
        }

        // 心跳，用于UI倒计时
        public void Tick_Award()
        {
            m_CurTimeCount += Time.fixedDeltaTime;
            if (m_CurTimeCount < 1)
            {
                return;
            }
            if (LeftTime > 0)
            {
                LeftTime--;
            }
            if (NewLeftTime > 0)
            {
                NewLeftTime--;
            }
            if (New7DayLeftTime > 0)
            {
                New7DayLeftTime--;
            }
            m_CurTimeCount = 0;
        }

        // 领奖，请求服务器
        public void SendAwardPacket(AwardActivityType type)
        {
            switch(type)
            {
                case AwardActivityType.AWARD_NEWSERVER:
                    {
                        CG_ASK_NEWSERVERAWARD packet = (CG_ASK_NEWSERVERAWARD)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_NEWSERVERAWARD);
                        packet.SetDay(m_nNewServerDays);
                        packet.SendPacket();
                    }
                    break;
                case AwardActivityType.AWARD_DAY:
                    {
                        CG_ASK_DAYAWARD packet = (CG_ASK_DAYAWARD)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_DAYAWARD);
                        packet.SetWeekDay((uint)m_nWeekDay);
                        packet.SendPacket();
                     }
                    break;
                case AwardActivityType.AWARD_ONLINE:
                    {
                        CG_ASK_ONLINEAWARD packet = (CG_ASK_ONLINEAWARD)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_ONLINEAWARD);
                        packet.SetOnlineAwardID((uint)m_nOnlineAwardID);
                        packet.SendPacket();
                    }
                    break;
                case AwardActivityType.AWARD_NEWONLINE:
                    {
                        CG_ASK_NEWONLINEAWARD packet = (CG_ASK_NEWONLINEAWARD)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_NEWONLINEAWARD);
                        packet.SetNewOnlineAwardID(m_nNewOnlineAwardID);
                        packet.SendPacket();
                    }
                    break;
                case AwardActivityType.AWARD_NEW7DAYONLINE:
                    {
                        CG_ASK_NEW7DAYONLINEAWARD packet = (CG_ASK_NEW7DAYONLINEAWARD)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_NEW7DAYONLINEAWARD);
                        packet.SetNewOnlineAwardID(m_nNew7DayOnlineAwardID);
                        packet.SendPacket();
                    }
                    break;
                default:
                    break;
            }
        }
		public void HandlePacket(GC_LEVELAWARD_DATA packet)
		{

			m_LevelRewardIDList = new List<int> ();
			m_LevelRewardStateList = new List<int> ();
			m_LevelRewardTipNum = 0;
			for(int i = 0;i<packet.awardidCount;i++)
			{
				m_LevelRewardIDList.Add(packet.awardidList[i]);
				m_LevelRewardStateList.Add(packet.flagList[i]);

				if(packet.flagList[i] == 2)
				{
					m_LevelRewardTipNum++;
				}
			}
			
			// 通知UI按钮 显示提醒
			if (null != FunctionButtonLogic.Instance())
			{
				FunctionButtonLogic.Instance().UpdateButtonAwardTips();
			}
			
			if (null != SGRewardRootManager.Instance())
			{
				if (SGRewardRootManager.Instance().m_DayAwardRoot.gameObject.activeSelf)
				{
					SGRewardRootManager.Instance().m_DayAwardRoot.ButtonDayAward();
				}
			}
			if (null != SGRewardRootManager.Instance())
			{
				SGRewardRootManager.Instance().UpdateTip();
			}
		}
		//=====DL7Day
		public void HandlePacket(GC_7DAYAWARD_DATA packet)
		{

			m_DL7RewardIDList = new List<int> ();
			m_DL7RewardStateList = new List<int> ();
			m_DL7RewardTipNum = 0;
			for(int i = 0;i<packet.awardidCount;i++)
			{
				m_DL7RewardIDList.Add(packet.awardidList[i]);
				m_DL7RewardStateList.Add(packet.flagList[i]);
				
				if(packet.flagList[i] == 2)
				{
					m_DL7RewardTipNum++;
				}
			}
			
			// 通知UI按钮 显示提醒
			if (null != FunctionButtonLogic.Instance())
			{
				FunctionButtonLogic.Instance().UpdateButtonAwardTips();
			}
			
			if (null != SGRewardRootManager.Instance())
			{
				if (SGRewardRootManager.Instance().m_DL7DayAwardRoot.gameObject.activeSelf)
				{
					SGRewardRootManager.Instance().m_DL7DayAwardRoot.ButtonDL7DayAward();
				}
			}
			if (null != SGRewardRootManager.Instance())
			{
				SGRewardRootManager.Instance().UpdateTip();
			}

		}

        public void HandlePacket(GC_NEWSERVERAWARD_DATA packet)
        {
            m_nNewServerDays = packet.Day;
            for (int i = 0; i < packet.flagCount; i++ )
            {
                int nFlag = packet.GetFlag(i);
                if (nFlag == 0)
                {
                    m_bNewServerAwardFlag[i] = false;
                }
                else
                    m_bNewServerAwardFlag[i] = true;
            }
            
            // 通知UI按钮 显示提醒
            if (null != FunctionButtonLogic.Instance())
            {
                FunctionButtonLogic.Instance().UpdateButtonAwardTips();
            }

            if (null != AwardLogic.Instance())
            {
                AwardLogic.Instance().UpdateTip();
                if (AwardLogic.Instance().m_NewServerAwardRoot.gameObject.activeSelf)
                {
                    AwardLogic.Instance().m_NewServerAwardRoot.ButtonNewServerAward();
                    AwardLogic.Instance().m_NewServerAwardRoot.PlayEffect(m_nNewServerDays);
                }
            }

        }

        public void HandlePacket(GC_DAYAWARD_DATA packet)
        {
            m_nWeekDay = packet.WeekDay;
            int nFlag = packet.Flag;
            if (nFlag == 0)
            {
                m_bDayAwardFlag = false;
            }
            else
                m_bDayAwardFlag = true;

            // 通知UI按钮 显示提醒
            if (null != FunctionButtonLogic.Instance())
            {
                FunctionButtonLogic.Instance().UpdateButtonAwardTips();
            }

            if (null != AwardLogic.Instance())
            {
                AwardLogic.Instance().UpdateTip();
                if (AwardLogic.Instance().m_DayAwardRoot.gameObject.activeSelf)
                {
                    AwardLogic.Instance().m_DayAwardRoot.ButtonDayAward();
                    AwardLogic.Instance().m_DayAwardRoot.PlayEffect(m_nWeekDay, m_bDayAwardFlag);
                }
            }
        }

		//===========
		public void HandlePacket(GC_DAILYAWARD_DATA packet)
		{

			m_Monthcount = packet.Monthcount;
			m_Monthmax = packet.Monthmax;
			m_Flag = packet.Flag;

			m_DailyRewardTipNum = m_Monthmax - m_Monthcount;

			m_DailyFlagList = new List<int> ();
			for(int i = 0;i<packet.dailyflagCount;i++)
			{
				m_DailyFlagList.Add(packet.dailyflagList[i]);
			}

			// 通知UI按钮 显示提醒
			if (null != FunctionButtonLogic.Instance())
			{
				FunctionButtonLogic.Instance().UpdateButtonAwardTips();
			}
			if (null != SGRewardRootManager.Instance())
			{
				if (SGRewardRootManager.Instance().m_RegDayAwardRoot.gameObject.activeSelf)
				{
					SGRewardRootManager.Instance().m_RegDayAwardRoot.ButtonRegDayAward();
				}
			}
			if (null != SGRewardRootManager.Instance())
			{
				SGRewardRootManager.Instance().UpdateTip();
			}
//			
//			if (null != AwardLogic.Instance())
//			{
//				AwardLogic.Instance().UpdateTip();
//				if (AwardLogic.Instance().m_DayAwardRoot.gameObject.activeSelf)
//				{
//					AwardLogic.Instance().m_DayAwardRoot.ButtonDayAward();
//					AwardLogic.Instance().m_DayAwardRoot.PlayEffect(m_nWeekDay, m_bDayAwardFlag);
//				}
//			}
		}
		//===========

        public void HandlePacket(GC_ONLINEAWARD_DATA packet)
        {
            m_nOnlineAwardID = packet.OnlineAwardID;
//			m_OnlineRewardIDList = new List<int> ();
//			m_OnlineRewardStateList = new List<int> ();
//			m_OnlineRewardTipNum = 0;
//			for(int i = 0;i<packet.OnlineAwardIDCount;i++)
//			{
//				m_OnlineRewardIDList.Add(packet.OnlineAwardIDList[i]);
//				m_OnlineRewardStateList.Add(packet.flagList[i]);
//				if(packet.flagList[i] == 2)
//				{
//					m_OnlineRewardTipNum++;
//				}
//			}

			m_nLeftTime = packet.LeftTime;
			if(m_nLeftTime == 0 && m_nOnlineAwardID != 0)
			{
				m_OnlineRewardTipNum = 1;
			}

            // 通知UI按钮 显示提醒
            if (null != FunctionButtonLogic.Instance())
            {
                FunctionButtonLogic.Instance().UpdateButtonAwardTips();
            }

			if (null != SGRewardRootManager.Instance())
			{
				if (SGRewardRootManager.Instance().m_OnlineAwardRoot.gameObject.activeSelf)
				{
					SGRewardRootManager.Instance().m_OnlineAwardRoot.ButtonOnlineAward();
//					SGRewardRootManager.Instance().m_OnlineAwardRoot.PlayEffect(m_nOnlineAwardID);
				}
			}
			if (null != SGRewardRootManager.Instance())
			{
				SGRewardRootManager.Instance().UpdateTip();
			}
//            if (null != AwardLogic.Instance())
//            {
//                AwardLogic.Instance().UpdateTip();
//                if (AwardLogic.Instance().m_OnlineAwardRoot.gameObject.activeSelf)
//                {
//                    AwardLogic.Instance().m_OnlineAwardRoot.ButtonOnlineAward();
//                    AwardLogic.Instance().m_OnlineAwardRoot.PlayEffect(m_nOnlineAwardID);
//                }
//            }
			if (FunctionButtonLogic.Instance())
			{
				FunctionButtonLogic.Instance().UpdateRewardButtonTip();
			}

        }
        public void HandlePacket(GC_NEWONLINEAWARD_DATA packet)
        {
            m_nNewOnlineAwardID = packet.NewOnlineAwardID;
            NewLeftTime = packet.NewleftTime;
            m_bNewOnlineAwardStart = packet.IsStart > 0;
            // 通知UI按钮 显示提醒
            if (null != FunctionButtonLogic.Instance())
            {
                FunctionButtonLogic.Instance().UpdateButtonAwardTips();
            }

            if (null != AwardLogic.Instance())
            {
                AwardLogic.Instance().UpdateTip();
                if (AwardLogic.Instance().m_NewOnlineAwardRoot.gameObject.activeSelf)
                {
                    AwardLogic.Instance().m_NewOnlineAwardRoot.ButtonOnlineAward();
                    AwardLogic.Instance().m_NewOnlineAwardRoot.PlayEffect(m_nNewOnlineAwardID);
                }
            }
        }

        // 活跃度奖励
        void SetActivenessFlag(int nTurnID, bool bFlag)
        {
            if (nTurnID >= 0 && nTurnID < Activeness_Max_Count)
            {
                m_bActivenessAwardFlag[nTurnID] = bFlag;
            }
        }
        public bool GetActivenessAwardFlag(int nTurnID)
        {
            if (nTurnID >= 0 && nTurnID < Activeness_Max_Count)
            {
                return m_bActivenessAwardFlag[nTurnID];
            }
            return false;
        }
        public void SendActivenessAward(int nTurnID)
        {
            CG_ASK_ACTIVENESSAWARD packet = (CG_ASK_ACTIVENESSAWARD)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_ACTIVENESSAWARD);
			packet.SetTurnid((uint)nTurnID);
            packet.SendPacket();
        }
        public void HandlePacket(GC_ASK_ACTIVENESSAWARD_RET packet)
        {
            int nTurnID = packet.Turnid;
            bool bFlag = (packet.Flag==0? false:true);
            SetActivenessFlag(nTurnID, bFlag);

            // 通知UI更新
            if (ActivityController.Instance())
            {
                ActivityController.Instance().UpdateAwardItemState(nTurnID);
                ActivityController.Instance().UpdateTabTips();
            }
            if (FunctionButtonLogic.Instance())
            {
                FunctionButtonLogic.Instance().UpdateActionButtonTip();
            }
        }
        public void HandlePacket(GC_SYNC_ACTIVENESSAWARD packet)
        {
            for (int i = 0; i < packet.flagCount; i++)
            {
                bool bFlag = (packet.GetFlag(i) == 0? false:true);
                SetActivenessFlag(i, bFlag);
            }
        }
        public void HandlePacket(GC_SYNC_ACTIVENESS packet)
        {
            Activeness = packet.Activeness;
        }
        //在线7天奖励
        // 开服奖励 表格数据--用于动态加载
        public const int MaxNew7DayOnlineAwardCount = 7;

        private bool m_IsShowNew7DayOnlineAwardWindow;
        public bool ShowNew7DayOnlineAwardWindow
        {
            get { return m_IsShowNew7DayOnlineAwardWindow; }
            set { m_IsShowNew7DayOnlineAwardWindow = value; }
        }
       
        private int m_nNew7DayOnlineAwardID;
        public int New7DayOnlineAwardID
        {
            get { return m_nNew7DayOnlineAwardID; }
            set { m_nNew7DayOnlineAwardID = value; }
        }
        private int m_nNew7DayLeftTime;
        public int New7DayLeftTime
        {
            get { return m_nNew7DayLeftTime; }
            set
            {
                m_nNew7DayLeftTime = value;
                if (AwardLogic.Instance())
                {
                    AwardLogic.Instance().New7DayLeftTime = m_nNew7DayLeftTime;
                }
            }
        }
        public bool m_bNew7DayOnlineAwardStart;
        public bool New7DayOnlineAwardStart
        {
            get { return m_bNew7DayOnlineAwardStart; }
            set { m_bNew7DayOnlineAwardStart = value; }
        }
        private Dictionary<int, OnlineAwardLine> m_New7DayOnlineAwardTable;
        public Dictionary<int, OnlineAwardLine> New7DayOnlineAwardTable
        {
            get { return m_New7DayOnlineAwardTable; }
            set { m_New7DayOnlineAwardTable = value; }
        }
        public void AddNew7DayOnlineAwardLine(OnlineAwardLine DataLine)
        {
            if (m_New7DayOnlineAwardTable.ContainsKey(DataLine.ID))
            {
                m_New7DayOnlineAwardTable[DataLine.ID] = DataLine;
            }
            else
            {
                m_New7DayOnlineAwardTable.Add(DataLine.ID, DataLine);
            }
        }
        private NewOnlineDateTime m_sNew7DayOnlineDateTime;
        public NewOnlineDateTime New7DayOnlineDateTime
        {
            get { return m_sNew7DayOnlineDateTime; }
            set { m_sNew7DayOnlineDateTime = value; }
        }
        public void HandlePacket(GC_NEW7DAYONLINEAWARD_DATA packet)
        {
            m_nNew7DayOnlineAwardID = packet.NewOnlineAwardID;
            New7DayLeftTime = packet.NewleftTime;
            m_bNew7DayOnlineAwardStart = packet.IsStart > 0;
            // 通知UI按钮 显示提醒
            if (null != FunctionButtonLogic.Instance())
            {
                FunctionButtonLogic.Instance().UpdateButtonAwardTips();
            }
            if (null != AwardLogic.Instance())
            {
                AwardLogic.Instance().UpdateTip();
                if (AwardLogic.Instance().m_New7DayOnlineAwardRoot.gameObject.activeSelf)
                {
                    AwardLogic.Instance().m_New7DayOnlineAwardRoot.ButtonOnlineAward();
                    AwardLogic.Instance().m_New7DayOnlineAwardRoot.PlayEffect(m_nNew7DayOnlineAwardID);
                }
            }
        }
        public void HandlePacket(GC_SYNC_NEW7DAYONLINEAWARDTABLE packet)
        {
             bool isShow = packet.IsShow > 0;
             ShowNew7DayOnlineAwardWindow = isShow;
             if (isShow)
             {
                 for (int i = 0; i < packet.idCount; i++)
                 {
                     OnlineAwardLine DataLine = new OnlineAwardLine();
                     DataLine.ID = packet.GetId(i);
                     DataLine.LeftTime = packet.GetLefttime(i);
                     DataLine.Exp = packet.GetExp(i);
                     DataLine.Money = packet.GetMoney(i);
                     DataLine.BindYuanbao = packet.GetBindyuanbao(i);
                     DataLine.Item1DataID = packet.GetItem1dataid(i);
                     DataLine.Item1Count = packet.GetItem1count(i);
                     DataLine.Item2DataID = packet.GetItem2dataid(i);
                     DataLine.Item2count = packet.GetItem2count(i);
                     AddNew7DayOnlineAwardLine(DataLine);

                     if (packet.HasStartDate)
                     {
                         m_sNew7DayOnlineDateTime.StartDate = packet.StartDate;
                     }
                     if (packet.HasEndDate)
                     {
                         m_sNew7DayOnlineDateTime.EndDate = packet.EndDate;
                     }
                     if (packet.HasStartTime)
                     {
                         m_sNew7DayOnlineDateTime.StartTime = packet.StartTime;
                     }
                     if (packet.HasEndTime)
                     {
                         m_sNew7DayOnlineDateTime.EndTime = packet.EndTime;
                     }
                 }
             }
        }
    }
    public struct NewOnlineDateTime
    {
        public int StartDate;
        public int EndDate;
        public int StartTime;
        public int EndTime;

        public void CleanUp()
        {
            StartDate = 0;
            EndDate = 0;
            StartTime = 0;
            EndTime = 0;
        }
    }
}

