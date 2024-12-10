using Games.GlobeDefine;
using UnityEngine;
using System.Collections;
using Module.Log;
using GCGame;
using System;
using GCGame.Table;

public class RankWindow : MonoBehaviour {
    public TabController m_TabController;

    //.. 
    public RankItem[] m_RankItem;
    
    // ...
    public UILabel[] m_RankItemTitle;

    // .. 我的排名数值
    public UILabel m_MeRank;
    public UILabel m_MeRankDesc;
    //..
    public UIImageButton NextImageButton;
    public UIImageButton PrevImageButton;

    // ..
    public UILabel m_Page;
    private int m_nPage = 0;
    private bool m_IsPage = true;
    private int m_nRankType = 1;

    private int m_nTipWaitWindow = 0;
    public bool isRankDataReturn { set; get; }

    //..
    public TabController m_ProfessionReputationController;

    //...
    public UIGrid m_TitleGrid;
    public UITopGrid m_ItemTopGrid;

    public GameObject m_ChongZhiTab;
    public UILabel m_ChongZhiTime;

    //..
    public GameObject m_RepRootObject;
    public UILabel m_RepValue;

    //..
    public GameObject m_RankRewardDescRootObject;
    public UILabel m_RankRewardDescLabel;
    public GameObject m_RankRewardDescBtn;

    private static RankWindow m_Instance = null;
    public static RankWindow Instance()
    {
        return m_Instance;
    }

    //..
    void Awake()
    {
        m_nTipWaitWindow = 0;
        m_Instance = this;
        m_TabController.delTabChanged = OnTabTableau;
        m_nPage = 0;
        SetPageText();
        m_MeRank.text = "？";
        m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_CANGJINGGE;
        m_ProfessionReputationController.delTabChanged = OnTabProfession;
        m_ProfessionReputationController.gameObject.SetActive(false);
        ClearRankData();
        CleanTitle();
        if (m_ChongZhiTab != null)
        {
            if (GameManager.gameManager.PlayerDataPool.OpenChongZhiRank)
            {
                m_ChongZhiTab.SetActive(true);
            }
            else
            {
                m_ChongZhiTab.SetActive(false);
            }
        }       
    }

    void SetPageText()
    {
        m_Page.text = (m_nPage + 1).ToString() + "/" + m_nTotalPage.ToString();
    }

    void OnDestroy()
    {
        m_Instance = null;
    }

