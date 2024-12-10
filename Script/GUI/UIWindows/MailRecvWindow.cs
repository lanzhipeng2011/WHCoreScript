using UnityEngine;
using System.Collections;
using GCGame;
using SPacket.SocketInstance;
using GCGame.Table;
using Module.Log;
using System.Collections.Generic;
using Games.Item;
using Games.GlobeDefine;
public class MailRecvWindow : MonoBehaviour {

	public GameObject MailListGrid;
    public GameObject CurMailGroup;

    public UILabel LabelCurMailText;
    public RewardItem CurRewardItem;
    public GameObject BtnReplay;
    public GameObject BtnGetItem;			// 更改为举报玩家

    public MailWindow TopMailWindow;
    public GameObject MoneyCountLabel;
    public GameObject MoneyIcon;
    public UILabel LabelRecvTips;           // 收件箱提示



    public UILabel BtnLabelDelAll;
    public UILabel BtnLabelGetAll;
    public UILabel BtnLabelDelCur;
    public UILabel BtnLabelGetCur;
    public UILabel BtnLabelReply;
	public MailSendWindow mailReply;
	public TabButton  m_tab;
    private MailListItem m_curSelectItem = null;
	// Use this for initialization

    private bool m_bFirstUpdate = true;

    private static List<System.UInt64> m_sortMailList = new List<System.UInt64>();
    private GameObject m_curItemResObj = null;     // 缓存的邮件ITEM
    private float m_UpdateTimer = 0;

    void Awake()
    {
        CurRewardItem.delItemClick = OnRewardItemClick;

        BtnLabelDelAll.text = Utils.GetDicByID(1126);
        BtnLabelGetAll.text = Utils.GetDicByID(1127);
        BtnLabelDelCur.text = Utils.GetDicByID(1128);
        BtnLabelReply.text = Utils.GetDicByID(1129);
		BtnLabelGetCur.text = Utils.GetDicByID(1159);
    }
	void Start () {
        m_bFirstUpdate = true;
        InitItems();
	}

    void OnEnable()
    {
        MailData.delMailUpdate += UpdateMailData;
		mailReply.gameObject.SetActive (false);
    }

    void OnDisable()
    {
        MailData.delMailUpdate -= UpdateMailData;
		mailReply.gameObject.SetActive (false);
    }

    void Update()
    {
        if (m_UpdateTimer > 0)
        {
            m_UpdateTimer -= Time.deltaTime;
            if (m_UpdateTimer <= 0)
            {
                UpdateMailItems();
            }
        }
    }

    // 删除当前邮件
    void OnClickDelCur()
    {
        MailData.UserMail curMail = GetMailByItem(m_curSelectItem);
        if (null == curMail)
        {
            return;
        }

        if (curMail.itemID > 0 || curMail.moneyCount > 0)
        {
            //仍有未提取的附件或钱币，确定要删除吗？
            MessageBoxLogic.OpenOKCancelBox(1135, 1000, DoDelegateCurMail);
            return;
        }

        DoDelegateCurMail();
    }

    void DoDelegateCurMail()
    {
        MailData.UserMail curMail = GetMailByItem(m_curSelectItem);
        if (null == curMail)
        {
            LogModule.ErrorLog("can not find cur select item");
            return;
        }
        CG_MAIL_OPERATION packetDelMail = (CG_MAIL_OPERATION)PacketDistributed.CreatePacket(MessageID.PACKET_CG_MAIL_OPERATION);
        packetDelMail.SetOperationType((int)CG_MAIL_OPERATION.MailOperation.OPERATION_DELETE);
        packetDelMail.SetMailGuid(curMail.key);
        packetDelMail.SendPacket();
    }

	void OnClickDelAll()
	{
        //确定要将所有一度邮件删除吗？
        MessageBoxLogic.OpenOKCancelBox(1137, 1000, TryDelegateAll);
	}

    void TryDelegateAll()
    {
        foreach (ulong mailKeys in MailData.UserMailMap.Keys)
        {
            MailData.UserMail curMail = MailData.UserMailMap[mailKeys];
            if ((curMail.itemID >= 0 || curMail.moneyCount > 0) && curMail.bReaded)
            {
                //仍有未提取的附件或钱币，确定要删除吗？
                MessageBoxLogic.OpenOKCancelBox(1135, 1000, DoDelegateAll);
                return;
            }
        }

        DoDelegateAll();
    }

    void DoDelegateAll()
    {
        foreach (ulong mailKeys in MailData.UserMailMap.Keys)
        {
            if (MailData.UserMailMap[mailKeys].bReaded)
            {
                CG_MAIL_OPERATION packetDelMail = (CG_MAIL_OPERATION)PacketDistributed.CreatePacket(MessageID.PACKET_CG_MAIL_OPERATION);
                packetDelMail.SetOperationType((int)CG_MAIL_OPERATION.MailOperation.OPERATION_DELETE);
                packetDelMail.SetMailGuid(mailKeys);
                packetDelMail.SendPacket();
            }
        }
    }

