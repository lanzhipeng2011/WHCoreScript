using UnityEngine;
using System.Collections;
using GCGame;
using Games.SwordsMan;
using GCGame.Table;
using Module.Log;

public class SwordsManShopItem : MonoBehaviour {

    public UILabel m_LableName;
    public UILabel m_LabelDesc;
    public UILabel m_LabelPrice;
    public GameObject m_HightLightBkSprite;
    public UISprite m_IconSprite;
    public UISprite m_QualitySprite;
    public int Price
    {
        get { return m_nPrice; }
    }
    private int m_nPrice;
    private string m_strName = "";

    private SwordsManShop m_Parent; 
    private int m_SwordsManDataID;
    public int SwordsManDataID
    {
        get { return m_SwordsManDataID; }
    }
    public static SwordsManShopItem CreateItem(GameObject grid, GameObject resItem,SwordsManShop parent)
    {
        GameObject curItem = Utils.BindObjToParent(resItem, grid);
        if (null == curItem)
        {
            return null;
        }       
        SwordsManShopItem curItemComponent = curItem.GetComponent<SwordsManShopItem>();
        if (null == curItemComponent)
        {
            return null;
        }
        curItemComponent.SetParent(parent);
        return curItemComponent;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parent"></param>
    public void SetParent(SwordsManShop parent)
    {
        m_Parent = parent;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ShopSwordsManTable"></param>
    public void SetShopSwordsMan(Tab_SwordsManScoreShop ShopSwordsManTable)
    {
        if (null == ShopSwordsManTable)
        {
            LogModule.ErrorLog("SetShopSwordsManShop::ShopSwordsManTable is null");
            return;
        }
        Tab_SwordsManAttr SwordsManAttrTable = TableManager.GetSwordsManAttrByID(ShopSwordsManTable.Id, 0);
        if (null == SwordsManAttrTable)
        {
            LogModule.ErrorLog("m_SwordsManAttrTable::SwordsManAttrTable is null");
            return;
        }
        m_SwordsManDataID = SwordsManAttrTable.Id;
        m_nPrice = ShopSwordsManTable.ScorePrice;
        m_strName = SwordsManAttrTable.Name;

        if (m_LableName != null)
        {
            m_LableName.text = SwordsManAttrTable.Name;
        }
        if (m_LabelDesc != null)
        {
            m_LabelDesc.text = SwordsManAttrTable.Tips;
        }
        if (m_LabelPrice != null)
        {
            if (GameManager.gameManager.PlayerDataPool.SwordsManScore >= m_nPrice)
            {
                m_LabelPrice.text = ShopSwordsManTable.ScorePrice.ToString();
            }
            else
            {
                m_LabelPrice.text = "[ff0000]" + ShopSwordsManTable.ScorePrice.ToString();
            }
        }
        if (m_IconSprite != null)
        {
            m_IconSprite.spriteName = SwordsManAttrTable.Icon;
        }  
        if (m_QualitySprite!= null)
        {
            m_QualitySprite.spriteName = SwordsMan.GetQualitySpriteName((SwordsMan.SWORDSMANQUALITY)SwordsManAttrTable.Quality);
        }
        if (m_HightLightBkSprite != null)
        {
            m_HightLightBkSprite.SetActive(false);
        }
    }

    void OnClickShopItem()
    {
        if (null == m_Parent)
        {
            LogModule.ErrorLog("OnClickShopItem::m_Parent null");
            return;
        }
        Tab_SwordsManAttr SwordsManAttrTable = TableManager.GetSwordsManAttrByID(m_SwordsManDataID, 0);
        if (null == SwordsManAttrTable)
        {
            LogModule.ErrorLog("OnClickShopItem::SwordsManAttrTable is null");
            return;
        }
        SwordsMan oSwordsMan = new SwordsMan();
        oSwordsMan.DataId = m_SwordsManDataID;
        oSwordsMan.Name = SwordsManAttrTable.Name;
        oSwordsMan.Quality = SwordsManAttrTable.Quality;
        oSwordsMan.Level = 1;
        SwordsManToolTipsLogic.ShowSwordsManTooltip(oSwordsMan, SwordsManToolTipsLogic.SwordsMan_ShowType.ScoreShop);
        m_Parent.OnShopSwordsManClick(this);
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnSelectShopItem()
    {
        if (m_HightLightBkSprite != null)
        {
            m_HightLightBkSprite.SetActive(true);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnCancelSelectShopItem()
    {
        if (m_HightLightBkSprite != null)
        {
            m_HightLightBkSprite.SetActive(false);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnClickExchange()
    {
        int nSize = GameManager.gameManager.PlayerDataPool.SwordsManBackPack.GetEmptyContainerSize();
        if (nSize <= 0)
        {
            MessageBoxLogic.OpenOKBox(2488, 1000);
            return;
        }

        //string str = "确定要花费"+m_nPrice.ToString()+"积分对话侠客"+m_strName;
        string str = StrDictionary.GetClientDictionaryString("#{2656}", m_nPrice, m_strName);
        MessageBoxLogic.OpenOKCancelBox(str, "", ExchangeSwordsmanOK);
    }

    void ExchangeSwordsmanOK()
    {
        CG_BUY_SWORDSMAN packet = (CG_BUY_SWORDSMAN)PacketDistributed.CreatePacket(MessageID.PACKET_CG_BUY_SWORDSMAN);
        packet.Swordsmanid = SwordsManDataID;
        packet.SendPacket();
    }

}
