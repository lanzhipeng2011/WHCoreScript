/******************************************************************************** *	文件名：NewServerAwardLogic.cs *	全路径：	\Script\GUI\NewServerAwardLogic.cs *	创建人：	贺文鹏 *	创建时间：2014-02-24 * *	功能说明： 在线奖励界面 *	        *	修改记录：*********************************************************************************/using UnityEngine;using System.Collections;using System.Collections.Generic;using GCGame;using GCGame.Table;using Games.AwardActivity;
using Module.Log;
using Games.LogicObj;public class NewServerAwardLogic : MonoBehaviour{    public UIGrid m_ItemGrid;    private int m_NewServerDays;

    public UIImageButton m_ButtonAward;

    public RewardAwardItem[] m_AwardItem; // 7个    // 初始化数据    void init()    {        m_NewServerDays = GameManager.gameManager.PlayerDataPool.AwardActivityData.NewServerDays;    }    bool GetNewServerAwardFlag(int nDay)    {        return GameManager.gameManager.PlayerDataPool.AwardActivityData.GetNewServerAwardFlag(nDay);    }    void CleanUp()    {        m_NewServerDays = -1;

        if (m_ItemGrid != null)
        {
        //    Utils.CleanGrid(m_ItemGrid.gameObject);
            m_ItemGrid.repositionNow = true;
            m_ItemGrid.sorted = true;
        }

        for (int i = 0; i < m_AwardItem.Length; i++)
        {
            if (m_AwardItem[i])
            {
                m_AwardItem[i].CleanUp();
            }
        }

        if (m_ButtonAward)        {
            m_ButtonAward.isEnabled = false;        }    }    // 在线奖励按钮    public void ButtonNewServerAward()    {        CleanUp();        init();
        UpDateWindow();    }    // 创建奖励list    void CreateAwardItemList()    {
        UIManager.LoadItem(UIInfo.AwardListItem, OnLoadAwardItem);    }    void OnLoadAwardItem(GameObject awardActivityItem, object param)
    {
        if (m_ItemGrid == null || null == awardActivityItem)        {
            return;        }
        int nMaxRecordCount = TableManager.GetNewServerAward().Count;
        for (int i = 0; i < nMaxRecordCount; i++)
        {
            Tab_NewServerAward pAward = TableManager.GetNewServerAwardByID(i, 0);
            if (pAward == null)
            {
                LogModule.DebugLog("DayAward: DayAward.txt can't find line " + i);
                continue;
            }

            string strName = "";
            AwardState awardState = AwardState.AWARD_CANNNTHAVE;

            if (m_NewServerDays < pAward.Id)
            {
                strName = "0" + pAward.Id.ToString();
                awardState = AwardState.AWARD_NOTHAVEDONE;
            }
            else if (m_NewServerDays == pAward.Id && GetNewServerAwardFlag(pAward.Id) == false)
            {
                strName = "0" + pAward.Id.ToString();
                awardState = AwardState.AWARD_CANHAVE;
            }
            else
            {
                strName = "1" + pAward.Id.ToString();
                if (GetNewServerAwardFlag(pAward.Id))
                {
                    awardState = AwardState.AWARD_HAVEDONE;
                }
                else
                    awardState = AwardState.AWARD_NOTHAVEDONE;
            }

            // 第6天的包 特殊处理，前面须领够4次，才能领包
            if (pAward.Id == nMaxRecordCount - 1)
            {
                int nNeedCount = 0;
                for (int nIndex = 0; nIndex < pAward.Id; nIndex++)
                {
                    if (GetNewServerAwardFlag(nIndex))
                    {
                        nNeedCount++;
                    }
                }
                if (nNeedCount < 4)
                {
                    awardState = AwardState.AWARD_NOTHAVEDONE;
                }
            }

            AwardActivityItem AwardItem = AwardActivityItem.CreateAwardItem(strName, m_ItemGrid.gameObject, awardActivityItem, AwardActivityType.AWARD_NEWSERVER, awardState);
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
    }    void OnAwardClick()
    {
        if (GetNewServerAwardFlag(m_NewServerDays) == true)
        {
            // 提示
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2175}");
            return;
        }

        // 第6天的包 特殊处理，前面须领够4次，才能领包
        if (m_NewServerDays == AwardActivityData.MaxNewServerDays - 1)
        {
            int nNeedCount = 0;
            for (int nIndex = 0; nIndex < m_NewServerDays; nIndex++)
            {
                if (GetNewServerAwardFlag(nIndex))
                {
                    nNeedCount++;
                }
            }
            if (nNeedCount < 4)
            {
                // 提示
                Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false,"#{2166}");
                return;
            }
        }

        GameManager.gameManager.PlayerDataPool.AwardActivityData.SendAwardPacket(AwardActivityType.AWARD_NEWSERVER);
    }    // 刷新界面    void UpDateWindow()
    {
        // 设置选中界面
        int nMaxRecordCount = AwardActivityData.MaxNewServerDays;
        for (int i = 0; i < nMaxRecordCount && i < m_AwardItem.Length; i++)
        {
            Tab_NewServerAward pAward = TableManager.GetNewServerAwardByID(i, 0);
            if (pAward == null)
            {
                continue;
            }
            if ( i >= m_AwardItem.Length )            {
                continue;            }
            // 选中状态
            if (m_NewServerDays == i && false == m_AwardItem[i].m_ChooseSprit.gameObject.activeInHierarchy)            {                m_AwardItem[i].SetChooseState(true);            }
            else if (true == m_AwardItem[i].m_ChooseSprit.gameObject.activeInHierarchy)
            {
                m_AwardItem[i].SetChooseState(false);
            }

            UpDateAwardState(i);

            if (pAward.Exp > 0)            {
                m_AwardItem[i].AddItem(RewardAwardItem.ItemType.ITEM_EXP, -1, pAward.Exp);            }

            if (pAward.Money > 0)            {
                m_AwardItem[i].AddItem(RewardAwardItem.ItemType.ITEM_MONEY, -1, pAward.Money);            }

            if (pAward.BindYuanbao > 0)
            {
                m_AwardItem[i].AddItem(RewardAwardItem.ItemType.ITEM_YUANBAO, -1, pAward.BindYuanbao);
            }

            if (pAward.ItemDataID >= 0 && pAward.ItemCount > 0)            {
                m_AwardItem[i].AddItem(RewardAwardItem.ItemType.ITEM_ITEM, pAward.ItemDataID, pAward.ItemCount);            }
        }// end for
    }

    void UpDateAwardState(int nDay)
    {
        if (nDay < 0 || nDay >= AwardActivityData.MaxNewServerDays)        {
            return;        }
        if ( nDay >= m_AwardItem.Length)        {
            return;        }
        if (GetNewServerAwardFlag(nDay) == true)
        {
            m_AwardItem[nDay].UpdateItemState(RewardAwardItem.AwardState.AWARD_HAVEDONE);
        }
        else
        {
            if (m_NewServerDays == nDay && m_ButtonAward)            {                m_AwardItem[nDay].UpdateItemState(RewardAwardItem.AwardState.AWARD_CANHAVE);
                m_ButtonAward.isEnabled = true;            }
            else
            {
                m_AwardItem[nDay].UpdateItemState(RewardAwardItem.AwardState.AWARD_CANNNTHAVE);
            }
        }
    }    public void PlayEffect(int nNewServerDay)
    {
        if (nNewServerDay >= 0 && nNewServerDay < AwardActivityData.MaxNewServerDays)        {
            if (GetNewServerAwardFlag(nNewServerDay) == true)            {
                m_AwardItem[nNewServerDay].PlayEffect();            }        }
    }}