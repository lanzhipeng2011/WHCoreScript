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
using Module.Log;
using GCGame;

public class OtherRoleViewLogic : MonoBehaviour
{

    // Use this for initialization
    private static OtherRoleViewLogic m_Instance = null;
    public static OtherRoleViewLogic Instance()
    {
        return m_Instance;
    }

    public enum CONTENT_TYPE
    {
        CONTENT_TYPE_INVALID = -1,
        CONTENT_TYPE_ATTR = 0,        // 属性
        CONTENT_TYPE_GEM,             // 宝石
    }

    public enum OPEN_TYPE
    {
        OPEN_TYPE_INVALID = -1,
        OPEN_TYPE_FRIEND = 0,        // 通过好友界面打开的
        OPEN_TYPE_POPMENU = 1,
        OPEN_TYPE_LASTSPEAKER = 2,
		OPEN_TYPE_TEAM = 3,
        OPEN_TYPE_NUM,             // 
    }

    public GameObject m_AttrView;//属性视图
    public GameObject m_GemView;//宝石视图

    public UISprite[] m_EquipSlotChooseSprite;
    public UISprite[] m_EquipSlotIcon;
    public UISprite[] m_EquipSlotQualityFrame;

    private string m_strName = "";
    public string Name
    {
        get { return m_strName; }
        set { m_strName = value; }
    }
    private int m_nCombatValue = 0;
    public int CombatValue
    {
        get { return m_nCombatValue; }
        set { m_nCombatValue = value; }
    }
    private int m_nLevel = 0;
    public int Level
    {
        get { return m_nLevel; }
        set { m_nLevel = value; }
    }
    private int m_Profession = -1;
    public int Profession
    {
        get { return m_Profession; }
        set { m_Profession = value; }
    }


    private UInt64 m_RoleGUID = 0;
    public System.UInt64 RoleGUID
    {
        get { return m_RoleGUID; }
        set { m_RoleGUID = value; }
    }
    private int m_nCurHp = 0;
    public int CurHp
    {
        get { return m_nCurHp; }
        set { m_nCurHp = value; }
    }
    private int m_nMaxHP = 0;
    public int MaxHP
    {
        get { return m_nMaxHP; }
        set { m_nMaxHP = value; }
    }
    private int m_nCurMp = 0;
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

    private int m_nPAttck = 0;//物攻
    public int PAttck
    {
        get { return m_nPAttck; }
        set { m_nPAttck = value; }
    }
    private int m_nMAttack = 0;//法功
    public int MAttack
    {
        get { return m_nMAttack; }
        set { m_nMAttack = value; }
    }
    private int m_nHit = 0;
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
    private int m_MDefense = 0;//法防
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
    public UISlider m_HPSprite;
    public UISlider m_MPSprite;
    public UISlider m_ExpSprite;

    public UILabel m_PAttackLable;
    public UILabel m_MAttckLabel;
    public UILabel m_AttackValueLable;
    public UILabel m_HitValueLabel;
    public UILabel m_CriticalValueLable;
    public UILabel m_PDefenseValueLable;
    public UILabel m_MDefenseValueLable;
    public UILabel m_DecriticalValueLabel;
    public UILabel m_DodgeValueLable;

	public UISprite m_VipSprite;

    public UILabel m_StrikeValueLable;
    public UILabel m_CriticalAddValueLable;
    public UILabel m_DucticalValueLable;
    public UILabel m_CriticalMisValueLable;

    public UILabel      m_NameLabel;
    public GameObject   m_EquipPackGrid;

    public GameObject m_FakeObjTopLeft;
    public GameObject m_FakeObjBottomRight;
    private OtherFakeObject m_PlayerFakeObj;
    //private int m_PlayerFakeObjID = GlobeVar.INVALID_ID;
    private GameObject m_FakeObjGameObject = null;

    private static OPEN_TYPE m_oPenType = OPEN_TYPE.OPEN_TYPE_INVALID;

    public List<UIImageButton> m_TabButtonList = new List<UIImageButton>();
    public ModelDragLogic m_ModelDrag;

    void Awake()
    {
        m_Instance = this;
    }

