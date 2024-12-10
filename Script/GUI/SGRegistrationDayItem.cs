using UnityEngine;
using System.Collections;
using GCGame.Table;
using Games.GlobeDefine;
using System.Collections.Generic;

public class SGRegistrationDayItem : MonoBehaviour {

	public UISprite m_Icon;
	public UISprite m_Quilaty;
	public UISprite m_IsOver;
	public UISprite m_IsUnOver;
//	public UILabel m_VipLevel;
	public UISprite m_VipSprite;
	public UILabel m_NumberLabel;

	public UISprite m_selectSprite;

	public int m_IndexNum;
	public List<Tab_Reward> thisReward;

	public int m_currGoodsId;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool SetData( List<Tab_Reward> _curveList , int isGet)
	{
		m_currGoodsId = _curveList [0].GoodsID;

		m_selectSprite.enabled = false;

		Tab_CommonItem curItem = TableManager.GetCommonItemByID(_curveList[0].GoodsID, 0);
		if(null == curItem)
		{
			return false;
		}

		thisReward = _curveList;

		switch(isGet)
		{
		case 0:
			m_IsOver.enabled = false;
			m_IsUnOver.enabled = false;
			break;
		case 1:
			m_IsOver.enabled = true;
			m_IsUnOver.enabled = false;
			break;
		case 2:
			m_IsOver.enabled = false;
			m_IsUnOver.enabled = true;
			break;
		}

		m_Icon.spriteName = curItem.Icon;
		m_Icon.depth = 111;
		if(_curveList[0].Goodsnum > 0)
		{
			m_NumberLabel.text = "";
			m_NumberLabel.text = _curveList[0].Goodsnum.ToString();
		}
		else
		{
			m_NumberLabel.text = "";
		}

		if(_curveList[0].Vip > 0)
		{
//			m_VipLevel.text = "";
//			string str = StrDictionary.GetClientDictionaryString("#{4}", _curveList[0].Vip);
			m_VipSprite.enabled = true;
			m_VipSprite.spriteName = "ui_welfare_"+(14+_curveList[0].Vip).ToString();
//			m_VipLevel.text = "VIP " +_curveList[0].Vip.ToString() + "双倍";
		}
		else
		{
			m_VipSprite.enabled = false;
//			m_VipLevel.text = "";
			m_VipSprite.spriteName = "";
		}



		int colorQuality = curItem.Quality - 1;
		if (colorQuality >= 0 && colorQuality < GlobeVar.QualityColorGrid.Length)
		{
			m_Quilaty.spriteName = GlobeVar.QualityColorGrid[curItem.Quality - 1];
		}
		return true;
	}
}
