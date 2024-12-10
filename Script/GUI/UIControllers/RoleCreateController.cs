/********************************************************************
	created:	2013/12/24
	created:	24:12:2013   14:02
	filename: 	RoleCreateController.cs
	author:		王迪
	
	purpose:	角色选择界面控制器
*********************************************************************/

using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
using SPacket.SocketInstance;
using GCGame;
using GCGame.Table;
using System.Collections.Generic;

public class RoleCreateController : UIControllerBase<RoleCreateController> {

    public enum MOUSE_BUTTON
    {
        MOUSE_BUTTON_LEFT,
        MOUSE_BUTTON_RIGHT,
        MOUSE_BUTTON_MIDDLE,
    }

    public GameObject[] m_SelectRoles;
    public GameObject[] m_RolesInfos;

    public UIInput inputName;
    public GameObject m_ProfessionDetail;
    public GameObject m_NameObject;
    public GameObject m_CharSelectRoot;
    public GameObject m_BtnEnterGame;
    public AudioClip m_SelectCharSound;
    private int m_ChooseIndex = GlobeVar.INVALID_ID;

//    private int m_LastChooseIndex = 2;
    //private Color m_disableColor = new Color(64.0f/255.0f, 64.0f/255.0f, 64.0f/255.0f, 1.0f);

    int[] professionIndex = 
    {
     (int)CharacterDefine.PROFESSION.TIANSHAN,
     (int)CharacterDefine.PROFESSION.XIAOYAO,
     (int)CharacterDefine.PROFESSION.SHAOLIN,
     (int)CharacterDefine.PROFESSION.DALI,
    };

    private string m_strChooseRole;
    private BoxCollider m_DragCollider;

    private float m_showProfessionDelay = 0;
    private float m_showProDetailDelay = 0;

    public  float m_delayBegin = 3;
    //private float m_delaySelect = 2.167f;
    private float m_delaySelectBack = 1.717f;

    private bool m_bBeginHeartBeat = false;      // 发送玩选人后，开始心跳
    private float m_HeartBeatTimer = 1.0f;

    void Awake()
    {
        SetInstance(this);
    }
	void Start ()
    {
		LoginData.m_randomNameList.Clear ();
        LoginUILogic.RequestRandomName();

        GCGame.Utils.PlaySceneMusic(43);        //scene_select_character

        LoginUILogic.Instance().ShowCreateRoleCamera();
        
        //LoginUILogic.Instance().CreateRoleReChoose();
        SelectChar(2);
        m_strChooseRole = "";
        SetRandomNameToInput();
	}

	void OnEnable()
	{
		//=====当创建角色界面显示时初始化旋转角度
		LoginUILogic.Instance().CreateRoleChar.transform.localRotation = Quaternion.Euler(0,145f,0);
	}

    void FixedUpdate()
    {
		//判断人物播放yanwustand时添加collider
		if(modelGo.GetComponent<Animation>().IsPlaying("yanwustand"))
		{
			if(!modelGo.transform.parent.transform.GetComponent<CapsuleCollider> ().enabled)
				modelGo.transform.parent.transform.GetComponent<CapsuleCollider> ().enabled = true;
		}

        if (Input.touchCount <= 0)
        {
            if (Input.GetMouseButton((int)MOUSE_BUTTON.MOUSE_BUTTON_LEFT))
            {
                Vector3 posPressed = Input.mousePosition;
                if (IsClickOnUI(posPressed))
                {
                    return;
                }

                if (!m_ProfessionDetail.activeSelf && !LoginUILogic.Instance().IsPlayingCameraAni())
                {
                    ProcessChooseRole3D(posPressed);
                }
            }
        }
        else
        {
            Touch touch = Input.GetTouch(0);
            Vector3 posPressed = touch.position;
            if (IsClickOnUI(posPressed))
            {
                return;
            }
            if (!m_ProfessionDetail.activeSelf && !LoginUILogic.Instance().IsPlayingCameraAni())
            {
                ProcessChooseRole3D(posPressed);
            }
            
        }

        if (m_showProfessionDelay > 0)
        {
            m_showProfessionDelay -= Time.deltaTime;
            if (m_showProfessionDelay <= 0)
            {
                if (null != LoginUILogic.Instance())
                {
                    LoginUILogic.Instance().ShowProfessionEffect(true);
                }
            }
        }

        if (m_showProDetailDelay > 0)
        {
            m_showProDetailDelay -= Time.deltaTime;
            if (m_showProDetailDelay <= 0)
            {
                m_ProfessionDetail.SetActive(true);
                m_CharSelectRoot.SetActive(true);
                m_NameObject.SetActive(true);
                m_BtnEnterGame.SetActive(true);
               
            }
        }

        if (m_bBeginHeartBeat)
        {
            m_HeartBeatTimer -= Time.deltaTime;
            if (m_HeartBeatTimer <= 0)
            {
                CG_CONNECTED_HEARTBEAT cgBeat = (CG_CONNECTED_HEARTBEAT)PacketDistributed.CreatePacket(MessageID.PACKET_CG_CONNECTED_HEARTBEAT);
                cgBeat.SetIsresponse(0);
                cgBeat.SendPacket();
                m_HeartBeatTimer = 1.0f;
            }
        }
    }