    //..
    public void OnRankRewardButtonClick()
    {
        if (m_RankRewardDescRootObject != null)
        {
            m_RankRewardDescRootObject.SetActive(true);
        }

        if (m_RankRewardDescLabel != null)
        {
            m_RankRewardDescLabel.text = "";

            switch (m_nRankType)
            {
                case (int)GameDefine_Globe.RANKTYPE.TYPE_CANGJINGGE://1//藏经阁副本
                    m_RankRewardDescLabel.text = Utils.GetDicByID(3198);
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_PRELIMINARYGUILDWARRANK: //2,//帮战海选排行
                    m_RankRewardDescLabel.text = Utils.GetDicByID(3199);

                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_PRELIMINARYGUILDWARKILLRANK: //3,//帮战海选 击杀人数排行
                    m_RankRewardDescLabel.text = Utils.GetDicByID(3200);
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_HUASHANZHANJI: // 4,//华山-战绩
                    m_RankRewardDescLabel.text = Utils.GetDicByID(3201);
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_HUASHANJINYAODAI: // 5,//华山-金腰带
                    m_RankRewardDescLabel.text = Utils.GetDicByID(3202);
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_USERLEVELRANK: // 6, //等级
                    m_RankRewardDescLabel.text = Utils.GetDicByID(3203);
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_USERCOMBATRANK: // 7, //战斗力
                    m_RankRewardDescLabel.text = Utils.GetDicByID(3204);
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_USERHPRANK: //8,//血量
                    m_RankRewardDescLabel.text = Utils.GetDicByID(3205);
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_GUILDCOMBAT: //9,//帮会战斗力
                    m_RankRewardDescLabel.text = Utils.GetDicByID(3206);
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_HUASHANPOS: //10,//华山-排名
                    m_RankRewardDescLabel.text = Utils.GetDicByID(3207);
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_SHAOLINREPUTATION: //11, //少林大弟子
                case (int)GameDefine_Globe.RANKTYPE.TYPE_TIANSHANREPUTATION: // 12,//天山大弟子
                case (int)GameDefine_Globe.RANKTYPE.TYPE_DALIREPUTATION: // 13,//大理大弟子
                case (int)GameDefine_Globe.RANKTYPE.TYPE_XIAOYAOREPUTATION: // 14,//逍遥大弟子
                    m_RankRewardDescLabel.text = Utils.GetDicByID(3208);
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_USERCOIN: // 15,//金币
                    m_RankRewardDescLabel.text = Utils.GetDicByID(3209);
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_MASTER: // 16,//宗师
                    m_RankRewardDescLabel.text = Utils.GetDicByID(3210);
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_USERCOSTYUANBAO: // 17,//消费排行榜
                    m_RankRewardDescLabel.text = Utils.GetDicByID(3211);
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_ATTACK: //18,//攻击力排行榜
                    m_RankRewardDescLabel.text = Utils.GetDicByID(3212);
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_CHONGZHI: // 19,//充值排行榜
                    m_RankRewardDescLabel.text = Utils.GetDicByID(3213);
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_MASTERAVTIVECOMBAT: // 20,//师门战力排行榜
                    m_RankRewardDescLabel.text = Utils.GetDicByID(3214);
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_SHAOSHISHAN: // 21,//少室山排行榜
                    m_RankRewardDescLabel.text = Utils.GetDicByID(3215);
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_TOTALONLINETIME: // 22,//在线时间排行榜
                    m_RankRewardDescLabel.text = Utils.GetDicByID(3216);
                    break;
                default:
                    break;
            }
        }
    }
    public void OnRankRewardHideButtonClick()
    {
        if (m_RankRewardDescRootObject != null)
        {
            m_RankRewardDescRootObject.SetActive(false);
        }
    }
    //..
    public bool IsRankType(int type)
    {
        return (m_nRankType == type);
    }

    //..
     public void OnTabProfession(TabButton button)
    {
		if (button.name == "TabP1")//少林大弟子
        {
            m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_SHAOLINREPUTATION;
        }
		else if (button.name == "TabP2") // 天山大弟子
        {
            m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_TIANSHANREPUTATION;
        }
		else if (button.name == "TabP3") // 大理大弟子
        {
            m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_DALIREPUTATION;
        }
		else if (button.name == "TabP4") // 逍遥大弟子
        {
            m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_XIAOYAOREPUTATION;
        }
        m_nPage = 0;
        if (isRankDataReturn == false)
        {
            SendRankPack();
        }

        SelfReputationControl(true);
        //SetRankItemTitleOnChangeTable("Tab9");
    }

    //..
    public void ChangeTabTableau(string Tab)
    {
        if (null != m_TabController)
        {
            m_TabController.ChangeTab(Tab);
        }
    }

    public void SelfReputationControl(bool show)
    {
        if (m_RepRootObject != null)
        {
            m_RepRootObject.SetActive(show);
        }

        if (m_RepValue != null && show)
        {
            PlayerData playerDataPool = GameManager.gameManager.PlayerDataPool;
            if (playerDataPool != null)
            {
                m_RepValue.text = playerDataPool.Reputation.ToString();
            }
        }
    }

