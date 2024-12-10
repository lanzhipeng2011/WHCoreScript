using UnityEngine;
using System.Collections;
using GCGame.Table;
using GCGame;
using Module.Log;
using Games.GlobeDefine;
using System.Collections.Generic;

public class LivingSkillLogic : UIControllerBase<LivingSkillLogic> {

    public enum FORMULA_TYPE
    {
        TYPE_DAZAO = 0,
        TYPE_ZHIYAO = 1,
    }

    public TabController m_SkillTab;
    public GameObject m_FormulaGrid;
    public GameObject m_StuffGrid;
    public UILabel m_MakeButtonLabel;
    public UILabel m_PayMoneyLabel;
    public UILabel m_PayStaminaLabel;
    public GameObject m_StuffCollectInfo;
    public UILabel m_StuffCollectLabel;
    public UILabel m_StaminaContentLabel;
    public UILabel m_CountDownLabel;
    public GameObject m_StaminaTips;

    private GameObject m_FormulaItem = null;
    private GameObject m_StuffItem = null;
    private int m_CurFormulaID = GlobeVar.INVALID_ID;

    void Awake()
    {
        SetInstance(this);
    }
	// Use this for initialization
	void Start () {
        m_SkillTab.delTabChanged = OnSkillTabClick;
        UIManager.LoadItem(UIInfo.LivingSkillFormulaItem, LoadFormulaItemOver);
        UIManager.LoadItem(UIInfo.LivingSkillStuffItem, LoadStuffItemOver);
        m_StuffCollectInfo.SetActive(false);
        UpdatePlayerStamina();
        UpdateCountDownLabel();
	}

    void OnDestroy()
    {
        SetInstance(null);
    }

    void LoadFormulaItemOver(GameObject resItem, object param)
    {
        if (null == resItem)
        {
            LogModule.ErrorLog("load LivingSkillFormulaItem error");
            return;
        }

        m_FormulaItem = resItem;

        if (m_FormulaItem != null && m_StuffItem != null)
        {
            m_MakeButtonLabel.text = StrDictionary.GetClientDictionaryString("#{1891}");
            UpdateLivingSkillInfo(FORMULA_TYPE.TYPE_DAZAO);
        }
    }

    void LoadStuffItemOver(GameObject resItem, object param)
    {
        if (null == resItem)
        {
            LogModule.ErrorLog("load YuanBaoShopItem error");
            return;
        }

        m_StuffItem = resItem;

        if (m_FormulaItem != null && m_StuffItem != null)
        {
            m_MakeButtonLabel.text = StrDictionary.GetClientDictionaryString("#{1891}");
            UpdateLivingSkillInfo(FORMULA_TYPE.TYPE_DAZAO);
        }
    }

    void OnSkillTabClick(TabButton curButton)
    {
        if (curButton.name == "DazaoButton")
        {
            m_MakeButtonLabel.text = StrDictionary.GetClientDictionaryString("#{1891}");
            UpdateLivingSkillInfo(FORMULA_TYPE.TYPE_DAZAO);
        }
        else if (curButton.name == "ZhiyaoButton")
        {
            m_MakeButtonLabel.text = StrDictionary.GetClientDictionaryString("#{1892}");
            UpdateLivingSkillInfo(FORMULA_TYPE.TYPE_ZHIYAO);
        }
    }

    void UpdateLivingSkillInfo(FORMULA_TYPE eType)
    {
        Utils.CleanGrid(m_FormulaGrid);
        Utils.CleanGrid(m_StuffGrid);

        bool bFirstFormula = true;
        int index = 0;
        foreach(KeyValuePair<int, List<Tab_LivingSkill>> pair in TableManager.GetLivingSkill())
        {
            Tab_LivingSkill tabLivingSkill = pair.Value[0];
            if (tabLivingSkill == null)
            {
                continue;
            }

            if (tabLivingSkill.Type == (int)eType)
            {
                // 加载对应分页商品
                string itemName = index < 10 ? ("0" + index.ToString()) : (index.ToString());
                GameObject FormulaItem = Utils.BindObjToParent(m_FormulaItem, m_FormulaGrid, itemName);
                if (FormulaItem == null)
                {
                    continue;
                }

                if (null != FormulaItem.GetComponent<LivingSkillFormulaLogic>())
                {
                    FormulaItem.GetComponent<LivingSkillFormulaLogic>().InitInfo(tabLivingSkill.Id);

                    if (bFirstFormula)
                    {
                        FormulaItem.GetComponent<LivingSkillFormulaLogic>().ChooseFormula();
                        bFirstFormula = false;
                    }
                }

                index++;
            }
        }

        m_FormulaGrid.GetComponent<UIGrid>().Reposition();
        m_FormulaGrid.GetComponent<UITopGrid>().Recenter(true);
    }

