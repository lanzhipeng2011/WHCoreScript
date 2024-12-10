using UnityEngine;
using System.Collections;
using GCGame.Table;

public class ChargeActivityLogic_CZ : MonoBehaviour {

    public GameObject m_SubPageHov;     //可领奖分页
    public GameObject m_SubPageNor;     //不可领奖分页

    public UIImageButton m_GetPrize40;  //40级领奖按钮
    public UIImageButton m_GetPrize50;  //50级领奖按钮
    public UIImageButton m_GetPrize60;  //60级领奖按钮
    public UIImageButton m_GetPrize70;  //70级领奖按钮
    public UIImageButton m_GetPrize80;  //80级领奖按钮
    public UIImageButton m_GetPrize90;  //90级领奖按钮

    public UILabel m_PrizeLabel40;  //40级领奖按钮
    public UILabel m_PrizeLabel50;  //50级领奖按钮
    public UILabel m_PrizeLabel60;  //60级领奖按钮
    public UILabel m_PrizeLabel70;  //70级领奖按钮
    public UILabel m_PrizeLabel80;  //80级领奖按钮
    public UILabel m_PrizeLabel90;  //90级领奖按钮

    private static ChargeActivityLogic_CZ m_Instance = null;
    public static ChargeActivityLogic_CZ Instance()
    {
        return m_Instance;
    }

