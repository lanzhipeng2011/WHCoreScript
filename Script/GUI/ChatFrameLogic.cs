using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Games.ChatHistory;
using GCGame.Table;
using Games.GlobeDefine;
using GCGame;
using Module.Log;
public class ChatFrameLogic : MonoBehaviour {

    private enum TEXT_STATE
    {
        TEXT_STATE_1 = 1,
        TEXT_STATE_2 = 2,
    }
    private TEXT_STATE m_eTextStateCur = TEXT_STATE.TEXT_STATE_2;

    public GameObject m_FirstChild;
    public Transform m_OffsetTrans;

    public GameObject m_Text1;
    public UILabel labelChatText1;
    public GameObject m_LinkRoot1;
    public GameObject m_EmotionRoot1;

    public GameObject m_Text2;
    public UILabel labelChatText2;
    public GameObject m_LinkRoot2;
    public GameObject m_EmotionRoot2;

    public GameObject m_InformSprite;

    //public List<TweenAlpha> m_FoldTween;

    private UILabel labelChatText;
    private GameObject m_LinkRoot;
    private GameObject m_EmotionRoot;

    private UIFont m_Font;
    private float m_ChatTextHeight = 0;

    private Vector3 m_TextReadyPos = new Vector3(0, -56, 0);
    private Vector3 m_TextShowPos = new Vector3(0, 0, 0);
    //private bool m_ChangeText = false;
    private Vector3 m_ChangeTextSpeed = new Vector3(0, 28, 0);

    private int MaxLines = 0;

//     public UIPanel ClipPanel;
//     private Transform m_ClipPanelTransform = null;
    private float m_updateCallTime = 0.5f;

    private Vector3 m_LocalPos;
    private List<GameObject> clearItemList = new List<GameObject>();
    private const int EMOTIONITEM_WIDTH = 32;
    private float SPACE_WIDTH = 0;

    private static ChatFrameLogic m_Instance = null;
    public static ChatFrameLogic Instance()
    {
        return m_Instance;
    }

    void Awake()
    {
#if UNITY_IPHONE || UNITY_WP8 || UNITY_BLACKBERRY   //|| UNITY_ANDROID
        TouchScreenKeyboard.hideInput = true;
#endif
        m_Instance = this;
    }

	// Use this for initialization
	void Start () {        
        m_FirstChild.SetActive(true);
        //InvokeRepeating("CheckNotifyData", 1, 1);
        m_Font = labelChatText2.font;
        m_ChatTextHeight = m_Font.CalculatePrintedSize(" ", true, UIFont.SymbolStyle.None).y;
        SPACE_WIDTH = m_Font.CalculatePrintedSize("　", true, UIFont.SymbolStyle.Uncolored).x;
        if (m_ChatTextHeight != 0)
        {
            MaxLines = (int)(labelChatText2.height / m_ChatTextHeight);
        }
        else
        {
            MaxLines = 2;
        }        
        InitTextPos();

        labelChatText = labelChatText2;
        m_LinkRoot = m_LinkRoot2;
        m_EmotionRoot = m_EmotionRoot2;

//         if (null != ClipPanel)
//         {
//             m_ClipPanelTransform = ClipPanel.transform;
//         }

        m_LocalPos = m_OffsetTrans.localPosition;
        clearItemList.Add(m_LinkRoot1);
        clearItemList.Add(m_EmotionRoot1);
        clearItemList.Add(m_LinkRoot2);
        clearItemList.Add(m_EmotionRoot2);
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (m_updateCallTime > 0)
        {
            m_updateCallTime -= Time.deltaTime;
            return;
        }

        m_updateCallTime = 0.5f;

        // 解决剪裁区域不更新的问题
//         if (null != m_ClipPanelTransform)
//         {
//             m_ClipPanelTransform.localPosition = m_ClipPanelTransform.localPosition;
//         }
//         else
//         {
//             m_ClipPanelTransform = ClipPanel.transform;
//         }

        //if (m_ChangeText)
        //{
            // 暂时取消滚动切换 防止接收聊天消息过快导致显示问题 所以调用不到此函数
            //ChangeText();
        //}
	}

