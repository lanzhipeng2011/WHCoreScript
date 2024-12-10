using UnityEngine;
using System.Collections;
using GCGame.Table;
using GCGame;

public class MoneyTreeLogic : UIControllerBase<MoneyTreeLogic>
{
    //public const int MaxAwardCount = 20; // 最大领取次数
    public const int MoneyTreeFreeAwardTimes = 8;   // 免费领取次数
    public const int MoneyTreeYuanBaoAwardTimes = 12;   // 元宝领取次数

    public UILabel m_TimerText;
    public UILabel m_CountTipText;
    public UILabel m_YuaBaoAwardCountTipText;

    public UILabel m_LingQuTip;
    public UILabel m_CountFreeText;
    public UILabel m_Right_Tip;

    public UIInputNumber m_AwardCountInput;

    public UIImageButton m_FreeAwardButton;
    public UIImageButton m_YuanBaoButton;

    private int m_CDTime;
    public int CDTime
    {
        get { return m_CDTime; }
        set {
            m_CDTime = value;
            UpdateTimerText();
        }
    }

    private int m_CurMoneyTreeID;
    public int CurMoneyTreeID
    {
        get { return m_CurMoneyTreeID; }
        set {
            m_CurMoneyTreeID = value;
            UpdateCountTip(m_CurMoneyTreeID);
        }

    }

    private int m_YuanBaoAwardCount;
    public int YuanBaoAwardCount
    {
        get { return m_YuanBaoAwardCount; }
        set {
            m_YuanBaoAwardCount = value;
            UpdateYuanBaoCountTip(m_YuanBaoAwardCount);
        }
    }

    void OnAwake()
    {
        SetInstance(this);
    }

	// Use this for initialization
	void Start () {
    }

    void OnEnable()
    {
        SetInstance(this);
        CleanUp();
        initWindow();
    }

    void OnDestroy()
    {
        SetInstance(null);
    }

    void initWindow()
    {
        CDTime = GameManager.gameManager.PlayerDataPool.MoneyTreeData.CDTime;
        CurMoneyTreeID = GameManager.gameManager.PlayerDataPool.MoneyTreeData.MoneyTreeID;
        YuanBaoAwardCount = GameManager.gameManager.PlayerDataPool.MoneyTreeData.YuanBaoAwardCount;

        if (m_LingQuTip && m_CountFreeText && m_Right_Tip)
        {
            m_LingQuTip.text = Utils.GetDicByID(1518);
            m_CountFreeText.text = Utils.GetDicByID(1520);
            m_Right_Tip.text = Utils.GetDicByID(1521);
        }

        if (CDTime <= 0 && m_CurMoneyTreeID < MoneyTreeFreeAwardTimes)
        {
            m_FreeAwardButton.isEnabled = true;
        }

        if (m_YuanBaoAwardCount < MoneyTreeYuanBaoAwardTimes)
        {
            m_YuanBaoButton.isEnabled = true;
        }

    }
    void CleanUp()
    {
        if (m_AwardCountInput)
        {
            m_AwardCountInput.value = "1";
        }

        CDTime = 0;
        m_TimerText.text = "";
        m_CountTipText.text = "";
        m_YuaBaoAwardCountTipText.text = "";

        if (m_FreeAwardButton && m_YuanBaoButton)
        {
            m_FreeAwardButton.isEnabled = false;
            m_YuanBaoButton.isEnabled = false;
        }
    }


    void CloseWindow()
    {
        UIManager.CloseUI(UIInfo.MoneyTreeRoot);
    }

    //void ButtonOKClick()
    //{
    //    if (m_AwardCountInput)
    //    {
    //        if (m_AwardCountInput.value == "")
    //        {
    //            return;
    //        }

    //        int nAwardCount = 0;
    //        bool nRet = int.TryParse(m_AwardCountInput.value, out nAwardCount);
    //        if (nRet)
    //        {
    //            if (nAwardCount == 1 && m_CDTime <= 0 && m_CurMoneyTreeID < MoneyTreeFreeAwardTimes)
    //            {
    //                OnOKClick();
    //            }
    //            else
    //            {
    //                int nMaxCount = TableManager.GetMoneyTree().Count;
    //                if (nAwardCount > nMaxCount - m_CurMoneyTreeID)
    //                {
    //                    nAwardCount = nMaxCount - m_CurMoneyTreeID;
    //                }

    //                int nNeedYuanbao  = GetNeedYuanBaoByAwardCount(nAwardCount);
    //                string str = "";
    //                str = StrDictionary.GetClientDictionaryString("#{1524}", nAwardCount, nNeedYuanbao);
    //                MessageBoxLogic.OpenOKCancelBox(str, null, OnOKClick, OnCancelClick);
    //            }
    //        }
    //    }
        
    //  }

