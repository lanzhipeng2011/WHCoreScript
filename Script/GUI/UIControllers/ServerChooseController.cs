/********************************************************************
	created:	2013/12/24
	created:	24:12:2013   14:01
	filename: 	ServerChooseController.cs
	author:		王迪
	
	purpose:	服务器选择UI控制器
*********************************************************************/

using UnityEngine;
using System.Collections;
using Module.Log;
using GCGame.Table;
public class ServerChooseController : UIControllerBase<ServerChooseController> {

    public enum Window
    {
        ServerTop,              // 简洁服务器选择界面
        ServerList,             // 复杂服务器选择界面
    }

    public ServerTopWindow m_ServerTopWindow;

    public GameObject m_ForceEnterTip;

	public UILabel labelVersion;

	public UILabel inPutLabel;
	public GameObject inPutObj;

    private float m_connnectTimer = 0;

    private bool m_bShowConnectResult = false;
    void Awake()
    {
       
        SetInstance(this);
    }
	void Start () 
    {
		//=====
		inPutObj.SetActive (false);
		clickOnOpenBtnNum = 0;
		//======
        SwitchWindow((int)Window.ServerTop);
        GCGame.Utils.PlaySceneMusic(42);    //scene_login
		labelVersion.text = string.Format("Version:{0}.{1}.{2}.{3}", PlatformHelper.GetGameVersion(), PlatformHelper.GetProgramVersion(), TableManager.GetPublicConfigByID(GameDefines.PublicResVersionKey, 0).IntValue, UserConfigData.ClientResVersion);

	}

    void OnEnable()
    {
        m_connnectTimer = 0;
		clickOnOpenBtnNum = 0;
    }

    void Update()
    {
        if (m_connnectTimer > 0)
        {
            m_connnectTimer -= Time.deltaTime;
        }
    }

    public void ConnectToServer(string szIp, int nPort)
    {
		NGUIDebug.Log("44444:");
        if (m_connnectTimer > 0)
        {
            return;
        }
		NGUIDebug.Log("555555:");
        m_connnectTimer = 3.0f;
        m_bShowConnectResult = false;
        NetManager.Instance().ConnectToServer(szIp, nPort, OnConnectResult);
        // 连接服务器，请等待
        MessageBoxLogic.OpenWaitBox(1003, 20, 0, OnConnectTimeOut);
    }
	//===========开启ID输入界面
	private int clickOnOpenBtnNum = 0;
	void OnOpenInputFun()
	{
		clickOnOpenBtnNum++;
		if(clickOnOpenBtnNum == 8)
		{
			inPutObj.SetActive(true);
			clickOnOpenBtnNum = 0;
		}else{
			inPutObj.SetActive(false);
		}
	}
	//=========开启ID输入界面end
    void OnConnectResult(bool bSuccess, string result)
    {
        
        if (bSuccess)
        {
            MessageBoxLogic.CloseBox();
			if(inPutLabel.text != "")
			{
				LoginUILogic.Instance().EnterAccount(inPutLabel.text);
			}else{
				LoginUILogic.Instance().EnterAccount();
			}
            
        }
        else
        {
            if (!m_bShowConnectResult)
            {
                StartCoroutine(RequestServerState(PlayerPreferenceData.LastServer));
                m_bShowConnectResult = true;
            }
            LogModule.WarningLog("connect fail");
            m_connnectTimer = 0;
        }
    }

    void OnConnectTimeOut()
    {
       
        if (!m_bShowConnectResult)
        {
            StartCoroutine(RequestServerState(PlayerPreferenceData.LastServer));
            m_bShowConnectResult = true;
        }

        LogModule.WarningLog("connect fail");
        m_connnectTimer = 0;

    }

    void OnEnterAccount()
    {
        GameManager.gameManager.OnLineState = false;
        LoginUILogic.Instance().EnterAccount();
    }

    public void SelectServerListItem(LoginData.ServerListData curListData)
    {
        //SwitchWindow((int)ServerChooseController.Window.ServerTop);
        m_ServerTopWindow.SetCurServerInfo(curListData);
    }


    public void ShowForceEnterTip(bool bShow)
    {
        m_ForceEnterTip.SetActive(bShow);
    }

    void OnForceLogin()
    {
        NetManager.SendUserLogin(LoginData.Ret_Login, true);
        ShowForceEnterTip(false);
    }

    void OnUpdateApp()
    {
        if (PlatformHelper.GetNetworkState() == PlatformHelper.NetworkState.STATE_WIFI)
        {
#if UNITY_ANDROID
			LoginUILogic.DoUpdateApp();
#else
			Application.OpenURL(PlatformHelper.GetUpdateAppUrl());
			ShowForceEnterTip(false);
#endif
        }
        else
        {
            ShowForceEnterTip(false);
            MessageBoxLogic.OpenOKCancelBox(2622, 1000, OnDoUpdateApp);
        }
        
    }

    void OnDoUpdateApp()
    {
		LoginUILogic.DoUpdateApp();
    }

    IEnumerator RequestServerState(int serverID)
    {
        WWW wwwData = new WWW(DownloadHelper.AddTimestampToUrl("http://" + serverID.ToString() + ".txt"));
        yield return wwwData;
        if (string.IsNullOrEmpty(wwwData.error) && wwwData.text != "None")
        {
			//提示： 无法连接服务器
			MessageBoxLogic.OpenOKBox(1005, 1000);
        }
        else
        {
            LogModule.ErrorLog(wwwData.error);
            //提示： 无法连接服务器
            MessageBoxLogic.OpenOKBox(1005, 1000);
        }
        
    }

}
