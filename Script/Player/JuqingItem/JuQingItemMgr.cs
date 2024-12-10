using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GCGame.Table;
using Games.Mission;
using Module.Log;
using Games.LogicObj;
using Games.GlobeDefine;
using Games.Events;

public class JuQingItemMgr : Singleton<CollectItem>
{
	public Dictionary<int,Obj> m_objJuqingItemPools;
	public JuQingItemMgr()
	{
		CleanUp();
	}
	public void CleanUp()
	{

	}
	// 创建item
	public void InitJuqingItem(int nSceneID)
	{
		if (nSceneID < 0)
		{
			return;
		}
		
		CleanUp();
	   
		
		List<Tab_JuqingItem> JuqingIemList = TableManager.GetJuqingItemByID(nSceneID);
		if (JuqingIemList == null)
		{
			return;
		}
		int diff = GameManager.copyscenedifficult+2;
		for (int i=0; i<diff; i++) 
		{
			Tab_JuqingItem JuqingItem=JuqingIemList[i];
			if (JuqingItem != null)
			{
				
				string strName = "JuqingItem" + JuqingItem.Index.ToString();
				if (IsItemExist(strName))
				{
					continue;
				}
				
				Singleton<ObjManager>.GetInstance().CreateJuqingItem(JuqingItem, strName, JuqingItem.Index);
				
			}

		}
	
	}
	bool IsItemExist(string strName)
	{
		if (strName == null)
		{
			return false;
		}
		
		GameObject gItemObj = Singleton<ObjManager>.GetInstance().FindOtherGameObj(strName);
		if (gItemObj)
		{
			return true;
		}
		
		return false;
	}

		
}

