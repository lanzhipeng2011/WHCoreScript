//********************************************************************
// 文件名: BackPackItemLogic.cs
// 描述: 背包界面物品格子UI逻辑
// 作者: TangYi
// 创建时间: 2013-12-25
//
// 修改历史: 100/300*0.7   100/140
//********************************************************************
using UnityEngine;
using System.Collections;
using Games.Item;
using GCGame.Table;
using System;
using Games.GlobeDefine;
using Module.Log;
public class BackPackItemLogic : MonoBehaviour {

    private static BackPackItemLogic m_Instance = null;
    public static BackPackItemLogic Instance()
    {
        return m_Instance;
    }

    public UISprite     m_ArrowUpSprite;
    public UISprite     m_ArrowDownSprite;
    public ItemSlotLogic m_ItemSlot;
    public UISprite     m_ItemSlotBgSprite;

    public GameItem     m_Item;
    //UI栏自身状态
    public STATUS       m_Status = STATUS.LOCK;
    public enum STATUS
    {
        LOCK,   //未解锁
        EMPTY,  //空格
        FILL,   //填充
    }

    // 新手指引
    private int m_NewPlayerGuide_Step = -1;
    public int NewPlayerGuide_Step
    {
        get { return m_NewPlayerGuide_Step; }
        set { m_NewPlayerGuide_Step = value; }
    }

    void Awake()
    {
        m_Instance = this;
    }

	// Use this for initialization
	void Start () {
	}

    void OnEnable()
    {
        Check_NewPlayerGuide();
    }

    void OnDestroy()
    {
        m_Instance = null;
    }
	

    public void UpdateBackPackItem(GameItem item) 
    {
        if (item != null && item.IsValid())
        {
            SetItemSlotInfo(item);
            m_ItemSlotBgSprite.spriteName = "ui_pub_021";
            SetArrow(item);
            m_Item = item;
            m_Status = STATUS.FILL;
        }
    }

    public void SetEmptyItem()
    {
        m_ItemSlot.ClearInfo();
        m_ItemSlotBgSprite.spriteName = "ui_pub_021";
        m_ArrowUpSprite.gameObject.SetActive(false);
        m_ArrowDownSprite.gameObject.SetActive(false);
        m_Item = null;
        m_Status = STATUS.EMPTY;
    }

    public void SetLockItem()
    {
        m_ItemSlot.ClearInfo();
        m_ItemSlotBgSprite.spriteName = "ui_pub_098";
        m_ArrowUpSprite.gameObject.SetActive(false);
        m_ArrowDownSprite.gameObject.SetActive(false);
        m_Item = null;
        m_Status = STATUS.LOCK;
    }

    private void SetItemSlotInfo(GameItem item)
    {
        if (item != null && item.IsValid())
        {
            m_ItemSlot.InitInfo_Item(item.DataID, ItemSlotOnClick, item.StackCount.ToString());

            if (BackPackLogic.Instance().m_QianKunDai != null)
            {
                m_ItemSlot.SetItemSlotChoose(BackPackLogic.Instance().m_QianKunDai.IsInStuffChoose(item));
            }
        }
    }

    void ItemSlotOnClick(int nItemID, ItemSlotLogic.SLOT_TYPE eItemType, string strSlotName)
    {
        if (m_Status == STATUS.FILL)
        {
            if (m_Item != null && m_Item.IsValid())
            {
                ShowTooltips();
            }
        }
    }

    public void SetArrow(GameItem item)
    {
        m_ArrowUpSprite.gameObject.SetActive(false);
        m_ArrowDownSprite.gameObject.SetActive(false);
        if (item != null && item.IsValid() && item.IsEquipMent())
        {
            if (item.GetProfessionRequire() == GlobeVar.INVALID_ID || item.GetProfessionRequire() == Singleton<ObjManager>.Instance.MainPlayer.Profession)
            {
                //获得身上对应槽位的装备
                int slotindex = item.GetEquipSlotIndex();
                GameItem compareEquip = GameManager.gameManager.PlayerDataPool.EquipPack.GetItem(slotindex);
                if (compareEquip != null)
                {
                    if (compareEquip.IsValid())
                    {
                        if (compareEquip.GetCombatValue() > item.GetCombatValue())
                        {
                            m_ArrowDownSprite.gameObject.SetActive(true);
                            m_ArrowUpSprite.gameObject.SetActive(false);
                            return;
                        }
                        else if (compareEquip.GetCombatValue() == item.GetCombatValue())
                        {
                            m_ArrowDownSprite.gameObject.SetActive(false);
                            m_ArrowUpSprite.gameObject.SetActive(false);
                            return;
                        }
                    }
                }
                m_ArrowDownSprite.gameObject.SetActive(false);
                m_ArrowUpSprite.gameObject.SetActive(true);
            }
        }
    }

