using UnityEngine;
using System.Collections;
using GCGame.Table;
using Games.GlobeDefine;
using Games.LogicObj;
using Games.UserCommonData;
using System.Collections.Generic;

public class NewItemGetLogic : UIControllerBase<NewItemGetLogic>{

    public enum NEWITEMTYPE
    {
        TYPE_INVALID = -1,
        TYPE_SKILL = 0,
        TYPE_MENUBAR,
        TYPE_FUNCTION,
    }
    private NEWITEMTYPE m_ItemType = NEWITEMTYPE.TYPE_INVALID;

    public UISprite m_ItemIcon;
    public TweenPosition m_ItemTween;
    public GameObject m_NoticeLabel;
    
    public GameObject BiliEffect;
     
    private GameObject m_destGameObject;
    private int m_IntParam = GlobeVar.INVALID_ID;

    public int m_NewPlayerGuide_Step = 0;

    public UIAtlas SkillAtlas;
    public UIAtlas MainMenuBarAtlas;
    public UIAtlas MainUIAtlas;

    public UILabel m_NameLabel;
    public UILabel m_DescLabel;

    void Awake()
    {
        SetInstance(this);
    }

	// Use this for initialization
	void Start () {
	
	}

    private class InitDataInfo
    {
        public InitDataInfo(string spriteName, GameObject destGameObject, NEWITEMTYPE type, int nNewSkillID)
        {
            _spriteName = spriteName;
            _destGameObject = destGameObject;
            _type = type;
            _nNewSkillID = nNewSkillID;
        }
        public string _spriteName;
        public GameObject _destGameObject;
        public NEWITEMTYPE _type;
        public int _nNewSkillID;
    }

    private static List<InitDataInfo> m_ItemListBuffer = new List<InitDataInfo>();
    private static bool m_bShow = false;

    public static void InitItemInfo(string spriteName, GameObject destGameObject, NEWITEMTYPE type, int nNewSkillID = GlobeVar.INVALID_ID)
    {
        InitDataInfo curInfo = new InitDataInfo(spriteName, destGameObject, type, nNewSkillID);
        if (!m_bShow)
        {
            UIManager.ShowUI(UIInfo.NewItemGetRoot, OnLoadItemGetRoot, curInfo);
            m_bShow = true;
        }
        else
        {
            m_ItemListBuffer.Add(curInfo);         
        }
    }

    static void OnLoadItemGetRoot(bool bSuccess, object param)
    {
        // 异常 如果资源加载失败
        if (bSuccess == false)
        {
            InitDataInfo curInfo = param as InitDataInfo;
            if (curInfo == null)
            {
                return;
            }
            if (curInfo._type == NEWITEMTYPE.TYPE_SKILL)
            {
                if (SkillBarLogic.Instance() != null)
                {
                    SkillBarLogic.Instance().UpdateSkillBarInfo();
                    /*
                    if (GameManager.gameManager.PlayerDataPool.ForthSkillFlag == true)
                    {
                        if (SkillBarLogic.Instance())
                        {
                            SkillBarLogic.Instance().NewPlayerGuide(4);
                        }
                    }*/
                }
            }
            else
            {
                if (curInfo._type == NEWITEMTYPE.TYPE_MENUBAR)
                {
                    if (MenuBarLogic.Instance() != null)
                    {
                        MenuBarLogic.Instance().InitButtonActive();
                    }
                    if (PlayerFrameLogic.Instance() != null)
                    {
                        PlayerFrameLogic.Instance().AddRemindNum();
                    }
                }
                else if (curInfo._type == NEWITEMTYPE.TYPE_FUNCTION)
                {
                    if (FunctionButtonLogic.Instance() != null)
                    {
                        FunctionButtonLogic.Instance().InitButtonActive();
                        FunctionButtonLogic.Instance().PlayNewButtonEffect();
                    }
                }
            }
            return;
        }

        if (NewItemGetLogic.Instance() != null)
        {
            InitDataInfo curInfo = param as InitDataInfo;
            NewItemGetLogic.Instance().Init(curInfo._spriteName, curInfo._destGameObject, curInfo._type, curInfo._nNewSkillID);
            if (JoyStickLogic.Instance() != null)
            {
                ProcessInput.Instance().ReleaseTouch();
                JoyStickLogic.Instance().ReleaseJoyStick();
            }
        }
    }

