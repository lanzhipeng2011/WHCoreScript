using UnityEngine;
using System.Collections;
using GCGame;
using GCGame.Table;
using Games.Item;
using Games.GlobeDefine;
using System;

public class CangJingGeWindow : MonoBehaviour
{

	private int CopySceneId = (int)GameDefine_Globe.SCENE_DEFINE.SCENE_WUSHENTA;
	public UILabel m_Yanbao;
    public UILabel m_Sweep;
    public UILabel m_Tier;
    public UILabel m_CalculatValue;
    public UILabel m_MonsterValue1;
    public UILabel m_MonsterValue2;
    public UILabel m_MonsterValue3;
    public UISprite m_DropIconSprite1;
    public UISprite m_DropIconSprite2;
    public UISprite m_DropIconSprite3;
    public UISprite m_RewardIconSprite1;
    public UISprite m_RewardIconSprite2;
    public UISprite m_RewardIconSprite3;
    public UILabel m_SweepCDTime;
    public UILabel m_TiaoZhanCount;
    public GameObject ItemParent;
    public CangJingGeItem[] m_CangJingGeItem;
    private static CangJingGeWindow m_Instance = null;
   // private bool m_bStartSweep;

    private int m_NewPlayerGuide_Step = -1;
    public GameObject m_BtnTiaoZhan;

    //缓存的奖励ID，注意，以后可以拖动选择关卡的时候，这个数据要改变。
    private int m_nDropItemID1 = -1;
    private int m_nDropItemID2 = -1;
    private int m_nDropItemID3 = -1;
    //玩家指针在哪层显示
    private int m_nTierOnPlayer = 0; 
    //关卡锁sprite
	private string m_sLockedFloor = "ui_activity_13";
	
    public GameObject m_ObjectSweepLabel1;
    public GameObject m_ObjectSweepLabel2;
    //private int m_nSweepTier;
    public static CangJingGeWindow Instance()
    {
        return m_Instance;
    }
    void Awake()
    {
        m_Instance = this;
    }
    // Use this for initialization
    void OnEnable()
    {
        m_Instance = this;
        //m_bStartSweep = false;
        UpdateInfo();
    }

    void OnDisable()
    {
        m_Instance = null;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSweep();
    }
    public void UpdateTierShow(int nCount)
    {
        for (int i = 0; i < m_CangJingGeItem.Length; i++)
        {            
            int nRealTie = m_nTierOnPlayer + i;
               
            //disable tree
            m_CangJingGeItem[i].m_Left.SetActive(false);
            m_CangJingGeItem[i].m_Right.SetActive(false);
            //show lock
            if (nRealTie > nCount)
            {
                m_CangJingGeItem[i].m_PlayerLeft.spriteName = m_sLockedFloor;
                m_CangJingGeItem[i].m_PlayerRight.spriteName = m_sLockedFloor;
                //m_CangJingGeItem[i].m_tierLeft.gameObject.SetActive(false);
                //m_CangJingGeItem[i].m_tierRight.gameObject.SetActive(false);
            }
            else if (nRealTie == m_nTierOnPlayer)
			{
				string spriteName = GetArrowSpriteName();
				m_CangJingGeItem[i].m_PlayerLeft.spriteName = spriteName;
				m_CangJingGeItem[i].m_PlayerRight.spriteName = spriteName;
			}
            else
            {
                m_CangJingGeItem[i].m_PlayerLeft.spriteName = "";
                m_CangJingGeItem[i].m_PlayerRight.spriteName = "";
                //m_CangJingGeItem[i].m_tierLeft.gameObject.SetActive(true);
                //m_CangJingGeItem[i].m_tierRight.gameObject.SetActive(true);
            }
            //show tier name
            if (nRealTie > 100 )
            {
                //m_CangJingGeItem[i].m_tierLeft.text = "未开启";
                //m_CangJingGeItem[i].m_tierRight.text = "未开启";
                m_CangJingGeItem[i].m_tierLeft.text = StrDictionary.GetClientDictionaryString("#{2789}");
                m_CangJingGeItem[i].m_tierRight.text = StrDictionary.GetClientDictionaryString("#{2789}");
                m_CangJingGeItem[i].m_LevelLeft.text = "";
                m_CangJingGeItem[i].m_LevelRight.text = "";
            }
            else
            {
                //m_CangJingGeItem[i].m_tierLeft.text = nRealTie.ToString() + "层";
                //m_CangJingGeItem[i].m_tierRight.text = nRealTie.ToString() + "层";
                m_CangJingGeItem[i].m_tierLeft.text = StrDictionary.GetClientDictionaryString("#{2788}", nRealTie);
                m_CangJingGeItem[i].m_tierRight.text = StrDictionary.GetClientDictionaryString("#{2788}", nRealTie);
                Tab_CangJingGeInfo cjg = TableManager.GetCangJingGeInfoByID(nRealTie, 0);
                if (cjg != null)
                {
                    m_CangJingGeItem[i].m_LevelLeft.text = StrDictionary.GetClientDictionaryString("#{3099}", cjg.Level);
                    m_CangJingGeItem[i].m_LevelRight.text = StrDictionary.GetClientDictionaryString("#{3099}", cjg.Level);
                }
            }
            
            m_CangJingGeItem[i].m_nTier = nRealTie;
            Vector3 pos = m_CangJingGeItem[i].gameObject.transform.localPosition;
			pos.y = 152 * i;
			m_CangJingGeItem[i].gameObject.transform.localPosition = pos;
            if (nRealTie % 2 == 1)
            {
                m_CangJingGeItem[i].m_Right.SetActive(true);
            }
            else
            {
                m_CangJingGeItem[i].m_Left.SetActive(true);
            }
        }
    }

