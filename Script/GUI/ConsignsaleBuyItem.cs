/********************************************************************
	创建时间:	2014/06/12 13:22
	全路径:		\TLOL\Version\Main\Project\Client\Assets\MLDJ\Script\GUI\ConsignsaleBuyItem.cs
	创建人:		luoy
	功能说明:	寄售行 购买界面 物品条目信息显示UI
	修改记录:
*********************************************************************/
using Games.ConsignSale;
using UnityEngine;
using System.Collections;

public class ConsignsaleBuyItem : MonoBehaviour
{
    public UISprite m_BakSprite;
    public UISprite m_IconSprite;
    public UISprite m_QualitySprite;
    public UILabel m_NameLable;
    public UILabel m_LevLable;
    public UILabel m_CountLable;
    public UILabel m_PriceLable;
    public UILabel m_owneName;
    private int m_nIndex = -1;

    void ClickItem()
    {
        if (ConsignSaleLogic.Instance() != null)
        {
//            if (ConsignSaleLogic.Instance().CurClickBuyItem !=null)
//            {
//
//               ConsignSaleLogic.Instance().CurClickBuyItem.m_BakSprite.MakePixelPerfect();
//            }
//			m_BakSprite.spriteName = "ui_pub_038";
//            m_BakSprite.MakePixelPerfect();
//            ConsignSaleLogic.Instance().CurClickBuyItem = this;

			ConsignSaleLogic.Instance().ShowItem(this);
            ConsignSaleLogic.Instance().m_Menu.GetComponent<ConsignSaleMenu>().OnShowBuyMenu();
            ConsignSaleLogic.Instance().SelBuyIndex = m_nIndex;
        }
    }

    public void SetItemIndex(int nIndex)
    {
        m_nIndex = nIndex;
    }
    //更新物品条目信息
    public  void UpdateItemInfo(ConsignSaleSearchInfo SearchItemInfo)
    {
        gameObject.SetActive(true);
        m_IconSprite.spriteName = SearchItemInfo.ItemInfo.GetIcon();
        m_IconSprite.MakePixelPerfect();
        m_QualitySprite.spriteName = SearchItemInfo.ItemInfo.GetQualityFrame();
        m_QualitySprite.MakePixelPerfect();
        m_NameLable.text = SearchItemInfo.ItemInfo.GetName();
        m_LevLable.text = SearchItemInfo.ItemInfo.GetMinLevelRequire().ToString();
        m_CountLable.text = SearchItemInfo.ItemInfo.StackCount.ToString();
        m_PriceLable.text = SearchItemInfo.Price.ToString();
        m_owneName.text = SearchItemInfo.OwnerName;
    }

	public void EnableHighlight(bool bEnable)
	{
		if (bEnable == true) 
		{
			//换成高亮图片
				m_BakSprite.spriteName = "ui_pub_038";
		
		} 
		else 
		{

				m_BakSprite.spriteName = "ui_pub_040";
	
	
		}
	}
	// Use this for initialization
	void Start () {
	
	}
}
