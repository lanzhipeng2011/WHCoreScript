using UnityEngine;
using System.Collections;
using Games.LogicObj;
using GCGame.Table;

public class BillBoard : MonoBehaviour 
{

    private GameObject m_BindObj = null;
    public GameObject BindObj
    {
        get { return m_BindObj; }
        set { m_BindObj = value; }
    }

    private float m_fDeltaHeight = 0.0f;

	public float fDeltaHeight
	{
		set{
			if(value != m_fDeltaHeight)
			{
				m_fDeltaHeight = value;
				m_Position = new Vector3(0,m_fDeltaHeight,0);
			}
		}

		get
		{
			return m_fDeltaHeight;
				}
	}

    void Awake()
    {		
		if (null == Camera.main)
		{
			return;
		}

		Vector3 vecUp = Camera.main.transform.rotation * Vector3.up;
		transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.back, vecUp);
        //这里算是一个BUG，HeadInfo的Y轴旋转总是差180度，目前暂未发现原因，所以在代码中进行手动旋转
        transform.Rotate(Vector3.up * 180);
    }

    void OnEnable()
    {
        transform.rotation = Quaternion.identity;
        if (null == Camera.main)
        {
            return;
        }

        Vector3 vecUp = Camera.main.transform.rotation * Vector3.up;
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.back, vecUp);
        //这里算是一个BUG，HeadInfo的Y轴旋转总是差180度，目前暂未发现原因，所以在代码中进行手动旋转
        transform.Rotate(Vector3.up * 180);
    }
    
    void Start()
    {
        if (m_BindObj != null)
        {
            Obj_OtherPlayer objOtherPlayer = m_BindObj.GetComponent<Obj_OtherPlayer>();
            if (null != objOtherPlayer)
            {
                // 关于直接创建坐骑上的玩家的名字板高度修正 应该也会执行一次Obj_Mount的RideMount 相当于设置了两次高度 没什么大影响 暂时不删除此处
				m_fDeltaHeight = objOtherPlayer.DeltaHeight + objOtherPlayer.GetMountNameBoardHeight();
            }
            else
            {
                Obj_Character objCharacter = m_BindObj.GetComponent<Obj_Character>();
                if (null != objCharacter)
                {
					m_fDeltaHeight = objCharacter.DeltaHeight;
                }
            } 

			m_Position = new Vector3(0,m_fDeltaHeight,0);
			m_BindObjTrans = m_BindObj.transform;
        }

		m_Transform = transform;
    }
    
	private Transform m_Transform = null;
	private Vector3 m_Position ;
	private Transform m_BindObjTrans = null;

	// Update is called once per frame
	void Update ()
    {
        if (null != m_BindObj && null == m_BindObjTrans)
        {
            m_BindObjTrans = m_BindObj.transform;
        }

		if (m_BindObj && null != m_Transform && null != m_BindObjTrans)
		{
			m_Transform.position = m_BindObjTrans.position;
			m_Transform.position += m_Position;
        }
	}

    public void RecoverHeight()
    {
        if (m_BindObj != null)
        {
            Obj_Character objChar = m_BindObj.GetComponent<Obj_Character>();
            if (null != objChar)
            {
                fDeltaHeight = objChar.DeltaHeight;
            }
        }
    }
}
