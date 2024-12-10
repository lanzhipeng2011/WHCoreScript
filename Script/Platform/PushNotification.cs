using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GCGame.Table;
using Module.Log;
class PushNotification
{
    //存储服务器过来的推送消息
    public struct SPushNotificationInfo
    {
        public string m_szNews;
        public int m_nData;
        public int m_nRepeatType;
    };
    public static List<SPushNotificationInfo> PushNotificationInfoList;
    public static void addPushNotificationInfo(string news, int data,int RepeatType)
    {

        if (PushNotificationInfoList == null)
        {
            PushNotificationInfoList = new List<SPushNotificationInfo>();
        }
        SPushNotificationInfo info;
        info.m_szNews = news;
        info.m_nData = data;
        info.m_nRepeatType = RepeatType;
        PushNotificationInfoList.Add(info);
//         LogModule.DebugLog("addPushNotificationInfo"
//                      + "info.m_szNews:" + info.m_szNews.ToString() + "info.m_nData:" + info.m_nData.ToString() + "info.m_nRepeatType:" + info.m_nRepeatType.ToString());
    }
    public static void ClearPushNotificationInfo()
    {
        if (PushNotificationInfoList != null)
        {
            PushNotificationInfoList.Clear();
        }
    }
    
       //服务器通知的推送
    public static void NotificationMessage2Server()
    {
        if (PushNotificationInfoList != null)
        {
            for (int i = 0; i < PushNotificationInfoList.Count; i++)
            {
#if UNITY_ANDROID
				AndroidNotificationMessage(PushNotificationInfoList[i].m_szNews, PushNotificationInfoList[i].m_nData, PushNotificationInfoList[i].m_nRepeatType);
#elif UNITY_IPHONE
				NotificationMessage(PushNotificationInfoList[i].m_szNews, PushNotificationInfoList[i].m_nData, PushNotificationInfoList[i].m_nRepeatType);
#endif    
//                 LogModule.DebugLog("NotificationMessage2Server"
//                      + "PushNotificationInfoList[i].m_szNews:" + PushNotificationInfoList[i].m_szNews.ToString()
//                      + "PushNotificationInfoList[i].m_nData:" + PushNotificationInfoList[i].m_nData.ToString()
//                      + "PushNotificationInfoList[i].m_nRepeatType:" + PushNotificationInfoList[i].m_nRepeatType.ToString());
            }
        }
     }
    public static void NotificationMessage2Clinet()
    {
        foreach (int key in TableManager.GetPushNotification().Keys)
        {
            Tab_PushNotification tablePush = TableManager.GetPushNotificationByID(key, 0);
            if (tablePush != null)
            {
#if UNITY_ANDROID
				AndroidNotificationMessage(tablePush.News, tablePush.Date, tablePush.Repeat);
#elif UNITY_IPHONE
                NotificationMessage(tablePush.News, tablePush.Date, tablePush.Repeat);
#endif
            }
        }

        //
        if (RestaurantData.m_PlayerRestaurantInfo != null && 
           RestaurantData.m_PlayerRestaurantInfo.m_VisitFrindList != null)
        {
            int nRestaurantPush = 0;      //判断饭店是否推送
            for (int i = 0; i < RestaurantData.m_PlayerRestaurantInfo.m_Desks.Length; i++)
            {
                if (RestaurantData.m_PlayerRestaurantInfo.m_Desks[i].m_DestState != RestaurantData.DeskState.PrepareFood)
                {
                    continue;
                }
                int nLeftTime = RestaurantData.m_PlayerRestaurantInfo.m_Desks[i].GetFoodLeftTime();
                if (nLeftTime > 0 && nLeftTime > nRestaurantPush && PlayerPreferenceData.SystemIsPushRestaurant)
                {
                    nRestaurantPush = nLeftTime + 1800; //CD为30分
                    string news = StrDictionary.GetClientDictionaryString("#{3190}");
                    System.DateTime newDate = System.DateTime.Now.AddSeconds(nLeftTime);
                    int date = newDate.Month * 1000000 + newDate.Day * 10000 + newDate.Hour * 100 + newDate.Minute;
#if UNITY_ANDROID
				AndroidNotificationMessage(news,date,0);
#elif UNITY_IPHONE
                    NotificationMessage(news, date, 0);
#endif
                }

            }
        }
   
    }
    //本地推送
    public static void NotificationMessage(string news, int date, int RepeatType)
    {       
        int year = System.DateTime.Now.Year;
        int month = System.DateTime.Now.Month;
        int day = System.DateTime.Now.Day;
        int hour = date / 100 % 100;
        int minute = date % 100;
        if (RepeatType == 0)
        {
            month = date / 1000000;
            day = date / 10000 % 100;
            System.DateTime newDate = new System.DateTime(year, month, day, hour, minute, 0);
            if (newDate > System.DateTime.Now)
            {
                PushNotification.NotificationMessage(news, newDate, 0);
            }    
        }
        else if (RepeatType == 1)
        {
            System.DateTime newDate = new System.DateTime(year, month, day, hour, minute, 0);
            if (newDate > System.DateTime.Now)
            {
                PushNotification.NotificationMessage(news, newDate, 1);
            }
            else
            {
                PushNotification.NotificationMessage(news, newDate.AddDays(1), 1);
            }
        }
        else if (RepeatType == 2)
        {
            System.DateTime newDate = new System.DateTime(year, month, day, hour, minute, 0);

            int nNewWeek = date / 10000 % 100;
            int nWeek = System.Convert.ToInt16(System.DateTime.Now.DayOfWeek);
            int num = Math.Abs(nNewWeek - nWeek);

            if (newDate > System.DateTime.Now)
            {
                PushNotification.NotificationMessage(news, newDate.AddDays(num), 2);
            }
            else
            {
                PushNotification.NotificationMessage(news, newDate.AddDays(num+7), 2);
            }                     
        }
    }
    //本地推送 你可以传入一个固定的推送时间
    public static void NotificationMessage(string news, System.DateTime newDate, int RepeatType)
	{
		//推送时间需要大于当前时间
		if(newDate > System.DateTime.Now)
		{
            #if UNITY_IPHONE 
            LocalNotification localNotification = new LocalNotification();
            localNotification.fireDate = newDate;
            localNotification.alertBody = news;
            localNotification.applicationIconBadgeNumber = 1;
            localNotification.hasAction = true;
            if (RepeatType == 1)
            {
                //是否每天定期循环
                localNotification.repeatCalendar = CalendarIdentifier.ChineseCalendar;
                localNotification.repeatInterval = CalendarUnit.Day;
            }
            else if (RepeatType == 2)
            {
                localNotification.repeatCalendar = CalendarIdentifier.ChineseCalendar;
                localNotification.repeatInterval = CalendarUnit.Week;
            }
            localNotification.soundName = LocalNotification.defaultSoundName;                
            NotificationServices.ScheduleLocalNotification(localNotification);
            #elif UNITY_ANDROID
				
			#endif
        }
	}
    //清空所有本地消息
    public static void CleanNotification()
    {
        #if UNITY_IPHONE 
        LocalNotification Notifcation = new LocalNotification();
        Notifcation.applicationIconBadgeNumber = -1;
        NotificationServices.PresentLocalNotificationNow(Notifcation);
        NotificationServices.CancelAllLocalNotifications();
        NotificationServices.ClearLocalNotifications();
        #elif UNITY_ANDROID
			AndroidHelper.doSdk("cleanNotification","");
        #endif
    }
	//组装android消息推送
	public static void AndroidNotificationMessage(string news, int date, int RepeatType)
	{
#if !UNITY_WP8
		JsonData data = new JsonData();
		data["news"] = news;
		data["date"] = date;
		data["repeatType"] = RepeatType;
		PlatformHelper.Notification(data.ToJson());
#endif
	}
}

