using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopBottom : MonoBehaviour
{
    public ShopManager ShopManager;
    public UILabel page;

    public void NextPage()
    {
        ShopManager.Ins.ShopContent.NextPage();
        RefreshPageText();
    }

    public void LastPage()
    {
        ShopManager.Ins.ShopContent.LastPage();
        RefreshPageText();
    }

    public void RefreshPageText()
    {
        page.text = ShopManager.ShopContent.CurPage + "/" + ShopManager.ShopContent.AllPage;
    }
}
