/********************************************************************
	created:	2014/01/14
	created:	14:1:2014   13:49
	filename: 	SysShopPage.cs
	author:		王迪
	
	purpose:	系统商店页，管理当前页的所有物品高亮控制以及初始化
*********************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GCGame.Table;
public class SysShopPage : MonoBehaviour {

    public float xStartPos = -128;
    public float yStartPos = 155;
    public float widthDiff = 210;
    public float heightDiff = 75;

    private SysShopPageItem m_curHighLightItem = null;
	// Use this for initialization
	void Start () {
	
	}

    void OnDisable()
    {
        if (null != m_curHighLightItem)
        {
            m_curHighLightItem.EnableHighLight(false);
            m_curHighLightItem = null;
        }
    }

    public SysShopPageItem GetCurHighLightItem()
    {
        return m_curHighLightItem;
    }

    public void AddItem(GameObject curItem, int index,  int shopIndex, Tab_SystemShop tabSysShop)
    {
        if (null == curItem)
            return;

        curItem.transform.localPosition = new Vector3(xStartPos + widthDiff * (index % 2), yStartPos - heightDiff * (index /2));
        SysShopPageItem shopPageItem = curItem.GetComponent<SysShopPageItem>();
        if (null != shopPageItem)
            shopPageItem.SetData(ChildItemClick, ChildItemDoubleClick, ChildItemIconClick, shopIndex, tabSysShop);
    }

    // 点中高亮
    void ChildItemClick(SysShopPageItem curItem)
    {
        HighLightItem(curItem);
    }

    // 双击直接购买
    void ChildItemDoubleClick(SysShopPageItem curItem)
    {
        HighLightItem(curItem);
        SysShopController.BuyItem(curItem.name, 1);        
    }

    // 图标被点中展示物品信息
    void ChildItemIconClick(SysShopPageItem curItem)
    {
        HighLightItem(curItem);
        if (null != SysShopController.Instance())
            SysShopController.Instance().ShowCurItemTip();
    }

    void HighLightItem(SysShopPageItem curItem)
    {
        if (m_curHighLightItem != null)
        {
            m_curHighLightItem.EnableHighLight(false);
        }

        if (null != curItem)
            curItem.EnableHighLight(true);

        m_curHighLightItem = curItem;
    }

   
}
