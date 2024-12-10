using UnityEngine;
using System.Collections;
using GCGame;
using GCGame.Table;
using System.Collections.Generic;

public class MercenaryWindow : UIControllerBase<MercenaryWindow>
{
    public UILabel LabelLeftTimes;
    public GameObject MemberListGrid;

    private static readonly int MERCESELEC_NUM = 3;
    private int SelectedNum { set; get; }

    private List<HuaShanPVPData.MercenaryInfo> m_curDataList = new List<HuaShanPVPData.MercenaryInfo>();

    void Awake()
    {
        m_curDataList.Clear();
        UIControllerBase<MercenaryWindow>.SetInstance(this);
    }

	// Use this for initialization
	void Start () {
      
	}

    void OnEnable()
    {
        HuaShanPVPData.delegateShowMercenaryList += ShowMercenaryMemberList;
        HuaShanPVPData.delegateShowMercenaryLeftTimes += ShowSelfLefTimes;
    }

    void OnDisable()
    {
        HuaShanPVPData.delegateShowMercenaryList -= ShowMercenaryMemberList;
        HuaShanPVPData.delegateShowMercenaryLeftTimes -= ShowSelfLefTimes;
    }

    void OnCloseClick()
    {
        SetInstance(null);
        UIManager.CloseUI(UIInfo.MercenaryWindowRoot);
    }

    void ShowSelfLefTimes()
    {
        LabelLeftTimes.text = StrDictionary.GetClientDictionaryString("#{1864}", 
            HuaShanPVPData.MercenaryTimesLeft);
    }


    public void OnEmercenaryBtbClick()
    {
        if (m_curDataList != null)
        {
            CG_MERCENARY_REQ packet = (CG_MERCENARY_REQ)PacketDistributed.CreatePacket(MessageID.PACKET_CG_MERCENARY_REQ);
            for (int i = 0; i < m_curDataList.Count; ++i)
            {
                packet.AddGuid(m_curDataList[i].guid);
                packet.AddSource(m_curDataList[i].relationship);
            }
            packet.Sceneclass = HuaShanPVPData.MercenarySceneClass; // ...
            packet.SendPacket();

            OnCloseClick();
        }
    }

    public bool OnOpItemClick(MercenaryListItem item, bool bSelected)
    {
        if (SelectedNum < MERCESELEC_NUM && bSelected)
        {
            m_curDataList.Add(item.GetData());
            SelectedNum++;
            return true;
        }
        else if (bSelected == false)
        {
            if (m_curDataList.Remove(item.GetData()))
                SelectedNum--;
            return true;
        }

        return false;
    }

    void ShowMercenaryMemberList()
    {
        UIManager.LoadItem(UIInfo.MercenaryListItem, OnLoadMemberItem);
    }

    void OnLoadMemberItem(GameObject resItem, object param)
    {
        Utils.CleanGrid(MemberListGrid);
        for (int i = 0; i < HuaShanPVPData.MercenaryList.Count; ++i)
        {
            MercenaryListItem.CreateItem(MemberListGrid, resItem, HuaShanPVPData.MercenaryList[i].guid.ToString(), this, HuaShanPVPData.MercenaryList[i]);
        }

        SelectedNum = 0;
        MemberListGrid.GetComponent<UIGrid>().repositionNow = true;
        MemberListGrid.GetComponent<UITopGrid>().Recenter(true);
    }
}