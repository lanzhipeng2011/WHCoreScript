//********************************************************************
// 文件名: StoryDialogLogic.cs
// 描述: 剧情任务界面
// 作者: WangZhe
// 创建时间: 2013-11-14
//
// 修改历史:
// 2013-11-14 王喆创建 拆分prefab 分离UI逻辑
//********************************************************************

using UnityEngine;
using System.Collections;
using GCGame.Table;
using Games.Mission;
using Games.GlobeDefine;
using Games.LogicObj;
public class StoryDialogLogic : MonoBehaviour {

    private static StoryDialogLogic m_Instance = null;
    public static StoryDialogLogic Instance()
    {
        return m_Instance;
    }

    //剧情对话控件
    public GameObject m_StoryDialogRoot;
    public GameObject m_StoryDialogTop;
    public GameObject m_StoryDialogBottom;

    public UISprite m_StoryDialogTopBG;             //顶部背景
    public UILabel m_StoryDialogTopLabel;           //顶部对话内容
    public UISprite m_StoryDialogTopRole;           //顶部人物头像
 
    public UISprite m_StoryDialogBottomBG;        //底部背景
    public UILabel m_StoryDialogBottomLabel;      //底部对话内容
    public UITexture m_StoryDialogBottomRole;      //底部人物头像
    public UILabel m_SpeakerName;                  // 对话姓名

    public System.Action DialogShowOver;
    private short m_CurSotryIndex;             //当前任务剧情index

    private int m_StoryID;                      //剧情ID
    public int StoryID{ get{return m_StoryID;} }

    private int m_StoryMissionID;               //任务ID

    private string[] m_MainPlayerHalfPic = { "Player_JK", "Player_CK", "Player_QK", "Player_PS" };

    private bool m_bAniDialog = false;          // 是否在动态显示对话文字中
    private float m_fAniDialogTimer = 0;        // 对话文字显示计时器
    private string m_strAniDialog = "";         // 对话全部文字内容
    private int m_nAniDialogTextIndex = 0;      // 对话文字索引
    private bool m_bAniDialogTopUI = false;     // 是否在顶部UI显示 目前一直为false
    public float m_AniDialogTime = 0.05f;       // 文字显示时间间隔

    private float m_NextPageTimer = 0;      // 剧情翻页 关闭计时
	public static GameObject  m_npc = null;
    public static bool ShowStory(int storyID)
    {
        UIManager.ShowUI(UIInfo.StoryDialogRoot, OnShowStory, storyID);

		return true;
	}
	
	static void OnShowStory(bool bSuccess, object storyID)
    {
        int nStoryID = (int)storyID;

        if (null != m_Instance)
        {
            Singleton<ObjManager>.Instance.MainPlayer.IsInModelStory = IsNeedSilentMode(nStoryID);

            if (m_Instance.PlayStory(nStoryID))
            {
			
                return;
            }
            else
            {
                UIManager.CloseUI(UIInfo.StoryDialogRoot);
				CameraController  cam=ObjManager.Instance.MainPlayer.CameraController;
				if(cam!=null)
				{
					cam.EndStory();
				}

                return;
            }
        }
    }

    void Awake()
    {
        m_Instance = this;
    }

	// Use this for initialization
	void Start () {
        InitBackgroundWidth();
        m_StoryDialogBottom.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	    if (m_bAniDialog)
	    {
            UpdateAniDialog();
	    }	    
	}

    void FixedUpdate()
    {
        AutoNextStory();
    }

    void OnDestroy()
    {
        m_Instance = null;
    }

	/// <summary>
	/// 初始化剧情对话UI 适应屏幕
	/// </summary>
	void InitStoryDialog()
	{
        CleanUp();
	}

