using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Games.Item;
using GCGame.Table;
using Games.GlobeDefine;

public class GemLogic : MonoBehaviour {

    enum CONSTVALUE
    {
        GEM_SLOT_NUM = 4,
        GEM_ITEM_NUM = 8,
    }

    public GameObject m_Page_Help;      //帮助
    public GameObject m_Page_UnEquip;   //卸载
    public GameObject m_Page_Equip;     //镶嵌

    public UISprite[] m_GemSlotSprite;
    public UISprite[] m_GemSlotChooseSprite;
    public ItemSlotLogic[] m_GemItem;
    public UISprite[] m_GemItemQualitySprite;

    public ItemSlotLogic m_ChooseGem;
    public UILabel m_ChooseGemNameLabel;
    public UILabel m_ChooseGemAttrLabel;

    public UILabel m_EquipGemNameLabel;
    public UILabel m_EquipGemAttrLable;

    public UILabel m_EquipGemCoinNum;

    private int[] m_GemSlotId = new int[(int)CONSTVALUE.GEM_SLOT_NUM]{-1,-1,-1,-1};     //当前四个槽位宝石ID
    private int m_CurEquipSlot = -1;                         //当前选择装备部位
    private int m_CurGemSlot = -1;                           //当前选择宝石位
    private GameItem m_CurGemItem = null;                    //当前选择宝石
    private List<GameItem> m_GemItemList;                    //背包里面的宝石列表
    private int m_CurGemItemPage = 1;                        //背包宝石列表显示分页数
    private ItemSlotLogic.OnClick[] deleOnClickGems = new ItemSlotLogic.OnClick[8];

    private float   m_Delay_Time = 0f;
    private int     m_Delay_CurEquipSlot = 0;
    private int     m_Delay_CurGemSlot = 0;
    private ulong   m_Delay_GemGuid = 0;

    private static GemLogic m_Instance = null;
    public static GemLogic Instance()
    {
        return m_Instance;
    }

    private int m_NewPlayerGuide_Step = -1;
    public GameObject m_SlotHole_1;
    public GameObject m_BtnMount;

    void OnDisable()
    {
        m_Instance = null;
        //关闭界面时 如果有延迟发包 则直接发包
        if (m_Delay_Time > 0)
        {
            CG_PUT_GEM gemPacket = (CG_PUT_GEM)PacketDistributed.CreatePacket(MessageID.PACKET_CG_PUT_GEM);
			gemPacket.SetEquipslot((uint)m_Delay_CurEquipSlot);
            gemPacket.SetIndex((uint)m_Delay_CurGemSlot);
            gemPacket.SetItemguid(m_Delay_GemGuid);
            gemPacket.SendPacket();

            m_Delay_CurEquipSlot = 0;
            m_Delay_CurGemSlot = 0;
            m_Delay_GemGuid = 0;
        }

        //关闭界面时 如果有UI特效播放 则关闭特效
        if (BackCamerControll.Instance())
        {
            BackCamerControll.Instance().StopSceneEffect(133, true);
        }

        CancelInvoke("SlowUpdate");
    }
	// Use this for initialization
    
    void SlowUpdate()
    {
        if (m_Delay_Time > 0)
	    {
            m_Delay_Time -= 0.3f;
            if (m_Delay_Time <= 0)
            {
                CG_PUT_GEM gemPacket = (CG_PUT_GEM)PacketDistributed.CreatePacket(MessageID.PACKET_CG_PUT_GEM);
				gemPacket.SetEquipslot((uint)m_Delay_CurEquipSlot);
				gemPacket.SetIndex((uint)m_Delay_CurGemSlot);
                gemPacket.SetItemguid(m_Delay_GemGuid);
                gemPacket.SendPacket();

                m_Delay_CurEquipSlot = 0;
                m_Delay_CurGemSlot = 0;
                m_Delay_GemGuid = 0;
            }
	    }
	    
	}



