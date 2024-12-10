using UnityEngine;
using System.Collections;
using Module.Log;
using GCGame.Table;
using Games.ChatHistory;
using GCGame;
using Games.GlobeDefine;

using Games.Item;

public class DungeonWindow : MonoBehaviour {

    private static DungeonWindow m_Instance = null;
    public static DungeonWindow Instance()
    {
        return m_Instance;
    }
    public TabController m_TabLevel;        // 难度等级Tab
    public TabController m_TabNum;          // 副本人数Tab   

    public UILabel m_Story;
    public UILabel m_Level;
    public UILabel m_Time;
    public UILabel m_ZhanLi;
	public UILabel m_ZhanLiDesc;
    public UILabel m_ShengYuCiShu;
    public UILabel m_Exp;
    public UILabel m_Money;
    public GameObject m_ZuiDuiPingTai;
    public GameObject m_Mercenary;
    public GameObject m_SaoDang;
    public GameObject m_HanRen;

    public GameObject[] m_DropItem;
    public UISprite[] m_DropItemSprite;
	public UISprite[] m_DropItemQulitySprite;
    //public UIGrid ButtonGrid;
    public int Diffcult { set; get; }
    public int CopyMode { set; get; }
    private int Send { set; get;  }
    private int CopySceneId = -1;

    private int m_NewPlayerGuide_Step = -1;
    public GameObject m_BtnEnter;
    public UILabel m_ChongZhiCiShu;

    public GameObject m_BtnDanren;
    public GameObject m_BtnDuiWu;
    public UIGrid m_CopyModelGrid;
    public UILabel m_ButtonAutoTeamLabel;
    void Awake()
    {
        m_Instance = this;
    }

    // 界面加载后调用
    void Start()
    {
        Send = 0;
        InvokeRepeating("DoSomeThing", 0, 1);

		CopyMode = 1;
		if (CopySceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_FENGHUOLIANTIAN)
						CopyMode = 2;
		//Diffcult = 1;
	}
    void OnEnable()
    {        
        Send = 0;
        m_Instance = this;
        UpdateTabInfo();
    }
    void OnDisable()
    {
        m_Instance = null;
    }

    public void OnGuYongClick()
    {
        if (GlobeVar.INVALID_ID == GameManager.gameManager.PlayerDataPool.TeamInfo.TeamID)
        {
            if (Singleton<ObjManager>.GetInstance().MainPlayer)
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{3170}");
            }
            //向服务器发送邀请某人加入队伍消息
            CG_REQ_TEAM_INVITE msg = (CG_REQ_TEAM_INVITE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_REQ_TEAM_INVITE);
            if(msg != null)
            {
                msg.Guid = GlobeVar.INVALID_GUID;
                msg.SendPacket();
            }
        }
     