	// 清理数据
	void CleanUp()
	{
		m_CurSotryIndex = 0;
		m_StoryID = -1;
		m_StoryMissionID = -1;
        m_NextPageTimer = 0;
	}
    /// <summary>
    /// start Story
    /// </summary>
    public void AutoBeginStory()
    {
        if (m_StoryID >= 0 && m_CurSotryIndex >=0)
        {
            Tab_StoryTable storyTable = TableManager.GetStoryTableByID(m_StoryID, 0);
            if (storyTable == null)
            {
                return;
            }

            Tab_ClientStoryTable tab = TableManager.GetClientStoryTableByID(storyTable.ClientStoryID, m_CurSotryIndex);
            if (tab != null)
            {
                ShowStoryUI(false, tab.SpriteName, StrDictionary.GetClientString_WithNameSex(tab.Story), tab.SpeakerID);
            }
            else
            {
                if (DialogShowOver != null) 
                {
                    DialogShowOver();
                }
                // 对话完成 设置剧情任务 状态
                if (m_StoryMissionID > -1)
                {
                    bool nRet = GameManager.gameManager.MissionManager.SetStoryMissionState(m_StoryMissionID, 2);
                    if (nRet)
                    {
                        if ((int)MISSIONTYPE.MISSION_MAIN == GameManager.gameManager.MissionManager.GetMissionType(m_StoryMissionID)
                            || (int)MISSIONTYPE.MISSION_BRANCH == GameManager.gameManager.MissionManager.GetMissionType(m_StoryMissionID))
                        {
                            MissionInfoController.ShowMissionDialogUI(m_StoryMissionID);
                        }
                    }
                }
                else
                {
                    if (storyTable.ScriptID > -1)
                    {
                        // 雁门关轻功后对话特殊情况 不再发包了直接在MainPlayer.OnPlayStoryOver进入打斗动画
                        if (m_StoryID != GlobeVar.YanMenGuan_BossStory1ID)
                        {
                            // 非任务剧情操作
                            CG_PLAYSTORY_OVER packet = (CG_PLAYSTORY_OVER)PacketDistributed.CreatePacket(MessageID.PACKET_CG_PLAYSTORY_OVER);
                            packet.SetStoryID(m_StoryID);
                            packet.SendPacket();
                        }
                    }

                    if (Singleton<ObjManager>.GetInstance().MainPlayer != null)
                        Singleton<ObjManager>.GetInstance().MainPlayer.OnPlayStoryOver(m_StoryID);
                }

				// 清理数据
				CleanUp();
				CameraController  cam=ObjManager.Instance.MainPlayer.CameraController;
				if(cam!=null)
				{
					cam.EndStory();
				}
                UIManager.CloseUI(UIInfo.StoryDialogRoot);
            }
        }
    }

    /// <summary>
    /// 显示对话
    /// </summary>
    void ShowNextStory()
    {
        if (m_bAniDialog)
        {
            // 如果动态对话未完全显示 那么本次点击只显示对话的全部内容 不进入下一句对话
            ShowAniDialogEnd();
        }
        else
        {
            m_CurSotryIndex++;
            AutoBeginStory();
        }       
    }

    // 自动翻页
    void AutoNextStory()
    {
        if (false != m_bAniDialog)
        {
            return;
        }

        Tab_StoryTable StoryTab = TableManager.GetStoryTableByID(m_StoryID, 0);
        if (null == StoryTab)
        {
            return;
        }

        Tab_ClientStoryTable StoryItem = TableManager.GetClientStoryTableByID(StoryTab.ClientStoryID, m_CurSotryIndex);
        if (null == StoryItem)
        {
            return;
        }
        if (m_NextPageTimer > StoryItem.NextPageTime)
        {
            m_NextPageTimer = 0;
            ShowNextStory();
            return;
        }
        else
        {
            m_NextPageTimer += Time.deltaTime;
        }
    }

