/********************************************************************
	created:	2014/01/21
	created:	21:1:2014   16:55
	filename: 	BelleController.cs
	author:		王迪
	
	purpose:	美人界面控制器，处理子窗口间数据交互
*********************************************************************/
using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
using Module.Log;
public class BelleController : UIControllerBase<BelleController>
{

    public TabController tabController;
    public GameObject curPanel;
    public BelleDetailWindow m_BelleDetailWindow;
    public BelleMatrixWindow m_BelleMatrixWindow;
    // 新手指引
    private int m_NewPlayerGuide_Step = 0;
    public int NewPlayerGuide_Step
    {
        get { return m_NewPlayerGuide_Step; }
        set { m_NewPlayerGuide_Step = 0; }
    }
    public GameObject m_BtnClose;

    private int m_PlayerCombatBuffer = 0;

	void Awake()
    {
        SetInstance(this);
        tabController.delTabChanged = OnTabChange;
        // 新手指引
        CheckNewPlayerGuide();
    }

    void Start()
    {
        if (Singleton<ObjManager>.GetInstance().MainPlayer != null)
        {
            m_PlayerCombatBuffer = Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.CombatValue;
        }

        BelleData.CleanBelleTip();
    }

    void OnDestroy()
    {
        SetInstance(null);
    }

    // 为一个阵位选一个美人
    public void SelectMatrixRole(string matrixID, string index)
    {
        MyBelleWindow curWindow = ChangeBelleWindow();
        if (null != curWindow)
        {
            curWindow.EnableSelectMode(true, matrixID, index);
        }
    }

    // 为一个美人选择一个阵型
    public void SelectMatrix(string belleID, BelleMatrixWindow.SelectFromType type)
    {
        BelleMatrixWindow curWindow = ChangeMatrixWindow();
        if (null != curWindow)
        {
            curWindow.EnableSelectMode(true, belleID, type);
        }
    }

    public void FinishSelectMatrix(string belleID, string matrixID, string matrixIndex, BelleMatrixWindow.SelectFromType type)
    {
        if (type == BelleMatrixWindow.SelectFromType.TYPE_MYBELLE)
        {
            BelleData.delBattle = Ret_BelleSelectMatrix;
        }
        else
        {
            BelleData.delBattle = Ret_MatrixSelectMatrix;
        }
        
        //SendBelleToMatrix(belleID, matrixID, matrixIndex);
    }

    MyBelleWindow ChangeBelleWindow()
    {
        GameObject objMyBelleWindow = tabController.ChangeTab("Tab2");
        if (null == objMyBelleWindow)
        {
            return null;
        }

        return objMyBelleWindow.GetComponent<MyBelleWindow>();
        
    }

    public BelleMatrixWindow ChangeMatrixWindow()
    {
         GameObject objBelleMatrixWindow = tabController.ChangeTab("Tab2");
        if (null == objBelleMatrixWindow)
        {
            return null;
        }

        return objBelleMatrixWindow.GetComponent<BelleMatrixWindow>();
    }

    public void OpenBelleDetailWindow(int id)
    {
        m_BelleDetailWindow.gameObject.SetActive(true);
        m_BelleDetailWindow.ShowBelle(id);
        tabController.GetHighlightTab().targetObject.SetActive(false);
    }

    void Ret_BelleSelectMatrix()
    {
        ChangeBelleWindow();
    }

    void Ret_MatrixSelectBelle()
    {
        ChangeMatrixWindow();
    }

    void Ret_MatrixSelectMatrix()
    {
        ChangeMatrixWindow();
    }

    void OnCloseClick()
    {
        if (m_BelleDetailWindow.gameObject.activeSelf)
        {
			if (m_NewPlayerGuide_Step == (int)GameDefine_Globe.NEWOLAYERGUIDE.MINGJIANG_QIMI)
            {
				NewPlayerGuide(m_NewPlayerGuide_Step);
            }

            m_BelleDetailWindow.gameObject.SetActive(false);
            tabController.GetHighlightTab().targetObject.SetActive(true);
        }
        else
        {
			NewPlayerGuide((int)GameDefine_Globe.NEWOLAYERGUIDE.UI_NOGUIDE);
            UIManager.CloseUI(UIInfo.Belle);
        }       
    }

    void OnTabChange(TabButton curButton)
    {
        m_BelleDetailWindow.gameObject.SetActive(false);
    }

    void CheckNewPlayerGuide()
    {
        if (MenuBarLogic.Instance() == null)
        {
            return;
        }
        int nIndex = MenuBarLogic.Instance().NewPlayerGuideIndex;
		if ((int)GameDefine_Globe.NEWOLAYERGUIDE.MINGJIANG_QIMI == nIndex)
        {
            NewPlayerGuide(nIndex);
            MenuBarLogic.Instance().NewPlayerGuideIndex = -1;
        }
    }

    public void NewPlayerGuide(int nIndex)
    {
		
		NewPlayerGuidLogic.CloseWindow();

        m_NewPlayerGuide_Step = nIndex;

        switch (nIndex)
		{
		case (int)GameDefine_Globe.NEWOLAYERGUIDE.MINGJIANG_QIMI:
			break;
		case (int)GameDefine_Globe.NEWOLAYERGUIDE.MINGJIANG_FIGHE:
		{
			TabButton tabButton = tabController.GetTabButton("Tab2");
			if (tabButton && tabButton.targetObject)
			{
				NewPlayerGuidLogic.OpenWindow(tabButton.gameObject, 130, 56, "", "bottom", 2, true, true);
				
				BelleMatrixWindow BelleWindow = tabButton.targetObject.GetComponent<BelleMatrixWindow>();
				if (BelleWindow)
				{
					BelleWindow.NewPlayerGuide_Step = 0;
				}
			}
		}
			break;
		case (int)GameDefine_Globe.NEWOLAYERGUIDE.UI_CLOSE:
	        {
	            NewPlayerGuidLogic.OpenWindow(m_BtnClose, 78, 78, "", "bottom", 2, true, true);
	        }
	        break;
		case (int)GameDefine_Globe.NEWOLAYERGUIDE.UI_NOGUIDE:
	        m_NewPlayerGuide_Step = -1;
	        break;
        }

    }
}
