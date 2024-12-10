using UnityEngine;
using System.Collections;

public class SkillTurntableContents : MonoBehaviour {

    //public SkillBarSwitch m_SkillBarSwitch;
    public Turntable m_Turntable;

	// Use this for initialization
	void Start () {
	
	}
	

    void OnPress(bool pressed)
    {
        //m_SkillBarSwitch.Press(pressed);
        m_Turntable.Press(pressed);
    }

    void OnDrag(Vector2 delta)
    {
        //m_SkillBarSwitch.Drag(delta);
        m_Turntable.Drag(delta);
        if (SkillBarLogic.Instance() && SkillBarLogic.Instance().NewPlayerGuide_Step == 4)
        {
            NewPlayerGuidLogic.CloseWindow();
            SkillBarLogic.Instance().NewPlayerGuide_Step = -1;
            GameManager.gameManager.PlayerDataPool.ForthSkillFlag = false;
        }
    }
}
