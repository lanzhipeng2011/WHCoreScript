using UnityEngine;
using System.Collections;
using Module.Log;
/// <summary>
/// android接口类，用于游戏调用android java代码
/// 该脚本应关联到每一个场景的 "Main Camera" 对象，以能接收SDK回调的消息。
/// 下面各方法中的逻辑处理，在游戏中应修改为真实的逻辑。
/// 
/// 代码示例，在游戏需要调用android登录的地方添加  AndroidHelper.doSdk("doLogin",paramJson);调用登录界面
///    点击支付按钮后添加   AndroidHelper.doSdk("doPay",paramJson);调用支付界面
///
/// </summary>

public class AndroidHelper : MonoBehaviour
{

	public static bool isDebug;
    private static AndroidHelper m_instance = null;

    public static AndroidHelper Instance()
    {
        return m_instance;
    }

    void Awake()
    {
        if (null != m_instance)
        {
            Destroy(this.gameObject);
        }

		#if UNITY_ANDROID
		string ret = AndroidHelper.platformHelper("IsEnableDebugMode");
		if ("1".Equals(ret))
		{
			isDebug =  true;
		}
		else
		{
			isDebug = false;
		}
		#endif

        m_instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

#if UNITY_ANDROID

	void Update()
	{
		if(Input.GetKeyUp(KeyCode.Escape))
		{
			PushNotification.NotificationMessage2Clinet();
			PushNotification.NotificationMessage2Server();
			doSdk("showQuit","");
		}
	}

    private const string SDK_JAVA_CLASS = "";

    private const string PLATFORM_CLASS = "";

	public static void openShare(string json){
		doSdk("share",json);
	}

    public static void doSdk(string functionName, string json)
    {
        //LogModule.DebugLog("----Unity3D doSdk..." + functionName + "----" + json);
       // onFuncCall("jniCall", functionName, json);
    }

    public static void onFuncCall(string func, params object[] args)
    {
        //LogModule.DebugLog("----Unity3D onFuncCall calling..." + func);
        using (AndroidJavaClass cls = new AndroidJavaClass(SDK_JAVA_CLASS))
        {
            cls.CallStatic(func, args);
        }
    }

    public static string platformHelper(string func, string json = "")
    {
       // LogModule.DebugLog("----Unity3D platformHelper calling..." + func + " " + json);
        using (AndroidJavaClass cls = new AndroidJavaClass(PLATFORM_CLASS))
        {
            string ret = cls.CallStatic<string>("jniCall", func, json);
           // LogModule.DebugLog("----Unity3D platformHelper func:" + func + ", json:" + json + " return:" + ret);
            return ret;
        }
    }
#endif

}