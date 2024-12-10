using UnityEngine;
using System.Collections;
using Module.Log;
using GCGame.Table;
using System.Collections.Generic;
using GCGame;
public class HelpController : UIControllerBase<HelpController>
{
    //public SubUIBG subBG;

    public GameObject prefPage = null;

    public GameObject m_HelpGrid_Page = null;
    public GameObject m_HelpGrid_Card = null;
    public GameObject m_HelpWindow = null;
    public GameObject m_HelpCardWindow = null;
    public UILabel m_HelpCardHeadTitle = null;

    void Awake()
    {
        SetInstance(this);
        //subBG.SetData("help_title_text", UIInfo.HelpController);

        // 加载页面
        LoadPageData();

        if (m_HelpWindow != null)
        {
            m_HelpWindow.SetActive(true);
        }
        if (m_HelpCardWindow != null)
        {
           m_HelpCardWindow.SetActive(false);
        }
    }

    private void LoadPageData()
    {
        if (null == m_HelpGrid_Page)
        {
            LogModule.ErrorLog("m_HelpGrid_Page id null");
            return;
        }
        Utils.CleanGrid(m_HelpGrid_Page.gameObject);
       Dictionary<int, List<Tab_HelpItem>> helpItemList = TableManager.GetHelpItem();
       for (int i = 0; i < helpItemList.Count; i++)
       {
           if (helpItemList.ContainsKey(i+1))
           {
               AddPage((i+1).ToString(), helpItemList[i+1]);
           }
       }
    }

    private void AddPage(string pageId,  List<Tab_HelpItem> pageItems)
    {
        if (m_HelpGrid_Page == null)
        {
            return;
        }
        GameObject pageItem = (GameObject)Instantiate(prefPage) as GameObject;
        if (null == pageItem)
        {
            LogModule.ErrorLog("pageItem is null, Instantiate failed!!");
            return;
        }
        pageItem.transform.parent = m_HelpGrid_Page.transform;
        pageItem.transform.localPosition = Vector3.zero;
        pageItem.transform.localScale = Vector3.one;

        HelpPageItem helpPageItem = pageItem.GetComponent<HelpPageItem>();
        if (null == helpPageItem)
        {
            LogModule.ErrorLog("helpPageItem is null");
            return;
        }
        helpPageItem.m_HelpWindow = m_HelpWindow;
        helpPageItem.m_HelpCardWindow = m_HelpCardWindow;
        helpPageItem.m_HelpGrid_Card = m_HelpGrid_Card.transform;
        helpPageItem.m_HelpCardHeadTitle = m_HelpCardHeadTitle;
        helpPageItem.SetHelpItem(pageItems);
        m_HelpGrid_Page.GetComponent<UIGrid>().repositionNow = true;
    }


    void OnClickBack()
    {
        m_HelpCardWindow.SetActive(false);
        m_HelpWindow.SetActive(true);
    }
   

    // Use this for initialization
    //void Start()
    //{

    //}

    void OnCloseClick()
    {
        UIManager.CloseUI(UIInfo.HelpController);
        UIManager.ShowUI(UIInfo.SystemAndAutoFight);
    }
}
