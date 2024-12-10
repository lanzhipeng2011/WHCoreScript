using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Clojure;
public class ShowTutorialHandAction : BlockAction
{
    public string StrPos;
    public GameObject PanelG; 
    public int OldDepth;
    public bool isNewPanel;
    public override void OnLoad(List<ASTNode> args)
    {
        StrPos = ((StringNode)args[1]).Val;
    }

    public override void OnExec()
    {
       
        switch (StrPos) 
        {
            case "JoyStick":
                NewPlayerGuidLogic.OpenWindow(JoyStickLogic.Instance().gameObject, 224, 224, GCGame.Utils.GetDicByID(2841), "right", 2, true);
                NewPlayerGuidLogic.Instance().CurShowType = "JoyStick";
                NewPlayerGuidLogic.Instance().TutorialHandClose += HandShowOver;
                NewPlayerGuidLogic.Instance().m_NewPlayerGuid.gameObject.SetActive(false);
                SetDepth(JoyStickLogic.Instance().transform.parent.parent.parent.gameObject);
            break;
            case "Attack":
                NewPlayerGuidLogic.OpenWindow(SkillBarLogic.Instance().m_SkillAttackBt, 195, 195, GCGame.Utils.GetDicByID(2874), "left", 2, true);
                NewPlayerGuidLogic.Instance().TutorialHandClose += HandShowOver;
                NewPlayerGuidLogic.Instance().CurShowType = "Attack";
                NewPlayerGuidLogic.Instance().m_NewPlayerGuid.gameObject.SetActive(false);
                SetDepth(SkillBarLogic.Instance().m_SkillAttackBt);
            break;
            case "Skill":
            NewPlayerGuidLogic.OpenWindow(SkillBarLogic.Instance().m_Skill2Bt, 180, 180, GCGame.Utils.GetDicByID(2875), "left", 2, true);
               NewPlayerGuidLogic.Instance().TutorialHandClose += HandShowOver;
               NewPlayerGuidLogic.Instance().CurShowType = "Skill";
               NewPlayerGuidLogic.Instance().m_NewPlayerGuid.gameObject.SetActive(false);
               SetDepth(SkillBarLogic.Instance().m_Skill2Bt.transform.parent.gameObject);
            break;
            case "SkillXP":
            NewPlayerGuidLogic.OpenWindow(SkillBarLogic.Instance().m_SkillXPBt, 125, 125, GCGame.Utils.GetDicByID(999) ,"left", 2, true);
               NewPlayerGuidLogic.Instance().TutorialHandClose += HandShowOver;
               NewPlayerGuidLogic.Instance().CurShowType = "SkillXP";
               NewPlayerGuidLogic.Instance().m_NewPlayerGuid.gameObject.SetActive(false);
               SetDepth(SkillBarLogic.Instance().m_SkillXPBt.transform.parent.gameObject);
            break;
        }
       
    }

    public void SetDepth(GameObject go) 
    {
       UIPanel panel = go.GetComponent<UIPanel>();
       if (panel == null)
       {
           panel = go.AddComponent<UIPanel>();
           isNewPanel = true;

       }
       else 
       {
           isNewPanel = false;
       }
       OldDepth = panel.depth;
       PanelG = panel.gameObject;
       panel.depth = 101;
       panel.gameObject.SetActive(false);
       panel.gameObject.SetActive(true);
    }

    public void HandShowOver() 
    {
        PanelG.GetComponent<UIPanel>().depth = OldDepth;
        if (isNewPanel) 
        {
            GameObject.Destroy(PanelG.GetComponent<UIPanel>());
        }
        PanelG.SetActive(false);
        PanelG.SetActive(true);
        this.IsFinish = true;
    }
    public override void OnExit()
    {
        if (NewPlayerGuidLogic.Instance() != null)
        {
            NewPlayerGuidLogic.Instance().TutorialHandClose -= HandShowOver;
        }
    }
}
