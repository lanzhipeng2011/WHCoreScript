using UnityEngine;
using System.Collections;

public class CameraFakeObjct : MonoBehaviour {
    public GameObject m_TopLeft;
    public GameObject m_BottomRight;

    void Awake()
    {
    }

	// Use this for initialization
	void Start () {
        if (m_TopLeft != null && m_BottomRight != null)
        {
            Init();
        }       
	}
	

    public void Init()
    {
        if (null == gameObject.GetComponent<UIViewport>())
        {
            gameObject.AddComponent<UIViewport>();
        }

        UIViewport UiViewPort = gameObject.GetComponent<UIViewport>();
        if (UiViewPort && m_TopLeft && m_BottomRight)
        {
            UiViewPort.topLeft = m_TopLeft.transform;
            UiViewPort.bottomRight = m_BottomRight.transform;
            if (UICamera.mainCamera)
            {
                UiViewPort.sourceCamera = UICamera.mainCamera;
            }
        }
    }
}
