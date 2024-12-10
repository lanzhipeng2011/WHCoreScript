//********************************************************************
// 文件名: TitleInevstitiveItemLogic.cs
// 描述: 称号界面中称号条目
// 作者: WangZhe
//********************************************************************

using UnityEngine;
using System.Collections;
using GCGame.Table;
using GCGame;
using Games.GlobeDefine;

public class TitleInvestitiveItemLogic : MonoBehaviour {

    public enum TITLE_CLASS
    {
        SYSTEM,
        USERDEF,
        NOHOLD,
    }

    private int m_TitleID = GlobeVar.INVALID_ID;
    public int TitleID
    {
        get { return m_TitleID; }
        set { m_TitleID = value; }
    }
    private string m_strUserDefFullName = "";
    private int m_Index = -1;
    public int Index
    {
        get { return m_Index; }
        set { m_Index = value; }
    }
    private TITLE_CLASS m_TitleClass = TITLE_CLASS.NOHOLD;

    public UIImageButton m_Button;
    public UILabel m_TitleNameLabel;

	// Use this for initialization
	void Start () {
	
	}
	

    public void InitSystemTitleInfo(int nTitleID, int i)
    {
        m_TitleID = nTitleID;
        m_Index = i;
        m_TitleClass = TITLE_CLASS.SYSTEM;

        Tab_TitleData tabTitleData = TableManager.GetTitleDataByID(nTitleID, 0);
        if (tabTitleData != null)
        {
            string strTitleName = Utils.GetTitleColor(tabTitleData.ColorLevel);
            strTitleName += tabTitleData.InvestitiveName;
            m_TitleNameLabel.text = strTitleName;
        }
        m_TitleNameLabel.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    public void InitUserDefTitleInfo(int nTitleID, string strUserDefFull, int i)
    {
        m_TitleID = nTitleID;
        m_strUserDefFullName = strUserDefFull;
        m_Index = i;
        m_TitleClass = TITLE_CLASS.USERDEF;

        Tab_TitleData tabTitleData = TableManager.GetTitleDataByID(nTitleID, 0);
        if (tabTitleData != null)
        {
            string strTitleName = Utils.GetTitleColor(tabTitleData.ColorLevel);
            strTitleName += m_strUserDefFullName;
            m_TitleNameLabel.text = strTitleName;
        }
        m_TitleNameLabel.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    public void InitNoHoldTitleInfo(int nTitleID)
    {
        m_TitleID = nTitleID;
        m_Index = GlobeVar.INVALID_ID;
        m_TitleClass = TITLE_CLASS.NOHOLD;

        Tab_TitleData tabTitleData = TableManager.GetTitleDataByID(nTitleID, 0);
        if (tabTitleData != null)
        {
            string strTitleName = "[FFECB8]";
            Tab_TitleType tabTitleType = TableManager.GetTitleTypeByID(tabTitleData.Type, 0);
            if (tabTitleType != null)
            {
                strTitleName += tabTitleType.TypeName;
                m_TitleNameLabel.text = strTitleName;
            }
        }
        m_TitleNameLabel.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
    }

    public void ChooseTitleInvestitive()
    {
        if (null != TitleInvestitiveLogic.Instance())
            TitleInvestitiveLogic.Instance().ChooseTitleInvestitive(m_Index, m_TitleID, m_TitleClass);

        m_Button.normalSprite = "ui_player_07";
		m_Button.hoverSprite = "ui_player_08";
		m_Button.pressedSprite = "ui_player_07";
		m_Button.target.spriteName = "ui_player_08";
        m_Button.target.MakePixelPerfect();
    }

    public void ClearChooseState()
    {
		m_Button.normalSprite = "ui_player_07";
		m_Button.hoverSprite = "ui_player_07";
		m_Button.pressedSprite = "ui_player_08";
		m_Button.target.spriteName = "ui_player_07";
        m_Button.target.MakePixelPerfect();
    }
}
