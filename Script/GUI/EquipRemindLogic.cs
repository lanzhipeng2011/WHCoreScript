using UnityEngine;
using System.Collections;
using Games.Item;
using System.Collections.Generic;
using GCGame.Table;
using Games.GlobeDefine;
using System;

public class EquipRemindLogic : MonoBehaviour {

    private static EquipRemindLogic m_Instance = null;
    public static EquipRemindLogic Instance()
    {
        return m_Instance;
    }
    
    public GameObject m_EquipRemind;
    public UISprite m_EquipIconSprite;
    public UISprite m_QualitySprite;

    List<GameItem> m_EquipBuffer = new List<GameItem>();
    const int EquipBufferSize = 4;

    bool m_bOnShow = false;
    float m_fStartShowTime = 0.0f;
    const float ShowTime = 20.0f;

    bool m_bOnHide = true;
    float m_fStartHideTime = 0.0f;
    const float HideTime = 2.0f;

    // 新手指引
    public UIImageButton m_EquipButton;
    private int m_NewPlayerGuide_Step = -1;

    ///////////////////////////////////////////
    // 改为bundle后 需要记录的初始化相关 begin
    private static int m_OpenNewPlayerIndex = -1;
    public static int OpenNewPlayerIndex
    {
        get { return m_OpenNewPlayerIndex; }
        set { m_OpenNewPlayerIndex = value; }
    }
    // 改为bundle后 需要记录的初始化相关 end
    ///////////////////////////////////////////

    void Awake()
    {
        m_Instance = this;
    }

	// Use this for initialization
	void Start () {
        if (ItemRemindLogic.Instance() != null)
        {
            ItemRemindLogic.Instance().HandleEquipRemind(false);
        } 
	}
	
	// Update is called once per frame
	void Update () {
        ShowRemind();
	}

    void OnDestroy()
    {
        m_Instance = this;
    }

    public static void InitEquipInfo(GameItem newEquip)
    {
        if (newEquip == null || !newEquip.IsValid())
        {
            return;
        }

        // 加入挂机判读
        if (Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            if (Singleton<ObjManager>.GetInstance().MainPlayer.IsAutoSellItem(newEquip.DataID))
            {
                return;
            }
        }
        List<object> initParams = new List<object>();
        initParams.Add(newEquip);
		if(newEquip.DataID == 59046)
		{
			UIManager.ShowUI(UIInfo.MountRemindRoot, EquipRemindLogic.ShowUIOver, initParams);        
		}else{
			UIManager.ShowUI(UIInfo.EquipRemindRoot, EquipRemindLogic.ShowUIOver, initParams);        
		}

    }

    static void ShowUIOver(bool bSuccess, object param)
    {
        if (bSuccess)
        {
            List<object> initParams = param as List<object>;
            if (EquipRemindLogic.Instance() != null)
            {
                EquipRemindLogic.Instance().Init(initParams[0] as GameItem);
                if (m_OpenNewPlayerIndex != -1)
                {
                    EquipRemindLogic.Instance().NewPlayerGuide(m_OpenNewPlayerIndex);
                    m_OpenNewPlayerIndex = -1;
                }
            }
        }        
    }

    void Init(GameItem newEquip)
    {
        if (newEquip == null || !newEquip.IsValid())
        {
            return;
        }

        // 是否正在显示提示
        if (m_EquipBuffer.Count == 0)
        {
            m_EquipBuffer.Add(newEquip);
            ShowRemind();
            return;
        }
        // 如果正在显示 检测是否可以进缓冲池
        else
        {
            if (m_EquipBuffer.Count <= EquipBufferSize)
            {
                for (int index = 0; index < m_EquipBuffer.Count; index++)
                {
                    GameItem equip = m_EquipBuffer[index];
                    if (equip == null || !equip.IsValid())
                    {
                        return;
                    }
                    if (newEquip.GetEquipSlotIndex() == equip.GetEquipSlotIndex())
                    {
                        if (newEquip.GetCombatValue_NoStarEnchance() > equip.GetCombatValue_NoStarEnchance())
                        {
                            m_EquipBuffer.Remove(equip);
                            m_EquipBuffer.Add(newEquip);
                        }
                        return;
                    }
                    if (newEquip.GetEquipSlotIndex() != equip.GetEquipSlotIndex() && index == m_EquipBuffer.Count - 1)
                    {
                        m_EquipBuffer.Add(newEquip);
                    }
                }
            }
        }
    }

