using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 游戏全局对象 整个游戏运行期间会一直存在
/// </summary>

public class ZQBObjectManager : MonoBehaviour {

	public static ZQBObjectManager Inst;
	void Awake()
	{
		Inst = this;
	}
	void Start()
	{
		PlatformHelper.Init();
		NGUILogHelpler.EnableTags["PlatformHelper"] = true;
		PlatformHelper.AppStart();
	}
	
	public void OnApplicationQuit()
	{
		PlatformHelper.AppQuit();
	}
	
	public void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			PlatformHelper.AppQuit();
		}
		else
		{
			PlatformHelper.AppStart();
		}
	}

	void OnDestory()
	{
		DontDestroyOnLoad (this.gameObject);
	}

}
public static class NGUILogHelpler
{
	
	public static Dictionary<string, bool> EnableTags = new Dictionary<string, bool>();
	
	public static void Log(string text, string tagName)
	{
		//return;
		if (EnableTags.ContainsKey(tagName) && EnableTags[tagName])
		{
			NGUIDebug.Log2(text);
		}
	}
}
