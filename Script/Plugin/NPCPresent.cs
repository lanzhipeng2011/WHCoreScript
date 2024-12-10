/********************************************************************
	created:	2014/03/14
	created:	14:3:2014   16:49
	filename: 	NPCPresent.cs
	author:		王迪
	purpose:	NPC表现类
*********************************************************************/	
using UnityEngine;
using System.Collections;

public class NPCPresent : MonoBehaviour {

    public NPCPresentChild dirChild;
    public float m_terrainHeight = 20.0f;
    public Color m_curColor = Color.white;
    public string NPCName = "NPC";

    private Vector3 m_localPosCache;
    //private Vector3 m_headPosCache = new Vector3(0, 0, 1);
    private float m_radius = 1;
    
    public int DataID = 0;
	void Start () {
        SetRadius(1);
        SetHeight(m_terrainHeight);
       
	}

    void OnDrawGizmos()
    {
        SetColor(m_curColor);
        m_localPosCache = transform.position;
      //  m_localPosCache.y = 0;
        transform.position = m_localPosCache;
       // m_localPosCache.y+= m_terrainHeight;

        Vector3 curEularRot = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0, curEularRot.y, 0);
        Gizmos.color = m_curColor;
        Gizmos.DrawSphere(m_localPosCache, m_radius);
        
    }

	public void SetRadius(float r)
    {
        if (null != dirChild)
        {
            m_radius = r;
            Vector3 childPos = dirChild.transform.localPosition;
            childPos.z = m_radius;
            dirChild.transform.localPosition = childPos;
        }
    }

	public void SetHeight(float h)
    {
        if (null != dirChild)
        {
            m_terrainHeight = h;
            dirChild.SetHeight(h);
        }
    }

	public void SetColor(Color color)
    {
        if (null != dirChild)
        {
            m_curColor = color;
            dirChild.SetColor(color);
        }
    }
}
