/********************************************************************
	创建时间:	2014/06/12 13:11
	全路径:	\Version\Main\Project\Client\Assets\MLDJ\Script\GUI\ConsignSaleBag.cs
	创建人:		luoy
	功能说明:	寄售行上架背包逻辑
	修改记录:
*********************************************************************/
using System.Collections.Generic;
using Games.Item;
using GCGame;
using GCGame.Table;
using Module.Log;
using UnityEngine;
using System.Collections;

public class ConsignSaleBag : MonoBehaviour
{
    public enum ITEM_TAB_PAGE
    {     //背包分页
        TAB_PAGE_ALL =0,       //全部
        TAB_PAGE_EQUIP,     //装备
        TAB_PAGE_MEDIC,     //药品
        TAB_PAGE_MATERIAL,  //材料
        MAX_TAB,
    };
	public GameObject  m_lastSaleBag;
    public ITEM_TAB_PAGE m_CurTabPage = ITEM_TAB_PAGE.TAB_PAGE_ALL; //当前背包分页
    public GameObject m_InfoLable;
    public GameObject m_BackPackItems;
    public GameObject m_BackPackItemGrid;
    public GameObject m_NoCanSaleItemLable;
    public UISprite[] m_TabPage_HighLight =new UISprite[(int)ITEM_TAB_PAGE.MAX_TAB];
    public List<GameObject> m_ItemInfo_HighLight = new List<GameObject>();
    public GameObject m_BagSaleUI;
    public const int ITEMOBJECT_COUNT = 25;     //实际创建多少个Itemobject
    public const int ITEMPACK_SIZE = 90;        //最多需要显示多少个Item
    public const int ITEMOBJECT_WIDTH = 150;     //每个ItemObject的宽度

    private int m_Cur_StartItem = 0;                   //存放当前存在的itemobject开始编号
    private int m_Cur_EndItem = 0 + ITEMOBJECT_COUNT;  //存放当前存在的itemobject结束编号
    private List<GameItem> CurItemList;                //当前需要显示的Item
    private bool m_bLoadItem =false;

    void OnEnable()
    {
        m_InfoLable.SetActive(true);
        m_BagSaleUI.SetActive(false);
        if (m_bLoadItem ==false)
        {
            InitPack();
        }
        else
        {
            UpdateBackPack();
        }
    }
    void OnDisable()
    {
        m_CurTabPage = ITEM_TAB_PAGE.TAB_PAGE_ALL;
    }
    //void Start()
    //{
       
    //}

    //void OnDestroy()
    //{
    //}
    /// <summary>
    /// 初始化背包
    /// </summary>
    public void InitPack()
    {
        UIManager.LoadItem(UIInfo.ConsignSaleBagItem, OnLoadBackPackItem);
    }

    void OnLoadBackPackItem(GameObject resObj, object param)
    {
        if (null == resObj)
        {
            LogModule.ErrorLog("load back pack item error");
            return;
        }
        for (int nIndex = 0; nIndex < (ITEMOBJECT_COUNT + 1); ++nIndex)
        {
            GameObject _gameObject =Utils.BindObjToParent(resObj, m_BackPackItemGrid,(nIndex + 1000).ToString());
            if (null != _gameObject)
            {
                ConsignSaleBagItem item = _gameObject.GetComponent<ConsignSaleBagItem>();
                if (null != item)
                    m_ItemInfo_HighLight.Add(item.m_HightSprite);
            }
        }

        UIGrid backPackItemGrid = m_BackPackItemGrid.GetComponent<UIGrid>();
        if (null != backPackItemGrid)
        {
            backPackItemGrid.sorted = true;
            backPackItemGrid.repositionNow = true;
        }

        UIDraggablePanel itemPanel = m_BackPackItems.GetComponent<UIDraggablePanel>();
        if (null != itemPanel)
            itemPanel.onDragFinished += OnItemDragFinished;

        UpdateBackPack();
        m_bLoadItem = true;
    }