    public static bool IsGrowUpCanGet()
    {
        PayActivityData payData = GameManager.gameManager.PlayerDataPool.PayActivity;
        if (Singleton<ObjManager>.GetInstance().MainPlayer == null)
        {
            return false;
        }
        int playerLevel = Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.Level;
        if (payData.IsGrowUpFlag() == true)
        {
            //40级按钮
            if (payData.IsGrowUp40Flag() == false)
            {
                if (playerLevel >= 40)
                {
                    return true;
                }
            }
            //50级按钮
            if (payData.IsGrowUp50Flag() == false)
            {
                if (playerLevel >= 50)
                {
                    return true;
                }
            }
            //60级按钮
            if (payData.IsGrowUp60Flag() == false)
            {
                if (playerLevel >= 60)
                {
                    return true;
                }
            }
            //70级按钮
            if (payData.IsGrowUp70Flag() == false)
            {
                if (playerLevel >= 70)
                {
                    return true;
                }
            }
            //80级按钮
            if (payData.IsGrowUp80Flag() == false)
            {
                if (playerLevel >= 80)
                {
                    return true;
                }
            }
            //90级按钮
            if (payData.IsGrowUp90Flag() == false)
            {
                if (playerLevel >= 90)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static bool IsNeedClickToday()
    {
        //未购买成长基金 && 当天首次登陆后未点击
        PlayerData playerData = GameManager.gameManager.PlayerDataPool;
        if (playerData.PayActivity.IsGrowUpFlag() == false && playerData.IsClickChargeActivityCZ == false)
        {
            return true;
        }
        return false;
    }

    void Awake()
    {
        m_Instance = this;
    }

	// Use this for initialization
	void Start () {
        InitPage();
	}

    public void InitPage()
    {
        PayActivityData payData = GameManager.gameManager.PlayerDataPool.PayActivity;
        if (payData.IsGrowUpFlag())
        {
            m_SubPageHov.gameObject.SetActive(true);
            m_SubPageNor.gameObject.SetActive(false);
            InitPrizeButton();
        }
        else
        {
            m_SubPageHov.gameObject.SetActive(false);
            m_SubPageNor.gameObject.SetActive(true);
        }
    }

    void InitPrizeButton()
    {
        PayActivityData payData = GameManager.gameManager.PlayerDataPool.PayActivity;
        int playerLevel = Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.Level;
        //40级按钮
        if (payData.IsGrowUp40Flag() == false)
        {
            if (playerLevel >= 40)
            {
                m_GetPrize40.isEnabled = true;
                m_PrizeLabel40.text = StrDictionary.GetClientDictionaryString("#{1378}"); //领取
            }
            else
            {
                m_GetPrize40.isEnabled = false;
                m_PrizeLabel40.text = StrDictionary.GetClientDictionaryString("#{1379}"); //未领取
            }
        }
        else
        {
            m_GetPrize40.isEnabled = false;
            m_PrizeLabel40.text = StrDictionary.GetClientDictionaryString("#{1380}"); //已领取
        }
        //50级按钮
        if (payData.IsGrowUp50Flag() == false)
        {
            if (playerLevel >= 50)
            {
                m_GetPrize50.isEnabled = true;
                m_PrizeLabel50.text = StrDictionary.GetClientDictionaryString("#{1378}"); //领取
            }
            else
            {
                m_GetPrize50.isEnabled = false;
                m_PrizeLabel50.text = StrDictionary.GetClientDictionaryString("#{1379}"); //未领取
            }
        }
        else
        {
            m_GetPrize50.isEnabled = false;
            m_PrizeLabel50.text = StrDictionary.GetClientDictionaryString("#{1380}"); //已领取
        }
        //60级按钮
        if (payData.IsGrowUp60Flag() == false)
        {
            if (playerLevel >= 60)
            {
                m_GetPrize60.isEnabled = true;
                m_PrizeLabel60.text = StrDictionary.GetClientDictionaryString("#{1378}"); //领取
            }
            else
            {
                m_GetPrize60.isEnabled = false;
                m_PrizeLabel60.text = StrDictionary.GetClientDictionaryString("#{1379}"); //未领取
            }
        }
        else
        {
            m_GetPrize60.isEnabled = false;
            m_PrizeLabel60.text = StrDictionary.GetClientDictionaryString("#{1380}"); //已领取
        }
        //70级按钮
        if (payData.IsGrowUp70Flag() == false)
        {
            if (playerLevel >= 70)
            {
                m_GetPrize70.isEnabled = true;
                m_PrizeLabel70.text = StrDictionary.GetClientDictionaryString("#{1378}"); //领取
            }
            else
            {
                m_GetPrize70.isEnabled = false;
                m_PrizeLabel70.text = StrDictionary.GetClientDictionaryString("#{1379}"); //未领取
            }
        }
        else
        {
            m_GetPrize70.isEnabled = false;
            m_PrizeLabel70.text = StrDictionary.GetClientDictionaryString("#{1380}"); //已领取
        }
        //80级按钮
        if (payData.IsGrowUp80Flag() == false)
        {
            if (playerLevel >= 80)
            {
                m_GetPrize80.isEnabled = true;
                m_PrizeLabel80.text = StrDictionary.GetClientDictionaryString("#{1378}"); //领取
            }
            else
            {
                m_GetPrize80.isEnabled = false;
                m_PrizeLabel80.text = StrDictionary.GetClientDictionaryString("#{1379}"); //未领取
            }
        }
        else
        {
            m_GetPrize80.isEnabled = false;
            m_PrizeLabel80.text = StrDictionary.GetClientDictionaryString("#{1380}"); //已领取
        }
        //90级按钮
        if (payData.IsGrowUp90Flag() == false)
        {
            if (playerLevel >= 90)
            {
                m_GetPrize90.isEnabled = true;
                m_PrizeLabel90.text = StrDictionary.GetClientDictionaryString("#{1378}"); //领取
            }
            else
            {
                m_GetPrize90.isEnabled = false;
                m_PrizeLabel90.text = StrDictionary.GetClientDictionaryString("#{1379}"); //未领取
            }
        }
        else
        {
            m_GetPrize90.isEnabled = false;
            m_PrizeLabel90.text = StrDictionary.GetClientDictionaryString("#{1380}"); //已领取
        }
    }

    void OnClickGet(int level)
    {
        PayActivityData payData = GameManager.gameManager.PlayerDataPool.PayActivity;
        payData.SendGrowUpPacket(level);
    }

    void OnClickButton40()
    {
        OnClickGet(40);
    }
    void OnClickButton50()
    {
        OnClickGet(50);
    }
    void OnClickButton60()
    {
        OnClickGet(60);
    }
    void OnClickButton70()
    {
        OnClickGet(70);
    }
    void OnClickButton80()
    {
        OnClickGet(80);
    }
    void OnClickButton90()
    {
        OnClickGet(90);
    }

}
