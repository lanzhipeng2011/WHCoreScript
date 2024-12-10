using UnityEngine;
using System.Collections;
using GCGame;

public class PVPRecordListItem : MonoBehaviour {

	public UILabel LabelTest;

//	private PVPRecordWindow m_parent;
	
	public static PVPRecordListItem CreateItem(GameObject grid, GameObject resItem, string name, PVPRecordWindow parent, string text)
	{
        GameObject curItem = Utils.BindObjToParent(resItem, grid, name);
        if (null != curItem)
        {
            PVPRecordListItem curItemComponent = curItem.GetComponent<PVPRecordListItem>();
            if (null != curItemComponent)
                curItemComponent.SetData(parent, text);

            return curItemComponent;
        }

        return null;
	}
	
	void SetData(PVPRecordWindow parent, string text)
	{
		LabelTest.text = text;
//		m_parent = parent;
	}
}