    void RoleCreateOnClick()
    {
        LoginUILogic.Instance().PlayRoleCreateOtherAni(m_strChooseRole);
    }

    void UpdateProDetailSprite(string charName, bool playSound = false)
    {
        charName = charName.ToLower();
        for(int i=0; i<m_ProfessionDetail.transform.childCount; i++)
        {
            Transform curChild = m_ProfessionDetail.transform.GetChild(i);
            curChild.gameObject.SetActive(curChild.name.Contains(charName));

            Transform curSelectChild = m_CharSelectRoot.transform.GetChild(i);
            curSelectChild.gameObject.SetActive(curSelectChild.name.Contains(charName));

            if (playSound)
            {
                NGUITools.PlaySound(m_SelectCharSound, 0.4f, 1.0f);
            }            
        }
    }
	
  
    void OnCloseClick()
    {
        if (LoginData.loginRoleList.Count > 0)
        {
            LoginUILogic.Instance().EnterChooseRole();
        }
        else 
        {
            NetWorkLogic.GetMe().DisconnectServer();
            LoginUILogic.Instance().EnterServerChoose();
        } 
    }

    void OnNameOkClick()
    {
        if (m_ChooseIndex == GlobeVar.INVALID_ID)
        {
            GUIData.AddNotifyData2Client(false,"#{2158}");
            return;
        }

        

        if (string.IsNullOrEmpty(inputName.value))
        {
            //请输入人物名称
            MessageBoxLogic.OpenOKBox(1281, 1000);
            return;
        }
        string strCurName = "";

        // 过滤掉 0 非法字符
        foreach (char curChar in inputName.value)
        {
            if ((int)curChar != 0)
            {
                strCurName += curChar;
            }
        }

        if (string.IsNullOrEmpty(strCurName))
        {
            //请输入人物名称
            MessageBoxLogic.OpenOKBox(1281, 1000);
            return;
        }

        int curCharNum = 0;     // 英文算一个，中文算两个 
        foreach (char curChar in strCurName)
        {
//             if ((int)curChar >= 128)
//             {
// 
//                 curCharNum += 2;
//             }
//             else if ((int)curChar >= 65 && (int)curChar <= 90)
//             {
//                 curCharNum += 2;
//             }    
//             else
//             {
//                 curCharNum++;
//             }
            curCharNum += 2;

            if (char.IsWhiteSpace(curChar))
            {
                //名字不能包含空格
                MessageBoxLogic.OpenOKBox(1280, 1000);
                return;
            }
        }
        if (curCharNum > 12)
        {
            // 名字过长
            MessageBoxLogic.OpenOKBox(1279, 1000);
            return;
        }

        if (inputName.value.Contains("*"))
        {
            MessageBoxLogic.OpenOKBox(1278, 1000);
            return;
        }
		//=====恢复原逻辑过滤非法字符
        if (null == Utils.GetStrFilter(strCurName, (int)Games.GlobeDefine.GameDefine_Globe.STRFILTER_TYPE.STRFILTER_NAME))
        {

			SendCreateRole();
        }
        else
        {
            // 包含非法字符
            MessageBoxLogic.OpenOKBox(1278,1000);
        }
    }

