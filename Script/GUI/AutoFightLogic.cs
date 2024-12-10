using UnityEngine;
using System;
using System.Collections;
using Games.LogicObj;
using Games.GlobeDefine;
using GCGame.Table;
using Module.Log;
using Games.Item;
using GCGame;
using Games.SkillModle;
using System.Collections.Generic;

public class AutoFightLogic : MonoBehaviour {

    private static AutoFightLogic m_Instance = null;


    public static AutoFightLogic Instance()
    {
        return m_Instance;
    }

    public UIToggle m_AutoFightOpenToggle;
    
    public UILabel m_AutoFightInfoHp;
    public UISlider m_AutoFightInfoHpSlider;
    public UILabel m_AutoFightInfoMp;
    public UISlider m_AutoFightInfoMppSlider;
    //public UILabel m_AutoFightInfoSearch;
   // public UISlider m_AutoFightInfoSearchSlider;
    public UIToggle m_AutoFightBaiSe;
    public UIToggle m_AutoFightYouXiu;
    public UIToggle m_AutoFightJingLiang;
    public UIToggle m_AutoFightShiShi;
    public UIToggle m_AutoFightChuanQi;
    public UIToggle m_AutoFightZhengQi;
    public UIToggle m_AutoFightQiTa;
    public UIToggle m_AutoFightDrug;
    public UIToggle m_AutoFightTeam;
	public UIToggle m_AutoFightAcceptTaem;
    public UISprite m_AutoFightInfoHpIndex;
    public UISprite m_AutoFightInfoHpQuality;
    public UISprite m_AutoFightInfoMpIndex;
    public UISprite m_AutoFightInfoMpQuality;
    public UISprite m_NilAutoFightInfoHp;
    public UISprite m_NilAutoFightInfoMp;
    public UISprite m_AutoFightMpSprite;
    public UILabel m_AutoBeiginName;

    public UILabel m_PlayerPosLabel;
     
    public UISprite m_AutoFightInfoEquipIndex;
    public UISprite m_NilAutoFightInfoEquip;
    public UISprite m_AutoFightInfoEquipQuality;
    // 新手指引
    public int m_NewPlayerGuide_Step = 0;
    public UILabel m_PickUpInfo;
    public GameObject m_PickUpDisable;
    private int m_viplimit = GlobeVar.USE_AUTOFIGHT_VIPLEVEL;
	public GameObject m_SkillBtGird;
	private List<SkillRootBarItemLogic>  m_skillButtonItemLogicList = new List<SkillRootBarItemLogic>();
	private List<UIToggle>   m_skillToggles=new List<UIToggle>();
    void Awake()
    {
        m_Instance = this;
    }

	// Use this for initialization
	void Start ()
	{
		UIManager.LoadItem(UIInfo.SystemSkillBarItem, OnLoadButtonItem);
		OnEnable();
	}

    void OnEnable()
    {
        if (m_AutoFightOpenToggle)
        {
            m_AutoFightOpenToggle.value = false;
        }
        //这里如果没有MainPlayer的话就跳过了，不太对，后续应该修改
        Obj_MainPlayer User = Singleton<ObjManager>.Instance.MainPlayer;
        if (User && User.Controller)
        {
            m_AutoFightOpenToggle.value = User.Controller.CombatFlag;

            m_AutoFightBaiSe.value = User.GetAutoPickUpFlag((int)GameDefine_Globe.AUTOCOMBAT_PICKUP_TYPE.PICKUP_EQUIP1);
            m_AutoFightYouXiu.value = User.GetAutoPickUpFlag((int)GameDefine_Globe.AUTOCOMBAT_PICKUP_TYPE.PICKUP_EQUIP2);
            m_AutoFightJingLiang.value = User.GetAutoPickUpFlag((int)GameDefine_Globe.AUTOCOMBAT_PICKUP_TYPE.PICKUP_EQUIP3);
            m_AutoFightShiShi.value = User.GetAutoPickUpFlag((int)GameDefine_Globe.AUTOCOMBAT_PICKUP_TYPE.PICKUP_EQUIP4);
            m_AutoFightChuanQi.value = User.GetAutoPickUpFlag((int)GameDefine_Globe.AUTOCOMBAT_PICKUP_TYPE.PICKUP_EQUIP5);
            m_AutoFightZhengQi.value = User.GetAutoPickUpFlag((int)GameDefine_Globe.AUTOCOMBAT_PICKUP_TYPE.PICKUP_STUFF);
            m_AutoFightQiTa.value = User.GetAutoPickUpFlag((int)GameDefine_Globe.AUTOCOMBAT_PICKUP_TYPE.PICKUP_OTHER);

            m_AutoFightTeam.value = User.AutoTaem;
			m_AutoFightAcceptTaem.value = User.AutoAcceptTaem; 
            m_AutoFightInfoHpSlider.value = User.AutoHpPercent;
            m_AutoFightInfoMppSlider.value = User.AutoMpPercent;
            //m_AutoFightInfoSearchSlider.value = (float)User.AutoRadius / 100;
            m_AutoFightDrug.value = User.AutoBuyDrug;
			int startindex=GameManager.gameManager.PlayerDataPool.AttackCount;
			for(int i=0;i<m_skillToggles.Count;i++)
			{
				UIToggle ut=m_skillToggles[i];
				if(ut!=null)
					ut.value=User.OwnAutoSkillInfo[startindex+i].CanAutoCombat;
			}
            UpdateDrug();
        }
        if (m_AutoFightOpenToggle.value == true)
        {
            m_AutoBeiginName.text = StrDictionary.GetClientDictionaryString("#{1444}");
        }
        else
        {
            m_AutoBeiginName.text = StrDictionary.GetClientDictionaryString("#{1194}");
        }

        // 新手指引
        Check_NewPlayerGuide();
        RefreshPickUp();

		AutoFightOK ();
    }

