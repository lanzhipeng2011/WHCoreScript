//********************************************************************
// 文件名: ChatInfoLogic.cs
// 描述: 聊天信息界面 从底部聊天按钮点开
// 作者: WangZhe
//********************************************************************

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Games.GlobeDefine;
using Games.ChatHistory;
using Games.Item;
using GCGame.Table;
using GCGame;
using Module.Log;
using System.Text.RegularExpressions;

public class ChatInfoLogic : UIControllerBase<ChatInfoLogic> {

    public enum CHANNEL_TYPE
    {
        CHANNEL_TYPE_INVALID = -1,
        CHAT_TYPE_WORLD = 0,                        // 世界频道
        CHAT_TYPE_TELL,                             // 密聊频道
        CHAT_TYPE_NORMAL,                           // 附近频道
        CHAT_TYPE_TEAM,		                        // 队伍频道
        CHAT_TYPE_GUILD,                            // 帮会频道	
        CHAT_TYPE_MASTER,                           // 师门频道		
        CHAT_TYPE_FRIEND,                           // 好友频道
        CHAT_TYPE_SYSTEM,                           // 系统频道
        CHAT_TYPE_NUM,
    }

    public enum LINK_TYPE
    {
        LINK_TYPE_INVALID1 = -1,
        LINK_TYPE_INVALID = 0,
        LINK_TYPE_ITEM = 1,        // 物品链接
        LINK_TYPE_EQUIP = 2,        // 装备链接
        LINK_TYPE_COPYTEAM = 3,		//副本组队
        LINK_TYPE_GUILD = 7,        // 帮会链接
    }

    // 链接移动方向 注意链接移动方向是与拖动条滚动方向相反的 拖动条向上 链接向下
    public enum EMOTIONLINK_MOVE_DIRECTION
    {
        EMOTIONLINK_MOVE_DOWN = -1,
        EMOTIONLINK_MOVE_UP = 1,
    }

    public UIGrid m_ChannelGrid;
    public List<GameObject> m_Channels = new List<GameObject>();    // 频道列表
    public GameUIInput m_Input;                                         // 输入框input脚本
    public GameChatInput m_GameChatInput;                           // 聊天输入 在NGUI自带的基础上修改
    public GameObject m_ChatInfoLinkRoot;                           // 聊天链接节点
    public GameObject m_EmotionRoot;
    public GameObject m_EmotionGrid;
    public GameObject m_EmotionItemRoot;
    public TweenPosition m_TouchKeyboardTween;
    public GameObject m_OpButtons;
    public GameObject m_ChatSetupRoot;
    public GameObject m_TellChatTitle;
    public UILabel m_TellPlayerNameLabel;
    public GameObject m_FastReplyButton;
    public GameObject m_FriendInformSprite;
    public GameObject m_TellInformSprite;
    public GameObject VoiceTipPanel;
    public ChatInfoSetupLogic m_ChatInfoSetupLogic;
    public LoudSpeakerLogic m_LoudSpeakerLogic;
    public LastSpeakerChatLogic m_LastSpeakerChatLogic;
    public FastReplyLogic m_FastReplyLogic;

    private CHANNEL_TYPE m_eCurChannelType = CHANNEL_TYPE.CHAT_TYPE_WORLD; // 当前频道
    public CHANNEL_TYPE CurChannelType
    {
        get { return m_eCurChannelType; }
        set { m_eCurChannelType = value; }
    }

    private UInt64 m_TellChatReceiverGuid = GlobeVar.INVALID_GUID;      // 密聊对象GUID
    public UInt64 TellChatReceiverGuid
    {
        get { return m_TellChatReceiverGuid; }
    }

    private string m_TellChatReceiverName = "";
    public string TellChatReceiverName
    {
        get { return m_TellChatReceiverName; }
    }

    private UInt64 m_FriendChatReceiverGuid = GlobeVar.INVALID_GUID;      // 好友聊天对象GUID
    public UInt64 FriendChatReceiverGuid
    {
        get { return m_FriendChatReceiverGuid; }
    }

    private string m_FriendChatReceiverName = "";
    public string FriendChatReceiverName
    {
        get { return m_FriendChatReceiverName; }
    }

    private LINK_TYPE m_eChatLinkType = LINK_TYPE.LINK_TYPE_INVALID;            // 本次输入的聊天链接类型
    public LINK_TYPE ChatLinkType
    {
        get { return m_eChatLinkType; }
    }

    private string m_LinkText = "";                                             // 链接文字内容

    private GameItem m_ItemBuffer = new GameItem();                             // 本次聊天链接物品缓存
    public GameItem ItemBuffer
    {
        get { return m_ItemBuffer; }
    }

    private GameItem m_EquipBuffer = new GameItem();                            // 本次聊天链接装备缓存
    public GameItem EquipBuffer
    {
        get { return m_EquipBuffer; }
    }

    private UInt64 m_guildIdBuffer = 0;                                        // 本次聊天帮会Id缓存
    public UInt64 GuildBuffer
    {
        get { return m_guildIdBuffer; }
    }

    private List<GameObject> m_LinkList = new List<GameObject>();               // 聊天栏中所有链接的list
    public List<GameObject> LinkList
    {
        get { return m_LinkList; }
    }

    // 用于标示玩家向上滚动聊天记录时 是否接收到新消息
    private bool m_WaitRefresh = false;
    public bool WaitRefresh
    {
        get { return m_WaitRefresh; }
        set { m_WaitRefresh = value; }
    }

    private UIFont m_Font;
    private float m_ChatTextHeight = 0;

    //static private string[] m_ChannelIconName = { "New_ZongHe_", "New_FuJin_", "New_MiLiao_", "New_DuiWu_", "New_BangHui_", "New_ShiJie_", "New_XiTong_" };
    private bool m_bKeyboardTweenOrigin = true;

    private List<UISprite> deletrans = new List<UISprite>();
    private const int MAX_TEXTNUM = 128;
    private const int EMOTIONITEM_WIDTH = 26;
    private float SPACE_WIDTH = 0;
    //===添加GM面板解锁
    private List<string> strKeyUnlockGM = new List<string>();
    private int tempKeyIndex;

    void Awake()
    {
        SetInstance(this);
        if (m_Font == null)
        {
            m_Font = m_GameChatInput.textList.textLabel.font;
            m_ChatTextHeight = m_Font.CalculatePrintedSize(" ", true, UIFont.SymbolStyle.Uncolored).y;
            SPACE_WIDTH = m_Font.CalculatePrintedSize("　", true, UIFont.SymbolStyle.Uncolored).x;
        }
        InitEmotionButtons();
    }

    // Use this for initialization
    void Start() {

        //===添加GM面板解锁
        strKeyUnlockGM.Add("Channel1-Tell");
        strKeyUnlockGM.Add("Channel6-Friend");
        strKeyUnlockGM.Add("Channel7-System");
        strKeyUnlockGM.Add("Channel2-Normal");

        Init();
    }

    void OnEnable()
    {
        if (m_GameChatInput.textList.maxHeight > 0)
        {
            Init();
        }
    }

    void OnDisable()
    {
        HideKeyboard();
        ClearData();
        SetInstance(null);
        m_ChatInfoSetupLogic.OnCloseChat();
        m_FastReplyLogic.OnCloseChat();
    }

    void Init()
    {
        SetInstance(this);

        m_ChatInfoSetupLogic.Init();
        m_FastReplyLogic.Init();

        m_OpButtons.SetActive(true);
        m_eCurChannelType = GameManager.gameManager.PlayerDataPool.ChooseChannel;
        if (m_eCurChannelType == CHANNEL_TYPE.CHAT_TYPE_TELL)
        {
            m_TellChatReceiverGuid = GameManager.gameManager.PlayerDataPool.LastTellGUID;
            m_TellChatReceiverName = GameManager.gameManager.PlayerDataPool.LastTellName;
        }
        else
        {
            m_TellChatReceiverGuid = GlobeVar.INVALID_GUID;
            m_TellChatReceiverName = "";
            if (m_eCurChannelType == CHANNEL_TYPE.CHAT_TYPE_SYSTEM)
            {
                m_OpButtons.SetActive(false);
                m_Input.enabled = false;
            }
            else
            {
                m_OpButtons.SetActive(true);
                m_Input.enabled = true;
            }
        }
        UpdateChannelState();
        UpdateChannelHistory();
        UpdateChatFrameInformSprite();

        UpdateTeamAndGuildChannel();

        UpdateFriendInformSprite();
        UpdateTellInformSprite();

#if !UNITY_EDITOR && UNITY_IPHONE
        InitTouchKeyboardTween();
#endif
    }

