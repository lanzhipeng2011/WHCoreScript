/********************************************************************
	created:	2014/01/15
	created:	15:1:2014   16:01
	filename: 	BelleIllustrateWindow.cs
	author:		王迪
	
	purpose:	美人图鉴窗口
*********************************************************************/
using UnityEngine;
using System.Collections;
using GCGame.Table;
using GCGame;
using Games.GlobeDefine;
using Module.Log;
using System;
using System.Collections.Generic;
public class BelleIllustrateWindow : MonoBehaviour {

    public GameObject ListGrid;
    public GameObject m_ObjBelleCardItem;
    // 新手指引
    private int m_NewPlayerGuide_Step = -1;

    private static List<int> m_BelleSortArray = new List<int>();
    
	// Use this for initialization
	void OnEnable ()
    {
		UpdateBelleCardItem();
		Check_NewPlayerGuide();
	}

    void UpdateBelleCardItem()
    {
        if (null == m_ObjBelleCardItem)
        {
            LogModule.ErrorLog("can not load res m_ObjBelleCardItem :");
            return;
        }

        Utils.CleanGrid(ListGrid);

        m_BelleSortArray.Clear();
        int m_curActiveBelle = 0;
        // 已经获得的美人排在前边
        foreach (int key in TableManager.GetBelle().Keys)
        {
            if(BelleData.OwnedBelleMap.ContainsKey(key))
            {
                m_BelleSortArray.Insert(m_curActiveBelle++, key);
            }
            else
            {
                m_BelleSortArray.Add(key);
            }
        }

        for (int i = 0, count = m_BelleSortArray.Count; i < count; i++ )
        {
            Tab_Belle curBelle = TableManager.GetBelleByID(m_BelleSortArray[i], 0);
            if (null != curBelle)
            {
                GameObject curItem = Utils.BindObjToParent(m_ObjBelleCardItem, ListGrid);

                if (null != curItem && null != curItem.GetComponent<BelleCardItem>())
                    curItem.GetComponent<BelleCardItem>().SetData(m_BelleSortArray[i], curBelle, BelleData.OwnedBelleMap.ContainsKey(m_BelleSortArray[i]));
            }
        }     

        ListGrid.GetComponent<UIGrid>().repositionNow = true;

        if (m_NewPlayerGuide_Step == 1)
        {
            if (null != ListGrid.transform.parent && null != ListGrid.transform.parent.GetComponent<UIDraggablePanel>())
                ListGrid.transform.parent.GetComponent<UIDraggablePanel>().scale = Vector3.zero;

            m_NewPlayerGuide_Step = -1;
        }
    }


    void Check_NewPlayerGuide()
    {
        if (BelleController.Instance())
        {
            int nStep = BelleController.Instance().NewPlayerGuide_Step;
			if ((int)GameDefine_Globe.NEWOLAYERGUIDE.MINGJIANG_QIMI == nStep)
            {
                NewPlayerGuide(1);
                BelleController.Instance().NewPlayerGuide_Step = -1;
            }
        }
    }

    public void NewPlayerGuide(int nIndex)
	{
		NewPlayerGuidLogic.CloseWindow();

		m_NewPlayerGuide_Step = nIndex;
        switch(nIndex)
        {
            case 0:
                break;
            case 1:
                if (ListGrid)
                {
                    Transform gItemTrans = ListGrid.transform.FindChild("1");
                    if (gItemTrans && gItemTrans.gameObject)
                    {
                        BelleCardItem BelleItem = gItemTrans.gameObject.GetComponent<BelleCardItem>();
                        if (BelleItem)
                        {
                            BelleItem.NewPlayerGuide(1);
                        }
                    }
                }
                break;
        }
    }
}
