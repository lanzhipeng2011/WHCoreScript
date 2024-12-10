using GCGame.Table;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemView : MonoBehaviour
{
    public UILabel nameLabel;
    public UILabel priceLabel;
    public UILabel numLabel;
    public UISprite iconSprite;
    public Tab_YuanBaoShop item;
    public ShopContent ShopContent;

    public void Init(Tab_YuanBaoShop item, ShopContent shopContent)
    {
        this.ShopContent = shopContent;
        this.item = item;
        var commonItemDic = TableManager.GetCommonItem();
        Tab_CommonItem commonItem = null;
        foreach (var ite in commonItemDic.Values)
        {
            foreach (var it in ite)
            {
                if(it.Id.Equals(item.ItemID))
                    commonItem = it;
            }
        }
        iconSprite.spriteName = commonItem.Icon;
        nameLabel.text = commonItem.Name;
        priceLabel.text = item.PriceForever.ToString();
        numLabel.text = item.Num.ToString();
    }
    public void SetBuyItem()
    {
        ShopContent.curitem = item;
    }
}
