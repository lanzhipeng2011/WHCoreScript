//********************************************************************
// 文件名: ItemTooltipsLogic.cs
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
using Games.UserCommonData;
using Games.LogicObj;
public class ItemTooltipsLogic : MonoBehaviour {
	
	public enum ShowType
	{
		Normal = 1,     //正常tooltips
		Info,           //无操作按钮的tooltips
		ShopBuy,            // 购买界面
		ShopBuyBatch,       // 可以批量购买
		QianKunDaiStuff,    // 乾坤袋材料选择 比Normal多一个放入
		ChatLink,           //聊天栏超链
		CangKu,             //仓库取出
		CangKuBackPack,     //仓库放入
	}
	
	public UILabel m_ItemNameLabel;
	public UILabel m_ItemTypeLabel;
	public UILabel m_ItemUseLevelLabel;
	public UILabel m_ItemBindLabel;
	public UILabel m_ItemPriceLabel;
	public UISprite m_ItemPriceIcon;
	public UILabel m_ItemMaxNumLabel;
	public UILabel m_ItemDescLabel;
	public UISprite m_ItemIconSprite;
	public UISprite m_ItemQualityGrid;
	public UIImageButton m_SellButton;
	public UIImageButton m_ThrowButton;
	public UIImageButton m_UseButton;
	public UIImageButton m_ShareLinkButton;
	public UIImageButton m_BuyButton;
	public UIImageButton m_BuyBatchButton;
	public UIImageButton m_ConsignSaleBuyButton;
	public UIImageButton m_CangKuInButton;
	public UIImageButton m_CangKuOutButton;
	public UILabel m_ItemSellLabel;
	public GameObject m_ItemDisableSprite;
	public UIImageButton m_UseBatchButton;
	public GameObject m_PutInQianKunDaiButton;
	public GameObject m_ComposeButton;
	public UIGrid m_OpButtonGrid;
	public UILabel m_ItemDescDynamicLabel;
	public UILabel m_RemainTimeLable;
	public GameItem m_Item = null;
	
	private static ItemTooltipsLogic m_Instance;
	public static ItemTooltipsLogic Instance()
	{
		return m_Instance;
	}
	
	private static GameItem m_curItem;
	private static ShowType m_curType;
	private static ItemSlotLogic m_curItemSlot;
	public static void ShowItemTooltip(int dataId, ShowType type, ItemSlotLogic slot = null)
	{
		GameItem item = new GameItem();
		item.DataID = dataId;
		if (item.IsValid() && !item.IsEquipMent())
		{
			ShowItemTooltip(item, type, slot);
		}
	}
	public static void ShowItemTooltip(GameItem equip, ShowType type, ItemSlotLogic slot = null)
	{
		m_curItem = equip;
		m_curType = type;
		m_curItemSlot = slot;
		UIManager.ShowUI(UIInfo.ItemTooltipsRoot, ItemTooltipsLogic.OnShowItemTip);
	}
	
	private static void OnShowItemTip(bool bSuccess, object param)
	{
		if (!bSuccess)
		{
			LogModule.ErrorLog("load equiptooltip error");
			return;
		}
		
		ItemTooltipsLogic.Instance().ShowTooltips(m_curItem, m_curType);
	}
	
	
	// 新手指引
	private int m_NewPlayerGuideFlag_Step = -1;
	
	void Awake()
	{
		m_Instance = this;
	}
	
	void Start()
	{
		Check_NewPlayerGuide();
	}
	
	
	void OnDestroy()
	{
		if (NumChooseController.Instance() != null)
		{
			UIManager.CloseUI(UIInfo.NumChoose);
		}
		m_Instance = null;
	}
	
