using UnityEngine;
using System.Collections;

public class SGBgFadeOut : MonoBehaviour {

	public UIPanel  m_pan;
	// Use this for initialization
	void Start () 
	{
		BoxCollider box=this.gameObject.AddComponent<BoxCollider> ();
		if (box != null) 
		{
			box.size=new Vector3(2000.0f,2000.0f,0);
			this.gameObject.tag="SubUI";
		}
		//DestroyImmediate (this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		m_pan.alpha -= 0.08f;
		if (m_pan.alpha <= 0)
			this.gameObject.SetActive (false);
	
	}
}
