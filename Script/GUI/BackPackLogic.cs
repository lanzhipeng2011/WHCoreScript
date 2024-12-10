//********************************************************************
// 文件名: BackPackItemLogic.cs
// 描述: 背包界面UI逻辑
// 作者: TangYi
// 创建时间: 2013-12-25
//
// 修改历史:
// 说明：
//********************************************************************
using UnityEngine;
using System.Collections;
using Games.LogicObj;
using Games.Item;
using GCGame;
using System.Collections.Generic;
using GCGame.Table;
using Games.GlobeDefine;
using System;
using Games.FakeObject;
using Module.Log;

public class BackPackLogic : MonoBehaviour {

    private static BackPackLogic m_Instance = null;
    public static BackPackLogic Instance()
    {
        return m_Instance;
    }

    public enum ITEM_TAB_PAGE{     //背包分页
        TAB_PAGE_ALL,       //全部
        TAB_PAGE_EQUIP,     //装备
        TAB_PAGE_MATERIAL,  //材料
        TAB_PAGE_FELLOW,    //伙伴
        TAB_PAGE_OTHER,     //其它
    };

    public GameObject m_FirstChild;
    public GameObject m_BackPackItems;
    public GameObject m_BackPackItemGrid;
    public GameObject m_EquipPackGrid;
    public UILabel m_PlayerName;
    public UILabel m_PlayerCombatValue;
    public UILabel m_PlayerLevel;
    public UILabel m_BackPakcSize;

    public UILabel m_Moneyinfo_CoinLable;
    public UILabel m_Moneyinfo_YuanBaoLable;
    public UILabel m_Moneyinfo_YuanBaoBindLable;

    public UISprite[] m_TabPage_HighLight;

    public GameObject m_FakeObjTopLeft;
    public GameObject m_FakeObjBottomRight;

    private FakeObject m_PlayerFakeObj;
    private int m_PlayerFakeObjID = GlobeVar.INVALID_ID;
    private GameObject m_FakeObjGameObject;

    private UInt64 m_TakeOffGuid = GlobeVar.INVALID_GUID;
    public UInt64 TakeOffGuid
    {
        get { return m_TakeOffGuid; }
        set { m_TakeOffGuid = value; }
    }

    // 新手指引相关
    private int m_NewPlayerGuideFlag_Step = -1;
    public int NewPlayerGuideFlag_Step
    {
        get { return m_NewPlayerGuideFlag_Step; }
        set { m_NewPlayerGuideFlag_Step = value; }
    }
    public GameObject m_CloseButton; // 关闭按钮

    private int m_PlayerCombatBuffer = 0;

    public GameObject m_EquipView;
    public GameObject m_QianKunDaiView;

    public UISprite[] m_EquipSlotIcon;
    public UISprite[] m_EquipSlotQualityFrame;
    public ModelDragLogic m_ModelDrag;
    private ITEM_TAB_PAGE m_CurTabPage = ITEM_TAB_PAGE.TAB_PAGE_ALL; //当前背包分页

    public QianKunDaiLogic m_QianKunDai;

   
    void Awake()
    {
        m_Instance = this;
    }

    void OnEnable()
    {
        m_Instance = this;
        GameManager.gameManager.ActiveScene.InitFakeObjRoot(m_FakeObjTopLeft, m_FakeObjBottomRight);
        GameManager.gameManager.ActiveScene.ShowFakeObj();
        m_FirstChild.SetActive(true);
        if (Singleton<ObjManager>.GetInstance().MainPlayer != null)
        {
            m_PlayerCombatBuffer = Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.CombatValue;
        }
		if(m_QianKunDaiView!=null)
		m_QianKunDaiView.SetActive (false);
        Init();

        Check_NewPlayerGuide();   
    }
    void OnDisable()
    {
        if (Singleton<ObjManager>.GetInstance().MainPlayer != null)
        {
            int nNewCombatValue = Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.CombatValue;
            if (nNewCombatValue > m_PlayerCombatBuffer)
            {
                int nAddCombatValue = nNewCombatValue - m_PlayerCombatBuffer;
                PowerRemindLogic.InitPowerInfo(nNewCombatValue, nAddCombatValue);
            }
        }
        m_Instance = null;
        DestroyPartnerFakeObj();
        GameManager.gameManager.ActiveScene.HideFakeObj();

        if (m_NewPlayerGuideFlag_Step == 2)
        {
            NewPlayerGuide(3);
        }
    }

    private void Init()
    {
        InitBackPack();
        InitEquipPack();
        OnClick_TabAll();
    }

    /// <summary>
    /// 更新玩家信息显示
    /// </summary>
    public void UpdatePlayerInfo()
    {
        if (Singleton<ObjManager>.GetInstance().MainPlayer != null)
        {
            string name = Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.RoleName;
            m_PlayerName.text = "[ffff00]" + name;
            //int combatValue = Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.CombatValue;
            //m_PlayerCombatValue.text = combatValue.ToString(); 
			//=============
			m_PlayerCombatValue.text =GameManager.gameManager.PlayerDataPool.PoolCombatValue.ToString(); 

            int level = Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.Level;
            m_PlayerLevel.text = "[ffff00]" + level.ToString();
        }
    }

    /// <summary>
    /// 初始化装备槽
    /// </summary>
    public void InitEquipPack()
    {
        UpdateEquipPack();
        //根据职业显示预览模型
        if (null != Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            int profession = Singleton<ObjManager>.GetInstance().MainPlayer.Profession;
            CreatePartnerFakeObj(profession);
        }
    }

