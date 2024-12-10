using UnityEngine;
using System.Collections;
using Games.SwordsMan;
using Module.Log;
using System.Collections.Generic;
using GCGame;
using System;
using Games.GlobeDefine;
using GCGame.Table;
using Games.Item;
/********************************************************************
    created:	2014-06-25
    filename: 	SwordsManLevelupController.cs
    author:		grx
    purpose:	侠客吞噬
*********************************************************************/
public class SwordsManLevelupController : MonoBehaviour {

    public GameObject m_SwordsManGrid;
    public Transform m_GridTranForm;

    public UILabel m_LableName;
    public UILabel m_LabelLevelValue;
    public UILabel m_LabelDesc;
    public UILabel[] m_LabelCurEffect;
    public UILabel[] m_LabelNextEffect;
    public UILabel m_LabelExp;
    public UISlider m_SliderExp;
    public UISprite m_SpriteIcon;
    public UISprite m_SpriteQuality;
    public UIPopupList m_SwordsMan_ChooseList;

    public GameObject m_CurEffectTitle;
    public GameObject m_NextEffectTitle;
    public GameObject m_CurEffectGrid;
    public GameObject m_NextEffectGrid;

    public GameObject m_ExpTitle;
    public UILabel m_LabelTips;


    private SwordsManContainer.PACK_TYPE m_PackType = SwordsManContainer.PACK_TYPE.TYPE_INVALID;
    private SwordsMan.SWORDSMANQUALITY m_curChooseQuality;
    private bool m_bClearAllSelect = false;
    private int m_nEatExp = 0;
    private UInt64 m_SwordsManGuid;

    static private SwordsMan m_sSwordsMan = null;
    static private SwordsManToolTipsLogic.SwordsMan_ShowType m_sShowType;

    public static void OpenWindow(SwordsMan oSwordsMan, SwordsManToolTipsLogic.SwordsMan_ShowType ShowType)
    {
        m_sShowType = ShowType;
        m_sSwordsMan = oSwordsMan;
        UIManager.ShowUI(UIInfo.SwordsManLevelUpRoot, OnOpenWindow, null);
    }

    public static void OnOpenWindow(bool bSuccess, object param)
    {
        if (bSuccess && SwordsManLevelupController.Instance() != null)
        {
            SwordsManLevelupController.Instance().SetLevelUpSwordsMan(m_sSwordsMan, m_sShowType);
        }
    }

    public static SwordsManLevelupController Instance()
    {
        return m_Instance;
    }

    static private SwordsManLevelupController m_Instance = null;

    //void Awake()
    //{
    //    m_Instance = this;
    //}

    //void OnDestroy()
    //{
    //    m_Instance = null;
    //}

    void OnEnable()
    {
        m_Instance = this;
    }

    void OnDisable()
    {
        m_Instance = null;
    }

    /// <summary>
    /// 
    /// </summary>
    public void UpdateSwordsManBackPack()
    {
        if (null == m_SwordsManGrid)
        {
            LogModule.ErrorLog("UpdateSwordsManBackPack m_SwordsManGrid is null");
            return;
        }
        UIManager.LoadItem(UIInfo.SwordsManItem, OnLoadSwordsManItem);
    }