	string GetArrowSpriteName()
	{
		int profession = GameManager.gameManager.PlayerDataPool.Profession;
		switch (profession)
		{
		case 0:
			return "ui_activity_31";
		case 1:
			return "ui_activity_32";
		case 2:
			return "ui_activity_33";
		case 3:
			return "ui_activity_30";
		default:
			return "";
		}
	}

    public void UpdateInfo()
    {
        m_Yanbao.text = "0";
        if (GameManager.gameManager.PlayerDataPool.CommonData.GetCommonData((int)Games.UserCommonData.USER_COMMONDATA.CD_COPYSCENE_CANGJINGGE_SWEEP) >= GameManager.gameManager.PlayerDataPool.CommonData.GetCopySceneMultiple(CopySceneId))
        {
            m_Yanbao.text = "10";//StrDictionary.GetClientDictionaryString("#{2100}", 10);
        }
        int _Tier = GameManager.gameManager.PlayerDataPool.CommonData.GetCommonData((int)Games.UserCommonData.USER_COMMONDATA.CD_COPYSCENE_CANGJINGGE_TIER);
        int _Sweep = GameManager.gameManager.PlayerDataPool.CommonData.GetCommonData((int)Games.UserCommonData.USER_COMMONDATA.CD_COPYSCENE_CANGJINGGE_SWEEP);
		int nMul = GameManager.gameManager.PlayerDataPool.CommonData.GetCopySceneMultiple(CopySceneId);

        //m_Sweep.text = StrDictionary.GetClientDictionaryString("#{2088}", (nMul * 3 - _Sweep).ToString() + "/" + (nMul * 3).ToString()) + strVipSweep;
        if (_Sweep >= nMul)
        {
            m_Sweep.text = (nMul * 3 - _Sweep).ToString() + "/" + (nMul * 3 - nMul).ToString();
            m_ObjectSweepLabel1.SetActive(true);
            m_ObjectSweepLabel2.SetActive(false);
        }
        else
        {
            m_ObjectSweepLabel1.SetActive(false);
            m_ObjectSweepLabel2.SetActive(true);
        }
        
        m_Tier.text = StrDictionary.GetClientDictionaryString("#{2089}", _Tier);
       
        m_DropIconSprite1.spriteName = "";
        m_DropIconSprite2.spriteName = "";
        m_DropIconSprite3.spriteName = "";
        m_RewardIconSprite1.spriteName = "";
        m_RewardIconSprite2.spriteName = "";
        m_RewardIconSprite3.spriteName = "";
        Tab_CangJingGeInfo pCangJingGeInfo = TableManager.GetCangJingGeInfoByID(_Tier, 0);
        if (pCangJingGeInfo == null)
        {
            return;
        }
        m_CalculatValue.text = StrDictionary.GetClientDictionaryString("#{2090}", pCangJingGeInfo.Calculat);
        m_MonsterValue1.text = pCangJingGeInfo.GetMonsterbyIndex(0).ToString();
        m_MonsterValue2.text = pCangJingGeInfo.GetMonsterbyIndex(1).ToString();
        m_MonsterValue3.text = pCangJingGeInfo.GetMonsterbyIndex(2).ToString();

        Tab_CommonItem pItem = TableManager.GetCommonItemByID(pCangJingGeInfo.GetDropbyIndex(0), 0);
        if (pItem != null)
        {
            m_DropIconSprite1.spriteName = pItem.Icon.ToString();
            m_nDropItemID1 = pItem.Id;
        }
		else
		{
			m_DropIconSprite1.gameObject.SetActive(false);
		}

        pItem = TableManager.GetCommonItemByID(pCangJingGeInfo.GetDropbyIndex(1), 0);
        if (pItem != null)
        {
            m_DropIconSprite2.spriteName = pItem.Icon.ToString();
            m_nDropItemID2 = pItem.Id;
		}
		else
		{
			m_DropIconSprite2.gameObject.SetActive(false);
		}
		
		pItem = TableManager.GetCommonItemByID(pCangJingGeInfo.GetDropbyIndex(2), 0);
        if (pItem != null)
        {
            m_DropIconSprite3.spriteName = pItem.Icon.ToString();
            m_nDropItemID3 = pItem.Id;
        }
		else
		{
			m_DropIconSprite3.gameObject.SetActive(false);
		}

        pItem = TableManager.GetCommonItemByID(pCangJingGeInfo.GetRewardbyIndex(0), 0);
        if (pItem != null)
        {
            m_RewardIconSprite1.spriteName = pItem.Icon.ToString();
        }

        pItem = TableManager.GetCommonItemByID(pCangJingGeInfo.GetRewardbyIndex(1), 0);
        if (pItem != null)
        {
            m_RewardIconSprite2.spriteName = pItem.Icon.ToString();
        }

        pItem = TableManager.GetCommonItemByID(pCangJingGeInfo.GetRewardbyIndex(2), 0);
        if (pItem != null)
        {
            m_RewardIconSprite3.spriteName = pItem.Icon.ToString();
        }
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
        //藏经阁 不分组队难度,所以取0下标
        Tab_CopySceneRule pCopySceneRule = TableManager.GetCopySceneRuleByID(pCopyScene.GetRulebyIndex(0), 0);
        if (pCopySceneRule == null)
        {
            return;
        }
        int ExtraNum = GameManager.gameManager.PlayerDataPool.CommonData.GetCopySceneExtraNumber(CopySceneId, 1);
        int nTabNum = pCopySceneRule.Number;
        int nNum = GameManager.gameManager.PlayerDataPool.CommonData.GetCopySceneNumber(CopySceneId, 1);
        m_TiaoZhanCount.text = (nTabNum * nMul + ExtraNum - nNum).ToString() + "/" + (nTabNum * nMul ).ToString();
        for (int i = 0; i < m_CangJingGeItem.Length; i++ )
        {
            m_CangJingGeItem[i].m_Left.SetActive(false);
            m_CangJingGeItem[i].m_Right.SetActive(false);
            if (i== 0 )
            {
				string spriteName = GetArrowSpriteName();
				m_CangJingGeItem[i].m_PlayerLeft.spriteName = spriteName;
				m_CangJingGeItem[i].m_PlayerRight.spriteName = spriteName;  
                //m_CangJingGeItem[i].m_PlayerLeft.spriteName = "OrangeStar";
                //m_CangJingGeItem[i].m_PlayerRight.spriteName = "OrangeStar";                
            }
            else
            {
                m_CangJingGeItem[i].m_PlayerLeft.spriteName = "";
                m_CangJingGeItem[i].m_PlayerRight.spriteName = "";
            }
            
            //m_CangJingGeItem[i].m_tierLeft.text = (i+1).ToString()+"层";
            //m_CangJingGeItem[i].m_tierRight.text = (i+1).ToString() + "层";

            m_CangJingGeItem[i].m_tierLeft.text = StrDictionary.GetClientDictionaryString("#{2788}", i+1);
            m_CangJingGeItem[i].m_tierRight.text = StrDictionary.GetClientDictionaryString("#{2788}", i+1);
            Tab_CangJingGeInfo cjg = TableManager.GetCangJingGeInfoByID(i + 1, 0);
            m_CangJingGeItem[i].m_LevelLeft.text = StrDictionary.GetClientDictionaryString("#{3099}", cjg.Level);
            m_CangJingGeItem[i].m_LevelRight.text = StrDictionary.GetClientDictionaryString("#{3099}", cjg.Level);

            if (i%2==0)
            {
                m_CangJingGeItem[i].m_Right.SetActive(true);
            }

            else
            {
                m_CangJingGeItem[i].m_Left.SetActive(true);
            }
        }
        if (GameManager.gameManager.PlayerDataPool.StartSweep)
        {
            m_nTierOnPlayer = GameManager.gameManager.PlayerDataPool.CangJIngGeTier + (int)(Time.realtimeSinceStartup - GameManager.gameManager.PlayerDataPool.CangJIngGeSecond);
            GameManager.gameManager.PlayerDataPool.CangJIngGeSecond = Time.realtimeSinceStartup;
        }
        else
        {
            m_nTierOnPlayer = _Tier;
        }
        UpdateTierShow(_Tier);
       
    }
    public void OnOpenCopyScene(int nSceneId)
    {
        CopySceneId = nSceneId;
        //UIManager.LoadItem(UIInfo.CangJingGeItem, OnLoadTierItem);
    }
//     void OnLoadTierItem(GameObject resItem, object param)
//     {
//         if (resItem == null)
//         {
//             return;
//         }
//         Utils.CleanGrid(ItemParent);
// 
//         for (int i = 1; i <= 5; i++)
//         {
//             GameObject newItem = Utils.BindObjToParent(resItem, ItemParent);
//             newItem.GetComponent<CangJingGeItem>().m_tier.text = i.ToString();
//         }
//         ItemParent.GetComponent<UIGrid>().repositionNow = true;
//     }
    public void OnOpenCopySceneClick()
    {
        if (m_NewPlayerGuide_Step == 1)
        {
            NewPlayerGuidLogic.CloseWindow();
            m_NewPlayerGuide_Step = -1;
        }
        if (Singleton<ObjManager>.GetInstance() == null)
        {
            return;
        }
        if (Singleton<ObjManager>.Instance.MainPlayer.IsInJianYu())
        {
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false,"#{2216}");
            return;
        }
//         int nReset = GameManager.gameManager.PlayerDataPool.CommonData.GetCopySceneReset(CopySceneId,1);

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
        //藏经阁 不分组队难度,所以取0下标
        Tab_CopySceneRule pCopySceneRule = TableManager.GetCopySceneRuleByID(pCopyScene.GetRulebyIndex(0), 0);
        if (pCopySceneRule == null)
        {
            return;
        }
        //是否超过上限次数
        int nMul = GameManager.gameManager.PlayerDataPool.CommonData.GetCopySceneMultiple(CopySceneId);
        int ExtraNum = GameManager.gameManager.PlayerDataPool.CommonData.GetCopySceneExtraNumber(CopySceneId, 1);
        if (pCopySceneRule.Number * nMul + ExtraNum <= GameManager.gameManager.PlayerDataPool.CommonData.GetCopySceneNumber(CopySceneId, 1))
        {            
            int nReset = GameManager.gameManager.PlayerDataPool.CommonData.GetCopySceneReset(CopySceneId,1);
            if (nReset >= 10)   //重置10次以上,不让重置
            {
                Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false,"#{2631}");
                return;
            }
            int nYuanBao = 10 * (int)System.Math.Pow(2,nReset);
            string dicStr = StrDictionary.GetClientDictionaryString("#{2632}", nYuanBao);
            MessageBoxLogic.OpenOKCancelBox(dicStr, "", OnCopySceneResetOK, OnCopySceneResetNO);
            return;
        }   
        Singleton<ObjManager>.GetInstance().MainPlayer.SendOpenScene(CopySceneId, 1, 1);
    }
    public void OnCopySceneResetOK()
    {
        CG_COPYSCENERESET packet = (CG_COPYSCENERESET)PacketDistributed.CreatePacket(MessageID.PACKET_CG_COPYSCENERESET);
        packet.NSceneClassID = CopySceneId;
        packet.Type = 1;
        packet.SendPacket();
    }
    public void OnCopySceneResetNO()
    {

    }
    public void OnSweepClick()
    {
        if (Singleton<ObjManager>.Instance.MainPlayer.IsInJianYu())
        {
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2216}");
            return;
        }
        int _Sweep = GameManager.gameManager.PlayerDataPool.CommonData.GetCommonData((int)Games.UserCommonData.USER_COMMONDATA.CD_COPYSCENE_CANGJINGGE_SWEEP);
        int nMul = GameManager.gameManager.PlayerDataPool.CommonData.GetCopySceneMultiple(CopySceneId);
        if (_Sweep >= nMul * 3)  //超上限了
        {
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2084}");
            return;
        }
        if (_Sweep >= nMul)
        {
            //string dicStr = "扣除10元宝确定么?";
            string dicStr = StrDictionary.GetClientDictionaryString("#{2790}");     
            MessageBoxLogic.OpenOKCancelBox(dicStr, "", OnSweepOK, OnSweepNO);
            return;
        }

        OnSweepOK();
    }
    public void OnSweepOK()
    {
        CG_ASK_COPYSCENE_SWEEP packet = (CG_ASK_COPYSCENE_SWEEP)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_COPYSCENE_SWEEP);
        packet.SceneID = CopySceneId;
        packet.SendPacket();
    }
    public void OnSweepNO()
    {

    }
    public void OnDropItemClicked(GameObject obj)
    {
        int nShowID = -1;
        if (obj.name == "DropItem1")
            nShowID = m_nDropItemID1;
        else if (obj.name == "DropItem2")
            nShowID = m_nDropItemID2;
        else if (obj.name == "DropItem3")
            nShowID = m_nDropItemID3;
        else
            return;

        GameItem item = new GameItem();
        item.DataID = nShowID;
        if (item.IsEquipMent())
        {
            EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Info);
        }
        else
        {
            ItemTooltipsLogic.ShowItemTooltip(item, ItemTooltipsLogic.ShowType.Info);
        }
    }
    public void OnRankClick()
    {
        //GUIData.AddNotifyData("#{2129}");
        //UIManager.ShowUI(UIInfo.RankRoot);
        //RankWindow.Instance().ChangeTabTableau("Tab1");
        CG_ASK_RANK scoreRankPak = (CG_ASK_RANK)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_RANK);
        scoreRankPak.NType = (int)GameDefine_Globe.RANKTYPE.TYPE_CANGJINGGE;
        scoreRankPak.NPage = 0;
        scoreRankPak.SendPacket();
    }
    private float m_fSweepBuffer = Time.realtimeSinceStartup;
    public void StartSweep()
    {
        if (Singleton<ObjManager>.GetInstance() == null)
        {
            return;
        }
        GameManager.gameManager.PlayerDataPool.StartSweep = true;
        GameManager.gameManager.PlayerDataPool.CangJIngGeSecond = Time.realtimeSinceStartup;
        m_fSweepBuffer = Time.realtimeSinceStartup;
        int _Tier = GameManager.gameManager.PlayerDataPool.CommonData.GetCommonData((int)Games.UserCommonData.USER_COMMONDATA.CD_COPYSCENE_CANGJINGGE_TIER);

        for (int i = 0; i < m_CangJingGeItem.Length; i++)
        {
            //disable all
            m_CangJingGeItem[i].m_Left.SetActive(false);
            m_CangJingGeItem[i].m_Right.SetActive(false);
            //show tier 
            m_CangJingGeItem[i].m_nTier = i + 1;

            //m_CangJingGeItem[i].m_tierLeft.text = (i + 1).ToString() + "层";
           // m_CangJingGeItem[i].m_tierRight.text = (i + 1).ToString() + "层";

            m_CangJingGeItem[i].m_tierLeft.text = StrDictionary.GetClientDictionaryString("#{2788}", i+1);
            m_CangJingGeItem[i].m_tierRight.text = StrDictionary.GetClientDictionaryString("#{2788}", i+1);

            Tab_CangJingGeInfo cjg = TableManager.GetCangJingGeInfoByID(i + 1, 0);
            m_CangJingGeItem[i].m_LevelLeft.text = StrDictionary.GetClientDictionaryString("#{3099}", cjg.Level);
            m_CangJingGeItem[i].m_LevelRight.text = StrDictionary.GetClientDictionaryString("#{3099}", cjg.Level);

			m_CangJingGeItem[i].m_PlayerLeft.spriteName = "";
			m_CangJingGeItem[i].m_PlayerRight.spriteName = "";

			if (i == 0)
			{
				//===========
				string spriteName = GetArrowSpriteName();
				m_CangJingGeItem[i].m_PlayerLeft.spriteName = spriteName;
				m_CangJingGeItem[i].m_PlayerRight.spriteName = spriteName;
				//m_CangJingGeItem[i].m_PlayerLeft.spriteName = "Arrow";
				//m_CangJingGeItem[i].m_PlayerRight.spriteName = "Arrow";
			}

            if (m_CangJingGeItem[i].m_nTier > _Tier)
            {
                m_CangJingGeItem[i].m_PlayerLeft.spriteName = m_sLockedFloor;
                m_CangJingGeItem[i].m_PlayerRight.spriteName = m_sLockedFloor;
            }
            //set pos
            Vector3 pos = m_CangJingGeItem[i].gameObject.transform.localPosition;
            pos.y = 152 * i;
            m_CangJingGeItem[i].gameObject.transform.localPosition = pos;
            if (i % 2 == 0)
            {
                m_CangJingGeItem[i].m_Right.SetActive(true);
            }
            else
            {
                m_CangJingGeItem[i].m_Left.SetActive(true);
            }
        }        
    }
    public void UpdateSweep()
    {
        //刷新CD时间
        string str = "";
        int CDTime = GameManager.gameManager.PlayerDataPool.CJGSweepCDTime;
         int _Sweep = GameManager.gameManager.PlayerDataPool.CommonData.GetCommonData((int)Games.UserCommonData.USER_COMMONDATA.CD_COPYSCENE_CANGJINGGE_SWEEP);
        int nMul = GameManager.gameManager.PlayerDataPool.CommonData.GetCopySceneMultiple(CopySceneId);
        if (CDTime <= 0 || _Sweep >= nMul * 3 )
        {
            str = "";
            m_SweepCDTime.text = "";
        }
        else
        {
            //if (CDTime / 60 > 0)
            {
                str = (CDTime / 60).ToString() + ":";
            }
            str += (CDTime % 60).ToString();
            m_SweepCDTime.text = StrDictionary.GetClientDictionaryString("#{2552}", str);
        }
        if (GameManager.gameManager.PlayerDataPool.StartSweep)
        {
            if (Time.realtimeSinceStartup - m_fSweepBuffer < 1 )
            {
                return;
            }
           // float _CangJIngGeSecond = Time.realtimeSinceStartup;
            int _Tier = GameManager.gameManager.PlayerDataPool.CommonData.GetCommonData((int)Games.UserCommonData.USER_COMMONDATA.CD_COPYSCENE_CANGJINGGE_TIER);
            float deltaTime = Time.deltaTime;  
            for (int i = 0; i < m_CangJingGeItem.Length; i++)
            {
                Vector3 pos = m_CangJingGeItem[i].gameObject.transform.localPosition;
                pos.y -= 80 * deltaTime;            
                if (pos.y <= -220)
                {
                    pos.y = 150 + (220 + 320 +221 + pos.y);
                    GameManager.gameManager.PlayerDataPool.CangJIngGeTier = m_CangJingGeItem[i].m_nTier + 1;
                    m_CangJingGeItem[i].m_nTier += 6;                   
                    if (m_CangJingGeItem[i].m_nTier > 100)
                    {
                        //m_CangJingGeItem[i].m_tierLeft.text = "未开启";
                        //m_CangJingGeItem[i].m_tierRight.text = "未开启";

                        m_CangJingGeItem[i].m_tierLeft.text = StrDictionary.GetClientDictionaryString("#{2789}");
                        m_CangJingGeItem[i].m_tierRight.text = StrDictionary.GetClientDictionaryString("#{2789}");
                        m_CangJingGeItem[i].m_LevelLeft.text = "";
                        m_CangJingGeItem[i].m_LevelRight.text = "";
                    }
                    else
                    {
                        //m_CangJingGeItem[i].m_tierLeft.text = (m_CangJingGeItem[i].m_nTier).ToString() + "层";
                       // m_CangJingGeItem[i].m_tierRight.text = (m_CangJingGeItem[i].m_nTier).ToString() + "层";

                        m_CangJingGeItem[i].m_tierLeft.text = StrDictionary.GetClientDictionaryString("#{2788}", m_CangJingGeItem[i].m_nTier);
                        m_CangJingGeItem[i].m_tierRight.text = StrDictionary.GetClientDictionaryString("#{2788}", m_CangJingGeItem[i].m_nTier);
                        Tab_CangJingGeInfo cjg = TableManager.GetCangJingGeInfoByID(m_CangJingGeItem[i].m_nTier, 0);
                        m_CangJingGeItem[i].m_LevelLeft.text = StrDictionary.GetClientDictionaryString("#{3099}", cjg.Level);
                        m_CangJingGeItem[i].m_LevelRight.text = StrDictionary.GetClientDictionaryString("#{3099}", cjg.Level);
                    }
                   

					m_CangJingGeItem[i].m_PlayerLeft.spriteName = "";
					m_CangJingGeItem[i].m_PlayerRight.spriteName = "";

                    if (m_CangJingGeItem[i].m_nTier > _Tier)
                    {
                        m_CangJingGeItem[i].m_PlayerLeft.spriteName = m_sLockedFloor;
                        m_CangJingGeItem[i].m_PlayerRight.spriteName = m_sLockedFloor;
                    }
                }
                else if (pos.y <= 0)
                {
                    if (m_CangJingGeItem[i].m_nTier < _Tier)
                    {
                        m_CangJingGeItem[i].m_PlayerLeft.spriteName = "";
                        m_CangJingGeItem[i].m_PlayerRight.spriteName = "";
                        if (i == m_CangJingGeItem.Length - 1)
                        {
							//===========
							string spriteName = GetArrowSpriteName();
							m_CangJingGeItem[0].m_PlayerLeft.spriteName = spriteName;
							m_CangJingGeItem[0].m_PlayerRight.spriteName = spriteName;
                            //m_CangJingGeItem[0].m_PlayerLeft.spriteName = "Arrow";
                            //m_CangJingGeItem[0].m_PlayerRight.spriteName = "Arrow";
                        }
                        else
                        {
							//===========
							string spriteName = GetArrowSpriteName();
							m_CangJingGeItem[i+1].m_PlayerLeft.spriteName = spriteName;
							m_CangJingGeItem[i+1].m_PlayerRight.spriteName = spriteName;
                            //m_CangJingGeItem[i + 1].m_PlayerLeft.spriteName = "Arrow";
                            //m_CangJingGeItem[i + 1].m_PlayerRight.spriteName = "Arrow";
                        }
                    }
                    else
                    {
                        GameManager.gameManager.PlayerDataPool.StartSweep = false;
                        GameManager.gameManager.PlayerDataPool.CangJIngGeTier = 0;
                    }
                     
                }
                m_CangJingGeItem[i].gameObject.transform.localPosition = pos;               
            }
            GameManager.gameManager.PlayerDataPool.CangJIngGeSecond = Time.realtimeSinceStartup;
        }
    }

    public void NewPlayerGuide()
    {
		NewPlayerGuidLogic.CloseWindow ();

        m_NewPlayerGuide_Step = 1;

		if (m_BtnTiaoZhan != null)
        {
            NewPlayerGuidLogic.OpenWindow(m_BtnTiaoZhan, 180, 60, "", "right", 2, true, true);
        }
    }
}