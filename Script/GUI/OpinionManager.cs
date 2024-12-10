using UnityEngine;
using System.Collections;
using System;

public class OpinionManager : MonoBehaviour {

	private int type = 1;
	public UIInput m_InputDesc;
	public UIInput m_InputNum;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnBugBtnFun()
	{
		type = 1;
	}
	public void OnJianYiBtnFun()
	{
		type = 2;
	}
	public void OnChongZhiBtnFun()
	{
		type = 3;
	}
	public void OnOtherBtnFun()
	{
		type = 4;
	}

	void OnCommintBtnFun()
	{
		//======
//		type  desc.value  num.value

		UInt64 numbers = 0;

		if(IsHandset (m_InputNum.value))
		{
			numbers = UInt64.Parse (m_InputNum.value);
		}else{
			MessageBoxLogic.OpenOKBox(6019);
			return;
		}

		string descs = "";
		if(!string.IsNullOrEmpty(m_InputDesc.value))
		{
			descs = m_InputDesc.value;
		}
		else
		{
			MessageBoxLogic.OpenOKBox(6022);
			return;
		}


		CG_SEND_FANKUI packet = (CG_SEND_FANKUI)PacketDistributed.CreatePacket(MessageID.PACKET_CG_SEND_FANKUI);

		packet.SetAccount (LoginData.accountData.m_account);
		packet.SetType ((uint)type);
		packet.SetDesc (descs);
		packet.SetNum (numbers);

		packet.SendPacket();



	}

	public bool IsHandset(string str_handset)
	{
		if(str_handset.Length == 11)
			return System.Text.RegularExpressions.Regex.IsMatch(str_handset, @"^[1]+[3,4,5,7,8,9]+\d{9}");
		
		return false;
	}
}
