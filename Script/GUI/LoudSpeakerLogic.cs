﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using GCGame;
using Games.ChatHistory;
using GCGame.Table;
using Games.GlobeDefine;
using Games.Item;

public class LoudSpeakerLogic : MonoBehaviour {

    private static LoudSpeakerLogic m_Instance = null;
    public static LoudSpeakerLogic Instance()
    {
        return m_Instance;
    }

    // 小喇叭滚动条label状态
    public enum TEXT_STATE
    {
        TEXT_STATE_1 = 1,
        TEXT_STATE_2 = 2,
    }

    public GameObject m_LoudSpeakerWindow;                            // 小喇叭节点
    public GameUIInput m_LoudSpeakerInput;                          // 小喇叭输入
    public UILabel m_NumLabel;
    public GameObject m_EmotionWindow;
    public GameObject m_EmotionWindowGrid;
    public UILabel m_TitleLabel;

    public GameObject m_Text1;
    public UILabel labelChatText1;
    public GameObject m_LinkRoot1;
    public GameObject m_EmotionRoot1;

    public GameObject m_Text2;
    public UILabel labelChatText2;
    public GameObject m_LinkRoot2;
    public GameObject m_EmotionRoot2;

    private UILabel labelChatText;
    private GameObject m_LinkRoot;
    private GameObject m_EmotionRoot;
    private Vector3 m_TextReadyPos = new Vector3();
    private Vector3 m_TextShowPos = new Vector3();

    // 当前小喇叭label状态
    private TEXT_STATE m_eTextStateCur = TEXT_STATE.TEXT_STATE_2;
    private bool m_ChangeText = false;                          // 是否需要滚动切换小喇叭
    private Vector3 m_ChangeTextSpeed = new Vector3(0, 28, 0);       // 小喇叭移动速度

    private UIFont m_Font;
    private float m_ChatTextHeight = 0;
    private int MaxLines = 0;

    private int m_LoudSpeakerNum = 1;
    private const int PerSubmitMax = 10;

    private ChatInfoLogic.LINK_TYPE m_eChatLinkType = ChatInfoLogic.LINK_TYPE.LINK_TYPE_INVALID;
    public ChatInfoLogic.LINK_TYPE ChatLinkType
    {
        get { return m_eChatLinkType; }
    }

    private string m_LinkText = "";                                             // 链接文字内容

    private GameItem m_EquipBuffer = new GameItem();                            // 本次聊天链接装备缓存
    public GameItem EquipBuffer
    {
        get { return m_EquipBuffer; }
    }
    private GameItem m_ItemBuffer = new GameItem();                             // 本次聊天链接物品缓存
    public GameItem ItemBuffer
    {
        get { return m_ItemBuffer; }
    }

    private UInt64 m_guildIdBuffer = 0;                                        // 本次聊天帮会Id缓存
    public UInt64 GuildBuffer
    {
        get { return m_guildIdBuffer; }
    }

    // 清空最后一个小喇叭 最长显示10秒 防止延迟 11秒后清空
    private float m_fClearTimer = 0;
    private bool m_bNeedClear = false;
    const int MaxShowTime = 10;
    private const string TEXT_COLOR = "[FFCC00]";
    private const int MAX_TEXTNUM = 128;
    private const int EMOTIONITEM_WIDTH = 26;
    private float SPACE_WIDTH = 0;

    void Awake()
    {
        m_Instance = this;
        if (m_Font == null)
        {
            m_Font = labelChatText2.font;
            m_ChatTextHeight = m_Font.CalculatePrintedSize(" ", true, UIFont.SymbolStyle.None).y;
            SPACE_WIDTH = m_Font.CalculatePrintedSize("　", true, UIFont.SymbolStyle.Uncolored).x;
            if (m_ChatTextHeight != 0)
            {
                MaxLines = (int)(labelChatText2.height / m_ChatTextHeight);
            }
            else
            {
                MaxLines = 1;
            }
        }
    }
	// Use this for initialization
	void Start () {
        InitTextPos();
        labelChatText = labelChatText2;
        m_LinkRoot = m_LinkRoot2;
        m_EmotionRoot = m_EmotionRoot2;
        InitEmotionButtons();
	}

    void OnEnable()
    {
        m_Instance = this;
        InitCurLoudSpeaker();
		m_fClearTimer -= MaxShowTime + 1;
        UpdateRemainText();
    }
	
