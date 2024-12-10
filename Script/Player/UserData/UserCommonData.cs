using UnityEngine;
using System.Collections;
using Module.Log;
using System.Collections.Generic;
namespace Games.UserCommonData
{
    enum USER_COMMONDATA
    {
        CD_COPYSCENE_QUITTIME               = 3,
        CD_COPYSCENE_NUMBER3                = 5,
        CD_COPYSCENE_NUMBER4                = 6,
        CD_CONSIGNSALE                      = 7,
        CD_COPYSCENE_CANGJINGGE_TIER        = 9,
        CD_COPYSCENE_CANGJINGGE_SWEEP       = 10,
        CD_STAMINA_BUYNUM                   = 13,
        CD_RESTAURANT_FINISHFOOD_NUM        = 14,
        CD_DAILYLUCKYDRAW_CDTIME            = 15,
        CD_NOTICE_DATA                      = 16,
        CD_ORIGINAL_NUMBER                  = 17,
        CD_CJG_SWEEP_CD                     = 18,
		CD_SNS_REWARD_NUMBER				= 19,
        CD_SHARE_NANGUA_CODE_REWARD_COUNT   = 32,
        CD_VIPCP_JXZ                        = 34,
        
        CD_VIPCP_YZW                        = 36,
        CD_VIPCP_ZLQJ                       = 37,
        CD_VIPCP_YWGM                       = 38,
        CD_VIPCP_NHCJ                       = 39,
        CD_VIPCP_SSS                        = 40,

        //已经跑商次数
        CD_GUILDBUSINESS_GOTTEN_NUM         = 97,
        CD_GUILDBUSINESS_ROB_CION           = 98,
        //跑商复活
        CD_GUILDBUSINESS_FUHUOTIMES         = 99,

        CD_MAX_NUM_DATA = 128, // 存储边界 上面添加
    }       

    enum USER_COMMONFLAG
    {
		CF_GEM_IPENFLAG = 14,
        CF_FELLOWFUNCTION_OPENFLAG = 20,            // 伙伴开启
        CF_BELLEFUNCTION_OPENFLAG = 21,             // 美人开启
        CF_ACTIVITYFUNCTION_OPENFLAG = 22,          // 活动开启
        CF_GUILDACTIVITY_FLAG = 23,                 // 帮会日常活动标记
        CF_STRENGTHENFUNCTION_OPENFLAG = 24,        // 强化开启
        CF_RESTAURANTFUNCTION_OPENFLAG = 25,        // 酒楼开启
        CF_GUILDFUNCTION_OPENFLAG = 26,             // 帮会功能开启标记
        CF_XIAKEFUNCTION_OPENFLAG = 27,             // 侠客功能开启标记
        CF_MISSION_DAILYMISSION_FLAG = 28,          //每日任务 完成标记
        COUNTER_DB_OPEN_MASTER = 29,                // 师门
        COUNTER_DB_OPEN_RELATION = 30,              // 好友
        COUNTER_DB_OPEN_SHOP = 31,                  // 商城
        COUNTER_DB_OPEN_MAIL = 32,                  // 邮件
        COUNTER_DB_OPEN_GEM = 33,                   // 镶嵌
        COUNTER_DB_OPEN_QIANKUNDAI = 34,            // 九黎壶
		COUNTER_DB_OPEN_MAKE = 35,                  // 制造
		COUNTER_DB_OPEN_GUAJI = 36,                  // 挂机

		CF_FIRSTCONST = 37,//首冲标记
		CF_WEEKFLAG = 38,//周卡月卡
		CF_MONTH = 39,//周卡月卡

        CF_ACTIVITY_SINGLEDAY_FLAG = 100,          //光棍节活动标记
        CF_ACTIVITY_THANKSGIVINGDAYFLAG = 101,     //感恩节活动标记
        CF_ACTIVITY_THANKSMISSIONERYDAYFLAG = 102,//感恩节活动任务标记
        CF_ACTIVITY_SINGLEDAYMISSION_FLAG = 103,   //光棍节活动任务标记
        CF_ACTIVITY_HALLOWEEN_FLAG = 104,   //万圣节活动任务标记
        CF_CYFANS_AWARD_FLAG = 105,      //畅游老玩家反馈
        CF_ISOPENKILLNPCEXP = 106,                // 是否提示杀怪经验
        CF_SNS_DAILY_REWARD = 107,                   //SNS每日奖励
        CF_MAX_NUM_FLAG = 160,  // 存储边界 上面添加
    }

