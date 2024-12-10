using System.Collections.Generic;
using Games.LogicObj;
using GCGame.Table;
using UnityEngine;
using System.Collections;

public class GuildWarPushMessageLogic : MonoBehaviour {

	// Use this for initialization
    public UILabel m_MessageNumLabel;
    private GuildWarPushMessageInfo m_curPopMessage =new GuildWarPushMessageInfo(); //缓存当前弹出的消息
    private int m_curMessageNum=0; //当前推送的消息数
    private static GuildWarPushMessageLogic m_Instance = null;
    private bool m_bIsWaitAnswer = false;
    public static GuildWarPushMessageLogic Instance()
    {
        return m_Instance;
    }
    void Awake()
    {
        m_Instance = this;
    }
	void Start () 
    {
	    m_MessageNumLabel.gameObject.SetActive(false);
        m_curPopMessage.CleanUp();
	    UpdateMessageNum();
    }

    void Update()
    {
        List<GuildWarPushMessageInfo> _List =new List<GuildWarPushMessageInfo>();
        for (int i = 0; i < GameManager.gameManager.PlayerDataPool.WarPushMessaeg.Count;i++)
        {
            float fPustTime = GameManager.gameManager.PlayerDataPool.WarPushMessaeg[i].PushTime;
            if (GameManager.gameManager.PlayerDataPool.WarPushMessaeg[i].MessageType ==(int)GC_ASK_STARTGUILDWAR.MESSAGETYPE.ASKCHALLENGE ||
                GameManager.gameManager.PlayerDataPool.WarPushMessaeg[i].MessageType ==(int)GC_ASK_STARTGUILDWAR.MESSAGETYPE.ASKWILDWAR)
            {
                if (Time.time - fPustTime > 60.0f) //约战应答 超过60s 移除
                {
                    _List.Add(GameManager.gameManager.PlayerDataPool.WarPushMessaeg[i]);
                }
            }
            else
            {
                if (Time.time - fPustTime > 30.0f) //超过20s 移除
                {
                    _List.Add(GameManager.gameManager.PlayerDataPool.WarPushMessaeg[i]);
                }
            }
        }
        for (int i = 0; i <_List.Count; i++)
        {
            GameManager.gameManager.PlayerDataPool.WarPushMessaeg.Remove(_List[i]);
            UpdateMessageNum();
        }
        if (GameManager.gameManager.PlayerDataPool.WarPushMessaeg.Count<=0 && m_bIsWaitAnswer ==false)
        {
            UIManager.CloseUI(UIInfo.GuilWarPushMessage);
        }
    }
    void OnDestroy()
    {
        m_Instance = null;
    }

    public  void UpdateMessageNum()
    {
        m_curMessageNum = GameManager.gameManager.PlayerDataPool.WarPushMessaeg.Count;
        if (m_curMessageNum > 0)
        {
            m_MessageNumLabel.text = m_curMessageNum.ToString();
            m_MessageNumLabel.gameObject.SetActive(true);
        }
    }
    void ClickMessageBt()
    {
        if (m_curMessageNum >0)
        {
            //取出消息队列的第一个并移除掉
            m_curPopMessage = GameManager.gameManager.PlayerDataPool.WarPushMessaeg[0];
            GameManager.gameManager.PlayerDataPool.WarPushMessaeg.RemoveAt(0);
            //弹出的MessageBox
            m_curMessageNum = GameManager.gameManager.PlayerDataPool.WarPushMessaeg.Count;
            m_MessageNumLabel.text = m_curMessageNum.ToString();
            string dicStr =GetMessageBoxStr();
            m_bIsWaitAnswer = true;
            MessageBoxLogic.OpenOKCancelBox(dicStr, "", OnAgreeEnterGuildWar, OnDisAgreeEnterGuildWar);
        }
    }

