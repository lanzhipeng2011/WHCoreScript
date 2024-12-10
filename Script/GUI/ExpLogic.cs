using UnityEngine;
using System.Collections;
using GCGame.Table;
using Games.LogicObj;
using System.Collections.Generic;
using GCGame;
using Module.Log;
public class ExpLogic : MonoBehaviour {

    private static ExpLogic m_Instance = null;
    public static ExpLogic Instance()
    {
        return m_Instance;
    }

    public GameObject m_FirstChild;
    public GameObject m_ExpProgress;
    public UISprite m_ExpBackground;
    public UISprite m_ExpSprite;
    public UILabel m_ExpLabel;
    public UISprite ExpIcon;
    public UISlider m_OffLineExpProgress;
    //public List<TweenAlpha> m_FoldTween;

    //private bool m_bExpAni = false;
    private int m_CurExpBuffer = 0;
    private int m_MaxExpBuffer = 0;
    private int m_CurExp = 0;
    private int m_MaxExp = 0;
    //当前离线经验以及离线经验上限
    private int m_CurOffLineExp = 0;
    private int m_MaxOffLineExp = 0;

    private int m_PlayerLevelBuffer = 0;

    public float AddExpRatio = 0.005f;      // 经验值条每帧增长比例

    void Awake()
    {
        m_Instance = this;
        transform.localPosition = new Vector3(0, -339, 0);
    }

	// Use this for initialization
	void Start ()
    {
        m_FirstChild.SetActive(true);
//         if (Screen.width % 1136 == 0)
//         {
//             m_ExpProgress.transform.localPosition = new Vector3(-666, 8, 0);
//             m_ExpBackground.GetComponent<UIWidget>().width = 1332;
//             m_ExpSprite.GetComponent<UIWidget>().width = 1305;
//         }

        UpdateExp();
        UpdateOffLineExp();
        //InitPlayerExp();
        UpdateLayout();
	}

    void UpdateLayout() 
    {

        float IconWidth = this.ExpIcon.width;
        float mheight = UIRoot.list[0].manualHeight;
        float astNum =  (float)Screen.width / (float)Screen.height;
        float expWidth = (int)(mheight * astNum) - IconWidth;
        //expWidth = expWidth * (1 / 0.8f);
        this.m_ExpBackground.width = (int)expWidth;
        this.m_ExpSprite.width = (int)expWidth;

        this.m_ExpBackground.transform.localPosition = new Vector3(-expWidth * 0.5f + IconWidth * 0.5f, this.m_ExpBackground.transform.localPosition.y, 0);
        this.m_ExpSprite.transform.localPosition = new Vector3(-expWidth * 0.5f + IconWidth * 0.5f, this.m_ExpSprite.transform.localPosition.y, 0);

        this.ExpIcon.transform.localPosition = new Vector3(-expWidth * 0.5f - IconWidth * 0.5f + 2.5f, this.ExpIcon.transform.localPosition.y, 0);
    }

    void OnDestroy()
    {
        m_Instance = null;
    }

    public void UpdateExp()
    {
        Obj_MainPlayer mainPlayer = Singleton<ObjManager>.Instance.MainPlayer;
        if(null  == mainPlayer)
        {
            return;
        }
        Tab_LevelUp curTabLevelup = TableManager.GetLevelUpByID(mainPlayer.BaseAttr.Level, 0);
        if (null == curTabLevelup)
        {
            return;
        }

        m_MaxExp = curTabLevelup.ExpNeed;
        m_CurExp = mainPlayer.BaseAttr.Exp;

        if (m_MaxExp != 0)
            m_ExpProgress.GetComponent<UISlider>().value = (float)m_CurExp / (float)m_MaxExp;

        m_ExpLabel.text = m_CurExp.ToString() + "/" + m_MaxExp.ToString();
        //m_ExpLabel.text = Utils.ConvertLargeNumToString(m_CurExp) + "/" + Utils.ConvertLargeNumToString(m_MaxExp);
        UpdateOffLineProgress();
    }

    void InitPlayerExp()
    {
        Obj_MainPlayer mainPlayer = Singleton<ObjManager>.Instance.MainPlayer;
        if (null == mainPlayer)
        {
            return;
        }

        m_PlayerLevelBuffer = mainPlayer.BaseAttr.Level;
        m_CurExp = mainPlayer.BaseAttr.Exp;

        Tab_LevelUp curTabLevelup = TableManager.GetLevelUpByID(m_PlayerLevelBuffer, 0);
        if (null == curTabLevelup)
        {
            return;
        }
        m_MaxExp = curTabLevelup.ExpNeed;

        if(m_MaxExp != 0)
        {
            m_ExpProgress.GetComponent<UISlider>().value = (float)m_CurExp / (float)m_MaxExp;
            m_CurExpBuffer = m_CurExp;
            m_MaxExpBuffer = m_MaxExp;
            m_ExpLabel.text = m_CurExp.ToString() + "/" + m_MaxExp.ToString();
            //m_ExpLabel.text = Utils.ConvertLargeNumToString(m_CurExp) + "/" + Utils.ConvertLargeNumToString(m_MaxExp);
            //m_bExpAni = true;
        }
        UpdateOffLineProgress();
    }

