using UnityEngine;
using System.Collections;
using Games.SwordsMan;
using Module.Log;
using GCGame.Table;
using Games.Item;
public class SwordsManToolTipsLogic : MonoBehaviour{

    public enum SwordsMan_ShowType
    {
        UnEquiped = 1,      //未装备的tooltips
        Equiped,            //已装备的tooltips
        ScoreShop,
        ChatLink,
    }

    public UILabel m_LableName;
    public UISprite m_SpriteIcon;
    public UISprite m_SpriteQuality;
    public UILabel m_LabelLevelValue;
    public UILabel m_LabelDesc;

    public GameObject m_CurEffectTitle;
    public GameObject m_NextEffectTitle;
    public GameObject m_ExpTitle;

    public UILabel[] m_LabelCurEffect;
    public UILabel[] m_LabelNextEffect;

    public UILabel m_LabelExp;
    public UISlider m_SliderExp;
    public GameObject m_EquipButton;
    public GameObject m_UnEquipButton;
    public GameObject m_LevelUpButton;
    public GameObject m_LockButton;
    public GameObject m_UnLockButton;

    public GameObject m_CurEffectGrid;
    public GameObject m_NextEffectGrid;

    private static SwordsManToolTipsLogic m_Instance = null;
    public static SwordsManToolTipsLogic Instance()
    {
        return m_Instance;
    }

    private static SwordsMan m_curItem;
    private static SwordsMan_ShowType m_curType;
    public static void ShowSwordsManTooltip(SwordsMan oSwordsMan, SwordsMan_ShowType ShowType)
    {
        m_curItem = oSwordsMan;
        m_curType = ShowType;
        UIManager.ShowUI(UIInfo.SwordsManTooltipsRoot, SwordsManToolTipsLogic.OnShowEquipTip);
    }

    private static void OnShowEquipTip(bool bSuccess, object param)
    {
        if (!bSuccess)
        {
            LogModule.ErrorLog("load equiptooltip error");
            return;
        }

        if (null != SwordsManToolTipsLogic.Instance())
            SwordsManToolTipsLogic.Instance().ShowTooltips(m_curItem, m_curType);
    }

    void Awake()
    {
        m_Instance = this;
    }

    void OnDestroy()
    {
        m_Instance = null;
    }

    private SwordsMan m_SwordsMan;
    private SwordsMan_ShowType m_ShowType;
    private void ShowTooltips(SwordsMan oSwordsMan, SwordsMan_ShowType ShowType)
    {
        if (null == oSwordsMan)
        {
            LogModule.ErrorLog("ShowTooltips::ShowTooltips is null");
            return;
        }
        m_SwordsMan = oSwordsMan;
        m_ShowType = ShowType;
        HideAllButton();
        ShowSwordsManIcon();
        ShowSwordsManName();
        ShowSwordsManLevel();
        ShowSwordsManExp();
        ShowSwordsManEffect();
        ShowSwordsManButton();
    }

    /// <summary>
    /// 
    /// </summary>
    void HideAllButton()
    {
        if (m_LockButton != null)
        {
            m_LockButton.SetActive(false);
        }
        if (m_UnLockButton != null)
        {
            m_UnLockButton.SetActive(false);
        }
        if (m_LevelUpButton != null)
        {
            m_LevelUpButton.SetActive(false);
        }
        if (m_EquipButton != null)
        {
            m_EquipButton.SetActive(false);
        }
        if (m_UnEquipButton != null)
        {
            m_UnEquipButton.SetActive(false);
        }
    }

    /// <summary>
    /// 侠客图标显示
    /// </summary>
    void ShowSwordsManIcon()
    {
        if (m_SwordsMan == null)
        {
            return;
        }
        if (m_SpriteIcon != null)
        {
            m_SpriteIcon.spriteName = m_SwordsMan.GetIcon();
        }
        if (m_SpriteQuality != null)
        {
            m_SpriteQuality.spriteName = SwordsMan.GetQualitySpriteName((SwordsMan.SWORDSMANQUALITY)m_SwordsMan.Quality); ;
        }
    }

