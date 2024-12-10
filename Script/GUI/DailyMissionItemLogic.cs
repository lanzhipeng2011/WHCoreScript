using UnityEngine;
using System.Collections;
using GCGame;
using Games.Mission;
using GCGame.Table;

public class DailyMissionItemLogic : MonoBehaviour {

    private int m_nMissionID;
    public int MissionID
    {
        get { return m_nMissionID; }
        set { m_nMissionID = value; }
    }
    private byte m_yQuality;
    private int m_nKind;
    public int Kind
    {
        get { return m_nKind; }
        set { m_nKind = value; }
    }

    public UILabel m_NameText;
    public UILabel m_QualityColorText;
    public UILabel m_ActivenessText;
    public UILabel m_StateText;
    public UILabel m_UpdateText;
    public UILabel m_MisDec;

    public UIImageButton m_StateButton;
    public UIImageButton m_UpdateButton;

    public UIGrid m_ItemGrid;
    public const int MaxItemCount = 5;
    public GameObject[] m_AwardItem = new GameObject[MaxItemCount];
    public UISprite m_ItemSprit;
    public UILabel[] m_AwardItemNumLable;

	// Use this for initialization
	void Start () {
        Check_NewPlayerGuide();
	}

    void CleanUp()
    {
        m_nMissionID = -1;
        m_yQuality = 0;

        if (m_NameText && m_QualityColorText && m_ActivenessText && m_StateText && m_UpdateText && m_MisDec)
        {
            m_NameText.text = "";
            m_QualityColorText.text = "";
            m_ActivenessText.text = "";
            m_StateText.text = "";
            m_UpdateText.text = Utils.GetDicByID(1620);
            m_MisDec.text = "";
        }

        for (int i = 0; i < m_AwardItem.Length; ++i)
        {
            if (null != m_AwardItem[i])
            {
                m_AwardItem[i].SetActive(false);
            }
        }
        //foreach (GameObject gObj in m_AwardItem)
        //{
        //    if (gObj)
        //    {
        //        gObj.SetActive(false);
        //    }
        //}
        if (m_ItemSprit)
        {
            m_ItemSprit.spriteName = "";
        }

        for (int i = 0; i < MaxItemCount; i++)
        {
            if (m_AwardItemNumLable[i])
            {
                m_AwardItemNumLable[i].text = "";
            }
        }
    }

    public void Init(int nKind, int nMissionID, byte yQuality)
    {
        CleanUp();
        m_nMissionID = nMissionID;
        m_yQuality = yQuality;
        m_nKind = nKind;

        SetTipText(nMissionID, yQuality);
        UpdateAwardItem(nMissionID);
        UpadateButtonState(nMissionID);
    }

    void SetTipText(int nMissionID, byte yQuality)
    {
        Tab_MissionBase MissionBase = TableManager.GetMissionBaseByID(nMissionID, 0);
        if (MissionBase == null || MissionBase.MissionType != (int)MISSIONTYPE.MISSION_DAILY)
        {
            return;
        }

        Tab_DailyMission DailyMission = TableManager.GetDailyMissionByID(MissionBase.DalityMissionTabID, 0);
        if (DailyMission == null)
        {
            return;

        }

        Tab_MissionDictionary MisDec = TableManager.GetMissionDictionaryByID(MissionID, 0);
        if (MisDec == null)
        {
            return;
        }

        if (m_NameText && m_QualityColorText && m_ActivenessText && m_MisDec)
        {
            m_NameText.text = DailyMission.Name;
            m_QualityColorText.text = GetStrColorByQuality(yQuality);
            m_ActivenessText.text = "+" + DailyMission.AwardActiveness;
            m_MisDec.text = MisDec.MissionDesc;
        }
        return;
    }

