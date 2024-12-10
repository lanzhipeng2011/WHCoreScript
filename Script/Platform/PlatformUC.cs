using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GCGame.Table;
using Games.LogicObj;
#if !UNITY_IPHONE && !UNITY_EDITOR
public static class PlatformUC
{
    static bool isDebugMode = false;
    static int loglevel  = 2;
    static int cpid = 71200; 
    static int gameid = 727770;
    static int serverid = 0;
    static string serverName = "default";
    static bool payhistory = true;
    static bool enablelogout = false;
    static bool isInit = false;
    static UCCallbackMessage CurUCCallbackMessage;
    public static Dictionary<string, Func<object, object>> Get()
    {
        return new Dictionary<string, Func<object, object>>()
        {
            {"ClickEnterGame",(fn)=>
            {
                if(UCGameSdk.getSid()==""||UCGameSdk.getSid()==null)
                {
                    UCGameSdk.login(false,"");
                }
                else
                {
                  ((System.Action)fn)();
                }
                return null;
            }},
            //EnterSceneOK
            {"EnterSceneOK",(v)=> 
            {
                GameObject newMainCam = GameObject.Find("Main Camera");
                if(newMainCam!=null&&newMainCam.GetComponent<UCCallbackMessage>()==null)
                {
                    CurUCCallbackMessage = newMainCam.AddComponent<UCCallbackMessage>();
                    GameObject.Destroy(GameRoot.Inst.GetComponent<UCCallbackMessage>());
                }
                return null;
            }},
             //ClickEsc
            { "ClickEsc" , (v)=> {  UCGameSdk.exitSDK(); return null; } },
           //RoleEnterGame
            {
                "RoleEnterGame",
                //string strAccountID, string strRoleType, string strRoleName, int RoleLevel
                (args)=>
                 {
                    object[] arglst =  (object[])args;
                    //UCGameSdk.notifyZone (serverName,(string)arglst[0] ,(string)arglst[1]);
                   LoginData.ServerListData serverData = LoginData.GetServerListDataByID(PlayerPreferenceData.LastServer);
                   LoginData.PlayerRoleData curRoleData = LoginData.GetPlayerRoleData(PlayerPreferenceData.LastRoleGUID);
                   JsonData json = new JsonData();
                   json ["roleLevel"] = ((int)arglst[3]).ToString();
                   json ["roleId"] = PlayerPreferenceData.LastRoleGUID.ToString();
                   json ["roleName"] =(string)arglst[2];
                   json ["zoneId"] = PlayerPreferenceData.LastServer;
                   json ["zoneName"] =serverData.m_name;
                   json["roleCTime"]= (long)curRoleData.RoleCreateTime;
                   json["roleLevelMTime"]= -1;
                   string datastr = json.ToJson ();
                   
                   UCGameSdk.submitExtendData ("loginGameRole", datastr);
                    NGUILogHelpler.Log(datastr,"PlatformHelper");
                    return null;
                }
            },
            {
                "UpdateRoleInfo",
                (info)=> 
                {
                   LoginData.ServerListData serverData = LoginData.GetServerListDataByID(PlayerPreferenceData.LastServer);
                   LoginData.PlayerRoleData curRoleData = LoginData.GetPlayerRoleData(PlayerPreferenceData.LastRoleGUID);

                   JsonData json = new JsonData();
                   json ["roleLevel"] = ((int)info).ToString();
                   json ["roleId"] = PlayerPreferenceData.LastRoleGUID.ToString();
                   json ["roleName"] =curRoleData.name;
                   json ["zoneId"] = PlayerPreferenceData.LastServer;
                   json ["zoneName"] =serverData.m_name;
                   json["roleCTime"]=(long)curRoleData.RoleCreateTime;
                   json["roleLevelMTime"]= -1;
                   string datastr = json.ToJson ();
                     NGUILogHelpler.Log(datastr,"PlatformHelper");
                   UCGameSdk.submitExtendData ("loginGameRole", datastr);
                   return null;
                }
            },
            //OnLoadGame
            {
                "OnLoadGame",(val)=>
                {
                    
                    CurUCCallbackMessage = GameRoot.Inst.gameObject.AddComponent<UCCallbackMessage>();
                    NGUILogHelpler.Log(CurUCCallbackMessage.ToString(),"PlatformHelper");
                    return null;
                }
            },
            //OnInitPlatform
            {
                "OnInitPlatform", (callfn)=>
                {
                    if(isInit==false)
                    {
                     UCGameSdk.initSDK(isDebugMode,loglevel,cpid,gameid,serverid,serverName,payhistory,enablelogout);
                     UCGameSdk.setOrientation(UCConfig.orientation);
                     CurUCCallbackMessage.InitResultEvent = (Action<int,string>)callfn;
                     isInit = true;
                    }
                    else
                    {
                        ((Action<int,string>)callfn)(0,"");
                    }
                    return null;
                }
            },
            //UserLogin
            {"UserLogin",(val)=>
            {
                UCGameSdk.login(false, "");
                return null;
            }},
            { "UserLogout",(val)=>
            {
                UCGameSdk.logout();
                return null;
            }},
            //SetAccountData
            {
                "SetAccountData",(val)=>
                {
                    LoginData.AccountData accountData = (LoginData.AccountData)val;
                    accountData.SetTestData("");
                    accountData.m_validateInfo =  UCGameSdk.getSid();
                    return null;
                }
            },
            //GetChannelID
            {
                "GetChannelID",(v)=> { return new object[] { "ANDROID_UC" ,false}; }
            },
            //Pay?
            {
                "StartPay",(v)=>
                {
                    object[] args = (object[])v;
                    UCGameSdk.pay(false,(float)args[0],serverid,(string)args[1],(string)args[2],(string)args[3],(string)args[4],"http://182.92.161.252:8888/api/user?msg=1021");  return null;
                }
            },
            //ReqPaymentGoodInfoList
            {
                "ReqPaymentGoodInfoList",(v)=> 
                {
                    RechargeData.m_dicGoodInfos = new Dictionary<string, RechargeData.GoodInfo>();
                    int[] price = new int[] {60,300,600,1980,3880,6480};

                    Dictionary<int,List<Tab_Recharge>> rechDic = TableManager.GetRecharge();
                    int i=0;
                    foreach(KeyValuePair<int,List<Tab_Recharge>> item in rechDic)
                    {
                       RechargeData.GoodInfo goodInfo = new RechargeData.GoodInfo();
                       string key = item.Value[0].RegisterID;
                       goodInfo.goods_register_id  = key;
                       goodInfo.goods_price = price[i].ToString();
                       RechargeData.m_dicGoodInfos.Add(key,goodInfo);
                       i++;
                    }
                    return null;
                }
            },
            //OnChargeRequest
            {
                "OnChargeRequest",
                (v)=> 
                {
                    NGUILogHelpler.Log("UC OnChargeRequest","PlatformHelper");
                   //string orderId, string iapId, float price, string priceType, float goldNumber, string payType
                   object[] args = (object[])v;
                   string customStr="ServerId="+PlayerPreferenceData.LastServer + "#RoleId="+PlayerPreferenceData.LastRoleGUID;
                   PlayerData playerDataPool = GameManager.gameManager.PlayerDataPool;
                   NGUILogHelpler.Log("call pay?","PlatformHelper");
                    try {
                   UCGameSdk.pay(false,(float)args[2],serverid,PlayerPreferenceData.LastRoleGUID.ToString(),
                       playerDataPool.MainPlayerBaseAttr.RoleName,playerDataPool.MainPlayerBaseAttr.Level.ToString(),customStr
                       ,"http://182.92.161.252:8887/api/pay");
                    }
                    catch (System.Exception e)
                    {
                        NGUILogHelpler.Log(e.Message,"PlatformHelper");
                    }
                   return null;
                }
            }
        };
    }

  
}
#endif