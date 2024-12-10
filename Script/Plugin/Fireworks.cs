using UnityEngine;
using System.Collections;
using Module.Log;
using System.Collections.Generic;

public class Fireworks : MonoBehaviour {

    public class ParticalData
    {
        public ParticalData(GameObject newObj, float duration)
        {

            m_startTime = Time.realtimeSinceStartup;
            m_duration = duration;
            m_curObj = newObj;
        }

        public bool isActive()
        {
            return m_curObj.activeSelf;
        }

        public void CheckActive()
        {
            if (!m_curObj.activeSelf)
            {
                return;
            }
            if (Time.realtimeSinceStartup - m_startTime >= m_duration)
            {
                m_curObj.SetActive(false);
            }
        }

        public void ReActive(float duration)
        {
            m_curObj.SetActive(true);
            m_startTime = Time.realtimeSinceStartup;
            m_duration = duration;
          
        }

        public void DeActive()
        {
            m_curObj.SetActive(false);
        }
        public float m_startTime;
        public float m_duration;
        public GameObject m_curObj;
    }
    
    public GameObject m_ResObj;
    public float[] m_delays;
    public float[] m_durations;
    public Vector3[] m_rotations;
    public Vector3[] m_positions;


    private Transform m_Trans;
    private int m_totalNum = 0;
    private int m_curIndex = 0;
    private float m_curDelay = 0;

    private List<ParticalData> m_usingObj = new List<ParticalData>();
	// Use this for initialization
	void Awake () 
    {
        m_Trans = transform;
	}


    void OnEnable()
    {
        if (null == m_ResObj)
        {
            LogModule.ErrorLog("must set a res Obj");
            return;
        }
        
        m_totalNum = m_delays.Length;
        m_curIndex = -1;
        if (m_rotations.Length != m_totalNum || m_positions.Length != m_totalNum)
        {
            LogModule.ErrorLog("data array lenth error");

            return;
        }

        if (m_totalNum <= 0)
        {
            return;
        }

        m_curDelay = m_delays[0];
        m_curIndex = 0;
        for (int i = 0; i < m_usingObj.Count; i++)
        {
            m_usingObj[i].DeActive();
        }

    }
	// Update is called once per frame
	void Update () 
    {
        if (m_curIndex >= m_totalNum || m_curIndex < 0)
        {
            return;
        }

        m_curDelay -= Time.deltaTime;
        if (m_curDelay > 0)
        {
            return;
        }

        // 添加一个
        if (m_usingObj.Count > 0)
        {
            Debug.Log(m_usingObj[0].m_startTime);
        }
        
        int disableObjIndex = -1;

        for (int i = 0; i < m_usingObj.Count; i++)
        {
            m_usingObj[i].CheckActive();
            if (disableObjIndex < 0 && !m_usingObj[i].isActive())
            {
                disableObjIndex = i;

            }
        }


        if (disableObjIndex < 0)
        {
           
            GameObject newObj = GetNewObj(m_curIndex);
            if (null == newObj)
            {
                return;
            }
            ParticalData curData = new ParticalData(newObj, m_durations[m_curIndex]);
            m_usingObj.Add(curData);
            curData.ReActive(m_durations[m_curIndex]);
        }
        else
        {
           
            SetObjInfo(m_curIndex, m_usingObj[disableObjIndex].m_curObj);
            m_usingObj[disableObjIndex].ReActive(m_durations[m_curIndex]);
            
        }

        m_curIndex++;

        if (m_curIndex < m_totalNum)
        {
            m_curDelay = m_delays[m_curIndex];
        }
    }


    GameObject GetNewObj(int index)
    {
        if (index < 0 || index >= m_totalNum)
        {
            return null;
        }
        GameObject obj = GameObject.Instantiate(m_ResObj) as GameObject;
        if (null == obj)
        {
            return null;
        }

        obj.transform.parent = m_Trans;
        SetObjInfo(index, obj);
        return obj;
    }

    void SetObjInfo(int index, GameObject curObj)
    {
        if (index < 0 || index >= m_totalNum || null == curObj)
        {
            return;
        }
        curObj.transform.localPosition = m_positions[index];
        curObj.transform.localRotation = Quaternion.Euler(m_rotations[index].x, m_rotations[index].y, m_rotations[index].z);
    }
}