    public class UserCommonData
    {
        public const int MAX_CHAR_COMMON_DATA_NUM = 128;
        public const int MAX_CHAR_COMMON_FLAG_NUM = 5;
        public const int MAX_COPY_DAY_NUMBER_NUM = 16;

        private int[] m_CommonData;
        private uint[] m_CommonFlag;
       public  struct CopySceneDayNumber
        {
            public int m_nDayCount;
            public int m_ndayTeamCount;
            public int m_nTotalCount1;
            public int m_nTotalCount2;
            public int m_nTotalCount3;
            public int m_nTeamTotalCount1;
            public int m_nTeamTotalCount2;
            public int m_nTeamTotalCount3;
            public int m_nMultiple;
            public int m_nResetCount;
            public int m_nResetTeamCount;
            public int m_nExtraDayCount;
            public int m_nExtradayTeamCount;
        };
        private Dictionary<int, CopySceneDayNumber> m_CopySceneDayNumber;
        public Dictionary<int, CopySceneDayNumber> CopySceneDayNumbers  
        {
            get { return m_CopySceneDayNumber; } 
        }
        public UserCommonData()
        {
            m_CommonData = new int[MAX_CHAR_COMMON_DATA_NUM];
            m_CommonFlag = new uint[MAX_CHAR_COMMON_FLAG_NUM];
            m_CopySceneDayNumber = new Dictionary<int, CopySceneDayNumber>();
        }
        public void ClearData()
        {
            if (m_CommonData != null)
            {
                for (int i = 0; i < MAX_CHAR_COMMON_DATA_NUM; i++)
                {
                    m_CommonData[i] = 0;
                }
            }
            if (m_CommonFlag != null)
            {
                for (int i = 0; i < MAX_CHAR_COMMON_FLAG_NUM; i++)
                {
                    m_CommonFlag[i] = 0;
                }
            }
            if (m_CopySceneDayNumber != null)
            {
                m_CopySceneDayNumber.Clear();
            }            
        }

        void SetCommonData(int nIndex, int nValue)
        {
            if (nIndex < 0 || nIndex >= MAX_CHAR_COMMON_DATA_NUM)
            {
                LogModule.DebugLog("SetCommonData: Index out of Range!!!");
                return;
            }
            int nData = m_CommonData[nIndex];
            m_CommonData[nIndex] = nValue;
            
            // 特殊添加 其他 更改 加在 OnCommonDataChange()中
            if (nIndex == (int)Games.UserCommonData.USER_COMMONDATA.CD_COPYSCENE_CANGJINGGE_SWEEP)
            {
                if (ActivityController.Instance() != null)
                {
                    ActivityController.Instance().UpdateTabTips();
                }
                if (FunctionButtonLogic.Instance())
                {
                    FunctionButtonLogic.Instance().UpdateActionButtonTip();
                }
                if (CangJingGeWindow.Instance() != null)
                {
                    CangJingGeWindow.Instance().UpdateInfo();
                    if (nData < nValue)//开始扫荡
                    {
                        CangJingGeWindow.Instance().StartSweep();
                    }
                }
            }
            // 更新 操作
            OnCommonDataChange(nIndex, nValue);
        }

        public int GetCommonData(int nIndex)
        {
            if (nIndex < 0 || nIndex >= MAX_CHAR_COMMON_DATA_NUM)
            {
                LogModule.DebugLog("GetCommonData: Index out of Range!!!");
                return -1;
            }

            return m_CommonData[nIndex];
        }

