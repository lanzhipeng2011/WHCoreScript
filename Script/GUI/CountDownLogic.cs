using System;
using UnityEngine;
using System.Collections;
using GCGame.Table;
using Games.GlobeDefine;

public class CountDownLogic : UIControllerBase<CountDownLogic>
{
    public string[] SpriteName = {"0","1", "2", "3", "4","5"};

    static public  int Countdown{ get; set; }
    private float m_CurTime = 0;

    void Awake()
    {
        UIControllerBase<CountDownLogic>.SetInstance(this);
    }
	// Use this for initialization
	void Start () {
        Countdown = 20;//倒计时现实正确
	}
	
	// Update is called once per frame
	void Update () {
        if (Countdown > 5)
        {
            m_CurTime += Time.deltaTime * 1000;
            if (Countdown > 5 && m_CurTime > 1000)
            {
                m_CurTime = 0;
                Countdown = Countdown - 1;
                if (Countdown % 5 == 0)
                {
                    string notice = StrDictionary.GetClientDictionaryString("#{5626}", Countdown);
                    GUIData.AddNotifyData(notice, false);
                }
            }
        }
        else if (Countdown > 0)
        {
            m_CurTime += Time.deltaTime * 1000;
            if( Countdown > 0 && m_CurTime > 1000)
            {
                m_CurTime = 0;
                Countdown = Countdown - 1;
                if (Countdown > 0)
                {
                    string notice = StrDictionary.GetClientDictionaryString("#{5626}", Countdown);
                    GUIData.AddNotifyData(notice, false);
                }
                else
                {
                    UIManager.CloseUI(UIInfo.CountDown);
                }
            }
        }

        //if (Countdown > 0)
        //{
        //    m_CurTime += Time.deltaTime * 1000;
        //    if (Countdown > 0 && m_CurTime > 1000)
        //    {
        //        m_CurTime = 0;
        //        Countdown = Countdown - 1;
        //        if (Countdown > 0)
        //        {
        //            SpriteNumber.spriteName = SpriteName[Countdown];
        //        }
        //        else
        //        {
        //            UIManager.CloseUI(UIInfo.CountDown);
        //        }
        //    }
        //}
	}

    static public void ShowCountDown( int sec )
    {
        Countdown = sec;
        string notice = StrDictionary.GetClientDictionaryString("#{5626}", Countdown);
        GUIData.AddNotifyData(notice, false);
        UIManager.ShowUI(UIInfo.CountDown);
        // 护送任务，张飞任务，出副本提示
        if ( !PlayerPreferenceData.NewPlayerGuideClose
            && (GameManager.gameManager.MissionManager.IsHaveMission(12)
            || GameManager.gameManager.MissionManager.IsHaveMission(21)))
        {
			FunctionButtonLogic.Instance().NewPlayerGuide((int)GameDefine_Globe.NEWOLAYERGUIDE.EXITDUNGEON);
        }
    }
}
