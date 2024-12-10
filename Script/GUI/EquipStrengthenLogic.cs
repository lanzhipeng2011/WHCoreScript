//********************************************************************
// 文件名: EquipStrengthenLogic.cs
// 描述: 装备强化界面UI逻辑
// 作者: TangYi
// 创建时间: 2013-12-25
//
// 修改历史:
//********************************************************************
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Games.Item;
using Games.GlobeDefine;
using GCGame.Table;
using Module.Log;
using GCGame;
public class EquipStrengthenLogic : UIControllerBase<EquipStrengthenLogic>
{
    public TabController m_TabController;         //分页控制器

    public GameObject m_Content_Enchance;
    public GameObject m_Content_Enchance_Grid;
    public GameObject m_Content_Star;
    public GameObject m_TabBtnStar;              // 打星页签
	public GameObject m_TabBtnGem;

    private const int EquipNum = 10;
    public UISprite[] m_Equip_Sprite;
    public UISprite[] m_Equip_Choose;
    public UISprite[] m_Equip_QualityFrame;
    public UILabel[] m_Equip_Lable;

    public UISprite[]       m_Star_StarLevelSprite;                 //打星 选中装备星级图标
    public UILabel          m_Star_Choose_Name;                     //打星 选中装备名字
    public EquipStrengthenAttrStar m_Star_Choose_Attr;              //打星 选中装备属性
    public UISprite         m_Star_Material_Sprite;                 //打星 材料图标
    public UILabel          m_Star_Material_Label;                  //打星 材料数量
    public UISprite         m_Star_Material_QualitySprite;          //打星 材料品质框
    public UILabel          m_Star_NeedCoin_Label;                  //打星 需要消耗金币
    public UILabel          m_Star_StarLevelMaxNotice;              //打星 星级已满提示
    public GameObject       m_Star_ConsumInfo;                      //打星 消耗信息 星级已满时隐藏
    public GameObject       m_Star_NextLevelInfo;                   //打星 下一级信息 星级已满时隐藏
    public UILabel          m_Star_SuccessRate_Label;               //打星 成功率
    public UILabel          m_Star_CurMaterial_Label;               //打星 当前拥有材料数量
    public UILabel          m_Star_CurMoney_Label;                  //打星 当前拥有金币数量
    public UISprite[]       m_Star_Mode_Choose_Sprite;              //打星 模式选中图标
    public GameObject       m_BtnMakeStar;                          //打星按钮

    public UILabel          m_Enchance_EquipName_Label;             //强化 选中装备名字
    public UILabel          m_Enchance_NextEncLevel_Label;          //强化 下一强化等级
    public EquipStrengthenAttrEnchance m_Enchance_EquipAttr;        //强化 选中装备属性
    public UISlider         m_Enchance_EncExp;                      //强化 强化经验条
    public UILabel          m_Enchance_EncExpValue;                 //强化 强化经验值
    public UILabel          m_Enchance_MaxLevelLabel;               //强化 等级已满提示
    public GameObject       m_Enchance_NextInfo;                    //强化 下一级信息 等级已满时隐藏
    public GameObject       m_Enchance_ChooseList;                  //强化 智能选择

    public PlayerGemDataViewModel GemViewModel;                     //镶嵌 镶嵌宝石
    public GameObject SetGemTips;                                   //镶嵌 镶嵌信息
    public GameObject SetGem;                                       //镶嵌 镶嵌宝石
    public GameObject RemoveGem;                                    //镶嵌 摘除宝石
    public GameObject EquipGemsGrid;                                //镶嵌 当前装备的宝石
    public GameObject AllGemsGrid;                                  //镶嵌 所有宝石
    private int       CurAllGemPageNumber = 0;                      //镶嵌 当前所有镶嵌宝石的页数
    private int       AllGemPageSize = 8;                           //镶嵌 所有宝石每页容量
    List<GameItem>    m_GemItemList;                                //镶嵌 适合当前装备所有宝石
    GameItem          CurSelSetGem;                                 //镶嵌 当前选择的需要镶嵌宝石
    int               SelSetGemIndex;
    int               SelEquipGemIndex;

	public GameObject m_GemHole0;
	public GameObject m_GemIcom0;
	public GameObject m_SetGemBtn;

    public GameObject       m_Enchance_AbsorbButton;                // 吸收按钮
    public GameObject       m_CloseButton;                          // 关闭按钮

    public UIToggle         m_ValuableToggle;                       //贵重材料勾选框


    private int m_CurSelectEquipIndex;
    private GameItem m_CurSelectEquip = null;   //当前选择装备
    private GameObject m_FristMeterialEquip = null;   //第一个材料装备

    private int m_CurStarMaterialID = -1;

    private float   m_Delay_Enchance_Time = 0f;
    private int     m_Delay_Enchance_CurPackBack = 0;
    private ulong   m_Delay_Enchance_EquipGuid = 0;
    private List<ulong> m_Delay_Enchance_MetarialList = new List<ulong>();
 
    private float   m_Delay_Star_Time = 0f;
    private int     m_Delay_Star_CurPackBack = 0;
    private ulong   m_Delay_Star_EquipGuid = 0;
    private int     m_Delay_Star_TargetLevel = 0;

    private float m_Delay_Gem_Time = -1f;
    private int m_Delay_Gem_CurPackBack;
    private int m_Delay_Gem_GemIndex; 
    private ulong m_Delay_Gem_GemGuid; 

    private const string EnchanceTabStr = "Button1-Strengthen";
    private const string StarTabStr = "Button2-Star";
    private const string GemTabStr = "Button3-Gem";

    public UILabel SelGemInfo;
    public UILabel SetGemNumber;

    public bool m_AutoChooseEnchanceStone = true;  //是否自动选中玄铁

    // 新手指引
    private int m_NewPlayerGuideFlag_Step = -1;
    public int NewPlayerGuideFlag_Step
    {
        get { return m_NewPlayerGuideFlag_Step; }
        set { m_NewPlayerGuideFlag_Step = value; }
    }
    private bool m_IsNotFirstOpenFlag = false; // 新手指引用 强化材料多次刷新问题
    private bool IsFirstShow = true;


    public GameObject QiangHuaChengGongEffect;
    public enum StarMode
    {
        STAR_NORMAL = 0,        //单次打星
        STAR_CUR12 = 1,         //打星至当前12星
        STAR_MAX12 = 2,         //打星至橙色12星

        Mode_Num = 3,
    }
    private int m_Star_Mode;

    //打开默认分页
    public enum TabPage
    {
        TAB_ENCHANCE = 1,
        TAB_STAR = 2,
    }
    public static TabPage m_DefaultTab = TabPage.TAB_ENCHANCE;
    public static void ShowWindow(TabPage DefaultTab, UIManager.OnOpenUIDelegate delOpenUI = null)
    {
        m_DefaultTab = DefaultTab;
        UIManager.ShowUI(UIInfo.EquipStren, delOpenUI);
    }

    public static bool IsAutoEnchanceMaterial(GameItem item)
    {
        List<GameItem> templist = new List<GameItem>();
        templist.Add(item);
        templist = ItemTool.EnchanceMaterialFilter(templist, false);
        if (templist.Count >= 1)
        {
            return true;
        }
        return false;
    }

    void OnEnable()
    {
        GemViewModel = GameViewModel.Get<PlayerGemDataViewModel>();
        QiangHuaChengGongEffect.SetActive(false);
        m_ValuableToggle.value = false;
        SetInstance(this);
        if (m_TabController != null)
        {
            m_TabController.InitData();
            m_TabController.delTabChanged = OnTabChanged;
        }

        InvokeRepeating("SlowUpdate", 0f, 0.3f);

        if (m_DefaultTab == TabPage.TAB_ENCHANCE)
        {
            m_TabController.ChangeTab(EnchanceTabStr);
        }
        else
        {
            m_TabController.ChangeTab(StarTabStr);
        }
    }

