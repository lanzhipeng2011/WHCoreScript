using UnityEngine;
using System.Collections;
using Module;
using Module.Log;
public class RestaurantFoodFilterWindow :MonoBehaviour
{
	public enum MUXUE_TAB_PAGE{     //墓穴分页
		TAB_PAGE_EXP,     //经验
		TAB_PAGE_MONEY,  //金钱
		TAB_PAGE_MAT,    //材料
		TAB_PAGE_OTHER,     //其它
	};
    public RestaurantFoodWindow m_FoodWindow;

    public GameObject[] m_levelSelect;
    public GameObject m_ExpSelect;
    public GameObject m_YuanBaoSelect;
    public GameObject m_MetarialSelect;
	public UISprite[] m_TabPage_HighLight;
	private MUXUE_TAB_PAGE  m_curPage;
	private int   m_selectLev=1;
    // Use this for initialization
    //void Start()
    //{
    //    //for (int i = 0; i < m_levelSelect.Length; i++)
    //    //{
    //    //    if (m_levelSelect[i])
    //    //    {
    //    //        m_levelSelect[i].SetActive(true);
    //    //    }
    //    //}
    //    //m_ExpSelect.SetActive(true);
    //    //m_YuanBaoSelect.SetActive(true);
    //    //m_MetarialSelect.SetActive(true);
        
    //}

    void OnEnable()
    {
        if (null == m_FoodWindow)
        {
            LogModule.ErrorLog("m_FoodWindow is null");
            return;
        }
//        if (null == m_ExpSelect)
//        {
//            LogModule.ErrorLog("m_ExpSelect is null");
//            return;
//        }
//        if (null == m_YuanBaoSelect)
//        {
//            LogModule.ErrorLog("m_YuanBaoSelect is null");
//            return;
//        }
//        if (null == m_MetarialSelect)
//        {
//            LogModule.ErrorLog("m_MetarialSelect is null");
//            return;
//        }
//        for (int i = 0; i < m_levelSelect.Length; i++)
//        {
//            if (m_FoodWindow.IsLevelFilter(i+1))
//            {
//                m_levelSelect[i].SetActive(true);
//            }
//            else
//            {
//                m_levelSelect[i].SetActive(false);
//            }
//        }
        if (m_FoodWindow.IsRewardTypeFilter(RestaurantFoodWindow.FOOD_REWARD_TYPE.REWARD_TYPE_EXP))
        {
			OnChangeTab(MUXUE_TAB_PAGE.TAB_PAGE_EXP);
        }
     
        if (m_FoodWindow.IsRewardTypeFilter(RestaurantFoodWindow.FOOD_REWARD_TYPE.REWARD_TYPE_COIN))
        {
			OnChangeTab(MUXUE_TAB_PAGE.TAB_PAGE_MONEY);
        }
      
        if (m_FoodWindow.IsRewardTypeFilter(RestaurantFoodWindow.FOOD_REWARD_TYPE.REWARD_TYPE_MATERIAL))
        {
			OnChangeTab(MUXUE_TAB_PAGE.TAB_PAGE_MAT);
        }
        
    }
    
    void OnClickBack()
    {
        if ( null == m_FoodWindow )
        {
            LogModule.ErrorLog("m_FoodWindow == null");
            return;
        }
        m_FoodWindow.OnCloseFliterWindow();
    }