    void ClearData()
    {
        m_GameChatInput.ClearChatHistory();
        m_Input.value = "";
        Utils.CleanGrid(m_ChatInfoLinkRoot);
        Utils.CleanGrid(m_EmotionItemRoot);
        m_EmotionRoot.SetActive(false);
        if (m_TouchKeyboardTween.tweenFactor == 1)
        {
            m_TouchKeyboardTween.Reset();
        }
        m_ChatSetupRoot.SetActive(false);
        UIPlayTween[] playArray = m_FastReplyButton.GetComponents<UIPlayTween>();
        for (int i = 0; i < playArray.Length; i++)
        {
            UITweener tween = playArray[i].tweenTarget.GetComponent<UITweener>();
            if (tween.tweenFactor == 1)
            {
                tween.Reset();
            }
        }

        m_eCurChannelType = CHANNEL_TYPE.CHAT_TYPE_WORLD;
        m_TellChatReceiverGuid = GlobeVar.INVALID_GUID;
        m_TellChatReceiverName = "";
        m_FriendChatReceiverGuid = GlobeVar.INVALID_GUID;
        m_FriendChatReceiverName = "";
        ClearLinkBuffer();
        if (m_LinkList != null)
        {
            m_LinkList.Clear();
        }
        m_bKeyboardTweenOrigin = true;
        m_WaitRefresh = false;
    }

    void OnCloseClick()
    {
        UIManager.CloseUI(UIInfo.ChatInfoRoot);
        if (ChatFrameLogic.Instance() != null)
        {
            ChatFrameLogic.Instance().InitCurChat();
        }
    }

    /// <summary>
    /// 打开聊天窗口时 用playerdata中的数据初始化聊天记录和左侧发言玩家列表
    /// </summary>
    public void InitChatInfo()
    {
        Utils.CleanGrid(m_ChatInfoLinkRoot);
        Utils.CleanGrid(m_EmotionItemRoot);
        m_GameChatInput.ClearChatHistory();
        for (int j = 0; j < GameManager.gameManager.PlayerDataPool.ChatHistory.ChatHistoryList.Count; ++j)
        {
            HandleChatHistory(GameManager.gameManager.PlayerDataPool.ChatHistory.ChatHistoryList[j]);
        }
    }

    /// <summary>
    /// 所有频道gameobject合法验证
    /// </summary>
    /// <returns></returns>
    bool IsAllChannelValid()
    {
        for (int i = 0; i < (int)CHANNEL_TYPE.CHAT_TYPE_NUM; i++)
        {
            if (m_Channels[i] == null)
            {
                return false;
            }
        }
        return true;
    }


