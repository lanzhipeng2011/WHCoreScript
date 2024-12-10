//********************************************************************
// 文件名: NoticeLogic.cs
// 描述: 公告主UI
// 作者: YangXin
// 创建时间: 2012-11-11
//********************************************************************
using UnityEngine;
using System.Collections;
using System;
using Games.GlobeDefine;

public class NoticeLogic : UIControllerBase<NoticeLogic>{

//    private static NoticeLogic m_Instance = null;
//     public static NoticeLogic Instance()
//     {
//         return m_Instance;
//     }

    public static bool IsReceiveData = false;
    void Awake()
    {
        UIControllerBase<NoticeLogic>.SetInstance(this);
       // m_Instance = this;
    }
//     void OnDestroy()
//     {
//     //    m_Instance = null;
//     }
	// Use this for initialization
	void Start () {
      //  gameObject.SetActive(false);
	}

    public static void TryOpen()
    {
        /*
        if (null != Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            int nTiem = Singleton<ObjManager>.GetInstance().MainPlayer.AutoNotice;
            if (nTiem == 0)
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.AutoNotice = DateTime.Now.Month * 100 + DateTime.Now.Day;
                Singleton<ObjManager>.GetInstance().MainPlayer.ServerAutoInfo();
                return;
            }
            int nmonth = nTiem / 100;
            int nday = nTiem % 100;
            if (nmonth == DateTime.Now.Month && DateTime.Now.Day == nday)
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.AutoNotice = DateTime.Now.Month * 100 + DateTime.Now.Day;
                Singleton<ObjManager>.GetInstance().MainPlayer.ServerAutoInfo();
                return;
            }
            
            Singleton<ObjManager>.GetInstance().MainPlayer.AutoNotice = DateTime.Now.Month * 100 + DateTime.Now.Day;
            Singleton<ObjManager>.GetInstance().MainPlayer.ServerAutoInfo();
            OpenUI();
       
        }     */

    }
    public static void OpenUI()
    {

		if ((int)GameDefine_Globe.SCENE_DEFINE.SCENE_YANMENGUANWAI == GameManager.gameManager.RunningScene) 
		{
			return;
		}
        if (true)
        {
            UIManager.ShowUI(UIInfo.Notice);
            IsReceiveData = false;
        }
    }
    
    public void CloseWindow()
    {

        UIManager.CloseUI(UIInfo.Notice);
    }
 
}
