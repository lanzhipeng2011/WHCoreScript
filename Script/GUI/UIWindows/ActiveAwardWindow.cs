/********************************************************************************
 *	文件名：ActiveAwardWindow.cs
 *	全路径：	\Script\GUI\UIWindows\ActiveAwardWindow.cs
 *	创建人：	贺文鹏
 *	创建时间：2014-02-24
 *
 *	功能说明： 活跃度奖励界面
 *	       
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GCGame;
using GCGame.Table;
using Games.AwardActivity;
using Module.Log;
public class ActiveAwardWindow : MonoBehaviour
{
    public UIGrid m_ItemGrid;

    private int m_nActiveness;
    public int Activeness
    {
        get { return m_nActiveness; }
        set { m_nActiveness = value; }
    }
    public GameObject m_AwardItem;

    // Use this for initialization
//     void Start()
//     {
//         ButtonDayAward();
//     }
    
    void OnEnable()
    {
        ButtonDayAward();
    }

    // 初始化数据
    void init()
    {
        Activeness = GameManager.gameManager.PlayerDataPool.AwardActivityData.Activeness;
    }

    void CleanUp()
	{
        Utils.CleanGrid(m_ItemGrid.gameObject);
        m_ItemGrid.repositionNow = true;
        m_ItemGrid.sorted = true;
    }

    // 在线奖励按钮
    public void ButtonDayAward()
    {
        CleanUp();
        init();
        CreateAwardItemList();
    }

    // 创建奖励list
    void CreateAwardItemList()
    {
       // UIManager.LoadItem(UIInfo.ActivenessAwardItem, OnLoadAwardItem);
        OnLoadAwardItem(m_AwardItem);
    }

    void OnLoadAwardItem(GameObject gAwardItem)
    {
        if (null == gAwardItem)
        {
            Debug.LogError("can not load award activeItem");
            return;
        }
        int nMaxRecordCount = TableManager.GetActivenessAward().Count;
        for (int i = 0; i < nMaxRecordCount; i++)
        {
            Tab_ActivenessAward pAward = TableManager.GetActivenessAwardByID(i, 0);
            if (pAward == null)
            {
                LogModule.DebugLog("ActivenessAward: ActivenessAward.txt can't find line " + i);
                continue;
            }

            string strName = i.ToString();
            AwardState awardState = AwardState.AWARD_CANNNTHAVE;
            bool bFlag = GameManager.gameManager.PlayerDataPool.AwardActivityData.GetActivenessAwardFlag(i);
            if (bFlag == false && Activeness >= pAward.MiniActiveness)
            {
                awardState = AwardState.AWARD_CANHAVE;
            }
            else if (bFlag == false && Activeness < pAward.MiniActiveness)
            {
                awardState = AwardState.AWARD_NOTHAVEDONE;
            }
            else if (bFlag == true)
            {
                awardState = AwardState.AWARD_HAVEDONE;
            }
            else
            {
                awardState = AwardState.AWARD_CANNNTHAVE;
            }

            ActivenessAwardItem AwardItem = ActivenessAwardItem.CreateAwardItem(strName, m_ItemGrid.gameObject, gAwardItem);
            if (AwardItem)
            {
                AwardItem.AddAwardUI(pAward.Exp, pAward.Money, pAward.BindYuanbao,pAward.AwardSkillExp, pAward.AwardReputation);
                AwardItem.AddItemUI(pAward.ItemDataID, pAward.ItemCount);
                AwardItem.AddAwardUIRepution( pAward.AwardReputation);
                AwardItem.AwardInfoText = StrDictionary.GetClientDictionaryString("#{1634}", pAward.MiniActiveness);
                AwardItem.AwardButtonState = awardState;
                AwardItem.TurnID = pAward.Id;
            }
        }// end for

        m_ItemGrid.repositionNow = true;
        m_ItemGrid.sorted = true;
    }

    public void UpdateAwardItemState(int nTurnID)
    {
        ActivenessAwardItem[] Item = m_ItemGrid.GetComponentsInChildren<ActivenessAwardItem>();
        for (int i = 0; i < Item.Length; ++i)
        {
            if (Item[i].TurnID == nTurnID)
            {
                Tab_ActivenessAward pAward = TableManager.GetActivenessAwardByID(nTurnID, 0);
                if (pAward == null)
                {
                    LogModule.DebugLog("ActivenessAward: ActivenessAward.txt can't find line " + nTurnID);
                    return;
                }
                AwardState awardState = AwardState.AWARD_CANNNTHAVE;
                bool bFlag = GameManager.gameManager.PlayerDataPool.AwardActivityData.GetActivenessAwardFlag(nTurnID);
                if (bFlag == false && Activeness > pAward.MiniActiveness)
                {
                    awardState = AwardState.AWARD_CANHAVE;
                }
                else if (bFlag == true)
                {
                    awardState = AwardState.AWARD_HAVEDONE;
                }
                else
                {
                    awardState = AwardState.AWARD_CANNNTHAVE;
                }
                Item[i].AwardButtonState = awardState;
                return;
            }
        }
    }

    
}