    /// <summary>
    /// 更新装备槽部分显示
    /// </summary>
    public void UpdateEquipPack()
    {
        GameItemContainer EquipPack = GameManager.gameManager.PlayerDataPool.EquipPack;
        for (int index = 0; index < EquipPack.ContainerSize; ++index)
        {
            GameItem equip = EquipPack.GetItem(GetEquipSlotByIndex(index));
            if (equip != null && equip.IsValid())
            {
                UISprite IconSprite = m_EquipSlotIcon[index];
                if (IconSprite != null)
                {
                    IconSprite.gameObject.SetActive(true);
                    IconSprite.spriteName = equip.GetIcon();
                    //IconSprite.MakePixelPerfect();

                    //显示品质边框
                    UISprite QualitySprite = m_EquipSlotQualityFrame[index];
                    if (QualitySprite != null)
                    {
                        QualitySprite.gameObject.SetActive(true);
                        QualitySprite.spriteName = equip.GetQualityFrame();
                        //QualitySprite.MakePixelPerfect();
                    }
                }
            }
            else
            {
                UISprite IconSprite = m_EquipSlotIcon[index];
                if (IconSprite != null)
                {
                    IconSprite.gameObject.SetActive(false);
                }
                UISprite QualitySprite = m_EquipSlotQualityFrame[index];
                if (QualitySprite != null)
                {
                    QualitySprite.gameObject.SetActive(false);
                }
            }
        }
    }

    int GetEquipSlotByIndex(int index)
    {
        switch (index)
        {
            case 0: return (int)EquipPackSlot.Slot_WEAPON;
            case 1: return (int)EquipPackSlot.Slot_HEAD;
            case 2: return (int)EquipPackSlot.Slot_RING;
            case 3: return (int)EquipPackSlot.Slot_ARMOR;
            case 4: return (int)EquipPackSlot.Slot_NECK;
            case 5: return (int)EquipPackSlot.Slot_CUFF;
            case 6: return (int)EquipPackSlot.Slot_AMULET;
            case 7: return (int)EquipPackSlot.Slot_LEG_GUARD;
            case 8: return (int)EquipPackSlot.Slot_BELT;
            case 9: return (int)EquipPackSlot.Slot_SHOES;
            default:
                break;
        }
        return -1;
    }

    /// <summary>
    /// 初始化背包
    /// </summary>
    public void InitBackPack()
    {
        InitBackPackItem();
        //UIManager.LoadItem(UIInfo.BackPackItem, OnLoadBackPackItem);
    }

    //void OnLoadBackPackItem(GameObject resObj, object param)
    //{
    //    if (null == resObj)
    //    {
    //        LogModule.ErrorLog("load back pack item error");
    //        return;
    //    }
    //    UpdatePlayerInfo();
    //    UpdateMoneyInfo();
    //    for (int nIndex = 0; nIndex < GameItemContainer.MAXSIZE_BACKPACK; ++nIndex)
    //    {
    //        Utils.BindObjToParent(resObj, m_BackPackItemGrid, (nIndex + 1000).ToString());
    //    }
    //    m_BackPackItemGrid.GetComponent<UIGrid>().sorted = true;
    //    m_BackPackItemGrid.GetComponent<UIGrid>().repositionNow = true;
    //    UpdateBackPack();
    //    InitEquipPack();
    //}

    void InitBackPackItem()
    {
        UpdatePlayerInfo();
        UpdateMoneyInfo();
        //for (int nIndex = 0; nIndex < GameItemContainer.MAXSIZE_BACKPACK; ++nIndex)
        //{
        //    Transform ItemTrans = m_BackPackItemGrid.transform.GetChild(nIndex);
        //    if (ItemTrans != null)
        //    {
        //        ItemTrans.gameObject.name = (1000 + nIndex).ToString();
        //    }
        //}
        m_BackPackItemGrid.GetComponent<UIGrid>().sorted = true;
        m_BackPackItemGrid.GetComponent<UIGrid>().repositionNow = true;
        UpdateBackPack();
    }
    
    /// <summary>
    /// 更新背包部分显示
    /// </summary>
    public void UpdateBackPack()
    {
        if (gameObject.activeSelf == false)
        {
            return;
        }

        //显示tab按钮高亮
        for (int i = 0; i < 5; ++i )
        {
            m_TabPage_HighLight[i].gameObject.SetActive(false);
        }

        int curTabIndex = (int)m_CurTabPage;
        if ( curTabIndex >= 0 && curTabIndex < 5 )
        {
            m_TabPage_HighLight[curTabIndex].gameObject.SetActive(true);
        }
        

        switch (m_CurTabPage)
        {
            case ITEM_TAB_PAGE.TAB_PAGE_ALL: ShowBackPack_All(); break;
            case ITEM_TAB_PAGE.TAB_PAGE_EQUIP: ShowBackPack_Equip(); break;
            case ITEM_TAB_PAGE.TAB_PAGE_MATERIAL: ShowBackPack_Material(); break;
            case ITEM_TAB_PAGE.TAB_PAGE_FELLOW: ShowBackPack_Fellow(); break;
            case ITEM_TAB_PAGE.TAB_PAGE_OTHER: ShowBackPack_Other(); break;
        };

        // 更新背包大小
        GameItemContainer BackPack = GameManager.gameManager.PlayerDataPool.BackPack;
        m_BackPakcSize.text = string.Format("{0}/{1}", BackPack.GetItemCount(), BackPack.ContainerSize);
    }