    public void OnTabTableau(TabButton button)
    {
        m_ProfessionReputationController.gameObject.SetActive(false);
        m_ChongZhiTime.gameObject.SetActive(false);
        if (m_RankRewardDescRootObject != null)
        {
            m_RankRewardDescRootObject.SetActive(false);
        }
        //if (button.name == "Tab1")
        //{
        //    m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_CANGJINGGE; 
        //}
        //else if (button.name == "Tab2") // 战绩榜
        //{
        //    m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_HUASHANZHANJI; 
        //}
        //else if (button.name == "Tab3") // 金腰带
        //{
        //    m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_HUASHANJINYAODAI; 
        //}
        //else if (button.name == "Tab4") // 等级
        //{
        //    m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_USERLEVELRANK; 
        //}
        //else if (button.name == "Tab5") // 战力
        //{
        //    m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_USERCOMBATRANK; 
        //}
        //else if (button.name == "Tab6") // 血量
        //{
        //    m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_USERHPRANK; 
        //}
        //else if (button.name == "Tab7") // 帮会战斗力
        //{
        //    m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_GUILDCOMBAT; 
        //}
        //else if (button.name == "Tab8") // 华山-排名
        //{
        //    m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_HUASHANPOS;
        //}
        //else if (button.name == "Tab9") //大弟子排行榜
        //{
        //    m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_SHAOLINREPUTATION;
        //    m_ProfessionReputationController.gameObject.SetActive(true);
        //    m_ProfessionReputationController.ChangeTab("Tab1");
        //}
        //else if (button.name == "Tab10") //金币排行榜
        //{
        //    m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_USERCOIN;
        //}
        //else if (button.name == "Tab11") //宗师排行榜
        //{
        //    m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_MASTER;
        //}
        //else if (button.name == "Tab12") //消费排行榜
        //{
        //    m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_USERCOSTYUANBAO;
        //}
        //else if (button.name == "Tab13") //攻击排行榜
        //{
        //    m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_ATTACK;
        //}
        //else if (button.name == "Tab14") //充值排行榜
        //{
        //    m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_CHONGZHI;
        //}
        //else if (button.name == "Tab15") // 师门战斗力
        //{
        //    m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_MASTERAVTIVECOMBAT;
        //}
        //else if (button.name == "Tab16") // 少室山排行榜
        //{
        //    m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_SHAOSHISHAN;
        //}

        SelfReputationControl(false);

        if (button.name == "Tab1") //等级
        {
            m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_USERLEVELRANK;
        }
        else if (button.name == "Tab2") // 战力
        {
            m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_USERCOMBATRANK;
        }
        else if (button.name == "Tab3") // 攻击
        {
            m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_ATTACK;
        }
        else if (button.name == "Tab4") // 血量
        {
            m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_USERHPRANK;
        }
        else if (button.name == "Tab5") // 金币
        {
            m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_USERCOIN;
        }
        else if (button.name == "Tab6") // 充值
        {
            m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_CHONGZHI;
            m_ChongZhiTime.gameObject.SetActive(true);
        }
        else if (button.name == "Tab7") // 消费
        {
            m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_USERCOSTYUANBAO;
        }
        else if (button.name == "Tab8") // 华山-排名
        {
            m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_HUASHANPOS;
        }
        else if (button.name == "Tab9") //金腰带
        {
            m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_HUASHANJINYAODAI;
        }
        else if (button.name == "Tab10") //战绩排行榜
        {
            m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_HUASHANZHANJI;
        }
        else if (button.name == "Tab11") //藏经阁排行榜
        {
            m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_CANGJINGGE;
        }
        else if (button.name == "Tab12") //帮会实力排行榜
        {
            m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_GUILDCOMBAT;
        }
        else if (button.name == "Tab13") //宗师排行榜
        {
            m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_MASTER;
        }
        else if (button.name == "Tab14") //大弟子排行榜
        {
            m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_SHAOLINREPUTATION;
            m_ProfessionReputationController.gameObject.SetActive(true);
            m_ProfessionReputationController.ChangeTab("Tab1");
        }
        else if (button.name == "Tab15") // 师门战斗力
        {
            m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_MASTERAVTIVECOMBAT;
        }
        else if (button.name == "Tab16") // 少室山排行榜
        {
            m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_SHAOSHISHAN;
        }
        else if (button.name == "Tab17") // 在线时间排行榜
        {
            m_nRankType = (int)GameDefine_Globe.RANKTYPE.TYPE_TOTALONLINETIME;
        }

        m_nPage = 0;
        if (isRankDataReturn == false)
        {
            SendRankPack();
        }
        
        SetRankItemTitleOnChangeTable(button.name);
        SetRewardBtnShow();
    }

