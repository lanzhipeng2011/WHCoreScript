using UnityEngine;
using GCGame.Table;

public class ShopBuywindow : MonoBehaviour 
{
	public UIInput input;
	public UISprite iconsprite;
    public UILabel priceLabel;
    public UILabel namelabel;
	private int count = 1;
	private Tab_YuanBaoShop item;

    public void OnBuyClick()
    {
        CG_BUY_YUANBAOGOODS packet = (CG_BUY_YUANBAOGOODS)PacketDistributed.CreatePacket(MessageID.PACKET_CG_BUY_YUANBAOGOODS);
        packet.GoodID = item.Id;
        packet.BuyNum = 1;
        packet.IsUseBind = (uint)0;
        packet.Deadline = (int)YuanBaoShopItemLogic.DEADLINE_PRICE.PRICE_FOREVER;
        packet.SendPacket();
    }

	public void Init(Tab_YuanBaoShop item)
    {
        if (ReferenceEquals(item, null)) return;
        count = 1;
        this.item = item;
        RefreshInput();
        RefreshPrice();
        var commonItemDic = TableManager.GetCommonItem();
        Tab_CommonItem commonItem = null;
        foreach (var ite in commonItemDic.Values)
        {
            foreach (var it in ite)
            {
                if (it.Id.Equals(item.ItemID))
                    commonItem = it;
            }
        }
        iconsprite.spriteName = commonItem.Icon;
        namelabel.text = commonItem.Name;
    }

    private void RefreshPrice()
    {
        priceLabel.text = "总价：" + (item.PriceForever * count).ToString();
    }

    private void RefreshInput()
    {
        input.value = count.ToString();
    }

    public void Add()
    {
        count++;
        RefreshInput(); 
        RefreshPrice();
    }
    public void Remove()
    {
        if (count == 1) return;
        count--;
        RefreshInput();
        RefreshPrice();
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
}
