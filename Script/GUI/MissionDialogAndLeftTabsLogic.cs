//********************************************************************
// 文件名: MissionDialogAndLeftTabsLogic.cs
// 全路径：	\Script\GUI\MissionDialogAndLeftTabsLogic.cs
// 描述: 任务、对话，左侧tabs逻辑（组队等）处理
// 作者: wanghua
// 创建时间: 2013-11-14
//
// 修改历史:
// 2013-11-14 王喆创建 拆分prefab 分离UI逻辑
//********************************************************************
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GCGame.Table;
using Games.Mission;
using Games.Events;
using Games.LogicObj;
using Module.Log;
using GCGame;
using Games.GlobeDefine;

public class MissionDialogAndLeftTabsLogic : MonoBehaviour
{
    private static MissionDialogAndLeftTabsLogic m_Instance = null;
    public static MissionDialogAndLeftTabsLogic Instance()
    {
        return m_Instance;
    }

    //左侧任务追踪信息
    public UIGrid m_MissionsList;
    public UITopGrid m_MissionsTopList;
    private List<MissionItemLogic> m_MissionItemsList; // 任务Item gameobj
    private int m_CommpletedItemCount = 0; // 完成未提交任务个数

    public GameObject m_ControlButton;			//任务追踪收起按钮

    public GameObject m_LeftTablesRoot;     //左侧分页，任务快捷、组队相关
    public GameObject m_LeftTabsOffset;     // 左侧tabs位移

    public UIImageButton m_TabMission;            // 任务标签sprite
    public UIImageButton m_TabTeam;               // 队伍标签sprite
    public GameObject m_ContentMission;
    public GameObject m_ContentTeam;

    private bool m_TabsFold = false;                // ControlButton控制的折叠状态 默认不折叠
    private bool m_bFold = false;                     // 头像按钮控制的折叠状态 默认不折叠

    // 新手指引相关
    private int[] m_GuideMissionID = {0, 1, 2, 99, 4, 5, 6, 8, 100,102 };
    private bool m_GuideMissionFlag = false;

    public TweenPosition m_TabTweenPos;
    public TweenPosition m_ContentTweenPos;
    public UIDraggablePanel m_dragPanel;

    public GameObject m_TeamButton;
    public GameObject m_LeaveTeamButton;

    //public List<TweenAlpha> m_FoldTween = new List<TweenAlpha>();

    void Awake()
    {
        m_Instance = this;
       if (m_dragPanel != null)
       {
           m_dragPanel.repositionClipping = true;
       }
    }

    // Use this for initialization
    void Start()
    {
        m_LeftTablesRoot.SetActive(true);
       
        m_MissionItemsList = new List<MissionItemLogic>();
        
        InitMissionFollow();

        if (PlayerPreferenceData.LeftTabChoose == 1)
        {
            if (GameManager.gameManager.PlayerDataPool.TeamInfo.TeamID >= 0)
            {
                SwitchToTeam();
            }            
        }
        else
        {
            SwitchToMission();
        }

        InitLeftTabControl(PlayerPreferenceData.LeftTabControl);
        ShowNewPlayerGuide(1);
    }

    void OnDestroy()
    {
        GameManager.gameManager.PlayerDataPool.MissionGridLastPos = m_MissionsList.transform.position;
        m_Instance = null;
    }

    // 更新所有任务UI
    public void InitMissionFollow()
    {
        UIManager.LoadItem(UIInfo.MissionItem, OnLoadMissionItem);
    }

