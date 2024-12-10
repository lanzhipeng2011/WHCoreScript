using UnityEngine;
using System.Collections;
using GCGame;
using System.Collections.Generic;
using Games.UserCommonData;
public class PVPWindow : MonoBehaviour
{
	public UILabel LabelRange;
	public UILabel LabelFightTime;
	public UILabel LabelPower;
    public UILabel LabelCom;

    void OnEnable()
    {
        UpdatePVPInfo();
        PVPData.delUpdateMyData += UpdatePVPInfo;
    }

    void OnDisable()
    {
        PVPData.delUpdateMyData -= UpdatePVPInfo;
    }

	void UpdatePVPInfo()
	{
		if (PVPData.MyPVPRange != -1 && PVPData.MyPVPRange <= 2000) 
			LabelRange.text = PVPData.MyPVPRange.ToString ();
		else 
			LabelRange.text = "2000+";

//        if (GameManager.gameManager.PlayerDataPool.CommonData.CopySceneDayNumbers.ContainsKey(17))
//        {
//            LabelFightTime.text = GameManager.gameManager.PlayerDataPool.CommonData.CopySceneDayNumbers[17].m_nDayCount.ToString();
//        }
//        else 
//        {
//            LabelFightTime.text = "0";
//        }
		LabelFightTime.text = PVPData.LeftFightTime.ToString ();
		LabelPower.text 	= PVPData.Power.ToString();

        if (Singleton<ObjManager>.GetInstance().MainPlayer != null)
        {
            LabelCom.text = Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.CombatValue.ToString();
        }
       
	}
}
