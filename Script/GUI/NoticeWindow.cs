//********************************************************************
// 文件名: NoticeLogic.cs
// 描述: 公告里的公告UI控件
// 作者: YangXin
// 创建时间: 2012-11-11
//********************************************************************
using UnityEngine;
using System.Collections;
using GCGame.Table;
using System.Collections.Generic;
using Module.Log;
using System.Net;
using GCGame;
using System.IO;
public class NoticeWindow : MonoBehaviour
{

    public GameObject ItemParent;
    // Use this for initialization
    public static List<string> dataList;
    public static List<string> newsList;

    private static NoticeWindow m_Instance = null;
    public UILabel MessageLabel;

    public static NoticeWindow Instance()
    {
        return m_Instance;
    }
    void Awake()
    {
        m_Instance = this;
    }
    void OnDestroy()
    {
        m_Instance = null;
    }
    void Start()
    {
        UIManager.LoadItem(UIInfo.NoticeItem, OnLoadNoticeItem);
        BoxCollider msgBox= MessageLabel.GetComponent<BoxCollider>();
        Bounds bds =  NGUIMath.CalculateRelativeWidgetBounds(MessageLabel.transform);
        msgBox.size = bds.size;
        msgBox.center = bds.center;

        Debug.Log(TableManager.GetPublicConfigByID(1)[0].StringValue + "?serverId=" + PlayerPreferenceData.LastServer.ToString());
        HttpWebRequest httpReq = (HttpWebRequest)HttpWebRequest.Create(TableManager.GetPublicConfigByID(1)[0].StringValue + "?serverId=" +PlayerPreferenceData.LastServer.ToString());
        HttpWebResponse response =(HttpWebResponse)httpReq.GetResponse();
        if (response.ContentLength > 0)
        {
            StreamReader sr = new StreamReader(response.GetResponseStream());
            MessageLabel.text = sr.ReadToEnd();
        }
    }

    // Update is called once per frame
    public static void addNotice(string news, string data)
    {
        if (dataList == null)
        {
            dataList = new List<string>();
        }
        dataList.Add(data);
        if (newsList == null)
        {
            newsList = new List<string>();
        }
        newsList.Add(news);
    }
    public static void ClearNotice()
    {
        if (dataList != null)
        {
            dataList.Clear();
        }
        if (newsList != null)
        {
            newsList.Clear();
        }
    }
    bool isNotice()
    {
        if (dataList == null || newsList == null)
        {
            return false;
        }
        if (dataList.Count != newsList.Count)
        {
            return false;
        }
        return true;
    }
  

    void OnLoadNoticeItem(GameObject resItem, object param)
    {
        if (resItem == null)
        {
            LogModule.ErrorLog("notice item load error");
            return;
        }

        if (isNotice() == false)
        {
            return;
        }

        for (int i = dataList.Count - 1; i >= 0; i--)
        {
            GameObject newItem = Utils.BindObjToParent(resItem, ItemParent);
            if (null != newItem)
            {
                Transform dataTransform = newItem.transform.FindChild("data");
                if (null != dataTransform && null != dataTransform.gameObject.GetComponent<UILabel>())
                    dataTransform.gameObject.GetComponent<UILabel>().text = dataList[i];

                newsList[i] = newsList[i].Replace("\\n", "\n");

                Transform newsTransform = newItem.transform.FindChild("news");
                if (null != newsTransform && null != newsTransform.gameObject.GetComponent<UILabel>())
                    newsTransform.gameObject.GetComponent<UILabel>().text = newsList[i];
            }
        }
       
        ItemParent.GetComponent<UIGrid>().repositionNow = true;
    }
    void InitSignInList()
    {
        
    }
}
