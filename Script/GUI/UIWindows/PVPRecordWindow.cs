using UnityEngine;
using System.Collections;
using GCGame.Table;
using System;
using GCGame;

public class PVPRecordWindow : MonoBehaviour {

	public GameObject RecordListGrid;
	// Use this for initialization
	void Start ()
	{
		RecordListGrid.GetComponent<UIGrid>().repositionNow = true;	
	}

    void OnEnable()
    {
        UpdateHistoryList();
        PVPData.delegateUpdatePvPRecordList += UpdateHistoryList;
    }
    void OnDisable()
    {
        PVPData.delegateUpdatePvPRecordList -= UpdateHistoryList;
    }
    
    void UpdateHistoryList()
    {
        UIManager.LoadItem(UIInfo.PVPRecordListItem, OnLoadHistoryListItem);
    }

    void OnLoadHistoryListItem(GameObject resItem, object param)
    {
        Utils.CleanGrid(RecordListGrid);
        int nIndex = 0;
        for (int i = 0; i < PVPData.ChallengeHistory.Count; ++i)
        {
            DateTime startTime = new DateTime(1970, 1, 1);
            DateTime sendDate = new DateTime((long)PVPData.ChallengeHistory[i].occurTime * 10000000L + startTime.Ticks, DateTimeKind.Unspecified);
            sendDate = sendDate.ToLocalTime();
            string strtime = sendDate.ToString("yyyy-MM-dd HH:mm:ss");
            int pos = PVPData.ChallengeHistory[i].rankPos + 1;
            if (PVPData.ChallengeHistory[i].isActive == 0)
            {
                string history = StrDictionary.GetClientDictionaryString("#{1227}", strtime, PVPData.ChallengeHistory[i].opponentName, pos);
                PVPRecordListItem.CreateItem(RecordListGrid, resItem, nIndex.ToString(), this, history);
            }
            else
            {
                string history = StrDictionary.GetClientDictionaryString("#{1228}", strtime, PVPData.ChallengeHistory[i].opponentName, pos);
                PVPRecordListItem.CreateItem(RecordListGrid, resItem, i.ToString(), this, history);
            }
            ++nIndex;
        }

        RecordListGrid.GetComponent<UIGrid>().Reposition();
        RecordListGrid.GetComponent<UITopGrid>().Recenter(true);
    }
}
