using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GCGame;
using Module.Log;

public class RelationNamePopController : UIControllerBase<RelationNamePopController> {

    public GameObject ListGrid;
	public bool m_isKq=false;
    public delegate void ChooseNameDelegate(ulong curRelationID, string recevierName);
    private ChooseNameDelegate m_delChooseName;
    private bool m_bPopWindow = true;
    private bool m_bRestaurantFriend = false;
    private Transform m_GridTranForm;
	private List<GameObject> nameItemList = new List<GameObject>();
	private int nameItemSlot = -1;

    void Awake()
    {
        SetInstance(this);
        m_GridTranForm = ListGrid.transform;
    }

	// Use this for initialization
	void Start () 
    {
        ResetItem();
	}

    public void SetIsPopWindow(bool bPop)
    {
        m_bPopWindow = bPop;
    }

    public void SetIsRestaurantFriend(bool bRestaurantFriend)
    {
        m_bRestaurantFriend = bRestaurantFriend;
    }

    public void ResetItem()
    {
        UIManager.LoadItem(UIInfo.RelationNameListItem, OnLoadItem);
    }

    void OnLoadItem(GameObject resItem, object param)
    {
        if (null == resItem)
        {
            LogModule.ErrorLog("load RelationNameListItem fail");
            return;
        }
        Utils.CleanGrid(ListGrid);
		nameItemSlot = -1;
		nameItemList.Clear ();
		Dictionary<ulong, Relation> relationList = GameManager.gameManager.PlayerDataPool.FriendList.RelationDataList;
		foreach (ulong key in relationList.Keys)
		{
//			if(m_isKq)
//			resItem.GetComponent<UIPanel>().depth=406;
//			else
//				resItem.GetComponent<UIPanel>().depth=390;
			GameObject nameItem = RelationNameListItem.CreateItem(ListGrid,resItem, key.ToString(), this, relationList[key]).gameObject;
			nameItemList.Add(nameItem);
		}
		ListGrid.GetComponent<UIGrid>().Reposition();
		ListGrid.GetComponent<UITopGrid>().Recenter(true);
		
		if (m_bRestaurantFriend)
		{
			UpdateRestaurantVisitState();
		}
	}
	
	void OnCloseClick()
	{
		m_bRestaurantFriend = false;
        UIManager.CloseUI(UIInfo.RelationNamePopWindow);
    }

    public void SetDelegate(ChooseNameDelegate delFun)
    {
        m_delChooseName = delFun;
    }
    public void OnItemClick(RelationNameListItem curItem)
    {
        if (m_bPopWindow)
        {
            m_bRestaurantFriend = false;
            UIManager.CloseUI(UIInfo.RelationNamePopWindow);
        }

        if (null != m_delChooseName)
        {
            ulong curID = 0;
            if (ulong.TryParse(curItem.gameObject.name, out curID))
            {
                Dictionary<ulong, Relation> relationList = GameManager.gameManager.PlayerDataPool.FriendList.RelationDataList;

                m_delChooseName(curID, relationList[curID].Name);

            }

        }
    }

    void UpdateRestaurantVisitState()
    {
         for (int i = 0; i < m_GridTranForm.childCount; i++)
        {
            RelationNameListItem item = m_GridTranForm.GetChild(i).GetComponent<RelationNameListItem>();
            if (item != null)
            {
                item.UpdateRestaurantVisitState();
            }
        }
    }

	public void ChooseReceiver(System.UInt64 receiverID, string recevierName)
	{
		if(nameItemSlot != -1)
		{
			nameItemList[nameItemSlot].GetComponent<RelationNameListItem>().SetItemFalse();
		}
		for (int slot = 0; slot < nameItemList.Count; slot++) 
		{
			if(nameItemList[slot].GetComponent<RelationNameListItem>().labelName.text == recevierName)
			{
				nameItemSlot = slot;
			}
		}
		nameItemList[nameItemSlot].GetComponent<RelationNameListItem>().SetItemHighLight();
	}
}
