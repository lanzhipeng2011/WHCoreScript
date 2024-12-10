/********************************************************************
	created:	2014/01/14
	created:	14:1:2014   13:50
	filename: 	SysShopPageItem.cs
	author:		王迪
	
	purpose:	系统商店物品条
*********************************************************************/
using UnityEngine;
using System.Collections;
using GCGame.Table;
using Games.Item;
using Games.GlobeDefine;
using Module.Log;
public class SysShopPageItem : MonoBehaviour {

    public GameObject SprHighLight;
    public UILabel LabelMoney;
    public UILabel LabelName;
    public UISprite sprIcon;
    public UISprite MoneyIcon;
    public UISprite QualitySprite;

    public delegate void ItemClickDelegate(SysShopPageItem item);
    public ItemClickDelegate delItemClick;
    public ItemClickDelegate delItemDoubleClick;
    public ItemClickDelegate delItemIconClick;


    private GameItem m_curGameItem;
	// Use this for initialization
	void Start () {
	
	}
	

    void OnItemClick()
    {
        if (null != delItemClick)
        {
            delItemClick(this);
        }
    }

    void OnItemDoubleClick()
    {
        if (null != delItemDoubleClick)
        {
            delItemDoubleClick(this);
        }
    }

    void OnItemIconClick()
    {
        if (null != delItemIconClick)
        {
            delItemIconClick(this);
        }
    }


    public void EnableHighLight(bool bEnable)
    {
        SprHighLight.SetActive(bEnable);
    }

    public void SetData(ItemClickDelegate clickFun, ItemClickDelegate doubleClickFun, ItemClickDelegate iconClickFun, int shopIndex, Tab_SystemShop tabSysShop)
    {
        if (tabSysShop == null)
        {
            return;
        }
        delItemClick = clickFun;
        delItemDoubleClick = doubleClickFun;
        delItemIconClick = iconClickFun;
        int pid = tabSysShop.GetPidbyIndex(shopIndex);
        LabelMoney.text = tabSysShop.GetPricebyIndex(shopIndex).ToString();
        Tab_CommonItem curTabItem = TableManager.GetCommonItemByID(pid, 0);
        if (null == curTabItem)
        {
            LogModule.WarningLog("can not read cur common item talbe :" + pid.ToString());
            return;

        }

        int groupCount = tabSysShop.GetNumPerGroupbyIndex(shopIndex);
        if (groupCount < 0)
        {
            groupCount = 1;
        }

        LabelName.text = curTabItem.Name;// +"*" + groupCount.ToString();
        sprIcon.spriteName = curTabItem.Icon;
        int colorQuality = curTabItem.Quality - 1;
        if (colorQuality >= 0 && colorQuality < GlobeVar.QualityColorGrid.Length)
        {
            QualitySprite.spriteName = GlobeVar.QualityColorGrid[curTabItem.Quality - 1];
        }
       

        int moneyType = tabSysShop.GetMoneyTypebyIndex(shopIndex);
        int moneySubType = tabSysShop.GetMoneySubTypebyIndex(shopIndex);
        if (moneyType == (int)Consume_Type.COIN)
        {
            MoneyIcon.spriteName = "qian5";
        }
        else if (moneyType == (int)Consume_Type.YUANBAO)
        {
            if (moneySubType == (int)Consume_SubType.YUANBAO_NORMAL)
            {
                MoneyIcon.spriteName = "qian2";
            }
            else if (moneySubType == (int)Consume_SubType.YUANBAO_BIND)
            {
                MoneyIcon.spriteName = "qian3";
            }
        }
      
        m_curGameItem = new GameItem();
        m_curGameItem.DataID = pid;
    }

    public GameItem GetGameItem()
    {
        return m_curGameItem;
    }
}