    void OnLoadMissionItem(GameObject resItem, object param)
    {
        // 清空 MissionFollow
        ClearUpMissionFollow();

        // 添加任务
        List<int> nMissionIDList = GameManager.gameManager.MissionManager.GetAllMissionID();
        int nMissionCount = nMissionIDList.Count;
        if (nMissionCount <= 0)
        {
            return;
        }

        List<int> nMissionSortList = GameManager.gameManager.PlayerDataPool.MissionSortList;
        if (nMissionSortList.Count > 0 && nMissionSortList.Count == nMissionCount)
        {
            InitMissionItemList(nMissionSortList, resItem);
        }
        else
        {
            for (int i = 0; i < nMissionIDList.Count; ++i)
            {
                if (false == AddMissionItem(nMissionIDList[i], resItem))
                {
                    continue;
                }

                if (false == UpdateMissionItem(nMissionIDList[i]))
                {
                    continue;
                }
            }

            for (int i = 0; i < m_MissionItemsList.Count;++i)
            {
                nMissionSortList.Add(m_MissionItemsList[i].MissionID);
            }
        }

        if (m_MissionsList)
        {
            m_MissionsList.repositionNow = true;
            InitMissionScrollViewPos();
        }

        ShowNewPlayerGuide(1); // 第一个任务
    }
    // 清空 MissionFollow 
    public void ClearUpMissionFollow()
    {
        Utils.CleanGrid(m_MissionsList.gameObject);
        m_MissionItemsList.Clear();
        m_CommpletedItemCount = 0;
    }

    string GetColorByQuality(byte yQuality)
    {
        string strColor = "";
        MISSION_QUALITY quality = (MISSION_QUALITY)yQuality;
        switch (quality)
        {
            case MISSION_QUALITY.MISSION_QUALITY_WHITE:
                strColor = "[ffffff]";
                break;
            case MISSION_QUALITY.MISSION_QUALITY_GREEN:
                strColor = "[1fff1f]";
                break;
            case MISSION_QUALITY.MISSION_QUALITY_BLUE:
                strColor = "[2c97f1]";
                break;
            case MISSION_QUALITY.MISSION_QUALITY_PURPLE:
                strColor = "[b040f7]";
                break;
            case MISSION_QUALITY.MISSION_QUALITY_ORANGE:
                strColor = "[df8e24]";
                break;
            default:
                strColor = "[ffffff]";
                break;
        }

        return strColor;
    }

    // 创建MissionItem
    bool AddMissionItem(int nMissionID, GameObject resItem)
    {
        if (nMissionID < 0)
        {
            return false;
        }

        Tab_MissionDictionary MissionDic = TableManager.GetMissionDictionaryByID(nMissionID, 0);
        if (MissionDic == null)
        {
            return false;
        }

        if (m_CommpletedItemCount < 0)
        {
            return false;
        }

        for (int i = 0; i < m_MissionItemsList.Count; ++i)
        {
            if (null != m_MissionItemsList[i])
            {
                if (m_MissionItemsList[i] && m_MissionItemsList[i].MissionID == nMissionID)
                {
                    return false;
                }
            }
        }

        GameObject ItemObj = Utils.BindObjToParent(resItem,m_MissionsList.gameObject,"MissionItem-1");
        if (ItemObj)
        {
            MissionItemLogic MissionItem = ItemObj.GetComponent<MissionItemLogic>();
            if (MissionItem == null)
            {
                return false;
            }

            byte yMissionQuality = GameManager.gameManager.MissionManager.GetMissionQuality(nMissionID);
            string strMissionColor = GetColorByQuality(yMissionQuality);

            MissionItem.MissionID = nMissionID;
            MissionItem.MissionTile.text = string.Format(MissionDic.MissionName, strMissionColor, "[ffffff]");
            MissionItem.MissionInfo.text = "[ffffff]" + string.Format(MissionDic.FollowText, "[ffffff]", "0");
            //MissionItem.MissionInfo.text = "[ffe6b4]" + string.Format(MissionDic.FollowText, "[fe3737]", "0");
            
            // 主线任务放在接任务最上面
            int nTempIndex = m_CommpletedItemCount;
            if (nTempIndex >=0 && nTempIndex < m_MissionItemsList.Count
                && (int)MISSIONTYPE.MISSION_MAIN == GameManager.gameManager.MissionManager.GetMissionType(m_MissionItemsList[nTempIndex].MissionID))
            {
                nTempIndex++;
            }
            m_MissionItemsList.Insert(nTempIndex, MissionItem);

            for (int nIndex = 0; nIndex < m_MissionItemsList.Count; nIndex++)
            {
                m_MissionItemsList[nIndex].name = "MissionItem" + nIndex;
            }
        }

        return true;
    }

