/******************************************************************************** *	文件名：DayAwardLogic.cs *	全路径：	\Script\GUI\DayAwardLogic.cs *	创建人：	贺文鹏 *	创建时间：2014-02-24 * *	功能说明： 在线奖励界面 *	        *	修改记录：*********************************************************************************/using UnityEngine;using System.Collections;using System.Collections.Generic;using GCGame;using GCGame.Table;using Games.AwardActivity;
using Module.Log;public class DayAwardLogic : MonoBehaviour{    public UIGrid m_ItemGrid;    private int m_WeekDay;    private bool m_DayAwardFlag;

    public UIImageButton m_ButtonAward;

    //public UIImageButton m_TabActive;
    //public GameObject m_ContainActive;

    public RewardAwardItem[] m_AwardItem;
    // 初始化数据    void init()    {        m_WeekDay = GameManager.gameManager.PlayerDataPool.AwardActivityData.WeekDay;        m_DayAwardFlag = GameManager.gameManager.PlayerDataPool.AwardActivityData.DayAwardFlag;    }    void CleanUp()    {        m_WeekDay = -1;        m_DayAwardFlag = true;

        if (m_ItemGrid != null)
        {
            //Utils.CleanGrid(m_ItemGrid.gameObject);
            m_ItemGrid.repositionNow = true;
            m_ItemGrid.sorted = true;        }

        for (int i = 0; i < m_AwardItem.Length; i++)
        {
            if (m_AwardItem[i] != null)            {
                m_AwardItem[i].CleanUp();            }
        }

        if (m_ButtonAward)        {
            m_ButtonAward.isEnabled = false;        }    }    // 在线奖励按钮    public void ButtonDayAward()    {        CleanUp();        init();
        UpdateWindow();    }    // 创建奖励list    void CreateAwardItemList()    {
        UIManager.LoadItem(UIInfo.AwardListItem, OnLoadAwardItem);            }    void OnLoadAwardItem(GameObject awardActivityItem, object param)
    {
        if (null == awardActivityItem || null == m_ItemGrid)
        {
            return;
        }
        int nMaxRecordCount = TableManager.GetDayAward().Count;
        for (int i = 0; i < nMaxRecordCount; i++)
        {
            Tab_DayAward pAward = TableManager.GetDayAwardByID(i, 0);
            if (pAward == null)
            {
                continue;
            }

            string strName = "";
            AwardState awardState = AwardState.AWARD_CANNNTHAVE;

            if (m_WeekDay < pAward.Id)
            {
                strName = "0" + pAward.Id.ToString();
                awardState = AwardState.AWARD_NOTHAVEDONE;
            }
            else if (m_WeekDay == pAward.Id && m_DayAwardFlag == false)
            {
                strName = "0" + pAward.Id.ToString();
                awardState = AwardState.AWARD_CANHAVE;
            }
            else
            {
                strName = "1" + pAward.Id.ToString();
                awardState = AwardState.AWARD_NOTHAVEDONE;
            }

            AwardActivityItem AwardItem = AwardActivityItem.CreateAwardItem(strName, m_ItemGrid.gameObject, awardActivityItem, AwardActivityType.AWARD_DAY, awardState);
            if (AwardItem)
            {
                AwardItem.AddExpMoneyYuanbaoUI(pAward.Exp, pAward.Money, pAward.BindYuanbao);
                AwardItem.AddItemUI(pAward.ItemDataID, pAward.ItemCount);
                AwardItem.AwardInfoText = pAward.DescName;
                AwardItem.AwardButtonState = awardState;
            }
        }// end for

        m_ItemGrid.repositionNow = true;
        m_ItemGrid.sorted = true;
    }

    void OnAwardClick()
    {
        if (m_DayAwardFlag == true)
        {
            // 提示 已经领了
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2175}");
            return;
        }

        // 发包 领取
        GameManager.gameManager.PlayerDataPool.AwardActivityData.SendAwardPacket(AwardActivityType.AWARD_DAY);
    }    void UpdateWindow()
    {
        int nMaxRecordCount = TableManager.GetDayAward().Count;
        for (int i = 0; i < nMaxRecordCount; i++)
        {
            Tab_DayAward pAward = TableManager.GetDayAwardByID(i, 0);
            if (pAward == null)
            {
                continue;
            }
            if ( i >= m_AwardItem.Length )            {
                continue;            }
            // 选中状态
            if (m_WeekDay == i && false == m_AwardItem[i].m_ChooseSprit.gameObject.activeInHierarchy)
            {
                m_AwardItem[i].SetChooseState(true);
            }
            else if (true == m_AwardItem[i].m_ChooseSprit.gameObject.activeInHierarchy)
            {
                m_AwardItem[i].SetChooseState(false);
            }

            // 领取状态
            UpDateAwardState(i);

            if (pAward.Exp > 0)
            {
                m_AwardItem[i].AddItem(RewardAwardItem.ItemType.ITEM_EXP, -1, pAward.Exp);
            }

            if (pAward.Money > 0)
            {
                m_AwardItem[i].AddItem(RewardAwardItem.ItemType.ITEM_MONEY, -1, pAward.Money);
            }

            if (pAward.BindYuanbao > 0)
            {
                m_AwardItem[i].AddItem(RewardAwardItem.ItemType.ITEM_YUANBAO, -1, pAward.BindYuanbao);
            }

            // 物品
            int nItemDataID = pAward.ItemDataID;
            int nItemCount = pAward.ItemCount;
            if (nItemDataID >= 0 && nItemCount > 0)
            {
                m_AwardItem[i].AddItem(RewardAwardItem.ItemType.ITEM_ITEM, nItemDataID, nItemCount);
            }
        }// end for
    }

    void UpDateAwardState(int nWeekDay)
    {
        int nMaxRecordCount = TableManager.GetDayAward().Count;
        if (nWeekDay < 0 || nWeekDay >= nMaxRecordCount)
        {
            return;
        }
        if (nWeekDay >= m_AwardItem.Length)        {
            return;        }
        if (m_WeekDay == nWeekDay)
        {
            if (m_DayAwardFlag == true)            {
                m_AwardItem[nWeekDay].UpdateItemState(RewardAwardItem.AwardState.AWARD_HAVEDONE);
            }
            else
            {
                m_AwardItem[nWeekDay].UpdateItemState(RewardAwardItem.AwardState.AWARD_CANHAVE);
                if (m_ButtonAward)                {
                    m_ButtonAward.isEnabled = true;                }
            }
        }
        else
        {
            m_AwardItem[nWeekDay].UpdateItemState(RewardAwardItem.AwardState.AWARD_CANNNTHAVE);
        }
    }

    public void PlayEffect(int nDay, bool bAwardFlag)
    {
        if (nDay >= 0 && nDay < AwardActivityData.MaxDayAwardDays)
        {
            if (bAwardFlag == true)
            {
                m_AwardItem[nDay].PlayEffect();
            }
        }
    }}