using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GCGame;
using Games.GlobeDefine;
using Module.Log;
using GCGame.Table;
public class TeamPlatformWindow : MonoBehaviour {

    private static TeamPlatformWindow m_Instance = null;
    public static TeamPlatformWindow Instance()
    {
        return m_Instance;
    }
    public TabController m_TabTeamType;        // 附近玩家和附近队伍tab
    public GameObject ItemParent;

    public GameObject ApplicationTeam;
    public GameObject InvitedTeam;
    public UILabel m_title;
    private int Diffcult = 1;
    private int CopyMode = 1;
    private int CopySceneId = -1;
    public UInt64 playerGUID = GlobeVar.INVALID_GUID;
    public int teamID = -1;
    private int modeType = 1;
    struct TeamPlatformplayerInfo
    {
        public UInt64 playerGuid;	//个人GUID
        public string playerName;	//个人名字
        public int playerLevel;	//个人等级
        public int playerProf;	    //个人职业
        public int playerCombat; 	//个人战斗力 
    };
    private static List<TeamPlatformplayerInfo> playerList = new List<TeamPlatformplayerInfo>();
    private static Dictionary<Int32, List<TeamPlatformplayerInfo>> TeamMap = new Dictionary<Int32, List<TeamPlatformplayerInfo>>();
    void Awake()
    {
        m_Instance = this;
    }
    // Use this for initialization
	void Start () {
	
	}

    void OnDestroy()
    {
        m_Instance = null;
    }
	
