using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Games.Item;
using GCGame.Table;

public class TooltipsEquipGem : MonoBehaviour {

    const int GemSlotNum = 4;
    public List<ItemSlotLogic> m_GemSlot = new List<ItemSlotLogic>();
    public GameObject m_GemTips;
    public UILabel m_GemNameLabel;
    public UILabel m_GemAttrLabel;

    private string m_GemClickSlot = "";
    private int m_EquipSlotIndex = 0;

	// Use this for initialization
	void Start () {
        m_GemTips.SetActive(false);
	}
	

    public void InitGemInfo(int nEquipSlotIndex)
    {
        gameObject.SetActive(true);
        GemData gemdata = GameManager.gameManager.PlayerDataPool.GemData;
        for (int i = 0; i < GemSlotNum; i++ )
        {
            if (i < m_GemSlot.Count)
            {
                m_GemSlot[i].InitInfo_Item(gemdata.GetGemId(nEquipSlotIndex, i), GemSlotOnClick);
                m_GemSlot[i].m_delSlotOnClick = GemSlotOnClick;
            }
        }
        m_EquipSlotIndex = nEquipSlotIndex;    
    }

    public void HideGemInfo()
    {
        gameObject.SetActive(false);
        for (int i = 0; i < m_GemSlot.Count; ++i)
        {
            if (null != m_GemSlot[i])
            {
                m_GemSlot[i].ClearInfo();
            }
        }
    }

    void GemSlotOnClick(int nItemID, ItemSlotLogic.SLOT_TYPE eItemType, string strSlotName)
    {
        Tab_GemAttr tabGemAttr = TableManager.GetGemAttrByID(nItemID, 0);
        if (tabGemAttr == null)
        {
            m_GemClickSlot = strSlotName;
            //OpenGemView();
        }
        else
        {
            if (m_GemTips.activeSelf && m_GemClickSlot == strSlotName)
            {
                m_GemTips.SetActive(false);
                m_GemNameLabel.text = "";
                m_GemAttrLabel.text = "";
                m_GemClickSlot = "";
            }
            else
            {
                m_GemTips.SetActive(true);
                m_GemNameLabel.text = tabGemAttr.Name;
                m_GemAttrLabel.text = ItemTool.GetGemAttr(nItemID);
                m_GemClickSlot = strSlotName;
            }
        }
    }

    void OpenGemView()
    {
        UIManager.CloseUI(UIInfo.EquipTooltipsRoot);
        if (RoleViewLogic.Instance() != null)
        {
            OnAfterStartGemView();
        }
        else
        {
            UIManager.ShowUI(UIInfo.RoleView, OnShowGemView);
        }
    }

    void OnShowGemView(bool bSuccess, object param)
    {
        if (RoleViewLogic.Instance() != null)
        {
            RoleViewLogic.Instance().m_delAfterStart = OnAfterStartGemView;
        }
    }

    void OnAfterStartGemView()
    {
        if (RoleViewLogic.Instance() != null)
        {
            RoleViewLogic.Instance().GemBtClick();
            RoleViewLogic.Instance().OnClick_Equip(m_EquipSlotIndex);
            if (GemLogic.Instance() != null)
            {
                if (m_GemClickSlot == "GemSlot1")
                {
                    GemLogic.Instance().OnClickGemSlot1();
                }
                else if (m_GemClickSlot == "GemSlot2")
                {
                    GemLogic.Instance().OnClickGemSlot2();
                }
                else if (m_GemClickSlot == "GemSlot3")
                {
                    GemLogic.Instance().OnClickGemSlot3();
                }
            }
        }
    }
}
