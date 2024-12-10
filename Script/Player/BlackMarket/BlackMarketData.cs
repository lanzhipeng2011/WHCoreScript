using Games.Item;
using UnityEngine;
using System.Collections;
using System;
public enum BLACKMARKETDATE
{
    MAXNUMPAGE =6,//每页最多六个
}
public struct BlackMarketItemInfo
{
	private int m_nItemIndex;
	public int ItemIndex
    {
        get { return m_nItemIndex; }
        set { m_nItemIndex = value; }
    }
    private GameItem m_ItemInfo ;
    public Games.Item.GameItem ItemInfo
    {
        get { return m_ItemInfo; }
        set { m_ItemInfo = value; }
    }
    private int m_nItemCount;
    public int ItemCount
    {
        get { return m_nItemCount; }
        set { m_nItemCount = value; }
    }
    private int m_nItemPrice;
    public int ItemPrice
    {
        get { return m_nItemPrice; }
        set { m_nItemPrice = value; }
    }
    private int m_nItemMoneyType;
    public int ItemMoneyType
    {
        get { return m_nItemMoneyType; }
        set { m_nItemMoneyType = value; }
    }

    public void CleanUp()
    {
        m_nItemIndex = -1;
        m_ItemInfo =new GameItem();
        m_nItemCount = 0;
        m_nItemPrice = 0;
        m_nItemMoneyType = -1;
    }

    public bool IsVaild()
    {
        return (m_nItemIndex!= -1 && m_ItemInfo.IsValid());
    }
}