    void OnDestroy()
    {
        m_Instance = null;
    }
    /*
    public void OpenWindow()
    {
        if (Singleton<ObjManager>.GetInstance().MainPlayer.Controller)
        {
            m_AutoFightOpenToggle.value = Singleton<ObjManager>.GetInstance().MainPlayer.Controller.CombatFlag;
        }
    }
    */

	//初始化技能按钮信息
	void OnLoadButtonItem(GameObject resObj, object param)
	{
		if (null == resObj)
		{
			LogModule.ErrorLog("load OnLoadButtonItem error");
			return;
		}
		Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
		if (_mainPlayer ==null)
		{
			return;
		}
		//已经学会的技能
		int startindex = GameManager.gameManager.PlayerDataPool.AttackCount;
		int length=_mainPlayer.OwnAutoSkillInfo.Length-startindex;
		for (int nIndex = startindex; nIndex < length; ++nIndex)
		{
			if (_mainPlayer.OwnAutoSkillInfo[nIndex].IsValid())
			{
				GameObject _gameObject = Utils.BindObjToParent(resObj, m_SkillBtGird, (nIndex + 1000).ToString());
				if (_gameObject!=null)
				{

					UIToggle  toggle=_gameObject.GetComponentInChildren<UIToggle>();
					if(toggle!=null&&toggle.gameObject!=null)
						m_skillToggles.Add(toggle);
					SkillRootBarItemLogic _buttonItemLogic = _gameObject.GetComponent<SkillRootBarItemLogic>();
					if (_buttonItemLogic != null)
					{
				
						_buttonItemLogic.UpdateSkillBarInfo(nIndex);
						m_skillButtonItemLogicList.Add(_buttonItemLogic);
					


					}
				}
			}
		}

		m_SkillBtGird.GetComponent<UIGrid>().hideInactive = true;
		m_SkillBtGird.GetComponent<UIGrid>().sorted = true;
		m_SkillBtGird.GetComponent<UIGrid>().Reposition();

	}
    public void AutoFightOK()
    {
        //LogModule.DebugLog("AutoFightOK:"+m_AutoFightOpenToggle.value);
        Obj_MainPlayer User = Singleton<ObjManager>.Instance.MainPlayer;
        if( User )  //更新信息
        {
            if (VipData.GetVipLv() >= m_viplimit && User.BaseAttr.Level >= GlobeVar.MAX_AUTOEQUIT_LIVE)
            {
                User.SetAutoPickUpFlag((int)GameDefine_Globe.AUTOCOMBAT_PICKUP_TYPE.PICKUP_EQUIP1, m_AutoFightBaiSe.value);
                User.SetAutoPickUpFlag((int)GameDefine_Globe.AUTOCOMBAT_PICKUP_TYPE.PICKUP_EQUIP2, m_AutoFightYouXiu.value);
                User.SetAutoPickUpFlag((int)GameDefine_Globe.AUTOCOMBAT_PICKUP_TYPE.PICKUP_EQUIP3, m_AutoFightJingLiang.value);
                User.SetAutoPickUpFlag((int)GameDefine_Globe.AUTOCOMBAT_PICKUP_TYPE.PICKUP_EQUIP4, m_AutoFightShiShi.value);
                User.SetAutoPickUpFlag((int)GameDefine_Globe.AUTOCOMBAT_PICKUP_TYPE.PICKUP_EQUIP5, m_AutoFightChuanQi.value);
                User.SetAutoPickUpFlag((int)GameDefine_Globe.AUTOCOMBAT_PICKUP_TYPE.PICKUP_STUFF, m_AutoFightZhengQi.value);
                User.SetAutoPickUpFlag((int)GameDefine_Globe.AUTOCOMBAT_PICKUP_TYPE.PICKUP_OTHER, m_AutoFightQiTa.value);
            }         
            User.AutoTaem = m_AutoFightTeam.value;
			User.AutoAcceptTaem=m_AutoFightAcceptTaem.value ;
            User.AutoHpPercent = m_AutoFightInfoHpSlider.value;
            User.AutoMpPercent = m_AutoFightInfoMppSlider.value;
            //User.AutoRadius = (int)(m_AutoFightInfoSearchSlider.value * 100);
            User.AutoBuyDrug = m_AutoFightDrug.value;

            
			int startindex = GameManager.gameManager.PlayerDataPool.AttackCount;
			int length=User.OwnAutoSkillInfo.Length-startindex;
			if(m_skillToggles.Count!=0)
			{
			for(int i=0;i<length;i++)
			{
				Tab_SkillEx _SkillEx = TableManager.GetSkillExByID(User.OwnAutoSkillInfo[startindex+i].SkillId, 0);
				if (_SkillEx !=null)
				{
					if (User.OwnAutoSkillInfo[startindex+i].IsValid())
				
				{
				UIToggle ut=m_skillToggles[i];
				if(ut!=null)
				{
							User.OwnAutoSkillInfo[startindex+i].CanAutoCombat=ut.value;
				}
				}
			}
			}
			}
			User.ServerAutoInfo();
            if (m_AutoFightOpenToggle.value == true)
            {

                User.EnterAutoCombat();
            }
            else
            {
                User.LeveAutoCombat();
            }
            //存储注册表
//             PlayerPreferenceData.LastAutoComabat = m_AutoFightOpenToggle.value ? 1 : 0;
//             PlayerPreferenceData.LastAutoPickUp = User.AutoPickUp;
//             PlayerPreferenceData.LastAutoTaem = User.AutoTaem ? 1 : 0;
//             PlayerPreferenceData.LastAutoHpPercent = User.AutoHpPercent;
//             PlayerPreferenceData.LastAutoMpPercent = User.AutoMpPercent;
//             PlayerPreferenceData.LastAutoRadius = User.AutoRadiu

//			if(ObjManager.GetInstance().MainPlayerOnLoad != null) 
//			{
//				ObjManager.GetInstance().MainPlayerOnLoad();
//			}
        }
        
    }