    bool DelMissionItem(int nMissionID)
    {
        if (nMissionID < 0)
        {
            return false;
        }

        int nIndex = -1;
        for (int i = 0; i < m_MissionItemsList.Count; i++ )
        {
            int ItemMissionID = m_MissionItemsList[i].MissionID;
            if (ItemMissionID == nMissionID)
            {
                nIndex = i;
                break;
            }
        }

        // 没任务
        if (nIndex < 0 || nIndex >= m_MissionItemsList.Count)
        {
            return false;
        }

        GameObject tempObj = m_MissionItemsList[nIndex].gameObject;
        tempObj.transform.parent = null;
        m_MissionItemsList.RemoveAt(nIndex);
        ResourceManager.DestroyResource(ref tempObj); // 直接删就可以里吧

        m_MissionsTopList.recenterTopNow = true;

        // 更新 其他Item名字
        for (; nIndex < m_MissionItemsList.Count; nIndex++ )
        {
            m_MissionItemsList[nIndex].name = "MissionItem" + nIndex;
		}

        if (GameManager.gameManager.MissionManager.IsHaveMission(nMissionID)
            && (byte)MissionState.Mission_Completed == GameManager.gameManager.MissionManager.GetMissionState(nMissionID))
        {
            if (m_CommpletedItemCount > 0)
            {
                m_CommpletedItemCount -= 1;
            }
        }

        return true;
    }

    bool UpdateMissionItem(int nMissionID)
    {
        if (nMissionID < 0)
        {
            return false;
        }

        int nIndex = -1;
        for (int i = 0; i < m_MissionItemsList.Count; i++)
        {
            int ItemMissionID = m_MissionItemsList[i].MissionID;
            if (ItemMissionID == nMissionID)
            {
                nIndex = i;
                break;
            }
        }
        // 没任务
        if (nIndex < 0 || nIndex >= m_MissionItemsList.Count)
        {
            return false;
        }
        Tab_MissionDictionary MDLine = TableManager.GetMissionDictionaryByID(nMissionID, 0);
        if (MDLine != null)
        {
            int nParam = GameManager.gameManager.MissionManager.GetMissionParam(nMissionID, 0);
            if (nParam != 0)
            {
                m_MissionItemsList[nIndex].MissionInfo.text = "[ffffff]" + string.Format(MDLine.FollowText, "[ffffff]", nParam);
                //m_MissionItemsList[nIndex].MissionInfo.text = "[ffe6b4]" + string.Format(MDLine.FollowText, "[fe3737]", nParam);
            }
         }

        MissionState misState = (MissionState)GameManager.gameManager.MissionManager.GetMissionState(nMissionID);
        if (MissionState.Mission_Completed == misState)
        {
            //string StrMissionTile = MDLine.MissionName;//m_MissionItemsList[nIndex].GetComponent<MissionItemLogic>().MissionTile.text;//StrDictionary.GetClientDictionaryString("#{1361}") + m_MissionItemsList[nIndex].GetComponent<MissionItemLogic>().MissionTile.text;
            m_MissionItemsList[nIndex].MissionTile.text = string.Format(MDLine.MissionName, "[1fff1f]", "[1fff1f]");

            int nParam = GameManager.gameManager.MissionManager.GetMissionParam(nMissionID, 0);
            m_MissionItemsList[nIndex].MissionInfo.text = "[1fff1f]" + string.Format(MDLine.FollowText, "[1fff1f]", nParam);
           
            UpdateMissionFollowBlink(nIndex, true);

            // 移位
            MissionItemLogic TempObj = m_MissionItemsList[nIndex];
            m_MissionItemsList.RemoveAt(nIndex);
            m_MissionItemsList.Insert(0, TempObj);
            for (int i = 0; i < m_MissionItemsList.Count; i++)
            {
                m_MissionItemsList[i].name = "MissionItem" + i;
            }
            m_CommpletedItemCount += 1;

            // 添加新手指引
            if (nMissionID == 2 || nMissionID == 7)
            {
                m_GuideMissionFlag = true;
                ShowNewPlayerGuide(nMissionID);
            }
			Obj_MainPlayer main = Singleton<ObjManager>.GetInstance ().MainPlayer;
			if (main) 
			{
				if(main.AutoComabat)
				{
					main.LeveAutoCombat ();
					if(SGAutoFightBtn.Instance!=null)
					{
						SGAutoFightBtn.Instance.UpdateAutoFightBtnState();
					}
				}
			}
       }
        else
        {
            string StrMissionTile = MDLine.MissionName;
            if (MissionState.Mission_Failed == misState)
            {
                StrMissionTile = StrMissionTile + "  [fe3737]" + StrDictionary.GetClientDictionaryString("#{1362}");
            }

            byte yMissionQuality = GameManager.gameManager.MissionManager.GetMissionQuality(nMissionID);
            string strMissionColor = GetColorByQuality(yMissionQuality);
            m_MissionItemsList[nIndex].MissionTile.text = string.Format(StrMissionTile, strMissionColor, "[ffffff]");//"[fe3737]" + StrMissionTile;
   
            UpdateMissionFollowBlink(nIndex, false);
        }

        return true;
    }

