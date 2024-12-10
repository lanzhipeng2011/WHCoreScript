using UnityEngine;
using System.Collections;
using GCGame;
using GCGame.Table;
using System.Collections.Generic;

public class WorldBossWindow : UIControllerBase<WorldBossWindow>
{
    public GameObject MemberListGrid;
    public GameObject PrePageButton;
    public GameObject NextPageButton;
    public UILabel PageTip;

    private List<HuaShanPVPData.WorldBossTeamInfo> m_curDataList = new List<HuaShanPVPData.WorldBossTeamInfo>();

    void Awake()
    {
        m_curDataList.Clear();
        UIControllerBase<WorldBossWindow>.SetInstance(this);
        HuaShanPVPData.WorldBossCurPage = 1;
    }

    void OnDestroy()
    {
        UIControllerBase<WorldBossWindow>.SetInstance(null);
    }

	// Use this for initialization
	void Start () 
	{
		InvokeRepeating ("AutoRefresh", 1.0f, 10.0f);
	}

    void OnEnable()
    {
        HuaShanPVPData.delegateShowWorldBossList += ShowWorldBossMemberList;
    }

    void OnDisable()
    {
        HuaShanPVPData.delegateShowWorldBossList -= ShowWorldBossMemberList;
    }

	void AutoRefresh()
	{
		ReqWorldBossTeamList();
	}

    void OnCloseClick()
    {
        SetInstance(null);
        UIManager.CloseUI(UIInfo.WorldBossWindowRoot);
    }
#region 暂时不用

    void PrePage()
    {
        if (HuaShanPVPData.WorldBossCurPage > 1)
        {
            HuaShanPVPData.WorldBossCurPage -= 1;
            ReqWorldBossTeamList();
        }
    }

    void NextPage()
    {
        if (HuaShanPVPData.WorldBossCurPage < HuaShanPVPData.WorldBossTotalPage)
        {
            HuaShanPVPData.WorldBossCurPage += 1;
            ReqWorldBossTeamList();
        }
    }

    void ReqWorldBossTeamList()
    {
        CG_WORLDBOSS_TEAMLIST_REQ packet = (CG_WORLDBOSS_TEAMLIST_REQ)PacketDistributed.CreatePacket(MessageID.PACKET_CG_WORLDBOSS_TEAMLIST_REQ);
		packet.Page = (uint)HuaShanPVPData.WorldBossCurPage;
        packet.SendPacket();
    }
#endregion

    public void OnOpItemClick(WorldBossListItem item)
    {
        //... 发出挑战
        if (Singleton<ObjManager>.GetInstance().MainPlayer != null)
        {
            if (item.GetData().id > 0 && Singleton<ObjManager>.GetInstance().MainPlayer.IsTeamLeader())
            {
                CG_WORLDBOSS_CHALLENGE packet = (CG_WORLDBOSS_CHALLENGE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_WORLDBOSS_CHALLENGE);
                packet.TeamId = item.GetData().id;
                packet.SendPacket();
            }
        }
    }

    void ShowWorldBossMemberList()
    {
//        if (HuaShanPVPData.WorldBossCurPage >= HuaShanPVPData.WorldBossTotalPage)
//            NextPageButton.SetActive(false);
//        else
//            NextPageButton.SetActive(true);
//
//        if (HuaShanPVPData.WorldBossCurPage <= 1)
//            PrePageButton.SetActive(false);
//        else
//            PrePageButton.SetActive(true);

        UIManager.LoadItem(UIInfo.WorldBossListItem, OnLoadWorlBossItem);
    }

    void OnLoadWorlBossItem(GameObject resItem, object param)
    {
        if (resItem == null)
        {
            return;
        }
        Utils.CleanGrid(MemberListGrid);
        foreach (HuaShanPVPData.WorldBossTeamInfo wbInfo in HuaShanPVPData.WorldBossList)
        {
            WorldBossListItem.CreateItem(MemberListGrid, resItem, wbInfo.id.ToString(), this, wbInfo);
        }
        PageTip.text = HuaShanPVPData.WorldBossCurPage.ToString() + "/" + HuaShanPVPData.WorldBossTotalPage.ToString();
        MemberListGrid.GetComponent<UIGrid>().repositionNow = true;
		MemberListGrid.GetComponent<UITopGrid>().Recenter(true);
    }
}