    void SendCreateRole()
    {

      //  PlatformHelper.SendUserAction(UserBehaviorDefine.RoleCreate_Enter);
        // 在这里记录有问题，应该确定发送成功，才能加入
        m_bBeginHeartBeat = true;
        GC_CREATEROLE_RETHandler.retCreateRoleFail = RetCreateRoleFail;
        CG_CREATEROLE createRolePacket = (CG_CREATEROLE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_CREATEROLE);
        createRolePacket.SetType(professionIndex[m_ChooseIndex]);
        createRolePacket.SetName(inputName.value);
        createRolePacket.SendPacket();
        MessageBoxLogic.OpenWaitBox(1014, GameDefines.CONNECT_TIMEOUT, GameDefines.CONNECT_WAIT_DELAY, NetManager.OnWaitPacketTimeOut);
        if (!GameManager.gameManager.OnLineState)
        {
            LoginUILogic.Instance().EnterGame();
        }
    }

    void RetCreateRoleFail(GC_CREATEROLE_RET.CREATEROLE_RESULT result)
    {
        m_bBeginHeartBeat = false;
        if(result == GC_CREATEROLE_RET.CREATEROLE_RESULT.CREATEROLE_FAIL_NAMEEXIST)
        {
            // 该名称已经被占用
            MessageBoxLogic.OpenOKBox(1282, 1000);
        }
        else if(result ==  GC_CREATEROLE_RET.CREATEROLE_RESULT.CREATEROLE_FAIL_NAMESCREENING)
        {
            // 包含非法字符
            MessageBoxLogic.OpenOKBox(1278, 1000);
        }
        else
        {
            // 创建人物失败，点击确定返回登录界面
            MessageBoxLogic.OpenOKBox(1283, 1000, OnEnterLogin);
        }
    }

    void OnEnterLogin()
    {
        if (null != LoginUILogic.Instance())
        {
            LoginUILogic.Instance().EnterServerChoose();
        }
    }

    void OnRandomNameClick()
    {
        SetRandomNameToInput();
    }

    void SetRandomNameToInput()
    {
		//服务器没加入，客户端自己生成
        if (LoginData.m_randomNameList.Count == 0)
        {
            List<Tab_RoleName> firstName = TableManager.GetRoleNameByID(1);
            List<Tab_RoleName> maleName = TableManager.GetRoleNameByID(2);
            List<Tab_RoleName> femaleName = TableManager.GetRoleNameByID(3);

            string curName = firstName[Random.Range(0, firstName.Count - 1)].FirstName;
            if (IsMaleIndex(m_ChooseIndex) > 0)
            {
                curName += maleName[Random.Range(0, maleName.Count - 1)].FirstName;
            }
            else
            {
                curName += femaleName[Random.Range(0, femaleName.Count - 1)].FirstName;
            }
            LoginData.m_randomNameList.Add(curName);
        }
		//每次用一个用了就删除
        inputName.value = LoginData.m_randomNameList[0];
		LoginData.m_randomNameList.RemoveAt(0);
	
		//如果还剩下一个，一定是服务器传送的还剩一个，请求更多的
		if (LoginData.m_randomNameList.Count == 1 )
		{
			LoginUILogic.RequestRandomName();
		}
    }

    int IsMaleIndex(int index)
    {
        return (index == 3 || index == 2) ?  1 : 0;
    }

    public void OnNameSubmit()
    {
        int curCharNum = 0;
        int curIndex = 0;
        foreach (char curChar in inputName.value)
        {
            if ((int)curChar < 128)
            {
                curCharNum++;
            }
            else
            {
                curCharNum += 2;
            }

            curIndex++;
            if (curCharNum == 12)
            {
                break;
            }

            if (curCharNum == 13)
            {
                curIndex -= 1;
                break;
            }
        }

        inputName.value = inputName.value.Substring(0, curIndex);
    }

    bool IsClickOnUI(Vector3 posPressed)
    {
        if (null == UICamera.mainCamera)
        {
            return false;
        }

        Ray rayUI = UICamera.mainCamera.ScreenPointToRay(posPressed);
        RaycastHit hitUI;
        if (Physics.Raycast(rayUI, out hitUI))
        {
            //if (hitUI.collider.gameObject.tag == "UI")
            if (hitUI.collider.gameObject.CompareTag("UI"))
            {
                return true;
            }
        }

        return false;
    }