    void SetRewardBtnShow( )
    {
        if (m_RankRewardDescBtn != null)
        {

            switch (m_nRankType)
            {
                case (int)GameDefine_Globe.RANKTYPE.TYPE_CANGJINGGE://1//藏经阁副本
                case (int)GameDefine_Globe.RANKTYPE.TYPE_PRELIMINARYGUILDWARRANK: //2,//帮战海选排行
                case (int)GameDefine_Globe.RANKTYPE.TYPE_PRELIMINARYGUILDWARKILLRANK: //3,//帮战海选 击杀人数排行
                case (int)GameDefine_Globe.RANKTYPE.TYPE_HUASHANZHANJI: // 4,//华山-战绩
                case (int)GameDefine_Globe.RANKTYPE.TYPE_HUASHANJINYAODAI: // 5,//华山-金腰带
                case (int)GameDefine_Globe.RANKTYPE.TYPE_GUILDCOMBAT: //9,//帮会战斗力
                case (int)GameDefine_Globe.RANKTYPE.TYPE_MASTERAVTIVECOMBAT: // 20,//师门战力排行榜
                case (int)GameDefine_Globe.RANKTYPE.TYPE_SHAOSHISHAN: // 21,//少室山排行榜
                    m_RankRewardDescBtn.SetActive(false);
                    break;
                case (int)GameDefine_Globe.RANKTYPE.TYPE_USERLEVELRANK: // 6, //等级
                case (int)GameDefine_Globe.RANKTYPE.TYPE_USERCOMBATRANK: // 7, //战斗力
                case (int)GameDefine_Globe.RANKTYPE.TYPE_USERHPRANK: //8,//血量
                case (int)GameDefine_Globe.RANKTYPE.TYPE_HUASHANPOS: //10,//华山-排名
                case (int)GameDefine_Globe.RANKTYPE.TYPE_SHAOLINREPUTATION: //11, //少林大弟子
                case (int)GameDefine_Globe.RANKTYPE.TYPE_TIANSHANREPUTATION: // 12,//天山大弟子
                case (int)GameDefine_Globe.RANKTYPE.TYPE_DALIREPUTATION: // 13,//大理大弟子
                case (int)GameDefine_Globe.RANKTYPE.TYPE_XIAOYAOREPUTATION: // 14,//逍遥大弟子
                case (int)GameDefine_Globe.RANKTYPE.TYPE_USERCOIN: // 15,//金币
                case (int)GameDefine_Globe.RANKTYPE.TYPE_MASTER: // 16,//宗师
                case (int)GameDefine_Globe.RANKTYPE.TYPE_USERCOSTYUANBAO: // 17,//消费排行榜
                case (int)GameDefine_Globe.RANKTYPE.TYPE_ATTACK: //18,//攻击力排行榜
                case (int)GameDefine_Globe.RANKTYPE.TYPE_CHONGZHI: // 19,//充值排行榜            
                case (int)GameDefine_Globe.RANKTYPE.TYPE_TOTALONLINETIME: // 22,//在线时间排行榜
                    m_RankRewardDescBtn.SetActive(true);
                    break;
                default:
                    break;
            }
        }
    }

    public void SetTitleNum(int n)
    {
        if (m_TitleGrid != null)
        {
            m_TitleGrid.cellWidth = 600 / n;
            m_TitleGrid.repositionNow = true;
        }
    }


