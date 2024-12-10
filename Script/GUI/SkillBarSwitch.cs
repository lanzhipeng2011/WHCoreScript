using UnityEngine;
using System.Collections;

public class SkillBarSwitch : MonoBehaviour {

    enum SKILL_BAR_STATE
    {
        SKILL_BAR_STATE_BASIC           = 0,
        SKILL_BAR_STATE_EX              = 1,
    }

    enum SWITCH_DIRECTION
    {
        SWITCH_TO_LEFT                  = -1,
        SWITCH_NO                       = 0,
        SWITCH_TO_RIGHT                 = 1,
    }

    private SKILL_BAR_STATE m_eCurSkillBarState = SKILL_BAR_STATE.SKILL_BAR_STATE_BASIC;
    private SWITCH_DIRECTION m_SwitchDirection = SWITCH_DIRECTION.SWITCH_NO; 

    private bool m_SwitchGo = false;
    private int m_TouchID = -1;

    public GameObject m_SkillBarGroup1;
    public GameObject m_SkillBarGroup2;

    public Vector3 posReady;
    public float speedSwitch;      

	// Use this for initialization
	void Start () {
        InitSkillBarGroup();
	}
	
	// Update is called once per frame
	void Update () {
        if (m_SwitchGo)
	    {
            SkillBarSwitchGo();
	    }
	    
	}

    public void Press(bool pressed)
    {
        if (!pressed && m_TouchID == UICamera.currentTouchID)
        {
            m_TouchID = -1;
        }
    }

    public void Drag(Vector2 delta)
    {
        if (!m_SwitchGo)
        {
            if (m_TouchID == -1)
            {
                m_TouchID = UICamera.currentTouchID;
            }
            if (delta.x > 0 && delta.y > 0)
            {
                BeforeSkillBarSwitch(SWITCH_DIRECTION.SWITCH_TO_RIGHT);
            }
            else if (delta.x < 0 && delta.y < 0)
            {
                BeforeSkillBarSwitch(SWITCH_DIRECTION.SWITCH_TO_LEFT);
            }
        }
    }

    void InitSkillBarGroup()
    {
        if (m_SkillBarGroup1 != null && m_SkillBarGroup2 != null)
        {
            if (m_eCurSkillBarState == SKILL_BAR_STATE.SKILL_BAR_STATE_BASIC)
            {
                m_SkillBarGroup1.transform.localPosition = Vector3.zero;
                m_SkillBarGroup2.transform.localPosition = new Vector3(-posReady.y, -posReady.x, 0);
            }
            else if (m_eCurSkillBarState == SKILL_BAR_STATE.SKILL_BAR_STATE_EX)
            {
                m_SkillBarGroup1.transform.localPosition = new Vector3(-posReady.y, -posReady.x, 0);
                m_SkillBarGroup2.transform.localPosition = Vector3.zero;
            }
        }
    }

    void BeforeSkillBarSwitch(SWITCH_DIRECTION direction)
    {
        m_SwitchDirection = direction;
        m_SwitchGo = true;
        m_SkillBarGroup1.SetActive(true);
        m_SkillBarGroup2.SetActive(true);

        Vector3 posReadyRight = posReady;
        Vector3 posReadyLeft = new Vector3(-posReady.y, -posReady.x, 0);

        if (m_eCurSkillBarState == SKILL_BAR_STATE.SKILL_BAR_STATE_BASIC)
        {
            m_eCurSkillBarState = SKILL_BAR_STATE.SKILL_BAR_STATE_EX;
            if (direction == SWITCH_DIRECTION.SWITCH_TO_LEFT)
            {
                m_SkillBarGroup2.transform.localPosition = posReadyRight;
            }
            else if (direction == SWITCH_DIRECTION.SWITCH_TO_RIGHT)
            {
                m_SkillBarGroup2.transform.localPosition = posReadyLeft;
            }
            return;
        }
        else if (m_eCurSkillBarState == SKILL_BAR_STATE.SKILL_BAR_STATE_EX)
        {
            m_eCurSkillBarState = SKILL_BAR_STATE.SKILL_BAR_STATE_BASIC;
            if (direction == SWITCH_DIRECTION.SWITCH_TO_LEFT)
            {
                m_SkillBarGroup1.transform.localPosition = posReadyRight;
            }
            else if (direction == SWITCH_DIRECTION.SWITCH_TO_RIGHT)
            {
                m_SkillBarGroup1.transform.localPosition = posReadyLeft;
            }
            return;
        }
    }

