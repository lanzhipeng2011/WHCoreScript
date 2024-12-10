using UnityEngine;
using System.Collections;
using System;
using Games.GlobeDefine;
using System.Collections.Generic;
using Module.Log;
using GCGame;
using Games.ChatHistory;
using Games.LogicObj;
using GCGame.Table;

public class LastSpeakerChatLogic : MonoBehaviour {

    private static LastSpeakerChatLogic m_Instance = null;
    public static LastSpeakerChatLogic Instance()
    {
        return m_Instance;
    }

    public GameObject m_LastSpeakersGrid;                           // 左侧发言玩家列表
    public GameObject m_ButtonMenu;
    public UILabel m_SpeakerNameLabel;
    public UIGrid m_MenuBtnGrid;
    public GameObject m_BtnAddFriend;
    public GameObject m_BtnTeamLeader;
    public GameObject m_BtnTeamMember;
    public GameObject m_BtnBeginChat;
    public ChatInfoLogic m_ChatInfoLogic;

    private List<GameObject> m_LastSpeakersList = new List<GameObject>();       // 发言玩家list
    private UInt64 m_CurSpeakerGUID = GlobeVar.INVALID_GUID;
    private string m_CurSpeakerName = "";
    private GameObject m_LastSpeakerItem = null;

    void Awake()
    {
        m_Instance = this;
    }

	// Use this for initialization
	void Start () {
        
	}

    void OnEnable()
    {
        m_Instance = this;

        if (null != Singleton<ObjManager>.GetInstance().MainPlayer &&
                Singleton<ObjManager>.GetInstance().MainPlayer.NeedRequestGuildInfo)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.ReqGuildInfo();
        }

