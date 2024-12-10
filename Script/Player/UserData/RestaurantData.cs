using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
using System.Collections.Generic;
using Module.Log;
using System;
using GCGame;

public class RestaurantData
{

    public const int GuestCountMax = 4;     // 每个餐桌最大客人数量
    public const int DeskCountMax = 10;     // 每个酒楼餐桌数量
    public const int FinishFoodMax = 5;     // 每日上菜最大次数
    public const int FoodLevelMax = 5;     // 菜品的最高等级

    public static int m_restaurantTipsCount = 0;

    // 餐桌状态，与服务器对应
    public enum DeskState
    {
        None = 0,
        PrepareFood = 1,
        EatFood = 2,
        WaitBilling = 3,
    }

    public class DeskInfo
    {
        public bool         m_IsActive  = false;                                        // 餐桌是否激活
        public int          m_DeskFinishTime = 0;                                       // 餐桌计时器
        public int[]        m_GuestIDs  = new int[GuestCountMax] { -1, -1, -1, -1 };    // 客人ID
        public DeskState    m_DestState = DeskState.None;                               // 状态
        public int          m_FoodID = -1;                                              // 食材的ID

        public void Rest()
        {
            m_IsActive = false;
            m_DeskFinishTime = 0;
//            for (int i = 0; i < m_GuestIDs.Length; i++)
//            {
//                m_GuestIDs[i] = -1;
//            }
            m_DestState = DeskState.None;
            m_FoodID = -1;
        }

        // 根据餐桌定时器返回本次状态结束时间差
        public string GetLeftTime()
        {
            int nLeftTime = GetFoodLeftTime();
            if ( nLeftTime <= 0 )
            {
                nLeftTime = 1;
            }
            return Utils.GetTimeDiffFormatString(nLeftTime);
        }

        public int GetFoodLeftTime()
        {
            return (m_DeskFinishTime - (int)Time.realtimeSinceStartup);
        }
    }

    // 酒楼结构体
    public class RestaurantInfo
    {
        public RestaurantInfo()
        {
            for (int i = 0; i < m_Desks.Length; i++)
            {
                m_Desks[i] = new DeskInfo();      
            }
        }

        public int m_RestaurantExp = 0;                             // 酒楼经验
        public int m_RestaurantLevel = 1;                           // 酒楼等级
        public ulong m_FrinedGuid = 0;                              // 当前好友酒楼好友GUID,玩家酒楼忽略
        public DeskInfo[] m_Desks = new DeskInfo[DeskCountMax];     // 餐桌
        public string m_MasterName;
        public List<UInt64> m_VisitFrindList = new List<UInt64>();
    }

    public static RestaurantInfo m_PlayerRestaurantInfo = new RestaurantInfo();
    public static RestaurantInfo m_FriendRestaurantInfo = new RestaurantInfo();

    // 更新酒楼信息
    public static void UpdateRestaurantInfo(RestaurantInfo curRestaurant, GC_RESTAURANT_UPDATE data)
    {
        curRestaurant.m_RestaurantExp = data.RestaurantExp;
        curRestaurant.m_RestaurantLevel = data.RestaurantLevel;
        curRestaurant.m_MasterName = data.Name;

        if (data.HasFriendGuid)
        {
            curRestaurant.m_FrinedGuid = data.FriendGuid;
        }

        // 先重置
        foreach (DeskInfo curDesk in curRestaurant.m_Desks)
        {
            curDesk.Rest();
        }
        int nRestaurantTipCount = 0;
        for (int i = 0; i < data.activeDeskIndexsCount; i++)
        {
            if (data.activeDeskIndexsList[i] >= DeskCountMax)
            {
                LogModule.ErrorLog("desk index is more than 10", i.ToString());
                break;
            }
            if (data.actionFinishTimersCount <= i)
            {
                LogModule.ErrorLog("food timer list length is less than desk count ", data.actionFinishTimersCount.ToString());
                break;
            }

//            if (data.deskGuestIDsCount <= i * GuestCountMax)
//            {
//                LogModule.ErrorLog("guest id count is less than desk count ", data.deskGuestIDsCount.ToString());
//                break;
//            }

            DeskInfo curDesk = curRestaurant.m_Desks[data.activeDeskIndexsList[i]];
            curDesk.m_IsActive = true;
            curDesk.m_DeskFinishTime = data.actionFinishTimersList[i] + (int)Time.realtimeSinceStartup;
//            for (int guestIndex = 0; guestIndex < GuestCountMax; guestIndex++)
//            {
//                int nDestGuestIDsListIndex = 4 * i + guestIndex;
//                if (guestIndex >= curDesk.m_GuestIDs.Length || nDestGuestIDsListIndex >= data.deskGuestIDsList.Count)
//                {
//                    LogModule.ErrorLog("guestIndex or nDestGuestIDsListIndex error");
//                    break;
//                }
   //         curDesk.m_GuestIDs[guestIndex] = data.deskGuestIDsList[nDestGuestIDsListIndex];
//            }

            curDesk.m_DestState = (DeskState)data.deskStatesList[i];
            curDesk.m_FoodID = data.GetDeskFoodIDs(i);
            if (RestaurantData.DeskState.WaitBilling == curDesk.m_DestState)
            {
                nRestaurantTipCount++;
            }
        }
        if (!data.HasFriendGuid)
        {
            //PlayerPreferenceData.RestaurantTipCount = nRestaurantTipCount;
            m_restaurantTipsCount = nRestaurantTipCount;
            if (MenuBarLogic.Instance() != null)
            {
                MenuBarLogic.Instance().UpdateRestaurantTips();
            }
            if (PlayerFrameLogic.Instance() != null)
            {
                PlayerFrameLogic.Instance().UpdateRemainNum();
            }
        }
    }

