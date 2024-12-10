using UnityEngine;
using System.Collections;

public class HuaShanRoundPKInfoWindow : MonoBehaviour {

    public PKInfoItem[] m_PKMemberList;
    public PKInfoWinnerItem[] m_PKWinnerList;

    void OnEnable()
    {
        ClearPkInfo();
    }

    void ClearPkInfo()
    {
        for (int i = 0; i < m_PKMemberList.Length; i++)
        {
            m_PKMemberList[i].ClearInfo();
        }
        for (int i = 0; i < m_PKWinnerList.Length; i++)
        {
            m_PKWinnerList[i].ClearInfo();
        }
    }

    public void UpdatePkInfo()
    {
        for (int i = 0; i < HuaShanPVPData.HuaShanPKInfoList.Count; ++i)
        {
            if (i < m_PKMemberList.Length && m_PKMemberList[i] != null)
            {
                m_PKMemberList[i].SetPKInfo(HuaShanPVPData.HuaShanPKInfoList[i]);
            }
            if (i < m_PKWinnerList.Length && m_PKWinnerList[i] != null)
            {
                m_PKWinnerList[i].SetPKInfo(HuaShanPVPData.HuaShanPKInfoList[i]);
            }
        }
    }
}
