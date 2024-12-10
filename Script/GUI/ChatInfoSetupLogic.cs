using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GCGame;

public class ChatInfoSetupLogic : MonoBehaviour {

    private static ChatInfoSetupLogic m_Instance = null;
    public static ChatInfoSetupLogic Instance()
    {
        return m_Instance;
    }

    private enum SETUP_TYPE
    {
        TYPE_INVALID = -1,
        TYPE_WORLD = 0,
        TYPE_TELL = 1,
        TYPE_NORMAL,
        TYPE_TEAM,
        TYPE_GUILD,
        TYPE_MASTER,
        TYPE_FRIEND,
        TYPE_SYSTEM,
    }

    private enum SETUPTOGGLE_INDEX
    {
        INVALID = -1,
        WORLD = 0,
        TELL,
        NORMAL,
        TEAM,
        GUILD,
        MASTER,
        FRIEND,
        SYSTEM,
        LOUDSPEAKER,
    }

    public List<GameObject> m_SetupTypeList = new List<GameObject>();
    public TweenPosition m_SetupPanelTween;
    public TweenPosition m_SetupPanelBGTween;
    public List<UIToggle> m_SetupTypeToggle = new List<UIToggle>();
    public UIGrid m_ToggleGrid;
    public UIToggle m_SetupCloseFriendMenu;
    public ChatInfoLogic m_ChatInfoLogic;

    private List<Vector3> m_TweenLeftTo = new List<Vector3>();
    private List<Vector3> m_TweenRightTo = new List<Vector3>();
    private int m_nCurSetupIndex = 0;

    void Awake()
    {
        m_Instance = this;
    }
	// Use this for initialization
	void Start () {
        
	}

    void OnEnable()
    {
        if (ChatInfoLogic.Instance() == null)
        {
            return;
        }

        SETUP_TYPE defaultSetup = ConvertChatInfoChannelToSetupType(ChatInfoLogic.Instance().CurChannelType);

        if (defaultSetup == SETUP_TYPE.TYPE_INVALID)
        {
            return;
        }

        if ((int)defaultSetup < 0 || (int)defaultSetup >= m_SetupTypeList.Count)
        {
            return;
        }

        SetupTypeOnClick(m_SetupTypeList[(int)defaultSetup]);
    }

    public void Init()
    {
        m_Instance = this;
        InitSetupTypePosInfo();
        LoadChannelSetupValue();
    }

    public void OnCloseChat()
    {
        m_Instance = null;
    }

    void InitSetupTypePosInfo()
    {
        for (int i = 0; i < m_SetupTypeList.Count; i++ )
        {
            m_TweenLeftTo.Add(m_SetupTypeList[0].transform.localPosition + new Vector3(70, 0, 0) * i);
        }

        for (int i = 0; i < m_SetupTypeList.Count; i++)
        {
            m_TweenRightTo.Add(m_SetupTypeList[m_SetupTypeList.Count - 1].transform.localPosition - new Vector3(70, 0, 0) * (m_SetupTypeList.Count - 1 - i));
        }
    }

