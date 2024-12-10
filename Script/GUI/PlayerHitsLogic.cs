//********************************************************************
// 文件名: PlayerHitsLogic.cs
// 描述: 玩家连击数逻辑 暂时
// 作者: WangZhe
// 创建时间: 2013-11-19
//
// 修改历史:
// 2013-11-19 王喆创建
//********************************************************************

using UnityEngine;
using System.Collections;
using Games.LogicObj;
using Games.GlobeDefine;

public class PlayerHitsLogic : MonoBehaviour {

    //鼠标按键枚举
    public enum MOUSE_BUTTON
    {
        MOUSE_BUTTON_LEFT,
        MOUSE_BUTTON_RIGHT,
        MOUSE_BUTTON_MIDDLE,
    }


    private float m_HitsEndTime;

    public GameObject m_FirstChild;
    public UISprite m_BackGround;
    public UISprite m_BackGroundHit;
    public UIGrid m_NumPicGrid;
    public UISprite m_NumPic_Ge;
    public UISprite m_NumPic_Shi;
    public UISprite m_NumPic_Bai;
        
    public GameObject m_EffectOffset;
    // private TweenPosition m_JumpEffect;
    private TweenScale m_CriticalEffect;
    private bool m_bShow = false;

    private static PlayerHitsLogic m_Instance = null;
    public static PlayerHitsLogic Instance()
    {
        return m_Instance;
    }

    void Awake()
    {
        m_Instance = this;
    }

	// Use this for initialization
	void Start () 
    {
        m_BackGround.gameObject.SetActive(true);
        m_BackGround.color = GlobeVar.TRANSPARENT_COLOR;

        m_BackGroundHit.gameObject.SetActive(true);
        m_BackGroundHit.color = GlobeVar.TRANSPARENT_COLOR;

        m_NumPic_Ge.gameObject.SetActive(true);
        m_NumPic_Ge.color = GlobeVar.TRANSPARENT_COLOR;

        m_NumPic_Bai.gameObject.SetActive(true);
        m_NumPic_Bai.color = GlobeVar.TRANSPARENT_COLOR;

        m_NumPic_Shi.gameObject.SetActive(true);
        m_NumPic_Shi.color = GlobeVar.TRANSPARENT_COLOR;

        // m_JumpEffect = m_EffectOffset.GetComponent<TweenPosition>();
        m_CriticalEffect = m_EffectOffset.GetComponent<TweenScale>();

        m_FirstChild.SetActive(true);

        gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
       
        // 5秒后没有连击算作中断 清空连击数
        ClearPlayerHits();
	}

    void OnDestroy()
    {
        m_Instance = null;
    }
    
    /// <summary>
    /// 增加连击数
    /// </summary>
    public void AddPlayerHits(int nHitNum, bool bCritical = false)
    {
        if (nHitNum >= 2)
        {
            m_BackGround.color = Color.white; 
            m_BackGroundHit.color = Color.white;
            if (nHitNum < 10)
            {
                m_NumPic_Ge.color = Color.white;
                m_NumPic_Shi.color = GlobeVar.TRANSPARENT_COLOR;
                m_NumPic_Bai.color = GlobeVar.TRANSPARENT_COLOR;
                m_NumPicGrid.transform.localPosition = new Vector3(-121, 0, 0);
            }
            else if (nHitNum < 100)
            {
                m_NumPic_Ge.color = Color.white;
                m_NumPic_Shi.color = Color.white;
                m_NumPic_Bai.color = GlobeVar.TRANSPARENT_COLOR;
                m_NumPicGrid.transform.localPosition = new Vector3(-107, 0, 0);
            }
            else if (nHitNum < 1000)
            {
                m_NumPic_Ge.color = Color.white;
                m_NumPic_Shi.color = Color.white;
                m_NumPic_Bai.color = Color.white;
                m_NumPicGrid.transform.localPosition = new Vector3(-80, 0, 0);
            }
            m_NumPic_Ge.spriteName = "Num" + (nHitNum % 10).ToString();
            m_NumPic_Shi.spriteName = "Num" + (nHitNum % 100 / 10).ToString();
            m_NumPic_Bai.spriteName = "Num" + (nHitNum / 100).ToString();
            //ClearTweenAlphaTwinkle();
            ResetNumFadeAni();
            m_HitsEndTime = Time.fixedTime;

            if (!m_bShow)
            {
                m_bShow = true;
                gameObject.SetActive(m_bShow);
            }

            //if (bCritical)
            //{
            PlayScaleEffect();
            //}
            //else
            //{
            //    PlayJumpEffect();
            //}
        }
    }

    /// <summary>
    /// 清空连击数
    /// </summary>
    void ClearPlayerHits()
    {
        if (m_bShow)
        {
            if (Time.fixedTime - m_HitsEndTime > 5.0f)
            {
                m_BackGround.color = GlobeVar.TRANSPARENT_COLOR; 
                m_BackGroundHit.color = GlobeVar.TRANSPARENT_COLOR;
                m_NumPic_Ge.color = GlobeVar.TRANSPARENT_COLOR;
                m_NumPic_Bai.color = GlobeVar.TRANSPARENT_COLOR;
                m_NumPic_Shi.color = GlobeVar.TRANSPARENT_COLOR;
                m_bShow = false;

                ResetNumFadeAni();

                gameObject.SetActive(false);
            }
            //else if (Time.fixedTime - m_HitsEndTime > 1.0f)
            //{
            //    PlayNumFadeAni();
            //}
        }
    }
    
    //void PlayNumFadeAni()
    //{
    //    if (m_NumPic_Ge.gameObject.activeSelf)
    //    {
    //        m_NumPic_Ge.fillAmount = 1.0f - (Time.fixedTime - m_HitsEndTime - 1.0f) / 4.0f;
    //    }
    //    if (m_NumPic_Shi.gameObject.activeSelf)
    //    {
    //        m_NumPic_Shi.fillAmount = 1.0f - (Time.fixedTime - m_HitsEndTime - 1.0f) / 4.0f;
    //    }
    //    if (m_NumPic_Bai.gameObject.activeSelf)
    //    {
    //        m_NumPic_Bai.fillAmount = 1.0f - (Time.fixedTime - m_HitsEndTime - 1.0f) / 4.0f;
    //    }
    //}

    void ResetNumFadeAni()
    {
        m_NumPic_Ge.fillAmount = 1.0f;
        m_NumPic_Shi.fillAmount = 1.0f;
        m_NumPic_Bai.fillAmount = 1.0f;
    }

    //public void PlayJumpEffect()
    //{
    //    m_JumpEffect.Reset();
    //    m_JumpEffect.Play();
    //}

    void PlayScaleEffect()
    {
        m_CriticalEffect.Reset();
        m_CriticalEffect.Play();
    }

    public void PlayTween(bool nDirection)
    {
        gameObject.SetActive(!nDirection);
    }
}
