/********************************************************************************
 *	文件名：	PlayerEnterSceneCache.cs
 *	全路径：	\Script\Player\UserData\PlayerEnterSceneCache.cs
 *	创建人：	李嘉
 *	创建时间：2013-12-30
 *
 *	功能说明：主角游戏全程需要保留数据_玩家登陆场景部分
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using System;
using Games.GlobeDefine;

public class PlayerEnterSceneCache
{
    public PlayerEnterSceneCache()
    {
        ClearEnterSceneInfo();
    }

    //进场景的位置，在收到EnterScene之后记录
    private Vector3 m_EnterScenePos;
    public UnityEngine.Vector3 EnterScenePos
    {
        get { return m_EnterScenePos; }
        set { m_EnterScenePos = value; }
    }

    //进场景后的ServerID
    private int m_nEnterSceneServerID;
    public int EnterSceneServerID
    {
        get { return m_nEnterSceneServerID; }
        set { m_nEnterSceneServerID = value; }
    }

    //进场景后的SceneID
    private int m_nEnterSceneSceneID;
    public int EnterSceneSceneID
    {
        get { return m_nEnterSceneSceneID; }
        set { m_nEnterSceneSceneID = value; }
    }

    //创建MainPlayer的RoleBaseID
    private int m_nEnterSceneRoleBaseID;
    public int EnterSceneRoleBaseID
    {
        get { return m_nEnterSceneRoleBaseID; }
        set { m_nEnterSceneRoleBaseID = value; }
    }

    //创建MainPlayer的CharModelID
    private int m_nEnterSceneCharModelID;
    public int EnterSceneCharModelID
    {
        get { return m_nEnterSceneCharModelID; }
        set { m_nEnterSceneCharModelID = value; }
    }

    //创建MainPlayer的职业ID
    private int m_nProfession;
    public int Profession
    {
        get { return m_nProfession; }
        set { m_nProfession = value; }
    }

    //创建MainPlayer的GUID
    private UInt64 m_Guid;
    public System.UInt64 Guid
    {
        get { return m_Guid; }
        set { m_Guid = value; }
    }

    //创建MainPlayer的职业ID
    private int m_EnterSceneMountID;
    public int EnterSceneMountID
    {
        get { return m_EnterSceneMountID; }
        set { m_EnterSceneMountID = value; }
    }

    //创建MainPlayer的跑商状态
    private int m_GuildBusinessState;
    public int GuildBusinessState
    {
        get { return m_GuildBusinessState; }
        set { m_GuildBusinessState = value; }
    }

    //////////////////////////////////////////////////////////////////////////
    //纸娃娃
    //////////////////////////////////////////////////////////////////////////
    private int m_WeaponDataID;
    public int WeaponDataID
    {
        get { return m_WeaponDataID; }
        set { m_WeaponDataID = value; }
    }

    private int m_ModelVisualID;
    public int ModelVisualID
    {
        get { return m_ModelVisualID; }
        set { m_ModelVisualID = value; }
    }

    private int m_WeaponEffectGem;
    public int WeaponEffectGem
    {
        get { return m_WeaponEffectGem; }
        set { m_WeaponEffectGem = value; }
    }
   
    //清空EnterSceneInfo
    public void ClearEnterSceneInfo()
    {
        m_EnterScenePos = Vector3.zero;
        m_nEnterSceneServerID = GlobeVar.INVALID_ID;
        m_nEnterSceneSceneID = GlobeVar.INVALID_ID;
        m_nEnterSceneRoleBaseID = GlobeVar.INVALID_ID;
        m_Guid = GlobeVar.INVALID_GUID;
        m_EnterSceneMountID = GlobeVar.INVALID_ID;
        m_WeaponDataID = GlobeVar.INVALID_ID;
        m_ModelVisualID = GlobeVar.INVALID_ID;
        m_WeaponEffectGem = GlobeVar.INVALID_ID;
        m_GuildBusinessState = GlobeVar.INVALID_ID;
    }
}
