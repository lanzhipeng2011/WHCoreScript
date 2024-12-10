using UnityEngine;
using System.Collections;
using GCGame.Table;
using GCGame;
using Module.Log;
using Games.GlobeDefine;
using Games.Item;
using Games.UserCommonData;
using System.Collections.Generic;
using Games.LogicObj;
using System;

public class OptionDialogLogic : UIControllerBase<OptionDialogLogic>
{
    public enum DialogEventType
    {
        EVENT_BLACKMARKET =1,   //黑市商人
        EVENT_CANGKU = 2,       //仓库
        EVENT_AWARD = 3,        //奖励NPC
        EVENT_GUILDBUSINESS = 5,//跑商NPC
    }
    public UITexture    m_NPCHead;	
    public UILabel      m_NPCTalk;
    public UILabel      m_NPCName;

    public GameObject   m_Button1;
    public GameObject   m_Button2;
    public UILabel      m_Button1Text;
	public UILabel      m_Button2Text;

    private Tab_NpcDialog m_NpcDialogInfo;
    private int m_curOptionDialogId;
    void CleanUp()
    {
        m_NpcDialogInfo = null;
        m_NPCHead.gameObject.SetActive(false);
        m_NPCName.text = "";
        m_NPCTalk.text = "";
        m_Button1.SetActive(false);
        m_Button2.SetActive(false);
        m_Button1Text.text = "";
        m_Button2Text.text = "";
        m_curOptionDialogId = -1;
    }
    void Awake()
    {
        SetInstance(this);
    }

    void Start()
    {

    }
    void OnDestroy()
    {
        SetInstance(null);
    }

    void OnCloseClick()
    {
        UIManager.CloseUI(UIInfo.OptionDialogRoot);
    }

    public static void ShowOptionDialogUI(int nDialogId)
    {
        UIManager.ShowUI(UIInfo.OptionDialogRoot, OnShowOptionDialog, nDialogId);
    }
    static void OnShowOptionDialog(bool bSuccess, object param)
    {
        if (!bSuccess)
        {
            LogModule.ErrorLog("load OptionDialog fail");
            return;
        }
        int _DialogId = (int)param;
        if (null != OptionDialogLogic.Instance())
        {
            OptionDialogLogic.Instance().DoShowOptionDialog(_DialogId);
        }
    }

    void UpdateNPCInfo()
    {
        Obj_NPC TargetNpc = Singleton<DialogCore>.GetInstance().CareNPC;
        if (TargetNpc != null)
        {
            if (TargetNpc.ModelID >= 0)
            {
                Tab_RoleBaseAttr roleBase = TableManager.GetRoleBaseAttrByID(TargetNpc.BaseAttr.RoleBaseID, 0);
                if (roleBase != null)
                {
                    Tab_CharModel charModel = TableManager.GetCharModelByID(TargetNpc.ModelID, 0);
                    if (charModel != null && m_NPCHead && m_NPCName)
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
            if (m_curOptionDialogId != -1)
            {
                Tab_NpcOptionDialog _npcOptionInfo = TableManager.GetNpcOptionDialogByID(m_curOptionDialogId, 0);
                if (_npcOptionInfo != null)
                {
                    m_NPCTalk.text = StrDictionary.GetClientString_WithNameSex(_npcOptionInfo.CenterText);
                }
            }
        }
    }
    void DoShowOptionDialog(int nDialogId)
    {
        CleanUp();
        m_NpcDialogInfo = TableManager.GetNpcDialogByID(nDialogId,0);
        if (m_NpcDialogInfo !=null)
        {
            m_curOptionDialogId = m_NpcDialogInfo.OptionDialogId;
            UpdateNPCInfo();
            UpdateButtons();
        }
    }

    private void UpdateButtons()
    {
        m_Button1.SetActive(true);
        m_Button2.SetActive(true);
        if (m_curOptionDialogId != -1)
        {
            Tab_NpcOptionDialog _npcOptionInfo = TableManager.GetNpcOptionDialogByID(m_curOptionDialogId, 0);
            if (_npcOptionInfo !=null)
            {
                m_Button1Text.text = _npcOptionInfo.Option1Text;
                m_Button2Text.text = _npcOptionInfo.Option2Text;
            }
        }
       
    }

    void OnButton1(GameObject obj)
    {
        if (m_curOptionDialogId != -1)
        {
            Tab_NpcOptionDialog _npcOptionInfo = TableManager.GetNpcOptionDialogByID(m_curOptionDialogId, 0);
            if (_npcOptionInfo != null)
            {
                //有下一个对话的显示下一个对话
                if (_npcOptionInfo.Option1NextDialog !=-1)
                {
                    m_curOptionDialogId = _npcOptionInfo.Option1NextDialog;
                    UpdateButtons();
                    UpdateNPCInfo();
                }
                else
                {
                    //没有对话  做对应的处理
                    OnProcessEvent(1);
                }
            }
        }
    }

    void OnButton2(GameObject obj)
    {
        if (m_curOptionDialogId != -1)
        {
            Tab_NpcOptionDialog _npcOptionInfo = TableManager.GetNpcOptionDialogByID(m_curOptionDialogId, 0);
            if (_npcOptionInfo != null)
            {
                //有下一个对话的显示下一个对话
                if (_npcOptionInfo.Option1NextDialog != -1)
                {
                    m_curOptionDialogId = _npcOptionInfo.Option1NextDialog;
                    UpdateButtons();
                    UpdateNPCInfo();
                }
                else
                {
                    //没有对话  做对应的处理
                    OnProcessEvent(2);
                }
            }
        }
    }
    void OnProcessEvent(int OptionIndex)
    {
        if (m_NpcDialogInfo != null)
        {
            DialogEventType _EventType = (DialogEventType)m_NpcDialogInfo.OptionDialogType;
            switch (_EventType)
            {
                case DialogEventType.EVENT_BLACKMARKET:
                    if (OptionIndex == 1)
                    {
                        //选项一
                        UIManager.ShowUI(UIInfo.BlackMarket);
                    }
                    break;
                case DialogEventType.EVENT_CANGKU:
                    if (OptionIndex == 1)
                    {
                        //选项一
                        UIManager.ShowUI(UIInfo.CangKu);
                    }
                    break;
                case DialogEventType.EVENT_AWARD:
                    if (OptionIndex == 1)
                    {
                        bool bRet = GameManager.gameManager.PlayerDataPool.CommonData.GetCommondFlag((int)USER_COMMONFLAG.CF_CYFANS_AWARD_FLAG);
                        if (!bRet)
                        {
                            CG_ASK_SPECIALAWARD Pack = (CG_ASK_SPECIALAWARD)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_SPECIALAWARD);
                            Pack.Awardid = (int)USER_COMMONFLAG.CF_CYFANS_AWARD_FLAG;
                            Pack.SendPacket();
                        }
                        else if ( Singleton<ObjManager>.GetInstance().MainPlayer )
                        {
                            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{3271}");
                        }
                    }
                    break;
                case DialogEventType.EVENT_GUILDBUSINESS:
                    if (OptionIndex > 0) 
                    {
                        if (Singleton<ObjManager>.GetInstance().MainPlayer) 
                        {
                            Singleton<ObjManager>.GetInstance().MainPlayer.DoAcceptGuildBusiness(OptionIndex);
                        }
                    }
                    break;
                default:
                    break;
            }
            OnCloseClick();
        }

    }
}
