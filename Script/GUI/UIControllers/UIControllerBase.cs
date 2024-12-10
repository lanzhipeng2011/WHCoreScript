/********************************************************************
	created:	2013/12/24
	created:	24:12:2013   13:59
	filename: 	UIControllerBase.cs
	author:		王迪
	
	purpose:	UI控制器基类，实现单间类，需要子类在Awake()函数中调用SetInstance(this)
*********************************************************************/

using UnityEngine;
using System.Collections;
using Module.Log;
public class UIControllerBase<T> : MonoBehaviour
{
    public GameObject[] childWindows;

    private static T _Instance;

    // 不确定窗口是否存在时需要判空
    public static T Instance()
    {
        return _Instance;
    }

    public static void SetInstance(T instance)
    {
        _Instance = instance;
    }

    // 如果覆盖了OnDestroy需要将_Instance置空，防止资源无法释放
    void OnDestroy()
    {
        Release();
    }

    void Release()
    {
        _Instance = default(T);      
    }

    public void SwitchWindow(int index)
    {
        if (null == childWindows)
        {
            LogModule.WarningLog("child window is not set");
            return;
        }

        if (index >= childWindows.Length)
        {
            LogModule.WarningLog("child window index out range :" + index.ToString() + " " + childWindows.Length.ToString());
            return;
        }

        for(int i=0; i<childWindows.Length; i++)
        {
           childWindows[i].SetActive(i == index);
        }
    }

}