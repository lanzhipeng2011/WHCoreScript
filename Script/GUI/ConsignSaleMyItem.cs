/********************************************************************
	创建时间:	2014/06/12 13:13
	全路径:		\TLOL\Version\Main\Project\Client\Assets\MLDJ\Script\GUI\ConsignSaleMyItem.cs
	创建人:		luoy
	功能说明:	寄售行上架物品信息条目
	修改记录:
*********************************************************************/
using Games.ConsignSale;
using Games.LogicObj;
using GCGame.Table;
using UnityEngine;
using System.Collections;

public class ConsignSaleMyItem : MonoBehaviour
{
    public UISprite m_IconSprite;
    public UISprite m_QualitySprite;
    public UILabel m_NameLable;
    public UILabel m_RemainTimeLable;
    public UILabel m_CountLable;
    public UILabel m_PriceLable;
    private int m_nIndex = 0;
    private MyConsignSaleItemInfo m_MyItemInfo;
    public void SetItemIndex(int nIndex)
    {
        m_nIndex = nIndex;
    }
    void ClickItem()
    {
        if (ConsignSaleLogic.Instance() != null)
        {
            ConsignSaleLogic.Instance().SelSaleIndex = m_nIndex;
            ConsignSaleLogic.Instance().CancelSaleItem();
        }
    }
    //更新物品条目信息
    public void UpdateItemInfo(MyConsignSaleItemInfo SearchItemInfo)
    {
        gameObject.SetActive(true);
        m_MyItemInfo = new MyConsignSaleItemInfo();
        m_MyItemInfo.CleanUp();
        m_MyItemInfo = SearchItemInfo;
        m_IconSprite.spriteName = SearchItemInfo.ItemInfo.GetIcon();
        m_IconSprite.MakePixelPerfect();
        m_QualitySprite.spriteName = SearchItemInfo.ItemInfo.GetQualityFrame();
        m_QualitySprite.MakePixelPerfect();
        m_NameLable.text = SearchItemInfo.ItemInfo.GetName();
        if (SearchItemInfo.RemainTime/3600.0f >=1)
        {
            m_RemainTimeLable.text = StrDictionary.GetClientDictionaryString("#{2747}", SearchItemInfo.RemainTime/3600);
        }
        else
        {
            m_RemainTimeLable.text =StrDictionary.GetClientDictionaryString("#{2748}");
        }
        m_CountLable.text = SearchItemInfo.ItemInfo.StackCount.ToString();
        m_PriceLable.text = SearchItemInfo.Price.ToString();
    }
    void ShareItem()
    {
        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (_mainPlayer ==null)
        {
            return;
        }
        float fElspeTime = Time.time - GameManager.gameManager.PlayerDataPool.LastConsignShareTime;
        //加一个操作CD
        int nMaxCDTime = 30;
        if (fElspeTime < nMaxCDTime)
        {
            int nRemainTime = (int)(nMaxCDTime - fElspeTime);
            //吆喝操作过于频繁,距离下次可吆喝时间还剩{0}秒
            _mainPlayer.SendNoticMsg(false, "#{2749}", nRemainTime+1);
            return;
        }
     
        string strAdditionShareMsg = StrDictionary.GetClientDictionaryString("#{2743}",
         m_MyItemInfo.ItemInfo.StackCount, m_MyItemInfo.ItemInfo.GetName(), m_MyItemInfo.Price);
        if (m_MyItemInfo.ItemInfo.IsEquipMent())
        {
            ShareTargetChooseLogic.InitEquipShare(m_MyItemInfo.ItemInfo,strAdditionShareMsg);
        }
        else
        {
            ShareTargetChooseLogic.InitItemShare(m_MyItemInfo.ItemInfo,strAdditionShareMsg);
        }
    }
	// Use this for initialization
	void Start () 
    {
        
	}
}
