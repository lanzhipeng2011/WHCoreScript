using UnityEngine;
using System.Collections;
using System;
using GCGame;

public class MailWindow : MonoBehaviour {
	
    public TabController m_TabController;
	//private bool m_bRecvWindow = true;

    public UILabel BtnLabelRecvBox;
    public UILabel BtnLabelSendBox;
    void Awake()
    {
        BtnLabelRecvBox.text = Utils.GetDicByID(1124);
        BtnLabelSendBox.text = Utils.GetDicByID(1125);
    }

	void OnEnable()
	{
		//m_bRecvWindow = false;
        m_TabController.ChangeTab("BtnRecvBox");
	}
	

    public void ReplayMail(UInt64 receiver, string recevierName)
    {
        GameObject sendWindow = m_TabController.ChangeTab("BtnSendBox");
        if (null != sendWindow && null != sendWindow.GetComponent<MailSendWindow>())
            sendWindow.GetComponent<MailSendWindow>().SetReceiver(receiver, recevierName);
    }

}