	private void ShowTooltips(GameItem item, ShowType type)
	{
		if (item == null)
		{
			CloseWindow();
			return;
		}
		if (item.IsValid() == false)
		{
			CloseWindow();
			return;
		}
		
		gameObject.SetActive(true);
		// 物品图标
		SetItemIcon(item);
		// 玩家是否可用
		SetItemDisableSprite(item);
		// 颜色品级
		SetItemQualityGrid(item);
		// 物品名 根据物品等级变颜色
		SetItemName(item);
		// 类别
		SetItemType(item);
		// 使用等级
		SetItemUseLevel(item);
		// 绑定信息
		SetItemBind(item);
		// 是否可出售
		SetItemSellInfo(item);
		// 售价
		SetItemPrice(item);
		// 叠加数量
		SetItemMaxNum(item);
		// 描述
		SetItemDesc(item);
		// 动态数据域
		SetItemDynamicDesc(item);
		// 显示剩余时间
		ShowRemainTime(item);
		
		int canuse = TableManager.GetCommonItemByID(item.DataID, 0).CanUse;
		int cansell = TableManager.GetCommonItemByID(item.DataID, 0).CanSell;
		int canthrow = TableManager.GetCommonItemByID(item.DataID, 0).CanThrow;
		int cancompose = TableManager.GetCommonItemByID (item.DataID, 0).IsCanQianKunDai;
		m_BuyButton.gameObject.SetActive(false);
		m_BuyBatchButton.gameObject.SetActive(false);
		
		m_SellButton.gameObject.SetActive(false);
		//丢弃按钮
		m_ThrowButton.gameObject.SetActive(false);
		//使用按钮
		m_UseButton.gameObject.SetActive(false);
		//批量使用按钮
		m_UseBatchButton.gameObject.SetActive(false);
		//链接按钮
		m_ShareLinkButton.gameObject.SetActive(false);
		//乾坤袋放入
		m_PutInQianKunDaiButton.SetActive(false);
		m_ComposeButton.SetActive (false);
		//寄售行求购信息
		m_ConsignSaleBuyButton.gameObject.SetActive(false);
		//仓库取回按钮
		m_CangKuOutButton.gameObject.SetActive(false);
		//仓库放入按钮
		m_CangKuInButton.gameObject.SetActive(false);
		if (type == ShowType.Info)    //仅显示信息 没有操作按钮的tips
		{
			//出售按钮
			
		}
		else if(type == ShowType.ShopBuy)
		{
			m_BuyButton.gameObject.SetActive(true);
		}
		else if(type == ShowType.ShopBuyBatch)
		{
			m_BuyButton.gameObject.SetActive(true);
			m_BuyBatchButton.gameObject.SetActive(true);
		}
		else if (type == ShowType.ChatLink)
		{
			//根据是否可以上架 决定是否显示求购按钮
			if (ConsignSaleBag.isCanConsignSale(item, true))
			{
				m_ConsignSaleBuyButton.gameObject.SetActive(true);
			}
		}
		else if (type == ShowType.CangKu)  //仓库界面 仓库物品tips
		{
			//显示取回按钮
			m_CangKuOutButton.gameObject.SetActive(true);
		}
		else if (type == ShowType.CangKuBackPack)  //仓库界面 背包物品tips
		{
			//显示放入仓库按钮
			m_CangKuInButton.gameObject.SetActive(true);
		}
		else
		{
			if (type == ShowType.QianKunDaiStuff)
			{
				m_PutInQianKunDaiButton.SetActive(true);
				m_ComposeButton.SetActive(false);
			}
			else
			{
				m_PutInQianKunDaiButton.SetActive(false);
			}
			
			//出售按钮
			m_SellButton.gameObject.SetActive((cansell == 1) ? true : false);
			//使用按钮
			m_UseButton.gameObject.SetActive((canuse == 1) ? true : false);
			//批量使用按钮
			m_UseBatchButton.gameObject.SetActive((canuse == 1 && item.StackCount > 1) ? true : false);
			//丢弃按钮
			m_ThrowButton.gameObject.SetActive((canthrow == 1) ? true : false);
			//链接按钮
			m_ShareLinkButton.gameObject.SetActive(true);
			if(m_PutInQianKunDaiButton.gameObject.activeInHierarchy==false)
		      	m_ComposeButton.SetActive((cancompose==1)?true:false);
			m_OpButtonGrid.Reposition();
		}

		if (item.Guid == GlobeVar.INVALID_GUID)
		{
			m_SellButton.gameObject.SetActive(false);
			if(m_ComposeButton!=null)
			m_ComposeButton.gameObject.SetActive(false);
		}
		m_Item = item;
		
		//gameObject.SetActive(true);
		//UIManager.ShowUI(UIInfo.ItemTooltipsRoot);
	}
	