    public void SetRankItemTitleOnChangeTable( string name)
    {
        CleanTitle();
        ClearRankData();
        m_MeRankDesc.text = Utils.GetDicByID(2049);
        if (name == "Tab1") //等级
        {
            //SetItemTitleTxt(0, 2041);
            //SetItemTitleTxt(1, 2042);
            //SetItemTitleTxt(2, 2044);
            //SetItemTitleTxt(3, 2043);
            //SetItemTitleTxt(4, 2045);
            //SetItemTitleTxt(5, 2046);
            //SetTitleNum(6);
            //SetItemLabelNum(6);

            SetItemTitleTxt(0, 2041);
            SetItemTitleTxt(1, 2042);
            SetItemTitleTxt(2, 2044);
            SetTitleNum(3);
            SetItemLabelNum(3);
        }
        else if (name == "Tab2") //战力排行榜
        {
            // 战绩榜
            //SetItemTitleTxt(0, 2041);
            //SetItemTitleTxt(1, 2042);
            //SetItemTitleTxt(2, 2056);
            //SetTitleNum(3);
            //SetItemLabelNum(3);

            SetItemTitleTxt(0, 2041);
            SetItemTitleTxt(1, 2042);
            SetItemTitleTxt(2, 2048);
            SetTitleNum(3);
            SetItemLabelNum(3);
        }
        else if (name == "Tab3") //攻击排行榜
        {
            // 金腰带  
            //SetItemTitleTxt(0, 2041);
            //SetItemTitleTxt(1, 2042);
            //SetItemTitleTxt(2, 2057);
            //SetTitleNum(3);
            //SetItemLabelNum(3);

            SetItemTitleTxt(0, 2041);
            SetItemTitleTxt(1, 2042);
            SetItemTitleTxt(2, 2713);
            SetTitleNum(3);
            SetItemLabelNum(3);
        }
        else if (name == "Tab4") //  血量排行榜
        {
            //等级
            //SetItemTitleTxt(0, 2041);
            //SetItemTitleTxt(1, 2042);
            //SetItemTitleTxt(2, 2044);
            //SetTitleNum(3);
            //SetItemLabelNum(3);

            SetItemTitleTxt(0, 2041);
            SetItemTitleTxt(1, 2042);
            SetItemTitleTxt(2, 2058);
            SetTitleNum(3);
            SetItemLabelNum(3);

        }
        else if (name == "Tab5") // 金币排行榜
        {
            // 战力
            //SetItemTitleTxt(0, 2041);
            //SetItemTitleTxt(1, 2042);
            //SetItemTitleTxt(2, 2048);
            //SetTitleNum(3);
            //SetItemLabelNum(3);

            SetItemTitleTxt(0, 2041);
            SetItemTitleTxt(1, 2042);
            SetItemTitleTxt(2, 1324);
            SetTitleNum(3);
            SetItemLabelNum(3);
        }
        else if (name == "Tab6") //充值排行帮
        {
            // 血量 
            //SetItemTitleTxt(0, 2041);
            //SetItemTitleTxt(1, 2042);
            //SetItemTitleTxt(2, 2058);
            //SetTitleNum(3);
            //SetItemLabelNum(3);

            SetItemTitleTxt(0, 2041);
            SetItemTitleTxt(1, 2042);
            SetItemTitleTxt(2, 2766);
            SetTitleNum(3);
            SetItemLabelNum(3);
            SetChongZhiTime();

        }
        else if (name == "Tab7") // 消费排行榜
        {
            // 帮会战斗力
            //SetItemTitleTxt(0, 2041);
            //SetItemTitleTxt(1, 1065);
            //SetItemTitleTxt(2, 2048);
            //SetTitleNum(3);
            //SetItemLabelNum(3);

            SetItemTitleTxt(0, 2041);
            SetItemTitleTxt(1, 2042);
            SetItemTitleTxt(2, 2528);
            SetTitleNum(3);
            SetItemLabelNum(3);

        }
        else if (name == "Tab8") // 华山-排名
        {
            SetItemTitleTxt(0, 2041);
            SetItemTitleTxt(1, 2042);
            SetTitleNum(2);
            SetItemLabelNum(3);
        }
        else if (name == "Tab9")//金腰带排行榜
        {
            //大弟子排行榜 
            //SetItemTitleTxt(0, 2041);
            //SetItemTitleTxt(1, 2042);
            //SetItemTitleTxt(2, 2473);
            //SetTitleNum(3);
            //SetItemLabelNum(3);

            SetItemTitleTxt(0, 2041);
            SetItemTitleTxt(1, 2042);
            SetItemTitleTxt(2, 2057);
            SetTitleNum(3);
            SetItemLabelNum(3);
        }
        else if (name == "Tab10") //战绩排行榜
        {
            // 金币 
            //SetItemTitleTxt(0, 2041);
            //SetItemTitleTxt(1, 2042);
            //SetItemTitleTxt(2, 1324);
            //SetTitleNum(3);
            //SetItemLabelNum(3);

            SetItemTitleTxt(0, 2041);
            SetItemTitleTxt(1, 2042);
            SetItemTitleTxt(2, 2056);
            SetTitleNum(3);
            SetItemLabelNum(3);
        }
        else if (name == "Tab11") //藏经阁排行榜
        {
            //宗师 
            //SetItemTitleTxt(0, 2041);
            //SetItemTitleTxt(1, 2042);
            //SetItemTitleTxt(2, 2510);
            //SetItemTitleTxt(3, 2511);
            //SetTitleNum(4);
            //SetItemLabelNum(4);

            SetItemTitleTxt(0, 2041);
            SetItemTitleTxt(1, 2042);
            SetItemTitleTxt(2, 2044);
           // SetItemTitleTxt(3, 2043);
            SetItemTitleTxt(3, 2045);
            SetItemTitleTxt(4, 2046);
            SetTitleNum(5);
            SetItemLabelNum(5);
        }
        else if (name == "Tab12") //帮会实力榜
        {
             //消费排行榜
            //SetItemTitleTxt(0, 2041);
            //SetItemTitleTxt(1, 2042);
            //SetItemTitleTxt(2, 2528);
            //SetTitleNum(3);
            //SetItemLabelNum(3);

            SetItemTitleTxt(0, 2041);
            SetItemTitleTxt(1, 1065);
            SetItemTitleTxt(2, 2048);
            SetTitleNum(3);
            SetItemLabelNum(3);
        }
		else if (name == "Tab13") //宗师排行榜
		{
            // 攻击  
            //SetItemTitleTxt(0, 2041);
            //SetItemTitleTxt(1, 2042);
            //SetItemTitleTxt(2, 2713);
            //SetTitleNum(3);
            //SetItemLabelNum(3);

            SetItemTitleTxt(0, 2041);
            SetItemTitleTxt(1, 2042);
            SetItemTitleTxt(2, 2510);
            SetItemTitleTxt(3, 2511);
            SetTitleNum(4);
            SetItemLabelNum(4);
		}
        else if (name == "Tab14") //门派大弟子
        {
            //充值 
            //SetItemTitleTxt(0, 2041);
            //SetItemTitleTxt(1, 2042);
            //SetItemTitleTxt(2, 2766);
            //SetTitleNum(3);
            //SetItemLabelNum(3);

            SetItemTitleTxt(0, 2041);
            SetItemTitleTxt(1, 2042);
            SetItemTitleTxt(2, 2473);
            SetTitleNum(3);
            SetItemLabelNum(3);
        }
        else if (name == "Tab15") // 师门战斗力
        {
            SetItemTitleTxt(0, 2041);
            SetItemTitleTxt(1, 1464);
            SetItemTitleTxt(2, 2048);
            SetTitleNum(3);
            SetItemLabelNum(3);
        }
        else if (name == "Tab16") //少室山排行榜
        {
            SetItemTitleTxt(0, 2041);
            SetItemTitleTxt(1, 2042);
            SetItemTitleTxt(2, 2044);
            SetItemTitleTxt(3, 3040);
            SetItemTitleTxt(4, 2046);
            SetTitleNum(5);
            SetItemLabelNum(5);
        }
        else if (name == "Tab17") //累计在线时间排行榜
        {
            SetItemTitleTxt(0, 2041);
            SetItemTitleTxt(1, 2042);
            SetItemTitleTxt(2, 3185);
            SetTitleNum(3);
            SetItemLabelNum(3);
        }
    }

