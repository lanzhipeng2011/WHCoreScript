using UnityEngine;
using System.Collections.Generic;
using System.Collections;
/// <summary>
/// 游戏全局对象 整个游戏运行期间会一直存在
/// </summary>
public class GameRoot : MonoBehaviour
{
    public static GameRoot Inst;
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

    public void OnGUI()
    {/*
        if (GUI.Button(new Rect(0, 0, 100, 40), "Start TalkingDataGA"))
        {
            try
            {
                TalkingDataGA.OnStart("4B78CA63F312A6C7F9BA638D84C7133F", "Default");
            }
            catch (System.Exception e)
            {
                NGUILogHelpler.Log(e.Message, "PlatformHelper");
            }
            NGUILogHelpler.Log("Start 成功", "PlatformHelper");
        }

        if (GUI.Button(new Rect(0, 100, 100, 40), "init UC"))
        {
            try
            {

                UCGameSdk.initSDK(true, 2, 71200, 727770, 0, "default", false, false);
            }
            catch (System.Exception e)
            {
                NGUILogHelpler.Log(e.Message, "PlatformHelper");
            }
        }
        if (GUI.Button(new Rect(0, 200, 100, 40), "load game"))
        {
            PlatformHelper.CallPlatform("OnLoadGame", null);
        }*/
    }
}


//public static class NGUILogHelpler
//{
//
//    public static Dictionary<string, bool> EnableTags = new Dictionary<string, bool>();
//
//    public static void Log(string text, string tagName)
//    {
//        if (EnableTags.ContainsKey(tagName) && EnableTags[tagName])
//        {
//            NGUIDebug.Log2(text);
//        }
//    }
//}
