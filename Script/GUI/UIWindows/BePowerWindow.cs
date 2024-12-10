using UnityEngine;
using System.Collections;
using GCGame;
using GCGame.Table;
using System.Collections.Generic;



public class BePowerWindow : UIControllerBase<BePowerWindow>
{
    public GameObject MemberListGrid;
    public TabController m_TabController;
    public GameObject m_LabelNullTips;

    public TabButton []m_Tab;
    public PowerLeftItem[] m_PowerLeftItem;
    
    public UISlider ProgressSlider;
    public UISprite[] SpriteLevelIcon;

    void Awake()
    {
        UIControllerBase<BePowerWindow>.SetInstance(this);
        m_TabController.delTabChanged = OnTabChangedCall;
    }

	// Use this for initialization
	void Start () {
      
	}


    void OnDestroy()
    {
        UIControllerBase<BePowerWindow>.SetInstance(null);
    }

    void OnEnable()
    {
        BePowerData.delegateShowBePowerItemList += ShowFunctionList;
        BePowerData.delegateShowBePowerLeft += ShowLeftList;
        SetDefaultLeftLevel();
		m_TabController.ChangeTab("Tab1");
    }

    void OnDisable()
    {
        BePowerData.delegateShowBePowerItemList -= ShowFunctionList;
        BePowerData.delegateShowBePowerLeft -= ShowLeftList;
    }

    void SetDefaultLeftLevel()
    {
        for (int i = 0; i < m_PowerLeftItem.Length; i++ )
        {
            if (m_PowerLeftItem[i] != null)
            {
                m_PowerLeftItem[i].SetData(i+1,0,0, (i == 0),i);
            }
        }
    }

    void OnTabChangedCall(TabButton button)
    {
        CG_REQ_POWERUP packet = (CG_REQ_POWERUP)PacketDistributed.CreatePacket(MessageID.PACKET_CG_REQ_POWERUP);
        packet.Flag = 0;
        if (button.name == "Tab1")
        {
            packet.Type = (int)BePowerData.BePowerType.BPTDEFINE_EQUIP;
            packet.SendPacket();
        }
        else if (button.name == "Tab2")
        {
            packet.Type = (int)BePowerData.BePowerType.BPTDEFINE_BELLE;
            packet.SendPacket();
        }
        else if (button.name == "Tab3")
        {
            packet.Type = (int)BePowerData.BePowerType.BPTDEFINE_GEM;
            packet.SendPacket();
        }
        else if (button.name == "Tab4")
        {
            packet.Type = (int)BePowerData.BePowerType.BPTDEFINE_XIAKE;
            packet.SendPacket();
        }
        else if (button.name == "Tab5")
        {
            packet.Type = (int)BePowerData.BePowerType.BPTDEFINE_SKILL;
            packet.SendPacket();
        }
        else if (button.name == "Tab6")
        {
            packet.Type = (int)BePowerData.BePowerType.BPTDEFINE_FELLOW;
            packet.SendPacket();
        }
    }

    public void ChangeTabOnReciveData(int type)
    {
        BePowerData.BePowerType eType = (BePowerData.BePowerType)type;
        switch (eType)
        {
            case BePowerData.BePowerType.BPTDEFINE_EQUIP:
                m_TabController.ChangeTab("Tab1");
                break;
            case BePowerData.BePowerType.BPTDEFINE_BELLE:
                m_TabController.ChangeTab("Tab2");
                break;
            case BePowerData.BePowerType.BPTDEFINE_GEM:
                m_TabController.ChangeTab("Tab3");
                break;
            case BePowerData.BePowerType.BPTDEFINE_XIAKE:
                m_TabController.ChangeTab("Tab4");
                break;
            case BePowerData.BePowerType.BPTDEFINE_SKILL:
                m_TabController.ChangeTab("Tab5");
                break;
            case BePowerData.BePowerType.BPTDEFINE_FELLOW:
                m_TabController.ChangeTab("Tab6");
                break;
            default:
                return;
        }

        ShowFunctionList();
    }

    void OnCloseButtonClick()
    {
        SetInstance(null);
        UIManager.CloseUI(UIInfo.CheckPowerRoot);
    }

    void ShowLeftList()
    {
        if (BePowerData.curScoreList.Count > 0)
        {
            for (int i = 0; i < BePowerData.curScoreList.Count; i++)
            {
                if (BePowerData.curScoreList[i].type <= m_Tab.Length && BePowerData.curScoreList[i].type > 0)
                {
                    if ( i < m_PowerLeftItem.Length )
                    {
                        if (m_PowerLeftItem[i] != null)
                        {
                            bool setSlider = false;
                            if (i < m_Tab.Length && m_Tab[i] != null )
                            {
                                GameObject curTab = m_TabController.GetHighlightTab().gameObject;
                                if (curTab != null)
                                {
                                    setSlider = m_Tab[i].gameObject.name == curTab.name;
                                }
                            }
                            m_PowerLeftItem[i].SetData(BePowerData.curScoreList[i].type,
                                BePowerData.curScoreList[i].value, BePowerData.curScoreList[i].level, setSlider);
                            if (setSlider) SetSlider(m_PowerLeftItem[i].ItemLevel);
                        }
                    }
                }
            }
        }
    }

    void SetSlider(int level)
    {
        if (ProgressSlider != null)
        {
            switch (level)
            {
                case 1:
                    ProgressSlider.value = 0.2f;
                    break;
                case 2:
                    ProgressSlider.value = 0.4f;
                    break;
                case 3:
                    ProgressSlider.value = 0.6f;
                    break;
                case 4:
                    ProgressSlider.value = 0.8f;
                    break;
                case 5:
                    ProgressSlider.value = 1.0f;
                    break;
                default:
                    ProgressSlider.value = 0;
                    break;
            }
        }

        for (int i = 0; i < SpriteLevelIcon.Length; i++)
        {
            bool activeThis = ((i+1) == level);
            SpriteLevelIcon[i].gameObject.SetActive(activeThis);
        }
    }

    void ShowFunctionList()
    {
        if (Singleton<ObjManager>.Instance.MainPlayer.IsDie())
        {
            UIManager.LoadItem(UIInfo.PowerPushReliveListItem, OnLoadItem);
        }
        else
        {
            UIManager.LoadItem(UIInfo.PowerPushListItem, OnLoadItem);
        }
    }

    void OnLoadItem(GameObject resItem, object param)
    {
        if (m_LabelNullTips != null)
        {
            m_LabelNullTips.SetActive((BePowerData.curDataList.Count <= 0));
        }

        if (MemberListGrid != null)
        {
            Utils.CleanGrid(MemberListGrid);

            for (int i = 0; i < BePowerData.curDataList.Count; ++i)
            {
                BePowerListItem item = BePowerListItem.CreateItem(MemberListGrid, resItem, BePowerData.curDataList[i].function.ToString(), this, BePowerData.curDataList[i]);
            }

            MemberListGrid.GetComponent<UIGrid>().Reposition();
            MemberListGrid.GetComponent<UITopGrid>().Recenter(true);
            ShowLeftList();
        }
    }
}