    void OnDisable()
    {
        //关闭界面时 如果有延迟发包 则直接发包
        if (m_Delay_Enchance_Time > 0)
        {
            //发送消息包
            CG_EQUIP_ENCHANCE equipEnchance = (CG_EQUIP_ENCHANCE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_EQUIP_ENCHANCE);
            equipEnchance.SetPacktype(m_Delay_Enchance_CurPackBack);
            equipEnchance.SetEquipguid(m_Delay_Enchance_EquipGuid);
            for (int i = 0; i < m_Delay_Enchance_MetarialList.Count; ++i)
            {
                equipEnchance.AddMaterialguid(m_Delay_Enchance_MetarialList[i]);
            }
            equipEnchance.SendPacket();

            m_Delay_Enchance_CurPackBack = 0;
            m_Delay_Enchance_EquipGuid = 0;
            m_Delay_Enchance_MetarialList.Clear();
        }
        if (m_Delay_Star_Time > 0)
        {
            //发送消息包
            CG_EQUIP_STAR equipStar = (CG_EQUIP_STAR)PacketDistributed.CreatePacket(MessageID.PACKET_CG_EQUIP_STAR);
            equipStar.SetPacktype(m_Delay_Star_CurPackBack);
            equipStar.SetEquipguid(m_Delay_Star_EquipGuid);
            if (m_Delay_Star_TargetLevel > 0)
            {
                equipStar.SetStarlevel((uint)m_Delay_Star_TargetLevel);
            }
            equipStar.SendPacket();

            m_Delay_Star_CurPackBack = 0;
            m_Delay_Star_EquipGuid = 0;
            m_Delay_Star_TargetLevel = 0;
        }

        if (this.m_Delay_Gem_Time >= 0)
        {
                CG_PUT_GEM gemPacket = (CG_PUT_GEM)PacketDistributed.CreatePacket(MessageID.PACKET_CG_PUT_GEM);
                gemPacket.SetEquipslot((uint)this.m_Delay_Gem_CurPackBack);
                gemPacket.SetIndex((uint)this.m_Delay_Gem_GemIndex);
                gemPacket.SetItemguid(this.m_Delay_Gem_GemGuid);
                gemPacket.SendPacket();
                this.m_Delay_Star_CurPackBack = -1;
                this.m_Delay_Gem_Time = -1;
        }

        //关闭界面时 如果有UI特效播放 则关闭特效
        if (BackCamerControll.Instance())
        {
            BackCamerControll.Instance().StopSceneEffect(130, true);
            BackCamerControll.Instance().StopSceneEffect(131, true);
            BackCamerControll.Instance().StopSceneEffect(133, true);
        }

        CancelInvoke("SlowUpdate");
        SetInstance(null);
    }

    void OnDestroy()
    {
        
    }

    void SlowUpdate()
    {
        if (m_Delay_Enchance_Time > 0)
	    {
            //强化延迟发包
            m_Delay_Enchance_Time -= 0.3f;
            if (m_Delay_Enchance_Time <= 0)
            {
                // 新手指引
                if (m_NewPlayerGuideFlag_Step == 5)
                {
                    NewPlayerGuide(3);
                }
                //发送消息包
                CG_EQUIP_ENCHANCE equipEnchance = (CG_EQUIP_ENCHANCE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_EQUIP_ENCHANCE);
                equipEnchance.SetPacktype(m_Delay_Enchance_CurPackBack);
                equipEnchance.SetEquipguid(m_Delay_Enchance_EquipGuid);
                for (int i = 0; i < m_Delay_Enchance_MetarialList.Count; ++i)
                {
                    equipEnchance.AddMaterialguid(m_Delay_Enchance_MetarialList[i]);
                }
                equipEnchance.SendPacket();

                m_Delay_Enchance_CurPackBack = 0;
                m_Delay_Enchance_EquipGuid = 0;
                m_Delay_Enchance_MetarialList.Clear();
            }
	    }

        if (m_Delay_Star_Time > 0)
        {
            // 新手指引
            if (m_NewPlayerGuideFlag_Step == 7)
            {
                NewPlayerGuide(3);
            }
            //打星延迟发包
            m_Delay_Star_Time -= 0.3f;
            if (m_Delay_Star_Time <= 0)
            {
                //发送消息包
                CG_EQUIP_STAR equipStar = (CG_EQUIP_STAR)PacketDistributed.CreatePacket(MessageID.PACKET_CG_EQUIP_STAR);
                equipStar.SetPacktype(m_Delay_Star_CurPackBack);
                equipStar.SetEquipguid(m_Delay_Star_EquipGuid);
                if (m_Delay_Star_TargetLevel > 0)
                {
                    equipStar.SetStarlevel((uint)m_Delay_Star_TargetLevel);
                }
                equipStar.SendPacket();

                m_Delay_Star_CurPackBack = 0;
                m_Delay_Star_EquipGuid = 0;
                m_Delay_Star_TargetLevel = 0;
            }
        }

        if (m_Delay_Gem_Time >= 0) 
        {
            this.m_Delay_Gem_Time -= 0.3f;
            if (m_Delay_Gem_Time < 0)
            {
                if (11 == NewPlayerGuideFlag_Step)
                {
                    NewPlayerGuide(3);
                }  
                CG_PUT_GEM gemPacket = (CG_PUT_GEM)PacketDistributed.CreatePacket(MessageID.PACKET_CG_PUT_GEM);
                gemPacket.SetEquipslot((uint)this.m_Delay_Gem_CurPackBack);
                gemPacket.SetIndex((uint)this.m_Delay_Gem_GemIndex);
                gemPacket.SetItemguid(this.m_Delay_Gem_GemGuid);
                gemPacket.SendPacket();
                this.m_Delay_Gem_CurPackBack = -1;
                m_Delay_Gem_Time = -1;
            }
        }
	}
   
    public void OnTabChanged(TabButton tableButton)
    {
        UpdateTab();
    }

    public void UpdateTab()
    {
        if (m_TabController.GetHighlightTab() != null)
        {
            GameObject curTab = m_TabController.GetHighlightTab().gameObject;
            if (null != curTab)
            {
                if (curTab.name == EnchanceTabStr)
                {
                    UpdateTab_Enchance();
                }
                else if (curTab.name == StarTabStr)
                {
                    UpdateTab_Star();
                }
                else if (curTab.name == GemTabStr) 
                {
                    UpdateTab_Gem();
                }
            }
        }
    }

    public bool IsCurEnchanceTab()
    {
        if (m_TabController.GetHighlightTab() == null)
        {
            return false;
        }
        GameObject curTab = m_TabController.GetHighlightTab().gameObject;
        if (null != curTab && curTab.name == EnchanceTabStr)
        {
            return true;
        }
        return false;
    }

    public bool IsCurStarTab()
    {
        if (m_TabController.GetHighlightTab() == null)
        {
            return false;
        }
        GameObject curTab = m_TabController.GetHighlightTab().gameObject;
        if (null != curTab && curTab.name == StarTabStr)
        {
            return true;
        }
        return false;
    }

    //刷新强化分页
    public void UpdateTab_Enchance()
    {
        //清空
        ClearTab_Enchance();
        UpdateTab_Enchance_Equip();
        UpdateTab_Enchance_Material();
        UpdateTab_Enchance_EquipInfo();
    }


