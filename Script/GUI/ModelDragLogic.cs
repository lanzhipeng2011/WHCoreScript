using UnityEngine;
using System.Collections;

public class ModelDragLogic : MonoBehaviour {

    private Transform m_ModelTrans;
    public Transform ModelTrans
    {
        get { return m_ModelTrans; }
        set { m_ModelTrans = value; }
    }

    private float m_RotateSpeed = 5;
    public float RotateSpeed
    {
        get { return m_RotateSpeed; }
        set { m_RotateSpeed = value; }
    }

	// Use this for initialization
	void Start () {
	
	}

    void OnDrag(Vector2 delta)
    {
        if (delta.x > 0)
        {
            if (m_ModelTrans != null)
            {
                m_ModelTrans.Rotate(Vector3.up * -m_RotateSpeed, Space.Self);
            }            
        }
        else
        {
            if (m_ModelTrans != null)
            {
                m_ModelTrans.Rotate(Vector3.up * m_RotateSpeed, Space.Self);
            }
        }
    }
}
