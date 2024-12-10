using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Lifetime;
using Games.GlobeDefine;
using Games.LogicObj;
using Games.SkillModle;
using GCGame;
using GCGame.Table;
using Module.Log;
using UnityEngine;
using System.Collections;

public class SkillRootLogic : MonoBehaviour 
{
	// Use this for initialization
    public UISprite m_SkillButtonDragSprite;
    public UISprite m_SkillBarDragSprite;
    public UILabel m_InfoSkillName;
    public UILabel m_InfoSkillCDTime;
    public UILabel m_InfoSkillDel;
    public UILabel m_InfoSkillType;
    public UILabel m_InfoSkillDec;
    public UILabel m_InfoNextSkillDec;
    public UILabel m_SkillUpNeedExp;
    public UILabel m_SkillUpNeedLev;
    public UILabel m_SkillUpNeedCoin;
    public UILabel m_SkillLevel;
    public UILabel m_SkillExp;
    public GameObject m_LevelUpInfo;
    public GameObject m_CurSkillDec;
    public GameObject m_NextSkillDec;
    public UIImageButton m_LevelUpBt;
    public GameObject m_LeftSkillDescBt;
    public GameObject m_RightSkillDescBt;

    public GameObject m_NormalSkillInfo;        //非师门技能信息
    public GameObject m_MasterSkillInfo;        //师门技能信息

    public UISprite m_SkillEffectIcon;//技能升级特效图标
    public GameObject m_SkillBak;//技能升级特效背景
    public TweenScale m_SkillEffectScale;
    public GameObject m_LevelBtTipsIcon;
    public GameObject m_MasterLevelBtTipsIcon;
    //未解锁的 技能栏
    public GameObject[] m_EmptySkillBarItem = new GameObject[(int)SKILLBAR.MAXSKILLBARNUM];

    //师门技能相关控件
    public UILabel m_CurTorchLabel;
    public UILabel m_NeedUserTorch;
    public UILabel m_NeedMasterTorch;
    public UILabel m_NeedLevel;
    public GameObject m_MasterLevelupInfo;
    public UIImageButton m_MasterLevelUpBt;
    public GameObject m_MasterLevelupLimit;
    public UILabel m_MasterSkillNotUseTips;

    private List<GameObject> m_skillBarList =new List<GameObject>();
    private List<SkillRootButtonItemLogic> m_skillButtonItemLogicList = new List<SkillRootButtonItemLogic>();
    private SkillRootButtonItemLogic m_curClickBtItem = null;
    public SkillRootButtonItemLogic CurClickBtItem
    {
        get { return m_curClickBtItem; }
        set { m_curClickBtItem = value; }
    }
    private int m_nSelectSkillIndex;
    public int SelectSkillIndex
    {
        get { return m_nSelectSkillIndex; }
        set { m_nSelectSkillIndex = value; }
    }
    public GameObject m_SkillBtGird;
    public GameObject m_SkillBarGird;
    public UIDraggablePanel m_skillBarDraggablePanel;
    public UIDraggablePanel m_skillBtDraggablePanel;
    private int m_NewPlayerGuide_Step = -1;
    public int NewPlayerGuide_Step
    {
        get { return m_NewPlayerGuide_Step; }
        set { m_NewPlayerGuide_Step = value; }
    }

    private static SkillRootLogic m_Instance = null;
    public static SkillRootLogic Instance()
    {
        return m_Instance;
    }
    void Awake()
    {
        m_Instance = this;
    }

    void OnDestroy()
    {
        m_Instance = null;
    }

	void Start ()
	{
        m_CurSkillDec.SetActive(true);
        m_SkillBarDragSprite.gameObject.SetActive(false);
        m_SkillButtonDragSprite.gameObject.SetActive(false);
        m_SkillBak.SetActive(false);
        UIManager.LoadItem(UIInfo.SkillRootButtonItem, OnLoadButtonItem);
        UIManager.LoadItem(UIInfo.SkillRootBarItem, OnLoadBarItem);
        //更新薪火值
        Singleton<ObjManager>.GetInstance().MainPlayer.ReqUpdateTorch();
        //更新师门信息
        if (Singleton<ObjManager>.GetInstance().MainPlayer.NeedRequestMasterInfo)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.ReqMasterInfo();
        }

