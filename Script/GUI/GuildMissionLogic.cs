using UnityEngine;
using System.Collections;
using GCGame.Table;
using Games.Mission;

public class GuildMissionLogic : MonoBehaviour {


    public Transform m_NotOpenTran;                 // 帮主未开启使用文字描述位置
    public Transform m_OpenTran;                    // 已接取文字描述
    public Transform m_MissionInfoTran;             // 参与帮会任务后显示界面位置

    public UILabel m_AssignLable;
    public UILabel m_AcceptLable;

    public UIImageButton m_AssignBtn;               // 发布任务按钮
    public UIImageButton m_PartakeBtn;              // 参与按钮

    public UILabel m_WealthLable;                   // 帮会财富

    public UILabel m_MissTitleLable;                // 任务标题
    public UILabel m_MissContLable;                 // 任务内容
    public UILabel m_SilverLable;                   // 金币奖励
    public UILabel m_ExpLable;                      // 经验奖励
    public UILabel m_DoneNumLable;                  // 已做任务环数

    public Transform m_AcceptBtn;                   // 接受任务按钮
    public Transform m_CompleteBtn;                 // 完成任务按钮
    public Transform m_AbandonBtn;                  // 放弃任务按钮

    public GameObject m_CanUse01;
    public GameObject m_CanUse02;
    public GameObject m_Invaild01;
    public GameObject m_Invaild02;

    private int m_MissionId = -2;                   // 现在显示的任务id

    private static GuildMissionLogic m_Instance = null;
    public static GuildMissionLogic Instance()
    {
        return m_Instance;
    }

    private void Awake()
    {
        m_Instance = this;
    }
	// Use this for initialization
	void Start () 
    {
		ClearUp ();
        UpdateGuildMission();
	}

	void ClearUp()
	{
		m_AssignLable.text = "0/0";
		m_AcceptLable.text = "0/0";
		m_WealthLable.text = "0";
	}

    public void UpdateGuildMission()
    {
        if (false == GameManager.gameManager.PlayerDataPool.IsHaveGuild())
        {
            m_NotOpenTran.gameObject.SetActive(true);
            m_OpenTran.gameObject.SetActive(false);
            m_MissionInfoTran.gameObject.SetActive(false);
            ChangeBtnSpite(m_AssignBtn, false);
			m_AssignBtn.gameObject.SetActive(false);
            ChangeBtnSpite(m_PartakeBtn, false);
            m_CanUse01.SetActive(false);
            m_Invaild01.SetActive(true);
            m_CanUse02.SetActive(false);
            m_Invaild02.SetActive(true);
            return;
        }

        if(GameManager.gameManager.PlayerDataPool.IsGuildChief()) 
        {
            m_AssignBtn.gameObject.SetActive( true );
            ChangeBtnSpite(m_AssignBtn, GameManager.gameManager.PlayerDataPool.GuildInfo.CanGMAssign);
            m_CanUse01.SetActive(GameManager.gameManager.PlayerDataPool.GuildInfo.CanGMAssign);
            m_Invaild01.SetActive(!GameManager.gameManager.PlayerDataPool.GuildInfo.CanGMAssign);
        }
        else
        {
            m_AssignBtn.gameObject.SetActive(false);
        }


        if (null != m_WealthLable)
        {
            m_WealthLable.text = GameManager.gameManager.PlayerDataPool.GuildInfo.GuildWeath.ToString();
        }

        if (null != m_AssignLable)
        {
            m_AssignLable.text = GameManager.gameManager.PlayerDataPool.GuildInfo.GMCurAssign.ToString() +
                "/" + GameManager.gameManager.PlayerDataPool.GuildInfo.GMMaxAssign.ToString();
        }

        if (null != m_AcceptLable)
        {
            m_AcceptLable.text = GameManager.gameManager.PlayerDataPool.GuildInfo.GMCurPartake.ToString() +
                "/" + GameManager.gameManager.PlayerDataPool.GuildInfo.GMMaxPartake.ToString();
        }
        
        Tab_GuildMissionGuild tabGMGuild =
            TableManager.GetGuildMissionGuildByID(GameManager.gameManager.PlayerDataPool.GuildInfo.GuildLevel, 0);
        bool isCanPartake = ((tabGMGuild != null) && GameManager.gameManager.PlayerDataPool.GuildInfo.GMCurPartake > 0
                            && (GameManager.gameManager.PlayerDataPool.GuildInfo.GMCanPartakeType < tabGMGuild.MemMaxTimesOneDay)
                            && (GameManager.gameManager.PlayerDataPool.GuildInfo.CMMisionID <= 0));
        ChangeBtnSpite(m_PartakeBtn, isCanPartake);

		if (null != m_DoneNumLable)
		{
			m_DoneNumLable.text = GameManager.gameManager.PlayerDataPool.GuildInfo.GMDoneNum.ToString();
		}

        if (m_CanUse02)
        {
            m_CanUse02.SetActive(isCanPartake);
        }

        if (m_Invaild02)
        {
            m_Invaild02.SetActive(!isCanPartake);
        }

        if (isCanPartake)
        {
            m_NotOpenTran.gameObject.SetActive(false);
            m_OpenTran.gameObject.SetActive(true);
            m_MissionInfoTran.gameObject.SetActive(false);
            return;
        }

        ShowMissionInfo();
        
    }

