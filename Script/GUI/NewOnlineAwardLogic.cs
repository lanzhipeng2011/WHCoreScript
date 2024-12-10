/******************************************************************************** *	文件名：NewOnlineAwardLogic.cs *	全路径：	\Script\GUI\NewOnlineAwardLogic.cs *	创建人：	杨鑫 *	创建时间：2014-07-25 * *	功能说明： 开服前三天在线奖励界面 *	        *	修改记录：*********************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GCGame;
using GCGame.Table;
using Games.AwardActivity;
using Module.Log;
public class NewOnlineAwardLogic : MonoBehaviour
{
    public UILabel m_LeftTimeText;
    public UIGrid m_ItemGrid;
    private int m_OnlineAwardID;
    private int m_LeftTime;
    public int LeftTime
    {
        set
        {
            m_LeftTime = value;
            UpdateLeftTime();
        }
    }
    public UIImageButton m_ButtonAward;
    public UISprite[] m_SpritMin;
    public UISprite[] m_SpritSec;
    public RewardAwardItem[] m_AwardItem;
    public UILabel m_DateText;
    // 初始化数据
    void init()
    {
        m_OnlineAwardID = GameManager.gameManager.PlayerDataPool.AwardActivityData.NewOnlineAwardID;
        m_LeftTime = GameManager.gameManager.PlayerDataPool.AwardActivityData.NewLeftTime;
        if (GameManager.gameManager.PlayerDataPool.AwardActivityData.NewOnlineAwardStart)        {
            m_ButtonAward.isEnabled = true;        }
        else
        {
            m_ButtonAward.isEnabled = false;
        }
    }

    void CleanUp()
    {
        m_OnlineAwardID = -1;
        m_LeftTime = -1;

        if (m_ItemGrid != null)
        {
            //Utils.CleanGrid(m_ItemGrid.gameObject);
            m_ItemGrid.repositionNow = true;
            m_ItemGrid.sorted = true;
        }

        for (int i = 0; i < m_AwardItem.Length; i++)
        {
            if (m_AwardItem[i] != null)
            {
                m_AwardItem[i].CleanUp();
            }
        }
        if (m_ButtonAward)        {
            m_ButtonAward.isEnabled = false;        }
    }

    // 在线奖励按钮
    public void ButtonOnlineAward()
    {
        CleanUp();
        init();
        UpdateWindow();
        UpdateLeftTime();
        UpdateDate();
    }   
    void OnAwardClick()
    {
        int nMaxRecordCount = GameManager.gameManager.PlayerDataPool.NewOnlineAwardTable.Count;//TableManager.GetOnlineAward().Count;
        if (m_OnlineAwardID >= nMaxRecordCount)
        {
            // 提示 已经领了
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2175}");
            return;
        }

        if (m_LeftTime > 0)
        {
            // 提示 不能领取
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2478}");
            return;
        }

        // 发包 领取
        GameManager.gameManager.PlayerDataPool.AwardActivityData.SendAwardPacket(AwardActivityType.AWARD_NEWONLINE);
    }

    void UpdateWindow()
    {
        //int nMaxRecordCount = TableManager.GetOnlineAward().Count;
        Dictionary<int, OnlineAwardLine> DataTab = GameManager.gameManager.PlayerDataPool.NewOnlineAwardTable;
        int nMaxRecordCount = DataTab.Count;//TableManager.GetOnlineAward().Count;
        for (int i = 0; i < nMaxRecordCount; i++)
        {
            if (DataTab.ContainsKey(i) == false)
            {
                continue;
            }
            //UpdateTimeSpirit(i);
            if (i >= m_AwardItem.Length)
            {
                continue;
            }
            // 更新选中状态
            if (m_OnlineAwardID == i && GameManager.gameManager.PlayerDataPool.AwardActivityData.NewOnlineAwardStart)
            {
                m_AwardItem[i].SetChooseState(true);
            }
            else
            {
                m_AwardItem[i].SetChooseState(false);
            }

            UpDateAwardState(i);

            if (DataTab[i].Exp > 0)
            {
                m_AwardItem[i].AddItem(RewardAwardItem.ItemType.ITEM_EXP, -1, DataTab[i].Exp);
            }

            if (DataTab[i].Money > 0)
            {
                m_AwardItem[i].AddItem(RewardAwardItem.ItemType.ITEM_MONEY, -1, DataTab[i].Money);
            }

            if (DataTab[i].BindYuanbao > 0)
            {
                m_AwardItem[i].AddItem(RewardAwardItem.ItemType.ITEM_YUANBAO, -1, DataTab[i].BindYuanbao);
            }

            // 物品
            int nItem1DataID = DataTab[i].Item1DataID;
            int nItem1Count = DataTab[i].Item1Count;
            if (nItem1DataID >= 0 && nItem1Count > 0)
            {
                m_AwardItem[i].AddItem(RewardAwardItem.ItemType.ITEM_ITEM, nItem1DataID, nItem1Count);
            }
            int nItem2DataID = DataTab[i].Item2DataID;
            int nItem2Count = DataTab[i].Item2count;
            if (nItem1DataID >= 0 && nItem1Count > 0)
            {
                m_AwardItem[i].AddItem(RewardAwardItem.ItemType.ITEM_ITEM, nItem2DataID, nItem2Count);
            }

        }// end for
    }

    // 剩余时间更新
    void UpdateLeftTime()
    {
        int nHour = m_LeftTime / (60 * 60);
        int nMin = (m_LeftTime % (60 * 60)) / 60;
        int nSec = m_LeftTime % 60;
        if (m_LeftTimeText != null && GameManager.gameManager.PlayerDataPool.AwardActivityData.NewOnlineAwardStart)
        {
            //m_LeftTimeText.text = "距离下次领奖：" + nHour / 10 + nHour % 10 + ":" + nMin / 10 + nMin % 10 + ":" + nSec / 10 + nSec % 10;
            m_LeftTimeText.text = StrDictionary.GetClientDictionaryString("#{2762}", Utils.GetTimeDiffFormatString(m_LeftTime));
        }
        else
        {
            m_LeftTimeText.text = StrDictionary.GetClientDictionaryString("#{2763}");
        }
    }

    void UpDateAwardState(int nOnlineAwardID)
    {
        int nMaxRecordCount = GameManager.gameManager.PlayerDataPool.NewOnlineAwardTable.Count;
        if (nOnlineAwardID < 0 || nOnlineAwardID >= nMaxRecordCount)
        {
            return;
        }
        if (nOnlineAwardID >= m_AwardItem.Length)
        {
            return;
        }

        RewardAwardItem.AwardState AwarState = RewardAwardItem.AwardState.AWARD_CANNNTHAVE;
        if (m_OnlineAwardID > nOnlineAwardID || m_OnlineAwardID < 0)
        {
            AwarState = RewardAwardItem.AwardState.AWARD_HAVEDONE;
        }
        else
        {
            if (m_OnlineAwardID == nOnlineAwardID && m_LeftTime <= 0 && m_ButtonAward)            {
                AwarState = RewardAwardItem.AwardState.AWARD_CANHAVE;
                m_ButtonAward.isEnabled = true;            }
        }

        m_AwardItem[nOnlineAwardID].UpdateItemState(AwarState);
    }

    void UpdateTimeSpirit(int nOnlineAwardID)
    {
        if (nOnlineAwardID >= m_SpritMin.Length || nOnlineAwardID >= m_SpritSec.Length)
        {
            return;
        }
        Dictionary<int, OnlineAwardLine> DataTab = GameManager.gameManager.PlayerDataPool.NewOnlineAwardTable;
        if (nOnlineAwardID < 0 || nOnlineAwardID >= DataTab.Count)
        {
            return;
        }
        if (false == DataTab.ContainsKey(nOnlineAwardID))
        {
            return;
        }

        int nLeftTime = DataTab[nOnlineAwardID].LeftTime;
        int nMin = nLeftTime / 60;
        int nShiWei = nMin / 10;
        int nGeWei = nMin % 10;

        if (nShiWei >= 0 && nShiWei <= 9
            && nGeWei >= 0 && nGeWei <= 9)
        {
            m_SpritMin[nOnlineAwardID].spriteName = nShiWei.ToString();
            m_SpritSec[nOnlineAwardID].spriteName = nGeWei.ToString();
        }
    }
    void UpdateDate()
    {
        int nStartMonth = GameManager.gameManager.PlayerDataPool.m_sNewOnlineDateTime.StartDate % 10000 / 100;
        int nStartDay = GameManager.gameManager.PlayerDataPool.m_sNewOnlineDateTime.StartDate % 100;
        int nEndMonth = GameManager.gameManager.PlayerDataPool.m_sNewOnlineDateTime.EndDate % 10000 / 100;
        int nEndDay = GameManager.gameManager.PlayerDataPool.m_sNewOnlineDateTime.EndDate % 100;
        string szSecond = (GameManager.gameManager.PlayerDataPool.m_sNewOnlineDateTime.StartTime % 100).ToString();
        if (GameManager.gameManager.PlayerDataPool.m_sNewOnlineDateTime.StartTime % 100 < 10 )        {
            szSecond = "0" + szSecond;        }         
        string szStartTime = (GameManager.gameManager.PlayerDataPool.m_sNewOnlineDateTime.StartTime % 10000 / 100).ToString() + ":" + szSecond;

        szSecond = (GameManager.gameManager.PlayerDataPool.m_sNewOnlineDateTime.EndTime % 100).ToString();
        if (GameManager.gameManager.PlayerDataPool.m_sNewOnlineDateTime.EndTime % 100 < 10)
        {
            szSecond = "0" + szSecond;
        }
        string szEndTime = (GameManager.gameManager.PlayerDataPool.m_sNewOnlineDateTime.EndTime % 10000 / 100).ToString() + ":" + szSecond;

        m_DateText.text = StrDictionary.GetClientDictionaryString("#{2912}", nStartMonth,nStartDay,nEndMonth,nEndDay,szStartTime,szEndTime);        
    }

    public void PlayEffect(int nNewOnlineAwardID)
    {
        int nIndex = nNewOnlineAwardID - 1;
        if (nIndex >= 0 && nIndex < AwardActivityData.MaxNewOnlineAwardCount)
        {
            m_AwardItem[nIndex].PlayEffect();
        }
    }

}
