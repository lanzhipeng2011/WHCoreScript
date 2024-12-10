/********************************************************************************
 *	文件名：	BackCamerControll.cs
 *	全路径：	\Script\Player\Skill\BackCamerControll.cs
 *	创建人：	罗勇
 *	
 *
 *	功能说明：备用摄像机逻辑脚本
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using Module.Log;
using Games.LogicObj;
public class BackCamerControll : MonoBehaviour {

	public GameObject m_BlackTex;
    private Transform m_Blacktransform;

    public UILabel m_BlackScreenLabel;
    public TweenAlpha m_BlackScreenLabelTween;
    private bool m_bIsBlackScreenPlaying = false;//是否正常播放黑屏
    private bool m_bIsBlackScreenLabelPlaying = false;//是否正常播放黑屏中央文字
    private static BackCamerControll m_Instance = null;
   
    private EffectLogic m_SceneEffecLogic;
    public EffectLogic SceneEffecLogic
    {
        get { return m_SceneEffecLogic; }
        set { m_SceneEffecLogic = value; }
    }
    public TweenAlpha m_BlackScreenTween;
    private bool m_bIsCloseIdle = true; //空闲不用时是否关闭改摄像机
    public bool IsCloseIdle
    {
        get { return m_bIsCloseIdle; }
        set { m_bIsCloseIdle = value; }
    }
    void Awake()
    {
        m_Instance = this;
    }

    public static BackCamerControll Instance()
    {
        return m_Instance;
    }
	// Use this for initialization
	void Start ()
	{
        m_bIsCloseIdle = true;
	    m_Blacktransform = m_BlackTex.transform;
        if (m_SceneEffecLogic == null)
        {
            m_SceneEffecLogic = gameObject.AddComponent<EffectLogic>();
            if (m_SceneEffecLogic != null)
            {
                m_SceneEffecLogic.InitEffect(gameObject);
            }
        }
        gameObject.SetActive(false);
	}

    
    
	// Update is called once per frame
	void Update () 
    {
        //空闲不用时关闭摄像机（不播放 全屏特效 黑屏 黑屏中央文字 ）
        if (m_bIsCloseIdle)
	    {
            if (m_bIsBlackScreenLabelPlaying == false &&
              m_bIsBlackScreenPlaying == false)
            {
                if (m_SceneEffecLogic != null && m_SceneEffecLogic.EffectCountCache.Count <= 0)
                {
                    if (gameObject.activeInHierarchy)
                    {
                        gameObject.SetActive(false);
                    }
                }
                else if (m_SceneEffecLogic == null)
                {
                    if (gameObject.activeInHierarchy)
                    {
                        gameObject.SetActive(false);
                    }
                }
            }  
	    }
	}
    public void StopSceneEffect(int effectID,bool bStopAll)
    {
        if (m_SceneEffecLogic)
        {
            m_SceneEffecLogic.StopEffect(effectID,bStopAll);
        }
    }
    public void PlaySceneEffect(int effectID,EffectLogic.PlayEffectDelegate delPlayEffect = null, object param = null)
    {
        gameObject.SetActive(true);
       
        if (m_SceneEffecLogic)
        {
            m_SceneEffecLogic.PlayEffect(effectID, delPlayEffect, param);
        }
    }

    public void InitBlackScreenTween(float duration, float to = 1, float delay = 0)
    {
        if (gameObject.activeInHierarchy ==false)
        {
            gameObject.SetActive(true);
        }
        m_bIsBlackScreenPlaying = true;
        m_BlackScreenTween.Reset();
        m_BlackScreenTween.duration = duration;
        m_BlackScreenTween.to = to;
        m_BlackScreenTween.delay = delay;
    }

    public void PlayBlackScreenTween()
    {
        m_BlackScreenTween.Play();
    }

    public void AfterPlayBlackScreen()
    {
//         Obj_MainPlayer mainplayer = Singleton<ObjManager>.Instance.MainPlayer;
//         if (mainplayer != null)
//         {
//             if (mainplayer.IsInModelStory)
//             {
//                 mainplayer.IsInModelStory = false;
//             }
//         }
        m_bIsBlackScreenPlaying = false;
    }
    public void AfterPlayBlackScreenLable()
    {
        m_bIsBlackScreenLabelPlaying = false;
    }
    public void InitBlackScreenLabel(string text)
    {
        m_BlackScreenLabel.text = text;
    }

    public void InitBlackScreenLabelTween(float duration, float to = 1, float delay = 0)
    {
        if (gameObject.activeInHierarchy == false)
        {
            gameObject.SetActive(true);
        }
        m_bIsBlackScreenLabelPlaying = true;
        m_BlackScreenLabelTween.Reset();
        m_BlackScreenLabelTween.duration = duration;
        m_BlackScreenLabelTween.to = to;
        m_BlackScreenLabelTween.delay = delay;
    }

    public void PlayBlackScreenLabelTween()
    {
        m_BlackScreenLabelTween.Play();
    }
}
