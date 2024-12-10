using UnityEngine;
using System.Collections;
using GCGame;
using Module.Log;
using System;

public class RelationNameListItem : MonoBehaviour {

    public UILabel labelName;
    private RelationNamePopController m_parentWindow = null;
    public GameObject m_VisitSprite;
	public GameObject m_ChooseSprite;

    private UInt64 m_Guid;
    
    
	// Use this for initialization
	void Start () {
		m_ChooseSprite.SetActive (false);
	}

    public static RelationNameListItem CreateItem(GameObject grid, GameObject resItem, string name, RelationNamePopController parentWindow, Relation relationData)
    {
        GameObject curItem = Utils.BindObjToParent(resItem, grid);
        if (null == curItem)
        {
            LogModule.ErrorLog("CreateItem, curItem error!");

        }
        curItem.name = name;
        RelationNameListItem curItemComponent = curItem.GetComponent<RelationNameListItem>();
        if (null == curItemComponent)
        {
            LogModule.ErrorLog("relation list item error!");
            ResourceManager.DestroyResource(ref curItem);
            return null;
        }

        curItemComponent.SetData(parentWindow, relationData);
        return curItemComponent;
    }

    public void SetData(RelationNamePopController parentWindow, Relation relationData)
    {
        m_parentWindow = parentWindow;
        labelName.text = relationData.Name;
        m_Guid = relationData.Guid;
        m_VisitSprite.SetActive(false);
    }

	void OnItemClick()
    {
		if (null != m_parentWindow) 
		{
			m_ChooseSprite.SetActive(true);
			m_parentWindow.OnItemClick(this);
		}
    }

    public void UpdateRestaurantVisitState()
    {
        if (null == RestaurantData.m_PlayerRestaurantInfo ||
            null == RestaurantData.m_PlayerRestaurantInfo.m_VisitFrindList )
        {
            return;
        }
        int nCount = RestaurantData.m_PlayerRestaurantInfo.m_VisitFrindList.Count;
        for (int i = 0; i < nCount; i++)
        {
            if (m_Guid == RestaurantData.m_PlayerRestaurantInfo.m_VisitFrindList[i])
            {
                m_VisitSprite.SetActive(true);
            }
        }
    }

	public void SetItemFalse()
	{
		if (null != m_parentWindow) 
		{
			m_ChooseSprite.SetActive(false);
		}
	}

	public void SetItemHighLight()
	{
		if (null != m_parentWindow) 
		{
			m_ChooseSprite.SetActive(true);
		}
	}
}
