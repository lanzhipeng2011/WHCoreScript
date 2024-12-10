using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
using GCGame;
using Module.Log;
using GCGame.Table;
using System;
public class RelationTeamWindow : MonoBehaviour {

    public GameObject teamListGrid;
    public TabController m_TabController;
    public UIImageButton m_CreateTeamButton;        //创建队伍按钮
    public RelationButtionWindowLogic m_ButtonWindow;
    public GameObject m_TeamMemberSceneInfo;

    public UILabel m_TeamMemberSceneName;
    public UILabel m_TeamMemberSceneChannel;
    public UILabel m_TeamMemberScenePos;

    private PlayerListItemLogic m_SelectPlayerItem;

    public UIGrid m_TabGrid;
	public UISprite  m_hig;
    public enum TeamTab
    {
        TeamTab_TeamInfo,
        TeamTab_NearPlayer,
        TeamTab_NearTeam,
    };

	void Awake () {
        
        m_TabController.delTabChanged = OnTabChanged;
	}

    void Start()
    {
        teamListGrid.GetComponent<UIGrid>().Reposition();
    }

    void OnEnable()
    {
        GUIData.delNearbyTeampUpdate += UpdateNearbyTeam;
        GUIData.delNearbyPlayerUpdate += UpdateNearbyPlayer;
        GUIData.delTeamDataUpdate += OnTeamInfoUpdate;
        SelectPlayerListItem(null);
        m_TabController.ChangeTab("0");

    }

    void OnDisable()
    {
        GUIData.delNearbyTeampUpdate -= UpdateNearbyTeam;
        GUIData.delNearbyPlayerUpdate -= UpdateNearbyPlayer;
        GUIData.delTeamDataUpdate -= OnTeamInfoUpdate;
        SelectPlayerListItem(null);
       
    }

    public void ChangeTeamTab(TeamTab Tab)
    {
        if (m_TabController == null)
        {
            return;
        }
        switch (Tab)
        {
            case TeamTab.TeamTab_TeamInfo:
                {
                    m_TabController.ChangeTab("0");
                }
                break;
            case TeamTab.TeamTab_NearPlayer:
                {
                    m_TabController.ChangeTab("1");
                }
                break;
            case TeamTab.TeamTab_NearTeam:
                {
                    m_TabController.ChangeTab("2");
                }
                break;
           default:
                {
                    break;
                }
        }
    }