    string GetStrColorByQuality(byte yQuality)
    {
        string strColor = "";
        MISSION_QUALITY quality = (MISSION_QUALITY)yQuality;
        switch (quality)
        {
            case MISSION_QUALITY.MISSION_QUALITY_WHITE:
                strColor = Utils.GetDicByID(1627);
                break;
            case MISSION_QUALITY.MISSION_QUALITY_GREEN:
                strColor = Utils.GetDicByID(1628);
                break;
            case MISSION_QUALITY.MISSION_QUALITY_BLUE:
                strColor = Utils.GetDicByID(1629);
                break;
            case MISSION_QUALITY.MISSION_QUALITY_PURPLE:
                strColor = Utils.GetDicByID(1630);
                break;
            case MISSION_QUALITY.MISSION_QUALITY_ORANGE:
                strColor = Utils.GetDicByID(1631);
                break;
            default:
                strColor = Utils.GetDicByID(1627);
                break;
        }

        return strColor;
    }

    void UpdateAwardItem(int nMissionID)
    {
        if (m_ItemGrid == null)
        {
            return;
        }

        Tab_MissionBase MissionBase = TableManager.GetMissionBaseByID(nMissionID, 0);
        if (MissionBase == null)
        {
            return;
        }
        Tab_DailyMission DailyMissionTab = TableManager.GetDailyMissionByID(MissionBase.DalityMissionTabID, 0);
        if (DailyMissionTab == null)
        {
            return;
        }

        // 品质、等级系数
        float fQualityFactor = 1.0f;
        float fLevelFactor = 1.0f;

        Tab_DailyMissionQuality QualityTab = TableManager.GetDailyMissionQualityByID(m_yQuality, 0);
        if (QualityTab != null)
        {
            fQualityFactor = QualityTab.AwardFactor;
            int nLevelFactorIndex = Singleton<ObjManager>.Instance.MainPlayer.BaseAttr.Level / 10;
            int nLevelFactorCount = QualityTab.getLevelFactorCount();
            if (nLevelFactorIndex >= nLevelFactorCount || nLevelFactorIndex < 0)
            {
                nLevelFactorIndex = nLevelFactorCount - 1;
            }
            if (QualityTab.GetLevelFactorbyIndex(nLevelFactorIndex) >= 0)
            {
                fLevelFactor = QualityTab.GetLevelFactorbyIndex(nLevelFactorIndex);
            }            
        }

        float fAwardFactor = fQualityFactor * fLevelFactor;
        if (fAwardFactor <= 0)
        {
            return;
        }
        
        int nIndex = 0;
        // 金钱
        if (DailyMissionTab.AwardMoney > 0 && nIndex >= 0 && nIndex < MaxItemCount
            && m_AwardItem[nIndex] && nIndex < m_AwardItemNumLable.Length)
        {
            m_AwardItem[nIndex].SetActive(true);
            if (m_AwardItemNumLable[nIndex] != null)
            {
                float fRewardMoney = DailyMissionTab.AwardMoney * fAwardFactor;
                int nRewardMoney = (int)fRewardMoney;
                m_AwardItemNumLable[nIndex].text = nRewardMoney.ToString();
            }
        }

        // 经验
        nIndex++;
        if (DailyMissionTab.AwardExp > 0 && nIndex >= 0 && nIndex < MaxItemCount
            && m_AwardItem[nIndex] && nIndex < m_AwardItemNumLable.Length)
        {
            m_AwardItem[nIndex].SetActive(true);
            if (m_AwardItemNumLable[nIndex] != null)
            {
                float fRewardExp = DailyMissionTab.AwardExp * fAwardFactor;
                int nRewardExp = (int)fRewardExp;
                m_AwardItemNumLable[nIndex].text = nRewardExp.ToString();
            }
        }

        // 声望
        nIndex++;
        if (DailyMissionTab.AwardReputation > 0 && nIndex >= 0 && nIndex < MaxItemCount
            && m_AwardItem[nIndex] && nIndex < m_AwardItemNumLable.Length)
        {
            m_AwardItem[nIndex].SetActive(true);
            if (m_AwardItemNumLable[nIndex] != null)
            {
                int nRewardRepution = DailyMissionTab.AwardReputation;
                m_AwardItemNumLable[nIndex].text = nRewardRepution.ToString();
            }
        }

        // 活跃度
        nIndex++;
        if (DailyMissionTab.AwardActiveness > 0 && nIndex >= 0 && nIndex < MaxItemCount
            && m_AwardItem[nIndex] && nIndex < m_AwardItemNumLable.Length)
        {
            m_AwardItem[nIndex].SetActive(true);
            if (m_AwardItemNumLable[nIndex] != null)
            {
                m_AwardItemNumLable[nIndex].text = DailyMissionTab.AwardActiveness.ToString();
            }
        }

        // 物品
        nIndex++;
        if (DailyMissionTab.AwardItemNum > 0)
        {
            Tab_CommonItem Item = TableManager.GetCommonItemByID(DailyMissionTab.AwardItemID, 0);
            if (Item != null && nIndex >= 0 && nIndex < MaxItemCount
                && m_AwardItem[nIndex] && m_ItemSprit && nIndex < m_AwardItemNumLable.Length)
            {
                m_AwardItem[nIndex].SetActive(true);
                m_ItemSprit.spriteName = Item.Icon;
                if (m_AwardItemNumLable[nIndex] != null)
                {
                    m_AwardItemNumLable[nIndex].text = DailyMissionTab.AwardItemNum.ToString();
                }
            }
        }

        m_ItemGrid.repositionNow = true;
    }

