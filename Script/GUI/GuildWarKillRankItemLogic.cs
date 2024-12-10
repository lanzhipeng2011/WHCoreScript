using System;
using GCGame.Table;
using UnityEngine;
using System.Collections;

public class GuildWarKillRankItemLogic : MonoBehaviour
{

    public UILabel m_SortNumLable;
    public UILabel m_KillerNameLable;
    public UILabel m_KillNumLable;

    public void CleanUp(int nSortIndex)
    {
        m_SortNumLable.text = StrDictionary.GetClientDictionaryString("#{2494}", nSortIndex + 1);
        m_KillerNameLable.text = "???";
        m_KillNumLable.text = "??";
        gameObject.SetActive(true);
    }
    public void InitInfo(GuildWarKillRank info)
    {
        m_SortNumLable.text = StrDictionary.GetClientDictionaryString("#{2494}", info.SortNum); 
        m_KillerNameLable.text = info.KillerName;
        m_KillNumLable.text = info.KillerNum.ToString();
        gameObject.SetActive(true);
    }
}
