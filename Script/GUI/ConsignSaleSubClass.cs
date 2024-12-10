/********************************************************************
	创建时间:	2014/06/12 13:25
	全路径:		\TLOL\Version\Main\Project\Client\Assets\MLDJ\Script\GUI\ConsignSaleSubClass.cs
	创建人:		luoy
	功能说明:	寄售行购买界面左侧 子分类按钮
	修改记录:
*********************************************************************/
using Games.ConsignSale;
using UnityEngine;
using System.Collections;

public class ConsignSaleSubClass : MonoBehaviour
{
    public UILabel m_NameLabel;
    public UISprite m_ClickSprite;
    private int m_nClass =(int)ConsignSale_SearchClass.INVAILD_TYPE;
    public int Class
    {
        get { return m_nClass; }
        set { m_nClass = value; }
    }

    private int m_nSubClass =(int) ConsignSale_SearchSubClass.INVAILD_TYPE;
    public int SubClass
    {
        get { return m_nSubClass; }
        set { m_nSubClass = value; }
    }
    public void SetNameInfo(string strName)
    {
        m_NameLabel.text = strName;
    }
    //点击筛选查询
    void ClickSubClassBt()
    {
        if (ConsignSaleLogic.Instance() != null)
        {
            ConsignSaleLogic.Instance().ProcessClickSearchBt(m_nClass, m_nSubClass);
            m_ClickSprite.gameObject.SetActive(true);
        }
    }
	// Use this for initialization
	void Start () {
	
	}
	
}
