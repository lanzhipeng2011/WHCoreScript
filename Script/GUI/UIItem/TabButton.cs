/********************************************************************
	created:	2014/01/14
	created:	14:1:2014   13:50
	filename: 	TabButton.cs
	author:		王迪
	
	purpose:	绑定在Tab页按钮，配合TabController 禁用开启指定GameObject
*********************************************************************/
using UnityEngine;
using System.Collections;

public class TabButton : MonoBehaviour {

    public GameObject objHighLight;
    public GameObject objNormal;
    public GameObject targetObject;

    private TabController m_curController;
	// Use this for initialization
	void Start () {
	}

    public void HighLightTab(bool bHighLight)
    {
		if(objHighLight != null)
        	objHighLight.SetActive(bHighLight);
		if(objNormal != null)
			objNormal.SetActive(!bHighLight);

        if (null != targetObject)
        {
            targetObject.SetActive(bHighLight);
        }
    }

    public void OnTabClick()
    {
        if (m_curController == null)
        {
            FindController();
        }
        if (m_curController != null)
        {
            m_curController.OnTabClicked(this);
        }
    }

    void FindController()
    {
        Transform parent = transform.parent;
        while (parent != null)
        {
            m_curController = parent.gameObject.GetComponent<TabController>();
            if (null != m_curController)
            {
                break;
            }

            parent = parent.parent;
        }
    }

}
