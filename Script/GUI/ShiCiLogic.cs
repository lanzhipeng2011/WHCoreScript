//////////////////////////////////////////////////////////////////////////
// 功能：任务--诗词界面
// 创建人：贺文鹏
// 时间：2014.5.22
//////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class ShiCiLogic : UIControllerBase<ShiCiLogic>
{

    public const int nMaxLineNum = 4;
    public TweenAlpha[] m_LineList = new TweenAlpha[nMaxLineNum];
    public float m_fIntervalTime = 2;
    private float m_fCurTimeCount = 0f;

    private int m_nCurLineIndex = -1;

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
        if (m_nCurLineIndex == 0)
        {
            if (m_LineList[m_nCurLineIndex] && m_fCurTimeCount >= m_fIntervalTime)
            {
                if (m_LineList[m_nCurLineIndex])
                {
                    m_LineList[m_nCurLineIndex].enabled = true;
                }
                m_fCurTimeCount = 0;
            }
            else
            {
                m_fCurTimeCount += Time.deltaTime;
            }
        }
        else if (m_nCurLineIndex >= nMaxLineNum)
        {
            if (m_fCurTimeCount >= m_fIntervalTime*2+2) // 6s
            {
                UIManager.CloseUI(UIInfo.ShiCiRoot);
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
        m_nCurLineIndex = 0;
        m_fCurTimeCount = 0f;
        // 设置所有诗 alpha=0
        for (int i = 0; i < nMaxLineNum; i++ )
        {
            if (m_LineList[i])
            {
                m_LineList[i].enabled = false;
                m_LineList[i].alpha = 0;
            }
        }
    }

    public void OnPlayNextLine()
    {
        m_nCurLineIndex += 1;
        if (m_nCurLineIndex <0 || m_nCurLineIndex >= nMaxLineNum)
        {
            //UIManager.CloseUI(UIInfo.ChallengeRewardRoot);
            return;
        }
        if (m_LineList[m_nCurLineIndex])
        {
            m_LineList[m_nCurLineIndex].enabled = true;
        }
    }
}