    void OnEnable()
    {
        GameManager.gameManager.ActiveScene.ShowFakeObj();

		m_Instance = this;
    }

    void OnDisable()
    {
		//====On Disable clean FakeObj   
		DestroyPartnerFakeObj();
		m_Instance = null;
		//====
        GameManager.gameManager.ActiveScene.HideFakeObj();
    }

    void OnDestroy()
    {
        DestroyPartnerFakeObj();
        m_Instance = null;
        GameManager.gameManager.ActiveScene.HideFakeObj();
    }

    public static void OnShowOtherRoleVirew(bool bSuccess, object param)
    {
        if (!bSuccess)
        {
            LogModule.ErrorLog("load OtherRoleViewLogic error");
            return;
        }

        if (null != OtherRoleViewLogic.Instance())
            OtherRoleViewLogic.Instance().ShowOtherRoleView();
    }

    public void ShowOtherRoleView()
     {
         GameManager.gameManager.ActiveScene.InitFakeObjRoot(m_FakeObjTopLeft, m_FakeObjBottomRight);
         GameManager.gameManager.ActiveScene.ShowFakeObj();
         Name = GameManager.gameManager.OtherPlayerData.Name;
         CombatValue = GameManager.gameManager.OtherPlayerData.CombatValue;
         Level = GameManager.gameManager.OtherPlayerData.Level;
         Profession = GameManager.gameManager.OtherPlayerData.Profession;
         RoleGUID = GameManager.gameManager.OtherPlayerData.RoleGUID;
         CurHp = GameManager.gameManager.OtherPlayerData.CurHp;
         MaxHP = GameManager.gameManager.OtherPlayerData.MaxHP;
         CurMp = GameManager.gameManager.OtherPlayerData.CurMp;
         MaxMp = GameManager.gameManager.OtherPlayerData.MaxMp;
         CurExp = GameManager.gameManager.OtherPlayerData.CurExp;
         MaxExp = GameManager.gameManager.OtherPlayerData.MaxExp;
         PAttck = GameManager.gameManager.OtherPlayerData.PAttck;
         MAttack = GameManager.gameManager.OtherPlayerData.MAttack;
         Hit = GameManager.gameManager.OtherPlayerData.Hit;
         Critical = GameManager.gameManager.OtherPlayerData.Critical;
         PDefense = GameManager.gameManager.OtherPlayerData.PDefense;
         MDefense = GameManager.gameManager.OtherPlayerData.MDefense;
         DeCritical = GameManager.gameManager.OtherPlayerData.DeCritical;
         Doge = GameManager.gameManager.OtherPlayerData.Doge;
         Strike = GameManager.gameManager.OtherPlayerData.Strike;
         CriticalAdd = GameManager.gameManager.OtherPlayerData.CriticalAdd;
         Ductical = GameManager.gameManager.OtherPlayerData.Ductical;
         CriticalMis = GameManager.gameManager.OtherPlayerData.CriticalMis;
         if (m_EquipPackGrid != null)
         {
             m_EquipPackGrid.GetComponent<UIGrid>().sorted = true;
             m_EquipPackGrid.GetComponent<UIGrid>().Reposition();
         }
         m_LeftAttrView_01.SetActive(true);
         m_LeftAttrView_02.SetActive(false);
         m_LeftAttrView_03.SetActive(false);
         m_ShowMoreInfoBt.SetActive(true);
         m_CancelShowInfoBt.SetActive(false);

         AttrBtClick();
     }

    /// <summary>
    /// 
    /// </summary>
    public static void SetOpenType(OPEN_TYPE opentype)
    {
        m_oPenType = opentype;
    }

