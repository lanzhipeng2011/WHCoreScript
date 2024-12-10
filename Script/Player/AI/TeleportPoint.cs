/********************************************************************************
 *	文件名：	TeleportPoint.cs
 *	全路径：	\Script\Player\TeleportPoint.cs
 *	创建人：	李嘉
 *	创建时间：2013-10-25
 *
 *	功能说明：游戏传送点逻辑
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Games.Events;
using Games.GlobeDefine;
using Games.LogicObj;
using GCGame.Table;
using Games.Mission;

public class TeleportPoint : MonoBehaviour 
{
    //传送点目标场景ID
    public int TeleportID = -1;
    public int ActiveRadius = 3;

    private bool m_bValid = true;       //是否合法，触发一次之后失效
    private float m_fLastInvaildTime = 0.0f;
    //头顶信息板相关
    private GameObject m_HeadInfoBoard;        // 头顶信息板总节点
    public UnityEngine.GameObject HeadInfoBoard
    {
        get { return m_HeadInfoBoard; }

    }
    private GameObject m_NameBoard;            // teleport名字板

    //缓存数据
    Transform m_MainPlayerTransform = null;
    Transform m_TeleportTransform = null;
    Tab_Teleport m_tabTeleport = null;

    private bool m_bEnterPKSceneCancel = false;
	//=====标记当前距离
	private bool m_bIsCancel = false;

    void InitNameBoard()
    {
        ResourceManager.LoadHeadInfoPrefab(UIInfo.NPCHeadInfo, gameObject, "NPCHeadInfo", OnLoadNameBoard);        
    }

    void OnLoadNameBoard(GameObject objNameBoard)
    {
        m_HeadInfoBoard = objNameBoard;
        //传送点名字版固定太高一个高度
        if (null != m_HeadInfoBoard)
        {
            Transform nameBoardOffset = m_HeadInfoBoard.transform.FindChild("NameBoardOffset");
            if (null != nameBoardOffset)
            {
				nameBoardOffset.localScale=new Vector3(0.4f,0.4f,0.4f);
                Transform nameBoard = nameBoardOffset.FindChild("NameBoard");
                if (null != nameBoard)
                {
                    m_NameBoard = nameBoard.gameObject;
                }
            }

            BillBoard billboardScript = m_HeadInfoBoard.GetComponent<BillBoard>();
            if (null != billboardScript)
            {
                billboardScript.fDeltaHeight = 3.0f;
            }
        }

        //传送点名字
        if (null != m_NameBoard)
        {
            UILabel nameBoardLabel = m_NameBoard.GetComponent<UILabel>();
            if (null != nameBoardLabel)
            {
                Tab_Teleport teleport = TableManager.GetTeleportByID(TeleportID, 0);
                if (null != teleport)
                {
                    nameBoardLabel.text = teleport.TeleportName;
                    nameBoardLabel.color = CharacterDefine.NPC_COLOR_NEUTRAL;
                }
                else
                {
                    nameBoardLabel.text = "";
                }
            }
        }
    }

    void Start()
    {
		if (GameManager.gameManager.RunningScene == 14||GameManager.gameManager.RunningScene == 15) 
		{
			//如果是江夏和巨鹿剧情副本，则删除传送点
			DestroyImmediate(this.gameObject);
		}
		Invoke("InitNB",3.0f);
        if (null == m_TeleportTransform)
        {
            m_TeleportTransform = transform;
        }
		enabled = false;
    }
	void InitNB()
	{
		InitNameBoard();
	}
    void OnBecameVisible()
    {
        enabled = true;
    }
    void OnBecameInvisible()
    {
        enabled = false;
    }
      
    void FixedUpdate()
    {
        if (false == m_bValid)
        {
            //3s内不能再次触发传送
            if (Time.time -m_fLastInvaildTime <3.0f)
            {
                return;
            }
            m_bValid = true;
        }

        if (null == m_MainPlayerTransform)
        {
            if (null != Singleton<ObjManager>.Instance.MainPlayer)
            {
                m_MainPlayerTransform = Singleton<ObjManager>.Instance.MainPlayer.transform;
            }

            if (null == m_MainPlayerTransform)
            {
                return;
            }
        }

        if (null != Singleton<ObjManager>.Instance.MainPlayer)
        {
			if (Vector3.Distance(m_MainPlayerTransform.position, m_TeleportTransform.position) <= ActiveRadius )
            {
                  //根据ID从表格中获取信息
                Tab_Teleport teleport = TableManager.GetTeleportByID(TeleportID, 0);
                if (null == teleport)
                {
                    return;
                }
                if (GameManager.gameManager.ActiveScene.IsCopyScene())
                {
                    if (teleport.DstSceneID == -1)
                    {
                         SendLeaveCopyScene();
                         m_bValid = false;
                         m_fLastInvaildTime = Time.time;//记录下失效时间点
                    }
                    else
                    {


                        Tab_SceneClass tabSceneClass = TableManager.GetSceneClassByID(teleport.DstSceneID, 0);
                        if (tabSceneClass != null)
                        {
                            if (tabSceneClass.Type == (int)GameDefine_Globe.SCENE_TYPE.SCENETYPE_COPYSCENE)
                            {
                                SendOpenCopyScene();
                                m_bValid = false;
                                m_fLastInvaildTime = Time.time;//记录下失效时间点
                            }
                        }
                    }
                }
                else
                {
                    if (IsCanPK(teleport.DstSceneID) && !IsIncPKValue(teleport.DstSceneID))
                    {
                        if (!m_bEnterPKSceneCancel)
                        {
                            m_tabTeleport = teleport;
                            MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{2672}"), "", EnterNonePKValueSceneOK, EnterNonePKValueSceneCancel);
                        }
                    }
                    else
                    {
						//==============判断当前如果是跑商任务逻辑
						if(m_bIsCancel == false)
						{

							bool isMissionBiz = false;
							// 添加任务
							List<int> nMissionIDList = GameManager.gameManager.MissionManager.GetAllMissionID();
							int nMissionCount = nMissionIDList.Count;
							if (nMissionCount != 0)
							{
								for(int i = 0;i<nMissionCount;i++)
								{
									if((int)MISSIONTYPE.MISSION_BIZ == GameManager.gameManager.MissionManager.GetMissionType(i))
									{
										isMissionBiz = true;
									}
								}
							} 
							//						if((teleport.DstSceneID == 8 || teleport.DstSceneID == 7 ) && 跑商任务)
							
							if(teleport.DstSceneID ==(int)GameDefine_Globe.SCENE_DEFINE.SCENE_WUDAOZHIDIAN || teleport.DstSceneID == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_TIANSHAN&& isMissionBiz)
							{
								MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{2672}"), "",FuncOKClicked,FunccCanCalClicked);//"注意！即将进入的场景PK不会增加残暴！是否继续？"
								m_bIsCancel = true;
								return;
							}

							SendChangeScene(teleport);

						}
						//===================end   
                    }                   
                }               
            } 
            else
            {
				//====超出传送点距离关闭弹窗
				if(m_bIsCancel)
					MessageBoxLogic.CloseBox();

                m_bEnterPKSceneCancel = false;
				m_bIsCancel = false;

            }
        }
    }

	void FuncOKClicked()
	{
		//根据ID从表格中获取信息
		Tab_Teleport teleport = TableManager.GetTeleportByID(TeleportID, 0);
		SendChangeScene(teleport);
	}
	void FunccCanCalClicked()
	{
		m_bIsCancel = true;
	}

    void EnterNonePKValueSceneOK()
    {
        if (m_tabTeleport != null)
        {
            SendChangeScene(m_tabTeleport);
            m_tabTeleport = null;
            m_bEnterPKSceneCancel = false;
        }
    }

    void EnterNonePKValueSceneCancel()
    {
        m_bEnterPKSceneCancel = true;
    }

    void SendChangeScene(Tab_Teleport teleport)
    {

        //向服务器发起切场景请求
        if (GameManager.gameManager.OnLineState)
        {
            //自动寻路处理
            GameManager.gameManager.AutoSearch.ProcessTelepoint(this);

			SceneData.RequestChangeScene((int)CG_REQ_CHANGE_SCENE.CHANGETYPE.TELEPORT, teleport.Id,0, 0);
            m_bValid = false;
            m_fLastInvaildTime = Time.time;//记录下失效时间点
        }
        else
        {
            //自动寻路处理
            GameManager.gameManager.AutoSearch.ProcessTelepoint(this);
            LoadingWindow.LoadScene((GameDefine_Globe.SCENE_DEFINE)teleport.DstSceneID);
        }
    }

    public void SendOpenCopyScene()
    {
        Tab_Teleport teleport = TableManager.GetTeleportByID(TeleportID, 0);
        if (null == teleport)
        {
            return;
        }
        if (GameManager.gameManager.PlayerDataPool.CopySceneChange) //正在传送中
        {
            return;
        }
        GameManager.gameManager.PlayerDataPool.CopySceneChange = true;
        //进入下个副本
        CG_OPEN_COPYSCENE packet = (CG_OPEN_COPYSCENE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_OPEN_COPYSCENE);
        packet.SceneID = teleport.DstSceneID;
        packet.Type = 1;
        packet.Difficult = 1;
        packet.EnterType = 2;   //传送点进入
        packet.SendPacket();
    }
    public void SendLeaveCopyScene()
    {
        // 副本场景直接发包返回副本前场景 不查表
        CG_LEAVE_COPYSCENE packet = (CG_LEAVE_COPYSCENE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_LEAVE_COPYSCENE);
        packet.NoParam = -1;
        packet.SendPacket();
    }
    public void CloseMessageBox()
    {

    }

    public static bool IsCanPK(int nSceneClassID)
    {
        Tab_SceneClass tabSceneClass = TableManager.GetSceneClassByID(nSceneClassID, 0);
        if (tabSceneClass != null)
        {
            int nPVPRule = tabSceneClass.PVPRule;
            Tab_PVPRule tabRule = TableManager.GetPVPRuleByID(nPVPRule, 0);
            if (tabRule != null)
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsIncPKValue(int nSceneClassID)
    {
        Tab_SceneClass tabSceneClass = TableManager.GetSceneClassByID(nSceneClassID, 0);
        if (tabSceneClass != null)
        {
            int nPVPRule = tabSceneClass.PVPRule;
            Tab_PVPRule tabRule = TableManager.GetPVPRuleByID(nPVPRule, 0);
            if (tabRule != null)
            {
                return tabRule.IsIncPKValue == 1 ? true : false;
            }
        }
        return false;
    }
}
