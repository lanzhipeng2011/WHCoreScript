using UnityEngine;
using System.Collections;

public class RoleChangeGd : MonoBehaviour 
{
	public GameObject  oldparant;
	public GameObject  newparant;
	public GameObject  weapon;
	public int index;
	private bool isyidong=false;

	// Use this for initialization
	void Start () 
	{

		
		if (index == 1)
		{
			//weapon.transform.localScale=new Vector3(1,1,1);
			this.transform.localRotation = Quaternion.Euler (new Vector3 (0, 90, 0));
		}
		if (index == 0)
		{
			//weapon.transform.localScale=new Vector3(1,1,1);
			this.transform.localRotation = Quaternion.Euler (new Vector3 (0, 90, 0));
		}
		if (index == 2)
		{
			//weapon.transform.localScale=new Vector3(1,1,1);
			this.transform.localRotation = Quaternion.Euler (new Vector3 (0, 90, 0));
			GameObject obj=this.GetComponentInChildren<SkinnedMeshRenderer>().gameObject;
			//obj.renderer.material.shader=Shader.Find("Mobile/Bumped Specular");
//			obj.renderer.material.SetFloat("_GlossQR",10.0f);
//			obj.renderer.material.SetFloat("_NoramlQR",10.0f);
//			obj.renderer.material.SetFloat("_Cutoff",0.5f);

		}
		if (index == 3)
		{
//			GameObject obj=this.GetComponentInChildren<SkinnedMeshRenderer>().gameObject;
//			obj.renderer.material.shader=Shader.Find("MLDJ/MYBumpedSpecular");
//			if(oldparant!=null)
//			{
//			weapon=oldparant.transform.GetChild (0).gameObject;
//			weapon.transform.parent = newparant.transform;
//			oldparant.transform.DetachChildren ();
//			weapon.transform.localPosition = Vector3.zero;
//			weapon.transform.localScale = Vector3.one;
//			weapon.transform.localRotation =Quaternion.identity;
//			}
		}


		//this.transform.Translate (Vector3.left *4, Space.Self);
		this.transform.localScale = new Vector3 (0.8f,0.8f,0.8f);

		animation.Stop ();
		Invoke ("diaoyong", 0.1f);
	}
	void diaoyong()
	{
		if (index == 0 || index == 1) 
		{
			animation.PlayQueued ("yanwu");
			animation.PlayQueued ("yanwustand");
		} 
		else
		{
			animation.PlayQueued ("skill40");
			animation.PlayQueued ("Stand");
		}
	
		isyidong = true;
	}
	// Update is called once per frame
	void Update () 
	{

//		if(isyidong)
//		   this.transform.Translate (this.transform.right * 1 * Time.deltaTime);
	}
}