        void OnCommonDataChange(int nIndex, int nValue)
        {
            if (nIndex == (int)Games.UserCommonData.USER_COMMONDATA.CD_COPYSCENE_QUITTIME)
            {
                if (FunctionButtonLogic.Instance() != null)
                {
                    FunctionButtonLogic.Instance().ExitTime = nValue;
                }
            }
            else if (nIndex == (int)Games.UserCommonData.USER_COMMONDATA.CD_CJG_SWEEP_CD)
            {
                GameManager.gameManager.PlayerDataPool.CJGSweepCDTime = nValue;               
            }
            else if ( nIndex == (int)Games.UserCommonData.USER_COMMONDATA.CD_VIPCP_JXZ
                   || nIndex == (int)Games.UserCommonData.USER_COMMONDATA.CD_VIPCP_YZW
                   || nIndex == (int)Games.UserCommonData.USER_COMMONDATA.CD_VIPCP_ZLQJ
                   || nIndex == (int)Games.UserCommonData.USER_COMMONDATA.CD_VIPCP_YWGM
                   || nIndex == (int)Games.UserCommonData.USER_COMMONDATA.CD_VIPCP_NHCJ
                   || nIndex == (int)Games.UserCommonData.USER_COMMONDATA.CD_VIPCP_SSS)
            {
                if (DungeonWindow.Instance() != null)
                {
                    DungeonWindow.Instance().UpdateTabInfo();
                }
            }
            else if (nIndex == (int)Games.UserCommonData.USER_COMMONDATA.CD_SHARE_NANGUA_CODE_REWARD_COUNT)
            {
               if (ShareRootWindow.Instance() != null)
               {
                   ShareRootWindow.Instance().UpdateRewardCount();
               }
            }
        }

        void SetCommonFlag(int nBits, bool bFlag)
        {
            if (nBits < 0 || nBits >= MAX_CHAR_COMMON_FLAG_NUM*8)
            {
                LogModule.DebugLog("SetCommonFlag: Index out of Range!!!");
                return;
            }

            int nIndex = nBits / (sizeof(int) * 8);
            if (nIndex >= 0 && nIndex < MAX_CHAR_COMMON_FLAG_NUM)
            {
                //int nOldBits = nBits;
                nBits = nBits % (sizeof(int) * 8);// 0-31
                if (nBits >= 0 && nBits <= 31)
                {
                    uint nDataValue = m_CommonFlag[nIndex];
                    if (bFlag != false)
                    {
                        nDataValue |= (uint)(1 << nBits);
                    }
                    else
                    {
                        nDataValue &= (uint)~(1 << nBits);
                    }

                    m_CommonFlag[nIndex] = nDataValue;
                }
                
            }

            // 更新后操作
            OnCommonFlagChange(nBits);
        }

        public bool GetCommondFlag(int nBits)
        {
            if (nBits < 0 || nBits >= sizeof(int)*MAX_CHAR_COMMON_FLAG_NUM*8)
            {
                LogModule.DebugLog("GetCommondFlag: Index out of Range!!!");
                return false;
            }
            int nIndex = nBits / (sizeof(int) * 8);
            if (nIndex >=0 && nIndex < MAX_CHAR_COMMON_FLAG_NUM)
            {
                nBits = nBits % (sizeof(int) * 8);
                if (nBits >= 0 && nBits <= 31)
                {
                    uint nDataValue = m_CommonFlag[nIndex];
                    uint nRet = (uint)(nDataValue & (1 << nBits));
                    return ((nRet > 0) ? true : false);
                }
            }
            return false;
        }

        void OnCommonFlagChange(int nBit)
        {
            if (nBit == (int)USER_COMMONFLAG.CF_GUILDACTIVITY_FLAG)
            {
                // 更新界面
                if (FunctionButtonLogic.Instance())
                {
                    FunctionButtonLogic.Instance().UpdateActionButtonTip();
                }

                if (ActivityController.Instance())
                {
                    ActivityController.Instance().UpdateGuildActivityWindow();
                }
            }
        }

