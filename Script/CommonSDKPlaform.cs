using UnityEngine;
using System.Collections;
using ProtoCmd;
using System.Collections.Generic;
using ProtoBuf.Serializers;
using System;
using Games.GlobeDefine;


public class PayDatas {        
	public uint rmb = 0;
	public string ext_data = "";   
	public string out_order = "";    
	public string notice_url = "";
	public string inner_order = ""; 
	public string goods_id = "";
	public string subTime = "";
	public string description = "";
	public string productName = "";
	public int productRealPrice = 0;
	public int productIdealPrice = 0;
	public int productCount = 1;
}

public class CommonSDKPlaform:MonoBehaviour
{
	class loginResult {        
		public string token = ""; //session id
		public string uid = "";   //username or user id
		public uint pid = 0;    //platform id
		public string szLoginDataEx = "";
//		public string account = "";//for zqgame sdk
//		public string szLoginAcccount = ""; //login return
//		public string szLoginSession = "";
//		public string szLoginDataEx = "";
//		public string uiLoginPlatUserID = "";
	}

    public void ClearLoginData()
    {
        this.m_loginResult.token = "";
        this.m_loginResult.uid = "";
    }
	
	
	public static CommonSDKPlaform Instance { private set; get; }
	
	#if CommonSDK
	//mono初始化会直接调用这个来初始化sdk
	AndroidJavaObject mainApplication = null;
	public AndroidJavaObject sdkBase = null;
	AndroidJavaObject sdkObject = null;
	AndroidJavaObject gameActivity = null;

	AndroidJavaObject feedbackSdk = null;
	AndroidJavaObject shareSdk = null;
	AndroidJavaObject umengPush = null;
	AndroidJavaObject umengAnalytics = null;

	#endif
	//static string tempText = "msg";
	static string sendText = "edit";
	
	loginResult m_loginResult = new loginResult();//登陆数据

	public PayDatas payDatas = new PayDatas();

	void Start () 
	{
		//设置屏幕自动旋转， 并置支持的方向
		Screen.orientation = ScreenOrientation.AutoRotation;
		Screen.autorotateToLandscapeLeft = true;
		Screen.autorotateToLandscapeRight = true;
		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;
        GameObject.DontDestroyOnLoad(gameObject);
	}
	//mono初始化会直接调用这个来初始化sdk
	void Awake()
	{        
		#if CommonSDK
//		mainApplication = new AndroidJavaClass("com.talkingsdk.MainApplication").CallStatic<AndroidJavaObject>("getInstance");
//		sdkBase = mainApplication.Call<AndroidJavaObject>("getSdkInstance");
//		sdkBase.Call("setUnityGameObject", gameObject.name);
		Instance = this;
		
//		Debug.LogError("Awake:" + gameObject.name);
		#endif
	}

