using UnityEngine;
using System.Collections;
using GCGame;
using System.Collections.Generic;
using System;
using Games.GlobeDefine;
using Module.Log;
using GCGame.Table;

public class RelationFriendWindow : MonoBehaviour {

    private static RelationFriendWindow m_Instance = null;
    public static RelationFriendWindow Instance()
    {
        return m_Instance;
    }

    public GameObject friendListGrid;
    public TabController m_TabController;
    public RelationButtionWindowLogic m_ButtonWindow;
	public UILabel friendNum;

    private PlayerListItemLogic m_SelectPlayerItem;
    private int m_TrailSceneClass = GlobeVar.INVALID_ID;
    private int m_TrailSceneInst = GlobeVar.INVALID_ID;
    private int m_TrailPosX = 0;
    private int m_TrailPosZ = 0;

	void Awake () {
        m_Instance = this;
        m_TabController.delTabChanged = OnTabChanged;
	}
	
	void OnEnable () {
        GUIData.delFriendDataUpdate += UpdateData;

        m_SelectPlayerItem = null;
        SelectPlayerListItem(null);
        //UpdateData();
        //向服务器申请更新好友列表
        m_TabController.ChangeTab("0");
      
        //打开的时候默认为好友
        //UpdateData();
	}

    void OnDisable()
    {
        GUIData.delFriendDataUpdate -= UpdateData;
        m_SelectPlayerItem = null;
        SelectPlayerListItem(null);
    }

    void OnDestroy()
    {
        m_Instance = null;
    }

    void UpdateData()
    {
        GameObject curTab = m_TabController.GetHighlightTab().gameObject;
		//update的时候先cleangrid，注释掉了update时候的clean。在note3上会出现闪UI的现象。
		Utils.CleanGrid(friendListGrid);
        SelectPlayerListItem(null);
        if (curTab.name == "0")
        {
            UpdateFriendListData();
			friendNum.transform.parent.gameObject.SetActive(true);
			friendNum.text = "好友:" + GameManager.gameManager.PlayerDataPool.FriendList.RelationDataList.Count.ToString() + "/" + GlobeVar.MAX_FRIEND_NUM.ToString();
        }
        else if (curTab.name == "1")
        {
            UpdateBlackList();
			friendNum.transform.parent.gameObject.SetActive(true);
			friendNum.text = "黑名单:" + GameManager.gameManager.PlayerDataPool.BlackList.RelationDataList.Count.ToString() + "/" + GlobeVar.MAX_BLACK_NUM.ToString();
        }
        else if (curTab.name == "2")
        {
            UpdateHateList();
			friendNum.transform.parent.gameObject.SetActive(true);
			friendNum.text = "仇人:" + GameManager.gameManager.PlayerDataPool.HateList.RelationDataList.Count.ToString() + "/" + GlobeVar.MAX_FRIEND_NUM.ToString();
        }
		else if(curTab.name == "3")
		{
			friendNum.transform.parent.gameObject.SetActive(false);
		}
        friendListGrid.GetComponent<UIGrid>().Reposition();
   
    }
    void UpdateFriendListData()
    {
        UIManager.LoadItem(UIInfo.PlayerListItem, OnLoadFriendItem);
    }

    void OnLoadFriendItem(GameObject resItem, object param)
    {
        if (null == resItem)
        {
            LogModule.ErrorLog("load friend item fail");
            return;
        }
        Utils.CleanGrid(friendListGrid);
        

        int nPlayerListItemIndex = 0;
        //先排在线的
        foreach (KeyValuePair<UInt64, Relation> _relation in GameManager.gameManager.PlayerDataPool.FriendList.RelationDataList)
        {
            if (_relation.Key == GlobeVar.INVALID_GUID)
            {
                continue;
            }
            //如果离线，则不排
            if (_relation.Value.State == (int)CharacterDefine.RELATION_TYPE.OFFLINE)
            {
                continue;
            }
            //GameObject newPlayerListItem = Utils.BindObjToParent(resItem, friendListGrid, nPlayerListItemIndex.ToString());
           //newPlayerListItem.GetComponent<PlayerListItemLogic>().InitPlayerListItemInfoFriend(_relation.Value);
            PlayerListItemLogic PlayerListItem = PlayerListItemLogic.CreateItem(friendListGrid, resItem, nPlayerListItemIndex.ToString(), this);
            if (PlayerListItem == null)
            {
                continue;
            }
            PlayerListItem.InitPlayerListItemInfoFriend(_relation.Value);
            if (m_SelectPlayerItem == null)
            {
                SelectPlayerListItem(PlayerListItem);
            }   
            nPlayerListItemIndex++;
        }

        //再排离线的
        foreach (KeyValuePair<UInt64, Relation> _relation in GameManager.gameManager.PlayerDataPool.FriendList.RelationDataList)
        {
            if (_relation.Key == GlobeVar.INVALID_GUID)
            {
                continue;
            }

            //如果离线，则不排
            if (_relation.Value.State == (int)CharacterDefine.RELATION_TYPE.ONLINE)
            {
                continue;
            }

            //GameObject newPlayerListItem = Utils.BindObjToParent(resItem, friendListGrid, nPlayerListItemIndex.ToString());
            //newPlayerListItem.GetComponent<PlayerListItemLogic>().InitPlayerListItemInfoFriend(_relation.Value);
            PlayerListItemLogic PlayerListItem = PlayerListItemLogic.CreateItem(friendListGrid, resItem, nPlayerListItemIndex.ToString(), this);
            if (PlayerListItem == null)
            {
                continue;
            }
            PlayerListItem.InitPlayerListItemInfoFriend(_relation.Value);
            if (m_SelectPlayerItem == null)
            {
                SelectPlayerListItem(PlayerListItem);
            }
            nPlayerListItemIndex++;
        }

        friendListGrid.GetComponent<UIGrid>().Reposition();
    }


