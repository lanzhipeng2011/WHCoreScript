﻿using UnityEngine;
using System.Collections;
using System;
#if  !UNITY_IPHONE
/// <summary>
/// 接收 SDK 的回调消息，进行处理。
/// 该脚本应关联到每一个场景的 "Main Camera" 对象，以能接收SDK回调的消息。
/// 下面各方法中的逻辑处理，在游戏中应修改为真实的逻辑。
/// </summary>
public class UCCallbackMessage : MonoBehaviour
{
    public Action<int,string> InitResultEvent;
    public Action<string, float, string, string> PaySuccuceEvent;

    public void OnUCGameSdkCallback (string jsonstr)
	{
		log ("UCCallbackMessage - OnUCGameSdkCallback message: jsonstr=" + jsonstr);
      

        JsonData json = JsonMapper.ToObject (jsonstr);
		string callbackType = (string)json ["callbackType"];
		int code = (int)json ["code"];
		JsonData data = json ["data"];

		switch (callbackType) {
		case UCConstants.CALLBACKTYPE_InitSDK:
			OnInitResult (code, (string)data);
			break;

		case UCConstants.CALLBACKTYPE_Login:
			OnLoginResult (code, (string)data);
			break;

		case UCConstants.CALLBACKTYPE_Logout:
			OnLogout (code, (string)data);
			break;

		case UCConstants.CALLBACKTYPE_FloatMenu:
			OnFloatMenu (code, (string)data);
			break;

		case UCConstants.CALLBACKTYPE_UserCenter:
			OnUserCenter (code, (string)data);
			break;

		case UCConstants.CALLBACKTYPE_EnterUI:
			OnEnterUI (code, (string)data);
			break;

		case UCConstants.CALLBACKTYPE_Pay:
			OnPayCallback (code, data);
			break;

		case UCConstants.CALLBACKTYPE_UPointCharge:
			OnUPointCharge (code, (string)data);
			break;

		case UCConstants.CALLBACKTYPE_IsUCVip:
			OnIsUCVipResult (code, (bool)data);
			break;

		case UCConstants.CALLBACKTYPE_GetUCVipInfo:
			OnGetUCVipInfoResult (code, data);
			break;

		case UCConstants.CALLBACKTYPE_ExitSDK:
			OnExitSDK (code, (string)data);
			break;

		}


	}

	private void OnInitResult (int code, string msg)
	{
        if (InitResultEvent != null)
        {
            InitResultEvent(code,msg);
        }
      
		log (string.Format ("UCCallbackMessage - OnInitResult: code={0}, msg={1}", code, msg));
		//输出初始化结果到页面(接入后删除)
		//SendMessage ("setMessage", string.Format ("UCCallbackMessage - OnInitResult: code={0}, msg={1}", code, msg));
		
		if (code == UCStatusCode.SUCCESS)
        {
			log ("init succeeded");
            
            log("UCGameSdk.login");          
        }
        else
        {
			log (string.Format ("Failed initing UC game sdk, code={0}, msg={1}", code, msg));
			//初始化失败处理
		}
	}

	private void OnLoginResult (int code, string msg)
	{
		log (string.Format ("UCCallbackMessage - OnLoginResult: code={0}, msg={1}", code, msg));
		
		//输出登录结果到页面(接入后删除)
		//SendMessage ("setMessage", string.Format ("UCCallbackMessage - OnLoginResult: code={0}, msg={1}", code, msg));
		
		if (code == UCStatusCode.SUCCESS) {
			string sid = UCGameSdk.getSid ();
            
			log ("login succeeded: sid=" + sid);


			//
			UCConfig.logined = true;
            UCGameSdk.createFloatButton();
            //显示悬浮按钮
            UCGameSdk.showFloatButton(100, 20, true);


		} else if (code == UCStatusCode.LOGIN_EXIT) {
			//登录界面退出，返回到游戏画面
			log ("login UI exit, back to game UI");
		} else {
			log (string.Format ("Failed login, code={0}, msg={1}", code, msg));

			//登录失败
			//
		}
	}

