/********************************************************************
	created:	2013/12/24
	created:	24:12:2013   14:03
	filename: 	RoleChooseController.cs
	author:		王迪
	
	purpose:	人物选择界面控制器
*********************************************************************/

using UnityEngine;
using System.Collections;
using GCGame;
using GCGame.Table;
using Games.LogicObj;
using Games.Animation_Modle;
using Games.GlobeDefine;
using Module.Log;
public class RoleChooseController : UIControllerBase<RoleChooseController>
{
	public int  RoleNum=4;

    public GameObject[]     labelNameBoard;     // 名字牌
    public GameObject       ObjCurRoleInfo;

    private GameObject      m_curRole;          // 当前人物模型
    private int             m_curRoleIndex = -1;

    private bool            m_bBeginHeartBeat = false;      // 发送玩选人后，开始心跳
    private float           m_HeartBeatTimer = 1.0f;
	public GameObject       m_changerRole;
    void Awake()
    {
        SetInstance(this);
    }

    void Start()
    {
#if PC
		m_changerRole.SetActive (true);
#endif

        LoginUILogic.Instance().ShowChooseRoleCamera();
        ObjCurRoleInfo.SetActive(false);
        ulong lastRoleGuid =  PlayerPreferenceData.LastRoleGUID;
		for (int i = 0; i < RoleNum; i++)
        {
            if (i < LoginData.loginRoleList.Count)
            {
                int curProfession = LoginData.loginRoleList[i].type;
                if(curProfession >= (int)CharacterDefine.PROFESSION.MAX)
                {
                    LogModule.ErrorLog("curProfession value is out of range " + curProfession);
                    curProfession = 0;
                }
                
                string strProfession = null;
                string strHeadPic = null;
                Tab_CharModel curCharModel = TableManager.GetCharModelByID(curProfession, 0);
                if (null == curCharModel)
                {
                    LogModule.ErrorLog("can not find cur char info in talbe, char id:"+ curProfession.ToString());
                }
                else
                {
                    strProfession = curCharModel.Name;
                    strHeadPic = curCharModel.HeadPic;
                }
                SetRoleBoardDetail(labelNameBoard[i], LoginData.loginRoleList[i].name, strProfession, LoginData.loginRoleList[i].level.ToString(), strHeadPic);
                if (LoginData.loginRoleList[i].guid == lastRoleGuid)
                {
                    SetCurRoleIndex(i);
                }
            }
            else
            {
                SetRoleBoardDetail(labelNameBoard[i], null);
            }
        }

        // 如果没有记录上次选择角色，默认展示第一个
        if (m_curRoleIndex < 0 && LoginData.loginRoleList.Count > 0)
        {
            SetCurRoleIndex(0);
        }
       
        LoadPlayer();

        GCGame.Utils.PlaySceneMusic(44);    //scene_selecting
    }

    void OnDestroy()
    {
        UIControllerBase<RoleChooseController>.SetInstance(null);
        ResourceManager.DestroyResource(ref m_curRole);
    }

    void Update()
    {
        if (m_bBeginHeartBeat)
        {
           // m_HeartBeatTimer -= Time.deltaTime;
           // if (m_HeartBeatTimer < 0)
            //{
                CG_CONNECTED_HEARTBEAT cgBeat = (CG_CONNECTED_HEARTBEAT)PacketDistributed.CreatePacket(MessageID.PACKET_CG_CONNECTED_HEARTBEAT);
                cgBeat.SetIsresponse(0);
                m_bBeginHeartBeat = false;
                cgBeat.SendPacket();
            //    m_HeartBeatTimer = 1.0f;
            //}
        }
    }

    void OnBackClick()
    {
        //NetManager.SendUserLogout();
        m_bBeginHeartBeat = false;
        NetWorkLogic.GetMe().DisconnectServer();
        LoginUILogic.Instance().EnterServerChoose();
    }

	//====添加临时账号切换
	private int tempId;
	void OnChangeClick()
	{
		string str = "注意：本操作不可逆，功能为临时使用，确认后返回选择服务器界面再次进入即可！！！";
		MessageBoxLogic.OpenOKCancelBox(str, "注意！！！", UnlockOk);
	}
	private void UnlockOk()
	{
		tempId = PlayerPrefs.GetInt("tempId");
		tempId++;
		PlayerPrefs.SetInt ("tempId", tempId);

		OnBackClick ();
	}