    public void OnLevelChange()
    {
        Obj_MainPlayer mainPlayer = Singleton<ObjManager>.Instance.MainPlayer;
        if (null == mainPlayer)
        {
            return;
        }
        Tab_LevelUp curTabLevelup = TableManager.GetLevelUpByID(mainPlayer.BaseAttr.Level, 0);
        if (null == curTabLevelup)
        {
            return;
        }

        m_MaxExp = curTabLevelup.ExpNeed;
        m_CurExp = mainPlayer.BaseAttr.Exp;
        if ( m_MaxExp > 0)
        {
            m_ExpProgress.GetComponent<UISlider>().value = (float)m_CurExp / (float)m_MaxExp;
        }
        m_ExpLabel.text = m_CurExp.ToString() + "/" + m_MaxExp.ToString();
        //m_ExpLabel.text = Utils.ConvertLargeNumToString(m_CurExp) + "/" + Utils.ConvertLargeNumToString(m_MaxExp);
        UpdateOffLineExp();
    }

    public void UpdateOffLineExp()
    {
        Obj_MainPlayer mainPlayer = Singleton<ObjManager>.Instance.MainPlayer;
        if (null == mainPlayer)
        {
            return;
        }
        Tab_OffLineExp curTabOffLine = TableManager.GetOffLineExpByID(mainPlayer.BaseAttr.Level, 0);
        if (null == curTabOffLine)
        {
            return;
        }

        m_MaxOffLineExp = curTabOffLine.OffLineExpLimit;
        m_CurOffLineExp = mainPlayer.BaseAttr.OffLineExp;
        UpdateOffLineProgress();
        //m_ExpProgress.GetComponent<UISlider>().value = (float)m_CurExp / (float)m_MaxExp;
       //m_ExpLabel.text = Utils.ConvertLargeNumToString(m_CurExp) + "/" + Utils.ConvertLargeNumToString(m_MaxExp);
    }
    public void UpdateOffLineProgress()
    {
        if (null == m_OffLineExpProgress)
        {
            return;
        }
        int nRemainLevelUpExp = m_MaxExp - m_CurExp;
        if (nRemainLevelUpExp <= 0)
        {
            m_OffLineExpProgress.value = (float)1.0;
            return;
        }
        if (m_MaxOffLineExp <= 0)
        {
            m_OffLineExpProgress.value = (float)0.0;
            return;
        }

        if (m_CurOffLineExp <= 0)
        {
            m_OffLineExpProgress.value = (float)0.0;
            return;
        }

        double fTempTotal = (double)(m_MaxExp)/(double)(nRemainLevelUpExp)*(double)m_MaxOffLineExp;
        double fTemp = (double)(m_CurExp)/(double)(nRemainLevelUpExp)*(double)m_MaxOffLineExp;
        if (fTempTotal <= 0)
        {
            m_OffLineExpProgress.value = (float)0.0;
            return;
        }
        double fGrogressValue = (double)(m_CurOffLineExp + fTemp) / (double)(fTempTotal);

        //
        double dExpProgressValue = m_ExpProgress.GetComponent<UISlider>().value;
        double dTemp = fGrogressValue - dExpProgressValue;
        if (dTemp < 0.01)
        {
            fGrogressValue = fGrogressValue + 0.01;
        }

        if (fGrogressValue <= 0.03 )
        {
            fGrogressValue = 0.04;
        }
        m_OffLineExpProgress.value = (float)fGrogressValue;
       
    }
    void UpdateExpAnimation()
    {
        if (m_CurExpBuffer != m_CurExp || m_MaxExpBuffer != m_MaxExp)
        {
            // 普通增长经验
            if (m_MaxExp == m_MaxExpBuffer)
            {
                if (m_CurExpBuffer + AddExpRatio * m_MaxExpBuffer >= m_CurExp)
                {
                    // 增长结束
                    m_CurExpBuffer = m_CurExp;
                }
                else
                {
                    m_CurExpBuffer += (int)(AddExpRatio * m_MaxExpBuffer);
                }
                m_ExpProgress.GetComponent<UISlider>().value = (float)m_CurExpBuffer / (float)m_MaxExpBuffer;
                m_ExpLabel.text = m_CurExpBuffer.ToString() + "/" + m_MaxExpBuffer.ToString();
                //m_ExpLabel.text = Utils.ConvertLargeNumToString(m_CurExpBuffer) + "/" + Utils.ConvertLargeNumToString(m_MaxExpBuffer);
            }       
            // 升级
            else if (m_MaxExp > m_MaxExpBuffer)
            {
                if (m_CurExpBuffer + AddExpRatio * m_MaxExpBuffer >= m_MaxExpBuffer)
                {
                    // 经验满了
                    m_CurExpBuffer = m_MaxExpBuffer;
                    m_ExpProgress.GetComponent<UISlider>().value = (float)m_CurExpBuffer / (float)m_MaxExpBuffer;

                    // 计算下一级经验
                    m_PlayerLevelBuffer++;
                    Tab_LevelUp nextTabLevelup = TableManager.GetLevelUpByID(m_PlayerLevelBuffer, 0);
                    if (null == nextTabLevelup)
                    {
                        return;
                    }
                    m_MaxExpBuffer = nextTabLevelup.ExpNeed;
                    m_CurExpBuffer = 0;
                }  
                else
                {
                    m_CurExpBuffer += (int)(AddExpRatio * m_MaxExpBuffer);
                }
                m_ExpProgress.GetComponent<UISlider>().value = (float)m_CurExpBuffer / (float)m_MaxExpBuffer;
                m_ExpLabel.text = m_CurExpBuffer.ToString() + "/" + m_MaxExpBuffer.ToString();
                //m_ExpLabel.text = Utils.ConvertLargeNumToString(m_CurExpBuffer) + "/" + Utils.ConvertLargeNumToString(m_MaxExpBuffer);
            }
        }
    }

    public void PlayTween(bool nDirection)
    {
        //foreach (TweenAlpha tween in m_FoldTween)
        //{
        //    tween.Play(nDirection);
        //}
        gameObject.SetActive(!nDirection);
    }
}
