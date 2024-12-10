using UnityEngine;
using System.Collections;
using Games.LogicObj;

public class RechargeBarLogic : UIControllerBase<RechargeBarLogic> {

	// Use this for initialization
    public GameObject m_VipButton;
    public GameObject m_ChargeButton;
    public UISprite m_RemainTips;
    public UISprite m_VipTips;

    public void RefreshVIPTips()
    {
        m_VipTips.gameObject.SetActive(GameManager.gameManager.PlayerDataPool.IsShowVipTip);
    }

    private int m_SavedVipCost;
    public void OnVipCostChange(int nVipCost)
    {
        m_VipButton.SetActive(false);
        if (nVipCost >= 0)
        {
            m_VipButton.SetActive(true);

            if (VipData.GetVipLv() > 0)
            {
                GameManager.gameManager.PlayerDataPool.IsShowVipTip = false;
                RefreshVIPTips();
            }
        }
        m_SavedVipCost = nVipCost;        
    }
	void Awake () 
    {
        m_SavedVipCost = -1;
        SetInstance(this);

        UpdateChargeActivity();
        UpdateChargeTip();
        RefreshVIPTips();
	}

    void OnDestroy()
    {
        SetInstance(null);
    }

    public void UpdateChargeActivity()
    {
        if (GameManager.gameManager.PlayerDataPool.IsServerFlagOpen(SERVER_FLAGS_ENUM.FLAG_PAYACT))
        {
            m_ChargeButton.SetActive(true);
        }
        else
        {
            m_ChargeButton.SetActive(false);
        }
    }

    void OnClickChargeActivity()
    {
        if (GameManager.gameManager.PlayerDataPool.IsServerFlagOpen(SERVER_FLAGS_ENUM.FLAG_PAYACT))
        {
            UIManager.ShowUI(UIInfo.ChargeActivity);
        }
    }

    void OnVipClick()
    {
        //if (m_SavedVipCost >= 0 && GameManager.gameManager.PlayerDataPool.IsServerFlagOpen(SERVER_FLAGS_ENUM.FLAG_VIP))
       // {
            UIManager.ShowUI(UIInfo.VipRoot);
       // }     
    }

	void OnShouChongClick()
	{
		UIManager.ShowUI(UIInfo.PackageRoot);
	}

    public void PlayTween(bool nDirection)
    {
        gameObject.SetActive(!nDirection);
    }

    public void UpdateChargeTip()
    {
        if (ChargeActivityLogic_SY.IsMonthCardCanGet() ||
            ChargeActivityLogic_SY.IsNeedClickToday() ||
            ChargeActivityLogic_CZ.IsGrowUpCanGet() ||
            ChargeActivityLogic_CZ.IsNeedClickToday())
        {
            m_RemainTips.gameObject.SetActive(true);
        }
        else
        {
            m_RemainTips.gameObject.SetActive(false);
        }
    }
}