    void Init(string spriteName, GameObject destGameObject, NEWITEMTYPE type, int IntParam = GlobeVar.INVALID_ID)
    {
        BiliEffect.SetActive(false);
        m_destGameObject = destGameObject;

        if (type == NEWITEMTYPE.TYPE_SKILL)
        {
            m_ItemIcon.atlas = SkillAtlas;
            m_ItemIcon.spriteName = spriteName;
            m_ItemIcon.MakePixelPerfect();
            m_NoticeLabel.GetComponent<UILabel>().text = StrDictionary.GetClientDictionaryString("#{1596}");

            Tab_SkillEx tabSkillEx = TableManager.GetSkillExByID(IntParam, 0);
            if (tabSkillEx != null)
            {
                Tab_SkillBase _skillBase = TableManager.GetSkillBaseByID(tabSkillEx.BaseId, 0);
                if (_skillBase !=null)
                {
                    m_NameLabel.text = _skillBase.Name;
                    m_DescLabel.text = "      " + tabSkillEx.Desc;
                    m_ItemIcon.gameObject.transform.localScale = 0.7f * Vector3.one;
                }
            }            
        }
        else if (type == NEWITEMTYPE.TYPE_MENUBAR)
        {
            m_ItemIcon.atlas = MainMenuBarAtlas;
            m_ItemIcon.spriteName = spriteName;
            m_ItemIcon.MakePixelPerfect();
            m_ItemIcon.gameObject.transform.localScale = Vector3.one;
            m_NoticeLabel.GetComponent<UILabel>().text = StrDictionary.GetClientDictionaryString("#{1863}");
            m_NameLabel.text = "";
            m_DescLabel.text = "";
        }
        else if (type == NEWITEMTYPE.TYPE_FUNCTION)
        {
            m_ItemIcon.atlas = MainUIAtlas;
            m_ItemIcon.spriteName = spriteName;
            m_ItemIcon.MakePixelPerfect();
            m_ItemIcon.gameObject.transform.localScale = Vector3.one;
            m_NoticeLabel.GetComponent<UILabel>().text = StrDictionary.GetClientDictionaryString("#{1863}");
            m_NameLabel.text = "";
            m_DescLabel.text = "";
        }

        m_ItemTween.Reset();
        m_ItemTween.to = transform.parent.InverseTransformPoint(destGameObject.transform.position);

        m_ItemType = type;

        m_IntParam = IntParam;

       // NewPlayerGuider(1);
    }

    void NewItemOnClick()
    {
        BiliEffect.SetActive(true);
        m_ItemTween.Play();
        m_NoticeLabel.SetActive(false);
        //StartCoroutine(SkillPlayTween()); 
    }

    //IEnumerator SkillPlayTween() 
    //{
 
    //    m_ItemTween.Play();
    //    m_NoticeLabel.SetActive(false);
    //    /*
    //    if (m_NewPlayerGuide_Step == 1)
    //    {
    //        NewPlayerGuidLogic.CloseWindow();
    //    }*/
    //}

    public void AfterTweenPosition()
    {
        if (m_ItemType == NEWITEMTYPE.TYPE_SKILL)
        {
            if(SkillBarLogic.Instance() != null)
            {
                if (GetItemListSkillCount() == 0)
                {
                    //播放新技能特效
                    SkillBarLogic.Instance().PlayNewSkillEffect(m_destGameObject);

                    SkillBarLogic.Instance().UpdateSkillBarInfo();
                }

//                 if (m_NewPlayerGuide_Step == 1 &&
//                     (m_IntParam == (int)GameDefine_Globe.PLAYER_FIRSTSKILL.FIRSTSKILL_SHAOLIN ||
//                     m_IntParam == (int)GameDefine_Globe.PLAYER_FIRSTSKILL.FIRSTSKILL_TIANSHAN ||
//                     m_IntParam == (int)GameDefine_Globe.PLAYER_FIRSTSKILL.FIRSTSKILL_DALI ||
//                     m_IntParam == (int)GameDefine_Globe.PLAYER_FIRSTSKILL.FIRSTSKILL_XIAOYAO))
//                 {
//                     if (SkillBarLogic.Instance() != null)
//                     {
//                         SkillBarLogic.Instance().NewPlayerGuide(3);
//                         m_NewPlayerGuide_Step = 0;
//                     }
//                 }
                /*
                if (m_NewPlayerGuide_Step == 1)
                {
                    m_NewPlayerGuide_Step = 0;
                }
                
                if (GameManager.gameManager.PlayerDataPool.ForthSkillFlag == true)
                {
                    if (SkillBarLogic.Instance())
                    {
                        SkillBarLogic.Instance().NewPlayerGuide(4);
                    }
                }*/

            }
        }
        else
        {
            if (m_ItemType == NEWITEMTYPE.TYPE_MENUBAR)
            {
                if (MenuBarLogic.Instance() != null)
                {
                    MenuBarLogic.Instance().InitButtonActive();
                }
                if (PlayerFrameLogic.Instance() != null)
                {
                    PlayerFrameLogic.Instance().AddRemindNum();
                }
            }
            else if (m_ItemType == NEWITEMTYPE.TYPE_FUNCTION)
            {
                if (FunctionButtonLogic.Instance() != null)
                {
                    FunctionButtonLogic.Instance().InitButtonActive();
                    FunctionButtonLogic.Instance().PlayNewButtonEffect();
                }
            }
            MissionNewPlayerGuide(m_IntParam);
        }

        m_ItemTween.enabled = false;

        if (m_ItemListBuffer.Count > 0)
        {
            InitDataInfo curInfo = m_ItemListBuffer[0];
            NewItemGetLogic.Instance().Init(curInfo._spriteName, curInfo._destGameObject, curInfo._type, curInfo._nNewSkillID);
            m_ItemListBuffer.RemoveAt(0);
        }
        else
        {
            UIManager.CloseUI(UIInfo.NewItemGetRoot);
            m_bShow = false;
        }                
    }

