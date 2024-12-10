/********************************************************************
	日期:	2014/03/13
	文件: 	CentreNoticeLogic.cs
	路径:	D:\work\code\mldj\Version\Main\Project\Client\Assets\MLDJ\Script\GUI\CentreNoticeLogic.cs
	作者:	YangXin
	描述:	屏幕中间提示
	修改:	
*********************************************************************/
using UnityEngine;
using System.Collections;
using GCGame.Table;

public class CentreNoticeLogic : MonoBehaviour {

    public UILabel[] m_News;
    public GameObject[] m_NewsObject;
    private Transform[] m_ObjTransform = new Transform[3];        //缓存Transform，提高运行效率,必须在Awake的时候就进行赋值
    private float[] m_fNewsTime = new float[3];
    private int m_nTimeMax = 5;
   
	// Use this for initialization
	void Start () {
        for (int i = 0; i < m_NewsObject.Length; i++)
        {
            m_NewsObject[i].SetActive(false);
            m_ObjTransform[i] = m_NewsObject[i].transform;
            m_fNewsTime[i] = Time.realtimeSinceStartup;
        }
        //InvokeRepeating("DoSomeThing", 0, 0.2f);
	}
    // Update is called once per frame
    void Update()
    {
        string curData = GUIData.GetNotifyData();
        if (null != curData)
        {
            AddNotice(curData);
        }
        for (int i = 0; i < m_NewsObject.Length; i++)
        {
            if (m_NewsObject[i].activeSelf)
            {
                if (Time.realtimeSinceStartup - m_fNewsTime[i] > m_nTimeMax)
                {
                    m_fNewsTime[i] = Time.realtimeSinceStartup;
                    m_NewsObject[i].SetActive(false);
                }
            }

        }        
    }
    private void AddNotice(string notice)
    {
        for (int i = 0; i < m_ObjTransform.Length; i++)
        {
            Vector3 pos = m_ObjTransform[i].localPosition;
            pos.y += 30;
            if (pos.y > 241)
            {
                pos.y = 180;
                m_News[i].text = notice;
                m_NewsObject[i].SetActive(true);
                m_fNewsTime[i] = Time.realtimeSinceStartup;
            }
            m_ObjTransform[i].localPosition = pos;
        }             
    }

}

