using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
using GCGame.Table;
using Games.LogicObj;

public class FashionItemLogic : MonoBehaviour {

    public ItemSlotLogic m_ItemSlot;
    public GameObject m_ChosenSprite;

    private int m_FashionID = GlobeVar.INVALID_ID;
    public int FashionID
    {
        get { return m_FashionID; }
    }

    public void InitInfo(int nFashionID)
    {
        m_FashionID = nFashionID;

        m_ItemSlot.InitInfo_Fashion(nFashionID, ChooseFashionItem);
        HideChosenSprite();
    }

    void ChooseFashionItem(int nFashionID, ItemSlotLogic.SLOT_TYPE eItemType, string strSlotName)
    {
        HandleItemChoose();
    }

    public void HandleItemChoose()
    {
        if (FashionLogic.Instance() != null)
        {
            FashionLogic.Instance().ChooseFashionItem(m_FashionID);
        }
        if (RoleViewLogic.Instance() != null)
        {
            RoleViewLogic.Instance().FitOnFashion(m_FashionID);
        }
    }

    public void UpdateHightLight(int nFashionID)
    {
        if (nFashionID == m_FashionID)
        {
            m_ItemSlot.ItemSlotChoose();
        }
        else
        {
            m_ItemSlot.ItemSlotChooseCancel();
        }
    }

    public void ShowChosenSprite()
    {
        m_ChosenSprite.SetActive(true);
    }

    public void HideChosenSprite()
    {
        m_ChosenSprite.SetActive(false);
    }
}
