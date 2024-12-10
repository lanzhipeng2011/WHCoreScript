using GCGame.Table;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopContent : MonoBehaviour
{
    public ShopManager ShopManager;
    public List<ShopItemView> shopItemViews;
    List<Tab_YuanBaoShop> yuanBaoShopAutoCreates;
    public Tab_YuanBaoShop curitem;
    int curPage = 0;
    public int CurPage { get { return curPage + 1; } }
    public int AllPage { get { return yuanBaoShopAutoCreates.Count / 6 + 1; } }

    public void SetData(List<Tab_YuanBaoShop> yuanBaoShopAutoCreates)
    {
        if (this.yuanBaoShopAutoCreates!=null&&this.yuanBaoShopAutoCreates.Equals(yuanBaoShopAutoCreates))
            return;
        this.yuanBaoShopAutoCreates = yuanBaoShopAutoCreates;
        curPage = 0;
        Refresh();
        ShopManager.ShopBottom.RefreshPageText();
    }
    public void NextPage()
    {
        curPage++;
        if (curPage * 6 > yuanBaoShopAutoCreates.Count)
        {
            curPage--;
            return;
        }
        Refresh();
    }
    public void LastPage()
    {
        curPage--;
        if (curPage < 0)
        {
            curPage++;
            return;
        }
        Refresh();
    }
    public void Refresh()
    {
        HideAll();
        int left = curPage * 6;
        int right = curPage * 6 + 6;
        int j = 0;
        for (int i = left; i < right; i++, j++)
        {
            //Debug.Log(i + "::::::::" + yuanBaoShopAutoCreates.Count);
            if(i < yuanBaoShopAutoCreates.Count)
            {
                shopItemViews[j].gameObject.SetActive(true);
                shopItemViews[j].Init(yuanBaoShopAutoCreates[i], this);
            }
        }
    }
    public void HideAll()
    {
        for (int i = 0; i < shopItemViews.Count; i++)
        {
            shopItemViews[i].gameObject.SetActive(false);
        }
    }
}
