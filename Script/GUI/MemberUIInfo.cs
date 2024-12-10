//********************************************************************
// 文件名: MemberUIInfo.cs
// 描述: 组队队员UI数据
// 作者: 李嘉
// 创建时间: 2014-01-26
//
// 修改历史:
// 2014-01-26 李嘉创建
//********************************************************************
using UnityEngine;
using System.Collections;
using Games.GlobeDefine;

public class MemberUIInfo : MonoBehaviour
{
    public UISprite m_HeadIcon;        //头像
	public UILabel m_MemberName;       //名字
	public UILabel m_LevelText;        //等级
	public UISprite m_HP;              //生命值图片
	public UILabel m_HPText;           //生命值数值
    public UISprite m_Captain;          //队长图标
	public GameObject m_CaptainTrail;	// 队长跟随

	private System.UInt64 m_MemberId = GlobeVar.INVALID_GUID;

    public void UpdateInfo(TeamMember member)
    {
        if (member.Guid == GlobeVar.INVALID_GUID)
        {
            return;
        }

		m_MemberId = member.Guid;

        if (null != m_HeadIcon)
        {
            if (member.Profession >= 0 && member.Profession < (int)CharacterDefine.PROFESSION.MAX)
            {
                m_HeadIcon.spriteName = GlobeVar.m_HeadIcon[member.Profession];
            }
        }
        if (null != m_LevelText)
        {
            m_LevelText.text = member.Level.ToString();
        }
        if (null != m_MemberName)
        {
            m_MemberName.text = member.MemberName;
        }
        if (null != m_HP && null != m_HPText)
        {
            if (member.MaxHP == 0)
            {
                m_HP.fillAmount = 1;
            }
            else
            {
                m_HP.fillAmount = (float)member.HP / (float)member.MaxHP;
            }

            m_HPText.text = member.HP.ToString() + '/' + member.MaxHP.ToString();
        }

        if (null != Singleton<ObjManager>.GetInstance().MainPlayer && null != m_Captain)
        {
            if (Singleton<ObjManager>.GetInstance().MainPlayer.IsTeamLeader(member.Guid))
            {
                m_Captain.gameObject.SetActive(true);
				m_CaptainTrail.SetActive(true);
				if(Singleton<ObjManager>.GetInstance().MainPlayer.IsTeamFollowState())
				{
					m_CaptainTrail.GetComponent<UIImageButton>().normalSprite = "ui_main_98";
					m_CaptainTrail.GetComponent<UIImageButton>().hoverSprite = "ui_main_98";
					m_CaptainTrail.GetComponent<UIImageButton>().pressedSprite = "ui_main_98";
					m_CaptainTrail.GetComponent<UIImageButton>().disabledSprite = "ui_main_98";
					m_CaptainTrail.GetComponent<UIImageButton>().target.MakePixelPerfect();
				}
				else
				{
					m_CaptainTrail.GetComponent<UIImageButton>().normalSprite = "ui_main_97";
					m_CaptainTrail.GetComponent<UIImageButton>().hoverSprite = "ui_main_97";
					m_CaptainTrail.GetComponent<UIImageButton>().pressedSprite = "ui_main_97";
					m_CaptainTrail.GetComponent<UIImageButton>().disabledSprite = "ui_main_97";
					m_CaptainTrail.GetComponent<UIImageButton>().target.MakePixelPerfect();
				}
            }
            else
            {
                m_Captain.gameObject.SetActive(false);
				m_CaptainTrail.SetActive(false);
            }
        }
    }

	void OnTrailClick()
	{
		if(Singleton<ObjManager>.GetInstance().MainPlayer.IsTeamLeader(m_MemberId))
		{
			if(Singleton<ObjManager>.GetInstance().MainPlayer.IsTeamFollowState())
			{
				ChangeTrailSprite();
				Singleton<ObjManager>.GetInstance().MainPlayer.LeaveTeamFollow();
			}
			else
			{
				if(Singleton<ObjManager>.GetInstance().MainPlayer.MemberInSence(m_MemberId))
				{
					ChangeTrailSprite();
					Singleton<ObjManager>.GetInstance().MainPlayer.EnterTeamFollow();
				}
			}
		}
	}

	void ChangeTrailSprite()
	{
		if (Singleton<ObjManager>.GetInstance ().MainPlayer.IsTeamFollowState ()) 
		{
			m_CaptainTrail.GetComponent<UIImageButton> ().normalSprite = "ui_main_97";
			m_CaptainTrail.GetComponent<UIImageButton> ().hoverSprite = "ui_main_97";
			m_CaptainTrail.GetComponent<UIImageButton> ().pressedSprite = "ui_main_97";
			m_CaptainTrail.GetComponent<UIImageButton> ().disabledSprite = "ui_main_97";
			m_CaptainTrail.GetComponent<UIImageButton> ().target.spriteName = "ui_main_97";
			m_CaptainTrail.GetComponent<UIImageButton> ().target.MakePixelPerfect ();			
		} 
		else 
		{
			m_CaptainTrail.GetComponent<UIImageButton>().normalSprite = "ui_main_98";
			m_CaptainTrail.GetComponent<UIImageButton>().hoverSprite = "ui_main_98";
			m_CaptainTrail.GetComponent<UIImageButton>().pressedSprite = "ui_main_98";
			m_CaptainTrail.GetComponent<UIImageButton>().disabledSprite = "ui_main_98";
			m_CaptainTrail.GetComponent<UIImageButton>().target.spriteName = "ui_main_98";
			m_CaptainTrail.GetComponent<UIImageButton>().target.MakePixelPerfect();
		}			
	}
}