/********************************************************************
	创建时间:	2014/06/12 13:23
	全路径:		\TLOL\Version\Main\Project\Client\Assets\MLDJ\Script\GUI\ConsignSaleQualityItem.cs
	创建人:		luoy
	功能说明:	寄售行品质按钮
	修改记录:
*********************************************************************/
using System.Reflection.Emit;
using UnityEngine;
using System.Collections;

public class ConsignSaleQualityItem : MonoBehaviour
{
    public UISprite m_SeleIcon;
    public UILabel  m_NameLabel;
    private int     m_nQuality =-1;
	// Use this for initialization
	void Start ()
    {
	
	}
    void OnEnable()
    {
        if (ConsignSaleLogic.Instance() != null)
        {
            if (ConsignSaleLogic.Instance().SearchQuality == m_nQuality)
            {
                EnableSeleIcon(true);
            }
            else
            {
                EnableSeleIcon(false);
            }
        }
    }
    void OnDisable()
    {
    }
    public void EnableSeleIcon(bool bIsShow)
    {
        m_SeleIcon.gameObject.SetActive(bIsShow);
    }

    public void SetLableName(string strName)
    {
        m_NameLabel.text = strName;
    }
    public void SetQuality(int quality)
    {
        m_nQuality = quality;
    }
    //点击 查询筛选
    private void Click()
    {
        if (ConsignSaleLogic.Instance() != null)
        {
            ConsignSaleLogic.Instance().SearchQuality = m_nQuality;
            ConsignSaleLogic.Instance().CurBuyPage = 0;
            ConsignSaleLogic.Instance().SendAskSearchInfo(ConsignSaleLogic.Instance().CurBuyPage);
            ConsignSaleLogic.Instance().m_QualityMenu.SetActive(false);
        }
    }
}
