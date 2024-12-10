using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GCGame.Table;

public class SGRegistrationDayRootManager : MonoBehaviour {


	public GameObject RegItem;
	public GameObject ItemGrid;

	public UILabel tipsLabel;
	public UILabel btnLabel;

	public UIButton  regBtn;

	public GameObject helpObject;
	public UILabel helpDescLabel;
	// Use this for initialization
	void Start () {

	}
	void OnEnabled()
	{
		ButtonRegDayAward ();
	}

	// Update is called once per frame
	void Update () {
	
	}

	public void ButtonRegDayAward()
	{
		CleanUp();
		InitUI();
	}

	private void CleanUp()
	{
		foreach (Transform child in ItemGrid.transform)
			Destroy (child.gameObject);

		ItemGrid.transform.localPosition = new Vector3 (-452f, 222f, 0f);
		helpObject.SetActive (false);
		string str = StrDictionary.GetClientDictionaryString("#{4643}");
		helpDescLabel.text = str;
	}
	private int currID;
	private int currType;
	private int tempIndex;
	private int ConsumeNum;

	public void InitUI()
	{

		//=============
		int num1 = GameManager.gameManager.PlayerDataPool.AwardActivityData.Monthcount;
		int num2 = GameManager.gameManager.PlayerDataPool.AwardActivityData.Monthmax;

		string str = StrDictionary.GetClientDictionaryString("#{4641}", num1,num2);
		tipsLabel.text = str;//"本月已累计签到"+num1+"次，目前最大可签到次数"+num2+"次。";

		tempIndex = 0;
		for(int i=0;i<60;i++)
		{
			List<Tab_Reward> _curveList = TableManager.GetRewardByID(i);
			if(_curveList != null && _curveList[0].Type == 3 )
			{
				GameObject itemObj1 = (GameObject)Instantiate (RegItem);
				itemObj1.name = "item0"+tempIndex;
				itemObj1.transform.parent = ItemGrid.transform;
				itemObj1.transform.localScale = Vector3.one;
				itemObj1.transform.localPosition = new Vector3( 0 + 150 * (tempIndex%7) , 0 - 150 * (int)(tempIndex/7) , 0f);

				int num3 = 	GameManager.gameManager.PlayerDataPool.AwardActivityData.DailyFlagList[tempIndex];
				itemObj1.GetComponent<SGRegistrationDayItem>().SetData(_curveList,num3);
				itemObj1.GetComponent<SGRegistrationDayItem> ().m_IndexNum = tempIndex;
				if(num1 == tempIndex)
				{
					currID = _curveList[0].RewardsId;
					ConsumeNum =  _curveList[0].Value;
					itemObj1.GetComponent<SGRegistrationDayItem> ().m_selectSprite.enabled = true;
					ItemObject = itemObj1;
				}

				tempIndex++;
			}
	
		}
//		ItemGrid.GetComponent<UIGrid> ().Reposition ();

		//=============flag 判断签到还是补签
		int flagNum = GameManager.gameManager.PlayerDataPool.AwardActivityData.Flag;
		regBtn.isEnabled = true;
		if(flagNum == 0)
		{
			btnLabel.text = "签 到";
			currType = 1;
		}else{
			if(num2 > num1)
			{
				btnLabel.text = "补 签";
				currType = 2;
			}else{
				regBtn.isEnabled = false;
			}
		}
		//============
	
	}

	//==============
	private void OnRegBtn()
	{

		if(currType == 1)
		{
			CG_ASK_DAILYAWARD regRewardPak = (CG_ASK_DAILYAWARD)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_DAILYAWARD);
			regRewardPak.Awardid = (uint)currID;
			regRewardPak.Awardtype = (uint)currType;
			regRewardPak.SendPacket();
		}else if(currType == 2){
			//ConsumeNum
			string str = StrDictionary.GetClientDictionaryString("#{4703}", ConsumeNum);
			MessageBoxLogic.OpenOKCancelBox(str, "", BuyOnClick, ConsumeCancel);
		}
	}

	//=========
	void BuyOnClick()
	{

		int nPlayerYuanBaoBind = GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBaoBind();
		int nPlayerYuanBao = GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBao();
		
		if (nPlayerYuanBaoBind < ConsumeNum)
		{
			// 元宝补充
			int nRepairYuanBao = ConsumeNum - nPlayerYuanBaoBind;
			if (nRepairYuanBao <= nPlayerYuanBao)
			{
				MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{1849}", nRepairYuanBao), "", ConsumeOK, ConsumeCancel);
			}
			else
			{
				// 元宝不足
				MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{1848}"), "", DoPay, ConsumeCancel);               
			}
		}
		else
		{
			ConsumeOK();
		}           
	}
	//==========
	private void DoPay()
	{
		RechargeData.PayUI();
	}

	private void ConsumeOK()
	{
		CG_ASK_DAILYAWARD regRewardPak = (CG_ASK_DAILYAWARD)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_DAILYAWARD);
		regRewardPak.Awardid = (uint)currID;
		regRewardPak.Awardtype = (uint)currType;
		regRewardPak.SendPacket();
	}
	private void ConsumeCancel(){}

	private void OnHelpBtn()
	{
		helpObject.SetActive (true);
	}

	//========
	private void OnHelpCloseBtn()
	{
		helpObject.SetActive (false);
	}

	//=============
	private GameObject ItemObject = null;
	private void OnItemBtnFun(GameObject strBtn)
	{

	
		if(ItemObject != null)
		{
			ItemObject.GetComponent<SGRegistrationDayItem> ().m_selectSprite.enabled = false;
		}

		strBtn.transform.parent.gameObject.GetComponent<SGRegistrationDayItem> ().m_selectSprite.enabled = true;
		ItemObject = strBtn.transform.parent.gameObject;

		//====弹出tips
		int goodsId = ItemObject.GetComponent<SGRegistrationDayItem> ().m_currGoodsId;
		ItemTooltipsLogic.ShowItemTooltip(goodsId, ItemTooltipsLogic.ShowType.Info);

		int IndexNum = ItemObject.GetComponent<SGRegistrationDayItem> ().m_IndexNum;
		int num3 = 	GameManager.gameManager.PlayerDataPool.AwardActivityData.DailyFlagList[IndexNum];

		List<Tab_Reward> thisReward = ItemObject.GetComponent<SGRegistrationDayItem> ().thisReward;

		currID = thisReward[0].RewardsId;
		ConsumeNum =  thisReward[0].Value;
		regBtn.isEnabled = true;
		if(num3 == 2)
		{

			btnLabel.text = "补 领";
			currType = 1;
		}else if(num3 == 1){

			btnLabel.text = "已领取";
			currType = 3;
			regBtn.isEnabled = false;
		}else if(num3 == 0)
		{
			int num2 = GameManager.gameManager.PlayerDataPool.AwardActivityData.Monthmax;
			if( IndexNum+1 <= num2 )
			{
				int flagNum = GameManager.gameManager.PlayerDataPool.AwardActivityData.Flag;
				if(flagNum == 0)
				{
					btnLabel.text = "签 到";
					currType = 1;
				}else{
					btnLabel.text = "补 签";
					currType = 2;
				}

			}else{
				btnLabel.text = "不可领取";
				currType = 3;
				regBtn.isEnabled = false;
			}

		}

	}

}
