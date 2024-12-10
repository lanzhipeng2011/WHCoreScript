/********************************************************************************
 *	文件名：	MasterShopItemLogic.cs
 *	全路径：	Script\GUI\MasterShopItemLogic.cs
 *
 *	功能说明：帮会商店商品逻辑类
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using GCGame.Table;
using Games.Item;
using Games.GlobeDefine;

public class MasterShopItemLogic : MonoBehaviour
{
    public ItemSlotLogic m_ItemSlot;        //物品逻辑
    public UILabel m_NumContent;            //物品数量
    public UILabel m_PriceContent;          //价格
    public UILabel m_NameLabel;             //商品名称
    public UISprite m_ItemIcon;             //商品图标

    public UIImageButton m_BuyItemBtn;     //购买按钮

    //private int m_GoodsId;   // 商品ID 表格第一列
    private int m_ItemID;    // 物品ID 表格第五列
    private string m_Name = "";

    private int m_nShopItemChangeMin = 1;    // 一次兑换商品最小数量
    private int m_nShopItemChangeMax = 99;   // 一次兑换商品最大数量
    
    private int m_nPrice;
    private int m_nConsumItemID;

    private bool m_nIsItemCanBuy = true;
    
    public void Init(Tab_MasterShop tabMasterShop)
    {
        if (null == tabMasterShop)
        {
            return;
        }

        if (GameManager.gameManager.PlayerDataPool.IsHaveMaster() == false ||
            GameManager.gameManager.PlayerDataPool.IsMasterReserveMember() == true)
        {
            m_BuyItemBtn.disabledSprite = "ui_pub_026";
			m_BuyItemBtn.hoverSprite = "ui_pub_026";
			m_BuyItemBtn.normalSprite = "ui_pub_026";
			m_BuyItemBtn.pressedSprite = "ui_pub_026";

            m_BuyItemBtn.enabled = false;
            m_BuyItemBtn.enabled = true;               //  先置false再置true是为了刷新sprite

            m_nIsItemCanBuy = false;
        }
        else
        {
             m_nIsItemCanBuy = true;
        }

        m_ItemID = tabMasterShop.ItemID;
        m_nPrice = tabMasterShop.Price;
        m_nConsumItemID = tabMasterShop.ConsumItemID;
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
            m_PriceContent.text = tabMasterShop.Price.ToString();
        }

        m_nShopItemChangeMax = tabMasterShop.MaxChangeNum;
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
        if (!m_nIsItemCanBuy)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{3194}");
            return;
        }

        NumChooseController.OpenWindow(m_nShopItemChangeMin, m_nShopItemChangeMax, 
            StrDictionary.GetClientDictionaryString("#{2837}"), MsgBoxBuyGuildItemOK, 1);
    }

    void MsgBoxBuyGuildItemOK(int nCurNum)
    {
        //nCurNum数量判断
        if (nCurNum < m_nShopItemChangeMin || nCurNum > m_nShopItemChangeMax)
        {
            return;
        }

        //判断
        if (GameManager.gameManager.PlayerDataPool.MasterInfo.IsValid() == false &&
            GameManager.gameManager.PlayerDataPool.MasterInfo.IsMasterChief() == false )
        {
            return;
        }

        //背包判断
        if (GameManager.gameManager.PlayerDataPool.BackPack.GetCanContainerSize() <= 0)
        {
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{1903}");
            return;
        }

        //判断是否够
        int nRealCost = m_nPrice*nCurNum;
        if (nRealCost > 0)
        {
            int QingYiItemNum = GameManager.gameManager.PlayerDataPool.BackPack.GetItemCountByDataId(m_nConsumItemID);
            if (nRealCost > QingYiItemNum)
            {
                //不够，返回
                Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{3149}");
                return;
            }
        }
        else
        {
            //可能溢出，直接返回
            return;
        }

        //发送购买消息包
        CG_MASTERSHOP_BUY msg = (CG_MASTERSHOP_BUY)PacketDistributed.CreatePacket(MessageID.PACKET_CG_MASTERSHOP_BUY);
        msg.GoodID = m_ItemID;
        msg.BuyNum = nCurNum;
        msg.SendPacket();
    }
}
