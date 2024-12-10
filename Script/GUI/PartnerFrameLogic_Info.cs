using UnityEngine;
using System.Collections;
using Games.Fellow;
using Games.GlobeDefine;
using GCGame.Table;
using Games.FakeObject;
using GCGame;
using System;

public class PartnerFrameLogic_Info : MonoBehaviour {

    public UIImageButton m_BattleButton;        //出战按钮
    public UIImageButton m_CallBackButton;      //召回按钮
    public UIImageButton m_LockButton;          //加锁按钮
    public UIImageButton m_UnLockButton;        //解锁按钮
    public UIImageButton m_ChangeNameButton;    //改名按钮
    public GameObject m_ChangeNameInput;        //改名弹出框

    public UISprite[] m_StarLevel;              //星级
    public UIInput m_NameInput;
    public enum ATTR
    {
        ATTR_NICKNAME = 0,          //昵称
        ATTR_LEVEL,                 //级别
        ATTR_CALLLEVEL,             //出战等级
        ATTR_TYPE,                  //类型
        ATTR_POWER,                 //战斗力
        ATTR_ATTACK,                //攻击
        ATTR_HIT,                   //命中
        ATTR_CRITICAL,              //暴击
        ATTR_ATTACKSPEED,           //攻速
        ATTR_BLESS,                 //加持
        ATTR_QUALITY,               //品质

        ATTR_NUM,                   //数量
    }
    public UILabel[] m_AttrInfo;
    public UILabel m_ExpLabel;      //经验文字
    public GameObject m_FakeObjTopLeft;
    public GameObject m_FakeObjBottomRight;

    private FakeObject m_PartnerFakeObj;
    private GameObject m_FakeObjGameObject;
    private int m_NewPlayerGuideFlag_Step = 0;
    public int NewPlayerGuideFlag_Step
    {
        get { return m_NewPlayerGuideFlag_Step; }
        set { m_NewPlayerGuideFlag_Step = value; }
    }
    public Fellow m_Fellow;

    private static PartnerFrameLogic_Info m_Instance = null;
    public ModelDragLogic m_ModelDrag;
    public static PartnerFrameLogic_Info Instance()
    {
        return m_Instance;
    }

    void OnEnable()
    {
        m_Instance = this;

        if (m_Fellow == null)
        {
            UpdateEmpty_Info();            
        }
        else
        {
            UpdateFellow_Info(m_Fellow);
        }
        GameManager.gameManager.ActiveScene.InitFakeObjRoot(m_FakeObjTopLeft, m_FakeObjBottomRight);
        GameManager.gameManager.ActiveScene.ShowFakeObj();

        Check_NewPlayerGuide();
    }

    void OnDisable()
    {
        m_Instance = null;
        DestroyPartnerFakeObj();
        GameManager.gameManager.ActiveScene.HideFakeObj();
    }

    void UpdateEmpty_Info()
    {
        for (int i = 0; i < (int)ATTR.ATTR_NUM; i++ )
        {
            m_AttrInfo[i].text = "";
        }
        m_ExpLabel.text = "";
    }

    public void UpdateFellow_Info(Fellow fellow)
    {
        UpdateCallButton(fellow);         //出战/召回按钮
        UpdateLockButton(fellow);         //加锁/解锁按钮
        UpdateModelReview(fellow);        //模型
        UpdateStarLevel(fellow);          //星级
        UpdateAttrInfo(fellow);           //属性信息
        m_Fellow = fellow;
    }

