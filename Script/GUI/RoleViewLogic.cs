using System;
using System.Security.Cryptography.X509Certificates;
using Games.GlobeDefine;
using Games.Item;
using Games.LogicObj;
using GCGame.Table;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Games.FakeObject;
using GCGame;
using Module.Log;
using Games.UserCommonData;
public class RoleViewLogic : MonoBehaviour
{

	// Use this for initialization
    private static RoleViewLogic m_Instance = null;
    public static RoleViewLogic Instance()
    {
        return m_Instance;
    }

    public enum CONTENT_TYPE
    {
        CONTENT_TYPE_INVALID = -1,
        CONTENT_TYPE_ATTR = 0,        // 属性
        CONTENT_TYPE_TITLE = 1,       // 称号
        CONTENT_TYPE_FASHION,         // 时装
        CONTENT_TYPE_GEM,             // 宝石
    }


    public UISprite   m_VipSprite;  
    public GameObject m_AttrView;//属性视图
    public GameObject m_TitleView;//称号视图
    public GameObject m_FashionView;//时装视图

    public UISprite[] m_EquipSlotChooseSprite;
    public UISprite[] m_EquipSlotIcon;
    public UISprite[] m_EquipSlotQualityFrame;

    private string m_strName = "";
    public string Name
    {
        get { return m_strName; }
        set { m_strName = value; }
    }
    private int m_nCombatValue =0;
    public int CombatValue
    {
        get { return m_nCombatValue; }
        set { m_nCombatValue = value; }
    }
    private int m_nLevel =0;
    public int Level
    {
        get { return m_nLevel; }
        set { m_nLevel = value; }
    }
    private int m_Profession =-1;
    public int Profession
    {
        get { return m_Profession; }
        set { m_Profession = value; }
    }
   
   
    private UInt64 m_RoleGUID =0;
    public System.UInt64 RoleGUID
    {
        get { return m_RoleGUID; }
        set { m_RoleGUID = value; }
    }
    private int m_nCurHp =0;
    public int CurHp
    {
        get { return m_nCurHp; }
        set { m_nCurHp = value; }
    }
    private int m_nMaxHP =0;
    public int MaxHP
    {
        get { return m_nMaxHP; }
        set { m_nMaxHP = value; }
    }
    private int m_nCurMp =0;
    public int CurMp
    {
        get { return m_nCurMp; }
        set { m_nCurMp = value; }
    }
    
    private int m_nMaxMp = 0;
    public int MaxMp
    {
        get { return m_nMaxMp; }
        set { m_nMaxMp = value; }
    }
    private int m_nCurExp = 0;
    public int CurExp
    {
        get { return m_nCurExp; }
        set { m_nCurExp = value; }
    }
    private int m_nMaxExp = 0;
    public int MaxExp
    {
        get { return m_nMaxExp; }
        set { m_nMaxExp = value; }
    }

    private int m_nOffLineExp = 0;
    public int OffLineExp
    {
        get { return m_nOffLineExp; }
        set { m_nOffLineExp = value; }
    }

    private int m_nOffLineMaxExp = 0;
    public int OffLineMaxExp
    {
        get { return m_nOffLineMaxExp; }
        set { m_nOffLineMaxExp = value; }
    }

    private int m_nPAttck =0;//物攻
    public int PAttck
    {
        get { return m_nPAttck; }
        set { m_nPAttck = value; }
    }
    private int m_nMAttack =0;//法功
    public int MAttack
    {
        get { return m_nMAttack; }
        set { m_nMAttack = value; }
    }
    private int m_nHit =0;
    public int Hit
    {
        get { return m_nHit; }
        set { m_nHit = value; }
    }
    private int m_nCritical = 0;//暴击
    public int Critical
    {
        get { return m_nCritical; }
        set { m_nCritical = value; }
    }
    private int m_nPDefense = 0;//物防
    public int PDefense
    {
        get { return m_nPDefense; }
        set { m_nPDefense = value; }
    }
    private int m_MDefense =0;//法防
    public int MDefense
    {
        get { return m_MDefense; }
        set { m_MDefense = value; }
    }
    private int m_DeCritical = 0;//暴击抗性
    public int DeCritical
    {
        get { return m_DeCritical; }
        set { m_DeCritical = value; }
    }
    private int m_nDoge = 0;//闪避
    public int Doge
    {
        get { return m_nDoge; }
        set { m_nDoge = value; }
    }
    private int m_nStrike = 0;//穿透
    public int Strike
    {
        get { return m_nStrike; }
        set { m_nStrike = value; }
    }
    private int m_nCriticalAdd = 0;//暴击加成
    public int CriticalAdd
    {
        get { return m_nCriticalAdd; }
        set { m_nCriticalAdd = value; }
    }
    private int m_nDuctical = 0;//韧性
    public int Ductical
    {
        get { return m_nDuctical; }
        set { m_nDuctical = value; }
    }
    private int m_nCriticalMis = 0;//暴击减免
    public int CriticalMis
    {
        get { return m_nCriticalMis; }
        set { m_nCriticalMis = value; }
    }