    void OnClickGetCur()
    {
	
        MailData.UserMail curMail = GetMailByItem(m_curSelectItem);
        if (null == curMail)
        {
            return;
        }
		if (curMail.senderType != GC_MAIL_UPDATE.MailSender.MAILSENDER_USER) 
		{
						CG_MAIL_OPERATION packetDelMail = (CG_MAIL_OPERATION)PacketDistributed.CreatePacket (MessageID.PACKET_CG_MAIL_OPERATION);
						packetDelMail.SetOperationType ((int)CG_MAIL_OPERATION.MailOperation.OPERATION_GETITEM);
						packetDelMail.SetMailGuid (curMail.key);
						packetDelMail.SendPacket ();
		}
        else
		// 此功能暂时修改为举报玩家 将玩家加入黑名单
		MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{5449}"), "", AddMailSendBlack);
    }

	void AddMailSendBlack()
	{
		MailData.UserMail curMail = GetMailByItem(m_curSelectItem);
		//如果非玩家，则无效
		if (GlobeVar.INVALID_GUID == curMail.SenderID)
		{
			return;
		}
		if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
		{
			return;
		}
		
		//如果目标是自己也不发送
		if (Singleton<ObjManager>.GetInstance().MainPlayer.GUID == curMail.SenderID)
		{
			return;
		}
		
		Singleton<ObjManager>.GetInstance().MainPlayer.ReqAddBlack(curMail.SenderID);
	}

	void OnClickGetAll()
	{
        foreach (ulong mailKeys in MailData.UserMailMap.Keys)
        {
            CG_MAIL_OPERATION packetDelMail = (CG_MAIL_OPERATION)PacketDistributed.CreatePacket(MessageID.PACKET_CG_MAIL_OPERATION);
            packetDelMail.SetOperationType((int)CG_MAIL_OPERATION.MailOperation.OPERATION_GETITEM);
            packetDelMail.SetMailGuid(mailKeys);
            packetDelMail.SendPacket();
        }
	}

    void OnClickReply()
    {
        MailData.UserMail curMail = GetMailByItem(m_curSelectItem);
        if (null == curMail)
        {
            return;
        }

        if (curMail.senderType == GC_MAIL_UPDATE.MailSender.MAILSENDER_SYS)
        {
            return;
        }

		// TopMailWindow.ReplayMail(curMail.SenderID, curMail.SenderName);
		//打开一个快捷回复窗口
		this.gameObject.SetActive (false);
		mailReply.gameObject.SetActive (true);
		mailReply.SetQucikReceiver (curMail.SenderID, curMail.SenderName);
		m_tab.OnTabClick ();
    }

    void InitItems()
    {
        UIManager.LoadItem(UIInfo.MailListItem, OnLoadItem);
    }

    void OnLoadItem(GameObject resItem, object param)
    {
        if (null == resItem)
        {
            LogModule.ErrorLog("load mail resItem fail");
            return;
        }

        m_curItemResObj = resItem;

        // 按照已读未读排序
        int m_curUnreadMailCount = 0;
        m_sortMailList.Clear();

        foreach (System.UInt64 key in MailData.UserMailMap.Keys)
        {
            if (!MailData.UserMailMap[key].bReaded)
            {
                m_sortMailList.Insert(m_curUnreadMailCount++, key);
            }
            else
            {
                m_sortMailList.Add(key);
            }
        }

        

        UpdateMailItems();
        
    }

    public void UpdateMailData(MailData.MailUpdateType curUpdateType, System.UInt64 curKey)
    {
        switch (curUpdateType )
        {
            case MailData.MailUpdateType.ADD:
                {
                    m_sortMailList.Insert(0, curKey);
                }
                break;
            case MailData.MailUpdateType.DEL:
                {
                    m_sortMailList.Remove(curKey);
                }
                break;
            default:
                break;
        }
        m_UpdateTimer = 0.5f;
    }

    void UpdateMailItems()
    {
        if (null == m_curItemResObj)
        {
            return;
        }

        // 记录上次正在操作的邮件，重新加载时直接选中
        string lastKey = null;
        if (null != m_curSelectItem)
        {
            lastKey = m_curSelectItem.gameObject.name;
        }

        m_curSelectItem = null;
        Utils.CleanGrid(MailListGrid);
        for (int i = 0; i < m_sortMailList.Count; i++)
        {
            System.UInt64 curKey = m_sortMailList[i];
            if (!MailData.UserMailMap.ContainsKey(curKey))
            {
                continue;
            }
            MailListItem curItem = MailListItem.CreateItem(MailListGrid, m_curItemResObj, curKey.ToString(), this, MailData.UserMailMap[curKey]);
            if (null == curItem)
            {
                continue;
            }
            if (null == m_curSelectItem && curKey.ToString() == lastKey)
            {
                ShowMailItem(curItem);
            }
        }

        MailListGrid.GetComponent<UIGrid>().Reposition();
        MailListGrid.GetComponent<UITopGrid>().Recenter(m_bFirstUpdate);
        m_bFirstUpdate = false;

        if (null == m_curSelectItem && MailListGrid.transform.childCount > 0)
        {
            ShowMailItem(MailListGrid.transform.GetChild(0).GetComponent<MailListItem>());
        }

        CurMailGroup.SetActive(null != m_curSelectItem);

        // 邮件上限提示
        LabelRecvTips.text = StrDictionary.GetClientDictionaryString("#{1252}", MailData.UserMailMap.Count);
    }

