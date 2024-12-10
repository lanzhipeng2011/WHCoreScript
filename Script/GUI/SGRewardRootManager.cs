
/************************************************************************/
/* 文件名：SGRewardRootManager.cs    
 * 创建日期：2015.11.25
 * 创建人：兰兰
 * 功能说明：在线奖励、等级奖励
/************************************************************************/

using UnityEngine;
using System.Collections;
using Module.Log;
using GCGame.Table;
using Games.UserCommonData;

public class SGRewardRootManager : UIControllerBase<SGRewardRootManager>
{
	public TabController m_TabButton; 

	public SGOnLineAwardLogic m_OnlineAwardRoot;
	public GameObject m_OnlineTips;
	public UILabel m_OnlineTipLabel;

	public SGDayAwardLogic m_DayAwardRoot;
	public GameObject m_DayTips;
	public UILabel m_DayTipLabel;

	public SGDL7DayManager m_DL7DayAwardRoot;
	public GameObject m_DL7DayTips;
	public UILabel m_DL7DayTipLabel;

	public SGRegistrationDayRootManager m_RegDayAwardRoot;
	public GameObject m_DailyTips;
	public UILabel m_DailyTipLabel;

	public UILabel[] titleList; 
	//////////////////////////////////
	void Awake()
	{
		SetInstance(this);
		m_TabButton.delTabChanged = OnTabChange;
	}

	// Use this for initialization
	void Start () {
	
	}

	void OnEnable()
	{
		#if UNITY_IPHONE || UNITY_WP8 || UNITY_BLACKBERRY   //|| UNITY_ANDROID
		TouchScreenKeyboard.hideInput = false;		
		#endif
//		CleanUp();
		UpdateTip();
		ShowWindow();
//		if (delegateAfterCall != null)
//		{
//			delegateAfterCall();
//			delegateAfterCall = null;
//		}
	}

	public void UpdateTip()
	{

//		int onlineNum = GameManager.gameManager.PlayerDataPool.AwardActivityData.OnlineRewardTipNum;
		int onlineNum=GameManager.gameManager.PlayerDataPool.AwardActivityData.LeftTime;
		int dayNum = GameManager.gameManager.PlayerDataPool.AwardActivityData.LevelRewardTipNum;
		int dailyNum = GameManager.gameManager.PlayerDataPool.AwardActivityData.DailyRewardTipNum;
		int dl7DayNum = GameManager.gameManager.PlayerDataPool.AwardActivityData.DL7RewardTipNum;


		if(dailyNum > 0)
		{
			m_DailyTips.SetActive(true);
			m_DailyTipLabel.text = dailyNum.ToString();
		}else{
//			m_OnlineTips.SetActive(false);
//			m_OnlineTipLabel.text = "";
			m_DailyTips.SetActive(false);
			m_DailyTipLabel.text="";
		}
//		if(onlineNum > 0)
		if(onlineNum == 0)
		{
			m_OnlineTips.SetActive(true);
//			m_OnlineTipLabel.text = onlineNum.ToString();
			m_OnlineTipLabel.text ="1";
		}else{
			m_OnlineTips.SetActive(false);
			m_OnlineTipLabel.text = "";
		}
		if(dayNum > 0)
		{
			m_DayTips.SetActive(true);
			m_DayTipLabel.text = dayNum.ToString();
		}else{
			m_DayTips.SetActive(false);
			m_DayTipLabel.text = "";
		}
		if(dl7DayNum > 0)
		{
			m_DL7DayTips.SetActive(true);
			m_DL7DayTipLabel.text = dl7DayNum.ToString();
		}else{
			m_DL7DayTips.SetActive(false);
			m_DL7DayTipLabel.text = "";
		}

	}

	void ShowWindow()
	{
		if (null == m_TabButton)
		{
			return;
		}
		m_TabButton.ChangeTab("Button2-Award");
	}
	// Update is called once per frame
	void Update () {
	
	}

	void OnDestroy()
	{
		SetInstance(null);
	}
	void CloseWindow()
	{
		UIManager.CloseUI(UIInfo.RewardRoot);
	}

	void OnTabChange(TabButton button)
	{

		clearLabelState ();

		if (button.name == "Button1-Award")
		{
//			ShowNewServerAward();
		}
		else if (button.name == "Button2-Award")
		{
			ShowOnlineAward();
			changeLabelColor(0);
		}
		else if (button.name == "Button3-Level-Award")
		{
			ShowDayAward();
			changeLabelColor(1);
		}
		else if (button.name == "Button4-Award")
		{
//			ShowNewOnlineAward();
		}
		else if (button.name == "Button7-Jihuo-Award")
		{
//			ShowCDkey();
		}
		else if (button.name == "Button8-Award")
		{
//			Show7DayNewOnlineAward();
		}
		else if (button.name == "Button9-Award")
		{
			ShowRegDayAward();
			changeLabelColor(2);
		}else if (button.name == "Button12-DL7Day-Award")
		{
			ShowDL7DayAward();
			changeLabelColor(2);
		}
	}
	public void ShowDL7DayAward()
	{
		if (m_DL7DayAwardRoot != null)
		{
			m_DL7DayAwardRoot.ButtonDL7DayAward();
		}
	}
	public void ShowDayAward()
	{
		if (m_DayAwardRoot != null)
		{
			m_DayAwardRoot.ButtonDayAward();
		}
	}

	public void ShowRegDayAward()
	{
		if (m_RegDayAwardRoot != null)
		{
			m_RegDayAwardRoot.ButtonRegDayAward();
		}
	}

	public void ShowOnlineAward()
	{
		if (m_OnlineAwardRoot != null)
		{
			m_OnlineAwardRoot.ButtonOnlineAward();
		}
	}

	private void clearLabelState()
	{
//		for(int i = 0 ; i<titleList.Length; i++)
//		{
//			titleList[i].color = new Color(25f/255f,105f/255f,180f/255f);
//			titleList[i].effectColor =  new Color(0f/255f,15f/255f,73f/255f);
//			titleList[i].effectDistance = new Vector2(3,3);
//		}
	}
	private void changeLabelColor(int index)
	{
//		titleList[index].color = new Color(217f/255f,241f/255f,254f/255f);
//		titleList[index].effectColor = new Color(15f/255f,94f/255f,239f/255f,102f/255f);
//		titleList[index].effectDistance = new Vector2(2,2);

	}

}
