//********************************************************************
// 文件名: GameTitleInvestitive.cs
// 描述: 称号结构
// 作者: WangZhe
// Modify Log:
//     2014-5-28 Lijia: 客户端效率优化，把GameSystemTitleInvestitive从class改为struct
//     2014-5-28 Lijia: 客户端效率优化，把GameUserDefTitleInvestitive从class改为struct
//********************************************************************

using UnityEngine;
using System.Collections.Generic;
using System;
using GCGame.Table;
using GCGame;
using Games.GlobeDefine;

namespace Games.TitleInvestitive
{
    enum TITLE_SIZE
    {
        TITLE_NAME = 22,
        SYSTEMTITLE_NUM = 64,//系统程序支持存储的32个
        USERDEF_NUM = 8, //定义称号支持存储8个
        USERDEF_START = SYSTEMTITLE_NUM,
        TITLE_TOTAL = USERDEF_START + USERDEF_NUM,
    };

    enum TITLE_TYPE
    {
        TYPE_NONE = -1,
        TYPE_SYSTITLE,
        TYPE_DEFTITE,
        TYPE_ALL,
    };

    enum USERDEF_SUBTYPE
    {
        TITLE_DEFTYPE_INVALID = -1,		//无效
        TITLE_DEFTYPE_TEST = 0,		//心情
        TITLE_DEFTYPE_YULIU0 = 1,		//预留
        TITLE_DEFTYPE_YULIU1 = 2,		//预留
        TITLE_DEFTYPE_YULIU2 = 3,		//预留
        TITLE_DEFTYPE_YULIU3 = 4,		//预留
        TITLE_DEFTYPE_YULIU4 = 5,		//预留
        TITLE_DEFTYPE_YULIU5 = 6,		//预留
        TITLE_DEFTYPE_YULIU6 = 7,		//预留
        TITLE_DEFTYPE_NUM = TITLE_SIZE.USERDEF_NUM		 //总个数
    };

    /// <summary>
    /// 物品品质
    /// </summary>
    public enum TITLE_COLORLEVEL
    {
        COLOR_INVALID = 0,
        COLOR_WHITE,				//白
        COLOR_GREEN,				//绿
        COLOR_BLUE,				    //蓝
        COLOR_PURPLE,				//紫
        COLOR_ORANGE,				//橙
    }

    // 系统称号
    public struct GameSystemTitleInvestitive
    {
        /// <summary>
        /// 称号ID
        /// </summary>
        private int m_TitleID;
        public int TitleID
        {
            get { return m_TitleID; }
            set { m_TitleID = value; }
        }

        public GameSystemTitleInvestitive(int nTitle)
        {
            m_TitleID = nTitle;
        }

        public void Clear()
        {
            m_TitleID = -1;
        }

        public int GetTitleType()
        {
            Tab_TitleData title = TableManager.GetTitleDataByID(m_TitleID, 0);
            if (title != null)
            {
                return title.Type;
            }
            return GlobeVar.INVALID_ID;
        }
    }

    // 自定义称号
    public struct GameUserDefTitleInvestitive
    {
        /// <summary>
        /// 称号ID
        /// </summary>
        private int m_TitleID;
        public int TitleID
        {
            get { return m_TitleID; }
            set { m_TitleID = value; }
        }

        /// <summary>
        /// 玩家自定义称号完整内容
        /// </summary>
        private string m_strFullTitleName;
        public string StrFullTitleName
        {
            get { return m_strFullTitleName; }
            set { m_strFullTitleName = value; }
        }

        public GameUserDefTitleInvestitive(int nTitleID)
        {
            m_TitleID = nTitleID;
            m_strFullTitleName = "";
        }

        public void Clear()
        {
            m_TitleID = -1;
            m_strFullTitleName = "";
        }

        public int GetTitleType()
        {
            Tab_TitleData title = TableManager.GetTitleDataByID(m_TitleID, 0);
            if (title != null)
            {
                return title.Type;
            }
            return GlobeVar.INVALID_ID;
        }
    }

