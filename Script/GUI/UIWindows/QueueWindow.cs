/********************************************************************
	created:	2013/12/25
	created:	25:12:2013   10:49
	filename: 	QueueWindow.cs
	author:		王迪
	
	purpose:	排队界面
*********************************************************************/

using UnityEngine;
using System.Collections;
using GCGame.Table;

public class QueueWindow : UIControllerBase<QueueWindow> {

    public UILabel labelTip;
    public GameObject FirstChild;
    enum ButtonState
    {
        STATE_UNCLICK,
        STATE_COUNT,
        STATE_TIMEOUT,
    };

    
    private float m_updateTimer = 3.0f;          // 更新定时器
	void Awake()
    {
        SetInstance(this);
        m_updateTimer = 3.0f;
	}

    void Start()
    {
        
        UpdateQueueInfo();
    }
	
	void Update ()
    {
        if (m_updateTimer > 0)
        {
            m_updateTimer -= Time.deltaTime;
            if (m_updateTimer <= 0)
            {
                if (LoginData.m_curQueueState == GC_LOGIN_QUEUE_STATUS.QUEUESTATUS.QUEUING)
                {
                    FirstChild.SetActive(true);
                }
                else
                {
                    UIManager.CloseUI(UIInfo.QueueWindow);
                }
            }
        }
	}

    public void UpdateQueueInfo()
    {
        if (LoginData.m_curQueueNum >= LoginData.QueueDefaultNum)
        {
            labelTip.text = StrDictionary.GetClientDictionaryString("3285");
        }
        else
        {
            labelTip.text = StrDictionary.GetClientDictionaryString("#{2172}", LoginData.m_curQueueNum + 1);
        }
        
    }

}