    public void UpadateButtonState(int nMissionID)
    {
        if (m_StateButton == null || m_UpdateButton == null || m_StateText == null)
        {
            return;
        }

        bool bIsHaveMission = GameManager.gameManager.MissionManager.IsHaveMission(nMissionID);
        if (bIsHaveMission == false)
        {
            m_StateButton.isEnabled = true;
            m_UpdateButton.gameObject.SetActive(true);
            m_StateText.text = Utils.GetDicByID(1618);
            gameObject.name = "1" + m_nKind;
            return;
        }

        MissionState state = (MissionState)GameManager.gameManager.MissionManager.GetMissionState(nMissionID);
        switch (state)
        {
            case MissionState.Mission_Accepted:
                m_StateButton.isEnabled = true;
                m_UpdateButton.gameObject.SetActive(false);
                m_StateText.text = Utils.GetDicByID(1032);
                gameObject.name = "1" + m_nKind;
                break;
            case MissionState.Mission_Completed:
                m_StateButton.isEnabled = true;
                m_UpdateButton.gameObject.SetActive(false);
                m_StateText.text = "[1fff1f]" + Utils.GetDicByID(1621);
                gameObject.name = "0" + m_nKind;
                break;
            default:
                m_StateButton.isEnabled = false;
                m_UpdateButton.gameObject.SetActive(true);
                m_StateText.text = Utils.GetDicByID(1618);
                gameObject.name = "1" + m_nKind;
                break;
        }
    }

    void AcceptButtonClick()
    {
        if (Singleton<ObjManager>.Instance.MainPlayer.IsInJianYu())
        {
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2216}");
            return;
        }

        if (m_NewPlayerGuide_Step == 1)
        {
            NewPlayerGuidLogic.CloseWindow();
            m_NewPlayerGuide_Step = -1;
        }