    /// <summary>
    /// 更新侠客信息
    /// </summary>
    void OnLoadSwordsManItem(GameObject resObj, object param)
    {
        if (null == m_SwordsManGrid)
        {
            LogModule.ErrorLog("OnLoadSwordsManItem m_SwordsManGrid is null");
            return;
        }
        Utils.CleanGrid(m_SwordsManGrid.gameObject);

        SwordsManContainer oSwordsManBackContainer = GameManager.gameManager.PlayerDataPool.GetSwordsManContainer(SwordsManContainer.PACK_TYPE.TYPE_BACKPACK);
        if (null == oSwordsManBackContainer)
        {
            LogModule.ErrorLog("OnLoadSwordsManItem::oSwordsManBackContainer is null");
            return;
        }
        SwordsManContainer Container = GameManager.gameManager.PlayerDataPool.GetSwordsManContainer(m_PackType);
        if (null == Container)
        {
            LogModule.ErrorLog("OnLoadSwordsManItem::Container is null");
            return;
        }
        SwordsMan CurSwordsMan = Container.GetSwordsManByGuid(m_SwordsManGuid);
        if (CurSwordsMan == null)
        {
            return;
        }
        List<SwordsMan> itemlist = SwordsManTool.ItemFilter(oSwordsManBackContainer);
        itemlist = SwordsManTool.ItemFilter(itemlist, (int)m_curChooseQuality);
        m_nEatExp = 0;
        for (int i = 0; i < itemlist.Count; i++)
        {
            SwordsMan oSwordsMan = itemlist[i];
            if (null == oSwordsMan)
            {
                LogModule.ErrorLog("OnLoadSwordsManItem::oSwordsMan is null");
                break;
            }
            if (false == oSwordsMan.IsValid())
            {
                continue;
            }
            SwordsManItem oSwordsManItem = SwordsManItem.CreateItem(m_SwordsManGrid, resObj, i.ToString(), this);
            if (null == oSwordsManItem)
            {
                LogModule.ErrorLog("OnLoadSwordsManItem::oSwordsManItem is null");
                break;
            }
            oSwordsManItem.SetData(oSwordsMan);
            if (oSwordsMan.Guid == CurSwordsMan.Guid)
            {
                continue;
            }
            if (oSwordsMan.Quality > CurSwordsMan.Quality)
            {
               continue;
            }
            if (oSwordsMan.Locked)
            {
                continue;
            }          
            oSwordsManItem.OnSelectSwordsMan();
            m_nEatExp += oSwordsManItem.m_oSwordsMan.GetEatExp();
        }
        m_SwordsManGrid.GetComponent<UIGrid>().repositionNow = true;
        ShowSwordsManExp();
    }

    /// <summary>
    /// 设置升级侠客信息
    /// </summary>
    void SetLevelUpSwordsMan(SwordsMan oSwordsMan, SwordsManToolTipsLogic.SwordsMan_ShowType ShowType)
    {
        m_curChooseQuality = SwordsMan.SWORDSMANQUALITY.ORANGE;
        m_bClearAllSelect = false;
        m_SwordsManGuid = oSwordsMan.Guid;

        if (null == oSwordsMan)
        {
            LogModule.ErrorLog("SetLevelUpSwordsMan::oSwordsMan is null");
            return;
        }
        if (SwordsManToolTipsLogic.SwordsMan_ShowType.Equiped == ShowType)
        {
            m_PackType = SwordsManContainer.PACK_TYPE.TYPE_EQUIPPACK;
        }
        else if (SwordsManToolTipsLogic.SwordsMan_ShowType.UnEquiped == ShowType)
        {
            m_PackType = SwordsManContainer.PACK_TYPE.TYPE_BACKPACK;
        }
        else
        {
            LogModule.ErrorLog("SetLevelUpSwordsMan::ShowType invalid");
            return;
        }
        if (m_LabelTips != null)
        {
            m_LabelTips.text = StrDictionary.GetClientDictionaryString("#{2719}");
        }
        UpdateSwordsManBackPack();
        UpdateSwordsManInfo();
    }