    private bool m_bIsNeedUpdateAttr = true;

    public GameObject m_LeftView;
    public GameObject m_LeftAttrView_01;
    public GameObject m_LeftAttrView_02;
    public GameObject m_LeftAttrView_03;
    public GameObject m_ShowMoreInfoBt;
    public GameObject m_CancelShowInfoBt;

    public UILabel m_CombatValueLable;
    public UILabel m_LevelValueLable;
    public UILabel m_ProfessionValueLable;
    public UILabel m_GUIDValueLable;
    public UILabel m_HPValueLable;
    public UILabel m_MPValueLable;
    public UILabel m_ExpValueLable;
    public UISprite m_HPSprite;
    public UISprite m_MPSprite;
    public UISprite m_ExpSprite;
    public UISprite m_OffLineExpSprite;

    public UILabel m_PAttackLable;
    public UILabel m_MAttckLabel;
    public UILabel m_AttackValueLable;
    public UILabel m_HitValueLabel;
    public UILabel m_CriticalValueLable;
    public UILabel m_PDefenseValueLable;
    public UILabel m_MDefenseValueLable;
    public UILabel m_DecriticalValueLabel;
    public UILabel m_DodgeValueLable;

    public UILabel m_StrikeValueLable;
    public UILabel m_CriticalAddValueLable;
    public UILabel m_DucticalValueLable;
    public UILabel m_CriticalMisValueLable;

    public UILabel m_NameLabel;
    public GameObject m_EquipPackGrid;

    public GameObject m_FakeObjTopLeft;
    public GameObject m_FakeObjBottomRight;
    private FakeObject m_PlayerFakeObj;
    private int m_PlayerFakeObjID = GlobeVar.INVALID_ID;
    private GameObject m_FakeObjGameObject = null;

    public List<GameObject> m_TabButtonList = new List<GameObject>();

    public GameObject m_TabFashion;
    public UIGrid m_TabGrid;

    public delegate void AfterStart();
    public AfterStart m_delAfterStart = null;

    // 新手指引
    private int m_NewPlayerGuide_Step = -1;
    public GameObject m_BtnClose;
    public GameObject m_EquipSlot_1;

    private UInt64 m_TakeOffGuid = GlobeVar.INVALID_GUID;
    public UInt64 TakeOffGuid
    {
        get { return m_TakeOffGuid; }
        set { m_TakeOffGuid = value; }
    }

    public ModelDragLogic m_ModelDrag;

    private static bool m_bHasStartTab = false;     // 是否需要在初始化时显示一个指定Tab
    private static int m_nStartTab = -1;   // 初始化指定Tab 

    public static void SetStartTab(int tab)
    {
        m_bHasStartTab = true;
        m_nStartTab = tab;
    }

    void TestStartTab()
    {
        if (!m_bHasStartTab)
        {
            return;
        }
        m_bHasStartTab = false;
        if(m_nStartTab == 4)
        {
             GemBtClick();
        }
    }

    void Start()
    {
        // 新手指引
        if (m_delAfterStart != null)
        {
            m_delAfterStart();
        }
    }

    void OnEnable()
    {
        m_Instance = this;
        GameManager.gameManager.ActiveScene.InitFakeObjRoot(m_FakeObjTopLeft, m_FakeObjBottomRight);
        GameManager.gameManager.ActiveScene.ShowFakeObj();
        m_bIsNeedUpdateAttr = true;
        m_LeftAttrView_01.SetActive(true);
        m_LeftAttrView_02.SetActive(false);
        m_LeftAttrView_03.SetActive(false);
        m_ShowMoreInfoBt.SetActive(true);
        m_CancelShowInfoBt.SetActive(false);
        m_LeftView.SetActive(true);

        bool bShowFashionTab = GameManager.gameManager.PlayerDataPool.IsServerFlagOpen(SERVER_FLAGS_ENUM.FLAG_FASHIONTAB);
        m_TabFashion.SetActive(bShowFashionTab);

        UpdateEquipPack();
        AttrBtClick();
        //InitTitleView();
        Check_NewPlayerGuide(); // 新手指引
        if (m_delAfterStart != null)
        {
            m_delAfterStart();
        }
        //更新预览模型
        int profession = Singleton<ObjManager>.GetInstance().MainPlayer.Profession;
        CreatePartnerFakeObj(profession);
        TestStartTab();
        GameManager.gameManager.ActiveScene.ShowFakeObj();
    }

    void OnDisable()
    {
        DestroyPartnerFakeObj();
        GameManager.gameManager.ActiveScene.HideFakeObj();
    }