    void SetupTypeOnClick(GameObject value)
    {
        int nClickIndex = 0;
        for (int i = 0; i < m_SetupTypeList.Count; i++ )
        {
            if (m_SetupTypeList[i].name == value.name)
            {
                nClickIndex = i;
                break;
            }
        }

        for (int i = 0; i < m_SetupTypeList.Count; i++)
        {
            TweenPosition tween = m_SetupTypeList[i].GetComponent<TweenPosition>();

            UIImageButton imageButton = m_SetupTypeList[i].GetComponent<UIImageButton>();
            if (tween != null)
            {
                tween.Reset();
                if (i <= nClickIndex)
                {
                    tween.to = m_TweenLeftTo[i];
                }
                else
                {
                    tween.to = m_TweenRightTo[i];
                }
                tween.Play();
            }
         
            if (i == nClickIndex)
            {
                m_SetupPanelTween.Reset();
                m_SetupPanelTween.from = tween.from;
                m_SetupPanelTween.to = tween.to;
                m_SetupPanelTween.Play();

                m_SetupPanelBGTween.Reset();
                m_SetupPanelBGTween.Play();
                imageButton.normalSprite = "ui_chat_08";
				imageButton.hoverSprite = "ui_chat_08";
				imageButton.pressedSprite = "ui_chat_08";
				imageButton.disabledSprite = "ui_chat_08";
				imageButton.target.spriteName = "ui_chat_08";
            }
            else
            {
				imageButton.normalSprite = "ui_chat_07";
				imageButton.hoverSprite = "ui_chat_07";
				imageButton.pressedSprite = "ui_chat_07";
				imageButton.disabledSprite = "ui_chat_07";
				imageButton.target.spriteName = "ui_chat_07";
            }
        }

        m_nCurSetupIndex = nClickIndex;
        LoadChannelSetupValue();
        if (value.name.Contains("Friend"))
        {
            m_SetupTypeToggle[(int)SETUPTOGGLE_INDEX.WORLD].gameObject.SetActive(false);
            m_SetupTypeToggle[(int)SETUPTOGGLE_INDEX.TELL].gameObject.SetActive(false);
            m_SetupTypeToggle[(int)SETUPTOGGLE_INDEX.NORMAL].gameObject.SetActive(false);
            m_SetupTypeToggle[(int)SETUPTOGGLE_INDEX.TEAM].gameObject.SetActive(false);
            m_SetupTypeToggle[(int)SETUPTOGGLE_INDEX.GUILD].gameObject.SetActive(false);
            m_SetupTypeToggle[(int)SETUPTOGGLE_INDEX.MASTER].gameObject.SetActive(false);
            m_SetupTypeToggle[(int)SETUPTOGGLE_INDEX.LOUDSPEAKER].gameObject.SetActive(false);
            m_SetupCloseFriendMenu.gameObject.SetActive(true);
        }
        else
        {
            m_SetupTypeToggle[(int)SETUPTOGGLE_INDEX.WORLD].gameObject.SetActive(true);
            m_SetupTypeToggle[(int)SETUPTOGGLE_INDEX.TELL].gameObject.SetActive(true);
            m_SetupTypeToggle[(int)SETUPTOGGLE_INDEX.NORMAL].gameObject.SetActive(true);
            m_SetupTypeToggle[(int)SETUPTOGGLE_INDEX.TEAM].gameObject.SetActive(true);
            m_SetupTypeToggle[(int)SETUPTOGGLE_INDEX.GUILD].gameObject.SetActive(true);
            m_SetupTypeToggle[(int)SETUPTOGGLE_INDEX.MASTER].gameObject.SetActive(true);
            m_SetupTypeToggle[(int)SETUPTOGGLE_INDEX.LOUDSPEAKER].gameObject.SetActive(true);
            m_SetupCloseFriendMenu.gameObject.SetActive(false);
        }
        m_ToggleGrid.Reposition();
    }

    public void AfterSetupTypeTween()
    {
        for (int i = 0; i < m_SetupTypeList.Count; ++i)
        {
            if (null != m_SetupTypeList[i])
            {
                TweenPosition tween = m_SetupTypeList[i].GetComponent<TweenPosition>();
                if (tween != null)
                {
                    tween.from = tween.to;
                }    
            }
        }
        //foreach (GameObject setup in m_SetupTypeList)
        //{
        //    TweenPosition tween = setup.GetComponent<TweenPosition>();
        //    if (tween != null)
        //    {
        //        tween.from = tween.to;
        //    }            
        //}
    }

