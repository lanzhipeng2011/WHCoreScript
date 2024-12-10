using UnityEngine;
using System.Collections;
using Games.Mission;

public class CGPlay : MonoBehaviour {
	// Use this for initialization
	void Start () {

	}

    /*
	void OnGUI()
	{
		bool bFlag = GameManager.gameManager.MissionManager.GetMissionFlag((int)MISSION_FLAG.MF_CGVIDEO);
        if (false == bFlag)
        {
            Handheld.PlayFullScreenMovie("mengjiangCG.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput);
			GameManager.gameManager.MissionManager.SetMissionFlag((int)MISSION_FLAG.MF_CGVIDEO, 1);
        }			
	}
     * */
}
