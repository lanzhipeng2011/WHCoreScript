using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Games.LogicObj;
using Games.SkillModle;
using GCGame;
using GCGame.Table;
using UnityEngine;
using System.Collections;
using Module.Log;

public class PvPRankWindow : MonoBehaviour {

	public GameObject RankMemberList;
    public UILabel PageTip;
    public GameObject PrePageButton;
    public GameObject NextPageButton;

	

    void OnEnable()
    {
        PVPData.delegateUpdatePvPRankList += ShowPvPRankList;
        PVPData.PvPRankCurPage = 1;
        RankMemberList.GetComponent<UIGrid>().Reposition();
        RankMemberList.GetComponent<UITopGrid>().Recenter(true);
        ReqPvPRankList();
    }
    void OnDisable()
    {
        PVPData.delegateUpdatePvPRankList -= ShowPvPRankList;
        Utils.CleanGrid(RankMemberList);
    }

    private static PvPRankWindow m_Instance = null;
    public static PvPRankWindow Instance()
    {
        return m_Instance;
    }
    void Awake()
    {
        m_Instance = this;
    }

    void ShowPvPRankList()
    {
        if (PVPData.PvPRankCurPage >= PVPData.PvPRankTotalPage)
            NextPageButton.SetActive(false);
        else
            NextPageButton.SetActive(true);

        if (PVPData.PvPRankCurPage <= 1)
            PrePageButton.SetActive(false);
        else
            PrePageButton.SetActive(true);

        UIManager.LoadItem(UIInfo.PvPRankListItem, OnLoadPvPRanListItem);
    }

    void OnLoadPvPRanListItem(GameObject resItem, object param)
    {
        Utils.CleanGrid(RankMemberList);
        for (int i = 0; i < PVPData.PvPRankList.Count; ++i)
        {
            PvPRankListItem.CreateItem(RankMemberList, resItem, PVPData.PvPRankList[i].id.ToString(), PVPData.PvPRankList[i]);
        }

        PageTip.text = PVPData.PvPRankCurPage.ToString() + "/" + PVPData.PvPRankTotalPage.ToString();
        RankMemberList.GetComponent<UIGrid>().repositionNow = true;
        RankMemberList.GetComponent<UITopGrid>().IsResetOnEnable = true;
        RankMemberList.GetComponent<UITopGrid>().recenterTopNow = true;
    }

    void ReqPvPRankList()
    {
        CG_CHALLENGERANKLIST_REQ packet = (CG_CHALLENGERANKLIST_REQ)PacketDistributed.CreatePacket(MessageID.PACKET_CG_CHALLENGERANKLIST_REQ);
		packet.Page = (uint)PVPData.PvPRankCurPage;
        packet.SendPacket();
    }

    void PrePage()
    {
        if (PVPData.PvPRankCurPage > 1)
        {
            PVPData.PvPRankCurPage -= 1;
            ReqPvPRankList();
        }
    }

    void NextPage()
    {
        if (PVPData.PvPRankCurPage < PVPData.PvPRankTotalPage)
        {
            PVPData.PvPRankCurPage += 1;
            ReqPvPRankList();
        }
    }  

    void OnPageClick()
    {
        if (PVPData.PvPRankCurPage > 1 || PVPData.PvPRankCurPage < PVPData.PvPRankTotalPage)
        {
            string str = Utils.GetDicByID(2960);
            NumChooseController.NumChooseInfo curInfo = new NumChooseController.NumChooseInfo(1, PVPData.PvPRankTotalPage, str, OnNumChoose, 1);
            UIManager.ShowUI(UIInfo.NumChoose, OnShowNumChoose, curInfo);
        }
    }

    // 批量购买确定
    void OnNumChoose(int curNum)
    {
        PVPData.PvPRankCurPage = curNum;
        ReqPvPRankList();
    }

    public static void OnShowNumChoose(bool bSuccess, object param)
    {
        if (!bSuccess)
        {
            return;
        }

        if (null == param)
        {
            LogModule.ErrorLog("ShowNumChoose:param not define.");
            return;
        }

        NumChooseController.NumChooseInfo curInfo = param as NumChooseController.NumChooseInfo;
        NumChooseController.Instance().SetData(curInfo._minValue, curInfo._maxValue, curInfo._szTitle, curInfo._okClickFun, curInfo._stepValue);
    }
}