    /// <summary>
    /// 名字
    /// </summary>
    void ShowSwordsManName()
    {
        if (m_SwordsMan == null)
        {
            return;
        }
        if (m_LableName != null)
        {
            m_LableName.text = m_SwordsMan.Name;
        }
    }

    /// <summary>
    /// 等级描述
    /// </summary>
    void ShowSwordsManLevel()
    {
        if (m_SwordsMan == null)
        {
            return;
        }
        if (m_LabelLevelValue != null)
        {
            m_LabelLevelValue.text = m_SwordsMan.Level.ToString(); ;
        }
        if (m_LabelDesc != null)
        {
            m_LabelDesc.text = m_SwordsMan.GetTips();
        }
    }

    /// <summary>
    /// 侠客属性加成效果
    /// </summary>
    void ShowSwordsManEffect()
    {
        if (m_SwordsMan == null)
        {
            LogModule.ErrorLog("ShowSwordsManEffect::m_SwordsMan is null");
            return;
        }
        Tab_SwordsManAttr pAttrTable = TableManager.GetSwordsManAttrByID(m_SwordsMan.DataId, 0);
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

        int nNextEffectIndex = 0;
        int nCurEffectIndex = 0;
        for (int i = 0; i < pAttrTable.getAddAttrTypeCount(); i++)
        {
            COMBATATTE nAttrType = (COMBATATTE)pAttrTable.GetAddAttrTypebyIndex(i);
            int nAttrValue = m_SwordsMan.GetComBatAttrById(nAttrType);
            int nNextLevelAttrValue = m_SwordsMan.GetNextLevelComBatAttrById(nAttrType);
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

        if (m_ShowType == SwordsMan_ShowType.Equiped || m_ShowType == SwordsMan_ShowType.UnEquiped)
        {
            if (m_SwordsMan.IsFullLevel())
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
        else if (m_ShowType == SwordsMan_ShowType.ScoreShop || m_ShowType == SwordsMan_ShowType.ChatLink)
        {         
            if (m_NextEffectTitle != null)
            {
                m_NextEffectTitle.SetActive(false);
            }         
            if (m_NextEffectGrid != null)
            {
                m_NextEffectGrid.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 显示及等级经验信息
    /// </summary>
    void ShowSwordsManExp()
    {
        if (m_ShowType == SwordsMan_ShowType.Equiped || m_ShowType == SwordsMan_ShowType.UnEquiped || m_ShowType == SwordsMan_ShowType.ChatLink)
        {
            if (m_SwordsMan.IsFullLevel())
            {
                if (m_ExpTitle != null)
                {
                    m_ExpTitle.SetActive(false);
                }
                if (m_LabelExp != null)
                {
                    m_LabelExp.gameObject.SetActive(false);
                }
                if (m_SliderExp != null)
                {
                    m_SliderExp.gameObject.SetActive(false);
                }
            }
            else
            {
                if (m_ExpTitle != null)
                {
                    m_ExpTitle.SetActive(true);
                }
                if (m_LabelExp != null)
                {
                    m_LabelExp.gameObject.SetActive(true);
                    m_LabelExp.text = m_SwordsMan.Exp.ToString() + "/" + m_SwordsMan.MaxExp.ToString();
                }
                if (m_SliderExp != null)
                {
                    m_SliderExp.gameObject.SetActive(true);
                    int nMaxExp = m_SwordsMan.MaxExp;
                    if (nMaxExp > 0)
                    {
                        float fSlider = (float)m_SwordsMan.Exp / (float)nMaxExp;
                        m_SliderExp.value = fSlider;

						//==========
						if(m_SliderExp.value == 0)
						{
							m_SliderExp.gameObject.SetActive(false);
						}else{
							m_SliderExp.gameObject.SetActive(true);
						}
                    }
                }
            }
        }
        else if (m_ShowType == SwordsMan_ShowType.ScoreShop )
        {
            if (m_ExpTitle != null)
            {
                m_ExpTitle.SetActive(false);
            }
            if (m_LabelExp != null)
            {
                m_LabelExp.gameObject.SetActive(false);
            }
            if (m_SliderExp != null)
            {
                m_SliderExp.gameObject.SetActive(false);
            }
        }
    }

    void ShowSwordsManButton()
    {
        if (m_ShowType == SwordsMan_ShowType.Equiped)
        {
            if (m_UnEquipButton != null)
            {
                m_UnEquipButton.SetActive(true);
            }
            if (m_SwordsMan.Locked)
            {
                if (m_UnLockButton != null)
                {
                    m_UnLockButton.SetActive(true);
                }
            }
            else
            {
                if (m_LockButton != null)
                {
                    m_LockButton.SetActive(true);
                }
            }

            if (m_LevelUpButton != null)
            {
                m_LevelUpButton.SetActive(true);
                UIImageButton iamgebutton = m_LevelUpButton.GetComponent<UIImageButton>();
                if (iamgebutton != null)
                {
                    if (m_SwordsMan.IsFullLevel())
                    {
                        m_LevelUpButton.GetComponent<UIImageButton>().isEnabled = false;
                    }
                    else
                    {
                        m_LevelUpButton.GetComponent<UIImageButton>().isEnabled = true;
                    }
                }
            }
        }
        else if (m_ShowType == SwordsMan_ShowType.UnEquiped)
        {
            if (m_EquipButton != null)
            {
                m_EquipButton.SetActive(true);
            }
            if (m_SwordsMan.Locked)
            {
                if (m_UnLockButton != null)
                {
                    m_UnLockButton.SetActive(true);
                }
            }
            else
            {
                if (m_LockButton != null)
                {
                    m_LockButton.SetActive(true);
                }
            }

            if (m_LevelUpButton != null)
            {
                m_LevelUpButton.SetActive(true);
                UIImageButton iamgebutton = m_LevelUpButton.GetComponent<UIImageButton>();
                if (iamgebutton != null)
                {
                    if (m_SwordsMan.IsFullLevel())
                    {
                        m_LevelUpButton.GetComponent<UIImageButton>().isEnabled = false;
                    }
                    else
                    {
                        m_LevelUpButton.GetComponent<UIImageButton>().isEnabled = true;
                    }
                }
            }
        }
        else if (m_ShowType == SwordsMan_ShowType.ScoreShop)
        {

        }
    }

    /// <summary>
    /// 装备侠客
    /// </summary>
    void OnEquipSwordsMan()
    {
        int nSiize = GameManager.gameManager.PlayerDataPool.SwordsManEquipPack.GetEmptyContainerSize();
        if (nSiize <= 0)
        {
            MessageBoxLogic.OpenOKBox(2556,1000);
            return;
        }
        if (null == m_SwordsMan)
        {
            LogModule.ErrorLog("OnEquipSwordsMan::m_SwordsMan is null");
            return;
        }
        CG_EQUIP_SWORDSMAN packet = (CG_EQUIP_SWORDSMAN)PacketDistributed.CreatePacket(MessageID.PACKET_CG_EQUIP_SWORDSMAN);
        packet.Swordsmanguid = m_SwordsMan.Guid;
        packet.SendPacket();
        CloseWindow();
    }

    /// <summary>
    /// 卸载侠客
    /// </summary>
    void OnUnEquipSwordsMan()
    {
        if (null == m_SwordsMan)
        {
            LogModule.ErrorLog("OnUnEquipSwordsMan::m_SwordsMan is null");
            return;
        }
        int nSize = GameManager.gameManager.PlayerDataPool.SwordsManBackPack.GetEmptyContainerSize();
        if (nSize <= 0)
        {
            MessageBoxLogic.OpenOKBox(2488,1000);
            return;
        }
        //MessageBoxLogic.OpenOKCancelBox(2487,1000, UnEquipSwordsManOK);
        UnEquipSwordsManOK();
    }

    /// <summary>
    /// 
    /// </summary>
    void UnEquipSwordsManOK()
    {
        if (m_SwordsMan == null)
        {
            return;
        }
        CG_UNEQUIP_SWORDSMAN packet = (CG_UNEQUIP_SWORDSMAN)PacketDistributed.CreatePacket(MessageID.PACKET_CG_UNEQUIP_SWORDSMAN);
        packet.Swordsmanguid = m_SwordsMan.Guid;
        packet.SendPacket();
        CloseWindow();
    }

    /// <summary>
    /// 吞噬升级
    /// </summary>
    void OnLevelUpSwordsMan()
    {
        if (m_SwordsMan == null)
        {
            return;
        }
        if (m_SwordsMan.DataId == 52 || m_SwordsMan.DataId == 36)
        {
			GUIData.AddNotifyData2Client(false,"#{2558}");
            return;
        }
        if (m_SwordsMan.IsFullLevel())
        {
			GUIData.AddNotifyData2Client(false,"#{2492}");
            return;
        }
        UIManager.CloseUI(UIInfo.SwordsManTooltipsRoot);
        if (SwordsManController.Instance() != null)
        {
            UIManager.CloseUI(UIInfo.SwordsManRoot);
        }
        SwordsManLevelupController.OpenWindow(m_SwordsMan, m_ShowType);
    }

    /// <summary>
    /// 锁定
    /// </summary>
    void OnLockSwordsMan()
    {
        if (m_ShowType == SwordsMan_ShowType.Equiped)
        {
            CG_LOCK_SWORDSMAN packet = (CG_LOCK_SWORDSMAN)PacketDistributed.CreatePacket(MessageID.PACKET_CG_LOCK_SWORDSMAN);
            packet.Swordsmanguid = m_SwordsMan.Guid;
            packet.Packtype = (int)SwordsManContainer.PACK_TYPE.TYPE_EQUIPPACK;
            packet.SendPacket();
            CloseWindow();
        }
        else if (m_ShowType == SwordsMan_ShowType.UnEquiped)
        {
            CG_LOCK_SWORDSMAN packet = (CG_LOCK_SWORDSMAN)PacketDistributed.CreatePacket(MessageID.PACKET_CG_LOCK_SWORDSMAN);
            packet.Swordsmanguid = m_SwordsMan.Guid;
            packet.Packtype = (int)SwordsManContainer.PACK_TYPE.TYPE_BACKPACK;
            packet.SendPacket();
            CloseWindow();
        }
    }
    /// <summary>
    /// 解锁
    /// </summary>
    void OnUnLockSwordsMan()
    {
        if (m_ShowType == SwordsMan_ShowType.Equiped)
        {
            CG_UNLOCK_SWORDSMAN packet = (CG_UNLOCK_SWORDSMAN)PacketDistributed.CreatePacket(MessageID.PACKET_CG_UNLOCK_SWORDSMAN);
            packet.Swordsmanguid = m_SwordsMan.Guid;
            packet.Packtype = (int)SwordsManContainer.PACK_TYPE.TYPE_EQUIPPACK;
            packet.SendPacket();
            CloseWindow();
        }
        else if (m_ShowType == SwordsMan_ShowType.UnEquiped)
        {
            CG_UNLOCK_SWORDSMAN packet = (CG_UNLOCK_SWORDSMAN)PacketDistributed.CreatePacket(MessageID.PACKET_CG_UNLOCK_SWORDSMAN);
            packet.Swordsmanguid = m_SwordsMan.Guid;
            packet.Packtype = (int)SwordsManContainer.PACK_TYPE.TYPE_BACKPACK;
            packet.SendPacket();
            CloseWindow();
        }
    }

    /// <summary>
    /// 点击关闭
    /// </summary>
    void OnClickClose()
    {
        CloseWindow();
    }

    /// <summary>
    /// 关闭界面
    /// </summary>
    void CloseWindow()
    {
        UIManager.CloseUI(UIInfo.SwordsManTooltipsRoot);
    }
}
