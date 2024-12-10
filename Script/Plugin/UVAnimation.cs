/********************************************************************
	created:	2013/12/20
	created:	20:12:2013   15:48
	filename: 	UVAnimation
	file ext:	cs
	author:		王迪
	
	purpose:	UV动画
*********************************************************************/

using UnityEngine;
using System.Collections;

public class UVAnimation : MonoBehaviour {

    public float speed = 5.0f;
    public float countX = 4.0f;
    public float countY = 4.0f;

    private float offsetX = 0;
    private float offsetY = 0;
    private Vector2 singleTexSize;

	// Use this for initialization
	void Start () {
        singleTexSize = new Vector2(1.0f / countX, 1.0f / countY);
        renderer.material.mainTextureScale = singleTexSize;
	}
	
	// Update is called once per frame
	void Update () {
        /*
        if(frames != null && gameObject.renderer.material != null)
        {
            int index= (int)((Time.time * duration)) % frames.Length;
            gameObject.renderer.material.mainTexture = frames[index];
        }
         * */

        float frame = Mathf.Floor(Time.time * speed);
        offsetX = frame / countX;
        offsetY = -(frame - frame % countX) / countY / countX;
        renderer.material.SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY));
	    
	}
}