    private void UpdateAttrInfo(Fellow fellow)
    {
        if (fellow.IsValid() == false)
        {
            return;
        }

        CloseChangeName();

        Tab_FellowAttr line = TableManager.GetFellowAttrByID(fellow.DataId, 0);
        if (line != null)
        {
            m_AttrInfo[(int)ATTR.ATTR_CALLLEVEL].text = line.CallLevel.ToString();
        }
        m_AttrInfo[(int)ATTR.ATTR_POWER].text = fellow.GetCombatValue().ToString(); 
        m_AttrInfo[(int)ATTR.ATTR_TYPE].text = Fellow.GetTypeString(fellow.DataId);
        m_AttrInfo[(int)ATTR.ATTR_ATTACK].text = fellow.CombatAttr_Attack.ToString();
        m_AttrInfo[(int)ATTR.ATTR_HIT].text = fellow.CombatAttr_Hit.ToString();
        m_AttrInfo[(int)ATTR.ATTR_CRITICAL].text = fellow.CombatAttr_Critical.ToString();
        m_AttrInfo[(int)ATTR.ATTR_ATTACKSPEED].text = fellow.CombatAttr_Guard.ToString();
        m_AttrInfo[(int)ATTR.ATTR_BLESS].text = fellow.CombatAttr_Bless.ToString();

        m_AttrInfo[(int)ATTR.ATTR_NICKNAME].text = fellow.Name;
        m_AttrInfo[(int)ATTR.ATTR_LEVEL].text = fellow.Level.ToString();
        m_AttrInfo[(int)ATTR.ATTR_QUALITY].text = Utils.GetFellowQualityName(fellow.Quality);

        //经验条
        Tab_LevelUp levelupLine = TableManager.GetLevelUpByID(fellow.Level, 0);
        if (levelupLine != null)
        {
            m_ExpLabel.text = String.Format("{0}/{1}", Utils.ConvertLargeNumToString_10w(fellow.Exp), Utils.ConvertLargeNumToString_10w(levelupLine.FellowExpNeed));
        }
    }

    string NowStarSpriteName(int starLevel)
    {
        int starQuality = starLevel / 12;
        switch(starQuality)
        {
            case 0:
			return "ui_pub_048";
            case 1:
			return "ui_pub_049";
            case 2:
			return "ui_pub_050";
            case 3:
			return "ui_pub_051";
            case 4:
			return "ui_pub_052";
            default:
			return "ui_pub_053";
        };
    }

    string PerStarSpriteName(int starLevel)
    {
        int starQuality = starLevel / 12;
        switch (starQuality - 1)
        {
            case -1:
			return "ui_pub_053";
            case 0:
			return "ui_pub_048";
			case 1:
			return "ui_pub_049";
			case 2:
			return "ui_pub_050";
			case 3:
			return "ui_pub_051";
			case 4:
			return "ui_pub_052";
            default:
			return "ui_pub_053";
        };
    }

    int NowStarNum(int starLevel)
    {
        return starLevel % 12;
    }

    private void UpdateStarLevel(Fellow fellow)
    {
        int starLevel = fellow.StarLevel;
        string nowStarSpriteName = NowStarSpriteName(starLevel);
        string perStarSpriteName = PerStarSpriteName(starLevel);
        int nowStarNum = NowStarNum(starLevel);

        for (int i = 0; i < 12; i++)
        {
            if (i < nowStarNum)
            {
                m_StarLevel[i].spriteName = nowStarSpriteName;
            }
            else
            {
                m_StarLevel[i].spriteName = perStarSpriteName;
            }
        }
    }

    private void UpdateModelReview(Fellow fellow)
    {
        //DestroyPartnerFakeObj();
        UpdatePartnerFakeObj(fellow);
    }

    private void UpdateLockButton(Fellow fellow)
    {
        if (fellow.Locked)
        {
            m_LockButton.gameObject.SetActive(false);
            m_UnLockButton.gameObject.SetActive(true);
        }
        else
        {
            m_LockButton.gameObject.SetActive(true);
            m_UnLockButton.gameObject.SetActive(false);
        }
    }

    private void UpdateCallButton(Fellow fellow)
    {
        if (fellow.Called)
        {
            m_BattleButton.gameObject.SetActive(false);
            m_CallBackButton.gameObject.SetActive(true);
        }
        else
        {
            m_BattleButton.gameObject.SetActive(true);
            m_CallBackButton.gameObject.SetActive(false);
        }
    }