    public void ShowQiangHuaChengGongEffect(ulong guid) 
    {
        GameItemContainer EquipPack = GameManager.gameManager.PlayerDataPool.EquipPack;
        for (int index = 0; index < EquipPack.ContainerSize; index++) 
        {
            if (EquipPack.GetItem(index).Guid == guid) 
            {
              QiangHuaChengGongEffect.SetActive(false);
              int uiindex =  ItemTool.GetUIIndexByEquipSlot(index);
              QiangHuaChengGongEffect.transform.parent = m_Equip_Sprite[uiindex].gameObject.transform;
              QiangHuaChengGongEffect.transform.localPosition = Vector3.zero;
              QiangHuaChengGongEffect.transform.localRotation = Quaternion.identity;
              QiangHuaChengGongEffect.transform.localScale = Vector3.one * 2400;
              QiangHuaChengGongEffect.SetActive(true);
            }
        }

    }
    public void UpdateTab_Enchance_Equip(bool hideLevel =false)
    {
        GameItemContainer EquipPack = GameManager.gameManager.PlayerDataPool.EquipPack;
        for (int index = 0;
            index < EquipPack.ContainerSize &&
            index < m_Equip_Sprite.Length &&
            index < m_Equip_QualityFrame.Length &&
            index < m_Equip_Lable.Length; 
            index++)
        {
            int slot = ItemTool.GetEquipSlotByUIIndex(index);
            GameItem equip = EquipPack.GetItem(slot);
            if (equip != null && equip.IsValid())
            {
                if (m_Equip_Sprite[index] != null)
                {
                    m_Equip_Sprite[index].gameObject.SetActive(true);
                    m_Equip_Sprite[index].spriteName = equip.GetIcon();
                }
                if (m_Equip_QualityFrame[index] != null)
                {
                    m_Equip_QualityFrame[index].gameObject.SetActive(true);
                    m_Equip_QualityFrame[index].spriteName = equip.GetQualityFrame();
                }
                if (m_Equip_Lable[index] != null)
                {
                    if (equip.IsBelt()||hideLevel)
                    {
                        m_Equip_Lable[index].gameObject.SetActive(false);
                    }
                    else
                    {
                        m_Equip_Lable[index].gameObject.SetActive(true);
                        m_Equip_Lable[index].text = "+" + equip.EnchanceLevel;
                    }
                }
               
                if (GetCurSelectEquip() == null && equip.IsBelt() == false)
                {
                    SetCurSelectEquip(equip);
                }
            }
            else
            {
                if (m_Equip_Sprite[index] != null)
                {
                    m_Equip_Sprite[index].gameObject.SetActive(false);
                }
                if (m_Equip_QualityFrame[index] != null)
                {
                    m_Equip_QualityFrame[index].gameObject.SetActive(false);
                }
                if (m_Equip_Lable[index] != null)
                {
                    m_Equip_Lable[index].gameObject.SetActive(false);
                }
            }
        }
    }

    public void UpdateTab_Enchance_Material()
    {
        UIManager.LoadItem(UIInfo.EquipStrengthenItem, OnLoadItemEnchance_Material);
    }

    void OnLoadItemEnchance_Material(GameObject resItem, object param)
    {
        if (null == resItem)
        {
            LogModule.ErrorLog("load EquipStrengthenItem fail");
            return;
        }

        Utils.CleanGrid(m_Content_Enchance_Grid);

        //显示材料选择
        List<GameItem> backpackitem = GameManager.gameManager.PlayerDataPool.BackPack.GetList();
        List<GameItem> materiallist = ItemTool.EnchanceMaterialFilter(backpackitem, m_ValuableToggle.value);
        for (int i = 0; i < materiallist.Count; i++)
        {
            GameItem item = materiallist[i];
            GameObject itemobject = Utils.BindObjToParent(resItem, m_Content_Enchance_Grid, (i + 1000).ToString());
            if (itemobject != null && itemobject.GetComponent<EquipStrengthenItemLogic>() != null)
            {
                itemobject.GetComponent<EquipStrengthenItemLogic>().UpdateItemInfo(item, EquipStrengthenItemLogic.Type.TYPE_ENC_METARIAL_BACKPACK);
              //  if (item.IsEnchanceExpItem())
              //  {
               //
              //      itemobject.GetComponent<EquipStrengthenItemLogic>().OnClickChooseItem();
              //  } 
                if (m_FristMeterialEquip == null)
                {
                    m_FristMeterialEquip = itemobject;
                }
            }
        }
        Check_NewPlayerGuide();
        m_Content_Enchance_Grid.GetComponent<UIGrid>().repositionNow = true;
    }

    public void UpdateTab_Enchance_EquipInfo()
    {
        if (GetCurSelectEquip() == null)
        {
            m_Enchance_EquipName_Label.text = "";
            m_Enchance_NextEncLevel_Label.text = "";
            m_Enchance_EquipAttr.ClearInfo();
            m_Enchance_EncExp.value = 0;
            m_Enchance_EncExpValue.text = "0/0";
            m_Enchance_MaxLevelLabel.gameObject.SetActive(false);
            m_Enchance_NextInfo.gameObject.SetActive(false);
        }
        else
        {
            GameItem equipItem = GetCurSelectEquip();
            if (equipItem != null && equipItem.IsValid())
            {
                m_Enchance_EquipName_Label.text = equipItem.GetName();
                if (TableManager.GetEquipEnchanceByID(equipItem.EnchanceLevel + 1, 0) != null)
                {
                    m_Enchance_MaxLevelLabel.gameObject.SetActive(false);
                    m_Enchance_NextInfo.gameObject.SetActive(true);

                    m_Enchance_NextEncLevel_Label.text = "下一等级：" + (equipItem.EnchanceLevel + 1).ToString();
                    m_Enchance_EquipAttr.ShowAttr(equipItem);
                    int nEnchanceExpMax = equipItem.GetEnchanceExpMax();
                    if (nEnchanceExpMax > 0)
                    {
                        m_Enchance_EncExp.value = (float)(equipItem.EnchanceExp) / (float)nEnchanceExpMax;
                        m_Enchance_EncExpValue.text = equipItem.EnchanceExp.ToString() + "/" + nEnchanceExpMax.ToString();
                    }
                    else
                    {
                        m_Enchance_EncExp.value = 0;
                        m_Enchance_EncExpValue.text = "0/0";
                    }
                }
                else
                {
                    //已经最高等级
                    m_Enchance_MaxLevelLabel.gameObject.SetActive(true);
                    m_Enchance_NextInfo.gameObject.SetActive(false);
                }
            }
        }
    }
    public void UpdateTab_Gem() 
    {
        UpdateTab_Enchance_Equip(true);
        SetGem.SetActive(false);
        SetGemTips.SetActive(true);
        RemoveGem.SetActive(false);
        SelGemInfo.gameObject.SetActive(false);
        SetGemNumber.gameObject.SetActive(false);
        this.SelEquipGemIndex = -1;
        UpdateEquipGemInfo(); 

		if (m_NewPlayerGuideFlag_Step == 8)
		{
			NewPlayerGuide(9);
		}
        
    }
    public void OnClickEquipGem(GameObject go) 
    {
		if (9 == m_NewPlayerGuideFlag_Step) 
		{
			NewPlayerGuide(10);
		}

       int GemSlotIndex = int.Parse( go.name.Substring(go.name.Length-1, 1) );
       this.SelEquipGemIndex = GemSlotIndex;
      
       UpdateGemInfo();
    }

    public void UpdateGemInfo() 
    {
        SetSelEquipGem(this.SelEquipGemIndex);
        UpdateEquipGemInfo();
        int gemid = GemViewModel.GetGemId(this.m_CurSelectEquipIndex, this.SelEquipGemIndex);
      
        if (gemid < 0)
        {
            SetGem.SetActive(true);
            SetGemTips.SetActive(false);
            RemoveGem.SetActive(false);
            UpdateAllGems();
        }
        else
        {
            this.SelGemInfo.gameObject.SetActive(false);
            this.SetGemNumber.gameObject.SetActive(false);
            SetGem.SetActive(false);
            SetGemTips.SetActive(false);
            RemoveGem.SetActive(true);
            this.UpdateRemoveGemInfo();
        }
    }
   

    public void OnClickRemoveGem() 
    {
        CG_TAKE_GEM gemPacket = (CG_TAKE_GEM)PacketDistributed.CreatePacket(MessageID.PACKET_CG_TAKE_GEM);
        gemPacket.SetEquipslot((uint)this.m_CurSelectEquipIndex);
        gemPacket.SetIndex((uint)this.SelEquipGemIndex); 
        gemPacket.SendPacket();
    }

