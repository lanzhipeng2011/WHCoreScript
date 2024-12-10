using UnityEngine;
using System.Collections;
using Games.LogicObj;

public class Bullet : MonoBehaviour {
	
	public float m_speed =5.0f;
	public float minDistance = 0.3f;
	public float m_duration = -1;           // 如果设置了持续时间，则SPEED根据持续时间计算
	public float startHeight = 1.0f;
	private Vector3 m_startPos;
	private Vector3 m_dir;
	private int m_targetID;
	
	private Transform mTrans;
	FXController mFxController;
	Transform mTargetTrans;
	
	private float m_targetGetTimer = 0;
	
	public void InitData(int sendId,int targetId)
	{
		mTrans = transform;
		if (null == mTrans.parent)
		{
			mFxController = null;
			return;
		}
		else
		{
			mFxController = mTrans.parent.GetComponent<FXController>();
		}
		
		if (null == mFxController)
		{
			return;
		}
		
		Obj_Character Sender = Singleton<ObjManager>.Instance.FindObjCharacterInScene(sendId);
		Obj_Character Target = Singleton<ObjManager>.Instance.FindObjCharacterInScene(targetId);
		if (null == Sender || null == Target)
		{
			mFxController.Stop();
			return;
		}
		
		m_startPos = Sender.transform.position;
		m_startPos.y += startHeight;
		mTrans.position = m_startPos;
		Vector3 vtar = Target.transform.position;
		vtar.y += startHeight;
		m_dir = ( vtar- m_startPos);
		if (m_duration > 0)
		{
			m_speed = m_dir.magnitude / m_duration;
		}
		m_dir.Normalize();
		
		m_targetID = targetId;
		
		
		
		m_targetGetTimer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (mTrans == null || null == mFxController)
		{
			return;
		}

		Obj_Character Target = Singleton<ObjManager>.Instance.FindObjCharacterInScene(m_targetID);
		
		m_targetGetTimer -= Time.deltaTime;
		
		if (null == mTargetTrans || m_targetGetTimer <= 0)
		{
			Obj_Character objTarget = Singleton<ObjManager>.Instance.FindObjCharacterInScene(m_targetID);
			
			if (objTarget == null)
			{
				mFxController.Stop();
				return;
			}
			else
			{
				mTargetTrans = objTarget.transform;
				m_targetGetTimer = 0.3f;
				//return ;
			}
		}

		Vector3 vtar = mTargetTrans.position;
		vtar.y += startHeight;
		Vector3 v1 = new Vector3 (vtar.x,0,vtar.z);
		Vector3 v2 = new Vector3 (mTrans.position.x, 0, mTrans.position.z);
		float sqDist = Vector3.Distance (v1, v2);
		
		
		if (sqDist < 1.2f)
		{
			if (mFxController != null)
			{
				mFxController.Stop();
				mTrans=null;

			}
			return ;
		}
		m_dir = (vtar - mTrans.position).normalized;


		mTrans.position +=  m_dir * m_speed * Time.deltaTime;
		
		mTrans.LookAt(vtar);

		
		
	}
}
