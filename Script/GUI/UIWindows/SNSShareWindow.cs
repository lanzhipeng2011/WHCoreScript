using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
using GCGame;
using GCGame.Table;

public class SNSShareWindow : MonoBehaviour {

	public UILabel m_labelShareDesc;

	// Use this for initialization
	void Start () 
	{
     	if (m_labelShareDesc != null) 
		{
			m_labelShareDesc.text = "ShareDesc";
		}
	}


	void SendSharePacket()
	{
		CG_SNS_SHARE packet = (CG_SNS_SHARE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_SNS_SHARE);
		packet.Sharetype = (int)ShareType.ShareType_SNS;
		packet.SendPacket();
	}

	public void OnWeiXinFriendClick()
	{
		SendSharePacket ();
	}

	public void OnWeiXinClick()
	{
		SendSharePacket ();
	}

	public void OnTXWeiBoClick()
	{
		SendSharePacket ();
	}

	public void OnSinaWeiBoClick()
	{
		SendSharePacket ();
	}

	public void OnCloseClick()
	{
		UIManager.CloseUI (UIInfo.SNSShareRoot);
	}


}
