using System;
using Games.GlobeDefine;
using UnityEngine;
using System.Collections;

public struct GuildActivityBossData
{
    public void SetBossData(int nSceneClassID, int nSceneInstanceID, float fPosX, float fPosZ)
    {
        m_nSceneClassID = nSceneClassID;
        m_nSceneInstanceID = nSceneInstanceID;
        m_fPosX = fPosX;
        m_fPosZ = fPosZ;
    }
    private int m_nSceneClassID;
    public int SceneClassID
    {
        get { return m_nSceneClassID; }
        set { m_nSceneClassID = value; }
    }
    private int m_nSceneInstanceID;
    public int SceneInstanceID
    {
        get { return m_nSceneInstanceID; }
        set { m_nSceneInstanceID = value; }
    }
    private float m_fPosX;
    public float PosX
    {
        get { return m_fPosX; }
        set { m_fPosX = value; }
    }
    private float m_fPosZ;
    public float PosZ
    {
        get { return m_fPosZ; }
        set { m_fPosZ = value; }
    }
}