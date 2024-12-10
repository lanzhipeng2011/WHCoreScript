/********************************************************************************
 *	文件名：	CameraController.cs
 *	全路径：	\Script\Player\CameraController.cs
 *	创建人：	李嘉
 *	创建时间：2013-11-05
 *
 *	功能说明： 摄像机控制类
 *	          所有的摄像机逻辑都在其中
 *	修改记录：
*********************************************************************************/


using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Games.LogicObj;
using GCGame.Table;
using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
using Module.Log;
//using Games.LogicObj;

//Require a Obj_MainPlayer to be attached to the same game object
//[RequireComponent(typeof(Obj_MainPlayer))]



public class CameraController : MonoBehaviour
{
    public float m_CameraXOffset = -3.73f;            //摄像机相对主角的X偏移

    public float m_CameraYOffset =14.1f;           //摄像机相对主角的Y偏移
    public float m_CameraZOffset = -20.5f;          //摄像机相对主角的Z偏移
	public float m_CameraXOffsetMax =-3.0f;            //摄像机相对主角的X偏移
	public float m_CameraYOffsetMax =9.0f;           //摄像机相对主角的Y偏移
	public float m_CameraZOffsetMax = -14.0f;          //摄像机相对主角的Z偏移
	public float m_CameraXOffsetMin = -2.0f;
	public float m_CameraYOffsetMin = 5.6f;
	public float m_CameraZOffsetMin = -6.3f;
	public float m_Scale = 1.0f;
	public float m_pinchSpeed = 90.0f;
    //public float m_UpOffset = -0.7f;                //摄像机Look旋转锁定的Y轴偏移（相对于归一化的向量）
    public float m_CenterOffest = 0.6f;
	public float m_CenterOffsetMax = 0.9f;
	public float m_CenterOffsetMin = 0.6f;
	public float m_PinchMax = 10.0f;
	public bool m_IsStory=false;
    /// <summary>
    /// 震屏START
    /// </summary>
    struct CameraRockInfo
    {
        public void CleanUp()
        {
            m_nCameraRockId = -1;
            m_fRockTime = 0.0f; //震屏已经震了多久
            m_fNeedRockTime = 0.0f; //震屏持续时间
            m_fDelayTime = 0.0f; //震屏延迟时间
            m_bContinueRockDie = false; //主角死亡是否继续震屏
            m_XRockOff = new AnimationCurve(); //震屏 摄像机X位置偏移
            m_YRockOff = new AnimationCurve(); //震屏 摄像机Y位置偏移
            m_ZRockOff = new AnimationCurve();//震屏 摄像机Z位置偏移
            m_RXRockOff = new AnimationCurve();//震屏 摄像机X旋转偏移
            m_RYRockOff = new AnimationCurve();//震屏 摄像机Y旋转偏移
            m_RZRockOff = new AnimationCurve();//震屏 摄像机Z旋转偏移
        }

        public bool IsValid()
        {
            return (m_nCameraRockId != -1);
        }
        public int m_nCameraRockId;
        public float m_fRockTime; //震屏已经震了多久
        public float m_fNeedRockTime; //震屏持续时间
        public float m_fDelayTime; //震屏延迟时间
        public bool m_bContinueRockDie; //主角死亡是否继续震屏
        public AnimationCurve m_XRockOff; //震屏 摄像机X位置偏移
        public AnimationCurve m_YRockOff; //震屏 摄像机Y位置偏移
        public AnimationCurve m_ZRockOff;//震屏 摄像机Z位置偏移
        public AnimationCurve m_RXRockOff;//震屏 摄像机X旋转偏移
        public AnimationCurve m_RYRockOff;//震屏 摄像机Y旋转偏移
        public AnimationCurve m_RZRockOff;//震屏 摄像机Z旋转偏移
    }

