/********************************************************************
	filename:	MasterPreviewItemLogic.cs
	date:		2014-5-9  14-34
	author:		tangyi
	purpose:	师门预览界面Item逻辑
*********************************************************************/
using UnityEngine;
using System.Collections;
using System;
using Games.GlobeDefine;
using GCGame.Table;

public class MasterPreviewItemLogic : MonoBehaviour
{
    //师门列表项控件
    public UILabel m_MasterIndexLable;        //师门在列表中的索引
    public UILabel m_MasterNameLabel;         //师门名称
    public UILabel m_MasterChiefNameLabel;    //帮主姓名
    public UILabel m_MasterTorchLabel;        //师门薪火
    public UILabel m_MasterMemberNumLabel;    //师门成员数量
    public UISprite[] m_MasterSkillSprite;    //师门技能图标
    public UISprite m_HighLightSprit;         //选中高亮

    private UInt64 m_MasterGuid;         //师门GUID

    //根据数据初始化控件
    public void Init(MasterPreviewInfo info, int nIndex)
    {
        //判断数据合法性
        if (info.MasterGuid == GlobeVar.INVALID_GUID)
        {
            return;
        }

        m_MasterGuid = info.MasterGuid;

        //填充数据
        if (null != m_MasterIndexLable)
        {
            m_MasterIndexLable.text = nIndex.ToString();
        }
        if (null != m_MasterNameLabel)
        {
            m_MasterNameLabel.text = info.MasterName;
        }
        if (null != m_MasterChiefNameLabel)
        {
            m_MasterChiefNameLabel.text = info.MasterChiefName;
        }
        if (null != m_MasterTorchLabel)
        {
            m_MasterTorchLabel.text = info.MasterTorch.ToString();
        }
        if (null != m_MasterMemberNumLabel)
        {
            m_MasterMemberNumLabel.text = info.MasterCurMemberNum.ToString();
        }
        int SkillIndex = 0;
        if (info.MasterSkillId1 >= 0)
        {
            Tab_SkillEx lineSkillEx = TableManager.GetSkillExByID(info.MasterSkillId1, 0);
            if (lineSkillEx != null)
            {
                Tab_SkillBase lineSkillBase = TableManager.GetSkillBaseByID(lineSkillEx.BaseId, 0);
                if (lineSkillBase != null && 
                    SkillIndex >= 0 && 
                    SkillIndex < m_MasterSkillSprite.Length && 
                    null != m_MasterSkillSprite[SkillIndex])
                {
                    m_MasterSkillSprite[SkillIndex].gameObject.SetActive(true);
                    m_MasterSkillSprite[SkillIndex].spriteName = lineSkillBase.Icon;
                    SkillIndex = SkillIndex + 1;
                }
            }
        }
        if (info.MasterSkillId2 >= 0)
        {
            Tab_SkillEx lineSkillEx = TableManager.GetSkillExByID(info.MasterSkillId2, 0);
            if (lineSkillEx != null)
            {
                Tab_SkillBase lineSkillBase = TableManager.GetSkillBaseByID(lineSkillEx.BaseId, 0);
                if (lineSkillBase != null &&
                    SkillIndex >= 0 &&
                    SkillIndex < m_MasterSkillSprite.Length &&
                    null != m_MasterSkillSprite[SkillIndex])
                {
                    m_MasterSkillSprite[SkillIndex].gameObject.SetActive(true);
                    m_MasterSkillSprite[SkillIndex].spriteName = lineSkillBase.Icon;
                    SkillIndex = SkillIndex + 1;
                }
            }
        }
        if (info.MasterSkillId3 >= 0)
        {
            Tab_SkillEx lineSkillEx = TableManager.GetSkillExByID(info.MasterSkillId3, 0);
            if (lineSkillEx != null)
            {
                Tab_SkillBase lineSkillBase = TableManager.GetSkillBaseByID(lineSkillEx.BaseId, 0);
                if (lineSkillBase != null &&
                    SkillIndex >= 0 &&
                    SkillIndex < m_MasterSkillSprite.Length &&
                    null != m_MasterSkillSprite[SkillIndex])
                {
                    m_MasterSkillSprite[SkillIndex].gameObject.SetActive(true);
                    m_MasterSkillSprite[SkillIndex].spriteName = lineSkillBase.Icon;
                    SkillIndex = SkillIndex + 1;
                }
            }
        }
		if (info.MasterSkillId4 >= 0)
		{
			Tab_SkillEx lineSkillEx = TableManager.GetSkillExByID(info.MasterSkillId4, 0);
			if (lineSkillEx != null)
			{
				Tab_SkillBase lineSkillBase = TableManager.GetSkillBaseByID(lineSkillEx.BaseId, 0);
				if (lineSkillBase != null &&
				    SkillIndex >= 0 &&
				    SkillIndex < m_MasterSkillSprite.Length &&
				    null != m_MasterSkillSprite[SkillIndex])
				{
					m_MasterSkillSprite[SkillIndex].gameObject.SetActive(true);
					m_MasterSkillSprite[SkillIndex].spriteName = lineSkillBase.Icon;
					SkillIndex = SkillIndex + 1;
				}
			}
		}
        for (; SkillIndex < 5; SkillIndex++)
        {
            if (SkillIndex >= 0 &&
                SkillIndex < m_MasterSkillSprite.Length &&
                null != m_MasterSkillSprite[SkillIndex])
            m_MasterSkillSprite[SkillIndex].gameObject.SetActive(false);
        }
    }

    void OnClickMaster()
    {
        if (GUIData.delMasterSelectChange != null)
        {
            GUIData.delMasterSelectChange(m_MasterGuid);
        }
    }

    public void SetHighLight(UInt64 guid)
    {
        if (guid == m_MasterGuid)
        {
            m_HighLightSprit.gameObject.SetActive(true);
        }
        else
        {
            m_HighLightSprit.gameObject.SetActive(false);
        }
    }
}
