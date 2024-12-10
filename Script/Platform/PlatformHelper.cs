using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Module.Log;
using System;
using Games.LogicObj;

public class PlatformHelper
{
    static List<Dictionary<string, Func<object, object>>> RegisterPlatforms; 
    public static void Init()
    {
        RegisterPlatforms = new List<Dictionary<string, Func<object, object>>>();
#if UNITY_ANDROID && !UNITY_EDITOR
#if UC
        RegisterPlatforms.Add(PlatformUC.Get());
#endif
#if TalkingData
        RegisterPlatforms.Add(PlatformTalkingData.Get());
#endif
#if PC
      RegisterPlatforms.Add(PlatformPC.Get());
#endif
     
#if LJ
     RegisterPlatforms.Add(PlatformLJ.Get());
#endif
#if CommonSDK
		RegisterPlatforms.Add(PlatformZQB.Get());
#endif
#else
        RegisterPlatforms.Add(PlatformPC.Get());
#endif
     
    }

    public static object CallPlatform(string action,object arg)
    {
        NGUILogHelpler.Log(action, "PlatformHelper");
        if(RegisterPlatforms!=null)
        for (int i=0;i<RegisterPlatforms.Count;i++)
        {
            var PlatDic = RegisterPlatforms[i];
            if (PlatDic.ContainsKey(action))
            {
                    var retVal = PlatDic[action](arg);
                    if (retVal != null)
                    {
                        object[] valArr = (object[])retVal;
                        if ((bool)valArr[1] == false) return valArr[0];
                    }
             }
        }
        return null;
    }


    public enum ChannelType
    {
        NONE,
        IOS_UNKNOWN,
        IOS_TEST,
        IOS_TY,
        IOS_APPSTORE,
        IOS_APPSTORE_TEST,
        IOS_PP,
        IOS_91,
        IOS_AS,
        IOS_XY,
        TEST,
        ANDROID_360,
        ANDROID_UC,
        ANDROID_XIAOMI,
        ANDROID_WDJ,
		ANDROID_BAIDU,
        ANDROID_OPPO,
        ANDROID_ANZHI,
        ANDROID_LENOVO,
        ANDROID_DOWNJOY,
        ANDROID_HUAWEI,
        ANDROID_EWAN,
		ANDROID_CYOU,
		ANDROID_VIVO,
		ANDROID_KUPAI,
		ANDROID_ZHANGYUE,
		ANDROID_PPS,
		ANDROID_MEIZU,
    }

    public static bool IsCYSDK()
    {
        return (PlatformHelper.GetChannelType() == PlatformHelper.ChannelType.TEST ||
                PlatformHelper.GetChannelType() == PlatformHelper.ChannelType.IOS_APPSTORE_TEST ||
                PlatformHelper.GetChannelType() == PlatformHelper.ChannelType.IOS_APPSTORE ||
                PlatformHelper.GetChannelType() == PlatformHelper.ChannelType.IOS_TEST ||
                PlatformHelper.GetChannelType() == PlatformHelper.ChannelType.IOS_TY);
    }

    // 游戏大版本
    private static int _getGameVersion()
    {
        return (int)VERSION.GameVersion;
    }

    // 程序版本
    private static int _getProgramVersion()
    {
        return (int)VERSION.ProgramVersion;
    }

    // 是否可以使用GM指令
    private static bool _isEnableGM()
    {
        return true;
    }

    // 是否可以自动锁屏
    private static void _setScreenCanAutoLock(bool bCanLock)
    {
    }

    // 资源更新地址
    private static string _getResDonwloadUrl()
    {
        return "http://10.161.21.42/res";//182.92.161.252/res";
        //return "";
    }

    // 是否开启资源更新
    private static bool _isEnableUpdate()
    {
        return false;
    }

 

    // 用户登出
    private static void _userLogout()
    {
    }

	// 推送
	private static void _notification(string news)
	{

	}

    // 进入用户中心
	private static void _enterUserCenter()
	{
		
	}

    //打开社会化分享界面
    private static void _showSocialShareCenter(string szShareContent)
    {

    }

	// 获取版本类型枚举
    private static string _getChannelString()
    {
        return ChannelType.TEST.ToString();
    }

    // 获取服务器列表地址
    private static string _getServerlistUrl()
    {
        return "0";
    }

    // 统计日志：角色进入游戏
    private static void _roleEnterGame(string strAccountID, string strRoleType, string strRoleName, int RoleLevel)
    {
    }

    // 统计日志：角色进入游戏
    private static void _onAccountLogin(string strServerID, string strUserID)
    {
     
    }

    // 支付
    private static void _makePay(string strRoleID, string groupID)
    {
        
    }