    private List<CameraRockInfo> m_CameraRockInfoList =new List<CameraRockInfo>(); 
    /// <summary>
    /// 震屏 END
    /// </summary>
    private bool m_bIsCameraInTrack = false;        //人物摄像机是否在处理轨迹,轨迹中的摄像机无法根据人物移动更新
    public bool IsCameraInTrack
    {
        get { return m_bIsCameraInTrack; }
        set { m_bIsCameraInTrack = value; }
    }

	private bool m_bIsCameraInAnimation = false;        //人物摄像机是否在处理轨迹,轨迹中的摄像机无法根据人物移动更新
	public bool IsCameraInAnimation
	{
		get { return m_bIsCameraInAnimation; }
		set { m_bIsCameraInAnimation = value; }
	}
    private Vector3 m_TrackSpeed = new Vector3(0, 0, 0);
    private float m_TrackTravel = 0;
    private float m_TrackSpeedScale = 10;

	private bool m_bPinching = false;
	public bool IsPinching() {return m_bPinching;}

    //场景特效
    private EffectLogic m_SceneEffecLogic;
    //恢复到摄像机跟随主角状态
    public void ResetCameraToMainPlayer()
    {
        m_bIsCameraInTrack = false;
        UpdateCamera();
    }
	public void ResetCameraYd()
	{
		m_isYd = false;
		UpdateCamera();
	}
    //缓存Transform
    private Transform m_MainCameraTransform = null;
    private Transform m_ControllerTransform = null;

	private bool m_isYd=false;
	private float m_aniLength=0;
	private string m_aniname;
    void Awake()
    {
        UpdateCamera();
    }
    void Start()
    {
        FastBloom curCompont = (FastBloom)Camera.main.gameObject.GetComponent("FastBloom");
        if (curCompont != null)
        {
            if (PlayerPreferenceData.SystemFloodlight == 0)
            {
                curCompont.enabled = false;
            }
            else
            {
                curCompont.enabled = true;
            }            
        }

		if(m_pinchSpeed < 1) m_pinchSpeed = 1;
		m_bPinching =false;
        PlaySceneEffect();

    }
	//镜头开启剧情模式
	public void InitStory(Transform  target)
	{
		return;
			if(m_IsStory)
			   return ;
			m_IsStory = true;
			followCamera cam = Camera.main.gameObject.GetComponent<followCamera> ();
			if (cam == null) 
			{
			 Camera.main.gameObject.AddComponent<followCamera> ();
			 cam = Camera.main.gameObject.GetComponent<followCamera> ();
			}
			if(cam!=null)
			{
			 cam.target=target;
			 cam.isEnable=true;
			 Transform  modle=ObjManager.Instance.MainPlayer.transform.FindChild("Model");
			 if(modle!=null)
				modle.gameObject.SetActive(false);
		}
	}
	//镜头结束剧情模式
	public void EndStory()
	{

		return;
		if(m_IsStory==false)
			return ;
		followCamera cam = Camera.main.GetComponent<followCamera> ();
		if (cam == null)
			return;
		m_IsStory = false;

		cam.isEnable = false;
		Obj_MainPlayer  main=ObjManager.Instance.MainPlayer;
		main.CameraController.m_IsStory=false;
		Transform  modle=main.transform.FindChild("Model");
		if(modle!=null)
			modle.gameObject.SetActive(true);
	}
	void PlaySceneEffect()
    {
        if (Camera.main == null)
        {
            return;
        }
        if (Camera.main.gameObject.GetComponent<EffectLogic>() == null)
        {
            Camera.main.gameObject.AddComponent<EffectLogic>();
            m_SceneEffecLogic = Camera.main.gameObject.GetComponent<EffectLogic>();
            if (m_SceneEffecLogic != null)
            {
                m_SceneEffecLogic.InitEffect(Camera.main.gameObject);
            }
        }

    }
    