	private void OnLogout (int code, string msg)
	{
		log (string.Format ("UCCallbackMessage - OnLogout: code={0}, msg={1}", code, msg));
		
		//输出退出登录结果到页面(接入后删除)
		//GameMain.setSdkMessage (string.Format ("UCCallbackMessage - OnLogout: code={0}, msg={1}", code, msg));
		
		if (code == UCStatusCode.SUCCESS) {
			//当前登录用户已退出，应将游戏切换到未登录的状态。

			UCConfig.logined = false;
			UCGameSdk.destroyFloatButton ();
			//DemoControl.Restart();
		} else {
			//unknown error
			log (string.Format ("unknown error: code={0}, msg={1}", code, msg));
		}
	}

	private void OnFloatMenu (int code, string msg)
	{
		log (string.Format ("UCCallbackMessage - OnFloatMenu: code={0}, msg={1}", code, msg));
		
		//输出悬浮菜单进入页面前后状态信息到页面(接入后删除)
		//GameMain.setSdkMessage (string.Format ("UCCallbackMessage - OnFloatMenu: code={0}, msg={1}", code, msg));
		
		if (code == UCStatusCode.SDK_OPEN) {
			//打开了SDK界面
			log ("user opened the SDK UI");
		} else if (code == UCStatusCode.SDK_CLOSE) {
			//SDK界面已关闭，回到游戏画面，游戏应根据实际需要决定是否进行画面刷新
			log ("user closed the SDK UI, backed to game UI");
		}
	}

	private void OnPayCallback (int code, JsonData jsonOrder)
	{
		log (string.Format ("UCCallbackMessage - OnPayCallback: code={0}", code));
		
		//输出支付回调信息到页面(接入后删除)
		//GameMain.setSdkMessage (string.Format ("UCCallbackMessage - OnPayCallback: code={0}", code));
	
		if (code == UCStatusCode.SUCCESS) {
			string orderId = (string)jsonOrder ["orderId"];			
			int payWayId = (int)jsonOrder ["payWayId"];
			string payWayName = (string)jsonOrder ["payWayName"];
			
			float orderAmount = 0;
	
			JsonData jdAmount = (JsonData)jsonOrder ["orderAmount"];
			switch(jdAmount.GetJsonType()) 
			{
			case JsonType.Int:				
				orderAmount = (float)(int)jdAmount;			
				break;
			case JsonType.Double:				
				orderAmount = (float)(double)jdAmount;			
				break;
			case JsonType.String:
				try
				{
				
					orderAmount = (float)Convert.ToDouble((string)jdAmount);
				}
				catch(Exception e)
				{
					log ("order amount is not a valid number");
				}
				break;
			default:
				log ("order amount is not a valid json number");
				break;
			}
			
			//充值下单成功，游戏应对下单结果进行处理，一般需把订单号、下单金额、支付渠道ID、支付渠道名称等信息上传到游戏服务器进行保存
			log (string.Format ("UCCallbackMessage - received order info: code={0}, orderId={1}, orderAmount={2:0.00}, payWayId={3}, payWayName={4}",
					code, orderId, orderAmount, payWayId, payWayName));

			//游戏根据需要进行订单处理，一般需要把订单号传回游戏服务器，在服务器上保存
		} else if (code == UCStatusCode.PAY_USER_EXIT) {
			//充值界面已关闭，回到游戏画面，游戏应根据实际需要决定是否进行画面刷新
		} else {
			//充值调用失败
		}

	}

	private void OnUserCenter (int code, string msg)
	{
		log (string.Format ("UCCallbackMessage - OnUserCenter: code={0}, msg={1}", code, msg));		
		//输出将要退出九游社区（个人中心）结果到页面(接入后删除)
		//GameMain.setSdkMessage (string.Format ("UCCallbackMessage - OnUserCenter: code={0}, msg={1}", code, msg));	
		
		if (code == UCStatusCode.SUCCESS) { 
			//用户退出了九游社区（个人中心）界面，返回游戏画面，游戏应根据实际需要决定是否进行画面刷新
			log ("user closed the user center UI, backed to game UI");
		} else {
			//fail
			log (string.Format ("unknown error: code={0}, msg={1}", code, msg));
		}
	}