    // 获取包体更新地址
    private static string _getUpdateAppUrl()
    {
	return "http://10.161.21.42/res";//182.92.161.252/res";
	}

    // 获取设备唯一ID
    private static string _getDeviceUDID()
    {
        return "";
    }

    // 是否开启调试模式：左上角调试框，FPS
    private static bool _IsEnableDebugMode()
    {
        return true;
    }

    private static string _getMediaChannel()
    {
        return "";
    }

	private static int _getNetworkType()
	{
		return 1;
	}

	private static string _getDeviceType()
	{
		return "";
	}

	private static string _getDeviceVersion()
	{
		return "";
	}

	private static string _getChannelID()
	{
		return "";
	}


	private static void _sendUserAction(string strEvent)
	{

	}
     

	private static void _showCallCenter()
	{

	}

	private static void _makePayWithGoodInfo(string roleID, string serverID, string roleName,string goodID, string goodName, string goodNum, string price,string registerID)
	{

	}

	private static void _reqPaymentGoodInfoList()
	{
		
	}

	private static void _showRechargeRecord()
	{
		
	}

	private static void _startBanner()
	{
	}

	private static void _showBanner(bool bShow)
	{

	}

    private static bool _isEnableShareCenter()
    {
        return true;
    }


    // 游戏大版本
    public static int GetGameVersion()
    {
        return _getGameVersion();
    }

    // 程序版本
    public static int GetProgramVersion()
    {
        return _getProgramVersion();
    }

    // 是否可以使用GM指令
    public static bool IsEnableGM()
    {
        return true;
        //return _isEnableGM();
    }


    // 是否可以自动锁屏
    public static void SetScreenCanAutoLock(bool bCanLock)
    {
        _setScreenCanAutoLock(bCanLock);
    }

    // 资源更新地址
    public static string GetResDonwloadUrl()
    {
        return _getResDonwloadUrl();
    }

    // 是否开启资源更新
    public static bool IsEnableUpdate()
    {
        return _isEnableUpdate();
    }

    public static void EnterSceneOK()
    {
        CallPlatform("EnterSceneOK",null);
    }

    public static void SetAccountData(LoginData.AccountData accountData)
    {
        CallPlatform("SetAccountData", accountData);
    }
    // 用户登录
    public static void UserLogin()
    {
        CallPlatform("UserLogin",null); 
    }
    //进入游戏按钮
    public static void ClickEnterGame(System.Action fn)
    {
        CallPlatform("ClickEnterGame", fn);
    }
    // 用户登出
    public static void UserLogout()
    {
        CallPlatform("UserLogout", null); 
    }

	// 推送
	public static void Notification(string news)
	{

	}

	// 推送开启
	public static void NotificationStart(string news)
	{
		CallPlatform("NotificationStart", null); 
	}
	// 推送关闭
	public static void NotificationStop(string news)
	{
		CallPlatform("NotificationStop", null); 
	}
	// 分享
	public static void UserShareing(string news)
	{
		CallPlatform("UserShareing", null); 
	}
	// 问题反馈
	public static void ProblemFeedback(string news)
	{
		CallPlatform("ProblemFeedback", null);
	}



    public static void ChangeAccount()
    {
		//===暂时无用和登出重复
		//CallPlatform("ChangeAccount", null); 
    }

