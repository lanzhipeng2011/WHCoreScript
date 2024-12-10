using UnityEngine;
using System.Collections;
using GCGame;
using GCGame.Table;
using Games.GlobeDefine;
public class PVPFindOpWindow : MonoBehaviour {

    public GameObject CurOpInfo;

    public UILabel LabelSuccessSpirit;        // 成功获得真气值
    public UILabel LabelFailGetSpirit;           // 失败获得真气值

    public GameObject m_PVPFindList;          //对手的实际Item

    public UILabel LableShowFindTip;
    public GameObject FindTips;

    public GameObject OpListGrid;
    public TabController m_PvPWindowTableController;

    private PVPData.OpponentInfo m_curData;
    private PVPOpListItem m_curSelectItem = null;

    public GameObject m_FightButton;


    void OnEnable()
    {
		m_curData.Clear ();
        PVPData.delUpdateOpponent += OnUpdateList;
        //MessageBoxLogic.OpenOKCancelBox(1183);
        CG_RANDOM_OPPONENT packet = (CG_RANDOM_OPPONENT)PacketDistributed.CreatePacket(MessageID.PACKET_CG_RANDOM_OPPONENT);
        packet.None = 0;
        packet.SendPacket();

        m_curSelectItem = null;
        LableShowFindTip.text = Utils.GetDicByID(1183);
    }

    void OnDisable()
    {
        Utils.CleanGrid(OpListGrid);
        PVPData.delUpdateOpponent -= OnUpdateList;
    }
    //void Start()
    //{
		
    //}

    void OnSKillLevelupClick( )
    {
//         if (null != m_PvPWindowTableController)
//             m_PvPWindowTableController.ChangeTab("Tab2");
        UIManager.ShowUI(UIInfo.SkillInfo);
    }

    void OnRefreshClick()
    {
        CG_RANDOM_OPPONENT packet = (CG_RANDOM_OPPONENT)PacketDistributed.CreatePacket(MessageID.PACKET_CG_RANDOM_OPPONENT);
        packet.None = 0;
        packet.SendPacket();
    }

    // 点击挑战按钮
    void OnFightClick()
    {
        if(!m_curData.IsValid())
        {
            return;
        }

        if (Singleton<ObjManager>.Instance.MainPlayer.IsInJianYu())
        {
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2216}");
            return;
        }

//        if (GameManager.gameManager.RunningScene != (int)GameDefine_Globe.SCENE_DEFINE.SCENE_WUDAOZHIDIAN)
//去掉此判断，现在那个场景都可以去
 //       {
            if (PVPData.NeedCostYuanBao > 0)
            {
                string str = StrDictionary.GetClientDictionaryString("#{2495}", PVPData.NeedCostYuanBao);
                MessageBoxLogic.OpenOKCancelBox(str, "", SendChallenge);
            }
            else
            {
                SendChallenge();
            }
//        }

        // 新手指引
        if (ActivityController.Instance())
        {
			if ((int)GameDefine_Globe.NEWOLAYERGUIDE.ACTIVITY_QUNXIONG == ActivityController.Instance().NewPlayerGuide_Step)
            {
				NewPlayerGuidLogic.CloseWindow();
				ActivityController.Instance().NewPlayerGuide_Step = -1;
            }
        }
    }

    void SendChallenge( )
    {
        if (PVPData.NeedCostYuanBao > 0)
        {
            int nPlayerYuanBao = GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBao();
            nPlayerYuanBao += GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBaoBind();
            if (nPlayerYuanBao < PVPData.NeedCostYuanBao)
            {
                Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2633}");
                return;
            }
        }
		CG_OPEN_COPYSCENE packet1 = (CG_OPEN_COPYSCENE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_OPEN_COPYSCENE);
		packet1.SceneID = 17;
		packet1.Type = 1;
		packet1.Difficult = 1;
		packet1.EnterType = 1;   //界面进入
		packet1.OpponentGuid = m_curData.guid;
		packet1.SendPacket();

//        CG_REQ_CHALLENGE packet = (CG_REQ_CHALLENGE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_REQ_CHALLENGE);
//        packet.SetOpponentGuid(m_curData.guid);
//        packet.SendPacket();
    }



	public void OnOpItemClick(PVPOpListItem item)
	{
        if (null != m_curSelectItem)
        {
            m_curSelectItem.EnableHighlight(false);
        }
        m_curSelectItem = item;
        m_curSelectItem.EnableHighlight(true);
		UpdateCurOpInfo(item.GetData());
	}

	void UpdateCurOpInfo(PVPData.OpponentInfo data)
	{
        m_curData = data;
        LabelSuccessSpirit.text = m_curData.winSpirit.ToString();
        LabelFailGetSpirit.text = m_curData.loseSpirit.ToString();
	}

    void OnUpdateList()
    {
        if (null == m_PVPFindList)
            return;

        FindTips.SetActive(false);
        Utils.CleanGrid(OpListGrid);
        m_curSelectItem = null;
        foreach (ulong opInfoKey in PVPData.OpponentMap.Keys)
        {
            PVPOpListItem item = PVPOpListItem.CreateItem(OpListGrid, m_PVPFindList, opInfoKey.ToString(), this, PVPData.OpponentMap[opInfoKey]);
            if (null == m_curSelectItem)
            {
                OnOpItemClick(item);
            }
        }

        OpListGrid.GetComponent<UIGrid>().Reposition();
        OpListGrid.GetComponent<UITopGrid>().Recenter(true);
        CurOpInfo.SetActive(PVPData.OpponentMap.Count > 0);

        // 新手指引
        if (ActivityController.Instance())
        {
			if ((int)GameDefine_Globe.NEWOLAYERGUIDE.ACTIVITY_QUNXIONG == ActivityController.Instance().NewPlayerGuide_Step)
            {
                if (PVPData.OpponentMap.Count > 0)
                {
					NewPlayerGuidLogic.CloseWindow();
                    NewPlayerGuidLogic.OpenWindow(m_FightButton, 180, 60, "", "left", 2, true, true);
				}
            }
        }
    }
}