    //更新场景摄像机
    public void UpdateCamera()
    {
		if (m_IsStory)
						return;
        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (null == _mainPlayer)
            return;

        if (null == m_MainCameraTransform)
        {
            m_MainCameraTransform = Camera.main.transform;
            if (null == m_MainCameraTransform)
            {
                return;
            }
        }

        if (null == m_ControllerTransform)
        {
            m_ControllerTransform = _mainPlayer.transform;
            if (null == m_ControllerTransform)
            {
                return;
            }
        }

        //有震屏需求 摄像机抖动
        if (m_CameraRockInfoList.Count >0 )
        {
            bool IsRocking = false;
            for (int i = 0; i < m_CameraRockInfoList.Count; i++)
            {
                if (m_CameraRockInfoList[i].IsValid() ==false)
                {
                    continue;
                }
                //struct 先拷贝出一份出来 修改完后 记得再赋值更新
                CameraRockInfo _TmpInfo = m_CameraRockInfoList[i];
                //死了就不震了
                if (_TmpInfo.m_bContinueRockDie == false && _mainPlayer.IsDie())
                {
                    _TmpInfo.CleanUp();
                }
                else
                {
                    if (_TmpInfo.m_fDelayTime>0)
                    {
                        _TmpInfo.m_fDelayTime -= Time.deltaTime; 
                    }
                    else
                    {
                        if (_TmpInfo.m_fDelayTime <= 0)
                        {
                            if (_TmpInfo.m_fRockTime - _TmpInfo.m_fNeedRockTime >= 0)
                            {
                                //震完了 清理数据
                                _TmpInfo.CleanUp();
                            }
                            else
                            {
                                _TmpInfo.m_fRockTime += Time.deltaTime;
                                CameraRock(i);
                                IsRocking = true;
                            }
                        } 
                    }
                }
                //重新赋值更新
                m_CameraRockInfoList[i] = _TmpInfo;
            }
            //清除无效的
            List<CameraRockInfo> _needMoveList =new List<CameraRockInfo>();
            for (int i = 0; i < m_CameraRockInfoList.Count; i++)
            {
                if (m_CameraRockInfoList[i].IsValid() ==false)
                {
                    _needMoveList.Add(m_CameraRockInfoList[i]);
                }
            }
            for (int i = 0; i < _needMoveList.Count; i++)
            {
                m_CameraRockInfoList.Remove(_needMoveList[i]);
            }
            if (IsRocking)
            {
                return;
            }
        }
		if (m_isYd) 
		{
			UpdateCameraYd();
			return ;
		}
        //如果正在播放游戏轨迹则不更新
        if (m_bIsCameraInTrack)
        {
            UpdateCameraTrack();
            return;
        }
		if (m_bIsCameraInAnimation)
		{

			return;
		}
        //更新摄像机的角度
		m_CameraXOffset = (m_CameraXOffsetMax - m_CameraXOffsetMin) * m_Scale + m_CameraXOffsetMin;
		m_CameraYOffset = (m_CameraYOffsetMax - m_CameraYOffsetMin) * m_Scale+ m_CameraYOffsetMin;
		m_CameraZOffset = (m_CameraZOffsetMax - m_CameraZOffsetMin) * m_Scale+ m_CameraZOffsetMin;
		m_CenterOffest = (m_CenterOffsetMax - m_CenterOffsetMin) * (1.0f-m_Scale) + m_CenterOffsetMin;
        Vector3 dir = new Vector3(m_CameraXOffset, m_CameraYOffset, m_CameraZOffset);
        Vector3 cameraPos = m_ControllerTransform.position + dir;
        //锁定摄像机Y轴高度
        //cameraPos.y = m_CameraYOffset;
        m_MainCameraTransform.localPosition = cameraPos;

        //更新摄像机的Look点，锁定Y轴偏移
        Vector3 pos = m_ControllerTransform.position;
        pos.y += m_CenterOffest;
        Vector3 lookPos = pos - m_MainCameraTransform.position;
        lookPos.Normalize();
        //lookPos.y = m_UpOffset;
        m_MainCameraTransform.rotation = Quaternion.LookRotation(lookPos);
    }

