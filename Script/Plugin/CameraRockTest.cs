using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using Games.GlobeDefine;
using Games.LogicObj;
using GCGame.Table;
using UnityEngine;
using System.Collections;
  
public class CameraRockTest : MonoBehaviour
{
    private float m_CameraXOffset = 7.8f;            //摄像机相对主角的X偏移
    private float m_CameraYOffset = 8.0f;           //摄像机相对主角的Y偏移
    private float m_CameraZOffset = -9.0f;          //摄像机相对主角的Z偏移
    private float m_CameraXOffsetMax = 7.8f;            //摄像机相对主角的X偏移
    private float m_CameraYOffsetMax = 8.0f;           //摄像机相对主角的Y偏移
    private float m_CameraZOffsetMax = -9.0f;          //摄像机相对主角的Z偏移
    private float m_CameraXOffsetMin = 4.0f;
    private float m_CameraYOffsetMin = 4.0f;
    private float m_CameraZOffsetMin = -5.0f;
    private float m_Scale = 1.0f;
    //private float m_pinchSpeed = 100.0f;
    //private float m_UpOffset = -0.7f;                //摄像机Look旋转锁定的Y轴偏移（相对于归一化的向量）
    private float m_CenterOffest = 0.6f;
    //private float m_CenterOffsetMax = 0.9f;
    //private float m_CenterOffsetMin = 0.6f;
    //private float m_PinchMax = 10.0f;

    private Vector3 m_PlayerPos = new Vector3(0, 0, 0);
    /// <summary>
    /// 震屏START
    /// </summary>
    private bool m_bIsPlayRock = false; //是否震屏
  
    public float m_fNeedRockTime = 0.0f; //震屏持续时间
    public float m_fDelayTime= 0.0f; //震屏延迟时间
    //private bool m_bContinueRockDie = false; //主角死亡是否继续震屏
    private float m_fRockTime = 0.0f; //震屏已经震了多久

    public AnimationCurve m_XRockOff =new AnimationCurve(); //震屏 摄像机X位置偏移
    public AnimationCurve m_YRockOff = new AnimationCurve(); //震屏 摄像机Y位置偏移
    public AnimationCurve m_ZRockOff = new AnimationCurve();//震屏 摄像机Z位置偏移

    public AnimationCurve m_RXRockOff = new AnimationCurve();//震屏 摄像机X旋转偏移
    public AnimationCurve m_RYRockOff = new AnimationCurve();//震屏 摄像机Y旋转偏移
    public AnimationCurve m_RZRockOff = new AnimationCurve();//震屏 摄像机Z旋转偏移

    /// <summary>
    /// 震屏 END
    /// </summary>
   
    private int m_PosxIndex =-1;
    public int PosxIndex
    {
        get { return m_PosxIndex; }
        set { m_PosxIndex = value; }
    }
    private int m_PosYIndex = -1;
    public int PosYIndex
    {
        get { return m_PosYIndex; }
        set { m_PosYIndex = value; }
    }
    private int m_PosZIndex = -1;
    public int PosZIndex
    {
        get { return m_PosZIndex; }
        set { m_PosZIndex = value; }
    }
    private int m_RXIndex = -1;
    public int RXIndex
    {
        get { return m_RXIndex; }
        set { m_RXIndex = value; }
    }
    private int m_RYIndex = -1;
    public int RYIndex
    {
        get { return m_RYIndex; }
        set { m_RYIndex = value; }
    }
    private int m_RZIndex = -1;
    public int RZIndex
    {
        get { return m_RZIndex; }
        set { m_RZIndex = value; }
    }

    // 缓存
    private Transform mTrans;
    private Transform mMainCameraTrans;