        HuaShanPVPData.MercenarySceneClass = CopySceneId;
        CG_MERCENARY_LIST_REQ packet = (CG_MERCENARY_LIST_REQ)PacketDistributed.CreatePacket(MessageID.PACKET_CG_MERCENARY_LIST_REQ);
        packet.Sceneclass = CopySceneId;
        packet.Diffcult = Diffcult;
        packet.SendPacket();
    }

    public void OnOpenCopyScene(int nSceneId)
    {       
        //m_Level.text = "无";
        //m_ZhanLi.text = "无";
        //m_ShengYuCiShu.text = "无";
        //m_Exp.text = "无";
        //m_Money.text = "无";
        m_Level.text = StrDictionary.GetClientDictionaryString("#{2791}");
        m_ZhanLi.text = StrDictionary.GetClientDictionaryString("#{2791}");
        m_ShengYuCiShu.text = StrDictionary.GetClientDictionaryString("#{2791}");
        m_Exp.text = StrDictionary.GetClientDictionaryString("#{2791}");
        m_Money.text = StrDictionary.GetClientDictionaryString("#{2791}");
		m_ZhanLiDesc.text = "";
        CopySceneId = nSceneId;
        Tab_SceneClass pSceneClass = TableManager.GetSceneClassByID(CopySceneId, 0);
        if (pSceneClass == null)
        {
            return;
        }

        //客户端掉落显示
        Tab_CangJingGeInfo pCangJingGeInfo = TableManager.GetCangJingGeInfoByID(CopySceneId + 200, 0);
        if (pCangJingGeInfo != null)
        {
            for (int i = 0; i < pCangJingGeInfo.getDropCount() && i < m_DropItem.Length && i < m_DropItemSprite.Length; i++ )
            {
                Tab_CommonItem pItem = TableManager.GetCommonItemByID(pCangJingGeInfo.GetDropbyIndex(i),0);
                if (pItem == null)
                {
                    m_DropItem[i].SetActive(false);
                    continue;
                }
                m_DropItem[i].SetActive(true);
                m_DropItemSprite[i].spriteName = pItem.Icon.ToString();
				m_DropItemQulitySprite[i].spriteName = GlobeVar.QualityColorGrid[pItem.Quality - 1];
            }
        }
        Tab_CopyScene pCopyScene = TableManager.GetCopySceneByID(pSceneClass.CopySceneID, 0);
        if (pCopyScene == null)
        {
            return;
        }
        m_Story.text = pCopyScene.DescInfo;
        m_TabLevel.delTabChanged = OnLevelTabChange;
        m_TabNum.delTabChanged = OnNumTabChange;
       
        if (nSceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_FENGHUOLIANTIAN)
        {
            m_BtnDanren.SetActive(false);
            m_BtnDuiWu.SetActive(true);
            m_CopyModelGrid.Reposition();

            m_TabNum.ChangeTab("DuiWu");
            //m_TabLevel.ChangeTab("JianDan");
        }
        else if (nSceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_GUOGUANZHANJIANG)
        {
            m_BtnDanren.SetActive(true);
            m_BtnDuiWu.SetActive(false);
            m_CopyModelGrid.Reposition();

            m_TabNum.ChangeTab("DanRen");
            //m_TabLevel.ChangeTab("JianDan");
        }
        else
        {
            m_BtnDanren.SetActive(true);
			//===暂时屏蔽 TODO
            m_BtnDuiWu.SetActive(true);
            m_CopyModelGrid.Reposition();

            m_TabNum.ChangeTab("DanRen");
            //m_TabLevel.ChangeTab("JianDan");
        }

		if( nSceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_FENGHUOLIANTIAN ||
		   nSceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_HULAOGUAN)
		{
			m_ZhanLiDesc.text = Utils.GetDicByID(2736);
		}
		else
		{
			m_ZhanLiDesc.text = Utils.GetDicByID(2735);
		}
        if (nSceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_QIXINGXUANZHEN ||
            nSceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_HULAOGUAN ||
            nSceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_HUSONGMEIREN ||
            nSceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_YEXIDAYING ||
            nSceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_GUOGUANZHANJIANG ||
            nSceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_FENGHUOLIANTIAN)
        {
            int nDiffcult = GetMaxDiffcult();
            if (0 == nDiffcult)
            {
                m_TabLevel.ChangeTab("JianDan");
            }
            else if (1 == nDiffcult)
            {
                m_TabLevel.ChangeTab("KunNan");
            }
            else if (2 == nDiffcult)
            {
                m_TabLevel.ChangeTab("TiaoZhan");
            }
//            else
//            {
//                m_TabLevel.ChangeTab("JianDan");
//            }        
        }
        else
        {
            m_TabLevel.ChangeTab("JianDan");
        }

        // 新手指引放这吧
        Check_NewPlayerGuide();
    }

    public int GetMaxDiffcult()
    {
        int nDiffcult = 0;
        if (null == Singleton<ObjManager>.GetInstance())
        {
            return nDiffcult;
        }
        Tab_SceneClass pSceneClass = TableManager.GetSceneClassByID(CopySceneId, 0);
        if (pSceneClass == null)
        {
            return nDiffcult;
        }
        Tab_CopyScene pCopyScene = TableManager.GetCopySceneByID(pSceneClass.CopySceneID, 0);
        if (pCopyScene == null)
        {
            return nDiffcult;
        }
        for (int i = 0; i < CharacterDefine.COPYSCENE_DIFFICULTY.Length; i++ )
        {
            Tab_CopySceneRule pCopySceneRule;
            if (1 == CopyMode)
            {
                pCopySceneRule = TableManager.GetCopySceneRuleByID(pCopyScene.GetRulebyIndex(i), 0);
            }
            else
            {
                pCopySceneRule = TableManager.GetCopySceneRuleByID(pCopyScene.GetRuleTeambyIndex(i), 0);
            }
            if (null == pCopySceneRule)
            {
                continue;
            }
            if (pCopySceneRule.Level <= Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.Level)
            {
                nDiffcult = i;
            }
        }
        return nDiffcult;
    }

    public void UpdateTabInfo()
    {
        Tab_SceneClass pSceneClass = TableManager.GetSceneClassByID(CopySceneId, 0);
        if (pSceneClass == null)
        {
            return;
        }

        Tab_CopyScene pCopyScene = TableManager.GetCopySceneByID(pSceneClass.CopySceneID, 0);
        if (pCopyScene == null)
        {
            return;
        }

        Tab_CopySceneRule pCopySceneRule;
        if (CopyMode == 1)
        {
            pCopySceneRule = TableManager.GetCopySceneRuleByID(pCopyScene.GetRulebyIndex(Diffcult - 1), 0);
        }
        else
        {
            pCopySceneRule = TableManager.GetCopySceneRuleByID(pCopyScene.GetRuleTeambyIndex(Diffcult - 1), 0);
        }
        if (pCopySceneRule == null)
        {
            return;
        }

        if (pCopySceneRule.StartTime == -1)
        {
            //m_Time.text = "全天";
            m_Time.text = StrDictionary.GetClientDictionaryString("#{2792}");
        }
        else
        {
            //m_Time.text = (pCopySceneRule.StartTime / 100).ToString() + "时" + (pCopySceneRule.StartTime % 100).ToString() + "分至" + (pCopySceneRule.EndTime / 100).ToString() + "时" + (pCopySceneRule.EndTime % 100).ToString() + "分";

            m_Time.text = StrDictionary.GetClientDictionaryString("#{2793}", pCopySceneRule.StartTime / 100, pCopySceneRule.StartTime % 100, pCopySceneRule.EndTime / 100, pCopySceneRule.EndTime % 100); 
        

        }
//        m_SaoDang.SetActive(false);
        //ButtonGrid.Reposition();
        m_Level.text = pCopySceneRule.Level.ToString();
        m_ZhanLi.text = pCopySceneRule.Battle.ToString();

        int nTabNum = pCopySceneRule.Number;
        int ExtraNum = GameManager.gameManager.PlayerDataPool.CommonData.GetCopySceneExtraNumber(CopySceneId, CopyMode);
		int nMul = GameManager.gameManager.PlayerDataPool.CommonData.GetCopySceneMultiple(CopySceneId);
		int nNum = GameManager.gameManager.PlayerDataPool.CommonData.GetCopySceneNumber(CopySceneId, CopyMode);
		string strVip = "";
		if ( CopyMode == 1) 
		{
            strVip = VipData.MakeVipString(CopySceneId);
		}
        m_ShengYuCiShu.text = (nTabNum * nMul + ExtraNum - nNum).ToString() + "/" + (nTabNum * nMul ).ToString() + strVip;

        m_Exp.text = pCopySceneRule.Exp.ToString();
        m_Money.text = pCopySceneRule.Money.ToString();
        OnButtonAutoTeamLabel();
    }

    public void UpdateCopySceneInfo(int nSceneId)
    {
        if (nSceneId == CopySceneId)
        {
            Tab_SceneClass pSceneClass = TableManager.GetSceneClassByID(CopySceneId, 0);
            if (pSceneClass == null)
            {
                return;
            }

            Tab_CopyScene pCopyScene = TableManager.GetCopySceneByID(pSceneClass.CopySceneID, 0);
            if (pCopyScene == null)
            {
                return;
            }
            Tab_CopySceneRule pCopySceneRule;
            if (CopyMode == 1)
            {
                pCopySceneRule = TableManager.GetCopySceneRuleByID(pCopyScene.GetRulebyIndex(Diffcult - 1), 0);
            }
            else
            {
                pCopySceneRule = TableManager.GetCopySceneRuleByID(pCopyScene.GetRuleTeambyIndex(Diffcult - 1), 0);
            }
            if (pCopySceneRule == null)
            {
                return;
            }
            int ExtraNum = GameManager.gameManager.PlayerDataPool.CommonData.GetCopySceneExtraNumber(CopySceneId, CopyMode);
            int nTabNum = pCopySceneRule.Number;
			int nMul = GameManager.gameManager.PlayerDataPool.CommonData.GetCopySceneMultiple(CopySceneId);
			int nNum = GameManager.gameManager.PlayerDataPool.CommonData.GetCopySceneNumber(CopySceneId, CopyMode);
            string strVip = "";
            if (CopyMode == 1)
            {
                strVip = VipData.MakeVipString(CopySceneId);
            }
            m_ShengYuCiShu.text = (nTabNum * nMul + ExtraNum - nNum).ToString() + "/" + (nTabNum * nMul ).ToString() + strVip;
        }

    }
    // 难度等级标签页切换
    void OnLevelTabChange(TabButton button)
    {
        if (button.name == "JianDan")
        {
            Diffcult = 1;
        }
        else if (button.name == "KunNan")
        {
            Diffcult = 2;
        }
        else if (button.name == "TiaoZhan")
        {
            Diffcult = 3;
        }
        UpdateTabInfo();
    }

    void OnNumTabChange(TabButton button)
    {
        if (button.name == "DanRen")
        {
            CopyMode = 1;
            m_ZuiDuiPingTai.SetActive(false);
            m_Mercenary.SetActive(false);
			m_HanRen.SetActive(false);
			m_SaoDang.SetActive(true);
        }
        else if (button.name == "DuiWu")
        {
            CopyMode = 2;
            OnButtonAutoTeamLabel();
            m_ZuiDuiPingTai.SetActive(true);
            if (CopySceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_FENGHUOLIANTIAN)
            {
                m_Mercenary.SetActive(false);
            }
            else
            {
				m_Mercenary.SetActive(true);
			}
			m_SaoDang.SetActive(false);
            m_HanRen.SetActive(true);           
        }

        if (CopySceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_YEXIDAYING
            || CopySceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_HULAOGUAN
            || (CopySceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_QIXINGXUANZHEN && CopyMode == 1)
            || (CopySceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_HUSONGMEIREN && CopyMode == 1)
            || CopySceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_GUOGUANZHANJIANG)
        {
            m_ChongZhiCiShu.text = StrDictionary.GetClientDictionaryString("#{2347}");
        }
        else
        {
            m_ChongZhiCiShu.text = StrDictionary.GetClientDictionaryString("#{2348}");
        }

        //ButtonGrid.Reposition();
        UpdateTabInfo();
    }

    // 备用
    public void SetData()
    {

    }

    void OnEnterJuXianZhuangClick()
    {
        if (m_NewPlayerGuide_Step == 1)
        {
            NewPlayerGuidLogic.CloseWindow();
            m_NewPlayerGuide_Step = -1;
        }
        if (Singleton<ObjManager>.Instance.MainPlayer.IsInJianYu())
        {
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2216}");
            return;
        }

        if (Send == 0)
        {
                Singleton<ObjManager>.GetInstance().MainPlayer.SendOpenScene(CopySceneId, CopyMode, Diffcult);
                Send = 1;
        
        }
    }
    public void OnTeamPlatformClick()
    {
        if (GameManager.gameManager.PlayerDataPool.AutoTeamState == true)
        {
            CG_ASK_AUTOTEAM packet = (CG_ASK_AUTOTEAM)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_AUTOTEAM);
            packet.SetSceneClassID(-1);
            packet.SetDifficulty(-1);
            packet.SendPacket();
        }
        else
        {
            CG_ASK_AUTOTEAM packet = (CG_ASK_AUTOTEAM)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_AUTOTEAM);
            packet.SetSceneClassID(CopySceneId);
            packet.SetDifficulty(Diffcult);
            packet.SendPacket();
        }
        ActivityController.Instance().m_TeamPlatformWindow.UpdateCopyScene(CopySceneId, CopyMode, Diffcult);  
       
    }
    public void OnTeamPlatformOpen()
    {
        ActivityController.Instance().m_TeamPlatformWindow.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
    private float timeSend = Time.realtimeSinceStartup;
    void DoSomeThing()
    {
        if (Time.realtimeSinceStartup - timeSend > 1)
        {
            timeSend = Time.realtimeSinceStartup;
            Send = 0;
        }
    }
    public void OnSaoDang()
    {
        CG_ASK_COPYSCENE_SWEEP packet = (CG_ASK_COPYSCENE_SWEEP)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_COPYSCENE_SWEEP);
        packet.SceneID = CopySceneId;
        packet.Difficult = Diffcult;
        packet.Type = CopyMode;
        packet.SendPacket();
    }
    private float OnHanRentimeSend = 0;
    public void OnHanRen()
    {
		//===去掉屏蔽//===暂时屏蔽喊人cd时间用于服务器测试
        if (Time.realtimeSinceStartup - OnHanRentimeSend < 30 )
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2163}");
            return;
       }
        OnHanRentimeSend = Time.realtimeSinceStartup;
        if(CopyMode == 1)
        {
            return;
        }
        Tab_SceneClass pSceneClass = TableManager.GetSceneClassByID(CopySceneId, 0);
        if (pSceneClass == null)
        {
            return;
        }
        //判断队伍是否已满
        if (GameManager.gameManager.PlayerDataPool.IsHaveTeam())
        {
            if (GameManager.gameManager.PlayerDataPool.TeamInfo.IsFull())
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1145}");
                return;
            }
        }
        else
        {
            if (Singleton<ObjManager>.GetInstance().MainPlayer)
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{3170}");
            }
            
            CG_REQ_TEAM_INVITE msg = (CG_REQ_TEAM_INVITE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_REQ_TEAM_INVITE);
            if (msg != null)
            {
                msg.Guid = GlobeVar.INVALID_GUID;
                msg.SendPacket();
            }
            
        }
        string text= "";
        if (Diffcult == 1)
        {
            text = StrDictionary.GetClientDictionaryString("#{2154}",pSceneClass.Name);
        }
        else  if (Diffcult == 2)
        {
            text = StrDictionary.GetClientDictionaryString("#{2155}",pSceneClass.Name);
        }
        else  if (Diffcult == 3)
        {
            text = StrDictionary.GetClientDictionaryString("#{2156}",pSceneClass.Name);
        }

        if (Singleton<ObjManager>.GetInstance().MainPlayer != null)
        {
            CG_CHAT packet = (CG_CHAT)PacketDistributed.CreatePacket(MessageID.PACKET_CG_CHAT);
            packet.Chattype = (int)CG_CHAT.CHATTYPE.CHAT_TYPE_WORLD;
            packet.Chatinfo = text;
            packet.Linktype = (int)GC_CHAT.LINKTYPE.LINK_TYPE_COPYTEAM;
            packet.AddIntdata(CopySceneId);
            packet.AddIntdata(Diffcult);
            packet.SendPacket();
           // Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false,"#{3032}");
        }       
    }  
 
    void Check_NewPlayerGuide()
    {
        if (ActivityController.Instance())
        {
            int nIndex = ActivityController.Instance().NewPlayerGuide_Step;

			if ( nIndex == (int)GameDefine_Globe.NEWOLAYERGUIDE.ACTIVITY_HULAOGUAN 
			    || nIndex == (int)GameDefine_Globe.NEWOLAYERGUIDE.ACTIVITY_GUOGUANZHANJIANG
			    || nIndex == (int)GameDefine_Globe.NEWOLAYERGUIDE.ACTIVITY_HUSONGMEIREN
			    || nIndex == (int)GameDefine_Globe.NEWOLAYERGUIDE.ACTIVITY_QIXINGXUANZHEN)
			{
				NewPlayerGuide(1);
				ActivityController.Instance().NewPlayerGuide_Step = -1;
				return;
			}

            NewPlayerGuidLogic.CloseWindow();
        }
    }
    public void NewPlayerGuide(int nIndex)
	{
		NewPlayerGuidLogic.CloseWindow();

        m_NewPlayerGuide_Step = nIndex;

        if (nIndex == 1)
        {
            NewPlayerGuidLogic.OpenWindow(m_BtnEnter.gameObject, 202, 64, "", "right", 2, true, true);
        }
    }
    public void OnButtonAutoTeamLabel()
    {
        string text = StrDictionary.GetClientDictionaryString("#{2956}");
        if ( GameManager.gameManager.PlayerDataPool.AutoTeamState == true)
        {
            text = StrDictionary.GetClientDictionaryString("#{2957}");
        }
        m_ButtonAutoTeamLabel.text = text;
    }

	//===================
	void OnItemClick(GameObject strBtn)
	{
		string strName = strBtn.name;
		string strName2 =  strName.Remove (0, 4);

		Tab_CangJingGeInfo pCangJingGeInfo = TableManager.GetCangJingGeInfoByID(CopySceneId + 200, 0);
		if (pCangJingGeInfo != null)
		{
			Tab_CommonItem pItem = TableManager.GetCommonItemByID(pCangJingGeInfo.GetDropbyIndex( int.Parse( strName2) -1),0);
			if (pItem != null)
			{
	//			ItemTooltipsLogic.ShowItemTooltip(pItem.Id, ItemTooltipsLogic.ShowType.Info);
   
				GameItem item = new GameItem();
				item.DataID = pItem.Id;
				if (item.IsEquipMent())
				{
					EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Info);
				}
				else
				{
					ItemTooltipsLogic.ShowItemTooltip(item, ItemTooltipsLogic.ShowType.Info);
				}
			}
		}
		//ItemTooltipsLogic.ShowItemTooltip(item, ItemTooltipsLogic.ShowType.Info);
	}

	//==========


}
