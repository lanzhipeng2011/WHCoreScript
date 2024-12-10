/************************************************************************/
/* 文件名：AwardLogic.cs    
 * 创建日期：2014.03.21
 * 创建人：贺文鹏
 * 功能说明：奖励整合界面
/************************************************************************/
using UnityEngine;
using System.Collections;
using Module.Log;
using GCGame.Table;
using Games.UserCommonData;

public class AwardLogic : UIControllerBase<AwardLogic>
{
    public TabController m_TabButton; 

    // 每日奖励
    public TabButton m_TabDayAward;
    public DayAwardLogic m_DayAwardRoot;
    public GameObject m_DayAwardTip;
    public UILabel m_DayAwardTipText;
    private int m_WeekDay;
    private bool m_DayAwardFlag;

    // 在线奖励
    public TabButton m_TabOnlineAward;
    public OnlineAwardLogic m_OnlineAwardRoot;
    public GameObject m_OnlineAwardTip;
    public UILabel m_OnlineAwardTipText;
    private int m_OnlineAwardID;
    private int m_LeftTime;
    public int LeftTime
    {
        set {
            m_LeftTime = value;
            if (m_OnlineAwardRoot != null && m_OnlineAwardRoot.gameObject.activeSelf)
            {
                m_OnlineAwardRoot.LeftTime = m_LeftTime;
            }
        }
    }

    //
    public GameObject m_SNSTab;     //SNS分页按钮
    public GameObject m_SNSAwardRoot;
    public GameObject m_SNSAwardTip;
    public UILabel m_SNSAwardTipText;
    public GameObject m_SNSShareBtnTip;
    public delegate void AfterCallDelegate();
    public AfterCallDelegate delegateAfterCall;

    public void SetAfterStartDelegateSNS()
    {
        delegateAfterCall += ShowSNSWindow;
    }

    void ShowSNSWindow()
    {
        m_TabButton.ChangeTab("Button5-SNS-Award");
//         m_DayAwardRoot.gameObject.SetActive(false);
//         m_OnlineAwardRoot.gameObject.SetActive(false);
//         m_NewServerAwardRoot.gameObject.SetActive(false);
//         m_NewOnlineAwardRoot.gameObject.SetActive(false);
//         m_CDkeyWindow.SetActive(false);
// 
//         m_SNSAwardRoot.SetActive(true);
    }


    // 首周奖励
    public TabButton m_TabNewServerAward;
    public NewServerAwardLogic m_NewServerAwardRoot;
    public GameObject m_NewServerAwardTip;
    public UILabel m_NewServerAwardTipText;
    private int m_NewServerDays;
    
    public UIGrid m_ButtonGrid;
    
