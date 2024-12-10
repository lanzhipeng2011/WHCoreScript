using UnityEngine;
using System.Collections;
using GCGame.Table;
using Games.GlobeDefine;
using System;

public class MasterReserveMemberItemLogic : MonoBehaviour {

    public UILabel m_NameLabel;                     //名字
    public UILabel m_GuildNameLabel;                //帮会名
    public UILabel m_LevelLabel;                    //级别
    public UILabel m_ProfessionLabel;               //职业
    public UILabel m_CombatValueLabel;              //战斗力
    public UISprite m_HighLightSprit;               //选中高亮

    private MasterMember m_MasterMember;

    public void Init(MasterMember member)
    {
        if (member.IsValid() == false)
        {
            return;
        }
        //显示待审批成员
        if (member.IsReserveMember() == false)
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
            if (member.GuildName != "" || member.GuildName != string.Empty)
            {
                m_GuildNameLabel.text = member.GuildName;
            }
            else
            {
                m_GuildNameLabel.text = m_GuildNameLabel.text = StrDictionary.GetClientDictionaryString("#{2865}");
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
    }

    void OnClickMemberItem()
    {
        if (GUIData.delMasterReserveMemberSelectChange != null)
        {
            GUIData.delMasterReserveMemberSelectChange(m_MasterMember.Guid, m_MasterMember.MemberName);
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

    public UInt64 GetGuid()
    {
        return m_MasterMember.Guid;
    }
}