    // 更新餐桌信息
    public static void UpdateDeskInfo(RestaurantInfo curRestaurant, GC_RESTAURANT_DESTUPDATE data)
    {
        int curDeskIndex = data.DestIndex;
        if (curDeskIndex >= curRestaurant.m_Desks.Length)
        {
            LogModule.ErrorLog("desk index is big than define " + curDeskIndex);
            return;
        }

        DeskInfo curDeskData = curRestaurant.m_Desks[curDeskIndex];
        curDeskData.m_IsActive = true;
        curDeskData.m_DestState = (DeskState)data.DeskState;
        if (curDeskData.m_DestState == DeskState.EatFood)
        {
//            for (int i = 0; i < data.deskGuestIDsCount; i++)
//            {
//                curDeskData.m_GuestIDs[i] = data.deskGuestIDsList[i];
//            }
        }
        if ( data.HasDeskFoodID )
        {
            curDeskData.m_FoodID = data.DeskFoodID;
        }
        else
        {
            curDeskData.m_FoodID = -1;
        }
        curDeskData.m_DeskFinishTime = (data.HasActionFinishTimer ? data.ActionFinishTimer : 0) +(int)Time.realtimeSinceStartup;
        int nRestaurantTipCount = 0;
        for (int i = 0; i < m_PlayerRestaurantInfo.m_Desks.Length; i++)
        {
            DeskInfo oDeskData = m_PlayerRestaurantInfo.m_Desks[i];
            if (oDeskData.m_IsActive && oDeskData.m_DestState == DeskState.WaitBilling)
            {
                nRestaurantTipCount++;
            }
        }
        if (!data.HasFriendGuid)
        {
            m_restaurantTipsCount = nRestaurantTipCount;
            
            if (MenuBarLogic.Instance() != null)
            {
                MenuBarLogic.Instance().UpdateRestaurantTips();
            }
            if (PlayerFrameLogic.Instance() != null)
            {
                PlayerFrameLogic.Instance().UpdateRemainNum();
            }
        }
    }


    public delegate void UpdatePlayerDataDelegate();
    public static UpdatePlayerDataDelegate delUpdatePlayerData;
    public static UpdatePlayerDataDelegate delUpdateFriendData;

    public static UpdatePlayerDataDelegate delUpdatePlayerDeskData;
    public static UpdatePlayerDataDelegate delUpdateFriendDeskData;

    public static void UpdatePlayerData(GC_RESTAURANT_UPDATE data)
    {
        if (data.HasFriendGuid)
        {
            UpdateRestaurantInfo(m_FriendRestaurantInfo, data);
            if (null != delUpdateFriendData) delUpdateFriendData();
        }
        else
        {
            UpdateRestaurantInfo(m_PlayerRestaurantInfo, data);
            if (null != delUpdatePlayerData) delUpdatePlayerData();
        }
    }

    public static void UpdatePlayerDeskData(GC_RESTAURANT_DESTUPDATE data)
    {
        if ( data.HasFriendGuid )
        {
            UpdateDeskInfo(m_FriendRestaurantInfo, data);
            if (null != delUpdateFriendDeskData) delUpdateFriendDeskData();
        }
        else
        {
            UpdateDeskInfo(m_PlayerRestaurantInfo, data);
            if (null != delUpdatePlayerDeskData) delUpdatePlayerDeskData();
        }
    }

    public static void UpdateRestaurantLevel(GC_RESTAURANT_LEVELUPDATE data)
    {
        m_PlayerRestaurantInfo.m_RestaurantLevel = data.RestaurantLevel;
        m_PlayerRestaurantInfo.m_RestaurantExp = data.RestaurantExp;
        if (null != delUpdatePlayerData) delUpdatePlayerData();
    }

    public static void CleanRestaurantTip()
    {
        m_restaurantTipsCount = 0;
        //m_belleActiveCount = 0;
        if (MenuBarLogic.Instance() != null)
        {
            MenuBarLogic.Instance().UpdateRestaurantTips();
        }
        if (PlayerFrameLogic.Instance() != null)
        {
            PlayerFrameLogic.Instance().UpdateRemainNum();
        }
    }
}
