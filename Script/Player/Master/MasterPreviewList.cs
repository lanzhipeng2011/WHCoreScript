/********************************************************************
	filename:	MasterPreviewList.cs
	date:		2014-5-7  17-14
	author:		tangyi
	purpose:	全服师门预览列表
*********************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Games.GlobeDefine;

public class MasterPreviewList
{
    public MasterPreviewList()
    {
        CleanUp();
    }

    public void CleanUp()
    {
        if (null == m_MasterInfoList)
        {
            m_MasterInfoList = new List<MasterPreviewInfo>();
        }
        m_MasterInfoList.Clear();
    }

    private List<MasterPreviewInfo> m_MasterInfoList;
    public List<MasterPreviewInfo> MasterInfoList
    {
        get { return m_MasterInfoList; }
        set { m_MasterInfoList = value; }
    }

    public void UpdateData(GC_MASTER_RET_LIST list)
    {
        m_MasterInfoList.Clear();
        for (int i = 0; i < list.masterGuidCount; ++i)
        {
            MasterPreviewInfo info = new MasterPreviewInfo();
            info.MasterGuid = list.GetMasterGuid(i);
            if (info.MasterGuid == GlobeVar.INVALID_GUID)
            {
                continue;
            }

            if (list.masterNameCount > i)
            {
                info.MasterName = list.GetMasterName(i);
            }

            if (list.masterTorchCount > i)
            {
                info.MasterTorch = list.GetMasterTorch(i);
            }

            if (list.masterChiefNameCount > i)
            {
                info.MasterChiefName = list.GetMasterChiefName(i);
            }

            if (list.memberNumCount > i)
            {
                info.MasterCurMemberNum = list.GetMemberNum(i);
            }

            if (list.createTimeCount > i)
            {
                info.MasterCreateTime = list.GetCreateTime(i);
            }

            if (list.skillID1Count > i)
            {
                info.MasterSkillId1 = list.GetSkillID1(i);
            }
            if (list.skillID2Count > i)
            {
                info.MasterSkillId2 = list.GetSkillID2(i);
            }
            if (list.skillID3Count > i)
            {
                info.MasterSkillId3 = list.GetSkillID3(i);
            }
            if (list.skillID4Count > i)
            {
                info.MasterSkillId4 = list.GetSkillID4(i);
            }

            //合法则添加
            if (info.IsValid())
            {
                m_MasterInfoList.Add(info);
            }
        }
    }
}
