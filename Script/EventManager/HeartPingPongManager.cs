using UnityEngine;
using System.Collections;

public class HeartPingPongManager : MonoBehaviour {

	/**
	屏蔽该逻辑（取消主动心跳），原因为当玩家信号弱时允许客户端长时间等待重连
	 */


	private int timeNumber = 0;
	// Use this for initialization
	void Start () {
		//#if UNITY_EDITOR
		//#else
			initEvent ();
		//#endif
	}
	
	// Update is called once per frame
	void Update () {

		#if UNITY_EDITOR
			//return;
		#endif

		if(!isFirst)
			return;

		if(timeNumber>0)
		{
			timeNumber--;
		}else{
			BeginPingPong();
		}
	}

	private void initEvent ()
	{
		//StartCoroutine (BeginPingPong (3f));
		EventManager.instance.addEventListener ("StartHeartPingPong", this.gameObject, "StartPingPong");
		//ReceiveHeartPingPong
		EventManager.instance.addEventListener ("ReceiveHeartPingPong", this.gameObject, "ReceivePingPong");
		//DontDestroyOnLoad (this.gameObject);
	}

	private void ReceivePingPong()
	{
		rectSucess = true;
	}

	private void StartPingPong()
	{
		isFirst = true;
		timeNumber = 200;
		rectSucess = true;
	}

	private bool rectSucess = true;
	private bool isFirst = true;

	void BeginPingPong()
	{
		if(!rectSucess)
		{
			NetManager.Instance().ConnectLost();
			isFirst = false;
		}else{
			CG_CONNECTED_HEARTBEAT cgBeat = (CG_CONNECTED_HEARTBEAT)PacketDistributed.CreatePacket(MessageID.PACKET_CG_CONNECTED_HEARTBEAT);
			cgBeat.SetIsresponse(0);
			cgBeat.SendPacket();
			timeNumber = 200;
			rectSucess = false;
		}
	}
}