    //刷新黑名单
    public void UpdateBlackList()
    {
        UIManager.LoadItem(UIInfo.PlayerListItem, OnLoadBlackItem);
    }

    void OnLoadBlackItem(GameObject resItem, object param)
    {
        if (null == resItem)
        {
            LogModule.ErrorLog("load friend item fail");
            return;
        }

        Utils.CleanGrid(friendListGrid);
        
        //填充好友信息
        int nPlayerListItemIndex = 0;
		//先排在线的
        foreach (KeyValuePair<UInt64, Relation> _relation in GameManager.gameManager.PlayerDataPool.BlackList.RelationDataList)
        {
            //GameObject newPlayerListItem = Utils.BindObjToParent(resItem, friendListGrid, nPlayerListItemIndex.ToString());
            //newPlayerListItem.GetComponent<PlayerListItemLogic>().InitPlayerListItemInfoBlack(_relation.Value);
			//如果离线，则不排
			if (_relation.Value.State == (int)CharacterDefine.RELATION_TYPE.OFFLINE)
			{
				continue;
			}

            PlayerListItemLogic PlayerListItem = PlayerListItemLogic.CreateItem(friendListGrid, resItem, nPlayerListItemIndex.ToString(), this);
            if (PlayerListItem == null)
            {
                continue;
            }
            PlayerListItem.InitPlayerListItemInfoBlack(_relation.Value);
            if (m_SelectPlayerItem == null)
            {
                SelectPlayerListItem(PlayerListItem);
            }
            nPlayerListItemIndex++;
        }

		//再排离线的
		foreach (KeyValuePair<UInt64, Relation> _relation in GameManager.gameManager.PlayerDataPool.BlackList.RelationDataList)
		{
			//GameObject newPlayerListItem = Utils.BindObjToParent(resItem, friendListGrid, nPlayerListItemIndex.ToString());
			//newPlayerListItem.GetComponent<PlayerListItemLogic>().InitPlayerListItemInfoBlack(_relation.Value);
			//如果离线，则不排
			if (_relation.Value.State == (int)CharacterDefine.RELATION_TYPE.ONLINE)
			{
				continue;
			}
			
			PlayerListItemLogic PlayerListItem = PlayerListItemLogic.CreateItem(friendListGrid, resItem, nPlayerListItemIndex.ToString(), this);
			if (PlayerListItem == null)
			{
				continue;
			}
			PlayerListItem.InitPlayerListItemInfoBlack(_relation.Value);
			if (m_SelectPlayerItem == null)
			{
				SelectPlayerListItem(PlayerListItem);
			}
			nPlayerListItemIndex++;
		}

        friendListGrid.GetComponent<UIGrid>().Reposition();
    }

    public void UpdateHateList()
    {
        UIManager.LoadItem(UIInfo.PlayerListItem, OnLoadHateItem);
    }

    void OnLoadHateItem(GameObject resItem, object param)
    {
        if (null == resItem)
        {
            LogModule.ErrorLog("load friend item fail");
            return;
        }

        //填充好友信息
        int nPlayerListItemIndex = 0;
        foreach (KeyValuePair<UInt64, Relation> _relation in GameManager.gameManager.PlayerDataPool.HateList.RelationDataList)
        {
            PlayerListItemLogic PlayerListItem = PlayerListItemLogic.CreateItem(friendListGrid, resItem, nPlayerListItemIndex.ToString(), this);
            if (PlayerListItem == null)
            {
                continue;
            }
            PlayerListItem.InitPlayerListItemInfoHate(_relation.Value);
            if (m_SelectPlayerItem == null)
            {
                SelectPlayerListItem(PlayerListItem);
            }
            nPlayerListItemIndex++;
        }

        friendListGrid.GetComponent<UIGrid>().Reposition();
    }


