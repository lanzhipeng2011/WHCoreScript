using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GCGame;
using Games.MoneyTree;
public class MoneyTreeButtonLogic : UIControllerBase<MoneyTreeButtonLogic> {

    //摇一摇
    private float m_fOld_X = 0;
    private float m_fOld_Y = 0;
    private float m_fNew_X = 0;
    private float m_fNew_Y = 0;
    private float m_fDiff_X = 0;
    private float m_fDiff_Y = 0;

    public UIButton m_MoneyTreeButton;
    public UISprite m_ButtonSprit;
    public UILabel m_TimerText;

    public GameObject m_TipGameObj;
    public UILabel m_TipText;

    private string m_ButtonNormalName = "icon-yaoqianshu";
    private string m_ButtonActiveName = "icon-yaoqianshu";

    private int m_CurMoneyTreeID = -1;
    public int CurMoneyTreeID
    {
        get { return m_CurMoneyTreeID; }
        set
        {
            m_CurMoneyTreeID = value;
            if (m_ButtonSprit)
            {
                m_ButtonSprit.spriteName = m_ButtonNormalName;
            }
        }
    }

    private int m_CDTime = 0;
    public int CDTime
    {
        get { return m_CDTime; }
        set {
            m_CDTime = value;
            UpdateTimerText();
        }
    }

    public List<TweenAlpha> m_FoldTween = new List<TweenAlpha>();
    public BoxCollider m_BtnCollider;

    void OnAwake()
    {
        SetInstance(this);
    }

	// Use this for initialization
	void Start () {
        SetInstance(this);

        CleanUp();
        Init();
	}
	
	// Update is called once per frame
	void Update () {
        ShakeUp();
	}

    void OnDestroy()
    {
        SetInstance(null);
    }

    void CleanUp()
    {
        for (int i = 0; i < m_FoldTween.Count; ++i)
        {
            if (null != m_FoldTween[i])
            {
                m_FoldTween[i].Reset();
                m_FoldTween[i].alpha = 1;
            }
        }
        //foreach (TweenAlpha tween in m_FoldTween)
        //{
        //    tween.Reset();
        //    tween.alpha = 1;
        //}

        if (m_ButtonSprit && m_TimerText && m_TipText && m_TipGameObj)
        {
            m_ButtonSprit.spriteName = m_ButtonNormalName;
            m_TimerText.text = "";

            m_TipText.text = "";
            m_TipGameObj.SetActive(false);
        }

        m_CDTime = 0;
        m_ShakeUpFlag = false;
    }

    void OnEnable()
    {
        m_ShakeUpFlag = false;
    }

    void Init()
    {
        CurMoneyTreeID = GameManager.gameManager.PlayerDataPool.MoneyTreeData.MoneyTreeID;
        if (CurMoneyTreeID < 0)
        {
            gameObject.SetActive(false);
            return;
        }
        CDTime = GameManager.gameManager.PlayerDataPool.MoneyTreeData.CDTime;
    }

    void ButtonClick()
    {
        if (Singleton<ObjManager>.Instance.MainPlayer)
        {
            if (Singleton<ObjManager>.Instance.MainPlayer.BaseAttr.Level >= 15)
            {
                UIManager.ShowUI(UIInfo.MoneyTreeRoot);
            }
            else
            {
                Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false,"#{3189}");
            }
        }
    }

    private bool m_ShakeUpFlag = false;
    public bool ShakeUpFlag
    {
        get { return m_ShakeUpFlag; }
        set { m_ShakeUpFlag = value; }
    }

    void ShakeUp()
    {
        if (Singleton<ObjManager>.GetInstance().MainPlayer == null)
        {
            return;
        }
        // 前提是玩家不能死啊
        if (Singleton<ObjManager>.GetInstance().MainPlayer.IsDie())
        {
            return;
        }

        m_fNew_X = Input.acceleration.x;
        m_fNew_Y = Input.acceleration.y;

        m_fDiff_X = m_fNew_X - m_fOld_X;
        m_fDiff_Y = m_fNew_Y - m_fOld_Y;

        m_fOld_X = m_fNew_X;
        m_fOld_Y = m_fNew_Y;

        if (m_fDiff_X > 1 || m_fDiff_Y > 1)
        {

            if (Singleton<ObjManager>.Instance.MainPlayer.BaseAttr.Level < 15)
            {
                Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{3189}");
                return;
            }

            if (m_ShakeUpFlag == true)
            {
                return;
            }
            m_ShakeUpFlag = true;

#if UNITY_IPHONE
            Handheld.Vibrate();
#endif
            if (m_CDTime <= 0 && m_CurMoneyTreeID < 8)
            {
                GameManager.gameManager.PlayerDataPool.MoneyTreeData.SendAwardPacket(1, 1);
            }
        }
    }

    public void UpdateTimerText()
    {
        if (Singleton<ObjManager>.Instance.MainPlayer == null)
        {
            return;
        }
        int nLevel = Singleton<ObjManager>.Instance.MainPlayer.BaseAttr.Level;

        if (m_TimerText == null ||  m_ButtonSprit == null || m_TipGameObj == null || m_TipText == null)
        {
            return;
        }
        CurMoneyTreeID = GameManager.gameManager.PlayerDataPool.MoneyTreeData.MoneyTreeID;
        if (CurMoneyTreeID >= MoneyTreeData.MaxFreeAwardNum)
        {
            m_TipText.text = "";
            m_TipGameObj.SetActive(false);
            m_TimerText.text = Utils.GetDicByID(1977);
            return;
        }
        if (m_CDTime <= 0 )
        {
            gameObject.SetActive(false);
            m_TimerText.text = Utils.GetDicByID(1977);
            m_ButtonSprit.spriteName = m_ButtonActiveName;
            gameObject.SetActive(true);
            if (nLevel >= 15)
            {
                m_TipGameObj.SetActive(true);
                m_TipText.text = "1";
            }
            return;
        }
        else if (m_CDTime > 0 )
        {
            m_TipText.text = "";
            m_TipGameObj.SetActive(false);
        }

        int nMin = (m_CDTime % (60 * 60)) / 60;
        int nSec = m_CDTime % 60;
        if (m_TimerText)
        {
            m_TimerText.text = "" + nMin / 10 + nMin % 10 + ":" + nSec / 10 + nSec % 10;
        }
    }

    public static void PlayTween(bool bFold)
    {
        if (Instance() == null)
        {
            return;
        }

        Instance().PlayCurTween(bFold);
    }

    void PlayCurTween(bool bFold)
    {
        if (m_BtnCollider)
        {
            m_BtnCollider.enabled = !bFold;
        }

        for (int i = 0; i < m_FoldTween.Count; ++i)
        {
            if (null != m_FoldTween[i])
            {
                m_FoldTween[i].Play(bFold);
            }
        }
        //foreach (TweenAlpha tween in m_FoldTween)
        //{
        //    tween.Play(bFold);
        //}
    }
}