	void SetItemSellInfo(GameItem item)
	{
		if (item != null && item.IsValid())
		{
			Tab_CommonItem tabItem = TableManager.GetCommonItemByID(item.DataID, 0);
			if (tabItem != null)
			{
				m_ItemSellLabel.text = "[FFFFCC]";
				m_ItemSellLabel.text += (ConsignSaleBag.isCanConsignSale(item) && !item.BindFlag) ? StrDictionary.GetClientDictionaryString("#{2382}") : StrDictionary.GetClientDictionaryString("#{2383}");
			}
		}
	}
	
	void SetItemIcon(GameItem item)
	{
		if (item != null && item.IsValid())
		{
			m_ItemIconSprite.spriteName = item.GetIcon();
			// m_ItemIconSprite.MakePixelPerfect();
		}
	}
	
	void SetItemDisableSprite(GameItem item)
	{
		if (item != null && item.IsValid())
		{
			if (item.IsFitForPlayer())
			{
				m_ItemDisableSprite.SetActive(false);
			}
			else
			{
				m_ItemDisableSprite.SetActive(true);
			}
		}
	}
	
	void SetItemQualityGrid(GameItem item)
	{
		if (item != null && item.IsValid())
		{
			Tab_CommonItem tabItem = TableManager.GetCommonItemByID(item.DataID, 0);
			if (tabItem != null)
			{
				int nQuality = tabItem.Quality;
				m_ItemQualityGrid.spriteName = GlobeVar.QualityColorGrid[nQuality - 1];
			}
		}
	}
	
	void SetItemName(GameItem item)
	{
		if (item != null && item.IsValid())
		{
			int nQuality = (int)item.GetQuality();
			m_ItemNameLabel.text = Utils.GetItemNameColor(nQuality);
			m_ItemNameLabel.text += TableManager.GetCommonItemByID(item.DataID, 0).Name;
			int nExistTime = TableManager.GetCommonItemByID(item.DataID, 0).ExistTime;
            if (nExistTime < 0)
            {
                return;
            }

			if ((nExistTime / 60f /24f) >= 1)
			{
				m_ItemNameLabel.text += "(" + (float)nExistTime / 60f / 24f + "天)";
			}
            else if ((int)nExistTime / 60f >= 1)
            {
                m_ItemNameLabel.text += "(" + (int)nExistTime / 60f + "小时)";
            }
            else
            {
                m_ItemNameLabel.text += "(" + nExistTime + "分钟)";
            }
		}
	}
	
	void SetItemType(GameItem item)
	{
		if (item != null && item.IsValid())
		{
			Tab_CommonItem tabItem = TableManager.GetCommonItemByID(item.DataID, 0);
			if (tabItem != null)
			{
				int nClassID = tabItem.ClassID;
				int nSubClassID = tabItem.SubClassID;
				m_ItemTypeLabel.text = "[FFFFCC]" + Utils.GetItemType(nClassID, nSubClassID);
			}
		}
	}
	
	void SetItemUseLevel(GameItem item)
	{
		if (item != null && item.IsValid())
		{
			int nPlayerLevel = Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.Level;
			Tab_CommonItem tabItem = TableManager.GetCommonItemByID(item.DataID, 0);
			if (null != tabItem)
			{
				int nItemUseLevel = tabItem.MinLevelRequire;
				if (nPlayerLevel >= nItemUseLevel)
				{
					m_ItemUseLevelLabel.text = "[FFFFCC]";
				}
				else
				{
					m_ItemUseLevelLabel.text = "[E60012]";
				}
				string strItemUseLevel = "使用等级：{0}级";
				m_ItemUseLevelLabel.text += string.Format(strItemUseLevel, nItemUseLevel);
			}
		}
	}
	
	void SetItemBind(GameItem item)
	{
		if (item != null && item.IsValid())
		{
			m_ItemBindLabel.text = "[FFFFCC]";
			if (item.BindFlag)
			{
				m_ItemBindLabel.text += "已绑定";
			}
			else
			{
				m_ItemBindLabel.text += "未绑定";
			}
		}
	}
	