    // 新开服在线奖励
    public TabButton m_TabNewOnlineAward;
    public NewOnlineAwardLogic m_NewOnlineAwardRoot;
    public GameObject m_NewOnlineAwardTip;
    public UILabel m_NewOnlineAwardTipText;
    private int m_NewOnlineAwardID;
    private int m_NewLeftTime;
    private bool m_IsShowNewOnlineAward;
    private bool m_IsNewOnlineAwardStart;
    public int NewLeftTime
    {
        set
        {
            m_NewLeftTime = value;
            if (m_NewOnlineAwardRoot != null && m_NewOnlineAwardRoot.gameObject.activeSelf)
            {
                m_NewOnlineAwardRoot.LeftTime = m_NewLeftTime;
            }
        }
    }
    //cdkey//////////////////////////////////
    public TabButton m_TabCDkey;
    public GameObject m_CDkeyWindow;
    public UIInput m_CDKeyInput;
    // 限时在线奖励
    public TabButton m_TabNew7DayOnlineAward;
    public New7DayOnlineAwardLogic m_New7DayOnlineAwardRoot;
    public GameObject m_New7DayOnlineAwardTip;
    public UILabel m_New7DayOnlineAwardTipText;
    private int m_New7DayOnlineAwardID;
    private int m_New7DayLeftTime;
    private bool m_IsShowNew7DayOnlineAward;
    private bool m_IsNew7DayOnlineAwardStart;
    public int New7DayLeftTime
    {
        set
        {
            m_New7DayLeftTime = value;
            if (m_New7DayOnlineAwardRoot != null && m_New7DayOnlineAwardRoot.gameObject.activeSelf)
            {
                m_New7DayOnlineAwardRoot.LeftTime = m_New7DayLeftTime;
            }
        }
    }
    void ShowCDkey()
    {
        m_CDkeyWindow.SetActive(true);
        m_CDKeyInput.value = "";
    }
    void SendCDkey()
    {
        if (m_CDKeyInput.value.Length > 0)
        {
            CG_REQUEST_CDKEY packet = (CG_REQUEST_CDKEY)PacketDistributed.CreatePacket(MessageID.PACKET_CG_REQUEST_CDKEY);
            packet.Cdkeystr = m_CDKeyInput.value;
            packet.SendPacket();
			CloseWindow();
        }
        else
        {
            if (Singleton<ObjManager>.GetInstance().MainPlayer)
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2922}");
            }
        }
    }
    void PasteCDkey()
    {

    }
    
    //////////////////////////////////
    void Awake()
    {
        //根据标记位确认是否显示按钮
        //分享功能
        if (false == GameManager.gameManager.PlayerDataPool.IsServerFlagOpen(SERVER_FLAGS_ENUM.FLAG_SNS) ||
            false ==PlatformHelper.IsEnableShareCenter())
        {
            m_SNSTab.SetActive(false);
        }

        //激活功能
        if (false == GameManager.gameManager.PlayerDataPool.IsServerFlagOpen(SERVER_FLAGS_ENUM.FLAG_ACTIVATION))
        {
            m_TabCDkey.gameObject.SetActive(false);
        }
        SetInstance(this);
        m_TabButton.delTabChanged = OnTabChange;
    }

	// Use this for initialization
	void Start () {
	}

    void OnEnable()
    {
#if UNITY_IPHONE || UNITY_WP8 || UNITY_BLACKBERRY   //|| UNITY_ANDROID
		TouchScreenKeyboard.hideInput = false;		
#endif
        CleanUp();
        UpdateTip();
        ShowWindow();
        if (delegateAfterCall != null)
        {
            delegateAfterCall();
            delegateAfterCall = null;
        }
	}

    void OnDestroy()
    {
        SetInstance(null);
    }


    void CloseWindow()
    {
		UIManager.CloseUI(UIInfo.RewardRoot);
	}
	
    void OnTabChange(TabButton button)
    {
        if (button.name == "Button1-Award")
        {
            ShowNewServerAward();
        }
        else if (button.name == "Button2-Award")
        {
            ShowOnlineAward();
        }
        else if (button.name == "Button3-Award")
        {
            ShowDayAward();
        }
        else if (button.name == "Button4-Award")
        {
            ShowNewOnlineAward();
        }
		else if (button.name == "Button6-Duihuan-Award")
		{
            ShowCDkey();
        }
        else if (button.name == "Button8-Award")
        {
            Show7DayNewOnlineAward();
        }
    }
   
    void ShowWindow()
    {
        if (null == m_TabButton)
        {
            return;
        }

        if (m_TabNewServerAward != null)
        {
            m_TabNewServerAward.gameObject.SetActive(false);
            if (m_NewServerDays >= 0 && m_NewServerDays < 7)
            {
                m_TabNewServerAward.gameObject.SetActive(true);
            }

            if (m_NewServerAwardTip.activeInHierarchy)
            {
                //ShowNewServerAward();
                m_TabButton.ChangeTab("Button1-Award");
            }
            else if(m_OnlineAwardTip.activeInHierarchy)
            {
                //ShowOnlineAward();
                m_TabButton.ChangeTab("Button2-Award");
            }
            else if (m_DayAwardTip.activeInHierarchy)
            {
                //ShowDayAward();
                m_TabButton.ChangeTab("Button3-Award");
            }
            else if (m_NewOnlineAwardTip.activeInHierarchy)
            {
                m_TabButton.ChangeTab("Button4-Award");
            }
            else if (m_New7DayOnlineAwardTip.activeInHierarchy)
            {
                m_TabButton.ChangeTab("Button8-Award");
            }
            else
            {
                // 无提醒 处理
                if (m_TabNewServerAward.gameObject.activeInHierarchy)
                {
                    //ShowNewServerAward();
                    m_TabButton.ChangeTab("Button1-Award");
                }
                else
                {
                    //ShowOnlineAward();
                    m_TabButton.ChangeTab("Button2-Award");
                }
            }

            if (m_ButtonGrid != null)
            {
                m_ButtonGrid.repositionNow = true;
            }
        }
        if (m_TabNewOnlineAward != null)
        {           
            if (m_IsShowNewOnlineAward)
            {
                m_TabNewOnlineAward.gameObject.SetActive(true);
            }
            else
            {
                m_TabNewOnlineAward.gameObject.SetActive(false);
            }
            if (m_ButtonGrid != null)
            {
                m_ButtonGrid.repositionNow = true;
            }
            if (m_IsShowNew7DayOnlineAward)
            {
                m_TabNew7DayOnlineAward.gameObject.SetActive(true);
            }
            else
            {
                m_TabNew7DayOnlineAward.gameObject.SetActive(false);
            }
        }
        
        if (m_TabCDkey != null)
        {
            if ( GameManager.gameManager.PlayerDataPool.IsServerFlagOpen(SERVER_FLAGS_ENUM.FLAG_ACTIVATION))
            {
                m_TabCDkey.gameObject.SetActive(true);
                if (m_ButtonGrid != null)
                {
                    m_ButtonGrid.repositionNow = true;
                }
            }
            else
            {
                m_TabCDkey.gameObject.SetActive(false);
            }
        }
    }

    void ShowDayAward()
    {
        if (m_DayAwardRoot != null)
        {
            m_DayAwardRoot.ButtonDayAward();
        }
    }

    public void ShowOnlineAward()
    {
        if (m_OnlineAwardRoot != null)
        {
            m_OnlineAwardRoot.ButtonOnlineAward();
        }
    }

    void ShowNewServerAward()
    {
        if (m_NewServerAwardRoot != null)
        {
            m_NewServerAwardRoot.ButtonNewServerAward();
        }
    }
    public void ShowNewOnlineAward()
    {
        if (m_NewOnlineAwardRoot != null)
        {
            m_NewOnlineAwardRoot.ButtonOnlineAward();
        }
    }
    public void Show7DayNewOnlineAward()
    {
        if (m_New7DayOnlineAwardRoot != null)
        {
            m_New7DayOnlineAwardRoot.ButtonOnlineAward();
        }
    }
    // 初始化数据
    void InitData()
    {
        m_WeekDay = GameManager.gameManager.PlayerDataPool.AwardActivityData.WeekDay;
        m_DayAwardFlag = GameManager.gameManager.PlayerDataPool.AwardActivityData.DayAwardFlag;

        m_OnlineAwardID = GameManager.gameManager.PlayerDataPool.AwardActivityData.OnlineAwardID;
        m_LeftTime = GameManager.gameManager.PlayerDataPool.AwardActivityData.LeftTime;

        m_NewServerDays = GameManager.gameManager.PlayerDataPool.AwardActivityData.NewServerDays;

        // 开服在线奖励
        m_IsShowNewOnlineAward = GameManager.gameManager.PlayerDataPool.ShouNowOnlineAwardWindow;
        m_NewOnlineAwardID = GameManager.gameManager.PlayerDataPool.AwardActivityData.NewOnlineAwardID;
        m_NewLeftTime = GameManager.gameManager.PlayerDataPool.AwardActivityData.NewLeftTime;
        m_IsNewOnlineAwardStart = GameManager.gameManager.PlayerDataPool.AwardActivityData.NewOnlineAwardStart;

        m_IsShowNew7DayOnlineAward = GameManager.gameManager.PlayerDataPool.AwardActivityData.ShowNew7DayOnlineAwardWindow;
        m_New7DayOnlineAwardID = GameManager.gameManager.PlayerDataPool.AwardActivityData.New7DayOnlineAwardID;
        m_New7DayLeftTime = GameManager.gameManager.PlayerDataPool.AwardActivityData.New7DayLeftTime;
        m_IsNew7DayOnlineAwardStart = GameManager.gameManager.PlayerDataPool.AwardActivityData.New7DayOnlineAwardStart;
    }

    void CleanUp()
    {
        // 每日奖励
        m_WeekDay = -1;
        m_DayAwardFlag = false;
        if (m_DayAwardTip != null && m_DayAwardTipText != null)
        {
            m_DayAwardTipText.text = "";
            m_DayAwardTip.SetActive(false);
        }

        // 在线奖励
        m_OnlineAwardID = -1;
        m_LeftTime = -1;

        if (m_OnlineAwardTip != null && m_OnlineAwardTipText != null)
        {
            m_OnlineAwardTipText.text = "";
            m_OnlineAwardTip.SetActive(false);
        }

        // 首周奖励
        m_NewServerDays = -1;

        if (m_NewServerAwardTip != null && m_NewServerAwardTipText != null)
        {
            m_NewServerAwardTipText.text = "";
            m_NewServerAwardTip.SetActive(false);
        }

        // 开服在线奖励
        m_NewOnlineAwardID = -1;
        m_NewLeftTime = -1;
        m_IsNewOnlineAwardStart = false;
        if (m_NewOnlineAwardTip != null && m_NewOnlineAwardTipText != null)
        {
            m_NewOnlineAwardTipText.text = "";
            m_NewOnlineAwardTip.SetActive(false);
        }


        //SNS
        if (m_SNSAwardTip != null && m_SNSAwardTipText != null)
        {
            m_SNSAwardTipText.text = "";
            m_SNSAwardTip.SetActive(false);
        }
        if (m_SNSShareBtnTip != null)
        {
            m_SNSShareBtnTip.SetActive(false);
        }

        //cdkey
        m_CDkeyWindow.SetActive(false);

        // 开服7天在线奖励
        m_New7DayOnlineAwardID = -1;
        m_New7DayLeftTime = -1;
        m_IsNew7DayOnlineAwardStart = false;
        if (m_New7DayOnlineAwardTip != null && m_New7DayOnlineAwardTipText != null)
        {
            m_New7DayOnlineAwardTipText.text = "";
            m_New7DayOnlineAwardTip.SetActive(false);
        }
    }

    public void UpdateTip()
    {
        InitData();

        if (Singleton<ObjManager>.Instance.MainPlayer)
        {
            if (Singleton<ObjManager>.Instance.MainPlayer.BaseAttr.Level >= 5)
            {
                UpdateDayAwardTip();
                UpdateOnlineAwardTip();
                UpdateNewServerAwardTip();
                UpdateNewOnlineAwardTip();
                UpdateNew7DayOnlineAwardTip();
            }
        }

        UpdateSNSAwardTip();
    }

    // 每日奖励提醒
    void UpdateDayAwardTip()
    {
        if (m_DayAwardTip == null || m_DayAwardTipText == null)
        {
            return;
        }
        if (m_WeekDay >= 0 && m_DayAwardFlag == false)
        {
            m_DayAwardTip.SetActive(true);
            m_DayAwardTipText.text = "1";
        }
        else
        {
            m_DayAwardTipText.text = "";
            m_DayAwardTip.SetActive(false);
        }
    }

    // 在线领奖提醒
    void UpdateOnlineAwardTip()
    {
        if (m_OnlineAwardTip == null || m_OnlineAwardTipText == null)
        {
            return;
        }
        if (m_OnlineAwardID >= 0 && m_LeftTime <= 0)
        {
            m_OnlineAwardTip.SetActive(true);
            m_OnlineAwardTipText.text = "1";
        }
        else
        {
            m_OnlineAwardTipText.text = "";
            m_OnlineAwardTip.SetActive(false);
        }
    }

    // 首周奖励提醒
    void UpdateNewServerAwardTip()
    {
        if (m_NewServerAwardTip == null || m_NewServerAwardTipText == null)
        {
            return;
        }
        bool bRet = GameManager.gameManager.PlayerDataPool.AwardActivityData.IsCanGetAward(m_NewServerDays);
        if (bRet)
        {
            m_NewServerAwardTip.SetActive(true);
            m_NewServerAwardTipText.text = "1";
        }
        else
        {
            m_NewServerAwardTipText.text = "";
            m_NewServerAwardTip.SetActive(false);
        }
    }
    // 新开服在线领奖提醒
    void UpdateNewOnlineAwardTip()
    {
        if (m_NewOnlineAwardTip == null || m_NewOnlineAwardTipText == null)
        {
            return;
        }
        if (m_NewOnlineAwardID >= 0 && m_NewLeftTime <= 0 && m_IsNewOnlineAwardStart)
        {
            m_NewOnlineAwardTip.SetActive(true);
            m_NewOnlineAwardTipText.text = "1";
        }
        else
        {
            m_NewOnlineAwardTipText.text = "";
            m_NewOnlineAwardTip.SetActive(false);
        }
    }
    // 新开服在线领奖提醒
    void UpdateNew7DayOnlineAwardTip()
    {
        if (m_New7DayOnlineAwardTip == null || m_New7DayOnlineAwardTipText == null)
        {
            return;
        }
        if (m_New7DayOnlineAwardID >= 0 && m_New7DayLeftTime <= 0 && m_IsNew7DayOnlineAwardStart)
        {
            m_New7DayOnlineAwardTip.SetActive(true);
            m_New7DayOnlineAwardTipText.text = "1";
        }
        else
        {
            m_New7DayOnlineAwardTipText.text = "";
            m_New7DayOnlineAwardTip.SetActive(false);
        }
    }
    //SNS
    void UpdateSNSAwardTip()
    {
        if (m_SNSAwardTip == null || m_SNSAwardTipText == null || m_SNSShareBtnTip == null)
        {
            return;
        }

        int nLevel = 0;
        if (Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            nLevel = Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.Level;
        }

        if (nLevel < 16)
        {
            return;
        }

        bool bRet = GameManager.gameManager.PlayerDataPool.CommonData.GetCommondFlag((int)USER_COMMONFLAG.CF_SNS_DAILY_REWARD);
        if(!bRet)
        {
            m_SNSAwardTip.SetActive(true);
            m_SNSAwardTipText.text = "1";
            m_SNSShareBtnTip.SetActive(true);
        }
        else
        {
            m_SNSAwardTipText.text = "";
            m_SNSAwardTip.SetActive(false);
            m_SNSShareBtnTip.SetActive(false);
        }
    }

}