/********************************************************************************
 *	文件名：	OtherPlayerData.cs
 *	全路径：	\Script\Player\UserData\OtherPlayerData.cs
 *	创建人：    grx
 *	创建时间：  2013-06-05
 *
 *	功能说明:   其他玩家的数据
 *	修改记录：
*********************************************************************************/

using System;
using System.Collections.Generic;
using Games.GlobeDefine;
using Games.ImpactModle;
using Games.SkillModle;
using UnityEngine;
using System.Collections;
using Games.Item;
using Games.MountModule;
using Games.TitleInvestitive;
using Games.ChatHistory;
using Games.Fellow;
using Games.AwardActivity;
using Games.MoneyTree;
using Games.UserCommonData;
using Games.DailyMissionData;
using Games.DailyLuckyDraw;


public class OtherPlayerData
{
    public OtherPlayerData()
    {
        m_oEquipPack = new GameItemContainer(GameItemContainer.SIZE_EQUIPPACK, GameItemContainer.Type.TYPE_EQUIPPACK);
        //宝石
        m_GemData = new GemData();
        m_GemData.CleanUp();
    }

    public void CleanUpData()
    {
        m_GemData.CleanUp();
        for (int i = 0; i < GameItemContainer.SIZE_EQUIPPACK; i++)
        {
            GameItem item = m_oEquipPack.GetItem(i);
            if (item != null )
            {
                item.CleanUp();
            }
        }
    }

    private string m_strName = "";
    public string Name
    {
        get { return m_strName; }
        set { m_strName = value; }
    }
    private int m_nCombatValue = 0;
    public int CombatValue
    {
        get { return m_nCombatValue; }
        set { m_nCombatValue = value; }
    }
    private int m_nLevel = 0;
    public int Level
    {
        get { return m_nLevel; }
        set { m_nLevel = value; }
    }
    private int m_Profession = -1;
    public int Profession
    {
        get { return m_Profession; }
        set { m_Profession = value; }
    }

    private UInt64 m_RoleGUID = 0;
    public System.UInt64 RoleGUID
    {
        get { return m_RoleGUID; }
        set { m_RoleGUID = value; }
    }
    private int m_nCurHp = 0;
    public int CurHp
    {
        get { return m_nCurHp; }
        set { m_nCurHp = value; }
    }
    private int m_nMaxHP = 0;
    public int MaxHP
    {
        get { return m_nMaxHP; }
        set { m_nMaxHP = value; }
    }
    private int m_nCurMp = 0;
    public int CurMp
    {
        get { return m_nCurMp; }
        set { m_nCurMp = value; }
    }

    private int m_nMaxMp = 0;
    public int MaxMp
    {
        get { return m_nMaxMp; }
        set { m_nMaxMp = value; }
    }
    private int m_nCurExp = 0;
    public int CurExp
    {
        get { return m_nCurExp; }
        set { m_nCurExp = value; }
    }
    private int m_nMaxExp = 0;
    public int MaxExp
    {
        get { return m_nMaxExp; }
        set { m_nMaxExp = value; }
    }

    private int m_nPAttck = 0;//物攻
    public int PAttck
    {
        get { return m_nPAttck; }
        set { m_nPAttck = value; }
    }
    private int m_nMAttack = 0;//法功
    public int MAttack
    {
        get { return m_nMAttack; }
        set { m_nMAttack = value; }
    }
    private int m_nHit = 0;
    public int Hit
    {
        get { return m_nHit; }
        set { m_nHit = value; }
    }
    private int m_nCritical = 0;//暴击
    public int Critical
    {
        get { return m_nCritical; }
        set { m_nCritical = value; }
    }
    private int m_nPDefense = 0;//物防
    public int PDefense
    {
        get { return m_nPDefense; }
        set { m_nPDefense = value; }
    }
    private int m_MDefense = 0;//法防
    public int MDefense
    {
        get { return m_MDefense; }
        set { m_MDefense = value; }
    }
    private int m_DeCritical = 0;//暴击抗性
    public int DeCritical
    {
        get { return m_DeCritical; }
        set { m_DeCritical = value; }
    }
    private int m_nDoge = 0;//闪避
    public int Doge
    {
        get { return m_nDoge; }
        set { m_nDoge = value; }
    }
    private int m_nStrike = 0;//穿透
    public int Strike
    {
        get { return m_nStrike; }
        set { m_nStrike = value; }
    }
    private int m_nCriticalAdd = 0;//暴击加成
    public int CriticalAdd
    {
        get { return m_nCriticalAdd; }
        set { m_nCriticalAdd = value; }
    }
    private int m_nDuctical = 0;//韧性
    public int Ductical
    {
        get { return m_nDuctical; }
        set { m_nDuctical = value; }
    }
    private int m_nCriticalMis = 0;//暴击减免
    public int CriticalMis
    {
        get { return m_nCriticalMis; }
        set { m_nCriticalMis = value; }
    }
    private int m_nModuleVisualID = 0;//模型ID
    public int ModuleVisualID
    {
        get { return m_nModuleVisualID; }
        set { m_nModuleVisualID = value; }
    }
    private int m_CurWeaponDataID = 0;
    public int CurWeaponDataID
    {
        get { return m_CurWeaponDataID; }
        set { m_CurWeaponDataID = value; }
    }
    private int m_WeaponEffectGem = 0;
    public int WeaponEffectGem
    {
        get { return m_WeaponEffectGem; }
        set { m_WeaponEffectGem = value; }
    }
    //////////////////////////////////////////////////////////////////////////
    //装备槽位
    //////////////////////////////////////////////////////////////////////////
    private GameItemContainer m_oEquipPack;
    public GameItemContainer EquipPack
    {
        get { return m_oEquipPack; }
        set { m_oEquipPack = value; }
    }

    //宝石信息
    private GemData m_GemData;
    public GemData GemData
    {
        get { return m_GemData; }
        set { m_GemData = value; }
    }

}