    public bool IsHaveRockInfoById(int nRockId)
    {
        if (nRockId ==-1)
        {
            return false;
        }
        for (int i = 0; i < m_CameraRockInfoList.Count; i++)
        {
            if (m_CameraRockInfoList[i].m_nCameraRockId ==nRockId)
            {
                return true;
            }
        }
        return false;
    }
    public void CleanUpRockInfoById(int nRockId)
    {
        for (int i = 0; i < m_CameraRockInfoList.Count; i++)
        {
            CameraRockInfo _tmpRockInfo = m_CameraRockInfoList[i];
            if (_tmpRockInfo.m_nCameraRockId == nRockId)
            {
                _tmpRockInfo.CleanUp();
                m_CameraRockInfoList[i] = _tmpRockInfo;
            }
        }
    }
    public void InitCameraRock(int nRockId)
    {
        Tab_CameraRock _cameraRock = TableManager.GetCameraRockByID(nRockId, 0);
        if (_cameraRock ==null)
        {
            return;
        }
        CameraRockInfo _tmpInfo =new CameraRockInfo();
        _tmpInfo.CleanUp();
        //初始化数据
        _tmpInfo.m_nCameraRockId = nRockId;
        _tmpInfo.m_fNeedRockTime = _cameraRock.NeedRockTime;
        _tmpInfo.m_fDelayTime = _cameraRock.DelayTime;
        //位置偏移动画曲线
        _tmpInfo.m_XRockOff = InitRockOff(_cameraRock.PosXAnimCurveId);
        _tmpInfo.m_YRockOff = InitRockOff(_cameraRock.PosYAnimCurveId);
        _tmpInfo.m_ZRockOff = InitRockOff(_cameraRock.PosZAnimCurveId);

        //旋转偏移动画曲线
        _tmpInfo.m_RXRockOff = InitRockOff(_cameraRock.RXAnimCurveId);
        _tmpInfo.m_RYRockOff = InitRockOff(_cameraRock.RYAnimCurveId);
        _tmpInfo.m_RZRockOff = InitRockOff(_cameraRock.RZAnimCurveId);
        _tmpInfo.m_bContinueRockDie = _cameraRock.IsContinueDie;
        
        m_CameraRockInfoList.Add(_tmpInfo);
    }
    protected void CameraRock(int nRockIndex)
    {
        if (nRockIndex>=0 && nRockIndex<m_CameraRockInfoList.Count)
        {
            float nNewXPos = m_CameraXOffset + m_CameraRockInfoList[nRockIndex].m_XRockOff.Evaluate(m_CameraRockInfoList[nRockIndex].m_fRockTime);
            float nNewYPos = m_CameraYOffset + m_CameraRockInfoList[nRockIndex].m_YRockOff.Evaluate(m_CameraRockInfoList[nRockIndex].m_fRockTime);
            float nNewZPos = m_CameraZOffset + m_CameraRockInfoList[nRockIndex].m_ZRockOff.Evaluate(m_CameraRockInfoList[nRockIndex].m_fRockTime);
            //更新摄像机的角度
            Vector3 dir = new Vector3(nNewXPos, nNewYPos, nNewZPos);
            Vector3 cameraPos = m_ControllerTransform.localPosition + dir;
            m_MainCameraTransform.localPosition = cameraPos;
            //更新摄像机的Look点，锁定Y轴偏移
            Vector3 pos = m_ControllerTransform.position;
            pos.y += m_CenterOffest;
            Vector3 lookPos = pos - m_MainCameraTransform.position;
            lookPos.Normalize();
            float nNewRXPos = lookPos.x + m_CameraRockInfoList[nRockIndex].m_RXRockOff.Evaluate(m_CameraRockInfoList[nRockIndex].m_fRockTime);
            float nNewRYPos = lookPos.y + m_CameraRockInfoList[nRockIndex].m_RYRockOff.Evaluate(m_CameraRockInfoList[nRockIndex].m_fRockTime);
            float nNewRZPos = lookPos.z + m_CameraRockInfoList[nRockIndex].m_RZRockOff.Evaluate(m_CameraRockInfoList[nRockIndex].m_fRockTime);
            m_MainCameraTransform.rotation = Quaternion.LookRotation(new Vector3(nNewRXPos, nNewRYPos, nNewRZPos));
        }
        
    }
    