    void OnCloseClick()
    {
        AutoFightOK();
        UIManager.CloseUI(UIInfo.SystemAndAutoFight);
       
    }

    public void AutoFightInfoHpSlider()
    {
        m_AutoFightInfoHp.text = ((int)(m_AutoFightInfoHpSlider.value * 99 )).ToString();
    }
    public void AutoFightInfoMppSlider()
    {
        m_AutoFightInfoMp.text = ((int)(m_AutoFightInfoMppSlider.value * 99)).ToString();
    }
//     public void AutoFightInfoSearchSlider()
//     {
//         m_AutoFightInfoSearch.text = ((int)(m_AutoFightInfoSearchSlider.value * 100)).ToString();
//     }
    public void OnAutoFihtHpClick()
    {
       // AutoDrugLogic.OpenUI(1,this);
        //UIManager.CloseUI(UIInfo.AutoDrug);
        UIManager.ShowUI(UIInfo.AutoDrug, OnOpenDrugWindowHp);        
        
        //AutoDrugLogic.Instance().transform.localPosition = new Vector3();
       
    }

    
    public void OnAutoFihtMpClick()
    {
        //AutoDrugLogic.OpenUI(2, this);
        //UIManager.CloseUI(UIInfo.AutoDrug);
        UIManager.ShowUI(UIInfo.AutoDrug, OnOpenDrugWindowMp);
        //AutoDrugLogic.Instance().setData(2, this);
        //AutoDrugLogic.Instance().transform.localPosition = new Vector3();
    }
   
