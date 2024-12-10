using UnityEngine;
using System.Collections;
using GCGame;
using Games.SwordsMan;
public class SwordsManItem : MonoBehaviour {

    public UILabel m_LableLevel;
    public UISprite m_IconSprite;
    public UISprite m_QualitySprite;
    public UISprite m_SelectSprite;
    public UISprite m_LockSprite;
    //public UILabel m_LabelLevelValue;
    //public UILabel m_LabelDesc;
    //public UILabel m_LabelCurEffect;
    //public UILabel m_LabelNextEffect;
    //public UILabel m_LabelExp;
    //public UISlider m_SliderExp;

    private SwordsManController m_BackParent = null;
    private SwordsManLevelupController m_LevelUpParent = null;
    public SwordsMan m_oSwordsMan = null;
    public Status m_Status = Status.STATUS_ENABLE;
    public enum Status
    {
        STATUS_ENABLE = 0,  //未选择
        STATUS_CHOOSED,     //选择
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="resItem"></param>
    /// <param name="name"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static SwordsManItem CreateItem(GameObject grid, GameObject resItem, string name, SwordsManController parent)
    {
        GameObject curItem = Utils.BindObjToParent(resItem, grid, name);
        if (null == curItem)
        {
            return null;
        }
        SwordsManItem curItemComponent = curItem.GetComponent<SwordsManItem>();
        if (null == curItemComponent)
        {
            return null;
        }
        curItemComponent.SetParent(parent);
        return curItemComponent;
    }

    public static SwordsManItem CreateItem(GameObject grid, GameObject resItem, string name, SwordsManLevelupController parent)
    {
        GameObject curItem = Utils.BindObjToParent(resItem, grid, name);
        if (null == curItem)
        {
            return null;
        }
        SwordsManItem curItemComponent = curItem.GetComponent<SwordsManItem>();
        if (null == curItemComponent)
       {
           return null;
       }
        curItemComponent.SetParent(parent);
        return curItemComponent;
    }

    /// <summary>
    /// 设置侠客信息
    /// </summary>
    public void SetData(SwordsMan oSwordsMan)
    {
        if (oSwordsMan == null)
        {
            ClearUp();
            return;
        }
        m_oSwordsMan = oSwordsMan;
        m_Status = Status.STATUS_ENABLE;
        if (null == m_IconSprite)
        {
            return;
        }
        if (null == m_QualitySprite)
        {
            return;
        }

        if (m_LableLevel != null)
        {
			m_LableLevel.text = "等级" + m_oSwordsMan.Level.ToString();
        }
        m_IconSprite.spriteName = m_oSwordsMan.GetIcon();
        m_QualitySprite.spriteName = SwordsMan.GetQualitySpriteName((SwordsMan.SWORDSMANQUALITY)m_oSwordsMan.Quality);
        OnSelectStateChange();
 
        //m_LabelLevelValue.text = m_oSwordsMan.Level.ToString();
        //m_LabelDesc.text = m_oSwordsMan.GetTips();
        //m_LabelCurEffect.text = m_oSwordsMan.GetCombatValue().ToString();
        //m_LabelExp.text = m_oSwordsMan.Exp.ToString();

        //int nMaxExp = m_oSwordsMan.MaxExp;
        //if (nMaxExp > 0)
        //{
        //    float fSlider = (float)m_oSwordsMan.Exp / (float)nMaxExp;
        //    m_SliderExp.value = fSlider;
        //}

    }

    void ClearUp()
    {
        m_oSwordsMan = null;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parent"></param>
    public void SetParent(SwordsManController parent)
    {
        m_BackParent = parent;
        m_LevelUpParent = null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parent"></param>
    public void SetParent(SwordsManLevelupController parent)
    {
        m_LevelUpParent = parent;
        m_BackParent = null;
    }

    /// <summary>
    /// 点击侠客
    /// </summary>
    void OnItemClick()
    {
        if (m_oSwordsMan == null)
        {
            return;
        }
        if (m_LevelUpParent != null)
        {
            if (m_oSwordsMan.Locked)
            {
				GUIData.AddNotifyData2Client(false,"#{2555}");
                return;
            }
            else
            {
                m_LevelUpParent.OnSelectSwordsManItem(this);
            }
        }
        else
        {
            SwordsManToolTipsLogic.ShowSwordsManTooltip(m_oSwordsMan, SwordsManToolTipsLogic.SwordsMan_ShowType.UnEquiped);
        }
    }

    public void OnSelectSwordsMan()
    {
        if (m_oSwordsMan == null)
        {
            return;
        }
        if (m_oSwordsMan.Locked)
        {
            return;
        }
        if (m_Status == Status.STATUS_ENABLE)
        {
            m_Status = Status.STATUS_CHOOSED;
        }
        else if (m_Status == Status.STATUS_CHOOSED)
        {
            m_Status = Status.STATUS_ENABLE;
        }
        OnSelectStateChange();
    }

    void OnSelectStateChange()
    {
        if (m_SelectSprite == null )
        {
            return;
        }
        if (m_LockSprite == null)
        {
            return;
        }
        if (m_oSwordsMan == null)
        {
            return;
        }
        if (m_Status == Status.STATUS_CHOOSED)
        {
            m_SelectSprite.gameObject.SetActive(true);
            m_LockSprite.gameObject.SetActive(false);
        }
        else if (m_Status == Status.STATUS_ENABLE)
        {
            m_SelectSprite.gameObject.SetActive(false);
            m_LockSprite.gameObject.SetActive(false);
        }
        if (m_oSwordsMan.Locked)
        {
            m_SelectSprite.gameObject.SetActive(false);
            m_LockSprite.gameObject.SetActive(true);
        }
    }

    public bool IsSelect()
    {
        return (m_Status == Status.STATUS_CHOOSED);
    }
}