    int GetItemListSkillCount()
    {
        int nCount = 0;
        for (int i = 0; i < m_ItemListBuffer.Count; i++ )
        {
            if (m_ItemListBuffer[i]._type == NEWITEMTYPE.TYPE_SKILL)
            {
                nCount += 1;
            }
        }
        return nCount;
    }

    public void NewPlayerGuider(int nIndex)
    {
        m_NewPlayerGuide_Step = nIndex;

        if (NewPlayerGuidLogic.IsOpenFlag == true)
        {
            NewPlayerGuidLogic.CloseWindow();
        }
        switch (nIndex)
        {
            case 1:
                NewPlayerGuidLogic.OpenWindow(m_ItemIcon.gameObject, 120, 120, "", "", 2, true);
                break;
            case 2: // 第四个技能滑动
                break;
            default:
                break;
        }
    }

    void MissionNewPlayerGuide(int nType)
    {
        bool bRet = false;
        switch (nType)
        {
            case (int)USER_COMMONFLAG.CF_FELLOWFUNCTION_OPENFLAG:
                {
                    bRet = GameManager.gameManager.MissionManager.IsHaveMission(14);
                    if (bRet)
                    {
                        NewPlayerGuide.NewPlayerGuideOpt("PlayerFrame", 2);
                    }
                }
                break;
            case (int)USER_COMMONFLAG.CF_BELLEFUNCTION_OPENFLAG:
                {
                    bRet = GameManager.gameManager.MissionManager.IsHaveMission(0);
                    if (false)
                    {
                        NewPlayerGuide.NewPlayerGuideOpt("PlayerFrame", 1);
                    }
                }
                break;
            case (int)USER_COMMONFLAG.CF_ACTIVITYFUNCTION_OPENFLAG:
                {
                    bRet = GameManager.gameManager.MissionManager.IsHaveMission(49);
                    if (bRet)
                    {
                        NewPlayerGuide.NewPlayerGuideOpt("FunctionButton", 2);
                    }
                }
                break;
            case (int)USER_COMMONFLAG.CF_STRENGTHENFUNCTION_OPENFLAG:
                {
                    bRet = GameManager.gameManager.MissionManager.IsHaveMission(23);
                    if (bRet)
                    {
                        NewPlayerGuide.NewPlayerGuideOpt("PlayerFrame", 0);
                    }
                }
                break;
            case (int)USER_COMMONFLAG.CF_RESTAURANTFUNCTION_OPENFLAG:
                {
                    bRet = GameManager.gameManager.MissionManager.IsHaveMission(37);
                    if (bRet)
                    {
                        NewPlayerGuide.NewPlayerGuideOpt("PlayerFrame", 5);
                    }
                }
                break;
            case (int)USER_COMMONFLAG.CF_GUILDFUNCTION_OPENFLAG:
                {
                    bRet = GameManager.gameManager.MissionManager.IsHaveMission(6);
                    if (bRet)
                    {
                        NewPlayerGuide.NewPlayerGuideOpt("PlayerFrame", 8);
                    }
                }
                break;
            case (int)USER_COMMONFLAG.COUNTER_DB_OPEN_GEM:
                {
                    bRet = GameManager.gameManager.MissionManager.IsHaveMission(6);
                    if (bRet)
                    {
                        NewPlayerGuide.NewPlayerGuideOpt("PlayerFrame", 9);
                    }
                }
                break;
            case (int)USER_COMMONFLAG.CF_XIAKEFUNCTION_OPENFLAG:
                break;
        }
    }
}