    /// <summary>
    /// 显示全部分页
    /// </summary>
    private void ShowBackPack_All()
    {
        // 过滤物品
        GameItemContainer BackPack = GameManager.gameManager.PlayerDataPool.BackPack;
		List<GameItem> itemlist = ItemTool.ItemFilter(BackPack, 0,0,m_QianKunDaiView.activeInHierarchy);
        if (null != itemlist)
        {
            CurItemList = itemlist;
            // 显示物品
            ShowBackPackItemList(itemlist);
        }        
    }

    /// <summary>
    /// 显示装备分页
    /// </summary>
    private void ShowBackPack_Equip()
    {
        // 过滤物品
        GameItemContainer BackPack = GameManager.gameManager.PlayerDataPool.BackPack;
		List<GameItem> itemlist = ItemTool.ItemFilter(BackPack, (int)ItemClass.EQUIP,0,m_QianKunDaiView.activeInHierarchy);
        CurItemList = itemlist;
        // 显示物品
        ShowBackPackItemList(itemlist);
    }

    /// <summary>
    /// 显示材料分页
    /// </summary>
    private void ShowBackPack_Material()
    {
        // 过滤物品
        GameItemContainer BackPack = GameManager.gameManager.PlayerDataPool.BackPack;
        List<GameItem> itemlist = ItemTool.ItemFilter(BackPack, (int)ItemClass.MATERIAL,0,m_QianKunDaiView.activeInHierarchy);
        CurItemList = itemlist;
        // 显示物品
        ShowBackPackItemList(itemlist);
    }

    /// <summary>
    /// 显示伙伴分页
    /// </summary>
    private void ShowBackPack_Fellow()
    {
        // 过滤物品
        GameItemContainer BackPack = GameManager.gameManager.PlayerDataPool.BackPack;
		List<GameItem> itemlist = ItemTool.ItemFilter(BackPack, (int)ItemClass.FELLOW,0,m_QianKunDaiView.activeInHierarchy);
        CurItemList = itemlist;
        // 显示物品
        ShowBackPackItemList(itemlist);
    }

    /// <summary>
    /// 显示其它分页
    /// </summary>
    private void ShowBackPack_Other()
    {
        // 过滤物品
        GameItemContainer BackPack = GameManager.gameManager.PlayerDataPool.BackPack;
		List<GameItem> itemlist1 = ItemTool.ItemFilter(BackPack, (int)ItemClass.STRENGTHEN,0,m_QianKunDaiView.activeInHierarchy);
		List<GameItem> itemlist2 = ItemTool.ItemFilter(BackPack, (int)ItemClass.PRIZE,0,m_QianKunDaiView.activeInHierarchy);
		List<GameItem> itemlist3 = ItemTool.ItemFilter(BackPack, (int)ItemClass.MEDIC,0,m_QianKunDaiView.activeInHierarchy);
		List<GameItem> itemlist4 = ItemTool.ItemFilter(BackPack, (int)ItemClass.MISSION,0,m_QianKunDaiView.activeInHierarchy);
		List<GameItem> itemlist5 = ItemTool.ItemFilter(BackPack, (int)ItemClass.MOUNT,0,m_QianKunDaiView.activeInHierarchy);
		List<GameItem> itemlist6 = ItemTool.ItemFilter(BackPack, (int)ItemClass.FASHION,0,m_QianKunDaiView.activeInHierarchy);
        itemlist1.AddRange(itemlist2);
        itemlist1.AddRange(itemlist3);
        itemlist1.AddRange(itemlist4);
        itemlist1.AddRange(itemlist5);
        itemlist1.AddRange(itemlist6);
        CurItemList = itemlist1;
        // 显示物品
        ShowBackPackItemList(itemlist1);
    }