    //召出伙伴
    public void OnCallClick()
    {
        if (m_Fellow != null)
        {
            if (m_Fellow.IsValid())
            {
                CG_CALL_FELLOW callFellow = (CG_CALL_FELLOW)PacketDistributed.CreatePacket(MessageID.PACKET_CG_CALL_FELLOW);
                callFellow.SetGuid(m_Fellow.Guid);
                callFellow.SendPacket();
                //播放音效
                GameManager.gameManager.SoundManager.PlaySoundEffect(30);       //pet_call
            }
        }
		if(PartnerFrameLogic.Instance() && m_NewPlayerGuideFlag_Step == 1)
		{
			NewPlayerGuide( 2 );
		}
    }

    //召回伙伴
    public void OnUnCallClick()
    {
        if (m_Fellow != null)
        {
            if (m_Fellow.IsValid())
            {
                CG_UNCALL_FELLOW uncallFellow = (CG_UNCALL_FELLOW)PacketDistributed.CreatePacket(MessageID.PACKET_CG_UNCALL_FELLOW);
                uncallFellow.SetGuid(m_Fellow.Guid);
                uncallFellow.SendPacket();
            }
        }
    }

    //伙伴加锁
    public void OnLockClick()
    {
        if (m_Fellow != null)
        {
            if (m_Fellow.IsValid())
            {
                CG_LOCK_FELLOW lockFellow = (CG_LOCK_FELLOW)PacketDistributed.CreatePacket(MessageID.PACKET_CG_LOCK_FELLOW);
                lockFellow.SetGuid(m_Fellow.Guid);
                lockFellow.SendPacket();
            }
        }
    }

    //伙伴解锁
    public void OnUnLockClick()
    {
        if (m_Fellow != null)
        {
            if (m_Fellow.IsValid())
            {
                CG_UNLOCK_FELLOW unlockFellow = (CG_UNLOCK_FELLOW)PacketDistributed.CreatePacket(MessageID.PACKET_CG_UNLOCK_FELLOW);
                unlockFellow.SetGuid(m_Fellow.Guid);
                unlockFellow.SendPacket();
            }
        }
    }

    private void DestroyPartnerFakeObj()
    {
        if (m_PartnerFakeObj != null)
        {
            m_PartnerFakeObj.Destroy();
            m_PartnerFakeObj = null;
        }
    }

    private void UpdatePartnerFakeObj(Fellow fellow)
    {
        if (m_FakeObjGameObject == null || m_PartnerFakeObj == null)
        {
            if (m_PartnerFakeObj != null)
            {
                DestroyPartnerFakeObj();
            }

            Tab_FellowAttr line = TableManager.GetFellowAttrByID(fellow.DataId, 0);
            if (line == null)
            {
                return;
            }
            int fakeObjId = line.FakeObjId;

            m_PartnerFakeObj = new FakeObject();
            if (m_PartnerFakeObj == null)
            {
                return;
            }

            m_PartnerFakeObj.initFakeObject(fakeObjId, GameManager.gameManager.ActiveScene.FakeObjTrans, out m_FakeObjGameObject);
            m_ModelDrag.ModelTrans = m_PartnerFakeObj.ObjAnim.transform;
        }
        else
        {
            Tab_FellowAttr line = TableManager.GetFellowAttrByID(fellow.DataId, 0);
            if (line == null)
            {
                return;
            }
            int fakeObjId = line.FakeObjId;

            Tab_FakeObject FakeObjTable = TableManager.GetFakeObjectByID(fakeObjId, 0);
            if (FakeObjTable == null)
            {
                return;
            }

            Singleton<ObjManager>.GetInstance().ReloadModel(m_FakeObjGameObject,
                FakeObjTable.FakeObjModel,
                Singleton<ObjManager>.GetInstance().AsycLoadFakeObjOver,
                FakeObjTable,
                m_PartnerFakeObj);
        }
    }

