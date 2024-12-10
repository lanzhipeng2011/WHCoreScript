//********************************************************************
// 文件名: GuildWindow.cs
// 全路径：	\Script\GUI\UIWindow\GuildWindow.cs
// 描述: 帮会界面逻辑
// 作者: lijia
// 创建时间: 2014-04-23
//********************************************************************

using Games.LogicObj;
using UnityEngine;
using System.Collections;
using GCGame;
using Module.Log;
using Games.GlobeDefine;
using System.Collections.Generic;
using System;
using GCGame.Table;
using System.Text.RegularExpressions;
#if UNITY_WP8
using UnityPort;
#endif

public class GuildWindow : MonoBehaviour
{
    public GameObject m_GuildListGrid;              //帮会列表Grid
    public GameObject m_GuildListItem;              //帮会列表项
    public GameObject m_GuildMemberListGrid;        //帮会正式成员列表Grid
    public GameObject m_GuildReserveMemberListGrid; //帮会待审批成员列表Grid
    public GameObject m_GuildMemberListItem;        //帮会列表项

    public TabController m_GuildTabController;      //帮会界面TabController

    //帮会信息控件
    public UILabel m_GuildNameLabel;                //帮会名称
    public UILabel m_LevelLabel;                    //帮会等级
    public UILabel m_MemberNumLabel;                //帮会人数
    public UILabel m_GuildMemberPageLable;          //帮会成员页数
    public UILabel m_GuildExpLabel;                 //帮会繁荣度
    public UILabel m_GuildNoticeLabel;              //帮会公告

	public GameObject m_GuildMaxSizeLabel;             //帮会公告最长提示

    public BoxCollider m_GuildNoticeCollider;       //帮会公告所在Input控件
	public UIImageButton m_CreateGuildBtn;          //帮会创建按钮OnBtnClickChangeNotice
    public UILabel m_GuildPageLable;                //帮会页数
    public GameObject m_GuildReserveRemind;         //帮会新加入待审批人员提醒
    public UILabel m_GuildReserveRemindNum;         //帮会新加入待审批人员人数提醒
    public UIImageButton m_guildRecruitBtn;         //招募成员按钮

    //帮会邮件相关
    public UILabel m_GuildMailLable;                //帮会邮件
    public BoxCollider m_GuildMailCollider;         //帮会邮件所在Input控件
    public Transform m_GuildInfoPlane;              //帮会信息界面
    public Transform m_GuildMailPlane;              //帮会邮件界面

    //帮会商店相关
    private GameObject m_GuildShopItem = null;      //元宝商店物品
    public GameObject m_GuildShopGrid;              //帮会商店Grid
    public UILabel m_GuildContribute;               //本人当前帮贡

    //帮会跑商界面
    public UILabel m_RobNumLable;                   //抢劫数
    public UISprite m_RobNumSprite;                 //抢劫进度条
    public UILabel[] m_GuildRobbedInfo;             //抢劫信息
    public UILabel m_AssignInfoLable;               //可开启信息
    public UILabel m_AcceptInfoLable;               //可接取信息
    public UIImageButton m_AutoAssignButton;        //自动选择
    public GameObject m_ChiefAssignBtn;             //帮主开启按钮
    public GameObject m_OpenGuildMakeBtn;           //帮主生产开启按钮
    public UILabel m_MemberTimeSLable;              //玩家跑商次数信息
    public UILabel m_GuildWealthLable;               //帮会财富
    public UIPanel m_GuildMakePanel;                //帮会生产界面
    public GameObject m_BusinessNum;                //帮会任务提示


    //帮会加成
    public UILabel m_HPAdditionLable;               //帮会生命值加成
    public UILabel m_MPAdditionLable;               //帮会法力值加成
    public UILabel m_APWAdditionLable;               //帮会攻击力值加成
	public UILabel m_APFAdditionLable;               //帮会攻击力值加成
	public UILabel m_DPWAdditionLable;               //帮会防御力值加成
	public UILabel m_DPFAdditionLable;               //帮会防御力值加成
    public UILabel m_HitAdditionLable;              //帮会命中值加成
    public UILabel m_DodgeAdditionLable;            //帮会闪避值加成
    public UILabel m_CriAdditionLable;              //帮会暴击值加成
    public UILabel m_DeCriAdditionLable;            //帮会暴抗值加成
    public UILabel m_CurGuildAdditionLevel;         //当前属性加成的帮会等级
    public GameObject m_ChallenegWarRoot;           //约战选取界面
    public UIToggle m_IsOnlyShowEnemyToggle;      //是否只显示敌对帮会

	public TabButton m_GuildListTabButton1;
	public TabButton m_GuildListTabButton2;

    private TabButton m_GuildInfoTabButton;         //帮会信息分页的按钮
    private TabButton m_GuildPreInfoTabButton;      //帮会待审批分页的按钮
    private TabButton m_GuildWarInfoTabButton;      //帮战分页按钮

    private TabButton m_GuildBusinessButton;        //帮派跑商界面

    private int m_nCurGuildAwardPageNum = 0;         //当先显示的帮会加成分页
    public TabButton GuildWarInfoTabButton
    {
        get { return m_GuildWarInfoTabButton; }
        set { m_GuildWarInfoTabButton = value; }
    }
    private TabButton m_GuildRewardTabButton;       //帮会福利分页按钮

    private int m_nCurGuildListPage;                //帮会列表当前页码
    private int m_nMaxGuildListPage;                //帮会列表最大页
    private const int m_nGuildNumPerPage = 9;       //每页帮会列表数量

    private int m_nCurGuildMemberPage;              //帮会成员当前页码
    private int m_nMaxGuildMemberPage;              //帮会成员最大页
    private const int m_nGuildMemberNumPerPage = 9; //每页帮会成员数量

    private UInt64 m_SelectMemberGuid = GlobeVar.INVALID_GUID;      //当前选中的帮会成员Guid
    public System.UInt64 SelectMemberGuid
    {
        get { return m_SelectMemberGuid; }
    }
    private static GuildWindow m_Instance = null;
    public static GuildWindow Instance()
    {
        return m_Instance;
    }

    private bool m_bIsOnlyShowEnemyGuild =false;
    private UInt64 m_curChallengeGuildGuid = GlobeVar.INVALID_GUID;//当前选择挑战的帮会
    public System.UInt64 CurChallengeGuildGuid
    {
        get { return m_curChallengeGuildGuid; }
        set { m_curChallengeGuildGuid = value; }
    }
    void Awake()
    {
        if (m_GuildTabController)
        {
            m_GuildTabController.delTabChanged = OnTabChanged;
        }
        m_Instance = this;
        m_nCurGuildAwardPageNum = 1;
    }