    /// <summary>
    /// 将传入的物品现在在背包界面
    /// </summary>
    /// <param name="itemlist"></param>
    private void ShowBackPackItemList(List<GameItem> itemlist)
    {
        GameItemContainer BackPack = GameManager.gameManager.PlayerDataPool.BackPack;
        //[old] for (int nIndex = m_Cur_StartItem; nIndex < (m_Cur_EndItem+1); ++nIndex)
        //{
        for (int nIndex = 0; nIndex < GameItemContainer.MAXSIZE_BACKPACK; ++nIndex)
        {
            if (nIndex < BackPack.ContainerSize)
            {
                if (nIndex < itemlist.Count)
                {
                    GameItem item = itemlist[nIndex];
                    Transform ItemTrans = m_BackPackItemGrid.transform.FindChild((nIndex + 1000).ToString());
                    if (ItemTrans != null)
                    {
                        ItemTrans.gameObject.SetActive(true);
                        if (null != ItemTrans.gameObject.GetComponent<BackPackItemLogic>())
                            ItemTrans.gameObject.GetComponent<BackPackItemLogic>().UpdateBackPackItem(item);
                    }
                }
                else
                {
                    Transform ItemTrans = m_BackPackItemGrid.transform.FindChild((nIndex + 1000).ToString());
                    if (ItemTrans != null)
                    {
                        if (m_CurTabPage == ITEM_TAB_PAGE.TAB_PAGE_ALL)
                        {
                            ItemTrans.gameObject.SetActive(true);
                            if (null != ItemTrans.gameObject.GetComponent<BackPackItemLogic>())
                                ItemTrans.gameObject.GetComponent<BackPackItemLogic>().SetEmptyItem();
                        }
                        else
                        {
                            ItemTrans.gameObject.SetActive(false);
                        }
                    }
                }
            }
            else
            {
                Transform ItemTrans = m_BackPackItemGrid.transform.FindChild((nIndex + 1000).ToString());
                if (ItemTrans != null)
                {
                    //只有全部分页才显示未解锁格子
                    if (m_CurTabPage == ITEM_TAB_PAGE.TAB_PAGE_ALL)
                    {
                        ItemTrans.gameObject.SetActive(true);
                        if (null != ItemTrans.gameObject.GetComponent<BackPackItemLogic>())
                            ItemTrans.gameObject.GetComponent<BackPackItemLogic>().SetLockItem();
                    }
                    else
                    {
                        ItemTrans.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 关闭背包
    /// </summary>
    public void CloseWindow()
    {
        if (m_NewPlayerGuideFlag_Step == 2)
        {
            NewPlayerGuide(3);
        }
        //Utils.CleanGrid(m_BackPackItemGrid);
        UIManager.CloseUI(UIInfo.BackPackRoot);
		m_QianKunDaiView.SetActive (false);
		//UIManager.CloseUI(UIInfo.);
    }

    /// <summary>
    /// 更新金钱显示
    /// </summary>
    public void UpdateMoneyInfo()
    {
        m_Moneyinfo_CoinLable.text = Utils.ConvertLargeNumToString(GameManager.gameManager.PlayerDataPool.Money.GetMoney_Coin());
        m_Moneyinfo_YuanBaoLable.text = GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBao().ToString();
        m_Moneyinfo_YuanBaoBindLable.text = GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBaoBind().ToString();
    }
    
    ///////////////////////////////////////////////////////////////////////////////////////////////
    // 按钮点击响应 Start
    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void OnClick_TabAll()
    {
        m_CurTabPage = ITEM_TAB_PAGE.TAB_PAGE_ALL;
        m_BackPackItemGrid.GetComponent<UITopGrid>().Recenter(true);
        //OnItemDragFinished();
        UpdateBackPack();
    }

    public void OnClick_TabEquip()
    {
        m_CurTabPage = ITEM_TAB_PAGE.TAB_PAGE_EQUIP;
        m_BackPackItemGrid.GetComponent<UITopGrid>().Recenter(true);
        //OnItemDragFinished();
        UpdateBackPack();
    }

    public void OnClick_TabMaterial()
    {
        m_CurTabPage = ITEM_TAB_PAGE.TAB_PAGE_MATERIAL;
        m_BackPackItemGrid.GetComponent<UITopGrid>().Recenter(true);
        //OnItemDragFinished();
        UpdateBackPack();
    }

    public void OnClick_TabFellow()
    {
        m_CurTabPage = ITEM_TAB_PAGE.TAB_PAGE_FELLOW;
        m_BackPackItemGrid.GetComponent<UITopGrid>().Recenter(true);
        //OnItemDragFinished();
        UpdateBackPack();
    }

    public void OnClick_TabOther()
    {
        m_CurTabPage = ITEM_TAB_PAGE.TAB_PAGE_OTHER;
        m_BackPackItemGrid.GetComponent<UITopGrid>().Recenter(true);
        //OnItemDragFinished();
        UpdateBackPack();
    }

    public void OnClick_SellBeginButton()
    {
        //[old] m_SellBeginButton.gameObject.SetActive(false);
        //m_SellButton.gameObject.SetActive(true);
        ////打开选择框
        //foreach (BackPackItemLogic item in m_BackPackItemGrid.GetComponentsInChildren<BackPackItemLogic>())
        //{
        //    if (item.m_Item != null)
        //    {
        //        if (item.m_Item.CanSell())
        //        {
        //            item.EnableToggle();
        //        }
        //    }
        //}
    }

    //public void OnClick_SellButton()
    //{
    //    m_SellBeginButton.gameObject.SetActive(true);
    //    m_SellButton.gameObject.SetActive(false);

    //    List<ulong> selllist = new List<ulong>();
    //    bool bNeedNotify = false;
    //    BackPackItemLogic[] ItemArry = m_BackPackItemGrid.GetComponentsInChildren<BackPackItemLogic>();
    //    for (int i = 0; i < ItemArry.Length; i++)
    //    {
    //        BackPackItemLogic item = ItemArry[i];
    //        if (item.m_Toggle.value == true)
    //        {
    //            if (item.m_Item != null)
    //            {
    //                if (item.m_Item.CanSell())
    //                {
    //                    selllist.Add(item.m_Item.Guid);
    //                }
    //                else
    //                {
    //                    bNeedNotify = true;
    //                }
    //            }
    //        }
    //        if (selllist.Count >= 30)
    //        {
    //            //商店售出消息包只接受最多三十个物品
    //            SysShopController.SellItem((int)GameItemContainer.Type.TYPE_BACKPACK, selllist);
    //            selllist.Clear();
    //        }
    //    }
    //    if (bNeedNotify)
    //    {
    //        Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1297}");
    //    }
    //    if (selllist.Count > 0 && selllist.Count <= 30)
    //    {
    //        SysShopController.SellItem((int)GameItemContainer.Type.TYPE_BACKPACK, selllist);
    //        m_BackPackItemGrid.GetComponent<UITopGrid>().Recenter(true);
    //        //OnItemDragFinished();
    //    }
    //    //关闭选择框
    //    BackPackItemLogic[] BackPackItemArry = m_BackPackItemGrid.GetComponentsInChildren<BackPackItemLogic>();
    //    for (int i = 0; i < BackPackItemArry.Length; i++)
    //    {
    //        BackPackItemLogic item = BackPackItemArry[i];
    //        item.DisableToggle();
    //    }
    //}

    public void OnClick_PayButton()
    {
        RechargeData.PayUI();
    }

    public void OnClick_UnLockButton()
    {
        int size = GameManager.gameManager.PlayerDataPool.BackPack.ContainerSize;
        Tab_BackPackUnlock line = TableManager.GetBackPackUnlockByID(((int)(size - GameItemContainer.SIZE_BACKPACK)/10 + 1), 0);
        if (line != null)
        {
            string str = StrDictionary.GetClientDictionaryString("#{1367}", line.ConsumeNum);
            MessageBoxLogic.OpenOKCancelBox(str, "", BackPackItemLogic.UnlockOk, BackPackItemLogic.UnlockCancel);
        }
        else if (null != Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1445}");
        }
    }

    /// <summary>
    /// 点击装备槽位
    /// </summary>
    public void OnClick_Equip_WEAPON()
    {
        GameItem item = GameManager.gameManager.PlayerDataPool.EquipPack.GetItem((int)EquipPackSlot.Slot_WEAPON);
        if (item != null && item.IsValid())
        {
            EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Equiped);
        }   
    }

    /// <summary>
    /// 点击装备槽位
    /// </summary>
    public void OnClick_Equip_HEAD()
    {
        GameItem item = GameManager.gameManager.PlayerDataPool.EquipPack.GetItem((int)EquipPackSlot.Slot_HEAD);
        if (item != null && item.IsValid())
        {
            EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Equiped);
        }
    }

    /// <summary>
    /// 点击装备槽位
    /// </summary>
    public void OnClick_Equip_ARMOR()
    {
        GameItem item = GameManager.gameManager.PlayerDataPool.EquipPack.GetItem((int)EquipPackSlot.Slot_ARMOR);
        if (item != null && item.IsValid())
        {
            
            EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Equiped);
        }
    }

    /// <summary>
    /// 点击装备槽位
    /// </summary>
    public void OnClick_Equip_LEG_GUARD()
    {
        GameItem item = GameManager.gameManager.PlayerDataPool.EquipPack.GetItem((int)EquipPackSlot.Slot_LEG_GUARD);
        if (item != null && item.IsValid())
        {
            
            EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Equiped);
        }
    }

