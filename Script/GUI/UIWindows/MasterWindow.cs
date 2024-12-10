/********************************************************************
	filename:	MasterWindow.cs
	date:		2014-5-8  11-20
	author:		tangyi
	purpose:	师门界面逻辑
*********************************************************************/

using UnityEngine;
using System.Collections;
using System;
using Games.GlobeDefine;
using GCGame;
using Module.Log;
using GCGame.Table;
using System.Collections.Generic;
using Games.SkillModle;
using System.Text.RegularExpressions;

public class MasterWindow : MonoBehaviour 
{
    public GameObject m_CreateMaster;                   //创建师门界面
    public TabController m_MasterTabController;         //师门分页控制器

    public GameObject m_MasterMemberGrid;               //帮会成员Grid
    public GameObject m_MasterMenberItem;               //帮会成员Item
    public GameObject m_MasterReserveMemberGrid;        //帮会待审批成员Grid
    public GameObject m_MasterReserveMemberItem;        //帮会待审批成员Item
    public GameObject m_MasterPreviewGrid;              //帮会预览Grid
    public GameObject m_MasterPreviewItem;              //帮会预览Item

    public UILabel m_MasterName;                        //师门名称
    public UILabel m_MasterCheifName;                   //掌门名称
    public UILabel m_MasterNotice;                      //师门公告
    public UILabel m_MasterOnlineMemberNum;             //在线弟子数量
    public UILabel m_MasterTotalMemberNum;              //总弟子数量
    public UILabel m_MasterMaxMemberNum;                //弟子上限数量
    public UILabel m_MasterPresent;                     //师门简介
    public UIInput m_MasterChangeNotice;                //师门公告修改
    public UILabel m_MasterTorchValue;                  //师门薪火
    public UILabel m_TorchValue;                        //个人薪火

    const int SKILL_NUM = 6;
    public UISprite[] m_SkillSprite;                    //师门技能图标
    public UILabel[] m_SkillName;                       //师门技能名称
    public UISprite[] m_SkillSelect;                    //师门技能选中
    public UILabel m_SkillDesc;                         //技能描述
    public UIImageButton m_SkillLearnBtn;               //技能学习按钮
    public UIImageButton m_SkillActiveBtn;              //技能激活按钮
    public UIImageButton m_SkillForgetBtn;              //技能遗忘按钮
    public UIInput m_SkillNameInput;                    //技能名称输入
    public GameObject m_SkillNameInputObject;           //技能名称输入框

    public UIImageButton m_DissolveBtn;                 //解散师门
    public UIImageButton m_LevelMasterBtn;              //离开师门
    public UIImageButton m_KickMemberBtn;               //踢出师门

    public UIInput m_MasterNameInput;                   //师门名称输入
    public UIInput m_MasterNoticeInput;                 //师门公告输入

    public UILabel m_ReserverMemberRemain;              //待审批成员提醒

    private UInt64 m_CurSelectMemberGuid = GlobeVar.INVALID_GUID;           //当前选择成员
    private string m_CurSelectMemberName;                               //当前选择成员姓名
    private UInt64 m_CurSelectReserveMember = GlobeVar.INVALID_GUID;    //当前选择待审批成员
    private string m_CurSelectReserveMemberName;                        //当前选择待审批成员
    private UInt64 m_CurSelectMaster = GlobeVar.INVALID_GUID;           //当前选择师门
    private int m_CurSelectSkill = -1;                                  //当前选择技能
    private int m_CurSelectSkillIndex = -1;                             //当前选择技能位置Index

    private int[] m_SkillID = new int[SKILL_NUM];       //技能ID
    private bool[] m_SkillActive = new bool[SKILL_NUM];  //师门是否学习技能
    private bool[] m_SkillOwn = new bool[SKILL_NUM];    //是否拥有技能

    private bool m_NeedUpdateMasterInfo = false;        //是否需要更新师门信息 打开一次界面只更新一次

    //帮会商店相关
    private GameObject m_MasterShopItem = null;      //商店物品
    public GameObject m_MasterShopGrid;              //商店Grid
    public UILabel m_MasterQingYi;                   //情义
    private int m_nQingYiItemID;                     //情义道具ID
    public GameObject m_MasterTabGrid;
    public UIPanel m_MaterMissionPanel;        // 师门任务界面


    public static int GetMasterRemainNum()
    {
        int RemainNum = 0;
        if ( GameManager.gameManager.PlayerDataPool.MasterInfo != null)
        {
            if (GameManager.gameManager.PlayerDataPool.IsMasterChief())
            {
                RemainNum = GameManager.gameManager.PlayerDataPool.MasterInfo.MasterReserveMemberList.Count;
            }
        }
        return RemainNum;
    }

    private static MasterWindow m_Instance = null;
    public static MasterWindow Instance()
    {
        return m_Instance;
    }

    void Awake()
    {
        m_Instance = this;
        if (m_MasterTabController != null)
        {
            m_MasterTabController.InitData();
            m_MasterTabController.delTabChanged = OnTabChanged;
        }
    }

    void Start()
    {
        //请求更新薪火值
        CG_ASK_TORCH_VALUE packet = (CG_ASK_TORCH_VALUE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_TORCH_VALUE);
        packet.SetType(1);
        packet.SendPacket();
    }

    void OnDestroy()
    {
        m_Instance = null;
    }

    void OnEnable()
    {
        GUIData.delMasterDataUpdate += UpdateData;
        GUIData.delMasterMemberSelectChange += OnSelectMemberChange;
        GUIData.delMasterReserveMemberSelectChange += OnSelectReserveMemberChange;
        GUIData.delMasterSelectChange+= OnSelectMasterChange;
        m_NeedUpdateMasterInfo = true;
        ShowTab();
    }

    void OnDisable()
    {
        GUIData.delMasterDataUpdate -= UpdateData;
        GUIData.delMasterMemberSelectChange -= OnSelectMemberChange;
        GUIData.delMasterReserveMemberSelectChange -= OnSelectReserveMemberChange;
        GUIData.delMasterSelectChange -= OnSelectMasterChange;
        m_NeedUpdateMasterInfo = false;
    }

