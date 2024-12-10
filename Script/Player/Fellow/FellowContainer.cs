//********************************************************************
// 文件名: FellowContainer.cs
// 描述: 伙伴容器
// 作者: TangYi
// 创建时间: 2014-2-18
//
// 修改历史:
//********************************************************************

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GCGame.Table;

namespace Games.Fellow
{
    public class FellowContainer
    {
        /// <summary>
        /// 伙伴容器默认容量
        /// </summary>
        private const int SIZE_FELLOWCONTAINER = 50;
        
        private List<Fellow> m_Fellows = new List<Fellow>();
        private int m_ContainerSize = 0;

        /// <summary>
        /// 构造函数
        /// </summary>
        public FellowContainer()
        {
            m_ContainerSize = SIZE_FELLOWCONTAINER;
            for (int i = 0; i < m_ContainerSize; ++i)
            {
                m_Fellows.Add(new Fellow());
            }
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void CleanUp()
        {
            for (int i = 0; i < m_Fellows.Count; ++i)
            {
                m_Fellows[i].CleanUp();
            }
        }

        /// <summary>
        /// 容器大小
        /// </summary>
        public int ContainerSize
        {
            get { return m_ContainerSize; }
        }
        public void AddContainerSize(int nAdd)
        {
            m_ContainerSize += nAdd;
            for (int i = 0; i < nAdd; ++i)
            {
                m_Fellows.Add(new Fellow());
            }
        }

        /// <summary>
        /// 根据索引取得Fellow
        /// </summary>
        /// <param name="Slot"></param>
        /// <returns></returns>
        public Fellow GetFellowByIndex(int slot)
        {
            if (slot >= 0 && slot < m_Fellows.Count)
            {
                return m_Fellows[slot];
            }
            return null;
        }

        /// <summary>
        /// 根据Guid取得Fellow
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public Fellow GetFellowByGuid(UInt64 guid)
        {
            for (int i = 0; i < m_Fellows.Count; ++i)
            {
                if (m_Fellows[i].Guid == guid)
                {
                    return m_Fellows[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 更新伙伴
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="fellow"></param>
        /// <returns></returns>
        public bool UpdateFellow(int slot, Fellow fellow)
        {
            bool bRet = false;
            if (slot >= 0 && slot < m_Fellows.Count)
            {
                m_Fellows[slot] = fellow;
                bRet = true;
            }
            return bRet;
        }

        /// <summary>
        /// 取得伙伴数量
        /// </summary>
        /// <returns></returns>
        public int GetFellowCount()
        {
            int count = 0;
            for (int i = 0; i < m_Fellows.Count; ++i)
            {
                Fellow fellow = m_Fellows[i];
                if (fellow.IsValid())
                {
                    ++count;
                }
            }
            return count;
        }

        /// <summary>
        /// 空槽位数量
        /// </summary>
        /// <returns></returns>
        public int GetEmptySlotCount()
        {
            return ContainerSize - GetFellowCount();
        }

        /// <summary>
        /// 是否有人形伙伴
        /// </summary>
        /// <returns></returns>
        public bool IsHaveHumanFellow()
        {
            for (int i = 0; i < m_Fellows.Count; ++i)
            {
                Fellow fellow = m_Fellows[i];
                if (fellow.IsValid())
                {
                    if (fellow.GetClassId() == (int)FELLOWCLASS.HUNMAN)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 是否有动物形伙伴
        /// </summary>
        /// <returns></returns>
        public bool IsHaveAnimalFellow()
        {
            for (int i = 0; i < m_Fellows.Count; ++i)
            {
                Fellow fellow = m_Fellows[i];
                if (fellow.IsValid())
                {
                    if (fellow.GetClassId() == (int)FELLOWCLASS.ANIMAL)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    public class FellowTool
    {
        public static List<Fellow> FellowSort(FellowContainer container)
        {
            List<Fellow> resultList = new List<Fellow>();
            for (int i = 0; i < container.ContainerSize; i++)
            {
                if (container.GetFellowByIndex(i) != null && container.GetFellowByIndex(i).IsValid())
                {
                    resultList.Add(container.GetFellowByIndex(i));
                }
            }

            for (int i=0; i<resultList.Count; i++)
            {
                int flag = 0;
                for (int j = 0; j < (resultList.Count - i - 1); j++)
                {
                    //当前召出伙伴排前面
                    if (resultList[j + 1].Called)
                    {
                        Fellow tempFellow = resultList[j + 1];
                        resultList[j + 1] = resultList[j];
                        resultList[j] = tempFellow;
                        flag = 1;
                    }
                    else if (resultList[j].Called == false)
                    {
                        //星级高的伙伴排前面
                        if (resultList[j + 1].StarLevel > resultList[j].StarLevel)
                        {
                            Fellow tempFellow = resultList[j + 1];
                            resultList[j + 1] = resultList[j];
                            resultList[j] = tempFellow;
                            flag = 1;
                        }
                        else if (resultList[j + 1].StarLevel == resultList[j].StarLevel)
                        {
                            //品质高的排前面
                            if (resultList[j + 1].Quality > resultList[j].Quality)
                            {
                                Fellow tempFellow = resultList[j + 1];
                                resultList[j + 1] = resultList[j];
                                resultList[j] = tempFellow;
                                flag = 1;
                            }
                            else if (resultList[j + 1].Quality == resultList[j].Quality)
                            {
                                //等级高的排前面
                                if (resultList[j + 1].Level > resultList[j].Level)
                                {
                                    Fellow tempFellow = resultList[j + 1];
                                    resultList[j + 1] = resultList[j];
                                    resultList[j] = tempFellow;
                                    flag = 1;
                                }
                                else if (resultList[j + 1].Level == resultList[j].Level)
                                {
                                    //DataId大的排前面
                                    if (resultList[j + 1].DataId > resultList[j].DataId)
                                    {
                                        Fellow tempFellow = resultList[j + 1];
                                        resultList[j + 1] = resultList[j];
                                        resultList[j] = tempFellow;
                                        flag = 1;
                                    }
                                    else if (resultList[j + 1].DataId == resultList[j].DataId)
                                    {
                                        //Guid大的排前面
                                        if (resultList[j + 1].Guid > resultList[j].Guid)
                                        {
                                            Fellow tempFellow = resultList[j + 1];
                                            resultList[j + 1] = resultList[j];
                                            resultList[j] = tempFellow;
                                            flag = 1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (flag == 0)
                {
                    break;
                }
            }

            return resultList;
        }

        public static string GetFellowSkillQualityFrame(int quality)
        {
            switch (quality)
            {
                case (int)FELLOWQUALITY.WHITE:
				return "ui_pub_012";
                case (int)FELLOWQUALITY.GREEN:
				return "ui_pub_013";
                case (int)FELLOWQUALITY.BLUE:
				return "ui_pub_014";
                case (int)FELLOWQUALITY.PURPLE:
				return "ui_pub_015";
                case (int)FELLOWQUALITY.ORANGE:
				return "ui_pub_016";
                default:
                    return "ui_pub_012";
            }
        }
    }
    
}