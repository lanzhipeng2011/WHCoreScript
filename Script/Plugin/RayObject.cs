using UnityEngine;
using System.Collections;

public class RayObject : MonoBehaviour {

    public Transform m_TargetTransform;
    public float m_Speed = 1.0f;

    private static Vector3 m_dir = new Vector3(0, 0, 1);
	// Use this for initialization
	void OnEnable () {
        m_TargetTransform.localPosition = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
        m_TargetTransform.localPosition += m_dir * m_Speed;
	}
}
