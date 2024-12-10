using UnityEngine;
using System.Collections;
using Games.ChatHistory;
using GCGame;
using GCGame.Table;
using System;
using Games.GlobeDefine;

public class ChatBubbleLogic : MonoBehaviour {

    public UILabel m_ChatText;
    public GameObject m_LinkRoot;
    public GameObject m_EmotionRoot;
    public UISprite m_Background;

    private UIFont m_Font = null;
    private float m_ChatTextHeight = GlobeVar.INVALID_ID;
    private Vector3 EmotionRootPos;
    private float SPACE_WIDTH = 0;

    void Awake()
    {
        m_Font = m_ChatText.font;
        if (m_Font != null)
        {
            m_ChatTextHeight = m_Font.CalculatePrintedSize(" ", true, UIFont.SymbolStyle.None).y;
            SPACE_WIDTH = m_Font.CalculatePrintedSize("　", true, UIFont.SymbolStyle.Uncolored).x;
        }        
        EmotionRootPos = m_EmotionRoot.transform.localPosition;
    }

    public void Show(ChatHistoryItem history)
    {        
        if (gameObject.activeSelf)
        {
            CancelInvoke("HideBubbleWait");
            ClearBubble();
        }

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        string strChatFull = history.ChatInfo;
        if (history.ELinkType.Count > 0)
        {
            strChatFull = MakeLinkEnabled(strChatFull, history);
        }

        strChatFull = MakeEmotionEnabled(strChatFull);

        m_ChatText.text = strChatFull;

        UpdateEmotionLinkPos(strChatFull);

        //m_Background.GetComponent<UIWidget>().width = (int)m_ChatText.printedSize.x + 28;
        m_Background.GetComponent<UIWidget>().height = (int)m_ChatText.printedSize.y + 6;

        Invoke("HideBubbleWait", 5);
    }

    void HideBubbleWait()
    {
        ClearBubble();
        gameObject.SetActive(false);
    }

    void ClearBubble()
    {
        m_ChatText.text = "";

        Utils.CleanGrid(m_LinkRoot);
        m_LinkRoot.transform.localPosition = Vector3.zero;

        Utils.CleanGrid(m_EmotionRoot);
        m_EmotionRoot.transform.localPosition = EmotionRootPos;
    }

