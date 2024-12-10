/********************************************************************
	created:	2014/03/14
	created:	14:3:2014   16:50
	filename: 	NPCPresentChild.cs
	author:		王迪
	
	purpose:	NPC编辑器编辑类子结点，用户表现方向
*********************************************************************/
using UnityEngine;
using System.Collections;

public class NPCPresentChild : MonoBehaviour {

	// Use this for initialization

    public Vector3 m_cubeSize = new Vector3(1, 1, 1);
    private float m_height = 20.0f;
    private Vector3 m_localPosCache;
    private Color m_color = Color.yellow;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDrawGizmos()
    {
        m_localPosCache = transform.position;
      //  m_localPosCache.y += m_height;
        Gizmos.color = m_color;
        Gizmos.DrawCube(m_localPosCache, m_cubeSize);
    }

    public void SetHeight(float h)
    {
        m_height = h;
    }

    public void SetColor(Color color)
    {
        m_color = color;
    }
}
