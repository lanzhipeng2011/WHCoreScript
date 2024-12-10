using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
using GCGame;
using GCGame.Table;

public class SNSShareCodeWindow : MonoBehaviour {
	// Use this for initialization

	public UILabel  m_inputCode;


	public void OnOkClick()
	{
		if (m_inputCode.text.Length > 0) 
		{
			CG_SNS_INVITE_CODE packet = (CG_SNS_INVITE_CODE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_SNS_INVITE_CODE);
			packet.Code = m_inputCode.text;
			packet.SendPacket();
		}
	}

	public void OnCloseClick()
	{
		UIManager.CloseUI (UIInfo.SNSShareCodeRoot);
	}
}
