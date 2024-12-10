/********************************************************************************
 *	文件名：	GameDefines.cs
 *	全路径：	\Script\GameDefines\GameDefines.cs
 *	创建人：	王华
 *	创建时间：2013-11-21
 *
 *	功能说明：全局定义，很多是要对外发布的
 *	修改记录：
*********************************************************************************/

using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using Module.Log;

// The output platform defines
public enum OutputVersionDefs
{
	Windows,
	
	Nd91Android,
	Nd91iPhone, // 91 iPhone sdk
	
	GfanAndroid, // gfan Android sdk
	GfaniPhone, // gfan iPhone sdk
	
	UCAndroid,
	UCiPhone,
	
	PPAndroid,
	PPiPhone,
	
	MiAndroid,
	MiiPhone,
	
	AppStore,
}

public class GameDefines
{
	public static OutputVersionDefs OutputVerDefs{
        get
        {
            if (Application.isEditor)
            {
                return OutputVersionDefs.Windows;
            }
            else
            {
                // Read the current sdk platform
                string sdkPlatform = "";
                using (FileStream fs = new FileStream("Resources/SDKPlatform.txt", FileMode.OpenOrCreate))
                {
                    using (StreamReader reader = new StreamReader(fs))
                    {
                        sdkPlatform = reader.ReadLine();
                        reader.Close();
                    }
                    fs.Close();
                }
                LogModule.DebugLog("Current sdkPlatform is " + sdkPlatform);

#if UNITY_ANDROID
				if (sdkPlatform.Equals("gfan"))
					return OutputVersionDefs.GfanAndroid;
				else if (sdkPlatform.Equals("91"))
					return OutputVersionDefs.Nd91Android;
				else if (sdkPlatform.Equals("uc"))
					return OutputVersionDefs.UCAndroid;
				else if (sdkPlatform.Equals("pp"))
					return OutputVersionDefs.PPAndroid;
				else if (sdkPlatform.Equals("xiaomi"))
					return OutputVersionDefs.MiAndroid;
				
				return OutputVersionDefs.GfanAndroid;
				
#elif UNITY_IPHONE
                if (sdkPlatform.Equals("gfan"))
                    return OutputVersionDefs.GfaniPhone;
                else if (sdkPlatform.Equals("91"))
                    return OutputVersionDefs.Nd91iPhone;
                else if (sdkPlatform.Equals("uc"))
                    return OutputVersionDefs.UCiPhone;
                else if (sdkPlatform.Equals("pp"))
                    return OutputVersionDefs.PPiPhone;
                else if (sdkPlatform.Equals("xiaomi"))
                    return OutputVersionDefs.MiiPhone;

                return OutputVersionDefs.Nd91iPhone;

#elif UNITY_WEBPLAYER
			return OutputVersionDefs.Windows;
#endif
            }

            return OutputVersionDefs.Windows;
        }
	}
	
	public static string GameChannel = "1";
	public static string GameVersion = "1.1.0";
    public static int PublicResVersionKey = 0;

    public static string GameHomepageUrl = "http://www.baidu.com/";
	public static string DownloadSkipToUrl = "http://www.baidu.com/";
    public static string OfficialSkipToUrl = "http://www.baidu.com/";

	public static string FlagiADUrl = "http://www.baidu.com";
	
	// Manifest.
	public static string Manifest{ get { return "Artist/manifest.xml"; } }
	
	// Manifest package.
	public static string ManifestPackage{ get { return "00004"; } }
	
	// Asset Package File.
	public static string AssetPackageFile{ get { return "Artist/AssetPackage.xml"; } }
	
	// Asset Package Package.
	public static string AssetPackagePackage{ get { return "00001"; } }
	
	public static string LOCAL_FILE_FLAG = "resources/";

    public static string GMCMD_BEGINORDER = ",,";
    public static string CLIENTGMCMD_BEGINORDER = "..";
    public static int MAX_TRY_CONNECT = 1;
    public const float CONNECT_TIMEOUT = 5.0f;         // 等待超时时间，超过时间可继续发包
    public const float CONNECT_WAIT_DELAY = 0.5f;       // 弹出等待框某人延迟时间

	public const int RESLEVEL_LOCAL = 0;
	#region GameSetting

	public static bool Setting_IsAutoLogin  = true;
	public static bool Setting_IsGuest		= false;
	public static string Setting_LoginName		= "";
	public static string Setting_LoginPass		= "";
	public static string Setting_LogingServer	= "";
	
    //画面，音乐声音，语言设置项
	public static int	Setting_ScreenQuality	= 1;
	public static bool	Setting_Gravity			= false;
	public static int	Setting_MusicVol		= 50;
	public static int	Setting_SoundVol		= 50;
	public static int	Setting_Language		= 0;
	
	public static bool	Setting_SkipCopyCameraTrack = false;
	public static bool	Setting_ShakeEnable		= true;
	
	#endregion
}

