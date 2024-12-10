using UnityEngine;
using System.Collections;
using System;
using Games.GlobeDefine;
public class TeamPlatformItem : MonoBehaviour {

    private UInt64 playerGUID = GlobeVar.INVALID_GUID;
    private int teamID = -1;
    public UILabel m_playerName;
    public UILabel[] m_playerLevel;
    public UILabel[] m_playerProf;
    public UILabel[] m_playerCombat;

//     public UILabel m_memberLevel1;
//     public UILabel m_memberProf1;
//     public UILabel m_memberCombat1;
// 
//     public UILabel m_memberLevel2;
//     public UILabel m_memberProf2;
//     public UILabel m_memberCombat2;
// 
//     public UILabel m_memberLevel3;
//     public UILabel m_memberProf3;
//     public UILabel m_memberCombat3;

    public UISprite m_playerHead;
	// Use this for initialization
	void Start () {
	
	}
	
    public void OnItemClick()
    {
        TeamPlatformWindow.Instance().playerGUID = playerGUID;
        TeamPlatformWindow.Instance().teamID = teamID;
    }
    public void UpdateData(UInt64 _playerGUID, int _teamID, int _prof)
    {       
        playerGUID = _playerGUID;
        teamID = _teamID;
    }
}
