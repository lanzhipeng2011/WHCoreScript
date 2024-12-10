using UnityEngine;
using System.Collections;

public class LightningBolt : MonoBehaviour
{
	
	// Use this for initialization
	void Start ()
	{
		if (null != renderer)
        {
            Material newMat = renderer.material;
            if (null != newMat)
            {
                newMat.SetFloat("_StartSeed", Random.value * 1000);
                renderer.material = newMat;
            }
        }
		
	}
}