    public void ShowMailItem(MailListItem curItem)
    {
        if (null == curItem) return;
        if (m_curSelectItem == curItem) return;

		mailReply.gameObject.SetActive (false);

        if (null != m_curSelectItem)
        {
            m_curSelectItem.EnableHighlight(false);
        }

        m_curSelectItem = curItem;
        m_curSelectItem.EnableHighlight(true);


        MailData.UserMail curMail = GetMailByItem(curItem);
        if (null == curMail)
        {
            return;
        }

        bool bHaveItem = false;
        LabelCurMailText.text = StrDictionary.GetClientString_WithNameSex(curMail.text);//curMail.text;
        if (curMail.itemID >= 0)
        {
            CurRewardItem.SetData(curMail.itemID,curMail.itemCount);
            CurRewardItem.gameObject.SetActive(true);
            bHaveItem = true;
        }
        else
        {
            CurRewardItem.gameObject.SetActive(false);
        }

        if (curMail.moneyCount > 0)
        {
			if(bHaveItem==false)
			{
				float x=550.0f;
				MoneyIcon.transform.localPosition=new Vector3(x,MoneyIcon.transform.localPosition.y,MoneyIcon.transform.localPosition.z);
			}
			else
			{
				float x=310.0f;
				MoneyIcon.transform.localPosition=new Vector3(x,MoneyIcon.transform.localPosition.y,MoneyIcon.transform.localPosition.z);
			}
            MoneyCountLabel.SetActive(true);
            MoneyCountLabel.GetComponent<UILabel>().text = curMail.moneyCount.ToString();
            MoneyIcon.SetActive(true);
            if (curMail.moneyType == 0)
            {
                MoneyIcon.GetComponent<UISprite>().spriteName = "bi";
            }
            else if (curMail.moneyType == 2)
            {
                MoneyIcon.GetComponent<UISprite>().spriteName = "yuanbao2";
            }
            else
            {
                MoneyIcon.GetComponent<UISprite>().spriteName = "yuanbao1";
            }
            bHaveItem = true;

        }
        else
        {
            MoneyCountLabel.SetActive(false);
            MoneyIcon.SetActive(false);
        }

		if(curMail.senderType != GC_MAIL_UPDATE.MailSender.MAILSENDER_USER)
		//添加时用户邮件显示举报玩家，其他显示提取附件
		BtnLabelGetCur.text = Utils.GetDicByID(4061);
		else
		BtnLabelGetCur.text = Utils.GetDicByID(1159);
		BtnReplay.SetActive(curMail.senderType == GC_MAIL_UPDATE.MailSender.MAILSENDER_USER);

        if (!curMail.bReaded)
        {
            CG_MAIL_OPERATION packetDelMail = (CG_MAIL_OPERATION)PacketDistributed.CreatePacket(MessageID.PACKET_CG_MAIL_OPERATION);
            packetDelMail.SetOperationType((int)CG_MAIL_OPERATION.MailOperation.OPERATION_SETREAD);
            packetDelMail.SetMailGuid(curMail.key);
            packetDelMail.SendPacket();
        }
    }

    MailData.UserMail GetMailByItem(MailListItem curItem)
    {
        if (null == curItem)
        {
            return null;
        }
        System.UInt64 curMailID = 0;
        if (!System.UInt64.TryParse(curItem.gameObject.name, out curMailID))
        {
            LogModule.ErrorLog("can not parse cur mail id:" + curMailID.ToString() + " name : " + curItem.gameObject.name);
            CurMailGroup.SetActive(false);
            return null;
        }

        if (!MailData.UserMailMap.ContainsKey(curMailID))
        {
            LogModule.ErrorLog("can not find cur mail id:" + curMailID.ToString());
            CurMailGroup.SetActive(false);
            return null;
        }

        return MailData.UserMailMap[curMailID];
    }

    void OnRewardItemClick(RewardItem curItem)
    {
        if (null == m_curSelectItem)
        {
            return;
        }

        MailData.UserMail curUserMail = GetMailByItem(m_curSelectItem);

        if(null == curUserMail)
        {
            return;
        }

        GameItem gameItem = new GameItem();
        gameItem.DataID = curUserMail.itemID;
        if (gameItem.IsEquipMent())
        {

            EquipTooltipsLogic.ShowEquipTooltip(gameItem,EquipTooltipsLogic.ShowType.Info);
        }
        else
        {
            ItemTooltipsLogic.ShowItemTooltip(gameItem, ItemTooltipsLogic.ShowType.Info);
        }
    }
}