    void Start()
    {
        m_GuildInfoPlane.gameObject.SetActive(true);
        m_GuildMailPlane.gameObject.SetActive(false);
        m_IsOnlyShowEnemyToggle.value = m_bIsOnlyShowEnemyGuild;
        m_ChallenegWarRoot.SetActive(false);
        
        //在OnEnable中有可能无法读取分页按钮，这里重新进行一次赋值
        if (null == m_GuildInfoTabButton && null == m_GuildPreInfoTabButton)
        {
            if (null != m_GuildTabController)
            {
                m_GuildInfoTabButton = m_GuildTabController.GetTabButton("Tab01_GuildInfo");
                m_GuildPreInfoTabButton = m_GuildTabController.GetTabButton("Tab03_GuildResMem");
                m_GuildWarInfoTabButton = m_GuildTabController.GetTabButton("Tab04_GuildWar");
                m_GuildRewardTabButton = m_GuildTabController.GetTabButton("Tab05_GuildReward");

                m_GuildBusinessButton = m_GuildTabController.GetTabButton("Tab06_GuildBusiness");
            }

            //如果无帮会则直接切换到帮会列表界面
            if (!GameManager.gameManager.PlayerDataPool.IsHaveGuild() ||
                GameManager.gameManager.PlayerDataPool.IsReserveGuildMember())
            {
                if (null != m_GuildTabController)
                {
                    if (null != m_GuildTabController && null != m_GuildPreInfoTabButton)
                    {
                        m_GuildTabController.ChangeTab("Tab02_GuildList");
                        //没有帮会的玩家，隐藏这两个选项
						m_GuildListTabButton1.gameObject.SetActive(true);
						m_GuildListTabButton2.gameObject.SetActive(false);
                        m_GuildInfoTabButton.gameObject.SetActive(false);
                        m_GuildPreInfoTabButton.gameObject.SetActive(false);
                        //m_GuildRewardTabButton.gameObject.SetActive(false);           // 没帮会的玩家也可以看到福利内容但是按钮不可以点击
                        m_GuildTabController.SortTabGrid();
                    }

                    return;
                }
            }
            else
            {
                if (null != m_GuildInfoTabButton && null != m_GuildPreInfoTabButton)
				{
					m_GuildInfoTabButton.gameObject.SetActive(true);
					m_GuildListTabButton1.gameObject.SetActive(false);
					m_GuildListTabButton2.gameObject.SetActive(true);
					UInt64 myGuid = Singleton<ObjManager>.GetInstance().MainPlayer.GUID;
                    bool bIsGuildViceChief = (GameManager.gameManager.PlayerDataPool.IsGuildChief()
                            || GameManager.gameManager.PlayerDataPool.IsGuildViceChief(myGuid));
                    m_GuildPreInfoTabButton.gameObject.SetActive(bIsGuildViceChief);
                    m_GuildWarInfoTabButton.gameObject.SetActive(true);
                    m_GuildRewardTabButton.gameObject.SetActive(true);
                    m_GuildBusinessButton.gameObject.SetActive(true);

                    m_GuildTabController.SortTabGrid();
                }
            }
        }
    }

    void OnDestroy()
    {
        m_Instance = null;
    }
   

    void OnEnable()
    {
        GUIData.delGuildDataUpdate += UpdateData;
        GUIData.delGuildMemberSelectChange += OnSelectMemberChange;
        if (m_GuildTabController)
        {
            m_GuildInfoTabButton = m_GuildTabController.GetTabButton("Tab01_GuildInfo");
            m_GuildPreInfoTabButton = m_GuildTabController.GetTabButton("Tab03_GuildResMem");
            m_GuildWarInfoTabButton = m_GuildTabController.GetTabButton("Tab04_GuildWar");
            m_GuildRewardTabButton = m_GuildTabController.GetTabButton("Tab05_GuildReward");

            m_GuildBusinessButton = m_GuildTabController.GetTabButton("Tab06_GuildBusiness");
        }

        //如果无帮会则直接切换到帮会列表界面
        if (!GameManager.gameManager.PlayerDataPool.IsHaveGuild() ||
            GameManager.gameManager.PlayerDataPool.IsReserveGuildMember())
        {
            if (null != m_GuildTabController && null != m_GuildPreInfoTabButton)
            {
                m_GuildTabController.ChangeTab("Tab02_GuildList");
                //没有帮会的玩家，隐藏这两个选项
                m_GuildInfoTabButton.gameObject.SetActive(false);
                m_GuildPreInfoTabButton.gameObject.SetActive(false);
                //m_GuildRewardTabButton.gameObject.SetActive(false);
                m_GuildTabController.SortTabGrid();
                return;
            }
        }
        else
        {
            if (null != m_GuildInfoTabButton && null != m_GuildPreInfoTabButton)
            {
                m_GuildInfoTabButton.gameObject.SetActive(true);
                m_GuildPreInfoTabButton.gameObject.SetActive(true);
                m_GuildWarInfoTabButton.gameObject.SetActive(true);
                m_GuildRewardTabButton.gameObject.SetActive(true);

                m_GuildBusinessButton.gameObject.SetActive(true);

                m_GuildTabController.SortTabGrid();
            }
        }
        //向服务器申请更新好友列表
        if (null != m_GuildTabController)
        {
            m_GuildTabController.ChangeTab("Tab01_GuildInfo");
        }
    }

    void OnDisable()
    {
        GUIData.delGuildDataUpdate -= UpdateData;
        GUIData.delGuildMemberSelectChange -= OnSelectMemberChange;
    }

    public void UpdateGuildBusinessInfo()
    {
        if (false == GameManager.gameManager.PlayerDataPool.IsHaveGuild())
        {
            m_BusinessNum.gameObject.SetActive(false);
            return;
        }

        int gbGotTime = GameManager.gameManager.PlayerDataPool.CommonData.GetCommonData((int)Games.UserCommonData.USER_COMMONDATA.CD_GUILDBUSINESS_GOTTEN_NUM);
        if (gbGotTime < 2 && GameManager.gameManager.PlayerDataPool.GuildInfo.GBCanAcceptTime > 0 && null != m_BusinessNum)
        {
            m_BusinessNum.gameObject.SetActive(true);
            return;
        }
        m_BusinessNum.gameObject.SetActive(false);
    }
    void UpdateData()
    {
        GameObject curTab = m_GuildTabController.GetHighlightTab().gameObject;

        //如果点击帮会列表分页
        if (curTab.name == "Tab02_GuildList")
        {
            //显示帮会列表
            ShowGuildList();
        }

        //如果点击帮会信息分页
        if (curTab.name == "Tab01_GuildInfo")
        {
            
            //清理正式成员Grid
            Utils.CleanGrid(m_GuildMemberListGrid);

            //如果发现需要申请，则发送申请消息
            if (null != Singleton<ObjManager>.GetInstance().MainPlayer &&
                Singleton<ObjManager>.GetInstance().MainPlayer.NeedRequestGuildInfo)
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.ReqGuildInfo();
            }
            else
            {
                //否则直接显示帮会信息
                ShowGuildInfo();
            }
        }
        
        //如果点击帮会预备成员分页
        if (curTab.name == "Tab03_GuildResMem")
        {
            //由于玩家打开帮会界面的时候会显示申请帮会信息，所以这里直接读取，而不申请
            ShowGuildReserveMemberList();
        }
        //如果点击的是帮战战况按钮
        if (curTab.name == "Tab04_GuildWar")
        {
         //   ShowGuildWarPremilinaryInfo();
        }

        //点击帮会福利按钮
        if (curTab.name == "Tab05_GuildReward")
        {
            UIManager.LoadItem(UIInfo.GuildShopItem, LoadGuildShopItemOver);
            //更新帮会加成信息
            if (!GameManager.gameManager.PlayerDataPool.IsHaveGuild() ||
               GameManager.gameManager.PlayerDataPool.IsReserveGuildMember())
            {
                ShowGuildAttrAddition(1);
            }
            else
            {
                ShowGuildAttrAddition(GameManager.gameManager.PlayerDataPool.GuildInfo.GuildLevel);
            }
        }

