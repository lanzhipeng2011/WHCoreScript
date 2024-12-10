using UnityEngine;
using System.Collections;

public class BlackMarketItemLogic : MonoBehaviour
{
    public UISprite m_ItemIconSprite; 
    public UISprite m_ItemQualitySprite;
    public UILabel m_ItemNameLable;
    public UILabel m_ItemCountLable;
    public UISprite m_MoneySprite;
    public UILabel  m_MoneyNumLable;
    private BlackMarketItemInfo m_GoodInfo;
	// Use this for initialization
    public void InitItemInfo(BlackMarketItemInfo GoodInfo)
    {
        //初始化货物信息
        m_GoodInfo.CleanUp();
        m_GoodInfo =GoodInfo;
        m_ItemIconSprite.spriteName = m_GoodInfo.ItemInfo.GetIcon();
        m_ItemIconSprite.MakePixelPerfect();
        m_ItemQualitySprite.spriteName = m_GoodInfo.ItemInfo.GetQualityFrame();
        m_ItemQualitySprite.MakePixelPerfect();
        m_ItemNameLable.text =m_GoodInfo.ItemInfo.GetName();
        m_ItemCountLable.text =m_GoodInfo.ItemCount.ToString();
        if (m_GoodInfo.ItemMoneyType == (int)MONEYTYPE.MONEYTYPE_COIN)
        {
            m_MoneySprite.spriteName = "qian1";
            m_MoneySprite.MakePixelPerfect();
        }
        else if (m_GoodInfo.ItemMoneyType == (int)MONEYTYPE.MONEYTYPE_YUANBAO)
        {
            m_MoneySprite.spriteName = "qian2";
            m_MoneySprite.MakePixelPerfect();
        }
        else if (m_GoodInfo.ItemMoneyType == (int)MONEYTYPE.MONEYTYPE_YUANBAO_BIND)
        {
            m_MoneySprite.spriteName = "qian3";
            m_MoneySprite.MakePixelPerfect();
        }
        m_MoneyNumLable.text =m_GoodInfo.ItemPrice.ToString();
        gameObject.SetActive(true);
    }
	void Start () 
    {
	
	}

    void ClickBuyBt()
    {
        //发包购买
        if (BlackMarketLogic.Instance() !=null)
        {
            CG_BUY_BLACKMARKETITEM buyPak = (CG_BUY_BLACKMARKETITEM)PacketDistributed.CreatePacket(MessageID.PACKET_CG_BUY_BLACKMARKETITEM);
            buyPak.Curpage = BlackMarketLogic.Instance().CurPage;
			buyPak.ItemIndex =(uint)m_GoodInfo.ItemIndex;
            buyPak.SendPacket();
        }
    }
    void SlotOnClick()
    {
        if (m_GoodInfo.ItemInfo.IsEquipMent())
        {
            EquipTooltipsLogic.ShowEquipTooltip(m_GoodInfo.ItemInfo, EquipTooltipsLogic.ShowType.InfoCompare);
        }
        else
        {
            ItemTooltipsLogic.ShowItemTooltip(m_GoodInfo.ItemInfo, ItemTooltipsLogic.ShowType.Info);
        }
    }
	
}
