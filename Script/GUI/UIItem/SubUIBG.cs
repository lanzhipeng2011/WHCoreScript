/********************************************************************
	created:	2014/02/17
	created:	17:2:2014   9:53
	filename: 	SubUIBG.cs
	author:		王迪
	
	purpose:	通用的UI背景
*********************************************************************/
using UnityEngine;
using System.Collections;

public class SubUIBG : MonoBehaviour {

    public UISprite sprTitle;
    private UIPathData m_curUIData;
    public void SetData(string titleSprite, UIPathData curUIData)
    {
        if (null != sprTitle) sprTitle.spriteName = titleSprite;
        m_curUIData = curUIData;
    }

    void OnCloseClick()
    {
        UIManager.CloseUI(m_curUIData);
    }
}
