using UnityEngine;
using System.Collections;
using Games.Mission;

public class ZiPaiLogic : UIControllerBase<ZiPaiLogic>
{
    public const int m_MissionID = 105;
    public const int nMaxButtonNum = 8;
    public UISprite[] m_NormalSprit = new UISprite[nMaxButtonNum];
    public UISprite[] m_AcitiveSprit = new UISprite[nMaxButtonNum];
    //    public UILabel[] m_TempLable = new UILabel[nMaxButtonNum]; // 临时替代
    //    private string[] m_RightSort = { "0", "1", "2", "3", "4", "5", "6", "7" };
    private int[] m_RightSort = { 0, 1, 2, 3, 4, 5, 6, 7 };
    private int[] m_CurSort = { 5, 1, 6, 4, 3, 0, 7, 2 };

    private int m_nLastIndex = -1; // 上次选中索引

    public float m_GameTime = 240; // 游戏时长
    public float m_EndIntervalTime = 2; // 结束等待时长
    private float m_CurTimeCount = 0;   // 计时器


    public TweenAlpha m_FinishEffect;
    private float m_CheckFinishTime = 0.5f;  // 每隔0.5s检查一次
    private float m_CurCheckTimeCount = 0;
    private bool m_bFinishFlag = false;

    public UILabel m_TimeCountText; //倒计时牌
    private float m_TipTimeCount = 0;

    void Awake()
    {
        SetInstance(this);
    }

    // Use this for initialization
    void Start()
    {
        Init();
    }

    void FixedUpdate()
    {
        UpDateTimeCountTip();

        // 全局时间
        if (m_CurTimeCount >= m_GameTime + m_EndIntervalTime)
        {
            UIManager.CloseUI(UIInfo.ZiPaiRoot);
        }
        else if (m_CurTimeCount >= m_GameTime && m_bFinishFlag == false)
        {
            // 直接设置完成
            FinishZiPai();
        }
        else
        {
            m_CurTimeCount += Time.deltaTime;
        }

        // 检查完成
        if (m_bFinishFlag == false)
        {
            if (m_CurCheckTimeCount >= m_CheckFinishTime)
            {
                CheckFinish();
                m_CurCheckTimeCount = 0;
            }
            else
            {
                m_CurCheckTimeCount += Time.deltaTime;
            }
        }
    }

    void OnDestroy()
    {
        SetInstance(null);
    }

    void Init()
    {
        m_nLastIndex = -1;
        m_CurTimeCount = 0;
        m_CurCheckTimeCount = 0;
        m_bFinishFlag = false;

        if (m_FinishEffect)
        {
            m_FinishEffect.enabled = false;
            m_FinishEffect.Reset();
            m_FinishEffect.gameObject.SetActive(false);
        }

        for (int i = 0; i < nMaxButtonNum; i++)
        {
            if (m_AcitiveSprit[i])
            {
                m_AcitiveSprit[i].gameObject.SetActive(false);
            }

        }
        if (m_TimeCountText)
        {
            m_TimeCountText.text = "";
        }
    }

    void CheckFinish()
    {
        if (m_bFinishFlag == true)
        {
            return;
        }
        // 检查完成
        if (IsInRightSort())
        {
            FinishZiPai();
        }
    }

    bool IsInRightSort()
    {
        for (int i = 0; i < nMaxButtonNum; i++)
        {
            if (m_CurSort[i] != m_RightSort[i])
            {
                return false;
            }

        }
        return true;
    }

