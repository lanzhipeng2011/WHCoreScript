using UnityEngine;
using System.Collections;
using GCGame.Table;

public class CDKeyRootLogic : UIControllerBase<CDKeyRootLogic>
{
	public UILabel      m_Input;

	void CleanUp()
	{
        m_Input.text = "";
	}
    void Awake()
    {
        SetInstance(this);
    }

	void Start () 
    {
        CleanUp();
	}	
	
    void OnDestroy()
    {
        SetInstance(null);
    }

    void OnCloseButtonClick()
    {
		UIManager.CloseUI(UIInfo.CDkeyRoot);
    }       

	void OnSend( )
	{
        if ( m_Input.text.Length > 0)
        {
            CG_REQUEST_CDKEY packet = (CG_REQUEST_CDKEY)PacketDistributed.CreatePacket(MessageID.PACKET_CG_REQUEST_CDKEY);
               packet.Cdkeystr = m_Input.text;
               packet.SendPacket();
               OnCloseButtonClick();
        }
	}
}