    void OnOpenDrugWindowHp(bool bSuccess, object param)
    {
        if (bSuccess)
        {
            AutoDrugLogic.Instance().setData(1, this);
        }
        else
        {
            LogModule.ErrorLog("open drug window fail");
        }
    }

    void OnOpenDrugWindowMp(bool bSuccess, object param)
    {
        if (bSuccess)
        {
            AutoDrugLogic.Instance().setData(2, this);
        }
        else
        {
            LogModule.ErrorLog("open drug window fail");
        }
    }
    public void OnAutoEquipClick()
    {
        int nlevel = 1;
        if (Singleton<ObjManager>.Instance.MainPlayer)
        {
            nlevel = Singleton<ObjManager>.Instance.MainPlayer.BaseAttr.Level;
        }
        if (VipData.GetVipLv() >= m_viplimit && nlevel >= GlobeVar.MAX_AUTOEQUIT_LIVE)
        {
            UIManager.ShowUI(UIInfo.AutoDrug, OnOpenDrugWindowEquip);
        }
    }
    void OnOpenDrugWindowEquip(bool bSuccess, object param)
    {
        int nlevel = 1;
        if (Singleton<ObjManager>.Instance.MainPlayer)
        {
            nlevel = Singleton<ObjManager>.Instance.MainPlayer.BaseAttr.Level;
        }
        if (VipData.GetVipLv() >= m_viplimit && nlevel >= GlobeVar.MAX_AUTOEQUIT_LIVE)
        {
            if (bSuccess)
            {
                AutoDrugLogic.Instance().setData(3, this);
                AutoDrugLogic.Instance().gameObject.transform.localPosition += new Vector3(140f,0,0);
            }
            else
            {
                LogModule.ErrorLog("open drug window fail");
            }
        }
    }
    public void UpdateDrug()
    {
        m_AutoFightInfoHpIndex.spriteName = "";
        m_AutoFightInfoMpIndex.spriteName = "";

        m_AutoFightInfoHpQuality.gameObject.SetActive(false);
        m_AutoFightInfoMpQuality.gameObject.SetActive(false);

        m_NilAutoFightInfoHp.gameObject.SetActive(false);
        m_NilAutoFightInfoMp.gameObject.SetActive(false);
        m_AutoFightInfoEquipIndex.spriteName = "";
        m_NilAutoFightInfoEquip.gameObject.SetActive(false);
        GameItemContainer BackPack = GameManager.gameManager.PlayerDataPool.BackPack;
        Obj_MainPlayer User = Singleton<ObjManager>.Instance.MainPlayer;
        if( User )  //更新信息
        {
            if (User.AutoHpID != -1)
            {
                Tab_CommonItem curItem = TableManager.GetCommonItemByID(User.AutoHpID,0);
                if (curItem != null)
                {
                    m_AutoFightInfoHpIndex.spriteName = curItem.Icon;
                    m_AutoFightInfoHpQuality.gameObject.SetActive(true);
                    m_AutoFightInfoHpQuality.spriteName = GlobeVar.QualityColorGrid[curItem.Quality-1];
                    if (BackPack.GetItemCountByDataId(User.AutoHpID) <= 0)
                    {
                        m_NilAutoFightInfoHp.gameObject.SetActive(true);
                    }
                }
                
            }
            if (User.AutoMpID != -1)
            {
                Tab_CommonItem curItem = TableManager.GetCommonItemByID(User.AutoMpID, 0);
                if (curItem != null)
                {
                    m_AutoFightInfoMpIndex.spriteName = curItem.Icon;
                    m_AutoFightInfoMpQuality.gameObject.SetActive(true);
                    m_AutoFightInfoMpQuality.spriteName = GlobeVar.QualityColorGrid[curItem.Quality - 1];
                    if (BackPack.GetItemCountByDataId(User.AutoMpID) <= 0)
                    {
                        m_NilAutoFightInfoMp.gameObject.SetActive(true);
                    }
                }
            }
            if (User.AutoEquipGuid !=GlobeVar.INVALID_GUID)
            {
                 GameItemContainer EquipPack = GameManager.gameManager.PlayerDataPool.EquipPack;
                 if (EquipPack != null)
                {
                    for (int index = 0; index < EquipPack.ContainerSize; index++)
                     {
                         GameItem equip = EquipPack.GetItem(index);
                         if (equip != null && equip.IsValid() && equip.Guid == User.AutoEquipGuid)
                         {
                             Tab_CommonItem curItem = TableManager.GetCommonItemByID(equip.DataID, 0);
                             if (null != curItem)
                             {
                                 m_AutoFightInfoEquipIndex.spriteName = curItem.Icon;
                                 //m_NilAutoFightInfoEquip.gameObject.SetActive(true);
                                 m_AutoFightInfoEquipQuality.spriteName = equip.GetQualityFrame();
                             }
                         }
                     }
                }
            }
        }
    }
    public void ToggleButtonClick()
    {
        if (m_NewPlayerGuide_Step == 1)
        {
            m_NewPlayerGuide_Step = 0;
            NewPlayerGuidLogic.CloseWindow();
        }
        if (m_AutoFightOpenToggle.value == true)
        {
            m_AutoBeiginName.text = StrDictionary.GetClientDictionaryString("#{1444}");
        }
        else
        {
            m_AutoBeiginName.text = StrDictionary.GetClientDictionaryString("#{1194}");
        }
        AutoFightOK();
        //UIManager.CloseUI(UIInfo.SystemAndAutoFight);
    }


