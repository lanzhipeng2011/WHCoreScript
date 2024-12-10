using Games.GlobeDefine;
using UnityEngine;
using System.Collections;
using Games.LogicObj;
using System;
using GCGame;

public class TargetFrameLogic : MonoBehaviour {

    private static TargetFrameLogic m_Instance = null;
    public static TargetFrameLogic Instance()
    {
        return m_Instance;
    }
    public UILabel m_TargetLevelText;
    public GameObject m_TargetFrameOffset;
    public UILabel m_TargetName;
    public UISprite m_TargetHPSprite;
    public UISprite m_TargetHPBakSprite;
    public UILabel m_TargetHPText;
    public BoxCollider m_TargetFrameBoxColliser;
	public GameObject m_UIAnchorGameObject;
    //public TweenAlpha m_FoldTween;

    private bool m_bFold = false;           // 折叠状态 false 目标头像收起 fold 目标头像显示
    private bool m_hasTarget = false;    // 是否有目标
    private float m_fHPBakSpeed = 0.003f;
    private int m_nLastHp = 0;
    //保存目标的某些信息
    private UInt64 m_TargetGuid = GlobeVar.INVALID_GUID;
    public System.UInt64 TargetGuid
    {
        get { return m_TargetGuid; }
        set { m_TargetGuid = value; }
    }

    private string m_strTargetName = "";
    public string StrTargetName
    {
        get { return m_strTargetName; }
        set { m_strTargetName = value; }
    }

    private int m_TargetServerID = GlobeVar.INVALID_ID;
    public int TargetServerID
    {
        get { return m_TargetServerID; }
        set { m_TargetServerID = value; }
    }
    
    void Awake()
    {
        m_Instance = this;
        //gameObject.SetActive(false);
        
    }

	// Use this for initialization
	void Start () {
        //InitUITweenerWhenChangeScene();
        HideTargetFrame();
        m_TargetHPText.text = "0/0";
        m_TargetHPSprite.fillAmount = 0;
        m_TargetHPBakSprite.fillAmount = 0;
	}
	
	// Update is called once per frame
	void Update () 
    {
		//==当选择目标消失的时候隐藏头像
		if(thisTargetObj == null)
		{
			CancelTarget();
			return;
		}
		//==在安卓平台下目标隐藏的时候隐藏头像
		if(!thisTargetObj.gameObject.activeInHierarchy)
		{
			CancelTarget();
			return;
		}
		//====当NPC目标距离玩家超过10隐藏头像
		if(thisTargetObj.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_NPC)
		{
			Obj_NPC currTarget =  thisTargetObj as Obj_NPC;
			if(currTarget.m_fLastDis2MainPlayer > 10)
			{
				thisTargetObj = null;
				CancelTarget();
				return;
			}
		}

        //残血
        if (m_TargetHPSprite.fillAmount >0 && m_TargetHPBakSprite.fillAmount<=0)
	    {
	        m_TargetHPBakSprite.fillAmount =m_TargetHPSprite.fillAmount;
	    }
	    else if (m_TargetHPSprite.fillAmount-m_TargetHPBakSprite.fillAmount<0)
	    {
	        m_TargetHPBakSprite.fillAmount -= m_fHPBakSpeed;
            if (m_TargetHPSprite.fillAmount - m_TargetHPBakSprite.fillAmount >=0)
            {
                m_TargetHPBakSprite.fillAmount=m_TargetHPSprite.fillAmount;
            }
	    }
	}

    void OnDestroy()
    {
        m_Instance = null;
    }