    void OnDestroy()
    {
        m_Instance = null;
    }
    /// <summary>
    /// 点击属性按钮
    /// </summary>
    public void AttrBtClick()
    {
        ChooseTabButton(CONTENT_TYPE.CONTENT_TYPE_ATTR);
        m_AttrView.SetActive(true);
        m_FashionView.SetActive(false);
        m_TitleView.SetActive(false);

        if (m_LeftAttrView_02.activeInHierarchy)
        {
            m_LeftView.SetActive(false);
            m_CancelShowInfoBt.SetActive(true);
            m_ShowMoreInfoBt.SetActive(false);
            //隐藏预览模型
            GameManager.gameManager.ActiveScene.HideFakeObj();
        }
       
       // 发包请求数据
        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (_mainPlayer != null && m_bIsNeedUpdateAttr) //战斗力不相同时 才去重新发包请求最新数据
        {
            CG_ASK_ROLE_DATA askPak =(CG_ASK_ROLE_DATA)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_ROLE_DATA);
            askPak.SetUserguid(_mainPlayer.GUID);
            askPak.SendPacket();
            m_bIsNeedUpdateAttr = false;
        }

         Obj_MainPlayer obj = Singleton<ObjManager>.GetInstance().MainPlayer;
         int nLevel = 0;
         int nLeft = 0;
         VipData.GetVipLevel(obj.VipCost, ref nLevel, ref nLeft);
         if (nLevel > 0) 
         {
             m_VipSprite.gameObject.SetActive(true);
             m_VipSprite.spriteName = "v" + nLevel.ToString();
         }
         else
         {
             m_VipSprite.gameObject.SetActive(false);
         }

