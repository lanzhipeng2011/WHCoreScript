//********************************************************************
// 文件名: GameTextList.cs
// 描述: 文本list 在NGUI自带的TextList基础上修改
// 作者: WangZhe
//********************************************************************

using UnityEngine;
using System.Collections.Generic;
using System.Text;
using Module.Log;
public class GameTextList : MonoBehaviour
{
    public enum Style
    {
        Text,
        Chat,
    }

    public Style style = Style.Text;
    public UILabel textLabel;
    public float maxHeight = 0f;
    public int maxEntries = 50;
    public bool supportScrollWheel = true;

    // Text list is made up of paragraphs
    protected class Paragraph
    {
        public string text;		// Original text
        public string[] lines;	// Split lines
    }

    protected char[] mSeparator = new char[] { '\n' };
    protected List<Paragraph> mParagraphs = new List<Paragraph>();
    private float mScroll = 0f;
    public float PlayerScroll
    {
        get { return Mathf.RoundToInt(mScroll); }
    }
    protected bool mSelected = false;
    protected int mTotalLines = 0;

    public UIScrollBar m_ChatScrollBar;
    private float m_FakeScrollBarValue = 0;

    private int m_offsetbuffer = 0;

    /// <summary>
    /// Clear the text.
    /// </summary>

    public void Clear()
    {
        mParagraphs.Clear();
        if (m_ChatScrollBar.gameObject != null)
        {
            m_ChatScrollBar.gameObject.SetActive(false);
        }
        mTotalLines = 0;
        UpdateVisibleText();
    }

    /// <summary>
    /// Add a new paragraph.
    /// </summary>

    public void Add(string text, out int linesCount) { Add(text, true, out linesCount); }

    /// <summary>
    /// Add a new paragraph.
    /// </summary>

    protected void Add(string text, bool updateVisible, out int linesCount)
    {
        linesCount = 1;
        Paragraph ce = null;

        if (mParagraphs.Count < maxEntries)
        {
            ce = new Paragraph();
        }
        else
        {
            ce = mParagraphs[0];
            mParagraphs.RemoveAt(0);
        }

        ce.text = text;
        mParagraphs.Add(ce);

        if (textLabel != null && textLabel.font != null)
        {
            // Rebuild the line
            string line;
            textLabel.font.WrapText(ce.text, out line, textLabel.width, 100000,
                0, textLabel.supportEncoding, textLabel.symbolStyle);
            ce.lines = line.Split(mSeparator);
            linesCount = ce.lines.Length;

            // Recalculate the total number of lines
            mTotalLines = 0;
            for (int i = 0, imax = mParagraphs.Count; i < imax; ++i)
                mTotalLines += mParagraphs[i].lines.Length;
        }

        // Update the visible text
        if (updateVisible) UpdateVisibleText();
    }

    /// <summary>
    /// Automatically find the values if none were specified.
    /// </summary>

    void Awake()
    {
        if (textLabel == null) textLabel = GetComponentInChildren<UILabel>();

        Collider col = collider;

        if (col != null)
        {
            // Automatically set the width and height based on the collider
            if (maxHeight <= 0f) maxHeight = col.bounds.size.y / transform.lossyScale.y;
        }

        m_ChatScrollBar.gameObject.SetActive(false);
    }

    /// <summary>
    /// Remember whether the widget is selected.
    /// </summary>

    void OnSelect(bool selected) { mSelected = selected; }

    /// <summary>
    /// Refill the text label based on what's currently visible.
    /// </summary>

    protected void UpdateVisibleText()
    {
        if (textLabel != null)
        {
            UIFont font = textLabel.font;

            if (font != null)
            {
                int lines = 0;
                int maxLines = maxHeight > 0 ? Mathf.FloorToInt(maxHeight / (textLabel.font.size * textLabel.font.pixelSize)) : 100000;

                if (mTotalLines > maxLines && !m_ChatScrollBar.gameObject.activeSelf)
                {
//                     m_ChatScrollBar.gameObject.SetActive(true);
//                     m_ChatScrollBar.value = 0;
                }
                if (m_ChatScrollBar.gameObject.activeSelf)
                {
                    m_ChatScrollBar.barSize = (float)maxLines / (float)mTotalLines;
                }

                int offset = Mathf.RoundToInt(mScroll);
                //LogModule.DebugLog("offset = " + offset);
                if (mTotalLines > maxLines)
                {
                    if (offset > m_offsetbuffer)
                    {
                        ChatInfoLogic.Instance().MoveLinkPos(ChatInfoLogic.EMOTIONLINK_MOVE_DIRECTION.EMOTIONLINK_MOVE_DOWN, Mathf.Abs(offset - m_offsetbuffer));
                        ChatInfoLogic.Instance().MoveEmotionPos(ChatInfoLogic.EMOTIONLINK_MOVE_DIRECTION.EMOTIONLINK_MOVE_DOWN, Mathf.Abs(offset - m_offsetbuffer));
                    }
                    else if (offset < m_offsetbuffer)
                    {
                        ChatInfoLogic.Instance().MoveLinkPos(ChatInfoLogic.EMOTIONLINK_MOVE_DIRECTION.EMOTIONLINK_MOVE_UP, Mathf.Abs(offset - m_offsetbuffer));
                        ChatInfoLogic.Instance().MoveEmotionPos(ChatInfoLogic.EMOTIONLINK_MOVE_DIRECTION.EMOTIONLINK_MOVE_UP, Mathf.Abs(offset - m_offsetbuffer));
                    }
                }                
                m_offsetbuffer = offset;

                // Don't let scrolling to exceed the visible number of lines
                if (maxLines + offset > mTotalLines)
                {
                    offset = Mathf.Max(0, mTotalLines - maxLines);
                    mScroll = offset;
                }

                if (style == Style.Chat)
                {
                    offset = Mathf.Max(0, mTotalLines - maxLines - offset);
                }

                StringBuilder final = new StringBuilder();
                string startColor = "";
                if (offset > 0)
                {
                    int halfindex = IsOffsetHalfParagraph(offset);
                    if (halfindex >= 0)
                    {
                        startColor = mParagraphs[halfindex].text.Substring(0, 8);
                    }
                }

                for (int i = 0, imax = mParagraphs.Count; i < imax; ++i)
                {
                    Paragraph p = mParagraphs[i];

                    for (int b = 0, bmax = p.lines.Length; b < bmax; ++b)
                    {
                        string s = p.lines[b];

                        if (offset > 0)
                        {
                            --offset;
                        }
                        else
                        {
                            if (final.Length > 0) final.Append("\n");
                            final.Append(s);
                            ++lines;
                            if (lines >= maxLines) break;
                        }
                    }
                    if (lines >= maxLines) break;
                }
                textLabel.text = startColor + final.ToString();
            }
        }
    }

