using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
using Module.Log;

public class PlatformListener : MonoBehaviour {

	private static PlatformListener m_instance = null;

    public static bool m_bReceiveNewLoginData = false;
	public static PlatformListener Instance()
	{
		return m_instance;
	}
	void Awake()
	{
		//为gameManager赋值，所有的其他操作都要放在后面
		if (null != m_instance)
		{
			Destroy(this.gameObject);
		}		
		m_instance = this;
		DontDestroyOnLoad(this.gameObject);
	}

	public void OnCYUserLogin(string param)
	{
        
        LogModule.DebugLog("----OnCYUserLogin:" + param);
#if UNITY_ANDROID
        JsonData paramObj = JsonMapper.ToObject(param);
        string data = (string)paramObj["data"];
        JsonData dataObj = JsonMapper.ToObject(data);
        string strValidateInfo = (string)dataObj["validateInfo"];
#else
        //param = "{\"uid\":\"514001613706505\",\"token\":\"9f38e276d3d758b5\",\"deviceid\":\"10001\",\"userip\":\"10001\",\"type\":\"c\",\"appid\":\"101005256\"},4001,10001"
        //Debug.Log(param);
		string keyValidateInfo = "\"validateInfo\":";

		int validateInfoPos = param.IndexOf(keyValidateInfo) + keyValidateInfo.Length + 1;
		string strValidateInfo = param.Substring(validateInfoPos, param.IndexOf("\"", validateInfoPos) - validateInfoPos);
		
        //int opCodePos = param.LastIndexOf(",") + 1;
        //string opCode = param.Substring(opCodePos);
		
        //int channelPos = param.LastIndexOf("},") + 2;
        //string stringChannel = param.Substring(channelPos, opCodePos - 1 - channelPos);
#endif
        LoginData.accountData.SetCYData(strValidateInfo);
        /*
        if (PlatformHelper.GetChannelType() == PlatformHelper.ChannelType.IOS_XY)
        {
            m_bReceiveNewLoginData = true;
        }
        */
        if (ServerChooseController.Instance() == null)
        {
            // 只允许在登陆界面登陆
            return;
        }

        SendLoginInfo();
	}

    public static void SendLoginInfo()
    {
        PlatformHelper.SendUserAction(UserBehaviorDefine.LoginGetTokenSuccess);
        MessageBoxLogic.OpenWaitBox(1003, 15, 0);
        NetManager.SendUserLogin(LoginData.Ret_Login, false);
        m_bReceiveNewLoginData = false;
    }

	public void OnCYUserLogout(string param)
    {
        if (GameManager.gameManager.RunningScene != (int)GameDefine_Globe.SCENE_DEFINE.SCENE_LOGIN)
        {
            NetManager.SendUserLogout();
        }
        else
        {
            if (LoginUILogic.Instance() != null)
            {
                LoginUILogic.Instance().EnterServerChoose();
            }
            else
            {
                LoadingWindow.LoadScene(GameDefine_Globe.SCENE_DEFINE.SCENE_LOGIN);
            }
        }
        
    }
	public void OnCYPayResult(string param)
	{
        LogModule.DebugLog("--------------OnCYPayResult---------------" + param);
		bool isPaySuccess = false;
		int nSuccess = 0;
		if(int.TryParse(param, out nSuccess))
		{
			isPaySuccess = nSuccess > 0;
		}
        if(isPaySuccess)
        {
            SendCYPay(1);
        }

        LogModule.DebugLog("============unity pay result :" + isPaySuccess.ToString());
	}

    public void OnLoginTimeOut()
    {
        NetWorkLogic.GetMe().DisconnectServer();
        MessageBoxLogic.OpenOKBox(1005, 1000);
    }
    public static void SendCYPay(int type)
    {
        CG_CYPAY_SUCCESS payPacket = (CG_CYPAY_SUCCESS)PacketDistributed.CreatePacket(MessageID.PACKET_CG_CYPAY_SUCCESS);
        payPacket.SetRoleGuid(string.Format("{0:X16}", PlayerPreferenceData.LastRoleGUID));
        if (type == 1)
        {
            payPacket.SetType((int)CG_CYPAY_SUCCESS.TYPE.PAYOVER);
        }
        else
        {
            payPacket.SetType((int)CG_CYPAY_SUCCESS.TYPE.MANUAL);
        }
        payPacket.SendPacket();
    }
	
	public void OnReqCYPayGoodListSuccess(string param)
	{
        LogModule.DebugLog(param);

		RechargeData.InitGoodInfo(param);

        if (null != RechargeController.Instance())
        {
            RechargeController.Instance().UpdateRechargeList();
        }
	}

	public void OnReqCYPayGoodListFail(string param)
	{
        LogModule.DebugLog(param);
	}

    public void OnUMengSNSSuccess(string param)
    {
        LogModule.DebugLog("SNS success" + param);
    }
}
