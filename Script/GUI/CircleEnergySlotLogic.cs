using UnityEngine;
using System.Collections;

public class CircleEnergySlotLogic : MonoBehaviour {

    public delegate void OnEnergyFull(bool showEffect);

    public struct CircleEnergyInfo
    {
        public float zeroSpriteFill;
        public float maxSpriteFill;
        public float zeroEffectRotation;
        public float maxEffectRotation;

        public OnEnergyFull deleEnergyFull;   
    }

    public UISprite m_EnergySprite;
    public GameObject m_EffectRotation;
    public GameObject m_EnergyBackground;

    private float m_ZeroSpriteFill;
    private float m_MaxSpriteFill;
    private float m_ZeroEffectRotation;
    private float m_MaxEffectRotation;
    private OnEnergyFull m_deleEnergyFull;

	// Use this for initialization
	void Start () {
        m_EffectRotation.transform.localRotation = Quaternion.AngleAxis(0, Vector3.forward);
        m_EffectRotation.SetActive(false);
        m_EnergyBackground.SetActive(false);
	}

    public void InitInfo(CircleEnergyInfo energyInfo)
    {
        m_ZeroSpriteFill = energyInfo.zeroSpriteFill;
        m_MaxSpriteFill = energyInfo.maxSpriteFill;
        m_ZeroEffectRotation = energyInfo.zeroEffectRotation;
        m_MaxEffectRotation = energyInfo.maxEffectRotation;
        m_deleEnergyFull = energyInfo.deleEnergyFull;
    }

    public void UpdateEnergy(int cur, int max)
    {
        if (max == 0)
        {
            return;
        }

      //  m_EffectRotation.SetActive(true);

        float fFillAmount = (float)cur / (float)max * (m_MaxSpriteFill - m_ZeroSpriteFill) + m_ZeroSpriteFill;
        m_EnergySprite.fillAmount = fFillAmount;

        /*
        if (0.0f == m_MaxEffectRotation)
            return;

        float fAngel = m_ZeroEffectRotation + (float)cur / (float)max * m_MaxEffectRotation;
        m_EffectRotation.transform.localRotation = Quaternion.AngleAxis(fAngel, Vector3.forward);

        if (cur >= max)
        {
            m_EnergyBackground.SetActive(true);
            if(null != m_deleEnergyFull) m_deleEnergyFull(true);
        }
        else
        {
            m_EnergyBackground.SetActive(false);
            if (null != m_deleEnergyFull) m_deleEnergyFull(false);
        }*/
    }
}
