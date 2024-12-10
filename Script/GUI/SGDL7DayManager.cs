using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GCGame.Table;

public class SGDL7DayManager : MonoBehaviour {

	public GameObject m_RewardItemObj;
	public GameObject m_RewardItemGrid;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// 7Day奖励按钮
	public void ButtonDL7DayAward()
	{
		CleanUp();
		Init();
	}

	private int tempIndex;
	private void Init()
	{
		tempIndex = 0;
		for(int i=0;i<50;i++)
		{
			List<Tab_Reward> _curveList = TableManager.GetRewardByID(i);
			if(_curveList != null && _curveList[0].Type == 4 )
			{
				GameObject rewardItem = (GameObject) Instantiate(m_RewardItemObj);
				rewardItem.name = "item0"+tempIndex;
				rewardItem.transform.parent = m_RewardItemGrid.transform;
				rewardItem.transform.localScale = Vector3.one;
				rewardItem.transform.localPosition = new Vector3(0f,93f - 150f * tempIndex,0f);
				tempIndex++;
				
				rewardItem.GetComponent<RewardItemManager>().initData(_curveList,3);
			}
		}
		m_RewardItemGrid.GetComponent<UIGrid> ().Reposition ();
	}

	private void CleanUp()
	{
		foreach (Transform child in m_RewardItemGrid.transform)
			Destroy (child.gameObject);
	}

}
