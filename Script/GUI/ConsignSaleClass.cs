/********************************************************************
	创建时间:	2014/06/12 13:20
	全路径:		\TLOL\Version\Main\Project\Client\Assets\MLDJ\Script\GUI\ConsignSaleClass.cs
	创建人:		luoy
	功能说明:	寄售行购买分页 左侧分类按钮
	修改记录:
*********************************************************************/
using Games.ConsignSale;
using UnityEngine;
using System.Collections;

public class ConsignSaleClass : MonoBehaviour
{
    public UILabel m_NameLabel;
    public UISprite m_ClickSprite;
    private int m_nClass =(int)ConsignSale_SearchClass.INVAILD_TYPE;
    public int Class
    {
        get { return m_nClass; }
        set { m_nClass = value; }
    }

    public void SetNameInfo(string strName)
    {
        m_NameLabel.text = strName;
    }

    public  void ClickClassBt()
    {
        if (ConsignSaleLogic.Instance() !=null)
        {
            ConsignSaleLogic.Instance().ProcessClickSearchBt(m_nClass,(int)ConsignSale_SearchSubClass.ALL);
            m_ClickSprite.gameObject.SetActive(true);
        }
    }
	// Use this for initialization
	void Start () {
	
	}
}