    public void UpdateRemoveGemInfo() 
    { 
        int GemId = GemViewModel.GetGemId(this.m_CurSelectEquipIndex,this.SelEquipGemIndex);
        Tab_CommonItem GemItem = TableManager.GetCommonItemByID(GemId)[0];
        this.RemoveGem.transform.Find("GemName").GetComponent<UILabel>().text = GemItem.Name;
        this.RemoveGem.transform.Find("GamValue").GetComponent<UILabel>().text =ItemTool.GetGemAttr(GemId);
        this.RemoveGem.transform.Find("GemItem/Icon").GetComponent<UISprite>().spriteName = GemItem.Icon;
    }
    public void OnClickAllGem(GameObject go) 
    {

        int selIndex =int.Parse( go.name.Substring(go.name.Length-1,1));
        int dataIndex =this.CurAllGemPageNumber*this.AllGemPageSize+selIndex;
        if (dataIndex >= this.m_GemItemList.Count)
        {
			NewPlayerGuide(4);
           // SetSelAllGem(-1);
            SelGemInfo.text = "";
            this.SetGemNumber.text = "";
            return;
		}
		if (10 == NewPlayerGuideFlag_Step) 
		{
			NewPlayerGuide(11);
		}
        SetSelAllGem(selIndex, this.m_GemItemList[dataIndex]);
        this.CurSelSetGem = this.m_GemItemList[dataIndex];
        ShowChooseGemInfo(this.CurSelSetGem);
        this.SelSetGemIndex = selIndex;
    }

    void ShowChooseGemInfo(GameItem item)
    {
        if (item.IsValid())
        {
            SelGemInfo.gameObject.SetActive(true);
            this.SetGemNumber.gameObject.SetActive(true);
            SelGemInfo.text = item.GetName();
            SelGemInfo.text += "   " +ItemTool.GetGemAttr(item.DataID);
            int mountNumber = TableManager.GetGemMountByID(this.SelEquipGemIndex+1)[0].ConsumeNum;
            this.SetGemNumber.text = StrDictionary.GetClientDictionaryString("#{2836}", mountNumber.ToString());
        }
        else
        {
            SelGemInfo.gameObject.SetActive(false);
            this.SetGemNumber.gameObject.SetActive(false);
        }
    }

  
    public void OnClickAllGemLeft() 
    {
        if (this.CurAllGemPageNumber>0) 
        {
            this.CurAllGemPageNumber--;
            UpdateAllGemPage();
        }
    }

    public void OnClickSetGem() 
    {
        if (this.CurSelSetGem == null||this.m_CurSelectEquipIndex==-1||this.CurSelSetGem.DataID==-1) 
        {
            GUIData.AddNotifyData("请先选择宝石和装备");
            return;
        }
		NewPlayerGuidLogic.CloseWindow();
        this.m_Delay_Gem_CurPackBack = this.m_CurSelectEquipIndex;
        this.m_Delay_Gem_GemIndex = this.SelEquipGemIndex;
        this.m_Delay_Gem_GemGuid = this.CurSelSetGem.Guid;
        BackCamerControll.Instance().PlaySceneEffect(133);
		GameManager.gameManager.SoundManager.PlaySoundEffect (28);
        m_Delay_Gem_Time = 1.5f;
    }
    

    public void OnClickAllGemRight() 
    {
        int pageSize = this.m_GemItemList.Count / this.AllGemPageSize;
        if (this.m_GemItemList.Count % this.AllGemPageSize != 0) 
        {
            pageSize++;
        }
        if (this.CurAllGemPageNumber < (pageSize-1))
        {
            this.CurAllGemPageNumber++;
            UpdateAllGemPage();
        }
    }

    void SetSelAllGem(int selIndex,GameItem item) 
    {
        for (int i = 0; i < this.AllGemPageSize;i++)
        {
            Transform ItemGo = this.AllGemsGrid.transform.Find("Gem"+i.ToString());
            if (selIndex == i)
            {
               
                ItemGo.Find("Sel").gameObject.SetActive(true);
            }
            else 
            {
                ItemGo.Find("Sel").gameObject.SetActive(false);
            }
        }
    }  
    void SetSelEquipGem(int selIndex) 
    {
        for (int i = 0; i < (int)GemSlot.OPEN_NUM;i++)
        {
            Transform ItemGo = this.EquipGemsGrid.transform.GetChild(i);
            if (selIndex == i)
            {
                ItemGo.Find("Sel").gameObject.SetActive(true);
            }
            else 
            {
                ItemGo.Find("Sel").gameObject.SetActive(false);
            }
        }
    }


    public void UpdateAllGems() 
    {
        GameItemContainer BackPack = GameManager.gameManager.PlayerDataPool.BackPack;
        m_GemItemList = ItemTool.ItemFilter(BackPack, (int)ItemClass.STRENGTHEN, (int)StrengthenSubClass.GEM);
        m_GemItemList = this.GemFilter(m_GemItemList);
        UpdateAllGemPage();
    }

    void UpdateAllGemPage() 
    {
        int startIndex = this.CurAllGemPageNumber * AllGemPageSize;
        int Gemi = 0;
        for (int i = startIndex; i < startIndex + AllGemPageSize; i++)
        {
            Transform ItemG = this.AllGemsGrid.transform.Find("Gem" + Gemi);
            ItemG.Find("Sel").gameObject.SetActive(false);
            if (i < m_GemItemList.Count)
            {
                GameItem item = m_GemItemList[i];
                ItemG.Find("Icon").gameObject.SetActive(true);
                UISprite IconSpr = ItemG.Find("Icon").GetComponent<UISprite>();
                IconSpr.spriteName = TableManager.GetCommonItemByID(item.DataID)[0].Icon;
				IconSpr.depth=10;
				ItemG.Find("Label").gameObject.SetActive(true);
                UILabel label = ItemG.Find("Label").gameObject.GetComponent<UILabel>();
               label.text = item.StackCount.ToString();
            }
            else
            {
                ItemG.Find("Label").gameObject.SetActive(false);
                ItemG.Find("Icon").gameObject.SetActive(false);
            }
            Gemi++;
        }
    }


    public void UpdateEquipGemInfo() 
    {
        int SlotIndex = this.m_CurSelectEquipIndex;
        SetSelEquipGem(this.SelEquipGemIndex);
        for (int i = 0; i < (int)GemSlot.OPEN_NUM; i++)
        {
            int gemid = -1;
            if (SlotIndex != -1)
            {
                gemid = GemViewModel.GetGemId(SlotIndex, i);
            }
            Transform GemItemGo = this.EquipGemsGrid.transform.GetChild(i);
            if (gemid == -1)
            {
                GemItemGo.Find("Icon").gameObject.SetActive(false);
            }
            else
            {
                UISprite IconSpr = GemItemGo.Find("Icon").GetComponent<UISprite>();
                IconSpr.gameObject.SetActive(true);
                IconSpr.spriteName =  TableManager.GetCommonItemByID(gemid)[0].Icon;
            }
        } 
    }
    List<GameItem> GemFilter(List<GameItem> gemList)
    {
        int slotIndex = this.m_CurSelectEquipIndex;
        List<GameItem> resultList = new List<GameItem>();
        for (int n = 0; n < gemList.Count; ++n)
        {
            Tab_GemAttr line = TableManager.GetGemAttrByID(gemList[n].DataID, 0);
            if (line != null)
            {
                //部位是否符合
                if (ItemTool.GetEquipSlotType(slotIndex) != line.BaseClass)
                {
                    continue;
                }
                //是否已有同类宝石
                for (int i = 0; i < (int)GemSlot.OPEN_NUM; i++)
                {
                    if (GemViewModel.GetGemId(slotIndex, i) >= 0)
                    {
                        Tab_GemAttr lineOther = TableManager.GetGemAttrByID(GemViewModel.GetGemId(slotIndex, i), 0);
                        if (lineOther == null)
                        {
                            continue;
                        }
                        if (lineOther.AttrClass == line.AttrClass)
                        {
                            continue;
                        }
                    }
                }
                //添加
                resultList.Add(gemList[n]);
            }
        }
        return resultList;
    }

    //刷新打星分页
    public void UpdateTab_Star()
    {
        //清空
        ClearTab_Star();
        UpdateTab_Star_Equip();
        UpdateTab_Star_EquipInfo();
        if (m_NewPlayerGuideFlag_Step == 6)
        {
            NewPlayerGuide(7);
        }
    }