        //点击帮会跑商界面
        if (curTab.name == "Tab06_GuildBusiness")
        {
			ClearGuildBusinessInfo();
            //如果发现需要申请，则发送申请消息
            if (null != Singleton<ObjManager>.GetInstance().MainPlayer &&
                Singleton<ObjManager>.GetInstance().MainPlayer.NeedReqGuildBusinessInfo)
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.ReqGuildBusinessInfo();
            }
        }

        //点击帮会任务界面
        if (curTab.name == "Tab07_GuildMission")
        {
            //如果发现需要申请，则发送申请消息
            if (null != Singleton<ObjManager>.GetInstance().MainPlayer &&
                Singleton<ObjManager>.GetInstance().MainPlayer.NeedReqGuildMissionInfo)
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.ReqGuildMissionInfo();
            }
        }
    }

	
	public void ChangeTabGuildMission()
	{
		if (false == GameManager.gameManager.PlayerDataPool.IsHaveGuild())
		{
			Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{3193}");
			return;
		}
		
		m_GuildTabController.ChangeTab("Tab07_GuildMission");
	}

    //加载帮会福利商店界面OK
    public void LoadGuildShopItemOver(GameObject resItem, object param)
    {
        //判断是否创建成功
        if (null == resItem)
        {
            LogModule.ErrorLog("load YuanBaoShopItem error");
            return;
        }

        m_GuildShopItem = resItem;

        //显示帮贡
        UpdateGuildShopContribute();
        
        //更新商品信息
        UpdateGuildShopGoodsInfo();
    }

    public void UpdateGuildShopContribute()
    {
        GameObject curTab = m_GuildTabController.GetHighlightTab().gameObject;
        if (curTab.name != "Tab05_GuildReward")
        {
            return;
        }

        if (null != m_GuildContribute)
        {
            GuildMember mainPlayerGuildInfo = GameManager.gameManager.PlayerDataPool.GuildInfo.GetMainPlayerGuildInfo();
            if (null != mainPlayerGuildInfo)
            {
                m_GuildContribute.text = mainPlayerGuildInfo.Contribute.ToString();
            }
        }
    }

    void UpdateGuildShopGoodsInfo()
    {
        Utils.CleanGrid(m_GuildShopGrid);

        if (null == m_GuildShopItem)
        {
            return;
        }

        int index = 0;
        int[] guildMakeItemIdS = new int[TableManager.GetGuildMake().Count];
        for (int i = 1; i <= TableManager.GetGuildMake().Count; ++i)
        {
            Tab_GuildMake tabMake = TableManager.GetGuildMakeByID(i, 0);
            if (null == tabMake)
            {
                continue;
            }

            guildMakeItemIdS[index] = tabMake.CommonItemId;

            // 加载对应商品
            GameObject GuildShopItem = Utils.BindObjToParent(m_GuildShopItem, m_GuildShopGrid, index.ToString());
            if (GuildShopItem == null)
            {
                continue;
            }

            if (null != GuildShopItem.GetComponent<GuildShopItemLogic>())
                GuildShopItem.GetComponent<GuildShopItemLogic>().InitGuildMakeGoods(tabMake);
            index++;
        }

        for (int i = 0; i < TableManager.GetGuildShop().Count; i++)
        {
            Tab_GuildShop tabGuildShop = TableManager.GetGuildShopByID(i, 0);
            if (tabGuildShop == null) { continue; }

            bool isInit = true;
            for (int k = 0; k < guildMakeItemIdS.Length; ++k)
            {
                if (guildMakeItemIdS[k] == tabGuildShop.ItemID)
                {
                    isInit = false;
                    continue;
                }
            }

            if (false == isInit) { continue; }

            // 加载对应商品
            GameObject GuildShopItem = Utils.BindObjToParent(m_GuildShopItem, m_GuildShopGrid, index.ToString());
            if (GuildShopItem == null)
            {
                continue;
            }

            if (null != GuildShopItem.GetComponent<GuildShopItemLogic>())
                GuildShopItem.GetComponent<GuildShopItemLogic>().Init(tabGuildShop);
            index++;
        }
        m_GuildShopGrid.GetComponent<UIGrid>().Reposition();
        m_GuildShopGrid.GetComponent<UITopGrid>().Recenter(true);
    }

    //显示帮会加成数值
    void ShowGuildAttrAddition(int nLevel)
    {
        //int nGuildLevel = GameManager.gameManager.PlayerDataPool.GuildInfo.GuildLevel;
        m_nCurGuildAwardPageNum = nLevel;
        int nPlayerLevel = Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.Level;
        Tab_GuildAttrAddition addition = TableManager.GetGuildAttrAdditionByID(m_nCurGuildAwardPageNum, 0);
        if (null == addition)
        {
            return;
        }

        //更新页眉显示
        if (null != m_CurGuildAdditionLevel)
        {
            //XX级帮会加成
            m_CurGuildAdditionLevel.text = StrDictionary.GetClientDictionaryString("#{3022}", nLevel);
        }

        //由于目前有10项属性加成，所以要判断数量
        if (addition.getBaseCount() == 10 && addition.getGrowthCount() == 10 && addition.getAddtionPercentCount() == 10)
        {
            if (null != m_HPAdditionLable)
            {
                int nHPRefix = (int)((addition.GetBasebyIndex(0) + nPlayerLevel * addition.GetGrowthbyIndex(0)) * addition.GetAddtionPercentbyIndex(0));
                m_HPAdditionLable.text = StrDictionary.GetClientDictionaryString("#{2400}", nHPRefix);
            }
            if (null != m_MPAdditionLable)
            {
                int nMPRefix = (int)((addition.GetBasebyIndex(1) + nPlayerLevel * addition.GetGrowthbyIndex(1)) * addition.GetAddtionPercentbyIndex(1));
                m_MPAdditionLable.text = StrDictionary.GetClientDictionaryString("#{2401}", nMPRefix);
            }
            if (null != m_APWAdditionLable)
            {
                int nPhyAtk = (int)((addition.GetBasebyIndex(2) + nPlayerLevel * addition.GetGrowthbyIndex(2)) * addition.GetAddtionPercentbyIndex(2));
				m_APWAdditionLable.text = StrDictionary.GetClientDictionaryString("#{2402}", nPhyAtk);
            }

			if(null != m_APFAdditionLable)
			{
				int nMagAtk = (int)((addition.GetBasebyIndex(3) + nPlayerLevel * addition.GetGrowthbyIndex(3)) * addition.GetAddtionPercentbyIndex(3));
				m_APFAdditionLable.text = StrDictionary.GetClientDictionaryString("#{5653}", nMagAtk);
			}
			if (null != m_DPWAdditionLable)
			{
				int nPhyDef = (int)((addition.GetBasebyIndex(4) + nPlayerLevel * addition.GetGrowthbyIndex(4)) * addition.GetAddtionPercentbyIndex(4));
				m_DPWAdditionLable.text = StrDictionary.GetClientDictionaryString("#{2403}", nPhyDef);
			}
			if(null != m_DPFAdditionLable)
			{
				int nMagDef = (int)((addition.GetBasebyIndex(5) + nPlayerLevel * addition.GetGrowthbyIndex(5)) * addition.GetAddtionPercentbyIndex(5));
				m_DPFAdditionLable.text = StrDictionary.GetClientDictionaryString("#{5654}", nMagDef);
			}
            
            if (null != m_HitAdditionLable)
            {
                int nHit = (int)((addition.GetBasebyIndex(6) + nPlayerLevel * addition.GetGrowthbyIndex(6)) * addition.GetAddtionPercentbyIndex(6));
                m_HitAdditionLable.text = StrDictionary.GetClientDictionaryString("#{2405}", nHit);
            }
            if (null != m_DodgeAdditionLable)
            {
                int nDodge = (int)((addition.GetBasebyIndex(7) + nPlayerLevel * addition.GetGrowthbyIndex(7)) * addition.GetAddtionPercentbyIndex(7));
                m_DodgeAdditionLable.text = StrDictionary.GetClientDictionaryString("#{2406}", nDodge);
            }
            if (null != m_CriAdditionLable)
            {
                int nCri = (int)((addition.GetBasebyIndex(8) + nPlayerLevel * addition.GetGrowthbyIndex(8)) * addition.GetAddtionPercentbyIndex(8));
                m_CriAdditionLable.text = StrDictionary.GetClientDictionaryString("#{2407}", nCri);
            }
            if (null != m_DeCriAdditionLable)
            {
                int nDecri = (int)((addition.GetBasebyIndex(9) + nPlayerLevel * addition.GetGrowthbyIndex(9)) * addition.GetAddtionPercentbyIndex(9));
                m_DeCriAdditionLable.text = StrDictionary.GetClientDictionaryString("#{2408}", nDecri);
            }
        }
    }
        
    // 切换标签页响应
    void OnTabChanged(TabButton curButton)
    {
        UpdateData();
    }
    
    //显示帮会列表，启动Bundle加载
    public void ShowGuildList()
    {
        //清空帮会列表
        Utils.CleanGrid(m_GuildListGrid);

        //根据等级处理创建帮会按钮
        if (null != Singleton<ObjManager>.GetInstance().MainPlayer && null != m_CreateGuildBtn)
        {
            if (Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.Level >= GlobeVar.CREATE_GUILD_LEVEL)
            {
                m_CreateGuildBtn.gameObject.SetActive(true);
            }
            else
            {
                m_CreateGuildBtn.gameObject.SetActive(false);
            }
        }

        // 不是帮主不显示招募按钮
        m_guildRecruitBtn.gameObject.SetActive(GameManager.gameManager.PlayerDataPool.IsGuildChief());
        //没有帮会不显示 显示敌对帮会选项
        if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid != GlobeVar.INVALID_GUID)
        {
            m_IsOnlyShowEnemyToggle.gameObject.SetActive(true);
        }
        else
        {
            m_IsOnlyShowEnemyToggle.gameObject.SetActive(false);
            m_bIsOnlyShowEnemyGuild = false;
        }
        //如果发现需要申请，则发送申请消息
        if (null != Singleton<ObjManager>.GetInstance().MainPlayer &&
            Singleton<ObjManager>.GetInstance().MainPlayer.NeedRequestGuildList)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.ReqGuildList();
            return;
        }

        //进入Bundle加载过程
        //UIManager.LoadItem(UIInfo.GuildListItem, OnLoadGuildListItem);
        OnLoadGuildListItem(m_GuildListItem, null);
    }

    //Bundle加载帮会列表结束
    void OnLoadGuildListItem(GameObject resItem, object param)
    {
        if (null == resItem)
        {
            LogModule.ErrorLog("load Guild List item fail");
            return;
        }

        Utils.CleanGrid(m_GuildListGrid);

        List<GuildPreviewInfo> list = GameManager.gameManager.PlayerDataPool.guildList.GuildInfoList;
        if (null == list)
        {
            return;
        }

        //计算最大页数和当前页数
        m_nCurGuildListPage = 1;
        if (m_bIsOnlyShowEnemyGuild)
        {
            int nEnemyCount = 0;
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].IsEnemyGuild)
                {
                    nEnemyCount++;
                }
            }
            m_nMaxGuildListPage = (int)((nEnemyCount - 1)/ m_nGuildNumPerPage) + 1;
        }
        else
        {
            m_nMaxGuildListPage = (int)((list.Count - 1) / m_nGuildNumPerPage) + 1;
        }
        m_GuildPageLable.text = string.Format("{0}/{1}", m_nCurGuildListPage, m_nMaxGuildListPage);

        //填充数据
        int nGuildListStartIndex = (m_nCurGuildListPage - 1) * m_nGuildNumPerPage;
        int nSelectCount =0;
        for (; nGuildListStartIndex < list.Count; ++nGuildListStartIndex)
        {
            if (nSelectCount>=m_nGuildNumPerPage)
            {
                break;
            }
            if (list.Count > nGuildListStartIndex)
            {
                GuildPreviewInfo info = list[nGuildListStartIndex];
                //如果需要筛选 敌对帮会 则跳过非敌对的帮会
                if (m_bIsOnlyShowEnemyGuild && info.IsEnemyGuild == false)
                {
                    continue;
                }
                if (info.GuildGuid != GlobeVar.INVALID_GUID)
                {
                    GameObject newGuildListItem = Utils.BindObjToParent(resItem, m_GuildListGrid, nGuildListStartIndex.ToString());
                    if (null != newGuildListItem && null != newGuildListItem.GetComponent<GuildListItemLogic>())
                        newGuildListItem.GetComponent<GuildListItemLogic>().Init(info, nGuildListStartIndex + 1);
                    nSelectCount++;
                }
            }
        }

        //Grid排序，防止列表异常
        m_GuildListGrid.GetComponent<UIGrid>().Reposition();
        m_GuildListGrid.GetComponent<UITopGrid>().Recenter(true);

        //根据等级处理创建帮会按钮
        if (null != Singleton<ObjManager>.GetInstance().MainPlayer && null != m_CreateGuildBtn)
        {
            if (Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.Level >= GlobeVar.CREATE_GUILD_LEVEL)
            {
                m_CreateGuildBtn.gameObject.SetActive(true);
            }
            else
            {
                m_CreateGuildBtn.gameObject.SetActive(false);
            }
        }
    }

    void ClickAskChallangeWarBt()
    {
        string dicStr = StrDictionary.GetClientDictionaryString("#{2592}");
        MessageBoxLogic.OpenOKCancelBox(dicStr, "", AgreeChallengeGuildWar, null);
    }
    void AgreeChallengeGuildWar()
    {
        //发帮战切磋请求包
        CG_ASK_CHALLENGEGUILDWAR Pak = (CG_ASK_CHALLENGEGUILDWAR)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_CHALLENGEGUILDWAR);
        Pak.Guildguid = m_curChallengeGuildGuid;
        Pak.SendPacket();
    }
    void ClickAskWildWarBt()
    {
        string dicStr = StrDictionary.GetClientDictionaryString("#{3169}");
        MessageBoxLogic.OpenOKCancelBox(dicStr, "", AgreeWildGuildWar, null);
    }
    void AgreeWildGuildWar()
    {
        //发野外宣战请求包
        CG_ASK_GUILDWILDWAR Pak = (CG_ASK_GUILDWILDWAR)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_GUILDWILDWAR);
        Pak.TargetGuildGuid = m_curChallengeGuildGuid;
        Pak.SendPacket();
    }
    public void ShowChallengeRoot()
    {
        m_ChallenegWarRoot.SetActive(true);
    }
    void CloseChallengeRoot()
    {
        m_ChallenegWarRoot.SetActive(false);
    }
    void ClearGuildInfo()
    {
        if (null != m_LevelLabel)
        {
            m_LevelLabel.text = ""; 
        }
        if (null != m_MemberNumLabel)
        {
            m_MemberNumLabel.text = "";
        }

        if (null != m_GuildExpLabel)
        {
            m_GuildExpLabel.text = "";
        }

        if (null != m_GuildNoticeLabel)
        {
			m_GuildNoticeLabel.text = "";
        }

        Utils.CleanGrid(m_GuildMemberListGrid);
    }

    //显示帮会信息，帮会信息由于没有其他Prefab，所以无需Bundle加载，直接读取数据
    public void ShowGuildInfo()
    {
        ClearGuildInfo();
        if (!GameManager.gameManager.PlayerDataPool.IsHaveGuild())
        {
            return;
        }

        Guild info = GameManager.gameManager.PlayerDataPool.GuildInfo;
        if (null != m_GuildNameLabel)
        {
            m_GuildNameLabel.text = info.GuildName;
        }
        if (null != m_LevelLabel)
        {
            //帮会等级:XX
            m_LevelLabel.text = StrDictionary.GetClientDictionaryString("#{1738}", info.GuildLevel);     
        }
        if (null != m_MemberNumLabel)
        {
            //帮会成员XX/XX
            m_MemberNumLabel.text = StrDictionary.GetClientDictionaryString("#{1739}", info.GetGuildFormalMemberCount(), GlobeVar.GetGuildMemberMax(info.GuildLevel));  
        }

        if (null != m_GuildExpLabel)
        {
			if(info.GuildLevel == GlobeVar.MAX_GUILD_LVL)
			{
				m_GuildExpLabel.text = StrDictionary.GetClientDictionaryString("#{6005}");
			}
			else
			{
            	//繁荣度 XX/XX
            	m_GuildExpLabel.text = StrDictionary.GetClientDictionaryString("#{1855}", info.GuildExp.ToString(), GlobeVar.GetGuildExpMax(info.GuildLevel).ToString());
			}
		}

        if (null != m_GuildNoticeLabel)
        {
            m_GuildNoticeLabel.text = info.GuildNotice;
        }

        //由于玩家打开帮会界面的时候会显示申请帮会信息，所以这里直接读取，而不申请
        ShowGuildMemberList();

        UpdateGuildReserveRemindNum();

        UpdateGuildBusinessInfo();
    }
    
    //显示帮会正式成员列表，启动Bundle加载
    void ShowGuildMemberList()
    {
        //清理正式成员Grid
        Utils.CleanGrid(m_GuildMemberListGrid);

        //进入Bundle加载过程
        //UIManager.LoadItem(UIInfo.GuildMemberListItem, OnLoadGuildMemberItem);
        OnLoadGuildMemberItem(m_GuildMemberListItem, 1);
    }
    private void OnLoadGuildMemberItem(GameObject resItem, int nPage)
    {
        if (null == resItem)
        {
            return;
        }
        
        m_nCurGuildMemberPage = nPage;
        
        //获取正式会员数量
        int nMemberCount = 0;
        foreach (KeyValuePair<UInt64, GuildMember> memberPair in GameManager.gameManager.PlayerDataPool.GuildInfo.GuildMemberList)
        {
            GuildMember member = memberPair.Value;
            if (member.Guid != GlobeVar.INVALID_GUID && member.Job != (int)GameDefine_Globe.GUILD_JOB.RESERVE)
            {
                nMemberCount++;
            }
        }

        //计算最大页数和当前页数
        m_nMaxGuildMemberPage = (int)((nMemberCount - 1) / m_nGuildMemberNumPerPage) + 1;

        m_GuildMemberPageLable.text = string.Format("{0}/{1}", m_nCurGuildMemberPage, m_nMaxGuildMemberPage);

        Utils.CleanGrid(m_GuildMemberListGrid);

        int nGuildMemberStartIndex = (m_nCurGuildMemberPage - 1) * m_nGuildMemberNumPerPage;
        //填充数据
        int nIndex = 0;
        foreach (KeyValuePair<UInt64, GuildMember> memberPair in GameManager.gameManager.PlayerDataPool.GuildInfo.GuildMemberList)
        {
            //从nGuildMemberStartIndex开始显示，之前的全部过滤掉
            if (nIndex < nGuildMemberStartIndex)
            {
                nIndex++;
                continue;
            }

            //显示玩家内容
            GuildMember member = memberPair.Value;
            if (member.Guid != GlobeVar.INVALID_GUID && member.Job != (int)GameDefine_Globe.GUILD_JOB.RESERVE)
            {
                GameObject newGuildListItem = Utils.BindObjToParent(resItem, m_GuildMemberListGrid, nIndex.ToString());
                if (null != newGuildListItem && null != newGuildListItem.GetComponent<GuildMemberListItemLogic>())
                {
                    newGuildListItem.GetComponent<GuildMemberListItemLogic>().Init(member, false);
                    nIndex++;
                }
            }

            //如果此时已经显示够了一页，则退出遍历
            if (nIndex >= nGuildMemberStartIndex + m_nGuildMemberNumPerPage)
                break;
        }

        m_GuildMemberListGrid.GetComponent<UIGrid>().Reposition();
        m_GuildMemberListGrid.GetComponent<UITopGrid>().Recenter(true);
    }

    ////帮会成员信息Bundle加载结束
    //void OnLoadGuildMemberItem(GameObject resItem, object param)
    //{
    //    if (null == resItem)
    //    {
    //        LogModule.ErrorLog("load Guild Info item fail");
    //        return;
    //    }

    //    Utils.CleanGrid(m_GuildMemberListGrid);


    //    //计算最大页数和当前页数
    //    m_nCurGuildMemberPage = 1;
    //    int nMemberCount = GameManager.gameManager.PlayerDataPool.GuildInfo.GuildMemberList.Count;
    //    m_nMaxGuildMemberPage = (int)(nMemberCount / m_nGuildMemberNumPerPage) + 1;

    //    m_GuildMemberPageLable.text = string.Format("{0}/{1}", m_nCurGuildMemberPage, m_nMaxGuildMemberPage);

    //    //填充数据
    //    int nGuildMemberListItemIndex = 0;
    //    foreach (KeyValuePair<UInt64, GuildMember> memberPair in GameManager.gameManager.PlayerDataPool.GuildInfo.GuildMemberList)
    //    {
    //        GuildMember member = memberPair.Value;
    //        if (member.Guid != GlobeVar.INVALID_GUID && member.Job != (int)GameDefine_Globe.GUILD_JOB.RESERVE)
    //        {
    //            GameObject newGuildListItem = Utils.BindObjToParent(resItem, m_GuildMemberListGrid, nGuildMemberListItemIndex.ToString());
    //            if (null != newGuildListItem && null != newGuildListItem.GetComponent<GuildMemberListItemLogic>())
    //            {
    //                newGuildListItem.GetComponent<GuildMemberListItemLogic>().Init(member, false);
    //                nGuildMemberListItemIndex++;
    //            }
    //        }
    //    }

    //    //Grid排序，防止列表异常
    //    m_GuildMemberListGrid.GetComponent<UIGrid>().Reposition();
    //    m_GuildMemberListGrid.GetComponent<UITopGrid>().Recenter(true);
    //}

    //显示帮会待审批成员列表，启动Bundle加载
    void ShowGuildReserveMemberList()
    {
        //清理待审批成员Grid
        Utils.CleanGrid(m_GuildReserveMemberListGrid);

        //进入Bundle加载过程
        //UIManager.LoadItem(UIInfo.GuildMemberListItem, OnLoadGuildReserveMemberItem);
        OnLoadGuildReserveMemberItem(m_GuildMemberListItem, null);
    }

    //帮会成员信息Bundle加载结束
    void OnLoadGuildReserveMemberItem(GameObject resItem, object param)
    {
        if (null == resItem)
        {
            LogModule.ErrorLog("load Guild Info item fail");
            return;
        }

        Utils.CleanGrid(m_GuildMemberListGrid);

        //填充数据
        int nGuildMemberListItemIndex = 0;
        foreach (KeyValuePair<UInt64, GuildMember> memberPair in GameManager.gameManager.PlayerDataPool.GuildInfo.GuildMemberList)
        {
            GuildMember member = memberPair.Value;
            if (member.Guid != GlobeVar.INVALID_GUID && member.Job == (int)GameDefine_Globe.GUILD_JOB.RESERVE)
            {
                GameObject newGuildListItem = Utils.BindObjToParent(resItem, m_GuildReserveMemberListGrid, nGuildMemberListItemIndex.ToString());
                if (null != newGuildListItem && null != newGuildListItem.GetComponent<GuildMemberListItemLogic>())
                {
                    newGuildListItem.GetComponent<GuildMemberListItemLogic>().Init(member, true);
                    nGuildMemberListItemIndex++;
                }
            }
        }

        //Grid排序，防止列表异常
        m_GuildReserveMemberListGrid.GetComponent<UIGrid>().Reposition();
        m_GuildReserveMemberListGrid.GetComponent<UITopGrid>().Recenter(true);

        //更新待审批标记相关函数调用
        //设置主菜单帮会待审批成员标记位
        if (MenuBarLogic.Instance() != null)
        {
            MenuBarLogic.Instance().UpdateGuildAndMasterReserveMember();
        }

        //更新帮会界面待审批标记
        UpdateGuildReserveRemindNum();

        //更新待审批成员标记为
        if (GameManager.gameManager.PlayerDataPool.GuildInfo.GetGuildReserveMemberCount() > 0)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.ShowGuildNewReserveFlag = true;
        }
        else
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.ShowGuildNewReserveFlag = false;
        }
    }

    //按钮相应消息
    //离开帮会按钮
    void OnBtnClickLeaveGuild()
    {
        if (null != Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.ReqLeavGuild();
        }
    }

    //激活帮会公告编辑框
    void OnActiveNoticeInput()
    {
        if (null != m_GuildNoticeCollider)
        {

            m_GuildNoticeCollider.enabled = false;
		
        }
    }

    //激活帮会邮件编辑框
    void OnActiveMailInput()
    {
        if (null != m_GuildMailCollider)
        {
            m_GuildMailCollider.enabled = false;

        }
    }

    //修改帮会公告
    void OnBtnClickChangeNotice()
    {
        if (null != m_GuildNoticeCollider)
        {
            m_GuildNoticeCollider.enabled = true;
			m_GuildMaxSizeLabel.SetActive(true);
        }

        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        //判断帮会名称控件是否存在
        if (null == m_GuildNoticeLabel)
        {
            return;
        }
        
        //判断公告是否符合要求
        if (string.IsNullOrEmpty(m_GuildNoticeLabel.text))
        {
            return;
        }
        
#if UNITY_WP8
        byte[] byteText = PortUtil.StringToASCII(m_GuildNoticeLabel.text);
#else
        byte[] byteText = System.Text.Encoding.ASCII.GetBytes(m_GuildNoticeLabel.text);
#endif
        if (byteText.Length >= GlobeVar.MAX_GUILD_NOTICE)
        {
            //提示公告过长
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2798}");
            return;
        }

        //屏蔽字检查
        if (null != Utils.GetStrFilter(m_GuildNoticeLabel.text, (int)GameDefine_Globe.STRFILTER_TYPE.STRFILTER_NAME)
             && !containsEmoji(m_GuildNoticeLabel.text))
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1278}");        // 包含非法字符
            return;
        }
        
        if (null != Singleton<ObjManager>.GetInstance().MainPlayer)
        {
			if(Singleton<ObjManager>.GetInstance().MainPlayer.ReqChangeGuildNotice(m_GuildNoticeLabel.text))
			{
				m_GuildNoticeLabel.color = Color.yellow;

			}
			else
			{
				Guild info = GameManager.gameManager.PlayerDataPool.GuildInfo;
				m_GuildNoticeLabel.text = info.GuildNotice;
			}
        }
    }

    //发送工会邮件
    void OnBtnClickOpenGuildMail()
    {
        if (null == m_GuildInfoPlane && null == m_GuildMailPlane)
        {
            return;
        }

        if (Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            UInt64 myGuid = Singleton<ObjManager>.GetInstance().MainPlayer.GUID;
            if (!GameManager.gameManager.PlayerDataPool.IsGuildChief() &&
                 !GameManager.gameManager.PlayerDataPool.IsGuildViceChief(myGuid))
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2513}");        // 包含非法字符
                return;
            }
        }

        m_GuildInfoPlane.gameObject.SetActive(false);
        m_GuildMailPlane.gameObject.SetActive(true);
    }

    //打开帮会信息界面
    void OnBtnClickOpenGuildInfo()
    {
        m_GuildInfoPlane.gameObject.SetActive(true);
		m_GuildMailPlane.gameObject.SetActive(false);
		m_GuildMemberListGrid.GetComponent<UIGrid>().Reposition();
		m_GuildMemberListGrid.GetComponent<UITopGrid>().Recenter(true);
    }

    //创建帮会按钮
    void OnBtnClickCreate()
    {
        //如果有帮会并且不是待审批成员，则无法创建帮会
        if (GameManager.gameManager.PlayerDataPool.IsHaveGuild() && false == GameManager.gameManager.PlayerDataPool.IsReserveGuildMember())
        {
			Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1772}");        // 包含非法字符
            return;
        }

        //关闭当前界面，弹出创建帮会界面
        UIManager.CloseUI(UIInfo.MasterAndGuildRoot);
        UIManager.ShowUI(UIInfo.CreateGuild);
    }

    //帮会列表翻页按钮
    void OnBtnClickGuildListPgUp()
    {
        GuildListPageChanged(m_nCurGuildListPage + 1);
    }
    void OnBtnClickGuildListPgDown()
    {
        GuildListPageChanged(m_nCurGuildListPage - 1);
    }

    //帮会成员翻页按钮
    void OnBtnClickGuildMemberPgUp()
    {
        GuildMemberPageChanged(m_nCurGuildMemberPage + 1);
    }
    void OnBtnClickGuildMemberPgDown()
    {
        GuildMemberPageChanged(m_nCurGuildMemberPage - 1);
    }

    //帮会成员列表翻页逻辑
    void GuildMemberPageChanged(int nPage)
    {
        //判断页码下限
        if (nPage < 1)
        {
            nPage = 1;
        }

        //判断页码上限
        if (nPage > m_nMaxGuildMemberPage)
        {
            nPage = m_nMaxGuildMemberPage;
        }

        //页码和当前页一致，不进行操作
        if (nPage == m_nCurGuildMemberPage)
        {
            return;
        }

        m_nCurGuildMemberPage = nPage;

        //进入Bundle加载过程
        //UIManager.LoadItem(UIInfo.GuildMemberListItem, LoadGuildMemberItemPageChange);
        OnLoadGuildMemberItem(m_GuildMemberListItem, nPage);
    }
    
    public  void OnlyShowEnemyToggle()
    {
        m_bIsOnlyShowEnemyGuild = m_IsOnlyShowEnemyToggle.value;
        ShowGuildList();
    }
    //帮会列表翻页逻辑
    void GuildListPageChanged(int nPage)
    {
        //判断页码下限
        if (nPage < 1)
        {
            nPage = 1;
        }

        //判断页码上限
        if (nPage > m_nMaxGuildListPage)
        {
            nPage = m_nMaxGuildListPage;
        }

        //页码和当前页一致，不进行操作
        if (nPage == m_nCurGuildListPage)
        {
            return;
        }

        m_nCurGuildListPage = nPage;

        //进入Bundle加载过程
        //UIManager.LoadItem(UIInfo.GuildListItem, LoadGuildListItemPageChange);
        LoadGuildListItemPageChange(m_GuildListItem, null);
    }
    
    private void LoadGuildListItemPageChange(GameObject resItem, object param)
    {
        if (null == resItem)
        {
            return;
        }

        m_GuildPageLable.text = string.Format("{0}/{1}", m_nCurGuildListPage, m_nMaxGuildListPage);

        Utils.CleanGrid(m_GuildListGrid);

        List<GuildPreviewInfo> list = GameManager.gameManager.PlayerDataPool.guildList.GuildInfoList;
        if (null == list)
        {
            return;
        }

        //填充数据
        int nGuildListStartIndex = (m_nCurGuildListPage - 1) * m_nGuildNumPerPage;
        int nSelectCount = 0;
        for (; nGuildListStartIndex < list.Count; ++nGuildListStartIndex)
        {
            if (nSelectCount>=m_nGuildNumPerPage)
            {
                break;
            }
            if (list.Count > nGuildListStartIndex)
            {
                GuildPreviewInfo info = list[nGuildListStartIndex];
                //如果需要筛选 敌对帮会 则跳过非敌对的帮会
                if (m_bIsOnlyShowEnemyGuild && info.IsEnemyGuild == false)
                {
                    continue;
                }
                if (info.GuildGuid != GlobeVar.INVALID_GUID)
                {
                    GameObject newGuildListItem = Utils.BindObjToParent(resItem, m_GuildListGrid, nGuildListStartIndex.ToString());
                    if (null != newGuildListItem && null != newGuildListItem.GetComponent<GuildListItemLogic>())
                        newGuildListItem.GetComponent<GuildListItemLogic>().Init(info, nGuildListStartIndex + 1);
                    nSelectCount++;
                }
            }
        }

        m_GuildListGrid.GetComponent<UIGrid>().Reposition();
        m_GuildListGrid.GetComponent<UITopGrid>().Recenter(true);
    }

    //所选玩家变化
    public void OnSelectMemberChange(UInt64 selectGuid)
    {
        m_SelectMemberGuid = selectGuid;
		GuildMemberListItemLogic[] memberS = m_GuildMemberListGrid.gameObject.GetComponentsInChildren<GuildMemberListItemLogic> ();
		foreach(GuildMemberListItemLogic member in memberS)
		{
			member.m_HightSprite.gameObject.SetActive(selectGuid == member.MemberGuid);
		}
    }

    //帮会属性加成翻页按钮
    void OnBtnClickGuildRewardPgDown()
    {
        //已经到达最低等级，不执行翻页
        if (m_nCurGuildAwardPageNum == 1)
            return;

        int nLevel = m_nCurGuildAwardPageNum - 1;
        if (nLevel <= 0)
        {
            nLevel = 1;
        }

        ShowGuildAttrAddition(nLevel);
    }

    void OnBtnClickGuildRewardPgUp()
    {
        //已经到达最高等级，不执行翻页
        if (m_nCurGuildAwardPageNum == 5)
            return;

        int nLevel = m_nCurGuildAwardPageNum + 1;
        if (nLevel > 5)
        {
            nLevel = 5;
        }

        ShowGuildAttrAddition(nLevel);
    }

    public void UpdateGuildReserveRemindNum()
    {
        if (null == m_GuildReserveRemind || null == m_GuildReserveRemindNum)
        {
            return;
        }

        if (Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            UInt64 myGuid = Singleton<ObjManager>.GetInstance().MainPlayer.GUID;
            if ( ! GameManager.gameManager.PlayerDataPool.IsGuildChief() &&
                 ! GameManager.gameManager.PlayerDataPool.IsGuildViceChief(myGuid))
            {
                m_GuildReserveRemind.SetActive(false);
                return;
            }
        }

        if (GameManager.gameManager.PlayerDataPool.GuildInfo.GetGuildReserveMemberCount() > 0)
        {
            m_GuildReserveRemind.SetActive(true);
            m_GuildReserveRemindNum.text = GameManager.gameManager.PlayerDataPool.GuildInfo.GetGuildReserveMemberCount().ToString();
        }
        else
        {
            m_GuildReserveRemind.SetActive(false);
        }
    }

    public void OnClickRecruitment()
    {
        //目前只有帮主可以执行招募功能
        if (!GameManager.gameManager.PlayerDataPool.IsGuildChief())
        {
            MessageBoxLogic.OpenOKBox(StrDictionary.GetClientDictionaryString("#{3109}"), "");
            return;
        }

        // 发世界聊天消息
        if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid != GlobeVar.INVALID_GUID)
        {
            ShareTargetChooseLogic.InitGuildShare(GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid);
        }
        
    }

    //发送邮件
    void OnBtnSendGuildMail()
    {
        if (null != m_GuildMailCollider)
        {
            m_GuildMailCollider.enabled = true;
        }

        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        //判断帮会名称控件是否存在
        if (null == m_GuildMailLable)
        {
            return;
        }

		if (string.IsNullOrEmpty(m_GuildMailLable.text) || m_GuildMailLable.text == "请输入邮件内容。")
        {
            // 请输入收件人姓名。
            MessageBoxLogic.OpenOKBox(1141, 1000);
            return;
        }


        if (m_GuildMailLable.text.Length > 60)
        {
            // 邮件正文不能超过60个字符。
            MessageBoxLogic.OpenOKBox(1259, 1000);
            return;
        }


        if (null != Utils.GetStrFilter(m_GuildMailLable.text, (int)GameDefine_Globe.STRFILTER_TYPE.STRFILTER_CHAT)
            && !containsEmoji(m_GuildMailLable.text))
        {
            // 邮件包含非法字符
            MessageBoxLogic.OpenOKBox(1278, 1000);
            return;
        }

        if (null != Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.ReqSendGuildMail(m_GuildMailLable.text);
        }

        m_GuildMailPlane.gameObject.SetActive(false);
        m_GuildInfoPlane.gameObject.SetActive(true);
		m_GuildMemberListGrid.GetComponent<UIGrid>().Reposition();
		m_GuildMemberListGrid.GetComponent<UITopGrid>().Recenter(true);
    }

    bool containsEmoji(String source)
    {
        int len = source.Length;
        char[] codePointArr = source.ToCharArray();
        for (int i = 0; i < len; i++)
        {
            char codePoint = codePointArr[i];
            if (!isEmojiCharacter(codePoint))
            {
                return true;
            }
        } return false;
    }


    private bool isEmojiCharacter(char codePoint)
    {
        return (codePoint == 0x0) || (codePoint == 0x9) || (codePoint == 0xA) ||
        (codePoint == 0xD) || ((codePoint >= 0x20) && (codePoint <= 0xD7FF)) ||
            ((codePoint >= 0xE000) && (codePoint <= 0xFFFD)) || ((codePoint >= 0x10000)
                                                                 && (codePoint <= 0x10FFFF));

    }

    //显示帮会财富界面
    public void ShowGuildBusiness()
    {
        ClearGuildBusinessInfo();

        //被劫信息
        for (int i = 0; i < m_GuildRobbedInfo.Length; i++)
        {
            if (null != m_GuildRobbedInfo[i] && string.Empty != GameManager.gameManager.PlayerDataPool.GuildInfo.GetGBRobbedGuildName(i))
            {
                m_GuildRobbedInfo[i].gameObject.SetActive(true);
                string robName = GameManager.gameManager.PlayerDataPool.GuildInfo.GetGBRobbPlayerName(i);
                string robGuildName = GameManager.gameManager.PlayerDataPool.GuildInfo.GetGBRobbedGuildName(i);
                m_GuildRobbedInfo[i].text = StrDictionary.GetClientDictionaryString("#{3951}", robGuildName, robName);
            }
            else
            {
                m_GuildRobbedInfo[i].gameObject.SetActive(false);
            }
        }

        m_AssignInfoLable.text = GameManager.gameManager.PlayerDataPool.GuildInfo.GBCanAssignTime.ToString() + "/"
            + GameManager.gameManager.PlayerDataPool.GuildInfo.GBOneWeekTime.ToString();               //可开启信息
  //      Debug.Log(GameManager.gameManager.PlayerDataPool.GuildInfo.GBIsAuto);
        ChangeAutoAssignButtonBg(GameManager.gameManager.PlayerDataPool.GuildInfo.GBIsAuto);             //自动选择
        m_AcceptInfoLable.text = GameManager.gameManager.PlayerDataPool.GuildInfo.GBCanAcceptTime.ToString();               //可接取信息
        m_GuildWealthLable.text = GameManager.gameManager.PlayerDataPool.GuildInfo.GuildWeath.ToString();

        m_ChiefAssignBtn.gameObject.SetActive(GameManager.gameManager.PlayerDataPool.IsGuildChief());
        m_AutoAssignButton.gameObject.SetActive(GameManager.gameManager.PlayerDataPool.IsGuildChief());
        m_OpenGuildMakeBtn.gameObject.SetActive(GameManager.gameManager.PlayerDataPool.IsGuildChief());

        UpdateMemDayTime();
        UpdateGBRobCoinInfo();
    }

    void ChangeAutoAssignButtonBg(bool IsAuto)
    {
        m_AutoAssignButton.gameObject.SetActive(false);
        if (true == IsAuto)
        {
            m_AutoAssignButton.normalSprite = "ui_pub_036";
			m_AutoAssignButton.hoverSprite = "ui_pub_036";
			m_AutoAssignButton.pressedSprite = "ui_pub_036";
			m_AutoAssignButton.disabledSprite = "ui_pub_036";
			m_AutoAssignButton.name = "ui_pub_036";
            m_AutoAssignButton.gameObject.SetActive(true);
            return;
        }
       
		m_AutoAssignButton.normalSprite = "ui_pub_035";
		m_AutoAssignButton.hoverSprite = "ui_pub_035";
		m_AutoAssignButton.pressedSprite = "ui_pub_035";
		m_AutoAssignButton.disabledSprite = "ui_pub_035";
        m_AutoAssignButton.name = "chat_show_bg";
        m_AutoAssignButton.gameObject.SetActive(true);
        return;
    }


    void UpdateMemDayTime()
    {
        Tab_GuildBusiness tab = TableManager.GetGuildBusinessByID(GameManager.gameManager.PlayerDataPool.GuildInfo.GuildLevel, 0);
        if (null == tab && null == m_MemberTimeSLable)
        {
            return;
        }
        int useTime = GameManager.gameManager.PlayerDataPool.CommonData.GetCommonData((int)Games.UserCommonData.USER_COMMONDATA.CD_GUILDBUSINESS_GOTTEN_NUM);
        int curTime = tab.MemTimes - useTime;
        if (curTime < 0)
        {
            curTime = 0;
        }
        m_MemberTimeSLable.text = curTime.ToString() + "/" + tab.MemTimes.ToString();              //玩家跑商次数信息
    }

    //清空帮会财富
    void ClearGuildBusinessInfo()
    {
        //清空被劫信息
        for(int i = 0; i < m_GuildRobbedInfo.Length; i++) 
        {
            if (null != m_GuildRobbedInfo[i])
            {
                m_GuildRobbedInfo[i].text = string.Empty;
            }
        }
        m_RobNumLable.text = "0/0";                   //抢劫数
        m_RobNumSprite.fillAmount = 0f;                 //抢劫进度条
        m_AssignInfoLable.text = "0/0";               //可开启信息
        ChangeAutoAssignButtonBg(false);             //自动选择
        m_AcceptInfoLable.text = "0/0";               //可接取信息
        m_MemberTimeSLable.text = "0/0";
        m_GuildWealthLable.text = "0";
        m_GuildMakePanel.gameObject.SetActive(false);
    }

    // 帮主分配次数
    void OnBtnChiefAssign()
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        //判断帮会名称控件是否存在
        if (null == m_GuildMailLable)
        {
            return;
        }

        if (null != Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.AssignGuildBusiness();
        }
    }

    void OnBtnAutoAssign()
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        //判断帮会名称控件是否存在
        if (null == m_GuildMailLable)
        {
            return;
        }

        if (null != Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SetAutoAssign();
        }
    }

    void OnBtnGoTo()
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        Singleton<ObjManager>.GetInstance().MainPlayer.GoToGBNPC();
        UIManager.CloseUI(UIInfo.MasterAndGuildRoot);
        UIManager.CloseUI(UIInfo.MenuBarRoot);
        if (PlayerFrameLogic.Instance() != null)
        {
            PlayerFrameLogic.Instance().PlayerFrameHeadOnClick();
        }
    }

    void UpdateGBRobCoinInfo() 
    {
        Tab_GuildBusiness tab = TableManager.GetGuildBusinessByID(GameManager.gameManager.PlayerDataPool.GuildInfo.GuildLevel, 0);
        int robCoin = GameManager.gameManager.PlayerDataPool.CommonData.GetCommonData((int)Games.UserCommonData.USER_COMMONDATA.CD_GUILDBUSINESS_ROB_CION);
        m_RobNumLable.text = robCoin.ToString() + "/" + tab.MaxRobCoinNum.ToString();                   //抢劫数
        m_RobNumSprite.fillAmount = (float)robCoin / (float)tab.MaxRobCoinNum;                 //抢劫进度条
    }

    void ShowGuildMakePlane()
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        //判断帮会名称控件是否存在
        if (null == m_GuildMakePanel)
        {
            return;
        }

        m_GuildMakePanel.gameObject.SetActive(true);
    }

}