    void ShowMissionInfo()
    {
        m_MissionId = GameManager.gameManager.PlayerDataPool.GuildInfo.CMMisionID;

        if (GameManager.gameManager.PlayerDataPool.GuildInfo.CMMisionID <= 0)
        {
           
            m_NotOpenTran.gameObject.SetActive(true);
            m_OpenTran.gameObject.SetActive(false);
            m_MissionInfoTran.gameObject.SetActive(false);
            return;
        }

        Tab_MissionDictionary tabDict = TableManager.GetMissionDictionaryByID(m_MissionId, 0);

        if (null == tabDict) { return; }

        if (null != m_MissTitleLable) { m_MissTitleLable.text = tabDict.MissionName; }

        if (null != m_MissContLable) { m_MissContLable.text = tabDict.MissionDesc; }

        Tab_MissionBase tabBase = TableManager.GetMissionBaseByID(m_MissionId, 0);

        if (null == tabBase) { return; }

        Tab_GuildMission tabGM = TableManager.GetGuildMissionByID(tabBase.GuildMissionTabID, 0);

        if (null == tabBase) { return; }

        Tab_GuildMissionAward tabAward = TableManager.GetGuildMissionAwardByID(GameManager.gameManager.PlayerDataPool.MainPlayerBaseAttr.Level, 0);

        if (null == tabAward) { return; }

        Tab_GuildMissionGuild tabGMGuid = TableManager.GetGuildMissionGuildByID(GameManager.gameManager.PlayerDataPool.GuildInfo.GuildLevel, 0);

        if (null == tabAward) { return; }

        int coinAward = (int)(tabGM.AwardMoney * tabAward.MoneyFactor * tabGMGuid.AwardFactor);
        if (null != m_SilverLable) { m_SilverLable.text = coinAward.ToString(); }

        int expAward = (int)(tabGM.AwardExp * tabAward.ExpFactor * tabGMGuid.AwardFactor);
        if (null != m_ExpLable) { m_ExpLable.text = expAward.ToString(); }

        m_NotOpenTran.gameObject.SetActive(false);
        m_OpenTran.gameObject.SetActive(false);
        m_MissionInfoTran.gameObject.SetActive(true);

        if (!GameManager.gameManager.MissionManager.IsHaveMission(m_MissionId))
        {
            m_AcceptBtn.gameObject.SetActive(true);
            m_CompleteBtn.gameObject.SetActive(false);
            m_AbandonBtn.gameObject.SetActive(false);
            return;
        }

        // 3、任务状态检查
        if ((byte)MissionState.Mission_Completed != GameManager.gameManager.MissionManager.GetMissionState(m_MissionId))
        {
            m_AcceptBtn.gameObject.SetActive(false);
            m_CompleteBtn.gameObject.SetActive(false);
            m_AbandonBtn.gameObject.SetActive(true);
            return;
        }

        m_AcceptBtn.gameObject.SetActive(false);
        m_CompleteBtn.gameObject.SetActive(true);
        m_AbandonBtn.gameObject.SetActive(false);

    }

    void ChangeBtnSpite(UIImageButton btn, bool isActive = true)
    {
        if (null == btn)
        {
            return;
        }

        if (false == isActive)
        {
			btn.normalSprite = "ui_pub_026_gray";
			btn.hoverSprite = "ui_pub_026_gray";
			btn.pressedSprite = "ui_pub_026_gray";
			btn.disabledSprite = "ui_pub_026_gray";
			btn.target.spriteName = "ui_pub_026_gray"; 
			return;
        }
		btn.normalSprite = "ui_pub_026";
		btn.hoverSprite = "ui_pub_026";
		btn.pressedSprite = "ui_pub_026";
		btn.disabledSprite = "ui_pub_026";
		btn.target.spriteName = "ui_pub_026"; 
    }

    void OnBtnAssign()
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        Tab_GuildMissionGuild tab = TableManager.GetGuildMissionGuildByID( GameManager.gameManager.PlayerDataPool.GuildInfo.GuildLevel, 0);
        if (null == tab) { return; }

        string dicStr = StrDictionary.GetClientDictionaryString("#{5432}", tab.ConsumeWealth, tab.CanAcceptTimesOnce);
        MessageBoxLogic.OpenOKCancelBox(dicStr, "", DoAssign, null);

    }

    void DoAssign()
    {
        Singleton<ObjManager>.GetInstance().MainPlayer.AssignGuildMission();
    }

    void OnBtnPartake()
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        Singleton<ObjManager>.GetInstance().MainPlayer.PartakeGuildMission();
    }

    void OnBtnAccept()
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        Singleton<ObjManager>.GetInstance().MainPlayer.AcceptGuildMission();
    }

    void OnBtnComplete()
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        Singleton<ObjManager>.GetInstance().MainPlayer.CompleteGuildMission();
    }

    void OnBtnAbandon()
    {
        string dicStr = StrDictionary.GetClientDictionaryString("#{5440}");
        MessageBoxLogic.OpenOKCancelBox(dicStr, "", AbandonGuildMission, null);
    }

    void AbandonGuildMission()
    {
        Singleton<ObjManager>.GetInstance().MainPlayer.AbandonGuildMission();
    }

    public void UpdateGuildMissionBtn(int Type)
    {
        bool canAccept = false;
        bool canAbandon = false;
        bool canComplete = false;
        switch (Type)
        {
            case 2:
                canAccept = true;
                break;
            case 1:
                canComplete = true;
                break;
            case 0:
                canAbandon = true;
                break;
            default:
                break;
        }
        m_AcceptBtn.gameObject.SetActive(canAccept);
        m_AbandonBtn.gameObject.SetActive(canAbandon);
        m_CompleteBtn.gameObject.SetActive(canComplete);
    }
}
