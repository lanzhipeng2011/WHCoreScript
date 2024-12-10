/********************************************************************
	created:	2013/12/24
	created:	24:12:2013   14:09
	filename: 	ServerListWindow.cs
	author:		王迪
	
	purpose:	服务器列表界面
*********************************************************************/

using UnityEngine;
using System.Collections;
using GCGame;
using System.Collections.Generic;
using GCGame.Table;
using Module;
using Module.Log;

public class ServerListWindow : UIControllerBase<ServerListWindow> {

    public GameObject m_ObjServerListItem;
    public GameObject m_ObjServerPageItem;

    public UIGrid ListItemParent;
    public UIGrid ListPageParent;

    public UILabel labelLastServer;
    public UISprite lastServerState; 
    public UISprite sprRecommand;
    public UILabel labelCurServerPageName;
    private List<GameObject> m_itemList = new List<GameObject>();
    private List<GameObject> m_pageList = new List<GameObject>();

    private string m_curSelectItemName = "0";

    void Awake()
    {
        UIControllerBase<ServerListWindow>.SetInstance(this);
    }
	// Use this for initialization
	void Start () 
    {
        UpdateItems();
	}

    void OnEnable()
    {
        LoginData.ServerListData lastServerData = LoginData.GetServerListDataByID(PlayerPreferenceData.LastServer);

        if (null != lastServerData)
        {
            SetCurServerInfo(lastServerData);
        }
        else
        {
            labelLastServer.text = StrDictionary.GetClientDictionaryString("#{1006}");
        }
    }

    void UpdateItems()
    {
        if (null == m_ObjServerListItem || null == m_ObjServerPageItem)
        {
            LogModule.ErrorLog("load server list item error");
            return;
        }

        // 找到上次登录服务器所在位置
        Dictionary<string, List<LoginData.PlayerRoleData>> curRoleInfoDic = UserConfigData.GetRoleInfoList(PlayerPreferenceData.LastAccount);
        int lastServerIndex = 0;
        for (int curDataIndex = 0; curDataIndex < LoginData.serverListData.Count; curDataIndex++)
        {
            if (LoginData.serverListData[curDataIndex].m_id == PlayerPreferenceData.LastServer)
            {
                lastServerIndex = curDataIndex;
                break;
            }
        }

        int curShowPageIndex = LoginData.serverPageData.Count -1;
        if(curShowPageIndex < 0) curShowPageIndex = 0;


        // 初始化左边页导航表,倒序的
        int serverPageCount =LoginData.serverPageData.Count;
        int itemCountMax = 10;
        for (int curPageIndex = serverPageCount-1; curPageIndex >= 0; curPageIndex--)
        {
            GameObject newPage = Utils.BindObjToParent(m_ObjServerPageItem, ListPageParent.gameObject, curPageIndex.ToString());
            if (null != newPage)
            {
                newPage.name = (curPageIndex).ToString();
                newPage.GetComponent<ServerPageItem>().SetTitle(LoginData.serverPageData[curPageIndex].m_pageName);
                m_pageList.Add(newPage);
            }

            if (LoginData.serverListData.Count - lastServerIndex <= LoginData.serverPageData[curPageIndex].m_pagePos)
            {
                curShowPageIndex = curPageIndex-1;
            }

            if (curPageIndex > 0)
            {
                int curItemCount = LoginData.serverPageData[curPageIndex].m_pagePos - LoginData.serverPageData[curPageIndex - 1].m_pagePos;
                if (curItemCount > itemCountMax)
                {
                    itemCountMax = curItemCount;
                }
            }
        }


        ListPageParent.repositionNow = true;

        // 设置10个默认服务器
        for (int curListItemIndex = 0; curListItemIndex < itemCountMax; curListItemIndex++)
        {
            GameObject newItem = Utils.BindObjToParent(m_ObjServerListItem, ListItemParent.gameObject);
            newItem.SetActive(false);
            m_itemList.Add(newItem);
        }

        ListItemParent.repositionNow = true;

       
        if (curShowPageIndex < m_pageList.Count)
        {
            int curPageDataIndex = LoginData.serverPageData.Count - curShowPageIndex -1;
            m_pageList[curPageDataIndex].GetComponent<ServerPageItem>().EnableHeightLight(true);
            labelCurServerPageName.text = LoginData.serverPageData[curShowPageIndex].m_pageName;
        }

        UpdateServerListItemByData( curShowPageIndex);
        
        //ListItemParent.repositionNow = true;
    }