    void LoadChannelSetupValue()
    {
        int playerSetup = 0;
        if (m_nCurSetupIndex == (int)SETUP_TYPE.TYPE_WORLD)
        {
            playerSetup = PlayerPreferenceData.ChannelConfig_World;
            if (Utils.GetIntNumber(playerSetup, 0, 1) == 0)
            {
                Utils.SetIntNumber(ref playerSetup, 0, 1, 1);
                PlayerPreferenceData.ChannelConfig_World = playerSetup;
            }
        }
        else if (m_nCurSetupIndex == (int)SETUP_TYPE.TYPE_TELL)
        {
            playerSetup = PlayerPreferenceData.ChannelConfig_Tell;
            if (Utils.GetIntNumber(playerSetup, 1, 1) == 0)
            {
                Utils.SetIntNumber(ref playerSetup, 1, 1, 1);
                PlayerPreferenceData.ChannelConfig_Tell = playerSetup;
            }
        }
        else if (m_nCurSetupIndex == (int)SETUP_TYPE.TYPE_NORMAL)
        {
            playerSetup = PlayerPreferenceData.ChannelConfig_Normal;
            if (Utils.GetIntNumber(playerSetup, 2, 1) == 0)
            {
                Utils.SetIntNumber(ref playerSetup, 2, 1, 1);
                PlayerPreferenceData.ChannelConfig_Normal = playerSetup;
            }
        }
        else if (m_nCurSetupIndex == (int)SETUP_TYPE.TYPE_TEAM)
        {
            playerSetup = PlayerPreferenceData.ChannelConfig_Team;
            if (Utils.GetIntNumber(playerSetup, 3, 1) == 0)
            {
                Utils.SetIntNumber(ref playerSetup, 3, 1, 1);
                PlayerPreferenceData.ChannelConfig_Team = playerSetup;
            }
        }
        else if (m_nCurSetupIndex == (int)SETUP_TYPE.TYPE_GUILD)
        {
            playerSetup = PlayerPreferenceData.ChannelConfig_Guild;
            if (Utils.GetIntNumber(playerSetup, 4, 1) == 0)
            {
                Utils.SetIntNumber(ref playerSetup, 4, 1, 1);
                PlayerPreferenceData.ChannelConfig_Guild = playerSetup;
            }
        }
        else if (m_nCurSetupIndex == (int)SETUP_TYPE.TYPE_MASTER)
        {
            playerSetup = PlayerPreferenceData.ChannelConfig_Master;
            if (Utils.GetIntNumber(playerSetup, 5, 1) == 0)
            {
                Utils.SetIntNumber(ref playerSetup, 5, 1, 1);
                PlayerPreferenceData.ChannelConfig_Master = playerSetup;
            }
        }
        else if (m_nCurSetupIndex == (int)SETUP_TYPE.TYPE_FRIEND)
        {
            playerSetup = PlayerPreferenceData.ChannelConfig_Friend;
            if (Utils.GetIntNumber(playerSetup, 0, 1) == 1 ||
                Utils.GetIntNumber(playerSetup, 1, 1) == 1 ||
                Utils.GetIntNumber(playerSetup, 2, 1) == 1 ||
                Utils.GetIntNumber(playerSetup, 3, 1) == 1 ||
                Utils.GetIntNumber(playerSetup, 4, 1) == 1 ||
                Utils.GetIntNumber(playerSetup, 5, 1) == 1)
            {
                Utils.SetIntNumber(ref playerSetup, 0, 6, 0);
                PlayerPreferenceData.ChannelConfig_Friend = playerSetup;
            }
            if (Utils.GetIntNumber(playerSetup, 6, 1) == 0)
            {
                Utils.SetIntNumber(ref playerSetup, 6, 1, 1);
                PlayerPreferenceData.ChannelConfig_System = playerSetup;
            }

            m_SetupCloseFriendMenu.value = PlayerPreferenceData.ChannelConfig_CloseFriendMenu == 1 ? true : false;
        }
        else if (m_nCurSetupIndex == (int)SETUP_TYPE.TYPE_SYSTEM)
        {
            playerSetup = PlayerPreferenceData.ChannelConfig_System;
            if (Utils.GetIntNumber(playerSetup, 7, 1) == 0)
            {
                Utils.SetIntNumber(ref playerSetup, 7, 1, 1);
                PlayerPreferenceData.ChannelConfig_System = playerSetup;
            }
        }

        for (int i = 0; i < m_SetupTypeToggle.Count; i++ )
        {
            int nValue = playerSetup % (int)Mathf.Pow(10, i + 1) / (int)Mathf.Pow(10, i);
            m_SetupTypeToggle[i].value = nValue == 1 ? true : false;

            if (i == (int)SETUPTOGGLE_INDEX.SYSTEM)
            {
                
            }

            // 综合频道取消
//             if (m_nCurSetupIndex == (int)SETUP_TYPE.TYPE_VARIOUS)
//             {
//                 // 特殊情况 综合频道设置锁定世界和系统频道
//                 if (i == (int)SETUPTOGGLE_INDEX.WORLD || i == (int)SETUPTOGGLE_INDEX.SYSTEM)
//                 {
//                     m_SetupTypeToggle[i].enabled = false;
//                 }
//                 else
//                 {
//                     m_SetupTypeToggle[i].enabled = true;
//                 }
//             }

            if (i == m_nCurSetupIndex)
            {
                m_SetupTypeToggle[i].enabled = false;
            }
            else
            {
                m_SetupTypeToggle[i].enabled = true;
            }
        }
    }    