    // 组队界面被打开调用
    void OnEnable()
    {
        m_TabTeamType.delTabChanged = OnTeamTabChange;
        m_TabTeamType.ChangeTab("NearbyPlayer");
    }
    // 难度等级标签页切换
    void OnTeamTabChange(TabButton button)
    {
        if (button.name == "NearbyPlayer")
        {
           // UIManager.LoadItem(UIInfo.TeamPlatformItem, OnLoadPlayerItem);
            ApplicationTeam.SetActive(false);
            InvitedTeam.SetActive(true);
            modeType = 1;
        }
        else if (button.name == "NearbyTeam")
        {
          //  UIManager.LoadItem(UIInfo.TeamPlatformItem, OnLoadTeamItem);
            ApplicationTeam.SetActive(true);
            InvitedTeam.SetActive(false);
            modeType = 2;
        }
        UpdateTeamItemInfo();
    }
    void OnLoadPlayerItem(GameObject resItem, object param)
    {
        if (resItem == null)
        {
            return;
        }
        Utils.CleanGrid(ItemParent);
//         foreach (TeamPlatformItem item in ItemParent.GetComponentsInChildren<TeamPlatformItem>())
//         {
//             Destroy(item);
//         }
        for (int i = playerList.Count - 1; i >= 0; i--)
        {
            GameObject newItem = Utils.BindObjToParent(resItem, ItemParent);
            if (null == newItem)
            {
                continue;
            }

            TeamPlatformItem newTeamPlatformItem = newItem.GetComponent<TeamPlatformItem>();
            if (null == newTeamPlatformItem)
            {
                continue;
            }

            for (int j = 0; j < 4; j++)
            {
                newTeamPlatformItem.m_playerLevel[j].text = "";
                newTeamPlatformItem.m_playerProf[j].text = "";
                newTeamPlatformItem.m_playerCombat[j].text = "";
            }
            newTeamPlatformItem.m_playerName.text = playerList[i].playerName;
            //newTeamPlatformItem.m_playerLevel[0].text = playerList[i].playerLevel.ToString() + "级";
            newTeamPlatformItem.m_playerLevel[0].text = StrDictionary.GetClientDictionaryString("#{2811}", playerList[i].playerLevel);
            newTeamPlatformItem.m_playerProf[0].text = StrDictionary.GetClientDictionaryString("#{" + CharacterDefine.PROFESSION_DICNUM[playerList[i].playerProf].ToString() + "}");
            //newTeamPlatformItem.m_playerCombat[0].text = playerList[i].playerCombat.ToString() + "战力";
            newTeamPlatformItem.m_playerCombat[0].text = StrDictionary.GetClientDictionaryString("#{2812}", playerList[i].playerCombat);
            newTeamPlatformItem.m_playerHead.spriteName = "";
            newTeamPlatformItem.UpdateData(playerList[i].playerGuid, -1, playerList[i].playerProf);
           
            if (null != GlobeVar.m_HeadIcon[playerList[i].playerProf])
            {
                newTeamPlatformItem.m_playerHead.spriteName = GlobeVar.m_HeadIcon[playerList[i].playerProf];
                newTeamPlatformItem.m_playerHead.MakePixelPerfect();
            }
        }        
        ItemParent.GetComponent<UIGrid>().repositionNow = true;
    }
    void OnLoadTeamItem(GameObject resItem, object param)
    {
        if (resItem == null)
        {
            return;
        }
//         foreach (TeamPlatformItem item in ItemParent.GetComponentsInChildren<TeamPlatformItem>())
//         {
//             Destroy(item);
//         }
        Utils.CleanGrid(ItemParent);
        foreach (KeyValuePair<Int32, List<TeamPlatformplayerInfo>> item in TeamMap)
        {
            int nCount = 0;
            GameObject newItem = Utils.BindObjToParent(resItem, ItemParent);
            if (null == newItem)
            {
                continue;
            }

            TeamPlatformItem newTeamPlatformItem = newItem.GetComponent<TeamPlatformItem>();
            if (null == newTeamPlatformItem)
            {
                continue;
            }

            for (int i = 0; i < 4; i++)
            {
                newTeamPlatformItem.m_playerLevel[i].text = "";
                newTeamPlatformItem.m_playerProf[i].text = "";
                newTeamPlatformItem.m_playerCombat[i].text = "";
            }
            foreach (TeamPlatformplayerInfo platformInfo in item.Value)
            {
                if (nCount >= 4)
                {
                    break;
                }

                //newTeamPlatformItem.m_playerLevel[nCount].text = platformInfo.playerLevel.ToString() + "级";
                newTeamPlatformItem.m_playerLevel[nCount].text = StrDictionary.GetClientDictionaryString("#{2811}", platformInfo.playerLevel);
                newTeamPlatformItem.m_playerProf[nCount].text = StrDictionary.GetClientDictionaryString("#{" + CharacterDefine.PROFESSION_DICNUM[platformInfo.playerProf].ToString() + "}");
                //newTeamPlatformItem.m_playerCombat[nCount].text = platformInfo.playerCombat.ToString() + "战力";
                newTeamPlatformItem.m_playerCombat[nCount].text = StrDictionary.GetClientDictionaryString("#{2812}", platformInfo.playerCombat);
                nCount++;
            }
            newTeamPlatformItem.m_playerName.text = item.Value[0].playerName;
            newTeamPlatformItem.m_playerHead.spriteName = "";
            if (null != GlobeVar.m_HeadIcon[item.Value[0].playerProf])
            {
                newTeamPlatformItem.m_playerHead.spriteName = GlobeVar.m_HeadIcon[item.Value[0].playerProf];
                newTeamPlatformItem.m_playerHead.MakePixelPerfect();
            }
            newTeamPlatformItem.UpdateData(item.Value[0].playerGuid, item.Key, item.Value[0].playerProf);
        }
        ItemParent.GetComponent<UIGrid>().repositionNow = true;
    }
    public void UpdateTeamItemInfo()
    {
        if (modeType == 1)
        {
            UIManager.LoadItem(UIInfo.TeamPlatformItem, OnLoadPlayerItem);

        }
        else if (modeType == 2)
        {
            UIManager.LoadItem(UIInfo.TeamPlatformItem, OnLoadTeamItem);
        }
    }
    public static void addplayerList(UInt64 _playerGuid, string _playerName, int _playerlevel, int _playerProf, int _playerCombat)
    {
        if (playerList == null)
        {
            playerList = new List<TeamPlatformplayerInfo>();
        }
        TeamPlatformplayerInfo info;
        info.playerGuid = _playerGuid;
        info.playerName = _playerName;
        info.playerLevel = _playerlevel;
        info.playerProf = _playerProf;
        info.playerCombat = _playerCombat;
        playerList.Add(info);
    }
    public static void ClearplayerList()
    {
        if (playerList != null)
        {
            playerList.Clear();
        }
    }
    public static void addTeamList(int _teamID,UInt64 _playerGuid, string _playerName, int _playerlevel, int _playerProf, int _playerCombat)
    {
        if (TeamMap.ContainsKey(_teamID))
        {

            TeamPlatformplayerInfo info;
            info.playerGuid = _playerGuid;
            info.playerName = _playerName;
            info.playerLevel = _playerlevel;
            info.playerProf = _playerProf;
            info.playerCombat = _playerCombat;
            TeamMap[_teamID].Add(info);

        }
        else
        {
            List<TeamPlatformplayerInfo> list = new List<TeamPlatformplayerInfo>();
            TeamPlatformplayerInfo info;
            info.playerGuid = _playerGuid;
            info.playerName = _playerName;
            info.playerLevel = _playerlevel;
            info.playerProf = _playerProf;
            info.playerCombat = _playerCombat;
            list.Add(info);
            TeamMap.Add(_teamID, list);
        }

    }
    public static void ClearTeamMap()
    {
        TeamMap.Clear();
    }
    public void UpdateCopyScene(int _CopySceneId,int _CopyMode,int _Diffcult)
    {
        CopySceneId = _CopySceneId;
        CopyMode = _CopyMode;
        Diffcult = _Diffcult;
        ClearplayerList();
        ClearTeamMap();
        if (_CopyMode == 2)
        {
            CG_ASK_TEAMPLATFORMINFO packet = (CG_ASK_TEAMPLATFORMINFO)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_TEAMPLATFORMINFO);
            packet.SceneClassID = _CopySceneId;
            packet.Difficulty = _Diffcult;
            packet.SendPacket();
        }       
        Tab_SceneClass pSceneClass = TableManager.GetSceneClassByID(CopySceneId, 0);
        if (pSceneClass != null)
        {
            if (Diffcult == 1)
            {
                m_title.text = StrDictionary.GetClientDictionaryString("#{2110}", pSceneClass.Name);
            }
            else if (Diffcult == 2)
            {
                m_title.text = StrDictionary.GetClientDictionaryString("#{2111}", pSceneClass.Name);
            }
            else if (Diffcult == 3)
            {
                m_title.text = StrDictionary.GetClientDictionaryString("#{2112}", pSceneClass.Name);
            }            
        }
        
    }
    //申请入队
    public void OnApplicationTeamClick()
    {
       // Utils.CleanGrid(ItemParent);
        if (Singleton<ObjManager>.GetInstance() == null)
        {
            return;
        }
        

        if (playerGUID == GlobeVar.INVALID_GUID || teamID == -1 || playerGUID == Singleton<ObjManager>.GetInstance().MainPlayer.GUID)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1930}");
            return;
        }
        if (CG_REQ_TEAM_JOINSend == false)
        {
            return;
        }
        CG_REQ_TEAM_JOINSend = false;
        CG_REQ_TEAM_JOIN packet = (CG_REQ_TEAM_JOIN)PacketDistributed.CreatePacket(MessageID.PACKET_CG_REQ_TEAM_JOIN);
        packet.SetTeamMemberGuid(playerGUID);
        packet.SendPacket();
    }
    //自动组队
    public void OnAutomaticTeamClick()
    {

        if (CopySceneId == -1 || Diffcult == -1)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1287}");
            return;
        }
        if (CG_ASK_AUTOTEAMSend == false)
        {
            return;
        }

        Tab_SceneClass curScene = TableManager.GetSceneClassByID(GameManager.gameManager.RunningScene, 0);
        if (null == curScene)
        {
            LogModule.ErrorLog("load scene map table fail :" + GameManager.gameManager.RunningScene);
            return;
        }
        if (2 == curScene.Type )
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2343}");
            return;
        }
        CG_ASK_AUTOTEAMSend = false;
        CG_ASK_AUTOTEAM packet = (CG_ASK_AUTOTEAM)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_AUTOTEAM);
        packet.SetSceneClassID(CopySceneId);
        packet.SetDifficulty(Diffcult);
        packet.SendPacket();
    }
    //进入副本
    public void OnEnterFBClick()
    {

        if (Singleton<ObjManager>.GetInstance() == null)
        {
            return;
        }

        if (CopySceneId == -1 || Diffcult == -1 || CopyMode == -1)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1287}");
            return;
        }

        if (Singleton<ObjManager>.Instance.MainPlayer.IsInJianYu())
        {
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2216}");
            return;
        }

        Singleton<ObjManager>.GetInstance().MainPlayer.SendOpenScene(CopySceneId, CopyMode, Diffcult);
    }
    //邀请入队
    public void OnInvitedTeamClick()
    {
        if (Singleton<ObjManager>.GetInstance() == null)
        {
            return;
        }
        if (playerGUID == GlobeVar.INVALID_GUID || playerGUID == Singleton<ObjManager>.GetInstance().MainPlayer.GUID)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1930}");
            return;
        }
        if (CG_REQ_TEAM_INVITESend == false)
        {
            return;
        }
        CG_REQ_TEAM_INVITESend = false;
        if (Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{3170}");
        }
        CG_REQ_TEAM_INVITE packet = (CG_REQ_TEAM_INVITE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_REQ_TEAM_INVITE);
        packet.SetGuid(playerGUID);
        packet.SendPacket();
    }
    private float timeSend = Time.realtimeSinceStartup;
    private bool CG_REQ_TEAM_INVITESend = true;
    private bool CG_ASK_AUTOTEAMSend = true;
    private bool CG_REQ_TEAM_JOINSend = true;
    void Update()
    {
        if (Time.realtimeSinceStartup - timeSend > 1)
        {
            timeSend = Time.realtimeSinceStartup;
            CG_REQ_TEAM_INVITESend = true;
            CG_ASK_AUTOTEAMSend = true;
            CG_REQ_TEAM_JOINSend = true;
        }
    }
}
