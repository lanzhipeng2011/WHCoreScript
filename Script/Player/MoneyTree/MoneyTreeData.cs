//********************************************************************
// 文件名: MoneyTreeData.cs
// 描述: 摇钱树数据
// 作者: HeWenpeng
// 创建时间: 2014-4-4
// 功能说明：摇钱树
// 修改历史:
//  2014-5-28 Lijia: 客户端效率优化，把MoneyTreeData改为struct
//********************************************************************

using UnityEngine;
using System.Collections;

namespace Games.MoneyTree
{
    public struct MoneyTreeData
    {
        public const int MaxFreeAwardNum = 8;
        // 在线奖励
        private int m_nMoneyTreeID;
        public int MoneyTreeID
        {
            get { return m_nMoneyTreeID; }
            set { 
                m_nMoneyTreeID = value;
				if(null != DivinationLogic.Instance())
				{
					DivinationLogic.Instance().CurDiviMFreeID = m_nMoneyTreeID;
				}
//                if (MoneyTreeLogic.Instance())
//                {
//                    MoneyTreeLogic.Instance().CurMoneyTreeID = m_nMoneyTreeID;
//                }
//                if (MoneyTreeButtonLogic.Instance())
//                {
//                    MoneyTreeButtonLogic.Instance().CurMoneyTreeID = m_nMoneyTreeID;
//                }
            }
        }
        private int m_nCDTime;
        public int CDTime
        {
            get { return m_nCDTime; }
            set { 
                m_nCDTime = value;

				if(null != DivinationLogic.Instance())
				{
					DivinationLogic.Instance().DiviMCDTime = m_nCDTime;
				}

//                if (MoneyTreeButtonLogic.Instance())
//                {
//                    MoneyTreeButtonLogic.Instance().CDTime = m_nCDTime;
//                }
//
//                if (MoneyTreeLogic.Instance())
//                {
//                    MoneyTreeLogic.Instance().CDTime = m_nCDTime;
//                }
            }
        }
        private float m_CurTimeCount; // 计时器

        private int m_YuanBaoAwardCount;    // 元宝消耗次数
        public int YuanBaoAwardCount
        {
            get { return m_YuanBaoAwardCount; }
            set {
                m_YuanBaoAwardCount = value;
                
				if(null != DivinationLogic.Instance())
				{
					DivinationLogic.Instance().CurDiviMBuyID = m_YuanBaoAwardCount;
				}

//                if (MoneyTreeLogic.Instance())
//                {
//                    MoneyTreeLogic.Instance().YuanBaoAwardCount = m_YuanBaoAwardCount;
//                }
            }
        }
        //public MoneyTreeData()
        //{
        //    CleanUp();
        //}

        public void CleanUp()
        {
            // 在线奖励
            m_nMoneyTreeID = -1;
            m_nCDTime = 0;
            m_CurTimeCount = 0;
            m_YuanBaoAwardCount = 0;
        }

        // 心跳，用于UI倒计时
        public void Tick_MoneyTreeAward()
        {
            m_CurTimeCount += Time.fixedDeltaTime;
            if (m_CurTimeCount <= 1 || CDTime <= 0)
            {
                return;
            }

            CDTime--;
            m_CurTimeCount = 0;
        }

        // 领奖，请求服务器
        public void SendAwardPacket(int nAwardCount, int nAwardType)
        {
            CG_MONEYTREE_ASKAWARD packet = (CG_MONEYTREE_ASKAWARD)PacketDistributed.CreatePacket(MessageID.PACKET_CG_MONEYTREE_ASKAWARD);
            packet.SetCurTurnID(m_nMoneyTreeID);
			packet.SetAwardCount((uint)nAwardCount);
			packet.SetAwardType((uint)nAwardType);
            packet.SendPacket();
        }

        public void HandlePacket(GC_MONEYTREE_DATA packet)
        {
            MoneyTreeID = packet.CurTurnID;
            CDTime = packet.AwardCDTime;
            int nRet = packet.Ret;
            YuanBaoAwardCount = packet.YuanBaoCount;

//            if (m_nMoneyTreeID < 0 || m_nMoneyTreeID > 20)
//            {
//                UIManager.CloseUI(UIInfo.MoneyTreeRoot);
//                if (FunctionButtonLogic.Instance())
//                {
//                    FunctionButtonLogic.Instance().UpdateMoneyTreeButton();
//                }
//            }

            // 通知UI按钮 显示提醒
            if (null != FunctionButtonLogic.Instance())
            {
                FunctionButtonLogic.Instance().UpdateButtonAwardTips();
            }

            if (nRet == 1)
            {
                if (BackCamerControll.Instance() != null)
                {
                    BackCamerControll.Instance().PlaySceneEffect(96);
                }

                if (null != GameManager.gameManager.SoundManager)
                {
                    GameManager.gameManager.SoundManager.PlaySoundEffect(129);    //pickup_coin
                }
            }
            else
            {
                if (FunctionButtonLogic.Instance())
                {
                    FunctionButtonLogic.Instance().UpdateMoneyTreeButton();
                }
            }

        }
    }
}

