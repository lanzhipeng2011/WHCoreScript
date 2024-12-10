using UnityEngine;
using System.Collections;
using GCGame.Table;
using Games.Item;
using GCGame;
using Games.GlobeDefine;
using Games.LogicObj;

public class CompareEquipTooltipsLogic : MonoBehaviour {

    public UILabel m_CompareNameLabel;
    public UILabel m_CompareProLabel;
    public UILabel m_CompareLevelLabel;
    public UILabel m_CompareBindLabel;
    public TooltipsEquipStar m_CompareStarLevel;
    public UILabel m_CompareStrengthenLevelLabel;
    public UILabel m_ComparePowerLabel;
    public TooltipsEquipAttr m_CompareAttrInfo;
    public TooltipsEquipGem m_CompareGemInfo;
    public UILabel m_CompareDescLabel;
    public UILabel m_ComparePriceLabel;
    public UISprite m_ComparePriceIcon;
    public UISprite m_CompareIconSprite;
    public UISprite m_CompareQualityGrid;
    public UISlider m_CompareStrenExp;
    public UILabel m_CompareStrenExpInfo;
    public UILabel m_CompareStrenExpTitle;
    public UILabel m_CompareSellLabel;

	// Use this for initialization
	void Start () {
	}

    void OnDestroy()
    {
    }

    /// <summary>
    /// 显示对比装备Tooltips
    /// </summary>
    /// <param name="nEquipSlot">玩家身上装备槽位</param>
    public void ShowTooltips(int nEquipSlot)
    {
        GameItem equip = GameManager.gameManager.PlayerDataPool.EquipPack.GetItem(nEquipSlot);
        if (equip != null && equip.IsValid())
        {
            // 装备图标
            ShowCompareIcon(equip);
            // 颜色品级
            ShowCompareQualityGrid(equip);
	        // 装备名
	        ShowCompareName(equip);
	        // 职业
	        ShowComparePro(equip);
	        // 人物等级需求
	        ShowCompareLevel(equip);
	        // 绑定信息
	        ShowCompareBind(equip);
            // 是否可出售
            SetCompareSellInfo(equip);
	        // 星级
	        ShowCompareStarLevel(equip);
	        // 强化等级
	        ShowCompareStrengthenLevel(equip);
	        // 战斗力
	        ShowComparePower(equip);
	        // 属性
	        ShowCompareAttrInfo(equip);
	        // 宝石
	        ShowCompareGemInfo(equip);
	        // 描述
	        ShowCompareDesc(equip);
	        // 售价
	        ShowComparePrice(equip);
            // 强化经验
            ShowEquipStrenExp(equip);
        }
    }

    void SetCompareSellInfo(GameItem equip)
    {
        Tab_CommonItem tabItem = TableManager.GetCommonItemByID(equip.DataID, 0);
        if (tabItem != null)
        {
            m_CompareSellLabel.text = "[FFFFCC]";
            m_CompareSellLabel.text += (ConsignSaleBag.isCanConsignSale(equip) && !equip.BindFlag) ? StrDictionary.GetClientDictionaryString("#{2382}") : StrDictionary.GetClientDictionaryString("#{2383}");
        }
    }

    void ShowCompareIcon(GameItem equip)
    {
        m_CompareIconSprite.spriteName = equip.GetIcon();
        //m_CompareIconSprite.MakePixelPerfect();
    }

    void ShowCompareQualityGrid(GameItem equip)
    {
        Tab_CommonItem tabItem = TableManager.GetCommonItemByID(equip.DataID, 0);
        if (tabItem != null)
        {
            int nQuality = tabItem.Quality;
            m_CompareQualityGrid.spriteName = GlobeVar.QualityColorGrid[nQuality - 1];
        }
    }

    void ShowCompareName(GameItem equip)
    {
        int nQuality = TableManager.GetCommonItemByID(equip.DataID, 0).Quality;
        m_CompareNameLabel.text = Utils.GetItemNameColor(nQuality);
        m_CompareNameLabel.text += TableManager.GetCommonItemByID(equip.DataID, 0).Name;
        int nExistTime = TableManager.GetCommonItemByID(equip.DataID, 0).ExistTime;
        if (nExistTime > 0)
        {
            m_CompareNameLabel.text += "(" + (float)nExistTime / 60f / 24f + "天)";
        }
    }

    void ShowComparePro(GameItem equip)
    {
        int nProfession = equip.GetProfessionRequire();
        if (0 <= nProfession && nProfession < (int)CharacterDefine.PROFESSION.MAX)
        {
            string strProText = "";
            Obj_MainPlayer mainplayer = Singleton<ObjManager>.Instance.MainPlayer;
            if (mainplayer != null)
            {
                if (mainplayer.Profession == nProfession)
                {
                    strProText = "[FFFFCC]";
                }
                else
                {
                    strProText = "[E60012]";
                }
            }
            strProText += StrDictionary.GetClientDictionaryString("#{" + CharacterDefine.PROFESSION_DICNUM[nProfession].ToString() + "}");
            m_CompareProLabel.text = strProText;
        }
        else
        {
            m_CompareProLabel.text = "";
        }
    }