	public void InitSDK()
	{
		#if CommonSDK
		mainApplication = new AndroidJavaClass("com.talkingsdk.MainApplication").CallStatic<AndroidJavaObject>("getInstance");
		sdkBase = mainApplication.Call<AndroidJavaObject>("getSdkInstance");
		sdkBase.Call("setUnityGameObject", gameObject.name);
		Debug.LogError("InitSDK:" + gameObject.name);
		#endif
	}
	
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			//Debug.LogError("Input.GetKeyDown(KeyCode.Escape)");
			KeyBack();
		}
		/*
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Debug.LogError("Input.GetKeyUp(KeyCode.Escape)");
            //KeyBack();
        }


        if (Input.GetKey(KeyCode.Home))
        {
            Debug.LogError("KeyCode.Home");
            KeyBack();
        }
         */
	} 
	
	/// <summary>
	/// 登陆接口
	/// </summary>
	public void Login()
	{
		Debug.LogError("go to login....");
		#if CommonSDK
		sdkBase.Call("login");
		#endif
	}
	
	/// <summary>
	/// 微信登陆接口
	/// </summary>
	public void LoginWeiXin()
	{
		Debug.LogError("go to login....");
		#if CommonSDK
		sdkBase.Call("loginWeiXin");
		#endif
	}
	
	/// <summary>
	/// 问题反馈
	/// </summary>
	public void ProblemFeedback()
	{
		Debug.LogError("go to ProblemFeedback....");
		#if CommonSDK
		feedbackSdk.Call("openFeedback");
		#endif
	}
	/// <summary>
	/// 推送（友盟）开启
	/// </summary>
	public void NotificationStart()
	{
		Debug.LogError("go to NotificationStart....");
		#if CommonSDK
		umengPush.Call("startPush");
		#endif
	}
	/// <summary>
	/// 推送（友盟）关闭
	/// </summary>
	public void NotificationStop()
	{
		Debug.LogError("go to NotificationStop....");
		#if CommonSDK
		umengPush.Call("stopPush");
		#endif
	}
	/// <summary>
	/// 分享（友盟）
	/// </summary>
	public void UserShareing()
	{
		Debug.LogError("go to UserShareing....");
		#if CommonSDK

		AndroidJavaObject shareParams = new AndroidJavaClass("com.talkingsdk.ShareParams");
		shareParams.Call("setShareMode","0,1,2,3,4,5");//分享方式 0:微信 1：朋友圈 2：新浪微博 3：QQ 4：腾讯微博 5:QQ空间
		shareParams.Call("setTitle","标题");
		shareParams.Call("setContent","内容");
		shareParams.Call("setSourceUrl","内容链接");
		shareParams.Call("setImgUrl","图片链接");

		umengPush.Call("stopPush");
		#endif
	}
	
	
	
	/// <summary>
	/// 通知SDK创建角色
	/// </summary>
	/// <param name="name"></param>
	public void CreateRoleToSDK(string playerName)
	{ 
		#if CommonSDK
//		if (m_loginResult.pid == 34 || m_loginResult.pid == 30)
//		{
//			sdkBase.Call("createRole", name);
//			Debug.LogError("go to UserUpLever  setServerNo:" + LoginMgr.GetInstance().m_zone.ToString() + "   setLevel:" + 1+ "   setRoleName:" + playerName);        
//			AndroidJavaObject playerData = new AndroidJavaObject("com.talkingsdk.models.PlayerData");
//			playerData.Call("setServerNo", LoginMgr.GetInstance().m_zone.ToString());
//			playerData.Call("setLevel", (int)1);
//			playerData.Call("setRoleName", playerName);
//			
//			//PPS没有这个，鱼丸有这个，以后接口要统一整理一遍。
//			if(m_loginResult.pid == 30)
//				playerData.Call("setServerName", "狼人归来");
//			
//			sdkBase.Call("createRole", playerData);
//		}

		//========
		LoginData.PlayerRoleData curRoleData = LoginData.GetPlayerRoleData(PlayerPreferenceData.LastRoleGUID);
		LoginData.ServerListData serverData = LoginData.GetServerListDataByID(PlayerPreferenceData.LastServer);

		AndroidJavaObject hashmap = new AndroidJavaObject("java.util.HashMap");
		System.IntPtr methodPut = AndroidJNIHelper.GetMethodID(hashmap.GetRawClass(), "put", "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");

		Dictionary<string, string> dict = new Dictionary<string, string>();
		dict.Add("roleCTime",curRoleData.RoleCreateTime.ToString()); //roleCTime

		object[] args = new object[2];
		foreach (KeyValuePair<string, string> kvp in dict)
		{
			AndroidJavaObject k = new AndroidJavaObject("java.lang.String", kvp.Key);
			AndroidJavaObject v = new AndroidJavaObject("java.lang.String", kvp.Value);
			args[0] = k;
			args[1] = v;
			AndroidJNI.CallObjectMethod(hashmap.GetRawObject(), methodPut, AndroidJNIHelper.CreateJNIArgArray(args));
		}

		AndroidJavaObject playerData = new AndroidJavaObject("com.talkingsdk.models.PlayerData");
		playerData.Call ("setServerNo", PlayerPreferenceData.LastServer.ToString());
		playerData.Call ("setServerName", serverData.m_name);
		playerData.Call("setRoleName",curRoleData.name);
		playerData.Call("setRoleId",(int)curRoleData.guid);
		playerData.Call("setLevel",(int)curRoleData.level);
		playerData.Call("setEx",hashmap);//payData.Call("setEx", hashmap);
		sdkBase.Call("createRole", playerData);
		//====end


		#endif
	}
	
	/// <summary>
	/// 支付接口
	/// </summary>
	public void Pay(RspPayForGoodsCommand payGoodsData)
	{
		#if CommonSDK
		AndroidJavaObject payData = new AndroidJavaObject("com.talkingsdk.models.PayData");
		AndroidJavaObject hashmap = new AndroidJavaObject("java.util.HashMap");
		System.IntPtr methodPut = AndroidJNIHelper.GetMethodID(hashmap.GetRawClass(), "put", "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");
		
		Debug.Log("m_loginResult.uid :" + m_loginResult.uid);
		//=============获取数据
		LoginData.PlayerRoleData curRoleData = LoginData.GetPlayerRoleData(PlayerPreferenceData.LastRoleGUID);
		LoginData.ServerListData serverData = LoginData.GetServerListDataByID(PlayerPreferenceData.LastServer);
		int nPlayerYuanBao = GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBao();
		int nPlayerVip = VipData.GetVipLv();
		int nPlayerLv = curRoleData.level;
		string nPlayerParty = GameManager.gameManager.PlayerDataPool.GuildInfo.GuildName;
		string nPlayerName = curRoleData.name;
		string nPlayerID = curRoleData.guid.ToString();
		string nPlayerServerName = serverData.m_name;
		int nPlayerServerID = serverData.m_id;
		//===========end

		Dictionary<string, string> dict = new Dictionary<string, string>();
		
		dict.Add("UserBalance", nPlayerYuanBao.ToString()); //用户余额
		dict.Add("UserGamerVip", nPlayerVip.ToString()); //vip 等级
		dict.Add("UserLevel", nPlayerLv.ToString()); //角色等级
		dict.Add("UserPartyName",nPlayerParty); //工会，帮派
		dict.Add("UserRoleName", nPlayerName); //角色名称
		dict.Add("UserRoleId", nPlayerID); //角色id
		dict.Add("UserServerName", nPlayerServerName); //服务器名字
		dict.Add("UserServerId", PlayerPreferenceData.LastServer.ToString()); //服务器ID
       
		dict.Add("GameMoneyAmount", (payDatas.rmb * 10).ToString());//		dict.Add("GameMoneyAmount", (payGoodsData.pay_info.rmb /10).ToString()); //充值游戏币数量，不含赠送金额 请按(支付金额*人民币与游戏币兑换率，例:6元买60钻石,此处填60)
		dict.Add("GameMoneyName", "元宝");
		dict.Add("UserId", m_loginResult.uid); //uid
		dict.Add("LoginAccount", m_loginResult.uid);//		dict.Add("LoginAccount", m_loginResult.szLoginAcccount);
		dict.Add("LoginDataEx", m_loginResult.szLoginDataEx);
		dict.Add("LoginSession", m_loginResult.token);//		dict.Add("LoginSession", m_loginResult.szLoginSession);
		//dict.Add("AccessKey", payDatas.ext_data); //dict.Add("AccessKey", System.Text.Encoding.UTF8.GetString(payGoodsData.pay_info.ext_data)); //签名（对应服务端sign）
		//dict.Add("OutOrderID", payDatas.out_order);//dict.Add("OutOrderID", System.Text.Encoding.UTF8.GetString(payGoodsData.pay_info.out_order)); //平台订单号
		//dict.Add("NoticeUrl", payDatas.notice_url);//dict.Add("NoticeUrl", System.Text.Encoding.UTF8.GetString(payGoodsData.pay_info.notice_url)); //支付回调地址
		
		object[] args = new object[2];
		foreach (KeyValuePair<string, string> kvp in dict)
		{
			AndroidJavaObject k = new AndroidJavaObject("java.lang.String", kvp.Key);
			AndroidJavaObject v = new AndroidJavaObject("java.lang.String", kvp.Value);
			args[0] = k;
			args[1] = v;
			AndroidJNI.CallObjectMethod(hashmap.GetRawObject(), methodPut, AndroidJNIHelper.CreateJNIArgArray(args));
		}
//		Debug.LogError("go to pay....  setMyOrderId " +System.Text.Encoding.UTF8.GetString(payGoodsData.pay_info.inner_order)+
//		               " payGoodsData.pay_info.out_order: " + System.Text.Encoding.UTF8.GetString(payGoodsData.pay_info.out_order) +
//		               " rmb: " + payGoodsData.pay_info.rmb +
//		               " setProductCount :" + payGoodsData.pay_info.goods_num+
//		               " setProductId :"+ payGoodsData.pay_info.goods_id+
//		               " setProductName :" + getProductionNameByID((int)payGoodsData.pay_info.goods_id) +
//		               " setSubmitTime:" + System.Text.Encoding.UTF8.GetString(payGoodsData.pay_info.inner_order).Substring(0, 14) +
//		               " extData:" + System.Text.Encoding.UTF8.GetString(payGoodsData.pay_info.ext_data)
//		               );
		
		//string 类型
		payData.Call("setMyOrderId", payDatas.inner_order);//System.Text.Encoding.UTF8.GetString(payGoodsData.pay_info.inner_order));
		payData.Call("setProductId", payDatas.goods_id);//payGoodsData.pay_info.goods_id.ToString());
		payData.Call("setSubmitTime", payDatas.subTime);//System.Text.Encoding.UTF8.GetString(payGoodsData.pay_info.inner_order).Substring(0, 14));
		payData.Call("setDescription",payDatas.description);//getProductionNameByID((int)payGoodsData.pay_info.goods_id));
		payData.Call("setProductName", payDatas.productName);//getProductionNameByID((int)payGoodsData.pay_info.goods_id));
		
		//int 类型 ,SDK 这边统一以分为单位
		payData.Call("setProductRealPrice", (int)(payDatas.rmb* 100));//((int)payGoodsData.pay_info.rmb));
//		payData.Call("setProductIdealPrice", payDatas.rmb);// ((int)payGoodsData.pay_info.rmb));
		payData.Call("setProductCount", payDatas.productCount);//(int)payGoodsData.pay_info.goods_num);
		payData.Call("setEx", hashmap);
		
		sdkBase.Call("pay", payData);
		
		#endif
	}
	
	
	string getProductionNameByID(int id)
	{
		
		return "Recharge";
	}
	
	public void SetUserID(string id)
	{
		#if CommonSDK
		Debug.Log("CommonSDK Platform  SetUserID:"+id.ToString());
		m_loginResult.uid = id;
		#endif
	}
	
	
