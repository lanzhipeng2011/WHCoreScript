/********************************************************************************
 *	文件名：MissionItemLogic.cs
 *	全路径：	\Script\GUI\MissionItemLogic.cs
 *	创建人：	贺文鹏
 *	创建时间：2014-02-17
 *
 *	功能说明： 任务追踪界面 任务项UI。
 *	       
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using GCGame.Table;
using Games.Mission;
using Games.Events;

public class MissionItemLogic : MonoBehaviour
{
    public UILabel m_MissionTitle;		//任务追踪标题
    public UILabel MissionTile
    {
        get { return m_MissionTitle; }
    }
    public UILabel m_MissionInfo;		//任务追踪信息
    public UILabel MissionInfo
    {
        get { return m_MissionInfo; }
    }
    private int m_MissionID = -1;
    public int MissionID
    {
        get { return m_MissionID; }
        set { m_MissionID = value; }
    }

    public Transform m_EffectSprite;

	void Awake()
	{
		UpdateMissionFollowBlink(false);
	}
	// Use this for initialization
	void Start () {
        Init();
	}

    void Init()
    {
        TweenAlpha[] missionTableAlphaArray = m_MissionTitle.gameObject.GetComponents<TweenAlpha>();
        for (int i = 0; i < missionTableAlphaArray.Length; ++i)
        {
            if (missionTableAlphaArray[i].tweenGroup == 2)
            {
                missionTableAlphaArray[i].enabled = false;
                if (!enabled)
                {
                    missionTableAlphaArray[i].Reset();
                }
            }
        }
        
        TweenAlpha[] missionTitleAlphaArray = m_MissionTitle.gameObject.GetComponents<TweenAlpha>();
        for (int i = 0; i < missionTitleAlphaArray.Length; ++i)
        {
            if (missionTitleAlphaArray[i].tweenGroup == 2)
            {
                missionTitleAlphaArray[i].enabled = false;
                if (!enabled)
                {
                    missionTitleAlphaArray[i].Reset();
                }
            }
        }
    }

    // 点击MissionItem
    void MissionItemClicked()
    {
		NewPlayerGuidLogic.CloseWindow ();

        if (GameManager.gameManager.MissionManager != null)
        {
			if (m_MissionID == 12 && GameManager.gameManager.RunningScene == 14) 
			{
				Singleton<ObjManager>.GetInstance().MainPlayer.EnterAutoCombat();
				if(SGAutoFightBtn.Instance!=null)
				{
					SGAutoFightBtn.Instance.UpdateAutoFightBtnState();
				}
				return;
			}
            GameManager.gameManager.MissionManager.MissionPathFinder(m_MissionID);
			Singleton<ObjManager>.GetInstance().MainPlayer.LeaveTeamFollow();
        }
    }

    public void UpdateMissionFollowBlink(bool enabled)
    {
//         foreach (TweenAlpha nTween in m_MissionTitle.gameObject.GetComponents<TweenAlpha>())
//         {
//             if (nTween.tweenGroup == 2)
//             {
//                 nTween.enabled = enabled;
//                 if (!enabled)
//                 {
//                     nTween.Reset();
//                 }
//             }
//         }
//         foreach (TweenAlpha nTween in m_MissionInfo.gameObject.GetComponents<TweenAlpha>())
//         {
//             if (nTween.tweenGroup == 2)
//             {
//                 nTween.enabled = enabled;
//                 if (!enabled)
//                 {
//                     nTween.Reset();
//                 }
//             }
//         }

        if (m_EffectSprite)
        {
            m_EffectSprite.gameObject.SetActive(enabled);
        }
        
    }
}
