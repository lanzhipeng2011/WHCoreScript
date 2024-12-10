/********************************************************************************
 *	文件名：	GuildMemberListItemLogic.cs
 *	全路径：	\Script\GUI\GuildMemberListItemLogic.cs
 *	创建人：	李嘉
 *	创建时间：2014-04-24
 *
 *	功能说明：帮会成员列表项
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using System;
using Games.GlobeDefine;
using GCGame.Table;
using GCGame;

public class GuildMemberListItemLogic : MonoBehaviour
{
    public GameObject m_MemberWidgeRoot;        //帮会正式成员控件根节点
    public GameObject m_ReserveMemberWidgeRoot; //待审批成员控件根节点

    //帮会正式成员控件
    public UILabel m_MemberNameLable;           //帮会成员姓名
    public UILabel m_MemberLevelLable;          //帮会成员等级
    public UILabel m_MemberProfLable;           //帮会成员职业
    public UILabel m_MemberStateLable;          //帮会成员状态(目前为最后登录时间)
    public UILabel m_MemberJobLable;            //帮会成员职位
    public UILabel m_MemberContriLable;         //帮会成员贡献度
    public UISprite m_VipSprite;                //vip等级显示
	public UISprite m_HightSprite;

    //帮会预备役成员控件
    public UILabel m_ReserveMemberNameLable;    //帮会预备成员名字
    public UILabel m_ReserveMemberLevelLable;   //帮会预备成员等级
    public UILabel m_ReserveMemberProfLable;    //帮会预备成员职业
    public UILabel m_ReserveMemberCombatvalLable;    //帮会预备成元战力

    private UInt64 m_MemberGuid;                //成员GUID
	public UInt64 MemberGuid
	{
		get
		{
			return m_MemberGuid;
		}
	}
    //private int m_MemberJob;                    //成员Job ID
    private bool m_bIsReserveMember;

    public void Init(GuildMember member, bool bIsReserveMember)
    {
        if (member.Guid == GlobeVar.INVALID_GUID)
        {
            return;
        }
        m_MemberGuid = member.Guid;
        m_bIsReserveMember = bIsReserveMember;

        if (bIsReserveMember)
        {
            m_MemberWidgeRoot.SetActive(false);
            m_ReserveMemberWidgeRoot.SetActive(true);

            m_ReserveMemberNameLable.text = member.MemberName;
            m_ReserveMemberLevelLable.text = member.Level.ToString();


            // 玩家职业
            m_ReserveMemberProfLable.text = StrDictionary.GetClientDictionaryString("#{" + CharacterDefine.PROFESSION_DICNUM[member.Profession].ToString() + "}");
            m_ReserveMemberCombatvalLable.text = member.ComBatVal.ToString();
        }
        else
        {
            m_MemberWidgeRoot.SetActive(true);
            m_ReserveMemberWidgeRoot.SetActive(false);

            m_MemberNameLable.text = member.MemberName;
            m_MemberLevelLable.text = member.Level.ToString();
			m_HightSprite.gameObject.SetActive(false);

            //判断职业是否合法
            if (member.Profession < 0 || member.Profession >= (int)CharacterDefine.PROFESSION.MAX)
            {
                //非法职业ID，则强制为0
                member.Profession = 0;
            }

            // 玩家职业
            m_MemberProfLable.text = StrDictionary.GetClientDictionaryString("#{" + CharacterDefine.PROFESSION_DICNUM[member.Profession].ToString() + "}");

            //最后登录时间,如果在线则显示在线
            //member.LastLogin为1970年1月1日至今的标准时间，需要转换为时分秒
            if (member.State == 1)
            {
                //在线，显示在线
                //m_MemberStateLable.text = "在线";
                m_MemberStateLable.text = StrDictionary.GetClientDictionaryString("#{2866}");
            }
            else
            {
                DateTime startTime = new DateTime(1970, 1, 1);
                DateTime sendDate = new DateTime((long)member.LastLogin*10000000L + startTime.Ticks, DateTimeKind.Unspecified);
                sendDate = sendDate.ToLocalTime();

                if (DateTime.Now.Subtract(sendDate).Days < 1)
                {
                    m_MemberStateLable.text = sendDate.ToString("HH:mm");
                }
                else
                {
                    m_MemberStateLable.text = sendDate.ToString("MM-dd");
                }
            }

            //帮贡
            m_MemberContriLable.text = member.Contribute.ToString();

            //职位
            if (member.Job < 0 || member.Job >= (int)GameDefine_Globe.GUILD_JOB.MAX)
            {
                member.Job = (int)GameDefine_Globe.GUILD_JOB.MEMBER;
            }
            //m_MemberJob = member.Job;
            m_MemberJobLable.text = GlobeVar.GUILD_JOB_NAME[member.Job];
            if (member.VIP > 0)
            {
                m_VipSprite.spriteName = "v" + member.VIP.ToString();
                m_VipSprite.gameObject.SetActive(true);
            }
            else
            {
                m_VipSprite.gameObject.SetActive(false);
            }
        }
    }

    void GuildListItemSelect()
    {
        //更新选择变化界面更新（如果未打开，则不处理）
        if (null != GUIData.delGuildMemberSelectChange)
        {
            GUIData.delGuildMemberSelectChange(m_MemberGuid);

            //如果点击的不是自己，则弹出菜单
            if (m_MemberGuid != Singleton<ObjManager>.Instance.MainPlayer.GUID && !m_bIsReserveMember)
            {
                PopMenuLogic.ShowMenu("GuildMemberPopMenu", this.gameObject);
            }
        }
    }

    //预备役会员审批通过
    public void GuildMemberAgreeReserve()
    {
        Singleton<ObjManager>.GetInstance().MainPlayer.ReqApproveGuildMember(m_MemberGuid, 1);
    }

    //预备役会员审批未通过
    public void GuildMemberDisagreeReserve()
    {
        Singleton<ObjManager>.GetInstance().MainPlayer.ReqApproveGuildMember(m_MemberGuid, 0);
    }
}