    public void OnClickSaveName()
    {
        if (m_Fellow == null)
        {
            //未选择伙伴
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1836}");
            return;
        }
        if (string.IsNullOrEmpty(m_NameInput.value))
        {
            //请输入伙伴名称
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1837}");
            return;
        }

        int curCharNum = 0;     // 英文算一个，中文算两个 
        foreach (char curChar in m_NameInput.value)
        {
            if ((int)curChar >= 128)
            {

                curCharNum += 2; ;
            }
            else if ((int)curChar >= 65 && (int)curChar <= 90)
            {
                curCharNum += 2;
            }
            else
            {
                curCharNum += 1;
            }

            if (char.IsWhiteSpace(curChar))
            {
                //名字不能包含空格
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1280}");
                return;
            }
        }

		// name is enmoji error
		if(containsEmoji(m_NameInput.value))
		{
			// enmoji error
			Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1278}");
			return;
		}

		//name length set 6//   change to 8
        if (curCharNum > 8)
        {
            // 名字过长
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1279}");
            return;
        }

        if (m_NameInput.value.Contains("*"))
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1278}");
            return;
        }

        if (null == Utils.GetStrFilter(m_NameInput.value, (int)Games.GlobeDefine.GameDefine_Globe.STRFILTER_TYPE.STRFILTER_NAME))
        {
            CG_FELLOW_CHANGE_NAME fellowPak = (CG_FELLOW_CHANGE_NAME)PacketDistributed.CreatePacket(MessageID.PACKET_CG_FELLOW_CHANGE_NAME);
            fellowPak.SetFellowguid(m_Fellow.Guid);
            fellowPak.SetName(m_NameInput.value);
            fellowPak.SendPacket();

            CloseChangeName();
        }
        else
        {
            // 包含非法字符
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1278}");
        }
    }

	public  bool containsEmoji(String source) {   
		int len = source.Length;    
		char[] codePointArr = source.ToCharArray();
		for (int i = 0; i < len; i++) { 
			char codePoint = codePointArr[i];
			if (!isEmojiCharacter(codePoint)) { 
				return true;
			}
		}        return false;
	}


	private  bool isEmojiCharacter(char codePoint) {  
		return (codePoint == 0x0) || (codePoint == 0x9) || (codePoint == 0xA) ||
		(codePoint == 0xD) || ((codePoint >= 0x20) && (codePoint <= 0xD7FF)) ||
			((codePoint >= 0xE000) && (codePoint <= 0xFFFD)) || ((codePoint >= 0x10000)
			                                                     && (codePoint <= 0x10FFFF));

	}


    public void OnClickChangeName()
    {
        if (m_Fellow == null)
        {
            //未选择伙伴
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1836}");
            return;
        }
        if (m_Fellow.Called)
        {
            //当前出战伙伴不能改名
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2134}");
            return;
        }

        OpenChangeName();
        m_NameInput.selected = true;
    }

    public void OnSubmitInput()
    {
        
    }

    public void OnClickCloseChangeName()
    {
        CloseChangeName();
    }

    void CloseChangeName()
    {
        m_ChangeNameInput.gameObject.SetActive(false);
        m_FakeObjGameObject.gameObject.SetActive(true);
    }

    void OpenChangeName()
    {
        m_ChangeNameInput.gameObject.SetActive(true);
        if (m_Fellow != null)
        {
            m_NameInput.value = m_Fellow.Name;
        }
        m_NameInput.selected = true;
        m_FakeObjGameObject.gameObject.SetActive(false);
    }

    void Check_NewPlayerGuide()
    {
        if (m_NewPlayerGuideFlag_Step > 0 )
        {
            NewPlayerGuide(m_NewPlayerGuideFlag_Step);
        }
    }

    public void NewPlayerGuide(int index)
    {

        if (index < 0)
        {
            return;
        }

        m_NewPlayerGuideFlag_Step = index;
        switch (index)
        {
	    case 1:
			NewPlayerGuidLogic.OpenWindow(m_BattleButton.gameObject, 202, 64, "", "right", 2, true, true);   
	        break;
	    case 2: // 返回指引
			PartnerAndMountLogic.Instance().NewPlayerGuide((int)GameDefine_Globe.NEWOLAYERGUIDE.UI_CLOSE);
	        m_NewPlayerGuideFlag_Step = -1;
	        break;
	    default:
	        break;
        }
    }
}
