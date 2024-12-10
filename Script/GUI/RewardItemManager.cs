using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GCGame.Table;

public class RewardItemManager : MonoBehaviour {

	public GameObject itemObj;
	public GameObject rewardBtn;
	public UISprite isRewardSprite;
	public UILabel descLabel;
	public UISprite bgSprite;
	public UISprite timeSprite;
//	public int OnlineTipsNumber;
//	public int DayTipsNumber;

	private int rewardID;
	private int rewardType;

	private int currState;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void clearUp()
	{
		isRewardSprite.enabled = false;
		rewardBtn.gameObject.SetActive(false);
	}

	private void OnRewardBtnFun()
	{

		if(currState == 0)
		{
			return;
		}

		if(rewardType == 1)
		{
			CG_ASK_ONLINEAWARD packet = (CG_ASK_ONLINEAWARD)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_ONLINEAWARD);
			packet.SetOnlineAwardID((uint)rewardID);
			packet.SendPacket();
		}else if(rewardType == 2)
		{
			CG_ASK_LEVELAWARD packet = (CG_ASK_LEVELAWARD)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_LEVELAWARD);
			packet.SetAwardid(rewardID);
			packet.SendPacket();
		}else if(rewardType == 3)
		{
			CG_ASK_7DAYAWARD packet = (CG_ASK_7DAYAWARD)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_7DAYAWARD);
			packet.SetAwardid(rewardID);
			packet.SendPacket();
		}
	}

	public void initData(List<Tab_Reward> _curveList ,int type)
	{
		clearUp ();

		for(int i=0;i<_curveList.Count;i++)
		{
			GameObject itemObj1 = (GameObject)Instantiate (itemObj);
			itemObj1.transform.parent = this.transform;
			itemObj1.transform.localScale = Vector3.one;
			itemObj1.transform.localPosition = new Vector3(-215f + 110f * i , 0f, 0f);

			itemObj1.GetComponent<RewardItem>().SetData(_curveList[i].GoodsID,_curveList[i].Goodsnum);
		}
		if(type == 1)
		{
			showOnline(_curveList[0].RewardsId);
		}else if(type == 2){
			showLevel(_curveList[0].RewardsId);
		}else if(type == 3)
		{
			showDL7Day(_curveList[0].RewardsId);
		}

		rewardType = type;

		descLabel.text = _curveList [0].Desc;

		rewardID = _curveList [0].RewardsId;
	}


	private void showDL7Day(int id)
	{
		List<int> idList = GameManager.gameManager.PlayerDataPool.AwardActivityData.DL7RewardIDList;
		List<int> stateList = GameManager.gameManager.PlayerDataPool.AwardActivityData.DL7RewardStateList;
		int index1 = idList.IndexOf (id);
		if( index1 != -1)
		{
			currState = stateList[index1];
			switch(stateList[index1])
			{
			case 0:
				rewardBtn.gameObject.SetActive(true);
				rewardBtn.GetComponent<UIImageButton>().isEnabled = false;
				break;
			case 1:
				rewardBtn.gameObject.SetActive(false);
				isRewardSprite.enabled = true;
				break;
			case 2:
				rewardBtn.gameObject.SetActive(true);
				rewardBtn.GetComponent<UIImageButton>().isEnabled = true;
				//				DayTipsNumber ++;
				break;
			}
		}
	}

	private void showLevel(int id)
	{
//		DayTipsNumber = 0;
		List<int> idList = GameManager.gameManager.PlayerDataPool.AwardActivityData.LevelRewardIDList;
		List<int> stateList = GameManager.gameManager.PlayerDataPool.AwardActivityData.LevelRewardStateList;
		int index1 = idList.IndexOf (id);
		if( index1 != -1)
		{
			currState = stateList[index1];
			switch(stateList[index1])
			{
			case 0:
				rewardBtn.gameObject.SetActive(true);
				rewardBtn.GetComponent<UIImageButton>().isEnabled = false;
				break;
			case 1:
				rewardBtn.gameObject.SetActive(false);
				isRewardSprite.enabled = true;
				break;
			case 2:
				rewardBtn.gameObject.SetActive(true);
				rewardBtn.GetComponent<UIImageButton>().isEnabled = true;
//				DayTipsNumber ++;
				break;
			}
		}
	}

	private void showOnline(int id)
	{

		int m_OnlineAwardID = GameManager.gameManager.PlayerDataPool.AwardActivityData.OnlineAwardID;
		int m_LeftTime = GameManager.gameManager.PlayerDataPool.AwardActivityData.LeftTime;
		currState = 0;
		if(m_OnlineAwardID == 0)
		{
			isRewardSprite.enabled = true;
//			rewardBtn.gameObject.SetActive(false);
			bgSprite.spriteName = "ui_welfare_03";
		}else if(id < m_OnlineAwardID && m_OnlineAwardID != 0)
		{
			isRewardSprite.enabled = true;
//			rewardBtn.gameObject.SetActive(false);
			bgSprite.spriteName = "ui_welfare_03";
		}else if(id == m_OnlineAwardID && m_LeftTime == 0)
		{
//			rewardBtn.gameObject.SetActive(true);
//			rewardBtn.GetComponent<UIImageButton>().isEnabled = true;
			isRewardSprite.enabled = false;
			currState = 1;
		}
		if(id == m_OnlineAwardID)
		{
			bgSprite.spriteName = "ui_welfare_02";
		}

		//================
		if((id+3)==10)
		{
			timeSprite.spriteName = "ui_welfare_" + (id + 3).ToString ();
		}else{
			timeSprite.spriteName = "ui_welfare_0" + (id + 3).ToString ();
		}

//		OnlineTipsNumber = 0;
//		List<int> idList = GameManager.gameManager.PlayerDataPool.AwardActivityData.OnlineRewardIDList;
//		List<int> stateList = GameManager.gameManager.PlayerDataPool.AwardActivityData.OnlineRewardStateList;
//		int index1 = idList.IndexOf (id);
//		if( index1 != -1)
//		{
//			currState = stateList[index1];
//			switch(stateList[index1])
//			{
//			case 0:
//				rewardBtn.gameObject.SetActive(true);
//				rewardBtn.GetComponent<UIImageButton>().isEnabled = false;
//				break;
//			case 1:
//				rewardBtn.gameObject.SetActive(false);
//				isRewardSprite.enabled = true;
//				break;
//			case 2:
//				rewardBtn.gameObject.SetActive(true);
//				rewardBtn.GetComponent<UIImageButton>().isEnabled = true;
////				OnlineTipsNumber++;
//				break;
//			}
//		}
	}

}