    int IsOffsetHalfParagraph(int offset)
    {
        int totalLines = 0;
        for (int i = 0, imax = mParagraphs.Count; i < imax; ++i)
        {
            totalLines += mParagraphs[i].lines.Length;
            if (totalLines == offset)
            {
                return -1;
            }
            else if (totalLines > offset)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// 鼠标滚轮操作 统一改为设置ScrollBar.value 之后会调用回调函数OnScrollBarValueChange
    /// </summary>
    /// <param name="val">滚轮位移</param>
    void OnScroll(float val)
    {
        if (mSelected && supportScrollWheel)
        {           
//             val *= (style == Style.Chat) ? 10f : -10f;
//             mScroll = Mathf.Max(0f, mScroll + val);           
//             UpdateVisibleText();   
            if (m_ChatScrollBar.gameObject.activeSelf)
            {
                UpdateScrollBarValue(val);  
            }
            else
            {
                UpdateFakeScrollBarValue(val);
            }
        }
    }

    /// <summary>
    /// 拖拽操作 统一改为设置ScrollBar.value 之后会调用回调函数OnScrollBarValueChange
    /// </summary>
    /// <param name="delta">拖拽位移</param>
    void OnDrag(Vector2 delta)
    {
//         if(mSelected)
//         {
//             float val = (delta.y > 0) ? -0.05f : 0.05f;
//             if (m_ChatScrollBar.gameObject.activeSelf)
//             {
//                 UpdateScrollBarValue(val);
//             }
//             else
//             {
//                 UpdateFakeScrollBarValue(val);
//             }
//         }
        if (mSelected)
        {
            bool withScroll = false;
            if (mScroll != 0)
            {
                withScroll = true;
            }

            int maxLines = maxHeight > 0 ? Mathf.FloorToInt(maxHeight / (textLabel.font.size * textLabel.font.pixelSize)) : 100000;
            float val = 0;
            if (delta.y > 0)
            {
                if (mScroll <= 0)
                {
                    return;
                }
            }
            else
            {
                if (mScroll >= mTotalLines - maxLines)
                {
                    return;
                }                
            }
            val = -delta.y / 10.0f;

            mScroll = mScroll + val;
            if (mScroll <= 0)
            {
                mScroll = 0;
                if (withScroll && ChatInfoLogic.Instance() != null && ChatInfoLogic.Instance().WaitRefresh)
                {
                    ChatInfoLogic.Instance().InitChatInfo();
                    ChatInfoLogic.Instance().WaitRefresh = false;
                }
            }
            if (mScroll >= mTotalLines - maxLines)
            {
                mScroll = mTotalLines - maxLines;
            }
            UpdateVisibleText();
        }
    }

    public void OnScrollBarValueChange()
    {
        if (m_ChatScrollBar != null)
        {
            int maxLines = maxHeight > 0 ? Mathf.FloorToInt(maxHeight / (textLabel.font.size * textLabel.font.pixelSize)) : 100000;
            // mTotalLines - maxLines为mScroll的最大值 把ScrollBar.value的0~1映射到0~(mTotalLines - maxLines)
            mScroll = m_ChatScrollBar.value * (mTotalLines - maxLines);
            UpdateVisibleText();
        }
    }

    void UpdateScrollBarValue(float val)
    {
        if (m_ChatScrollBar.gameObject.activeSelf)
        {
            if (m_ChatScrollBar.value + val >= 1)
            {
                m_ChatScrollBar.value = 1;
            }
            else if (m_ChatScrollBar.value + val <= 0)
            {
                m_ChatScrollBar.value = 0;
            }
            else
            {
                m_ChatScrollBar.value += val;
            }
        }        
    }

    void UpdateFakeScrollBarValue(float val)
    {
        if (m_FakeScrollBarValue + val >= 1)
        {
            m_FakeScrollBarValue = 1;
        }
        else if (m_FakeScrollBarValue + val <= 0)
        {
            m_FakeScrollBarValue = 0;
        }
        else
        {
            m_FakeScrollBarValue += val;
        }

        int maxLines = maxHeight > 0 ? Mathf.FloorToInt(maxHeight / (textLabel.font.size * textLabel.font.pixelSize)) : 100000;
        mScroll = m_FakeScrollBarValue * (mTotalLines - maxLines);
        UpdateVisibleText();
    }
}