    void ShowRemind()
    {
        if (m_EquipBuffer.Count > 0)
        {
            // 未处在显示10秒和隐藏2秒状态 且自身需要显示
            if (!m_EquipRemind.activeSelf && !m_bOnShow && !m_bOnHide)
            {
                //使用物品提醒在的时候 暂时不显示
                if (UseItemRemindLogic.Instance() == null ||
                    (UseItemRemindLogic.Instance() != null && UseItemRemindLogic.Instance().m_ItemRemind.activeInHierarchy ==false))
                {
                    m_fStartShowTime = Time.fixedTime;
                    m_EquipRemind.SetActive(true);
                    if (ItemRemindLogic.Instance() != null)
                    {
                        ItemRemindLogic.Instance().HandleEquipRemind(true);
                    }

                    if (m_EquipBuffer[0] == null || !m_EquipBuffer[0].IsValid())
                    {
                        return;
                    }

                    Tab_CommonItem tabItem = TableManager.GetCommonItemByID(m_EquipBuffer[0].DataID, 0);
                    if (tabItem != null)
                    {
                        m_EquipIconSprite.spriteName = tabItem.Icon;
                        m_QualitySprite.spriteName = GlobeVar.QualityColorGrid[tabItem.Quality - 1];
                        m_bOnShow = true;
                    }
                }
            }
            // 处在显示状态且时间已达到10秒
            if (Time.fixedTime - m_fStartShowTime >= ShowTime && m_bOnShow)
            {
                m_bOnShow = false;
                m_bOnHide = true;
                m_fStartShowTime = 0.0f;
                m_fStartHideTime = Time.fixedTime;
                m_EquipRemind.SetActive(false);
                if (ItemRemindLogic.Instance() != null)
                {
                    ItemRemindLogic.Instance().HandleEquipRemind(false);
                } 
                m_EquipBuffer.RemoveAt(0);
            }
            // 处在隐藏状态且时间已达到2秒
            if (Time.fixedTime - m_fStartHideTime >= HideTime && m_bOnHide)
            {
                m_bOnHide = false;
                m_fStartHideTime = 0.0f;
            }
        }
        else
        {
         //   UIManager.CloseUI(UIInfo.EquipRemindRoot);
        }
    }

    void OnEquipClick()
    {
        // 新手指引
        if (m_NewPlayerGuide_Step == 1)
        {
            NewPlayerGuidLogic.CloseWindow();
            m_NewPlayerGuide_Step = -1;
        }

        if (m_EquipBuffer.Count > 0 && null != m_EquipBuffer[0] && m_EquipBuffer[0].IsValid())
        {
            GameItem m_RealEquip = GameManager.gameManager.PlayerDataPool.BackPack.GetItemByGuid(m_EquipBuffer[0].Guid);
            if (m_RealEquip != null && m_RealEquip.IsValid())
            {
                if (m_RealEquip.BindFlag == false && m_RealEquip.GetBindType() != 0)
                {
                    MessageBoxLogic.OpenOKCancelBox(3028, 1000, OnEquipClick_OK, null);
                }
                else
                {
                    OnEquipClick_OK();
                }
			}else{
				CloseCurEquip();
			}
        }
    }

    void OnEquipClick_OK()
    {
        if (m_EquipBuffer.Count > 0 && null != m_EquipBuffer[0] && m_EquipBuffer[0].IsValid())
        {

			if(m_EquipBuffer[0].DataID == 59046)
			{
				CG_USE_ITEM useitem = (CG_USE_ITEM)PacketDistributed.CreatePacket(MessageID.PACKET_CG_USE_ITEM);
				useitem.SetItemguid(m_EquipBuffer[0].Guid);
				useitem.SendPacket();
			}else{
				GameManager.gameManager.SoundManager.PlaySoundEffect(144);
				
				CG_EQUIP_ITEM equipitem = (CG_EQUIP_ITEM)PacketDistributed.CreatePacket(MessageID.PACKET_CG_EQUIP_ITEM);
				equipitem.SetEquipguid(m_EquipBuffer[0].Guid);
				equipitem.SetIsEquipRemind(1);
				equipitem.SendPacket();
			}
            CloseCurEquip();
        }
    }

    public void CloseCurEquip()
    {
        m_bOnShow = false;
        m_bOnHide = false;
        m_fStartShowTime = 0.0f;
        m_fStartHideTime = 0.0f;
        m_EquipBuffer.RemoveAt(0);
        m_EquipRemind.SetActive(false);

        if (ItemRemindLogic.Instance() != null)
        {
            ItemRemindLogic.Instance().HandleEquipRemind(false);
        } 
    }

    public void NewPlayerGuide(int nIndex)
    {
        if (NewPlayerGuidLogic.Instance() != null
            || NewItemGetLogic.Instance() != null)
        {
            return;
        }
        // 新手指引已打开
        if (NewPlayerGuidLogic.IsOpenFlag == false)
        {
            switch (nIndex)
            {
                case 1:
                    NewPlayerGuidLogic.OpenWindow(m_EquipButton.gameObject, 180, 60, "", "left", 2, true, true);
                    break;
            }
            m_NewPlayerGuide_Step = nIndex;
        }
    }

    public UInt64 GetCurEquipGuid()
    {
        if (m_EquipBuffer[0] != null && m_EquipBuffer[0].IsValid())
        {
            return m_EquipBuffer[0].Guid;
        }
        return GlobeVar.INVALID_GUID;
    }
}