    void OnTabChanged(TabButton curButton)
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            Module.Log.LogModule.ErrorLog("OnTabChanged:: MainPlayer is null");
            return;
        }
        GameObject curTab = m_TabController.GetHighlightTab().gameObject;
        if (null == curTab)
        {
            Module.Log.LogModule.ErrorLog("OnTabChanged:: curTab is null");
            return;
        }

        if (curTab.name == "0")
        {
            UpdateTeamInfo(Singleton<ObjManager>.GetInstance().MainPlayer.GUID);
        }
        else if (curTab.name == "1")
        {
            Utils.CleanGrid(teamListGrid);
            SelectPlayerListItem(null);
            ReqNearbyPlayer();
        }
        else if (curTab.name == "2")
        {
            Utils.CleanGrid(teamListGrid);
            SelectPlayerListItem(null);
            ReqNearbyTeam();
        }
    }

    public void OnTeamInfoUpdate()
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
             Module.Log.LogModule.ErrorLog("OnTeamInfoUpdate:: MainPlayer is null");
             return;
        }
        UInt64 oLastSelectGUID = Singleton<ObjManager>.GetInstance().MainPlayer.GUID;
        GameObject curTab = m_TabController.GetHighlightTab().gameObject;

        if (curTab != null && "0" == curTab.name)
        {
           if ( m_SelectPlayerItem != null)
           {
               oLastSelectGUID = m_SelectPlayerItem.GUID;
           }
        }
		UpdateTeamInfo(oLastSelectGUID);
       
    }
    //更新组队信息
    void UpdateTeamInfo(UInt64 oLastSelectMemberGuid)
    {
        if (false == GameManager.gameManager.PlayerDataPool.IsHaveTeam())
        {
            m_CreateTeamButton.gameObject.SetActive(true);
        }
        else
        {
            m_CreateTeamButton.gameObject.SetActive(false);
        }

        GameObject curTab = m_TabController.GetHighlightTab().gameObject;
        if (null == curTab)
        {
            LogModule.ErrorLog("UpdateTeamInfo curTab is null");
            return;
        }

        TabButton tabButton = m_TabController.GetTabButton("0");
        if (null == tabButton)
        {
            LogModule.ErrorLog("UpdateTeamInfo tabButton is null");
            return;
        }

        if (false == GameManager.gameManager.PlayerDataPool.IsHaveTeam())
        {
            tabButton.gameObject.SetActive(false);
			ChageTab_pic(true);

        }
        else
        {
            tabButton.gameObject.SetActive(true);
			ChageTab_pic(false);
        }
        if (m_TabGrid != null)
        {
            m_TabGrid.repositionNow = true;
        }  
        
        if (GameManager.gameManager.PlayerDataPool.IsHaveTeam())
        {
            if (curTab.name != "0")
            {
                ChangeTeamTab(TeamTab.TeamTab_TeamInfo);
                return;
            }
            UIManager.LoadItem(UIInfo.PlayerListItem, OnLoadTeamItem, oLastSelectMemberGuid);
        }
        else
        {
            ChangeTeamTab(TeamTab.TeamTab_NearPlayer);
        }     
    }

    void OnLoadTeamItem(GameObject resItem, object param)
    {
        //如果位选中该分页，则执行选中操作，否则执行刷新操作
        GameObject curTab = m_TabController.GetHighlightTab().gameObject;
        if (null == curTab)
        {
            LogModule.ErrorLog("OnLoadTeamItem curTab is null");
            return;
        }
        
        if (curTab.name != "0")
        {
            return;
        }

        if (null == resItem)
        {
            Module.Log.LogModule.ErrorLog("OnLoadTeamItem fail resItem is null");
            return;
        }

         if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
             Module.Log.LogModule.ErrorLog("OnLoadTeamItem:: MainPlayer is null");
             return;
        }

        Utils.CleanGrid(teamListGrid);
        SelectPlayerListItem(null);
        //ClearPlayerListItem();
        //首先判断是否有队伍
        if (GameManager.gameManager.PlayerDataPool.TeamInfo.TeamID < 0)
        {
            return;
        }

        m_CreateTeamButton.gameObject.SetActive(false);

        UInt64 oLastSelectGUID = (UInt64)param;
        bool bLastSelectExsit = false;
        for (int i = 0; i < GlobeVar.MAX_TEAM_MEMBER; i++)
        {
            if (null != GameManager.gameManager.PlayerDataPool.TeamInfo.GetTeamMember(i) &&
                true == GameManager.gameManager.PlayerDataPool.TeamInfo.GetTeamMember(i).IsValid())
            {
                if (oLastSelectGUID == GameManager.gameManager.PlayerDataPool.TeamInfo.GetTeamMember(i).Guid)
                {
                    bLastSelectExsit = true;
                    break;
                }
            }
        }
        if (false == bLastSelectExsit)
        {
            oLastSelectGUID = Singleton<ObjManager>.GetInstance().MainPlayer.GUID;
        }

        for (int i = 0; i < GlobeVar.MAX_TEAM_MEMBER; i++)
        {
            if (null != GameManager.gameManager.PlayerDataPool.TeamInfo.GetTeamMember(i) &&
                true == GameManager.gameManager.PlayerDataPool.TeamInfo.GetTeamMember(i).IsValid())
            {
                //GameObject newPlayerListItem = Utils.BindObjToParent(resItem, teamListGrid, i.ToString());
                //newPlayerListItem.GetComponent<PlayerListItemLogic>().InitPlayerListItemInfo(GameManager.gameManager.PlayerDataPool.TeamInfo.GetTeamMember(i));
                PlayerListItemLogic PlayerListItem = PlayerListItemLogic.CreateItem(teamListGrid, resItem, i.ToString(), this);
                if (PlayerListItem == null)
                {
                    continue;
                }
                PlayerListItem.InitPlayerListItemInfo(GameManager.gameManager.PlayerDataPool.TeamInfo.GetTeamMember(i), i);
                if (m_SelectPlayerItem == null &&
                    oLastSelectGUID == GameManager.gameManager.PlayerDataPool.TeamInfo.GetTeamMember(i).Guid)
                {
                    SelectPlayerListItem(PlayerListItem);
                }                 
            }
        }
        teamListGrid.GetComponent<UIGrid>().Reposition();
    }


    //更新附近队伍
    public void UpdateNearbyTeam(GC_NEAR_TEAMLIST packet)
    {
        if (null == packet)
        {
            return;
        }
        UIManager.LoadItem(UIInfo.PlayerListItem, OnLoadNearbyTeamItem, packet);
    }

    void OnLoadNearbyTeamItem(GameObject resItem, object param)
    {
        if (null == resItem)
        {
            Module.Log.LogModule.ErrorLog("load friend item fail");
            return;
        }

        GameObject curTab = m_TabController.GetHighlightTab().gameObject;
        if (curTab.name != "2")
        {
            return;
        }

        GC_NEAR_TEAMLIST packet = (GC_NEAR_TEAMLIST)param;
        Utils.CleanGrid(teamListGrid);
        SelectPlayerListItem(null);

        int idCount = packet.GuidCount;
        for (int i = 0; i < idCount; i++)
        {
            NearbyTeam team = new NearbyTeam();
            team.Guid = packet.GetGuid(i);
            team.Name = packet.GetName(i);
            team.Level = packet.GetLevel(i);
            team.Profession = packet.GetProf(i);
            team.CombatNum = packet.GetCombatNum(i);
            team.TeamID = packet.GetTeamID(i);

            //GameObject newPlayerListItem = Utils.BindObjToParent(resItem, teamListGrid, i.ToString());
            //newPlayerListItem.GetComponent<PlayerListItemLogic>().InitPlayerListItemInfo(team);
            PlayerListItemLogic PlayerListItem = PlayerListItemLogic.CreateItem(teamListGrid, resItem, i.ToString(), this);
            if (PlayerListItem == null)
            {
                continue;
            }
            PlayerListItem.InitPlayerListItemInfo(team);
            if (m_SelectPlayerItem == null)
            {
                SelectPlayerListItem(PlayerListItem);
            }
        }
        teamListGrid.GetComponent<UIGrid>().Reposition();
       
    }

    //更新附近玩家
    public void UpdateNearbyPlayer(GC_NEAR_PLAYERLIST packet)
    {
        if (null == packet)
        {
            return;
        }
        UIManager.LoadItem(UIInfo.PlayerListItem, OnLoadNearbyPlayerItem, packet);
    }

    void OnLoadNearbyPlayerItem(GameObject resItem, object param)
    {
        if (null == resItem)
        {
            Module.Log.LogModule.ErrorLog("load friend item fail");
            return;
        }

        GameObject curTab = m_TabController.GetHighlightTab().gameObject;
        if (curTab.name != "1")
        {
            return;
        }
        GC_NEAR_PLAYERLIST packet = (GC_NEAR_PLAYERLIST)param;
        Utils.CleanGrid(teamListGrid);
        SelectPlayerListItem(null);
        int idCount = packet.GuidCount;
        for (int i = 0; i < idCount; i++)
        {
            NearbyPlayer player = new NearbyPlayer();
            player.Guid = packet.GetGuid(i);
            player.Name = packet.GetName(i);
            player.Level = packet.GetLevel(i);
            player.Profession = packet.GetProf(i);
            player.CombatNum = packet.GetCombatNum(i);
            PlayerListItemLogic PlayerListItem = PlayerListItemLogic.CreateItem(teamListGrid, resItem, i.ToString(), this);
            if (PlayerListItem == null)
            {
                continue;
            }
            PlayerListItem.InitPlayerListItemInfo(player);
            if (m_SelectPlayerItem == null)
            {
                SelectPlayerListItem(PlayerListItem);
            }
            //GameObject newPlayerListItem = Utils.BindObjToParent(resItem, teamListGrid, i.ToString());
            //newPlayerListItem.GetComponent<PlayerListItemLogic>().InitPlayerListItemInfo(player);
        }
        teamListGrid.GetComponent<UIGrid>().Reposition();
    }
     

    //请求附近玩家
    void ReqNearbyPlayer()
    {
        if (false == GameManager.gameManager.PlayerDataPool.IsHaveTeam())
        {
            m_CreateTeamButton.gameObject.SetActive(true);
        }
        else
        {
            m_CreateTeamButton.gameObject.SetActive(false);
        }

        CG_REQ_NEAR_LIST packet = (CG_REQ_NEAR_LIST)PacketDistributed.CreatePacket(MessageID.PACKET_CG_REQ_NEAR_LIST);
        packet.IsNearPlayerList = 1;
        packet.SendPacket();
    }

    //请求附近队伍
    void ReqNearbyTeam()
    {
        if (false == GameManager.gameManager.PlayerDataPool.IsHaveTeam())
        {
            m_CreateTeamButton.gameObject.SetActive(true);
        }
        else
        {
            m_CreateTeamButton.gameObject.SetActive(false);
        }

        //发送给服务器请求
        CG_REQ_NEAR_LIST packet = (CG_REQ_NEAR_LIST)PacketDistributed.CreatePacket(MessageID.PACKET_CG_REQ_NEAR_LIST);
        packet.IsNearPlayerList = 0;
        packet.SendPacket();
    }

    //创建队伍按钮
    void OnClickCreateTeam()
    {
        if (false == GameManager.gameManager.PlayerDataPool.IsHaveTeam() &&
            null != Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.ReqInviteTeam(GlobeVar.INVALID_GUID);
        }
    }

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
            if (m_TeamMemberSceneInfo != null)
            {
                m_TeamMemberSceneInfo.SetActive(false);
            }
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
            m_ButtonWindow.SetPlayerListItemInfo(m_SelectPlayerItem.GUID, m_SelectPlayerItem.TeamID, m_SelectPlayerItem.m_PlayerName, m_SelectPlayerItem.ItemType);
        }
        UpdateTeamMemberScenePos(selectItem.TeamPosIndex);
    }

    void UpdateTeamMemberScenePos(int nTeamMemberPosIndex)
    {
        if (null == m_TeamMemberSceneInfo)
        {
            LogModule.ErrorLog("UpdateTeamMemberScenePos::m_TeamMemberSceneInfo is null");
            return;
        }
        if ( null == m_TeamMemberSceneName)
        {
            LogModule.ErrorLog("UpdateTeamMemberScenePos::m_TeamMemberSceneName is null");
            return;
        }
        if (null == m_TeamMemberSceneChannel)
        {
            LogModule.ErrorLog("UpdateTeamMemberScenePos::m_TeamMemberSceneChannel is null");
            return;
        }
        if (null == m_TeamMemberScenePos)
        {
            LogModule.ErrorLog("UpdateTeamMemberScenePos::m_TeamMemberScenePos is null");
            return;
        }
        if (nTeamMemberPosIndex < 0 || nTeamMemberPosIndex >= GlobeVar.MAX_TEAM_MEMBER)
        {     
            m_TeamMemberSceneInfo.SetActive(false);
            return;
        }
        TeamMember member = GameManager.gameManager.PlayerDataPool.TeamInfo.GetTeamMember(nTeamMemberPosIndex);
        if (null == member)
        {
            m_TeamMemberSceneInfo.SetActive(false);
            return;
        }
        if (false == member.IsValid())
        {
            m_TeamMemberSceneInfo.SetActive(false);
            return;
        }

        Tab_SceneClass curScene = TableManager.GetSceneClassByID(member.SceneClassID, 0);
        if (null == curScene)
        {
            m_TeamMemberSceneInfo.SetActive(false);
            return;
        }
        m_TeamMemberSceneInfo.SetActive(true);
        m_TeamMemberSceneName.color = SceneData.GetSceneNameColor(member.SceneClassID);
        m_TeamMemberSceneName.text = curScene.Name;
        //m_TeamMemberSceneChannel.text = StrDictionary.GetClientDictionaryString("{#1177}", member.SceneInstID + 1);
        m_TeamMemberScenePos.text = "X:" + ((int)member.ScenePos.x).ToString() + " Y:" + ((int)member.ScenePos.z).ToString();
    }

	void OnClickSortLvl()
	{
		PlayerListItemLogic[] itemList = teamListGrid.GetComponentsInChildren<PlayerListItemLogic> ();
		Array.Sort (itemList, (p1, p2) => p1.PlayerLvl.CompareTo (p2.PlayerLvl));
		for (int i = 0; i < itemList.Length; i++) 
		{
			itemList[i].gameObject.name = (itemList.Length - 1 - i).ToString();	
		}
		teamListGrid.GetComponent<UIGrid> ().Reposition ();
	}

	void OnClickSortBattle()
	{
		PlayerListItemLogic[] itemList = teamListGrid.GetComponentsInChildren<PlayerListItemLogic> ();
		Array.Sort (itemList, (p1, p2) => p1.PlayerBattle.CompareTo (p2.PlayerBattle));
		for (int i = 0; i < itemList.Length; i++) 
		{
			itemList[i].gameObject.name = (itemList.Length - 1 - i).ToString();	
		}
		teamListGrid.GetComponent<UIGrid> ().Reposition ();
	}
	//附近玩家换图片
	void ChageTab_pic(bool isfirst)
	{
		TabButton curTab = m_TabController.GetTabButton("1");
		UISprite  [] pic=
		curTab.gameObject.GetComponentsInChildren<UISprite> ();
		if(isfirst)
		{

			m_hig.spriteName="ui_pub_008";
			m_TabController.transform.localPosition=new Vector3(56.7f,-85.0f,0);
			foreach (UISprite  p in pic) 
			{
			if(p.spriteName=="ui_pub_009")
				p.spriteName="ui_pub_007";
			 if(p.spriteName=="ui_pub_010")
					p.spriteName="ui_pub_008";
			}

		}

		else
		{
			m_TabController.transform.localPosition=new Vector3(-92.6f,-85.0f,0);
			foreach (UISprite  p in pic) 
			{
				if(p.spriteName=="ui_pub_007")
					p.spriteName="ui_pub_009";
				if(p.spriteName=="ui_pub_008")
					p.spriteName="ui_pub_010";
			}
		}

	}
}