    void SkillBarSwitchGo()
    {
        Vector3 posReadyRight = posReady;
        Vector3 posReadyLeft = new Vector3(-posReady.y, -posReady.x, 0);
        Vector3 speedSwitchRight = posReadyRight / 100f;
        Vector3 speedSwitchLeft = -posReadyLeft / 100f;

        if (m_eCurSkillBarState == SKILL_BAR_STATE.SKILL_BAR_STATE_BASIC)
        {
            if (m_SwitchDirection == SWITCH_DIRECTION.SWITCH_TO_LEFT)
            {
                m_SkillBarGroup1.transform.localPosition += (float)m_SwitchDirection * speedSwitch * Time.deltaTime * speedSwitchRight;
                m_SkillBarGroup2.transform.localPosition += (float)m_SwitchDirection * speedSwitch * Time.deltaTime * speedSwitchLeft;

                if (m_SkillBarGroup1.transform.localPosition.x <= 0 && m_SkillBarGroup1.transform.localPosition.y <= 0)
                {
                    m_SkillBarGroup1.transform.localPosition = Vector3.zero;
                    m_SkillBarGroup2.transform.localPosition = posReadyLeft;
                    m_SkillBarGroup2.SetActive(false);
                    m_SwitchGo = false;
                }
            }
            else if (m_SwitchDirection == SWITCH_DIRECTION.SWITCH_TO_RIGHT)
            {
                m_SkillBarGroup1.transform.localPosition += (float)m_SwitchDirection * speedSwitch * Time.deltaTime * speedSwitchLeft;
                m_SkillBarGroup2.transform.localPosition += (float)m_SwitchDirection * speedSwitch * Time.deltaTime * speedSwitchRight;

                if (m_SkillBarGroup1.transform.localPosition.x >= 0 && m_SkillBarGroup1.transform.localPosition.y >= 0)
                {
                    m_SkillBarGroup1.transform.localPosition = Vector3.zero;
                    m_SkillBarGroup2.transform.localPosition = posReadyRight;
                    m_SkillBarGroup2.SetActive(false);
                    m_SwitchGo = false;
                }
            }
        }
        else if (m_eCurSkillBarState == SKILL_BAR_STATE.SKILL_BAR_STATE_EX)
        {
            if (m_SwitchDirection == SWITCH_DIRECTION.SWITCH_TO_LEFT)
            {
                m_SkillBarGroup1.transform.localPosition += (float)m_SwitchDirection * speedSwitch * Time.deltaTime * speedSwitchLeft;
                m_SkillBarGroup2.transform.localPosition += (float)m_SwitchDirection * speedSwitch * Time.deltaTime * speedSwitchRight;

                if (m_SkillBarGroup2.transform.localPosition.x <= 0 && m_SkillBarGroup2.transform.localPosition.y <= 0)
                {
                    m_SkillBarGroup1.transform.localPosition = posReadyLeft;
                    m_SkillBarGroup2.transform.localPosition = Vector3.zero;
                    m_SkillBarGroup1.SetActive(false);
                    m_SwitchGo = false;
                }
            }
            else if (m_SwitchDirection == SWITCH_DIRECTION.SWITCH_TO_RIGHT)
            {
                m_SkillBarGroup1.transform.localPosition += (float)m_SwitchDirection * speedSwitch * Time.deltaTime * speedSwitchRight;
                m_SkillBarGroup2.transform.localPosition += (float)m_SwitchDirection * speedSwitch * Time.deltaTime * speedSwitchLeft;

                if (m_SkillBarGroup2.transform.localPosition.x >= 0 && m_SkillBarGroup2.transform.localPosition.y >= 0)
                {
                    m_SkillBarGroup1.transform.localPosition = posReadyRight;
                    m_SkillBarGroup2.transform.localPosition = Vector3.zero;
                    m_SkillBarGroup1.SetActive(false);
                    m_SwitchGo = false;
                }
            }
        }
    }
}
