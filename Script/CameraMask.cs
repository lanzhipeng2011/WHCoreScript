using UnityEngine;
using System.Collections;
using Games;
using Games.LogicObj;

public class CameraMask : MonoBehaviour {
	
	//得到主人公
	private GameObject hero;
	//记录上次的对象
	private GameObject last_obj;
 
	void Start()
	{
		hero =null;
		
	}
	void Update()
	{
		if (hero == null) 
		{
			hero=ObjManager.GetInstance().MainPlayer.gameObject;
		}
		//为了调式时看的清楚画的线
		Debug.DrawLine(hero.transform.position,transform.position,Color.red);
		RaycastHit hit;
		
		if(Physics.Linecast(hero.transform.position,transform.position,out hit))
		{
			last_obj = hit.collider.gameObject;
			string name_tag = last_obj.tag;
			//判断
			if(name_tag != "MainCamera" && name_tag != "terrain"&&name_tag != "terrain")
			{
				//让遮挡物变半透明
				MeshRenderer obj_color = last_obj.GetComponentInChildren<MeshRenderer>();

				obj_color.enabled=false;
			}
		}//还原
		else if(last_obj != null)
		{
			MeshRenderer obj_color = last_obj.GetComponentInChildren<MeshRenderer>();
			obj_color.enabled=true;
			last_obj = null;
		}
		
	}
	
}