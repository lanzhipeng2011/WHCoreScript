using UnityEngine;
using System.Collections;

public class BoomChain : MonoBehaviour {

    public GameObject m_BoomObject;
    public float m_DelayMax = 0.5f;
    public float m_DelayMin = 0.4f;
    public int m_Count = 1;
    public float m_DistanceMax = 5;
    public float m_DistanceMin = 1;
    public float m_Direction = 0;


    private int m_curIndex = 0;
    private float m_timer = 0;
    private GameObject[] m_objArray = null;

	// Use this for initialization
	void OnEnable () {
        m_curIndex = 0;
        m_timer = 0;

        if(null == m_objArray)
        {
            m_objArray = new GameObject[m_Count];

            for(int i=0; i<m_objArray.Length; i++)
            {
                m_objArray[i] = GameObject.Instantiate(m_BoomObject) as GameObject;
                m_objArray[i].transform.parent = transform;
                m_objArray[i].transform.localRotation = m_BoomObject.transform.localRotation;
            }
        }
        float curX = 0;
        float curY = 0;
        for(int i=0; i<m_objArray.Length; i++)
        {
            float curLen = Random.Range(m_DistanceMin, m_DistanceMax);
            curX += Mathf.Cos(m_Direction) * curLen;
            curY += Mathf.Sin(m_Direction) * curLen;
            m_objArray[i].transform.localPosition = new Vector3(curX, 0, curY);
            
            m_objArray[i].SetActive(false);
        }
   
	}

    // Update is called once per frame
	void Update()
    {
        if (m_curIndex >= m_objArray.Length || m_curIndex < 0)
        {
            return;
        }

        m_timer -= Time.deltaTime;
        if (m_timer > 0)
        {
            return;
        }

        m_timer = Random.Range(m_DelayMin, m_DelayMax);

        if (null != m_objArray)
            m_objArray[m_curIndex++].SetActive(true);
	}
}
