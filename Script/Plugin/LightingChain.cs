using UnityEngine;
using System.Collections;
using Games.LogicObj;

public class LightingChain : MonoBehaviour {
    public float OrgLength = 1.5f;
    public Transform LightingParent;
    public float Height = 0.1f;

    private int m_targetID;
    private Transform m_ObjTransform = null;

    public void InitData(int targetId)
    {
        m_targetID = targetId;
        m_ObjTransform = transform;
    }

	void Update()
    {
        if (null == m_ObjTransform || null == m_ObjTransform.parent)
        {
            return;
        }

        Vector3 targetPos;
        Obj_Character Target = Singleton<ObjManager>.Instance.FindObjCharacterInScene(m_targetID);
        if (null == Target)
        {
            FXController curController = m_ObjTransform.parent.GetComponent<FXController>();
            if (null != curController)
            {
                curController.Stop();
            }

            return;
        }

        targetPos = Target.transform.position;
        targetPos.y += Height;
        LightingParent.localScale = new Vector3(1, 1, (targetPos - m_ObjTransform.position).magnitude / OrgLength);
        transform.LookAt(targetPos);

    }
}