    public void ShowTab()
    {
        m_MasterTabController.GetTabButton("6MasterInfoTab").gameObject.SetActive(false);
        m_MasterTabController.GetTabButton("5MasterMemberTab").gameObject.SetActive(false);
        m_MasterTabController.GetTabButton("4MasterCheckLisctTab").gameObject.SetActive(false);
        m_MasterTabController.GetTabButton("3MasterJoinTab").gameObject.SetActive(false);
        m_MasterTabController.GetTabButton("2MasterCreateTab").gameObject.SetActive(false);
        m_MasterTabController.GetTabButton("1MasterShopTab").gameObject.SetActive(false);

        //无师门 或 待审批 => 加入师门 创建师门
        if (GameManager.gameManager.PlayerDataPool.IsHaveMaster() == false ||
            GameManager.gameManager.PlayerDataPool.IsMasterReserveMember() == true)
        {
            m_MasterTabController.GetTabButton("3MasterJoinTab").gameObject.SetActive(true);
            m_MasterTabController.GetTabButton("2MasterCreateTab").gameObject.SetActive(true);
            m_MasterTabController.GetTabButton("1MasterShopTab").gameObject.SetActive(true);

            m_MasterTabController.ChangeTab("3MasterJoinTab");
        }

        //有师门 并 非掌门 => 师门信息 师门成员
        if (GameManager.gameManager.PlayerDataPool.IsHaveMaster() == true &&
            GameManager.gameManager.PlayerDataPool.IsMasterChief() == false &&
            GameManager.gameManager.PlayerDataPool.IsMasterReserveMember() == false)
        {
            m_MasterTabController.GetTabButton("6MasterInfoTab").gameObject.SetActive(true);
            m_MasterTabController.GetTabButton("5MasterMemberTab").gameObject.SetActive(true);
            m_MasterTabController.GetTabButton("1MasterShopTab").gameObject.SetActive(true);

            m_MasterTabController.ChangeTab("6MasterInfoTab");
        }

        //有师门 并 是掌门 => 师门信息 师门成员 审批列表
        if (GameManager.gameManager.PlayerDataPool.IsHaveMaster() == true &&
            GameManager.gameManager.PlayerDataPool.IsMasterChief() == true &&
            GameManager.gameManager.PlayerDataPool.IsMasterReserveMember() == false)
        {
            m_MasterTabController.GetTabButton("6MasterInfoTab").gameObject.SetActive(true);
            m_MasterTabController.GetTabButton("5MasterMemberTab").gameObject.SetActive(true);
            m_MasterTabController.GetTabButton("4MasterCheckLisctTab").gameObject.SetActive(true);
            m_MasterTabController.GetTabButton("1MasterShopTab").gameObject.SetActive(true);

            m_MasterTabController.ChangeTab("6MasterInfoTab");
            
        }

        if (m_MasterTabGrid != null)
        {
            m_MasterTabGrid.GetComponent<UIGrid>().Reposition();
        }
    }

    void UpdateData()
    {
        GameObject curTab = m_MasterTabController.GetHighlightTab().gameObject;
        if (curTab.name == "6MasterInfoTab")
        {
            //师门信息
            if (Singleton<ObjManager>.GetInstance().MainPlayer.NeedRequestMasterInfo && m_NeedUpdateMasterInfo)
            {
                m_NeedUpdateMasterInfo = false;
                Singleton<ObjManager>.GetInstance().MainPlayer.ReqMasterInfo();
            }
            else
            {
                ShowInfoTabPage();
            }
        }
        else if (curTab.name == "5MasterMemberTab")
        {
            //成员列表
            ShowMemberTabPage();
        }
        else if (curTab.name == "4MasterCheckLisctTab")
        {
            //审批列表
            ShowCheckTabPage();
        }
        else if (curTab.name == "3MasterJoinTab")
        {
            //加入师门
            if (Singleton<ObjManager>.GetInstance().MainPlayer.NeedRequestMasterList)
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.ReqMasterList();
            }
            else
            {
                ShowJoinTabPage();
            }
        }
        else if (curTab.name == "2MasterCreateTab")
        {
            //创建师门
            ShowCreateTabPage();
        }
        else if (curTab.name == "1MasterShopTab")
        {
            UpdateQingYi();
        }
       
        UpdateMasterReserverRemain();
    }

    void OnTabChanged(TabButton tableButton)
    {
        UpdateData();
        if (tableButton.name == "1MasterShopTab")
        {
            m_nQingYiItemID = -1;
            UIManager.LoadItem(UIInfo.MasterShopItem, LoadMasterShopItemOver);
        }
    }
#region 商店

    //加载商店界面OK
    public void LoadMasterShopItemOver(GameObject resItem, object param)
    {
        //判断是否创建成功
        if (null == resItem)
        {
            LogModule.ErrorLog("load YuanBaoShopItem error");
            return;
        }

        m_MasterShopItem = resItem;

        //更新商品信息
        UpdateMasterShopGoodsInfo();
        //显示情义
        UpdateQingYi();
    }

    public void UpdateQingYi()
    {
        int nQingYiNumber = 0;
        if (m_nQingYiItemID > 0 )
        {
            nQingYiNumber = GameManager.gameManager.PlayerDataPool.BackPack.GetItemCountByDataId(m_nQingYiItemID);
        }

        if (m_MasterQingYi != null)
        {
            m_MasterQingYi.text = nQingYiNumber.ToString();
        }
    }

    void UpdateMasterShopGoodsInfo()
    {
        Utils.CleanGrid(m_MasterShopGrid);

        if (null == m_MasterShopItem)
        {
            return;
        }

        for (int i = 0; i < TableManager.GetMasterShop().Count; i++)
        {
            Tab_MasterShop tabMasterShop = TableManager.GetMasterShopByID(i, 0);
            if (tabMasterShop == null || tabMasterShop.ConsumItemID <= 0 || tabMasterShop.Price <= 0)
            {
                continue;
            }

            if (m_nQingYiItemID <= 0)
            {
                m_nQingYiItemID = tabMasterShop.ConsumItemID;
            }

            // 加载对应商品
            GameObject MasterShopItem = Utils.BindObjToParent(m_MasterShopItem, m_MasterShopGrid, i.ToString());
            if (MasterShopItem == null)
            {
                continue;
            }

            if (null != MasterShopItem.GetComponent<MasterShopItemLogic>())
                MasterShopItem.GetComponent<MasterShopItemLogic>().Init(tabMasterShop);
        }
        m_MasterShopGrid.GetComponent<UIGrid>().Reposition();
        m_MasterShopGrid.GetComponent<UITopGrid>().Recenter(true);
    }