    /// <summary>
    /// 点击属性按钮
    /// </summary>
    public void AttrBtClick()
    {
        ChooseTabButton(CONTENT_TYPE.CONTENT_TYPE_ATTR);
		/*
        if (m_AttrView != null)
        {
            m_AttrView.SetActive(true);
        }
        if (m_GemView != null)
        {
            m_GemView.SetActive(false);
        }       
        UpdateAttrView();
        */
		m_AttrView.SetActive(true);
		//m_FashionView.SetActive(false);
		//m_TitleView.SetActive(false);
		m_GemView.SetActive(false);
		
		if (m_LeftAttrView_02.activeInHierarchy) {
			m_LeftView.SetActive (false);
			m_CancelShowInfoBt.SetActive (true);
			m_ShowMoreInfoBt.SetActive (false);
			//隐藏预览模型
			GameManager.gameManager.ActiveScene.HideFakeObj ();
		} else 
		{
			GameManager.gameManager.ActiveScene.ShowFakeObj();
			UpdateAttrView();
		}

		//=====VIP
		//Obj_MainPlayer obj = Singleton<ObjManager>.GetInstance().MainPlayer;
		Obj_OtherPlayer objOther = Singleton<ObjManager>.GetInstance ().FindObjInScene ((int)m_RoleGUID) as Obj_OtherPlayer;
		int nLevel = 0;
		int nLeft = 0;
		VipData.GetVipLevel(objOther.VipCost, ref nLevel, ref nLeft);
		if (nLevel > 0) 
		{
			m_VipSprite.gameObject.SetActive(true);
			m_VipSprite.spriteName = "v" + nLevel.ToString();
		}
		else
		{
			m_VipSprite.gameObject.SetActive(false);
		}


    }

