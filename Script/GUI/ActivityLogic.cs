using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GCGame.Table;

public class ActivityLogic : MonoBehaviour
{
	

	public GameObject m_Grid;
	public GameObject m_Item;

	public UILabel  m_NumberLabel;
	public UILabel  m_TimeLabel;


	private static ActivityLogic m_Instance = null;
	public static ActivityLogic Instance()
	{
		return m_Instance;
	}


	private int Content_type;
	
	void Awake()
	{
		m_Instance = this;
	}
	
	void Start()
	{
	}
	
	void OnDestroy()
	{
		m_Instance = null;
	}

	void OnEnable()
	{
		if((int)ActiveRechargeController.m_Content_type != -1)
		{
			Content_type = (int)ActiveRechargeController.m_Content_type + 1;
			init(Content_type);
		}
	}

	private void ClearItem()
	{
		foreach (Transform child in m_Grid.transform)
			Destroy (child.gameObject);

	}


	public void OnDateChange()
	{
		switch((int)ActiveRechargeController.m_Content_type + 1)
		{

		case 4:
		case 5:
			int constNumbers = ActivityRechargeData.UserRechargeDataMap[(int)ActiveRechargeController.m_Content_type+1].Numbers * 10;
			m_NumberLabel.text = constNumbers.ToString();
			m_TimeLabel.text = ActivityRechargeData.UserRechargeDataMap[(int)ActiveRechargeController.m_Content_type+1].startTime +
				"----"+ActivityRechargeData.UserRechargeDataMap[(int)ActiveRechargeController.m_Content_type+1].endTime;
			break;
		case 6:
		case 7:
			m_NumberLabel.text = ActivityRechargeData.UserRechargeDataMap[(int)ActiveRechargeController.m_Content_type+1].Numbers.ToString();
			m_TimeLabel.text = ActivityRechargeData.UserRechargeDataMap[(int)ActiveRechargeController.m_Content_type+1].startTime +
				"----"+ActivityRechargeData.UserRechargeDataMap[(int)ActiveRechargeController.m_Content_type+1].endTime;
			break;
		}
	}



	public void OnClickChongZhi()
	{



		int payType = 0;
		switch((int)ActiveRechargeController.m_Content_type)
		{
		case 0:

			break;
		case 1:
			sendAskPayActivity(1);
			break;
		case 2:
			sendAskPayActivity(2);
			break;
		case 3:
		case 4:
		case 5:
		case 6:
		case 7:
			RechargeData.PayUI();
			break;
		}


	}

	void OnShopClick()
	{

		CG_ASK_YUANBAOSHOP_OPEN packet = (CG_ASK_YUANBAOSHOP_OPEN)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_YUANBAOSHOP_OPEN);
		packet.NoParam = 1;
		packet.SendPacket();
	}

	private void sendAskPayActivity(int payType)
	{
		CG_ASK_PAY_ACTIVITY  msg = (CG_ASK_PAY_ACTIVITY)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_PAY_ACTIVITY);
		msg.SetPrizetype ((uint)payType);
		msg.SendPacket ();
	}


	private int tempIndex;
	private void init(int type)
	{
		tempIndex = 0;

		ClearItem ();

		for(int i = 0; i < 999999; i++ )
		{
			Tab_Activity acti = TableManager.GetActivityByID (i, 0);
			if(acti != null && acti.Type == type)
			{
				GameObject rewardItem = (GameObject)Instantiate(m_Item);
				rewardItem.transform.parent = m_Grid.transform;
				rewardItem.transform.localScale = Vector3.one;
				rewardItem.transform.localPosition = new Vector3(0f,0f - 240f * tempIndex ,0f);
				tempIndex++;

				rewardItem.GetComponent<ActivelyItemManager>().init(acti);
			}
		}


		//m_Grid.GetComponent<UIGrid> ().Reposition ();
	}



}

