/********************************************************************
	文件名: 	ConsignSaleLogic.cs
	创建时间:	2014/06/12 13:10
	全路径:	\TLOL\Version\Main\Project\Client\Assets\MLDJ\Script\GUI
	创建人:		luoy
	功能说明:	寄售行界面逻辑
	修改记录:
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Games.ConsignSale;
using Games.Item;
using Games.LogicObj;
using GCGame;
using GCGame.Table;
using Module.Log;
using UnityEngine;
using System.Collections;

public class ConsignSaleLogic : MonoBehaviour
{
   public GameObject m_Menu;//购买菜单
   public GameObject m_QualityMenu;//品质菜单
   public GameObject m_SaleBag;//出售背包
   public GameObject m_buyRoot;//购买分页
   public GameObject m_buyGird;
   public UIGrid m_buyUIGird;
   public UITopGrid m_buyUITopGird;
   public GameObject m_SaleRoot;//出售分页
   public GameObject m_SaleGird;
   public UIGrid m_SaleUIGird;//出售物品显示节点
   public UITopGrid m_SaleUITopGird;
   public GameObject m_searchGird;
   public UITable m_searchUITable;
   public GameObject m_SubWuQiGrid;
   public UIGrid m_SubWuQiUIGrid;
   public GameObject m_SubFangJuGrid;
   public UIGrid m_SubFangJuUIGrid;
   public GameObject m_SubShiPinGrid;
   public UIGrid m_SubShiPinUIGrid;
   public GameObject m_SubYaoPinGird;
   public UIGrid m_SubYaoPinUIGrid;
   public GameObject m_SubCaiLiaoGird;
   public UIGrid m_SubCaiLiaoUIGrid;
   public GameObject m_LeftInfoRoot;
   public UIInput m_KeyWordInput;

   public GameObject m_SaleUpPage;
   public GameObject m_SaleDownPage;
   public GameObject m_BuyUpPage;
   public GameObject m_BuyDownPage;
   public GameObject m_NotFindBuyLable;
   public GameObject m_NotFindSaleLable;
   public GameObject m_BuyLableHight;
   public GameObject m_SaleLableHight;

   private ConsignsaleBuyItem[] m_BuyItem = new ConsignsaleBuyItem[(int)ConsignSaleData.MAXCOUNTSANDTOCLIENT];
   private ConsignSaleMyItem[] m_SaleItem = new ConsignSaleMyItem[(int)ConsignSaleData.MAXCOUNTSANDTOCLIENT];


   private ConsignsaleBuyItem m_CurClickBuyItem =null;
   public ConsignsaleBuyItem CurClickBuyItem
   {
       get { return m_CurClickBuyItem; }
       set { m_CurClickBuyItem = value; }
   }
	
	private ConsignsaleBuyItem m_LastClickBuyItem =null;
	public ConsignsaleBuyItem LastClickBuyItem
	{
		get { return m_LastClickBuyItem; }
		set { m_LastClickBuyItem = value; }
	}
   public UILabel m_moneyLable;
   public UILabel m_SaleCountLable;

   public UISprite[] m_SortSprite =new UISprite[(int)ConsignSale_SortClass.MAXNUM];

   private  List<ConsignSaleSearchInfo> m_buyItemInfo =new List<ConsignSaleSearchInfo>();
   public List<ConsignSaleSearchInfo> BuyItemInfo //购买界面物品信息
   {
       get { return m_buyItemInfo; }
       set { m_buyItemInfo = value; }
   }
   private List<MyConsignSaleItemInfo> m_SaleItemInfo = new List<MyConsignSaleItemInfo>();
   public List<MyConsignSaleItemInfo> SaleItemInfo//出售界面物品信息
   {
       get { return m_SaleItemInfo;}
       set { m_SaleItemInfo = value;}
   }
   private int m_nSearchClass =(int)ConsignSale_SearchClass.INVAILD_TYPE; //查询分类
   public int SearchClass
   {
       get { return m_nSearchClass; }
       set { m_nSearchClass = value; }
   }
   private int m_nSearchSubClass = (int) ConsignSale_SearchSubClass.INVAILD_TYPE; //查询子分类
   public int SearchSubClass
   {
       get { return m_nSearchSubClass; }
       set { m_nSearchSubClass = value; }
   }
   private int m_nSearchQuality =(int)ItemQuality.QUALITY_INVALID; //查询品质分类
   public int SearchQuality
   {
       get { return m_nSearchQuality; }
       set { m_nSearchQuality = value; }
   }
   private int m_nSortClass = (int) ConsignSale_SortClass.QUALITY;//查询排序分类
   private int m_nSortType = (int) ConsignSale_SortType.DOWN; //查询排序方式(顺序 倒序)
   private string m_keyWord="";
   public  int m_nCurBuyPage =0; //当前购买查看页
   public int CurBuyPage 
   {
       get { return m_nCurBuyPage; }
       set { m_nCurBuyPage = value; }
   }
   private int m_nCurSalePage = 0; //当前出售查看页
   public int CurSalePage
   {
       get { return m_nCurSalePage; }
       set { m_nCurSalePage = value; }
   }
   private bool m_bSaleFirstClick = true;
   private bool m_bBuyFirstClick = true;

   private bool m_bSaleItemLoad = false;
   private bool m_bBuyItemLoad = false;

   private int m_nSelBuyIndex = -1; //购买界面当前选中项
   public int SelBuyIndex
   {
       get { return m_nSelBuyIndex; }
       set { m_nSelBuyIndex = value; }
   }
   private int m_nCurSaleCount = 0;

   private int m_nSelSaleIndex = -1; //出售界面当前选中项
   public int SelSaleIndex
   {
       get { return m_nSelSaleIndex; }
       set { m_nSelSaleIndex = value; }
   }
   public int CurSaleCount //当前上架数
   {
       get { return m_nCurSaleCount; }
       set { m_nCurSaleCount = value; }
   }
   private int m_nMaxSaleCount = 0; //最大上架数
   public int MaxSaleCount
   {
       get { return m_nMaxSaleCount; }
       set { m_nMaxSaleCount = value; }
   }
   private static ConsignSaleLogic m_Instance = null;
   public static ConsignSaleLogic Instance()
   {
       return m_Instance;
   }

   private List<GameObject> m_ClickSprite = new List<GameObject>();
    bool m_bIsSearchForQiuGou =false;
	// Use this for initialization
    void Awake()
    {
        m_Instance = this;
    }
	void Start () 
    {
        //先隐藏 暂时不显示的
        m_SubWuQiGrid.SetActive(false);
        m_SubFangJuGrid.SetActive(false);
        m_SubShiPinGrid.SetActive(false);
        m_SubYaoPinGird.SetActive(false);
        m_SaleUpPage.SetActive(false);
        m_SaleDownPage.SetActive(false);
        m_BuyUpPage.SetActive(false);
        m_BuyDownPage.SetActive(false);
        m_Menu.SetActive(false);
        m_QualityMenu.SetActive(false);
        m_SaleBag.SetActive(false);
        m_NotFindBuyLable.SetActive(false);
        m_NotFindSaleLable.SetActive(false);
        //默认打开点击购买分页
	    ClickBuyBt();
        m_moneyLable.text = GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBao().ToString();
	}

    void OnDestroy()
    {
        m_Instance = null;
    }

    //隐藏 购买界面物品显示条
    void CleanBuyItemInfo()
    {
        for (int _ItemIndex = 0; _ItemIndex < (int)ConsignSaleData.MAXCOUNTSANDTOCLIENT; _ItemIndex++)
        {
            m_BuyItem[_ItemIndex].gameObject.SetActive(false);
        }
        if (CurClickBuyItem != null)
        {
			LastClickBuyItem=CurClickBuyItem;
            CurClickBuyItem.m_BakSprite.MakePixelPerfect();
            CurClickBuyItem = null;
        }
    }
    //更新购买界面信息
    public void UpdateBuyInfo()
    {
        //先清除一下
        CleanBuyItemInfo();
        //显示无查找到
        if (m_buyItemInfo.Count <= 0)
        {
            m_NotFindBuyLable.SetActive(true);
            return;
        }
        m_NotFindBuyLable.SetActive(false);
        //先隐藏 上一页和下一页
        m_BuyUpPage.SetActive(false);
        m_BuyDownPage.SetActive(false);
        //更新查找到的物品条目信息
        for (int _ItemIndex = 0; _ItemIndex < (int)ConsignSaleData.MAXCOUNTSANDTOCLIENT; _ItemIndex++)
        {
            if (_ItemIndex>=0 && _ItemIndex<m_buyItemInfo.Count)
            {

                m_BuyItem[_ItemIndex].UpdateItemInfo(m_buyItemInfo[_ItemIndex]);
            }
        }
//		if(CurClickBuyItem==null&&m_buyItemInfo.Count>0)
//		{
//			ShowItem(null);
//		}
        m_moneyLable.text = GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBao().ToString();
        //当前页不是第一页 显示查看上一页
        if (m_nCurBuyPage >1)
        {
            m_BuyUpPage.SetActive(true);
        }
        //查询的结果 达到显示的上限 显示查看下一页
        if (m_buyItemInfo.Count >= (int)ConsignSaleData.MAXCOUNTSANDTOCLIENT)
        {
            m_BuyDownPage.SetActive(true);
        }
        //重新排列
        m_buyUITopGird.Recenter(true);
        m_buyUIGird.hideInactive = true;
        m_buyUIGird.sorted = true;
        m_buyUIGird.Reposition();
    }
    //点击购买分页
    void ClickBuyBt()
    {
        m_SaleRoot.SetActive(false);
        m_buyRoot.SetActive(true);
        m_SaleLableHight.SetActive(false);
        m_BuyLableHight.SetActive(true);
        if (m_bBuyFirstClick ==false && m_bBuyItemLoad ==false)
        {
            return;
        }
        //第一次点击 先load item
        if (m_bBuyFirstClick)
        {
           CreateBuyItem();
        }
        else //后面的点击直接发包请求信息
        {
           SendAskSearchInfo(m_nCurBuyPage);
        }
        m_bBuyFirstClick = false;
    }
    //点击出售分页
    void ClickSaleBt()
    {
        m_buyRoot.SetActive(false);
        m_SaleRoot.SetActive(true);
        m_BuyLableHight.SetActive(false);
        m_SaleLableHight.SetActive(true);
        if (m_bSaleFirstClick == false && m_bSaleItemLoad == false)
        {
            return;
        }
        if (m_bSaleFirstClick)//第一次点击 先load item
        {
            CreateSaleItem();
        }
        else//后面的点击直接发包请求信息
        {
            AskMySaleItemInfo(m_nCurSalePage);
        }
        m_bSaleFirstClick = false;
    }
    void CreateSaleItem()
    {
        UIManager.LoadItem(UIInfo.ConsignSaleMySaleItem, OnLoadSaleItem);
    }
    //加载出售界面物品条目item
    void OnLoadSaleItem(GameObject resObj, object param)
    {
        if (null == resObj)
        {
            LogModule.ErrorLog("load SaleItem error");
            return;
        }
        for (int nIndex = 0; nIndex < (int)ConsignSaleData.MAXCOUNTSANDTOCLIENT; ++nIndex)
        {
            GameObject _gameObject = Utils.BindObjToParent(resObj, m_SaleGird, (nIndex + 1000).ToString());
            if (null != _gameObject)
            {
                ConsignSaleMyItem myItem = _gameObject.GetComponent<ConsignSaleMyItem>();
                if (null != myItem)
                {
                    myItem.SetItemIndex(nIndex);
                    m_SaleItem[nIndex] = myItem;
                }

                _gameObject.SetActive(false);
            }
        }
        AskMySaleItemInfo(0);
    }
    void CreateBuyItem()
    {
        UIManager.LoadItem(UIInfo.ConsignSaleBuyItem, OnLoadBuyItem);
    }
    //加载购买界面物品条目item
    void OnLoadBuyItem(GameObject resObj, object param)
    {
        if (null == resObj)
        {
            LogModule.ErrorLog("load OnLoadBuyItem error");
            return;
        }
        for (int nIndex = 0; nIndex < (int)ConsignSaleData.MAXCOUNTSANDTOCLIENT; ++nIndex)
        {
            GameObject _gameObject = Utils.BindObjToParent(resObj, m_buyGird, (nIndex + 1000).ToString());
            if (null != _gameObject)
            {
                ConsignsaleBuyItem buyItem = _gameObject.GetComponent<ConsignsaleBuyItem>();
                if (null != buyItem)
                {
                    buyItem.SetItemIndex(nIndex);
                    m_BuyItem[nIndex] = buyItem;
                }

                _gameObject.SetActive(false);
            }
        }
     
        //加载完后 开始加载左边分类按钮
        CreateSearchClassBtItem();
        m_bBuyItemLoad = true;
    }
    //请求 自己上架的物品信息
    public  void SendAskSearchInfo(int nCurPage)
    {
		KeyWordInputCommit ();
        CG_ASK_CONSIGNSALEITEMINFO searchPak = (CG_ASK_CONSIGNSALEITEMINFO)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_CONSIGNSALEITEMINFO);
        searchPak.SetSearchclass(m_nSearchClass);
        searchPak.SetSearchsubclass(m_nSearchSubClass);
        searchPak.SetSearchquality(m_nSearchQuality);
        searchPak.SetSortclass(m_nSortClass);
        searchPak.SetSorttype(m_nSortType);
        searchPak.SetKeyword(m_keyWord);
        searchPak.SetPage(nCurPage);
       searchPak.SendPacket();
    }
    
    void CreateSearchClassBtItem()
    {
        UIManager.LoadItem(UIInfo.ConsignSaleClassItem, OnLoadSearchClassBtItem);
    }

    void SetClassBtInfo(GameObject _gameObject,string btName, int nClass, GameObject _tweenTarget)
    {
        if (null == _gameObject || null == _gameObject.GetComponent<UIPlayTween>())
            return;

        ConsignSaleClass consignSaleClassInfo = _gameObject.GetComponent<ConsignSaleClass>();
        if (null != consignSaleClassInfo)
        {
            consignSaleClassInfo.SetNameInfo(btName);
            consignSaleClassInfo.Class = nClass;
            _gameObject.GetComponent<UIPlayTween>().tweenTarget = _tweenTarget;
            m_ClickSprite.Add(consignSaleClassInfo.m_ClickSprite.gameObject);
            _gameObject.SetActive(true);
            if (null != consignSaleClassInfo.m_ClickSprite)
                consignSaleClassInfo.m_ClickSprite.gameObject.SetActive(false);
        }
    }
    //加载左边分类按钮
    void OnLoadSearchClassBtItem(GameObject resObj, object param)
    {
        if (null == resObj)
        {
            LogModule.ErrorLog("load OnLoadBuyItem error");
            return;
        }

        //武器按钮
        GameObject _gameObject = Utils.BindObjToParent(resObj, m_searchGird, "00WuqiSubBt");
        if (null != _gameObject)
        {
            SetClassBtInfo(_gameObject, StrDictionary.GetClientDictionaryString("#{1694}"), (int)ConsignSale_SearchClass.WEAPON, m_SubWuQiGrid);
            //第一次加载 点击分类按钮(求购时 不点击)
            if (m_bIsSearchForQiuGou == false || m_bBuyFirstClick == false)
            {
                _gameObject.GetComponent<ConsignSaleClass>().ClickClassBt();
            }
        }

        //防具按钮
        _gameObject = Utils.BindObjToParent(resObj, m_searchGird, "10FangJuBt");
        if (null != _gameObject)
            SetClassBtInfo(_gameObject, StrDictionary.GetClientDictionaryString("#{1695}"), (int)ConsignSale_SearchClass.FANGJU, m_SubFangJuGrid);
        //饰品按钮
        _gameObject = Utils.BindObjToParent(resObj, m_searchGird, "20ShiPinBt");
        if (null != _gameObject)
            SetClassBtInfo(_gameObject, StrDictionary.GetClientDictionaryString("#{1696}"), (int)ConsignSale_SearchClass.CHARM, m_SubShiPinGrid);
        //材料按钮
        _gameObject = Utils.BindObjToParent(resObj, m_searchGird, "30CaiLiaoBt");
        if (null != _gameObject)
            SetClassBtInfo(_gameObject, StrDictionary.GetClientDictionaryString("#{2386}"), (int)ConsignSale_SearchClass.MATERIAL, m_SubCaiLiaoGird);
        //药品按钮
        _gameObject = Utils.BindObjToParent(resObj, m_searchGird, "40YaoPinBt");
        if (null != _gameObject)
            SetClassBtInfo(_gameObject, StrDictionary.GetClientDictionaryString("#{1697}"), (int)ConsignSale_SearchClass.MEDIC, m_SubYaoPinGird);

        //药品按钮 禁用UIPlayTween
        if (_gameObject != null && _gameObject.GetComponent<UIPlayTween>() != null)
        {
            _gameObject.GetComponent<UIPlayTween>().enabled = false;
        }

        //加载完后 开始加载子分类按钮
        CreateSearchSubClassBtItem();
    }

    void CreateSearchSubClassBtItem()
    {
        UIManager.LoadItem(UIInfo.ConsignSaleSubClassItem, OnLoadSearchSubClassBtItem);
    }

    void SetSubClassBtInfo(GameObject _gameObject,string btName,int _Class,int _subClass)
    {
        if (null == _gameObject)
            return;

        ConsignSaleSubClass saleSubClass = _gameObject.GetComponent<ConsignSaleSubClass>();
        if (null != saleSubClass)
        {
            saleSubClass.SetNameInfo(btName);
            saleSubClass.Class = _Class;
            saleSubClass.SubClass = _subClass;
            if (null != _gameObject.GetComponent<UIDragPanelContents>())
                _gameObject.GetComponent<UIDragPanelContents>().draggablePanel = m_LeftInfoRoot.GetComponent<UIDraggablePanel>();

            m_ClickSprite.Add(saleSubClass.m_ClickSprite.gameObject);
            if (null != saleSubClass.m_ClickSprite)
                saleSubClass.m_ClickSprite.gameObject.SetActive(false);
        }
    }
    //开始加载子分类按钮
    void OnLoadSearchSubClassBtItem(GameObject resObj, object param)
    {
        if (null == resObj)
        {
            LogModule.ErrorLog("load OnLoadBuyItem error");
            return;
        }

        //武器子按钮
        GameObject _gameObject = Utils.BindObjToParent(resObj, m_SubWuQiGrid, "0GunBt");
        if (null != _gameObject)
            SetSubClassBtInfo(_gameObject, StrDictionary.GetClientDictionaryString("#{1699}"), (int)ConsignSale_SearchClass.WEAPON, (int)ConsignSale_SearchSubClass.WEAPON_SHAOLIN);

        _gameObject = Utils.BindObjToParent(resObj, m_SubWuQiGrid, "1ShuangDuanBt");
        if (null != _gameObject)
            SetSubClassBtInfo(_gameObject, StrDictionary.GetClientDictionaryString("#{1700}"), (int)ConsignSale_SearchClass.WEAPON, (int)ConsignSale_SearchSubClass.WEAPON_TIANSHAN);

        _gameObject = Utils.BindObjToParent(resObj, m_SubWuQiGrid, "2Jian");
        if (null != _gameObject)
            SetSubClassBtInfo(_gameObject, StrDictionary.GetClientDictionaryString("#{1701}"), (int)ConsignSale_SearchClass.WEAPON, (int)ConsignSale_SearchSubClass.WEAPON_DALI);

        _gameObject = Utils.BindObjToParent(resObj, m_SubWuQiGrid, "3Fu");
        if (null != _gameObject)
            SetSubClassBtInfo(_gameObject, StrDictionary.GetClientDictionaryString("#{1702}"), (int)ConsignSale_SearchClass.WEAPON, (int)ConsignSale_SearchSubClass.WEAPON_XIAOYAO);

        m_SubWuQiUIGrid.hideInactive = false;
        m_SubWuQiUIGrid.sorted = true;
        m_SubWuQiUIGrid.repositionNow = true;

      //  防具子按钮
        _gameObject = Utils.BindObjToParent(resObj, m_SubFangJuGrid, "0HuWan");
        if (null != _gameObject)
            SetSubClassBtInfo(_gameObject, StrDictionary.GetClientDictionaryString("#{1703}"), (int)ConsignSale_SearchClass.FANGJU, (int)ConsignSale_SearchSubClass.CUFF);

        _gameObject = Utils.BindObjToParent(resObj, m_SubFangJuGrid, "1MaoZi");
        if (null != _gameObject)
            SetSubClassBtInfo(_gameObject, StrDictionary.GetClientDictionaryString("#{1704}"), (int)ConsignSale_SearchClass.FANGJU, (int)ConsignSale_SearchSubClass.HEAD);

        _gameObject = Utils.BindObjToParent(resObj, m_SubFangJuGrid, "2ShangYi");
        if (null != _gameObject)
            SetSubClassBtInfo(_gameObject, StrDictionary.GetClientDictionaryString("#{1705}"), (int)ConsignSale_SearchClass.FANGJU, (int)ConsignSale_SearchSubClass.ARMOR);

        _gameObject = Utils.BindObjToParent(resObj, m_SubFangJuGrid, "3KuZi");
        if (null != _gameObject)
            SetSubClassBtInfo(_gameObject, StrDictionary.GetClientDictionaryString("#{1706}"), (int)ConsignSale_SearchClass.FANGJU, (int)ConsignSale_SearchSubClass.LEG_GUARD);

        _gameObject = Utils.BindObjToParent(resObj, m_SubFangJuGrid, "4XieZi");
        if (null != _gameObject)
            SetSubClassBtInfo(_gameObject, StrDictionary.GetClientDictionaryString("#{1707}"), (int)ConsignSale_SearchClass.FANGJU, (int)ConsignSale_SearchSubClass.SHOES);

        m_SubFangJuUIGrid.hideInactive = false;
        m_SubFangJuUIGrid.sorted = true;
        m_SubFangJuUIGrid.repositionNow = true;

        //饰品子按钮
        _gameObject = Utils.BindObjToParent(resObj, m_SubShiPinGrid, "0HuFu");
        if (null != _gameObject)
            SetSubClassBtInfo(_gameObject, StrDictionary.GetClientDictionaryString("#{2385}"), (int)ConsignSale_SearchClass.CHARM, (int)ConsignSale_SearchSubClass.AMULET);
        _gameObject = Utils.BindObjToParent(resObj, m_SubShiPinGrid, "1DiaoZui");
        if (null != _gameObject)
            SetSubClassBtInfo(_gameObject, StrDictionary.GetClientDictionaryString("#{1708}"), (int)ConsignSale_SearchClass.CHARM, (int)ConsignSale_SearchSubClass.CHARM1);
        _gameObject = Utils.BindObjToParent(resObj, m_SubShiPinGrid, "2JieZi");
        if (null != _gameObject)
            SetSubClassBtInfo(_gameObject, StrDictionary.GetClientDictionaryString("#{1709}"), (int)ConsignSale_SearchClass.CHARM, (int)ConsignSale_SearchSubClass.CHARM2);

        m_SubShiPinUIGrid.hideInactive = false;
        m_SubShiPinUIGrid.sorted = true;
        m_SubShiPinUIGrid.repositionNow = true;
        
        //材料子按钮
        _gameObject = Utils.BindObjToParent(resObj, m_SubCaiLiaoGird, "0DaZaoTu");
        if (null != _gameObject)
            SetSubClassBtInfo(_gameObject, StrDictionary.GetClientDictionaryString("#{2388}"), (int)ConsignSale_SearchClass.MATERIAL, (int)ConsignSale_SearchSubClass.LUEPRINT);

        _gameObject = Utils.BindObjToParent(resObj, m_SubCaiLiaoGird, "1ShenghuoCaiLiao");
        if (null != _gameObject)
            SetSubClassBtInfo(_gameObject, StrDictionary.GetClientDictionaryString("#{2387}"), (int)ConsignSale_SearchClass.MATERIAL, (int)ConsignSale_SearchSubClass.LIFE_MATERIAL);

        m_SubCaiLiaoUIGrid.hideInactive = false;
        m_SubCaiLiaoUIGrid.sorted = true;
        m_SubCaiLiaoUIGrid.repositionNow = true;
        //分类按钮加载完了 开始排列
        m_searchUITable.sorted = true;
        m_searchUITable.hideInactive = true;
        m_searchUITable.keepWithinPanel = true;
        m_searchUITable.repositionNow = true;
        //全部加载完了
        m_bSaleItemLoad = true;
    }
    //点击查询按钮
    public void ProcessClickSearchBt(int nSearchClass, int nSearchSubClass)
    {
        for (int i = 0; i < m_ClickSprite.Count; i++)
        {
            m_ClickSprite[i].SetActive(false); //隐藏所有筛选按钮的点击图标
        }
        m_nCurBuyPage = 0;
        m_nSearchClass = nSearchClass;
        m_nSearchSubClass = nSearchSubClass;
        m_nSearchQuality = (int)ItemQuality.QUALITY_INVALID;
        m_nSortClass = (int)ConsignSale_SortClass.QUALITY;
        m_nSortType = (int)ConsignSale_SortType.DOWN;
        SendAskSearchInfo(m_nCurBuyPage);
        //隐藏排序箭头
        for (int i = 0; i < (int)ConsignSale_SortClass.MAXNUM; i++)
        {
            m_SortSprite[i].gameObject.SetActive(false);
        }
    }

    //点击购买分页标题 进行排序查询
    void ProcessClickTitleSort(int nSortClass)
    {
        if (m_nSortClass !=nSortClass)
        {
            m_nSortType =(int)ConsignSale_SortType.DOWN;
        }
        else
        {
            m_nSortType =(m_nSortType + 1)%2;
        }
        m_nSortClass =nSortClass;
        for (int i = 0; i < (int)ConsignSale_SortClass.MAXNUM; i++)
        {
            m_SortSprite[i].gameObject.SetActive(false);
        }
        if (m_nSortType == (int)ConsignSale_SortType.UP)
        {
            if (nSortClass >= 0 && nSortClass < (int)ConsignSale_SortClass.MAXNUM && null != m_SortSprite[nSortClass])
            {
                m_SortSprite[nSortClass].spriteName = "ui_auction_06";
                m_SortSprite[nSortClass].MakePixelPerfect();
                Vector3 _veclocalEulerAngles = m_SortSprite[nSortClass].transform.localEulerAngles;
                _veclocalEulerAngles.z = 0;
                m_SortSprite[nSortClass].transform.localEulerAngles = _veclocalEulerAngles;
                m_SortSprite[nSortClass].gameObject.SetActive(true);
            }
        }
        else
        {
            if (nSortClass >= 0 && nSortClass < (int)ConsignSale_SortClass.MAXNUM && null != m_SortSprite[nSortClass])
            {
				m_SortSprite[nSortClass].spriteName = "ui_auction_06";
                m_SortSprite[nSortClass].MakePixelPerfect();
                Vector3 _veclocalEulerAngles = m_SortSprite[nSortClass].transform.localEulerAngles;
                _veclocalEulerAngles.z = -180;
                m_SortSprite[nSortClass].transform.localEulerAngles = _veclocalEulerAngles;
                m_SortSprite[nSortClass].gameObject.SetActive(true);
            }
        }
        m_nCurBuyPage = 0;
        SendAskSearchInfo(m_nCurBuyPage);
    }

    void ClickPinZhiLable()
    {
       m_QualityMenu.SetActive(true);
    }
    void ClickDengJiLable()
    {
        ProcessClickTitleSort((int)ConsignSale_SortClass.LEVEL);
        
    }
    void ClickShuLingLable()
    {
        ProcessClickTitleSort((int)ConsignSale_SortClass.COUNT);
    }
    void ClickZongJiaLable()
    {
        ProcessClickTitleSort((int)ConsignSale_SortClass.PRICE);
    }
    //购买物品
    public void BuyItem()
    {
        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (_mainPlayer ==null)
        {
            return;
        }
        if (m_nSelBuyIndex>=0 && m_nSelBuyIndex<m_buyItemInfo.Count)
        {
            //金钱是否足够
            int nMoney =GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBao();
            if (nMoney <m_buyItemInfo[m_nSelBuyIndex].Price)
            {
                _mainPlayer.SendNoticMsg(false, "#{1637}");
                return;
            }
            //发包购买
            CG_BUY_CONSIGNSALEITEMINFO buyPak = (CG_BUY_CONSIGNSALEITEMINFO)PacketDistributed.CreatePacket(MessageID.PACKET_CG_BUY_CONSIGNSALEITEMINFO);
            buyPak.SetSearchclass(m_nSearchClass);
            buyPak.SetSearchsubclass(m_nSearchSubClass);
            buyPak.SetSearchquality(m_nSearchQuality);
            buyPak.SetSortclass(m_nSortClass);
            buyPak.SetSorttype(m_nSortType);
            buyPak.SetKeyword(m_keyWord);
            buyPak.SetPage(m_nCurBuyPage);
            buyPak.SetItemguid(m_buyItemInfo[m_nSelBuyIndex].ItemInfo.Guid);
            buyPak.SendPacket();
        }
    }
    //查看购买界面物品信息
    public void ShowBuyItemInfo()
    {
        if (m_nSelBuyIndex >= 0 && m_nSelBuyIndex<m_buyItemInfo.Count)
        {
            if (m_buyItemInfo[m_nSelBuyIndex].ItemInfo.IsEquipMent())
            {
                EquipTooltipsLogic.ShowEquipTooltip(m_buyItemInfo[m_nSelBuyIndex].ItemInfo, EquipTooltipsLogic.ShowType.InfoCompare);
            }
            else
            {
                ItemTooltipsLogic.ShowItemTooltip(m_buyItemInfo[m_nSelBuyIndex].ItemInfo, ItemTooltipsLogic.ShowType.Info);
            }
        }
    }
    void CleanSaleInfo()
    {
        for (int _ItemIndex = 0; _ItemIndex < (int) ConsignSaleData.MAXCOUNTSANDTOCLIENT; _ItemIndex++)
        {
            m_SaleItem[_ItemIndex].gameObject.SetActive(false);
        }
       
    }
    //更新出售界面信息
    public void UpdateSaleInfo()
    {
        CleanSaleInfo();
        m_moneyLable.text = GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBao().ToString();
        m_SaleCountLable.text = m_nCurSaleCount.ToString() + "/" + m_nMaxSaleCount.ToString();
        if (m_SaleItemInfo.Count <=0)
        {
            m_NotFindSaleLable.SetActive(true);
            return;
        }
        m_NotFindSaleLable.SetActive(false);
        m_SaleUpPage.SetActive(false);
        m_SaleDownPage.SetActive(false);
        for (int _ItemIndex = 0; _ItemIndex < (int)ConsignSaleData.MAXCOUNTSANDTOCLIENT; _ItemIndex++)
        {
            if (_ItemIndex >= 0 && _ItemIndex < m_SaleItemInfo.Count)
            {
                m_SaleItem[_ItemIndex].UpdateItemInfo(m_SaleItemInfo[_ItemIndex]);
            }
        }
      
        //当前页不是第一页 显示查看上一页
        if (m_nCurSalePage > 0)
        {
            m_SaleUpPage.SetActive(true);
        }
        //查询的结果 达到显示的上限 显示查看下一页
        if (m_SaleItemInfo.Count >= (int)ConsignSaleData.MAXCOUNTSANDTOCLIENT &&
            m_nCurSaleCount > (m_nCurSalePage + 1) * (int)ConsignSaleData.MAXCOUNTSANDTOCLIENT)
        {
            m_SaleDownPage.SetActive(true);
        }
        //重新排列
        m_SaleUITopGird.Recenter(true);
        m_SaleUIGird.hideInactive = true;
        m_SaleUIGird.sorted = true;
        m_SaleUIGird.repositionNow = true;
    }
    //发包查询上架的物品
    void AskMySaleItemInfo(int nCurPage)
    {
        //发包查询上架的物品
        CG_ASK_MYCONSIGNSALEITEM asksalePak = (CG_ASK_MYCONSIGNSALEITEM)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_MYCONSIGNSALEITEM);
        asksalePak.SetCurpage(nCurPage);
        asksalePak.SendPacket();
    }
    //出售物品
    public void SaleItem(GameItem saleItem,int nItemCount,int nItemPrice)
    {
        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (_mainPlayer ==null)
        {
            return;
        }
        //校验物品
        if (ConsignSaleBag.isCanConsignSale(saleItem) ==false)
        {
            return;
        }
        if (nItemCount<=0 || nItemCount>saleItem.StackCount)
        {
            //提示 数量输入错误
            _mainPlayer.SendNoticMsg(false, "#{1691}");
            return;
        }
        if (nItemPrice <2)
        {
            //提示 价格输入错误
            _mainPlayer.SendNoticMsg(false, "#{1692}");
            return;
        }
        //发包购买
        CG_CONSIGNSALEITEM salePak = (CG_CONSIGNSALEITEM)PacketDistributed.CreatePacket(MessageID.PACKET_CG_CONSIGNSALEITEM);
        salePak.SetItemguid(saleItem.Guid);
        salePak.SetItemcount((UInt32)nItemCount);
		salePak.SetItemprice((UInt32)nItemPrice);
		salePak.SetCurpage((UInt32)m_nCurBuyPage);
        salePak.SendPacket();
    }
    //下架物品
    public void CancelSaleItem()
    {
        if (m_nSelSaleIndex>=0 && m_nSelSaleIndex<m_SaleItemInfo.Count)
        {
            //发包下架
            CG_CANCELCONSIGNSALEITEM cancelsalePak = (CG_CANCELCONSIGNSALEITEM)PacketDistributed.CreatePacket(MessageID.PACKET_CG_CANCELCONSIGNSALEITEM);
            cancelsalePak.SetItemguid(m_SaleItemInfo[m_nSelSaleIndex].ItemInfo.Guid);
            cancelsalePak.SetCurpage(m_nCurSalePage);
            cancelsalePak.SendPacket();

			//发包查询上架的物品
			CG_ASK_MYCONSIGNSALEITEM asksalePak = (CG_ASK_MYCONSIGNSALEITEM)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_MYCONSIGNSALEITEM);
			asksalePak.SetCurpage(m_nCurSalePage);
			asksalePak.SendPacket();
        }
    }

    void ClickSaleButton()
    {
		//加个更新，不然看的时候回重叠
		ConsignSaleLogic.Instance().m_SaleBag.GetComponent<ConsignSaleBag>().UpdateBackPack();

        m_SaleBag.SetActive(true);

		//上架界面遮不住售卖物品，先关掉
		m_SaleGird.SetActive (false);
    }

    void ClickBuyDownPage()
    {
        SendAskSearchInfo(m_nCurBuyPage+1);
    }
    void ClickBuyUpPage()
    {
        if (m_nCurBuyPage >0)
        {
            SendAskSearchInfo(m_nCurBuyPage-1);
        }
    }
    void ClickSaleDownPage()
    {
        AskMySaleItemInfo(m_nCurSalePage+1);
    }
    void ClickSaleUpPage()
    {
        if (m_nCurSalePage > 0)
        {
            AskMySaleItemInfo(m_nCurSalePage-1);
        }
    }
    void OnCloseClick()
    {
        UIManager.CloseUI(UIInfo.ConsignSaleRoot);
    }


    void ClickKeyWordSearchBt()
    {
        m_nCurBuyPage = 0;
        SendAskSearchInfo(m_nCurBuyPage);
    }

    public void SearchForAskBuy(string _keyWord) //求购
    {
        m_bIsSearchForQiuGou = true;
        m_nCurBuyPage = 0;
        m_nSearchClass =(int)ConsignSale_SearchClass.ALL; ;
        m_nSearchSubClass =(int)ConsignSale_SearchSubClass.ALL;
        m_nSearchQuality =(int)ItemQuality.QUALITY_INVALID;
        m_nSortClass =(int)ConsignSale_SortClass.QUALITY;
        m_nSortType = (int)ConsignSale_SortType.DOWN;
        m_keyWord = _keyWord;
        m_KeyWordInput.value = _keyWord;
        SendAskSearchInfo(m_nCurBuyPage);
    }
    public void KeyWordInputCommit()
    {
        m_keyWord = m_KeyWordInput.value;
    }
	public void ShowItem(ConsignsaleBuyItem  curItem)
	{
		if (null == curItem) 
						return;
			
		if (m_CurClickBuyItem == curItem) return;

		if (m_CurClickBuyItem != null) {
			m_CurClickBuyItem.EnableHighlight(false);
				}
		m_CurClickBuyItem = curItem;
		if (m_LastClickBuyItem != null) {

			m_LastClickBuyItem.EnableHighlight(false);
			m_LastClickBuyItem=null;
		}
		m_CurClickBuyItem.EnableHighlight(true);

	}
}
