/********************************************************************
	创建时间:	2014/06/12 13:24
	全路径:		\TLOL\Version\Main\Project\Client\Assets\MLDJ\Script\GUI\ConsignSaleQualityMenu.cs
	创建人:		luoy
	功能说明:	寄售行品质菜单
	修改记录:
*********************************************************************/
using Games.ConsignSale;
using Games.Item;
using GCGame;
using GCGame.Table;
using Module.Log;
using UnityEngine;
using System.Collections;

public class ConsignSaleQualityMenu : MonoBehaviour {

	// Use this for initialization
    public GameObject m_GridObj;
    
	void Start ()
	{
	    CreateQualityItem();
	}
    void CreateQualityItem()
    {
        UIManager.LoadItem(UIInfo.ConsignSaleQualityItem, OnLoadQualityItem);
    }

    void OnLoadQualityItem(GameObject resObj, object param)
    {
        if (null == resObj)
        {
            LogModule.ErrorLog("OnLoadQualityItem error");
            return;
        }

        GameObject _gameObject = Utils.BindObjToParent(resObj, m_GridObj, "00all");
        if (null != _gameObject)
        {
            ConsignSaleQualityItem QualityItem = _gameObject.GetComponent<ConsignSaleQualityItem>();
            if (null != QualityItem)
            {
                QualityItem.SetLableName(StrDictionary.GetClientDictionaryString("#{1712}"));
                QualityItem.SetQuality((int)ItemQuality.QUALITY_INVALID);

                QualityItem = null;
                _gameObject = Utils.BindObjToParent(resObj, m_GridObj, "01white");
                if (null != _gameObject)
                {
                    QualityItem = _gameObject.GetComponent<ConsignSaleQualityItem>();
                }
                if (null != QualityItem)
                {
                    QualityItem.SetLableName(StrDictionary.GetClientDictionaryString("#{1713}"));
                    QualityItem.SetQuality((int)ItemQuality.QUALITY_WHITE);
                }

                QualityItem = null;
                _gameObject = Utils.BindObjToParent(resObj, m_GridObj, "02green");
                if (null != _gameObject)
                {
                    QualityItem = _gameObject.GetComponent<ConsignSaleQualityItem>();
                }
                if (null != QualityItem)
                {
                    QualityItem.SetLableName(StrDictionary.GetClientDictionaryString("#{1714}"));
                    QualityItem.SetQuality((int)ItemQuality.QUALITY_GREEN);
                }

                QualityItem = null;
                _gameObject = Utils.BindObjToParent(resObj, m_GridObj, "03blue");
                if (null != _gameObject)
                {
                    QualityItem = _gameObject.GetComponent<ConsignSaleQualityItem>();
                }
                if (null != QualityItem)
                {
                    QualityItem.SetLableName(StrDictionary.GetClientDictionaryString("#{1715}"));
                    QualityItem.SetQuality((int)ItemQuality.QUALITY_BLUE);
                }

                QualityItem = null;
                _gameObject = Utils.BindObjToParent(resObj, m_GridObj, "04puple");
                if (null != _gameObject)
                {
                    QualityItem = _gameObject.GetComponent<ConsignSaleQualityItem>();
                }
                if (null != QualityItem)
                {
                    QualityItem.SetLableName(StrDictionary.GetClientDictionaryString("#{1716}"));
                    QualityItem.SetQuality((int)ItemQuality.QUALITY_PURPLE);
                }

                QualityItem = null;
                _gameObject = Utils.BindObjToParent(resObj, m_GridObj, "05orange");
                if (null != _gameObject)
                {
                    QualityItem = _gameObject.GetComponent<ConsignSaleQualityItem>();
                }
                if (null != QualityItem)
                {
                    QualityItem.SetLableName(StrDictionary.GetClientDictionaryString("#{1717}"));
                    QualityItem.SetQuality((int)ItemQuality.QUALITY_ORANGE);
                }
            }
        }

        UIGrid gridObj = m_GridObj.GetComponent<UIGrid>();
        if (null != gridObj)
        {
            gridObj.sorted = true;
            gridObj.repositionNow = true;
        }
    }

}
