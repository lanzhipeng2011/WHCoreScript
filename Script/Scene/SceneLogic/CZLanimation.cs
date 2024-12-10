using UnityEngine;
using System.Collections;

public class CZLanimation : MonoBehaviour
{

	    public GameObject left;
     	public GameObject right;
	    public GameObject  third=null;
		void Start ()
		{
	      
		}
	     public void playanimation()
	    {
		if(left!=null)
		left.animation.Play ();
		if(right!=null)
		right.animation.Play ();
		if(third!=null)
			third.animation.Play ();
	    }
		// Update is called once per frame
		void Update ()
		{
	
		}
}