	void SetItemPrice(GameItem item)
	{
		if (item != null && item.IsValid())
		{
			Tab_CommonItem tabItem = TableManager.GetCommonItemByID(item.DataID, 0);
			if (tabItem != null)
			{
				// 价格
				m_ItemPriceLabel.text = "[FFFFCC]";
				m_ItemPriceLabel.text += tabItem.SellPrice.ToString();
				
				// 出售方式 图标
				//             int nPower = Mathf.FloorToInt(Mathf.Log10((float)nItemPrice));
				//             m_ItemPriceIcon.gameObject.transform.localPosition = new Vector3(84 + 12 * nPower, 0, 0);
				if (tabItem.SellMoneyType == (int)ItemSellMoneyType.TYPE_COIN)
				{
					m_ItemPriceIcon.spriteName = "qian5";
				}
				else if (tabItem.SellMoneyType == (int)ItemSellMoneyType.TYPE_YUANBAO)
				{
					m_ItemPriceIcon.spriteName = "qian2";
				}
				else if (tabItem.SellMoneyType == (int)ItemSellMoneyType.TYPE_BIND_YUANBAO)
				{
					m_ItemPriceIcon.spriteName = "qian3";
				}
			}
		}
	}
	
	void SetItemDesc(GameItem item)
	{
		if (item != null && item.IsValid())
		{
			m_ItemDescLabel.text = "[FFFFCC]";
			if (null != TableManager.GetCommonItemByID(item.DataID, 0))
			{
				string strItemDesc = TableManager.GetCommonItemByID(item.DataID, 0).Tips;
				m_ItemDescLabel.text += strItemDesc;
			}
		}
	}
	void SetItemDynamicDesc(GameItem item)
	{
		if (item != null && item.IsValid())
		{
			if (item.GetClass() == (int)ItemClass.MEDIC
			    && (item.GetSubClass() == (int)MedicSubClass.HP_DY
			    || item.GetSubClass() == (int)MedicSubClass.MP_DY
			    || item.GetSubClass() == (int)MedicSubClass.HPMP_DY
			    ))
			{
				m_ItemDescDynamicLabel.gameObject.SetActive(true);
				
				if (item.DynamicData[0] == 1)
				{
					m_ItemDescDynamicLabel.text = StrDictionary.GetClientDictionaryString("#{3159}", item.DynamicData[1]);
				}
				else
				{
					int dataId = item.DataID;
					Tab_UsableItem line = TableManager.GetUsableItemByID(dataId, 0);
					if (line != null)
					{
						m_ItemDescDynamicLabel.text = StrDictionary.GetClientDictionaryString("#{3159}", line.UseParamB);
					}                    
				}
			}
			else
			{
				m_ItemDescDynamicLabel.gameObject.SetActive(false);
			}
		}
	}
	
	void ShowRemainTime(GameItem item)
	{
		if (item != null && item.IsValid())
		{
			Tab_CommonItem line = TableManager.GetCommonItemByID(item.DataID, 0);
			if (line != null)
			{
				if (line.ExistTime != -1)
				{
					int remainSecond = (line.ExistTime * 60) - (GlobalData.ServerAnsiTime - item.CreateTime);
					if (remainSecond > 0)
					{
						if (remainSecond >= 24 * 3600)
						{
							//m_RemainTimeLable.text = string.Format("{0}天", (int)(remainSecond / 3600 / 24));
							m_RemainTimeLable.text = StrDictionary.GetClientDictionaryString("#{2833}", Mathf.RoundToInt((float)remainSecond / 3600.0f / 24.0f));
						}
						else if (remainSecond >= 60 * 60)
						{
							//m_RemainTimeLable.text = string.Format("{0}小时", (int)(remainSecond / 60 / 60));
							m_RemainTimeLable.text = StrDictionary.GetClientDictionaryString("#{2834}", Mathf.RoundToInt((float)remainSecond / 60.0f / 60.0f));
						}
						else
						{
							//m_RemainTimeLable.text = string.Format("{0}小时", 1);
							m_RemainTimeLable.text = StrDictionary.GetClientDictionaryString("#{2834}", 1);
						}
					}
					else
					{
						//m_RemainTimeLable.text = string.Format("{0}小时", 0);
						m_RemainTimeLable.text = StrDictionary.GetClientDictionaryString("#{2834}", 0);
					}
					m_RemainTimeLable.gameObject.SetActive(true);
				}
				else
				{
					m_RemainTimeLable.gameObject.SetActive(false);
				}
			}
		}
	}
	