        if (FashionLogic.Instance() != null && FashionLogic.Instance().IsFitOn)
        {
            ResetFitOn();
        }
    }

    public void OnCombatChange()
    {
        m_bIsNeedUpdateAttr = true;
        if (m_AttrView.activeInHierarchy)
        {
            // 战斗力发生变化 重新发包请求最新数据
            Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
            if (_mainPlayer != null)
            {
                CG_ASK_ROLE_DATA askPak = (CG_ASK_ROLE_DATA)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_ROLE_DATA);
                askPak.SetUserguid(_mainPlayer.GUID);
                askPak.SendPacket();
                m_bIsNeedUpdateAttr = false;
            }
        }
    }
    /// <summary>
    /// 更新右边属性页面
    /// </summary>
    public void UpdateAttrRightView()
    {
        m_bIsNeedUpdateAttr = false;
        //战斗力
        m_CombatValueLable.text = m_nCombatValue.ToString();
        //等级
        m_LevelValueLable.text = m_nLevel.ToString();
        //职业名
        if (m_Profession == (int) CharacterDefine.PROFESSION.SHAOLIN)
        {
            m_ProfessionValueLable.text = StrDictionary.GetClientDictionaryString("#{1178}");
        }
        else if (m_Profession == (int) CharacterDefine.PROFESSION.DALI)
        {
            m_ProfessionValueLable.text = StrDictionary.GetClientDictionaryString("#{1179}");
        }
        else if (m_Profession == (int) CharacterDefine.PROFESSION.TIANSHAN)
        {
            m_ProfessionValueLable.text = StrDictionary.GetClientDictionaryString("#{1180}");
        }
        else if (m_Profession == (int) CharacterDefine.PROFESSION.XIAOYAO)
        {
            m_ProfessionValueLable.text = StrDictionary.GetClientDictionaryString("#{1181}");
        }
        //GUID 
        Int32 high = (Int32) (m_RoleGUID >> 32);
        Int32 low = (Int32) (m_RoleGUID & 0xFFFFFFFF);
        m_GUIDValueLable.text = string.Format("{0}-{1}", high.ToString("X8"), low.ToString("X8"));
//		string currPlayerId = PlayerPrefs.GetString ("CurrPlayerID");
//		string currPlayerIndex = PlayerPrefs.GetString ("CurrPlayerIndex");
//		m_GUIDValueLable.text = currPlayerId+"-"+currPlayerIndex;
        //血
        m_HPValueLable.text = String.Format("{0}/{1}",Utils.ConvertLargeNumToString(m_nCurHp),Utils.ConvertLargeNumToString(m_nMaxHP));
        if (m_nMaxHP != 0)
        {
            m_HPSprite.fillAmount = (float) m_nCurHp/(float) m_nMaxHP;
        }
        else
        {
            m_HPSprite.fillAmount = 1;
        }
    //    m_HPSprite.MakePixelPerfect();
        //蓝
        m_MPValueLable.text = String.Format("{0}/{1}",Utils.ConvertLargeNumToString(m_nCurMp),Utils.ConvertLargeNumToString(m_nMaxMp));
        if (m_nMaxMp != 0)
        {
            m_MPSprite.fillAmount = (float) m_nCurMp/(float) m_nMaxMp;
        }
        else
        {
            m_MPSprite.fillAmount = 1;
        }
      //  m_MPSprite.MakePixelPerfect();
        //经验
        m_ExpValueLable.text = String.Format("{0}/{1}", Utils.ConvertLargeNumToString(m_nCurExp),Utils.ConvertLargeNumToString(m_nMaxExp));
        if (m_nMaxExp != 0)
        {
            m_ExpSprite.fillAmount = (float) m_nCurExp/(float) m_nMaxExp;
        }
        else
        {
            m_ExpSprite.fillAmount = 1;
        }
     //   m_ExpSprite.MakePixelPerfect();
        UpdateOffLineExp();
        //攻击力
        if (m_Profession == (int) CharacterDefine.PROFESSION.DALI ||
            m_Profession == (int) CharacterDefine.PROFESSION.TIANSHAN ||
            m_Profession == (int) CharacterDefine.PROFESSION.SHAOLIN)
        {
            m_PAttackLable.gameObject.SetActive(true);
            m_MAttckLabel.gameObject.SetActive(false);
            m_AttackValueLable.text = m_nPAttck.ToString();
        }
        else if (m_Profession == (int) CharacterDefine.PROFESSION.XIAOYAO)
        {
            m_PAttackLable.gameObject.SetActive(false);
            m_MAttckLabel.gameObject.SetActive(true);
            m_AttackValueLable.text = m_nMAttack.ToString();
        }
        m_PDefenseValueLable.text = m_nPDefense.ToString();//物防
        m_MDefenseValueLable.text = m_MDefense.ToString();//法防

        m_HitValueLabel.text = m_nHit.ToString();//命中
        m_CriticalValueLable.text = m_nCritical.ToString();//暴击
        m_DecriticalValueLabel.text = m_DeCritical.ToString();//暴抗
        m_DodgeValueLable.text = m_nDoge.ToString();//闪避
        m_StrikeValueLable.text = m_nStrike.ToString();//穿透
        m_CriticalAddValueLable.text = m_nCriticalAdd.ToString();//暴击伤害加成
        m_DucticalValueLable.text = m_nDuctical.ToString();//韧性
        m_CriticalMisValueLable.text = m_nCriticalMis.ToString();//暴击伤害减免
    }

    void UpdateOffLineExp()
    {
        //离线经验
        int nRemainLevelUpExp = m_nMaxExp - m_nCurExp;
        if (nRemainLevelUpExp <= 0)
        {
            m_OffLineExpSprite.fillAmount = (float)1.0;
            return;
        }
        if (m_nOffLineMaxExp <= 0)
        {
            m_OffLineExpSprite.fillAmount = (float)0.0;
            LogModule.ErrorLog("m_MaxOffLineExp is 0");
            return;
        }
        if (m_nOffLineExp <= 0)
        {
            m_OffLineExpSprite.fillAmount = (float)0.0;
            return;
        }

        double fTempTotal = (double)(m_nMaxExp) / (double)(nRemainLevelUpExp) * (double)m_nOffLineMaxExp;
        double fTemp = (double)(m_nCurExp) / (double)(nRemainLevelUpExp) * (double)m_nOffLineMaxExp;
        if (fTempTotal <= 0)
        {
            m_OffLineExpSprite.fillAmount = (float)0.0;
            return;
        }
        double dGrogressValue = (double)(m_nOffLineExp + fTemp) / (double)(fTempTotal);
        
        double dExpProgressValue = m_ExpSprite.fillAmount;
        double dTemp = dGrogressValue - dExpProgressValue;
        if (dTemp < 0.01)
        {
            dGrogressValue = dGrogressValue + 0.01;
        }

        if (dGrogressValue <= 0.03)
        {
            dGrogressValue = 0.04;
        }
        m_OffLineExpSprite.fillAmount = (float)dGrogressValue;

        m_OffLineExpSprite.MakePixelPerfect();
    }

    void CancelShowInfoBtClick()
    {
        m_LeftAttrView_02.SetActive(false);
        m_CancelShowInfoBt.SetActive(false);
        m_ShowMoreInfoBt.SetActive(true);
        m_LeftView.SetActive(true);
        //打开预览模型
        GameManager.gameManager.ActiveScene.ShowFakeObj();
        //播放站立动作
        if (null != m_PlayerFakeObj)
            m_PlayerFakeObj.PlayAnim(0);
    }

    public void UpdateCurAttr()
    {
        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (_mainPlayer != null)
        {
            //血
            m_HPValueLable.text = String.Format("{0}/{1}", Utils.ConvertLargeNumToString(_mainPlayer.BaseAttr.HP), Utils.ConvertLargeNumToString(_mainPlayer.BaseAttr.MaxHP));
            if (m_nMaxHP != 0)
            {
                m_HPSprite.fillAmount = (float)_mainPlayer.BaseAttr.HP / (float)_mainPlayer.BaseAttr.MaxHP;
            }
            else
            {
                m_HPSprite.fillAmount = 1;
            }
            m_HPSprite.MakePixelPerfect();
            //蓝
            m_MPValueLable.text = String.Format("{0}/{1}", Utils.ConvertLargeNumToString(_mainPlayer.BaseAttr.MP), Utils.ConvertLargeNumToString(_mainPlayer.BaseAttr.MaxMP));
            if (m_nMaxMp != 0)
            {
                m_MPSprite.fillAmount = (float)_mainPlayer.BaseAttr.MP / (float)_mainPlayer.BaseAttr.MaxMP;
            }
            else
            {
                m_MPSprite.fillAmount = 1;
            }
            m_MPSprite.MakePixelPerfect();
            //经验
            m_ExpValueLable.text = String.Format("{0}/{1}", Utils.ConvertLargeNumToString(_mainPlayer.BaseAttr.Exp), Utils.ConvertLargeNumToString(m_nMaxExp));
            if (m_nMaxExp != 0)
            {
                m_ExpSprite.fillAmount = (float)_mainPlayer.BaseAttr.Exp / (float)m_nMaxExp;
            }
            else
            {
                m_ExpSprite.fillAmount = 1;
            }
            UpdateOffLineExp();
        }
    }
    void MoreInfoBtClick()
    {
      m_LeftAttrView_02.SetActive(true);
      m_ShowMoreInfoBt.SetActive(false);
      m_CancelShowInfoBt.SetActive(true);
      m_LeftView.SetActive(false);
      //隐藏预览模型
      GameManager.gameManager.ActiveScene.HideFakeObj();
    }
    
    /// <summary>
    /// 更新装备槽部分显示
    /// </summary>
    public void UpdateEquipPack()
    {
        m_NameLabel.text =GameManager.gameManager.PlayerDataPool.MainPlayerBaseAttr.RoleName;
        GameItemContainer EquipPack = GameManager.gameManager.PlayerDataPool.EquipPack;
        for (int index = 0; index < EquipPack.ContainerSize; ++index)
        {
            GameItem equip = EquipPack.GetItem(GetEquipSlotByIndex(index));
            if (equip != null && equip.IsValid())
            {
                UISprite IconSprite = m_EquipSlotIcon[index];
                if (IconSprite != null)
                {
                    string icon = TableManager.GetCommonItemByID(equip.DataID, 0).Icon;
                    IconSprite.spriteName = icon;
                    //IconSprite.MakePixelPerfect();

                    UISprite QualitySprite = m_EquipSlotQualityFrame[index];
                    if (QualitySprite != null)
                    {
                        QualitySprite.gameObject.SetActive(true);
                        QualitySprite.spriteName = equip.GetQualityFrame();
                    }
                }
            }
            else
            {
                UISprite IconSprite = m_EquipSlotIcon[index];
                if (IconSprite != null)
                {
                    IconSprite.spriteName = "Dark";
                    //IconSprite.MakePixelPerfect();

                    UISprite QualitySprite = m_EquipSlotQualityFrame[index];
                    if (QualitySprite != null)
                    {
                        QualitySprite.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    int GetEquipSlotByIndex(int index)
    {
        switch (index)
        {
            case 0: return (int)EquipPackSlot.Slot_WEAPON;
            case 1: return (int)EquipPackSlot.Slot_HEAD;
            case 2: return (int)EquipPackSlot.Slot_RING;
            case 3: return (int)EquipPackSlot.Slot_ARMOR;
            case 4: return (int)EquipPackSlot.Slot_NECK;
            case 5: return (int)EquipPackSlot.Slot_CUFF;
            case 6: return (int)EquipPackSlot.Slot_AMULET;
            case 7: return (int)EquipPackSlot.Slot_LEG_GUARD;
            case 8: return (int)EquipPackSlot.Slot_BELT;
            case 9: return (int)EquipPackSlot.Slot_SHOES;
            default:
                break;
        }
        return -1;
    }

    int GetIndexByEquipSlot(int slot)
    {
        switch (slot)
        {
            case (int)EquipPackSlot.Slot_WEAPON: return 0;
            case (int)EquipPackSlot.Slot_HEAD: return 1;
            case (int)EquipPackSlot.Slot_RING: return 2;
            case (int)EquipPackSlot.Slot_ARMOR: return 3;
            case (int)EquipPackSlot.Slot_NECK: return 4;
            case (int)EquipPackSlot.Slot_CUFF: return 5;
            case (int)EquipPackSlot.Slot_AMULET: return 6;
            case (int)EquipPackSlot.Slot_LEG_GUARD: return 7;
            case (int)EquipPackSlot.Slot_BELT: return 8;
            case (int)EquipPackSlot.Slot_SHOES: return 9;
            default:
                break;
        }
        return 0;
    }

    /// <summary>
    /// 销毁FakeObj
    /// </summary>
    private void DestroyPartnerFakeObj()
    {
        if (m_PlayerFakeObj != null)
        {
            m_PlayerFakeObj.Destroy();
            m_PlayerFakeObj = null;
        }
    }

    /// <summary>
    /// 创建FakeObj
    /// </summary>
    /// <param name="pro"></param>
    private void CreatePartnerFakeObj(int pro)
    {
        if (m_PlayerFakeObj != null)
        {
            DestroyPartnerFakeObj();
        }
        CharacterDefine.PROFESSION profession = (CharacterDefine.PROFESSION)pro;
        //对应FakeObject.txt配置
        int fakeObjId = -1;
        switch (profession)
        {
            case CharacterDefine.PROFESSION.SHAOLIN:
                fakeObjId = 7;
                break;
            case CharacterDefine.PROFESSION.TIANSHAN:
                fakeObjId = 8;
                break;
            case CharacterDefine.PROFESSION.DALI:
                fakeObjId = 10;
                break;
            case CharacterDefine.PROFESSION.XIAOYAO:
                fakeObjId = 9;
                break;
            default:
                fakeObjId = 7;
                break;
        }

        m_PlayerFakeObj = new FakeObject();
        if (m_PlayerFakeObj == null)
        {
            return;
        }

        m_PlayerFakeObjID = fakeObjId;
        m_PlayerFakeObj.initFakeObject(fakeObjId, GameManager.gameManager.ActiveScene.FakeObjTrans, out m_FakeObjGameObject);
        if (null != m_PlayerFakeObj.ObjAnim)
            m_ModelDrag.ModelTrans = m_PlayerFakeObj.ObjAnim.transform;
    }

    public void AfterLoadTitleItem()
    {
       
    }

    /// <summary>
    /// 点击装备槽位
    /// </summary>
    public void OnClick_Equip_WEAPON()
    {
       
           GameItem item = GameManager.gameManager.PlayerDataPool.EquipPack.GetItem(0);
           if (item != null && item.IsValid())
            {
                EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Equiped);
            }
        
    }

    /// <summary>
    /// 点击装备槽位
    /// </summary>
    public void OnClick_Equip_HEAD()
    {
        if (m_NewPlayerGuide_Step == 1)
        {
            NewPlayerGuide(2);
        }
        
            GameItem item = GameManager.gameManager.PlayerDataPool.EquipPack.GetItem(1);
            if (item != null && item.IsValid())
            {
                EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Equiped);
            }
        
    }

    /// <summary>
    /// 点击装备槽位
    /// </summary>
    public void OnClick_Equip_ARMOR()
    {
        
      GameItem item = GameManager.gameManager.PlayerDataPool.EquipPack.GetItem(2);
      if (item != null && item.IsValid())
      {
           EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Equiped);
       }  
    }

    /// <summary>
    /// 点击装备槽位
    /// </summary>
    public void OnClick_Equip_LEG_GUARD()
    {
        
            GameItem item = GameManager.gameManager.PlayerDataPool.EquipPack.GetItem(3);
            if (item != null && item.IsValid())
            {
                EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Equiped);
            }
       
    }

    /// <summary>
    /// 点击装备槽位
    /// </summary>
    public void OnClick_Equip_CUFF()
    {
       
       GameItem item = GameManager.gameManager.PlayerDataPool.EquipPack.GetItem(4);
       if (item != null && item.IsValid())
        {
                EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Equiped);
       }
      
    }

    /// <summary>
    /// 点击装备槽位
    /// </summary>
    public void OnClick_Equip_SHOES()
    {
       
      GameItem item = GameManager.gameManager.PlayerDataPool.EquipPack.GetItem(5);
      if (item != null && item.IsValid())
      {
                EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Equiped);
       }
    
    }

    /// <summary>
    /// 点击装备槽位
    /// </summary>
    public void OnClick_Equip_NECK()
    {
      GameItem item = GameManager.gameManager.PlayerDataPool.EquipPack.GetItem(6);
      if (item != null && item.IsValid())
      {
                EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Equiped);
      }      
    }

    /// <summary>
    /// 点击装备槽位
    /// </summary>
    public void OnClick_Equip_RING()
    {
            GameItem item = GameManager.gameManager.PlayerDataPool.EquipPack.GetItem(7);
            if (item != null && item.IsValid())
            {
                EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Equiped);
            }
    }

    /// <summary>
    /// 点击装备槽位
    /// </summary>
    public void OnClick_Equip_AMULET()
    {
            GameItem item = GameManager.gameManager.PlayerDataPool.EquipPack.GetItem(8);
            if (item != null && item.IsValid())
            {
                EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Equiped);
            }
    }

    /// <summary>
    /// 点击装备槽位
    /// </summary>
    public void OnClick_Equip_BELT()
    {

        GameItem item = GameManager.gameManager.PlayerDataPool.EquipPack.GetItem(9);
        if (item != null && item.IsValid())
        {
           EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Equiped);
        }
        
    }

    public void OnClick_Equip(int slot)
    {
        switch (slot)
        {
        case (int)EquipPackSlot.Slot_WEAPON:
            OnClick_Equip_WEAPON();
            break;
        case (int)EquipPackSlot.Slot_HEAD:
            OnClick_Equip_HEAD();
            break;
        case (int)EquipPackSlot.Slot_RING:
            OnClick_Equip_RING();
            break;
        case (int)EquipPackSlot.Slot_ARMOR:
            OnClick_Equip_ARMOR();
            break;
        case (int)EquipPackSlot.Slot_NECK:
            OnClick_Equip_NECK();
            break;
        case (int)EquipPackSlot.Slot_CUFF:
            OnClick_Equip_CUFF();
            break;
        case (int)EquipPackSlot.Slot_AMULET:
            OnClick_Equip_AMULET();
            break;
        case (int)EquipPackSlot.Slot_LEG_GUARD:
            OnClick_Equip_LEG_GUARD();
            break;
        case (int)EquipPackSlot.Slot_BELT:
            OnClick_Equip_BELT();
            break;
        case (int)EquipPackSlot.Slot_SHOES:
            OnClick_Equip_SHOES();
            break;
        default:
            break;
        }
    }

    void ClearAllSlotChoose()
    {
        for (int i=0; i < (int)EquipPackSlot.Slot_NUM; i++)
        {
            m_EquipSlotChooseSprite[i].gameObject.SetActive(false);
        }
    }

    public void TitleBtClick()
    {
        ChooseTabButton(CONTENT_TYPE.CONTENT_TYPE_TITLE);
        m_AttrView.SetActive(false);
        m_FashionView.SetActive(false);
        m_TitleView.SetActive(true);
		m_LeftView.SetActive(true);
        if (FashionLogic.Instance() != null && FashionLogic.Instance().IsFitOn)
        {
            ResetFitOn();
        }
    }

    public void FashionBtClick()
    {
        ChooseTabButton(CONTENT_TYPE.CONTENT_TYPE_FASHION);
        m_AttrView.SetActive(false);
        m_FashionView.SetActive(true);
        m_TitleView.SetActive(false);
//         if (FashionLogic.Instance() != null)
//         {
//             FashionLogic.Instance().ClearFashionItemChoose();
//         }
    }

    public void GemBtClick()
    {
        ChooseTabButton(CONTENT_TYPE.CONTENT_TYPE_GEM);
        m_AttrView.SetActive(false);
        m_FashionView.SetActive(false);
        m_TitleView.SetActive(false);
        m_LeftView.SetActive(true);
        if (m_NewPlayerGuide_Step == 0)
        {
            NewPlayerGuide(1);
        }
        if (FashionLogic.Instance() != null && FashionLogic.Instance().IsFitOn)
        {
            ResetFitOn();
        }
    }

    public void CloseWindow()
    {
        if (m_NewPlayerGuide_Step == 3)
        {
            NewPlayerGuide(4);
        }

        UIManager.CloseUI(UIInfo.RoleView);

        if (null != m_TitleView.GetComponent<TitleInvestitiveLogic>())
            m_TitleView.GetComponent<TitleInvestitiveLogic>().OnCloseBackPack();
    }

    void ChooseTabButton(CONTENT_TYPE eContent)
    {
        if (m_NewPlayerGuide_Step == 0)
        {
            NewPlayerGuide(1);
        }

        m_TabButtonList[(int)eContent].transform.Find("Hight").gameObject.SetActive(true);
        m_TabButtonList[(int)eContent].transform.Find("Background").gameObject.SetActive(false);
       // m_TabButtonList[(int)eContent].target.MakePixelPerfect();

        for (int i = 0; i < m_TabButtonList.Count; i++)
        {
            if (i != (int)eContent && m_TabButtonList[i]!=null)
            {
                m_TabButtonList[i].transform.Find("Hight").gameObject.SetActive(false);
                m_TabButtonList[i].transform.Find("Background").gameObject.SetActive(true);
               // m_TabButtonList[i].target.MakePixelPerfect();
            }
           
        }

        ClearAllSlotChoose();
        //查看切换到其他非属性界面 如果模型被隐藏了则把预览模型打开
        if (eContent != CONTENT_TYPE.CONTENT_TYPE_ATTR && GameManager.gameManager.ActiveScene.FakeObjRoot.activeInHierarchy == false)
        {
            //打开预览模型
            GameManager.gameManager.ActiveScene.ShowFakeObj();
            //播放站立动作
            if (m_PlayerFakeObj !=null)
            {
                m_PlayerFakeObj.PlayAnim(0);
            }
        }
       
    }

