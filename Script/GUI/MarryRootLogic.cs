using UnityEngine;
using System.Collections;
using GCGame.Table;
using GCGame;
using Module.Log;
using Games.GlobeDefine;
using Games.Item;
using System.Collections.Generic;
using Games.LogicObj;
using System;

public class MarryRootLogic : UIControllerBase<MarryRootLogic>
{
    public UITexture    m_NPCHead;
	public UILabel      m_NPCTalk;
    public UILabel      m_NPCName;

    public GameObject   m_Button1;
    public GameObject   m_Button2;
    public UILabel      m_Button1Text;
	public UILabel      m_Button2Text;

	private int 		m_Mode;    
	public static UInt64 m_SavedGUID;

    private const int MARRY_MODE = GlobeVar.MARRY_NPCID;
    private const int DIVORCE_MODE = GlobeVar.DIVORCE_NPCID;
    private const int PARADE_MODE = GlobeVar.PARADE_NPCID;
    private const int DIVORCE_MODE_STEP1 = 10000;
    private const int DIVORCE_MODE_STEP2 = 20000;

	void CleanUp()
	{
		m_NPCHead.gameObject.SetActive(false);
		m_NPCName.text = "";
		m_NPCTalk.text = "";
		m_Button1.SetActive (false);
		m_Button2.SetActive (false);
		m_Button1Text.text = "";
		m_Button2Text.text = "";
	}
    void Awake()
    {
        SetInstance(this);
    }

	void Start () 
    {
        
	}		
    void OnDestroy()
    {
        SetInstance(null);
    }

    void OnCloseClick()
    {
		UIManager.CloseUI(UIInfo.MarryRoot);
    }       

	public static void ShowMarryDialogUI(int nMode)
	{
		UIManager.ShowUI (UIInfo.MarryRoot, OnShowMarryDialog, nMode);
	}
	static void OnShowMarryDialog(bool bSuccess, object oMode)
	{
		if (! bSuccess) 
		{
			LogModule.ErrorLog ("load MarryDialog fail");
			return;
		}
		int nMode = (int)oMode;
		if (null != MarryRootLogic.Instance ()) 
		{
			MarryRootLogic.Instance ().DoShowMarryDialog (nMode);
		}
	}

	void UpdateNPCInfo()
	{
		Obj_NPC TargetNpc = Singleton<DialogCore>.GetInstance().CareNPC;
		if ( TargetNpc != null  )
		{
			if (TargetNpc.ModelID >= 0)
			{
				Tab_RoleBaseAttr roleBase = TableManager.GetRoleBaseAttrByID(TargetNpc.BaseAttr.RoleBaseID, 0);
				if ( roleBase != null )
				{
					Tab_CharModel charModel = TableManager.GetCharModelByID(TargetNpc.ModelID, 0);
					if ( charModel != null && m_NPCHead && m_NPCName )
					{
						m_NPCName.text = roleBase.Name;
                        Texture curTexture = ResourceManager.LoadResource("Texture/MissionRole/" + charModel.NPCSpriteName, typeof(Texture)) as Texture;
                        if (null != curTexture)
                        {
                            m_NPCHead.gameObject.SetActive(true);
                            m_NPCHead.mainTexture = curTexture;
                        }
                        else
                        {
                            m_NPCHead.gameObject.SetActive(false);
                        }
					}
				}
			}
			Tab_NpcDialog DialogLine = TableManager.GetNpcDialogByID(TargetNpc.DefaultDialogID, 0);
			if ( DialogLine != null ) 
			{
				if ( m_NPCTalk ) 
				{
					m_NPCTalk.text = StrDictionary.GetClientString_WithNameSex (DialogLine.Dialog);
				}
			}
		}
	}

	void DoShowMarryDialog(int nMode)
	{
		m_Mode = nMode;
		CleanUp ();
		UpdateNPCInfo ();
		UpdateButtons ();
	}