#endregion

    void OnSelectMemberChange(UInt64 selectGuid, string selectName)
    {
        m_CurSelectMemberGuid = selectGuid;
        m_CurSelectMemberName = selectName;
        //选中高亮
        MasterMemberItemLogic[] ItemArry = m_MasterMemberGrid.GetComponentsInChildren<MasterMemberItemLogic>();
        for (int i = 0; i < ItemArry.Length; i++)
        { 
            MasterMemberItemLogic item = ItemArry[i];
            item.SetHighLight(selectGuid);
        }
    }

    void OnSelectReserveMemberChange(UInt64 selectGuid, string selectName)
    {
        m_CurSelectReserveMember = selectGuid;
        m_CurSelectReserveMemberName = selectName;
        //选中高亮
        MasterReserveMemberItemLogic[] ItemArry = m_MasterReserveMemberGrid.GetComponentsInChildren<MasterReserveMemberItemLogic>();
        for (int i = 0; i < ItemArry.Length; i++)
        {
            MasterReserveMemberItemLogic item = ItemArry[i];
            item.SetHighLight(selectGuid);
        }
    }

    void OnSelectMasterChange(UInt64 selectGuid)
    {
        m_CurSelectMaster = selectGuid;
        //选中高亮
        MasterPreviewItemLogic[] ItemArry = m_MasterPreviewGrid.GetComponentsInChildren<MasterPreviewItemLogic>();
        for (int i = 0; i < ItemArry.Length; i++)
        {
            MasterPreviewItemLogic item = ItemArry[i];
            item.SetHighLight(selectGuid);
        }
    }

    //帮会信息分页
    void ShowInfoTabPage()
    {

        if (GameManager.gameManager.PlayerDataPool.IsHaveMaster() == false)
        {
            ClearInfoTabPage();
            return;
        }

        if (GameManager.gameManager.PlayerDataPool.IsMasterChief())
        {
            m_DissolveBtn.gameObject.SetActive(true);
            m_LevelMasterBtn.gameObject.SetActive(false);
            m_MasterChangeNotice.enabled = true;
        }
        else
        {
            m_DissolveBtn.gameObject.SetActive(false);
            m_LevelMasterBtn.gameObject.SetActive(true);
            m_MasterChangeNotice.enabled = false;
        }

        Master info = GameManager.gameManager.PlayerDataPool.MasterInfo;
        if (m_MasterName != null)
        {
            m_MasterName.text = info.MasterName;
        }
        if (m_MasterCheifName != null)
        {
            m_MasterCheifName.text = info.MasterChiefName;
        }
        if (m_MasterNotice != null)
        {
            m_MasterNotice.text = info.MasterNotice;
        }
        if (m_MasterOnlineMemberNum != null)
        {
            m_MasterOnlineMemberNum.text = info.MasterOnlineNum.ToString();
        }
        if (m_MasterTotalMemberNum != null)
        {
            m_MasterTotalMemberNum.text = info.MasterMemberNum.ToString();
        }
        if (m_MasterMaxMemberNum != null)
        {
            m_MasterMaxMemberNum.text = GlobeVar.MAX_MASTER_MEMBER_NUM.ToString();
        }
        if (m_MasterOnlineMemberNum != null)
        {
            m_MasterOnlineMemberNum.text = info.MasterOnlineNum.ToString();
        }
        if (m_MasterPresent != null)
        {
            DateTime startTime = new DateTime(1970, 1, 1);
            DateTime createTime = new DateTime(startTime.Ticks + (long)info.MasterCreateTime * 10000000L, DateTimeKind.Unspecified);
            createTime = createTime.ToLocalTime();
            m_MasterPresent.text = StrDictionary.GetClientDictionaryString("#{2054}", createTime.ToString("yy"), createTime.ToString("MM"), createTime.ToString("dd"));
        }
        if (m_MasterTorchValue != null)
        {
            m_MasterTorchValue.text = info.MasterTorch.ToString();
        }

        if (m_TorchValue != null)
        {
            m_TorchValue.text = GameManager.gameManager.PlayerDataPool.TorchValue.ToString();
        }

        //显示师门技能信息
        ShowSkillInfo();
    }

    void ShowSkillInfo()
    {
        //找出需要显示的技能id
        List<int> totalSkillList = new List<int>();
        foreach (KeyValuePair<int, List<Tab_MasterSkill>> kvp in TableManager.GetMasterSkill())
        {
            Tab_MasterSkill line = kvp.Value[0];
            if (line != null)
            {
                if (line.Level == 1)
                {
                    totalSkillList.Add(line.Id);
                }
            }
        }

        //当前师门已经学习的技能
        Dictionary<int, string> masterSkillList = GameManager.gameManager.PlayerDataPool.MasterInfo.MasterSkillList;

        //当前个人已经激活的技能
        List<int> ownSkillList = new List<int>();
        OwnSkillData[] InfoArry = GameManager.gameManager.PlayerDataPool.OwnSkillInfo;
        for (int i = 0; i < InfoArry.Length; i++)
        {
            OwnSkillData info = InfoArry[i];
            if (info.SkillId >= 0)
            {
                ownSkillList.Add(info.SkillId);
            }
        }

        //显示技能信息
        for (int index = 0; index < SKILL_NUM && index < totalSkillList.Count; index++)
        {
            int skillid = totalSkillList[index];
            //技能是否合法
            Tab_SkillEx lineSkillEx = TableManager.GetSkillExByID(skillid, 0);
            if (lineSkillEx == null)
            {
                continue;
            }
            Tab_SkillBase lineSkillBase = TableManager.GetSkillBaseByID(lineSkillEx.BaseId, 0);
            if (lineSkillBase == null)
            {
                continue;
            }

            if (masterSkillList.ContainsKey(skillid))
            {
                //师门已经学习的技能
                int sameBaseSkillId = GetSameBaseSkill(ownSkillList, skillid);
                if (sameBaseSkillId >= 0)
                {
                    //个人已经激活的技能
                    m_SkillID[index] = sameBaseSkillId;
                    m_SkillActive[index] = true;
                    m_SkillOwn[index] = true;

                    m_SkillSprite[index].spriteName = lineSkillBase.Icon;
                    m_SkillName[index].text = masterSkillList[skillid];
                }
                else
                {
                    //个人未激活的技能
                    m_SkillID[index] = skillid;
                    m_SkillActive[index] = true;
                    m_SkillOwn[index] = false;

                    m_SkillSprite[index].spriteName = lineSkillBase.Icon;
                    m_SkillName[index].text = masterSkillList[skillid];
                }
            }
            else
            {
                //师门未学习的技能
                m_SkillID[index] = skillid;
                m_SkillActive[index] = false;
                m_SkillOwn[index] = false;

                m_SkillSprite[index].spriteName = lineSkillBase.Icon;
                //m_SkillName[index].text = "未学习";
                m_SkillName[index].text = StrDictionary.GetClientDictionaryString("#{2795}");
            }
        }
        //隐藏按钮
        m_SkillLearnBtn.gameObject.SetActive(false);
        m_SkillActiveBtn.gameObject.SetActive(false);
        m_SkillForgetBtn.gameObject.SetActive(false);
        m_SkillNameInputObject.gameObject.SetActive(false);

        //默认选中技能
        OnClickSkill((m_CurSelectSkillIndex >= 0) ? m_CurSelectSkillIndex : 0);
    }

    int GetSameBaseSkill(List<int> ownSkillList, int skillid)
    {
        Tab_MasterSkill skillLine = TableManager.GetMasterSkillByID(skillid, 0);
        if (skillLine != null)
        {
            for (int index = 0; index < ownSkillList.Count; index++)
            {
                if (ownSkillList[index] >= 0)
                {
                    Tab_MasterSkill tempLine = TableManager.GetMasterSkillByID(ownSkillList[index], 0);
                    if (tempLine != null)
                    {
                        if (tempLine.BaseSkill == skillLine.BaseSkill)
                        {
                            return ownSkillList[index];
                        }
                    }
                }
            }
        }
        return -1;
    }

    //成员列表分页
    void ShowMemberTabPage()
    {
        if (GameManager.gameManager.PlayerDataPool.IsMasterChief())
        {
            m_KickMemberBtn.gameObject.SetActive(true);
        }
        else
        {
            m_KickMemberBtn.gameObject.SetActive(false);
        }

        //启动Bundle加载
        //UIManager.LoadItem(UIInfo.MasterMemberItem, OnLoadMasterMemberItem);
        OnLoadMasterMemberItem(m_MasterMenberItem, null);
    }
    void OnLoadMasterMemberItem(GameObject resItem, object param)
    {
        if (null == resItem)
        {
            LogModule.ErrorLog("load master member item fail");
            return;
        }

        Utils.CleanGrid(m_MasterMemberGrid);

        //填充数据
        int nMasterMemberItemIndex = 0;
        foreach (KeyValuePair<UInt64, MasterMember> info in GameManager.gameManager.PlayerDataPool.MasterInfo.MasterMemberList)
        {
            MasterMember member = info.Value;
            if (member.IsValid() && member.IsReserveMember() == false)
            {
                GameObject newMasterMemberItem = Utils.BindObjToParent(resItem, m_MasterMemberGrid, (nMasterMemberItemIndex+1000).ToString());
                if (null != newMasterMemberItem && null != newMasterMemberItem.GetComponent<MasterMemberItemLogic>())
                {
                    newMasterMemberItem.GetComponent<MasterMemberItemLogic>().Init(member);
                    nMasterMemberItemIndex++;
                }
            }
        }

        //Grid排序，防止列表异常
        m_MasterMemberGrid.GetComponent<UIGrid>().Reposition();
        m_MasterMemberGrid.GetComponent<UITopGrid>().Recenter(true);
    }

    //审批列表分页
    void ShowCheckTabPage()
    {
        //启动Bundle加载
        //UIManager.LoadItem(UIInfo.MasterReserveMemberItem, OnLoadMasterReserveMemberItem);
        OnLoadMasterReserveMemberItem(m_MasterReserveMemberItem, null);
        m_ReserverMemberRemain.gameObject.SetActive(false);
    }
    void OnLoadMasterReserveMemberItem(GameObject resItem, object param)
    {
        if (null == resItem)
        {
            LogModule.ErrorLog("load master reserve member item fail");
            return;
        }

        Utils.CleanGrid(m_MasterReserveMemberGrid);

        //填充数据
        int nMasterMemberItemIndex = 0;
        foreach (KeyValuePair<UInt64, MasterMember> info in GameManager.gameManager.PlayerDataPool.MasterInfo.MasterReserveMemberList)
        {
            MasterMember member = info.Value;
            if (member.IsValid() && member.IsReserveMember() == true)
            {
                GameObject newMasterMemberItem = Utils.BindObjToParent(resItem, m_MasterReserveMemberGrid, nMasterMemberItemIndex.ToString());
                if (null != newMasterMemberItem &&
                    null != newMasterMemberItem.GetComponent<MasterReserveMemberItemLogic>())
                    newMasterMemberItem.GetComponent<MasterReserveMemberItemLogic>().Init(member);
                nMasterMemberItemIndex++;
            }
        }

        //Grid排序，防止列表异常
        m_MasterReserveMemberGrid.GetComponent<UIGrid>().Reposition();
        m_MasterReserveMemberGrid.GetComponent<UITopGrid>().Recenter(true);
    }

    //加入师门分页
    void ShowJoinTabPage()
    {
        //启动Bundle加载
        //UIManager.LoadItem(UIInfo.MasterPreviewItem, OnLoadMasterPreviewItem);
        OnLoadMasterPreviewItem(m_MasterPreviewItem, null);
    }

    void OnLoadMasterPreviewItem(GameObject resItem, object param)
    {
        if (null == resItem)
        {
            LogModule.ErrorLog("load master List item fail");
            return;
        }

        Utils.CleanGrid(m_MasterPreviewGrid);

        //填充数据
        int nMasterPreviewItemIndex = 0;
        List<MasterPreviewInfo> masterInfoList = GameManager.gameManager.PlayerDataPool.MasterPreList.MasterInfoList;
        for (int i = 0; i < masterInfoList.Count; i++)
        {
            MasterPreviewInfo info = masterInfoList[i];
            if (info.MasterGuid != GlobeVar.INVALID_GUID)
            {
                GameObject newMasterPreviewItem = Utils.BindObjToParent(resItem, m_MasterPreviewGrid, nMasterPreviewItemIndex.ToString());
                if (null != newMasterPreviewItem &&
                    null != newMasterPreviewItem.GetComponent<MasterPreviewItemLogic>())
                newMasterPreviewItem.GetComponent<MasterPreviewItemLogic>().Init(info, nMasterPreviewItemIndex + 1);
                nMasterPreviewItemIndex++;
            }
        }

        //Grid排序，防止列表异常
        m_MasterPreviewGrid.GetComponent<UIGrid>().Reposition();
        m_MasterPreviewGrid.GetComponent<UITopGrid>().Recenter(true);
    }

    //自立门户分页
    void ShowCreateTabPage()
    {

    }

    void ClearInfoTabPage()
    {
        if (m_MasterName != null)
        {
            m_MasterName.text = "";
        }
        if (m_MasterCheifName != null)
        {
            m_MasterCheifName.text = "";
        }
        if (m_MasterNotice != null)
        {
            m_MasterNotice.text = "";
        }
        if (m_MasterOnlineMemberNum)
        {
            m_MasterOnlineMemberNum.text = "";
        }
        if (m_MasterTotalMemberNum)
        {
            m_MasterTotalMemberNum.text = "";
        }
        if (m_MasterMaxMemberNum)
        {
            m_MasterMaxMemberNum.text = "";
        }
        if (m_MasterPresent != null)
        {
            m_MasterPresent.text = "";
        }
        if (m_SkillDesc != null)
        {
            m_SkillDesc.text = "";
        }
        m_SkillNameInputObject.gameObject.SetActive(false);
        ClearSkillChoose();
    }

    //点击 创建师门
    public void OnClickCreateMaster()
    {
        if (string.IsNullOrEmpty(m_MasterNameInput.value) ||
            string.IsNullOrEmpty(m_MasterNoticeInput.value))
        {
            if (string.IsNullOrEmpty(m_MasterNameInput.value) && string.IsNullOrEmpty(m_MasterNoticeInput.value)) 
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, StrDictionary.GetClientDictionaryString("#{2796}"));
                return;
            }
            //Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "请输入师门名称和公告");

            if (string.IsNullOrEmpty(m_MasterNameInput.value))
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, StrDictionary.GetClientDictionaryString("#{5592}"));
                return;
            }

            if (string.IsNullOrEmpty(m_MasterNoticeInput.value))
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, StrDictionary.GetClientDictionaryString("#{2806}"));

                return;
            }
        }

        int curNameCharNum = 0;
        int curNoticeCharNum = 0;   // 英文算一个，中文算两个 
        for (int index = 0; index < m_MasterNameInput.value.Length; index++)
        {
            char curChar = m_MasterNameInput.value[index];
            if ((int)curChar >= 128) 
            {
                curNameCharNum += 2;
            }
            else if ((int)curChar >= 65 && (int)curChar <= 90)
            {
                curNameCharNum += 2;
            }    
            else
            { 
                curNameCharNum++;
            }
            if (char.IsWhiteSpace(curChar))
            {
                //Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "输入不能包含空格");
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, StrDictionary.GetClientDictionaryString("#{2797}"));
                return;
            }
        }
        for (int index = 0; index < m_MasterNoticeInput.value.Length; index++)
        {
            char curChar = m_MasterNoticeInput.value[index];
            if ((int)curChar >= 128)
            {
                curNoticeCharNum += 2;
            }
            else if ((int)curChar >= 65 && (int)curChar <= 90)
            {
                curNoticeCharNum += 2;
            }    
            else
            {
                curNoticeCharNum++;
            }
            if (char.IsWhiteSpace(curChar))
            {
                //Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "输入不能包含空格");
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, StrDictionary.GetClientDictionaryString("#{2797}"));
                return;
            }
        }
        if (curNameCharNum > GlobeVar.MAX_MASTER_NAME)
        {
            // 名字过长
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1279}");
            return;
        }
        if (curNoticeCharNum > GlobeVar.MAX_MASTER_NOTICE)
        {
            // 公告过长
            //Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "公告内容过长");
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, StrDictionary.GetClientDictionaryString("#{2807}"));
            return;
        }
        if (m_MasterNameInput.value.Contains("*") || m_MasterNoticeInput.value.Contains("*"))
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1278}");
            return;
        }
        
        if (null == Utils.GetStrFilter(m_MasterNameInput.value, (int)GameDefine_Globe.STRFILTER_TYPE.STRFILTER_NAME) 
            && null == Utils.GetStrFilter(m_MasterNoticeInput.value, (int)GameDefine_Globe.STRFILTER_TYPE.STRFILTER_NAME)
            && !containsEmoji(m_MasterNameInput.value) && !containsEmoji(m_MasterNoticeInput.value))
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.ReqCreateMaster(m_MasterNameInput.value, m_MasterNoticeInput.value);
        }
        else
        {
            // 包含非法字符
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1278}");
        }
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

    //点击 加入师门
    public void OnClickJoinMaster()
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        if (m_CurSelectMaster == GlobeVar.INVALID_GUID || m_CurSelectMaster == 0)
        {
            //Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "请先选择师门");
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, StrDictionary.GetClientDictionaryString("#{2799}"));
            return;
        }

        if (GameManager.gameManager.PlayerDataPool.MasterInfo.MasterGuid != GlobeVar.INVALID_GUID)
        {
            //如果在一个师门中，但是暂时为待审批帮众，则可以去申请其他师门
            MasterMember mySelfMasterInfo;
            if (GameManager.gameManager.PlayerDataPool.MasterInfo.MasterReserveMemberList.TryGetValue(Singleton<ObjManager>.GetInstance().MainPlayer.GUID, out mySelfMasterInfo))
            {
                if (mySelfMasterInfo.IsReserveMember())
                {
                    //只能同时申请一个师门，将替换原来的请求，是否继续？
                    //string dicStr = StrDictionary.GetClientDictionaryString("只能同时申请一个师门，将替换原来的请求，是否继续？");
                    //string dicStr = "只能同时申请一个师门，将替换原来的请求，是否继续？";
                    string dicStr = StrDictionary.GetClientDictionaryString("#{2800}");
                    MessageBoxLogic.OpenOKCancelBox(dicStr, "", AgreeChangeJoinMasterRequest, null);
                    return;
                }
            }

            //否则提示“你已属于一个师门不能加入”
            //Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "你已属于一个师门不能加入");
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, StrDictionary.GetClientDictionaryString("#{2801}"));
            return;
        }

        Singleton<ObjManager>.GetInstance().MainPlayer.ReqJoinMaster(m_CurSelectMaster);
    }

    //点击 加入师门-确认
    void AgreeChangeJoinMasterRequest()
    {
        if (null != Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.ReqJoinMaster(m_CurSelectMaster);
        }
    }

    //点击 批准申请
    void OnClickReserveApprove()
    {
        if (GameManager.gameManager.PlayerDataPool.IsMasterChief() == false)
        {
            return;
        }

        if (m_CurSelectReserveMember == GlobeVar.INVALID_GUID || m_CurSelectReserveMember == 0)
        {
            //Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "未选择");
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, StrDictionary.GetClientDictionaryString("#{2896}"));
            return;
        }
        //同意
        Singleton<ObjManager>.GetInstance().MainPlayer.ReqApproveMasterMember(m_CurSelectReserveMember, true);
    }

    //点击 拒绝申请
    void OnClickReserveRefuse()
    {
        if (GameManager.gameManager.PlayerDataPool.IsMasterChief() == false)
        {
            return;
        }

        if (m_CurSelectReserveMember == GlobeVar.INVALID_GUID || m_CurSelectReserveMember == 0)
        {
            //Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "未选择");
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, StrDictionary.GetClientDictionaryString("#{2896}"));
            return;
        }
        //拒绝
        Singleton<ObjManager>.GetInstance().MainPlayer.ReqApproveMasterMember(m_CurSelectReserveMember, false);
    }

    //点击 全部批准申请
    void OnClickReserveApproveAll()
    {
        if (GameManager.gameManager.PlayerDataPool.IsMasterChief() == false)
        {
            return;
        }

        MasterReserveMemberItemLogic[] item = m_MasterReserveMemberGrid.GetComponentsInChildren<MasterReserveMemberItemLogic>();
        for (int i = 0; i < item.Length; ++i)
        {
            //同意
            Singleton<ObjManager>.GetInstance().MainPlayer.ReqApproveMasterMember(item[i].GetGuid(), true);
        }
    }

    //点击 全部拒绝申请
    void OnClickReserveRefuseAll()
    {
        if (GameManager.gameManager.PlayerDataPool.IsMasterChief() == false)
        {
            return;
        }

        MasterReserveMemberItemLogic[] item = m_MasterReserveMemberGrid.GetComponentsInChildren<MasterReserveMemberItemLogic>();
        for (int i = 0; i < item.Length; ++i)
        {
            //同意
            Singleton<ObjManager>.GetInstance().MainPlayer.ReqApproveMasterMember(item[i].GetGuid(), false);
        }
    }

    //点击 踢出帮会
    void OnClickKickMember()
    {
        if (GameManager.gameManager.PlayerDataPool.IsMasterChief() == false)
        {
            return;
        }

        if (m_CurSelectMemberGuid == GlobeVar.INVALID_GUID || m_CurSelectMemberGuid == 0)
        {
            //Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(true, "未选择");
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(true, StrDictionary.GetClientDictionaryString("#{2896}"));
            return;
        }

        if (m_CurSelectMemberGuid == Singleton<ObjManager>.GetInstance().MainPlayer.GUID)
        {
            //Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(true, "不能选择自己");
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(true, StrDictionary.GetClientDictionaryString("#{2802}"));
            return;
        }

        string dicStr = StrDictionary.GetClientDictionaryString("#{1514}");
        MessageBoxLogic.OpenOKCancelBox(dicStr, "", DeleteMember, null);
        
    }

    void DeleteMember()
    {
        Singleton<ObjManager>.GetInstance().MainPlayer.ReqKickMasterMember(m_CurSelectMemberGuid);
    }

    //点击 解散师门
    void OnClickDissolve()
    {
        //掌门才能解散
        if (GameManager.gameManager.PlayerDataPool.IsMasterChief() == false)
        {
            return;
        }
        string dicStr = StrDictionary.GetClientDictionaryString("#{3186}");
        MessageBoxLogic.OpenOKCancelBox(dicStr, "", ClickDissolveOK, null);
    }

    void ClickDissolveOK()
    {
        Singleton<ObjManager>.GetInstance().MainPlayer.ReqLeavMaster();
    }

    //点击 退出师门
    void OnClickLevel()
    {
        //有师门才能退
        if (GameManager.gameManager.PlayerDataPool.IsHaveMaster() == false)
        {
            return;
        }
        //待审批成员没法退
        if (GameManager.gameManager.PlayerDataPool.IsMasterReserveMember() == true)
        {
            return;
        }

        string dicStr = StrDictionary.GetClientDictionaryString("#{5595}");
        MessageBoxLogic.OpenOKCancelBox(dicStr, "", ClickDissolveOK, null);
    }

    //点击 师门技能1
    void OnClickSkill1()
    {
        OnClickSkill(0);
    }
    //点击 师门技能2
    void OnClickSkill2()
    {
        OnClickSkill(1);
    }
    //点击 师门技能3
    void OnClickSkill3()
    {
        OnClickSkill(2);
    }
    //点击 师门技能4
    void OnClickSkill4()
    {
        OnClickSkill(3);
    }
    //点击 师门技能5
    void OnClickSkill5()
    {
        OnClickSkill(4);
    }
    //点击 师门技能6
    void OnClickSkill6()
    {
        OnClickSkill(5);
    }

    void OnClickSkill(int index)
    {
        if (index >=0 && index < SKILL_NUM)
        {
            //设置技能选中状态
            ClearSkillChoose();
            m_SkillSelect[index].gameObject.SetActive(true);

            int skillid = m_SkillID[index];
            //设置当前选择技能
            m_CurSelectSkill = skillid;
            m_CurSelectSkillIndex = index;
            //显示技能描述
            Tab_SkillEx lineSkillEx = TableManager.GetSkillExByID(skillid, 0);
            if (lineSkillEx != null)
            {
                m_SkillDesc.text = lineSkillEx.Desc;
            }
            //所有按钮隐藏
            m_SkillLearnBtn.gameObject.SetActive(false);
            m_SkillActiveBtn.gameObject.SetActive(false);
            m_SkillForgetBtn.gameObject.SetActive(false);
            
            //显示按钮
            if (m_SkillActive[index])
            {
                if (m_SkillOwn[index])
                {
                    //已经拥有 
                    //显示遗忘按钮
                    m_SkillForgetBtn.gameObject.SetActive(true);
                }
                else
                {
                    //未学习
                    //显示学习按钮
                    m_SkillLearnBtn.gameObject.SetActive(true);
                }
            }
            else
            {
                if (GameManager.gameManager.PlayerDataPool.IsMasterChief())
                {
                    //只有掌门才显示激活按钮
                    m_SkillActiveBtn.gameObject.SetActive(true);
                    m_SkillNameInput.gameObject.SetActive(true);
                }
            }
        }
    }

    void ClearSkillChoose()
    {
        for (int i = 0; i < SKILL_NUM; i++ )
        {
            m_SkillSelect[i].gameObject.SetActive(false);
        }
    }

    //点击 技能学习按钮
    void OnClickSkillLearn()
    {
        if (m_CurSelectSkill < 0)
        {
            //Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(true, "请先选择技能");
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(true, StrDictionary.GetClientDictionaryString("#{2803}"));
            return;
        }

        Singleton<ObjManager>.GetInstance().MainPlayer.ReqLearnMasterSkill(m_CurSelectSkill);
    }

    //点击 技能遗忘按钮
    void OnClickSkillForget()
    {
        if (m_CurSelectSkill < 0)
        {
            //Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "请先选择技能");
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, StrDictionary.GetClientDictionaryString("#{2803}"));
            return;
        }

        Singleton<ObjManager>.GetInstance().MainPlayer.ReqForgetMasterSkill(m_CurSelectSkill);
    }

    //点击 技能激活按钮
    void OnClickSkillActive()
    {
        if (m_CurSelectSkill < 0)
        {
            //Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(true, "请先选择技能");
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(true, StrDictionary.GetClientDictionaryString("#{2803}"));
            return;
        }

        m_SkillNameInputObject.gameObject.SetActive(true);
    
    }

    //点击 技能激活确认
    void OnClickSkillActive_OK()
    {
        if (string.IsNullOrEmpty(m_SkillNameInput.value))
        {
            //Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(true, "请输入技能名称");
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(true, StrDictionary.GetClientDictionaryString("#{2804}"));
            return;
        }

        int curNameCharNum = 0; // 英文算一个，中文算两个 
        for (int index = 0; index < m_SkillNameInput.value.Length; index++)
        {
            char curChar = m_SkillNameInput.value[index];
            if ((int)curChar >= 128)
            {
                curNameCharNum += 2;
            }
            else if ((int)curChar >= 65 && (int)curChar <= 90)
            {
                curNameCharNum += 2;
            }  
            else
            {
                curNameCharNum++;
            }
            if (char.IsWhiteSpace(curChar))
            {
                //Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "输入不能包含空格");
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, StrDictionary.GetClientDictionaryString("#{2805}"));
                return;
            }
        }
        if (curNameCharNum > GlobeVar.MAX_MASTER_SKILL_NAME)
        {
            // 名字过长
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1279}");
            return;
        }
        if (m_SkillNameInput.value.Contains("*") || m_SkillNameInput.value.Contains("*"))
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1278}");
            return;
        }

        if (null == Utils.GetStrFilter(m_SkillNameInput.value, (int)GameDefine_Globe.STRFILTER_TYPE.STRFILTER_NAME)
            && !containsEmoji(m_SkillNameInput.value))
        {
            m_SkillNameInputObject.gameObject.SetActive(false);

            Singleton<ObjManager>.GetInstance().MainPlayer.ReqActiveMasterSkill(m_SkillNameInput.value, m_CurSelectSkill);
        }
        else
        {
            // 包含非法字符
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1278}");
        }
    }


    void OnClickSkillActive_Cancel()
    {
        m_SkillNameInputObject.gameObject.SetActive(false);
    }

    //点击 设为密聊
    void OnClickChat()
    {
        if (m_CurSelectMemberGuid == GlobeVar.INVALID_GUID || m_CurSelectMemberGuid == 0)
        {
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{5598}");
            return;
        }

        //如果目标是自己也不发送加好友
        if (Singleton<ObjManager>.GetInstance().MainPlayer.GUID == m_CurSelectMemberGuid)
        {
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2802}");
            return;
        }

        //未打开过则创建
        if (null == ChatInfoLogic.Instance())
        {
            UIManager.ShowUI(UIInfo.ChatInfoRoot, ShowChatInfoRootOver);
        }
    }

    void ShowChatInfoRootOver(bool bSuccess, object param)
    {
        if (bSuccess)
        {
            //发起私聊
            if (null != ChatInfoLogic.Instance())
            {
                ChatInfoLogic.Instance().BeginChat(m_CurSelectMemberGuid, m_CurSelectMemberName);
            }
        }
    }

    //点击 设为密聊 待审批界面
    void OnClickChat_Reserve()
    {
        if (m_CurSelectReserveMember == GlobeVar.INVALID_GUID || m_CurSelectReserveMember == 0)
        {
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{5598}");
            return;
        }

        //如果目标是自己也不发送加好友
        if (m_CurSelectMemberGuid == Singleton<ObjManager>.GetInstance().MainPlayer.GUID)
        {
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2802}");
            return;
        }

        //未打开过则创建
        if (null == ChatInfoLogic.Instance())
        {
            UIManager.ShowUI(UIInfo.ChatInfoRoot, ShowChatInfoRootOver_Reserve);
        }
    }

    void ShowChatInfoRootOver_Reserve(bool bSuccess, object param)
    {
        if (bSuccess)
        {
            //发起私聊
            if (null != ChatInfoLogic.Instance())
            {
                ChatInfoLogic.Instance().BeginChat(m_CurSelectReserveMember, m_CurSelectReserveMemberName);
            }
        }
    }

    //点击 加为好友
    void OnClickFriend()
    {
        //如果非玩家，则不显示
        if (m_CurSelectMemberGuid == GlobeVar.INVALID_GUID || m_CurSelectMemberGuid == 0)
        {
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{5598}");
            return;
        }

        //如果目标是自己也不发送加好友
        if (m_CurSelectMemberGuid == Singleton<ObjManager>.GetInstance().MainPlayer.GUID)
        {
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2802}");
            return;
        }

        CG_ADDFRIEND msg = (CG_ADDFRIEND)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ADDFRIEND);
        msg.Guid = m_CurSelectMemberGuid;
        msg.SendPacket();

        // Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2906}");
    }

    //点击 邀请入队
    void OnClickInviteTeam()
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        if (m_CurSelectMemberGuid == GlobeVar.INVALID_GUID || m_CurSelectMemberGuid == 0)
        {
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{5598}");
            return;
        }

        //如果有队伍，则判断下队伍是否已满
        if (GlobeVar.INVALID_ID != GameManager.gameManager.PlayerDataPool.TeamInfo.TeamID)
        {
            if (GameManager.gameManager.PlayerDataPool.TeamInfo.IsFull())
            {
				Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1145}");
                return;
            }
        }

        //如果目标是自己也不发送加好友
        if (m_CurSelectMemberGuid == Singleton<ObjManager>.GetInstance().MainPlayer.GUID)
        {
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2802}");
            return;
        }

        if (m_CurSelectMemberGuid != 0 &&
            m_CurSelectMemberGuid != GlobeVar.INVALID_GUID &&
            m_CurSelectMemberGuid != Singleton<ObjManager>.Instance.MainPlayer.GUID)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.ReqInviteTeam(m_CurSelectMemberGuid);
        }
    }

    //点击 申请入队
    void OnClickApplyTeam()
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        if (m_CurSelectMemberGuid == GlobeVar.INVALID_GUID || m_CurSelectMemberGuid == 0)
        {
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{5598}");
            return;
        }

        //如果目标是自己也不发送加好友
        if (m_CurSelectMemberGuid == Singleton<ObjManager>.GetInstance().MainPlayer.GUID)
        {
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2802}");
            return;
        }

        /*ReqJoinTeam会检查
        if (GlobeVar.INVALID_ID != GameManager.gameManager.PlayerDataPool.TeamInfo.TeamID)
        {
            return;
        }*/

        if (m_CurSelectMemberGuid != 0 &&
            m_CurSelectMemberGuid != GlobeVar.INVALID_GUID &&
            m_CurSelectMemberGuid != Singleton<ObjManager>.Instance.MainPlayer.GUID)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.ReqJoinTeam(m_CurSelectMemberGuid);
        }
    }

    //点击 修改公告
    void OnClickChangeNotice()
    {
        if (string.IsNullOrEmpty(m_MasterChangeNotice.value))
        {
            //Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "请输入师门公告");
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, StrDictionary.GetClientDictionaryString("#{2806}"));
            return;
        }

        int curNoticeCharNum = 0;   // 英文算一个，中文算两个 
        foreach (char curChar in m_MasterChangeNotice.value)
        {
            if ((int)curChar >= 128)
            {
                curNoticeCharNum += 2;
            }
            else if ((int)curChar >= 65 && (int)curChar <= 90)
            {
                curNoticeCharNum += 2;
            }  
            else
            {
                curNoticeCharNum++;
            }
            if (char.IsWhiteSpace(curChar))
            {
                //Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "输入不能包含空格");
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, StrDictionary.GetClientDictionaryString("#{2805}"));
                return;
            }
        }
        if (curNoticeCharNum > GlobeVar.MAX_MASTER_NOTICE)
        {
            // 公告过长
            //Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "公告内容过长");
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, StrDictionary.GetClientDictionaryString("#{2807}"));
            return;
        }
        if (m_MasterChangeNotice.value.Contains("*"))
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1278}");
            return;
        }

        if (null == Utils.GetStrFilter(m_MasterChangeNotice.value, (int)GameDefine_Globe.STRFILTER_TYPE.STRFILTER_NAME)
            && !containsEmoji(m_MasterChangeNotice.value))
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.ReqChangeNotice(m_MasterChangeNotice.value);
        }
        else
        {
            // 包含非法字符
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1278}");
        }
    }

    void UpdateMasterReserverRemain()
    {
        //有师门 并 是掌门
        if (GameManager.gameManager.PlayerDataPool.IsHaveMaster() == true &&
            GameManager.gameManager.PlayerDataPool.IsMasterChief() == true &&
            GameManager.gameManager.PlayerDataPool.IsMasterReserveMember() == false)
        {
            if (GetMasterRemainNum() > 0)
            {
                m_ReserverMemberRemain.gameObject.SetActive(true);
                m_ReserverMemberRemain.text = GetMasterRemainNum().ToString();
            }
            else
            {
                m_ReserverMemberRemain.gameObject.SetActive(false);
            }
        }
    }

    void OnClickOpenMissionPanel()
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        // 包含非法字符
        Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "功能开发中！");
        return;

        if (GameManager.gameManager.PlayerDataPool.IsHaveMaster() == false)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{3194}");
            return;
        }

        if (null != m_MaterMissionPanel)
        {
            m_MaterMissionPanel.gameObject.SetActive(true);
        }
    }

    void OnClickCloseMissionPanel()
    {
        if (null != m_MaterMissionPanel)
        {
            m_MaterMissionPanel.gameObject.SetActive(false);
        }
    }

    void OnBtnGoTo()
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        Singleton<ObjManager>.GetInstance().MainPlayer.GoToMasterMissionNPC();
        UIManager.CloseUI(UIInfo.MasterAndGuildRoot);
        UIManager.CloseUI(UIInfo.MenuBarRoot);
        if (PlayerFrameLogic.Instance() != null)
        {
            PlayerFrameLogic.Instance().PlayerFrameHeadOnClick();
        }
    }

    void OnBtnRecruit()
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        Singleton<ObjManager>.GetInstance().MainPlayer.MasterRecruit();
    }

}
