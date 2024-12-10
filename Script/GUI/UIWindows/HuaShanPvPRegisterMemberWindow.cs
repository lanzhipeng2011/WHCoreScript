using UnityEngine;
using System.Collections;
using GCGame;
using GCGame.Table;

public class HuaShanPvPRegisterMemberWindow : MonoBehaviour
{
    public GameObject title;
    public UILabel LabelSelfPos;
    public GameObject MemberListGrid;
    public float RefreshTimeCounter { set; get; }
    public TabController m_HuaShanTabController;

	// Use this for initialization
	void Start () {
       // HuaShanPVPData.MySelfeHuaShanPvPPos = -1;
        RefreshTimeCounter = 0;
	}

    void OnEnable()
    {
        HuaShanPVPData.delegateShowRegisterMemberList += ShowRegisterMemberList;
       // HuaShanPVPData.delegateShowSelfRegisterInfo += ShowSelfPvPPos;
    }

    void OnDisable()
    {
        Utils.CleanGrid(MemberListGrid);
        HuaShanPVPData.delegateShowRegisterMemberList -= ShowRegisterMemberList;
       // HuaShanPVPData.delegateShowSelfRegisterInfo -= ShowSelfPvPPos;
    }

    public void OnReturnButtonClick()
    {
        if (m_HuaShanTabController != null)
        {
            m_HuaShanTabController.ChangeTab("Tab1");
            HuaShanPVPData.delegateShowRegisterMemberList -= ShowRegisterMemberList;
        }
    }

    void ShowRegisterMemberList()
    {
        UIManager.LoadItem(UIInfo.HuaShanMemberListItem, OnLoadMemberItem);
    }

    void OnLoadMemberItem(GameObject resItem, object param)
    {
        Utils.CleanGrid(MemberListGrid);
        for (int i = 0; i < HuaShanPVPData.RegisterMemberList.Count; ++i)
        {
            HuashanPvPMemberListItem.CreateItem(MemberListGrid, resItem, 
                                                HuaShanPVPData.RegisterMemberList[i].pos.ToString(),
                                                this, 
                                                HuaShanPVPData.RegisterMemberList[i].pos.ToString(), 
                                                HuaShanPVPData.RegisterMemberList[i].name, 
                                                HuaShanPVPData.RegisterMemberList[i].combat.ToString());
        }
        
        MemberListGrid.GetComponent<UIGrid>().Reposition();
        //MemberListGrid.GetComponent<UITopGrid>().Recenter(true);
    }

    //void ShowSelfPvPPos( )
    //{
    //    int nPos = HuaShanPVPData.MySelfeHuaShanPvPPos;
    //    string history = StrDictionary.GetClientDictionaryString("#{1635}", nPos == -1 ? "64+" : nPos.ToString());
    //    LabelSelfPos.text = history;
    //}
}