	void UpdateButtons()
	{
        if (m_Mode == PARADE_MODE)
        {
            m_Button1.SetActive(true);
            m_Button2.SetActive(true);
            m_Button1Text.text = StrDictionary.GetClientDictionaryString("#{3137}");
            m_Button2Text.text = StrDictionary.GetClientDictionaryString("#{3138}");
            m_NPCTalk.text = StrDictionary.GetClientDictionaryString("#{3136}");
        }

		if ( m_Mode == MARRY_MODE )
		{
			m_Button1.SetActive(true);
			m_Button2.SetActive(true);
            m_Button1Text.text = StrDictionary.GetClientDictionaryString("#{2724}");
            m_Button2Text.text = StrDictionary.GetClientDictionaryString("#{2725}");
		}
        if (m_Mode == DIVORCE_MODE)
        {
            m_Button1.SetActive(true);
            m_Button2.SetActive(true);
            m_Button1Text.text = StrDictionary.GetClientDictionaryString("#{2726}");
            m_Button2Text.text = StrDictionary.GetClientDictionaryString("#{2725}"); ;
        }
        if (m_Mode == DIVORCE_MODE_STEP1)
        {
            m_Button1.SetActive(true);
            m_Button2.SetActive(true);
            m_Button1Text.text = StrDictionary.GetClientDictionaryString("#{2729}");
            m_Button2Text.text = StrDictionary.GetClientDictionaryString("#{2730}");
            m_NPCTalk.text = StrDictionary.GetClientDictionaryString("#{2728}");
        }
        if (m_Mode == DIVORCE_MODE_STEP2)
        {
            m_Button1.SetActive(true);
            m_Button2.SetActive(true);

            int nProfession = GameManager.gameManager.PlayerDataPool.Profession;
            if (nProfession % 2 == 1)
            {
                //m_Button1Text.text = StrDictionary.GetClientDictionaryString("#{2732}","他");
                m_Button1Text.text = StrDictionary.GetClientDictionaryString("#{2862}");
            }
            else
            {
               //m_Button1Text.text = StrDictionary.GetClientDictionaryString("#{2732}","她");
                m_Button1Text.text = StrDictionary.GetClientDictionaryString("#{2863}");
            }
            m_Button2Text.text = StrDictionary.GetClientDictionaryString("#{2733}");
            m_NPCTalk.text = StrDictionary.GetClientDictionaryString("#{2731}");
        }
        
	}

	//s:0申请/1确认/2取消/3答应/4拒绝/5离婚
	//c:0二级确认/1被求/2对方取消
    public enum MARRY_PACKET_TYPE
    {
        MARRY_PACKET_ASKMARRY = 0,
        MARRY_PACKET_CONFIRM = 1,
        MARRY_PACKET_CANCEL = 2,
        MARRY_PACKET_ACCEPT =  3,
        MARRY_PACKET_REFUSE = 4,
        MARRY_PACKET_DIVORCE = 5,
        MARRY_PACKET_PARADE = 6,
    };