    void ProcessChooseRole3D(Vector3 posPressed)
    {
        Ray ray = Camera.main.ScreenPointToRay(posPressed);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            PlayerCharOnClicked3D(hit.collider.gameObject.name);
        }
    }
	private string tempName = "DaoKe";
    public void PlayerCharOnClicked(GameObject go) 
    {
        LoginUILogic.Instance().CreateRoleChar.transform.localRotation = Quaternion.Euler(0,127.9f,0);
		//判断上次点击人物名称是否与当前相同
		if(tempName == go.name)
			return;
		else
			tempName = go.name;

        for (int i = 0; i < m_SelectRoles.Length;i++ )
        {
            if (go.name == m_SelectRoles[i].name) 
            {
                SelectChar(i);
            }
        }
    }
	private GameObject modelGo;
	int lastsound=0;
    public void SelectChar(int selecti) 
    {
        for (int i = 0; i < m_SelectRoles.Length;i++)
        {
            m_SelectRoles[i].transform.Find("BG").GetComponent<UISprite>().spriteName = "ui_Vocation_13";
        }
        m_SelectRoles[selecti].transform.Find("BG").GetComponent<UISprite>().spriteName = "ui_Vocation_14";
        m_ChooseIndex = selecti;

        for (int i = 0; i < m_RolesInfos.Length;i++ )
        {
            m_RolesInfos[i].gameObject.SetActive(false);
        }
        m_RolesInfos[selecti].gameObject.SetActive(true);
		if(lastsound!=0)
			GameManager.gameManager.SoundManager.StopSoundEffect (lastsound);
		string weapon = "Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/HH_weaponHandRt";
		string weapon2 = ""; 
		switch (selecti) 
		{
		case 0:
			GameManager.gameManager.SoundManager.PlaySoundEffect(2113);  
			//GameManager.gameManager.SoundManager.PlaySoundEffect(114);  
			lastsound=2113;
			weapon2="Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 L Clavicle/Bip001 L UpperArm/Bip001 L Forearm/Bip001 L Hand/HH_weaponHandLf";
			

			break;
		case 1:
			GameManager.gameManager.SoundManager.PlaySoundEffect(2114);  //login_dali
			//GameManager.gameManager.SoundManager.PlaySoundEffect(116);  
			lastsound=2114;
			break;
		case 2:
			GameManager.gameManager.SoundManager.PlaySoundEffect(2112);  //login_tianshan
			//GameManager.gameManager.SoundManager.PlaySoundEffect(113);  
			lastsound=2112;
				break;
		case 3:
			GameManager.gameManager.SoundManager.PlaySoundEffect(2115);  //login_xiaoyao
			//GameManager.gameManager.SoundManager.PlaySoundEffect(115);  
			lastsound=2115;
			break;
		}
        for (int i = 0; i < LoginUILogic.Instance().CreateRoleModels.Length;i++ )
        {
            LoginUILogic.Instance().CreateRoleModels[i].SetActive(false);
        }
        GameObject ModelGo =  LoginUILogic.Instance().CreateRoleModels[selecti];
        ModelGo.SetActive(true);
        ModelGo.GetComponent<Animation>().Stop();
		//在切换人物时去掉人物身上的collider
		modelGo = ModelGo;
		modelGo.transform.parent.transform.GetComponent<CapsuleCollider> ().enabled = false;
		//  蒲扇没有演武动作
//        if (selecti == 1)
//        {
//            ModelGo.GetComponent<Animation>().PlayQueued("skill40");
//            ModelGo.GetComponent<Animation>().PlayQueued("Stand");
//        }
//        else 
        {
            ModelGo.GetComponent<Animation>().PlayQueued("yanwu");
            ModelGo.GetComponent<Animation>().PlayQueued("yanwustand");
        }
		GameObject obj=ModelGo.GetComponentInChildren<SkinnedMeshRenderer>().gameObject;

		obj.renderer.material.shader=Shader.Find("MLDJ/MYBumpedSpecular");
		obj.renderer.material.SetFloat("_GlossQR",10.0f);
		obj.renderer.material.SetFloat("_NoramlQR",10.0f);
		obj.renderer.material.SetFloat("_Cutoff",0.5f);
		GameObject weaponobj = GameObject.Find (weapon);
		if (weapon2 != "") 
		{
			GameObject weaponobj2 = GameObject.Find (weapon2);
			weaponobj2.GetComponentInChildren<MeshRenderer>().renderer.material.shader=Shader.Find("MLDJ/MYBumpedSpecular");

		}
		weaponobj.GetComponentInChildren<MeshRenderer>().renderer.material.shader=Shader.Find("MLDJ/MYBumpedSpecular");


    }
    void PlayerCharOnClicked3D(string name)
    {
        float changeDelay = 0;
        if (name.Contains("XiaoYao"))
        {
            changeDelay = LoginUILogic.Instance().PlayCameraAni("Camera_xiaoyao");
            m_strChooseRole = "XiaoYao";
            m_ChooseIndex = 1;
            LoginUILogic.Instance().CreateRoleClick(m_strChooseRole);
            m_DragCollider.enabled = true;
        }
        else if (name.Contains("Tianshan"))
        {
           changeDelay = LoginUILogic.Instance().PlayCameraAni("Camera_tianshan");
            m_strChooseRole = "Tianshan";
            m_ChooseIndex = 0;
            LoginUILogic.Instance().CreateRoleClick(m_strChooseRole);
            m_DragCollider.enabled = true;
        }
        else if (name.Contains("ShaoLin"))
        {
            changeDelay = LoginUILogic.Instance().PlayCameraAni("Camera_shaolin");
            m_strChooseRole = "ShaoLin";
            m_ChooseIndex = 2;
            LoginUILogic.Instance().CreateRoleClick(m_strChooseRole);
            m_DragCollider.enabled = true;
        }
        else if (name.Contains("DaLi"))
        {
            changeDelay = LoginUILogic.Instance().PlayCameraAni("Camera_dali");
            m_strChooseRole = "DaLi";
            m_ChooseIndex = 3;
            LoginUILogic.Instance().CreateRoleClick(m_strChooseRole);
            m_DragCollider.enabled = true;
        }

        if (changeDelay > 0)
        {
            UpdateProDetailSprite(m_strChooseRole, true);
            ShowProDetailDelay(0.1f);
            SetRandomNameToInput();
        }
       
    }

    void ReturnChooseChar()
    {
        if (m_strChooseRole.Contains("XiaoYao"))
        {
            LoginUILogic.Instance().m_CameraAni.Play("Camera_xiaoyao_hui");
            LoginUILogic.Instance().m_CameraAni.PlayQueued("shexiangji_huangdong");
            m_strChooseRole = "";         
            m_ChooseIndex = GlobeVar.INVALID_ID;
            LoginUILogic.Instance().CreateRoleReChoose();
            m_DragCollider.enabled = false;
        }
        else if (m_strChooseRole.Contains("Tianshan"))
        {
            LoginUILogic.Instance().m_CameraAni.Play("Camera_tianshan_hui");
            LoginUILogic.Instance().m_CameraAni.PlayQueued("shexiangji_huangdong");
            m_strChooseRole = "";
            m_ChooseIndex = GlobeVar.INVALID_ID;
            LoginUILogic.Instance().CreateRoleReChoose();
            m_DragCollider.enabled = false;
        }
        else if (m_strChooseRole.Contains("ShaoLin"))
        {
            LoginUILogic.Instance().m_CameraAni.Play("Camera_shaolin_hui");
            LoginUILogic.Instance().m_CameraAni.PlayQueued("shexiangji_huangdong");
            m_strChooseRole = "";
            m_ChooseIndex = GlobeVar.INVALID_ID;
            LoginUILogic.Instance().CreateRoleReChoose();
            m_DragCollider.enabled = false;
        }
        else if (m_strChooseRole.Contains("DaLi"))
        {
            LoginUILogic.Instance().m_CameraAni.Play("Camera_dali_hui");
            LoginUILogic.Instance().m_CameraAni.PlayQueued("shexiangji_huangdong");
            m_strChooseRole = "";
            m_ChooseIndex = GlobeVar.INVALID_ID;
            LoginUILogic.Instance().CreateRoleReChoose();
            m_DragCollider.enabled = false;
        }
        ShowProfessionDelay(m_delaySelectBack);
    }

    void ShowProfessionDelay(float delay)
    {
        m_showProfessionDelay = delay;
    }

 
    void ShowProDetailDelay(float delay)
    {
        if (delay > 0)
        {
            m_showProDetailDelay = delay;
        }
        
    }

    void HideProDetail()
    {
        m_ProfessionDetail.SetActive(false);
        m_CharSelectRoot.SetActive(false);
        m_NameObject.SetActive(false);
        m_BtnEnterGame.SetActive(false);
        m_showProDetailDelay = 0;
    }
}
 