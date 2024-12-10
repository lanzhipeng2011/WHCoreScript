//////////////////////////////////////////////////////////////////////////
// 功能：任务--参悟剑谱界面
// 创建人：贺文鹏
// 时间：2014.5.23
//////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class JianPuLogic : UIControllerBase<JianPuLogic>
{

    public const int nMaxPicNum = 6;
    public UISprite[] m_PicList = new UISprite[nMaxPicNum];
    public float m_fIntervalTime = 1;
    private float m_fCurTimeCount = 0f;

    private int m_nCurPicIndex = -1;

    void Awake()
    {
        SetInstance(this);
    }

	// Use this for initialization
	void Start () {
        Init();
	}
	
    void FixedUpdate()
    {
        if (m_nCurPicIndex == 0)
        {
            if (m_PicList[m_nCurPicIndex] && m_fCurTimeCount >= m_fIntervalTime)
            {
                TweenPosition tweenPos = m_PicList[m_nCurPicIndex].GetComponent<TweenPosition>();
                if (tweenPos)
                {
                    tweenPos.Play(true);
                }
                m_fCurTimeCount = 0;
            }
            else
            {
                m_fCurTimeCount += Time.deltaTime;
            }
        }
        else if (m_nCurPicIndex >= nMaxPicNum)
        {
            if (m_fCurTimeCount >= m_fIntervalTime)
            {
                UIManager.CloseUI(UIInfo.JianPuRoot);
                return;
            }
            else
            {
                m_fCurTimeCount += Time.deltaTime;
            }
        }
    }

    void OnDestroy()
    {
        SetInstance(null);
    }

    void Init()
    {
        m_nCurPicIndex = 0;
        m_fCurTimeCount = 0f;
        for (int i = 0; i < nMaxPicNum; i++)
        {
            if (m_PicList[i])
            {
                TweenPosition tweenPos = m_PicList[i].GetComponent<TweenPosition>();
                if (tweenPos)
                {
                    tweenPos.enabled = false;
                    tweenPos.Reset();
                }

                TweenAlpha tweenAlpha = m_PicList[i].GetComponent<TweenAlpha>();
                if (tweenAlpha)
                {
                    tweenAlpha.enabled = false;
                    tweenAlpha.Reset();
                }
            }
        }
    }

    public void OnPlayTweenPositionOver()
    {
        if (m_nCurPicIndex >= 0 && m_nCurPicIndex < nMaxPicNum && m_PicList[m_nCurPicIndex])
        {
            TweenAlpha tweenAlpha = m_PicList[m_nCurPicIndex].GetComponent<TweenAlpha>();
            if (tweenAlpha)
            {
                tweenAlpha.Play(true);
            }
        }
    }

    public void OnPlayTweenAlphaOver()
    {
        m_nCurPicIndex += 1;
        if (m_nCurPicIndex >= 0 && m_nCurPicIndex < nMaxPicNum && m_PicList[m_nCurPicIndex])
        {
            TweenPosition tweenPos = m_PicList[m_nCurPicIndex].GetComponent<TweenPosition>();
            if (tweenPos)
            {
                tweenPos.Play(true);
            }
        }
    }
}