    void OnAgreeEnterGuildWar()
    {
        m_bIsWaitAnswer = false;
        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (_mainPlayer ==null)
        {
            return;
        }
        switch (m_curPopMessage.MessageType)
        {
            //回应是否进副本打海选赛
            case  (int)GC_ASK_STARTGUILDWAR.MESSAGETYPE.STARTPREMINARY:
            {
                CG_RET_STARTWAR Pak = (CG_RET_STARTWAR)PacketDistributed.CreatePacket(MessageID.PACKET_CG_RET_STARTWAR);
                Pak.SetMessaegType(m_curPopMessage.MessageType);
                Pak.SetIsAgree(1);
                Pak.SetPointType(m_curPopMessage.PointType);
                Pak.SetWarType(m_curPopMessage.WarType);
                Pak.SendPacket();
                //_mainPlayer.SendNoticMsg(false, "#{2576}");
            }
                break;
            //回应是否进副本打海选赛
            case (int)GC_ASK_STARTGUILDWAR.MESSAGETYPE.PROTECTPOINT:
            case (int)GC_ASK_STARTGUILDWAR.MESSAGETYPE.ROBPOINT:
                {
                    CG_RET_STARTWAR Pak = (CG_RET_STARTWAR)PacketDistributed.CreatePacket(MessageID.PACKET_CG_RET_STARTWAR);
                    Pak.SetMessaegType(m_curPopMessage.MessageType);
                    Pak.SetIsAgree(1);
                    Pak.SetPointType(m_curPopMessage.PointType);
                    Pak.SetWarType(m_curPopMessage.WarType);
                    Pak.SendPacket();
                    //_mainPlayer.SendNoticMsg(false, "#{2576}");
                }
                break;
            //回应是否接受约战(副本战)
            case (int)GC_ASK_STARTGUILDWAR.MESSAGETYPE.ASKCHALLENGE:
                {
                    CG_RET_STARTWAR Pak = (CG_RET_STARTWAR)PacketDistributed.CreatePacket(MessageID.PACKET_CG_RET_STARTWAR);
                    Pak.SetMessaegType(m_curPopMessage.MessageType);
                    Pak.SetIsAgree(1);
                    Pak.SetPointType(m_curPopMessage.PointType);
                    Pak.SetWarType(m_curPopMessage.WarType);
                    Pak.SendPacket();
                }
                break;
            //回应是否接受野外宣战
            case (int)GC_ASK_STARTGUILDWAR.MESSAGETYPE.ASKWILDWAR:
                {
                    CG_RET_STARTWAR Pak = (CG_RET_STARTWAR)PacketDistributed.CreatePacket(MessageID.PACKET_CG_RET_STARTWAR);
                    Pak.SetMessaegType(m_curPopMessage.MessageType);
                    Pak.SetIsAgree(1);
                    Pak.SetPointType(m_curPopMessage.PointType);
                    Pak.SetWarType(m_curPopMessage.WarType);
                    Pak.SetChallengeGuildGuid(m_curPopMessage.ChallengeGuildGUID);
                    Pak.SendPacket();
                }
                break;
        }
        if (m_curMessageNum <= 0)
        {
            UIManager.CloseUI(UIInfo.GuilWarPushMessage);
        }
    }

