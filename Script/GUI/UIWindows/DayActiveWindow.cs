using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SPacket.SocketInstance;
using GCGame.Table;
using Module.Log;
public class DayActiveWindow : MonoBehaviour
{

    public GameObject ItemParent;

    void Start()
    {
        InitDayActiveList();
        UpdateDayActiveInfo();
    }

    void OnEnable()
    {
        UpdateDayActiveInfo();        
    }

    void InitDayActiveList()
    {
       
    }

    void UpdateDayActiveInfo()
    {
       
    }

    void OnGetRewardClick(GameObject item)
    {
        
    }

    void Ret_DatActivePointReward(List<int> items)
    {
        UpdateDayActiveInfo();
    }
}