	void SetItemMaxNum(GameItem item)
	{
		if (item != null && item.IsValid())
		{
			m_ItemMaxNumLabel.text = "[FFFFCC]";
			string strItemMaxNum = "叠加数量:{0}";
			if (null != TableManager.GetCommonItemByID(item.DataID, 0))
			{
				int nItemMaxNum = TableManager.GetCommonItemByID(item.DataID, 0).MaxStackSize;
				m_ItemMaxNumLabel.text += string.Format(strItemMaxNum, nItemMaxNum);
			}
		}
	}
	
	public void CloseWindow()
	{
		NewPlayerGuidLogic.CloseWindow();
		m_Item = null;
		//gameObject.SetActive(false);
		UIManager.CloseUI(UIInfo.ItemTooltipsRoot);
	}
	
	void ItemSell()
	{
		if (m_Item != null && m_Item.IsValid())
		{

			//========道具表中有元宝购买价格的道具，需弹出确认界面
			if((int)m_Item.GetQuality() >= 4 || TableManager.GetYuanBaoShopByID(m_Item.DataID) != null);
			{
				string str = StrDictionary.GetClientDictionaryString("#{6006}");
				MessageBoxLogic.OpenOKCancelBox(str, "", SellOk, SellCancel);
				return;
			}

			List<ulong> selllist = new List<ulong>();
			selllist.Add(m_Item.Guid);
			SysShopController.SellItem((int)GameItemContainer.Type.TYPE_BACKPACK, selllist);
			CloseWindow();
		}
	}

	void SellOk()
	{
		if (m_Item != null && m_Item.IsValid())
		{
			List<ulong> selllist = new List<ulong>();
			selllist.Add(m_Item.Guid);
			SysShopController.SellItem((int)GameItemContainer.Type.TYPE_BACKPACK, selllist);
			CloseWindow();
		}
	
	}
	void SellCancel()
	{
	}
	
	void ItemThrow()
	{
		if (m_Item != null && m_Item.IsValid())
		{
			if (Singleton<ObjManager>.Instance.MainPlayer.CheckThrowItem(m_Item))
			{
				Singleton<ObjManager>.Instance.MainPlayer.ThrowItem(m_Item);
			}
			CloseWindow();
		}
	}
	
	void ItemUse()
	{
		if (m_Item != null && m_Item.IsValid())
		{
			if (m_NewPlayerGuideFlag_Step == 0)
			{
				NewPlayerGuidLogic.CloseWindow();
				m_NewPlayerGuideFlag_Step = -1;
				if (BackPackLogic.Instance())
				{
					BackPackLogic.Instance().NewPlayerGuide(2);
				}
			}
			if (null != Singleton<ObjManager>.Instance.MainPlayer &&
			    Singleton<ObjManager>.Instance.MainPlayer.CheckUseItem(m_Item))
			{
				Singleton<ObjManager>.Instance.MainPlayer.UseItem(m_Item);
			}
			CloseWindow();
		}
	}
	
	void ItemBatchUse()
	{
		if (m_Item != null && m_Item.IsValid())
		{
			//NumChooseController.OpenWindow(1, m_Item.StackCount, "使用", OnBatchUseNumChoose,1);
			NumChooseController.OpenWindow(1, m_Item.StackCount, StrDictionary.GetClientDictionaryString("#{2840}"), OnBatchUseNumChoose, 1);
		}
	}
	
	void OnBatchUseNumChoose(int num)
	{
		if (m_Item != null && m_Item.IsValid())
		{
			for (int i = 0; i < num && i < m_Item.StackCount; i++)
			{
				if (null != Singleton<ObjManager>.Instance.MainPlayer &&
				    Singleton<ObjManager>.Instance.MainPlayer.CheckUseItem(m_Item))
				{
					Singleton<ObjManager>.Instance.MainPlayer.UseItem(m_Item);
				}
			}
			CloseWindow();
		}
	}
	
	void ItemShareLink()
	{
		if (m_Item != null && m_Item.IsValid())
		{
			ShareTargetChooseLogic.InitItemShare(m_Item);
		}
		//ShareLinkDirectChatInfo();
	}
	