    void ButtonOKClick(GameObject obj)
    {
        if (obj.name == "Button-OK")
        {
            if (m_CurMoneyTreeID < MoneyTreeFreeAwardTimes)
            {
                //OnOKClick();
                if (m_CDTime > 0)
                {
                    Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2478}");
                }
                else
                {
                    GameManager.gameManager.PlayerDataPool.MoneyTreeData.SendAwardPacket(1, 1);
                }
            }
        }
        else if (obj.name == "Button-paste")
        {
//             int nMaxCount = TableManager.GetMoneyTree().Count;
//             if (nAwardCount > nMaxCount - m_CurMoneyTreeID)
//             {
//                 nAwardCount = nMaxCount - m_CurMoneyTreeID;
//             }
            if (m_YuanBaoAwardCount >= MoneyTreeYuanBaoAwardTimes)
            {
                return;
            }

            int nNeedYuanbao = GetNeedYuanBaoByAwardCount(1);
            int nAwardCoin = GetAwardMoneyByYuanBaoAwardCount(1);
            string str = "";
            str = StrDictionary.GetClientDictionaryString("#{2718}", nNeedYuanbao, nAwardCoin);
            MessageBoxLogic.OpenOKCancelBox(str, null, OnOKClick, OnCancelClick);
        }
    }

    void ButtonCancelClick()
    {
        CloseWindow();
    }

    void OnOKClick()
    {
//         if (m_AwardCountInput)
//         {
//             string strAwardCount = m_AwardCountInput.value;
//             int nAwardCount = 0;
//             if (int.TryParse(strAwardCount, out nAwardCount))
//             {
//                 GameManager.gameManager.PlayerDataPool.MoneyTreeData.SendAwardPacket(nAwardCount);
//             }
//         }
        GameManager.gameManager.PlayerDataPool.MoneyTreeData.SendAwardPacket(1, 2);
    }

    void OnCancelClick()
    {
        MessageBoxLogic.CloseBox();
    }

    void ButtonUpClick()
    {
        int nCount = 0;
        bool nRet = int.TryParse(m_AwardCountInput.value, out nCount);
        int nMaxCount = TableManager.GetMoneyTree().Count;
        if (nRet == true && nCount >= 0 && nCount < nMaxCount - m_CurMoneyTreeID && m_AwardCountInput)
        {
            m_AwardCountInput.value = (nCount + 1).ToString();
        }
        else
        {
            m_AwardCountInput.value = (nMaxCount - m_CurMoneyTreeID).ToString();
        }

    }

    void ButtonDownClick()
    {
        int nCount = 0;
        bool nRet = int.TryParse(m_AwardCountInput.value, out nCount);
        int nMaxCount = TableManager.GetMoneyTree().Count;
        if (nRet == true && nCount > 1 && nCount <= nMaxCount - m_CurMoneyTreeID && m_AwardCountInput)
        {
            m_AwardCountInput.value = (nCount - 1).ToString();
        }
        else
        {
            m_AwardCountInput.value = "1";
        }
    }

    void UpdateTimerText()
    {
        if (m_CDTime <= 0)
        {
            m_TimerText.text = "";
            if (m_FreeAwardButton && m_FreeAwardButton.isEnabled == false)
            {
                m_FreeAwardButton.isEnabled = true;
            }
            return;
        }

        int nMin = (m_CDTime % (60 * 60)) / 60;
        int nSec = m_CDTime % 60;
        if (m_TimerText)
        {
            m_TimerText.text = ""+ nMin / 10 + nMin % 10 + ":" + nSec / 10 + nSec % 10;
        }

        if (m_FreeAwardButton && m_FreeAwardButton.isEnabled)
        {
            m_FreeAwardButton.isEnabled = false;
        }
    }

    int GetNeedYuanBaoByAwardCount(int nAwardCount)
    {
        int nNeedYuanBao = 0;
        if (m_YuanBaoAwardCount >= 0 && m_YuanBaoAwardCount < MoneyTreeYuanBaoAwardTimes)
        {
            for (int i = 0; i < nAwardCount; i++)
            {
                Tab_MoneyTree MoneyTree = TableManager.GetMoneyTreeByID(MoneyTreeFreeAwardTimes + m_YuanBaoAwardCount + i, 0);
                if (MoneyTree != null)
                {
                    nNeedYuanBao += MoneyTree.BindYuanbao;
                }
            }
        }

        return nNeedYuanBao;
    }

    int GetAwardMoneyByYuanBaoAwardCount(int nAwardCount)
    {
        int nAwardCoin = 0;
        if (m_YuanBaoAwardCount >= 0 && m_YuanBaoAwardCount < MoneyTreeYuanBaoAwardTimes)
        {
            for (int i = 0; i < nAwardCount; i++)
            {
                Tab_MoneyTree MoneyTree = TableManager.GetMoneyTreeByID((MoneyTreeFreeAwardTimes + m_YuanBaoAwardCount + i), 0);
                if (MoneyTree != null)
                {
                    nAwardCoin += MoneyTree.Money;
                }
            }
        }

        return nAwardCoin;
    }

    private void UpdateCountTip(int nMoneyTreeID)
    {
        if (m_CountTipText)
        {
            m_CountTipText.text = StrDictionary.GetClientDictionaryString("#{2479}", MoneyTreeFreeAwardTimes-nMoneyTreeID, MoneyTreeFreeAwardTimes);
        }

        if (nMoneyTreeID >= MoneyTreeFreeAwardTimes)
        {
            m_FreeAwardButton.isEnabled = false;
        }
    }

    private void UpdateYuanBaoCountTip(int nCount)
    {
        if (m_YuaBaoAwardCountTipText)
        {
            m_YuaBaoAwardCountTipText.text = StrDictionary.GetClientDictionaryString("#{2480}", MoneyTreeYuanBaoAwardTimes- nCount, MoneyTreeYuanBaoAwardTimes);
        }

        if (nCount >= MoneyTreeYuanBaoAwardTimes)
        {
            m_YuanBaoButton.isEnabled = false;
        }
    }
}
