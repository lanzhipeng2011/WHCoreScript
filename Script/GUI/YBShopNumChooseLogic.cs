using UnityEngine;
using System.Collections;
using GCGame.Table;
using Games.Item;
using System.Text.RegularExpressions;

public class YBShopNumChooseLogic : MonoBehaviour {

    public ItemSlotLogic m_ItemSlot;
    //public UILabel m_NumLabel;
    public UIInput m_NumInput;
    public UILabel m_SumPriceLabel;
    public UILabel m_ItemNameLabel;

    private int m_CurNum = 1;
    private int m_SumPrice = 0;

    private int m_GoodsId;
    private ItemSlotLogic.SLOT_TYPE m_eSlotType;
    private int m_ItemID;
    //private int m_GoodsNum;
    private int m_ItemPrice = 0;
    private bool m_bChooseBind = true;
    private CG_BUY_YUANBAOGOODS.DEADLINE_TYPE m_eDeadlineType;

	// Use this for initialization
	void Start () {
	
	}

    public void InitInfo(int nGoodsId, ItemSlotLogic.SLOT_TYPE eSlotType, int nItemID, int nGoodsNum, int nPrice, bool bBind, 
        CG_BUY_YUANBAOGOODS.DEADLINE_TYPE eDeadlineType, string strItemName)
    {
        m_CurNum = 1;
        m_NumInput.value = m_CurNum.ToString();
        m_ItemNameLabel.text = strItemName;

        m_GoodsId = nGoodsId;
        m_eSlotType = eSlotType;
        m_ItemID = nItemID;
        //m_GoodsNum = nGoodsNum;
        m_ItemSlot.InitInfo(eSlotType, nItemID, GoodsOnClick);
        m_ItemPrice = nPrice;
        m_bChooseBind = bBind;
        m_eDeadlineType = eDeadlineType;
        UpdateSumPrice();
    }

    void GoodsOnClick(int nItemID, ItemSlotLogic.SLOT_TYPE eSlotType, string strSlotName)
    {
        if (eSlotType == ItemSlotLogic.SLOT_TYPE.TYPE_ITEM)
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
    }

    void AddNum()
    {
        if (m_eSlotType != ItemSlotLogic.SLOT_TYPE.TYPE_FASHION)
        {
            if (false == CheckString(m_NumInput.value) 
                || m_NumInput.value == string.Empty
                || System.Convert.ToInt32(m_NumInput.value) >= 999
                || System.Convert.ToInt32(m_NumInput.value) < 1)
            {
                m_CurNum = 1;
            }
            else
            {
                m_CurNum += 1;
            }
        }

        m_NumInput.value = m_CurNum.ToString();
        UpdateSumPrice();
    }

    void SubNum()
    {
        if (m_eSlotType != ItemSlotLogic.SLOT_TYPE.TYPE_FASHION)
        {
            if (false == CheckString(m_NumInput.value) 
                || m_NumInput.value == string.Empty 
                || System.Convert.ToInt32(m_NumInput.value) > 999
                || System.Convert.ToInt32(m_NumInput.value) <= 1)
            {
                m_CurNum = 999;
            }
            else
            {

                m_CurNum -= 1;
            }
        }

        m_NumInput.value = m_CurNum.ToString();
        UpdateSumPrice();
    }

    public void NumChooseSubmit()
    {
        bool bCanParse = int.TryParse(m_NumInput.value, out m_CurNum);
        if (!bCanParse)
        {
            return;
        }

        UpdateSumPrice();
    }

    void ChargeOnClick()
    {
        DoPay();
    }

    void BuyOnClick()
    {

		int curNum = 0;
		
		bool bCanParse = int.TryParse(m_NumInput.value, out curNum);
		if (curNum <= 0)
		{
			return;          
		}

        if (m_bChooseBind)
        {
            int nPlayerYuanBaoBind = GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBaoBind();
            int nPlayerYuanBao = GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBao();

            if (nPlayerYuanBaoBind < m_SumPrice)
            {
                // 元宝补充
                int nRepairYuanBao = m_SumPrice - nPlayerYuanBaoBind;
                if (nRepairYuanBao <= nPlayerYuanBao)
                {
                    MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{1849}", nRepairYuanBao), "", RepairYuanBaoOK, RepairYuanBaoCancel);
                }
                else
                {
                    // 元宝不足
                    MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{1848}"), "", BuyChargeOK, BuyChargeCancel);               
                }
            }
            else
            {
                SendBuyGoodsPacket();

                CloseWindow();
            }           
        }
        else
        {
            int nPlayerYuanBao = GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBao();
            
            if (nPlayerYuanBao < m_SumPrice)
            {
                // 元宝不足
                MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{1848}"), "", BuyChargeOK, BuyChargeCancel);
            }
            else
            {
                SendBuyGoodsPacket();

                CloseWindow();               
            }
        }
    }

    void BuyChargeOK()
    {
        DoPay();
    }

    void DoPay()
    {
        RechargeData.PayUI();
    }

    void BuyChargeCancel()
    {
        
    }

    void RepairYuanBaoOK()
    {
        SendBuyGoodsPacket();

        CloseWindow();
    }

    void RepairYuanBaoCancel()
    {

    }

    void UpdateSumPrice()
    {
        m_SumPrice = m_CurNum * m_ItemPrice;
        string dicStr = StrDictionary.GetClientDictionaryString("#{1850}", m_SumPrice);
        m_SumPriceLabel.text = dicStr;
    }

    void SendBuyGoodsPacket()
    {
        int curNum = 0;

        bool bCanParse = int.TryParse(m_NumInput.value, out curNum);
		if (curNum <= 0)
        {
            return;          
        }

        //PlayGoodsSoundEffect();

        CG_BUY_YUANBAOGOODS packet = (CG_BUY_YUANBAOGOODS)PacketDistributed.CreatePacket(MessageID.PACKET_CG_BUY_YUANBAOGOODS);
        packet.GoodID = m_GoodsId;
		packet.BuyNum = (uint)curNum;
		packet.IsUseBind = (uint)(m_bChooseBind ? 1 : 0);
		packet.Deadline = (uint)m_eDeadlineType;
        packet.SendPacket();

        PlatformHelper.OnPurchase(m_GoodsId.ToString(),curNum,m_ItemPrice);
    }

    void CloseWindow()
    {
        gameObject.SetActive(false);

        m_CurNum = 1;
        m_NumInput.value = "1";
        m_SumPrice = 0;
    }

    void PlayGoodsSoundEffect()
    {
        if (m_eSlotType == ItemSlotLogic.SLOT_TYPE.TYPE_ITEM)
        {
            GameItem item = new GameItem();
            item.DataID = m_ItemID;

            if (item.IsGem() || item.IsEnchanceExpItem() || item.IsStarStone())
            {
                GameManager.gameManager.SoundManager.PlaySoundEffect(110);  //buy_gem
            }
            else if (item.IsLivingSkillDrawing() || item.IsFellowSkillBook())
            {
                GameManager.gameManager.SoundManager.PlaySoundEffect(111);     //buy_paper
            }
            else
            {
                GameManager.gameManager.SoundManager.PlaySoundEffect(112);  //buy_drug
            }            
        }
        else
        {
            GameManager.gameManager.SoundManager.PlaySoundEffect(112);      //buy_drug
        }
    }

    bool CheckString(string str)
    {
        Regex reg = new Regex(@"\D");
        return !reg.IsMatch(str);
    }
}
