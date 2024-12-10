using UnityEngine;
using System.Collections;
using Games.Item;
using GCGame;
using Games.ChatHistory;
using Games.GlobeDefine;
using System;
using Games.SwordsMan;
using GCGame.Table;
public class ChatLinkLogic : MonoBehaviour {

    private GC_CHAT.LINKTYPE m_LinkType = GC_CHAT.LINKTYPE.LINK_TYPE_INVALID;

    private GameItem m_EquipOrItemLink = new GameItem();
    public GameItem EquipOrItemLink
    {
        get { return m_EquipOrItemLink; }
        set { m_EquipOrItemLink = value; }
    }

    private UInt64 m_playerGUID = GlobeVar.INVALID_GUID;
    public UInt64 playerGUID
    {
        get { return m_playerGUID; }
        set { m_playerGUID = value; }
    }

    private string m_playerName = "";
    public string PlayerName
    {
        get { return m_playerName; }
        set { m_playerName = value; }
    }

    private UInt64 m_playerInfoGUID = GlobeVar.INVALID_GUID;
    public UInt64 playerInfoGUID
    {
        get { return m_playerInfoGUID; }
        set { m_playerInfoGUID = value; }
    }

    private UInt64 m_guildIdForRecruit = GlobeVar.INVALID_GUID;
    public UInt64 guildIdForRecruit
    {
        get { return m_guildIdForRecruit; }
        set { m_guildIdForRecruit = value; }
    }

    private SwordsMan m_SwordsManLink = new SwordsMan();
    public SwordsMan SwordsManLink
    {
        get { return m_SwordsManLink; }
        set { m_SwordsManLink = value; }
    }

    private bool m_bNameLink = false;

    public struct MoveToLinkInfo
    {
        public int SceneClassID;
        public int SceneInstID;
        public int PosX;
        public int PosZ;
    }

    private MoveToLinkInfo m_MoveToLink = new MoveToLinkInfo();

	// Use this for initialization
	void Start () {
	
	}

    public void Init(ChatHistoryItem history, int linkindex)
    {
        if (linkindex < 0 || linkindex >= history.ELinkType.Count)
            return;

        m_LinkType = history.ELinkType[linkindex];

        if (m_LinkType == GC_CHAT.LINKTYPE.LINK_TYPE_ITEM)
        {
            Utils.ReadLinkFromHistory_Item(history, m_EquipOrItemLink, linkindex);
        }
        else if (m_LinkType == GC_CHAT.LINKTYPE.LINK_TYPE_EQUIP)
        {
            Utils.ReadLinkFromHistory_Equip(history, m_EquipOrItemLink, linkindex);
        }
        else if (m_LinkType == GC_CHAT.LINKTYPE.LINK_TYPE_COPYTEAM)
        {
            Utils.ReadLinkFromHistory_Team(history, out m_playerGUID);
        }
        else if (m_LinkType == GC_CHAT.LINKTYPE.LINK_TYPE_MOVETO)
        {
            Utils.ReadLinkFromHistory_MoveTo(history, ref m_MoveToLink, linkindex);
        }
        else if (m_LinkType == GC_CHAT.LINKTYPE.LINK_TYPE_PLAYERINFO)
        {
            Utils.ReadLinkFromHistory_PlayerInfo(history, out m_playerInfoGUID, linkindex);
        }
        else if (m_LinkType == GC_CHAT.LINKTYPE.LINK_TYPE_SWORDSMAN)
        {
            Utils.ReadLinkFromHistory_SwordsMan(history, m_SwordsManLink, linkindex);
        }
        else if (m_LinkType == GC_CHAT.LINKTYPE.LINK_TYPE_GUILDCRUITE)
        {
            Utils.ReadLinkFromHistory_GuildInfo(history,out m_guildIdForRecruit,linkindex);
        }
    }

    public void Init_NameLink(UInt64 playerGUID, string playerName)
    {
        m_playerGUID = playerGUID;
        m_playerName = playerName;
        m_LinkType = GC_CHAT.LINKTYPE.LINK_TYPE_INVALID;
        m_bNameLink = true;
    }

