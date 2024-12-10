using UnityEngine;
using System.Collections;

public class SkillProgressLogic : MonoBehaviour {

    private static SkillProgressLogic m_Instance = null;
    public static SkillProgressLogic Instance()
    {
        return m_Instance;
    }
    public enum ProgressModel
    {
        INVALD_TYPE =-1,
        ORDERMODEL,
        REVERSEDMODE,
    }
    private float m_fContinueTime = 0.0f;
    private ProgressModel m_playModel;

    public GameObject m_FirstChild;

    private float m_fElapseTime = 0.0f;
    private float m_fRecordTime = 0.0f;
    public UISlider m_ProgressSlider;
    void Awake()
    {
       
    }
	// Use this for initialization
	void Start ()
	{
        m_Instance = this;
        m_ProgressSlider.value = 0;
        gameObject.SetActive(false);
        m_FirstChild.SetActive(true);
	}

    void OnDestroy()
    {
        m_Instance = null;
    }

    public void PlaySkillProgress(ProgressModel PlayModel,float fContinTime)
    {
        gameObject.SetActive(true);
        if (PlayModel ==ProgressModel.ORDERMODEL)
        {
            m_ProgressSlider.value = 0;
        }
        else if (PlayModel == ProgressModel.REVERSEDMODE)
        {
            m_ProgressSlider.value = 1;
        }
        m_playModel = PlayModel;
        m_fContinueTime = fContinTime;
        m_fElapseTime = 0.0f;
        m_fRecordTime = Time.time;
    }
	// Update is called once per frame
	void Update () 
    {
	    if (m_fContinueTime >0)
	    {
            m_fElapseTime = m_fElapseTime + (Time.time - m_fRecordTime);
            if (m_fContinueTime-m_fElapseTime <=0)
	        {
	            CloseWindow();
	        }
            else
            {
                if (m_playModel == ProgressModel.ORDERMODEL)
                {
                    m_ProgressSlider.value = m_fElapseTime / m_fContinueTime;
                }
                else if (m_playModel == ProgressModel.REVERSEDMODE)
                {
                    m_ProgressSlider.value = (m_fContinueTime-m_fElapseTime) / m_fContinueTime;
                }
               
            }
            m_fRecordTime = Time.time;
	    }
	}
    public void CloseWindow()
    {
       gameObject.SetActive(false);
       m_fContinueTime = 0.0f;
    }
}