    public void SetItemTitleTxt(int idx, int dict)
    {
        if (idx < m_RankItemTitle.Length && idx >= 0)
        {
            m_RankItemTitle[idx].text = Utils.GetDicByID(dict);
        }
    }

    public void CleanTitle()
    {
        for (int i = 0; i < m_RankItemTitle.Length; ++i)
        {
            m_RankItemTitle[i].text = "";
        }
    }
    public void SetItemLabelNum(int n)
    {
        for (int i = 0; i < m_RankItem.Length; ++i)
        {
            m_RankItem[i].SetMaxLabel(n);
        }
    }

    public void ClearRankData()
    {
        for (int i = 0; i < m_RankItem.Length; ++i)
        {
            m_RankItem[i].Cleanup();
            m_RankItem[i].gameObject.SetActive(false);
        }
        m_MeRank.text = Utils.GetDicByID(2059);
    }
    //..
    public void OnCloseClick()
    {
        UIManager.CloseUI(UIInfo.RankRoot);
    }
    public int GetPage()
    {
        return m_nPage;
    }



    private int m_nTotalPage = 1;
    public void SetTotalPage( int total, int curPage )
    {
        m_nPage = curPage;
        m_nTotalPage = total;
        if (PrevImageButton != null)
        {
            PrevImageButton.isEnabled = ((m_nPage > 0));
        }
        if (NextImageButton != null)
        {
            NextImageButton.isEnabled = ((m_nPage + 1) < m_nTotalPage);
        }
        SetPageText();
    }