    void OnDestroy()
    {
#if UNITY_IPHONE || UNITY_WP8 || UNITY_BLACKBERRY //|| UNITY_ANDROID
        TouchScreenKeyboard.hideInput = false;
#endif
        m_Instance = null;
    }

    private string curData;
    void CheckNotifyData()
    {
//         curData = GUIData.GetNotifyData();
//         if (null != curData)
//         {
//             labelChatText.text = curData;
//         }
    }

    void ShowChatInfo()
    {
		NewPlayerGuidLogic.CloseWindow ();
        UIManager.ShowUI(UIInfo.ChatInfoRoot);
    }

    public void OnReceiveChat(GC_CHAT pak)
    {
        if (pak.Chattype == (int)GC_CHAT.CHATTYPE.CHAT_TYPE_LOUDSPEAKER)
        {
            if (null != LoudSpeakerFrameLogic.Instance())
                LoudSpeakerFrameLogic.Instance().OnReceiveLoudSpeaker();
        }
        else
        {
            InitCurChat();
        }        
    }

    public string GetChannelName(int nChatType)
    {
        string strChannelName = "";
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
        return strChannelName;
    }

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
			if((history.SenderGuid == Singleton<ObjManager>.Instance.MainPlayer.GUID 
			   && nReceiverVipLevel <= 0)
			   || (history.SenderGuid != Singleton<ObjManager>.Instance.MainPlayer.GUID
			    && nSenderVIPLevel <=0))
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

        GameObject emotion = ResourceManager.LoadChatVIPIcon(m_EmotionRoot);
        if (emotion != null && emotion.GetComponent<UISprite>() != null)
        {
			emotion.GetComponent<UISprite>().spriteName = VipData.GetStarIconByLevel(nShowVipLevel);
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
            string strChannelColor = Utils.GetChannelColor(history);
            fulltext = fulltext.Substring(0, fulltext.IndexOf("</a>")) + strChannelColor + fulltext.Substring(fulltext.IndexOf("</a>") + 4);

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
                    if (null != link.GetComponent<BoxCollider>())
                        link.GetComponent<BoxCollider>().size = new Vector3(RightSideSpace.x - LeftSideSpace.x, m_ChatTextHeight, 0);

                    if (null != link.GetComponent<ChatLinkLogic>())
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
                if (emotion != null && emotion.GetComponent<UISprite>() != null)
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
        LeftSideSpace = m_Font.CalculatePrintedSize(striptext.Substring(0, stripEmotionStart), true, UIFont.SymbolStyle.None);
        
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
//         float spaceWidth = m_Font.CalculatePrintedSize(" ", true, UIFont.SymbolStyle.None).x;
//         int emotionWidth = emotion.GetComponent<UISprite>().width;
//         int spaceNum = Mathf.CeilToInt((float)emotionWidth / spaceWidth);
//          string strResult = "";
//         for (int i = 0; i < spaceNum; i++)
//         {
//             strResult += "　";
//         }
//         return strResult;
        return "　";
    }

    void InitTextPos()
    {
        m_TextReadyPos = m_Text1.gameObject.transform.localPosition;
        m_TextShowPos = m_Text2.gameObject.transform.localPosition;
    }

    void SelectTextNumber()
    {
        labelChatText = labelChatText2;
        m_EmotionRoot = m_EmotionRoot2;
        m_LinkRoot = m_LinkRoot2;
        m_EmotionRoot.transform.localPosition = Vector3.zero;
        m_LinkRoot.transform.localPosition = Vector3.zero;
        ClearCurChat();

        /*if (m_eTextStateCur == TEXT_STATE.TEXT_STATE_1)
        {
            m_eTextStateCur = TEXT_STATE.TEXT_STATE_2;

            labelChatText = labelChatText2;
            m_EmotionRoot = m_EmotionRoot2;
            m_LinkRoot = m_LinkRoot2;

            if (m_Text2.gameObject.transform.localPosition == m_TextReadyPos)
            {
                //m_ChangeText = true;

                m_Text2.gameObject.transform.localPosition = m_TextShowPos;

                m_Text1.gameObject.transform.localPosition = m_TextReadyPos;

                ClearCurChat();
            }
            m_EmotionRoot2.transform.localPosition = Vector3.zero;
            m_LinkRoot2.transform.localPosition = Vector3.zero;
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
                //m_ChangeText = true;

                m_Text1.gameObject.transform.localPosition = m_TextShowPos;

                m_Text2.gameObject.transform.localPosition = m_TextReadyPos;

                ClearCurChat();
            }
            m_EmotionRoot1.transform.localPosition = Vector3.zero;
            m_LinkRoot1.transform.localPosition = Vector3.zero;
            return;
        }*/
    }

