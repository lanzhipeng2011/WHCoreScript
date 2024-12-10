using UnityEngine;
using System;
using System.Collections;
using Games.Item;
using Module.Log;

public class ShareTargetChooseLogic : MonoBehaviour {

    private static ShareTargetChooseLogic m_Instance = null;
    public static ShareTargetChooseLogic Instance()
    {
        return m_Instance;
    }
    private static string m_additionShareMsg =""; //分享附加的文字
    public static string AdditionShareMsg
    {
        get { return m_additionShareMsg; }
        set { m_additionShareMsg = value; }
    }
    
    private enum SHARE_TARGET
    {
        TARGET_CHATINFO = 0,
        TARGET_LOUDSPEAKER = 1,
    }

    public TabController m_TabController;

    private SHARE_TARGET m_eShareTarget = SHARE_TARGET.TARGET_CHATINFO;
    private ChatInfoLogic.LINK_TYPE m_eShareType = ChatInfoLogic.LINK_TYPE.LINK_TYPE_INVALID;

    private GameItem m_EquipOrItemBuffer = new GameItem();
    private UInt64 m_GuildForApplyBuffer = 0;
    

    void Awake()
    {
        m_Instance = this;
        m_TabController.delTabChanged = ShareTargetOnClick;
    }

	// Use this for initialization
	void Start () {
	
	}

    void OnDestroy()
    {
        m_Instance = null;
    }

    public static void InitEquipShare(GameItem item,string additionShareMsg ="")
    {
        m_additionShareMsg = additionShareMsg;
        UIManager.ShowUI(UIInfo.ShareTargetChooseRoot, OnLoadEquipShare, item);
    }

    static void OnLoadEquipShare(bool bSuccess, object item)
    {
        if (!bSuccess)
        {
            LogModule.ErrorLog("load ShareTargetChooseRoot fail");
        }
        GameItem curItem = item as GameItem;
        if (ShareTargetChooseLogic.Instance() != null)
        {
            ShareTargetChooseLogic.Instance().InitEquipShareInfo(curItem);
        }
    }
    void InitEquipShareInfo(GameItem item)
    {
        m_EquipOrItemBuffer = item;
        if (item != null && item.IsValid())
        {
            m_eShareType = ChatInfoLogic.LINK_TYPE.LINK_TYPE_EQUIP;
        }
    }

    public static void InitItemShare(GameItem item, string additionShareMsg ="")
    {
        m_additionShareMsg =additionShareMsg;
        UIManager.ShowUI(UIInfo.ShareTargetChooseRoot, OnLoadItemShare, item);
       
    }

    static void OnLoadItemShare(bool bSuccess, object item)
    {
        if (!bSuccess)
        {
            LogModule.ErrorLog("load ShareTargetChooseRoot fail");
        }
        GameItem curItem = item as GameItem;
        if (ShareTargetChooseLogic.Instance() != null)
        {
            ShareTargetChooseLogic.Instance().InitItemShareInfo(curItem);
        }
    }

    void InitItemShareInfo(GameItem item)
    {
        m_EquipOrItemBuffer = item;
        if (item != null && item.IsValid())
        {
            m_eShareType = ChatInfoLogic.LINK_TYPE.LINK_TYPE_ITEM;
        }
    }

    public static void InitGuildShare(UInt64 guildId, string additionShareMsg = "")
    {
        m_additionShareMsg = additionShareMsg;
        //m_additionShareMsg += "[" + GameManager.gameManager.PlayerDataPool.GuildInfo.GuildName + "]";
        UIManager.ShowUI(UIInfo.ShareTargetChooseRoot, OnLoadGuildShare, guildId);
    }

    static void OnLoadGuildShare(bool bSuccess, object item)
    {
        if (!bSuccess)
        {
            LogModule.ErrorLog("load ShareTargetChooseRoot fail");
        }

        UInt64 guildId = Convert.ToUInt64(item);
        
        if (ShareTargetChooseLogic.Instance() != null)
        {
            ShareTargetChooseLogic.Instance().InitGuildShareInfo(guildId);
        }
    }

    void InitGuildShareInfo(UInt64 guild)
    {
        m_GuildForApplyBuffer = guild;
        if (guild != 0)
        {
            m_eShareType = ChatInfoLogic.LINK_TYPE.LINK_TYPE_GUILD;
        }
    }

    void ShareTargetOnClick(TabButton value)
    {
        if (value.name.Contains("ChatInfo"))
        {
            m_eShareTarget = SHARE_TARGET.TARGET_CHATINFO;
        }
        else if (value.name.Contains("LoudSpeaker"))
        {
            m_eShareTarget = SHARE_TARGET.TARGET_LOUDSPEAKER;
        }
    }

    void ShareTargetOK()
    {
        if (EquipTooltipsLogic.Instance() != null)
        {
            EquipTooltipsLogic.Instance().CloseWindow();
        }
        if (ItemTooltipsLogic.Instance() != null)
        {
            ItemTooltipsLogic.Instance().CloseWindow();
        }
        //寄售行的吆喝 记录下吆喝时间
        if (ConsignSaleLogic.Instance() !=null)
        {
            GameManager.gameManager.PlayerDataPool.LastConsignShareTime = Time.time;
        }
        UIManager.ShowUI(UIInfo.ChatInfoRoot, ShowChatInfoRootOver);
    }

    void ShareTargetCancel()
    {
        UIManager.CloseUI(UIInfo.ShareTargetChooseRoot);
    }

    void ShowChatInfoRootOver(bool bSuccess, object param)
    {
        if (null == ChatInfoLogic.Instance())
            return;

        if (bSuccess)
        {
            if (m_eShareTarget == SHARE_TARGET.TARGET_CHATINFO && null != ChatInfoLogic.Instance())
            {
                if (m_eShareType == ChatInfoLogic.LINK_TYPE.LINK_TYPE_EQUIP)
                {
                    ChatInfoLogic.Instance().InsertEquipLinkText(m_EquipOrItemBuffer);
                }
                else if (m_eShareType == ChatInfoLogic.LINK_TYPE.LINK_TYPE_ITEM)
                {
                    ChatInfoLogic.Instance().InsertItemLinkText(m_EquipOrItemBuffer);
                }
                else if (m_eShareType == ChatInfoLogic.LINK_TYPE.LINK_TYPE_GUILD)
                {
                    ChatInfoLogic.Instance().InsertGuildLinkText(m_GuildForApplyBuffer);
                }
            }
            else if (m_eShareTarget == SHARE_TARGET.TARGET_LOUDSPEAKER && null != LoudSpeakerLogic.Instance())
            {
                LoudSpeakerLogic.Instance().ShowLoudSpeaker();
                if (m_eShareType == ChatInfoLogic.LINK_TYPE.LINK_TYPE_EQUIP)
                {
                    LoudSpeakerLogic.Instance().InsertEquipLinkText(m_EquipOrItemBuffer);
                }
                else if (m_eShareType == ChatInfoLogic.LINK_TYPE.LINK_TYPE_ITEM)
                {
                    LoudSpeakerLogic.Instance().InsertItemLinkText(m_EquipOrItemBuffer);
                }
                else if (m_eShareType == ChatInfoLogic.LINK_TYPE.LINK_TYPE_GUILD)
                {
                    LoudSpeakerLogic.Instance().InsertGuildLinkText(m_GuildForApplyBuffer);
                }
            }

            m_EquipOrItemBuffer = null;
            m_GuildForApplyBuffer = 0;
            UIManager.CloseUI(UIInfo.ShareTargetChooseRoot);
        }
    }
}
