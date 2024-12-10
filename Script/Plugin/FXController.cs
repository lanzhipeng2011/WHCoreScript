/********************************************************************
	created:	2013/12/24
	created:	24:12:2013   14:14
	filename: 	FXController.cs
	author:		王迪
	
	purpose:	特效控制器，绑定在特效上，控制生命周期与释放
*********************************************************************/

using UnityEngine;
using System.Collections;
using Module.Log;
public class FXController : MonoBehaviour
{
	public enum FXType
	{
		TYPE_PARTICLE = 0,
		TYPE_WEAPONTRAIL = 1,
	}

	private float m_fDuration = 0;              // 特效持续时间
	public float Duration
	{
		get { return m_fDuration; }
		set { m_fDuration = value; }
	}
	private bool m_bOnlyDeactivate = false;     // 播放结束后是否只失效：true 播放完成后特效并不销毁，只是禁用。false 播放完成后，销毁特效
	public bool OnlyDeactivate
	{
		get { return m_bOnlyDeactivate; }
		set { m_bOnlyDeactivate = value; }
	}
	private int m_nEffectID = 0;                // EffectLogic分配ID 用于识别单个特效
	public int EffectID
	{
		get { return m_nEffectID; }
		set { m_nEffectID = value; }
	}
	private float m_fDelay = 0;                 // 播放延迟
	public float Delay
	{
		get { return m_fDelay; }
		set { m_fDelay = value; }
	}
	private FXType m_FxType;                   // 粒子种类
	public FXController.FXType FxType
	{
		get { return m_FxType; }
		set { m_FxType = value; }
	}

	private float m_fRemainduration = 0;
    public float Remainduration
    {
        get { return m_fRemainduration; }
        set { m_fRemainduration = value; }
    }
	private float m_fRemaindelay = 0;
    public float Remaindelay
    {
        get { return m_fRemaindelay; }
    }
	private GameObject m_fxObject = null;

    private GameObject m_EffectGameObj = null;
    Vector3 m_StartFXPos =new Vector3(0,0,0);
    Quaternion m_StartRotation =new Quaternion();
    private bool m_bIsFellowOwner = false;
    public bool IsFellowOwner
    {
        get { return m_bIsFellowOwner; }
        set { m_bIsFellowOwner = value; }
    }
    public UnityEngine.GameObject EffectGameObj
    {
        get { return m_EffectGameObj; }
        set { m_EffectGameObj = value; }
    }

    private EffectLogic m_curEffectHandle;      // EffectLogic
    public GameObject m_FirstChild;             // 第一个结点，通常绑定逻辑脚本\

    public float PlayerFinishTime = 0f;

	public void Play(EffectLogic curEffectHandle)
	{
		Transform child = transform.GetChild(0);
		if (null == child)
		{
			LogModule.ErrorLog("can not find effect in fxController");
            ResourceManager.DestroyResource(this.gameObject);
			return;
		}

        m_FirstChild = child.gameObject;
		m_fRemainduration = m_fDuration;
		m_fRemaindelay = m_fDelay;
		m_fxObject = child.gameObject;
        m_curEffectHandle = curEffectHandle;
	    PlayerFinishTime = 0f;
		gameObject.SetActive(true);
	}

	public void Stop()
	{
		if (m_FxType == FXType.TYPE_PARTICLE)
		{
			StopCoroutine("CheckIfAlive");
		}
		RemoveParticle();
	}

	void DoPlay()
	{
        if (null == m_fxObject)
            return;

		if (m_FxType == FXType.TYPE_WEAPONTRAIL)
		{
            if (null != m_fxObject.GetComponent<MeleeWeaponTrail>())
                m_fxObject.GetComponent<MeleeWeaponTrail>().Emit = true;
		}
		else
		{
			m_fxObject.SetActive(true);
		}
        //记录下播放时候的位置
        m_StartFXPos = gameObject.transform.position;
	    m_StartRotation = gameObject.transform.rotation;
	}

	void OnEnable()
	{
        if (gameObject.transform.parent != null)
        {
            gameObject.transform.position = gameObject.transform.parent.position;
            gameObject.transform.rotation = gameObject.transform.parent.rotation;
        }
		m_fRemaindelay = m_fDelay;
        if (m_fxObject != null && m_fxObject.particleSystem != null)
		{
			m_FxType = FXType.TYPE_PARTICLE;
		}

		m_fRemainduration = m_fDuration;

		if (m_fRemaindelay == 0)
		{
			DoPlay();
		}
	}

	void Update()
	{
		if (m_fRemaindelay > 0)
		{
			m_fRemaindelay -= Time.deltaTime;

			if (m_fRemaindelay <= 0)
			{
				DoPlay();
			}
			return;
		}

		if (m_fRemainduration > 0)
		{
            //如果不随主人移动 则 修正坐标为 播放时的坐标
		    if (IsFellowOwner ==false)
		    {
                gameObject.transform.position = m_StartFXPos;
		        gameObject.transform.rotation = m_StartRotation;
		    }
		   
			m_fRemainduration -= Time.deltaTime;
			if (m_fRemainduration <= 0)
			{
				Stop();
			}
		}
	}

	IEnumerator CheckIfAlive()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.5f);
			if (null != particleSystem && !particleSystem.IsAlive(true))
			{
				RemoveParticle();
			}
				
		}
	}

	void RemoveParticle()
	{
        PlayerFinishTime = Time.time;

        if (m_bOnlyDeactivate)
        {
            if (null != m_fxObject)
            {
                if (m_FxType == FXType.TYPE_WEAPONTRAIL)
                {
                    if (m_fxObject.GetComponent<MeleeWeaponTrail>() != null)
                        m_fxObject.GetComponent<MeleeWeaponTrail>().Emit = false;
                }
                else
                {
                    m_fxObject.SetActive(false);
                    this.gameObject.SetActive(false);
                }
            }

            if (null != m_curEffectHandle)
            {
                m_curEffectHandle.EffectDeactive(this);
            }
        }
        else
        {
            if (null != m_curEffectHandle)
            {
                m_curEffectHandle.EffectDestroyed(EffectID);
            }

            ResourceManager.DestroyResource(this.gameObject);
        }
	}
}
