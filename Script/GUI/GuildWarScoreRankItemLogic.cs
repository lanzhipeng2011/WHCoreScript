using GCGame.Table;
using UnityEngine;
using System.Collections;

public class GuildWarScoreRankItemLogic : MonoBehaviour
{

    public UILabel m_SortNumLable;
    public UILabel m_GuildNameLable;
    public UILabel m_GuildScoreLable;
    // Use this for initialization
	void Start () 
    {
	
	}
    public void CleanUp(int nSortIndex)
    {
        m_SortNumLable.text = StrDictionary.GetClientDictionaryString("#{2494}", nSortIndex + 1);
        m_GuildNameLable.text = "???";
        m_GuildScoreLable.text = "??";
        gameObject.SetActive(true);
    }

    public void InitInfo(GuildWarPreliminaryRank info)
    {
        m_SortNumLable.text = StrDictionary.GetClientDictionaryString("#{2494}", info.SortNum); 
        m_GuildNameLable.text = info.GuildName;
        m_GuildScoreLable.text = info.Score.ToString();
        gameObject.SetActive(true);
    }
}
