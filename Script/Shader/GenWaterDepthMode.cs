using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent (typeof(Camera))]

public class GenWaterDepthMode : MonoBehaviour {

	void OnEnable () {
		camera.depthTextureMode |= DepthTextureMode.Depth;	
	}

}
