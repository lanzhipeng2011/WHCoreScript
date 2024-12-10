//********************************************************************
// 文件名: DailyLuckyDrawData.cs
// 描述: 每日幸运抽奖数据
// 作者: gaona
// 创建时间: 2014-4-18
// 功能说明：每日幸运抽奖活动数据，包括，获奖ID，抽奖类型，已抽中奖励标记位等
// 修改历史:
//********************************************************************

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace Games.DailyLuckyDraw
{
    public enum DLDDRAWTYPE
    {
        DLD_DRAWTYPE_ONE,
        DLD_DRAWTYPE_TEN,
        DLD_DRAWTYPE_ONE_YUANBAO,
    }
    public enum BONUSRAREDEGREE
    {
        DLD_BONUS_LOW=1,
        DLD_BONUS_MIDDLE,
        DLD_BONUS_HIGH,
        DLD_BONUS_VERYHIGH,
    }
    public class DailyLuckyDrawData
    {
        private int m_nDrawFreeTimes;
        public int DrawFreeTimes
        {
            get { return m_nDrawFreeTimes; }
            set { m_nDrawFreeTimes = value; }
        }
        private int m_nDrawFreeCDTime;
        public int DrawFreeCDTime
        {
            get { return m_nDrawFreeCDTime; }
            set { m_nDrawFreeCDTime = value; }
        }
        private float m_fTickTime;
        public float TickTime
        {
            get { return m_fTickTime; }
            set { m_fTickTime = value; }
        }
  
        public const int m_nMaxBonusBoxCount = 14;//有修改，需要同步修改DailyLuckyDrawLogic.cs
        public const int m_nMaxGainBonusCount = 10;//有修改，需要同步修改DailyLuckyDrawLogic.cs
        private int[] m_nGainBonusArray = new int[m_nMaxGainBonusCount];

        private int m_nDrawType;
		public  int DrawType
        {
            get { return m_nDrawType; }
            set { m_nDrawType = value; }
        }

        private bool m_bDrawing;
        public bool Drawing
        {
            get { return m_bDrawing; }
            set { m_bDrawing = value; }
        }
        public void CleanUp()
        {
            m_nDrawType = -1;
            m_nDrawFreeTimes = 0;
            m_bDrawing = false;

            CleanUpGainBonus();

        }
        public void CleanUpGainBonus()
        {
            for (int i = 0; i < m_nMaxGainBonusCount; i++)
            {
                m_nGainBonusArray[i] = -1;
            }
        }

        public DailyLuckyDrawData()
        {
            CleanUp();
        }

        public int GetMaxBonusBoxCount()
        {
            return m_nMaxBonusBoxCount;
        }
        public int GetMaxGainBonusCount()
        {
            return m_nMaxGainBonusCount;
        }
        public void HandlePacket(GC_DAILYLUCKYDRAW_GAINBONUS packet)
        {

            m_nDrawType = packet.Drawtype;

            CleanUpGainBonus();
            for (int i = 0; i < m_nMaxGainBonusCount && i < packet.gainbonusidCount; i++)
            {
                m_nGainBonusArray[i] = packet.GetGainbonusid(i);
            }

			if (null != DivinationLogic.Instance ()) 
			{
				DivinationLogic.Instance ().CloseDivination ();
			}

//			if (!BonusItemGetLogic.Instance())
//            {
//				BonusItemGetLogic.InitBonusInfo(BonusItemGetLogic.BONUSTYPE.TYPE_DAILYLUKCYDRAW);
//            }
			if (!DiviBonusRootLogic.Instance ()) 
			{
				DiviBonusRootLogic.InitDiviBonusInfo(true);
			}


			if (BackCamerControll.Instance() != null)
			{
				BackCamerControll.Instance().PlaySceneEffect(138);
			}

            m_bDrawing = false;

        }
        public void HandlePacket(GC_DAILYLUCKYDRAW_UPDATE packet)
        {
            if (packet.HasDrawfreetimes)
            {
                m_nDrawFreeTimes = packet.Drawfreetimes;
                //if(FunctionButtonLogic.Instance().)
            }
            if (packet.HasDrawfreecdtime)
            {
                m_nDrawFreeCDTime = packet.Drawfreecdtime;
            }

			if (DivinationLogic.Instance ()) 
			{
				DivinationLogic.Instance().CurDiviTFreeID = m_nDrawFreeTimes;
				DivinationLogic.Instance().DiviTCDTime = m_nDrawFreeTimes;
			}
//            if (DailyLuckyDrawLogic.Instance())
//            {
//                DailyLuckyDrawLogic.Instance().UpdateNumbers();
//            }
//
//            if (FunctionButtonLogic.Instance())
//            {
//                FunctionButtonLogic.Instance().UpdateDaliyLuckNum();
//                FunctionButtonLogic.Instance().UpdateButtonAwardTips();
//            }
        }
        public void HandlePacket(GC_DAILYLUCKYDRAW_FAIL packet)
        {
            if (DailyLuckyDrawLogic.Instance())
            {               
                DailyLuckyDrawLogic.Instance().HandleFailMsg(packet.Failreason);
            }
            m_bDrawing = false;
        }

        //获得奖励的ID
        public int GetGainBonusID(int nIndex)
        {
            if (nIndex >= 0 && nIndex < m_nGainBonusArray.Length)
            {
                return m_nGainBonusArray[nIndex];
            }
            return -1;
        }
        // 
        public void Tick_FreeCDTime()
        {
            TickTime += Time.fixedDeltaTime;
            if (TickTime < 1)
            {
                return;
            }
            if (DrawFreeCDTime > 0)
            {
                DrawFreeCDTime--;

				if(DivinationLogic.Instance())
				{
					DivinationLogic.Instance().DiviTCDTime = DrawFreeCDTime;
				}
//                if (DailyLuckyDrawLogic.Instance())
//                {
//                    DailyLuckyDrawLogic.Instance().UpdateNumbers();
//                }
//
//                if (FunctionButtonLogic.Instance())
//                {
//                    FunctionButtonLogic.Instance().UpdateDaliyLuckNum();
//                    FunctionButtonLogic.Instance().UpdateButtonAwardTips();
//                }
            }
            TickTime = 0;
        }
    }
}
