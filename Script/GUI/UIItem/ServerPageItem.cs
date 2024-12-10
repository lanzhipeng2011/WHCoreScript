using UnityEngine;
using System.Collections;

public class ServerPageItem : MonoBehaviour {

    public UILabel m_LabelPageName;
    public GameObject m_SprHighlight;
    public GameObject m_SprNormal;
    void Start()
    {

    }

    public void SetTitle(string title)
    {
        m_LabelPageName.text = title;
    }

    void OnSelectPageClick()
    {
        UIControllerBase<ServerListWindow>.Instance().ServerPageSelected(gameObject.name);
    }

    public void EnableHeightLight(bool bEnable)
    {
        m_SprHighlight.SetActive(bEnable);
        m_SprNormal.SetActive(!bEnable);
    }
}
