using UnityEngine;
using System.Collections;
using Module.Log;
public class PKInfoWinnerItem : MonoBehaviour {

    public UISprite m_WinnerNormalBk;
    public UISprite m_WinnerWinBk;
    public UILabel m_WinnerNameLable;

    public void ClearInfo()
    {
        if (m_WinnerNormalBk != null)
        {
            m_WinnerNormalBk.gameObject.SetActive(true);
        }
        if (m_WinnerWinBk != null)
        {
            m_WinnerWinBk.gameObject.SetActive(false);
        }
        if (m_WinnerNameLable != null)
        {
            m_WinnerNameLable.text = "";
        }
    }

    public void SetPKInfo(HuaShanPVPData.MemberPKInfo PKInfo)
    {
        if (null == PKInfo)
        {
            LogModule.ErrorLog("SetPKInfo::PKInfo is null");
            return;
        }
        if (PKInfo.m_winnername != "")
        {
            m_WinnerNameLable.text = PKInfo.m_winnername;
            if (m_WinnerNormalBk != null)
            {
                m_WinnerNormalBk.gameObject.SetActive(false);
            }
            if (m_WinnerWinBk != null)
            {
                m_WinnerWinBk.gameObject.SetActive(true);
            }
        }
        else
        {
            m_WinnerNameLable.text = "??????";
            if (m_WinnerNormalBk != null)
            {
                m_WinnerNormalBk.gameObject.SetActive(true);
            }
            if (m_WinnerWinBk != null)
            {
                m_WinnerWinBk.gameObject.SetActive(false);
            }
        }
    }

}
