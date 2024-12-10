using UnityEngine;
using System.Collections;

public class ChargeActivityLogic_SY : MonoBehaviour {

    public UISlider m_ProcessBar;
    public UILabel m_TotalYBLabel;
    public UIImageButton m_GetButton;
    public UISprite m_RemainTips;

    private static ChargeActivityLogic_SY m_Instance = null;
    public static ChargeActivityLogic_SY Instance()
    {
        return m_Instance;
    }

    void Awake()
    {
        m_Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        InitPrize();
    }

    public static bool IsMonthCardCanGet()
    {
        PayActivityData payData = GameManager.gameManager.PlayerDataPool.PayActivity;
        if (payData.IsMonthCardFlag() == true &&        //已经触发月卡
            payData.IsMonthCardOver() == false &&       //月卡未领取结束
            payData.IsMonthCardTodayFlag() == false)    //今日未领取月卡
        {
            return true;
        }
        return false;
    }

    public static bool IsNeedClickToday()
    {
        //未购买月卡 && 当天首次登陆后未点击
        PlayerData playerData = GameManager.gameManager.PlayerDataPool;
        if (playerData.PayActivity.IsMonthCardFlag() == false && playerData.IsClickChargeActivitySY == false)
        {
            return true;
        }
        return false;
    }

    public void InitPrize()
    {
        PayActivityData payData = GameManager.gameManager.PlayerDataPool.PayActivity;
        int totalYB = payData.MonthCardYBTotal;
        if (totalYB == 0)
        {
            m_ProcessBar.value = 0;
        }
        else
        {
            m_ProcessBar.value = (float)totalYB / (30 * 50);
        }
        m_TotalYBLabel.text = string.Format("累计领取：{0}元宝", totalYB);

        if (payData.IsMonthCardFlag() == true &&        //已经触发月卡
            payData.IsMonthCardOver() == false &&       //月卡未领取结束
            payData.IsMonthCardTodayFlag() == false)    //今日未领取月卡
        {
            m_GetButton.isEnabled = true;
            m_RemainTips.gameObject.SetActive(true);
        }
        else
        {
            m_GetButton.isEnabled = false;
            m_RemainTips.gameObject.SetActive(false);
        }

        if (ChargeActivityLogic.Instance() != null)
        {
            ChargeActivityLogic.Instance().UpdateRemainTips();
        }
    }

    void OnClickGet()
    {
        PayActivityData payData = GameManager.gameManager.PlayerDataPool.PayActivity;
        if (payData.IsMonthCardFlag() == false)
        {
            return;
        }

        if (payData.IsMonthCardOver() == true)
        {
            return;
        }

        if (payData.IsMonthCardTodayFlag() == true)
        {
            return;
        }
        //发包领奖
        payData.SendMonthCardPacket();
    }
}