//     void InitTitleView()
//     {
//         m_TitleView.GetComponent<TitleInvestitiveLogic>().Init();
//     }

    public void UpdateFashionView()
    {
        int nModelVisualID = Singleton<ObjManager>.Instance.MainPlayer.ModelVisualID;
        bool isDefaultVisual = false;
        Tab_ItemVisual tabItemVisual = null;

        tabItemVisual = TableManager.GetItemVisualByID(nModelVisualID, 0);
        if (tabItemVisual == null)
        {
            isDefaultVisual = true;
        }
        else
        {
            isDefaultVisual = false;
        }
        
        if (isDefaultVisual)
        {
            tabItemVisual = TableManager.GetItemVisualByID(GlobeVar.DEFAULT_VISUAL_ID, 0);
            if (tabItemVisual == null)
            {
                return;
            }
        }

        int nCharModelID = Singleton<ObjManager>.Instance.MainPlayer.GetCharModelID(tabItemVisual);
        Tab_CharModel tabCharModel = TableManager.GetCharModelByID(nCharModelID, 0);
        if (tabCharModel == null)
        {
            return;
        }

        Tab_FakeObject tabFakeObject = TableManager.GetFakeObjectByID(m_PlayerFakeObjID, 0);
        if (tabFakeObject == null)
        {
            return;
        }

        Singleton<ObjManager>.GetInstance().ReloadModel(m_FakeObjGameObject,
            tabCharModel.ResPath,
            Singleton<ObjManager>.GetInstance().AsycLoadFakeObjOver,
            tabFakeObject,
            m_PlayerFakeObj);
    }

    public void FitOnFashion(int nFashionID)
    {
        Tab_FashionData tabFashionData = TableManager.GetFashionDataByID(nFashionID, 0);
        if (tabFashionData == null)
        {
            return;
        }

        Tab_ItemVisual tabItemVisual = TableManager.GetItemVisualByID(tabFashionData.ItemVisualID, 0);
        if (tabItemVisual == null)
        {
            return;
        }

        int nCharModelID = Singleton<ObjManager>.Instance.MainPlayer.GetCharModelID(tabItemVisual);
        Tab_CharModel tabCharModel = TableManager.GetCharModelByID(nCharModelID, 0);
        if (tabCharModel == null)
        {
            return;
        }

        Tab_FakeObject tabFakeObject = TableManager.GetFakeObjectByID(m_PlayerFakeObjID, 0);
        if (tabFakeObject == null)
        {
            return;
        }

        List<object> param = new List<object>();
        param.Add(tabFakeObject);
        param.Add(m_PlayerFakeObj);

        Singleton<ObjManager>.GetInstance().ReloadModel(m_FakeObjGameObject,
            tabCharModel.ResPath,
            Singleton<ObjManager>.GetInstance().AsycLoadRoleViewFitOnObjOver,
            param);
    }

    public void ResetFitOn()
    {
        if (null != m_PlayerFakeObj && null != m_PlayerFakeObj.ObjAnim)
        {
            m_PlayerFakeObj.initFakeObject(m_PlayerFakeObjID, GameManager.gameManager.ActiveScene.FakeObjTrans, out m_FakeObjGameObject);
            m_ModelDrag.ModelTrans = m_PlayerFakeObj.ObjAnim.transform;
            if (null != FashionLogic.Instance())
                FashionLogic.Instance().IsFitOn = false;
        }
    }

    void Check_NewPlayerGuide()
    {
        if (MenuBarLogic.Instance() == null)
        {
            return;
        }
        int nIndex = MenuBarLogic.Instance().NewPlayerGuideIndex;
        if (nIndex == 9)
        {
            NewPlayerGuide(0);
            MenuBarLogic.Instance().NewPlayerGuideIndex = -1;
        }
    }

    public void NewPlayerGuide(int nIndex)
	{
		if (nIndex < 0) 
		{
			return;
		}
		NewPlayerGuidLogic.CloseWindow();
        m_NewPlayerGuide_Step = nIndex;
        switch(nIndex)
        {
            case 0:
                if (m_TabButtonList.Count > (int)CONTENT_TYPE.CONTENT_TYPE_GEM 
                    && (int)CONTENT_TYPE.CONTENT_TYPE_GEM >= 0
                    && m_TabButtonList[(int)CONTENT_TYPE.CONTENT_TYPE_GEM])
                {
                    NewPlayerGuidLogic.OpenWindow(m_TabButtonList[(int)CONTENT_TYPE.CONTENT_TYPE_GEM].gameObject, 154, 64, "", "bottom", 2, true, true);
                }
                break;
            case 1:
                {
                    NewPlayerGuidLogic.OpenWindow(m_EquipSlotIcon[1].gameObject, 112, 112, "", "bottom", 2, true, true);
                }
                break;
            case 2:
                
                m_NewPlayerGuide_Step = -1;
                break;
            case 3:
                NewPlayerGuidLogic.OpenWindow(m_BtnClose, 78, 78, "", "bottom", 2, true, true);
                break;
            case 4:
                m_NewPlayerGuide_Step = -1;
                break;
        }
    }

    public void CombatNumClick()
    {
        UIManager.ShowUI(UIInfo.CheckPowerRoot);
        CG_REQ_POWERUP packet = (CG_REQ_POWERUP)PacketDistributed.CreatePacket(MessageID.PACKET_CG_REQ_POWERUP);
        packet.Type = (int)BePowerData.BePowerType.BPTDEFINE_EQUIP;
        packet.Flag = 1;
        packet.SendPacket();
    }
}
