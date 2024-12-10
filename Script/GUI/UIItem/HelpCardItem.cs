using UnityEngine;
using System.Collections;
using GCGame.Table;

public class HelpCardItem : MonoBehaviour {

	// Use this for initialization
    public UISprite m_IconSprite;
    public UISprite[] m_StarIconSprite;
    public UILabel m_DescLable;
    public UILabel m_TitleLable;

    public const int HELP_RECOMMEND_MAX = 5;
    private Tab_HelpItem m_helpItem = null;
    private int m_nRecommendIndex = -1;
	void Start () {
	
	}
	
   
    /// <summary>
    ///  //点击前往按钮
    /// </summary>
    void OnClickQianWang()
    {
        OpenRecommUI();
    }

    /// <summary>
    /// 打开推荐功能的界面
    /// </summary>
    void OpenRecommUI()
    {
        if (m_helpItem == null)
        {
            return;
        }
        if (m_nRecommendIndex < 0 || m_nRecommendIndex >= HELP_RECOMMEND_MAX)
        {
            return;
        }
        // 根据RecommTitle 打开对应的界面
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="helpItem"></param>
    /// <param name="nRecommendIndex"></param>
    public void SetCardInfo(Tab_HelpItem helpItem, int nRecommendIndex)
    {
        if (helpItem == null)
        {
            return;
        }
        if (nRecommendIndex >= HELP_RECOMMEND_MAX || nRecommendIndex < 0)
        {
            return;
        }
        m_helpItem = helpItem;
        m_nRecommendIndex = nRecommendIndex;

        if (m_IconSprite != null )
        {
            m_IconSprite.spriteName = helpItem.GetRecomIconbyIndex(nRecommendIndex);
        }
        if (m_DescLable != null)
        {
            m_DescLable.text = helpItem.GetRecomDescbyIndex(nRecommendIndex);
        }
        if (m_TitleLable != null)
        {
            m_TitleLable.text = helpItem.GetRecomTitlebyIndex(nRecommendIndex);
        }
        int nStar = helpItem.GetRecomStarbyIndex(nRecommendIndex);
        for (int i = 0; i < m_StarIconSprite.Length; i++)
        {
            if (nStar - 1 >= i)
            {
                m_StarIconSprite[i].spriteName = "partner-star1"; 
            }
            else
            {
                m_StarIconSprite[i].spriteName = "partner-star2"; 
            }
        }
    }

}