    // 游戏称号 包含两种
    public class GameTitleInvestitive
    {
        private GameSystemTitleInvestitive[] m_SystemTitle = new GameSystemTitleInvestitive[(int)TITLE_SIZE.SYSTEMTITLE_NUM];
        public GameSystemTitleInvestitive[] SystemTitle
        {
            get { return m_SystemTitle; }
        }
        
        private GameUserDefTitleInvestitive[] m_UserDefTitle = new GameUserDefTitleInvestitive[(int)TITLE_SIZE.USERDEF_NUM];
        public GameUserDefTitleInvestitive[] UserDefTitle
        {
            get { return m_UserDefTitle; }
        }

        private int m_ActiveTitle;
        public int ActiveTitle
        {
            get { return m_ActiveTitle; }
            set { m_ActiveTitle = value; }
        }

        public GameTitleInvestitive()
        {
            for (int i = 0; i < (int)TITLE_SIZE.SYSTEMTITLE_NUM; i++ )
            {
                m_SystemTitle[i] = new GameSystemTitleInvestitive(GlobeVar.INVALID_ID);
            }
            for (int i = 0; i < (int)TITLE_SIZE.USERDEF_NUM; i++)
            {
                m_UserDefTitle[i] = new GameUserDefTitleInvestitive(GlobeVar.INVALID_ID);
            }
            m_ActiveTitle = -1;
        }

        public bool ReadAllTitleInvestitive(GC_UPDATE_ALL_TITLEINVESTITIVE rData)
        {
            // 读取服务器的同步数据 其中TitleID索引是0~40 UserDefFullTitleName是自定义称号特有 索引为0~7
            bool bRet = false;
            if (rData.TitleIDCount == rData.TitleIndexCount)
            {
                for (int i = 0, j = 0; i < rData.TitleIDCount; i++)
                {
                    if (rData.TitleIndexList[i] >= 0 && rData.TitleIndexList[i] < (int)TITLE_SIZE.USERDEF_START)
                    {
                        m_SystemTitle[rData.TitleIndexList[i]].TitleID = rData.TitleIDList[i];
                    }
                    else
                    {
                        if (j >= rData.UserDefFullTitleNameCount)
                        {
                            break;
                        }

                        m_UserDefTitle[rData.TitleIndexList[i] - (int)TITLE_SIZE.USERDEF_START].TitleID = rData.TitleIDList[i];
                        // 自定义内容特殊索引
                        m_UserDefTitle[rData.TitleIndexList[i] - (int)TITLE_SIZE.USERDEF_START].StrFullTitleName = rData.UserDefFullTitleNameList[j];

                        j++;
                    }
                }
                m_ActiveTitle = rData.ActiveTitle;
                bRet = true;
            }           
            return bRet;
        }

        public int GetCurrentTitleID()
        {
            int nRet = -1;
            if(m_ActiveTitle >= 0 && m_ActiveTitle < (int)TITLE_SIZE.USERDEF_START)
            {
                nRet = m_SystemTitle[m_ActiveTitle].TitleID;
            }
            else if (m_ActiveTitle >= (int)TITLE_SIZE.USERDEF_START && m_ActiveTitle < (int)TITLE_SIZE.TITLE_TOTAL)
            {
                nRet = m_UserDefTitle[m_ActiveTitle - (int)TITLE_SIZE.USERDEF_START].TitleID;
            }
            return nRet;
        }

