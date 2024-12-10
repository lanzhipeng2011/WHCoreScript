using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GCGame.Table;
using Games.Item;


public class ActiveRechargeController : MonoBehaviour {

	public enum CONTENT_TYPE
	{
		CONTENT_TYPE_INVALID        = -1,
		CONTENT_TYPE_FIRST           = 0,        // 首充礼包
		CONTENT_TYPE_GRADE         = 1,        // 等级基金
		CONTENT_TYPE_GROW,                     // 成长基金
		CONTENT_TYPE_RECHARGE,                     // 累计充值
		CONTENT_TYPE_DAILERECHARGE ,                     // 每日充值
		CONTENT_TYPE_CUMULATIVE,                     // 累计消费
		CONTENT_TYPE_DAILYCONSUMPTION,                     // 每日消费
		CONTENT_TYPE_WEEK ,                     // 周卡月卡
		CONTENT_TYPE_OPINION ,					 // 意见
		CONTENT_TYPE_BANGDING					//绑定
	}
	
	public UISprite m_TitleLable;
	public List<UIButtonMessage> m_TabButtonList;
	public List<GameObject> m_ContentList; 
	
	public GameObject m_Item;
	public static CONTENT_TYPE m_Content_type=CONTENT_TYPE.CONTENT_TYPE_INVALID;
	

	void Awake()
	{

	}
	
	// Use this for initialization
	void Start () 
	{
		ShowFrist ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void ShowContent(CONTENT_TYPE eContent)
	{
		for (int i = 0; i < m_ContentList.Count; i++) 
		{
			m_ContentList[i].SetActive(false);
			m_TabButtonList[i].GetComponent<TabButton>().objHighLight.SetActive(false);
		}
		int index = (int)eContent + 1;
//		m_TitleLable.spriteName = "ui_charge_0" + index.ToString;
		m_ContentList [(int)eContent].SetActive (true);
		m_TabButtonList [(int)eContent].GetComponent<TabButton> ().objHighLight.SetActive (true);
	}
		
	void OnCloseClick()
	{
		UIManager.CloseUI(UIInfo.PackageRoot);
	} 
	
	public void ShowFrist()
	{
		m_Content_type = CONTENT_TYPE.CONTENT_TYPE_FIRST;
		ShowContent(CONTENT_TYPE.CONTENT_TYPE_FIRST);

	}

	public void ShowGrade()
	{
		m_Content_type = CONTENT_TYPE.CONTENT_TYPE_GRADE;
		ShowContent(CONTENT_TYPE.CONTENT_TYPE_GRADE);

	}

	public void ShowGrow()
	{
		m_Content_type = CONTENT_TYPE.CONTENT_TYPE_GROW;
		ShowContent(CONTENT_TYPE.CONTENT_TYPE_GROW);
	}

	public void ShowRecharge()
	{
		m_Content_type = CONTENT_TYPE.CONTENT_TYPE_RECHARGE;
		ShowContent(CONTENT_TYPE.CONTENT_TYPE_RECHARGE);
		SendGC ();
	}
	
	public void ShowDailyRecharge()
	{
		m_Content_type = CONTENT_TYPE.CONTENT_TYPE_DAILERECHARGE;
		ShowContent(CONTENT_TYPE.CONTENT_TYPE_DAILERECHARGE);
		SendGC ();

	}
	
	public void ShowCumulative()
	{
		m_Content_type = CONTENT_TYPE.CONTENT_TYPE_CUMULATIVE;
		ShowContent(CONTENT_TYPE.CONTENT_TYPE_CUMULATIVE);
		SendGC ();
	}

	public void ShowDailyCumulative()
	{
		m_Content_type = CONTENT_TYPE.CONTENT_TYPE_DAILYCONSUMPTION;
		ShowContent(CONTENT_TYPE.CONTENT_TYPE_DAILYCONSUMPTION);
		SendGC ();
	}

	public void ShowWeek()
	{
		m_Content_type = CONTENT_TYPE.CONTENT_TYPE_WEEK;
		ShowContent(CONTENT_TYPE.CONTENT_TYPE_WEEK);

	}

	public void ShowOpinion()
	{
		m_Content_type = CONTENT_TYPE.CONTENT_TYPE_OPINION;
		ShowContent(CONTENT_TYPE.CONTENT_TYPE_OPINION);
	}

	public void ShowBangDing()
	{
		m_Content_type = CONTENT_TYPE.CONTENT_TYPE_BANGDING;
		ShowContent(CONTENT_TYPE.CONTENT_TYPE_BANGDING);
	}
	//===4 累计充值  5 每日累冲  6 累计消费  7 每日消费
	private void SendGC()
	{
		CG_ACK_ACTIVE_LEICHONG  msg = (CG_ACK_ACTIVE_LEICHONG)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ACK_ACTIVE_LEICHONG);
		msg.SetType ((uint)((int)m_Content_type + 1));
		msg.SendPacket ();
	}

	void OnClickItem(GameObject go)
	{
		
		GameItem item = new GameItem();
		item.DataID =int.Parse( go.name);
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
