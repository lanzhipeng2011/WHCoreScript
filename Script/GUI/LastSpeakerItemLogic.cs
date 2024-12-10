using UnityEngine;
using System.Collections;
using System;
using Games.GlobeDefine;

public class LastSpeakerItemLogic : MonoBehaviour {

    private UInt64 m_SpeakerGuid = GlobeVar.INVALID_GUID;
    public UInt64 SpeakerGuid
    {
        get { return m_SpeakerGuid; }
        set { m_SpeakerGuid = value; }
    }

    private string m_SpeakerName = "";
    public string SpeakerName
    {
        get { return m_SpeakerName; }
        set { m_SpeakerName = value; }
    }

    private ChatInfoLogic m_ChatInfoLogic = null;
    public ChatInfoLogic ChatInfoLogic
    {
        get { return m_ChatInfoLogic; }
        set { m_ChatInfoLogic = value; }
    }

    private LastSpeakerChatLogic m_LastSpeakerChatLogic = null;
    public LastSpeakerChatLogic LastSpeakerChatLogic
    {
        get { return m_LastSpeakerChatLogic; }
        set { m_LastSpeakerChatLogic = value; }
    }

    public UILabel m_SpeakerNameLabel;
    public GameObject m_ChooseFrame;
    public GameObject m_InformSprite;

	// Use this for initialization
	void Start () {

	}
	

    public void Init(UInt64 nSpeakerGuid, string strSpeakerName, ChatInfoLogic chatinfo, LastSpeakerChatLogic parentLogic)
    {
        if (m_SpeakerNameLabel != null)
        {
            m_SpeakerGuid = nSpeakerGuid;
            m_SpeakerName = strSpeakerName;
            m_SpeakerNameLabel.text = strSpeakerName.ToString();
            m_ChatInfoLogic = chatinfo;
            m_LastSpeakerChatLogic = parentLogic;

            if (m_ChatInfoLogic.CurChannelType == ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_FRIEND)
            {
                if (GameManager.gameManager.PlayerDataPool.ChatHistory.FriendSendList.Contains(m_SpeakerGuid))
                {
                    ShowInform();
                }
            }
        }        
    }

    public void CopyFrom(LastSpeakerItemLogic item)
    {
        if (m_SpeakerNameLabel != null)
        {
            m_SpeakerGuid = item.SpeakerGuid;
            m_SpeakerName = item.SpeakerName;
            m_SpeakerNameLabel.text = m_SpeakerName.ToString();
            m_ChatInfoLogic = item.ChatInfoLogic;
            m_LastSpeakerChatLogic = item.LastSpeakerChatLogic;

            if (m_ChatInfoLogic.CurChannelType == ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_FRIEND)
            {
                if (GameManager.gameManager.PlayerDataPool.ChatHistory.FriendSendList.Contains(m_SpeakerGuid))
                {
                    ShowInform();
                }
            }
        }  
    }

    void LastSpeakerOnClick()
    {
        if (Singleton<ObjManager>.Instance.MainPlayer == null)
        {
            return;
        }

        if (Singleton<ObjManager>.Instance.MainPlayer.GUID == m_SpeakerGuid)
        {
            return;
        }

        if (m_LastSpeakerChatLogic != null)
        {
            if (m_ChatInfoLogic.CurChannelType == ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_FRIEND &&
                PlayerPreferenceData.ChannelConfig_CloseFriendMenu == 1)
            {
                m_LastSpeakerChatLogic.ChoosePlayer(m_SpeakerGuid, m_SpeakerName);
            }            
            else
            {
                m_LastSpeakerChatLogic.ShowButtonMenu(m_SpeakerGuid, m_SpeakerName);
            }
        }

        ChooseSpeaker();
    }

    public void ChooseSpeaker()
    {
        m_ChooseFrame.SetActive(true);

        if (m_ChatInfoLogic != null)
        {
            if (m_ChatInfoLogic.CurChannelType == ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_FRIEND)
            {
                m_ChatInfoLogic.FilterFriendChat(m_SpeakerGuid, m_SpeakerName);

                if (GameManager.gameManager.PlayerDataPool.ChatHistory.FriendSendList.Contains(m_SpeakerGuid))
                {
                    GameManager.gameManager.PlayerDataPool.ChatHistory.FriendSendList.Remove(m_SpeakerGuid);
                    HideInform();

                    if (GameManager.gameManager.PlayerDataPool.ChatHistory.FriendSendList.Count == 0)
                    {
                        m_ChatInfoLogic.m_FriendInformSprite.SetActive(false);
                    }                    
                }                
            }
        }
    }

    public void CancelChoose()
    {
        m_ChooseFrame.SetActive(false);
    }

    public void ShowInform()
    {
        m_InformSprite.SetActive(true);
    }

    public void HideInform()
    {
        m_InformSprite.SetActive(false);
    }
}