    /// <summary>
    /// 更新属性页面
    /// </summary>
    public void UpdateAttrView()
    {
        UpdateEquipPack();
        UpdateAttrRightView();
        //更新预览模型
        CreatePartnerFakeObj();
    }
    /// <summary>
    /// 更新右边属性页面
    /// </summary>
    void UpdateAttrRightView()
    {
        if (m_CombatValueLable != null)
        {
            m_CombatValueLable.text = m_nCombatValue.ToString();
        }
        if (null == m_LeftAttrView_01)
        {
            LogModule.ErrorLog("UpdateAttrRightView:m_LeftAttrView_01 is null");
            return;
        }
        if (null == m_LeftAttrView_02)
        {
            LogModule.ErrorLog("UpdateAttrRightView:m_LeftAttrView_02 is null");
            return;
        }
        if (null == m_LeftAttrView_03)
        {
            LogModule.ErrorLog("UpdateAttrRightView:m_LeftAttrView_03 is null");
            return;
        }
        if (null == m_LevelValueLable)
        {
            LogModule.ErrorLog("m_LevelValueLable is null");
            return;
        }
        if (null == m_ProfessionValueLable)
        {
            LogModule.ErrorLog("m_ProfessionValueLable is null");
            return;
        }
        if (null == m_GUIDValueLable)
        {
            LogModule.ErrorLog("m_GUIDValueLable is null");
            return;
        }

        if (null == m_HPValueLable)
        {
            LogModule.ErrorLog("m_HPValueLable is null");
            return;
        }

        if (null == m_HPSprite)
        {
            LogModule.ErrorLog("m_HPSprite is null");
            return;
        }

        if (null == m_MPValueLable)
        {
            LogModule.ErrorLog("m_MPValueLable is null");
            return;
        }

        if (null == m_MPSprite)
        {
            LogModule.ErrorLog("m_MPSprite is null");
            return;
        }

        if (null == m_ExpValueLable)
        {
            LogModule.ErrorLog("m_ExpValueLable is null");
            return;
        }

        if (null == m_ExpSprite)
        {
            LogModule.ErrorLog("m_ExpSprite is null");
            return;
        }
        m_LevelValueLable.text = m_nLevel.ToString();
        //职业名
        if (m_Profession == (int)CharacterDefine.PROFESSION.SHAOLIN)
        {
            m_ProfessionValueLable.text = StrDictionary.GetClientDictionaryString("#{1178}");
        }
        else if (m_Profession == (int)CharacterDefine.PROFESSION.DALI)
        {
            m_ProfessionValueLable.text = StrDictionary.GetClientDictionaryString("#{1179}");
        }
        else if (m_Profession == (int)CharacterDefine.PROFESSION.TIANSHAN)
        {
            m_ProfessionValueLable.text = StrDictionary.GetClientDictionaryString("#{1180}");
        }
        else if (m_Profession == (int)CharacterDefine.PROFESSION.XIAOYAO)
        {
            m_ProfessionValueLable.text = StrDictionary.GetClientDictionaryString("#{1181}");
        }
       
        //GUID 
        Int32 high = (Int32)(m_RoleGUID >> 32);
        Int32 low = (Int32)(m_RoleGUID & 0xFFFFFFFF);
        m_GUIDValueLable.text = string.Format("{0}-{1}", high.ToString("X8"), low.ToString("X8"));

        //血
        m_HPValueLable.text = String.Format("{0}/{1}", Utils.ConvertLargeNumToString(m_nCurHp), Utils.ConvertLargeNumToString(m_nMaxHP));
        if (m_nMaxHP != 0)
        {
            m_HPSprite.value = (float)m_nCurHp / (float)m_nMaxHP;
        }
        else
        {
            m_HPSprite.value = 1;
        }
       
        //蓝
        m_MPValueLable.text = String.Format("{0}/{1}", Utils.ConvertLargeNumToString(m_nCurMp), Utils.ConvertLargeNumToString(m_nMaxMp));
        if (m_nMaxMp != 0)
        {
            m_MPSprite.value = (float)m_nCurMp / (float)m_nMaxMp;
        }
        else
        {
            m_MPSprite.value = 1;
        }
        


        m_ExpValueLable.text = String.Format("{0}/{1}", Utils.ConvertLargeNumToString(m_nCurExp), Utils.ConvertLargeNumToString(m_nMaxExp));
        if (m_nMaxExp != 0)
        {
            m_ExpSprite.value = (float)m_nCurExp / (float)m_nMaxExp;
        }
        else
        {
            m_ExpSprite.value = 1;
        }

        

        if (m_Profession == (int)CharacterDefine.PROFESSION.DALI ||
            m_Profession == (int)CharacterDefine.PROFESSION.TIANSHAN ||
            m_Profession == (int)CharacterDefine.PROFESSION.SHAOLIN)
        {
            m_PAttackLable.gameObject.SetActive(true);
            m_MAttckLabel.gameObject.SetActive(false);
            m_AttackValueLable.text = m_nPAttck.ToString();
        }
        else if (m_Profession == (int)CharacterDefine.PROFESSION.XIAOYAO)
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

    /// <summary>
    /// 
    /// </summary>

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

    void CancelShowInfoBtClick()
    {
        m_LeftAttrView_02.SetActive(false);
        m_CancelShowInfoBt.SetActive(false);
        m_ShowMoreInfoBt.SetActive(true);
		m_LeftView.SetActive (true);
        //打开预览模型
        GameManager.gameManager.ActiveScene.ShowFakeObj();
    }

    void MoreInfoBtClick()
    {
        m_LeftAttrView_02.SetActive(true);
        m_ShowMoreInfoBt.SetActive(false);
        m_CancelShowInfoBt.SetActive(true);
		m_LeftView.SetActive (false);
        //隐藏预览模型
        GameManager.gameManager.ActiveScene.HideFakeObj();
    }

    /// <summary>
    /// 更新装备槽部分显示
    /// </summary>
    public void UpdateEquipPack()
    {
        if (m_NameLabel != null )
        {
            m_NameLabel.text = m_strName;
        }
        GameItemContainer EquipPack = GameManager.gameManager.OtherPlayerData.EquipPack;
        for (int index = 0; index < EquipPack.ContainerSize; ++index)
        {
            GameObject EquipObject = m_EquipPackGrid.transform.FindChild("Equip" + index).gameObject;
            if (null == EquipObject)
            {
                continue;
            }
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
    private void CreatePartnerFakeObj()
    {
        if (m_PlayerFakeObj != null)
        {
            DestroyPartnerFakeObj();
        }
        CharacterDefine.PROFESSION profession = (CharacterDefine.PROFESSION)m_Profession;
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

        m_PlayerFakeObj = new OtherFakeObject();
        if (m_PlayerFakeObj == null)
        {
            return;
        }
        //m_PlayerFakeObjID = fakeObjId;
        m_PlayerFakeObj.initFakeObject(fakeObjId, GameManager.gameManager.ActiveScene.FakeObjTrans, out m_FakeObjGameObject);
        m_ModelDrag.ModelTrans = m_PlayerFakeObj.ObjAnim.transform;
    }

    /// <summary>
    /// 点击装备槽位
    /// </summary>
    public void OnClick_Equip_WEAPON()
    {
        if (null == m_GemView)
        {
            LogModule.ErrorLog("m_GemView is null");
            return;
        }

        if (m_GemView.activeSelf && OtherGemViewLogic.Instance() != null)
        {
            OtherGemViewLogic.Instance().OnClickEquiSlot((int)EquipPackSlot.Slot_WEAPON);
            ClearAllSlotChoose();
            int index = GetIndexByEquipSlot((int)EquipPackSlot.Slot_WEAPON);
            if (index < m_EquipSlotChooseSprite.Length)
            {
                m_EquipSlotChooseSprite[index].gameObject.SetActive(true);
            }
            
        }
        else
        {
            GameItem item = GameManager.gameManager.OtherPlayerData.EquipPack.GetItem(0);
            if (item != null && item.IsValid())
            {
                EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Info);
            }
        }
    }

    /// <summary>
    /// 点击装备槽位
    /// </summary>
    public void OnClick_Equip_HEAD()
    {
        if (null == m_GemView)
        {
            LogModule.ErrorLog("m_GemView is null");
            return;
        }
        if (m_GemView.activeSelf && OtherGemViewLogic.Instance() != null)
        {
            OtherGemViewLogic.Instance().OnClickEquiSlot((int)EquipPackSlot.Slot_HEAD);
            ClearAllSlotChoose();
            int index = GetIndexByEquipSlot((int)EquipPackSlot.Slot_HEAD);
            if (index < m_EquipSlotChooseSprite.Length)
            {
                m_EquipSlotChooseSprite[index].gameObject.SetActive(true);
            }
        }
        else
        {
            GameItem item = GameManager.gameManager.OtherPlayerData.EquipPack.GetItem(1);
            if (item != null && item.IsValid())
            {
                EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Info);
            }
        }
    }

    /// <summary>
    /// 点击装备槽位
    /// </summary>
    public void OnClick_Equip_ARMOR()
    {
        if (null == m_GemView)
        {
            LogModule.ErrorLog("m_GemView is null");
            return;
        }
        if (m_GemView.activeSelf && OtherGemViewLogic.Instance() != null)
        {
            OtherGemViewLogic.Instance().OnClickEquiSlot((int)EquipPackSlot.Slot_ARMOR);
            ClearAllSlotChoose();
            int index = GetIndexByEquipSlot((int)EquipPackSlot.Slot_ARMOR);
            if (index < m_EquipSlotChooseSprite.Length)
            {
                m_EquipSlotChooseSprite[index].gameObject.SetActive(true);
            }
        }
        else
        {
            GameItem item = GameManager.gameManager.OtherPlayerData.EquipPack.GetItem(2);
            if (item != null && item.IsValid())
            {
                EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Info);
            }
        }
    }

