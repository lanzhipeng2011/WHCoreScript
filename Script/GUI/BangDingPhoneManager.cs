using UnityEngine;
using System.Collections;
using GCGame.Table;

public class BangDingPhoneManager : MonoBehaviour {

	public UIInput m_InputNum;
	public UIGrid m_Grid;
	public GameObject m_Item;

	// Use this for initialization
	void Start () {
		init ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnBangDingBtnFun()
	{
		string inputNum = m_InputNum.value;
		if(IsHandset (inputNum))
		{
			CG_ASK_BINDNUMBER packet = (CG_ASK_BINDNUMBER)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_BINDNUMBER);

			packet.SetAccount (LoginData.accountData.m_account);
			packet.SetNum (inputNum);
			packet.SendPacket();
		}else{
			MessageBoxLogic.OpenOKBox(6019);
		}
	}
	public bool IsHandset(string str_handset)
	{
		if(str_handset.Length == 11)
			return System.Text.RegularExpressions.Regex.IsMatch(str_handset, @"^[1]+[3,4,5,7,8,9]+\d{9}");

		return false;
	}


	private void init()
	{
		
		if (null != m_Item) 
		{
			Tab_Activity acti = TableManager.GetActivityByID (66, 0);
			if(acti == null)
				return;
			int n = acti.getItemCount ();
			for (int i=0; i<n; i++)
			{
				int index = acti.GetItembyIndex (i);
				if (index != 0) 
				{
					Tab_CommonItem item = TableManager.GetCommonItemByID (index, 0);
					GameObject rewardItem = (GameObject)Instantiate (m_Item);
					VipItem vip = rewardItem.GetComponent<VipItem> ();				
					vip.m_BonusImage.spriteName = item.Icon;
					vip.gameObject.name = item.Id.ToString();
					vip.m_BonusText.text =  acti.GetNumbyIndex(i).ToString();
					rewardItem.transform.parent = m_Grid.transform;
					rewardItem.transform.localScale = Vector3.one;
					rewardItem.transform.localPosition = new Vector3 (0 + 130f * i, 0f, 0f);							
					
				}
			}
		}
		
		
	}
}