	void OnButton1( GameObject obj )
	{
        if (m_Mode == DIVORCE_MODE)
        {
            m_Mode = DIVORCE_MODE_STEP1;
            UpdateButtons();
            return;
        }
        if (m_Mode == DIVORCE_MODE_STEP1)
        {
            m_Mode = DIVORCE_MODE_STEP2;
            UpdateButtons();
            return;
        }
        if (m_Mode == DIVORCE_MODE_STEP2)
        {
            OnCloseClick();
            return;
        }
        if (m_Mode == PARADE_MODE)
        {
            AutoSearchPoint point = new AutoSearchPoint(13, 36.0f, 99.0f);
            if (GameManager.gameManager && GameManager.gameManager.AutoSearch)
            {
                GameManager.gameManager.AutoSearch.BuildPath(point);
            }
            OnCloseClick();
            return;
        }

		if ( GameManager.gameManager.PlayerDataPool.TeamInfo.TeamID == GlobeVar.INVALID_ID ) 
		{
			Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false,"#{2531}");
			OnCloseClick ();
			return;
		}
		UInt64 otherGUID = GameManager.gameManager.PlayerDataPool.TeamInfo.GetTeamMember (1).Guid;
		if ( otherGUID == GlobeVar.INVALID_GUID )
		{
			Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false,"#{2534}");
			OnCloseClick ();
			return;
		}

        if (m_Mode == MARRY_MODE)
		{
			CG_REQ_MARRAGE Pack = (CG_REQ_MARRAGE)PacketDistributed.CreatePacket (MessageID.PACKET_CG_REQ_MARRAGE);
			Pack.SetId (otherGUID);
            Pack.SetType((int)MARRY_PACKET_TYPE.MARRY_PACKET_ASKMARRY);
			Pack.SendPacket ();
			OnCloseClick ();
        }        
	}

	void OnButton2( GameObject obj )
	{
        if (m_Mode == DIVORCE_MODE_STEP1)
        {
            m_Mode = DIVORCE_MODE_STEP2;
            UpdateButtons();
            return;
        }
        if (m_Mode == DIVORCE_MODE_STEP2)
        {
            CG_REQ_MARRAGE Pack = (CG_REQ_MARRAGE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_REQ_MARRAGE);
            Pack.SetId(GlobeVar.INVALID_GUID);
            Pack.SetType((int)MARRY_PACKET_TYPE.MARRY_PACKET_DIVORCE);
            Pack.SendPacket();
            OnCloseClick();
            return;
        }        

        if (m_Mode == PARADE_MODE)
        {
            if (GameManager.gameManager.PlayerDataPool.TeamInfo.TeamID == GlobeVar.INVALID_ID)
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2531}");
                OnCloseClick();
                return;
            }
            UInt64 otherGUID = GameManager.gameManager.PlayerDataPool.TeamInfo.GetTeamMember(1).Guid;
            if (otherGUID == GlobeVar.INVALID_GUID)
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2534}");
                OnCloseClick();
                return;
            }

            CG_REQ_MARRAGE Pack = (CG_REQ_MARRAGE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_REQ_MARRAGE);
            Pack.SetId(otherGUID);
            Pack.SetType((int)MARRY_PACKET_TYPE.MARRY_PACKET_PARADE);
            Pack.SendPacket();
            OnCloseClick();
            return;
        }
		OnCloseClick ();
	}

	public static void Handler(GC_RET_MARRAGE packet)
	{
        if (packet.Type == (int)MARRY_PACKET_TYPE.MARRY_PACKET_ASKMARRY && packet.Id != GlobeVar.INVALID_GUID)
		{
			string strTarget = "";
			if ( GameManager.gameManager.PlayerDataPool.TeamInfo.TeamID == GlobeVar.INVALID_ID )
			{
				return;
			}
			for(int i = 0; i < GlobeVar.MAX_TEAM_MEMBER; ++i )
			{
				TeamMember tm = GameManager.gameManager.PlayerDataPool.TeamInfo.GetTeamMember(i);
				if ( null != tm && true == tm.IsValid() && tm.Guid == packet.Id )
				{
					strTarget = tm.MemberName;
				}
			}
			MarryRootLogic.m_SavedGUID = packet.Id;
			//MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{1415}", strTarget),"求婚",
            MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{1415}", strTarget),
                StrDictionary.GetClientDictionaryString("#{2864}"),	ConfirmPromise,CancelNone);
		}
        else if (packet.Type == (int)MARRY_PACKET_TYPE.MARRY_PACKET_CONFIRM && packet.Id != GlobeVar.INVALID_GUID)
		{
			string strTarget = "";
			if ( GameManager.gameManager.PlayerDataPool.TeamInfo.TeamID == GlobeVar.INVALID_ID )
			{
				return;
			}
			for(int i = 0; i < GlobeVar.MAX_TEAM_MEMBER; ++i )
			{
				TeamMember tm = GameManager.gameManager.PlayerDataPool.TeamInfo.GetTeamMember(i);
				if ( null != tm && true == tm.IsValid() && tm.Guid == packet.Id )
				{
					strTarget = tm.MemberName;
				}
			}			
			MarryRootLogic.m_SavedGUID = packet.Id;
			MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{1416}", strTarget),
                StrDictionary.GetClientDictionaryString("#{2864}"), RetPromiseOK, RetPromiseCancel, GlobeVar.INVALID_ID, MessageBoxLogic.PASSWORD.MARRYROOT);
		}
        else if (packet.Type == (int)MARRY_PACKET_TYPE.MARRY_PACKET_CANCEL && packet.Id != GlobeVar.INVALID_GUID)
		{
			if ( MarryRootLogic.m_SavedGUID == packet.Id )
			{
				MarryRootLogic.m_SavedGUID = GlobeVar.INVALID_GUID;
                if (MessageBoxLogic.Instance() != null &&
                    MessageBoxLogic.Instance().EPassword == MessageBoxLogic.PASSWORD.MARRYROOT)
                {
                    MessageBoxLogic.CloseBox();
                }                				
			}
		}
	}

	public static void ConfirmPromise()
	{
		CG_REQ_MARRAGE Pack = (CG_REQ_MARRAGE)PacketDistributed.CreatePacket (MessageID.PACKET_CG_REQ_MARRAGE);
		Pack.SetId (m_SavedGUID);
        Pack.SetType((int)MARRY_PACKET_TYPE.MARRY_PACKET_CONFIRM);
		Pack.SendPacket ();
		m_SavedGUID = GlobeVar.INVALID_GUID;
	}

	public static void CancelNone()
	{
		m_SavedGUID = GlobeVar.INVALID_GUID;
	}

	public static void RetPromiseOK()
	{
		CG_REQ_MARRAGE Pack = (CG_REQ_MARRAGE)PacketDistributed.CreatePacket (MessageID.PACKET_CG_REQ_MARRAGE);
		Pack.SetId (m_SavedGUID);
        Pack.SetType((int)MARRY_PACKET_TYPE.MARRY_PACKET_ACCEPT);
		Pack.SendPacket ();
		m_SavedGUID = GlobeVar.INVALID_GUID;
	}

	public static void RetPromiseCancel()
	{
		CG_REQ_MARRAGE Pack = (CG_REQ_MARRAGE)PacketDistributed.CreatePacket (MessageID.PACKET_CG_REQ_MARRAGE);
		Pack.SetId (m_SavedGUID);
        Pack.SetType((int)MARRY_PACKET_TYPE.MARRY_PACKET_REFUSE);
		Pack.SendPacket ();
		m_SavedGUID = GlobeVar.INVALID_GUID;
	}

}
