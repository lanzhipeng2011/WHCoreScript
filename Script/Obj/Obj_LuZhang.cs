using UnityEngine;
using System.Collections;
using Games.LogicObj;
public class Obj_LuZhang : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(ObjManager.GetInstance().MainPlayer!=null)
		{
			if(this.gameObject!=null&&this.gameObject.GetComponentInChildren<NavMeshObstacle>()!=null)
			{
			   if(ObjManager.GetInstance().MainPlayer.AutoComabat)
			   {
				this.gameObject.GetComponentInChildren<NavMeshObstacle>().enabled=false;
			   }
			    else
			   {
				this.gameObject.GetComponentInChildren<NavMeshObstacle>().enabled=true;
			   }
			}
		}
	}
}
