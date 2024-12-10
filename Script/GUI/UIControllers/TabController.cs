/********************************************************************
	created:	2014/01/14
	created:	14:1:2014   13:43
	filename: 	TabController.cs
	author:		王迪
	
	purpose:	Tab页控制器，结构为Tab页控制器--Grid字节点--Tab字项
*********************************************************************/
using UnityEngine;
using System.Collections;
using Module.Log;
public class TabController : MonoBehaviour {

    public int startSelectTab = 0;          // 初始高亮索引

    private TabButton curHighLightTab;      // 当前高亮的索引
    private Transform m_grid;               // 子结点必须有一个GRID
    private bool m_bEnableClick;

    public delegate void TabChangedDelegate(TabButton curButton);
    public TabChangedDelegate delTabChanged;
	void Awake () 
    {

        InitData();
	}

    public void InitData()
    {
        if (null != m_grid)
        {
            return;
        }

        if (gameObject.GetComponent<UIGrid>() != null)
        {
            m_grid = transform;
        }
        else
        {
            m_grid = transform.GetChild(0);
        }

        if (null == m_grid)
        {
            m_grid = transform;
        }
        
        if (null != m_grid)
        {
            for (int i = 0; i < m_grid.childCount; i++)
            {
                if (i == startSelectTab)
                {
                    curHighLightTab = m_grid.GetChild(i).gameObject.GetComponent<TabButton>();
                    curHighLightTab.HighLightTab(true);
                }
                else
                {
                    m_grid.GetChild(i).gameObject.GetComponent<TabButton>().HighLightTab(false);
                }
            }


        }

        m_bEnableClick = true;
    }
    public void OnTabClicked(TabButton curTab)
    {
        if (!m_bEnableClick)
        {
            return;
        }

        if (curHighLightTab == curTab)
        {
            return;
        }
        DoChangeTab(curTab);
    }

    // 切换标签
    public GameObject ChangeTab(string tabName)
    {
        InitData();
        if (null == m_grid)
        {
            LogModule.ErrorLog("tabcontroller can not find grid");
            return null;
        }

        Transform curTabTrans = m_grid.FindChild(tabName);
        if (null == curTabTrans)
        {
            LogModule.ErrorLog("can not find tabButton" + tabName);
            return null;
        }

        return DoChangeTab(curTabTrans.GetComponent<TabButton>());
    }

    // 控制是否接收点击事件
    public void EnableClick(bool bEnable)
    {
        m_bEnableClick = bEnable;
    }

    // 根据名称获取按钮
    public TabButton GetTabButton(string tabName)
    {
        if (null == m_grid)
        {
            return null;
        }

        Transform curTabTrans = m_grid.FindChild(tabName);
        if (null == curTabTrans)
        {
            LogModule.ErrorLog("can not find tabButton" + tabName);
            return null;
        }

        return curTabTrans.GetComponent<TabButton>();
    }

    // 获取当前选中TabButton
    public TabButton GetHighlightTab()
    {
        return curHighLightTab;
    }

    GameObject DoChangeTab(TabButton curTab)
    {
        
        if (null != curHighLightTab)
        {
            curHighLightTab.HighLightTab(false);
        }
        curTab.HighLightTab(true);
        curHighLightTab = curTab;
        if (null != delTabChanged) delTabChanged(curTab);
        return curHighLightTab.GetComponent<TabButton>().targetObject;
    }

    public void SortTabGrid()
    {
        if (null!= m_grid)
        {
            if (null != m_grid.gameObject.GetComponent<UIGrid>())
            {
                m_grid.gameObject.GetComponent<UIGrid>().Reposition();
            }

            if (null != m_grid.gameObject.GetComponent<UITopGrid>())
            {
                m_grid.gameObject.GetComponent<UITopGrid>().Recenter(true);
            }
        }
    }
}
