using UnityEngine;
using System.Collections;
using Games.LogicObj;
using Module.Log;
using GCGame.Table;

public class SGBiograyhyItemManager : MonoBehaviour {

	public UISprite m_isAlreadyReach;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void SetData(int index)
	{

		Tab_BiographyItem itemTab = TableManager.GetBiographyItemByID(index,0);

		SetAlreadyReachSp(itemTab.BeginLevel,itemTab.EndLevel);
	}

	void SetAlreadyReachSp(int beginLevel,int endLevel)
	{
		Obj_MainPlayer mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
		if (null == mainPlayer)
		{
			LogModule.ErrorLog("BiographyController SetAlreadyReachSp mainPlayer empty");
			return;
		}
		
		if (/*(mainPlayer.BaseAttr.Level >= endLevel) ||*/ (mainPlayer.BaseAttr.Level >= beginLevel && mainPlayer.BaseAttr.Level <= endLevel))
		{
			if (m_isAlreadyReach != null)
				NGUITools.SetActive(m_isAlreadyReach.gameObject,true);
		}
		else 
		{
			if (m_isAlreadyReach != null)
				NGUITools.SetActive(m_isAlreadyReach.gameObject, false);
		}
	}
}