    void ShowCompareLevel(GameItem equip)
    {
        int nPlayerLevel = Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.Level;
        int nCompareLevel = TableManager.GetCommonItemByID(equip.DataID, 0).MinLevelRequire;
        if (nPlayerLevel >= nCompareLevel)
        {
            m_CompareLevelLabel.text = "[FFFFCC]";
        }
        else
        {
            m_CompareLevelLabel.text = "[E60012]";
        }
        string strCompareLevel = "人物等级需求: {0}";
        m_CompareLevelLabel.text += string.Format(strCompareLevel, nCompareLevel);
    }

    void ShowCompareBind(GameItem equip)
    {
        m_CompareBindLabel.text = "[FFFFCC]";
        if (equip.BindFlag)
        {
            //m_CompareBindLabel.text += "已绑定";
            m_CompareBindLabel.text += StrDictionary.GetClientDictionaryString("#{2827}");
        }
        else
        {
            //m_CompareBindLabel.text += "未绑定";
            m_CompareBindLabel.text += StrDictionary.GetClientDictionaryString("#{2828}");
        }
    }

    void ShowCompareStarLevel(GameItem equip)
    {
        int nStarLevel = equip.StarLevel;
        m_CompareStarLevel.ShowStar(nStarLevel);
    }

    void ShowCompareStrengthenLevel(GameItem equip)
    {
        int nStrengthenLevel = equip.EnchanceLevel;
        //string strStrengthenLevel = "[FFFF33]强化 +{0}";
        string strStrengthenLevel = StrDictionary.GetClientDictionaryString("#{2830}", nStrengthenLevel);
        m_CompareStrengthenLevelLabel.text = strStrengthenLevel;
        //腰带不显示强化等级
        if (equip.IsBelt())
        {
            m_CompareStrengthenLevelLabel.text = "";
        }
    }

    void ShowComparePower(GameItem equip)
    {
        //m_ComparePowerLabel.text = "战斗力: " + equip.GetCombatValue().ToString();
        m_ComparePowerLabel.text = StrDictionary.GetClientDictionaryString("#{2829}", equip.GetCombatValue());
    }

    void ShowCompareAttrInfo(GameItem equip)
    {
        m_CompareAttrInfo.ShowAttr(equip);
    }

    void ShowCompareGemInfo(GameItem equip)
    {
        //m_CompareGemInfo.InitGemInfo(equip.GetEquipSlotIndex());
    }

    void ShowCompareDesc(GameItem equip)
    {
        m_CompareDescLabel.text = "[FFFFCC]";
        string strCompareDesc = TableManager.GetCommonItemByID(equip.DataID, 0).Tips;
        m_CompareDescLabel.text += strCompareDesc;
    }

    void ShowComparePrice(GameItem equip)
    {
        Tab_CommonItem tabItem = TableManager.GetCommonItemByID(equip.DataID, 0);
        if (tabItem != null)
        {
            // 价格
            m_ComparePriceLabel.text = "[FFFFCC]";
            m_ComparePriceLabel.text += tabItem.SellPrice.ToString();

            // 出售方式 图标
//             int nPower = Mathf.FloorToInt(Mathf.Log10((float)nComparePrice));
//             m_ComparePriceIcon.gameObject.transform.localPosition = new Vector3(84 + 12 * nPower, 0, 0);
            if (tabItem.SellMoneyType == (int)ItemSellMoneyType.TYPE_COIN)
            {
                m_ComparePriceIcon.spriteName = "qian5";
            }
            else if (tabItem.SellMoneyType == (int)ItemSellMoneyType.TYPE_YUANBAO)
            {
                m_ComparePriceIcon.spriteName = "qian2";
            }
            else if (tabItem.SellMoneyType == (int)ItemSellMoneyType.TYPE_BIND_YUANBAO)
            {
                m_ComparePriceIcon.spriteName = "qian3";
            }
        }
    }

    void ShowEquipStrenExp(GameItem equip)
    {
        m_CompareStrenExpTitle.gameObject.SetActive(true);
        m_CompareStrenExp.gameObject.SetActive(true);
        m_CompareStrenExpInfo.gameObject.SetActive(true);

        int nEnchanceExpMax = equip.GetEnchanceExpMax();
        if (nEnchanceExpMax > 0)
        {
            m_CompareStrenExp.value = (float)(equip.EnchanceExp) / (float)nEnchanceExpMax;
            m_CompareStrenExpInfo.text = equip.EnchanceExp.ToString() + "/" + nEnchanceExpMax.ToString();
        }
        else
        {
            m_CompareStrenExp.value = 0;
            m_CompareStrenExpInfo.text = "0/0";
        }
        //腰带不显示强化经验
        if (equip.IsBelt())
        {
            m_CompareStrenExpTitle.gameObject.SetActive(false);
            m_CompareStrenExp.gameObject.SetActive(false);
            m_CompareStrenExpInfo.gameObject.SetActive(false);
        }
    }

    void ShowSuit()
    {

    }
}