    /// <summary>
    /// 点击装备槽位
    /// </summary>
    public void OnClick_Equip_CUFF()
    {
        GameItem item = GameManager.gameManager.PlayerDataPool.EquipPack.GetItem((int)EquipPackSlot.Slot_CUFF);
        if (item != null && item.IsValid())
        {
            
            EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Equiped);
        }
    }

    /// <summary>
    /// 点击装备槽位
    /// </summary>
    public void OnClick_Equip_SHOES()
    {
        GameItem item = GameManager.gameManager.PlayerDataPool.EquipPack.GetItem((int)EquipPackSlot.Slot_SHOES);
        if (item != null && item.IsValid())
        {
            
            EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Equiped);
        }
    }

    /// <summary>
    /// 点击装备槽位
    /// </summary>
    public void OnClick_Equip_NECK()
    {
        GameItem item = GameManager.gameManager.PlayerDataPool.EquipPack.GetItem((int)EquipPackSlot.Slot_NECK);
        if (item != null && item.IsValid())
        {
            
            EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Equiped);
        }
    }

    /// <summary>
    /// 点击装备槽位
    /// </summary>
    public void OnClick_Equip_RING()
    {
        GameItem item = GameManager.gameManager.PlayerDataPool.EquipPack.GetItem((int)EquipPackSlot.Slot_RING);
        if (item != null && item.IsValid())
        {
            
            EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Equiped);
        }
    }

    /// <summary>
    /// 点击装备槽位
    /// </summary>
    public void OnClick_Equip_AMULET()
    {
        GameItem item = GameManager.gameManager.PlayerDataPool.EquipPack.GetItem((int)EquipPackSlot.Slot_AMULET);
        if (item != null && item.IsValid())
        {

            EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Equiped);
        }
    }

    /// <summary>
    /// 点击装备槽位
    /// </summary>
    public void OnClick_Equip_BELT()
    {
        GameItem item = GameManager.gameManager.PlayerDataPool.EquipPack.GetItem((int)EquipPackSlot.Slot_BELT);
        if (item != null && item.IsValid())
        {

            EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Equiped);
        }
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////
    // 按钮点击响应 End
    ///////////////////////////////////////////////////////////////////////////////////////////////
    public const int ITEMOBJECT_COUNT = 25;     //实际创建多少个Itemobject
    public const int ITEMPACK_SIZE = 90;        //最多需要显示多少个Item
    public const int ITEMOBJECT_WIDTH = 80;     //每个ItemObject的宽度
    
    private int m_Cur_StartItem = 0;                   //存放当前存在的itemobject开始编号
    private int m_Cur_EndItem = 0 + ITEMOBJECT_COUNT;  //存放当前存在的itemobject结束编号

    private List<GameItem> CurItemList;                //当前需要显示的Item

    /// <summary>
    /// 每次滑动结束后调用
    /// 
    /// 根据此次滑动是向上还是向下：1或2
    /// 1.把上面的gameobject移动到下面 并填充新的item信息
    /// 2.把下面的gameobject移动到上面 并填充新的item信息
    /// 
    /// 总共只有26个object 通过不断的上下移动和改变信息来实现完整显示
    /// </summary>
    public void OnItemDragFinished()
    {
        //根据DragPanel坐标 和 ItemObject宽度  计算出此时应该显示的Item的开始编号和结束编号
        int panelPosY = (int)m_BackPackItems.transform.localPosition.y;
        int Target_StartItem = (int)(panelPosY / ITEMOBJECT_WIDTH) - 10;
        int Target_EndItem = (int)(panelPosY / ITEMOBJECT_WIDTH) + 15;

        //开始编号和结束编号的取值范围： [0, ITEMPACK_SIZE) 前开后闭
        if (Target_StartItem < 0)
        {
            Target_StartItem = 0;
        }
        if (Target_EndItem > ITEMPACK_SIZE-1)
        {
            Target_EndItem = ITEMPACK_SIZE - 1;
        }

        if (Target_StartItem > m_Cur_StartItem && Target_EndItem > m_Cur_EndItem)
        {
            //手指向上滑   顶端的ItemObject移动到尾端 显示较大编号Item
            for (int nIndex = m_Cur_StartItem; nIndex < Target_StartItem; ++nIndex )
            {
                int TargetPos = m_Cur_EndItem + 1 + (nIndex - m_Cur_StartItem);
                if (TargetPos < 0 || TargetPos > ITEMPACK_SIZE-1)
                {
                    continue;
                }
                Transform ItemTransform = m_BackPackItemGrid.transform.FindChild((nIndex+1000).ToString());
                if (ItemTransform != null)
                {
                    GameObject ItemObject = ItemTransform.gameObject;
                    //移动到目标位置
                    int y = (-1)*(TargetPos * ITEMOBJECT_WIDTH);
                    ItemObject.transform.localPosition = new Vector3(0, y, 0);
                    //改为目标名称
                    ItemObject.gameObject.name = (TargetPos + 1000).ToString();
                    //填充要显示的Item内容
                    GameItemContainer BackPack = GameManager.gameManager.PlayerDataPool.BackPack;
                    if (TargetPos < BackPack.ContainerSize)
                    {
                        if (TargetPos < CurItemList.Count)
                        {
                            //有物品的格子
                            GameItem Item = CurItemList[TargetPos];
                            ItemObject.gameObject.SetActive(true);
                            if (null != ItemObject.GetComponent<BackPackItemLogic>())
                                ItemObject.GetComponent<BackPackItemLogic>().UpdateBackPackItem(Item);
                        }
                        else
                        {
                            //空格子
                            if (m_CurTabPage == ITEM_TAB_PAGE.TAB_PAGE_ALL)
                            {
                                ItemObject.gameObject.SetActive(true);
                                if (null != ItemObject.GetComponent<BackPackItemLogic>())
                                    ItemObject.GetComponent<BackPackItemLogic>().SetEmptyItem();
                            }
                            else
                            {
                                ItemObject.gameObject.SetActive(false);
                            }
                        }
                    }
                    else
                    {
                        //未解锁的格子
                        //只显示一个未解锁格子
                        if (m_CurTabPage == ITEM_TAB_PAGE.TAB_PAGE_ALL)
                        {
                            ItemObject.gameObject.SetActive(true);
                            if (null != ItemObject.GetComponent<BackPackItemLogic>())
                                ItemObject.GetComponent<BackPackItemLogic>().SetLockItem();
                        }
                        else
                        {
                            ItemObject.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }

        if (Target_StartItem < m_Cur_StartItem && Target_EndItem < m_Cur_EndItem)
        {
            //手指向下滑   尾端的ItemObject移动到顶端 显示较小编号Item
            for (int nIndex = m_Cur_EndItem; nIndex > Target_EndItem; --nIndex)
            {
                int TargetPos = m_Cur_StartItem - 1 - (m_Cur_EndItem - nIndex);
                if (TargetPos < 0 || TargetPos > ITEMPACK_SIZE - 1)
                {
                    continue;
                }
                Transform ItemTransform = m_BackPackItemGrid.transform.FindChild((nIndex + 1000).ToString());
                if (ItemTransform != null)
                {
                    GameObject ItemObject = ItemTransform.gameObject;
                    //移动到目标位置
                    int y = (-1) * (TargetPos * ITEMOBJECT_WIDTH);
                    ItemObject.transform.localPosition = new Vector3(0, y, 0);
                    //改为目标名称
                    ItemObject.gameObject.name = (TargetPos + 1000).ToString();
                    //填充要显示的Item内容
                    GameItemContainer BackPack = GameManager.gameManager.PlayerDataPool.BackPack;
                    if (TargetPos < BackPack.ContainerSize)
                    {
                        if (TargetPos < CurItemList.Count)
                        {
                            //有物品的格子
                            GameItem Item = CurItemList[TargetPos];
                            ItemObject.gameObject.SetActive(true);
                            ItemObject.GetComponent<BackPackItemLogic>().UpdateBackPackItem(Item);
                        }
                        else
                        {
                            //空格子
                            if (m_CurTabPage == ITEM_TAB_PAGE.TAB_PAGE_ALL)
                            {
                                ItemObject.gameObject.SetActive(true);
                                ItemObject.GetComponent<BackPackItemLogic>().SetEmptyItem();
                            }
                            else
                            {
                                ItemObject.gameObject.SetActive(false);
                            }
                        }
                    }
                    else
                    {
                        //未解锁的格子
                        //只显示一个未解锁格子
                        if (m_CurTabPage == ITEM_TAB_PAGE.TAB_PAGE_ALL)
                        {
                            ItemObject.gameObject.SetActive(true);
                            ItemObject.GetComponent<BackPackItemLogic>().SetLockItem();
                        }
                        else
                        {
                            ItemObject.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }

        //更新当前开始编号 和 结束编号
        if (Target_StartItem >= 0 && Target_EndItem < ITEMPACK_SIZE)
        {
            if ((Target_EndItem - Target_StartItem) == ITEMOBJECT_COUNT)
            {
                m_Cur_StartItem = Target_StartItem;
                m_Cur_EndItem = Target_EndItem;
            }
            else if (Target_StartItem == 0)
            {
                m_Cur_StartItem = 0;
                m_Cur_EndItem = 0 + ITEMOBJECT_COUNT;
            }
            else if (Target_EndItem == (ITEMPACK_SIZE - 1))
            {
                m_Cur_StartItem = ITEMPACK_SIZE - 1 - ITEMOBJECT_COUNT;
                m_Cur_EndItem = ITEMPACK_SIZE - 1;
            }
        }
    }

    /// <summary>
    /// 销毁FakeObj
    /// </summary>
    private void DestroyPartnerFakeObj()
    {
        if (m_PlayerFakeObj != null)
        {
            m_PlayerFakeObj.Destroy();
            m_PlayerFakeObj = null;
        }
    }

    private void CreatePartnerFakeObj(int pro)
    {
        if (m_PlayerFakeObj != null)
        {
            DestroyPartnerFakeObj();
        }
        CharacterDefine.PROFESSION profession = (CharacterDefine.PROFESSION)pro;
        //对应FakeObject.txt配置
        int fakeObjId = -1;
        switch (profession)
        {
            case CharacterDefine.PROFESSION.SHAOLIN:
                fakeObjId = 3;
                break;
            case CharacterDefine.PROFESSION.TIANSHAN:
                fakeObjId = 4;
                break;
            case CharacterDefine.PROFESSION.DALI:
                fakeObjId = 6;
                break;
            case CharacterDefine.PROFESSION.XIAOYAO:
                fakeObjId = 5;
                break;
            default:
                fakeObjId = 3;
                break;
        }

        m_PlayerFakeObj = new FakeObject();
        if (m_PlayerFakeObj == null)
        {
            return;
        }

        m_PlayerFakeObjID = fakeObjId;
        m_PlayerFakeObj.initFakeObject(fakeObjId, GameManager.gameManager.ActiveScene.FakeObjTrans, out m_FakeObjGameObject);
        m_ModelDrag.ModelTrans = m_PlayerFakeObj.ObjAnim.transform;
    }

    /// <summary>
    /// 把开始编号和结束编号恢复到 0和25
    /// 切换分页时使用
    /// </summary>
    private void InitStartAndEnd()
    {
        for (int nIndex = m_Cur_StartItem; nIndex < (m_Cur_EndItem + 1); ++nIndex )
        {
            Transform ItemTransform = m_BackPackItemGrid.transform.FindChild((nIndex+1000).ToString());
            if (ItemTransform != null)
            {
                GameObject ItemObject = ItemTransform.gameObject;
                ItemObject.gameObject.name = (nIndex - m_Cur_StartItem + 1000).ToString();
            }
        }
        m_Cur_StartItem = 0;
        m_Cur_EndItem = 0 + ITEMOBJECT_COUNT;
    }

    void Check_NewPlayerGuide()
    {
        if (MenuBarLogic.Instance() == null)
        {
            return;
        }
        int nIndex = MenuBarLogic.Instance().NewPlayerGuideIndex;
        if((int) GameDefine_Globe.NEWOLAYERGUIDE.COMPOUND == nIndex)
		{
			NewPlayerGuide(1);
			MenuBarLogic.Instance().NewPlayerGuideIndex = -1;
		}
    }

    public void NewPlayerGuide(int nIndex)
    {
		if (nIndex < 0) 
		{
			return;		
		}
		
		NewPlayerGuidLogic.CloseWindow();

		m_NewPlayerGuideFlag_Step = nIndex;

		switch (m_NewPlayerGuideFlag_Step)
		{
		case 1:
			int nPoisonForgCount = GameManager.gameManager.PlayerDataPool.BackPack.GetItemCountByDataId(GlobeVar.CompountDataID);
			if (nPoisonForgCount < 3)
			{
				return;
			}

			GameObject ItemObject = null;

			for(int i = 0; i < ITEMPACK_SIZE; i++)
			{
				Transform ItemTransform = m_BackPackItemGrid.transform.FindChild((i + 1000).ToString());
				if (ItemTransform != null)
				{
					GameObject ItemObject1 = ItemTransform.gameObject;
					if(ItemObject1.GetComponent<BackPackItemLogic>().m_Item.DataID == GlobeVar.CompountDataID 
					   && ItemObject1.GetComponent<BackPackItemLogic>().m_Item.StackCount >= 3)
					{
						ItemObject = ItemObject1;
						break;
					}
					else if(ItemObject1.GetComponent<BackPackItemLogic>().m_Item.DataID == -1)
					{
						break;
					}
				}
			}
			if(ItemObject == null)
			{
				return;
			}
			NewPlayerGuidLogic.OpenWindow(ItemObject, 112, 112, "", "right", 2, true, true);
			break;
		}
        

    }

    //public void OnItemDragFinished()
    //{
    //    int dragPosY = (int)m_BackPackItems.transform.localPosition.y;
    //    int startItem = (int)(dragPosY / 80) - 10;
    //    int endItem = (int)(dragPosY / 80) + 15;

    //    for (int nIndex = 0; nIndex < 90; ++nIndex)
    //    {
    //        GameObject ItemObject = m_BackPackItemGrid.transform.FindChild((nIndex + 1000).ToString()).gameObject;
    //        if (ItemObject != null)
    //        {
    //            if (nIndex < startItem || nIndex > endItem)
    //            {
    //                if (ItemObject.activeSelf == true)
    //                {
    //                    ItemObject.SetActive(false);
    //                }
    //            }
    //            else
    //            {
    //                if (ItemObject.activeSelf == false)
    //                {
    //                    ItemObject.SetActive(true);
    //                }
    //            }
    //        }
    //    }
    //}

    static public void SwitchQianKunDaiView(bool bSuccess, object param)
    {
        if (bSuccess)
        {
            if (BackPackLogic.Instance() != null)
            {
                BackPackLogic.Instance().m_EquipView.SetActive(false);
                BackPackLogic.Instance().m_QianKunDaiView.SetActive(true);
				Instance().UpdateBackPack();
                GameManager.gameManager.ActiveScene.HideFakeObj();
            }
        }
    }

    static public void SwitchEquipPackView(bool bSuccess, object param)
    {
        if (bSuccess)
        {
            if (BackPackLogic.Instance() != null)
            {
                BackPackLogic.Instance().m_EquipView.SetActive(true);
                BackPackLogic.Instance().m_QianKunDaiView.SetActive(false);
                GameManager.gameManager.ActiveScene.ShowFakeObj();
            }
        }
    }

	public static void SwitchQianKunDaiViewPutInDirectly(bool bSuccess, object param)
	{
		if (bSuccess && (param != null))
		{
			GameItem item = param as GameItem;
			if (((item != null) && item.IsValid()) && (Instance() != null))
			{
				Instance().m_EquipView.SetActive(false);
				Instance().m_QianKunDaiView.SetActive(true);
				Instance().UpdateBackPack();
				GameManager.gameManager.ActiveScene.HideFakeObj();
				BackPackItemLogic[] componentsInChildren = Instance().m_BackPackItemGrid.GetComponentsInChildren<BackPackItemLogic>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if ((null != componentsInChildren[i]&&componentsInChildren[i].m_Item!=null) && (componentsInChildren[i].m_Item.Guid == item.Guid))
					{
						Instance().m_QianKunDai.ChooseStuff(componentsInChildren[i].m_Item, componentsInChildren[i].m_ItemSlot);
						break;
					}
				}
			}
		}
	}
    public void UpdatePlayerEquipView()
    {
        int nModelVisualID = Singleton<ObjManager>.Instance.MainPlayer.ModelVisualID;
        bool isDefaultVisual = false;
        Tab_ItemVisual tabItemVisual = null;

        tabItemVisual = TableManager.GetItemVisualByID(nModelVisualID, 0);
        if (tabItemVisual == null)
        {
            isDefaultVisual = true;
        }
        else
        {
            isDefaultVisual = false;
        }

        if (isDefaultVisual)
        {
            tabItemVisual = TableManager.GetItemVisualByID(GlobeVar.DEFAULT_VISUAL_ID, 0);
            if (tabItemVisual == null)
            {
                return;
            }
        }

        int nCharModelID = Singleton<ObjManager>.Instance.MainPlayer.GetCharModelID(tabItemVisual);
        Tab_CharModel tabCharModel = TableManager.GetCharModelByID(nCharModelID, 0);
        if (tabCharModel == null)
        {
            return;
        }

        Tab_FakeObject tabFakeObject = TableManager.GetFakeObjectByID(m_PlayerFakeObjID, 0);
        if (tabFakeObject == null)
        {
            return;
        }

        Singleton<ObjManager>.GetInstance().ReloadModel(m_FakeObjGameObject,
            tabCharModel.ResPath,
            Singleton<ObjManager>.GetInstance().AsycLoadFakeObjOver,
            tabFakeObject,
            m_PlayerFakeObj);
    }

    public void CancelItemSlotChoose(UInt64 guid)
    {
        BackPackItemLogic[] item = m_BackPackItemGrid.GetComponentsInChildren<BackPackItemLogic>();
        for (int i = 0; i < item.Length; i++ )
        {
            if (item[i].m_Item.Guid == guid)
            {
                item[i].m_ItemSlot.ItemSlotChooseCancel();
                break;
            }
        }
    }

    // 整理
    void TidyBag()
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }
        int oldSize = GameManager.gameManager.PlayerDataPool.BackPack.ContainerSize;
        GameManager.gameManager.PlayerDataPool.BackPack = new GameItemContainer(oldSize, GameItemContainer.Type.TYPE_EQUIPPACK);
        CG_ASK_TIDY msg = (CG_ASK_TIDY)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_TIDY);
        msg.SetBagType(0);
        msg.SendPacket();
    }
}