        public void AskSetCommonFlag(int nBits, bool bFlag)
        {
            int nFlag = (bFlag == true ? 1 : 0);
            CG_ASK_SETCOMMONFLAG askPacket = (CG_ASK_SETCOMMONFLAG)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_SETCOMMONFLAG);
            askPacket.SetNBits(nBits);
            askPacket.SetNFlag(nFlag);
            askPacket.SendPacket();
        }
        void SetCopySceneNumber(int nSceneClassID, CopySceneDayNumber copyInfo/*  int nDayCount, int nDayTeamCount, int nTotalCount1, int nTotalCount2, int nTotalCount3, int nTeamTotalCount1, int nTeamTotalCount2, int nTeamTotalCount3, int nMultiple*/)
        {
            CopySceneDayNumber info;
            info = copyInfo;
//             info.m_nDayCount = copyInfo.nDayCount;
//             info.m_ndayTeamCount = nDayTeamCount;
//             info.m_nTotalCount1 = nTotalCount1;
//             info.m_nTotalCount2 = nTotalCount2;
//             info.m_nTotalCount3 = nTotalCount3;
//             info.m_nTeamTotalCount1 = nTeamTotalCount1;
//             info.m_nTeamTotalCount2 = nTeamTotalCount2;
//             info.m_nTeamTotalCount3 = nTeamTotalCount3;
//             info.m_nMultiple = nMultiple;
            if (m_CopySceneDayNumber.ContainsKey(nSceneClassID))
            {
                info.m_nExtraDayCount = m_CopySceneDayNumber[nSceneClassID].m_nExtraDayCount;
                info.m_nExtradayTeamCount = m_CopySceneDayNumber[nSceneClassID].m_nExtradayTeamCount;  
                m_CopySceneDayNumber[nSceneClassID] = info;   
            }
            else
            {
                m_CopySceneDayNumber.Add(nSceneClassID, info);
            }
            if (DungeonWindow.Instance() != null)
            {
                DungeonWindow.Instance().UpdateCopySceneInfo(nSceneClassID); 
            }
            if (nSceneClassID == 14 && CangJingGeWindow.Instance() != null)
            {
                CangJingGeWindow.Instance().UpdateInfo();
            }
            if (ActivityController.Instance() != null )
            {
                ActivityController.Instance().UpdateTabTips();
            }
            if (FunctionButtonLogic.Instance())
            {
                FunctionButtonLogic.Instance().UpdateActionButtonTip();
            }
            
        }
        void SetCopySceneMultiple(int nSceneClassID,int nMultiple)
        {
           
            if (m_CopySceneDayNumber.ContainsKey(nSceneClassID))
            {
                CopySceneDayNumber info;
                info.m_nDayCount = m_CopySceneDayNumber[nSceneClassID].m_nDayCount;
                info.m_ndayTeamCount =m_CopySceneDayNumber[nSceneClassID].m_ndayTeamCount;
                info.m_nTotalCount1 = m_CopySceneDayNumber[nSceneClassID].m_nTotalCount1;
                info.m_nTotalCount2 = m_CopySceneDayNumber[nSceneClassID].m_nTotalCount2;
                info.m_nTotalCount3 = m_CopySceneDayNumber[nSceneClassID].m_nTotalCount3;
                info.m_nTeamTotalCount1 = m_CopySceneDayNumber[nSceneClassID].m_nTeamTotalCount1;
                info.m_nTeamTotalCount2 = m_CopySceneDayNumber[nSceneClassID].m_nTeamTotalCount2;
                info.m_nTeamTotalCount3 = m_CopySceneDayNumber[nSceneClassID].m_nTeamTotalCount3;
                info.m_nResetCount = m_CopySceneDayNumber[nSceneClassID].m_nResetCount;
                info.m_nResetTeamCount = m_CopySceneDayNumber[nSceneClassID].m_nResetTeamCount;
                info.m_nMultiple = nMultiple;
                info.m_nExtraDayCount = m_CopySceneDayNumber[nSceneClassID].m_nExtraDayCount;
                info.m_nExtradayTeamCount = m_CopySceneDayNumber[nSceneClassID].m_nExtradayTeamCount;
                m_CopySceneDayNumber[nSceneClassID] = info;
            }
            else
            {
                CopySceneDayNumber info;
                info.m_nDayCount = 0;
                info.m_ndayTeamCount = 0;
                info.m_nTotalCount1 = 0;
                info.m_nTotalCount2 = 0;
                info.m_nTotalCount3 = 0;
                info.m_nTeamTotalCount1 = 0;
                info.m_nTeamTotalCount2 = 0;
                info.m_nTeamTotalCount3 = 0;
                info.m_nResetCount = 0;
                info.m_nResetTeamCount = 0;
                info.m_nExtraDayCount = 0;
                info.m_nExtradayTeamCount = 0;
                info.m_nMultiple = nMultiple;
                m_CopySceneDayNumber.Add(nSceneClassID, info);
            }
            if (DungeonWindow.Instance() != null)
            {
                DungeonWindow.Instance().UpdateCopySceneInfo(nSceneClassID);
            }
            if (ActivityController.Instance() != null)
            {
                ActivityController.Instance().UpdateTabTips();
            }
            if (FunctionButtonLogic.Instance())
            {
                FunctionButtonLogic.Instance().UpdateActionButtonTip();
            }

        }
        void SetCopySceneExtra(int nSceneClassID, int nExtraDayCount, int nExtradayTeamCount)
        {
           
            if (m_CopySceneDayNumber.ContainsKey(nSceneClassID))
            {
                CopySceneDayNumber info;
                info.m_nDayCount = m_CopySceneDayNumber[nSceneClassID].m_nDayCount;
                info.m_ndayTeamCount =m_CopySceneDayNumber[nSceneClassID].m_ndayTeamCount;
                info.m_nTotalCount1 = m_CopySceneDayNumber[nSceneClassID].m_nTotalCount1;
                info.m_nTotalCount2 = m_CopySceneDayNumber[nSceneClassID].m_nTotalCount2;
                info.m_nTotalCount3 = m_CopySceneDayNumber[nSceneClassID].m_nTotalCount3;
                info.m_nTeamTotalCount1 = m_CopySceneDayNumber[nSceneClassID].m_nTeamTotalCount1;
                info.m_nTeamTotalCount2 = m_CopySceneDayNumber[nSceneClassID].m_nTeamTotalCount2;
                info.m_nTeamTotalCount3 = m_CopySceneDayNumber[nSceneClassID].m_nTeamTotalCount3;
                info.m_nResetCount = m_CopySceneDayNumber[nSceneClassID].m_nResetCount;
                info.m_nResetTeamCount = m_CopySceneDayNumber[nSceneClassID].m_nResetTeamCount;
                info.m_nMultiple = m_CopySceneDayNumber[nSceneClassID].m_nMultiple;
                info.m_nExtraDayCount = nExtraDayCount;
                info.m_nExtradayTeamCount = nExtradayTeamCount;
                m_CopySceneDayNumber[nSceneClassID] = info;
            }
            else
            {
                CopySceneDayNumber info;
                info.m_nDayCount = 0;
                info.m_ndayTeamCount = 0;
                info.m_nTotalCount1 = 0;
                info.m_nTotalCount2 = 0;
                info.m_nTotalCount3 = 0;
                info.m_nTeamTotalCount1 = 0;
                info.m_nTeamTotalCount2 = 0;
                info.m_nTeamTotalCount3 = 0;
                info.m_nResetCount = 0;
                info.m_nResetTeamCount = 0;
                info.m_nExtraDayCount = nExtraDayCount;
                info.m_nExtradayTeamCount = nExtradayTeamCount;
                info.m_nMultiple = 0;
                m_CopySceneDayNumber.Add(nSceneClassID, info);
            }
            if (DungeonWindow.Instance() != null)
            {
                DungeonWindow.Instance().UpdateCopySceneInfo(nSceneClassID);
            }
            if (ActivityController.Instance() != null)
            {
                ActivityController.Instance().UpdateTabTips();
            }
            if (FunctionButtonLogic.Instance())
            {
                FunctionButtonLogic.Instance().UpdateActionButtonTip();
            }

        }
        public int GetCopySceneNumber(int nSceneClassID, int nSingle)   //当天次数
        {

            if (m_CopySceneDayNumber.ContainsKey(nSceneClassID))
            {
                if (nSingle == 1)
                {
                    return m_CopySceneDayNumber[nSceneClassID].m_nDayCount;
                }
                else
                {
                    return m_CopySceneDayNumber[nSceneClassID].m_ndayTeamCount;
                }               
            }
            return 0;
        }
        public int GetCopySceneTotalNumber(int nSceneClassID, int nDiffcult, int nSingle)   //总次数
        {
            if (m_CopySceneDayNumber.ContainsKey(nSceneClassID))
            {
                if (nSingle == 1)
                {
                    if (nDiffcult == 1)
                    {
                        return m_CopySceneDayNumber[nSceneClassID].m_nTotalCount1;
                    }
                    else if (nDiffcult == 2)
                    {
                        return m_CopySceneDayNumber[nSceneClassID].m_nTotalCount2;
                    }
                    else if ( nDiffcult == 3)
                    {
                        return m_CopySceneDayNumber[nSceneClassID].m_nTotalCount3;
                    }
                   
                }
                else
                {
                    if (nDiffcult == 1)
                    {
                        return m_CopySceneDayNumber[nSceneClassID].m_nTeamTotalCount1;
                    }
                    else if (nDiffcult == 2)
                    {
                        return m_CopySceneDayNumber[nSceneClassID].m_nTeamTotalCount2;
                    }
                    else if (nDiffcult == 3)
                    {
                        return m_CopySceneDayNumber[nSceneClassID].m_nTeamTotalCount3;
                    }
                }
            }
            return 0;
        }
        public int GetCopySceneMultiple(int nSceneClassID)   
        {

            if (m_CopySceneDayNumber.ContainsKey(nSceneClassID))
            {
                return m_CopySceneDayNumber[nSceneClassID].m_nMultiple;
            }
            return 1;
        }
        public int GetCopySceneReset(int nSceneClassID,int nSingle)
        {
            if (m_CopySceneDayNumber.ContainsKey(nSceneClassID))
            {
                if ( nSingle == 1)
                {
                    return m_CopySceneDayNumber[nSceneClassID].m_nResetCount;
                }
                else
                {
                    return m_CopySceneDayNumber[nSceneClassID].m_nResetTeamCount;
                }               
            }
            return 0;
        }
        public int GetCopySceneExtraNumber(int nSceneClassID, int nSingle)   //当天次数
        {

            if (m_CopySceneDayNumber.ContainsKey(nSceneClassID))
            {
                if (nSingle == 1)
                {
                    return m_CopySceneDayNumber[nSceneClassID].m_nExtraDayCount;
                }
                else
                {
                    return m_CopySceneDayNumber[nSceneClassID].m_nExtradayTeamCount;
                }
            }
            return 0;
        }
        public void HandlePacket(GC_SYNC_COMMONDATA packet)
        {
            for (int i = 0; i < packet.nIndexCount; i++ )
            {
                SetCommonData((int)packet.GetNIndex(i), (int)packet.GetNValue(i));
            }
        }