    // 切换标签页响应
    void OnTabChanged(TabButton curButton)
    {
        UpdateData();
        if (curButton.name == "0")
        {
			ReqUpdateFriendUserInfo();
        }
        if (curButton.name == "3")
        {
            SelectPlayerListItem(null);
        }
    }

    //请求更新好友列表
    void ReqUpdateFriendUserInfo()
    {
        if (null != Singleton<ObjManager>.GetInstance().MainPlayer)
        {

            CG_REQ_FRIEND_USERINFO packet = (CG_REQ_FRIEND_USERINFO)PacketDistributed.CreatePacket(MessageID.PACKET_CG_REQ_FRIEND_USERINFO);
            packet.Guid = Singleton<ObjManager>.GetInstance().MainPlayer.GUID;
             packet.SendPacket();

          //  CG_REQ_FRIEND_USERINFO packet = (CG_REQ_FRIEND_USERINFO)PacketDistributed.CreatePacket(MessageID.PACKET_CG_REQ_FRIEND_USERINFO);
          //  packet.Guid = Singleton<ObjManager>.GetInstance().MainPlayer.GUID;
          //  packet.SendPacket();

        }
    }

    //public void UpdateSelectState(PlayerListItemLogic selectItem)
    //{
    //    foreach (PlayerListItemLogic item in friendListGrid.GetComponentsInChildren<PlayerListItemLogic>())
    //    {
    //        if (null == selectItem || selectItem != item)
    //        {
    //            item.OnSelected(false);
    //        }
    //    }
    //}

    public void OnClickPlayerListItem(PlayerListItemLogic selectItem)
    {
        if (selectItem == null)
        {
            LogModule.ErrorLog("OnClickPlayerListItem::selectItem = null");
            return;
        }
        SelectPlayerListItem(selectItem);
    }

    public void SelectPlayerListItem(PlayerListItemLogic selectItem)
    {
        if (null == selectItem)
        {
            if (m_ButtonWindow != null)
            {
                m_ButtonWindow.SetPlayerListItemInfo(GlobeVar.INVALID_GUID, GlobeVar.INVALID_ID, "", PlayerListItemLogic.PlayerListItemType.Invalid);
            }      
            m_SelectPlayerItem = null;
            return;
        }
        if (m_SelectPlayerItem != null)
        {
            m_SelectPlayerItem.OnCancelSelectItem();
        }
        m_SelectPlayerItem = selectItem;
        m_SelectPlayerItem.OnSelectItem();

        if (m_ButtonWindow != null)
        {
            m_ButtonWindow.SetPlayerListItemInfo(selectItem.GUID, selectItem.TeamID, selectItem.m_PlayerName, selectItem.ItemType);
        }
    }

    public void HandleRetTrail(int nSceneClass, int nSceneInst, int nPosX, int nPosZ)
    {
        m_TrailSceneClass = nSceneClass;
        m_TrailSceneInst = nSceneInst;
        m_TrailPosX = nPosX;
        m_TrailPosZ = nPosZ;

        string strContent = "";
        Tab_SceneClass tabSceneClass = TableManager.GetSceneClassByID(nSceneClass, 0);
        if (tabSceneClass != null)
        {
            if (tabSceneClass.Type != (int)GameDefine_Globe.SCENE_TYPE.SCENETYPE_COPYSCENE &&
                nSceneClass != (int)GameDefine_Globe.SCENE_DEFINE.SCENE_JIANYU)
            {
                //strContent = StrDictionary.GetClientDictionaryString("#{3071}", tabSceneClass.Name, nSceneInst + 1);
				strContent = StrDictionary.GetClientDictionaryString("#{3071}", tabSceneClass.Name);
                MessageBoxLogic.OpenOKCancelBox(strContent, "", TrailOK, TrailCancel);
            }
            else
            {
                //strContent = StrDictionary.GetClientDictionaryString("#{3075}", tabSceneClass.Name, nSceneInst + 1);
				strContent = StrDictionary.GetClientDictionaryString("#{3075}", tabSceneClass.Name);
                MessageBoxLogic.OpenOKCancelBox(strContent, "", TrailCancel);
            } 
        }               
    }

    void TrailOK()
    {
        SceneData.RequestChangeScene((int)CG_REQ_CHANGE_SCENE.CHANGETYPE.TRAIL, 0, m_TrailSceneClass, m_TrailSceneInst, m_TrailPosX, m_TrailPosZ);
        UIManager.CloseUI(UIInfo.RelationRoot);
        if (PlayerFrameLogic.Instance() != null)
        {
            PlayerFrameLogic.Instance().PlayerFrameHeadOnClick();
        }
    }

    void TrailCancel()
    {
        m_TrailSceneClass = GlobeVar.INVALID_ID;
        m_TrailSceneInst = GlobeVar.INVALID_ID;
        m_TrailPosX = 0;
        m_TrailPosZ = 0;
    }
}
