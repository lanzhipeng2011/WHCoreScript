using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GCGame.Table;
using Module.Log;
using System;
using GCGame;

public class HelpPageItem : MonoBehaviour {

    public GameObject prefCard = null;

    public Transform m_HelpGrid_Card = null;

    public GameObject m_HelpCardWindow = null;

    public GameObject m_HelpWindow = null;

    public UILabel m_HelpCardHeadTitle = null;

    public UILabel[] m_LableHelpItemName;

    public GameObject[] m_HelpItemButton;

    private List<Tab_HelpItem> m_PageHelpItems = null;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ListPageHelpItem"></param>
    public void SetHelpItem(List<Tab_HelpItem> ListPageHelpItem)
    {
        if (ListPageHelpItem == null)
        {
            LogModule.ErrorLog("SetHelpItem::ListPageHelpItem = null");
            return;
        }
        m_PageHelpItems = ListPageHelpItem;
        for (int i = 0; i < m_LableHelpItemName.Length && i < m_HelpItemButton.Length; i++)
        {
            if (i >= m_PageHelpItems.Count)
            {
                m_HelpItemButton[i].SetActive(false);
                continue;
            }
            m_HelpItemButton[i].SetActive(true);
            m_LableHelpItemName[i].text = ListPageHelpItem[i].HelpName;
        }
    }

    private void AddCard(Tab_HelpItem helpItem, int nIndex)
    {
        if (null == m_HelpGrid_Card)
        {
            LogModule.ErrorLog("AddCard::m_HelpGrid_Card is null");
            return;
        }
        GameObject card = (GameObject)Instantiate(prefCard) as GameObject;
        if (null == card)
        {
            LogModule.ErrorLog("card is null, instantiate failed");
            return;
        }
        card.transform.parent = m_HelpGrid_Card;
        card.transform.localPosition = Vector3.one;
        card.transform.localScale = Vector3.one;
        HelpCardItem item = card.GetComponent<HelpCardItem>();
        if (null != item)
        {
            item.SetCardInfo(helpItem, nIndex);
        }
    }

    private void SetCards(Tab_HelpItem helpItem)
    {
        if (m_HelpGrid_Card == null)
        {
            LogModule.ErrorLog("SetCards::m_HelpGrid_Card = null");
            return;
        }
        Utils.CleanGrid(m_HelpGrid_Card.gameObject);
        if (helpItem == null)
        {
            LogModule.ErrorLog("SetCards::helpItem = null");
            return;
        }
        if (m_HelpCardHeadTitle != null)
        {
            m_HelpCardHeadTitle.text = helpItem.HelpName;
        }
        AddCard(helpItem, 0);
        AddCard(helpItem, 1);
        AddCard(helpItem, 2);
        AddCard(helpItem, 3);
        AddCard(helpItem, 4);
        m_HelpGrid_Card.GetComponent<UIGrid>().repositionNow = true;
    }


    void OnClickHelpButton(int nIndex)
    {
       if (m_HelpWindow != null)
       {
           m_HelpWindow.SetActive(false);
       }
       if (nIndex >=0 && nIndex < m_PageHelpItems.Count)
       {
           SetCards(m_PageHelpItems[nIndex]);
       }
       if (m_HelpCardWindow != null)
       {
            m_HelpCardWindow.SetActive(true);
       }             
    }

    void OnClickHelpButton1()
    {
        OnClickHelpButton(0);

    }
    void OnClickHelpButton2()
    {
        OnClickHelpButton(1);
    }
    void OnClickHelpButton3()
    {
        OnClickHelpButton(2);
    }
    void OnClickHelpButton4()
    {
        OnClickHelpButton(3);
    }
    void OnClickHelpButton5()
    {
        OnClickHelpButton(4);
    }
    void OnClickHelpButton6()
    {
        OnClickHelpButton(5);
    }
    void OnClickHelpButton7()
    {
        OnClickHelpButton(6);
    }
    void OnClickHelpButton8()
    {
        OnClickHelpButton(7);
    }
    void OnClickHelpButton9()
    {
        OnClickHelpButton(8);
    }
}
