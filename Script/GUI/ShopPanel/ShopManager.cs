using GCGame.Table;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Ins;
    private void Awake() { Ins = this; }
    Dictionary<int, List<Tab_YuanBaoShop>> dic = new Dictionary<int, List<Tab_YuanBaoShop>>();
    public ShopTopToggle ShopTopToggle;
    public ShopContent ShopContent;
    public ShopBottom ShopBottom;
    public ShopBuywindow ShopBuywindow;
    void Start()
    {
        InitData();
        SetContentData(0);
    }

    public void SetContentData(int index)
    {
        ShopContent.SetData(dic[index]);
    }

    private void InitData()
    {
        var shop = TableManager.GetYuanBaoShop();
        foreach (var item in shop)
        {
            foreach (var ite in item.Value)
            {
                if (!dic.ContainsKey(ite.TabIndex))
                    dic.Add(ite.TabIndex, new List<Tab_YuanBaoShop>());
                dic[ite.TabIndex].Add(ite);
            }
        }
    }
    public void OpenBuyWindow()
    {
        ShopBuywindow.gameObject.SetActive(true);
        ShopBuywindow.Init(ShopContent.curitem);
    }
    public void CloseShop()
    {
        UIManager.CloseUI(UIInfo.MyShop);
    }
}