    // 不再滚动切换
    /*void ChangeText()
    {
        m_Text1.gameObject.transform.localPosition += Time.deltaTime * m_ChangeTextSpeed;
        m_Text2.gameObject.transform.localPosition += Time.deltaTime * m_ChangeTextSpeed;

        if (m_eTextStateCur == TEXT_STATE.TEXT_STATE_1)
        {
            if (m_Text1.gameObject.transform.localPosition.y >= m_TextShowPos.y)
            {
                m_Text1.gameObject.transform.localPosition = m_TextShowPos;
                m_Text2.gameObject.transform.localPosition = m_TextReadyPos;

                Transform[] linkTransArray = m_LinkRoot2.GetComponentsInChildren<Transform>();
                for (int i = 0; i < linkTransArray.Length; i++)
                {
                    if (linkTransArray[i].gameObject != m_LinkRoot2)
                    {
                        Destroy(linkTransArray[i].gameObject);
                    }
                }

                Transform[] emotionTransArray = m_EmotionRoot2.GetComponentsInChildren<Transform>();
                for (int i = 0; i < emotionTransArray.Length; i++)
                {
                    if (emotionTransArray[i].gameObject != m_LinkRoot2)
                    {
                        Destroy(emotionTransArray[i].gameObject);
                    }
                }

                //m_ChangeText = false;
            }
        }

        if (m_eTextStateCur == TEXT_STATE.TEXT_STATE_2)
        {
            if (m_Text2.gameObject.transform.localPosition.y >= m_TextShowPos.y)
            {
                m_Text2.gameObject.transform.localPosition = m_TextShowPos;
                m_Text1.gameObject.transform.localPosition = m_TextReadyPos;

                Transform[] linkTransArray = m_LinkRoot1.GetComponentsInChildren<Transform>();
                for (int i = 0; i < linkTransArray.Length; i++)
                {
                    if (linkTransArray[i].gameObject != m_LinkRoot2)
                    {
                        Destroy(linkTransArray[i].gameObject);
                    }
                }

                Transform[] emotionTransArray = m_EmotionRoot1.GetComponentsInChildren<Transform>();
                for (int i = 0; i < emotionTransArray.Length; i++)
                {
                    if (emotionTransArray[i].gameObject != m_LinkRoot2)
                    {
                        Destroy(emotionTransArray[i].gameObject);
                    }
                }

                //m_ChangeText = false;
            }
        }
    }*/

    void UpdateEmotionLinkPos(string strChatFull)
    {
        float fChatWidth = m_Font.CalculatePrintedSize(strChatFull.Replace(Environment.NewLine, ""), true, UIFont.SymbolStyle.None).x;
        int moveHeight = Mathf.CeilToInt(fChatWidth / labelChatText.width) - 1;
		moveHeight = (moveHeight > 1 ? 1 : moveHeight);
        m_EmotionRoot.transform.localPosition += new Vector3(0, moveHeight * m_ChatTextHeight, 0);
        m_LinkRoot.transform.localPosition += new Vector3(0, moveHeight * m_ChatTextHeight, 0);
    }