	void PutInQianKunDai()
	{
		if (m_Item != null && m_Item.IsValid())
		{
			if (QianKunDaiLogic.Instance() != null && QianKunDaiLogic.Instance().gameObject.activeSelf)
			{
				QianKunDaiLogic.Instance().ChooseStuff(m_Item, m_curItemSlot);
			}

			CloseWindow();
		}

		if(QianKunDaiLogic.Instance () != null && m_NewPlayerGuideFlag_Step == 0)
		{
			QianKunDaiLogic.Instance ().NewPlayerGuide(1);
			m_NewPlayerGuideFlag_Step = -1;
		}
	}
	
	void Check_NewPlayerGuide()
	{
		if (BackPackLogic.Instance() != null)
		{
			int nIndex = BackPackLogic.Instance().NewPlayerGuideFlag_Step;
			if (nIndex == 1)
			{
				NewPlayerGuide(0);
				BackPackLogic.Instance().NewPlayerGuideFlag_Step = -1;
			}
		}
		else if (SysShopController.Instance() != null)
		{
			int nIndex = SysShopController.Instance().NewPlayerGuide_Step;
			if (nIndex == 1)
			{
				NewPlayerGuide(1);
				SysShopController.Instance().NewPlayerGuide_Step = -1;
			}
		}
		
	}
	public void NewPlayerGuide(int nIndex)
	{
		if (nIndex < 0)
		{
			return;
		}

		NewPlayerGuidLogic.CloseWindow();

		m_NewPlayerGuideFlag_Step = nIndex;

		switch (m_NewPlayerGuideFlag_Step)
		{
		case 0:
			NewPlayerGuidLogic.OpenWindow(m_PutInQianKunDaiButton.gameObject, 120, 76, "", "right", 2, true, true);
			break;
		case 1:
			NewPlayerGuidLogic.OpenWindow(m_BuyBatchButton.gameObject, 120, 76, "", "right", 2, true, true);
			break;
		}
	}
	
	void OnBuyClick()
	{
		if (SysShopController.Instance() != null)
		{
			SysShopController.Instance().BuyCurItem();
		}
	}
	
	void OnBuyBatchClick()
	{
		if (SysShopController.Instance() != null)
		{
			if (m_NewPlayerGuideFlag_Step == 1)
			{
				m_NewPlayerGuideFlag_Step = -1;
				NewPlayerGuidLogic.CloseWindow();
			}
			SysShopController.Instance().BuyBatchCurItem();
		}
	}
	
	void ShareLinkDirectChatInfo()
	{
		UIManager.ShowUI(UIInfo.ChatInfoRoot, ShowChatInfoRootOver);
	}
	
	void ShowChatInfoRootOver(bool bSuccess, object param)
	{
		if (m_Item != null && m_Item.IsValid())
		{
			if (bSuccess)
			{
				ChatInfoLogic.Instance().InsertItemLinkText(m_Item);
				ItemTooltipsLogic.Instance().CloseWindow();
			}
		}
	}
	//寄售行求购
	void ConsignSaleBuyBt()
	{
		UIManager.ShowUI(UIInfo.ConsignSaleRoot, BuyItemOpenConsignSale);
		
	}
	
	void BuyItemOpenConsignSale(bool bSuccess, object param)
	{
		if (m_Item != null && m_Item.IsValid())
		{
			if (bSuccess)
			{
				if (ConsignSaleLogic.Instance() != null)
				{
					ConsignSaleLogic.Instance().SearchForAskBuy(m_Item.GetName());
				}
				CloseWindow();
			}
		}
	}
	
	void CangKuIn()
	{
		if (m_Item != null && m_Item.IsValid() && null != Singleton<ObjManager>.Instance.MainPlayer)
		{
			Singleton<ObjManager>.Instance.MainPlayer.CangKuPutIn(m_Item);
			CloseWindow();
		}
	}
	
	void CangKuOut()
	{
		if (m_Item != null && m_Item.IsValid() && null != Singleton<ObjManager>.Instance.MainPlayer)
		{
			Singleton<ObjManager>.Instance.MainPlayer.CangKuTakeOut(m_Item);
			CloseWindow();
		}
	}

	//by dsy  加入合成按钮
    void OnCompose()
	{
		UIManager.ShowUI(UIInfo.BackPackRoot, BackPackLogic.SwitchQianKunDaiViewPutInDirectly,m_Item);
		CloseWindow();
	}
	


}
