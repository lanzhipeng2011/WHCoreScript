using UnityEngine;
using System.Collections;
using Games.LogicObj;
public class Obj_ZhaLan :  Obj {

	public static Obj_ZhaLan instance=null;
	public static Obj_ZhaLan GetInstance()
	{
		return instance;
	}
	// Use this for initialization
	void Start () 
	{
		instance = this;
	}
	public void playdeath()
	{
		Destroy ();
//		CZLanimation ani=this.gameObject.GetComponentInChildren<CZLanimation>();
//		if(ani!=null)
//		{
//			ani.playanimation();
//			PlayEffect(301);
//			Invoke("Destroy",1.0f);
//		}

	}
	void Destroy()
	{
		GameObject.DestroyImmediate (this.gameObject);
	}
	// Update is called once per frame
	void Update () 
	{
	
	}
}