    void ShowTooltips()
    {
        if (m_Item != null && m_Item.IsValid())
        {
            if (m_Item.IsEquipMent())
            {
                if (BackPackLogic.Instance().m_QianKunDai != null && BackPackLogic.Instance().m_QianKunDai.gameObject.activeInHierarchy)
                {
                    EquipTooltipsLogic.ShowEquipTooltip(m_Item, EquipTooltipsLogic.ShowType.QianKunDaiStuff, m_ItemSlot);
                }
                else
                {
                    EquipTooltipsLogic.ShowEquipTooltip(m_Item, EquipTooltipsLogic.ShowType.UnEquiped);
                }
            }
            else
            {//  by dsy
                if (BackPackLogic.Instance().m_QianKunDai != null && BackPackLogic.Instance().m_QianKunDai.gameObject.activeInHierarchy)
                {
					//UIManager.ShowUI(UIInfo.BackPackRoot, BackPackLogic.SwitchQianKunDaiView);
                    ItemTooltipsLogic.ShowItemTooltip(m_Item, ItemTooltipsLogic.ShowType.QianKunDaiStuff, m_ItemSlot);
                }
                else
                {
                    ItemTooltipsLogic.ShowItemTooltip(m_Item, ItemTooltipsLogic.ShowType.Normal);
                }
            }
        }
    }

    /// <summary>
    /// 单击
    /// </summary>
    public void OnItemClick()
    {
        switch (m_Status)
        {
            case STATUS.LOCK:
                {
                    //TODO 弹出解锁界面
                    int size = GameManager.gameManager.PlayerDataPool.BackPack.ContainerSize;
                    Tab_BackPackUnlock line = TableManager.GetBackPackUnlockByID(((int)(size - GameItemContainer.SIZE_BACKPACK)/10 + 1), 0);
                    if (line != null)
                    {
                        string str = StrDictionary.GetClientDictionaryString("#{1367}", line.ConsumeNum);
                        MessageBoxLogic.OpenOKCancelBox(str, "", UnlockOk, UnlockCancel);
                    }
                }
                break;
            case STATUS.EMPTY:
                {
                    //Do Nothing
                }
                break;
            case STATUS.FILL:
                {
                    GameItem item = m_Item;
                    if (item != null && item.IsValid())
                    {
//                         if (QianKunDaiLogic.Instance() != null)
//                         {
//                             QianKunDaiLogic.Instance().ChooseStuff(m_Item, m_ItemSlot);
//                         }
//                         else
//                         {
                        ShowTooltips();

                            // 新手指引
                            if (item.DataID == GlobeVar.PosionForgDataID && m_NewPlayerGuide_Step == 0)  // 毒烟使用
                            {
                                NewPlayerGuide(1);
                            }
                        //}                       
                    }
                }
                break;
        }
    }

    //确认解锁
    public static void UnlockOk()
    {
        CG_BACKPACK_UNLOCK unlock = (CG_BACKPACK_UNLOCK)PacketDistributed.CreatePacket(MessageID.PACKET_CG_BACKPACK_UNLOCK);
        unlock.SetType(1);
        unlock.SendPacket();
    }
    //取消解锁
    public static void UnlockCancel()
    {

    }

    /// <summary>
    /// 双击
    /// </summary>
    public void OnItemDoubleClick()
    {
        
    }

    void Check_NewPlayerGuide()
    {
        if (gameObject.name != "1000")
        {
            return;
        }
        if (BackPackLogic.Instance() == null)
        {
            return;
        }
        int nIndex = BackPackLogic.Instance().NewPlayerGuideFlag_Step;
        if (nIndex == 0)
        {
            NewPlayerGuide(0);
            BackPackLogic.Instance().NewPlayerGuideFlag_Step = -1;
        }
    }

    void NewPlayerGuide(int nIndex)
    {
        m_NewPlayerGuide_Step = nIndex;
        if (nIndex == 0)
        {
            NewPlayerGuidLogic.OpenWindow(m_ItemSlot.gameObject, 100, 100, "", "right", 2, true, true);
        }
        else if (nIndex == 1)
        {
            if (BackPackLogic.Instance())
            {
                BackPackLogic.Instance().NewPlayerGuide(1);
            }
            m_NewPlayerGuide_Step = -1;
        }
    }
}