    void OnButtonOKClick()
    {
        if (m_bFinishFlag)
        {
            UIManager.CloseUI(UIInfo.ZiPaiRoot);
        }
        else
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2119}");
        }
    }

    void OnButtonClick(GameObject gObj)
    {
        if (gObj == null)
        {
            return;
        }

        if (m_bFinishFlag)
        {
            return;
        }

        int nCurIndex = -1;
        bool bRet = int.TryParse(gObj.name, out nCurIndex);
        if (bRet && nCurIndex >= 0 && nCurIndex < nMaxButtonNum)
        {
            if (m_AcitiveSprit[nCurIndex] == null)
            {
                return;
            }
            if (m_nLastIndex == nCurIndex)
            {
                m_nLastIndex = -1;
                m_AcitiveSprit[nCurIndex].gameObject.SetActive(false);
            }
            else if (m_nLastIndex == -1)
            {
                m_nLastIndex = nCurIndex;
                m_AcitiveSprit[nCurIndex].gameObject.SetActive(true);
            }
            else
            {
                Swap(m_nLastIndex, nCurIndex);
            }
        }
    }

    // 切换图片
    void Swap(int nLastIndex, int nCurIndex)
    {
        if (nLastIndex < 0 || nLastIndex >= nMaxButtonNum
            || nCurIndex < 0 || nCurIndex >= nMaxButtonNum)
        {
            return;
        }
        if (m_NormalSprit[nLastIndex] && m_AcitiveSprit[nLastIndex]
            && m_NormalSprit[nCurIndex] && m_AcitiveSprit[nCurIndex])
        {
            m_AcitiveSprit[nLastIndex].gameObject.SetActive(false);
            m_AcitiveSprit[nCurIndex].gameObject.SetActive(false);

            string strTem = m_NormalSprit[nLastIndex].spriteName;
            m_NormalSprit[nLastIndex].spriteName = m_NormalSprit[nCurIndex].spriteName;
            m_NormalSprit[nCurIndex].spriteName = strTem;

            //             m_NormalSprit[nCurIndex].gameObject.SetActive(true);
            //             m_NormalSprit[nCurIndex].gameObject.SetActive(true);

            int nValue = m_CurSort[nLastIndex];
            m_CurSort[nLastIndex] = m_CurSort[nCurIndex];
            m_CurSort[nCurIndex] = nValue;

            m_nLastIndex = -1;
        }
    }

    void FinishBlink()
    {
        if (m_FinishEffect)
        {
            m_FinishEffect.gameObject.SetActive(true);
            m_FinishEffect.Play(true);
        }
    }

    void FinishZiPai()
    {
        m_bFinishFlag = true;
        SortZiPai();
        FinishBlink();
        Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2120}");

        bool isHaveMission = GameManager.gameManager.MissionManager.IsHaveMission(m_MissionID);
        if (isHaveMission)
        {
            MissionState misState = (MissionState)GameManager.gameManager.MissionManager.GetMissionState(m_MissionID);
            if (MissionState.Mission_Accepted == misState)
            {
                GameManager.gameManager.MissionManager.SetMissionParam(m_MissionID, 0, 1);
                GameManager.gameManager.MissionManager.SetMissionState(m_MissionID, 2);
            }
        }
    }

    void SortZiPai()
    {
        for (int nIndex = 0; nIndex < nMaxButtonNum; nIndex++)
        {
            if (m_CurSort[nIndex] == nIndex)
            {
                continue;
            }
            for (int i = nIndex + 1; i < nMaxButtonNum; i++)
            {
                if (m_CurSort[i] == nIndex)
                {
                    Swap(nIndex, i);
                }
            }
        }
    }

    void UpDateTimeCountTip()
    {
        if (m_CurTimeCount > m_GameTime)
        {
            return;
        }

        if (m_TimeCountText == null)
        {
            return;
        }

        m_TipTimeCount += Time.deltaTime;
        if (m_TipTimeCount < 1)
        {
            return;
        }

        int nLeftTime = (int)(m_GameTime - m_CurTimeCount);
        if (nLeftTime >= 0)
        {
            int nMini = nLeftTime / 60;
            int nSecond = nLeftTime % 60;
            m_TimeCountText.text = "" + nMini / 10 + nMini % 10 + ":" + nSecond / 10 + nSecond % 10;
        }
        m_TipTimeCount = 0;
    }
}