    void ConfirmEllipsis(ref string strChatFull)
    {
        float fChatWidth = m_Font.CalculatePrintedSize(strChatFull, true, UIFont.SymbolStyle.None).x;
        float fEllipsisWidth = m_Font.CalculatePrintedSize("...", true, UIFont.SymbolStyle.None).x;
        if (fChatWidth > labelChatText.width * MaxLines - fEllipsisWidth)
        {
            string strCurChat = strChatFull;
            float fCurChatWidth;
            do 
            {
                strCurChat = strCurChat.Substring(0, strCurChat.Length/2);
                fCurChatWidth = m_Font.CalculatePrintedSize(strCurChat, true, UIFont.SymbolStyle.None).x;
            } while (fCurChatWidth > labelChatText.width * MaxLines - fEllipsisWidth);

            for (int i = strCurChat.Length; i < strChatFull.Length; i++ )
            {
                if (m_Font.CalculatePrintedSize(strChatFull.Substring(0, i), true, UIFont.SymbolStyle.None).x >= labelChatText.width * MaxLines - fEllipsisWidth)
                {
                    strChatFull = strChatFull.Substring(0, i - 2) + "...";
                    break;
                }
            }
        }
    }

    public void InitCurChat()
    {
        int HistoryCount = GameManager.gameManager.PlayerDataPool.ChatHistory.ChatHistoryList.Count;
        if (HistoryCount > 0)
        {
            int nLastLoudSpeakerIndex = -1;
            List<ChatHistoryItem> listChatHistory = GameManager.gameManager.PlayerDataPool.ChatHistory.ChatHistoryList;
            for (int i = listChatHistory.Count - 1; i >= 0; i--)
            {
                if (listChatHistory[i].EChannel != GC_CHAT.CHATTYPE.CHAT_TYPE_LOUDSPEAKER &&
                    !string.IsNullOrEmpty(listChatHistory[i].ChatInfo) &&
                    ChatInfoSetupLogic.IsChannelReceiveChat(GameManager.gameManager.PlayerDataPool.ChooseChannel, listChatHistory[i].EChannel))
                {
                    nLastLoudSpeakerIndex = i;
                    break;
                }
            }

            if (nLastLoudSpeakerIndex != -1)
            {
                // 显示聊天信息
                ChatHistoryItem LastHistory = listChatHistory[nLastLoudSpeakerIndex];

                string strChannelName = GetChannelName((int)LastHistory.EChannel);
                string strSenderName = "";
                string strLastChatInfo = LastHistory.ChatInfo;
                if (LastHistory.EChannel == GC_CHAT.CHATTYPE.CHAT_TYPE_TELL ||
                    LastHistory.EChannel == GC_CHAT.CHATTYPE.CHAT_TYPE_FRIEND)
                {
                    if (LastHistory.SenderName == Singleton<ObjManager>.Instance.MainPlayer.BaseAttr.RoleName)
                    {
                        //strSenderName = "你对[" + LastHistory.ReceiverName + "]说：";
                        strSenderName = StrDictionary.GetClientDictionaryString("#{2822}", LastHistory.ReceiverName);
                    }
                    else
                    {
                        //strSenderName = "[" + LastHistory.SenderName + "]对你说：";
                        strSenderName = StrDictionary.GetClientDictionaryString("#{2823}", LastHistory.SenderName);
                    }
                    if (strLastChatInfo[0] == '#')
                    {
						strLastChatInfo = StrDictionary.GetServerDictionaryFormatString(strLastChatInfo);
                    }
                }
                else if (LastHistory.EChannel == GC_CHAT.CHATTYPE.CHAT_TYPE_SYSTEM
                    || LastHistory.EChannel == GC_CHAT.CHATTYPE.CHAT_TYPE_GUILD)
                {
                    if (string.IsNullOrEmpty(strLastChatInfo))
                    {
                        return;
                    }
                    // 系统频道可能无发送人
                    if (LastHistory.SenderGuid != GlobeVar.INVALID_GUID && LastHistory.SenderName != "")
                    {
                        //strSenderName = "[" + LastHistory.SenderName + "]说：";
                        strSenderName = StrDictionary.GetClientDictionaryString("#{2824}", LastHistory.SenderName);
                    }
					else if (strLastChatInfo[0] == '#' && LastHistory.EChannel == GC_CHAT.CHATTYPE.CHAT_TYPE_SYSTEM)
                    {
						strLastChatInfo = StrDictionary.GetServerErrorString(strLastChatInfo);
                    }
					else if (strLastChatInfo[0] == '#')
					{
						strLastChatInfo = StrDictionary.GetServerDictionaryFormatString(strLastChatInfo);
					}
                }
                else
                {
                    //strSenderName = "[" + LastHistory.SenderName + "]说：";
                    if (LastHistory.ELinkType.Count > 0 && LastHistory.GetLinkIntDataCountByIndex(0) == (int)GC_CHAT.LINKTYPE.LINK_TYPE_GUILDCRUITE)
                    {
                        strSenderName = "[" + StrDictionary.GetClientDictionaryString("#{3108}", "") + "]:";
                    }
                    else
                    {
                        strSenderName = StrDictionary.GetClientDictionaryString("#{2824}", LastHistory.SenderName);
                    }
                }

                SelectTextNumber();

                string strChatInfo = strLastChatInfo;
                string strChatFull = strChannelName + strSenderName + strChatInfo;

                strChatFull = ShowVIPIcon(strChatFull, LastHistory);

                if (LastHistory.ELinkType.Count > 0)
                {
                    strChatFull = MakeLinkEnabled(strChatFull, LastHistory);
                }

                strChatFull = MakeEmotionEnabled(strChatFull);

                if (!(LastHistory.ELinkType.Count > 0 && LastHistory.GetLinkIntDataCountByIndex(0) == (int)GC_CHAT.LINKTYPE.LINK_TYPE_GUILDCRUITE))
                {
                    strChatFull = MakeNameLinkEnabled(strChatFull, LastHistory);
                }

                string strChannelColor = Utils.GetChannelColor(LastHistory);
                strChatFull = strChannelColor + strChatFull;

                ConfirmEllipsis(ref strChatFull);
                labelChatText.text = strChatFull;

                UpdateEmotionLinkPos(strChatFull);
            }
        }        
    }

