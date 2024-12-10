/********************************************************************
	created:	2014/01/20
	created:	20:1:2014   10:42
	filename: 	MyBelleWindow.cs
	author:		王迪
	
	purpose:	美人收集系统——我的美人界面
*********************************************************************/
using UnityEngine;
using System.Collections;
using GCGame.Table;
using GCGame;
using System;
using Module.Log;
public class MyBelleWindow : MonoBehaviour {

    public GameObject ListGrid;
    public BelleMatrixWindow m_BelleMatrixWindow;
//    public TweenPosition m_BelleListPopWindowTween;
	public GameObject m_BelleListPopWindow;
	public GameObject m_BelleMatrixListWindow;
    public GameObject m_ObjBelleSmallCardItem;
   
    //private GameObject m_BelleSmallCardItemRes = null;
    void OnEnable()
    {
//        m_BelleListPopWindowTween.gameObject.transform.localPosition = m_BelleListPopWindowTween.to;
		m_BelleListPopWindow.SetActive (false);
		m_BelleMatrixListWindow.SetActive (true);
        BelleData.delEvolutionRapid += OnEvolution;
        //UpdateBelleData();
        UpdateBelleSmallCard();
    }


    void OnDisable()
    {
        BelleData.delEvolutionRapid -= OnEvolution;
    }

    void OnEvolution()
    {
        UpdateBelleData();
    }

    public void Show()
    {
		m_BelleListPopWindow.SetActive (true);
		m_BelleMatrixListWindow.SetActive (false);
        UpdateBelleData();
    }
    public void Hide()
    {
		m_BelleListPopWindow.SetActive (false);//.Play(true);
		m_BelleMatrixListWindow.SetActive (true);
    }
	void Start () 
    {
        //UpdateBelleSmallCard();
	}

    void UpdateBelleSmallCard()
    {
        if (null == m_ObjBelleSmallCardItem)
        {
            LogModule.ErrorLog("can not load res m_ObjBelleSmallCardItem :");
            return;
        }

        Utils.CleanGrid(ListGrid);
        // 显示所有BELLE
        foreach (int key in BelleData.OwnedBelleMap.Keys)
        {
            Tab_Belle curBelle = TableManager.GetBelleByID(key, 0);
            if (null != curBelle)
            {
                GameObject curItem = Utils.BindObjToParent(m_ObjBelleSmallCardItem, ListGrid);
                BelleSmallCardItem curItemScript = curItem.GetComponent<BelleSmallCardItem>();
                if (curItemScript)
                {
                    curItemScript.SetData(this, key, curBelle, 1);
                }
            }
        }

        ListGrid.GetComponent<UIGrid>().repositionNow = true;
        ListGrid.GetComponent<UITopGrid>().recenterTopNow = true;

    }


    // 通知窗口更新为当前ITEM
    public void SelectBelleItem(BelleSmallCardItem item)
    {
        if (null == item)
        {
            return;
        }

        m_BelleMatrixWindow.SelectRoleToMatrix(item.gameObject.name);

       
    }

    // 开启选择模式，选择模式下，行为与默认模式不一样
    public void EnableSelectMode(bool bEnable, string matrixID, string matrixIndex)
    {
        //ShowBelleItem(m_curSelectItem);
    }

    // 选择模式下，点击了Item
    public void OnSelectItemOk(BelleSmallCardItem item)
    {
        m_BelleMatrixWindow.SelectRoleToMatrix(item.gameObject.name);
    }

    void UpdateBelleData()
    {
        UpdateBelleSmallCard();
    }
}