    void OnDisAgreeEnterGuildWar()
    {
        m_bIsWaitAnswer = false;
        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (_mainPlayer == null)
        {
            return;
        }
        switch (m_curPopMessage.MessageType)
        {
            //回应是否进副本打海选赛
            case (int)GC_ASK_STARTGUILDWAR.MESSAGETYPE.STARTPREMINARY:
                {
                    CG_RET_STARTWAR Pak = (CG_RET_STARTWAR)PacketDistributed.CreatePacket(MessageID.PACKET_CG_RET_STARTWAR);
                    Pak.SetMessaegType(m_curPopMessage.MessageType);
                    Pak.SetIsAgree(0);
                    Pak.SetPointType(m_curPopMessage.PointType);
                    Pak.SetWarType(m_curPopMessage.WarType);
                    Pak.SendPacket();
                    _mainPlayer.SendNoticMsg(false, "#{2577}");
                }
                break;
            //回应是否进副本打海选赛
            case (int)GC_ASK_STARTGUILDWAR.MESSAGETYPE.PROTECTPOINT:
            case (int)GC_ASK_STARTGUILDWAR.MESSAGETYPE.ROBPOINT:
                {
                    CG_RET_STARTWAR Pak = (CG_RET_STARTWAR)PacketDistributed.CreatePacket(MessageID.PACKET_CG_RET_STARTWAR);
                    Pak.SetMessaegType(m_curPopMessage.MessageType);
                    Pak.SetIsAgree(0);
                    Pak.SetPointType(m_curPopMessage.PointType);
                    Pak.SetWarType(m_curPopMessage.WarType);
                    Pak.SendPacket();
                    _mainPlayer.SendNoticMsg(false, "#{2577}");
                }
                break;
            //回应是否接受约战(副本战)
            case (int)GC_ASK_STARTGUILDWAR.MESSAGETYPE.ASKCHALLENGE:
                {
                    CG_RET_STARTWAR Pak = (CG_RET_STARTWAR)PacketDistributed.CreatePacket(MessageID.PACKET_CG_RET_STARTWAR);
                    Pak.SetMessaegType(m_curPopMessage.MessageType);
                    Pak.SetIsAgree(0);
                    Pak.SetPointType(m_curPopMessage.PointType);
                    Pak.SetWarType(m_curPopMessage.WarType);
                    Pak.SendPacket();
                }
                break;
            //回应是否接受野外宣战
            case (int)GC_ASK_STARTGUILDWAR.MESSAGETYPE.ASKWILDWAR:
                {
                    CG_RET_STARTWAR Pak = (CG_RET_STARTWAR)PacketDistributed.CreatePacket(MessageID.PACKET_CG_RET_STARTWAR);
                    Pak.SetMessaegType(m_curPopMessage.MessageType);
                    Pak.SetIsAgree(0);
                    Pak.SetPointType(m_curPopMessage.PointType);
                    Pak.SetWarType(m_curPopMessage.WarType);
                    Pak.SetChallengeGuildGuid(m_curPopMessage.ChallengeGuildGUID);
                    Pak.SendPacket();
                }
                break;
        }
        if (m_curMessageNum <= 0)
        {
            UIManager.CloseUI(UIInfo.GuilWarPushMessage);
        }
    }
	
    public string GetMessageBoxStr()
    {
        switch (m_curPopMessage.MessageType)
        {
            //回应是否进副本打海选赛
            case (int)GC_ASK_STARTGUILDWAR.MESSAGETYPE.STARTPREMINARY:
                {
                    return StrDictionary.GetClientDictionaryString("#{2579}");
                }
                break;
            //回应是否进副本打海选赛
            case (int)GC_ASK_STARTGUILDWAR.MESSAGETYPE.PROTECTPOINT:
            {
                string strPointName = GuildWarInfoLogic.GetWarPointNameByType(m_curPopMessage.PointType);
                return StrDictionary.GetClientDictionaryString("#{2581}",strPointName);
            }
                break;
            case (int)GC_ASK_STARTGUILDWAR.MESSAGETYPE.ROBPOINT:
                {
                    string strPointName = GuildWarInfoLogic.GetWarPointNameByType(m_curPopMessage.PointType);
                    return StrDictionary.GetClientDictionaryString("#{2583}", strPointName);
                }
                break;
            //回应是否接受约战
            case (int)GC_ASK_STARTGUILDWAR.MESSAGETYPE.ASKCHALLENGE:
                {
                    return StrDictionary.GetClientDictionaryString("#{2610}",m_curPopMessage.ChallengeGuildName);
                }
                break;
            //回应是否接受约战
            case (int)GC_ASK_STARTGUILDWAR.MESSAGETYPE.ASKWILDWAR:
                {
                    return StrDictionary.GetClientDictionaryString("#{3118}", m_curPopMessage.ChallengeGuildName);
                }
                break;
        }
        return "";
    }
}
