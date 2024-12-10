using UnityEngine;
using System.Collections;

public class GuildWarFightGroupItemLogic : MonoBehaviour
{
    public UILabel  m_GuildAName;
    public UILabel  m_GuildBName;
    public UISprite m_GuildABak;
    public UISprite m_GuildBBak;
	// Use this for initialization
	void Start () 
    {
	
	}

    public void CleanUp()
    {
        m_GuildAName.text = "??????";
        m_GuildBName.text = "??????";
        m_GuildABak.spriteName = "changtai";
        m_GuildABak.transform.localPosition =new Vector3(0,0,0);
        m_GuildABak.MakePixelPerfect();
        m_GuildBBak.spriteName = "changtai";
        m_GuildBBak.MakePixelPerfect();
        m_GuildBBak.transform.localPosition =new Vector3(0,0,0);
        gameObject.SetActive(false);
    }
    public void InitGroupInfo(GuildWarGroupInfo _info)
    {
        m_GuildAName.text = _info.GuildAName;
        m_GuildBName.text = _info.GuildBName;
        if (_info.WinType == (int)GC_RET_FINALGUILDWARGROUPINFO.WINTYPE.AGUILDWIN)
        {
            m_GuildABak.spriteName = "shengli";
            m_GuildABak.MakePixelPerfect();
            m_GuildABak.transform.localPosition =new Vector3(-8,15,0);
        }
        else if (_info.WinType == (int) GC_RET_FINALGUILDWARGROUPINFO.WINTYPE.BGUILDWIN)
        {

            m_GuildBBak.spriteName = "shengli";
            m_GuildBBak.MakePixelPerfect();
            m_GuildBBak.transform.localPosition =new Vector3(-8,15,0);
        }
        gameObject.SetActive(true);
    }
	
}
