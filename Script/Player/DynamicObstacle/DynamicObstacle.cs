using UnityEngine;
using System.Collections;
using GCGame.Table;

public class DynamicObstacle {

	// Use this for initialization
	void Start () {
	
	}


    static public void HandleObstacle(GC_DYNAMICOBSTACLE_OPT packet)
    { 
        int nIndex = packet.Index;
        bool bFlag = (packet.Flag == 1 ? true : false);
        if(bFlag) // 创建
        {
            CreateObstacle(nIndex);
        }
        else
        {
            RemoveObstacle(nIndex);
        }
    }

    static void CreateObstacle(int nIndex)
    {
        Tab_DynamicObstacle obstacle = TableManager.GetDynamicObstacleByID(nIndex, 0);
        if (obstacle == null)
        {
            return;
        }
        string strName = "DyncObstacle" + nIndex;
        Singleton<ObjManager>.GetInstance().CreateDyncObstacle(obstacle,strName);
        return;
    }

    static void RemoveObstacle(int nIndex)
    {
        Tab_DynamicObstacle obstacle = TableManager.GetDynamicObstacleByID(nIndex, 0);
        if (obstacle == null)
        {
            return;
        }
        string strName = "DyncObstacle" + nIndex;

        Singleton<ObjManager>.GetInstance().RemoveOtherGameObj(strName);
    }

}