    /// <summary>
    /// 设置对话UI信息
    /// </summary>
    /// <param name="bTopUI">是否为顶部UI</param>
    /// <param name="strRolePicName">角色头像图片名</param>
    /// <param name="strDialog">对话内容</param>
    void ShowStoryUI(bool bTopUI, string strRolePicName, string strDialog, int nDataID)
    {
        //InitAniDialog(strDialog, bTopUI);
        // 暂时不手动设置altas
        if (bTopUI)
        {
            m_StoryDialogTop.SetActive(true);
            m_StoryDialogTopRole.spriteName = strRolePicName;
            // 对话内容交由动态显示设置
            // 又让改回去了
            m_StoryDialogTopLabel.text = strDialog;
        }
        else
        {
            m_StoryDialogBottom.SetActive(true);
            // 对话内容交由动态显示设置
            // 又让改回去了
            m_StoryDialogBottomLabel.text = strDialog;
            if (-1 == nDataID)
            {
                string strSpeakerName = "";
                int nProfession = 0;

                if (Singleton<ObjManager>.GetInstance().MainPlayer != null)
                {
                    strSpeakerName = Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.RoleName;
                    nProfession = Singleton<ObjManager>.GetInstance().MainPlayer.Profession;
                }
                else
                {
                    strSpeakerName = GameManager.gameManager.PlayerDataPool.MainPlayerBaseAttr.RoleName;
                    nProfession = GameManager.gameManager.PlayerDataPool.EnterSceneCache.Profession;
                }

                m_SpeakerName.text = strSpeakerName;

                if (null != m_MainPlayerHalfPic[nProfession])
                {
                    //m_StoryDialogBottomRole.spriteName = m_MainPlayerHalfPic[nProfession];

                    m_StoryDialogBottomRole.mainTexture = ResourceManager.LoadResource("Texture/MissionRole/" + m_MainPlayerHalfPic[nProfession], typeof(Texture)) as Texture; 
                }
            }
            else
            {
                Tab_RoleBaseAttr RoleBase = TableManager.GetRoleBaseAttrByID(nDataID, 0);
                if (RoleBase != null)
                {
                    m_SpeakerName.text = RoleBase.Name;
                }
                else
                    m_SpeakerName.text = "";
				//获取角色头像
                m_StoryDialogBottomRole.mainTexture = ResourceManager.LoadResource("Texture/MissionRole/" + strRolePicName, typeof(Texture)) as Texture; 
                //m_StoryDialogBottomRole.spriteName = strRolePicName;
            }
        }
    }

    // 播放剧情
    public bool PlayStory(int nStoryID)
    {
        // 如果正在键盘输入 隐藏键盘
        if (ChatInfoLogic.Instance() != null)
        {
            ChatInfoLogic.Instance().HideKeyboard();
        }
        if (ProcessInput.Instance() != null)
        {
            ProcessInput.Instance().ReleaseTouch();
        }
        if (JoyStickLogic.Instance() != null)
        {
            JoyStickLogic.Instance().ReleaseJoyStick();
        }        
        InitStoryDialog();
        if (nStoryID < 0)
        {
            return false;
        }
        Tab_StoryTable StoryTable = TableManager.GetStoryTableByID(nStoryID, 0);
        if (StoryTable == null)
        {
            return false;
        }

        m_StoryMissionID = StoryTable.MissionID;
        if (m_StoryMissionID > -1)
        {
            // 任务状态判断
            MissionState misState = (MissionState)GameManager.gameManager.MissionManager.GetMissionState(m_StoryMissionID);
            if (MissionState.Mission_Accepted != misState)
            {
                return false;
            }
        }

        // NPC距离判断
        Tab_ClientStoryTable StoryLine = TableManager.GetClientStoryTableByID(StoryTable.ClientStoryID, 0);
        if (StoryLine == null)
        {
            return false;
        }
	
		if (StoryLine.SpeakerID == -1) 
		{
			StoryLine = TableManager.GetClientStoryTableByID(StoryTable.ClientStoryID, 1);
			if (StoryLine == null)
			{
				return false;
			}
		}
		
		Tab_RoleBaseAttr RoleBase = TableManager.GetRoleBaseAttrByID(StoryLine.SpeakerID, 0);

		string name;
		if (RoleBase != null)
		{
			if(StoryLine.IsNeedAni==1)
			{
			name = RoleBase.Name;
			Obj_Character obj = Singleton<ObjManager>.GetInstance().FindObjCharacterInSceneByName(name);


			CameraController main=ObjManager.Instance.MainPlayer.CameraController;
			if(main!=null&&obj!=null)
			{
				main.InitStory(obj.gameObject.transform);
				
			}
			}

		}

        // 全填-1的时候不检测距离
        if (StoryLine.TargetPosX > -1 && StoryLine.TargetPosZ > -1 && StoryLine.TargetPosRadius > -1)
        {
            Vector3 userPos = new Vector3(0, 0, 0);
            if (Singleton<ObjManager>.GetInstance().MainPlayer != null)
            {
                userPos = Singleton<ObjManager>.GetInstance().MainPlayer.Position;
            }
            else
            {
                userPos = GameManager.gameManager.PlayerDataPool.EnterSceneCache.EnterScenePos;
            }
            
            Vector3 TargetPos = new Vector3(StoryLine.TargetPosX, 0, StoryLine.TargetPosZ);
            TargetPos.y = userPos.y;
		
            float dis = Mathf.Abs(Vector3.Distance(userPos, TargetPos));
            if (dis > StoryLine.TargetPosRadius)
            {
                return false;
            }
        }

        UIManager.ShowUI(UIInfo.StoryDialogRoot);
        
        m_StoryID = nStoryID;
        AutoBeginStory();
        if (Singleton<ObjManager>.GetInstance().MainPlayer != null)
            Singleton<ObjManager>.GetInstance().MainPlayer.OnStartPlayStory(m_StoryID);

        return true;
    }