    public void UpdateTab_Star_Equip()
    {
        //紫色以下不能打星
        if (GetCurSelectEquip() != null && GetCurSelectEquip().IsValid())
        {
            if (GetCurSelectEquip().GetQuality() < ItemQuality.QUALITY_PURPLE)
            {
                ClearCurSelectEquip();
            }
        }
        GameItemContainer EquipPack = GameManager.gameManager.PlayerDataPool.EquipPack;
        for (int index = 0; index < EquipPack.ContainerSize; index++)
        {
            int slot = ItemTool.GetEquipSlotByUIIndex(index);
            GameItem equip = EquipPack.GetItem(slot);
            if (equip != null && equip.IsValid())
            {
                if (m_Equip_Sprite[index] != null)
                {
                    m_Equip_Sprite[index].gameObject.SetActive(true);
                    m_Equip_Sprite[index].spriteName = equip.GetIcon();
                }
                if (m_Equip_QualityFrame[index] != null)
                {
                    m_Equip_QualityFrame[index].gameObject.SetActive(true);
                    m_Equip_QualityFrame[index].spriteName = equip.GetQualityFrame();
                }
                if (m_Equip_Lable[index] != null)
                {
                    if (equip.IsBelt())
                    {
                        m_Equip_Lable[index].gameObject.SetActive(false);
                    }
                    else
                    {
                        m_Equip_Lable[index].gameObject.SetActive(true);
                        int showLevel = 0;
                        if (equip.StarLevel >= 1)
                        {
                            showLevel = (equip.StarLevel - 1) % 12 + 1;
                        }
                        m_Equip_Lable[index].text = ItemTool.GetStarColor(equip.StarLevel);
                        m_Equip_Lable[index].text += "+" + showLevel;
                    }
                }
                if (GetCurSelectEquip() == null && equip.IsBelt() == false)
                {
                    //紫色以下不能打星
                    if (equip.GetQuality() >= ItemQuality.QUALITY_PURPLE)
                    {
                        SetCurSelectEquip(equip);
                    }
                }
            }
            else
            {
                if (m_Equip_Sprite[index] != null)
                {
                    m_Equip_Sprite[index].gameObject.SetActive(false);
                }
                if (m_Equip_QualityFrame[index] != null)
                {
                    m_Equip_QualityFrame[index].gameObject.SetActive(false);
                }
                if (m_Equip_Lable[index] != null)
                {
                    m_Equip_Lable[index].gameObject.SetActive(false);
                }
            }
        }
    }

    public void UpdateTab_Star_EquipInfo()
    {
        OnClickStarMode_Normal();
        if (GetCurSelectEquip() == null)
        {
            ShowStarLevel(0);
            m_Star_Choose_Name.text = "";
            m_Star_Choose_Attr.ClearInfo();
            m_Star_Material_Sprite.spriteName = "";
            m_Star_Material_Label.text = "";
            m_Star_Material_QualitySprite.spriteName = "";
            m_Star_NeedCoin_Label.text = "";
            m_Star_StarLevelMaxNotice.gameObject.SetActive(false);
            m_Star_ConsumInfo.gameObject.SetActive(false);
            m_Star_NextLevelInfo.gameObject.SetActive(false);
            m_Star_CurMaterial_Label.text = GameManager.gameManager.PlayerDataPool.BackPack.GetItemCountByDataId(GameDefine_Globe.STAR_COMMONITEM_ID).ToString();
            m_Star_CurMoney_Label.text = Utils.ConvertLargeNumToString(GameManager.gameManager.PlayerDataPool.Money.GetMoney_Coin());
            m_Star_SuccessRate_Label.text = "";
            m_CurStarMaterialID = -1;
        }
        else
        {
            GameItem curEquip = GetCurSelectEquip();
            if (curEquip != null && curEquip.IsValid())
            {
                ShowStarLevel(curEquip.StarLevel);
                m_Star_Choose_Name.text = curEquip.GetName();
                m_Star_Choose_Attr.ShowAttr(curEquip);

                Tab_EquipStar starline = TableManager.GetEquipStarByID(curEquip.StarLevel + 1, 0);
                if (starline != null)
                {
                    m_Star_StarLevelMaxNotice.gameObject.SetActive(false);
                    m_Star_ConsumInfo.gameObject.SetActive(true);
                    m_Star_NextLevelInfo.gameObject.SetActive(true);
                    Tab_CommonItem materialine = TableManager.GetCommonItemByID(starline.ConsumeSubType, 0);
                    if (materialine != null)
                    {
                        m_Star_Material_Sprite.spriteName = materialine.Icon;
                        m_Star_Material_Label.text = starline.ConsumeNum.ToString();
                        m_Star_Material_QualitySprite.spriteName = GlobeVar.QualityColorGrid[materialine.Quality - 1];
                        m_Star_NeedCoin_Label.text = starline.NeedCoin.ToString();
                    }
                    m_Star_CurMaterial_Label.text = GameManager.gameManager.PlayerDataPool.BackPack.GetItemCountByDataId(starline.ConsumeSubType).ToString();
                    m_Star_CurMoney_Label.text = Utils.ConvertLargeNumToString(GameManager.gameManager.PlayerDataPool.Money.GetMoney_Coin());
                    m_Star_SuccessRate_Label.text = string.Format("{0}%", (int)(curEquip.GetStarShowRate() * 100));
                    m_CurStarMaterialID = starline.ConsumeSubType;
                }
                else
                {
                    //星级已满
                    m_Star_StarLevelMaxNotice.gameObject.SetActive(true);
                    m_Star_ConsumInfo.gameObject.SetActive(false);
                    m_Star_NextLevelInfo.gameObject.SetActive(false);
                    m_CurStarMaterialID = -1;
                }
            }
        }
    }

    //清空强化分页
    public void ClearTab_Enchance()
    {
        Utils.CleanGrid(m_Content_Enchance_Grid);

        m_Enchance_EquipName_Label.text = "";
        m_Enchance_NextEncLevel_Label.text = "";
        m_Enchance_EquipAttr.ClearInfo();
        m_Enchance_EncExpValue.text = "";
        m_Enchance_NextInfo.gameObject.SetActive(false);
        m_Enchance_MaxLevelLabel.gameObject.SetActive(false);
    }

    //清空打星分页
    public void ClearTab_Star()
    {
        //打星预览信息
        ShowStarLevel(0);
        m_Star_Choose_Attr.ClearInfo();
        m_Star_Material_Sprite.spriteName = "";
        m_Star_Material_Label.text = "";
        m_Star_Material_QualitySprite.spriteName = "";
        m_Star_NeedCoin_Label.text = "";
        m_Star_SuccessRate_Label.text = "";
        m_Star_StarLevelMaxNotice.gameObject.SetActive(false);
        for (int i = 0; i < 3 && i < m_Star_Mode_Choose_Sprite.Length; i++)
        {
            m_Star_Mode_Choose_Sprite[i].gameObject.SetActive(false);
        }
    }

