using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GCGame;

public class PowerRemindLogic : MonoBehaviour {

    private static PowerRemindLogic m_Instance = null;
    public static PowerRemindLogic Instance()
    {
        return m_Instance;
    }

    public GameObject m_PowerRemind;
    public UILabel m_NewPowerLabel;
    public UILabel m_AddPowerLabel;

    private int m_nBufferNewPower = 0;
    private int m_nBufferAddPower = 0;

    bool m_bOnShow = false;
    float m_fStartShowTime = 0.0f;
    const float ShowTime = 5.0f;
    bool m_bOnBet = false;
    const float BeginBetTime = 1.0f;
    const int PowerMaxLength = 8;
    float[] BetAniTime = new float[PowerMaxLength]{1.5f, 1.6f, 1.7f, 1.8f, 1.9f, 2.0f, 2.1f, 2.2f};
    int[] AniUpdate = new int[PowerMaxLength] { 1, 2, 3, 1, 2, 3, 2, 3 };
    int powerAni = 0;
    int aniNum = 0;
    bool aniStop = false;

    void Awake()
    {
        m_Instance = this;
    }

	// Use this for initialization
	void Start () {
        //m_PowerRemind.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        ShowRemind();
        PlayBetAni();
	}

    void OnDestroy()
    {
        m_Instance = this;
    }

    public static void InitPowerInfo(int nCombatValue, int nAddCombatValue)
    {
        List<object> initParams = new List<object>();
        initParams.Add(nCombatValue);
        initParams.Add(nAddCombatValue);
        UIManager.ShowUI(UIInfo.PowerRemindRoot, PowerRemindLogic.ShowUIOver, initParams);
    }

    static void ShowUIOver(bool bSuccess, object param)
    {
        if (bSuccess)
        {
            List<object> initParams = param as List<object>;
            if (PowerRemindLogic.Instance() != null)
            {
                PowerRemindLogic.Instance().Init((int)initParams[0], (int)initParams[1]);
            }
        }
    }

    void Init(int nCombatValue, int nAddCombatValue)
    {
        m_nBufferNewPower = nCombatValue;
        m_nBufferAddPower = nAddCombatValue;

        m_bOnShow = true;
        m_fStartShowTime = Time.fixedTime;
        powerAni = 0;
    }

    void ShowRemind()
    {
        if (m_bOnShow)
        {
            m_NewPowerLabel.text = (m_nBufferNewPower - m_nBufferAddPower).ToString();
            m_AddPowerLabel.text = m_nBufferAddPower.ToString();

            if (Time.fixedTime - m_fStartShowTime >= ShowTime)
            {
                m_bOnShow = false;
                m_bOnBet = false;
                aniNum = 0;
                aniStop = false;
                UIManager.CloseUI(UIInfo.PowerRemindRoot);
            }
            else if (!m_bOnBet && Time.fixedTime - m_fStartShowTime >= BeginBetTime)
            {
                m_bOnBet = true;
            }
            
        }
    }

    void PlayBetAni()
    {
        if (m_bOnBet)
        {
            for (int i = PowerMaxLength - 1; i >= 0; i--)
            {
                if (Time.fixedTime - m_fStartShowTime >= BetAniTime[i])
                {
                    if (m_nBufferNewPower >= Mathf.Pow(10, i))
                    {
                        UpdatePowerLabel(i);
                        if (i == Mathf.FloorToInt(Mathf.Log10(m_nBufferNewPower)))
                        {
                            if (!aniStop)
                            {
                                aniStop = true;
                                PowerRemindAniStop();
                            }
                        }
                        break;
                    }                   
                }
                else if (i == 0)
                {
                    UpdatePowerLabel(-1);
                } 
            }
        }
    }

    void UpdatePowerLabel(int stop)
    {
        aniNum += 1;
        if (aniNum > 6)
        {
            aniNum = 1;
        }

        for (int i = PowerMaxLength - 1; i >= 0; i-- )
        {
            if (m_nBufferNewPower >= Mathf.Pow(10, i))
            {
                if (stop < i)
                {
                    int number = Utils.GetIntNumber(powerAni, i, 1);
                    Utils.SetIntNumber(ref powerAni, i, 1, (number + (aniNum % AniUpdate[i] == 0 ? 1 : 0)) % 10);
                }
                else
                {
                    int number = Utils.GetIntNumber(m_nBufferNewPower, i, 1);
                    Utils.SetIntNumber(ref powerAni, i, 1, number);
                }
            }
        }
        m_NewPowerLabel.text = GetPowerAniStr();
    }

    string GetPowerAniStr()
    {
        int length = Mathf.FloorToInt(Mathf.Log10(m_nBufferNewPower));
        int lengthAni = Mathf.FloorToInt(Mathf.Log10(powerAni));
        string result = "";
        for (int i = 0; i < length - lengthAni; i++ )
        {
            result += "0";
        }
        result += powerAni.ToString();
        return result;
    }

    void PowerRemindAniStop()
    {
        m_NewPowerLabel.gameObject.GetComponent<Animation>().Play("PowerRemind");
    }
}