        if (m_LastSpeakerItem == null)
        {
            UIManager.LoadItem(UIInfo.LastSpeakerItem, OnLoadLastSpeakerItem);
        }
        else
        {
            UpdateSpeakers();
        }
    }

    void OnDisable()
    {
        ClearData();
        m_Instance = null;
    }

    void ClearData()
    {
        m_ButtonMenu.SetActive(false);
        if (m_LastSpeakersList != null)
        {
            m_LastSpeakersList.Clear();
        }        
        m_CurSpeakerGUID = GlobeVar.INVALID_GUID;
        m_CurSpeakerName = "";
    }

    public void OnReceiveChat()
    {
        if (m_ChatInfoLogic == null)
        {
            return;
        }

        int HistoryCount = GameManager.gameManager.PlayerDataPool.ChatHistory.ChatHistoryList.Count;
        ChatHistoryItem LastHistory = GameManager.gameManager.PlayerDataPool.ChatHistory.ChatHistoryList[HistoryCount - 1];

        if (m_ChatInfoLogic.CurChannelType == ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_WORLD)
        {
            // 记录左侧上次发言玩家
            AddLastSpeakers(LastHistory);
        }
        if (m_ChatInfoLogic.CurChannelType == ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_FRIEND)
        {
            if (LastHistory.EChannel == GC_CHAT.CHATTYPE.CHAT_TYPE_FRIEND &&
                LastHistory.SenderGuid != Singleton<ObjManager>.Instance.MainPlayer.GUID)
            {
                LastSpeakerItemLogic[] itemArray = m_LastSpeakersGrid.GetComponentsInChildren<LastSpeakerItemLogic>();
                for (int i = 0; i < itemArray.Length; i++)
                {
                    if (GameManager.gameManager.PlayerDataPool.ChatHistory.FriendSendList.Contains(itemArray[i].SpeakerGuid))
                    {
                        itemArray[i].ShowInform();
                    }
                }
            }
        }
        if (m_ChatInfoLogic.CurChannelType == ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_TELL)
        {
            UpdateSpeakers();
        }
    }

    public void UpdateSpeakers()
    {
        if (m_ChatInfoLogic == null)
        {
            return;
        }

        if (m_LastSpeakerItem == null)
        {
            return;
        }

        Utils.CleanGrid(m_LastSpeakersGrid);
        m_LastSpeakersList.Clear();

        switch (m_ChatInfoLogic.CurChannelType)
        {
            case ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_WORLD:
                {
                    List<ChatHistoryItem> historyList = GameManager.gameManager.PlayerDataPool.ChatHistory.ChatHistoryList;
                    for (int i = 0; i < historyList.Count; ++i)
                    {
                        AddLastSpeakers(historyList[i]);
                    }
                }
                break;
            case ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_TELL:
                {
                    List<LastSpeaker> tellList = GameManager.gameManager.PlayerDataPool.TellChatSpeakers.LastSpeakerList;
                    for (int i = 0; i < tellList.Count; i++ )
                    {
                        AddNewLastSpeaker(tellList[i].Guid, tellList[i].Name);
                    }
                }
                break;
            case ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_NORMAL:
                {
                    foreach (KeyValuePair<string, Obj> objPair in Singleton<ObjManager>.Instance.ObjPools)
                    {
                        if (objPair.Value.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER)
                        {
                            Obj_OtherPlayer objPlayer = objPair.Value as Obj_OtherPlayer;
                            if (objPlayer != null)
                            {
                                AddNewLastSpeaker(objPlayer.GUID, objPlayer.BaseAttr.RoleName);
                            }                            
                        }
                    }
                }
                break;
            case ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_TEAM:
                {
                    if (GameManager.gameManager.PlayerDataPool.IsHaveTeam())
                    {
                        for (int i = 0; i < GlobeVar.MAX_TEAM_MEMBER; i++ )
                        {
                            TeamMember member = GameManager.gameManager.PlayerDataPool.TeamInfo.GetTeamMember(i);
                            if (member != null && member.IsValid() && member.Guid != Singleton<ObjManager>.Instance.MainPlayer.GUID)
                            {
                                AddNewLastSpeaker(member.Guid, member.MemberName);
                            }
                        }                        
                    }
                }
                break;
            case ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_GUILD:
                {
                    if (GameManager.gameManager.PlayerDataPool.IsHaveGuild())
                    {
                        foreach (KeyValuePair<UInt64, GuildMember> memberPair in GameManager.gameManager.PlayerDataPool.GuildInfo.GuildMemberList)
                        {
                            GuildMember member = memberPair.Value;
                            if (member.Guid != GlobeVar.INVALID_GUID &&
                                member.Job != (int)GameDefine_Globe.GUILD_JOB.RESERVE)
                            {
                                AddNewLastSpeaker(member.Guid, member.MemberName);
                            }
                        }
                    }
                }
                break;
            case ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_FRIEND:
                {
                    GameObject lastitem = null;
                    foreach (KeyValuePair<UInt64, Relation> _relation in GameManager.gameManager.PlayerDataPool.FriendList.RelationDataList)
                    {
                        lastitem = AddNewLastSpeaker(_relation.Key, _relation.Value.Name);
                    }
                    if (lastitem != null && lastitem.GetComponent<LastSpeakerItemLogic>() != null)
                    {
                        lastitem.GetComponent<LastSpeakerItemLogic>().ChooseSpeaker();
                    }
                }
                break;
            case ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_SYSTEM:
                break;
        }
    }

    /// <summary>
    /// 增加一个左侧发言玩家
    /// </summary>
    /// <param name="newSenderGuid">发言玩家GUID</param>
    /// <param name="newSenderName">发言玩家名字</param>
    public void AddLastSpeakers(ChatHistoryItem history)
    {
        if (m_LastSpeakersGrid != null && 
            history.EChannel != GC_CHAT.CHATTYPE.CHAT_TYPE_SYSTEM &&
            history.SenderGuid != GlobeVar.INVALID_GUID &&
            history.SenderName != "")
        {
            int nLastSpeakerIndex = GetLastSpeakerIndexByGuid(history.SenderGuid);
            if (nLastSpeakerIndex == GlobeVar.INVALID_ID && m_LastSpeakersList.Count < GlobeVar.MAX_LAST_SPEAKERS)
            {
                // 没有 且发言列表不到上限 直接加
                AddNewLastSpeaker(history.SenderGuid, history.SenderName);
            }
            else if (nLastSpeakerIndex != GlobeVar.INVALID_ID)
            {
                // 发言列表中有 挪位置
                ChangeLastSpeakerItem(nLastSpeakerIndex, history.SenderGuid, history.SenderName);
            }
            else if (m_LastSpeakersList.Count >= GlobeVar.MAX_LAST_SPEAKERS)
            {
                // 没有 且发言列表到上限 删除第一个
                ChangeLastSpeakerItem(0, history.SenderGuid, history.SenderName);
            }
        }
    }

    void OnLoadLastSpeakerItem(GameObject resItem, object param)
    {
        if (null == resItem)
        {
            LogModule.ErrorLog("load LastSpeakerItem error");
            return;
        }

        m_LastSpeakerItem = resItem;

        UpdateSpeakers();
    }

    GameObject AddNewLastSpeaker(UInt64 newSenderGuid, string newSenderName)
    {
        int nInvertedOrder = GlobeVar.MAX_LAST_SPEAKERS - m_LastSpeakersList.Count;
        GameObject LastSpeakerItem = Utils.BindObjToParent(m_LastSpeakerItem, m_LastSpeakersGrid,
                                nInvertedOrder < 10 ?
                                ("LastSpeakerItem" + "0" + nInvertedOrder.ToString()) :
                                ("LastSpeakerItem" + nInvertedOrder.ToString()));

        if (LastSpeakerItem != null && LastSpeakerItem.GetComponent<LastSpeakerItemLogic>() != null)
        {
            LastSpeakerItem.GetComponent<LastSpeakerItemLogic>().Init(newSenderGuid, newSenderName, m_ChatInfoLogic, this);
            m_LastSpeakersList.Add(LastSpeakerItem);
            m_LastSpeakersGrid.GetComponent<UIGrid>().Reposition();
        }

        return LastSpeakerItem;
    }

    /// <summary>
    /// 增加左侧发言玩家时 如果list中玩家存在或list数量已打数量上限 需要位移刷新数据
    /// </summary>
    /// <param name="from">刷新起点</param>
    /// <param name="newSenderGuid">新增发送者GUID</param>
    /// <param name="newSenderName">新增发送者名字</param>
    void ChangeLastSpeakerItem(int from, UInt64 newSenderGuid, string newSenderName)
    {
        for (int i = from; i < m_LastSpeakersList.Count - 1; i++)
        {
            if (null != m_LastSpeakersList[i] && 
                null != m_LastSpeakersList[i + 1] &&
                null != m_LastSpeakersList[i].GetComponent<LastSpeakerItemLogic>())
                m_LastSpeakersList[i].GetComponent<LastSpeakerItemLogic>().CopyFrom(m_LastSpeakersList[i + 1].GetComponent<LastSpeakerItemLogic>());
        }

        if (null != m_LastSpeakersList[m_LastSpeakersList.Count - 1] &&
            null != m_LastSpeakersList[m_LastSpeakersList.Count - 1].GetComponent<LastSpeakerItemLogic>())
            m_LastSpeakersList[m_LastSpeakersList.Count - 1].GetComponent<LastSpeakerItemLogic>().Init(newSenderGuid, newSenderName, m_ChatInfoLogic, this);
    }

    /// <summary>
    /// 查询发言玩家列表中某个玩家的索引
    /// </summary>
    /// <param name="nSpeakerGuid">查询玩家GUID</param>
    /// <returns>若存在返回索引 否则返回-1</returns>
    int GetLastSpeakerIndexByGuid(UInt64 nSpeakerGuid)
    {
        for (int i = 0; i < m_LastSpeakersList.Count; i++)
        {
            if (null != m_LastSpeakersList[i] &&
                null != m_LastSpeakersList[i].GetComponent<LastSpeakerItemLogic>() &&
                m_LastSpeakersList[i].GetComponent<LastSpeakerItemLogic>().SpeakerGuid == nSpeakerGuid)
            {
                return i;
            }
        }
        return GlobeVar.INVALID_ID;
    }

    public void ShowButtonMenu(UInt64 speakerGUID, string speakerName)
    {
        ChoosePlayer(speakerGUID, speakerName);
        ShowMenu(speakerName);
    }

    public void ChoosePlayer(UInt64 speakerGUID, string speakerName)
    {
        m_CurSpeakerGUID = speakerGUID;
        m_CurSpeakerName = speakerName;

        LastSpeakerItemLogic[] itemArray = m_LastSpeakersGrid.GetComponentsInChildren<LastSpeakerItemLogic>();
        for (int i = 0; i < itemArray.Length; i++)
        {
            if (null != itemArray[i] && itemArray[i].SpeakerGuid != speakerGUID)
            {
                itemArray[i].CancelChoose();
            }
        }
    }

    void ShowMenu(string speakerName)
    {
        m_SpeakerNameLabel.text = speakerName;

        m_ButtonMenu.SetActive(true);
        m_BtnAddFriend.SetActive(true);
        m_BtnTeamLeader.SetActive(true);
        m_BtnTeamMember.SetActive(true);
        m_BtnBeginChat.SetActive(true);
        if (m_ChatInfoLogic.CurChannelType == ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_FRIEND)
        {
            m_BtnAddFriend.SetActive(false);
            m_BtnBeginChat.SetActive(false);
        }
        if (m_ChatInfoLogic.CurChannelType == ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_TEAM)
        {
            m_BtnTeamLeader.SetActive(false);
            m_BtnTeamMember.SetActive(false);
        }
        m_MenuBtnGrid.Reposition();
    }

    // 以下为菜单十个按钮的响应函数
    void SetTellChat()
    {
        if (m_CurSpeakerGUID != GlobeVar.INVALID_GUID && 
            m_CurSpeakerGUID != Singleton<ObjManager>.Instance.MainPlayer.GUID)
        {
            //发起私聊
            //肯定打开了。。最近发言列表就在聊天界面左侧
            if (null != m_ChatInfoLogic)
            {
                m_ChatInfoLogic.BeginChat(m_CurSpeakerGUID, m_CurSpeakerName);
            }
        }

        CloseButtonMenu();
    }

    void AddFriend()
    {
        //如果非玩家，则不显示
        if (m_CurSpeakerGUID == GlobeVar.INVALID_GUID)
        {
            return;
        }

        //如果目标是自己也不发送加好友
        if (m_CurSpeakerGUID == Singleton<ObjManager>.GetInstance().MainPlayer.GUID)
        {
            return;
        }

        if (GameManager.gameManager.PlayerDataPool.FriendList.IsExist(m_CurSpeakerGUID))
        {
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2394}");
            return;
        }

		Singleton<ObjManager>.GetInstance().MainPlayer.ReqAddFriend(m_CurSpeakerGUID);

        //CG_ADDFRIEND msg = (CG_ADDFRIEND)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ADDFRIEND);
        //msg.Guid = m_CurSpeakerGUID;
        //msg.SendPacket();

        // Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2906}");

        CloseButtonMenu();
    }

    void PlayerView()
    {
        //如果非玩家，则无效
        if (GlobeVar.INVALID_GUID == m_CurSpeakerGUID)
        {
            return;
        }
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        //如果目标是自己也不发送
        if (Singleton<ObjManager>.GetInstance().MainPlayer.GUID == m_CurSpeakerGUID)
        {
            return;
        }

        Singleton<ObjManager>.GetInstance().MainPlayer.ReqViewOtherPlayer(m_CurSpeakerGUID, OtherRoleViewLogic.OPEN_TYPE.OPEN_TYPE_LASTSPEAKER);

        CloseButtonMenu();
    }

    void InviteInGuild()
    {
        CloseButtonMenu();
    }

    void InviteInTeam_SelfCaptain()
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        //如果有队伍，则判断下队伍是否已满
        if (GlobeVar.INVALID_ID != GameManager.gameManager.PlayerDataPool.TeamInfo.TeamID)
        {
            if (GameManager.gameManager.PlayerDataPool.TeamInfo.IsFull())
            {
				Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1145}");
                return;
            }
        }

        if (m_CurSpeakerGUID != GlobeVar.INVALID_GUID &&
            m_CurSpeakerGUID != Singleton<ObjManager>.Instance.MainPlayer.GUID)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.ReqInviteTeam(m_CurSpeakerGUID);
        }

        CloseButtonMenu();
    }

    void InviteInTeam_SelfMember()
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }
        /*ReqJoinTeam会检查
        if (GlobeVar.INVALID_ID != GameManager.gameManager.PlayerDataPool.TeamInfo.TeamID)
        {
            return;
        }*/

        if (m_CurSpeakerGUID != GlobeVar.INVALID_GUID &&
            m_CurSpeakerGUID != Singleton<ObjManager>.Instance.MainPlayer.GUID)
        {
			if (GameManager.gameManager.PlayerDataPool.IsHaveTeam ()) 
			{
				MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{4605}"), "", TeamLeave);
			}
            Singleton<ObjManager>.GetInstance().MainPlayer.ReqJoinTeam(m_CurSpeakerGUID);
        }

        CloseButtonMenu();
    }

	void TeamLeave()
	{
		if (GameManager.gameManager.PlayerDataPool.TeamInfo.TeamID != GlobeVar.INVALID_ID)
		{
			if (null != Singleton<ObjManager>.GetInstance().MainPlayer)
			{
				Singleton<ObjManager>.GetInstance().MainPlayer.ReqLeaveTeam();
			}
		}
	}
	
	void ReportIllegal()
	{
		CloseButtonMenu();
	}

    void ShieldPlayer()
    {
        //如果非玩家，则无效
        if (GlobeVar.INVALID_GUID == m_CurSpeakerGUID)
        {
            return;
        }
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        //如果目标是自己也不发送
        if (Singleton<ObjManager>.GetInstance().MainPlayer.GUID == m_CurSpeakerGUID)
        {
            return;
        }

        Singleton<ObjManager>.GetInstance().MainPlayer.ReqAddBlack(m_CurSpeakerGUID);

        CloseButtonMenu();
    }
    //寄售行求购
    void ConsignSaleInfoBt()
    {
        UIManager.ShowUI(UIInfo.ConsignSaleRoot, BuyItemOpenConsignSale, m_CurSpeakerName);
    }

    void BuyItemOpenConsignSale(bool bSuccess, object speakerName)
    {
        if (bSuccess)
        {
            if (ConsignSaleLogic.Instance() != null && speakerName !=null)
            {
                string _SpeakerName = (string) speakerName;
                ConsignSaleLogic.Instance().SearchForAskBuy(_SpeakerName);
            }
        }
    }
    void CloseButtonMenu()
    {
        m_ButtonMenu.SetActive(false);
        m_CurSpeakerGUID = GlobeVar.INVALID_GUID;
        m_CurSpeakerName = "";
        m_SpeakerNameLabel.text = "";
    }
    //邀请入帮
    void InviteGuild()
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        if (GlobeVar.INVALID_GUID == GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid)
        {
            //你当前未加入帮会，无法邀请。
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2686}");
            return;
        }

        if (m_CurSpeakerGUID != GlobeVar.INVALID_GUID &&
            m_CurSpeakerGUID != Singleton<ObjManager>.Instance.MainPlayer.GUID)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.ReqInviteGuild(m_CurSpeakerGUID);
        }
    }

    //申请入帮
    void ReqJoinGuild()
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }
        if ( m_CurSpeakerGUID == Singleton<ObjManager>.Instance.MainPlayer.GUID)
        {
            return;
        }

        if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid != GlobeVar.INVALID_GUID)
        {
            GuildMember mySelfGuildInfo = GameManager.gameManager.PlayerDataPool.GuildInfo.GetMainPlayerGuildInfo();
            if (null != mySelfGuildInfo)
            {
                if (mySelfGuildInfo.Job == (int)GameDefine_Globe.GUILD_JOB.RESERVE)
                {
                    //只能同时申请一个帮会，将替换原来的请求，是否继续？
                    string dicStr = StrDictionary.GetClientDictionaryString("#{1861}");
                    MessageBoxLogic.OpenOKCancelBox(dicStr, "", AgreeChangeJoinGuildRequest, null);
                    return;
                }
            }
            //你当前已经有帮会，无法申请。
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{3094}");
            return;
        }
        Singleton<ObjManager>.GetInstance().MainPlayer.ReqJoinOtherPlayerGuild(m_CurSpeakerGUID, m_CurSpeakerName);
    }

    void AgreeChangeJoinGuildRequest()
    {
        if (Singleton<ObjManager>.GetInstance().MainPlayer != null)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.ReqJoinOtherPlayerGuild(m_CurSpeakerGUID, m_CurSpeakerName);
        }
    }

    // 响应函数结束
}
