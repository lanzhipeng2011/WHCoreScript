using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
using GCGame.Table;

public class PartnerAndMountLogic : UIControllerBase<PartnerAndMountLogic>
{
    public UIImageButton m_TabMount;
    public UIImageButton m_TabPartner;

    public MountAndFellowLogic m_MountRoot;
    public GameObject m_PartnerRoot;
    
    private int m_nTabType = 0;
    public int TabType
    {
        get { return m_nTabType; }
        set { m_nTabType = value; }
    }

    // 新手指引
    private int m_NewPlayerGuideFlag_Step = -1;
    public int NewPlayerGuideFlag_Step
    {
        get { return m_NewPlayerGuideFlag_Step; }
        set { m_NewPlayerGuideFlag_Step = value; }
    }
    public GameObject m_CloseButton;

    void OnEnable()
    {
        SetInstance(this);

        Check_NewPlayerGuide();
        ShowPartnerRoot();
    }

    void OnDisable()
    {
        TabType = -1;
        SetInstance(null);
    }

    public void CloseWindow()
    {
        UIManager.CloseUI(UIInfo.PartnerAndMountRoot);
		NewPlayerGuide((int)GameDefine_Globe.NEWOLAYERGUIDE.UI_NOGUIDE);
    }

    void ShowMountRoot()
    {
        if (TabType == 1)
        {
            return;
        }
        TabType = 1;
        m_PartnerRoot.SetActive(false);
        m_MountRoot.gameObject.SetActive(true);

        if (m_MountRoot)
        {
            m_MountRoot.MountButton();
        }
        ChooseMountTab();
    }

    public void ShowPartnerRoot()
    {
        if (TabType == 2)
        {
            return;
        }
        TabType = 2;
        m_MountRoot.gameObject.SetActive(false);
        m_PartnerRoot.SetActive(true);

        ChoosePartnerTab();

//         if (m_NewPlayerGuideFlag_Step == 2 || m_NewPlayerGuideFlag_Step == 6)
//         {
//             NewPlayerGuidLogic.CloseWindow();
// 
//             if (PartnerFrameLogic.Instance())
//             {
//                 PartnerFrameLogic.Instance().NewPlayerGuide(m_NewPlayerGuideFlag_Step);
//             }
//             m_NewPlayerGuideFlag_Step = -1;
//         }
    }

    void ChooseMountTab()
    {
		m_TabMount.normalSprite = "ui_attendants_03";
		m_TabMount.hoverSprite = "ui_attendants_03";
		m_TabMount.pressedSprite = "ui_attendants_03";
		m_TabMount.disabledSprite = "ui_attendants_03";
		m_TabMount.target.spriteName = "ui_attendants_03";
     //   m_TabMount.target.MakePixelPerfect();

		m_TabPartner.normalSprite = "ui_attendants_02";
		m_TabPartner.hoverSprite = "ui_attendants_02";
		m_TabPartner.pressedSprite = "ui_attendants_02";
		m_TabPartner.disabledSprite = "ui_attendants_02";
		m_TabPartner.target.spriteName = "ui_attendants_02";
      //  m_TabPartner.target.MakePixelPerfect();
    }

    void ChoosePartnerTab()
    {
		m_TabMount.normalSprite = "ui_attendants_04";
		m_TabMount.hoverSprite = "ui_attendants_04";
		m_TabMount.pressedSprite = "ui_attendants_04";
		m_TabMount.disabledSprite = "ui_attendants_04";
		m_TabMount.target.spriteName = "ui_attendants_04";
       // m_TabMount.target.MakePixelPerfect();

		m_TabPartner.normalSprite = "ui_attendants_01";
		m_TabPartner.hoverSprite = "ui_attendants_01";
		m_TabPartner.pressedSprite = "ui_attendants_01";
		m_TabPartner.disabledSprite = "ui_attendants_01";
		m_TabPartner.target.spriteName = "ui_attendants_01";
      //  m_TabPartner.target.MakePixelPerfect();
    }

    void Check_NewPlayerGuide()
    {
        if (MenuBarLogic.Instance() == null)
        {
            return;
        }

        int nIndex = MenuBarLogic.Instance().NewPlayerGuideIndex;

		if (nIndex == (int)GameDefine_Globe.NEWOLAYERGUIDE.PET_GAIN || nIndex == (int)GameDefine_Globe.NEWOLAYERGUIDE.PET_FIGHT)
        {
            NewPlayerGuide(nIndex);
            MenuBarLogic.Instance().NewPlayerGuideIndex = -1;
        }
    }

    public void NewPlayerGuide(int index)
    {
        if (index < 0)
        {
            return;
        }
		
		NewPlayerGuidLogic.CloseWindow();

        m_NewPlayerGuideFlag_Step = index;

        switch (index)
        {
		case (int)GameDefine_Globe.NEWOLAYERGUIDE.UI_CLOSE:
	        NewPlayerGuidLogic.OpenWindow(m_CloseButton, 78, 78, StrDictionary.GetClientDictionaryString("#{2867}"), "bottom", 2, true, true);
	        break;
		case (int)GameDefine_Globe.NEWOLAYERGUIDE.PET_GAIN:
		case (int)GameDefine_Globe.NEWOLAYERGUIDE.PET_FIGHT:
			break;
		default:
			m_NewPlayerGuideFlag_Step = -1;
	        break;
        }
    }

}
