using UnityEngine;
using System.Collections;

public class ClipSlider : MonoBehaviour {

    public UIPanel m_LoadingSliderPanel;
    public GameObject m_LoadingEffect;

    public float m_clipStartPos;
    public float m_clipEndPos;
    public float m_effectStartPos;

    private Vector4 m_vecClipRange;
    private Vector3 m_vecEffectPos;
    private float m_nextProgressValue = 0;
    public float m_curSpeed = 0.03f;
    private float m_curProgressValue = 0;

	// Use this for initialization
	void Start () {
        m_vecClipRange = m_LoadingSliderPanel.clipRange;
        m_vecEffectPos = m_LoadingEffect.transform.localPosition;
        SetProgress(0);
	}

    void Update()
    {
        //
        float progressValueDiff = m_nextProgressValue - m_curProgressValue;

        if (progressValueDiff > 0)
        {
            if (m_curSpeed > progressValueDiff)
            {
                m_curProgressValue += progressValueDiff;

            }
            else
            {
                m_curProgressValue += m_curSpeed;
            }

            SetProgress(m_curProgressValue);
        }
    }

    public void SetProgress(float value)
    {
        float curLength = (m_clipEndPos - m_clipStartPos) * value;
        m_vecClipRange.x = curLength + m_clipStartPos;
        m_vecEffectPos.x = curLength + m_effectStartPos;

        m_LoadingEffect.transform.localPosition = m_vecEffectPos;
        m_LoadingSliderPanel.clipRange = m_vecClipRange;
    }

    public void SetNextProgress(float value)
    {
        m_nextProgressValue = value;
    }
}
