using UnityEngine;
using System.Collections;

public class GuildWarWinGroupItemLogic : MonoBehaviour
{
    public UILabel m_WinGuildName;

    public void CleanUp()
    {
        m_WinGuildName.text = "??????";
        gameObject.SetActive(false);
    }
    public void InitInfo(string WinName)
    {
        m_WinGuildName.text = WinName;
        gameObject.SetActive(true);
    }
}