    public int TotalPage
    {
        get { return m_nTotalPage; }
    }

    public void SetIsPage(bool IsPage)
    {
        m_IsPage = IsPage;
    }
    public void OnUpClick()
    {
        if (m_nPage > 0)
        {
            m_nPage--;
            SetPageText();
            SendRankPack();
        }
    }
    public void OnDownClick()
    {
        if ((m_nPage+1) < TotalPage)
        {
            m_nPage++;
            if (m_nPage >= 25)
            {
                m_nPage = 24;
            }
            SetPageText();
            SendRankPack();
        }
    }

    //..
    public void SendRankPack()
    {
        CG_ASK_RANK packet = (CG_ASK_RANK)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_RANK);
        packet.NType = m_nRankType;
        packet.NPage = m_nPage;
        packet.SendPacket();
        if (m_nTipWaitWindow != 0)
        {
            MessageBoxLogic.OpenWaitBox(1290, 1, 0); 
        }
        m_nTipWaitWindow = m_nTipWaitWindow + 1;
    }

    void SetChongZhiTime()
    {
        DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0);
        //开始时间
        DateTime BeginTime = new DateTime(startTime.Ticks + (long)GameManager.gameManager.PlayerDataPool.ChongZhiStartTime * 10000000L, DateTimeKind.Unspecified);
        BeginTime = BeginTime.ToLocalTime();
        //结束时间
        DateTime EndTime = new DateTime(startTime.Ticks + (long)GameManager.gameManager.PlayerDataPool.ChongZhiEndTime * 10000000L, DateTimeKind.Unspecified);
        EndTime = EndTime.ToLocalTime();

        string StrTime = StrDictionary.GetClientDictionaryString("#{3076}", BeginTime.ToString("yyyy/MM/dd HH:mm:ss"), EndTime.ToString("yyyy/MM/dd HH:mm:ss"));

        if (m_ChongZhiTime != null)
        {
            m_ChongZhiTime.text = StrTime;
        }
    }
}
