using UnityEngine;
using System.Collections;
using GCGame.Table;
using Games.Fellow;

public class PartnerSkillItemLogic : MonoBehaviour
{
    public UILabel m_PartnerSkillLabel;
    public UISprite m_PartnerSkillSprite;
    public UISprite m_SkillUnActiveSprite;
    public UISprite m_ChooseSprite;
    public UISprite m_QualitySprite;
    public UILabel m_RemainTips;
    public UISprite m_EquipSprite;
    public UISprite m_TipsSprite;

    public int m_SkillId = -1;
    private bool m_isActive = false;
	

    public void UpdatePartnerSkill(int skillId, bool isActive, Fellow fellow)
    {
        m_EquipSprite.gameObject.SetActive(false);
        m_TipsSprite.gameObject.SetActive(false);

        Tab_SkillEx lineSkillEx = TableManager.GetSkillExByID(skillId, 0);
        if (lineSkillEx != null)
        {
            Tab_SkillBase lineSkillBase = TableManager.GetSkillBaseByID(lineSkillEx.BaseId, 0);
            if (lineSkillBase != null)
            {
                m_PartnerSkillSprite.spriteName = lineSkillBase.Icon;
            }
        }

        Tab_FellowSkill lineFellowSkill = TableManager.GetFellowSkillByID(skillId, 0);
        if (lineFellowSkill != null)
        {
            m_PartnerSkillLabel.text = "等级 " + lineFellowSkill.Level.ToString();
            m_QualitySprite.spriteName = FellowTool.GetFellowSkillQualityFrame(lineFellowSkill.Quality);
            if (isActive == false)
            {
                int needNum = lineFellowSkill.ActiveConsumeNum;
                int realNum = GameManager.gameManager.PlayerDataPool.BackPack.GetItemCountByDataId(lineFellowSkill.ActiveConsumeSubType);
                if (realNum >= needNum)
                {
                    m_TipsSprite.gameObject.SetActive(true);
                    m_TipsSprite.spriteName = "jihuo";
                }
            }
            else
            {
                if (fellow.IsHaveSkillId(skillId))
                {
                    m_EquipSprite.gameObject.SetActive(true);
                }
                else
                {
                    int needNum = lineFellowSkill.LevelupConsumeNum;
                    int realNum = GameManager.gameManager.PlayerDataPool.BackPack.GetItemCountByDataId(lineFellowSkill.LevelupConsumeSubType);
                    if (realNum >= needNum && needNum > 0)
                    {
                        m_TipsSprite.gameObject.SetActive(true);
                        m_TipsSprite.spriteName = "shengji";
                    }
                }
            }
        }

        if (isActive)
        {
            m_PartnerSkillLabel.gameObject.SetActive(true);
            m_SkillUnActiveSprite.gameObject.SetActive(false);
        }
        else
        {
            m_PartnerSkillLabel.gameObject.SetActive(false);
            m_SkillUnActiveSprite.gameObject.SetActive(true);
        }

        m_SkillId = skillId;
        m_isActive = isActive;
    }

    public void SetChoosed()
    {
        ClearAllChooseFlag();
        SetChooseFlag(true);
        if (PartnerFrameLogic_Skill.Instance() != null)
        {
            PartnerFrameLogic_Skill.Instance().OnClickPartnerSkill(m_SkillId, m_isActive, false);
        }
    }

    public void OnClickPartnerSkill()
    {
        ClearAllChooseFlag();
        SetChooseFlag(true);
        if (PartnerFrameLogic_Skill.Instance() != null)
        {
            PartnerFrameLogic_Skill.Instance().OnClickPartnerSkill(m_SkillId, m_isActive, true);
        }
    }

    public void ClearAllChooseFlag()
    {
        if (transform.parent == null)
        {
            return;
        }

        PartnerSkillItemLogic[] item = transform.parent.gameObject.GetComponentsInChildren<PartnerSkillItemLogic>();
        for (int i = 0; i < item.Length; ++i)
        {
            if (null != item[i])
            {
                item[i].SetChooseFlag(false);
            }
        }
    }

    public void SetChooseFlag(bool bChoose)
    {
        if (bChoose)
        {
            m_ChooseSprite.gameObject.SetActive(true);
        }
        else
        {
            m_ChooseSprite.gameObject.SetActive(false);
        }
    }
}
