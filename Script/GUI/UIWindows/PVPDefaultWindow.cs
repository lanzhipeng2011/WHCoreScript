using UnityEngine;
using System.Collections;

public class PVPDefaultWindow : MonoBehaviour {

	public TabController PVPTabController;

    void OnEnable( )
    {
        PVPTabController.ChangeTab("Tab1");
    }

	void OnFightClick()
	{
        // 新手指引
        if (ActivityController.Instance())
        {
            if (1 == ActivityController.Instance().NewPlayerGuide_Step)
            {
                ActivityController.Instance().NewPlayerGuide_Step = 2;
                NewPlayerGuidLogic.CloseWindow();
            }
        }	
	}
    
}