    public void OnFormulaChoose(int nFormulaID)
    {
        Tab_LivingSkill tabLivingSkill = TableManager.GetLivingSkillByID(nFormulaID, 0);
        if (tabLivingSkill == null)
        {
            return;
        }

        Utils.CleanGrid(m_StuffGrid);

        for (int i = 0; i < tabLivingSkill.getStuffIDCount(); i++ )
        {
            int nStuffID = tabLivingSkill.GetStuffIDbyIndex(i);
            if (nStuffID != GlobeVar.INVALID_ID)
            {
                GameObject StuffItem = Utils.BindObjToParent(m_StuffItem, m_StuffGrid, i.ToString());
                if (StuffItem != null)
                {
                    StuffItem.GetComponent<LivingSkillStuffLogic>().InitInfo(tabLivingSkill.GetStuffIDbyIndex(i), tabLivingSkill.GetStuffCountbyIndex(i));
                }
            }
        }

        m_StuffGrid.GetComponent<UIGrid>().Reposition();
        m_StuffGrid.GetComponent<UITopGrid>().Recenter(true);

        m_CurFormulaID = nFormulaID;

        m_PayMoneyLabel.text =  tabLivingSkill.Money.ToString();
        m_PayStaminaLabel.text =  tabLivingSkill.Stamina.ToString();

        foreach (LivingSkillFormulaLogic formula in m_FormulaGrid.GetComponentsInChildren<LivingSkillFormulaLogic>())
        {
            if (formula.FormulaID != nFormulaID)
            {
                formula.ChooseCancel();
            }            
        }
    }

    void CloseWindow()
    {
        UIManager.CloseUI(UIInfo.LivingSkill);
    }

    void MakeItem()
    {
        Tab_LivingSkill tabLivingSkill = TableManager.GetLivingSkillByID(m_CurFormulaID, 0);
        if (tabLivingSkill == null)
        {
            return;
        }

        if (Singleton<ObjManager>.Instance.MainPlayer.BaseAttr.Level < tabLivingSkill.OpenLevel)
        {
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false,"#{1900}",tabLivingSkill.OpenLevel);
            return;
        }

        bool isBind = false;
        for (int i = 0; i < tabLivingSkill.getStuffIDCount(); i++ )
        {
            int nStuffID = tabLivingSkill.GetStuffIDbyIndex(i);
            int nStuffCount = tabLivingSkill.GetStuffCountbyIndex(i);
            if (nStuffID != GlobeVar.INVALID_ID)
            {
                if (GameManager.gameManager.PlayerDataPool.BackPack.GetItemCountByDataId(nStuffID) < nStuffCount)
                {
                    Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{1901}");
                    return;
                }    
                if (!isBind)
                {
                    if (GameManager.gameManager.PlayerDataPool.BackPack.GetBindItemCountByDataId(nStuffID) > 0)
                    {
                        isBind = true;
                    }                    
                }                
            }
        }