	// Update is called once per frame
	void Update () {
        if (m_ChangeText)
        {
            ChangeText();
        }
        if (Time.fixedTime - m_fClearTimer > MaxShowTime + 1 && m_bNeedClear)
        {
            m_bNeedClear = false;
            SelectTextNumber();
        }        
	}

    void OnDisable()
    {
        ClearData();
        m_Instance = null;
    }

    void ClearData()
    {
        m_LoudSpeakerWindow.SetActive(false);
        m_LoudSpeakerInput.value = "";
        m_NumLabel.text = "1";
        m_EmotionWindow.SetActive(false);
        m_TitleLabel.text = "";
        m_Text1.gameObject.transform.localPosition = m_TextReadyPos;
        labelChatText1.text = "";
        Utils.CleanGrid(m_LinkRoot1);
        Utils.CleanGrid(m_EmotionRoot1);
		m_EmotionRoot1.transform.localPosition =  new Vector3(0f,28f,0f);//Vector3.zero;
		m_LinkRoot1.transform.localPosition =  new Vector3(0f,28f,0f);//Vector3.zero;
        m_Text2.gameObject.transform.localPosition = m_TextShowPos;
        labelChatText2.text = "";
        Utils.CleanGrid(m_LinkRoot2);
        Utils.CleanGrid(m_EmotionRoot2);
		m_EmotionRoot2.transform.localPosition =  new Vector3(0f,28f,0f);//Vector3.zero;
		m_LinkRoot2.transform.localPosition =  new Vector3(0f,28f,0f);//Vector3.zero;

        m_eTextStateCur = TEXT_STATE.TEXT_STATE_2;
        m_ChangeText = false;
        m_LoudSpeakerNum = 1;
        ClearLinkBuffer();
    }

    /// <summary>
    /// 打开小喇叭窗口
    /// </summary>
    public void ShowLoudSpeaker()
    {
        m_LoudSpeakerWindow.SetActive(true);
        //Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2218}");
//         if (m_EmotionRoot.activeSelf)
//         {
//             m_EmotionRoot.SetActive(false);
//         }
//         if (m_ChatSetupRoot.activeSelf)
//         {
//             m_ChatSetupRoot.SetActive(false);
//         }
    }

    /// <summary>
    /// 关闭小喇叭窗口
    /// </summary>
    void CloseLoudSpeaker()
    {
        m_EmotionWindow.SetActive(false);
        m_LoudSpeakerWindow.SetActive(false);
    }

    /// <summary>
    /// 发送小喇叭
    /// </summary>
    void LoudSpeakerOnSubmit()
    {


		if(MAX_TEXTNUM - Utils.GetStrTextNum(m_LoudSpeakerInput.value) <= 0)//if(text.Length >= 128)
		{
			Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{6021}");//限制字数128
			return;
		}

        if (GameManager.gameManager.PlayerDataPool.BackPack.GetItemCountByDataId(GlobeVar.QianLiChuanYinDataID) < m_LoudSpeakerNum)
        {
            if (Singleton<ObjManager>.Instance.MainPlayer != null)
            {
                Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{1603}");
                return;
            }
        }

        string str = StrDictionary.GetClientDictionaryString("#{1602}", m_LoudSpeakerNum);
        MessageBoxLogic.OpenOKCancelBox(str, "", LoudSpeakerOnSubmitOK, LoudSpeakerOnSubmitCancel);
    }