	void Start()
    {
        if (null == GameManager.gameManager)
            return;

        if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_YUQINGGONG)
        {
            m_PlayerPos = new Vector3(19.6f, 22.47f, 14.7f);
        }
        else if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_SHAOSHISHAN)
        {
            m_PlayerPos = new Vector3(45.5f, 20.0f, 9.07f);
        }
        else if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_TIANSHAN)
        {
            m_PlayerPos = new Vector3(14.5f, 20.0f, 50.2f);
        }
        else if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_XINGZILIN)
        {
            m_PlayerPos = new Vector3(32f, 15.5f, 6.3f);
        }
        else if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_JIANHUGONG)
        {
            m_PlayerPos = new Vector3(18f, 15.4f, 10.9f);
        }
        else if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_GUOGUANZHANJIANG)
        {
            m_PlayerPos = new Vector3(90.35f, 8.39f, 50.0f);
        }
        else if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_ERHAI)
        {
            m_PlayerPos = new Vector3(20.0f, 18.39f, 18.0f);
        }
        else if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_XIAOJINGHU)
        {
            m_PlayerPos = new Vector3(42.0f, 17.39f, 7.0f);
        }

        this.transform.position = m_PlayerPos;
    }
    void Update()
    {
        UpdateCameraRock();
    }

    public void Play()
    {
        m_fRockTime = 0.0f;
        m_bIsPlayRock = true;
    }
    //更新场景摄像机
    public void UpdateCameraRock()
    {
        //摄像机抖动
        if (m_bIsPlayRock )
        {
            m_fDelayTime -= Time.deltaTime;
            if (m_fDelayTime <=0)
            {
                if (m_fRockTime - m_fNeedRockTime >= 0)
                {
                    m_bIsPlayRock = false;
                    m_fRockTime = 0.0f;
                    m_fDelayTime = 0.0f;
                    return;

                }
                CarmerRock();
                return;
            }
        }

        if (null == mTrans)
        {
            mTrans = gameObject.transform;
            if (null == mTrans)
            {
                return;
            }
        }

        if (null == mMainCameraTrans)
        {
            mMainCameraTrans = Camera.main.transform;
            if (null == mMainCameraTrans)
            {
                return;
            }
        }


        //更新摄像机的角度
		m_CameraXOffset = (m_CameraXOffsetMax - m_CameraXOffsetMin) * m_Scale + m_CameraXOffsetMin;
		m_CameraYOffset = (m_CameraYOffsetMax - m_CameraYOffsetMin) * m_Scale+ m_CameraYOffsetMin;
		m_CameraZOffset = (m_CameraZOffsetMax - m_CameraZOffsetMin) * m_Scale+ m_CameraZOffsetMin;
        Vector3 dir = new Vector3(m_CameraXOffset, m_CameraYOffset, m_CameraZOffset);
        Vector3 cameraPos = mTrans.localPosition + dir;

        mMainCameraTrans.localPosition = cameraPos;

        //更新摄像机的Look点，锁定Y轴偏移
        Vector3 pos = mTrans.position;
        pos.y += m_CenterOffest;
        Vector3 lookPos = pos - mMainCameraTrans.position;
        lookPos.Normalize();

        mMainCameraTrans.rotation = Quaternion.LookRotation(lookPos);
    }
    protected void CarmerRock()
    {
        m_fRockTime += Time.deltaTime;
        float nNewXPos = m_CameraXOffset + m_XRockOff.Evaluate(m_fRockTime);
        float nNewYPos = m_CameraYOffset + m_YRockOff.Evaluate(m_fRockTime);
        float nNewZPos = m_CameraZOffset + m_ZRockOff.Evaluate(m_fRockTime);

        //更新摄像机的角度
        Vector3 dir = new Vector3(nNewXPos, nNewYPos, nNewZPos);
        Vector3 cameraPos = this.gameObject.transform.localPosition + dir;
      
        Camera.main.transform.localPosition = cameraPos;

        //更新摄像机的Look点
        Vector3 pos = this.gameObject.transform.position;
        pos.y += m_CenterOffest;
        Vector3 lookPos = pos - Camera.main.transform.position;
        lookPos.Normalize();

        float nNewRXPos = lookPos.x + m_RXRockOff.Evaluate(m_fRockTime);
        float nNewRYPos = lookPos.y + m_RYRockOff.Evaluate(m_fRockTime);
        float nNewRZPos = lookPos.z + m_RZRockOff.Evaluate(m_fRockTime);
        Camera.main.transform.rotation = Quaternion.LookRotation(new Vector3(nNewRXPos,nNewRYPos,nNewRZPos));
    }
    public  void WriteCurveValue()
    {
        StreamWriter swRet;
        FileInfo RetFile = new FileInfo("D:\\AnimationCurveRet.txt");
        if (RetFile.Exists)
        {
            swRet = new StreamWriter("D:\\AnimationCurveRet.txt", true, System.Text.Encoding.Unicode);
        }
        else
        {
            swRet = new StreamWriter("D:\\AnimationCurveRet.txt", false, System.Text.Encoding.Unicode);
            swRet.WriteLine("动画曲线ID\t描述\t曲线节点索引\t时间\t值\t切入正切角度\t切出正切角度\t正切模式\t开始WrapMode（0Default 1Clamp 1Once 2Loop 4PingPong 8ClampForever）\t结束WrapMode（0Default 1Clamp 1Once 2Loop 4PingPong 8ClampForever）");
        }
        int nIndex = Convert.ToInt32(System.DateTime.Now.ToString("ddHHmmss"))*10;
        if (m_XRockOff.length >0)
        {
            int keyCount = 0;
            nIndex++;
            m_PosxIndex = nIndex;
            foreach (Keyframe frame in m_XRockOff.keys)
            {
                string strLine = nIndex + "\t" + "PosX震屏持续时间:"+m_fNeedRockTime+ "\t" + keyCount + "\t" + frame.time + "\t" + frame.value + "\t" + frame.inTangent + "\t" +
                                 frame.outTangent + "\t" + frame.tangentMode + "\t" + (int)m_XRockOff.preWrapMode + "\t" + (int)m_XRockOff.postWrapMode;
              
                swRet.WriteLine(strLine);
                keyCount++;
            }
        }

        if (m_YRockOff.length > 0)
        {
            int keyCount = 0;
            nIndex++;
            m_PosYIndex = nIndex;
            foreach (Keyframe frame in m_YRockOff.keys)
            {
                string strLine = nIndex + "\t" + "PosY震屏持续时间:" + m_fNeedRockTime + "\t" + keyCount + "\t" + frame.time + "\t" + frame.value + "\t" + frame.inTangent + "\t" +
                                 frame.outTangent + "\t" + frame.tangentMode + "\t" + (int)m_YRockOff.preWrapMode + "\t" + (int)m_YRockOff.postWrapMode;
              
                swRet.WriteLine(strLine);
                keyCount++;
            }
        }
        if (m_ZRockOff.length > 0)
        {
            int keyCount = 0;
            nIndex++;
            m_PosZIndex = nIndex;
            foreach (Keyframe frame in m_ZRockOff.keys)
            {
                string strLine = nIndex + "\t" + "PosZ震屏持续时间:" + m_fNeedRockTime + "\t" + keyCount + "\t" + frame.time + "\t" + frame.value + "\t" + frame.inTangent + "\t" +
                                 frame.outTangent + "\t" + frame.tangentMode + "\t" + (int)m_ZRockOff.preWrapMode + "\t" + (int)m_ZRockOff.postWrapMode;
                swRet.WriteLine(strLine);
                keyCount++;
            }
        }

        if (m_RXRockOff.length > 0)
        {
            int keyCount = 0;
            nIndex++;
            m_RXIndex = nIndex;
            foreach (Keyframe frame in m_RXRockOff.keys)
            {
                string strLine = nIndex + "\t" + "RX震屏持续时间:" + m_fNeedRockTime + "\t" + keyCount + "\t" + frame.time + "\t" + frame.value + "\t" + frame.inTangent + "\t" +
                                 frame.outTangent + "\t" + frame.tangentMode + "\t" + (int)m_RXRockOff.preWrapMode + "\t" + (int)m_RXRockOff.postWrapMode;
                swRet.WriteLine(strLine);
                keyCount++;
            }
        }

        if (m_RYRockOff.length > 0)
        {
            int keyCount = 0;
            nIndex++;
            m_RYIndex = nIndex;
            foreach (Keyframe frame in m_RYRockOff.keys)
            {
                string strLine = nIndex + "\t" + "RY震屏持续时间:" + m_fNeedRockTime + "\t" + keyCount + "\t" + frame.time + "\t" + frame.value + "\t" + frame.inTangent + "\t" +
                                 frame.outTangent + "\t" + frame.tangentMode + "\t" + (int)m_RYRockOff.preWrapMode + "\t" + (int)m_RYRockOff.postWrapMode;
           
                swRet.WriteLine(strLine);
                keyCount++;
            }
        }

        if (m_RZRockOff.length > 0)
        {
            int keyCount = 0;
            nIndex++;
            m_RZIndex = nIndex;
            foreach (Keyframe frame in m_RZRockOff.keys)
            {
                string strLine = nIndex + "\t" + "RZ震屏持续时间:" + m_fNeedRockTime + "\t" + keyCount + "\t" + frame.time + "\t" + frame.value + "\t" + frame.inTangent + "\t" +
                                 frame.outTangent + "\t" + frame.tangentMode + "\t" + (int)m_RZRockOff.preWrapMode + "\t" + (int)m_RZRockOff.postWrapMode;
              
                swRet.WriteLine(strLine);
                keyCount++;
            }
        }

        swRet.Close();
        swRet.Dispose();
    }

  
    public AnimationCurve InitRockOff(int nCurverId)
    {
        AnimationCurve RockCurve = new AnimationCurve();
        if (nCurverId == -1)
        {
            return RockCurve;
        }
        StreamReader srRet;
        FileInfo RetFile = new FileInfo("D:\\AnimationCurveRet.txt");
        if (RetFile.Exists ==false)
        {
            return RockCurve;
        }
        srRet = new StreamReader("D:\\AnimationCurveRet.txt", System.Text.Encoding.Unicode);
        
        List<String> strList =new List<string>();
        string strLine = srRet.ReadLine();
        if (strLine !=null)
        {
            string[] SplitArray = strLine.Split(new char[] {'\t'});
            if (SplitArray[0]!=null &&SplitArray[0] == nCurverId.ToString())
            {
                strList.Add(strLine);
            }
        }
        
       
        while(strLine !=null && srRet.EndOfStream ==false)
        {
            strLine = srRet.ReadLine();
            if (strLine != null)
            {
                string[] SplitArrayNew = strLine.Split(new char[] { '\t' });
                if (SplitArrayNew[0] != null && SplitArrayNew[0] == nCurverId.ToString())
                {
                    strList.Add(strLine);
                } 
            }
        }

        int preWrapMode = -1;
        int postWrapMode = -1;
        if (strList.Count >0)
        {
            Keyframe[] XCurverKeyframes = new Keyframe[strList.Count];
            for (int i = 0; i < strList.Count; i++)
            {
                string[] SplitInfo = strList[i].Split(new char[] {'\t'});
                XCurverKeyframes[i].time =(float) Convert.ToDouble(SplitInfo[3]);
                XCurverKeyframes[i].value = (float)Convert.ToDouble(SplitInfo[4]);
                XCurverKeyframes[i].inTangent = (float)Convert.ToDouble(SplitInfo[5]);
                XCurverKeyframes[i].outTangent = (float)Convert.ToDouble(SplitInfo[6]);
                XCurverKeyframes[i].tangentMode = Convert.ToInt32(SplitInfo[7]);
                preWrapMode = Convert.ToInt32(SplitInfo[8]);
                postWrapMode = Convert.ToInt32(SplitInfo[9]);
            }
            RockCurve = new AnimationCurve(XCurverKeyframes);
            RockCurve.preWrapMode = (WrapMode)preWrapMode;
            RockCurve.postWrapMode = (WrapMode)postWrapMode;
        }
       
           
       
        return RockCurve;
    }
    public void SetRockXCurve(AnimationCurve curve)
    {
        m_XRockOff = curve;
    }
    public void SetRockYCurve(AnimationCurve curve)
    {
        m_YRockOff = curve;
    }
    public void SetRockZCurve(AnimationCurve curve)
    {
        m_ZRockOff = curve;
    }
    public void SetRockRXCurve(AnimationCurve curve)
    {
        m_RXRockOff = curve;
    }
    public void SetRockRYCurve(AnimationCurve curve)
    {
        m_RYRockOff = curve;
    }
    public void SetRockRZCurve(AnimationCurve curve)
    {
        m_RZRockOff = curve;
    }
}