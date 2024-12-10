using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Games.Item;
using GCGame.Table;
using Games.GlobeDefine;

public class OtherGemViewLogic : MonoBehaviour
{

    enum CONSTVALUE
    {
        GEM_SLOT_NUM = 4,
        GEM_ITEM_NUM = 8,
    }

    public GameObject m_Page_Help;      //帮助
    public GameObject m_Page_UnEquip;   //卸载

    public UISprite[] m_GemSlotSprite;
    public UISprite[] m_GemSlotChooseSprite;
    public UISprite[] m_GemItemQualitySprite;

    public ItemSlotLogic m_ChooseGem;
    public UILabel m_ChooseGemNameLabel;
    public UILabel m_ChooseGemAttrLabel;


    private int[] m_GemSlotId = new int[(int)CONSTVALUE.GEM_SLOT_NUM] { -1, -1, -1, -1 };     //当前四个槽位宝石ID
    private int m_CurEquipSlot = -1;                         //当前选择装备部位
    private int m_CurGemSlot = -1;                           //当前选择宝石位

    private static OtherGemViewLogic m_Instance = null;
    public static OtherGemViewLogic Instance()
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

    void OnEnable()
    {
        InitEmpty();
    }

    void InitEmpty()
    {
        m_CurEquipSlot = -1;
        //m_CurGemSlot = -1;
        //m_CurGemItem = null;
        m_Page_Help.gameObject.SetActive(true);
        m_Page_UnEquip.gameObject.SetActive(false);
        for (int i = 0; i < (int)CONSTVALUE.GEM_SLOT_NUM; i++)
        {
            m_GemSlotSprite[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < (int)CONSTVALUE.GEM_SLOT_NUM; i++)
        {
            m_GemSlotChooseSprite[i].gameObject.SetActive(false);
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
        m_CurEquipSlot = equipslot;
        m_CurGemSlot = -1;
        UpdateGemSlot();
    }

    /// <summary>
    /// 更新宝石槽位
    /// </summary>
    public void UpdateGemSlot()
    {
        GemData gemdata = GameManager.gameManager.OtherPlayerData.GemData;
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
		ClearGemSlotChoose ();
		if(m_CurGemSlot >= 0)
		{
			ClickGemSlot(m_CurGemSlot);
		}
    }

    void ShowUnEquipPage()
    {
        m_Page_Help.gameObject.SetActive(false);
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
        m_Page_UnEquip.gameObject.SetActive(false);
    }

    void ClearGemSlotChoose()
    {
        for (int i = 0; i < (int)CONSTVALUE.GEM_SLOT_NUM; i++)
        {
            m_GemSlotChooseSprite[i].gameObject.SetActive(false);
        }
    }

    void ClickGemSlot(int slot)
    {
        if (m_CurEquipSlot < 0 || m_CurEquipSlot >= (int)EquipPackSlot.Slot_NUM)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2108}");
            return;
        }
        ////Tab_GemOpenLimit line = TableManager.GetGemOpenLimitByID((slot + 1).ToString(), 0);
        ////if (line == null)
        ////{
        ////    return;
        ////}
        ////自身等级小于开放等级
        ////if (Singleton<ObjManager>.Instance.MainPlayer.BaseAttr.Level < line.OpenLevel)
        ////{
        ////    Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2165}", line.OpenLevel);
        ////    return;
        ////}
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
        m_GemSlotChooseSprite[slot].gameObject.SetActive(true);
    }

    public void OnClickGemSlot1()
    {
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
}
