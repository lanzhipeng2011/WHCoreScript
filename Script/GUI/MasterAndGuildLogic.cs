//********************************************************************
// 文件名: MasterAndGuildLogic.cs
// 全路径：	\Script\GUI\MasterAndGuildLogic.cs
// 描述: 玩家师门和帮会信息
// 作者: lijia
// 创建时间: 2014-04-23
//********************************************************************
using UnityEngine;
using System.Collections;

public class MasterAndGuildLogic : MonoBehaviour
{
    private static MasterAndGuildLogic m_Instance = null;
    public static MasterAndGuildLogic Instance()
    {
        return m_Instance;
    }

    //外部控件
    public TabController m_TabController;
    public GuildWindow m_GuildWindow;       //帮会页面

    void Awake()
    {
        m_Instance = this;
    }
    
    void OnDestroy()
    {
        m_Instance = null;
    }

    //关闭界面
    void OnClose()
    {
        UIManager.CloseUI(UIInfo.MasterAndGuildRoot);
    }
}