    void UpdateServerListItemByData(int page)
    {
        if (page < 0 || page >= LoginData.serverPageData.Count)
        {
            return;
        }

        int curShowEndIndex = LoginData.serverPageData[page].m_pagePos;
        int curShowStartIndex = curShowEndIndex;
        if(page < LoginData.serverPageData.Count-1)
        {
            curShowStartIndex = LoginData.serverPageData[page + 1].m_pagePos;
        }
        else
        {
            curShowStartIndex = LoginData.serverListData.Count;
        }

        curShowStartIndex = LoginData.serverListData.Count - curShowStartIndex;
        curShowEndIndex = LoginData.serverListData.Count - curShowEndIndex;
 
        if (curShowStartIndex < 0)
        {
            curShowStartIndex = 0;
        }
        if (curShowEndIndex > LoginData.serverListData.Count)
        {
            curShowEndIndex = LoginData.serverListData.Count;
        }

        for (int i = 0; i < m_itemList.Count; i++)
        {
            if (curShowStartIndex < curShowEndIndex)
            {
                LoginData.ServerListData curListData = LoginData.serverListData[curShowStartIndex];
                m_itemList[i].name = curListData.m_id.ToString();

                m_itemList[i].GetComponent<ServerListItem>().SetState(curListData.m_name, (ServerListItem.State)curListData.m_state, (ServerListItem.Type)curListData.m_type, null);
                if (curListData.m_id == PlayerPreferenceData.LastServer)
                {
                    m_itemList[i].GetComponent<ServerListItem>().EnableHeightLight(true);
                    m_curSelectItemName = m_itemList[i].name;
                }
                else
                {
                    m_itemList[i].GetComponent<ServerListItem>().EnableHeightLight(false);
                }
                m_itemList[i].SetActive(true);
                curShowStartIndex++;
            }
            else
            {
                m_itemList[i].SetActive(false);
            }
            
        }

        ListItemParent.repositionNow = true;

    }

    void OnBtnBackClick()
    {
        UIControllerBase<ServerChooseController>.Instance().SwitchWindow((int)ServerChooseController.Window.ServerTop);
    }

    void OnBtnNextClick()
    {
        // PlatformHelper.SendUserAction(UserBehaviorDefine.ServerChoose_Enter);
        PlatformHelper.ClickEnterGame(() =>
        {
            int curSelectServerID = 0;
            if (!int.TryParse(m_curSelectItemName, out curSelectServerID))
            {
                return;
            }
            LoginData.ServerListData curData = LoginData.GetServerListDataByID(curSelectServerID);

            if (null != curData)
            {
                PlayerPreferenceData.LastServer = curSelectServerID;
                ServerChooseController.Instance().ConnectToServer(curData.m_ip, curData.m_port);
            }
            else
            {
                // 错误：当前服务器未配置
                MessageBoxLogic.OpenOKBox(1002, 1001);
            }
        });
    }

    public void ServerSelected(string name)
    {
        for (int i = 0; i < m_itemList.Count; ++i)
        {
            bool bHightLight = m_itemList[i].name == name;
            m_itemList[i].GetComponent<ServerListItem>().EnableHeightLight(bHightLight);
            
            
        }
        m_curSelectItemName = name;
        
        int curID = 0;
        if(int.TryParse(name, out curID))
        {
            for (int i = 0; i < LoginData.serverListData.Count; ++i)
            {
                if (LoginData.serverListData[i].m_id == curID)
                {
                    labelLastServer.text = LoginData.serverListData[i].m_name;
                    UIControllerBase<ServerChooseController>.Instance().SelectServerListItem(LoginData.serverListData[i]);
                }
            }
        }
    }

    public void ServerPageSelected(string name)
    {
        for (int i = 0; i < m_pageList.Count; ++i)
        {
            bool bHightLight = m_pageList[i].name == name;
            m_pageList[i].GetComponent<ServerPageItem>().EnableHeightLight(m_pageList[i].name == name);
            if (bHightLight)
            {
                labelCurServerPageName.text = LoginData.serverPageData[LoginData.serverPageData.Count - i -1].m_pageName;
            }
        }

        int curPage = 0;
        if (int.TryParse(name, out curPage))
        {
            UpdateServerListItemByData(curPage);
        }
    }

    void SetCurServerInfo(LoginData.ServerListData serverListData)
    {
        labelLastServer.text = serverListData.m_name;
        lastServerState.gameObject.SetActive(true);
        //sprRecommand.gameObject.SetActive(serverListData.m_type > 0);
        switch ((ServerListItem.State)serverListData.m_state)
        {
            case ServerListItem.State.HOT:
                lastServerState.spriteName = "hongchang";
                break;
            case ServerListItem.State.NEW:
                lastServerState.spriteName = "lvchang";
                break;
            case ServerListItem.State.NORAML:
                lastServerState.spriteName = "huangchang";
                break;
            case ServerListItem.State.STOP:
                lastServerState.spriteName = "huichang";
                break;
        }

    }
}
