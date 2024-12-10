using UnityEngine;
using System.Collections;


//此类用于对参数进行集中管理
public static class UCConfig
{

	//联调环境参数
	public static int cpid = 20087;
	public static int gameid = 119474;
	public static int serverid = 1333;
	public static string servername = "test1";

	//正式环境参数
	//public static int cpid = ;
	//public static int gameid = ;
	//public static int serverid = ;
	//public static string servername = ;


	public static bool debugMode = true;
	public static int logLevel = UCConstants.LOGLEVEL_DEBUG;
	public static int orientation = UCConstants.ORIENTATION_LANDSCAPE;
	public static bool enablePayHistory = false;
	public static bool enableLogout = false;


	public static bool inited = false;
	public static int initRetryTimes = 0;
	public static bool logined = false;
	
	public static int loginUISwitch = UCConstants.USE_WIDGET;

}