    public string MakeLinkEnabled(string fulltext, ChatHistoryItem history)
    {
        int linkindex = 0;
        while(Utils.IsContainChatLink(fulltext))
        {
            int linkstart_whole = fulltext.IndexOf("<a>");

            int linkstart = NGUITools.StripSymbols(fulltext).IndexOf("<a>");
            int linkend = NGUITools.StripSymbols(fulltext).IndexOf("</a>") - 3;           // 减3为减去"<a>"三个字符的长度

            string strLinkColor = Utils.GetLinkColor(history, linkindex);
            fulltext = fulltext.Substring(0, fulltext.IndexOf("<a>")) + strLinkColor + fulltext.Substring(fulltext.IndexOf("<a>") + 3);
            fulltext = fulltext.Substring(0, fulltext.IndexOf("</a>")) + "[-]" + fulltext.Substring(fulltext.IndexOf("</a>") + 4);

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

        if (null == m_Font)
        {
            m_Font = m_ChatText.font;
            if (m_Font != null)
            {
                m_ChatTextHeight = m_Font.CalculatePrintedSize(" ", true, UIFont.SymbolStyle.None).y;
                SPACE_WIDTH = m_Font.CalculatePrintedSize("　", true, UIFont.SymbolStyle.Uncolored).x;
            } 
        }

        string striptext = NGUITools.StripSymbols(text);

        LeftSideSpace = m_Font.CalculatePrintedSize(striptext.Substring(0, linkstart), true, UIFont.SymbolStyle.None);
        RightSideSpace = m_Font.CalculatePrintedSize(striptext.Substring(0, linkend), true, UIFont.SymbolStyle.None);
        float fLinkLength = RightSideSpace.x - LeftSideSpace.x;
        float fEllipsisWidth = m_Font.CalculatePrintedSize("...", true, UIFont.SymbolStyle.None).x;

        int nLeftNextLineNum = 0;
        int nRightNextLineNum = 0;

        int nLineStart = 0;
        for (int i = nLineStart + 1; i <= linkstart; i++)
        {
            float fChatWidth = m_Font.CalculatePrintedSize(striptext.Substring(nLineStart, i - nLineStart), true, UIFont.SymbolStyle.None).x;
            if (fChatWidth > m_ChatText.width)
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
            if (fChatWidth > m_ChatText.width)
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

            if (bNeedShow)
            {
                GameObject emotion = ResourceManager.LoadEmotionItem(m_EmotionRoot);
                if (emotion != null)
                {
                    if (null != emotion.GetComponent<UISprite>())
                        emotion.GetComponent<UISprite>().spriteName = "emotion (" + strSpriteID + ")";

                    emotion.transform.localPosition = new Vector3(fLeftSideSpace.x, m_ChatTextHeight / 2, 0);
                    emotion.transform.localPosition -= new Vector3(0, m_ChatTextHeight * nNextLineNum, 0);
                    emotion.transform.localRotation = Quaternion.identity;

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

        if (null == m_Font)
        {
            m_Font = m_ChatText.font;
            if (m_Font != null)
            {
                m_ChatTextHeight = m_Font.CalculatePrintedSize(" ", true, UIFont.SymbolStyle.None).y;
                SPACE_WIDTH = m_Font.CalculatePrintedSize("　", true, UIFont.SymbolStyle.Uncolored).x;
            }
        }

        string striptext = NGUITools.StripSymbols(text);

        int stripEmotionStart = striptext.IndexOf("[em=");
        LeftSideSpace = m_Font.CalculatePrintedSize(striptext.Substring(0, stripEmotionStart), true, UIFont.SymbolStyle.None);
        float fEllipsisWidth = m_Font.CalculatePrintedSize("...", true, UIFont.SymbolStyle.None).x;
//         if (LeftSideSpace.x + EMOTIONITEM_WIDTH > labelChatText.GetComponent<UIWidget>().width * MaxLines - fEllipsisWidth)
//         {
//             needShow = false;
//             return;
//         }

        int nLineStart = 0;
        for (int i = nLineStart + 1; i <= stripEmotionStart; i++)
        {
            float fChatWidth = m_Font.CalculatePrintedSize(striptext.Substring(nLineStart, i - nLineStart), true, UIFont.SymbolStyle.None).x;
            if (i == stripEmotionStart)
            {
                if (fChatWidth > m_ChatText.width)
                {
                    nNextLineNum += 1;
                    LeftSideSpace.x -= m_Font.CalculatePrintedSize(striptext.Substring(nLineStart, i - nLineStart - 1), true, UIFont.SymbolStyle.Uncolored).x;
                    nLineStart = i - 1;
                }
                else if (fChatWidth + SPACE_WIDTH > m_ChatText.width)
                {
                    nNextLineNum += 1;
                    LeftSideSpace.x = 0;
                }
            }
            else
            {
                if (fChatWidth > m_ChatText.width)
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
        return "　";
    }

    void UpdateEmotionLinkPos(string strChatFull)
    {
        if (null == m_Font)
        {
            m_Font = m_ChatText.font;
            if (m_Font != null)
            {
                m_ChatTextHeight = m_Font.CalculatePrintedSize(" ", true, UIFont.SymbolStyle.None).y;
                SPACE_WIDTH = m_Font.CalculatePrintedSize("　", true, UIFont.SymbolStyle.Uncolored).x;
            }
        }

        float fChatWidth = m_Font.CalculatePrintedSize(strChatFull.Replace(Environment.NewLine, ""), true, UIFont.SymbolStyle.None).x;
        int moveHeight = Mathf.CeilToInt(fChatWidth / m_ChatText.width) - 1;

        m_EmotionRoot.transform.localPosition += new Vector3(0, moveHeight * m_ChatTextHeight, 0);
        m_LinkRoot.transform.localPosition += new Vector3(0, moveHeight * m_ChatTextHeight, 0);
    }
}