    void InitBackgroundWidth()
    {
        /*// iphone5
        if (Screen.width % 1136 == 0)
        {
            m_StoryDialogTopBG.GetComponent<UIWidget>().width = 1346;
            m_StoryDialogBottomBG.GetComponent<UIWidget>().width = 1346;
        }
        // ipad
        else if (Screen.height % 768 == 0)
        {
            m_StoryDialogTopBG.GetComponent<UIWidget>().width = 1034;
            m_StoryDialogBottomBG.GetComponent<UIWidget>().width = 1034;
        }
        // iphone4
        else
        {
            m_StoryDialogTopBG.GetComponent<UIWidget>().width = 1032;
            m_StoryDialogBottomBG.GetComponent<UIWidget>().width = 1032;
        }*/
        m_StoryDialogTopBG.GetComponent<UIWidget>().width = UIRootAdapter.GetLogicWidth() + 100;
        m_StoryDialogBottomBG.GetComponent<UIWidget>().width = UIRootAdapter.GetLogicWidth() + 100;
    }

    /// <summary>
    /// 初始化动态对话
    /// </summary>
    /// <param name="text">文字内容</param>
    /// <param name="bTopUI">是否在顶部显示 目前一直是false</param>
    void InitAniDialog(string text, bool bTopUI)
    {
        m_StoryDialogTopLabel.text = "";
        m_StoryDialogBottomLabel.text = "";
        m_strAniDialog = text;
        m_fAniDialogTimer = Time.fixedTime;
        m_nAniDialogTextIndex = 0;
        m_bAniDialogTopUI = bTopUI;
        m_bAniDialog = true;
    }

    void UpdateAniDialog()
    {
        if (Time.fixedTime - m_fAniDialogTimer > m_AniDialogTime)
        {
            m_fAniDialogTimer = Time.fixedTime;

            // 如果有颜色 需要先解析颜色
            if (m_strAniDialog[m_nAniDialogTextIndex] == '[')
            {
                string strColor = GetColorSymbols(ref m_nAniDialogTextIndex);
                if (m_bAniDialogTopUI)
                {
                    m_StoryDialogTopLabel.text += strColor;
                }
                else
                {
                    m_StoryDialogBottomLabel.text += strColor;
                }
            }
            
            // 显示下一文字
            if (m_bAniDialogTopUI)
            {
                m_StoryDialogTopLabel.text += m_strAniDialog[m_nAniDialogTextIndex];
            }
            else
            {
                m_StoryDialogBottomLabel.text += m_strAniDialog[m_nAniDialogTextIndex];
            }            

            if (m_nAniDialogTextIndex < m_strAniDialog.Length - 1)
            {
                m_nAniDialogTextIndex++;
            } 
            else
            {
                m_strAniDialog = "";
                m_nAniDialogTextIndex = 0;
                m_bAniDialogTopUI = false;
                m_bAniDialog = false;
            }
        }
    }

    string GetColorSymbols(ref int nBeginIndex)
    {
        string strColor = "";
        if (m_strAniDialog[nBeginIndex + 7] != ']')
        {
            return "";
        }

        for (int i = nBeginIndex; i < nBeginIndex + 8; i++)
        {
            strColor += m_strAniDialog[i];            
        }
        nBeginIndex = nBeginIndex + 8;
        return strColor;
    }

    void ShowAniDialogEnd()
    {
        if (m_bAniDialogTopUI)
        {
            m_StoryDialogTopLabel.text = m_strAniDialog;
        }
        else
        {
            m_StoryDialogBottomLabel.text = m_strAniDialog;
        }
        m_strAniDialog = "";
        m_nAniDialogTextIndex = 0;
        m_bAniDialogTopUI = false;
        m_bAniDialog = false;
    }
    //是否进入安静模式，即只能看电视，不能点击
    public static bool IsNeedSilentMode(int storyid)
    {
        if (GlobeVar.MARRY_PARADE_STORY == storyid)
        {
            return true;
        }
        return false;
    }

    public void ShowStory()
    {
        m_StoryDialogBottom.gameObject.SetActive(true);
    }
}
