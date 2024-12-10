using UnityEngine;
using System.Collections;
using GCGame.Table;
using Games.GlobeDefine;
using System;

public class MasterMemberItemLogic : MonoBehaviour {

    public UILabel m_NameLabel;                     //名字
    public UILabel m_GuildNameLabel;                //帮会名
    public UILabel m_LevelLabel;                    //级别
    public UILabel m_ProfessionLabel;               //职业
    public UILabel m_CombatValueLabel;              //战斗力
    public UILabel m_OffLineTimeLable;              //离线时间
    public UISprite m_HighLightSprit;               //选中高亮

    private MasterMember m_MasterMember;

    public void Init(MasterMember member)
    {
        if (member.IsValid() == false)
        {
            return;
        }
        //显示正式成员
        if (member.IsReserveMember())
        {
            return;
        }

        m_MasterMember = member;

        if (m_NameLabel != null)
        {
            m_NameLabel.text = member.MemberName;
        }
        if (m_GuildNameLabel != null)
        {
            if (member.GuildName != null && member.GuildName != "")
            {
                m_GuildNameLabel.text = member.GuildName;
            }
            else
            {
                //m_GuildNameLabel.text = "无";
                m_GuildNameLabel.text = StrDictionary.GetClientDictionaryString("#{2865}");
            }
        }
        if (m_LevelLabel != null)
        {
            m_LevelLabel.text = member.Level.ToString();
        }
        if (m_ProfessionLabel != null)
        {
            m_ProfessionLabel.text = StrDictionary.GetClientDictionaryString("#{" + CharacterDefine.PROFESSION_DICNUM[member.Profession].ToString() + "}");
        }
        if (m_CombatValueLabel != null)
        {
            m_CombatValueLabel.text = member.CombatValue.ToString();
        }
        if (m_OffLineTimeLable != null)
        {
            //member.LastLogin为1970年1月1日至今的标准时间，需要转换为时分秒
            if (member.State == 1)
            {
                //在线，显示在线
                //m_OffLineTimeLable.text = "在线";
                m_OffLineTimeLable.text = StrDictionary.GetClientDictionaryString("#{2866}");
            }
            else
            {
                DateTime startTime = new DateTime(1970, 1, 1);
                DateTime sendDate = new DateTime((long)member.LastLogin * 10000000L + startTime.Ticks, DateTimeKind.Unspecified);
                sendDate = sendDate.ToLocalTime();
                if (DateTime.Now.Subtract(sendDate).Days < 1)
                {
                    m_OffLineTimeLable.text = sendDate.ToString("HH:mm");
                }
                else
                {
                    m_OffLineTimeLable.text = sendDate.ToString("MM-dd");
                }
            } 
        }
    }

    void OnClickMemberItem()
    {
        if (GUIData.delMasterMemberSelectChange != null)
        {
            GUIData.delMasterMemberSelectChange(m_MasterMember.Guid, m_MasterMember.MemberName);
        }
    }

    public void SetHighLight(UInt64 guid)
    {
        if (guid == m_MasterMember.Guid)
        {
            m_HighLightSprit.gameObject.SetActive(true);
        }
        else
        {
            m_HighLightSprit.gameObject.SetActive(false);
        }
    }
}
