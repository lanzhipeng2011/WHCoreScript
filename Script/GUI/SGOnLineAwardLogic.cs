using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GCGame.Table;

public class SGOnLineAwardLogic : MonoBehaviour {

	public GameObject m_RewardItemObj;
	public GameObject m_RewardItemGrid;
	public GameObject m_AwardBtn;

	public UILabel m_LeftTimeText;

	private int m_LeftTime;
	public int LeftTime
	{
		set { 
			m_LeftTime = value;
			UpdateLeftTime();
		}
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		UpdateLeftTime ();
	}

	// 在线奖励按钮
	public void ButtonOnlineAward()
	{
		CleanUp();
		init();
//		UpdateWindow();
//		//CreateAwardItemList();
//		//UpdateTips();
		UpdateLeftTime();
//		//ShowAward();
	}

	// 剩余时间更新
	void UpdateLeftTime()
	{
		m_LeftTime = GameManager.gameManager.PlayerDataPool.AwardActivityData.LeftTime;
		int nHour = m_LeftTime / (60 * 60);
		int nMin = (m_LeftTime % (60 * 60)) / 60;
		int nSec = m_LeftTime % 60;
		if (m_LeftTimeText != null)
		{
			//m_LeftTimeText.text = "距离下次领奖：" + nHour/10 + nHour%10 + ":" + nMin/10 + nMin%10 + ":" + nSec/10 + nSec%10;
			m_LeftTimeText.text = StrDictionary.GetClientDictionaryString("#{2868}", nHour / 10, nHour % 10, nMin / 10, nMin % 10, nSec / 10 , nSec % 10);
		}
	}
	
	private void CleanUp()
	{
		foreach (Transform child in m_RewardItemGrid.transform)
						Destroy (child.gameObject);

		m_LeftTime = -1;
	}
	private int tempIndex;
	private void init()
	{
		tempIndex = 0;
		for(int i=0;i<30;i++)
		{
			List<Tab_Reward> _curveList = TableManager.GetRewardByID(i);
			if(_curveList != null && _curveList[0].Type == 1 )
			{
				GameObject rewardItem = (GameObject) Instantiate(m_RewardItemObj);
				rewardItem.name = "item0"+tempIndex;
				rewardItem.transform.parent = m_RewardItemGrid.transform;
				rewardItem.transform.localScale = Vector3.one;
				rewardItem.transform.localPosition = new Vector3(45f + 180f * tempIndex, -20f,0f);
				tempIndex++;

				rewardItem.GetComponent<RewardItemManager>().initData(_curveList,1);
			}
		}
		m_RewardItemGrid.GetComponent<UIGrid> ().Reposition ();

		if(m_AwardBtn)
		{
			int m_OnlineAwardID = GameManager.gameManager.PlayerDataPool.AwardActivityData.OnlineAwardID;
			if(m_OnlineAwardID == 0)
				m_AwardBtn.SetActive(false);
			else
				m_AwardBtn.SetActive(true);
		}

	}

	private void OnAwardClick()
	{
		int rewardID = GameManager.gameManager.PlayerDataPool.AwardActivityData.OnlineAwardID;
		CG_ASK_ONLINEAWARD packet = (CG_ASK_ONLINEAWARD)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_ONLINEAWARD);
		packet.SetOnlineAwardID((uint)rewardID);
		packet.SendPacket();
	}

}
