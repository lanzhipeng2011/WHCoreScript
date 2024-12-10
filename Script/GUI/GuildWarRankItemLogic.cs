using UnityEngine;
using System.Collections;

public class GuildWarRankItemLogic : MonoBehaviour
{
    public UILabel m_WinGuildName;
    public UILabel m_WinGuildScore;

    public void CleanUp()
    {
        m_WinGuildName.text = "??????";
        m_WinGuildScore.text = "???";
        gameObject.SetActive(true);
    }
    public void InitInfo(string WinName,int nScore)
    {
        m_WinGuildName.text = WinName;
        m_WinGuildScore.text = nScore.ToString();
    }
}