        public string GetCurrentTitle()
        {
            string strRet = "";
			m_ActiveTitle = m_ActiveTitle;
            if (m_ActiveTitle >= 0 && m_ActiveTitle < (int)TITLE_SIZE.USERDEF_START)
            {
                Tab_TitleData tabTitleData = TableManager.GetTitleDataByID(m_SystemTitle[m_ActiveTitle].TitleID, 0);
                if (tabTitleData != null)
                {
                    strRet = Utils.GetTitleColor(tabTitleData.ColorLevel);
                    strRet += tabTitleData.InvestitiveName;
                }
            }
            else if (m_ActiveTitle >= (int)TITLE_SIZE.USERDEF_START && m_ActiveTitle < (int)TITLE_SIZE.TITLE_TOTAL)
            {
                Tab_TitleData tabTitleData = TableManager.GetTitleDataByID(m_UserDefTitle[m_ActiveTitle - (int)TITLE_SIZE.USERDEF_START].TitleID, 0);
                if (tabTitleData != null)
                {
                    strRet = Utils.GetTitleColor(tabTitleData.ColorLevel);
                    strRet += m_UserDefTitle[m_ActiveTitle - (int)TITLE_SIZE.USERDEF_START].StrFullTitleName;
                }
            }
            return strRet;
        }

        public bool ChangeTitle(int nNewIndex)
        {
            if (0 <= nNewIndex && nNewIndex < (int)TITLE_SIZE.TITLE_TOTAL)
            {
                m_ActiveTitle = nNewIndex;
                return true;
            }
            return false;
        }

        public void HandleDeleteTitle(int nDeleteIndex, int nActiveTitle)
        {
            if (0 <= nDeleteIndex && nDeleteIndex < (int)TITLE_SIZE.USERDEF_START)
            {
                m_SystemTitle[nDeleteIndex].TitleID = -1;
            }
            else if ((int)TITLE_SIZE.USERDEF_START <= nDeleteIndex && nDeleteIndex < (int)TITLE_SIZE.TITLE_TOTAL)
            {
                m_UserDefTitle[nDeleteIndex - (int)TITLE_SIZE.USERDEF_START].TitleID = -1;
                m_UserDefTitle[nDeleteIndex - (int)TITLE_SIZE.USERDEF_START].StrFullTitleName = "";
            }
            m_ActiveTitle = nActiveTitle;
        }

        public void HandleGainTitle(int nTitleIndex, int nTitleID, string strUserDef, int nActiveTitle)
        {
            if (0 <= nTitleIndex && nTitleIndex < (int)TITLE_SIZE.USERDEF_START)
            {
                m_SystemTitle[nTitleIndex].TitleID = nTitleID;
            }
            else if ((int)TITLE_SIZE.USERDEF_START <= nTitleIndex && nTitleIndex < (int)TITLE_SIZE.TITLE_TOTAL)
            {
                m_UserDefTitle[nTitleIndex - (int)TITLE_SIZE.USERDEF_START].TitleID = nTitleID;
                m_UserDefTitle[nTitleIndex - (int)TITLE_SIZE.USERDEF_START].StrFullTitleName = strUserDef;
            }
            m_ActiveTitle = nActiveTitle;
        }
        public void HandleUpdateDefTitle(int nTitleID, string strUserDef)
        {
            for (int nIndex = (int)TITLE_SIZE.USERDEF_START; nIndex < (int)TITLE_SIZE.TITLE_TOTAL; nIndex++)
            {
                if (nTitleID == m_UserDefTitle[nIndex - (int)TITLE_SIZE.USERDEF_START].TitleID)
                {
                    m_UserDefTitle[nIndex - (int)TITLE_SIZE.USERDEF_START].StrFullTitleName = strUserDef;
                }
            }
        }

        public void ClearData()
        {
            for (int i = 0; i < (int)TITLE_SIZE.SYSTEMTITLE_NUM; i++)
            {
                m_SystemTitle[i].Clear();
            }
            for (int i = 0; i < (int)TITLE_SIZE.USERDEF_NUM; i++)
            {
                m_UserDefTitle[i].Clear();
            }
            m_ActiveTitle = -1;
        }

        public bool IsHaveType(int type)
        {
            for (int i = 0; i < (int)TITLE_SIZE.SYSTEMTITLE_NUM; i++)
            {
                if (m_SystemTitle[i].GetTitleType() == type)
                {
                    return true;
                }
            }
            for (int i = 0; i < (int)TITLE_SIZE.USERDEF_NUM; i++)
            {
                if (m_UserDefTitle[i].GetTitleType() == type)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
