using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
using GCGame;
using GCGame.Table;
using Games.Item;
using System;
using Games.UserCommonData;
public enum SNSRewardTarget
{
	SNSRTT_SHARE_USER,
	SNSRTT_JOIN_USER,
}

public enum SNSReward
{
	SNSRT_FIRST,
	SNSRT_LEVEL,
}

public class SNSWindow : MonoBehaviour {
	// Use this for initialization

	public SNSItemLogic []m_LevelRewardItem;
	public SNSItemLogic []m_ShareRewardItem;
	public UILabel m_labelTimes;
    public UILabel m_inputCode;

    public GameObject m_NanGuaButton;

	const int SNSREWARDTIMES = 20;


	void OnEnable()
	{
		int levelSet = 0, shareSet = 0;

        for (int i = 1; i < m_LevelRewardItem.Length; i++)
        {
            m_LevelRewardItem[i].ClearInfo();
        }

        for (int i = 1; i < m_ShareRewardItem.Length; i++)
        {
            m_ShareRewardItem[i].ClearInfo();
        }

		for( int  i = 1; i < 5 ; i++)
		{
			Tab_SNSReward reward = TableManager.GetSNSRewardByID(i, 0);
            if (reward != null)
            {
                if (reward.TargetType == (int)SNSRewardTarget.SNSRTT_SHARE_USER)
                {
                    if (shareSet < m_ShareRewardItem.Length)
                    {
                        m_ShareRewardItem[shareSet].InitItem(reward.ItemDataId);
                        shareSet++;
                    }
                }
                else if (reward.TargetType == (int)SNSRewardTarget.SNSRTT_JOIN_USER)
                {
                    m_LevelRewardItem[levelSet].InitItem(reward.ItemDataId);
                    levelSet++;
                }
            }
		}

		if(m_labelTimes != null)
		{
			int rewardTimes = GameManager.gameManager.PlayerDataPool.CommonData.GetCommonData((int)Games.UserCommonData.USER_COMMONDATA.CD_SNS_REWARD_NUMBER);
			int left = SNSREWARDTIMES - rewardTimes;
			if(left <= 0 ) left = 0;

			m_labelTimes.text = left.ToString() +"/" + SNSREWARDTIMES.ToString();

		}

        bool bOpenHalloween = GameManager.gameManager.PlayerDataPool.CommonData.GetCommondFlag((int)USER_COMMONFLAG.CF_ACTIVITY_HALLOWEEN_FLAG);

        m_NanGuaButton.SetActive(bOpenHalloween);
             
	}
	
	void SendSharePacket()
	{
		CG_SNS_SHARE packet = (CG_SNS_SHARE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_SNS_SHARE);
		packet.Sharetype = (int)ShareType.ShareType_SNS;
		packet.SendPacket();
	}

    public void ShareEvent( )
    {
        if (false == GameManager.gameManager.PlayerDataPool.IsServerFlagOpen(SERVER_FLAGS_ENUM.FLAG_SNS))
        {
            return;
        }

        SendSharePacket();

        string str = StrDictionary.GetClientDictionaryString("#{1919}", "", 
            Utils.GenServerNameWithSelfGuid(), Utils.GenCodeWithSelfGuid(ShareType.ShareType_SNS));
        PlatformHelper.ShowSocialShareCenter(str);
    }

	public void OnWeiXinFriendClick()
	{
        ShareEvent();
	}
	
	public void OnWeiXinClick()
	{
        ShareEvent();
	}
	
	public void OnTXWeiBoClick()
	{
        ShareEvent();
	}
	
	public void OnSinaWeiBoClick()
	{
        ShareEvent();
	}

	public void OnCloseClick()
	{
		UIManager.CloseUI (UIInfo.SNSRoot);
	}

    public void OnOkClick()
    {
        if (m_inputCode.text.Length > 0)
        {
            CG_SNS_INVITE_CODE packet = (CG_SNS_INVITE_CODE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_SNS_INVITE_CODE);
            packet.Code = m_inputCode.text;
            packet.SendPacket();
        }
    }

    void OnClickNanGuaShare()
    {     
        ShareRootWindow.ShowShareWindow(ShareType.ShareType_NanGua,OpenType.OpenType_ActiviteCode);
    }
}