    //确认发送
    public void LoudSpeakerOnSubmitOK()
    {
        CG_ASK_LOUDSPEAKER_POOL packet = (CG_ASK_LOUDSPEAKER_POOL)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_LOUDSPEAKER_POOL);
        packet.SpeakerNum = m_LoudSpeakerNum;
        packet.SendPacket();
    }
    //取消发送
    public void LoudSpeakerOnSubmitCancel()
    {

    }

    void InitTextPos()
    {
        m_TextReadyPos = m_Text1.gameObject.transform.localPosition;
        m_TextShowPos = m_Text2.gameObject.transform.localPosition;
    }

    public void OnReceiveLoudSpeaker()
    {
        SelectTextNumber();
        InitCurLoudSpeaker();
    }

    void SelectTextNumber()
    {
        if (m_eTextStateCur == TEXT_STATE.TEXT_STATE_1)
        {
            m_eTextStateCur = TEXT_STATE.TEXT_STATE_2;

            labelChatText = labelChatText2;
            m_EmotionRoot = m_EmotionRoot2;
            m_LinkRoot = m_LinkRoot2;

            if (m_Text2.gameObject.transform.localPosition == m_TextReadyPos)
            {
                m_ChangeText = true;
            }
			m_EmotionRoot2.transform.localPosition =  new Vector3(0f,28f,0f);//Vector3.zero;
			m_LinkRoot2.transform.localPosition =  new Vector3(0f,28f,0f);//Vector3.zero;
            return;
        }
        else if (m_eTextStateCur == TEXT_STATE.TEXT_STATE_2)
        {
            m_eTextStateCur = TEXT_STATE.TEXT_STATE_1;

            labelChatText = labelChatText1;
            m_EmotionRoot = m_EmotionRoot1;
            m_LinkRoot = m_LinkRoot1;

            if (m_Text1.gameObject.transform.localPosition == m_TextReadyPos)
            {
                m_ChangeText = true;
            }
			m_EmotionRoot1.transform.localPosition = new Vector3(0f,28f,0f);//Vector3.zero;
			m_LinkRoot1.transform.localPosition =  new Vector3(0f,28f,0f);//Vector3.zero;
            return;
        }
    }


    void InitCurLoudSpeaker()
    {
        int HistoryCount = GameManager.gameManager.PlayerDataPool.ChatHistory.ChatHistoryList.Count;
        if (HistoryCount == 0) 
		{
			return;
		}

		// 找是否有小喇叭
	    int nLastLoudSpeakerIndex = -1;
	    List<ChatHistoryItem> listChatHistory = GameManager.gameManager.PlayerDataPool.ChatHistory.ChatHistoryList;
	    for (int i = listChatHistory.Count - 1; i >= 0; i-- )
	    {
	        if (listChatHistory[i].EChannel == GC_CHAT.CHATTYPE.CHAT_TYPE_LOUDSPEAKER &&
	            !string.IsNullOrEmpty(listChatHistory[i].ChatInfo))
	        {
	            nLastLoudSpeakerIndex = i;
	            break;
	        }
	    }

		// 没有小喇叭
	    if (nLastLoudSpeakerIndex == -1) 
		{
			return;
		}

        // 显示聊天信息
        ChatHistoryItem LastLoudSpeaker = listChatHistory[nLastLoudSpeakerIndex];
        string strSenderName = "";
        if (LastLoudSpeaker.ELinkType.Count > 0 && LastLoudSpeaker.GetLinkIntDataCountByIndex(0) == (int)GC_CHAT.LINKTYPE.LINK_TYPE_GUILDCRUITE)
        {
            strSenderName = "[" + StrDictionary.GetClientDictionaryString("#{3108}", "")+ "]:";
        }
        else
        {
            strSenderName = StrDictionary.GetClientDictionaryString("#{2824}", LastLoudSpeaker.SenderName);
        }
        
        string strChatInfo = LastLoudSpeaker.ChatInfo;
        string strChatFull = strSenderName + strChatInfo;

        strChatFull = ShowVIPIcon(strChatFull, LastLoudSpeaker);

        if (LastLoudSpeaker.ELinkType.Count > 0)
        {
            strChatFull = MakeLinkEnabled(strChatFull, LastLoudSpeaker);
        }

        strChatFull = MakeEmotionEnabled(strChatFull);

        if (!(LastLoudSpeaker.ELinkType.Count > 0 && LastLoudSpeaker.GetLinkIntDataCountByIndex(0) == (int)GC_CHAT.LINKTYPE.LINK_TYPE_GUILDCRUITE))
        {
            strChatFull = MakeNameLinkEnabled(strChatFull, LastLoudSpeaker);
        }

        ConfirmEllipsis(ref strChatFull);
        strChatFull = TEXT_COLOR + strChatFull;
        labelChatText.text = strChatFull;

        UpdateEmotionLinkPos(strChatFull);

        m_fClearTimer = Time.fixedTime;
        m_bNeedClear = true;          
    }

    string MakeNameLinkEnabled(string fulltext, ChatHistoryItem history)
    {
        if (history.SenderGuid != GlobeVar.INVALID_GUID && history.SenderName != "")
        {
            GameObject link = null;
            link = ResourceManager.LoadChatLink(m_LinkRoot);
            if (link != null && m_Font != null)
            {
                UInt64 linkguid = history.SenderGuid;
                string linkname = history.SenderName;                
                string stripText = NGUITools.StripSymbols(fulltext);
                int linkstart = stripText.IndexOf(linkname) - 1;
                int linkend = stripText.IndexOf(linkname) + linkname.Length;
                Vector2 LeftSideSpace = m_Font.CalculatePrintedSize(stripText.Substring(0, linkstart), true, UIFont.SymbolStyle.Uncolored);
                Vector2 RightSideSpace = m_Font.CalculatePrintedSize(stripText.Substring(0, linkend + 1), true, UIFont.SymbolStyle.Uncolored);
                link.transform.localPosition = new Vector3((LeftSideSpace.x + RightSideSpace.x) / 2, m_ChatTextHeight / 2, 0);
                link.GetComponent<BoxCollider>().size = new Vector3(RightSideSpace.x - LeftSideSpace.x, m_ChatTextHeight, 0);
                link.GetComponent<ChatLinkLogic>().Init_NameLink(linkguid, linkname);
                fulltext = fulltext.Substring(0, linkstart) + "[00FFFC]" + fulltext.Substring(linkstart, linkname.Length + 2) + TEXT_COLOR + fulltext.Substring(linkend + 1);
            }
        }
        return fulltext;
    }

    string ShowVIPIcon(string fulltext, ChatHistoryItem history)
    {
        if (history.SenderGuid == GlobeVar.INVALID_GUID || history.SenderName == "" || null == m_Font)
        {
            return fulltext;
        }

        int nSenderVIPLevel = history.SenderVIPLevel;
        if (nSenderVIPLevel <= 0)
        {
            return fulltext;
        }

        int nSenderNameIndex = fulltext.IndexOf("【" + history.SenderName + "】");
        fulltext = fulltext.Substring(0, nSenderNameIndex) + "　" + fulltext.Substring(nSenderNameIndex);
        float fLeftSpace = m_Font.CalculatePrintedSize(fulltext.Substring(0, nSenderNameIndex), true, UIFont.SymbolStyle.Uncolored).x;

        GameObject emotion = ResourceManager.LoadChatVIPIcon(m_EmotionRoot);
        if (emotion != null)
        {
            emotion.GetComponent<UISprite>().spriteName = VipData.GetStarIconByLevel(nSenderVIPLevel);
            emotion.transform.localPosition = new Vector3(fLeftSpace - 2, m_ChatTextHeight / 2, 0);
        }
        return fulltext;
    }

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
            fulltext = fulltext.Substring(0, fulltext.IndexOf("</a>")) + TEXT_COLOR + fulltext.Substring(fulltext.IndexOf("</a>") + 4);

            Vector2 LeftSideSpace, RightSideSpace;
            int nNextLineNum;
            bool bNeedShow;
            CalculateLinkSpace(ref fulltext, linkstart_whole, linkstart, linkend, out LeftSideSpace, out RightSideSpace, out nNextLineNum, out bNeedShow);
            m_ChatTextHeight = RightSideSpace.y;

            if (bNeedShow)
            {
                GameObject link = null;
                link = ResourceManager.LoadChatLink(m_LinkRoot);
                if (link != null)
                {
                    link.transform.localPosition = new Vector3((LeftSideSpace.x + RightSideSpace.x) / 2, m_ChatTextHeight / 2, 0);
                    link.transform.localPosition -= new Vector3(0, m_ChatTextHeight * nNextLineNum, 0);
                    link.GetComponent<BoxCollider>().size = new Vector3(RightSideSpace.x - LeftSideSpace.x, m_ChatTextHeight, 0);
                    link.GetComponent<ChatLinkLogic>().Init(history, linkindex);
                }
            }
            else
            {
                fulltext = fulltext.Replace("<a>", "");
                fulltext = fulltext.Replace("</a>", "");
            }

            linkindex += 1;
        }

        return fulltext;
    }

    void CalculateLinkSpace(ref string text, int linkstart_whole, int linkstart, int linkend, out Vector2 LeftSideSpace, out Vector2 RightSideSpace, out int nNextLineNum, out bool needShow)
    {
        nNextLineNum = 0;
        LeftSideSpace = Vector2.zero;
        RightSideSpace = Vector2.zero;
        needShow = true;

        if (null == m_Font)
            return;

        //拆分再合并字符串 防止把加了中括号的玩家名字当颜色代码解析了。。。只解析说话内容中的颜色代码
        //已修改StripSymbols代码 不会把名字当颜色代码解析
//         int nChatStart = text.IndexOf("说：");
//         string strPrefix = text.Substring(0, nChatStart);
//         string strChat = text.Substring(nChatStart);
//         string striptext = strPrefix + NGUITools.StripSymbols(strChat);
        string striptext = NGUITools.StripSymbols(text);

        LeftSideSpace = m_Font.CalculatePrintedSize(striptext.Substring(0, linkstart), true, UIFont.SymbolStyle.None);
        RightSideSpace = m_Font.CalculatePrintedSize(striptext.Substring(0, linkend), true, UIFont.SymbolStyle.None);
        float fLinkLength = RightSideSpace.x - LeftSideSpace.x;
        float fEllipsisWidth = m_Font.CalculatePrintedSize("...", true, UIFont.SymbolStyle.None).x;

        int nLeftNextLineNum = 0;
        int nRightNextLineNum = 0;
        if (LeftSideSpace.x > labelChatText.GetComponent<UIWidget>().width * MaxLines - fEllipsisWidth ||
            RightSideSpace.x > labelChatText.GetComponent<UIWidget>().width * MaxLines - fEllipsisWidth)
        {
            needShow = false;
            return;
        }

        int nLineStart = 0;
        for (int i = nLineStart + 1; i <= linkstart; i++)
        {
            float fChatWidth = m_Font.CalculatePrintedSize(striptext.Substring(nLineStart, i - nLineStart), true, UIFont.SymbolStyle.None).x;
            if (fChatWidth > labelChatText.width)
            {
                nLeftNextLineNum += 1;
                LeftSideSpace.x -= m_Font.CalculatePrintedSize(striptext.Substring(nLineStart, i - nLineStart - 1), true, UIFont.SymbolStyle.None).x;
                nLineStart = i - 1;
            }
        }

        nLineStart = 0;
        for (int i = nLineStart + 1; i <= linkend; i++)
        {
            float fChatWidth = m_Font.CalculatePrintedSize(striptext.Substring(nLineStart, i - nLineStart), true, UIFont.SymbolStyle.None).x;
            if (fChatWidth > labelChatText.width)
            {
                nRightNextLineNum += 1;
                RightSideSpace.x -= m_Font.CalculatePrintedSize(striptext.Substring(nLineStart, i - nLineStart - 1), true, UIFont.SymbolStyle.None).x;
                nLineStart = i - 1;
            }
        }

        nNextLineNum = nRightNextLineNum;

        // 是否超出范围 需要让链接另起一行
        if (nLeftNextLineNum < nRightNextLineNum)
        {
            text = text.Substring(0, linkstart_whole) + Environment.NewLine + text.Substring(linkstart_whole);
            //RightSideSpace.x = RightSideSpace.x - LeftSideSpace.x;
            LeftSideSpace.x = 0;
            RightSideSpace.x = fLinkLength;
        }
    }

    string MakeEmotionEnabled(string fulltext)
    {
        while (Utils.IsContainEmotion(fulltext))
        {
            Vector2 fLeftSideSpace;
            int nNextLineNum;
            bool bNeedShow;

            CalculateEmotionSpace(ref fulltext, out fLeftSideSpace, out nNextLineNum, out bNeedShow);
            m_ChatTextHeight = fLeftSideSpace.y;

            int nEmotionStart = fulltext.IndexOf("[em=");
            int nEmotionEnd = fulltext.Substring(nEmotionStart).IndexOf("]") + nEmotionStart;

            string strEmotion = fulltext.Substring(nEmotionStart, nEmotionEnd - nEmotionStart + 1);
            string strSpriteID = strEmotion.Substring(4).Replace("]", "");

            if (bNeedShow && nNextLineNum < MaxLines)
            {
                GameObject emotion = ResourceManager.LoadEmotionItem(m_EmotionRoot);
                if (emotion != null)
                {
                    emotion.GetComponent<UISprite>().spriteName = "emotion (" + strSpriteID + ")";

                    emotion.transform.localPosition = new Vector3(fLeftSideSpace.x, m_ChatTextHeight / 2, 0);
                    emotion.transform.localPosition -= new Vector3(0, m_ChatTextHeight * nNextLineNum, 0);

                    string spaceEmotionItem = GetSpaceEmotionItem(emotion);
                    fulltext = fulltext.Substring(0, nEmotionStart) + spaceEmotionItem + fulltext.Substring(nEmotionEnd + 1);
                }
            }
            else
            {
                fulltext = fulltext.Substring(0, nEmotionStart) + fulltext.Substring(nEmotionEnd + 1);
                break;
            }
        }
        return fulltext;
    }

    void CalculateEmotionSpace(ref string text, out Vector2 LeftSideSpace, out int nNextLineNum, out bool needShow)
    {
        nNextLineNum = 0;
        LeftSideSpace = Vector2.zero;
        needShow = true;

        //拆分再合并字符串 防止把加了中括号的玩家名字当颜色代码解析了。。。只解析说话内容中的颜色代码
        //已修改StripSymbols代码 不会把名字当颜色代码解析
        //         int nChatStart = text.IndexOf("说：");
        //         string strPrefix = text.Substring(0, nChatStart);
        //         string strChat = text.Substring(nChatStart);
        //         string striptext = strPrefix + NGUITools.StripSymbols(strChat);
        string striptext = NGUITools.StripSymbols(text);

        int stripEmotionStart = striptext.IndexOf("[em=");
        LeftSideSpace = m_Font.CalculatePrintedSize(striptext.Substring(0, stripEmotionStart), true, UIFont.SymbolStyle.Uncolored);
        float fEllipsisWidth = m_Font.CalculatePrintedSize("...", true, UIFont.SymbolStyle.None).x;
        if (LeftSideSpace.x + EMOTIONITEM_WIDTH > labelChatText.GetComponent<UIWidget>().width * MaxLines - fEllipsisWidth)
        {
            needShow = false;
            return;
        }

        int nLineStart = 0;
        for (int i = nLineStart + 1; i <= stripEmotionStart; i++)
        {
            float fChatWidth = m_Font.CalculatePrintedSize(striptext.Substring(nLineStart, i - nLineStart), true, UIFont.SymbolStyle.None).x;
            if (i == stripEmotionStart)
            {
                if (fChatWidth > labelChatText.width)
                {
                    nNextLineNum += 1;
                    LeftSideSpace.x -= m_Font.CalculatePrintedSize(striptext.Substring(nLineStart, i - nLineStart - 1), true, UIFont.SymbolStyle.Uncolored).x;
                    nLineStart = i - 1;
                }
                else if (fChatWidth + SPACE_WIDTH > labelChatText.width)
                {
                    nNextLineNum += 1;
                    LeftSideSpace.x = 0;
                }
            }
            else
            {
                if (fChatWidth > labelChatText.width)
                {
                    nNextLineNum += 1;
                    LeftSideSpace.x -= m_Font.CalculatePrintedSize(striptext.Substring(nLineStart, i - nLineStart - 1), true, UIFont.SymbolStyle.Uncolored).x;
                    nLineStart = i - 1;
                }
            }
        }
    }

    string GetSpaceEmotionItem(GameObject emotion)
    {
//         float spaceWidth = m_Font.CalculatePrintedSize(" ", true, UIFont.SymbolStyle.Uncolored).x;
//         int emotionWidth = emotion.GetComponent<UISprite>().width;
//         int spaceNum = Mathf.CeilToInt((float)emotionWidth / spaceWidth);
//         string strResult = "";
//         for (int i = 0; i < spaceNum; i++)
//         {
//             strResult += " ";
//         }
//         return strResult;
        return "　";
    }

    void ConfirmEllipsis(ref string strChatFull)
    {
        if (null == m_Font)
            return;

        float fChatWidth = m_Font.CalculatePrintedSize(strChatFull, true, UIFont.SymbolStyle.None).x;
        if (fChatWidth > labelChatText.width * MaxLines)
        {
            string strCurChat = strChatFull;
            float fCurChatWidth;
            do
            {
                strCurChat = strCurChat.Substring(0, strCurChat.Length / 2);
                fCurChatWidth = m_Font.CalculatePrintedSize(strCurChat, true, UIFont.SymbolStyle.None).x;
            } while (fCurChatWidth > labelChatText.width * MaxLines);

            for (int i = strCurChat.Length; i < strChatFull.Length; i++)
            {
                if (m_Font.CalculatePrintedSize(strChatFull.Substring(0, i), true, UIFont.SymbolStyle.None).x >= labelChatText.width * MaxLines)
                {
                    strChatFull = strChatFull.Substring(0, i - 2) + "...";
                    break;
                }
            }
        }
    }

    void UpdateEmotionLinkPos(string strChatFull)
    {
        if (null == m_Font)
            return;

        float fChatWidth = m_Font.CalculatePrintedSize(strChatFull.Replace(Environment.NewLine, ""), true, UIFont.SymbolStyle.None).x;
        int moveHeight = Mathf.CeilToInt(fChatWidth / labelChatText.width) - 1;
		moveHeight = (moveHeight > 1) ? 1 : moveHeight;
		m_EmotionRoot.transform.localPosition = new Vector3(0f, 28f, 0f);//new Vector3(0, moveHeight * m_ChatTextHeight, 0);
		m_LinkRoot.transform.localPosition = new Vector3(0f, 28f, 0f);//new Vector3(0, moveHeight * m_ChatTextHeight, 0);
    }

    void AddLoudSpeakerNum()
    {
        if (m_LoudSpeakerNum + 1 > PerSubmitMax)
        {
            return;
        }
        m_LoudSpeakerNum += 1;
        m_NumLabel.text = m_LoudSpeakerNum.ToString();
    }

    void SubtractLoudSpeakerNum()
    {
        if (m_LoudSpeakerNum - 1 <= 0)
        {
            return;
        }
        m_LoudSpeakerNum -= 1;
        m_NumLabel.text = m_LoudSpeakerNum.ToString();
    }

    public void SendLoudSpeakerInfo()
    {
        string text = m_LoudSpeakerInput.value;
        if (!string.IsNullOrEmpty(text))
        {
            text = Utils.StrFilter_Chat(text);
            Utils.SendLoudSpeaker(text, m_LoudSpeakerNum);
            m_LoudSpeakerInput.value = "";
            m_LoudSpeakerWindow.SetActive(false);
        }
        else
        {
            if (Singleton<ObjManager>.Instance.MainPlayer != null)
            {
                Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, StrDictionary.GetClientDictionaryString("#{1601}"));
            }            
        }
    }

    void ChangeText()
    {
        m_Text1.gameObject.transform.localPosition += Time.deltaTime * m_ChangeTextSpeed;
        m_Text2.gameObject.transform.localPosition += Time.deltaTime * m_ChangeTextSpeed;

        if (m_eTextStateCur == TEXT_STATE.TEXT_STATE_1)
        {
            if (m_Text1.gameObject.transform.localPosition.y >= m_TextShowPos.y)
            {
                m_Text1.gameObject.transform.localPosition = m_TextShowPos;
                m_Text2.gameObject.transform.localPosition = m_TextReadyPos;

                Utils.CleanGrid(m_LinkRoot2);
                Utils.CleanGrid(m_EmotionRoot2);

                labelChatText2.text = "";
                m_ChangeText = false;
            }
        }

        if (m_eTextStateCur == TEXT_STATE.TEXT_STATE_2)
        {
            if (m_Text2.gameObject.transform.localPosition.y >= m_TextShowPos.y)
            {
                m_Text2.gameObject.transform.localPosition = m_TextShowPos;
                m_Text1.gameObject.transform.localPosition = m_TextReadyPos;

                Utils.CleanGrid(m_LinkRoot1);
                Utils.CleanGrid(m_EmotionRoot1);

                labelChatText1.text = "";
                m_ChangeText = false;
            }
        }
    }

    void ShowOrCloseEmotionRoot()
    {
        m_EmotionWindow.SetActive(!m_EmotionWindow.activeSelf);
    }

    void InitEmotionButtons()
    {
        for (int i = 0; i < GlobeVar.EmotionTiger_Num; i++)
        {
            GameObject EmotionButton = ResourceManager.LoadEmotionButton(m_EmotionWindowGrid, i);
            if (EmotionButton != null)
            {
                EmotionButton.GetComponent<UISprite>().spriteName = "emotion (" + (i + 1).ToString() + ")";
            }
        }
        m_EmotionWindowGrid.GetComponent<UIGrid>().Reposition();
    }

    public bool IsLoudSpeakerWindowShow()
    {
        return m_LoudSpeakerWindow.activeSelf;
    }

    public void InsertEmotion(GameObject value)
    {
        string name = value.GetComponent<UISprite>().spriteName;
        string id = name.Replace("emotion (", "").Replace(")", "");
        string strEmotion = "[em=" + id + "]";
        if (Utils.GetStrTextNum(m_LoudSpeakerInput.value + strEmotion) <= MAX_TEXTNUM)
        {
            m_LoudSpeakerInput.value += strEmotion;
            UpdateRemainText();
        }       
    }

    public string InsertLinkSymbols(string text)
    {
        if (m_eChatLinkType != ChatInfoLogic.LINK_TYPE.LINK_TYPE_INVALID && m_eChatLinkType != ChatInfoLogic.LINK_TYPE.LINK_TYPE_INVALID1)
        {
            text = text.Replace(m_LinkText, "<a>" + m_LinkText + "</a>");
			if(m_eChatLinkType == ChatInfoLogic.LINK_TYPE.LINK_TYPE_GUILD)
			{
				text = StrDictionary.GetClientDictionaryString("#{3111}", GameManager.gameManager.PlayerDataPool.GuildInfo.GuildName) + text;
			}
        }
        return text;
    }

    public void InsertEquipLinkText(GameItem equip)
    {
        ClearLinkBuffer();
        m_EquipBuffer = equip;
        m_eChatLinkType = ChatInfoLogic.LINK_TYPE.LINK_TYPE_EQUIP;
        m_LinkText = "[" + TableManager.GetCommonItemByID(m_EquipBuffer.DataID, 0).Name + "]";
		if (ShareTargetChooseLogic.AdditionShareMsg != "") {
			string name = TableManager.GetCommonItemByID (m_EquipBuffer.DataID, 0).Name;
			string text = ShareTargetChooseLogic.AdditionShareMsg;
			text = text.Replace (name, m_LinkText);
			m_LoudSpeakerInput.value = string.Format ("{0}", text);
		} 
		else 
		{
			m_LoudSpeakerInput.value = string.Format("{0}{1}", m_LinkText, ShareTargetChooseLogic.AdditionShareMsg);
		}
    }

    public void InsertItemLinkText(GameItem item)
    {
        ClearLinkBuffer();
        m_ItemBuffer = item;
        m_eChatLinkType = ChatInfoLogic.LINK_TYPE.LINK_TYPE_ITEM;
        m_LinkText = "[" + TableManager.GetCommonItemByID(m_ItemBuffer.DataID, 0).Name + "]";
    //    m_LoudSpeakerInput.value = string.Format("{0}{1}", m_LinkText, ShareTargetChooseLogic.AdditionShareMsg); 
		if (ShareTargetChooseLogic.AdditionShareMsg != "") {
			string name = TableManager.GetCommonItemByID (m_ItemBuffer.DataID, 0).Name;
			string text = ShareTargetChooseLogic.AdditionShareMsg;
			text = text.Replace (name, m_LinkText);
			m_LoudSpeakerInput.value = string.Format ("{0}", text);
		} 
		else 
		{
			m_LoudSpeakerInput.value = string.Format("{0}{1}", m_LinkText, ShareTargetChooseLogic.AdditionShareMsg);
		}
    }

    public void InsertGuildLinkText(UInt64 guild)
    { 
        ClearLinkBuffer();
        m_eChatLinkType = ChatInfoLogic.LINK_TYPE.LINK_TYPE_GUILD;
        m_guildIdBuffer = guild;

		m_LinkText = "[" + StrDictionary.GetClientDictionaryString("#{3294}")+ "]"; 
        m_LoudSpeakerInput.value = string.Format("{0}{1}", m_LinkText, ShareTargetChooseLogic.AdditionShareMsg);
    }

    public void ClearLinkBuffer()
    {
        m_eChatLinkType = ChatInfoLogic.LINK_TYPE.LINK_TYPE_INVALID;
        m_LinkText = "";
        m_ItemBuffer = null;
        m_EquipBuffer = null;
        m_guildIdBuffer = 0;
    }

    public bool UpdateRemainText()
    {
        int nTextNum = Utils.GetStrTextNum(m_LoudSpeakerInput.value);
        if (nTextNum > MAX_TEXTNUM)
        {
            m_TitleLabel.text = StrDictionary.GetClientDictionaryString("#{2720}", 0);
            return false;
        }
        else
        {
            m_TitleLabel.text = StrDictionary.GetClientDictionaryString("#{2720}", MAX_TEXTNUM - Utils.GetStrTextNum(m_LoudSpeakerInput.value));
            return true;
        }        
    }
}