    // 获取版本类型枚举 (这个地方需要和王迪沟通下，看看是不是这样写的)
    public static ChannelType GetChannelType()
    {
        string strChannel = _getChannelString();
        if (strChannel == "IOSTest")
        {
            return ChannelType.IOS_TEST;
        }
        else if (strChannel == "AppStore")
        {
            return ChannelType.IOS_APPSTORE;
        }
        else if (strChannel == "TY")
        {
            return ChannelType.IOS_TY;
        }
        else if (strChannel == "AppStoreTest")
        {
            return ChannelType.IOS_APPSTORE_TEST;
        }
        else if (strChannel == "PP")
        {
            return ChannelType.IOS_PP;
        }
        else if (strChannel == "91")
        {
            return ChannelType.IOS_91;
        }
        else if (strChannel == "XY")
        {
            return ChannelType.IOS_XY;
        }
        else if (strChannel == "ANDROID_360")
        {
            return ChannelType.ANDROID_360;
        }
        else if (strChannel == "ANDROID_UC")
        {
            return ChannelType.ANDROID_UC;
		}
        else if (strChannel == "ANDROID_XIAOMI")
        {
            return ChannelType.ANDROID_XIAOMI;
        }
        else if (strChannel == "ANDROID_WDJ")
        {
            return ChannelType.ANDROID_WDJ;
        }
		else if (strChannel == "ANDROID_BAIDU")
		{
			return ChannelType.ANDROID_BAIDU;
		}
        else if (strChannel == "ANDROID_OPPO")
        {
            return ChannelType.ANDROID_OPPO;
        }
        else if (strChannel == "ANDROID_ANZHI")
        {
            return ChannelType.ANDROID_ANZHI;
        }
        else if (strChannel == "ANDROID_LENOVO")
        {
            return ChannelType.ANDROID_LENOVO;
        }
        else if (strChannel == "ANDROID_DOWNJOY")
        {
            return ChannelType.ANDROID_DOWNJOY;
        }
        else if (strChannel == "ANDROID_HUAWEI")
        {
            return ChannelType.ANDROID_HUAWEI;
        }
        else if (strChannel == "ANDROID_EWAN")
        {
            return ChannelType.ANDROID_EWAN;
        }
		else if (strChannel == "ANDROID_CYOU")
		{
			return ChannelType.ANDROID_CYOU;
		}
		else if (strChannel == "ANDROID_VIVO")
		{
			return ChannelType.ANDROID_VIVO;
		}
		else if (strChannel == "ANDROID_KUPAI")
		{
			return ChannelType.ANDROID_KUPAI;
		}
		else if (strChannel == "ANDROID_ZHANGYUE")
		{
			return ChannelType.ANDROID_ZHANGYUE;
		}
		else if (strChannel == "ANDROID_PPS")
		{
			return ChannelType.ANDROID_PPS;
		}
		else if (strChannel == "ANDROID_MEIZU")
		{
			return ChannelType.ANDROID_MEIZU;
		}
        else if (strChannel == "ANDROID_TEST")
        {
            return ChannelType.TEST;
        }
        else
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return ChannelType.IOS_UNKNOWN;
            }
        }

        return ChannelType.TEST;
    }

    // 获取服务器列表地址
    public static string GetServerListUrl()
    {
        string strServerListPath = "222";//_getServerlistUrl();
        if (string.Equals(strServerListPath, "0") || string.IsNullOrEmpty(strServerListPath))
        {
            //如果从平台URL获取错误，只好这么搞了
#if UNITY_ANDROID && !UNITY_EDITOR
            strServerListPath = Application.streamingAssetsPath + "/iplist.txt";
 
#elif UNITY_IPHONE && !UNITY_EDITOR
			strServerListPath ="file://" + Application.streamingAssetsPath + "/iplist.txt";
#else // IOS PC Editor
            strServerListPath = "file://" + Application.streamingAssetsPath + "/iplist.txt";
#endif
        }
        else
        {
            strServerListPath = "file://" + Application.streamingAssetsPath + "/iplist.txt";
           // + PlatformHelper.GetChannelID() + "/iplist.txt";//182.92.161.252/" + PlatformHelper.GetChannelID()+"/iplist.txt";
        }

        return strServerListPath;
    }

   
    public static void OnInitPlatform(System.Action<int,string> fn)
    {
        CallPlatform("OnInitPlatform",fn);
    }
    public static void OnChargeSuccess(string orderId)
    {
        CallPlatform("OnChargeSuccess",orderId);
    }
    public static void OnChargeRequest(string orderId, string iapId, float price, string priceType, float goldNumber, string payType)
    {
        CallPlatform("OnChargeRequest",new object[] { orderId, iapId , price, priceType, goldNumber, payType });
    }

    public static void MissionBegin(string missionName)
    {
        CallPlatform("MissionBegin",missionName);
    }

    public static void MissionCompleted(string missionName)
    {
       CallPlatform("MissionCompleted", missionName);
    }

    public static void MissionFaild(string missonName,string cause)
    {
        CallPlatform("MissionFaild", missonName);
    }
    /// <summary>
    ///购买虚拟物品
    /// </summary>
    /// <param name="name">物品唯一标识</param>
    /// <param name="number">数量</param>
    /// <param name="price">单价</param>
    public static void OnPurchase(string name,int number,float price)
    {
        CallPlatform("OnPurchase", new object[] {name,number,price });
    }

    /// <summary>
    /// 玩家获得虚拟货币
    /// </summary>
    /// <param name="rewNumber">金额</param>
    /// <param name="reason">获得原因</param>
    public static void OnReward(float rewNumber,string reason)
    {

    }
    public static void UpdateRoleInfo(int level)
    {
        CallPlatform("UpdateRoleInfo", level);
    }

    // 统计日志：角色进入游戏
    public static void RoleEnterGame(string strAccountID, string strRoleType, string strRoleName, int RoleLevel)
    {
        CallPlatform("RoleEnterGame", new object[] { strAccountID, strRoleType, strRoleName, RoleLevel });
    }

    // 统计日志：角色进入游戏
    public static void OnAccountLogin(string strServerID, string strUserID)
    {
        _onAccountLogin(strServerID, strUserID);
    }

    // 支付
    public static void MakePay()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (GetChannelType() == ChannelType.IOS_APPSTORE || GetChannelType() == ChannelType.IOS_APPSTORE_TEST)
            {

                return;
            }
        }
        else if (Application.platform == RuntimePlatform.Android)
        {

            return;
        }

        Obj_MainPlayer mainPlayer = Singleton<ObjManager>.Instance.MainPlayer;
        if (null != mainPlayer)
        {
            mainPlayer.SendNoticMsg(false, "#{2136}");
        }
    }

    // 请求商品列表
	public static void ReqPaymentGoodInfoList()
	{
        CallPlatform("ReqPaymentGoodInfoList",null);
	}

    // 使用GOODINFO方式支付
	public static bool MakePayWithGoodInfo(RechargeData.GoodInfo curGoodInfo)
	{
        if (null == curGoodInfo)
        {
            LogModule.ErrorLog("goodinfo null");
            return false;
        }
		LoginData.PlayerRoleData curRole = LoginData.GetPlayerRoleData(PlayerPreferenceData.LastRoleGUID);
		if(null == curRole)
		{
			LogModule.ErrorLog("get role data fail");
			return false;
		}
        _makePayWithGoodInfo(string.Format("{0:X16}", curRole.guid), 
            PlayerPreferenceData.LastServer.ToString(), curRole.name, curGoodInfo.goods_id, curGoodInfo.goods_name,curGoodInfo.goods_number, curGoodInfo.goods_price, curGoodInfo.goods_register_id);
		return true;
	}

    // 显示充值记录
	public static void ShowRechargeRecord()
	{
		_showRechargeRecord();
	}

    // 获取包体更新地址
    public static string GetUpdateAppUrl()
    {
        return _getUpdateAppUrl();
    }

    // 获取设备唯一ID
    public static string GetDeviceUDID()
    {
        return _getDeviceUDID();
    }

    // 是否开启调试模式：左上角调试框，FPS
    public static bool IsEnableDebugMode()
    {
        return _IsEnableDebugMode();
    }


    // 媒体渠道ID
    public static string GetMediaChannel()
    {
        return _getMediaChannel();
    }


    public enum NetworkState
    {
        NOTCONNECT = 0,
        STATE_WIFI = 1,
        STATE_3G = 2,
        STATE_2G = 3,
        STATE_UNKNOWN = 4,
    };

    // 获取网络状况
    public static NetworkState GetNetworkState()
    {
        switch (_getNetworkType())
        {
            case -1:
            case 0:
                return NetworkState.NOTCONNECT;
            case 1:
                return NetworkState.STATE_WIFI;
            case 2:
                return NetworkState.STATE_3G;
            case 3:
                return NetworkState.STATE_2G;
        }
        return NetworkState.STATE_UNKNOWN;
    }

    // 获取渠道标识
    public static string GetChannelID()
    {
       return (string)CallPlatform("GetChannelID",null);
    }

    // 获取设备类型
    public static string GetDeviceType()
    {
        LogModule.DebugLog(_getDeviceType());
        return _getDeviceType();
    }

    // 获取设备版本号
    public static string GetDeviceVersion()
    {
        LogModule.DebugLog(_getDeviceVersion());
        return _getDeviceVersion();
    }

    // 发送客户端自定义事件
    public static void SendUserAction(string strEvent)
    {
        _sendUserAction(strEvent);
    }

  

    public static void OnLoadGame()
    {
        CallPlatform("OnLoadGame",null);
    }
    public static void AppStart()
	{
        CallPlatform("AppStart", null);
    }

    public static void ClickEsc()
    {
        CallPlatform("ClickEsc", null);
    }
    public static void AppQuit()
    {
        CallPlatform("AppQuit", null);
    }

    // 进入用户中心
	public static void EnterUserCenter()
	{
		_enterUserCenter();
	}

    // 打开客服界面
	public static void ShowCallCenter()
	{
		_showCallCenter();
	}

    //打开社会化分享界面
    //参数为要分享的内容
    public static void ShowSocialShareCenter(string szShareContent)
    {
        if (GameManager.gameManager.PlayerDataPool.IsServerFlagOpen(SERVER_FLAGS_ENUM.FLAG_SNS))
        {
            _showSocialShareCenter(szShareContent);
        }
    }

	public static void StartAD()
	{
		_startBanner();
	}

	public static void ShowAD(bool bShow)
	{
		_showBanner(bShow);
	}

    public static void OnRoleCreate()
    {
        CallPlatform("OnRoleCreate",null);
    }

    public static bool IsEnableShareCenter()
    {
        return _isEnableShareCenter();
    }
}