        public void HandlePacket(GC_SYNC_COMMONFLAG packet)
        {
            for (int i = 0; i < packet.nIndexCount; i++)
            {
                m_CommonFlag[i] = packet.GetNValue(i);
            }
        }

        public void HandlePacket(GC_ASK_COMMONFLAG_RET packet)
        {
            bool bFlag = (packet.NFlag == 1? true:false);
            SetCommonFlag(packet.NBits, bFlag);
        }

        public void HandlePacket(GC_SYNC_COPYSCENENUMBER packet)
        {
            for (int i = 0; i < packet.nSceneClassIDCount; i++)
            {
                if (packet.nDayCountCount <= 0)
                {
                    SetCopySceneMultiple(packet.GetNSceneClassID(i), packet.GetNMultiple(i));
                }
                else
                {
                    CopySceneDayNumber info;

                    info.m_nDayCount =  packet.GetNDayCount(i);
                    info.m_ndayTeamCount = packet.GetNTeamDayCount(i);
                    info.m_nTotalCount1 = packet.GetNTotalCount1(i);
                    info.m_nTotalCount2 = packet.GetNTotalCount2(i);
                    info.m_nTotalCount3 = packet.GetNTotalCount3(i);
                    info.m_nTeamTotalCount1 = packet.GetNTeamTotalCount1(i);
                    info.m_nTeamTotalCount2 = packet.GetNTeamTotalCount2(i);
                    info.m_nTeamTotalCount3 = packet.GetNTeamTotalCount3(i);
                    info.m_nMultiple = packet.GetNMultiple(i);
                    info.m_nResetCount = packet.GetNDayResetCount(i);
                    info.m_nResetTeamCount = packet.GetNTeamDayResetCount(i);
                    info.m_nExtraDayCount = 0;
                    info.m_nExtradayTeamCount = 0;
                    SetCopySceneNumber(packet.GetNSceneClassID(i), info);
                }
            }
        }
        public void HandlePacket(GC_SYNC_COPYSCENEEXTRANUMBER packet)
        {
            for (int i = 0; i < packet.nSceneClassIDCount; i++)
            {
                SetCopySceneExtra(packet.GetNSceneClassID(i), packet.GetNDayExtraCount(i), packet.GetNTeamDayExtraCount(i));
            }
        }
    }
}
