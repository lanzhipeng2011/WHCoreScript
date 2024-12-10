using UnityEngine;
using System.Collections;
using Module.Log;

#if !UNITY_WP8

/// <summary>
/// 接收java代码的回调消息，进行相关逻辑处理。
/// 该脚本应关联到每一个场景的 "Main Camera" 对象，以能接收SDK回调的消息。
/// 下面各方法中的逻辑处理，在游戏中应修改为真实的逻辑。
/// </summary>

public class AndroidCallback : MonoBehaviour
{

    private static AndroidCallback m_instance = null;

    public static AndroidCallback Instance()
    {
        return m_instance;
    }
    
    void Awake()
    {
        if (null != m_instance)
        {
            Destroy(this.gameObject);
        }
        m_instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void OnCallResult(string jsonstr)
    {
       // LogModule.DebugLog("----Unity-OnCallResult message: jsonstr=" + jsonstr);
        JsonData jsonobj = JsonMapper.ToObject(jsonstr);
        string func = (string)jsonobj["func"];
        string json = (string)jsonobj["json"];
        if (func.Equals("onLogin"))
        {
            onLogin(json);
        }
        else if (func.Equals("onPay"))
        {
            onPay(json);
        }
        else if (func.Equals("onLogout"))
        {
            onLogout(json);
        }
        else if (func.Equals("onChangeAccount"))
        {
            onChangeAccount(json);
        }else if(func.Equals("onShare")){
			onShare(json);
		}
        else if (func.Equals("onPaymentGoodInfoList"))
        {
            onPaymentGoodInfoList(json);
        }
    }

    //登录完成逻辑处理//
    //登录回调 1.08版本的聚合SDK登录返回为加密的字符串，客户端直接拿这个串去验证，成功后服务器给返回uid和token
    //现在登录回调中客户端得不到uid和token了，擦擦擦！！！  2014-06-25
    private void onLogin(string jsonLoginInfo)
    {
        PlatformListener.Instance().OnCYUserLogin(jsonLoginInfo);
    }

    //支付完成逻辑处理//
    private void onPay(string jsonOrder)
    {
        //LogModule.DebugLog("----Unity-onPay-jsonOrder==" + jsonOrder);
        //GameRoot.onPay(jsonOrder);
        PlatformListener.Instance().OnCYPayResult(jsonOrder);
        //LogModule.DebugLog("--------------onPay done---------------");
    }

    //退出完成逻辑处理//
    private void onLogout(string jsonLogoutMsg)
    {
        //返回登录界面，释放清除相关资源等//
       // LogModule.DebugLog("----logout");

        CG_ASK_QUIT_GAME packet = (CG_ASK_QUIT_GAME)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_QUIT_GAME);
        packet.Type = (int)CG_ASK_QUIT_GAME.GameSelecTType.GAMESELECTTYPE_QUIT;
        packet.SendPacket();
        Application.Quit();
    }

    // 注销完成的逻辑
    private void onChangeAccount(string jsonLogoutMsg)
    {
        //LogModule.DebugLog("----onChangeAccount");
        PlatformListener.Instance().OnCYUserLogout(jsonLogoutMsg);
    }
	//分享成功
	private void onShare(string json)
	{
		PlatformListener.Instance().OnUMengSNSSuccess(json);
	}
    private void onPaymentGoodInfoList(string goods)
    {
        PlatformListener.Instance().OnReqCYPayGoodListSuccess(goods);
    }

}

#endif