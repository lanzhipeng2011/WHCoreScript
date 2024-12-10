//********************************************************************
// 文件名: MoneyTipsLogic.cs
// 描述: 物品tips界面脚本
// 作者: WangZhe
//********************************************************************

using UnityEngine;
using System.Collections;
using GCGame.Table;
using Games.Item;
using Games.GlobeDefine;
using GCGame;
using System.Collections.Generic;
using Module.Log;

public class MoneyTipsLogic : MonoBehaviour {

    public enum MoneyType
    {
        ITEM_NONE,
        ITEM_MONEY,
        ITEM_EXP,
        ITEM_YUANBAO,
        ITEM_SHENGWANG, 
        ITEM_ZHENQI,
    }

    public UILabel m_ItemNameLabel;
    public UILabel m_ItemTypeLabel;
    public UILabel m_ItemUseLevelLabel;
    public UILabel m_ItemBindLabel;
    public UILabel m_ItemDescLabel;
    public UISprite m_ItemIconSprite;
    public UILabel m_ItemSellLabel;

    private static MoneyTipsLogic m_Instance;
    public static MoneyTipsLogic Instance()
    {
        return m_Instance;
    }

    private static MoneyType m_curType;
    private static int m_Num;
    public static void ShowMoneyTip(MoneyType type, int nNum)
    {
        m_curType = type;
        m_Num = nNum;
        UIManager.ShowUI(UIInfo.MoneyTipRoot, MoneyTipsLogic.OnShowMoneyTip);
    }

    private static void OnShowMoneyTip(bool bSuccess, object param)
    {
        if (!bSuccess)
        {
            LogModule.ErrorLog("load MoneyTipRoot error");
            return;
        }

        MoneyTipsLogic.Instance().ShowTooltips();
    }

    void Awake()
    {
        m_Instance = this;
    }

    void Start()
    {
    }

    void CleanUp()
    {
        if (m_ItemNameLabel && m_ItemTypeLabel && m_ItemUseLevelLabel
            && m_ItemBindLabel && m_ItemDescLabel && m_ItemIconSprite
            && m_ItemSellLabel)
        {
            m_ItemNameLabel.text = "";
            m_ItemTypeLabel.text = "";
            m_ItemUseLevelLabel.text = "";
            m_ItemBindLabel.text = "";
            m_ItemDescLabel.text = "";
            m_ItemIconSprite.spriteName = "";
            m_ItemSellLabel.text = "";
        }
    }
	
    void OnDestroy()
    {
        m_Instance = null;
    }

    private void ShowTooltips()
    {
        gameObject.SetActive(true);
        CleanUp();

        // 物品图标
        SetItemIcon();
        // 物品名 根据物品等级变颜色
        SetItemName();
        // 类别
        SetItemType();
        // 使用等级
        SetItemUseLevel();
        // 绑定信息
        SetItemBind();
        // 是否可出售
        SetItemSellInfo();
        // 描述
        SetItemDesc();
    }

    void SetItemSellInfo()
    {
        if (m_ItemSellLabel)
        {
            m_ItemSellLabel.text = "[FFFFCC]";
            m_ItemSellLabel.text += StrDictionary.GetClientDictionaryString("#{2383}");
        }
    }

    void SetItemIcon()
    {
        if (m_ItemIconSprite)
        {
            string ItemIcom = "";
            switch (m_curType)
            {
                case MoneyType.ITEM_EXP:
                    ItemIcom = "jingyan";
                    break;
                case MoneyType.ITEM_MONEY:
                    ItemIcom = "jinbi";
                    break;
                case MoneyType.ITEM_YUANBAO:
                    ItemIcom = "bdyuanbao";
                    break;
                case MoneyType.ITEM_SHENGWANG:
                    ItemIcom = "shengwang";
                    break;
                case MoneyType.ITEM_ZHENQI:
                    ItemIcom = "zhenqi";
                    break;
            }
            m_ItemIconSprite.spriteName = ItemIcom;
            m_ItemIconSprite.MakePixelPerfect();
        }
    }

    void SetItemName()
    {
        if (m_ItemNameLabel == null)
        {
            return;
        }
        string ItemName = "[FF9933]";
        switch (m_curType)
        {
            case MoneyType.ITEM_EXP:
                ItemName = StrDictionary.GetClientDictionaryString("#{1325}");
                break;
            case MoneyType.ITEM_MONEY:
                ItemName = StrDictionary.GetClientDictionaryString("#{1324}");
                break;
            case MoneyType.ITEM_YUANBAO:
                ItemName = StrDictionary.GetClientDictionaryString("#{2913}");
                break;
            case MoneyType.ITEM_SHENGWANG:
                ItemName = StrDictionary.GetClientDictionaryString("#{3078}");
                break;
            case MoneyType.ITEM_ZHENQI:
                ItemName = StrDictionary.GetClientDictionaryString("#{3080}");
                break;
        }
        m_ItemNameLabel.text += ItemName;
    }

    void SetItemType()
    {
        if (m_ItemTypeLabel == null)
        {
            return;
        }
        string strItemType = "[FFFFCC]";
        switch (m_curType)
        {
            case MoneyType.ITEM_EXP:
                strItemType = StrDictionary.GetClientDictionaryString("#{1325}");
                break;
            case MoneyType.ITEM_MONEY:
                strItemType = StrDictionary.GetClientDictionaryString("#{1324}");
                break;
            case MoneyType.ITEM_YUANBAO:
                strItemType = StrDictionary.GetClientDictionaryString("#{2913}");
                break;
            case MoneyType.ITEM_SHENGWANG:
                strItemType = StrDictionary.GetClientDictionaryString("#{3078}");
                break;
            case MoneyType.ITEM_ZHENQI:
                strItemType = StrDictionary.GetClientDictionaryString("#{3080}");
                break;
        }
        m_ItemTypeLabel.text += strItemType;
    }

    void SetItemUseLevel()
    {
        if (m_ItemUseLevelLabel)
        {
            m_ItemUseLevelLabel.text = "[FFFFCC]";
            string strItemUseLevel = "使用等级：{0}级";
            m_ItemUseLevelLabel.text += string.Format(strItemUseLevel, 1);
        }
    }

    void SetItemBind()
    {
        if (m_ItemBindLabel)
        {
            m_ItemBindLabel.text = "[FFFFCC]";
            m_ItemBindLabel.text += "已绑定";
        }
    }

    void SetItemDesc()
    {
        string strItemDesc = "[FFFFCC]";
        switch (m_curType)
        {
            case MoneyType.ITEM_EXP:
                strItemDesc = StrDictionary.GetClientDictionaryString("#{2914}", m_Num);
                break;
            case MoneyType.ITEM_MONEY:
                strItemDesc = StrDictionary.GetClientDictionaryString("#{2915}", m_Num);
                break;
            case MoneyType.ITEM_YUANBAO:
                strItemDesc = StrDictionary.GetClientDictionaryString("#{2916}", m_Num);
                break;
            case MoneyType.ITEM_SHENGWANG:
                strItemDesc = StrDictionary.GetClientDictionaryString("#{3079}", m_Num);
                break;
            case MoneyType.ITEM_ZHENQI:
                strItemDesc = StrDictionary.GetClientDictionaryString("#{3081}", m_Num);
                break;
        }
        m_ItemDescLabel.text += strItemDesc;
    }

    public void CloseWindow()
    {
        //gameObject.SetActive(false);
        UIManager.CloseUI(UIInfo.MoneyTipRoot);
    }
}
