/********************************************************************************
 *	文件名：	GuildShopItemLogic.cs
 *	全路径：	Script\GUI\GuildShopItemLogic.cs
 *	创建人：	李嘉
 *	创建时间：2014-06-19
 *
 *	功能说明：帮会商店商品逻辑类
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using GCGame.Table;
using Games.Item;
using Games.GlobeDefine;

public class GuildShopItemLogic : MonoBehaviour
{
    public ItemSlotLogic m_ItemSlot;        //物品逻辑
    public UILabel m_PriceContent;          //价格
    public UILabel m_NameLabel;             //商品名称
    public UISprite m_ItemIcon;             //商品图标
    public UIImageButton m_BuyItemBtn;      //购买按钮

    //private int m_GoodsId;   // 商品ID 表格第一列
    private int m_ItemID;    // 物品ID 表格第五列
    private string m_Name = "";

    private int m_nShopItemChangeMin = 1;    // 一次兑换商品最小数量
    private int m_nShopItemChangeMax = 99;   // 一次兑换商品最大数量
    
    private int m_nPrice;

    private bool m_nIsItemCanBuy = true;
    
    public void Init(Tab_GuildShop tabGuildShop)
    {
        if (null == tabGuildShop)
        {
            return;
        }

        if (!GameManager.gameManager.PlayerDataPool.IsHaveGuild() ||
         GameManager.gameManager.PlayerDataPool.IsReserveGuildMember())
        {
            m_BuyItemBtn.gameObject.SetActive(false);               //  先置false再置true是为了刷新sprite
            m_nIsItemCanBuy = false;
        }
        else
        {
            m_nIsItemCanBuy = true;
        }

        //m_GoodsId = tabGuildShop.Id;
        m_ItemID = tabGuildShop.ItemID;
        m_nPrice = tabGuildShop.Price;
        Tab_CommonItem tabCommonItem = TableManager.GetCommonItemByID(m_ItemID, 0);
        if (tabCommonItem == null)
        {
            return;
        }
        m_Name = tabCommonItem.Name;

        m_ItemSlot.InitInfo(ItemSlotLogic.SLOT_TYPE.TYPE_ITEM, m_ItemID, GoodsOnClick);

        if (null != m_NameLabel)
        {
            m_NameLabel.text = m_Name;
        }
        if (null != m_PriceContent)
        {
            m_PriceContent.text = tabGuildShop.Price.ToString();
        }

        m_nShopItemChangeMax = tabGuildShop.MaxChangeNum;
    }

    public void InitGuildMakeGoods(Tab_GuildMake tabMake)
    {
        if (null == tabMake)
        {
            return;
        }

        if (!GameManager.gameManager.PlayerDataPool.IsHaveGuild() ||
         GameManager.gameManager.PlayerDataPool.IsReserveGuildMember())
        {
            m_BuyItemBtn.gameObject.SetActive(false);
            m_nIsItemCanBuy = false;
        }
        else
        {
            m_nIsItemCanBuy = true;
        }

        //m_GoodsId = tabGuildShop.Id;
        m_ItemID = tabMake.CommonItemId;

        Tab_GuildShop tabShop = null;
        for (int i = 1; i <= TableManager.GetGuildShop().Count; ++i)
        {
            Tab_GuildShop tabGuildShop = TableManager.GetGuildShopByID(i, 0);
            if (tabGuildShop == null)
            {
                continue;
            }
            if (tabGuildShop.ItemID == m_ItemID)
            {
                tabShop = tabGuildShop;
                break;
            }
        }
        if (null == tabShop)
        {
            return;
        }

        m_nPrice = tabShop.Price;

        Tab_CommonItem tabCommonItem = TableManager.GetCommonItemByID(m_ItemID, 0);
        if (tabCommonItem == null)
        {
            return;
        }
        m_Name = tabCommonItem.Name;

        int itemCount = GameManager.gameManager.PlayerDataPool.GuildPack.GetItemCountByDataId(m_ItemID);

        m_ItemSlot.InitInfo(ItemSlotLogic.SLOT_TYPE.TYPE_ITEM, m_ItemID, GoodsOnClick, itemCount.ToString(), true);

        if (null != m_NameLabel)
        {
            m_NameLabel.text = m_Name;
        }
        if (null != m_PriceContent)
        {
            m_PriceContent.text = m_nPrice.ToString();
        }

        m_nShopItemChangeMax = itemCount > tabShop.MaxChangeNum ? itemCount : tabShop.MaxChangeNum ;

        if (itemCount <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }

    void GoodsOnClick(int nItemID, ItemSlotLogic.SLOT_TYPE eSlotType, string strSlotName)
    {
        GameItem item = new GameItem();
        item.DataID = nItemID;
        if (item.IsEquipMent())
        {
            EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Info);
        }
        else
        {
            ItemTooltipsLogic.ShowItemTooltip(item, ItemTooltipsLogic.ShowType.Info);
        }
    }

    void Buy()
    {
        //NumChooseController.OpenWindow(m_nShopItemChangeMin, m_nShopItemChangeMax, "购买", MsgBoxBuyGuildItemOK,1);
        if(!m_nIsItemCanBuy)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{3193}");
            return;
        }
        NumChooseController.OpenWindow(m_nShopItemChangeMin, m_nShopItemChangeMax, StrDictionary.GetClientDictionaryString("#{2837}"), MsgBoxBuyGuildItemOK, 1);
    }

    void MsgBoxBuyGuildItemOK(int nCurNum)
    {
        //nCurNum数量判断
        if (nCurNum < m_nShopItemChangeMin || nCurNum > m_nShopItemChangeMax)
        {
            return;
        }

        //判断帮会GUID
        if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid == GlobeVar.INVALID_GUID)
        {
            return;
        }

        //背包判断
        if (GameManager.gameManager.PlayerDataPool.BackPack.GetCanContainerSize() <= 0)
        {
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{1903}");
            return;
        }

        //判断帮贡是否够
        int nRealCost = m_nPrice*nCurNum;
        if (nRealCost > 0)
        {
            int nContribute = GameManager.gameManager.PlayerDataPool.GuildInfo.GetMemberContribute(Singleton<ObjManager>.GetInstance().MainPlayer.GUID);
            if (nRealCost > nContribute)
            {
                //帮贡不够，返回
                Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2465}");
                return;
            }
        }
        else
        {
            //可能溢出，直接返回
            return;
        }

        //发送购买消息包
        CG_BUY_GUILDGOODS msg = (CG_BUY_GUILDGOODS)PacketDistributed.CreatePacket(MessageID.PACKET_CG_BUY_GUILDGOODS);
        msg.GoodID = m_ItemID;
		msg.BuyNum =(uint)nCurNum;
        msg.SendPacket();
    }
}
