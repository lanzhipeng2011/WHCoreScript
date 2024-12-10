/********************************************************************
	created:	2014/02/17
	created:	17:2:2014   9:56
	filename    UIRootAdapter.cs
	author:		王迪
	
	purpose:	绑定在UIROOT上做UI适配
*********************************************************************/
using UnityEngine;
using System.Collections;

public class UIRootAdapter : MonoBehaviour {

    enum DTNodeFlag
    {
        DT_NODE_OPEN = 0x01,
        DT_NODE_CLOSED = 0x02
    }
	// Use this for initialization
	void OnEnable () {
        UIRoot root = gameObject.GetComponent<UIRoot>();
        if (null != root)
        {
            root.manualHeight = UIRootAdapter.GetLogicHeight();
            
        }
        byte flags = (byte)DTNodeFlag.DT_NODE_OPEN;
        unchecked
        {
            flags &= (byte)(~DTNodeFlag.DT_NODE_OPEN);
        }

        byte[] byts = System.BitConverter.GetBytes(flags);
        int i=5;
	}
    
    public static int GetLogicWidth()
    {
        return Screen.width * GetLogicHeight() / Screen.height;
    }

    public static int GetLogicHeight()
    {

        return 1080;
		if (Screen.height % 768 == 0)
        {
            //return  768;
            return 720;
        }
        else
        {
            return 640;
            /*
			if(GameManager.gameManager.RunningSceneName == "Login")
			{
				return 640;
			}
			else if (Screen.width % 1136 == 0)
            {
                return  750;
            }
            else
            {
                return  680;
            }
            */
        }
    }
	
}
