using UnityEngine;
using System.Collections;

public class PKNoticeLogic : MonoBehaviour
{
    public TweenAlpha m_tweenAlpha;
    private float m_fLastBeginTime =0.0f;
    private float m_fDurationTime = 0.0f;
	// Use this for initialization
    private static PKNoticeLogic m_Instance = null;
    public static PKNoticeLogic Instance()
    {
        return m_Instance;
    }
    // Use this for initialization
    void Awake()
    {
        m_Instance = this;
    }

    void OnDestroy()
    {
        m_Instance = null;
    }
	void Start () 
    {
	    gameObject.SetActive(false);
    }

    public void PlayPkNotice(float fDurationTime)
    {
        m_tweenAlpha.Reset();
        gameObject.SetActive(true);
        m_fLastBeginTime = Time.time;
        m_fDurationTime = fDurationTime;
        m_tweenAlpha.enabled = true;
    }
	// Update is called once per frame
	void Update () 
    {
	    if (Time.time-m_fLastBeginTime >=m_fDurationTime)
	    {
            gameObject.SetActive(false);
            m_tweenAlpha.enabled = false;
	    }
	}
}