    void Check_NewPlayerGuide()
    {
        if (FunctionButtonLogic.Instance() == null)
        {
            return;
        }

        int nIndex = FunctionButtonLogic.Instance().NewPlayerGuide_Step;
        if (nIndex == 7)
        {
            NewPlayerGuide(1);
            FunctionButtonLogic.Instance().NewPlayerGuide_Step = -1;
        }
    }

    public void NewPlayerGuide(int nIndex)
    {
        m_NewPlayerGuide_Step = nIndex;
        switch (nIndex)
        {
            case 1:
                //NewPlayerGuidLogic.OpenWindow(m_AutoFightOpenToggle.gameObject, 300, 100, "请点此处", "left",0, true, true);
                NewPlayerGuidLogic.OpenWindow(m_AutoFightOpenToggle.gameObject, 300, 100, StrDictionary.GetClientDictionaryString("#{2813}"), "left", 2, true, true);
                break;
        }
    }

    public void Update() 
    {

        if (m_PlayerPosLabel != null && ObjManager.Instance.MainPlayer != null) 
        {
            m_PlayerPosLabel.text ="(" + ObjManager.Instance.MainPlayer.transform.position.x.ToString()+","+ObjManager.Instance.MainPlayer.transform.position.z+")";
        }
    }

    void RefreshPickUp()
    {
        int nlevel = 1;
        if (Singleton<ObjManager>.Instance.MainPlayer)
        {
            nlevel = Singleton<ObjManager>.Instance.MainPlayer.BaseAttr.Level;
        }
        if (VipData.GetVipLv() >= m_viplimit && nlevel >= GlobeVar.MAX_AUTOEQUIT_LIVE)
        {
            m_PickUpDisable.SetActive(false);
            m_PickUpInfo.text = StrDictionary.GetClientDictionaryString("#{3278}");// "自动强化设置";
            m_PickUpInfo.color = Color.white;

            m_AutoFightBaiSe.enabled = true;
            m_AutoFightYouXiu.enabled = true;
            m_AutoFightJingLiang.enabled = true;
            m_AutoFightShiShi.enabled = true;
            m_AutoFightChuanQi.enabled = true;
            m_AutoFightZhengQi.enabled = true;
            m_AutoFightQiTa.enabled = true;
        }
        else
        {
            m_PickUpDisable.SetActive(true);
            m_PickUpInfo.text = StrDictionary.GetClientDictionaryString("#{3279}");// "VIP2开启自动强化功能";
            m_PickUpInfo.color = new Color(209f / 255f, 63f / 255f, 48f / 255f);

            m_AutoFightBaiSe.enabled = false;
            m_AutoFightYouXiu.enabled = false;
            m_AutoFightJingLiang.enabled = false;
            m_AutoFightShiShi.enabled = false;
            m_AutoFightChuanQi.enabled = false;
            m_AutoFightZhengQi.enabled = false;
            m_AutoFightQiTa.enabled = false;
        }
    }
}
