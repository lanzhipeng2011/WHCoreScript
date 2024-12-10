using UnityEngine;
using System.Collections;
using GCGame.Table;
using System.Collections.Generic;
using SPacket.SocketInstance;

public class LevelRewardWindow : MonoBehaviour {

    public GameObject ItemParent;

	// Use this for initialization
	void Start () 
    {
        InitLevelRewardInfo();
        UpdateLevelRewardInfo();
	}

    void InitLevelRewardInfo()
    {
        
    }

    void UpdateLevelRewardInfo()
    {
       
    }
    void OnGetRewardClick(GameObject item)
    {
      
    }   

    void Ret_LevelReward(List<int> items)
    {
        UpdateLevelRewardInfo();
    }

}
