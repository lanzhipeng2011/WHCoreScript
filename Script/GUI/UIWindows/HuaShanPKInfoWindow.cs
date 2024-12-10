using UnityEngine;
using System.Collections;

public class HuaShanPKInfoWindow : MonoBehaviour {

    public TabController m_HuaShanTabController;
    public HuaShanRoundPKInfoWindow[] m_RoundWindow;

    void OnEnable()
    {
        HuaShanPVPData.delegateShowPkInfo += ShowPKInfoList;
    }

    void OnDisable()
    {
        HuaShanPVPData.delegateShowPkInfo -= ShowPKInfoList;
    }

    public void OnReturnButtonClick()
    {
        if (m_HuaShanTabController != null)
        {
            m_HuaShanTabController.ChangeTab("Tab1");
            HuaShanPVPData.delegateShowPkInfo -= ShowPKInfoList;
        }
    }
   
    public void ShowPKInfoList()
    {
        for (int i = 0; i < m_RoundWindow.Length; i++)
        {
            m_RoundWindow[i].gameObject.SetActive(false);
        }
        if (HuaShanPVPData.Rounder <= m_RoundWindow.Length && HuaShanPVPData.Rounder >= 1)
        {
            m_RoundWindow[HuaShanPVPData.Rounder - 1].gameObject.SetActive(true);
            m_RoundWindow[HuaShanPVPData.Rounder - 1].UpdatePkInfo();
        }
        else
        {
            if (m_RoundWindow.Length >=1)
            {
                m_RoundWindow[m_RoundWindow.Length - 1].gameObject.SetActive(true);        
            }
        }
    }
}