    // 更新任务信息UI操作
    public void UpDateMissionFollow(int nMissionID, string strOpt)
    {
        
        switch (strOpt)
        {
            case "add":
                {
                    UIManager.LoadItem(UIInfo.MissionItem, OnUpDateMissionFollowLoadItem, nMissionID);
                    return;
                }
                break;
            case "Del":
                {
                    DelMissionItem(nMissionID);
                }
                break;
            case "state":
                {
                    UpdateMissionItem(nMissionID);
                }
                break;
            default:
                break;
        }

        if (m_MissionsList)
        {
            m_MissionsList.repositionNow = true;
            UpDateMissionSortLst();
        }
    }

    void OnUpDateMissionFollowLoadItem(GameObject resItem, object info)
    {
        if (resItem == null)
        {
            LogModule.ErrorLog("load missionitem fail");
            return;
        }

        int missionID = (int)info;

        AddMissionItem(missionID, resItem);
        UpdateMissionItem(missionID);
        //UIManager.CloseUI(UIInfo.MissionInfoController);
        m_GuideMissionFlag = true;
        if (m_MissionsList)
        {
            m_MissionsList.repositionNow = true;
            UpDateMissionSortLst();
        }

        ShowNewPlayerGuide(missionID);
    }

    void ShowNewPlayerGuide(int nMissionID)
    {
        if (nMissionID == 1)
        {
            m_GuideMissionFlag = true;
        }

        if (false == m_GuideMissionFlag)
        {
            return;
        }

        m_GuideMissionFlag = false;

        bool bRet = false;
        for (int i = 0; i < m_GuideMissionID.Length; i++)
        {
            if (nMissionID == m_GuideMissionID[i])
            {
                bRet = true;
                break;
            }
        }

        if (false == bRet)
        {
            return;
        }

        for (int i = 0; i < m_MissionItemsList.Count; ++i)
        {
            if (null != m_MissionItemsList[i])
            {
                if (0 == PlayerPreferenceData.LeftTabControl
                    && m_bFold == false
                    && nMissionID == m_MissionItemsList[i].MissionID)
                {
                    NewPlayerGuidLogic.OpenWindow(m_MissionItemsList[i].gameObject, 110, 110, "", "right", 2);
                }
            }
        }
        if (m_MissionsTopList)
        {
            m_MissionsTopList.recenterTopNow = true;
        }
    }

    public void PlayTween(bool nDirection)
    {
        //BeforeClickPlayerFrame(nDirection);
        m_bFold = nDirection;

        gameObject.SetActive(!nDirection);
        //m_LeftTabsOffset.GetComponent<UIPlayTween>().Play(nDirection);
        //foreach (TweenAlpha tween in m_FoldTween)
        //{
        //    tween.Play(nDirection);
        //}
    }