    void OnEnable()
    {
        m_Instance = this;
        deleOnClickGems[0] = OnClickGemItem1;
        deleOnClickGems[1] = OnClickGemItem2;
        deleOnClickGems[2] = OnClickGemItem3;
        deleOnClickGems[3] = OnClickGemItem4;
        deleOnClickGems[4] = OnClickGemItem5;
        deleOnClickGems[5] = OnClickGemItem6;
        deleOnClickGems[6] = OnClickGemItem7;
        deleOnClickGems[7] = OnClickGemItem8;
        InvokeRepeating("SlowUpdate", 0f, 0.3f);
        InitEmpty();
    }

    void InitEmpty()
    {
        m_CurEquipSlot = -1;
        m_CurGemSlot = -1;
        m_CurGemItem = null;
        m_Page_Help.gameObject.SetActive(true);
        m_Page_UnEquip.gameObject.SetActive(false);
        m_Page_Equip.gameObject.SetActive(false);
        for (int i = 0; i < (int)CONSTVALUE.GEM_SLOT_NUM; i++)
        {
            m_GemSlotSprite[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < (int)CONSTVALUE.GEM_SLOT_NUM; i++)
        {
            m_GemSlotChooseSprite[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < (int)CONSTVALUE.GEM_ITEM_NUM; i++)
        {
            m_GemItem[i].ClearInfo();
        }
        for (int i = 0; i < (int)CONSTVALUE.GEM_SLOT_NUM; i++)
        {
            m_GemItemQualitySprite[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 点击某个装备部位
    /// </summary>
    /// <param name="equipslot"></param>
    public void OnClickEquiSlot(int equipslot)
    {
        m_Page_Help.gameObject.SetActive(true);
        m_Page_UnEquip.gameObject.SetActive(false);
        m_Page_Equip.gameObject.SetActive(false);
        m_CurEquipSlot = equipslot;
        m_CurGemSlot = -1;
        UpdateGemSlot();
    }

    /// <summary>
    /// 更新宝石槽位
    /// </summary>
    public void UpdateGemSlot()
    {
        GemData gemdata = GameManager.gameManager.PlayerDataPool.GemData;
        for (int i = 0; i < (int)CONSTVALUE.GEM_SLOT_NUM; i++)
        {
            m_GemSlotId[i] = gemdata.GetGemId(m_CurEquipSlot, i);
        }
        for (int i = 0; i < (int)CONSTVALUE.GEM_SLOT_NUM; i++)
        {
            if (m_GemSlotId[i] >= 0)
            {
                Tab_CommonItem line = TableManager.GetCommonItemByID(m_GemSlotId[i], 0);
                if (line != null)
                {
                    m_GemSlotSprite[i].gameObject.SetActive(true);
                    m_GemSlotSprite[i].spriteName = line.Icon;
                    m_GemItemQualitySprite[i].gameObject.SetActive(true);
                    m_GemItemQualitySprite[i].spriteName = GlobeVar.QualityColorGrid[line.Quality - 1];
                }
            }
            else
            {
                m_GemSlotSprite[i].gameObject.SetActive(false);
                m_GemItemQualitySprite[i].gameObject.SetActive(false);
            }
        }
        ClearGemSlotChoose();
        ClearGemItemChoose();
        if (m_CurGemSlot >= 0)
        {
            ClickGemSlot(m_CurGemSlot);
        }
    }

    void ClickGemSlot(int slot)
    {
        if (m_CurEquipSlot < 0 || m_CurEquipSlot >= (int)EquipPackSlot.Slot_NUM)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2108}");
            return;
        }
        Tab_GemOpenLimit line = TableManager.GetGemOpenLimitByID(slot+1, 0);
        if (line == null)
        {
            return;
        }
        //自身等级小于开放等级
        if (Singleton<ObjManager>.Instance.MainPlayer.BaseAttr.Level < line.OpenLevel)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2165}", line.OpenLevel);
            return;
        }
        m_CurGemSlot = slot;
        if (m_GemSlotId[m_CurGemSlot] >= 0)
        {
            ShowUnEquipPage();
        }
        else
        {
            ShowEquipPage();
        }
        ClearGemSlotChoose();
        ClearGemItemChoose();
        m_GemSlotChooseSprite[slot].gameObject.SetActive(true);
    }

    public void OnClickGemSlot1()
    {
        if (m_NewPlayerGuide_Step == 0)
        {
            NewPlayerGuide(1);
        }
        ClickGemSlot(0);
    }

    public void OnClickGemSlot2()
    {
        ClickGemSlot(1);
    }

    public void OnClickGemSlot3()
    {
        ClickGemSlot(2);
    }

    public void OnClickGemSlot4()
    {
        ClickGemSlot(3);
    }

    void ClearGemSlotChoose()
    {
        for (int i = 0; i < (int)CONSTVALUE.GEM_SLOT_NUM; i++)
        {
            m_GemSlotChooseSprite[i].gameObject.SetActive(false);
        }
    }

    void ShowUnEquipPage()
    {
        m_Page_Help.gameObject.SetActive(false);
        m_Page_Equip.gameObject.SetActive(false);
        m_Page_UnEquip.gameObject.SetActive(true);
        if (m_CurGemSlot >= 0 && m_CurGemSlot < (int)CONSTVALUE.GEM_SLOT_NUM)
        {
            int gemId = m_GemSlotId[m_CurGemSlot];
            if (gemId >= 0)
            {
                Tab_CommonItem line = TableManager.GetCommonItemByID(gemId, 0);
                if (line != null)
                {
                    m_ChooseGem.InitInfo(ItemSlotLogic.SLOT_TYPE.TYPE_ITEM, gemId);
                    m_ChooseGemNameLabel.text = line.Name;
                    m_ChooseGemAttrLabel.text = ItemTool.GetGemAttr(gemId);
                }
            }
        }
    }

    void ShowEquipPage()
    {
        m_Page_Help.gameObject.SetActive(false);
        m_Page_Equip.gameObject.SetActive(true);
        m_Page_UnEquip.gameObject.SetActive(false);
        UpdateGemItemList();
    }

    /// <summary>
    /// 更新宝石物品
    /// </summary>
    public void UpdateGemItemList()
    {
        GameItemContainer BackPack = GameManager.gameManager.PlayerDataPool.BackPack;
        m_GemItemList = ItemTool.ItemFilter(BackPack, (int)ItemClass.STRENGTHEN, (int)StrengthenSubClass.GEM);
        m_GemItemList = GemFilter(m_GemItemList);

        int TotalPage = (int)(m_GemItemList.Count / (int)CONSTVALUE.GEM_ITEM_NUM) + 1;
        if (m_CurGemItemPage < 1)
        {
            m_CurGemItemPage = 1;
        }
        if (m_CurGemItemPage > TotalPage)
        {
            m_CurGemItemPage = TotalPage;
        }
        //显示背包里面的宝石物品
        int starIndex = (m_CurGemItemPage - 1) * (int)CONSTVALUE.GEM_ITEM_NUM;
        for (int i = 0; i < (int)CONSTVALUE.GEM_ITEM_NUM; i++)
        {
            int GemItemIndex = i + starIndex;
            if (GemItemIndex >= 0 && GemItemIndex < m_GemItemList.Count)
            {
                m_GemItem[i].InitInfo(ItemSlotLogic.SLOT_TYPE.TYPE_ITEM, m_GemItemList[GemItemIndex].DataID, deleOnClickGems[i], m_GemItemList[GemItemIndex].StackCount.ToString(), true);
            }
            else
            {
                m_GemItem[i].ClearInfo();
            }
        }
        ClearGemItemChoose();
    }

    List<GameItem> GemFilter(List<GameItem> gemList)
    {
        List<GameItem> resultList = new List<GameItem>();
        for (int n = 0; n < gemList.Count; ++n)
        {
            Tab_GemAttr line = TableManager.GetGemAttrByID(gemList[n].DataID, 0);
            if (line != null)
            {
                //部位是否符合
                if (ItemTool.GetEquipSlotType(m_CurEquipSlot) != line.BaseClass)
                {
                    continue;
                }
                //是否已有同类宝石
                for (int i = 0; i < (int)CONSTVALUE.GEM_SLOT_NUM; i++)
                {
                    if (m_GemSlotId[i] >= 0)
                    {
                        Tab_GemAttr lineOther = TableManager.GetGemAttrByID(m_GemSlotId[i], 0);
                        if (lineOther == null)
                        {
                            continue;
                        }
                        if (lineOther.AttrClass == line.AttrClass)
                        {
                            continue;
                        }
                    }
                }
                //添加
                resultList.Add(gemList[n]);
            }
        }
        //foreach (GameItem item in gemList)
        //{
        //    Tab_GemAttr line = TableManager.GetGemAttrByID(item.DataID.ToString(), 0);
        //    if (line != null)
        //    {
        //        //部位是否符合
        //        if (ItemTool.GetEquipSlotType(m_CurEquipSlot) != line.BaseClass)
        //        {
        //            continue;
        //        }
        //        //是否已有同类宝石
        //        for (int i = 0; i < (int)CONSTVALUE.GEM_SLOT_NUM; i++ )
        //        {
        //            if (m_GemSlotId[i] >= 0)
        //            {
        //                Tab_GemAttr lineOther = TableManager.GetGemAttrByID(m_GemSlotId[i].ToString(), 0);
        //                if (lineOther == null)
        //                {
        //                    continue;
        //                }
        //                if (lineOther.AttrClass == line.AttrClass)
        //                {
        //                    continue;
        //                }
        //            }
        //        }
        //        //添加
        //        resultList.Add(item);
        //    }
        //}
        return resultList;
    }

    public void OnClickPrePage()
    {
        if (m_CurGemItemPage > 1)
        {
            m_CurGemItemPage--;
        }
        else
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2105}");
        }
        UpdateGemItemList();
        m_CurGemItem = null;
    }

    public void OnClickNextPage()
    {
        int TotalPage = (int)(m_GemItemList.Count / (int)CONSTVALUE.GEM_ITEM_NUM) + 1;
        if (m_CurGemItemPage < TotalPage)
        {
            m_CurGemItemPage++;
        }
        else
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2105}");
        }
        UpdateGemItemList();
        m_CurGemItem = null;
    }

    public void OnClickUnEquipGem()
    {
        if (m_CurEquipSlot < 0 || m_CurEquipSlot >= (int)EquipPackSlot.Slot_NUM)
        {
            return;
        }
        if (m_CurGemSlot < 0 || m_CurGemSlot >= (int)CONSTVALUE.GEM_SLOT_NUM)
        {
            return;
        }
        if (m_GemSlotId[m_CurGemSlot] < 0)
        {
            return;
        }
        CG_TAKE_GEM gemPacket = (CG_TAKE_GEM)PacketDistributed.CreatePacket(MessageID.PACKET_CG_TAKE_GEM);
		gemPacket.SetEquipslot((uint)m_CurEquipSlot);
		gemPacket.SetIndex((uint)m_CurGemSlot);
        gemPacket.SendPacket();
    }

    public void OnClickEquipGem()
    {
        if (m_NewPlayerGuide_Step == 2)
        {
            NewPlayerGuide(3);
        }

        if (m_Delay_Time > 0)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2215}");
            return;
        }

        if (m_CurEquipSlot < 0 || m_CurEquipSlot >= (int)EquipPackSlot.Slot_NUM)
        {
            return;
        }
        if (m_CurGemSlot < 0 || m_CurGemSlot >= (int)CONSTVALUE.GEM_SLOT_NUM)
        {
            return;
        }
        if (m_CurGemItem == null || m_CurGemItem.IsValid() == false)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2101}");
            return;
        }
        Tab_GemMount line = TableManager.GetGemMountByID((m_CurGemSlot + 1), 0);
        if (line != null)
        {
            int CoinNum = GameManager.gameManager.PlayerDataPool.Money.GetMoney_Coin();
            if (CoinNum < line.ConsumeNum)
            {
                //金币不足
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1830}");
                return;
            }
        }

        //是否满足宝石孔位级别需求
        if (Singleton<ObjManager>.GetInstance().MainPlayer.CheckLevelForGemSlot(m_CurGemSlot) == false)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2104}");
            return;
        }

        //同一部位是否有相同属性宝石
        if (Singleton<ObjManager>.GetInstance().MainPlayer.IsSameGemForEquipSlot(m_CurGemItem.DataID, m_CurEquipSlot))
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2103}");
            return;
        }

        if (null != GameManager.gameManager)
        {
            GameManager.gameManager.SoundManager.PlaySoundEffect(28);
        }

        //播放特效 延迟发包
        if (BackCamerControll.Instance() != null)
        {
            BackCamerControll.Instance().PlaySceneEffect(133);
        }
        m_Delay_Time = 2.0f;
        m_Delay_CurEquipSlot = m_CurEquipSlot;
        m_Delay_CurGemSlot = m_CurGemSlot;
        m_Delay_GemGuid = m_CurGemItem.Guid;
        
        //清空选择的宝石
        ClearGemItemChoose();
    }

    void ShowChooseGemInfo(GameItem item)
    {
        if (item.IsValid())
        {
            m_EquipGemNameLabel.text = item.GetName();
            m_EquipGemAttrLable.text = ItemTool.GetGemAttr(item.DataID);
            Tab_GemMount line = TableManager.GetGemMountByID((m_CurGemSlot+1), 0);
            if (line != null)
            {
                //m_EquipGemCoinNum.text = string.Format("消耗金币:{0}", line.ConsumeNum);
                m_EquipGemCoinNum.text = StrDictionary.GetClientDictionaryString("#{2836}", line.ConsumeNum);
            }
        }
    }

    void ClearChooseGemInfo()
    {
        m_EquipGemNameLabel.text = "";
        m_EquipGemAttrLable.text = "";
        m_EquipGemCoinNum.text = "";
    }

    void OnClickGemItem1(int nItemID, ItemSlotLogic.SLOT_TYPE eItemType,string strSlotName)
    {
        if (m_NewPlayerGuide_Step == 1)
        {
            NewPlayerGuide(2);
        }
        int ItemIndex = (m_CurGemItemPage-1) * (int)CONSTVALUE.GEM_ITEM_NUM + 0;
        if (ItemIndex >= 0 && ItemIndex < m_GemItemList.Count)
        {
            ClearGemItemChoose();
            m_CurGemItem = m_GemItemList[ItemIndex];
            m_GemItem[0].ItemSlotChoose();
            ShowChooseGemInfo(m_CurGemItem);
        }
    }

    void OnClickGemItem2(int nItemID, ItemSlotLogic.SLOT_TYPE eItemType, string strSlotName)
    {
        int ItemIndex = (m_CurGemItemPage - 1) * (int)CONSTVALUE.GEM_ITEM_NUM + 1;
        if (ItemIndex >= 0 && ItemIndex < m_GemItemList.Count)
        {
            ClearGemItemChoose();
            m_CurGemItem = m_GemItemList[ItemIndex];
            m_GemItem[1].ItemSlotChoose();
            ShowChooseGemInfo(m_CurGemItem);
        }
    }

    void OnClickGemItem3(int nItemID, ItemSlotLogic.SLOT_TYPE eItemType, string strSlotName)
    {
        int ItemIndex = (m_CurGemItemPage - 1) * (int)CONSTVALUE.GEM_ITEM_NUM + 2;
        if (ItemIndex >= 0 && ItemIndex < m_GemItemList.Count)
        {
            ClearGemItemChoose();
            m_CurGemItem = m_GemItemList[ItemIndex];
            m_GemItem[2].ItemSlotChoose();
            ShowChooseGemInfo(m_CurGemItem);
        }
    }

    void OnClickGemItem4(int nItemID, ItemSlotLogic.SLOT_TYPE eItemType, string strSlotName)
    {
        int ItemIndex = (m_CurGemItemPage - 1) * (int)CONSTVALUE.GEM_ITEM_NUM + 3;
        if (ItemIndex >= 0 && ItemIndex < m_GemItemList.Count)
        {
            ClearGemItemChoose();
            m_CurGemItem = m_GemItemList[ItemIndex];
            m_GemItem[3].ItemSlotChoose();
            ShowChooseGemInfo(m_CurGemItem);
        }
    }

    void OnClickGemItem5(int nItemID, ItemSlotLogic.SLOT_TYPE eItemType, string strSlotName)
    {
        int ItemIndex = (m_CurGemItemPage - 1) * (int)CONSTVALUE.GEM_ITEM_NUM + 4;
        if (ItemIndex >= 0 && ItemIndex < m_GemItemList.Count)
        {
            ClearGemItemChoose();
            m_CurGemItem = m_GemItemList[ItemIndex];
            m_GemItem[4].ItemSlotChoose();
            ShowChooseGemInfo(m_CurGemItem);
        }
    }

    void OnClickGemItem6(int nItemID, ItemSlotLogic.SLOT_TYPE eItemType, string strSlotName)
    {
        int ItemIndex = (m_CurGemItemPage - 1) * (int)CONSTVALUE.GEM_ITEM_NUM + 5;
        if (ItemIndex >= 0 && ItemIndex < m_GemItemList.Count)
        {
            ClearGemItemChoose();
            m_CurGemItem = m_GemItemList[ItemIndex];
            m_GemItem[5].ItemSlotChoose();
            ShowChooseGemInfo(m_CurGemItem);
        }
    }

    void OnClickGemItem7(int nItemID, ItemSlotLogic.SLOT_TYPE eItemType, string strSlotName)
    {
        int ItemIndex = (m_CurGemItemPage - 1) * (int)CONSTVALUE.GEM_ITEM_NUM + 6;
        if (ItemIndex >= 0 && ItemIndex < m_GemItemList.Count)
        {
            ClearGemItemChoose();
            m_CurGemItem = m_GemItemList[ItemIndex];
            m_GemItem[6].ItemSlotChoose();
            ShowChooseGemInfo(m_CurGemItem);
        }
    }

    void OnClickGemItem8(int nItemID, ItemSlotLogic.SLOT_TYPE eItemType, string strSlotName)
    {
        int ItemIndex = (m_CurGemItemPage - 1) * (int)CONSTVALUE.GEM_ITEM_NUM + 7;
        if (ItemIndex >= 0 && ItemIndex < m_GemItemList.Count)
        {
            ClearGemItemChoose();
            m_CurGemItem = m_GemItemList[ItemIndex];
            m_GemItem[7].ItemSlotChoose();
            ShowChooseGemInfo(m_CurGemItem);
        }
    }

    void ClearGemItemChoose()
    {
        for (int i = 0; i < (int)CONSTVALUE.GEM_ITEM_NUM; i++)
        {
            m_GemItem[i].ItemSlotChooseCancel();
        }
        ClearChooseGemInfo();
        m_CurGemItem = null;
    }

    public void NewPlayerGuide(int nIndex)
    {
		if (nIndex < 0) 
		{
			return;
		}
		NewPlayerGuidLogic.CloseWindow ();
        m_NewPlayerGuide_Step = nIndex;
        switch (nIndex)
        {
            case 0:
                NewPlayerGuidLogic.OpenWindow(m_SlotHole_1, 112, 112, "", "bottom", 2, true, true);
                break;
            case 1:
                NewPlayerGuidLogic.OpenWindow(m_GemItem[0].gameObject, 112, 112, "", "bottom", 2, true, true);
                break;
            case 2:
                 NewPlayerGuidLogic.OpenWindow(m_BtnMount, 202, 64, "", "bottom", 2, true, true);
                break;
            case 3:
                if (RoleViewLogic.Instance())
                {
                    RoleViewLogic.Instance().NewPlayerGuide(3);
                }
                m_NewPlayerGuide_Step = -1;
                break;
        }
    }
}