        if (Singleton<ObjManager>.Instance.MainPlayer.BaseAttr.CurStamina < tabLivingSkill.Stamina)
        {
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2139}");
            return;
        }

        if (GameManager.gameManager.PlayerDataPool.BackPack.GetCanContainerSize() <= 0)
        {
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{1903}");
            return;
        }

        if (GameManager.gameManager.PlayerDataPool.Money.GetMoney_Coin() < tabLivingSkill.Money)
        {
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{1902}");
            return;
        }

        if (isBind)
        {
            MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{2659}"), "", MakeItemOK, MakeItemCancel);
        }
        else
        {
            MakeItemOK();
        }
    }

    void MakeItemOK()
    {
        CG_USE_LIVINGSKILL packet = (CG_USE_LIVINGSKILL)PacketDistributed.CreatePacket(MessageID.PACKET_CG_USE_LIVINGSKILL);
        packet.FormulaID = m_CurFormulaID;
        packet.SendPacket();

        //停掉挂机
        if (Singleton<ObjManager>.Instance.MainPlayer)
		{


						Singleton<ObjManager>.Instance.MainPlayer.LeveAutoCombat ();
						if (SGAutoFightBtn.Instance != null) {
								SGAutoFightBtn.Instance.UpdateAutoFightBtnState ();
						}
		}
    }

    void MakeItemCancel(){}

    void BuyItem()
    {
        UIManager.ShowUI(UIInfo.ConsignSaleRoot, BuyItemOpenConsignSale);
    }

    void BuyItemOpenConsignSale(bool bSuccess, object param)
    {
        if (bSuccess)
        {
            Tab_LivingSkill tabLivingSkill = TableManager.GetLivingSkillByID(m_CurFormulaID, 0);
            if (tabLivingSkill == null)
            {
                return;
            }

            if (ConsignSaleLogic.Instance() != null)
            {
                ConsignSaleLogic.Instance().SearchForAskBuy(tabLivingSkill.Name);
            }            
        }
    }

    public void HandleUpdateItem()
    {
        // 刷新当前配方界面信息
        foreach (LivingSkillStuffLogic stuff in m_StuffGrid.GetComponentsInChildren<LivingSkillStuffLogic>())
        {
            stuff.HandleUpdateItem();
        }
    }

    public void ShowStuffCollectInfo(int nStuffDataID)
    {
        m_StuffCollectInfo.SetActive(true);

        Tab_LivingSkillStuff tabLivingSkillStuff = TableManager.GetLivingSkillStuffByID(nStuffDataID, 0);
        if (tabLivingSkillStuff == null)
        {
            m_StuffCollectLabel.text = "";
            return;
        }

        m_StuffCollectLabel.text = tabLivingSkillStuff.CollectInfo.Replace("#r", "\n"); ;
    }

    void CloseStuffCollect()
    {
        m_StuffCollectInfo.SetActive(false);
    }

    public void UpdatePlayerStamina()
    {
        if (Singleton<ObjManager>.Instance.MainPlayer != null)
        {
            int nCurStamina = Singleton<ObjManager>.Instance.MainPlayer.BaseAttr.CurStamina;
            int nStaminaFull = Singleton<ObjManager>.Instance.MainPlayer.GetStaminaFull();
            m_StaminaContentLabel.text = StrDictionary.GetClientDictionaryString("#{2122}", nCurStamina, nStaminaFull);                
        }
    }

    void BuyStamina()
    {
        int nPlayerVIPLevel = VipData.GetVipLv();
        Tab_StaminaBuyRule tabRightRule = null;
        foreach (KeyValuePair<int, List<Tab_StaminaBuyRule>> pair in TableManager.GetStaminaBuyRule())
        {
            Tab_StaminaBuyRule tabBuyRule = pair.Value[0];
            if (tabBuyRule == null)
            {
                continue;
            }

            if (tabBuyRule.VIPRequire <= nPlayerVIPLevel)
            {
                tabRightRule = tabBuyRule;
            }
        }

        if (tabRightRule == null)
        {
            return;
        }

        // 超出当日购买次数上限
        int nTodayBuyNum = GameManager.gameManager.PlayerDataPool.CommonData.GetCommonData((int)Games.UserCommonData.USER_COMMONDATA.CD_STAMINA_BUYNUM);
        if (nTodayBuyNum >= tabRightRule.BuyNumMax || nTodayBuyNum < 0)
        {
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2131}");
            return;
        }

        string strMessage = StrDictionary.GetClientDictionaryString("#{2123}", nTodayBuyNum + 1, GlobeVar.STAMINA_BUYVALUE, tabRightRule.Price);
        MessageBoxLogic.OpenOKCancelBox(strMessage, "", BuyStaminaOK, BuyStaminaCancel);
    }

    void BuyStaminaOK()
    {
        CG_BUY_STAMINA packet = (CG_BUY_STAMINA)PacketDistributed.CreatePacket(MessageID.PACKET_CG_BUY_STAMINA);
        packet.NoParam = 1;
        packet.SendPacket();
    }

    void BuyStaminaCancel(){}

    public void UpdateCountDownLabel()
    {
        int nRemainTime = GameManager.gameManager.PlayerDataPool.StaminaCountDown;
        if (nRemainTime >= 0)
        {
			int nhouse=nRemainTime/3600;
			nRemainTime=nRemainTime-3600*nhouse;
            int nMinutes = nRemainTime / 60;
            int nSeconds = nRemainTime % 60;
            string strSeconds = nSeconds < 10 ? ("0" + nSeconds.ToString()) : (nSeconds.ToString());
            m_CountDownLabel.text = StrDictionary.GetClientDictionaryString("#{2124}", nhouse,nMinutes, strSeconds);
        }
        else
        {
            m_CountDownLabel.text = "";
        }
    }

    void ShowStaminaTips()
    {
        m_StaminaTips.SetActive(true);
    }

    void HideStaminaTips()
    {
        m_StaminaTips.SetActive(false);
    }
}