    public void SwitchToTeam()
    {
        //if (GameManager.gameManager.PlayerDataPool.TeamInfo.TeamID >= 0)
        //{
        m_ContentMission.SetActive(false);
        m_ContentTeam.SetActive(true);

        m_TabMission.normalSprite = "ui_main_03";
        m_TabMission.hoverSprite = "ui_main_02";
        m_TabMission.pressedSprite = "ui_main_02";
        m_TabMission.target.spriteName = "ui_main_03";
        m_TabMission.target.MakePixelPerfect();
        m_TabTeam.normalSprite = "ui_main_02";
        m_TabTeam.hoverSprite = "ui_main_02";
        m_TabTeam.pressedSprite = "ui_main_02";
        m_TabTeam.target.spriteName = "ui_main_02";
        m_TabTeam.target.MakePixelPerfect();

        PlayerPreferenceData.LeftTabChoose = 1;

        if (null != TeamList.Instance())
        {
            TeamList.Instance().UpdateTeamMember();
        }
        if (m_TeamButton != null)
        {
            if (GameManager.gameManager.PlayerDataPool.TeamInfo.TeamID >= 0)
            {
                m_TeamButton.SetActive(false);
                if (GameManager.gameManager.PlayerDataPool.TeamInfo.GetTeamMemberCount() == 1)
                {
                    m_LeaveTeamButton.SetActive(true);
                }
                else
                {
                    m_LeaveTeamButton.SetActive(false);
                }
            }
            else
            {
                m_TeamButton.SetActive(true);
                m_LeaveTeamButton.SetActive(false);
            }
        }
        //}
    }

    public void SwitchToMission()
    {
        m_ContentMission.SetActive(true);
        m_ContentTeam.SetActive(false);

        m_TabMission.normalSprite = "ui_main_02";
        m_TabMission.hoverSprite = "ui_main_02";
        m_TabMission.pressedSprite = "ui_main_02";
        m_TabMission.target.spriteName = "ui_main_02";
        m_TabMission.target.MakePixelPerfect();
        m_TabTeam.normalSprite = "ui_main_03";
        m_TabTeam.hoverSprite = "ui_main_02";
        m_TabTeam.pressedSprite = "ui_main_02";
        m_TabTeam.target.spriteName = "ui_main_03";
        m_TabTeam.target.MakePixelPerfect();

        PlayerPreferenceData.LeftTabChoose = 0;
    }

    void UpdateMissionFollowBlink(int nIndex, bool enabled)
    {
        if (nIndex < 0 || nIndex > m_MissionItemsList.Count)
        {
            return;
        }

        m_MissionItemsList[nIndex].UpdateMissionFollowBlink(enabled);
    }

    public void AfterClickPlayerFrame()
    {
        TweenAlpha[] alphaArray = m_LeftTabsOffset.GetComponentsInChildren<TweenAlpha>();
        for (int i=0; i<alphaArray.Length; ++i)
        {
            if (alphaArray[i].tweenGroup == 2)
            {
                if (alphaArray[i].enabled)
                {
                    alphaArray[i].enabled = false;
                }
            }
        }

        if (!m_bFold)
        {
            //InitMissionFollow();
            //m_MissionsList.SetActive(true);
            // 遍历 已完成 闪烁
            for (int i = 0; i < m_CommpletedItemCount; i++)
            {
                if (m_MissionItemsList[i])
                {
                    int nMissionID = m_MissionItemsList[i].MissionID;
                    MissionState misState = (MissionState)GameManager.gameManager.MissionManager.GetMissionState(nMissionID);
                    if (misState == MissionState.Mission_Completed)
                    {
                        UpdateMissionFollowBlink(i, true);
                    }
                }
            }
        }
    }

    public void CloseMissionInfoRoot()
    {
        UIManager.CloseUI(UIInfo.MissionInfoController);
    }

    public void AfterTabsControlButtonClicked()
    {
        m_TabsFold = !m_TabsFold;
    }

