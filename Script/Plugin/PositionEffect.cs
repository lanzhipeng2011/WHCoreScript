using UnityEngine;
using System.Collections;
using Module.Log;

public class PositionEffect : MonoBehaviour {

	private Transform m_ModelParent = null;
	// Use this for initialization
	void Start () 
	{
		Transform curTrans = transform;
		Transform parent = null;
		while (null != curTrans) 
		{
			if (curTrans.name == "Model") 
			{
				Transform curParent = curTrans.parent;
				if(null == curParent)
				{
                    LogModule.DebugLog("model is error");
				}
				else
				{
					parent = curParent;
				}

				break;
			}

			curTrans = curTrans.parent;
		}

		if (null == parent) 
		{
            LogModule.DebugLog("model is error");
			return;
		}

		m_ModelParent = parent;
	}
	
	// Update is called once per frame
	void Update () {
		if (null == m_ModelParent) 
		{
			return;
				
		}

		transform.rotation = m_ModelParent.rotation;
	
	}
}