//	/// <summary>
//	/// 
//	/// </summary>
//	/// <param name="jsonData"></param>
//	public void SetLoginData(string jsonData)
//	{
//		/*
//        Result :{
//        "nPlatformID" : 0,
//        "szAcccount" : "whtest3",
//        "szDataEx" : "whtest3",
//        "szSession" : "",
//        "uiPlatUserID" : 0
//        }
//        */
//		
//		Debug.LogError("Result :" + jsonData);
//		
//		JsonData jd = JsonMapper.ToObject(UnderWorld.Utils.JSON.JsonEncode(UnderWorld.Utils.JSON.JsonDecode(jsonData)));
//		
//		/*
//        if (((IDictionary)jd).Contains("nPlatformID"))
//        {
//            Debug.LogError("nPlatformID :" + (jd["nPlatformID"]).ToString());
//        }
//        */
//		
//		
//		if (((IDictionary)jd).Contains("szAcccount"))
//		{
//			Debug.LogError("szAcccount :" + (jd["szAcccount"]).ToString());
//			m_loginResult.szLoginAcccount = (jd["szAcccount"]).ToString();            
//		}
//		
//		if (((IDictionary)jd).Contains("szDataEx"))
//		{
//			Debug.LogError("szDataEx :" + (jd["szDataEx"]).ToString());
//			m_loginResult.szLoginDataEx = (jd["szDataEx"]).ToString();  
//		}
//		
//		if (((IDictionary)jd).Contains("szSession"))
//		{
//			Debug.LogError("szSession :" + (jd["szSession"]).ToString());
//			m_loginResult.szLoginSession = (jd["szSession"]).ToString();  
//		}
//		
//		if (((IDictionary)jd).Contains("uiPlatUserID"))
//		{
//			Debug.LogError("uiPlatUserID :" + (jd["uiPlatUserID"]).ToString());
//			m_loginResult.uiLoginPlatUserID = (jd["uiPlatUserID"]).ToString();  
//		}
//		
//		
//	}
	
	public uint GetPlatformID(){
		return m_loginResult.pid;
	}

	public string GetUID(){
		return m_loginResult.uid;
	}

	/// <summary>
	/// 显示Tool Bar
	/// </summary>
	public void ShowToolBar()
	{
		#if CommonSDK
		Debug.LogError("go to showToolBar....");
		sdkBase.Call("showToolBar");
		#endif
	}
	
	/// <summary>
	/// 隐藏Tool Bar
	/// </summary>
	public void DestroyToolBar()
	{
		#if CommonSDK
		Debug.LogError("go to destroyToolBar....");
		sdkBase.Call("destroyToolBar");
		#endif
	}
	
	/// <summary>
	/// 显示用户中心
	/// </summary>
	public void ShowUserCenter()
	{
		#if CommonSDK
		Debug.LogError("go to showUserCenter....");
		sdkBase.Call("showUserCenter");
		#endif
	}
	
	
	/// <summary>
	/// 切换帐号
	/// </summary>
	public void ChangeAccount()
	{
		#if CommonSDK
		Debug.LogError("go to change account....");
		sdkBase.Call("changeAccount");
		//GameMain.Instance.LogoutAccount();
		#endif
	}
	
	
	/// <summary>
	/// 登出
	/// </summary>
	public void Logout()
	{
        ClearLoginData();
		#if CommonSDK
		Debug.LogError("go to logout....");
		sdkBase.Call("logout");

		DestroyToolBar ();
		#endif
	}
	
	
	/// <summary>
	/// 进入游戏回调
	/// </summary>
	/// <param name="serverNo"></param>
	/// <param name="playerID"></param>
	/// <param name="playerName"></param>
	public void EnterGame(string serverNo, int playerID, string playerName)
	{
		
		Debug.Log("go to EnterGam  serverNo: " + serverNo + "   setRoleId:" + playerID.ToString() + "   playerName:" + playerName);
		#if CommonSDK

		LoginData.ServerListData serverData = LoginData.GetServerListDataByID(PlayerPreferenceData.LastServer);
		LoginData.PlayerRoleData curRoleData = LoginData.GetPlayerRoleData(PlayerPreferenceData.LastRoleGUID);

		//============
		AndroidJavaObject hashmap = new AndroidJavaObject("java.util.HashMap");
		System.IntPtr methodPut = AndroidJNIHelper.GetMethodID(hashmap.GetRawClass(), "put", "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");
		
		Dictionary<string, string> dict = new Dictionary<string, string>();
		dict.Add("roleCTime",curRoleData.RoleCreateTime.ToString()); //roleCTime
		
		object[] args = new object[2];
		foreach (KeyValuePair<string, string> kvp in dict)
		{
			AndroidJavaObject k = new AndroidJavaObject("java.lang.String", kvp.Key);
			AndroidJavaObject v = new AndroidJavaObject("java.lang.String", kvp.Value);
			args[0] = k;
			args[1] = v;
			AndroidJNI.CallObjectMethod(hashmap.GetRawObject(), methodPut, AndroidJNIHelper.CreateJNIArgArray(args));
		}
		//==============end

		AndroidJavaObject playerData = new AndroidJavaObject("com.talkingsdk.models.PlayerData");
		playerData.Call ("setServerNo", PlayerPreferenceData.LastServer.ToString());
		playerData.Call ("setServerName", serverData.m_name);
		playerData.Call("setRoleName",curRoleData.name);
		playerData.Call("setRoleId",(int)curRoleData.guid);
		playerData.Call("setLevel",(int)curRoleData.level);
		playerData.Call("setEx",hashmap);
		sdkBase.Call("enterGame", playerData);

		#endif
	}
	
	
	/// <summary>
	/// 玩家升级
	/// </summary>
	public void UserUpLever(string serverNo, int level, string playerName)
	{
		Debug.LogError("go to UserUpLever  setServerNo:" + serverNo + "   setLevel:" + level.ToString() + "   setRoleName:" + playerName);
		#if CommonSDK
//		AndroidJavaObject playerData = new AndroidJavaObject("com.talkingsdk.models.PlayerData");
//		playerData.Call("setServerNo", serverNo);
//		playerData.Call("setLevel", level);
//		playerData.Call("setRoleName", playerName);

		LoginData.ServerListData serverData = LoginData.GetServerListDataByID(PlayerPreferenceData.LastServer);
		LoginData.PlayerRoleData curRoleData = LoginData.GetPlayerRoleData(PlayerPreferenceData.LastRoleGUID);

		//============
		AndroidJavaObject hashmap = new AndroidJavaObject("java.util.HashMap");
		System.IntPtr methodPut = AndroidJNIHelper.GetMethodID(hashmap.GetRawClass(), "put", "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");
		Dictionary<string, string> dict = new Dictionary<string, string>();
		dict.Add("roleCTime",curRoleData.RoleCreateTime.ToString()); //roleCTime
		object[] args = new object[2];
		foreach (KeyValuePair<string, string> kvp in dict)
		{
			AndroidJavaObject k = new AndroidJavaObject("java.lang.String", kvp.Key);
			AndroidJavaObject v = new AndroidJavaObject("java.lang.String", kvp.Value);
			args[0] = k;
			args[1] = v;
			AndroidJNI.CallObjectMethod(hashmap.GetRawObject(), methodPut, AndroidJNIHelper.CreateJNIArgArray(args));
		}
		//==============end

		AndroidJavaObject playerData = new AndroidJavaObject("com.talkingsdk.models.PlayerData");
		playerData.Call ("setServerNo", PlayerPreferenceData.LastServer.ToString());
		playerData.Call ("setServerName", serverData.m_name);
		playerData.Call("setRoleName",curRoleData.name);
		playerData.Call("setRoleId",(int)curRoleData.guid);
		playerData.Call("setLevel",(int)curRoleData.level);
		playerData.Call("setEx",hashmap);
		sdkBase.Call("userUpLevel", playerData);

		//=========统计升级
		if(umengAnalytics != null)
			umengAnalytics.Call("levelup", level);
		//===============
		#endif
	}
	
	
	/// <summary>
	/// 设置分数
	/// </summary>
	public void SetRankScore(string sorce, string rank)
	{
		//目前rank字段是没用的，暂时先传个OK过去，以后游泳的时候再打开
		Debug.LogError("SetRankScore:" + sorce + "  rank:" + rank);
		#if CommonSDK
		sdkBase.Call("uploadScore", sorce, rank);
		#endif
	}
	
	
	/// <summary>
	/// 社交分享 - 韩国SDK加的接口，只有韩国版才会有
	/// </summary>
	public void SocietyShare(string url, string name, string desc, string picName)
	{
		Debug.LogError("go to KakaoShare....");
		#if CommonSDK && UNITY_ANDROID
		sdkBase.Call("doShare", desc, url, name);
		#elif CommonSDK && UNITY_IPHONE		
		_KakaoShare(url, name, desc, picName);
		#endif
	}
	
	
	/// <summary>
	/// 显示游戏中心（这个与用户中心不是一个东西） - 韩国SDK加的接口，只有韩国版才会有
	/// </summary>
	public void ShowGameCenterInfo()
	{
		Debug.LogError("ShowGameCenterInfo....");
		#if CommonSDK && UNITY_ANDROID
		sdkBase.Call("showGameCenter", "1", "Test", "1", "1");
		#elif CommonSDK && UNITY_IPHONE		
		_ShowGameCenterInfo();
		#endif
	}
	
	
	/// <summary>
	/// 显示广告通知 - 韩国SDK加的接口，只有韩国版才会有
	/// </summary>
	public void ShowAdsNotice()
	{
		Debug.LogError("ShowAdsNotice....");
		#if CommonSDK && UNITY_ANDROID
		sdkBase.Call("showAdsNotice");
		#elif CommonSDK && UNITY_IPHONE		
		_ShowAdNotices();
		#endif
	}
	
	
	
	
	/// <summary>
	/// 销毁SDK进程
	/// </summary>
	public void DestroyActivity()
	{
		Debug.LogError("DestroyActivity");
		#if CommonSDK
		sdkBase.Call("onActivityDestroy");
		#endif
	}
	
	
	
	/// <summary>
	/// 监听返回事件
	/// </summary>
	public void KeyBack()
	{
		Debug.LogError("KeyBack");
		
		#if CommonSDK
		sdkBase.Call("onKeyBack");
		#endif
		
	}
	
	
	/// <summary>
	/// 获取登陆数据
	/// </summary>
	/// <returns></returns>
	public PlayerRequestLoginClientCmd GetLoginData()
	{
		PlayerRequestLoginClientCmd cmd = new PlayerRequestLoginClientCmd();
		cmd.platform_id =m_loginResult.pid;
		cmd.app_loginkey = m_loginResult.token;
		cmd.app_uid = m_loginResult.uid;
		cmd.account = m_loginResult.uid;
		cmd.internal_test = 1;
		cmd.session = System.Text.Encoding.UTF8.GetBytes(m_loginResult.token);
		return cmd;
	}
	
	
	/// <summary>
	/// SDK初始化完成
	/// </summary>
	/// <param name="test"></param>
	 
	 
	public Action<int,string> InitResultEvent;
	void OnInitComplete(string result)
	{
		Debug.LogError("OnInitComplete:" + result);
		JsonData jd = JsonMapper.ToObject(UnderWorld.Utils.JSON.JsonEncode(UnderWorld.Utils.JSON.JsonDecode(result)));
		m_loginResult.pid = uint.Parse((string)jd["PlatformId"]);
		//UmengPlatformHelp.UmengInit(m_loginResult.pid.ToString());//初始化友盟统计平台

		int code = (int)jd["code"];//int.Parse((string)jd["code"]);
		string msg = (string)jd ["msg"];
		if (InitResultEvent != null)
		{
			InitResultEvent(code,msg);
		}
		if (code == 1)
		{
//			log ("init succeeded");
//			
//			log("UCGameSdk.login");          
		}
		else
		{
//			log (string.Format ("Failed initing UC game sdk, code={0}, msg={1}", code, msg));
			//初始化失败处理
		}


		#if CommonSDK
		//============问题反馈初始化
		//feedbackSdk= new AndroidJavaClass("com.talkingsdk.plugin.ZQBFeedback").CallStatic<AndroidJavaObject>("getInstance");
		//=========end
		//=========分享（友盟）初始化
		//shareSdk= new AndroidJavaClass("com.talkingsdk.plugin.ZQBShare").CallStatic<AndroidJavaObject>("getInstance");
		//========end
		//=========推送（友盟）初始化
		//umengPush = new AndroidJavaClass("com.talkingsdk.plugin.ZQBPush").CallStatic<AndroidJavaObject>("getInstance");
		//========end
		//=========统计（友盟）初始化
		if(umengAnalytics == null)
			umengAnalytics= new AndroidJavaClass("com.talkingsdk.plugin.ZQBAnalytics").CallStatic<AndroidJavaObject>("getInstance");
		//========end
		#endif

	}



	/// <summary>
	/// 返回用户登录后的会话标识，此标识会在失效时刷新，游戏在每次需要使用该标识时应从SDK获取
	/// </summary>
	/// <returns>用户登录会话标识</returns>
	public string getSid ()
	{
//		log ("Unity3D getSid calling...");
//		
//		using (AndroidJavaClass cls = new AndroidJavaClass(SDK_JAVA_CLASS)) {
//			return cls.CallStatic<string> ("getSid");
//		}

		return m_loginResult.token;

	}

	
	/// <summary>
	/// 回调 - 登陆
	/// </summary>
	/// <param name="result"></param>
	void OnLoginResult(string result)
	{
		Debug.LogError("OnLoginSuccess:" + result);
		NGUILogHelpler.Log("result:"+result,"PlatformHelper");
		JsonData jd = JsonMapper.ToObject(UnderWorld.Utils.JSON.JsonEncode(UnderWorld.Utils.JSON.JsonDecode(result)));
		
		/*
        m_loginResult.pid = uint.Parse((string)jd["pid"]);
        m_loginResult.token = (string)jd["token"];
        m_loginResult.uid = (string)jd["uid"];
         */

		m_loginResult.token = (string)jd["SessionId"];
		string uid = (string)jd["UserId"];
		if (uid == "")
		{
			uid = (string)jd["UserName"];
		}
		m_loginResult.uid = uid;
		m_loginResult.szLoginDataEx = (string)jd["Ext"];

		//如果有额外的平台ID，则给它重新赋值
		if (((IDictionary)jd).Contains("Ext"))
		{
			JsonData ext = jd["Ext"];
			if (((IDictionary)ext).Contains("PlatformId"))
			{
				Debug.Log("PlatformId Reset :" + (string)ext["PlatformId"]);
				m_loginResult.pid = uint.Parse((string)ext["PlatformId"]);
			}
		}
		
		LoginMgr.GetInstance().ThirdlyPlatformLogin();
		
		Debug.LogError("UmengPlatformHelp.UmengInit ID:" + m_loginResult.pid.ToString());

		//======
		ShowToolBar ();
		//===end

		#if CommonSDK
		int pid = sdkBase.Call<int>("getPlatformId");
		m_loginResult.pid = (uint)pid;
		//=======统计登录
		if(umengAnalytics != null)
			umengAnalytics.Call("login", m_loginResult.uid);
		//========
		#endif


	}
	
	
	string payResult = "";
	string orderID = "";
	
	void OnPayResult(string result)
	{
		Debug.LogError("OnPayResult:" + result);
		
		Debug.LogError("OnPayResult:" + result);
		
		
		JsonData jd = JsonMapper.ToObject(UnderWorld.Utils.JSON.JsonEncode(UnderWorld.Utils.JSON.JsonDecode(result)));
		
		
		
		if (((IDictionary)jd).Contains("MyOrderId"))
		{
			orderID = (string)jd["MyOrderId"];
		}

		string StrPrice;
		int Numbers = 0;

		
		if (((IDictionary)jd).Contains("ProductIdealPrice"))
		{
			StrPrice = (string)jd["ProductIdealPrice"];
			Numbers = int.Parse(StrPrice);
		}
		//如果有额外的平台ID，则给它重新赋值
		if (((IDictionary)jd).Contains("Ext"))
		{
			JsonData ext = jd["Ext"];
			if (((IDictionary)ext).Contains("PayResult"))
			{
				Debug.Log("PlatformId Reset :" + (string)ext["PayResult"]);
				payResult = (string)ext["PayResult"];
			}
		}
		
		PayNetWork.GetInstance().SendMessagePayResultOrder(m_loginResult.pid, orderID, payResult);

		//============更新充值界面
		if(null != RechargeController.Instance())
		{
			RechargeController.Instance().UpdateRechargeList();
		}

		#if CommonSDK
		//======统计充值（友盟）
		if(umengAnalytics != null)
			umengAnalytics.Call("pay",(double)Numbers , Numbers);//金额 虚拟币数量
        #endif

	}
	
	
	/// <summary>
	/// 发送充值凭证，这个只用于验证测试。
	/// </summary>
	public void SendMessageToTestPayResultOrder()
	{
		if (orderID != "")
		{
			PayNetWork.GetInstance().SendMessagePayResultOrder(m_loginResult.pid, orderID, payResult);
		}
		else
		{
			Debug.Log("U have not recharge , please check again !");
		}
	}
	
	
	
	
	void OnLogoutResult(string result)
	{
		Debug.LogError("OnLogoutResult: " + result);
		NGUILogHelpler.Log("OnLogoutResult","PlatformHelper");
		NetManager.SendUserLogout();
		ClearLoginData();
		OnClearRole();
		//PlatformHelper.UserLogout();
		//UIManager.CloseUI(UIInfo.SystemAndAutoFight);

		#if CommonSDK
		//=======统计登出
		if(umengAnalytics != null)
			umengAnalytics.Call("logout");
		//========end
        #endif
		///TestMain.Instance.switchRole ();
	}

	public void OnClearRole()    
	{

		LoginUILogic.m_LoginSelect = 1;
		LoadingWindow.LoadScene(GameDefine_Globe.SCENE_DEFINE.SCENE_LOGIN);

	}

	
	void OnChangeAccountResult(string result)
	{
		Debug.Log("OnChangeAccount:" + result);
		///TestMain.Instance.switchRole();
	}
	
	
	void OnDestroy()
	{
		Debug.Log("CommonSDKPlatform OnDestroy");
		//Debug.LogError("GameMain destroy");
		//if (GameNetwork.Instance != null) 
		//{
		//    GameNetwork.Instance.OnGameDestroy();
		//}
		//===脚本继承下去不做销毁
//		#if CommonSDK
//		DestroyActivity();
//		#endif
		
	}
	
	/*
    void OnGUI()
    {
        //login
       
        if (GUI.Button(new Rect(200, 100, 300, 100), "Login"))
        {
            Login();
        }

        //if (GUI.Button(new Rect(200, 250, 300, 100), "Pay"))
       // {
        //    Pay();
        //}

        if (GUI.Button(new Rect(200, 400, 300, 100), "ChangeAccount"))
        {
            ChangeAccount();
        }

        if (GUI.Button(new Rect(200, 550, 300, 100), "Logout"))
        {
            Logout();
        }

        if (GUI.Button(new Rect(200, 700, 300, 100), "ShowToolBar"))
        {
            ShowToolBar();
        }

        if (GUI.Button(new Rect(200, 850, 300, 100), "DestroyToolBar"))
        {
            DestroyToolBar();
        }

        if (GUI.Button(new Rect(200, 1000, 300, 100), "ShowUserCenter"))
        {
            ShowUserCenter();
        }

        GUI.Label(new Rect(200, 1150, 300, 300), tempText);
    }

    */
	
	
	
	
}
