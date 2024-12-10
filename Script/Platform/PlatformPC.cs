using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GCGame.Table;
public static class PlatformPC
{
   
   
    public static Dictionary<string, Func<object, object>> Get()
    {
        return new Dictionary<string, Func<object, object>>()
        {
            {"ClickEnterGame",(fn)=> {((System.Action)fn)();  return null; } },
            //ClickEsc
            { "ClickEsc" , (v)=> {  MessageBoxLogic.OpenOKCancelBox("确定退出游戏吗？", "提示信息", () => { Application.Quit();});  return null; } },
            /* ;*/
            //OnInitPlatform
            {
                "OnInitPlatform", (callfn)=>
                {
                    if(callfn!=null)
                    {
                        ((Action<int,string>)callfn)(0,"");
                    }
                    
                    return null;
                }
            },
            //SetAccountData
            {
                "SetAccountData",(val)=>
                {
                    int tempId = PlayerPrefs.GetInt("tempId");
                    string StrAccount;
                    StrAccount = SystemInfo.deviceUniqueIdentifier.Substring(0, 6) + tempId.ToString();
		            PlayerPrefs.SetString ("CurrPlayerID", StrAccount);
                    LoginData.AccountData accountData = (LoginData.AccountData)val;
                    accountData.SetTestData(StrAccount);
                    return null;
                }
            },
            //GetChannelID
            {
                "GetChannelID",(v)=> { return new object[]{ "TEST" ,false}; }
            },
            //ReqPaymentGoodInfoList
            {
                "ReqPaymentGoodInfoList",(v)=>
                {
                    RechargeData.m_dicGoodInfos = new Dictionary<string, RechargeData.GoodInfo>();
                    int[] price = new int[] {60,300,600,1980,3880,6480,80,250};

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
                   //string orderId, string iapId, float price, string priceType, float goldNumber, string payType
                   object[] args = (object[])v;
                   CG_ADDPAY_TEST msg = (CG_ADDPAY_TEST)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ADDPAY_TEST);
                   msg.SetObjId(0);
					msg.SetPayvalue((uint)((float)args[2]));
                   msg.SendPacket();
                   return null;
                }
            }
        };
    }

  
}