    public void PlayTween(bool nDirection)
    {
        m_bFold = nDirection;
        if (!m_bFold)
        {
            if (m_hasTarget)
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

	//===存储当前选择对象信息
	private Obj_Character thisTargetObj;
    /// <summary>
    /// 切换目标
    /// </summary>
    /// <param name="targetObj">目标obj</param>
    public void ChangeTarget(Obj_Character targetObj)
    {
        if (!m_bFold)
        {
            ShowTargetFrame();
        }

        m_hasTarget = true;
        bool isSameTarget = false;
        if (null == targetObj)
            return;

		thisTargetObj = targetObj;

        if (m_TargetServerID == targetObj.ServerID)
        {
            isSameTarget = true;
        }
        else
        {
            m_TargetLevelText.text = "0";
            m_TargetHPText.text = "0/0";
            m_TargetHPSprite.fillAmount = 0;
            m_TargetHPBakSprite.fillAmount = 0;
            m_nLastHp = 0;
        }
        m_TargetServerID = targetObj.ServerID;
        if (targetObj.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER || 
            targetObj.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER ||
            targetObj.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_ZOMBIE_PLAYER)
        {
            Obj_OtherPlayer otherPlayer = targetObj as Obj_OtherPlayer;
            if (null != otherPlayer)
            {
                m_TargetGuid = otherPlayer.GUID;
                m_strTargetName = otherPlayer.BaseAttr.RoleName;
            }
        }
        else
        {
            m_TargetGuid = GlobeVar.INVALID_GUID;
            m_strTargetName = "";
        }
        m_TargetLevelText.text = targetObj.BaseAttr.Level.ToString();
        
        SetTargetName(targetObj);

        bool isWorldBoss = (targetObj.BaseAttr.RoleBaseID == 847);
        if (isWorldBoss == true || IsGuildBossNpc(targetObj) == true)
        {
            //m_TargetHPText.text = "???" + "/" + "???";
            float perCent = (float)targetObj.BaseAttr.HP / (float)targetObj.BaseAttr.MaxHP * 100;
            m_TargetHPText.text = perCent.ToString("0.000") + "%";
        }
        else
        {
            m_TargetHPText.text = Utils.ConvertLargeNumToString(targetObj.BaseAttr.HP) + "/" + Utils.ConvertLargeNumToString(targetObj.BaseAttr.MaxHP);
        }
        
        if (targetObj.BaseAttr.MaxHP !=0)
        {
            m_TargetHPSprite.fillAmount = (float)targetObj.BaseAttr.HP / (float)targetObj.BaseAttr.MaxHP;
        }
        else
        {
            m_TargetHPSprite.fillAmount = 0;
        }
        if (m_TargetHPSprite.fillAmount <=0)
        {
            m_TargetHPSprite.fillAmount = 0;
            m_TargetHPBakSprite.fillAmount = 0;
            m_fHPBakSpeed = 0.003f;
        }
        else if (m_TargetHPBakSprite.fillAmount - m_TargetHPSprite.fillAmount>0)
        {
            m_fHPBakSpeed = (m_TargetHPBakSprite.fillAmount - m_TargetHPSprite.fillAmount)*Time.deltaTime;
        }
        else if (m_TargetHPBakSprite.fillAmount - m_TargetHPSprite.fillAmount < 0)
        {
            m_TargetHPBakSprite.fillAmount = m_TargetHPSprite.fillAmount;
        }
        if (isSameTarget && m_nLastHp <= targetObj.BaseAttr.HP)
        {
            m_TargetHPBakSprite.fillAmount = m_TargetHPSprite.fillAmount;
        }
          
       

        m_nLastHp = targetObj.BaseAttr.HP;
    }

    public void CancelTarget()
    {
        if (gameObject.activeSelf)
        {
            HideTargetFrame();
        }
        m_hasTarget = false;
        m_TargetGuid = GlobeVar.INVALID_GUID;
        m_TargetServerID = GlobeVar.INVALID_ID;
        m_strTargetName = "";

        m_TargetLevelText.text = "0";
        m_TargetHPText.text = "0/0";
        m_TargetHPSprite.fillAmount = 0;
        m_TargetHPBakSprite.fillAmount = 0;
        m_nLastHp = 0;
    }

    /// <summary>
    /// 应对切换场景时UI异常消失 重新初始化Tween动画
    /// </summary>
    //void InitUITweenerWhenChangeScene()
    //{
    //    m_FoldTween.Reset();
    //    m_FoldTween.alpha = 1;
    //    m_FoldTween.from = 1;
    //    m_FoldTween.to = 0;
    //}


    void ShowTargetFrame()
    {
        //m_FoldTween.alpha = 1;
        m_TargetFrameBoxColliser.enabled = true;
        gameObject.SetActive(true);
    }

    void HideTargetFrame()
    {
        //m_FoldTween.alpha = 0;
        m_TargetFrameBoxColliser.enabled = false;
        gameObject.SetActive(false);
    }
    
    //void SetTargetFrameTweenAlpha()
    //{
    //    if (m_bFold)
    //    {
    //        if (!m_hasTarget)
    //        {
    //            m_FoldTween.Reset();
    //            m_FoldTween.from = 0;
    //            m_FoldTween.to = 0;
    //            m_TargetFrameBoxColliser.enabled = false;
    //            m_FoldTween.Play();
    //        }
    //        else
    //        {
    //            m_FoldTween.Reset();
    //            m_FoldTween.from = 1;
    //            m_FoldTween.to = 0;
    //            m_TargetFrameBoxColliser.enabled = false;
    //            m_FoldTween.Play();
    //        }
    //    }
    //    else
    //    {
    //        if (!m_hasTarget)
    //        {
    //            m_FoldTween.Reset();
    //            m_FoldTween.from = 0;
    //            m_FoldTween.to = 0;
    //            m_TargetFrameBoxColliser.enabled = false;
    //            m_FoldTween.Play();
    //        }
    //        else
    //        {
    //            m_FoldTween.Reset();
    //            m_FoldTween.from = 0;
    //            m_FoldTween.to = 1;
    //            m_TargetFrameBoxColliser.enabled = true;
    //            m_FoldTween.Play();
    //        }
    //    }
    //}

    //点击头像弹出菜单
    void OnClickTargetFrame()
    {
        //只有玩家可以弹出菜单，所以发现guid非法则不弹
        if (m_TargetGuid == GlobeVar.INVALID_GUID || m_strTargetName == "")
        {
            return;
        }

        //显示则点击消失，隐藏则点击显示
        if (PopMenuLogic.Instance() == null)
        {
            PopMenuLogic.ShowMenu("TargetFramePopMenu", /*m_TargetHeadSprite.gameObject*/m_UIAnchorGameObject);
            return;
        }
        else
        {
            UIManager.CloseUI(UIInfo.PopMenuRoot);
        }

    }

    void SetTargetName(Obj_Character targetObj)
    {
        m_TargetName.text = targetObj.BaseAttr.RoleName;
        m_TargetName.color = targetObj.GetNameBoardColor();
    }

    bool IsGuildBossNpc(Obj_Character targetObj)
    {
        if (targetObj.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_NPC)
        {
            Obj_NPC rNpc = targetObj as Obj_NPC;
            if (rNpc != null)
            {
                return rNpc.IsGuildActivityBoss;
            }
        }
        return false;
    }
}