    string MakeNameLinkEnabled(string fulltext, ChatHistoryItem history)
    {
        if (history.SenderGuid != GlobeVar.INVALID_GUID && history.SenderName != "")
        {
            GameObject link = null;
            link = ResourceManager.LoadChatLink(m_LinkRoot);
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
                link.transform.localPosition = new Vector3((LeftSideSpace.x + RightSideSpace.x) / 2, m_ChatTextHeight / 2, 0);
                if (null != link.GetComponent<BoxCollider>())
                    link.GetComponent<BoxCollider>().size = new Vector3(RightSideSpace.x - LeftSideSpace.x, m_ChatTextHeight, 0);

                if (null != link.GetComponent<ChatLinkLogic>())
                    link.GetComponent<ChatLinkLogic>().Init_NameLink(linkguid, linkname);

                fulltext = fulltext.Substring(0, linkstart) + "[00FFFC]" + fulltext.Substring(linkstart, linkname.Length + 2) + Utils.GetChannelColor(history) + fulltext.Substring(linkend + 1);
            }
        }
        return fulltext;
    }

    public void PlayTween(bool nDirection)
    {
        //foreach (TweenAlpha tween in m_FoldTween)
        //{
        //    tween.Play(nDirection);
        //}
        //foreach (BoxCollider box in gameObject.GetComponentsInChildren<BoxCollider>())
        //{
        //    box.enabled = !nDirection;
        //}

        //gameObject.SetActive(!nDirection);
        if (nDirection)
        {
            m_OffsetTrans.localPosition = GlobeVar.INFINITY_FAR;
        }
        else
        {
            m_OffsetTrans.localPosition = m_LocalPos;
        }
    }

    void ClearCurChat()
    {
        labelChatText1.text = "";
        labelChatText2.text = "";
        
        for (int i = 0; i < clearItemList.Count; i++)
        {
            Utils.CleanGrid(clearItemList[i]);
        }

        m_LinkRoot1.transform.DetachChildren();
        m_EmotionRoot1.transform.DetachChildren();
        m_LinkRoot2.transform.DetachChildren();
        m_EmotionRoot2.transform.DetachChildren();
    }
}
