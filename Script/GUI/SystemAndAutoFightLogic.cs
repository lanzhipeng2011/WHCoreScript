using UnityEngine;
using System.Collections;

public class SystemAndAutoFightLogic : MonoBehaviour {

    private static SystemAndAutoFightLogic m_Instance = null;
    public static SystemAndAutoFightLogic Instance()
    {
        return m_Instance;
    }

    public TabController m_TabController;

    void Awake()
    {
        m_Instance = this;
    }

    void OnDestroy()
    {
        m_Instance = null;
    }

    void OnCloseClick()
    {
        UIManager.CloseUI(UIInfo.SystemAndAutoFight);
    }
}
