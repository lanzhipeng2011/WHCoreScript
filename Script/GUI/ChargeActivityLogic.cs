using UnityEngine;
using System.Collections;

public class ChargeActivityLogic : MonoBehaviour {

    public TabController m_TabController;
    public TabButton m_ShouChongButton;
    public TabButton m_ShouZhouButton;
    public TabButton m_ShouYueButton;
    public TabButton m_ChengZhangButton;
    public UIGrid m_ButtonGrid;
    public UISprite m_RemainMonthCardTips;
    public UISprite m_RemainGrowUpTips;

    private static ChargeActivityLogic m_Instance = null;
    public static ChargeActivityLogic Instance()
    {
        return m_Instance;
    }

    void Awake()
    {
        m_Instance = this;
        if (m_TabController != null)
        {
            m_TabController.delTabChanged = OnTabChanged;
        }
    }

	// Use this for initialization
	void Start () {
        InitButton();
	}
    
    void InitButton()
    {
        m_ShouChongButton.gameObject.SetActive(false);
        m_ShouZhouButton.gameObject.SetActive(false);
        m_ShouYueButton.gameObject.SetActive(false);
        m_ChengZhangButton.gameObject.SetActive(false);

        PayActivityData payData = GameManager.gameManager.PlayerDataPool.PayActivity;
        int playerLevel = GameManager.gameManager.PlayerDataPool.MainPlayerBaseAttr.Level;
        TabButton m_SelectButton = null;
        //首充
        if (payData.IsFirstTimeFlag() == false)
        {
            m_ShouChongButton.gameObject.SetActive(true);
            if (m_SelectButton == null){ m_SelectButton = m_ShouChongButton; }
        }
        //首周
        if (payData.IsFirstWeekOver() == false && payData.IsFirstTimeFlag() == true)
        {
            m_ShouZhouButton.gameObject.SetActive(true);
            if (m_SelectButton == null) { m_SelectButton = m_ShouZhouButton; }
        }
        //首月
        if (payData.IsMonthCardOver() == false)
        {
            m_ShouYueButton.gameObject.SetActive(true);
            if (m_SelectButton == null) { m_SelectButton = m_ShouYueButton; }
        }
        //成长
        if (payData.IsGrowUpFlag() == true)
        {
            if (payData.IsGrowUpOver() == false)
            {
                m_ChengZhangButton.gameObject.SetActive(true);
                if (m_SelectButton == null) { m_SelectButton = m_ChengZhangButton; }
            }
        }
        else
        {
            if (playerLevel <= 50)
            {
                m_ChengZhangButton.gameObject.SetActive(true);
                if (m_SelectButton == null) { m_SelectButton = m_ChengZhangButton; }
            }
        }
        m_ButtonGrid.repositionNow = true;
        if (null != m_SelectButton)
        {
            m_TabController.OnTabClicked(m_SelectButton);
        }

        if (m_ShouZhouButton.gameObject.activeSelf == false)
        {
            GameManager.gameManager.PlayerDataPool.IsClickChargeActivitySZ = true;
        }
        if (m_ShouYueButton.gameObject.activeSelf == false)
        {
            GameManager.gameManager.PlayerDataPool.IsClickChargeActivitySY = true;
        }
        if (m_ChengZhangButton.gameObject.activeSelf == false)
        {
            GameManager.gameManager.PlayerDataPool.IsClickChargeActivityCZ = true;
        }

        UpdateRemainTips();
    }

    public void UpdateCurTab()
    {
        if (ChargeActivityLogic_SC.Instance())
        {
            ChargeActivityLogic_SC.Instance().InitPrize();
        }
        if (ChargeActivityLogic_SY.Instance())
        {
            ChargeActivityLogic_SY.Instance().InitPrize();
        }
        if (ChargeActivityLogic_CZ.Instance())
        {
            ChargeActivityLogic_CZ.Instance().InitPage();
        }
    }

    void OnClickClose()
    {
        UIManager.CloseUI(UIInfo.ChargeActivity);
    }

    void OnClickCharge()
    {
        RechargeData.PayUI();
    }

    void OnTabChanged(TabButton button)
    {
        if (button.gameObject.name == "Tab1")
        {
            //首充
        }
        else if (button.gameObject.name == "Tab2")
        {
            //首周
            GameManager.gameManager.PlayerDataPool.IsClickChargeActivitySZ = true;
        }
        else if (button.gameObject.name == "Tab3")
        {
            //招财进宝
            GameManager.gameManager.PlayerDataPool.IsClickChargeActivitySY = true;
        }
        else if (button.gameObject.name == "Tab4")
        {
            //成长基金
            GameManager.gameManager.PlayerDataPool.IsClickChargeActivityCZ = true;
        }
        UpdateRemainTips();
    }

    public void UpdateRemainTips()
    {
        if (ChargeActivityLogic_SY.IsMonthCardCanGet() ||
            ChargeActivityLogic_SY.IsNeedClickToday())
        {
            m_RemainMonthCardTips.gameObject.SetActive(true);
        }
        else
        {
            m_RemainMonthCardTips.gameObject.SetActive(false);
        }

        if (ChargeActivityLogic_CZ.IsGrowUpCanGet() ||
            ChargeActivityLogic_CZ.IsNeedClickToday())
        {
            m_RemainGrowUpTips.gameObject.SetActive(true);
        }
        else
        {
            m_RemainGrowUpTips.gameObject.SetActive(false);
        }
    }
}