    public void ToggleOnValueChange()
    {
        int playerSetup = 0;
        for (int i = 0; i < m_SetupTypeToggle.Count; i++)
        {
            int nToggleValue;
            if (i == m_nCurSetupIndex)
            {
                nToggleValue = 1;
            }
            else
            {
                nToggleValue = m_SetupTypeToggle[i].value ? 1 : 0;
            }
            int nValue = nToggleValue * (int)Mathf.Pow(10, i);
            playerSetup += nValue;
        }

        if (m_nCurSetupIndex == (int)SETUP_TYPE.TYPE_WORLD)
        {
            PlayerPreferenceData.ChannelConfig_World = playerSetup;
        }
        else if (m_nCurSetupIndex == (int)SETUP_TYPE.TYPE_TELL)
        {
            PlayerPreferenceData.ChannelConfig_Tell = playerSetup;
        }
        else if (m_nCurSetupIndex == (int)SETUP_TYPE.TYPE_NORMAL)
        {
            PlayerPreferenceData.ChannelConfig_Normal = playerSetup;
        }
        else if (m_nCurSetupIndex == (int)SETUP_TYPE.TYPE_TEAM)
        {
            PlayerPreferenceData.ChannelConfig_Team = playerSetup;
        }
        else if (m_nCurSetupIndex == (int)SETUP_TYPE.TYPE_GUILD)
        {
            PlayerPreferenceData.ChannelConfig_Guild = playerSetup;
        }
        else if (m_nCurSetupIndex == (int)SETUP_TYPE.TYPE_MASTER)
        {
            PlayerPreferenceData.ChannelConfig_Master = playerSetup;
        }
        else if (m_nCurSetupIndex == (int)SETUP_TYPE.TYPE_FRIEND)
        {
            PlayerPreferenceData.ChannelConfig_Friend = playerSetup;
            PlayerPreferenceData.ChannelConfig_CloseFriendMenu = m_SetupCloseFriendMenu.value ? 1 : 0;
        }
        else if (m_nCurSetupIndex == (int)SETUP_TYPE.TYPE_SYSTEM)
        {
            PlayerPreferenceData.ChannelConfig_System = playerSetup;
        }

        if (m_ChatInfoLogic != null)
        {
            m_ChatInfoLogic.UpdateChannelHistory();
        }
    }