    /// <summary>
    /// 切换频道点击响应事件
    /// </summary>
    /// <param name="value">按钮名</param>
    void ChangeChannel(GameObject value)
    {
        if (IsAllChannelValid() && m_Input != null)
        {
            Debug.Log("chose type");
            if (value == m_Channels[(int)m_eCurChannelType])
            {
                Debug.Log("return");
                return;
            }
            //===添加GM面板解锁
            if (strKeyUnlockGM[tempKeyIndex] == value.name)
            {
                tempKeyIndex++;
            } else {
                tempKeyIndex = 0;
            }

            if (tempKeyIndex == 4)
            {
                EventManager.instance.dispatchEvent(new Hashtable(), "ShowGMPanel");
                tempKeyIndex = 0;
            }


            if (value.name == m_Channels[(int)CHANNEL_TYPE.CHAT_TYPE_TELL].name)
            {
                m_OpButtons.SetActive(true);
                m_eCurChannelType = CHANNEL_TYPE.CHAT_TYPE_TELL;
                m_Input.enabled = true;
                m_Input.value = "";
            }
            else if (value.name == m_Channels[(int)CHANNEL_TYPE.CHAT_TYPE_NORMAL].name)
            {
                m_OpButtons.SetActive(true);
                m_eCurChannelType = CHANNEL_TYPE.CHAT_TYPE_NORMAL;
                m_Input.enabled = true;
                m_Input.value = "";
            }
            else if (value.name == m_Channels[(int)CHANNEL_TYPE.CHAT_TYPE_TEAM].name)
            {
                m_OpButtons.SetActive(true);
                m_eCurChannelType = CHANNEL_TYPE.CHAT_TYPE_TEAM;
                m_Input.enabled = true;
                m_Input.value = "";
            }
            else if (value.name == m_Channels[(int)CHANNEL_TYPE.CHAT_TYPE_GUILD].name)
            {
                m_OpButtons.SetActive(true);
                m_eCurChannelType = CHANNEL_TYPE.CHAT_TYPE_GUILD;
                m_Input.enabled = true;
                m_Input.value = "";
            }
            else if (value.name == m_Channels[(int)CHANNEL_TYPE.CHAT_TYPE_MASTER].name)
            {
                m_OpButtons.SetActive(true);
                m_eCurChannelType = CHANNEL_TYPE.CHAT_TYPE_MASTER;
                m_Input.enabled = true;
                m_Input.value = "";
            }
            else if (value.name == m_Channels[(int)CHANNEL_TYPE.CHAT_TYPE_WORLD].name)
            {
                m_OpButtons.SetActive(true);
                m_eCurChannelType = CHANNEL_TYPE.CHAT_TYPE_WORLD;
                m_Input.enabled = true;
                m_Input.value = "";
            }
            else if (value.name == m_Channels[(int)CHANNEL_TYPE.CHAT_TYPE_SYSTEM].name)
            {
                m_OpButtons.SetActive(false);
                m_eCurChannelType = CHANNEL_TYPE.CHAT_TYPE_SYSTEM;
                m_Input.enabled = false;
                m_Input.value = "";
            }
            else if (value.name == m_Channels[(int)CHANNEL_TYPE.CHAT_TYPE_FRIEND].name)
            {
                m_OpButtons.SetActive(true);
                m_eCurChannelType = CHANNEL_TYPE.CHAT_TYPE_FRIEND;
                m_Input.enabled = true;
            }
            GameManager.gameManager.PlayerDataPool.ChooseChannel = m_eCurChannelType;
            if (m_eCurChannelType == CHANNEL_TYPE.CHAT_TYPE_TELL)
            {
                GameManager.gameManager.PlayerDataPool.LastTellGUID = m_TellChatReceiverGuid;
                GameManager.gameManager.PlayerDataPool.LastTellName = m_TellChatReceiverName;
            }
            else
            {
                GameManager.gameManager.PlayerDataPool.LastTellGUID = GlobeVar.INVALID_GUID;
                GameManager.gameManager.PlayerDataPool.LastTellName = "";
            }
            UpdateChannelState();
            UpdateChannelHistory();
            UpdateChatFrameInformSprite();
            if (m_LastSpeakerChatLogic != null)
            {
                m_LastSpeakerChatLogic.UpdateSpeakers();
                if (m_LastSpeakerChatLogic.m_ButtonMenu.activeSelf)
                {
                    m_LastSpeakerChatLogic.m_ButtonMenu.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// 更新频道状态 换aprite
    /// </summary>
    void UpdateChannelState()
    {
        if (IsAllChannelValid())
        {
            for (int i = 0; i < (int)CHANNEL_TYPE.CHAT_TYPE_NUM; i++)
            {
                if (i == (int)m_eCurChannelType)
                {
                    UIImageButton imagebutton = m_Channels[i].GetComponent<UIImageButton>();
                    imagebutton.normalSprite = "ui_pub_077";
                    imagebutton.hoverSprite = "ui_pub_077";
                    imagebutton.pressedSprite = "ui_pub_078";
                    imagebutton.disabledSprite = "ui_pub_077";
                    imagebutton.target.spriteName = "ui_pub_077";
                    imagebutton.target.MakePixelPerfect();
                }
                else
                {
                    UIImageButton imagebutton = m_Channels[i].GetComponent<UIImageButton>();
                    imagebutton.normalSprite = "ui_pub_078";
                    imagebutton.hoverSprite = "ui_pub_078";
                    imagebutton.pressedSprite = "ui_pub_077";
                    imagebutton.disabledSprite = "ui_pub_078";
                    imagebutton.target.spriteName = "ui_pub_078";
                    imagebutton.target.MakePixelPerfect();
                }
            }
        }
    }

    /// <summary>
    /// 获取频道名
    /// </summary>
    /// <param name="nChatType">GC_CHAT.CHATTYPE的频道类型</param>
    /// <returns></returns>
    public string GetChannelName(int nChatType)
    {
        string strChannelName = "";
        if (IsAllChannelValid())
        {
            switch ((GC_CHAT.CHATTYPE)nChatType)
            {
                case GC_CHAT.CHATTYPE.CHAT_TYPE_NORMAL:
                    {
                        //strChannelName = "[附近]";
                        strChannelName = StrDictionary.GetClientDictionaryString("#{2814}");
                    }
                    break;
                case GC_CHAT.CHATTYPE.CHAT_TYPE_TELL:
                    {
                        //strChannelName = "[密聊]";
                        strChannelName = StrDictionary.GetClientDictionaryString("#{2815}");
                    }
                    break;
                case GC_CHAT.CHATTYPE.CHAT_TYPE_TEAM:
                    {
                        //strChannelName = "[队伍]";
                        strChannelName = StrDictionary.GetClientDictionaryString("#{2816}");
                    }
                    break;
                case GC_CHAT.CHATTYPE.CHAT_TYPE_GUILD:
                    {
                        //strChannelName = "[帮会]";
                        strChannelName = StrDictionary.GetClientDictionaryString("#{2817}");
                    }
                    break;
                case GC_CHAT.CHATTYPE.CHAT_TYPE_MASTER:
                    {
                        //strChannelName = "[师门]";
                        strChannelName = StrDictionary.GetClientDictionaryString("#{3364}");
                    }
                    break;
                case GC_CHAT.CHATTYPE.CHAT_TYPE_WORLD:
                    {
                        //strChannelName = "[世界]";
                        strChannelName = StrDictionary.GetClientDictionaryString("#{2818}");
                    }
                    break;
                case GC_CHAT.CHATTYPE.CHAT_TYPE_SYSTEM:
                    {
                        //strChannelName = "[系统]";
                        strChannelName = StrDictionary.GetClientDictionaryString("#{2819}");
                    }
                    break;
                case GC_CHAT.CHATTYPE.CHAT_TYPE_FRIEND:
                    {
                        //strChannelName = "[好友]";
                        strChannelName = StrDictionary.GetClientDictionaryString("#{2820}");
                    }
                    break;
                case GC_CHAT.CHATTYPE.CHAT_TYPE_LOUDSPEAKER:
                    {
                        //strChannelName = "[小喇叭]";
                        strChannelName = StrDictionary.GetClientDictionaryString("#{2821}");
                    }
                    break;
                default:
                    break;
            }
        }
        return strChannelName;
    }

    /// <summary>
    /// 获取当前频道 把CHANNEL_TYPE转化为CG_CHAT.CHATTYPE返回
    /// </summary>
    /// <returns></returns>
    public int GetCurChannelType()
    {
        int nChannel = -1;
        if (IsAllChannelValid())
        {
            switch (m_eCurChannelType)
            {
                case CHANNEL_TYPE.CHAT_TYPE_NORMAL:
                    {
                        nChannel = (int)CG_CHAT.CHATTYPE.CHAT_TYPE_NORMAL;
                    }
                    break;
                case CHANNEL_TYPE.CHAT_TYPE_TELL:
                    {
                        nChannel = (int)CG_CHAT.CHATTYPE.CHAT_TYPE_TELL;
                    }
                    break;
                case CHANNEL_TYPE.CHAT_TYPE_TEAM:
                    {
                        nChannel = (int)CG_CHAT.CHATTYPE.CHAT_TYPE_TEAM;
                    }
                    break;
                case CHANNEL_TYPE.CHAT_TYPE_GUILD:
                    {
                        nChannel = (int)CG_CHAT.CHATTYPE.CHAT_TYPE_GUILD;
                    }
                    break;
                case CHANNEL_TYPE.CHAT_TYPE_MASTER:
                    {
                        nChannel = (int)CG_CHAT.CHATTYPE.CHAT_TYPE_MASTER;
                    }
                    break;
                case CHANNEL_TYPE.CHAT_TYPE_WORLD:
                    {
                        nChannel = (int)CG_CHAT.CHATTYPE.CHAT_TYPE_WORLD;
                    }
                    break;
                case CHANNEL_TYPE.CHAT_TYPE_SYSTEM:
                    {
                        // 正常逻辑不可能进入
                    }
                    break;
                case CHANNEL_TYPE.CHAT_TYPE_FRIEND:
                    {
                        nChannel = (int)CG_CHAT.CHATTYPE.CHAT_TYPE_FRIEND;
                    }
                    break;
                default:
                    break;
            }
        }
        return nChannel;
    }

    /// <summary>
    /// 点击提交
    /// </summary>
    public void OnSubmitClick()
    {
        m_GameChatInput.OnSubmit();
        if (m_EmotionRoot.activeSelf)
        {
            m_EmotionRoot.SetActive(false);
        }
        if (m_ChatSetupRoot.activeSelf)
        {
            m_ChatSetupRoot.SetActive(false);
        }
#if !UNITY_EDITOR && UNITY_IPHONE
        ChatInfoGoBack();
#endif
    }

    /// <summary>
    /// 打开状态下收到聊天信息
    /// </summary>
    /// <param name="pak"></param>
    public void OnReceiveChat()
    {
        // 采用最新一条聊天记录
        int HistoryCount = GameManager.gameManager.PlayerDataPool.ChatHistory.ChatHistoryList.Count;
        ChatHistoryItem LastHistory = GameManager.gameManager.PlayerDataPool.ChatHistory.ChatHistoryList[HistoryCount - 1];
        HandleChatHistory(LastHistory);
        if (LastHistory.EChannel == GC_CHAT.CHATTYPE.CHAT_TYPE_LOUDSPEAKER)
        {
            m_LoudSpeakerLogic.OnReceiveLoudSpeaker();
        }

        UpdateFriendInformSprite();
    }

    /// <summary>
    /// 更新频道聊天记录 在切换频道时用
    /// </summary>
    public void UpdateChannelHistory()
    {
        m_GameChatInput.ClearChatHistory();
        for (int i = 0; i < m_LinkList.Count; ++i)
        {
            if (null != m_LinkList[i])
            {
                Destroy(m_LinkList[i]);
            }
        }
        m_LinkList.Clear();

        UISprite[] spriteArray = m_EmotionItemRoot.GetComponentsInChildren<UISprite>();
        for (int i = 0; i < spriteArray.Length; i++)
        {
            Destroy(spriteArray[i].gameObject);
        }
        deletrans.Clear();

        for (int j = 0; j < GameManager.gameManager.PlayerDataPool.ChatHistory.ChatHistoryList.Count; ++j)
        {
            HandleChatHistory(GameManager.gameManager.PlayerDataPool.ChatHistory.ChatHistoryList[j]);
        }
    }

    /// <summary>
    /// 插入物品链接
    /// </summary>
    /// <param name="item">物品结构</param>
    public void InsertItemLinkText(GameItem item)
    {
        ClearLinkBuffer();
        m_ItemBuffer = item;
        m_eChatLinkType = LINK_TYPE.LINK_TYPE_ITEM;
        m_LinkText = "[" + TableManager.GetCommonItemByID(m_ItemBuffer.DataID, 0).Name + "]";
        if (ShareTargetChooseLogic.AdditionShareMsg != "") {
            string name = TableManager.GetCommonItemByID(m_ItemBuffer.DataID, 0).Name;
            string text = ShareTargetChooseLogic.AdditionShareMsg;
            text = text.Replace(name, m_LinkText);
            m_Input.value = string.Format("{0}", text);
        }
        else
        {
            m_Input.value = string.Format("{0}{1}", m_LinkText, ShareTargetChooseLogic.AdditionShareMsg);
        }
    }

    /// <summary>
    /// 插入装备链接
    /// </summary>
    /// <param name="equip">装备物品结构</param>
    public void InsertEquipLinkText(GameItem equip)
    {
        ClearLinkBuffer();
        m_EquipBuffer = equip;
        m_eChatLinkType = LINK_TYPE.LINK_TYPE_EQUIP;
        m_LinkText = "[" + TableManager.GetCommonItemByID(m_EquipBuffer.DataID, 0).Name + "]";
        if (ShareTargetChooseLogic.AdditionShareMsg != "") {
            string name = TableManager.GetCommonItemByID(m_EquipBuffer.DataID, 0).Name;
            string text = ShareTargetChooseLogic.AdditionShareMsg;
            text = text.Replace(name, m_LinkText);
            m_Input.value = string.Format("{0}", text);
        }
        else
        {
            m_Input.value = string.Format("{0}{1}", m_LinkText, ShareTargetChooseLogic.AdditionShareMsg);
        }
    }

    /// <summary>
    /// 插入帮会链接
    /// </summary>
    /// <param name="guild">帮会Id</param>
    public void InsertGuildLinkText(UInt64 guild)
    {
        ClearLinkBuffer();
        m_eChatLinkType = LINK_TYPE.LINK_TYPE_GUILD;
        m_guildIdBuffer = guild;

        m_LinkText = "[" + StrDictionary.GetClientDictionaryString("#{3294}") + "]";
        m_Input.value = string.Format("{0}{1}", m_LinkText, ShareTargetChooseLogic.AdditionShareMsg);
    }

    /// <summary>
    /// 插入链接标记
    /// </summary>
    /// <param name="text">玩家发言</param>
    /// <returns></returns>
    public string InsertLinkSymbols(string text)
    {
        if (m_eChatLinkType != LINK_TYPE.LINK_TYPE_INVALID && m_eChatLinkType != LINK_TYPE.LINK_TYPE_INVALID1)
        {
            text = text.Replace(m_LinkText, "<a>" + m_LinkText + "</a>");
            if (m_eChatLinkType == LINK_TYPE.LINK_TYPE_GUILD)
            {
                text = StrDictionary.GetClientDictionaryString("#{3111}", GameManager.gameManager.PlayerDataPool.GuildInfo.GuildName) + text;
            }
        }
        return text;
    }

    /// <summary>
    /// 激活链接
    /// </summary>
    /// <param name="fulltext">已和频道 玩家名拼接完毕的完整发言</param>
    /// <param name="history">该条发言所在的聊天历史</param>
    /// <returns></returns>
    public string MakeLinkEnabled(string fulltext, ChatHistoryItem history)
    {
        int linkindex = 0;
        while (Utils.IsContainChatLink(fulltext))
        {
            int linkstart_whole = fulltext.IndexOf("<a>");

            int linkstart = NGUITools.StripSymbols(fulltext).IndexOf("<a>");
            int linkend = NGUITools.StripSymbols(fulltext).IndexOf("</a>") - 3;           // 减3为减去"<a>"三个字符的长度

            string strLinkColor = Utils.GetLinkColor(history, linkindex);
            fulltext = fulltext.Substring(0, fulltext.IndexOf("<a>")) + strLinkColor + fulltext.Substring(fulltext.IndexOf("<a>") + 3);
            string strChannelColor = Utils.GetChannelColor(history);
            fulltext = fulltext.Substring(0, fulltext.IndexOf("</a>")) + strChannelColor + fulltext.Substring(fulltext.IndexOf("</a>") + 4);

            Vector2 LeftSideSpace, RightSideSpace;
            int nNextLineNum;
            CalculateLinkSpace(ref fulltext, linkstart_whole, linkstart, linkend, out LeftSideSpace, out RightSideSpace, out nNextLineNum);
            m_ChatTextHeight = RightSideSpace.y;

            GameObject link = null;
            link = ResourceManager.LoadChatLink(m_ChatInfoLinkRoot);
            if (link != null)
            {
                link.transform.localPosition = new Vector3((LeftSideSpace.x + RightSideSpace.x) / 2, -m_ChatTextHeight / 2, 0);
                link.transform.localPosition -= new Vector3(0, m_ChatTextHeight * nNextLineNum, 0);
                link.transform.localPosition -= new Vector3(0, m_ChatTextHeight * m_GameChatInput.textList.PlayerScroll, 0);

                if (null != link.GetComponent<BoxCollider>())
                    link.GetComponent<BoxCollider>().size = new Vector3(RightSideSpace.x - LeftSideSpace.x, m_ChatTextHeight, 0);

                if (null != link.GetComponent<ChatLinkLogic>())
                    link.GetComponent<ChatLinkLogic>().Init(history, linkindex);

                m_LinkList.Add(link);
            }

            linkindex += 1;
        }

        return fulltext;
    }

    /// <summary>
    /// 计算链接的空格数
    /// </summary>
    /// <param name="text">完整发言</param>
    /// <param name="linkstart">链接起始位置 string索引</param>
    /// <param name="linkend">链接结束位置 string索引</param>
    /// <param name="LeftSideSpace">左侧空格数 返回值</param>
    /// <param name="RightSideSpace">右侧空格数 返回值</param>
    /// <param name="nNextLineNum">换行数</param>
    void CalculateLinkSpace(ref string text, int linkstart_whole, int linkstart, int linkend, out Vector2 LeftSideSpace, out Vector2 RightSideSpace, out int nNextLineNum)
    {
        nNextLineNum = 0;
        LeftSideSpace = Vector2.zero;
        RightSideSpace = Vector2.zero;

        //拆分再合并字符串 防止把加了中括号的玩家名字当颜色代码解析了。。。只解析说话内容中的颜色代码
        //已修改StripSymbols代码 不会把名字当颜色代码解析
        //         int nChatStart = text.IndexOf("说：");
        //         string strPrefix = text.Substring(0, nChatStart);
        //         string strChat = text.Substring(nChatStart);
        //         string striptext = strPrefix + NGUITools.StripSymbols(strChat);
        string striptext = NGUITools.StripSymbols(text);

        LeftSideSpace = m_Font.CalculatePrintedSize(striptext.Substring(0, linkstart), true, UIFont.SymbolStyle.Uncolored);
        RightSideSpace = m_Font.CalculatePrintedSize(striptext.Substring(0, linkend), true, UIFont.SymbolStyle.Uncolored);
        float fLinkLength = RightSideSpace.x - LeftSideSpace.x;

        int nLeftNextLineNum = 0;
        int nRightNextLineNum = 0;

        int nLineStart = 0;
        for (int i = nLineStart + 1; i <= linkstart; i++)
        {
            float fChatWidth = m_Font.CalculatePrintedSize(striptext.Substring(nLineStart, i - nLineStart), true, UIFont.SymbolStyle.Uncolored).x;
            if (fChatWidth > m_GameChatInput.textList.textLabel.GetComponent<UIWidget>().width)
            {
                nLeftNextLineNum += 1;
                LeftSideSpace.x -= m_Font.CalculatePrintedSize(striptext.Substring(nLineStart, i - nLineStart - 1), true, UIFont.SymbolStyle.Uncolored).x;
                nLineStart = i - 1;
            }
        }

        nLineStart = 0;
        for (int i = nLineStart + 1; i <= linkend; i++)
        {
            float fChatWidth = m_Font.CalculatePrintedSize(striptext.Substring(nLineStart, i - nLineStart), true, UIFont.SymbolStyle.Uncolored).x;
            if (fChatWidth > m_GameChatInput.textList.textLabel.GetComponent<UIWidget>().width)
            {
                nRightNextLineNum += 1;
                RightSideSpace.x -= m_Font.CalculatePrintedSize(striptext.Substring(nLineStart, i - nLineStart - 1), true, UIFont.SymbolStyle.Uncolored).x;
                nLineStart = i - 1;
            }
        }

        nNextLineNum = nRightNextLineNum;

        // 是否超出范围 需要让链接另起一行
        if (nLeftNextLineNum < nRightNextLineNum)
        {
            text = text.Substring(0, linkstart_whole) + Environment.NewLine + text.Substring(linkstart_whole);
            LeftSideSpace.x = 0;
            RightSideSpace.x = fLinkLength;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="linkstart"></param>
    /// <param name="LeftSideSpace"></param>
    /// <param name="nNextLineNum"></param>
    void CalculateEmotionSpace(ref string text, out Vector2 LeftSideSpace, out int nNextLineNum)
    {
        nNextLineNum = 0;
        LeftSideSpace = Vector2.zero;

        //拆分再合并字符串 防止把加了中括号的玩家名字当颜色代码解析了。。。只解析说话内容中的颜色代码
        //已修改StripSymbols代码 不会把名字当颜色代码解析
        //         int nChatStart = text.IndexOf("说：");
        //         string strPrefix = text.Substring(0, nChatStart);
        //         string strChat = text.Substring(nChatStart);
        //         string striptext = strPrefix + NGUITools.StripSymbols(strChat);
        string striptext = NGUITools.StripSymbols(text);

        int stripEmotionStart = striptext.IndexOf("[em=");
        LeftSideSpace = m_Font.CalculatePrintedSize(striptext.Substring(0, stripEmotionStart), true, UIFont.SymbolStyle.Uncolored);

        int nLineStart = 0;
        for (int i = nLineStart + 1; i <= stripEmotionStart; i++)
        {
            float fChatWidth = m_Font.CalculatePrintedSize(striptext.Substring(nLineStart, i - nLineStart), true, UIFont.SymbolStyle.Uncolored).x;
            if (i == stripEmotionStart)
            {
                if (fChatWidth > m_GameChatInput.textList.textLabel.GetComponent<UIWidget>().width)
                {
                    nNextLineNum += 1;
                    LeftSideSpace.x -= m_Font.CalculatePrintedSize(striptext.Substring(nLineStart, i - nLineStart - 1), true, UIFont.SymbolStyle.Uncolored).x;
                    nLineStart = i - 1;
                }
                else if (fChatWidth + SPACE_WIDTH > m_GameChatInput.textList.textLabel.GetComponent<UIWidget>().width)
                {
                    nNextLineNum += 1;
                    LeftSideSpace.x = 0;
                }
            }
            else
            {
                if (fChatWidth > m_GameChatInput.textList.textLabel.GetComponent<UIWidget>().width)
                {
                    nNextLineNum += 1;
                    LeftSideSpace.x -= m_Font.CalculatePrintedSize(striptext.Substring(nLineStart, i - nLineStart - 1), true, UIFont.SymbolStyle.Uncolored).x;
                    nLineStart = i - 1;
                }
            }
        }
    }

    /// <summary>
    /// 链接位移 链接每次移动一行
    /// </summary>
    /// <param name="direction">方向</param>
    /// <param name="times">倍数</param>
    public void MoveLinkPos(EMOTIONLINK_MOVE_DIRECTION direction, int times)
    {
        int nHeightMax = m_GameChatInput.textList.textLabel.GetComponent<UIWidget>().height;
        for (int i = 0; i < m_LinkList.Count; ++i)
        {
            if (m_LinkList[i] != null)
            {
                m_LinkList[i].transform.localPosition += new Vector3(0, m_ChatTextHeight * (int)direction * times, 0);
                if (m_LinkList[i].transform.localPosition.y >= nHeightMax || m_LinkList[i].transform.localPosition.y < 0)
                {
                    m_LinkList[i].SetActive(false);
                }
                else
                {
                    m_LinkList[i].SetActive(true);
                }
            }
        }
    }

    public void MoveEmotionPos(EMOTIONLINK_MOVE_DIRECTION direction, int times)
    {
        UISprite[] spriteArray = m_EmotionItemRoot.GetComponentsInChildren<UISprite>();
        for (int i = 0; i < spriteArray.Length; i++)
        {
            spriteArray[i].gameObject.transform.localPosition += new Vector3(0, m_ChatTextHeight * (int)direction * times, 0);
        }
    }

    void ShowOrCloseEmotionRoot()
    {
        m_EmotionRoot.SetActive(!m_EmotionRoot.activeSelf);

        // 如果打开表情 关闭设置
        if (m_EmotionRoot.activeSelf)
        {
            if (m_ChatSetupRoot.activeSelf)
            {
                m_ChatSetupRoot.SetActive(false);
            }
        }
    }

    public void InsertEmotion(GameObject value)
    {
        string name = value.GetComponent<UISprite>().spriteName;
        string id = name.Replace("emotion (", "").Replace(")", "");
        string strEmotion = "[em=" + id + "]";
        if (Utils.GetStrTextNum(m_Input.value + strEmotion) <= MAX_TEXTNUM)
        {
            m_Input.value += strEmotion;
        }
    }

    void InputOnActive()
    {
        if (m_EmotionRoot.activeSelf)
        {
            m_EmotionRoot.SetActive(false);
        }
        if (m_ChatSetupRoot.activeSelf)
        {
            m_ChatSetupRoot.SetActive(false);
        }
#if !UNITY_EDITOR && UNITY_IPHONE
        ChatInfoMove();
#endif
    }

    public void OnPlayVoice(GameObject go)
    {
        /// if (!Regex.IsMatch(go.name, "http://f.aiwaya.cn.*"))
        // {
        //     return;
        //}
        string DownLoadfilePath = string.Format("{0}/{1}.amr", Application.persistentDataPath, DateTime.Now.ToFileTime());
        VoiceManager.Instance.DownLoadFileRequest(DownLoadfilePath, (data)=> 
        {
            if (data.result == 0)
            {
                VoiceManager.Instance.StartPlayRecord(DownLoadfilePath);
            }

        });
        
    }
   
    string MakeEmotionEnabled(string fulltext)
    {
        while (Utils.IsContainEmotion(fulltext))
        {
            Vector2 fLeftSideSpace;
            int nNextLineNum;

            CalculateEmotionSpace(ref fulltext, out fLeftSideSpace, out nNextLineNum);
            m_ChatTextHeight = fLeftSideSpace.y;

            int nEmotionStart = fulltext.IndexOf("[em=");
            int nEmotionEnd = fulltext.Substring(nEmotionStart).IndexOf("]") + nEmotionStart;

            string strEmotion = fulltext.Substring(nEmotionStart, nEmotionEnd - nEmotionStart + 1);
            string strSpriteID = strEmotion.Substring(4).Replace("]", "");

            GameObject emotion = ResourceManager.LoadEmotionItem(m_EmotionItemRoot);
            if (emotion != null)
            {
                
                if (strEmotion.Length > 7)
                {
                    emotion.SetActive(false);
                    GameObject curVoiceBtn = (GameObject)GameObject.Instantiate(VoiceBtn);
                    curVoiceBtn.transform.parent = this.m_EmotionItemRoot.transform;
                    curVoiceBtn.gameObject.SetActive(true);
                    curVoiceBtn.transform.localScale = Vector3.one * 0.7f;
                    curVoiceBtn.transform.localPosition = new Vector3(fLeftSideSpace.x + 10, -m_ChatTextHeight / 2, 0);
                    curVoiceBtn.transform.localPosition -= new Vector3(0, m_ChatTextHeight * nNextLineNum, 0);
                    curVoiceBtn.transform.localPosition -= new Vector3(0, m_ChatTextHeight * m_GameChatInput.textList.PlayerScroll, 0);
                    curVoiceBtn.name = strSpriteID;
                    string spaceEmotionItem = GetSpaceEmotionItem(curVoiceBtn);
                    fulltext = fulltext.Substring(0, nEmotionStart) + spaceEmotionItem + fulltext.Substring(nEmotionEnd + 1);
                }
                else
                {
                    emotion.GetComponent<UISprite>().spriteName = "emotion (" + strSpriteID + ")";
                    emotion.transform.localPosition = new Vector3(fLeftSideSpace.x, -m_ChatTextHeight / 2, 0);
                    emotion.transform.localPosition -= new Vector3(0, m_ChatTextHeight * nNextLineNum, 0);
                    emotion.transform.localPosition -= new Vector3(0, m_ChatTextHeight * m_GameChatInput.textList.PlayerScroll, 0);
                    string spaceEmotionItem = GetSpaceEmotionItem(emotion);
                    fulltext = fulltext.Substring(0, nEmotionStart) + spaceEmotionItem + fulltext.Substring(nEmotionEnd + 1);
                }
            }
        }
        return fulltext;
    }

    void InitTouchKeyboardTween()
    {
        if (null != GameManager.gameManager.ActiveScene.UIRoot &&
            null != GameManager.gameManager.ActiveScene.UIRoot.GetComponent<UIRoot>())
            m_TouchKeyboardTween.to = new Vector3(0, GameManager.gameManager.ActiveScene.UIRoot.GetComponent<UIRoot>().manualHeight / 2 + 110, 0);
    }

    public void ChatInfoMove()
    {
        if (m_bKeyboardTweenOrigin)
        {
            m_TouchKeyboardTween.Play(true);
            m_bKeyboardTweenOrigin = false;
        }
    }

    public void ChatInfoGoBack()
    {
        if (!m_bKeyboardTweenOrigin)
        {
            m_TouchKeyboardTween.Play(false);
            m_bKeyboardTweenOrigin = true;
        }
    }

    public void HideKeyboard()
    {
        m_Input.HideKeyboard();
    }

    void InitEmotionButtons()
    {
        for (int i = 0; i < GlobeVar.EmotionTiger_Num; i++)
        {
            GameObject EmotionButton = ResourceManager.LoadEmotionButton(m_EmotionGrid, i);
            if (EmotionButton != null)
            {
                EmotionButton.GetComponent<UISprite>().spriteName = "emotion (" + (i + 1).ToString() + ")";
            }
        }
        m_EmotionGrid.GetComponent<UIGrid>().Reposition();
    }

    public void BeginChat(UInt64 nReceiverGUID, string strReceiverName)
    {
        if (m_eCurChannelType != CHANNEL_TYPE.CHAT_TYPE_TELL)
        {
            m_eCurChannelType = CHANNEL_TYPE.CHAT_TYPE_TELL;
            GameManager.gameManager.PlayerDataPool.ChooseChannel = m_eCurChannelType;
            UpdateChannelState();
            UpdateChannelHistory();
            UpdateChatFrameInformSprite();
            if (m_LastSpeakerChatLogic != null)
            {
                m_LastSpeakerChatLogic.UpdateSpeakers();
            }
        }
        else
        {
            GameManager.gameManager.PlayerDataPool.ChooseChannel = m_eCurChannelType;
        }

        m_OpButtons.SetActive(true);

        //         if (m_eCurChannelType != CHANNEL_TYPE.CHAT_TYPE_FRIEND)
        //         {
        m_TellChatReceiverGuid = nReceiverGUID;
        m_TellChatReceiverName = strReceiverName;
        GameManager.gameManager.PlayerDataPool.LastTellGUID = nReceiverGUID;
        GameManager.gameManager.PlayerDataPool.LastTellName = strReceiverName;
        //         }
        //         else
        //         {
        //             m_FriendChatReceiverGuid = nReceiverGUID;
        //             m_FriendChatReceiverName = strReceiverName;
        //         }

        m_Input.enabled = true;
        m_Input.value = "/" + strReceiverName + " ";
    }

    public void ClearLinkBuffer()
    {
        m_eChatLinkType = LINK_TYPE.LINK_TYPE_INVALID;
        m_LinkText = "";
        m_ItemBuffer = null;
        m_EquipBuffer = null;
        m_guildIdBuffer = 0;
    }

    // 获取适合表情宽度的空格数
    string GetSpaceEmotionItem(GameObject emotion)
    {
        //         float spaceWidth = m_Font.CalculatePrintedSize(" ", true, UIFont.SymbolStyle.Uncolored).x;
        //         int emotionWidth = emotion.GetComponent<UISprite>().width;
        //         int spaceNum = Mathf.CeilToInt((float)emotionWidth / spaceWidth);
        //         string strResult = "";
        //         for (int i = 0; i < spaceNum; i++ )
        //         {
        //             strResult += " ";
        //         }
        //         return strResult;
        // 暂时用中文全角模式下的空格取代
        return "　";
    }

    void HandleChatHistory(ChatHistoryItem history)
    {
        if (string.IsNullOrEmpty(history.ChatInfo))
        {
            return;
        }

        if (ChatInfoSetupLogic.IsChannelReceiveChat(m_eCurChannelType, history.EChannel))
        {
            if (m_GameChatInput.textList.PlayerScroll > 0)
            {
                m_WaitRefresh = true;
                return;
            }
           
            int VoiceCount = "voice://".Length;
            if (history.ChatInfo.Length >= VoiceCount && history.ChatInfo.Substring(0, VoiceCount) == "voice://")
            {
                history.ChatInfo =  "[em="+ history.ChatInfo.Substring(VoiceCount, history.ChatInfo.Length - VoiceCount) + "]";
            }
           
            string strChannelName = GetChannelName((int)history.EChannel);
            string strSenderName = "";
            string strHisChatInfo = history.ChatInfo;
            if (m_eCurChannelType == CHANNEL_TYPE.CHAT_TYPE_FRIEND && history.EChannel == GC_CHAT.CHATTYPE.CHAT_TYPE_FRIEND)
            {
                // 如果是好友频道 则只显示和该好友之间的对话
                if (history.SenderGuid != m_FriendChatReceiverGuid && history.ReceiverGuid != m_FriendChatReceiverGuid)
                {
                    return;
                }
            }
            if (history.EChannel == GC_CHAT.CHATTYPE.CHAT_TYPE_TELL ||
                history.EChannel == GC_CHAT.CHATTYPE.CHAT_TYPE_FRIEND)
            {
                if (history.SenderName == Singleton<ObjManager>.Instance.MainPlayer.BaseAttr.RoleName)
                {
                    //strSenderName = "你对[" + history.ReceiverName + "]说：";
                    strSenderName = StrDictionary.GetClientDictionaryString("#{2822}", history.ReceiverName);
                }
                else
                {
                    //strSenderName = "[" + history.SenderName + "]对你说：";
                    strSenderName = StrDictionary.GetClientDictionaryString("#{2823}", history.SenderName);
                }
                if (strHisChatInfo[0] == '#')
                {
                    strHisChatInfo = StrDictionary.GetServerDictionaryFormatString(strHisChatInfo);
                }
            }
            else if (history.EChannel == GC_CHAT.CHATTYPE.CHAT_TYPE_SYSTEM
                || history.EChannel == GC_CHAT.CHATTYPE.CHAT_TYPE_GUILD
                || history.EChannel == GC_CHAT.CHATTYPE.CHAT_TYPE_MASTER)
            {
                if (string.IsNullOrEmpty(strHisChatInfo))
                {
                    return;
                }
                // 系统频道可能无发送人
                if (history.SenderGuid != GlobeVar.INVALID_GUID && history.SenderName != "")
                {
                    //strSenderName = "[" + history.SenderName + "]说：";
                    strSenderName = StrDictionary.GetClientDictionaryString("#{2824}", history.SenderName);
                }
                else if (strHisChatInfo[0] == '#' && history.EChannel == GC_CHAT.CHATTYPE.CHAT_TYPE_SYSTEM)
                {
                    strHisChatInfo = StrDictionary.GetServerErrorString(strHisChatInfo);
                }
                else if (strHisChatInfo[0] == '#')
                {
                    strHisChatInfo = StrDictionary.GetServerDictionaryFormatString(strHisChatInfo);
                }
            }
            else
            {
                //strSenderName = "[" + history.SenderName + "]说：";
                if (history.ELinkType.Count > 0 && history.GetLinkIntDataCountByIndex(0) == (int)GC_CHAT.LINKTYPE.LINK_TYPE_GUILDCRUITE)
                {
                    strSenderName = "[" + StrDictionary.GetClientDictionaryString("#{3108}", "") + "]:";
                }
                else
                {
                    strSenderName = StrDictionary.GetClientDictionaryString("#{2824}", history.SenderName);
                }
            }
            string strChatInfo = strHisChatInfo;
            string strChatFull = strChannelName + strSenderName + strChatInfo;

            strChatFull = ShowVIPIcon(strChatFull, history);

            if (history.ELinkType.Count > 0)
            {
                strChatFull = MakeLinkEnabled(strChatFull, history);
            }

            strChatFull = MakeEmotionEnabled(strChatFull);

            if (!(history.ELinkType.Count > 0 && history.GetLinkIntDataCountByIndex(0) == (int)GC_CHAT.LINKTYPE.LINK_TYPE_GUILDCRUITE))
            {
                strChatFull = MakeNameLinkEnabled(strChatFull, history);
            }
           
           string strChannelColor = Utils.GetChannelColor(history);
           m_GameChatInput.ShowNewChat(strChannelColor + strChatFull);
            
        }
    }
    public GameObject VoiceBtn;

    string ShowVIPIcon(string fulltext, ChatHistoryItem history)
    {
        if (history.SenderGuid == GlobeVar.INVALID_GUID || history.SenderName == "")
        {
            return fulltext;
        }

        int nSenderVIPLevel = history.SenderVIPLevel;
        int nReceiverVipLevel = history.ReciverVIPLevel;

        int nShowVipLevel = nSenderVIPLevel;

        int nIconIndex = 0;
        if (history.EChannel == GC_CHAT.CHATTYPE.CHAT_TYPE_TELL ||
            history.EChannel == GC_CHAT.CHATTYPE.CHAT_TYPE_FRIEND)
        {
            if ((history.SenderGuid == Singleton<ObjManager>.Instance.MainPlayer.GUID
                && nReceiverVipLevel <= 0)
               || (history.SenderGuid != Singleton<ObjManager>.Instance.MainPlayer.GUID
                && nSenderVIPLevel <= 0))
            {
                return fulltext;
            }
            else if (history.SenderGuid == Singleton<ObjManager>.Instance.MainPlayer.GUID &&
                     history.ReceiverGuid != GlobeVar.INVALID_GUID && history.ReceiverName != "")
            {
                nIconIndex = fulltext.IndexOf("【" + history.ReceiverName + "】");
                nShowVipLevel = nReceiverVipLevel;
            }
            else
            {
                nIconIndex = fulltext.IndexOf("【" + history.SenderName + "】");
            }
        }
        else if (nSenderVIPLevel <= 0)
        {
            return fulltext;
        }
        else
        {
            nIconIndex = fulltext.IndexOf("【" + history.SenderName + "】");
        }

        if (nIconIndex == GlobeVar.INVALID_ID)
        {
            return fulltext;
        }

        fulltext = fulltext.Substring(0, nIconIndex) + "　" + fulltext.Substring(nIconIndex);
        float fLeftSpace = m_Font.CalculatePrintedSize(fulltext.Substring(0, nIconIndex), true, UIFont.SymbolStyle.Uncolored).x;

        GameObject emotion = ResourceManager.LoadChatVIPIcon(m_EmotionItemRoot);
        if (emotion != null && emotion.GetComponent<UISprite>() != null)
        {
            emotion.GetComponent<UISprite>().spriteName = VipData.GetStarIconByLevel(nShowVipLevel);
            emotion.transform.localPosition = new Vector3(fLeftSpace - 2, -m_ChatTextHeight / 2, 0);
        }
        return fulltext;
    }

    string MakeNameLinkEnabled(string fulltext, ChatHistoryItem history)
    {
        if (history.SenderGuid != GlobeVar.INVALID_GUID && history.SenderName != "")
        {
            GameObject link = null;
            link = ResourceManager.LoadChatLink(m_ChatInfoLinkRoot);
            if (link != null)
            {
                UInt64 linkguid = GlobeVar.INVALID_GUID;
                string linkname = "";
                if ((history.EChannel == GC_CHAT.CHATTYPE.CHAT_TYPE_TELL || history.EChannel == GC_CHAT.CHATTYPE.CHAT_TYPE_FRIEND) &&
                    history.SenderGuid == Singleton<ObjManager>.GetInstance().MainPlayer.GUID)
                {
                    linkguid = history.ReceiverGuid;
                    linkname = history.ReceiverName;
                }
                else
                {
                    linkguid = history.SenderGuid;
                    linkname = history.SenderName;
                }
                string stripText = NGUITools.StripSymbols(fulltext);
                int linkstart = stripText.IndexOf(linkname) - 1;
                int linkend = stripText.IndexOf(linkname) + linkname.Length;
                Vector2 LeftSideSpace = m_Font.CalculatePrintedSize(stripText.Substring(0, linkstart), true, UIFont.SymbolStyle.Uncolored);
                Vector2 RightSideSpace = m_Font.CalculatePrintedSize(stripText.Substring(0, linkend + 1), true, UIFont.SymbolStyle.Uncolored);
                link.transform.localPosition = new Vector3((LeftSideSpace.x + RightSideSpace.x) / 2, -m_ChatTextHeight / 2, 0);
                if (null != link.GetComponent<BoxCollider>())
                    link.GetComponent<BoxCollider>().size = new Vector3(RightSideSpace.x - LeftSideSpace.x, m_ChatTextHeight, 0);

                if (null != link.GetComponent<ChatLinkLogic>())
                    link.GetComponent<ChatLinkLogic>().Init_NameLink(linkguid, linkname);

                fulltext = fulltext.Substring(0, linkstart) + "[00FFFC]" + fulltext.Substring(linkstart, linkname.Length + 2) + Utils.GetChannelColor(history) + fulltext.Substring(linkend + 1);
                m_LinkList.Add(link);
            }
        }
        return fulltext;
    }

    void ShowChatSetup()
    {
        m_ChatSetupRoot.SetActive(!m_ChatSetupRoot.activeSelf);

        // 如果打开设置 关闭表情
        if (m_ChatSetupRoot.activeSelf)
        {
            if (m_EmotionRoot.activeSelf)
            {
                m_EmotionRoot.SetActive(false);
            }
        }
    }

    public void ClearCurInput()
    {
        m_Input.value = "";
        ClearLinkBuffer();
    }

    public void RecoverFastReply()
    {
        UIPlayTween[] playArray = m_FastReplyButton.GetComponents<UIPlayTween>();
        for (int i = 0; i < playArray.Length; i++)
        {
            playArray[i].Play(false);
        }
    }

    public void FastReplyTweenOver()
    {
        UIImageButton imgeButton = m_FastReplyButton.GetComponent<UIImageButton>();
        if (Math.Abs(m_FastReplyButton.transform.rotation.z) == 1)
        {
            imgeButton.normalSprite = "ui_chat_03";
            imgeButton.hoverSprite = "ui_chat_03";
            imgeButton.pressedSprite = "ui_chat_03";
            imgeButton.disabledSprite = "ui_chat_03";
            imgeButton.target.spriteName = "ui_chat_03";
        }
        else
        {
            imgeButton.normalSprite = "ui_chat_03";
            imgeButton.hoverSprite = "ui_chat_03";
            imgeButton.pressedSprite = "ui_chat_03";
            imgeButton.disabledSprite = "ui_chat_03";
            imgeButton.target.spriteName = "ui_chat_03";
        }
        m_FastReplyButton.transform.Rotate(new Vector3(0, 0, 180));
    }

    public void UpdateTeamAndGuildChannel()
    {
        for (int i = 0; i < (int)CHANNEL_TYPE.CHAT_TYPE_NUM; i++)
        {
            if (m_Channels[i].name.Contains("Team"))
            {
                m_Channels[i].SetActive(GameManager.gameManager.PlayerDataPool.IsHaveTeam());
            }
            if (m_Channels[i].name.Contains("Guild"))
            {
                m_Channels[i].SetActive(GameManager.gameManager.PlayerDataPool.IsHaveGuild());
            } if (m_Channels[i].name.Contains("Master"))
            {
                m_Channels[i].SetActive(GameManager.gameManager.PlayerDataPool.IsHaveMaster());
            }
        }

        m_ChannelGrid.Reposition();
    }

    public void UpdateSpeakerList_Team()
    {
        if (m_eCurChannelType == ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_TEAM)
        {
            if (m_LastSpeakerChatLogic != null)
            {
                m_LastSpeakerChatLogic.UpdateSpeakers();
            }
        }
    }

    public void UpdateSpeakerList_Guild()
    {
        if (m_eCurChannelType == ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_GUILD)
        {
            if (m_LastSpeakerChatLogic != null)
            {
                m_LastSpeakerChatLogic.UpdateSpeakers();
            }
        }
    }

    public void UpdateSpeakerList_Master()
    {
        if (m_eCurChannelType == ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_MASTER)
        {
            if (m_LastSpeakerChatLogic != null)
            {
                m_LastSpeakerChatLogic.UpdateSpeakers();
            }
        }
    }

    public void FilterFriendChat(UInt64 guid, string name)
    {
        if (m_FriendChatReceiverGuid == guid)
        {
            return;
        }

        m_FriendChatReceiverGuid = guid;
        m_FriendChatReceiverName = name;

        m_GameChatInput.ClearChatHistory();
        for (int i = 0; i < m_LinkList.Count; ++i)
        {
            if (null != m_LinkList[i])
            {
                Destroy(m_LinkList[i]);
            }
        }
        m_LinkList.Clear();

        UISprite[] spriteArray = m_EmotionItemRoot.GetComponentsInChildren<UISprite>();
        for (int i = 0; i < spriteArray.Length; i++)
        {
            Destroy(spriteArray[i].gameObject);
        }
        deletrans.Clear();

        for (int j = 0; j < GameManager.gameManager.PlayerDataPool.ChatHistory.ChatHistoryList.Count; ++j)
        {
            ChatHistoryItem history = GameManager.gameManager.PlayerDataPool.ChatHistory.ChatHistoryList[j];
            if (history.EChannel == GC_CHAT.CHATTYPE.CHAT_TYPE_FRIEND)
            {
                HandleChatHistory(history);
            }
        }

        m_Input.value = "/" + name + " ";
    }

    void UpdateFriendInformSprite()
    {
        if (GameManager.gameManager.PlayerDataPool.ChatHistory.FriendSendList.Count > 0)
        {
            m_FriendInformSprite.SetActive(true);
        }
    }

    void UpdateTellInformSprite()
    {
        if (GameManager.gameManager.PlayerDataPool.ChatHistory.HasNewTellChat && m_eCurChannelType != CHANNEL_TYPE.CHAT_TYPE_TELL)
        {
            m_TellInformSprite.SetActive(true);
        }
    }

    void UpdateChatFrameInformSprite()
    {
        if (ChatFrameLogic.Instance() == null)
        {
            return;
        }

        if (m_eCurChannelType == CHANNEL_TYPE.CHAT_TYPE_TELL)
        {
            if (GameManager.gameManager.PlayerDataPool.ChatHistory.HasNewTellChat)
            {
                ChatFrameLogic.Instance().m_InformSprite.SetActive(false);
                m_TellInformSprite.SetActive(false);
                GameManager.gameManager.PlayerDataPool.ChatHistory.HasNewTellChat = false;
            }
        }
        if (m_eCurChannelType == CHANNEL_TYPE.CHAT_TYPE_FRIEND)
        {
            if (GameManager.gameManager.PlayerDataPool.ChatHistory.HasNewFriendChat)
            {
                ChatFrameLogic.Instance().m_InformSprite.SetActive(false);
                GameManager.gameManager.PlayerDataPool.ChatHistory.HasNewFriendChat = false;
            }
        }
    }

    void OnBtnTalkPress()
    {
        m_Input.label.text = m_Input.value ;
        if (!VoiceManager.Instance.IsEnable)
        {
            GUIData.AddNotifyData("当前平台不支持语音");
            return;
        }

        if ((this.m_eCurChannelType != CHANNEL_TYPE.CHAT_TYPE_FRIEND && this.m_eCurChannelType != CHANNEL_TYPE.CHAT_TYPE_TELL))
        {
            GUIData.AddNotifyData(StrDictionary.GetClientDictionaryString("#{5090}"));
            return;
        }
        string StrValue = m_Input.value.Trim();
        if (StrValue.Length < 1||StrValue[0]!='/')
        {
            GUIData.AddNotifyData(StrDictionary.GetClientDictionaryString("#{5090}"));
            return;
        } 
        

        this.VoiceTipPanel.SetActive(true);
        VoiceManager.Instance.StartRecordRequest();
    }

    void OnBtnTalkRelese()
    { 
        this.VoiceTipPanel.SetActive(false);
        VoiceManager.Instance.StopRecordRequest((data)=> 
        {
            
            ChatHistoryItem item = new ChatHistoryItem();
            item.CleanUp();
            VoiceManager.Instance.UploadFileRequest(data.strfilepath,(uploadData)=> 
            {
                
                if (ChatInfoLogic.Instance().GetCurChannelType() == (int)CG_CHAT.CHATTYPE.CHAT_TYPE_TELL)
                {
                    item.EChannel = GC_CHAT.CHATTYPE.CHAT_TYPE_TELL;
                }
                else if (ChatInfoLogic.Instance().GetCurChannelType() == (int)CG_CHAT.CHATTYPE.CHAT_TYPE_FRIEND)
                {
                    item.EChannel = GC_CHAT.CHATTYPE.CHAT_TYPE_FRIEND; 
                }
                m_Input.value += "voice://" + uploadData.fileurl;
            });
           
        });
      
    }

    int VoiceAnim_CurIndex = 0;
    float VoiceAnim_addTime = 0;
    GameObject[] RecvLineSprites = new GameObject[3];
    void Update()
    {
        this.UpdateVoiceAnim();
    }
    void UpdateVoiceAnim()
    {
        if(this.VoiceTipPanel.activeSelf == false) return;
        
        if (RecvLineSprites[0] == null)
        {
            for (int i=1;i<4; i++)
            {
                RecvLineSprites[i-1] = this.VoiceTipPanel.transform.Find("recving/"+i.ToString()).gameObject;
            }
        }
        VoiceAnim_addTime += Time.deltaTime;
        if (VoiceAnim_addTime > 0.5f)
        {
            VoiceAnim_addTime = 0;
            VoiceAnim_CurIndex++;
            if (VoiceAnim_CurIndex > 3)
            {
                VoiceAnim_CurIndex = 1;
            }

            for (int i = 0; i <3;i++)
            {
                if (i < VoiceAnim_CurIndex)
                {
                    RecvLineSprites[i].gameObject.SetActive(true);
                }
                else
                {
                    RecvLineSprites[i].gameObject.SetActive(false);
                }
            }
        }
        
    }
}
