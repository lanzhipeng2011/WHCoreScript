using UnityEngine;
using System.Collections;
using Module.Log;

public class PKInfoItem : MonoBehaviour {

    public UISprite[] m_MemberNormalBk;
    public UISprite[] m_MemberWinBk;
    public UILabel[] m_MemberNameLable;

    public void ClearInfo()
    {
        for (int i = 0; i < m_MemberNameLable.Length; i++)
        {
            if (null != m_MemberNameLable[i])
            {
                m_MemberNameLable[i].text = "????????";
            }
        }
        for (int i = 0; i < m_MemberNormalBk.Length; i++)
        {
            if (null != m_MemberNormalBk[i])
            {
                m_MemberNormalBk[i].gameObject.SetActive(true);
            } 
        }
         for (int i = 0; i < m_MemberWinBk.Length; i++)
        {
            if (null != m_MemberWinBk[i])
            {
                m_MemberWinBk[i].gameObject.SetActive(false);
            }
        }
    }

    public void SetPKInfo(HuaShanPVPData.MemberPKInfo PKInfo)
    {
        if (PKInfo == null)
        {
            LogModule.ErrorLog("SetPKInfo::PKInfo is null");
            return;
        }

        if (m_MemberNameLable.Length < 2 )
        {
            LogModule.ErrorLog("SetPKInfo::m_MemberNameLable.Length < 2");
            return;
        }
        if (m_MemberNormalBk.Length < 2)
        {
            LogModule.ErrorLog("SetPKInfo::m_MemberNormalBk.Length < 2");
            return;
        }
        if (m_MemberWinBk.Length < 2)
        {
            LogModule.ErrorLog("SetPKInfo::m_MemberWinBk.Length < 2");
            return;
        }
        for (int i = 0; i < m_MemberNameLable.Length; i++)
        {
            if (null == m_MemberNameLable[i])
            {
                LogModule.ErrorLog("SetPKInfo::m_MemberNameLable[i] is null");
                return;
            }
        }
        for (int i = 0; i < m_MemberNormalBk.Length; i++)
        {
            if (null == m_MemberNormalBk[i])
            {
                LogModule.ErrorLog("SetPKInfo::m_MemberNormalBk[i] is null");
                return;
            }
        }
        for (int i = 0; i < m_MemberWinBk.Length; i++)
        {
            if (null == m_MemberWinBk[i])
            {
                LogModule.ErrorLog("SetPKInfo::m_MemberWinBk[i] is null");
                return;
            }
        }
        if (null == PKInfo)
        {
            return;
        }
        m_MemberNameLable[0].text = PKInfo.m_fristname;
        m_MemberNameLable[1].text = PKInfo.m_secondname;
        if ( PKInfo.m_fristname == PKInfo.m_winnername)
        {
            m_MemberNormalBk[0].gameObject.SetActive(false);
            m_MemberWinBk[0].gameObject.SetActive(true);
            m_MemberNormalBk[1].gameObject.SetActive(true);
            m_MemberWinBk[1].gameObject.SetActive(false);
        }
        else
        {
            m_MemberNormalBk[0].gameObject.SetActive(true);
            m_MemberWinBk[0].gameObject.SetActive(false);
            m_MemberNormalBk[1].gameObject.SetActive(false);
            m_MemberWinBk[1].gameObject.SetActive(true);
        }
		//======If there is no winner 
		if(PKInfo.m_fristname == null ||PKInfo.m_fristname == "" || PKInfo.m_fristname == " ")
		{
			m_MemberNormalBk[0].gameObject.SetActive(true);
			m_MemberWinBk[0].gameObject.SetActive(false);
			m_MemberNormalBk[1].gameObject.SetActive(true);
			m_MemberWinBk[1].gameObject.SetActive(false);
		}


    }

}