        bool isHaveMission = GameManager.gameManager.MissionManager.IsHaveMission(m_nMissionID);
        if (isHaveMission)
        {
            MissionState state = (MissionState)GameManager.gameManager.MissionManager.GetMissionState(m_nMissionID);
            if (state == MissionState.Mission_Accepted)
            {
                // 放弃任务
                string str = Utils.GetDicByID(1975);
                MessageBoxLogic.OpenOKCancelBox(str, null, OnAbandonMessageOK, OnCancelClick);
            }
            else if (state == MissionState.Mission_Completed)
            {
              //  GameManager.gameManager.SoundManager.PlaySoundEffect(136);  //get_reward
                GameManager.gameManager.MissionManager.CompleteMission(m_nMissionID);
            }
        }
        else
        {
           // GameManager.gameManager.SoundManager.PlaySoundEffect(135);  //yes没有这个声音，先注释掉
            GameManager.gameManager.MissionManager.AcceptMission(m_nMissionID);
        }
    }

    void UpdateButtonClick()
    {
        if (Singleton<ObjManager>.Instance.MainPlayer.IsInJianYu())
        {
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2216}");
            return;
        }

        Tab_MissionBase MissionBase = TableManager.GetMissionBaseByID(m_nMissionID, 0);
        if (MissionBase == null)
        {
            return;
        }
        Tab_DailyMission DailyMission = TableManager.GetDailyMissionByID(MissionBase.DalityMissionTabID, 0);
        if (DailyMission == null)
        {
            return;
        }
        bool bIsHaveMission = GameManager.gameManager.MissionManager.IsHaveMission(m_nMissionID);
        if (bIsHaveMission)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2444}");
            return;
        }
        string str = "";
        str = StrDictionary.GetClientDictionaryString("#{1538}", DailyMission.ConsumeYuanBao);
        MessageBoxLogic.OpenOKCancelBox(str, null, OnMessageOK, OnCancelClick);
    }

    void OnAbandonMessageOK()
    {
        GameManager.gameManager.MissionManager.AbandonMission(m_nMissionID);
    }

    void OnMessageOK()
    {
        GameManager.gameManager.PlayerDataPool.DailyMissionData.AskUpdateDailyMission(m_nKind);
    }

    void OnCancelClick()
    {
        MessageBoxLogic.CloseBox();
    }

    // 新手指引
    private int m_NewPlayerGuide_Step = -1;
    void Check_NewPlayerGuide()
    {
        if (ActivityController.Instance() == null)
        {
            return;
        }
        DailyMissionActiveWindow DailyWindow = ActivityController.Instance().m_DailyMissionActiveWindow;
        if (DailyWindow == null || DailyWindow.gameObject.activeSelf == false)
        {
            return;
        }

        DailyMissionWindow DailyMissionWin = DailyWindow.m_DailyMissionWindow;
        if (DailyMissionWin == null || DailyMissionWin.gameObject.activeSelf == false)
        {
            return;
        }

        if (DailyMissionWin.NewPlayerGuide_Step == 1 && IsMasterMission())
        {
            bool bIsHaveMission = GameManager.gameManager.MissionManager.IsHaveMission(m_nMissionID);
            byte yState = GameManager.gameManager.MissionManager.GetMissionState(m_nMissionID);
            if (bIsHaveMission == true && yState == (byte)MissionState.Mission_Completed)
            {
                NewPlayerGuide(1);
                PlayerPreferenceData.DailyMissionGuideFlag = 1;
            }
            DailyMissionWin.NewPlayerGuide_Step = -1;
        }
        else if (ActivityController.Instance().NewPlayerGuide_Step == 2 && IsMasterMission())
        {
            NewPlayerGuide(1);
            ActivityController.Instance().NewPlayerGuide_Step = -1;
        }
    }

    void NewPlayerGuide(int nIndex)
    {
        m_NewPlayerGuide_Step = nIndex;
        switch (nIndex)
        {
            case 1:
                if (m_StateButton)
                {
                    NewPlayerGuidLogic.OpenWindow(m_StateButton.gameObject, 180, 60, "", "right", 2, true, true);
                }
                break;
        }
    }

    bool IsMasterMission()
    {
        Tab_MissionBase MissionBase = TableManager.GetMissionBaseByID(m_nMissionID, 0);
        if (MissionBase == null)
        {
            return false;
        }

        Tab_DailyMission DailyMission = TableManager.GetDailyMissionByID(MissionBase.DalityMissionTabID, 0);
        if (DailyMission == null)
        {
            return false;
        }

        if (DailyMission.Type == 0)
        {
            return true;
        }

        return false;
    }

    void MissionClick()
    {
        GameManager.gameManager.MissionManager.MissionPathFinder(m_nMissionID);
    }
}