	private void OnUPointCharge (int code, string msg)
	{
		log (string.Format ("UCCallbackMessage - OnUPointCharge: code={0}, msg={1}", code, msg));
		//输出U点充值页面返回结果到页面(接入后删除)
		//GameMain.setSdkMessage (string.Format ("UCCallbackMessage - OnUPointCharge: code={0}, msg={1}", code, msg));
		if (code == UCStatusCode.SDK_CLOSE) {
			//U点充值完成，返回游戏画面，游戏应根据实际需要决定是否进行画面刷新
			log ("user closed the user center UI, backed to game UI");
		} else if (code == UCStatusCode.NO_INIT) {
			log ("not inited");
		} else if (code == UCStatusCode.NO_LOGIN) {
			log ("not logined");
		} else {
			//unknown error
			log (string.Format ("unknown error: code={0}, msg={1}", code, msg));
		}
	}

	private void OnEnterUI (int code, string msg)
	{
		log (string.Format ("UCCallbackMessage - OnEnterUI: code={0}, msg={1}", code, msg));
		//输出进入的页面信息到页面(接入后删除)
		//GameMain.setSdkMessage (string.Format ("UCCallbackMessage - OnEnterUI: code={0}, msg={1}", code, msg));
	}

	private void OnIsUCVipResult (int code, bool isUCVip)
	{
		log (string.Format ("UCCallbackMessage - OnIsUCVipResult: code={0}, isUCVip={1}", code, isUCVip));
		//输出是否是UC会员结果到页面(接入后删除)
		//GameMain.setSdkMessage (string.Format ("UCCallbackMessage - OnIsUCVipResult: code={0}, isUCVip={1}", code, isUCVip));
		if (code == UCStatusCode.SUCCESS) {
			//成功获得结果
			//isUCVip
		} else {
			//fail
			log (string.Format ("failed get whether or not current user is a UC VIP member: code={0}", code));
		}
	}

	private void OnGetUCVipInfoResult (int code, JsonData jsonUCVipInfo)
	{
		log (string.Format ("UCCallbackMessage - OnGetUCVipInfoResult: code={0}, data={1}", code, jsonUCVipInfo.ToJson ()));
		//将当前用户的会员信息输出到页面(接入后删除)
		//GameMain.setSdkMessage (string.Format ("UCCallbackMessage - OnGetUCVipInfoResult: code={0}, data={1}", code, jsonUCVipInfo.ToJson ()));
		if (code == UCStatusCode.SUCCESS) {
			//成功获得UC会员信息
			int status = (int)jsonUCVipInfo ["status"];
			int grade = (int)jsonUCVipInfo ["grade"];
			string validFrom = (string)jsonUCVipInfo ["validFrom"];
			string validTo = (string)jsonUCVipInfo ["validTo"];

			log (string.Format ("status={0}, grade={1}, validFrom={2}, validTo={3}", status, grade, validFrom, validTo));

			JsonData privilegeList = jsonUCVipInfo ["privilegeList"];
			if (privilegeList.IsArray) {
				int size = privilegeList.Count;
				JsonData privilege;

				int enjoy = 0;
				int pId = 0;

				for (int i = 0; i < size; i++) {
					privilege = privilegeList [i];
					enjoy = (int)privilege ["enjoy"];
					pId = (int)privilege ["pId"];

					log (string.Format ("privilege list[{0}]: enjoy={1}, pId={2}", i, enjoy, pId));
				}
			}

		} else {
			//fail
			log (string.Format ("failed get current user's UC VIP member info: code={0}", code));
		}


	}

	private void OnExitSDK (int code, string msg)
	{

		log (string.Format ("UCCallbackMessage - OnExitSDK: code={0}, msg={1}", code, msg));
		//输出进入的页面信息到页面(接入后删除)
		if (code == UCStatusCode.SDK_EXIT) {
			//exitSDK
			Application.Quit ();

		} else {
			//fail
		}
	}
	private void log (string msg)
	{
        //Debug.Log(msg);
        NGUILogHelpler.Log(msg, "PlatformHelper");
    }




}
#endif