        Check_NewPlayerGuide();
	}
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
        for (int nIndex = 0; nIndex < _mainPlayer.OwnSkillInfo.Length; ++nIndex)
        {
            if (_mainPlayer.OwnSkillInfo[nIndex].IsValid())
            {
                GameObject _gameObject = Utils.BindObjToParent(resObj, m_SkillBtGird, (nIndex + 1000).ToString());
                if (_gameObject!=null)
                {
                    SkillRootButtonItemLogic _buttonItemLogic = _gameObject.GetComponent<SkillRootButtonItemLogic>();
                    if (_buttonItemLogic != null)
                    {
                        _buttonItemLogic.UpdateButtonInfo(nIndex);
                        m_skillButtonItemLogicList.Add(_buttonItemLogic);
                        if (nIndex == 0)
                        {
                            _buttonItemLogic.ClickButton();
                        }
                        UIDragObject _dragObject =_buttonItemLogic.m_SkillIconSprite.GetComponent<UIDragObject>();
                        if (null != _dragObject)
                        {
                            _dragObject.enabled = true;
                            _dragObject.target = m_SkillButtonDragSprite.transform;
                            int _skillId = _mainPlayer.OwnSkillInfo[nIndex].SkillId;
                            //普攻和Xp 被动技 禁用拖曳图标
                            if (_skillId > 0)
                            {
                                Tab_SkillEx _skillEx = TableManager.GetSkillExByID(_skillId, 0);
                                if (_skillEx != null)
                                {
                                    Tab_SkillBase _skillBase = TableManager.GetSkillBaseByID(_skillEx.BaseId, 0);

                                    if (_skillBase != null)
                                    {
                                        if ((_skillBase.SkillClass & (int)SKILLCLASS.AUTOREPEAT) != 0 ||
                                            (_skillBase.SkillClass & (int)SKILLCLASS.XP) != 0 ||
                                            (_skillBase.SkillClass & (int)SKILLCLASS.PASSIVITY) != 0)
                                        {
                                            _dragObject.enabled = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        //未学习的技能
        for (int nIndex = 1; nIndex <= TableManager.GetSkillActive().Count; nIndex++)
        {
            Tab_SkillActive _skillActiveInfo = TableManager.GetSkillActiveByID(nIndex,0);
            if (_skillActiveInfo !=null && _mainPlayer.Profession ==_skillActiveInfo.Profession)
            {
                Tab_SkillEx _SkillEx = TableManager.GetSkillExByID(_skillActiveInfo.SkillId, 0);
                if (_SkillEx !=null)
                {
                    if (_mainPlayer.IsStudySkill(_SkillEx.BaseId) == false)
                    {
                        GameObject _gameObject = Utils.BindObjToParent(resObj, m_SkillBtGird,(nIndex + 2000).ToString());
                        if (_gameObject !=null)
                        {
                            SkillRootButtonItemLogic _buttonItemLogic = _gameObject.GetComponent<SkillRootButtonItemLogic>();
                            if (_buttonItemLogic !=null)
                            {
                                _buttonItemLogic.UpdateNoStudyButtonInfo(_skillActiveInfo.SkillId);
                                m_skillButtonItemLogicList.Add(_buttonItemLogic);
                                if (null != _gameObject.GetComponent<SkillRootButtonItemLogic>())
                                {
                                    UIDragObject _dragObject = _gameObject.GetComponent<SkillRootButtonItemLogic>().m_SkillIconSprite.GetComponent<UIDragObject>();
                                    // 禁用拖曳图标
                                    if (null != _dragObject)
                                        _dragObject.enabled = false;
                                }
                            }
                        }
                    }
                }
            }
        }
        m_SkillBtGird.GetComponent<UIGrid>().hideInactive = true;
        m_SkillBtGird.GetComponent<UIGrid>().sorted = true;
        m_SkillBtGird.GetComponent<UIGrid>().Reposition();
        m_skillBtDraggablePanel.repositionClipping = true;
    }

    public void PlaySkillLevelUpEffect()
    {
        if (BackCamerControll.Instance() !=null)
        {
            BackCamerControll.Instance().StopSceneEffect(GlobeVar.SKILLLEVUPEFFECTID,true);
            BackCamerControll.Instance().PlaySceneEffect(GlobeVar.SKILLLEVUPEFFECTID);
            if (m_curClickBtItem !=null)
            {
                m_SkillEffectIcon.spriteName = m_curClickBtItem.m_SkillIconSprite.spriteName;
                m_SkillEffectIcon.MakePixelPerfect();
                m_SkillEffectScale.Reset();
                m_SkillBak.SetActive(true);
            }
        }
    }

    public void SkillLevelUpEffectOver()
    {
        m_SkillBak.SetActive(false);
    }
    //初始化 技能栏信息
    void OnLoadBarItem(GameObject resObj, object param)
    {
        if (null == resObj)
        {
            LogModule.ErrorLog("load OnLoadBarItem error");
            return;
        }
        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (_mainPlayer == null)
        {
            return;
        }
        int nSkillBarUnLockCount = _mainPlayer.NeedSkillBarNum();
        for (int nIndex = 0; nIndex < (int)SKILLBAR.MAXSKILLBARNUM; ++nIndex)
        {
            if (nIndex >= 0 && nIndex < nSkillBarUnLockCount)
            {
                GameObject _gameObject = Utils.BindObjToParent(resObj, m_SkillBarGird, (nIndex + 1000).ToString());
                m_skillBarList.Add(_gameObject);
                if (null != _gameObject.GetComponent<SkillRootBarItemLogic>())
                    _gameObject.GetComponent<SkillRootBarItemLogic>().m_IconSprite.gameObject.SetActive(false);
                //读取技能栏配置
                Dictionary<string, SkillBarInfo[]> _skillBarSetMap = UserConfigData.GetSkillBarSetInfo();
                if (_skillBarSetMap != null && _skillBarSetMap.ContainsKey(_mainPlayer.GUID.ToString()))
                {
                    SkillBarInfo[] _skillBarSetInfo = _skillBarSetMap[_mainPlayer.GUID.ToString()];
                    _gameObject.GetComponent<SkillRootBarItemLogic>().UpdateSkillBarInfo(_skillBarSetInfo[nIndex].SkillIndex);
                }
                if (m_EmptySkillBarItem[nIndex] !=null)
                {
                    m_EmptySkillBarItem[nIndex].SetActive(false);
                }
            }
            else if (m_EmptySkillBarItem[nIndex] != null)
            {
                m_EmptySkillBarItem[nIndex].SetActive(true);
            }
        }
       
        m_SkillBarGird.GetComponent<UIGrid>().hideInactive = true;
        m_SkillBarGird.GetComponent<UIGrid>().sorted = true;
        m_SkillBarGird.GetComponent<UIGrid>().Reposition();
        m_skillBarDraggablePanel.repositionClipping = true;
        SkillLevelUpGuide();
    }
    
    public void SaveSkillBarSetInfo()
    {
        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (_mainPlayer == null)
        {
            return;
        }
        //更新右下角技能栏
        if (SkillBarLogic.Instance() != null)
        {
            for (int _BarIndex = 0; _BarIndex <m_skillBarList.Count; _BarIndex++)
            {
                if (_BarIndex >= 0 && _BarIndex < (int)SKILLBAR.MAXSKILLBARNUM)
                {
                   SkillBarLogic.Instance().MySkillBarInfo[_BarIndex].SkillIndex = m_skillBarList[_BarIndex].GetComponent<SkillRootBarItemLogic>().SkillIndex;
                }
            }
            //保存配置
            UserConfigData.AddSkillBarSetInfo(_mainPlayer.GUID.ToString(), SkillBarLogic.Instance().MySkillBarInfo);
            //更新
            SkillBarLogic.Instance().UpdateSkillBarInfo();
        }
    }

    public void ReleaseButtonDragSprite(int nSkillIndex)
    {
        Vector3 _dragVector3 = m_SkillBarGird.transform.InverseTransformPoint(m_SkillButtonDragSprite.transform.position);
        for (int i = 0; i < m_skillBarList.Count; i++)
        {
            float fDis = Vector3.Distance(_dragVector3, m_skillBarList[i].transform.localPosition);
            if (fDis-50 <=0)
            {
                m_skillBarList[i].GetComponent<SkillRootBarItemLogic>().UpdateSkillBarInfo(nSkillIndex);
                SaveSkillBarSetInfo();
                break;
            }
        }
        m_SkillButtonDragSprite.gameObject.SetActive(false);
    }
    public void OnCloseClick()
    {
        if (BackCamerControll.Instance() != null)
        {
            BackCamerControll.Instance().StopSceneEffect(GlobeVar.SKILLLEVUPEFFECTID, true);
        }
        UIManager.CloseUI(UIInfo.SkillInfo);
    }

    public void UpdateSkillInfo()
    {
        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (_mainPlayer == null)
        {
            return;
        }
        if (m_nSelectSkillIndex < 0 || m_nSelectSkillIndex >= _mainPlayer.OwnSkillInfo.Length)
        {
            return;
        }
        //更新左侧的所有的技能栏信息
        for (int nButtonIndex = 0; nButtonIndex < m_skillButtonItemLogicList.Count; nButtonIndex++)
        {
            if (m_skillButtonItemLogicList[nButtonIndex] != null)
            {
                int nSkillBaseId =m_skillButtonItemLogicList[nButtonIndex].SkillBaseId;
                int nSkillIndex = -1;
                if (_mainPlayer.IsStudySkill(nSkillBaseId, ref nSkillIndex))
                {
                    if (nSkillIndex >=0 && nSkillIndex<_mainPlayer.OwnSkillInfo.Length)
                    {
                        m_skillButtonItemLogicList[nButtonIndex].UpdateButtonInfo(nSkillIndex);
                    }
                }
            }
        }
        //更新当前选中的技能按钮信息
//         if (m_curClickBtItem != null)
//         {
//             m_curClickBtItem.UpdateButtonInfo(m_nSelectSkillIndex);
//         }
        //显示右侧技能信息
        ShowSkillInfo();
    }
    public  void ShowSkillInfo()
    {
        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (_mainPlayer == null)
        {
            return;
        }
        if (m_nSelectSkillIndex < 0 || m_nSelectSkillIndex >= _mainPlayer.OwnSkillInfo.Length)
        {
            return;
        }
        int nSkillId = _mainPlayer.OwnSkillInfo[m_nSelectSkillIndex].SkillId;
        Tab_SkillEx _skillEx = TableManager.GetSkillExByID(nSkillId, 0);
        if (_skillEx != null)
        {
            Tab_SkillBase _skillBase = TableManager.GetSkillBaseByID(_skillEx.BaseId, 0);
            if (_skillBase != null)
            {
                //名字
                if ((_skillBase.SkillClass & (int)SKILLCLASS.MASTERSKILL) != 0)
                {
                    m_InfoSkillName.text = GameManager.gameManager.PlayerDataPool.GetMasterSkillName(nSkillId);
                }
                else
                {
                    m_InfoSkillName.text = _skillBase.Name;
                }
                
                //冷却
                Tab_CoolDownTime _CDTimeInfo = TableManager.GetCoolDownTimeByID(_skillEx.CDTimeId, 0);
                if (_CDTimeInfo != null)
                {
                    int CDTime = _CDTimeInfo.CDTime / 1000;
                    m_InfoSkillCDTime.text = StrDictionary.GetClientDictionaryString("#{1794}", CDTime);
                }
                else
                {
                    m_InfoSkillCDTime.text = StrDictionary.GetClientDictionaryString("#{1793}");
                }
                //消耗
                string strDelInfo1 = StrDictionary.GetClientDictionaryString("#{1793}");
                string strDelInfo2 = StrDictionary.GetClientDictionaryString("#{1793}");
                SKILLDELANDGAINTYPE nType_1 = (SKILLDELANDGAINTYPE)_skillEx.GetDelTypebyIndex(0);
                if (nType_1 != SKILLDELANDGAINTYPE.HPTYPE_INVAILID)
                {
                    strDelInfo1 = GetTypeDesc(nType_1, _skillEx.GetDelNumbyIndex(0));
                    m_InfoSkillDel.text = strDelInfo1;
                }
                SKILLDELANDGAINTYPE nType_2 = (SKILLDELANDGAINTYPE)_skillEx.GetDelTypebyIndex(1);
                if (nType_2 != SKILLDELANDGAINTYPE.HPTYPE_INVAILID)
                {
                    strDelInfo2 = GetTypeDesc(nType_1, _skillEx.GetDelNumbyIndex(1));
                    m_InfoSkillDel.text = strDelInfo2;
                }
                if (nType_1 != SKILLDELANDGAINTYPE.HPTYPE_INVAILID &&
                    nType_2 != SKILLDELANDGAINTYPE.HPTYPE_INVAILID)
                {
                    m_InfoSkillDel.text = strDelInfo1 + "/" + strDelInfo2;
                }
                else if (nType_1 == SKILLDELANDGAINTYPE.HPTYPE_INVAILID &&
                    nType_2 == SKILLDELANDGAINTYPE.HPTYPE_INVAILID)
                {
                    m_InfoSkillDel.text = StrDictionary.GetClientDictionaryString("#{1793}");
                }
                
                //技能类型
                if (_skillBase.UseType == (int)SKILLUSETYPE.SHUNFA)
                {
                    m_InfoSkillType.text = StrDictionary.GetClientDictionaryString("#{1795}");
                }
                else if (_skillBase.UseType == (int)SKILLUSETYPE.YINCHANG)
                {
                    m_InfoSkillType.text = StrDictionary.GetClientDictionaryString("#{1796}");
                }
                else if (_skillBase.UseType == (int)SKILLUSETYPE.BEIDONG)
                {
                    m_InfoSkillType.text = StrDictionary.GetClientDictionaryString("#{3101}");
                }
                //技能描述 
                m_InfoSkillDec.text = _skillEx.Desc;

                //技能当前等级
                m_SkillLevel.text = StrDictionary.GetClientDictionaryString("#{2162}",_skillEx.Level);
                //技能升级信息

                //普攻和XP技无升级
                if ((_skillBase.SkillClass & (int)SKILLCLASS.AUTOREPEAT) != 0 ||
                    (_skillBase.SkillClass & (int)SKILLCLASS.XP) != 0)
                {
                    m_NormalSkillInfo.gameObject.SetActive(true);
                    m_MasterSkillInfo.gameObject.SetActive(false);

                    m_SkillExp.text = Utils.ConvertLargeNumToString(PVPData.Power);
                    m_SkillLevel.text= StrDictionary.GetClientDictionaryString("#{1810}"); //标明最高等级
                    m_LevelUpInfo.SetActive(false);
                    m_LeftSkillDescBt.SetActive(false);
                    m_RightSkillDescBt.SetActive(false);
                    m_NextSkillDec.SetActive(false);
                }
                //师门技能升级不走通用表格
                else if ((_skillBase.SkillClass & (int)SKILLCLASS.MASTERSKILL) != 0)
                {
                    m_NormalSkillInfo.gameObject.SetActive(false);
                    m_MasterSkillInfo.gameObject.SetActive(true);

                    Tab_MasterSkill line = TableManager.GetMasterSkillByID(nSkillId, 0);
                    if (line != null)
                    {
                        m_MasterLevelupInfo.SetActive(true);

                        m_MasterLevelupLimit.gameObject.SetActive(true);
                        m_MasterSkillNotUseTips.gameObject.SetActive(false);

                        if (line.NextSkillId == -1)
                        {
                   //         m_SkillLevel.text += StrDictionary.GetClientDictionaryString("#{1810}"); //标明最高等级
                            m_MasterLevelupInfo.SetActive(false);
                            m_LeftSkillDescBt.SetActive(false);
                            m_RightSkillDescBt.SetActive(false);
                            m_NextSkillDec.SetActive(false);
                        }
                        else
                        {
                            //当前个人薪火
                            m_CurTorchLabel.text = GameManager.gameManager.PlayerDataPool.TorchValue.ToString();
                            m_MasterLevelUpBt.isEnabled = true;
                            //显示角标
                            m_MasterLevelBtTipsIcon.SetActive(true);
                            int needUserTorch = line.ConsumeNum;
                            int needMasterTorch = line.MinMasterTorch;
                            int needLevel = line.MinLevel;
                            //个人薪火
                            if (GameManager.gameManager.PlayerDataPool.TorchValue >= needUserTorch)
                            {
                                m_NeedUserTorch.text = needUserTorch.ToString();
                            }
                            else
                            {
                                m_NeedUserTorch.text = StrDictionary.GetClientDictionaryString("#{1797}", needUserTorch);
                                m_MasterLevelUpBt.isEnabled = false; // 灰化按钮
                                //隐藏角标
                                m_MasterLevelBtTipsIcon.SetActive(false);
                            }
                            //师门薪火
                            if (GameManager.gameManager.PlayerDataPool.MasterInfo.MasterTorch >= needMasterTorch)
                            {
                                m_NeedMasterTorch.text = needMasterTorch.ToString();
                            }
                            else
                            {
                                m_NeedMasterTorch.text = StrDictionary.GetClientDictionaryString("#{1797}", needMasterTorch);
                                m_MasterLevelUpBt.isEnabled = false; // 灰化按钮
                                //隐藏角标
                                m_MasterLevelBtTipsIcon.SetActive(false);
                            }
                            //等级
                            if (GameManager.gameManager.PlayerDataPool.MainPlayerBaseAttr.Level >= needLevel)
                            {
                                m_NeedLevel.text = needLevel.ToString();
                            }
                            else
                            {
                                m_NeedLevel.text = StrDictionary.GetClientDictionaryString("#{1797}", needLevel);
                                m_MasterLevelUpBt.isEnabled = false; // 灰化按钮
                                //隐藏角标
                                m_MasterLevelBtTipsIcon.SetActive(false);
                            }

                            //师门技能是否可用
                            if (_mainPlayer.MasterSkillCanUse(nSkillId) == false)
                            {
                                m_MasterLevelupLimit.gameObject.SetActive(false);
                                m_MasterSkillNotUseTips.gameObject.SetActive(true);
                                m_MasterLevelUpBt.isEnabled = false; // 灰化按钮
                                //隐藏角标
                                m_MasterLevelBtTipsIcon.SetActive(false);
                            }
                            
                            //下一级技能描述
                            Tab_SkillEx _skillNextEx = TableManager.GetSkillExByID(line.NextSkillId, 0);
                            if (_skillNextEx != null)
                            {
                                m_InfoNextSkillDec.text = _skillNextEx.Desc;
                                m_LeftSkillDescBt.SetActive(false);
                                m_RightSkillDescBt.SetActive(true);
                                m_NextSkillDec.SetActive(false);
                                m_CurSkillDec.SetActive(true);
                            }
                            else
                            {
                                m_InfoNextSkillDec.text = "";
                                m_LeftSkillDescBt.SetActive(false);
                                m_RightSkillDescBt.SetActive(false);
                                m_NextSkillDec.SetActive(false);
                                m_CurSkillDec.SetActive(true);
                            }
                        }
                    }
                }
                else
                {
                    m_NormalSkillInfo.gameObject.SetActive(true);
                    m_MasterSkillInfo.gameObject.SetActive(false);

                    m_SkillExp.text = Utils.ConvertLargeNumToString(PVPData.Power);
                    Tab_SkillLevelUp _levelUp = TableManager.GetSkillLevelUpByID(nSkillId, 0);
                    if (_levelUp != null)
                    {
                        m_LevelUpInfo.SetActive(true);
                        if (_levelUp.NextSkillId == -1)
                        {
                            m_SkillLevel.text = StrDictionary.GetClientDictionaryString("#{1810}"); //标明最高等级
                            m_LevelUpInfo.SetActive(false);
                            m_LeftSkillDescBt.SetActive(false);
                            m_RightSkillDescBt.SetActive(false);
                            m_NextSkillDec.SetActive(false);
                        }
                        else
                        {
                            int nNeedExp = _levelUp.NeedConsume;
                            int nNeedLeve = _levelUp.Level;
                            //所需等级 
                            if (_mainPlayer.BaseAttr.Level >= nNeedLeve)
                            {
                                m_SkillUpNeedLev.text = nNeedLeve.ToString();
                            }
                            else
                            {
                                m_SkillUpNeedLev.text = StrDictionary.GetClientDictionaryString("#{1797}", nNeedLeve);
                                m_LevelUpBt.isEnabled = false; // 灰化按钮
                                //隐藏升级角标
                                m_LevelBtTipsIcon.SetActive(false);
                            }
                            //所需真气
                            if (PVPData.Power >= nNeedExp)
                            {
                                m_SkillUpNeedExp.text = nNeedExp.ToString();
                            }
                            else
                            {
                                m_SkillUpNeedExp.text = StrDictionary.GetClientDictionaryString("#{1797}", nNeedExp);
                                m_LevelUpBt.isEnabled = false; // 灰化按钮
                                //隐藏升级角标
                                m_LevelBtTipsIcon.SetActive(false);
                            }
                            //所需金币
                            int nCoin = GameManager.gameManager.PlayerDataPool.Money.GetMoney_Coin();
                            if (nCoin >=_levelUp.NeedCoin)
                            {
                                m_SkillUpNeedCoin.text = _levelUp.NeedCoin.ToString();
                            }
                            else
                            {
                                m_SkillUpNeedCoin.text = StrDictionary.GetClientDictionaryString("#{1797}", _levelUp.NeedCoin);
                                m_LevelUpBt.isEnabled = false; // 灰化按钮
                                //隐藏升级角标
                                m_LevelBtTipsIcon.SetActive(false);
                            }
                            if (PVPData.Power >= nNeedExp && _mainPlayer.BaseAttr.Level >= nNeedLeve && nCoin >= _levelUp.NeedCoin)
                            {
                                m_LevelUpBt.isEnabled = true; // 恢复按钮
                                //显示升级角标
                                m_LevelBtTipsIcon.SetActive(true);
                            }
                            

                            //下一级技能描述
                            Tab_SkillEx _skillNextEx = TableManager.GetSkillExByID(_levelUp.NextSkillId, 0);
                            if (_skillNextEx != null)
                            {
                                m_InfoNextSkillDec.text = _skillNextEx.Desc;
                                m_LeftSkillDescBt.SetActive(false);
                                m_RightSkillDescBt.SetActive(true);
                                m_NextSkillDec.SetActive(false);
                                m_CurSkillDec.SetActive(true);
                            }
                            else
                            {
                                m_InfoNextSkillDec.text = "";
                                m_LeftSkillDescBt.SetActive(false);
                                m_RightSkillDescBt.SetActive(false);
                                m_NextSkillDec.SetActive(false);
                                m_CurSkillDec.SetActive(true);
                            }
                        }
                    }
                }
            }
        }
    }
    public void ShowNoStudySkillInfo(int nSkillId)
    {
        Tab_SkillEx _skillEx = TableManager.GetSkillExByID(nSkillId, 0);
        if (_skillEx != null)
        {
            Tab_SkillBase _skillBase = TableManager.GetSkillBaseByID(_skillEx.BaseId, 0);
            if (_skillBase != null)
            {
                //名字
                if ((_skillBase.SkillClass & (int) SKILLCLASS.MASTERSKILL) != 0)
                {
                    m_InfoSkillName.text = GameManager.gameManager.PlayerDataPool.GetMasterSkillName(nSkillId);
                }
                else
                {
                    m_InfoSkillName.text = _skillBase.Name;
                }
                //冷却
                Tab_CoolDownTime _CDTimeInfo = TableManager.GetCoolDownTimeByID(_skillEx.CDTimeId, 0);
                if (_CDTimeInfo != null)
                {
                    int CDTime = _CDTimeInfo.CDTime/1000;
                    m_InfoSkillCDTime.text = StrDictionary.GetClientDictionaryString("#{1794}", CDTime);
                }
                else
                {
                    m_InfoSkillCDTime.text = StrDictionary.GetClientDictionaryString("#{1793}");
                }
                //消耗
                string strDelInfo1 = StrDictionary.GetClientDictionaryString("#{1793}");
                string strDelInfo2 = StrDictionary.GetClientDictionaryString("#{1793}");
                SKILLDELANDGAINTYPE nType_1 = (SKILLDELANDGAINTYPE) _skillEx.GetDelTypebyIndex(0);
                if (nType_1 != SKILLDELANDGAINTYPE.HPTYPE_INVAILID)
                {
                    strDelInfo1 = GetTypeDesc(nType_1, _skillEx.GetDelNumbyIndex(0));
                    m_InfoSkillDel.text = strDelInfo1;
                }
                SKILLDELANDGAINTYPE nType_2 = (SKILLDELANDGAINTYPE) _skillEx.GetDelTypebyIndex(1);
                if (nType_2 != SKILLDELANDGAINTYPE.HPTYPE_INVAILID)
                {
                    strDelInfo2 = GetTypeDesc(nType_1, _skillEx.GetDelNumbyIndex(1));
                    m_InfoSkillDel.text = strDelInfo2;
                }
                if (nType_1 != SKILLDELANDGAINTYPE.HPTYPE_INVAILID &&
                    nType_2 != SKILLDELANDGAINTYPE.HPTYPE_INVAILID)
                {
                    m_InfoSkillDel.text = strDelInfo1 + "/" + strDelInfo2;
                }
                else if (nType_1 == SKILLDELANDGAINTYPE.HPTYPE_INVAILID &&
                         nType_2 == SKILLDELANDGAINTYPE.HPTYPE_INVAILID)
                {
                    m_InfoSkillDel.text = StrDictionary.GetClientDictionaryString("#{1793}");
                }
                //技能类型
                if (_skillBase.UseType == (int) SKILLUSETYPE.SHUNFA)
                {
                    m_InfoSkillType.text = StrDictionary.GetClientDictionaryString("#{1795}");
                }
                else if (_skillBase.UseType == (int) SKILLUSETYPE.YINCHANG)
                {
                    m_InfoSkillType.text = StrDictionary.GetClientDictionaryString("#{1796}");
                }
                else if (_skillBase.UseType == (int)SKILLUSETYPE.BEIDONG)
                {
                    m_InfoSkillType.text = StrDictionary.GetClientDictionaryString("#{3101}");
                }
                //技能描述 
                m_InfoSkillDec.text = _skillEx.Desc;
                //可以激活的等级
                for (int i = 1; i <= TableManager.GetSkillActive().Count; i++)
                {
                    Tab_SkillActive _skillActiveInfo = TableManager.GetSkillActiveByID(i, 0);
                    if (_skillActiveInfo != null)
                    {
                        if (_skillActiveInfo.SkillId == nSkillId)
                        {
                            m_SkillLevel.text = String.Format("[FF0000]{0}", StrDictionary.GetClientDictionaryString("#{2770}", _skillActiveInfo.Level));
                            break;
                        }
                    }
                }
                //无技能升级信息
                m_NormalSkillInfo.gameObject.SetActive(true);
                m_MasterSkillInfo.gameObject.SetActive(false);
                m_SkillExp.text = Utils.ConvertLargeNumToString(PVPData.Power);
                m_LevelUpInfo.SetActive(false);
                m_LeftSkillDescBt.SetActive(false);
                m_RightSkillDescBt.SetActive(false);
                m_NextSkillDec.SetActive(false);
            }
        }
    }

    public static int GetCanLevUpSkillCount()
    {
        //已经学会的技能
        int nCanLevUpCount = 0;
        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (_mainPlayer == null)
        {
            return 0;
        }
        for (int nIndex = 0; nIndex < _mainPlayer.OwnSkillInfo.Length; ++nIndex)
        {
            if (_mainPlayer.OwnSkillInfo[nIndex].IsValid())
            {
                if (IsCanLevelUpSkill(_mainPlayer.OwnSkillInfo[nIndex].SkillId) ||
                    IsCanLevelUpMasterSkill(_mainPlayer.OwnSkillInfo[nIndex].SkillId))
                {
                    nCanLevUpCount++;
                }
            }
        }
        return nCanLevUpCount;
    }
    public static bool IsCanLevelUpSkill( int nCurSkillId)
    {
        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (_mainPlayer == null)
        {
            return false;
        }
        Tab_SkillLevelUp _skillLevUp = TableManager.GetSkillLevelUpByID(nCurSkillId, 0);
        if (_skillLevUp == null)
        {
            return false;
        }
        if (_mainPlayer.Profession != _skillLevUp.Profession)
        {
            return false;
        }
        if (_mainPlayer.BaseAttr.Level < _skillLevUp.Level)
        {
            return false;
        }
        if (_mainPlayer.IsHaveSkill(_skillLevUp.NeedSkillId) == false)
        {
            return false;
        }
        if (_skillLevUp.NextSkillId == -1)
        {
            return false;
        }
        //真气不足
        if (PVPData.Power < _skillLevUp.NeedConsume)
        {
            return false;
        }
        //金币不足
        int nCoin = GameManager.gameManager.PlayerDataPool.Money.GetMoney_Coin();
        if (nCoin < _skillLevUp.NeedCoin)
        {
            return false;
        }
        return true;
    }
    public static bool IsCanLevelUpMasterSkill(int nCurSkillId)
    {
        Tab_SkillEx _skillEx = TableManager.GetSkillExByID(nCurSkillId, 0);
        if (_skillEx == null)
        {
            return false;
        }
        
        Tab_SkillBase _skillBase = TableManager.GetSkillBaseByID(_skillEx.BaseId, 0);
        if (_skillBase == null)
        {
            return false;
        }

        //技能类型为师门技能
        if ((_skillBase.SkillClass & (int)SKILLCLASS.MASTERSKILL) == 0)
        {
            return false;
        }

        Tab_MasterSkill masterskill = TableManager.GetMasterSkillByID(nCurSkillId, 0);
        if (masterskill == null)
        {
            return false;
        }

        if (masterskill.NextSkillId == -1)
        {
            return false;
        }
        //个人薪火
        if (GameManager.gameManager.PlayerDataPool.TorchValue < masterskill.ConsumeNum)
        {
            return false;
        }
        //师门薪火
        if (GameManager.gameManager.PlayerDataPool.MasterInfo.MasterTorch < masterskill.MinMasterTorch)
        {
            return false;
        }
        //等级
        if (GameManager.gameManager.PlayerDataPool.MainPlayerBaseAttr.Level < masterskill.MinLevel)
        {
            return false;
        }
        if (Singleton<ObjManager>.GetInstance().MainPlayer != null)
        {
            if (Singleton<ObjManager>.GetInstance().MainPlayer.MasterSkillCanUse(nCurSkillId) == false)
            {
                return false;
            }
        }

        return true;
    }
    string GetTypeDesc(SKILLDELANDGAINTYPE nType,int nValue)
    {
        string strInfo = StrDictionary.GetClientDictionaryString("#{1793}");
        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (_mainPlayer != null)
        {
            switch (nType)
            {
                case SKILLDELANDGAINTYPE.HPTYPE_VALUE: //HP的数值
                    strInfo =StrDictionary.GetClientDictionaryString("#{1798}",nValue);;
                    break;
                case SKILLDELANDGAINTYPE.HPTYPE_RATE: //HP的百分比
                  strInfo =StrDictionary.GetClientDictionaryString("#{1799}",nValue);;
                    break;
                case SKILLDELANDGAINTYPE.MPTYPE_VALUE: //MP数值
                    strInfo = StrDictionary.GetClientDictionaryString("#{1804}", nValue);
                    break;
                case SKILLDELANDGAINTYPE.MPTYPE_RATE: //MP百分比
                    strInfo = StrDictionary.GetClientDictionaryString("#{1805}", nValue);
                    break;
                case SKILLDELANDGAINTYPE.XPTYPE_VALUE: //XP数值
                    strInfo = StrDictionary.GetClientDictionaryString("#{1806}", nValue);
                    break;
                case SKILLDELANDGAINTYPE.XPTYPE_RATE: //XP百分比
                    strInfo = StrDictionary.GetClientDictionaryString("#{1807}", nValue);
                    break;
                default:
                    break;
            }
        }
        return strInfo;
    }
    void ClickLevelUp()
    {
        // 新手指引
        if (m_NewPlayerGuide_Step == 2)
        {
            NewPlayerGuidLogic.CloseWindow();
            m_NewPlayerGuide_Step = -1;
        }

        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (_mainPlayer == null)
        {
            return ;
        }
        if (m_nSelectSkillIndex <0 || m_nSelectSkillIndex >=_mainPlayer.OwnSkillInfo.Length)
        {
            return;
        }
        int nCurSelSkillId =_mainPlayer.OwnSkillInfo[m_nSelectSkillIndex].SkillId;
        if (IsCanLevelUpSkill(nCurSelSkillId) || IsCanLevelUpMasterSkill(nCurSelSkillId))
        {
            //发包
            CG_ASK_LEVELUPSKILL packet = (CG_ASK_LEVELUPSKILL)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_LEVELUPSKILL);
            packet.SetSkillId(nCurSelSkillId);
            packet.SendPacket();
        }
    }

    void ClickLeftSkillInfo()
    {
        m_NextSkillDec.SetActive(false);
        m_CurSkillDec.SetActive(true);
        m_LeftSkillDescBt.SetActive(false);
        m_RightSkillDescBt.SetActive(true);
    }
    void ClickRightSkillInfo()
    {
        m_CurSkillDec.SetActive(false);
        m_NextSkillDec.SetActive(true);
        m_LeftSkillDescBt.SetActive(true);
        m_RightSkillDescBt.SetActive(false);
    }

    void Check_NewPlayerGuide()
    {
        if (MenuBarLogic.Instance() == null)
        {
            return;
        }
        int nIndex = MenuBarLogic.Instance().NewPlayerGuideIndex;
        if (nIndex == 7)
        {
            NewPlayerGuide(1);
            MenuBarLogic.Instance().NewPlayerGuideIndex = -1;
        }
    }

    public void NewPlayerGuide(int nIndex)
    {
        switch(nIndex)
        {
            case 1:
                break;
            case 2:
                NewPlayerGuidLogic.OpenWindow(m_LevelUpBt.gameObject, 180, 70, "", "right", 2, true, true);
                break;
        }
        m_NewPlayerGuide_Step = nIndex;
    }

    void SkillLevelUpGuide()
    {
        if (m_NewPlayerGuide_Step != 1)
        {
            return;
        }
        
        if (m_SkillBtGird == null)
        {
            return;
        }

        Transform gObjTransform = m_SkillBtGird.transform.FindChild("1002");
        if (gObjTransform == null)
        {
            return;
        }
        SkillRootButtonItemLogic item = gObjTransform.GetComponent<SkillRootButtonItemLogic>();
        if (item)
        {
            NewPlayerGuidLogic.OpenWindow(item.gameObject, 300, 150, "", "right", 2, true, true);
        }
    }
}
