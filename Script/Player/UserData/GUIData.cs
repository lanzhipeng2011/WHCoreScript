/********************************************************************
	created:	2013/12/26
	created:	26:12:2013   11:42
	filename: 	GUIData.
	author:		王迪
	
	purpose:	用于处理UI所需数据，作为网络与本地数据中转
*********************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GCGame.Table;
using System;

public class GUIData 
{
    // GCNotify消息处理
    private static List<string> NotifyDataList = new List<string>();
    public static string GetNotifyData()
    {
        string news = null;
        if (NotifyDataList.Count > 0)
        {            
            news = NotifyDataList[0];
            NotifyDataList.RemoveAt(0);
        }
        return news;
       // return NotifyDataList.Count > 0 ? NotifyDataList.Dequeue() : null;
    }
    public static void AddNotifyData(string data, bool IsFilterRepeat = false)
    {
         string str = "";
         if (!string.IsNullOrEmpty(data))
         {
             char firstChar = data[0];
             if (firstChar != '#')
             {
                 str = data;
             }
             else
             {
                 str = StrDictionary.GetServerErrorString(data);
             }
         }
         if (NotifyDataList.Count > 0 && IsFilterRepeat)
        {
            if (NotifyDataList[NotifyDataList.Count - 1] != str)
            {
                 NotifyDataList.Add(str);
            }
        }
        else
        {
            NotifyDataList.Add(str);
        }
       
        //NotifyDataQueue.Enqueue(str);
    }
    public static void AddNotifyData2Client(bool IsFilterRepeat, string data, params object[] args)
    {
        string str = "";
        if (!string.IsNullOrEmpty(data))
        {
            char firstChar = data[0];
            if (firstChar != '#')
            {
                str = data;
            }
            else
            {
                str = StrDictionary.GetClientDictionaryString(data, args);
            }
        }
        if (NotifyDataList.Count > 0 && IsFilterRepeat)
        {
            if (NotifyDataList[NotifyDataList.Count - 1] != str)
            {
                NotifyDataList.Add(str);
            }
        }
        else
        {
            NotifyDataList.Add(str);
        }
       //NotifyDataQueue.Enqueue(str);
    }

    public delegate void FriendDataUpdateDelegate();
    public static FriendDataUpdateDelegate delFriendDataUpdate;

    public delegate void PlayerFindResultDelegate(GC_FINDPLAYER packet);
    public static PlayerFindResultDelegate delPlayerFindResult;

    public delegate void NearbyTeamUpdateDelegate(GC_NEAR_TEAMLIST packet);
    public static NearbyTeamUpdateDelegate delNearbyTeampUpdate;

    public delegate void NearbyPlayerUpdateDelegate(GC_NEAR_PLAYERLIST packet);
    public static NearbyPlayerUpdateDelegate delNearbyPlayerUpdate;

    public delegate void TeamDataUpdateDelegate();
    public static TeamDataUpdateDelegate delTeamDataUpdate;

    public delegate void MoneyChanged();
    public static MoneyChanged delMoneyChanged;

    public delegate void GuildDataUpdateDelegate();
    public static GuildDataUpdateDelegate delGuildDataUpdate;

    public delegate void GuildMemberSelectChangeDelegate(UInt64 selectGuid);
    public static GuildMemberSelectChangeDelegate delGuildMemberSelectChange;

    public delegate void MasterDataUpdateDelegate();
    public static MasterDataUpdateDelegate delMasterDataUpdate;

    public delegate void MasterMemberSelectChangeDelegate(UInt64 selectGuid, string selectName);
    public static MasterMemberSelectChangeDelegate delMasterMemberSelectChange;

    public delegate void MasterReserveMemberSelectChangeDelegate(UInt64 selectGuid, string selectName);
    public static MasterReserveMemberSelectChangeDelegate delMasterReserveMemberSelectChange;

    public delegate void MasterSelectChangeDelegate(UInt64 selectGuid);
    public static MasterSelectChangeDelegate delMasterSelectChange;
}