    public void CleanAllBackItemHighLight()
    {
        for (int i = 0; i < m_ItemInfo_HighLight.Count; i++)
        {
            m_ItemInfo_HighLight[i].SetActive(false);
        }
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
        for (int i = 0; i <(int)ITEM_TAB_PAGE.MAX_TAB; ++i)
        {
            m_TabPage_HighLight[i].gameObject.SetActive(false);
        }
        int curTabIndex = (int)m_CurTabPage;
        if (m_CurTabPage >= 0 && curTabIndex < (int)ITEM_TAB_PAGE.MAX_TAB)
        {
            m_TabPage_HighLight[curTabIndex].gameObject.SetActive(true);
        }

        UITopGrid topGrid = m_BackPackItemGrid.GetComponent<UITopGrid>();
        if (null != topGrid)
            topGrid.Recenter(true);

        switch (m_CurTabPage)
        {
            case ITEM_TAB_PAGE.TAB_PAGE_ALL: ShowBackPack_All(); break;
            case ITEM_TAB_PAGE.TAB_PAGE_EQUIP: ShowBackPack_Equip(); break;
            case ITEM_TAB_PAGE.TAB_PAGE_MEDIC: ShowBackPack_Medic(); break;
            case ITEM_TAB_PAGE.TAB_PAGE_MATERIAL: ShowBackPack_Material(); break;
        };
    }
    public static bool isCanConsignSale(GameItem checkItemInfo,bool isChatLink =false)
    {
        if (null == checkItemInfo)
            return false;

        if (checkItemInfo.IsValid() ==false)
        {
            return false;
        }
       if (checkItemInfo.BindFlag && isChatLink ==false)
        {
            return false;
        }
        
        Tab_CommonItem _tabCommonItem = TableManager.GetCommonItemByID(checkItemInfo.DataID, 0);
        if (_tabCommonItem ==null)
        {
            return false;
        }

        int nCanConsignSale = _tabCommonItem.IsCanConsign;
        if (nCanConsignSale != 1)
        {
            return false;
        }

        //有时效的物品不让上
        if (_tabCommonItem.ExistTime !=-1)
        {
            return false;
        }
        int itemclass = _tabCommonItem.ClassID;
        int itemSubclass = _tabCommonItem.SubClassID;
        if (itemclass != (int)ItemClass.EQUIP &&
            itemclass != (int)ItemClass.MEDIC &&
            itemclass != (int)ItemClass.MATERIAL)
        {
            return false;
        }
        //装备只上护符和戒指
        //if (itemclass == (int)ItemClass.EQUIP)
        //{
        //    if (itemSubclass !=(int)EquipSubClass.CHARM2 &&
        //        itemSubclass != (int)EquipSubClass.AMULET)
        //    {
        //        return false;
        //    }
        //}
        //材料只上生活材料和打造图
        if (itemclass == (int)ItemClass.MATERIAL)
        {
            if (itemSubclass != (int)MaterialSubClass.BLUEPRINT &&
                itemSubclass != (int)MaterialSubClass.LIFE_MATERIAL)
            {
                return false;
            }
        }
        //药品只上属性药
        if (itemclass == (int)ItemClass.MEDIC)
        {
            if (itemSubclass != (int)MedicSubClass.ATTR)
            {
                return false;
            }
        }
        return true;
    }
    //过滤上架物品
    private List<GameItem> FilterConsignSale(GameItemContainer Container, int nClass, int nSubClass = 0)
    {
        List<GameItem> resultlist = new List<GameItem>();
        for (int nIndex = 0; nIndex < Container.ContainerSize; ++nIndex)
        {
            GameItem item = Container.GetItem(nIndex);
            if (null != item && item.IsValid())
            {
                int itemclass = TableManager.GetCommonItemByID(item.DataID, 0).ClassID;
                int itemsubclass = TableManager.GetCommonItemByID(item.DataID, 0).SubClassID;
                if ((itemclass == nClass || nClass == 0) &&
                    (itemsubclass == nSubClass || nSubClass == 0) &&
                    isCanConsignSale(item))
                {
                    resultlist.Add(item);
                }
            }
        }
        return ItemTool.ItemSort(resultlist);
    }
    /// <summary>
    /// 显示全部分页
    /// </summary>
    private void ShowBackPack_All()
    {
        // 过滤物品
        GameItemContainer BackPack = GameManager.gameManager.PlayerDataPool.BackPack;
        List<GameItem> itemlist1 =FilterConsignSale(BackPack, (int)ItemClass.EQUIP);
        List<GameItem> itemlist2 =FilterConsignSale(BackPack, (int)ItemClass.MEDIC);
        List<GameItem> itemlist3 =FilterConsignSale(BackPack, (int)ItemClass.MATERIAL);
        if (null != itemlist1)
        {
            itemlist1.AddRange(itemlist2);
            itemlist1.AddRange(itemlist3);
            CurItemList = itemlist1;

            //显示物品
            ShowBackPackItemList(itemlist1);
        }
    }
    /// <summary>
    /// 显示装备分页
    /// </summary>
    private void ShowBackPack_Equip()
    {
        // 过滤物品
        GameItemContainer BackPack = GameManager.gameManager.PlayerDataPool.BackPack;
        List<GameItem> itemlist = FilterConsignSale(BackPack, (int)ItemClass.EQUIP);
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
        GameItemContainer BackPack =GameManager.gameManager.PlayerDataPool.BackPack;
        List<GameItem> itemlist =FilterConsignSale(BackPack, (int)ItemClass.MATERIAL);
        CurItemList = itemlist;
        // 显示物品
        ShowBackPackItemList(itemlist);
    }
    /// <summary>
    /// 显示其它分页
    /// </summary>
    private void ShowBackPack_Medic()
    {
        // 过滤物品
        GameItemContainer BackPack = GameManager.gameManager.PlayerDataPool.BackPack;
        List<GameItem> itemlist1 = FilterConsignSale(BackPack, (int)ItemClass.MEDIC);
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
        bool isHaveCanSaleItem = false;
        for (int nIndex = m_Cur_StartItem; nIndex < (m_Cur_EndItem + 1); ++nIndex)
        {
            if (nIndex < BackPack.ContainerSize)
            {
                if (nIndex < itemlist.Count)
                {
                    GameItem item = itemlist[nIndex];
                    Transform itemTranform = m_BackPackItemGrid.transform.FindChild((nIndex + 1000).ToString());
                    if (null != itemTranform)
                    {
                        GameObject ItemObject = itemTranform.gameObject;
                        if (ItemObject != null)
                        {
                            ItemObject.gameObject.SetActive(true);
                            ConsignSaleBagItem bagItem = ItemObject.GetComponent<ConsignSaleBagItem>();
                            if (null != bagItem)
                                bagItem.UpdateBackPackItem(item);
                            isHaveCanSaleItem = true;
                        }
                    }
                }
                else
                {
                    GameObject ItemObject = m_BackPackItemGrid.transform.FindChild((nIndex + 1000).ToString()).gameObject;
                    if (ItemObject != null)
                    {
                        ItemObject.gameObject.SetActive(false);
                    }
                }
            }
        }
        //是否显示无可上架物品
        if (isHaveCanSaleItem)
        {
            m_NoCanSaleItemLable.SetActive(false);
        }
        else
        {
            m_NoCanSaleItemLable.SetActive(true);
        }
    }
    /// <summary>
    /// 关闭背包
    /// </summary>
    public void CloseWindow()
    {
       gameObject.SetActive(false);
		m_lastSaleBag.SetActive (true);
		//发包查询上架的物品
		CG_ASK_MYCONSIGNSALEITEM asksalePak = (CG_ASK_MYCONSIGNSALEITEM)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_MYCONSIGNSALEITEM);
		asksalePak.SetCurpage(0);
		asksalePak.SendPacket();

    }
    ///////////////////////////////////////////////////////////////////////////////////////////////
    // 按钮点击响应 Start
    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void OnClick_TabAll()
    {
        m_CurTabPage = ITEM_TAB_PAGE.TAB_PAGE_ALL;
        OnItemDragFinished();
        UpdateBackPack();
    }

    public void OnClick_TabEquip()
    {
        m_CurTabPage = ITEM_TAB_PAGE.TAB_PAGE_EQUIP;
        OnItemDragFinished();
        UpdateBackPack();
    }

    public void OnClick_TabMaterial()
    {
        m_CurTabPage = ITEM_TAB_PAGE.TAB_PAGE_MATERIAL;
        OnItemDragFinished();
        UpdateBackPack();
    }
    public void OnClick_TabMedic()
    {
        m_CurTabPage = ITEM_TAB_PAGE.TAB_PAGE_MEDIC;
        OnItemDragFinished();
        UpdateBackPack();
    }
    /// <summary>
    /// 每次滑动结束后调用
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
        if (Target_EndItem > ITEMPACK_SIZE - 1)
        {
            Target_EndItem = ITEMPACK_SIZE - 1;
        }
        if (Target_StartItem > m_Cur_StartItem && Target_EndItem > m_Cur_EndItem)
        {
            //手指向上滑   顶端的ItemObject移动到尾端 显示较大编号Item
            for (int nIndex = m_Cur_StartItem; nIndex < Target_StartItem; ++nIndex)
            {
                int TargetPos = m_Cur_EndItem + 1 + (nIndex - m_Cur_StartItem);
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
                        if (null != CurItemList && TargetPos < CurItemList.Count)
                        {
                            //有物品的格子
                            GameItem Item = CurItemList[TargetPos];
                            ItemObject.gameObject.SetActive(true);
                            ConsignSaleBagItem bagItem = ItemObject.GetComponent<ConsignSaleBagItem>();
                            if (null != bagItem)
                                bagItem.UpdateBackPackItem(Item);
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
                        if (null != CurItemList && TargetPos < CurItemList.Count)
                        {
                            //有物品的格子
                            GameItem Item = CurItemList[TargetPos];
                            ItemObject.gameObject.SetActive(true);

                            ConsignSaleBagItem bagItem = ItemObject.GetComponent<ConsignSaleBagItem>();
                            if (null != bagItem)
                                bagItem.UpdateBackPackItem(Item);
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
}