    void InitLeftTabControl(int nControl)
    {
        m_TabTweenPos.Reset();
        m_ContentTweenPos.Reset();

        // 以折叠状态初始化
        if (nControl == 1)
        {
            m_TabsFold = true;

			m_TabTweenPos.from = new Vector3(-195, 123, 0);
            m_TabTweenPos.to = new Vector3(150, 123, 0);

            m_ContentTweenPos.from = new Vector3(-212, 0, 0);
			m_ContentTweenPos.to = new Vector3(210, 0, 0);

        }
        // 以展开状态初始化
        else
        {
            m_TabsFold = false;

            m_TabTweenPos.from = new Vector3(150, 123, 0);
			m_TabTweenPos.to = new Vector3(-195, 123, 0);

			m_ContentTweenPos.from = new Vector3(210, 0, 0);
			m_ContentTweenPos.to = new Vector3(-212, -0, 0);
        }

        m_TabTweenPos.gameObject.transform.localPosition = m_TabTweenPos.from;
        m_ContentTweenPos.gameObject.transform.localPosition = m_ContentTweenPos.from;
    }

    void LeftControlClick()
    {
        if (PlayerPreferenceData.LeftTabControl == 1)
        {
            PlayerPreferenceData.LeftTabControl = 0;
            GameManager.gameManager.SoundManager.PlaySoundEffect(128);
            return;
        }
//        else
//        {
//            NewPlayerGuidLogic.CloseWindow();
//            PlayerPreferenceData.LeftTabControl = 1;
//            GameManager.gameManager.SoundManager.PlaySoundEffect(8);
//            return;
//        }
    }

    void OpenTeamWindow()
    {
		PlayerFrameLogic.Instance().SwitchAllWhenPopUIShow (false);
        RelationLogic.OpenTeamWindow(RelationTeamWindow.TeamTab.TeamTab_NearPlayer);
    }

    public void UpdateTeamInfo()
    {
        if (m_TeamButton != null)
        {
            if (GameManager.gameManager.PlayerDataPool.TeamInfo.TeamID >= 0)
            {
                m_TeamButton.SetActive(false);
				if (GameManager.gameManager.PlayerDataPool.TeamInfo.GetTeamMemberCount() == 1)
				{
					m_LeaveTeamButton.SetActive(true);
				}
				else
				{
					m_LeaveTeamButton.SetActive(false);
				}
            }
            else
            {
                m_TeamButton.SetActive(true);
				m_LeaveTeamButton.SetActive(false);
            }
        }
    }

    void LeaveTeamOnClick()
    {
        MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{3181}"), "", LeaveTeamOK);
    }

