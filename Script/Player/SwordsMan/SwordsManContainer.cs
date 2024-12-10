//********************************************************************
// 文件名: SwordsManContainer.cs
// 描述: 侠客容器
// 作者: grx
// 创建时间: 2014-06-26
//********************************************************************

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GCGame.Table;
using Games.GlobeDefine;

namespace Games.SwordsMan
{
    public class SwordsManContainer
    {
        public enum PACK_TYPE
        {
            TYPE_INVALID = -1,
            TYPE_BACKPACK = 3,
            TYPE_EQUIPPACK = 4,
            TYPE_NUM,
        }

        // 背包容量
        public const int SWORDSMAN_BACKPACK_SIZE = 50;
     
        // 装备槽容量
        public const int SWORDSMAN_EQUIPPACK_SIZE = 0;
      

        private List<SwordsMan> m_Items = new List<SwordsMan>();
        private int m_ContainerSize = 0;

        public SwordsManContainer(int nSize, PACK_TYPE nType)
        {
            m_ContainerSize = nSize;
            m_ContainerType = nType;
            for (int i = 0; i < m_ContainerSize; ++i)
            {
                m_Items.Add(new SwordsMan());
            }
        }

        /// <summary>
        /// 容器大小
        /// </summary>
        public int ContainerSize
        {
            get { return m_ContainerSize; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nAdd"></param>
        public void AddContainerSize(int nAdd)
        {
            m_ContainerSize += nAdd;
            for (int i = 0; i < nAdd; ++i)
            {
                m_Items.Add(new SwordsMan());
            }
        }

        /// <summary>
        /// 容器可用大小
        /// </summary>
        public int GetEmptyContainerSize()
        {
            int Num = 0;
            for (int i = 0; i < m_Items.Count; ++i)
            {
                if (m_Items[i].IsValid() == false)
                {
                    Num++;
                }
            }
            return Num;
        }


        /// <summary>
        /// 容器类型
        /// </summary>
        private PACK_TYPE m_ContainerType = PACK_TYPE.TYPE_INVALID;
        public SwordsManContainer.PACK_TYPE ContainerType
        {
            get { return m_ContainerType; }
        }

        /// <summary>
        /// 取得物品
        /// </summary>
        public SwordsMan GetSwordsMan(int slot)
        {
            if (slot >= 0 && slot < m_Items.Count)
            {
                return m_Items[slot];
            }
            return null;
        }

        /// <summary>
        /// 根据GUID取得物品
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public SwordsMan GetSwordsManByGuid(UInt64 guid)
        {
            if (guid == GlobeVar.INVALID_GUID)
            {
                return null;
            }

            for (int i = 0; i < m_Items.Count; ++i)
            {
                if (m_Items[i].Guid == guid)
                {
                    return m_Items[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 更新物品
        /// </summary>
        /// <param name="slot">槽位</param>
        /// <param name="item">物品</param>
        public bool UpdateSwordsMan(int slot, SwordsMan item)
        {
            bool bRet = false;
            if (slot >= 0 && slot < m_Items.Count)
            {
                m_Items[slot] = item;
                bRet = true;
            }
            return bRet;
        }

        /// <summary>
        /// 有效物品数量
        /// </summary>
        /// <returns></returns>
        public int GetSwordsManCount()
        {
            int count = 0;
            for (int i = 0; i < m_Items.Count; ++i)
            {
                SwordsMan item = m_Items[i];
                if (null != item && item.IsValid())
                {
                    ++count;
                }
            }
            return count;
        }

        /// <summary>
        /// 取得物品列表
        /// </summary>
        /// <returns></returns>
        public List<SwordsMan> GetList()
        {
            return m_Items;
        }

        public int GetAllSwordsManCombatValue()
        {
            int CombatValue = 0;
            for (int i = 0; i < m_Items.Count; ++i)
            {
                SwordsMan item = m_Items[i];
                if (null != item && item.IsValid())
                {
                    CombatValue += item.GetCombatValue();
                }
            }
            return CombatValue;
        }
    }

    class SwordsManTool
    {
        static public List<SwordsMan> ItemFilter(SwordsManContainer oContainer)
        {
            if (null == oContainer)
                return null;

            List<SwordsMan> resultlist = new List<SwordsMan>();
            for (int nIndex = 0; nIndex < oContainer.ContainerSize; ++nIndex)
            {
                SwordsMan oSwordsMan = oContainer.GetSwordsMan(nIndex);
                if (null != oSwordsMan && oSwordsMan.IsValid())
                {
                    resultlist.Add(oSwordsMan);           
                }
            }
            return ItemSort(resultlist);
        }
        /// <summary>
        /// 筛选出高于品质的物品
        /// </summary>
        /// <param name="ItemList"></param>
        /// <param name="nQuality"></param>
        /// <returns></returns>
        static public List<SwordsMan> ItemFilter(List<SwordsMan> ItemList, int nQuality)
        {
            if (null == ItemList)
                return null;

            List<SwordsMan> resultlist = new List<SwordsMan>();
            for (int nIndex = 0; nIndex < ItemList.Count; ++nIndex)
            {
                SwordsMan item = ItemList[nIndex];
                if (null != item && item.IsValid())
                {
                    if (item.Quality <= nQuality)
                    {
                        resultlist.Add(item);
                    }
                }
            }
            return ItemSort(resultlist);
        }

        /// <summary>
        /// 背包显示排序
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        static public List<SwordsMan> ItemSort(List<SwordsMan> list)
        {
            if (list == null)
            {
                return null;
            }
            for (int i = 0; i < list.Count - 1; ++i)
            {
                int flag = 0;
                for (int j = 0; j < list.Count - i - 1; ++j)
                {
                    //排序等级高的排前面
                    if (list[j].Quality < list[j + 1].Quality)
                    {
                        SwordsMan temp = list[j];
                        list[j] = list[j + 1];
                        list[j + 1] = temp;
                        flag = 1;
                    }
                    else if (list[j].Quality == list[j + 1].Quality)
                    {
                        
                        //装备等级小的排后面
                        if (list[j].Level < list[j + 1].Level)
                        {
                            SwordsMan temp = list[j];
                            list[j] = list[j + 1];
                            list[j + 1] = temp;
                            flag = 1;
                        }                                         
                    }
                }
                if (flag == 0)
                {
                    break;
                }
            }
            return list;
        }

        static public string GetStarColourSprite(int value)
        {
            switch (value)
            {
                case 0:
                    return "WhiteStar";
                case 1:
                    return "GreenStar";
                case 2:
                    return "BlueStar";
                case 3:
                    return "PurpleStar";
                case 4:
                    return "OrangeStar";
                default:
                    break;
            }
            return "";
        } 
    };
}