    public static bool IsChannelReceiveChat(ChatInfoLogic.CHANNEL_TYPE playerChannel, GC_CHAT.CHATTYPE chatChannel)
    {
        int playerSetup = 0;
        if (playerChannel == ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_WORLD)
        {
            playerSetup = PlayerPreferenceData.ChannelConfig_World;
        }
        else if (playerChannel == ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_TELL)
        {
            playerSetup = PlayerPreferenceData.ChannelConfig_Tell;
        }
        else if (playerChannel == ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_NORMAL)
        {
            playerSetup = PlayerPreferenceData.ChannelConfig_Normal;
        }
        else if (playerChannel == ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_TEAM)
        {
            playerSetup = PlayerPreferenceData.ChannelConfig_Team;
        }
        else if (playerChannel == ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_GUILD)
        {
            playerSetup = PlayerPreferenceData.ChannelConfig_Guild;
        }
        else if (playerChannel == ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_MASTER)
        {
            playerSetup = PlayerPreferenceData.ChannelConfig_Master;
        }
        else if (playerChannel == ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_FRIEND)
        {
            playerSetup = PlayerPreferenceData.ChannelConfig_Friend;
        }
        else if (playerChannel == ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_SYSTEM)
        {
            playerSetup = PlayerPreferenceData.ChannelConfig_System;
        }

        int chatSetupIndex = (int)SETUPTOGGLE_INDEX.INVALID;
        if (chatChannel == GC_CHAT.CHATTYPE.CHAT_TYPE_LOUDSPEAKER)
        {
            // 小喇叭直接返回true
            //return true;
            // 小喇叭频道改为可选择接收
            chatSetupIndex = (int)SETUPTOGGLE_INDEX.LOUDSPEAKER;
        }
        else if (chatChannel == GC_CHAT.CHATTYPE.CHAT_TYPE_WORLD)
        {
            chatSetupIndex = (int)SETUPTOGGLE_INDEX.WORLD;
        }
        else if (chatChannel == GC_CHAT.CHATTYPE.CHAT_TYPE_TELL)
        {
            chatSetupIndex = (int)SETUPTOGGLE_INDEX.TELL;
        }
        else if (chatChannel == GC_CHAT.CHATTYPE.CHAT_TYPE_NORMAL)
        {
            chatSetupIndex = (int)SETUPTOGGLE_INDEX.NORMAL;
        }
        else if (chatChannel == GC_CHAT.CHATTYPE.CHAT_TYPE_TEAM)
        {
            chatSetupIndex = (int)SETUPTOGGLE_INDEX.TEAM;
        }
        else if (chatChannel == GC_CHAT.CHATTYPE.CHAT_TYPE_GUILD)
        {
            chatSetupIndex = (int)SETUPTOGGLE_INDEX.GUILD;
        }
        else if (chatChannel == GC_CHAT.CHATTYPE.CHAT_TYPE_MASTER)
        {
            chatSetupIndex = (int)SETUPTOGGLE_INDEX.MASTER;
        }
        else if (chatChannel == GC_CHAT.CHATTYPE.CHAT_TYPE_FRIEND)
        {
            chatSetupIndex = (int)SETUPTOGGLE_INDEX.FRIEND;
        }
        else if (chatChannel == GC_CHAT.CHATTYPE.CHAT_TYPE_SYSTEM)
        {
            chatSetupIndex = (int)SETUPTOGGLE_INDEX.SYSTEM;
        }

        if (chatSetupIndex != (int)SETUPTOGGLE_INDEX.INVALID)
        {
            int nIsReceive = playerSetup % (int)Mathf.Pow(10, chatSetupIndex + 1) / (int)Mathf.Pow(10, chatSetupIndex);
            return nIsReceive == 1 ? true : false;
        }
        return false;
    }

    void CloseSetup()
    {
        gameObject.SetActive(false);
    }

    SETUP_TYPE ConvertChatInfoChannelToSetupType(ChatInfoLogic.CHANNEL_TYPE type)
    {
        switch (type)
        {
            case ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_WORLD:
                return SETUP_TYPE.TYPE_WORLD;
            case ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_TELL:
                return SETUP_TYPE.TYPE_TELL;
            case ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_NORMAL:
                return SETUP_TYPE.TYPE_NORMAL;
            case ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_TEAM:
                return SETUP_TYPE.TYPE_TEAM;
            case ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_GUILD:
                return SETUP_TYPE.TYPE_GUILD;
            case ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_MASTER:
                return SETUP_TYPE.TYPE_MASTER;
            case ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_FRIEND:
                return SETUP_TYPE.TYPE_FRIEND;
            case ChatInfoLogic.CHANNEL_TYPE.CHAT_TYPE_SYSTEM:
                return SETUP_TYPE.TYPE_SYSTEM;
            default:
                return SETUP_TYPE.TYPE_INVALID;
        }
    }
}