    void LeaveTeamOK()
    {
        if (GameManager.gameManager.PlayerDataPool.TeamInfo.TeamID != GlobeVar.INVALID_ID)
        {
            if (null != Singleton<ObjManager>.GetInstance().MainPlayer)
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.ReqLeaveTeam();
            }
        }
    }

    public void HandleSyncTeamInfo()
    {
        if (m_TeamButton != null)
        {
            if (GameManager.gameManager.PlayerDataPool.TeamInfo.TeamID >= 0)
            {
                m_TeamButton.SetActive(false);
                if (GameManager.gameManager.PlayerDataPool.TeamInfo.GetTeamMemberCount() == 1)
                {
                    m_LeaveTeamButton.SetActive(true);
                }
                else
                {
                    m_LeaveTeamButton.SetActive(false);
                }
            }
            else
            {
                m_TeamButton.SetActive(true);
                m_LeaveTeamButton.SetActive(false);
            }
        }
    }

    // 策划要求：任务追踪 跨场景不变  添加 任务Item并设置状态
    void InitMissionItemList(List<int> nMissionSortList, GameObject resItem)
    {

        for (int i = 0; i < nMissionSortList.Count; ++i)
        {
            int nMissionID = nMissionSortList[i];
            if (nMissionID < 0)
            {
                return;
            }

            Tab_MissionDictionary MissionDic = TableManager.GetMissionDictionaryByID(nMissionID, 0);
            if (MissionDic == null)
            {
                return;
            }

            GameObject ItemObj = Utils.BindObjToParent(resItem, m_MissionsList.gameObject, "MissionItem" + i);
            if (ItemObj)
            {
                MissionItemLogic MissionItem = ItemObj.GetComponent<MissionItemLogic>();
                if (MissionItem == null)
                {
                    return;
                }

                byte yMissionQuality = GameManager.gameManager.MissionManager.GetMissionQuality(nMissionID);
                string strMissionColor = GetColorByQuality(yMissionQuality);

                MissionItem.MissionID = nMissionID;
                MissionItem.MissionTile.text = string.Format(MissionDic.MissionName, strMissionColor, "[ffffff]");
                MissionItem.MissionInfo.text = "[ffffff]" + string.Format(MissionDic.FollowText, "[ffffff]", "0");
                m_MissionItemsList.Add(MissionItem);

                // 设置状态
                MissionState misState = (MissionState)GameManager.gameManager.MissionManager.GetMissionState(nMissionID);
                if (MissionState.Mission_Completed == misState)
                {
                    MissionItem.MissionTile.text = string.Format(MissionDic.MissionName, "[1fff1f]", "[1fff1f]");

                    int nParam = GameManager.gameManager.MissionManager.GetMissionParam(nMissionID, 0);
                    MissionItem.MissionInfo.text = "[1fff1f]" + string.Format(MissionDic.FollowText, "[1fff1f]", nParam);

                    UpdateMissionFollowBlink(i, true);

                    m_CommpletedItemCount += 1;

                    // 添加新手指引
					if (nMissionID == 2 || nMissionID == 7 || nMissionID == 1)
                    {
                        m_GuideMissionFlag = true;
                        ShowNewPlayerGuide(nMissionID);
                    }
                }
                else
                {
                    string StrMissionTile = MissionDic.MissionName;
                    if (MissionState.Mission_Failed == misState)
                    {
                        StrMissionTile = StrMissionTile + " [fe3737]" + StrDictionary.GetClientDictionaryString("#{1362}");
                    }
                    MissionItem.MissionTile.text = string.Format(StrMissionTile, strMissionColor, "[ffffff]");
   
                    int nParam = GameManager.gameManager.MissionManager.GetMissionParam(nMissionID, 0);
                    if (nParam != 0)
                    {
                        MissionItem.MissionInfo.text = "[ffffff]" + string.Format(MissionDic.FollowText, "[ffffff]", nParam);
                        //MissionItem.MissionInfo.text = "[ffe6b4]" + string.Format(MissionDic.FollowText, "[fe3737]", nParam);
                    }
                    // 添加新手指引
                    if (nMissionID == 6 || nMissionID == 14)
                    {
                        m_GuideMissionFlag = true;
                        ShowNewPlayerGuide(nMissionID);
                    }
                }
            }
        }

        if (m_MissionsList)
        {
            m_MissionsList.repositionNow = true;
        }
    }

    void UpDateMissionSortLst()
    {
        GameManager.gameManager.PlayerDataPool.MissionSortList.Clear();
        for (int i = 0; i < m_MissionItemsList.Count; ++i )
        {
            GameManager.gameManager.PlayerDataPool.MissionSortList.Add(m_MissionItemsList[i].MissionID);
        }
    }

    void InitMissionScrollViewPos()
    {
        // 默认 组队界面的话，不记录了
        if (PlayerPreferenceData.LeftTabChoose == 1)
        {
            return;
        }

        Vector3 vLastGridPos = GameManager.gameManager.PlayerDataPool.MissionGridLastPos;
        if (vLastGridPos == Vector3.zero)
        {
            return;
        }

        Vector3 vCurGridPos = m_MissionsList.transform.position;
        Vector3 vOffset = vCurGridPos - vLastGridPos;
        m_dragPanel.MoveAbsolute(-vOffset);
    }

	public void LeaveTeamFollow()
	{
		if (m_ContentTeam.activeSelf) 
		{
			TeamList.Instance().UpdateTeamMember();
		}
	}
}
