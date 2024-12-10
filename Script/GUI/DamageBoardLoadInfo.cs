//********************************************************************
// 文件名: DamageBoardLoadInfo.cs
// 描述: 伤害板创建参数
// 作者: Lijia
// 创建时间: 2014-05-21
//
// 修改历史:
// 2014-05-21 李嘉创建
//********************************************************************
using UnityEngine;
using System.Collections;

//伤害板创建参数
public class DamageBoardLoadInfo
{
    private DamageBoardLoadInfo()
    {

    }

    public DamageBoardLoadInfo(int nType, string szValue, Vector3 pos, bool isProfessionSkill)
    {
        m_nType = nType;
        m_szValue = szValue;
        m_pos = pos;
        m_IsProfessionSkill = isProfessionSkill;
    }

    private int m_nType;            //伤害板类型
    public int Type
    {
        get { return m_nType; }
    }
    private string m_szValue;       //伤害板内容
    public string Value
    {
        get { return m_szValue; }
    }
    private Vector3 m_pos;          //伤害板位置
    public UnityEngine.Vector3 Pos
    {
        get { return m_pos; }
    }

    private bool m_IsProfessionSkill;    //是否玩家技能
    public bool IsProfessionSkill
    {
        get { return m_IsProfessionSkill; }
    }

}