    void OnClickFilterLevel1()
    {
        if (null != m_FoodWindow)
        {
            if ( m_FoodWindow.IsLastLevelFilter(1) )
            {
                return;
            }
			m_selectLev=1;
            m_FoodWindow.SetFilterLevel(1);
			m_FoodWindow.OnCloseFliterWindow();

//            if (m_levelSelect.Length >= 1 )
//            {
//                m_levelSelect[0].SetActive(!m_levelSelect[0].activeSelf);
//            }
        }
    }
    void OnClickFilterLevel2()
    {
        if (null != m_FoodWindow)
        {
            if (m_FoodWindow.IsLastLevelFilter(2))
            {
                return;
            }
			m_selectLev=2;
            m_FoodWindow.SetFilterLevel(2);
			m_FoodWindow.OnCloseFliterWindow();
//            if (m_levelSelect.Length >= 2)
//            {
//                m_levelSelect[1].SetActive(!m_levelSelect[1].activeSelf);
//            }
        }
    }
    void OnClickFilterLevel3()
    {
        if (null != m_FoodWindow)
        {
            if (m_FoodWindow.IsLastLevelFilter(3))
            {
                return;
            }
			m_selectLev=3;
            m_FoodWindow.SetFilterLevel(3);
			m_FoodWindow.OnCloseFliterWindow();
//            if (m_levelSelect.Length >= 3)
//            {
//                m_levelSelect[2].SetActive(!m_levelSelect[2].activeSelf);
//            }
        }
    }
    void OnClickFilterLevel4()
    {
        if (null != m_FoodWindow)
        {
            if (m_FoodWindow.IsLastLevelFilter(4))
            {
                return;
            }
			m_selectLev=4;
            m_FoodWindow.SetFilterLevel(4);
			m_FoodWindow.OnCloseFliterWindow();
//            if (m_levelSelect.Length >= 4)
//            {
//                m_levelSelect[3].SetActive(!m_levelSelect[3].activeSelf);
//            }
        }
    }
    void OnClickFilterLevel5()
    {
        if (null != m_FoodWindow)
        {
            if (m_FoodWindow.IsLastLevelFilter(5))
            {
                return;
            }
			m_selectLev=5;
            m_FoodWindow.SetFilterLevel(5);
			m_FoodWindow.OnCloseFliterWindow();
//            if (m_levelSelect.Length >= 5)
//            {
//                m_levelSelect[4].SetActive(!m_levelSelect[4].activeSelf);
//            }
        }
    }
    void OnFilterExp()
    {
        if (null != m_FoodWindow)
        {
            if (m_FoodWindow.IsLastRewardTypeFilter(RestaurantFoodWindow.FOOD_REWARD_TYPE.REWARD_TYPE_EXP))
            {
                return;
            }
			OnChangeTab(MUXUE_TAB_PAGE.TAB_PAGE_EXP);
            m_FoodWindow.SetFilterRewardType(RestaurantFoodWindow.FOOD_REWARD_TYPE.REWARD_TYPE_EXP);
//            if (m_ExpSelect != null)
//            {
//                m_ExpSelect.SetActive(!m_ExpSelect.activeSelf);
//            }
        }
    }
    void OnFilterYuanBao()
    {
        if (null != m_FoodWindow)
        {
            if (m_FoodWindow.IsLastRewardTypeFilter(RestaurantFoodWindow.FOOD_REWARD_TYPE.REWARD_TYPE_COIN))
            {
                return;
            }
			OnChangeTab(MUXUE_TAB_PAGE.TAB_PAGE_MONEY);
            m_FoodWindow.SetFilterRewardType(RestaurantFoodWindow.FOOD_REWARD_TYPE.REWARD_TYPE_COIN);
//            if (m_YuanBaoSelect != null)
//            {
//                m_YuanBaoSelect.SetActive(!m_YuanBaoSelect.activeSelf);
//            }
        }
    }
    void OnFilterMaterial()
    {
        if (null != m_FoodWindow)
        {
            if (m_FoodWindow.IsLastRewardTypeFilter(RestaurantFoodWindow.FOOD_REWARD_TYPE.REWARD_TYPE_MATERIAL))
            {
                return;
            }
			OnChangeTab(MUXUE_TAB_PAGE.TAB_PAGE_MAT);
            m_FoodWindow.SetFilterRewardType(RestaurantFoodWindow.FOOD_REWARD_TYPE.REWARD_TYPE_MATERIAL);
//            if (m_ExpSelect != null)
//            {
//                m_MetarialSelect.SetActive(!m_MetarialSelect.activeSelf);
//            }
        }
    }
	void OnChangeTab(MUXUE_TAB_PAGE  page)
	{
		m_curPage = page;
		//显示tab按钮高亮
		for (int i = 0; i <3; ++i )
		{
			m_TabPage_HighLight[i].gameObject.SetActive(false);
		}
		
		int curTabIndex = (int)m_curPage;
		if ( curTabIndex >= 0 && curTabIndex < 3 )
		{
			m_TabPage_HighLight[curTabIndex].gameObject.SetActive(true);
		}

	}
}