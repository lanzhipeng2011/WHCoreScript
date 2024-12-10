using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActivityRechargeData : MonoBehaviour {


	public class RechargeData
	{
		public int types;
		public int Numbers;
		public string startTime;
		public string endTime;
	}

	public static Dictionary<int, RechargeData> UserRechargeDataMap = new Dictionary<int, RechargeData>();
	
	public static void UpdateLeiChongData(GC_RET_ACTIVE_LEICHONG data)
	{
		RechargeData curData = null;
		for(int i= 0; i < data.typeCount; i++)
		{
			if (UserRechargeDataMap.ContainsKey((int)data.typeList[i]))
			{
				curData = UserRechargeDataMap[(int)data.typeList[i]];
				curData.types = (int)data.typeList[i];
				curData.Numbers = (int)data.numList[i];
				curData.startTime = data.startdayList[i];
				curData.endTime = data.enddayList[i];

			}
			else
			{
				curData = new RechargeData();
				curData.types = (int)data.typeList[i];
				curData.Numbers = (int)data.numList[i];
				curData.startTime = data.startdayList[i];
				curData.endTime = data.enddayList[i];

				UserRechargeDataMap.Add((int)data.typeList[i], curData);
			}
		}
		//===
		if(ActivityLogic.Instance() != null)
			ActivityLogic.Instance ().OnDateChange ();



		
	}
}