    void LinkOnClick()
    {
        if (m_LinkType == GC_CHAT.LINKTYPE.LINK_TYPE_ITEM)
        {
            ItemTooltipsLogic.ShowItemTooltip(m_EquipOrItemLink, ItemTooltipsLogic.ShowType.ChatLink);
        }
        else if (m_LinkType == GC_CHAT.LINKTYPE.LINK_TYPE_EQUIP)
        {            
            EquipTooltipsLogic.ShowEquipTooltip(m_EquipOrItemLink, EquipTooltipsLogic.ShowType.ChatLink);
        }
        else if (m_LinkType == GC_CHAT.LINKTYPE.LINK_TYPE_COPYTEAM)
        {
            //检查自己
            if (GameManager.gameManager.PlayerDataPool.IsHaveTeam())
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2179}");
                return;
                
            }
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2178}");
            //发送请求            
            CG_REQ_TEAM_JOIN packet = (CG_REQ_TEAM_JOIN)PacketDistributed.CreatePacket(MessageID.PACKET_CG_REQ_TEAM_JOIN);
            packet.SetTeamMemberGuid(m_playerGUID);
            packet.SendPacket();
        }
        else if (m_LinkType == GC_CHAT.LINKTYPE.LINK_TYPE_MOVETO)
        {
            AutoSearchPoint point = new AutoSearchPoint(m_MoveToLink.SceneClassID, m_MoveToLink.PosX, m_MoveToLink.PosZ);
            GameManager.gameManager.AutoSearch.BuildPath(point);
        }
        else if (m_LinkType == GC_CHAT.LINKTYPE.LINK_TYPE_SWORDSMAN)
        {
            SwordsManToolTipsLogic.ShowSwordsManTooltip(m_SwordsManLink, SwordsManToolTipsLogic.SwordsMan_ShowType.ChatLink);
        }
        else if (m_LinkType == GC_CHAT.LINKTYPE.LINK_TYPE_PLAYERINFO)
        {
            if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
            {
                return;
            }
            Singleton<ObjManager>.GetInstance().MainPlayer.ReqViewOtherPlayer(m_playerInfoGUID, OtherRoleViewLogic.OPEN_TYPE.OPEN_TYPE_LASTSPEAKER);
        }
        else if (m_LinkType == GC_CHAT.LINKTYPE.LINK_TYPE_GUILDCRUITE)
        {
            //如果符合要求，则发送申请加入帮会的请求
            ApplyToJoinGuild();
        }
        else
        {
            if (m_bNameLink)
            {
                if (Singleton<ObjManager>.GetInstance().MainPlayer == null)
                {
                    return;
                }

                if (m_playerGUID == GlobeVar.INVALID_GUID)
                {
                    return;
                }

                if (m_playerGUID == Singleton<ObjManager>.GetInstance().MainPlayer.GUID)
                {
                    return;
                }

                if (null == ChatInfoLogic.Instance())
                {
                    UIManager.ShowUI(UIInfo.ChatInfoRoot, ShowChatInfoRootOver);
                }
                else
                {
                    ChatInfoLogic.Instance().BeginChat(m_playerGUID, m_playerName);
                }
            }
        }
    }

    void ApplyToJoinGuild()
    {
        //玩家等级判断
        if (GameManager.gameManager.PlayerDataPool.MainPlayerBaseAttr.Level < GlobeVar.JOIN_GUILD_LEVEL)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1780}");    //你的人物等级不足20级，无法加入帮会
            return;
        }
        //你当前已经有帮会，无法申请。
        if (GameManager.gameManager.PlayerDataPool.IsHaveGuild())
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{3094}");
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
            //直接申请
            else
            {
                AgreeChangeJoinGuildRequest();
            }
            return;
        }
        //直接申请
        else
        {
            AgreeChangeJoinGuildRequest();
        }
    }

    void AgreeChangeJoinGuildRequest()
    {
        if (null != Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.ReqJoinGuild(m_guildIdForRecruit);
        }
    }

    void ShowChatInfoRootOver(bool bSuccess, object param)
    {
        if (bSuccess)
        {
            //发起私聊
            if (null != ChatInfoLogic.Instance())
            {
                ChatInfoLogic.Instance().BeginChat(m_playerGUID, m_playerName);
            }
        }
    }
}