    protected AnimationCurve InitRockOff(int nCurverId)
    {
        AnimationCurve RockCurve =new AnimationCurve();
        if (nCurverId!=-1)
        {
            List<Tab_AnimationCurve> _curveList = TableManager.GetAnimationCurveByID(nCurverId);
            if (_curveList.Count>0)
            {
                Keyframe[] XCurverKeyframes = new Keyframe[_curveList.Count];
                for (int i = 0; i < _curveList.Count; i++)
                {
                    XCurverKeyframes[i].time = _curveList[i].Time;
                    XCurverKeyframes[i].value = _curveList[i].Value;
                    XCurverKeyframes[i].inTangent = _curveList[i].InTangent;
                    XCurverKeyframes[i].outTangent = _curveList[i].OutTangent;
                    XCurverKeyframes[i].tangentMode = _curveList[i].TangentMode;
                }
                RockCurve = new AnimationCurve(XCurverKeyframes);
                RockCurve.preWrapMode = (WrapMode)_curveList[0].PreWrapMode;
                RockCurve.postWrapMode = (WrapMode)_curveList[0].PostWrapMode; 
            }
        }
        return RockCurve;
    }
    public void InitYD(string name)
	{
		m_isYd = true;
		m_aniname = name;
		m_aniLength = m_MainCameraTransform.gameObject.animation[name].length;
	}
    public void InitCameraTrack(Vector3 posStart, Vector3 posEnd)
    {
        m_bIsCameraInTrack = true;
        m_TrackTravel = Vector3.Distance(posStart, posEnd);
        m_TrackSpeed = (posEnd - posStart) / (m_TrackTravel*2.0f);
    }

    public void UpdateCameraTrack()
    {
        if (m_bIsCameraInTrack)
        {
            if (m_TrackTravel > 0)
            {
                m_MainCameraTransform.position += m_TrackSpeed * m_TrackSpeedScale * Time.deltaTime;
                m_TrackTravel -= m_TrackSpeedScale * Time.deltaTime;
            }
        }
    }
	public void UpdateCameraYd()
	{
		if (m_isYd)
		{
			m_MainCameraTransform.gameObject.animation.Play(m_aniname);
			m_aniLength=m_aniLength-Time.deltaTime;
			if(m_aniLength<0)
				m_isYd=false;
		}

	}
	void OnPinch( PinchGesture gesture )
	{
        if (JoyStickLogic.Instance() != null && JoyStickLogic.Instance().FingerID != -1)
        {
            return;
        }
		if(gesture.Phase == ContinuousGesturePhase.Started)
		{
            Obj_MainPlayer mainPlayer = Singleton<ObjManager>.Instance.MainPlayer;
            if (null != mainPlayer)
            {
                if (Singleton<ObjManager>.GetInstance().MainPlayer.IsCanOperate_Move())
                {
                    Singleton<ObjManager>.GetInstance().MainPlayer.StopMove();
                }
            }
			m_bPinching = true;
		}
		else if( gesture.Phase == ContinuousGesturePhase.Updated )
		{
			float absDelta = Mathf.Abs(gesture.Delta);
			if(absDelta > m_PinchMax)
			{
				absDelta = m_PinchMax;
			}
			float curDelta = absDelta * gesture.Delta / Mathf.Abs(gesture.Delta);
			m_Scale -= (curDelta * (0.5f + 0.5f*m_Scale) / m_pinchSpeed);
			if(m_Scale > 1.0f) m_Scale = 1.0f;
			if(m_Scale < 0.2f) m_Scale = 0.2f;			
		}
		else
		{
			m_bPinching = false;
		}
	}
}
