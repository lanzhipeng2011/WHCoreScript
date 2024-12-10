using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
#if  !UNITY_IPHONE
public static class PlatformTalkingData
{
    static TDGAAccount Account;
    public static Dictionary<string, Func<object, object>> Get()
    {
        return new Dictionary<string, Func<object, object>>()
        {
            //AppStart
            {
               "AppStart",
               (val) =>
               {
                   NGUILogHelpler.Log("PlatformHelper.AppStart", "PlatformHelper");
                   TalkingDataGA.OnStart("4B78CA63F312A6C7F9BA638D84C7133F", "Default");
                   return null;
               }
            },
            //AppQuit
            {
                "AppQuit", (val) => { TalkingDataGA.OnEnd(); return null; }
            },

            //MissionBegin
            {
                "MissionBegin", (missionName)=> { TDGAMission.OnBegin((string)missionName); return null;}
            },
            //MissionCompleted
            {
                "MissionCompleted", (missionName)=> {   TDGAMission.OnCompleted((string)missionName); return null;}
            },
            //MissionFaild
            {
                "MissionFaild", (missionName)=> { TDGAMission.OnFailed((string)missionName,"放弃"); return null; }
            },
            //OnPurchase
            {
                "OnPurchase",
                (args)=>
                {
                     object[] arglst =  (object[])args;
                     TDGAItem.OnPurchase((string)arglst[0],(int)arglst[1],(double)arglst[2]);
                    return null;
                }
            },
            //RoleEnterGame
            {
                "RoleEnterGame",
                //string strAccountID, string strRoleType, string strRoleName, int RoleLevel
                (args)=>
                 {
                    object[] arglst =  (object[])args;
                    Account =  TDGAAccount.SetAccount((string)arglst[0]);
                    Account.SetAccountName((string)arglst[1]+"-"+ (string)arglst[2]);
                    Account.SetLevel((int)arglst[3]);
                    string serverName = LoginData.GetServerListDataByID(PlayerPreferenceData.LastServer).m_name;
                    Account.SetGameServer(serverName);
                    return null;
                }
            },
            //UpdateRoleInfo
            {
                "UpdateRoleInfo",
                (info)=> {if(Account!=null) { Account.SetLevel((int)info); }  return null;}
            },
            //OnChargeRequest
            {
               "OnChargeRequest",
                (args) =>
                {
                    NGUILogHelpler.Log("Talkingdata OnChargeRequest","PlatformHelper");
                    object[] arglst =  (object[])args;
                    TDGAVirtualCurrency.OnChargeRequest((string)arglst[0],(string)arglst[1],(double)arglst[2],(string)arglst[3],(double)arglst[4],(string)arglst[5]);
                    return null;
                }
            },
            //OnChargeSuccess
            {
                "OnChargeSuccess",
                (orderId)=> { TDGAVirtualCurrency.OnChargeSuccess((string)orderId);  return null; }
            }
        };
    }
}
#endif