    /// <summary>
    /// 点击装备槽位
    /// </summary>
    public void OnClick_Equip_LEG_GUARD()
    {
        if (null == m_GemView)
        {
            LogModule.ErrorLog("m_GemView is null");
            return;
        }
        if (m_GemView.activeSelf && OtherGemViewLogic.Instance() != null)
        {
            OtherGemViewLogic.Instance().OnClickEquiSlot((int)EquipPackSlot.Slot_LEG_GUARD);
            ClearAllSlotChoose();
            int index = GetIndexByEquipSlot((int)EquipPackSlot.Slot_LEG_GUARD);
            if (index < m_EquipSlotChooseSprite.Length)
            {
                m_EquipSlotChooseSprite[index].gameObject.SetActive(true);
            }
            
        }
        else
        {
            GameItem item = GameManager.gameManager.OtherPlayerData.EquipPack.GetItem(3);
            if (item != null && item.IsValid())
            {
                EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Info);
            }
        }
    }

    /// <summary>
    /// 点击装备槽位
    /// </summary>
    public void OnClick_Equip_CUFF()
    {
        if (null == m_GemView)
        {
            LogModule.ErrorLog("m_GemView is null");
            return;
        }
        if (m_GemView.activeSelf && OtherGemViewLogic.Instance() != null)
        {
            OtherGemViewLogic.Instance().OnClickEquiSlot((int)EquipPackSlot.Slot_CUFF);
            ClearAllSlotChoose();
            int index = GetIndexByEquipSlot((int)EquipPackSlot.Slot_CUFF);
            if (index < m_EquipSlotChooseSprite.Length)
            {
                m_EquipSlotChooseSprite[index].gameObject.SetActive(true);
            }
           
        }
        else
        {
            GameItem item = GameManager.gameManager.OtherPlayerData.EquipPack.GetItem(4);
            if (item != null && item.IsValid())
            {
                EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Info);
            }
        }
    }

    /// <summary>
    /// 点击装备槽位
    /// </summary>
    public void OnClick_Equip_SHOES()
    {
        if (null == m_GemView)
        {
            LogModule.ErrorLog("m_GemView is null");
            return;
        }
        if (m_GemView.activeSelf && OtherGemViewLogic.Instance() != null)
        {
            OtherGemViewLogic.Instance().OnClickEquiSlot((int)EquipPackSlot.Slot_SHOES);
            ClearAllSlotChoose();
            int index = GetIndexByEquipSlot((int)EquipPackSlot.Slot_SHOES);
            if (index < m_EquipSlotChooseSprite.Length)
            {
                m_EquipSlotChooseSprite[index].gameObject.SetActive(true);
            }
        }
        else
        {
            GameItem item = GameManager.gameManager.OtherPlayerData.EquipPack.GetItem(5);
            if (item != null && item.IsValid())
            {
                EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Info);
            }
        }
    }

    /// <summary>
    /// 点击装备槽位
    /// </summary>
    public void OnClick_Equip_NECK()
    {
        if (null == m_GemView)
        {
            LogModule.ErrorLog("m_GemView is null");
            return;
        }
        if (m_GemView.activeSelf && OtherGemViewLogic.Instance() != null)
        {
            OtherGemViewLogic.Instance().OnClickEquiSlot((int)EquipPackSlot.Slot_NECK);
            ClearAllSlotChoose();
            int index = GetIndexByEquipSlot((int)EquipPackSlot.Slot_NECK);
            if (index < m_EquipSlotChooseSprite.Length)
            {
                m_EquipSlotChooseSprite[index].gameObject.SetActive(true);
            }
        }
        else
        {
            GameItem item = GameManager.gameManager.OtherPlayerData.EquipPack.GetItem(6);
            if (item != null && item.IsValid())
            {
                EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Info);
            }
        }
    }

    /// <summary>
    /// 点击装备槽位
    /// </summary>
    public void OnClick_Equip_RING()
    {
        if (null == m_GemView)
        {
            LogModule.ErrorLog("m_GemView is null");
            return;
        }
        if (m_GemView.activeSelf && OtherGemViewLogic.Instance() != null)
        {
            OtherGemViewLogic.Instance().OnClickEquiSlot((int)EquipPackSlot.Slot_RING);
            ClearAllSlotChoose();
            int index = GetIndexByEquipSlot((int)EquipPackSlot.Slot_RING);
            if (index < m_EquipSlotChooseSprite.Length)
            {
                m_EquipSlotChooseSprite[index].gameObject.SetActive(true);
            }
        }
        else
        {
            GameItem item = GameManager.gameManager.OtherPlayerData.EquipPack.GetItem(7);
            if (item != null && item.IsValid())
            {
                EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Info);
            }
        }
    }

    /// <summary>
    /// 点击装备槽位
    /// </summary>
    public void OnClick_Equip_AMULET()
    {
        if (null == m_GemView)
        {
            LogModule.ErrorLog("m_GemView is null");
            return;
        }
        if (m_GemView.activeSelf && OtherGemViewLogic.Instance() != null)
        {
            OtherGemViewLogic.Instance().OnClickEquiSlot((int)EquipPackSlot.Slot_AMULET);
            ClearAllSlotChoose();
            int index = GetIndexByEquipSlot((int)EquipPackSlot.Slot_AMULET);
            if (index < m_EquipSlotChooseSprite.Length)
            {
                m_EquipSlotChooseSprite[index].gameObject.SetActive(true);
            }      
        }
        else
        {
            GameItem item = GameManager.gameManager.OtherPlayerData.EquipPack.GetItem(8);
            if (item != null && item.IsValid())
            {
                EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Info);
            }
        }
    }

    /// <summary>
    /// 点击装备槽位
    /// </summary>
    public void OnClick_Equip_BELT()
    {
        if (null == m_GemView)
        {
            LogModule.ErrorLog("m_GemView is null");
            return;
        }
        if (m_GemView.activeSelf && OtherGemViewLogic.Instance() != null)
        {
            //Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2133}");
            OtherGemViewLogic.Instance().OnClickEquiSlot((int)EquipPackSlot.Slot_BELT);
            ClearAllSlotChoose();
            int index = GetIndexByEquipSlot((int)EquipPackSlot.Slot_BELT);
            if (index < m_EquipSlotChooseSprite.Length)
            {
                m_EquipSlotChooseSprite[index].gameObject.SetActive(true);
            }      
        }
        else
        {
            GameItem item = GameManager.gameManager.OtherPlayerData.EquipPack.GetItem(9);
            if (item != null && item.IsValid())
            {
                EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Info);
            }
        }
    }

    void ClearAllSlotChoose()
    {
        for (int i = 0; i < (int)EquipPackSlot.Slot_NUM && i < m_EquipSlotChooseSprite.Length; i++)
        {
            if (m_EquipSlotChooseSprite[i] != null)
            {
                m_EquipSlotChooseSprite[i].gameObject.SetActive(false);
            }           
        }
    }

    public void GemBtClick()
    {
        ChooseTabButton(CONTENT_TYPE.CONTENT_TYPE_GEM);
        if (m_AttrView != null)
        {
            m_AttrView.SetActive(false);
        }
       if (m_GemView != null)
       {
           m_GemView.SetActive(true);
       }   
		m_LeftView.SetActive(true);
		GameManager.gameManager.ActiveScene.ShowFakeObj();
    }

    public void CloseWindow()
    {
        UIManager.CloseUI(UIInfo.OtherRoleView);
        if (m_oPenType == OPEN_TYPE.OPEN_TYPE_FRIEND)
        {
			PlayerFrameLogic.Instance().SwitchAllWhenPopUIShow (false);
            UIManager.ShowUI(UIInfo.RelationRoot);
        }
        if (m_oPenType == OPEN_TYPE.OPEN_TYPE_LASTSPEAKER)
        {
            UIManager.ShowUI(UIInfo.ChatInfoRoot);
        }
		if (m_oPenType == OPEN_TYPE.OPEN_TYPE_TEAM) 
		{
			PlayerFrameLogic.Instance().SwitchAllWhenPopUIShow (false);
			RelationLogic.OpenTeamWindow(RelationTeamWindow.TeamTab.TeamTab_TeamInfo);
		}
        m_oPenType = OPEN_TYPE.OPEN_TYPE_INVALID;
    }

    void ChooseTabButton(CONTENT_TYPE eContent)
    {
        if ((int)eContent < m_TabButtonList.Count && (int)eContent >= 0)
        {
            m_TabButtonList[(int)eContent].normalSprite = "tab0";
			m_TabButtonList[(int)eContent].hoverSprite = "tab0";
			m_TabButtonList[(int)eContent].pressedSprite = "tab0";
			m_TabButtonList[(int)eContent].disabledSprite = "tab0";
			m_TabButtonList[(int)eContent].target.spriteName = "tab0";
            m_TabButtonList[(int)eContent].target.MakePixelPerfect();
        }
        for (int i = 0; i < m_TabButtonList.Count; i++)
        {
            if (i != (int)eContent)
            {
                m_TabButtonList[i].normalSprite = "tab";
				m_TabButtonList[i].hoverSprite = "tab";
				m_TabButtonList[i].pressedSprite = "tab";
				m_TabButtonList[i].disabledSprite = "tab";
				m_TabButtonList[i].target.spriteName = "tab";
                m_TabButtonList[i].target.MakePixelPerfect();
            }
        }
        ClearAllSlotChoose();
    }
}