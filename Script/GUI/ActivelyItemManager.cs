using UnityEngine;
using System.Collections;
using GCGame.Table;

public class ActivelyItemManager : MonoBehaviour {

	public GameObject items;
	public GameObject itemGrid;
	public UILabel label;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void init(Tab_Activity acti)
	{
		label.text = acti.Describe;
		CleanUp ();

		int n = acti.getItemCount();
		for(int j = 0; j < n; j++)
		{
			int index = acti.GetItembyIndex( j );
			if( index != 0)
			{
				Tab_CommonItem come = TableManager.GetCommonItemByID( index, 0);
				GameObject item =  (GameObject)Instantiate(items);
				VipItem vip = item.GetComponent<VipItem>();
				vip.m_BonusImage.spriteName = come.Icon;
				vip.m_BonusText.text =  acti.GetNumbyIndex(j).ToString();//acti.getItemCount().ToString(); 
				vip.gameObject.name = come.Id.ToString();
				item.transform.parent = itemGrid.transform;
				item.transform.localPosition = Vector3.zero;
				item.transform.localScale = Vector3.one;
				
			}
		}

		itemGrid.GetComponent<UIGrid>().Reposition();
	}


	private void CleanUp()
	{
		foreach (Transform child in itemGrid.transform)
			Destroy (child.gameObject);
	}
}