    void OnEnterClick()
    {
        if (m_curRoleIndex < 0)
        {
            MessageBoxLogic.OpenOKBox(1369, 1000);
            return;
        }
        m_bBeginHeartBeat = true;
        NetManager.SendChooseRole(LoginData.loginRoleList[m_curRoleIndex].guid, RetSelectRoleFail);

		//========当前选择的角色
		PlayerPrefs.SetString ("CurrPlayerIndex", m_curRoleIndex.ToString ());

        if (!GameManager.gameManager.OnLineState)
        {
            LoginUILogic.Instance().EnterGame();
        }
        else
        {
            MessageBoxLogic.OpenWaitBox(1290, GameDefines.CONNECT_TIMEOUT,GameDefines.CONNECT_WAIT_DELAY, NetManager.OnWaitPacketTimeOut); 
        }
    }

    void OnRoleBandClick(GameObject item)
    {
        int index = -1;
        if (int.TryParse(item.name, out index))
        {
            if (index >= LoginData.loginRoleList.Count)
            {
                LoginUILogic.Instance().ShowCreateRoleCamera();
                LoginUILogic.Instance().EnterCreateRole();
            }
            else
            {
                if (index != m_curRoleIndex)
                {
                    SetCurRoleIndex(index);
                    LoadPlayer();
                }
            }
        }
    }

    // 展示当前角色，并设置相关信息
    void SetCurRoleIndex(int index)
    {
        m_curRoleIndex = index;

		for (int i = 0; i < RoleNum; i++)
        {
            labelNameBoard[i].transform.FindChild("Highlight").gameObject.SetActive( (i == m_curRoleIndex));
        }
        ObjCurRoleInfo.SetActive(true);
        Transform transLabelName = ObjCurRoleInfo.transform.FindChild("LabelName");
        Transform transLabelFamily = ObjCurRoleInfo.transform.FindChild("LabelProfession");
        Transform transLabelLevel = ObjCurRoleInfo.transform.FindChild("LabelLevel");

        transLabelName.gameObject.GetComponent<UILabel>().text = LoginData.loginRoleList[index].name;
        transLabelFamily.gameObject.GetComponent<UILabel>().text =  Utils.GetDicByID(CharacterDefine.PROFESSION_DICNUM[LoginData.loginRoleList[index].type]);
        transLabelLevel.gameObject.GetComponent<UILabel>().text = LoginData.loginRoleList[index].level.ToString();

    }


    // 设置当前角色信息，如果没有信息显示创建角色按钮
    void SetRoleBoardDetail(GameObject board, string name = null, string family = null, string level = null, string spriteName = null)
    {
        if (name != null)
        {
            UIImageButton[] buttonArray = board.transform.GetComponentsInChildren<UIImageButton>();
            if (buttonArray != null && buttonArray.Length > 0)
            {
                buttonArray[0].gameObject.SetActive(false);
            }
            board.transform.FindChild("RoleHead").GetComponent<UISprite>().spriteName = spriteName;
            board.transform.FindChild("RoleHead").gameObject.SetActive(true);
        }
        else
        {
            UIImageButton[] buttonArray = board.transform.GetComponentsInChildren<UIImageButton>();
            if (buttonArray != null && buttonArray.Length > 0)
            {
                buttonArray[0].gameObject.SetActive(true);
            }
            board.transform.FindChild("RoleHead").gameObject.SetActive(false);
        }
    }

    // 加载人物并放置到3D相机下
    void LoadPlayer()
    {
        if (m_curRoleIndex < 0)
        {
            return;
        }
        
        if (null != m_curRole)
        {
            ResourceManager.DestroyResource(ref m_curRole);
        }

        int Type = LoginData.loginRoleList[m_curRoleIndex].type;
        int ModelVisualID = LoginData.loginRoleList[m_curRoleIndex].ModelVisualID;
        int WeaponID = LoginData.loginRoleList[m_curRoleIndex].WeaponID;
		//====TODO===临时逻辑 后期宝石走正常逻辑
	     int WeaponEffectGem = LoginData.loginRoleList[m_curRoleIndex].WeaponEffectGem;
//		int WeaponEffectGem = 0;
//		switch(Type)
//		{
//		case 0:
//			WeaponEffectGem = 8301;
//			break;
//		case 1:
//			WeaponEffectGem = 8201;
//			break;
//		case 2:
//			WeaponEffectGem = 8901;
//			break;
//		case 3:
//			WeaponEffectGem = 8701;
//			break;
//		}

        LoginUILogic.Instance().ShowModel(Type, ModelVisualID, WeaponID, WeaponEffectGem);
    }

    void RetSelectRoleFail(GC_SELECTROLE_RET.SELECTROLE_RESULT result)
    {
        m_bBeginHeartBeat = false;
        // 选择角色失败，点击确定返回登录界面
        MessageBoxLogic.OpenOKBox(1291,1000, OnEnterServerMain);
    }

    void OnEnterServerMain()
    {
        if (null != LoginUILogic.Instance())
        {
            LoginUILogic.Instance().EnterServerChoose();
        }
    }
}