    /// <summary>
    /// 一键吞噬
    /// </summary>
    void OnAkeyLevelup()
    {
        SwordsManContainer Container = GameManager.gameManager.PlayerDataPool.GetSwordsManContainer(m_PackType);
        if (Container == null)
        {
            return;
        }
        SwordsMan oSwordsMan = Container.GetSwordsManByGuid(m_SwordsManGuid);
        if (oSwordsMan == null)
        {
            return;
        }

        if (null == m_GridTranForm)
        {
            LogModule.ErrorLog("OnAkeyLevelup::m_GridTranForm is null");
            return;
        }
        List<ulong> guidlist = new List<ulong>();
        for (int i = 0; i < m_GridTranForm.childCount; i++)
        {
            SwordsManItem item = m_GridTranForm.GetChild(i).GetComponent<SwordsManItem>();
            if (item.m_Status == SwordsManItem.Status.STATUS_CHOOSED && item.m_oSwordsMan != null)
            {
                guidlist.Add(item.m_oSwordsMan.Guid);
            }
        }
        if (guidlist.Count <= 0)
        {
            MessageBoxLogic.OpenOKBox(2675, 1000);
            return;
        }
        else
        {
            CG_SWORDSMAN_LEVELUP packet = (CG_SWORDSMAN_LEVELUP)PacketDistributed.CreatePacket(MessageID.PACKET_CG_SWORDSMAN_LEVELUP);
            packet.Swordsmanid = oSwordsMan.Guid;
            packet.Eatallswordsman = 1;
			packet.Packtype =(uint)m_PackType;
            packet.FilterQuality = (int)m_curChooseQuality;
            for (int i = 0; i < guidlist.Count; ++i)
            {
                packet.AddEatguid(guidlist[i]);
            }
            packet.SendPacket();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    public void OnSelectSwordsManItem(SwordsManItem item)
    {
        SwordsManContainer Container = GameManager.gameManager.PlayerDataPool.GetSwordsManContainer(m_PackType);
        if (Container == null)
        {
            return;
        }
        SwordsMan oSwordsMan = Container.GetSwordsManByGuid(m_SwordsManGuid);
        if (oSwordsMan == null)
        {
            return;
        }

        if (item == null)
        {
            return;
        }
        if (item.m_oSwordsMan == null)
        {
            return;
        }
        if (item.m_oSwordsMan.Quality > oSwordsMan.Quality)
        {
			GUIData.AddNotifyData2Client(false,"#{2497}");
            return;
        }
        if (item.m_oSwordsMan.Locked)
        {
            return;
        }
        if (item.m_oSwordsMan.Guid != oSwordsMan.Guid)
        {
            item.OnSelectSwordsMan();
            if (item.IsSelect())
            {
                m_nEatExp += item.m_oSwordsMan.GetEatExp();
            }
            else
            {
                m_nEatExp -= item.m_oSwordsMan.GetEatExp();
            }
            ShowSwordsManExp();
        }
        else
        {
			GUIData.AddNotifyData2Client(false,"#{2557}");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void OnClickClose()
    {
        UIManager.CloseUI(UIInfo.SwordsManLevelUpRoot);
        UIManager.ShowUI(UIInfo.SwordsManRoot);
    }

    //强化 智能选择 弹出列表选择
    public void OnXiakeChoose()
    {
        if (null == m_SwordsMan_ChooseList)
        {
            return;
        }
        if (null == m_GridTranForm)
        {
            LogModule.ErrorLog("OnXiakeChoose::m_GridTranForm is null");
            return;
        }
        m_bClearAllSelect = false;
        m_nEatExp = 0;
        if (m_SwordsMan_ChooseList.value == StrDictionary.GetClientDictionaryString("#{1208}"))
        {
            m_curChooseQuality = SwordsMan.SWORDSMANQUALITY.WHITE;
        }
        else if (m_SwordsMan_ChooseList.value == StrDictionary.GetClientDictionaryString("#{1209}"))
        {
            m_curChooseQuality = SwordsMan.SWORDSMANQUALITY.GREEN;
        }
        else if (m_SwordsMan_ChooseList.value == StrDictionary.GetClientDictionaryString("#{1210}"))
        {
            m_curChooseQuality = SwordsMan.SWORDSMANQUALITY.BLUE;
        }
        else if (m_SwordsMan_ChooseList.value == StrDictionary.GetClientDictionaryString("#{1211}"))
        {
            m_curChooseQuality = SwordsMan.SWORDSMANQUALITY.PURPLE;
        }
        else if (m_SwordsMan_ChooseList.value == StrDictionary.GetClientDictionaryString("#{2716}"))
        {
            //全部
            m_curChooseQuality = SwordsMan.SWORDSMANQUALITY.ORANGE;
        }
        else
        {
            m_bClearAllSelect = true;
        }
        if (m_bClearAllSelect)
        {
            for (int i = 0; i < m_GridTranForm.childCount; i++)
            {
                SwordsManItem oItem = m_GridTranForm.GetChild(i).GetComponent<SwordsManItem>();
                if (oItem == null || oItem.m_oSwordsMan == null)
                {
                    continue;
                }
                if (oItem.IsSelect())
                {
                    oItem.OnSelectSwordsMan();
                }                
            }
        }
        else
        {
            UpdateSwordsManBackPack();
        }
        ShowSwordsManExp();

        //for (int i = 0; i < m_GridTranForm.childCount; i++)
        //{
        //    SwordsManItem oItem = m_GridTranForm.GetChild(i).GetComponent<SwordsManItem>();
        //    if (oItem == null || oItem.m_oSwordsMan == null)
        //    {
        //        continue;
        //    }
        //    if (oItem.m_oSwordsMan.Quality > (int)m_curChooseQuality)
        //    {
        //        oItem.gameObject.SetActive(false);
        //        continue;
        //    }
        //    oItem.gameObject.SetActive(true);
        //    if (m_bClearAllSelect)
        //    {
        //        if (oItem.IsSelect())
        //        {
        //            oItem.OnSelectSwordsMan();
        //        }
        //        continue;
        //    }

        //    if (oItem.m_oSwordsMan.Guid == m_oSwordsMan.Guid)
        //    {
        //        if (oItem.IsSelect())
        //        {
        //            oItem.OnSelectSwordsMan();
        //        }
        //        continue;
        //    }
        //    if (oItem.m_oSwordsMan.Locked)
        //    {
        //        if (oItem.IsSelect())
        //        {
        //            oItem.OnSelectSwordsMan();
        //        }
        //        continue;
        //    }
        //    if (oItem.m_oSwordsMan.Quality > m_oSwordsMan.Quality)
        //    {
        //        if (oItem.IsSelect())
        //        {
        //            oItem.OnSelectSwordsMan();
        //        }
        //        continue;
        //    }
        //    if (oItem.IsSelect() == false)
        //    {
        //        oItem.OnSelectSwordsMan();
        //    }
        //    m_nEatExp += oItem.m_oSwordsMan.GetEatExp();
        //}
        //m_SwordsManGrid.GetComponent<UIGrid>().repositionNow = true;
        //ShowSwordsManExp();
    }

    public void UpdateSwordsManInfo()
    {
        if (null == m_LableName)
        {
            return;
        }
        if (null == m_LabelLevelValue)
        {
            return;
        }
        if (null == m_LabelDesc)
        {
            return;
        }
        SwordsManContainer Container = GameManager.gameManager.PlayerDataPool.GetSwordsManContainer(m_PackType);
        if (Container == null)
        {
            return;
        }
        SwordsMan oSwordsMan = Container.GetSwordsManByGuid(m_SwordsManGuid);
        if (oSwordsMan == null)
        {
            return;
        }

        m_LableName.text = oSwordsMan.Name;
        m_LabelLevelValue.text = oSwordsMan.Level.ToString();
        m_LabelDesc.text = oSwordsMan.GetTips();
        if (m_SpriteIcon != null)
        {
            m_SpriteIcon.spriteName = oSwordsMan.GetIcon();
        }
        if (m_SpriteQuality != null)
        {
            m_SpriteQuality.spriteName = SwordsMan.GetQualitySpriteName((SwordsMan.SWORDSMANQUALITY)oSwordsMan.Quality); ;
        }
        ShowSwordsManExp();
        ShowSwordsManEffect();
    }

    /// <summary>
    /// 
    /// </summary>
    void ShowSwordsManExp()
    {
        if (null == m_LabelExp)
        {
            return;
        }
        if (null == m_SliderExp)
        {
            return;
        }
        if (null == m_ExpTitle)
        {
            return;
        }
        SwordsManContainer Container = GameManager.gameManager.PlayerDataPool.GetSwordsManContainer(m_PackType);
        if (Container == null)
        {
            return;
        }
        SwordsMan oSwordsMan = Container.GetSwordsManByGuid(m_SwordsManGuid);
        if (oSwordsMan == null)
        {
            return;
        }
        if (oSwordsMan.IsFullLevel())
        {
            m_LabelExp.gameObject.SetActive(false);
            m_SliderExp.gameObject.SetActive(false);
            m_ExpTitle.SetActive(false);
            return;
        }
        m_LabelExp.gameObject.SetActive(true);
        m_SliderExp.gameObject.SetActive(true);
        m_ExpTitle.SetActive(true);

        if (m_nEatExp > 0)
        {
            m_LabelExp.text = string.Format("{0}+[33c8ff]{1}[-]/{2}", oSwordsMan.Exp, m_nEatExp, oSwordsMan.MaxExp);
        }
        else
        {
            m_LabelExp.text = string.Format("{0}/{1}", oSwordsMan.Exp, oSwordsMan.MaxExp);
        }

        int nMaxExp = oSwordsMan.MaxExp;
        if (nMaxExp > 0)
        {
            float fSlider = (float)oSwordsMan.Exp / (float)nMaxExp;
            m_SliderExp.value = fSlider;
        }
    }

    void ShowSwordsManEffect()
    {
        SwordsManContainer Container = GameManager.gameManager.PlayerDataPool.GetSwordsManContainer(m_PackType);
        if (Container == null)
        {
            return;
        }
        SwordsMan oSwordsMan = Container.GetSwordsManByGuid(m_SwordsManGuid);
        if (oSwordsMan == null)
        {
            LogModule.ErrorLog("ShowSwordsManEffect::oSwordsMan is null");
            return;
        }
        Tab_SwordsManAttr pAttrTable = TableManager.GetSwordsManAttrByID(oSwordsMan.DataId, 0);
        if (null == pAttrTable)
        {
            LogModule.ErrorLog("ShowSwordsManEffect::pAttrTable is null");
            return;
        }

        for (int i = 0; i < m_LabelCurEffect.Length; i++)
        {
            m_LabelCurEffect[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < m_LabelNextEffect.Length; i++)
        {
            m_LabelNextEffect[i].gameObject.SetActive(false);
        }
        int nCurEffectIndex = 0;
        int nNextEffectIndex = 0;
        for (int i = 0; i < pAttrTable.getAddAttrTypeCount(); i++)
        {
            COMBATATTE nAttrType = (COMBATATTE)pAttrTable.GetAddAttrTypebyIndex(i);
            int nAttrValue = oSwordsMan.GetComBatAttrById(nAttrType);
            int nNextLevelAttrValue = oSwordsMan.GetNextLevelComBatAttrById(nAttrType);
            if (nAttrValue > 0 && nCurEffectIndex < m_LabelCurEffect.Length && nCurEffectIndex >= 0)
            {
                m_LabelCurEffect[nCurEffectIndex].text = string.Format("{0}+{1}", ItemTool.ConvertAttrToString(nAttrType), nAttrValue.ToString());
                m_LabelCurEffect[nCurEffectIndex].gameObject.SetActive(true);
                ++nCurEffectIndex;
            }
            if (nNextLevelAttrValue > 0 && nNextEffectIndex < m_LabelNextEffect.Length && nNextEffectIndex >= 0)
            {
                m_LabelNextEffect[nNextEffectIndex].text = string.Format("{0}+{1}", ItemTool.ConvertAttrToString(nAttrType), nNextLevelAttrValue.ToString());
                m_LabelNextEffect[nNextEffectIndex].gameObject.SetActive(true);
                ++nNextEffectIndex;
            }
        }

        if (m_CurEffectGrid != null)
        {
            m_CurEffectGrid.SetActive(true);
        }
        if (m_CurEffectTitle != null)
        {
            m_CurEffectTitle.SetActive(true);
        }
        if (oSwordsMan.IsFullLevel())
        {
            if (m_NextEffectGrid != null)
            {
                m_NextEffectGrid.SetActive(false);
            }
            if (m_NextEffectTitle != null)
            {
                m_NextEffectTitle.SetActive(false);
            }
        }
        else
        {
            if (m_NextEffectGrid != null)
            {
                m_NextEffectGrid.SetActive(true);
            }
            if (m_NextEffectTitle != null)
            {
                m_NextEffectTitle.SetActive(true);
            }
        }
    }
}