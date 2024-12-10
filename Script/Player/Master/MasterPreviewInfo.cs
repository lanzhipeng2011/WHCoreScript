/********************************************************************
	filename:	MasterPreviewInfo.cs
	date:		2014-5-7  17-07
	author:		tangyi
	purpose:	师门预览数据结构
*********************************************************************/
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Games.GlobeDefine;

public class MasterPreviewInfo
{
    public bool IsValid()
    {
        return (m_MasterGuid != GlobeVar.INVALID_GUID);
    }

    //师门GUID
    private UInt64 m_MasterGuid;                     
    public System.UInt64 MasterGuid
    {
        get { return m_MasterGuid; }
        set { m_MasterGuid = value; }
    }
    //师门名称
    private string m_MasterName;                     
    public string MasterName
    {
        get { return m_MasterName; }
        set { m_MasterName = value; }
    }
    //师门薪火
    private int m_MasterTorch;
    public int MasterTorch
    {
        get { return m_MasterTorch; }
        set { m_MasterTorch = value; }
    }
    //掌门姓名
    private string m_MasterChiefName;
    public string MasterChiefName
    {
        get { return m_MasterChiefName; }
        set { m_MasterChiefName = value; }
    }
    //师门当前人数
    private int m_MasterCurMemberNum;
    public int MasterCurMemberNum
    {
        get { return m_MasterCurMemberNum; }
        set { m_MasterCurMemberNum = value; }
    }
    //创建时间
    private int m_MasterCreateTime;
    public int MasterCreateTime
    {
        get { return m_MasterCreateTime; }
        set { m_MasterCreateTime = value; }
    }
    //师门技能ID
    private int m_MasterSkillId1;
    public int MasterSkillId1
    {
        get { return m_MasterSkillId1; }
        set { m_MasterSkillId1 = value; }
    }
    private int m_MasterSkillId2;
    public int MasterSkillId2
    {
        get { return m_MasterSkillId2; }
        set { m_MasterSkillId2 = value; }
    }
    private int m_MasterSkillId3;
    public int MasterSkillId3
    {
        get { return m_MasterSkillId3; }
        set { m_MasterSkillId3 = value; }
    }
    private int m_MasterSkillId4;
    public int MasterSkillId4
    {
        get { return m_MasterSkillId4; }
        set { m_MasterSkillId4 = value; }
    }
}