    public void OnClickEquip(int index)
    {
     	if (8 == m_NewPlayerGuideFlag_Step) 
		{
			NewPlayerGuide(9);	
		}
        int slot = ItemTool.GetEquipSlotByUIIndex(index);
        GameItem equip = GameManager.gameManager.PlayerDataPool.EquipPack.GetItem(slot);

        if (!IsCurEnchanceTab() && !IsCurStarTab()) 
        {
            SetCurSelectEquip(equip, index);
              UpdateTab();
              return;
        }
        if (equip != null && equip.IsValid())
        {
            if (IsCurEnchanceTab())
            {
                if (equip.IsBelt())
                {
                    Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2634}");
                }
                else
                {
                    SetCurSelectEquip(equip);
                    UpdateTab();
                }
            }
            else if (IsCurStarTab())
            {
                if (equip.IsBelt())
                {
                    Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2634}");
                }
                else if (equip.GetQuality() < ItemQuality.QUALITY_PURPLE)
                {
                    Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2640}");
                }
                else
                {
                    SetCurSelectEquip(equip);
                    UpdateTab();
                }
            }
        }
    }

    void OnClickEquip0()
    {
        OnClickEquip(0);
    }
    
    void OnClickEquip1()
    {
        OnClickEquip(1);
    }
    
    void OnClickEquip2()
    {
        OnClickEquip(2);
    }
    
    void OnClickEquip3()
    {
        OnClickEquip(3);
    }
    
    void OnClickEquip4()
    {
        OnClickEquip(4);
    }
    
    void OnClickEquip5()
    {
        OnClickEquip(5);
    }
    
    void OnClickEquip6()
    {
        OnClickEquip(6);
    }

    void OnClickEquip7()
    {
        OnClickEquip(7);
    }

    void OnClickEquip8()
    {
        OnClickEquip(8);
    }

    void OnClickEquip9()
    {
        OnClickEquip(9);
    }

    //装备强化 点击选择材料之后
    public void ItemAfterChoose_Enchance_Metarial(GameItem clickitem)
    {
        int totalexp = CalcTotalExpAfterEnchance();
        GameItem equipItem = GetCurSelectEquip();
        if (equipItem != null)
        {
            if (equipItem.IsValid() && totalexp > 0)
            {
                int nEnchanceExpMax = equipItem.GetEnchanceExpMax();
                m_Enchance_EncExpValue.text = equipItem.EnchanceExp.ToString() + "[00A0FF]+" + totalexp.ToString() + "[FFFF69]/" + nEnchanceExpMax.ToString();
            }
            if (m_NewPlayerGuideFlag_Step == 0)
            {
                NewPlayerGuide(2);
            }
        }
    }

    //装备强化 取消选择材料之后
    public void ItemAfterCancel_Enchance_Metarial(GameItem clickitem)
    {
        int totalexp = CalcTotalExpAfterEnchance();
        GameItem equipItem = GetCurSelectEquip();
        if (equipItem != null)
        {
            if (equipItem.IsValid())
            {
                int nEnchanceExpMax = equipItem.GetEnchanceExpMax();
                if (totalexp > 0)
                {
                    m_Enchance_EncExpValue.text = equipItem.EnchanceExp.ToString() + "[00A0FF]+" + totalexp.ToString() + "[FFFF69]/" + nEnchanceExpMax.ToString();
                }
                else
                {
                    m_Enchance_EncExpValue.text = equipItem.EnchanceExp.ToString() + "/" + nEnchanceExpMax.ToString();
                }
            }
        }
    }

    void OnCloseButtonClick()
    {
       
        ClearCurSelectEquip();
        UIManager.CloseUI(UIInfo.EquipStren);
		if (NewPlayerGuideFlag_Step > 0) 
		{
			NewPlayerGuide(4);
		}
    }

    string GetEnchanceColorType(string strId) 
    {
        string DicValue = StrDictionary.GetClientDictionaryString(strId);
        string DicType = DicValue.IndexOf(']') > 0 ? DicValue.Split(']')[1] : DicValue;
        return DicType;
    }
    //强化 智能选择 弹出列表选择
    public void OnEnchanceChoose()
    {
        if (IsFirstShow) 
        {
            IsFirstShow = false;
            return;
        }
        if (m_Enchance_ChooseList != null)
        {
            UIPopupList list = m_Enchance_ChooseList.GetComponent<UIPopupList>();
            if (list == null)
            {
                return;
            }
            string SelType  =list.value.IndexOf(']')>0? list.value.Split(']')[1]:list.value;
            if (SelType == GetEnchanceColorType("#{1208}"))
            {
                EquipStrengthenItemLogic[] ItemArry = m_Content_Enchance_Grid.GetComponentsInChildren<EquipStrengthenItemLogic>();
                for (int i = 0; i < ItemArry.Length; i++)
                {
                    EquipStrengthenItemLogic item = ItemArry[i];
                    //选中白色
                    if (item.m_item.GetQuality() == ItemQuality.QUALITY_WHITE)
                    {
                        if (item.m_Status == EquipStrengthenItemLogic.Status.STATUS_ENABLE)
                        {
                            item.OnClickChooseItem();
                        }
                    }
                    else
                    {
                        if (item.m_Status == EquipStrengthenItemLogic.Status.STATUS_CHOOSED)
                        {
                            item.OnClickChooseItem();
                        }
                    }
                }
            }
            else if (SelType == GetEnchanceColorType("#{1209}"))
            {
                EquipStrengthenItemLogic[] ItemArry = m_Content_Enchance_Grid.GetComponentsInChildren<EquipStrengthenItemLogic>();
                for (int i = 0; i < ItemArry.Length; i++)
                {
                    EquipStrengthenItemLogic item = ItemArry[i];
                    //选中绿色 白色
                    if (item.m_item.GetQuality() == ItemQuality.QUALITY_GREEN ||
                        item.m_item.GetQuality() == ItemQuality.QUALITY_WHITE)
                    {
                        if (item.m_Status == EquipStrengthenItemLogic.Status.STATUS_ENABLE)
                        {
                            item.OnClickChooseItem();
                        }
                    }
                    else
                    {
                        if (item.m_Status == EquipStrengthenItemLogic.Status.STATUS_CHOOSED)
                        {
                            item.OnClickChooseItem();
                        }
                    }
                }
            }
            else if (SelType == GetEnchanceColorType("#{1210}"))
            {
                EquipStrengthenItemLogic[] ItemArry = m_Content_Enchance_Grid.GetComponentsInChildren<EquipStrengthenItemLogic>();
                for (int i = 0; i < ItemArry.Length; i++)
                {
                    EquipStrengthenItemLogic item = ItemArry[i];
                    //选中蓝色 绿色 白色
                    if (item.m_item.GetQuality() == ItemQuality.QUALITY_BLUE ||
                        item.m_item.GetQuality() == ItemQuality.QUALITY_GREEN ||
                        item.m_item.GetQuality() == ItemQuality.QUALITY_WHITE)
                    {
                        if (item.m_Status == EquipStrengthenItemLogic.Status.STATUS_ENABLE)
                        {
                            item.OnClickChooseItem();
                        }
                    }
                    else
                    {
                        if (item.m_Status == EquipStrengthenItemLogic.Status.STATUS_CHOOSED)
                        {
                            item.OnClickChooseItem();
                        }
                    }
                }
            }
            else if (SelType == GetEnchanceColorType("#{1211}"))
            {
                EquipStrengthenItemLogic[] ItemArry = m_Content_Enchance_Grid.GetComponentsInChildren<EquipStrengthenItemLogic>();
                for (int i = 0; i < ItemArry.Length; i++)
                {
                    EquipStrengthenItemLogic item = ItemArry[i];
                    //选中紫色 蓝色 绿色 白色
                    if (item.m_item.GetQuality() == ItemQuality.QUALITY_PURPLE ||
                        item.m_item.GetQuality() == ItemQuality.QUALITY_BLUE ||
                        item.m_item.GetQuality() == ItemQuality.QUALITY_GREEN ||
                        item.m_item.GetQuality() == ItemQuality.QUALITY_WHITE)
                    {
                        if (item.m_Status == EquipStrengthenItemLogic.Status.STATUS_ENABLE)
                        {
                            item.OnClickChooseItem();
                        }
                    }
                    else
                    {
                        if (item.m_Status == EquipStrengthenItemLogic.Status.STATUS_CHOOSED)
                        {
                            item.OnClickChooseItem();
                        }
                    }
                }
            }
            else if (SelType == GetEnchanceColorType("#{1212}"))
            {
                EquipStrengthenItemLogic[] ItemArry = m_Content_Enchance_Grid.GetComponentsInChildren<EquipStrengthenItemLogic>();
                for (int i = 0; i < ItemArry.Length; i++)
                {
                    EquipStrengthenItemLogic item = ItemArry[i];
                    if (item.m_Status == EquipStrengthenItemLogic.Status.STATUS_ENABLE)
                    {
                        item.OnClickChooseItem();
                    }
                }
            }
            else
            {
                //清空选择
                EquipStrengthenItemLogic[] ItemArry = m_Content_Enchance_Grid.GetComponentsInChildren<EquipStrengthenItemLogic>();
                for (int i = 0; i < ItemArry.Length; i++)
                {
                    EquipStrengthenItemLogic item = ItemArry[i];
                    if (item.m_Status == EquipStrengthenItemLogic.Status.STATUS_CHOOSED)
                    {
                        item.OnClickChooseItem();
                    }
                }
            }
        }
    }


    //点击强化按钮
    void OnEnchanceButtonClick()
    {
       
        // 新手指引
        if (m_NewPlayerGuideFlag_Step == 2)
        {
            NewPlayerGuide(5);
        }

        if (m_Delay_Enchance_Time > 0)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2215}");
            return;
        }

        if (GetCurSelectEquip() == null)
        {
            //未选择装备
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1224}");
            return;
        }
        if (GetCurSelectEquip().IsValid() == false)
        {
            //未选择装备
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1224}");
            return;
        }

        ulong equipguid = GetCurSelectEquip().Guid;
        //TT1703 蓝色及以下装备吸收有星级装备时 需要提示
        int noticeStep = 0;
        if (GetCurSelectEquip().GetQuality() <= ItemQuality.QUALITY_BLUE)
        {
            noticeStep = 1;
        }
        List<ulong> metariallist = new List<ulong>();
        EquipStrengthenItemLogic[] ItemArry = m_Content_Enchance_Grid.GetComponentsInChildren<EquipStrengthenItemLogic>();
        for (int i = 0; i < ItemArry.Length; i++)
        {
            EquipStrengthenItemLogic item = ItemArry[i];
            if (item.m_Status == EquipStrengthenItemLogic.Status.STATUS_CHOOSED)
            {
                metariallist.Add(item.m_item.Guid);
                //TT1703 蓝色及以下装备吸收有星级装备时 需要提示
                if (noticeStep == 1)
                {
                    if (item.m_item.StarLevel > 0)
                    {
                        noticeStep = 2;
                    }
                }
            }
        }
        if (equipguid == GlobeVar.INVALID_GUID)
        {
            //TODO 未选择装备
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1224}");
            return;
        }
        if (metariallist.Count == 0)
        {
            //TODO 未选择材料
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1225}");
            return;
        }

        if (GetCurSelectEquip().EnchanceLevel >= GlobeVar.EQUIP_ENCHANCE_MAX_LEVEL)
        {
            //装备强化等级已满
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1273}");
            return;
        }

        m_Delay_Enchance_CurPackBack = (int)GameItemContainer.Type.TYPE_EQUIPPACK;
        m_Delay_Enchance_EquipGuid = equipguid;
        m_Delay_Enchance_MetarialList.Clear();
        m_Delay_Enchance_MetarialList.AddRange(metariallist);

        //TT1703 蓝色及以下装备吸收有星级装备时 需要提示
        if (noticeStep == 2)
        {
            MessageBoxLogic.OpenOKCancelBox(2514, 1000, EnchanceEquipOk, EnchanceEquipCance);
        }
        else
        {
            EnchanceEquipOk();
        }
    }

    void EnchanceEquipOk()
    {
        if (null != GameManager.gameManager)
        {
            GameManager.gameManager.SoundManager.PlaySoundEffect(28);
        }

        //播放特效 延迟发包
        if (BackCamerControll.Instance() != null)
        {
            BackCamerControll.Instance().StopSceneEffect(130, false);
            BackCamerControll.Instance().PlaySceneEffect(130);
        }
        m_Delay_Enchance_Time = 2.3f;
    }

    void EnchanceEquipCance()
    {
        m_Delay_Enchance_CurPackBack = 0;
        m_Delay_Enchance_EquipGuid = 0;
        m_Delay_Enchance_MetarialList.Clear();
    }

    //选择普通打星模式
    void OnClickStarMode_Normal()
    {
        if (m_Delay_Star_Time > 0)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2215}");
            return;
        }
        m_Star_Mode_Choose_Sprite[(int)StarMode.STAR_NORMAL].gameObject.SetActive(true);
        m_Star_Mode_Choose_Sprite[(int)StarMode.STAR_CUR12].gameObject.SetActive(false);
        m_Star_Mode_Choose_Sprite[(int)StarMode.STAR_MAX12].gameObject.SetActive(false);
        m_Star_Mode = (int)StarMode.STAR_NORMAL;
    }

    //选择打星至当前12星
    void OnClickStarMode_Cur12()
    {
        if (m_Delay_Star_Time > 0)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2215}");
            return;
        }
        m_Star_Mode_Choose_Sprite[(int)StarMode.STAR_NORMAL].gameObject.SetActive(false);
        m_Star_Mode_Choose_Sprite[(int)StarMode.STAR_CUR12].gameObject.SetActive(true);
        m_Star_Mode_Choose_Sprite[(int)StarMode.STAR_MAX12].gameObject.SetActive(false);
        m_Star_Mode = (int)StarMode.STAR_CUR12;
    }

    //选择打星至橙色12星
    void OnClickStarMode_Max12()
    {
        if (m_Delay_Star_Time > 0)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2215}");
            return;
        }
        m_Star_Mode_Choose_Sprite[(int)StarMode.STAR_NORMAL].gameObject.SetActive(false);
        m_Star_Mode_Choose_Sprite[(int)StarMode.STAR_CUR12].gameObject.SetActive(false);
        m_Star_Mode_Choose_Sprite[(int)StarMode.STAR_MAX12].gameObject.SetActive(true);
        m_Star_Mode = (int)StarMode.STAR_MAX12;
    }

    //点击打星按钮
    void OnStarButtonClick()
    {
        if (m_Delay_Star_Time > 0)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2215}");
            return;
        }

        GameItem curEquip = GetCurSelectEquip();
        if (curEquip == null)
        {
            //未选择装备
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1224}");
            return;
        }
        if (curEquip.IsValid() == false)
        {
            //未选择装备
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1224}");
            return;
        }

        if (curEquip.StarLevel >= curEquip.GetMaxStarLevel())
        {
            //装备打星星级已满
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1274}");
            return;
        }

        int starTargetLevel = 0;
        if (m_Star_Mode == (int)StarMode.STAR_NORMAL)
        {
            starTargetLevel = 0;
        }
        else if (m_Star_Mode == (int)StarMode.STAR_CUR12)
        {
            starTargetLevel = ((curEquip.StarLevel / 12) + 1) * 12;
        }
        else if (m_Star_Mode == (int)StarMode.STAR_MAX12)
        {
            starTargetLevel = curEquip.GetMaxStarLevel();
        }

        Tab_EquipStar line = TableManager.GetEquipStarByID(curEquip.StarLevel + 1, 0);
        if (line != null)
        {
            int ItemNum = GameManager.gameManager.PlayerDataPool.BackPack.GetItemCountByDataId(line.ConsumeSubType);
            if (ItemNum < line.ConsumeNum)
            {
                //打星石数量不足
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1275}");
                return;
            }

            //金币数量
            int CoinNum = GameManager.gameManager.PlayerDataPool.Money.GetMoney_Coin();
            if (CoinNum < line.NeedCoin)
            {
                //金币数量不足
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1830}");
                return;
            }
        }

        if (null != GameManager.gameManager)
        {
            GameManager.gameManager.SoundManager.PlaySoundEffect(28);
        }

        //播放特效 延迟发包
        if (BackCamerControll.Instance() != null)
        {
            BackCamerControll.Instance().StopSceneEffect(131, false);
            BackCamerControll.Instance().PlaySceneEffect(131);
        }
        m_Delay_Star_Time = 2.3f;
        m_Delay_Star_CurPackBack = (int)GameItemContainer.Type.TYPE_EQUIPPACK;
        m_Delay_Star_EquipGuid = curEquip.Guid;
        m_Delay_Star_TargetLevel = starTargetLevel;
    }

    void OnClickStarMaterial()
    {
        if (m_CurStarMaterialID >= 0)
        {
            GameItem item = new GameItem();
            item.DataID = m_CurStarMaterialID;
            ItemTooltipsLogic.ShowItemTooltip(item, ItemTooltipsLogic.ShowType.Info);
        }
    }

    public void OnChangeValuableToggle()
    {
        // 新手指引相关
        if (m_IsNotFirstOpenFlag == false)
        {
            m_IsNotFirstOpenFlag = true;
            return;
        }

        //刷新下强化材料
        UpdateTab_Enchance_Material();
    }

    //更新可增加经验值
    int CalcTotalExpAfterEnchance()
    {
        GameItem equip = GetCurSelectEquip();
        if (equip == null)
	    {
            LogModule.DebugLog(StrDictionary.GetClientDictionaryString("#{1224}"));
            return 0;
	    }
        if (equip.IsValid() == false)
        {
            LogModule.DebugLog(StrDictionary.GetClientDictionaryString("#{1224}"));
            return 0;
        }

        //计算当前的总经验 （原有经验+ 材料经验）
        int totalexp = 0;
        EquipStrengthenItemLogic[] ItemArry = m_Content_Enchance_Grid.GetComponentsInChildren<EquipStrengthenItemLogic>();
        for (int i = 0; i < ItemArry.Length; i++)
        {
            EquipStrengthenItemLogic item = ItemArry[i];
            if (item.m_Status == EquipStrengthenItemLogic.Status.STATUS_CHOOSED)
            {
                totalexp += item.m_item.GetFullEnchanceExp();
            }
        }

        return totalexp;
    }

    private GameItem GetCurSelectEquip()
    {
        return m_CurSelectEquip;
    }
    public void SetCurSelectEquip(GameItem item,int uiIndex = -1)
    {
       
        ClearCurSelectEquip();
        m_CurSelectEquip = item;
      
        int index = uiIndex;
        if (index == -1)
        {
            index = ItemTool.GetUIIndexByEquipSlot(item.GetSubClass() - 1);
        }
          this.m_CurSelectEquipIndex =ItemTool.GetEquipSlotByUIIndex(index);
        if (m_Equip_Choose[index] != null)
        {
            m_Equip_Choose[index].gameObject.SetActive(true);
        }
    }
    private void ClearCurSelectEquip()
    {
        m_CurSelectEquip = null;
        for (int i = 0; i < EquipNum; i++)
        {
            m_Equip_Choose[i].gameObject.SetActive(false);
        }
    }

    //显示选择装备的打星等级
    private void ShowStarLevel(int level)
    {
        int starnum = 0;
        int starcolour = 0;
        if (level > 0)
        {
            starnum = ((level-1) % 12) + 1;
            starcolour = (int)((level-1) / 12);
        }
        for (int i = 0; i < m_Star_StarLevelSprite.Length; i++)
        {
            if (i < starnum)
            {
                m_Star_StarLevelSprite[i].spriteName = ItemTool.GetStarColourSprite(starcolour);
                m_Star_StarLevelSprite[i].gameObject.SetActive(true);
            }
            else 
            {
                if (starcolour >= 1)
                {
                    m_Star_StarLevelSprite[i].spriteName = ItemTool.GetStarColourSprite(starcolour - 1);
                    m_Star_StarLevelSprite[i].gameObject.SetActive(true);
                }
                else
                {
                    m_Star_StarLevelSprite[i].gameObject.SetActive(false);
                }
                
            }
        }
    }

    //滚动到第一个被选中的格子
    public void ScrollToFirstChooseItem(GameObject gridObject)
    {
        if (gridObject == null)
        {
            return;
        }
        GameObject draggablePanel = gridObject.transform.parent.gameObject;
        if (draggablePanel == null)
        {
            return;
        }
        float itemHeight = gridObject.GetComponent<UIGrid>().cellHeight;
        //计算当前格子位置
        int curPos = (int)(draggablePanel.transform.localPosition.y / itemHeight) + 1;
        //计算目标格子位置(第一个被选中的格子)
        int targetPos = 0;
        int count = 0;
        EquipStrengthenItemLogic[] ItemArry = gridObject.GetComponentsInChildren<EquipStrengthenItemLogic>();
        for (int i = 0; i < ItemArry.Length; i++)
        {
            EquipStrengthenItemLogic item = ItemArry[i];
            count++;
            //记录第一个选中格子
            if (item.m_Status == EquipStrengthenItemLogic.Status.STATUS_CHOOSED && targetPos == 0)
            {
                targetPos = count;
            }
        }
        //滚动
        if (targetPos > 0 && count > 3)
        {
            //如果目标格子在最后四个 那么只滚动到倒数第四个(同时只能显示四个格子)
            if (targetPos >= (count-3))
            {
                targetPos = count - 3;
            }
            float moveOffest = (targetPos - curPos) * itemHeight;
            Vector3 target = new Vector3(0, moveOffest, 0);
            draggablePanel.GetComponent<UIDraggablePanel>().MoveRelative(target);
        }
    }

    void Check_NewPlayerGuide()
    {
        if (MenuBarLogic.Instance() == null)
        {
            return;
        }
        int nIndex = MenuBarLogic.Instance().NewPlayerGuideIndex;
		if (nIndex == (int)GameDefine_Globe.NEWOLAYERGUIDE.EQUIP_INTENSIFY)
        {
            NewPlayerGuide(0);
        }
		else if (nIndex == (int)GameDefine_Globe.NEWOLAYERGUIDE.EQUIP_START)
        {
            NewPlayerGuide(6);
        }
		else if (nIndex == (int)GameDefine_Globe.NEWOLAYERGUIDE.EQUIP_GEM)
		{
			NewPlayerGuide(8);
		}
		MenuBarLogic.Instance().NewPlayerGuideIndex = -1;
    }

    public void NewPlayerGuide(int nIndex)
    {
        if (nIndex < 0)
        {
            return;
        }
		
		NewPlayerGuidLogic.CloseWindow();

        m_NewPlayerGuideFlag_Step = nIndex;

        switch (nIndex)
        {
	    case 0:
	        if (m_FristMeterialEquip != null)
	        {
				NewPlayerGuidLogic.OpenWindow(m_FristMeterialEquip, 114, 114, "", "right", 2, true, true);
	        }
	        break;
	    case 1:
	        break;
	    case 2:
	        {
	            NewPlayerGuidLogic.OpenWindow(m_Enchance_AbsorbButton, 202, 64, "", "right", 2, true, true);
	        }
	        break;
	    case 3:
	        {
	            NewPlayerGuidLogic.OpenWindow(m_CloseButton, 78, 78, "", "right", 2, true, true);
	        }
	        break;
	    case 4:
	        m_NewPlayerGuideFlag_Step = -1;
	        break;
	    case 5:
	        break;
	    case 6:
	        if (m_TabBtnStar)
	        {
	            NewPlayerGuidLogic.OpenWindow(m_TabBtnStar, 154, 64, "", "right", 2, true, true);
	        }
	        break;
	    case 7:
	        if (m_BtnMakeStar)
	        {
	            NewPlayerGuidLogic.OpenWindow(m_BtnMakeStar, 202, 64, "", "right", 2, true, true);
	        }
	        break;
		case 8:
			if (m_TabBtnGem)
			{

				NewPlayerGuidLogic.OpenWindow(m_TabBtnGem, 154, 64, "", "right", 2, true, true);
			}
			break;
		case 9:
			if (m_GemHole0)
			{
				//首个位置没有装备的时候出新手指引没问题。
				OnClickEquip(0);
				NewPlayerGuidLogic.OpenWindow(m_GemHole0, 114, 114, "", "right", 2, true, true);
			}
			break;
		case 10:
			if (m_GemIcom0)
			{
				NewPlayerGuidLogic.OpenWindow(m_GemIcom0, 114, 114, "", "right", 2, true, true);
			}
			break;
		case 11:
			if (m_SetGemBtn)
			{
				NewPlayerGuidLogic.OpenWindow(m_SetGemBtn, 202, 64, "", "right", 2, true, true);
			}
			break;
		}
    }
}