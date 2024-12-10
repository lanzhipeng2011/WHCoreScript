using UnityEngine;
using System.Collections;
using GCGame;
using GCGame.Table;
using System.Collections.Generic;
using Games.LogicObj;
using Module.Log;
using Games.GlobeDefine;
using Games.Scene;

public class SceneMapLogic : MonoBehaviour 
{
    class FriendNPCData
    {
        public string m_name;
        public Vector3 m_pos;
    }

    private static SceneMapLogic m_Instance = null;
    public static SceneMapLogic Instance()
    {
        return m_Instance;
    }

    public UILabel m_SceneNameLabel;    //场景名称
    public UILabel m_ScenePosLable;          //当前坐标点
    //public UISprite m_SceneMapSprite;
    public GameObject m_TransmitPointGrid;

    public GameObject AutoSearchWindow;


    public UITexture m_TextureMap;
	public GameObject m_TextureRole;
    public GameObject m_ObjMapDot;
    public GameObject m_ObjMapNpc;
    public Transform m_MapRoot;

    public UISprite m_ClickEffectSprite;
    public Transform m_ClickEffectTran;
    public UIImageButton m_BtnChangeChannel;

    private int m_curSceneID = 1;
    private Tab_SceneClass m_curTabScene;
	private bool m_bLoadingItem = false;

    private static int m_curNPCListScene = -1;
    private static RecycleList<FriendNPCData> m_FriendNPCRecycleList = new RecycleList<FriendNPCData>();

    private List<Transform> m_ObjMapFriendDotList = new List<Transform>();

    void Awake()
    {
        m_Instance = this;
        
    }

    
    void OnDestroy()
    {
        m_Instance = null;
    }

    void Start()
    {
        NeedOffsetMapName = "";
        m_curSceneID = GameManager.gameManager.RunningScene;

        //根据加载场景ID获取场景信息
        m_curTabScene = TableManager.GetSceneClassByID(m_curSceneID, 0);
        if (null == m_curTabScene)
        {
            LogModule.ErrorLog("current is not defined in table: " + m_curSceneID.ToString());
            return;
        }
        ShowSceneInfo(m_curTabScene);
        // 初始化NPC信息
        if (m_curNPCListScene != GameManager.gameManager.RunningScene)
        {
            m_FriendNPCRecycleList.Clear();

            Dictionary<int, List<Tab_SceneNpc>> curTabSceneNPCDic = TableManager.GetSceneNpc();
            foreach (KeyValuePair<int, List<Tab_SceneNpc>> curPair in curTabSceneNPCDic)
            {
                if (curPair.Value[0].SceneID == GameManager.gameManager.RunningScene)
                {
                    Tab_RoleBaseAttr curRoleBase = TableManager.GetRoleBaseAttrByID(curPair.Value[0].DataID, 0);
					if(curRoleBase == null)//如果curRoleBase为空进行下一次循环
						continue;
                    if (curRoleBase.Camp == 1)
                    {
                        FriendNPCData newData = m_FriendNPCRecycleList.GetNewItem();
                        newData.m_name = curRoleBase.Name;
                        newData.m_pos = new Vector3(curPair.Value[0].PosX, 0, curPair.Value[0].PosZ);
                        m_FriendNPCRecycleList.Add(newData);
                    }
                }
            }
            m_curNPCListScene = GameManager.gameManager.RunningScene;
        }

        for (int i = 0; i < m_FriendNPCRecycleList.UsingList().Count; i++)
        {
            GameObject newNPCDot = GameObject.Instantiate(m_ObjMapNpc) as GameObject;
            if (null != newNPCDot)
            {
                newNPCDot.transform.parent = m_MapRoot;
                newNPCDot.transform.localScale = Vector3.one;
                newNPCDot.transform.localPosition = ScenePosToMapPos(m_FriendNPCRecycleList.UsingList()[i].m_pos, m_curTabScene);
                newNPCDot.SetActive(true);
            }
        }

        for (int i = 0; i < GlobeVar.MAX_TEAM_MEMBER; i++)
        {
            GameObject newNPCDot = GameObject.Instantiate(m_ObjMapDot) as GameObject;
            if (null != newNPCDot)
            {
                newNPCDot.transform.parent = m_MapRoot;
                newNPCDot.transform.localScale = Vector3.one;
                UITexture npcTexture = newNPCDot.GetComponent<UITexture>();
                m_ObjMapFriendDotList.Add(newNPCDot.transform);
            }
        }

       

        if (GameManager.gameManager.ActiveScene.IsCopyScene())
        {
            m_BtnChangeChannel.isEnabled = false;
        }
        else
        {
            m_BtnChangeChannel.isEnabled = true;
        }

        StartCoroutine(UpdateTeamPos());
    }

    IEnumerator UpdateTeamPos()
    {
        while (true)
        {
            if (null != Singleton<ObjManager>.GetInstance().MainPlayer && null != m_curTabScene)
            {
                if (m_TextureMap.gameObject.activeSelf && null != m_curTabScene)
                {
                    m_TextureRole.transform.localPosition = ScenePosToMapPos(Singleton<ObjManager>.GetInstance().MainPlayer.transform.localPosition, m_curTabScene);
                }

                for (int i = 0; i < m_ObjMapFriendDotList.Count; i++)
                {
                    if (i < GameManager.gameManager.PlayerDataPool.TeamInfo.teamMember.Length)
                    {
                        ulong curGuid = GameManager.gameManager.PlayerDataPool.TeamInfo.teamMember[i].Guid;
                        if (Singleton<ObjManager>.GetInstance().MainPlayer.GUID != curGuid)
                        {
                            Obj_OtherPlayer curPlayer = Singleton<ObjManager>.GetInstance().FindOtherPlayerInScene(curGuid);
                            if (null != curPlayer)
                            {
                                m_ObjMapFriendDotList[i].localPosition = ScenePosToMapPos(curPlayer.transform.localPosition, m_curTabScene);
                                m_ObjMapFriendDotList[i].gameObject.SetActive(true);
                            }
                        }
                    }
                    else
                    {
                        m_ObjMapFriendDotList[i].gameObject.SetActive(false);
                    }
                }
            }

            yield return new WaitForSeconds(1.0f);
        }
        
    }
    void InitSceneTransmitPoint()
    {
		m_bLoadingItem = true;
        UIManager.LoadItem(UIInfo.SceneMapTransmitPoint, OnLoadTransmitPoint);
    }

    void OnLoadTransmitPoint(GameObject resObj, object param)
    {
        if (null == resObj)
        {
            LogModule.ErrorLog("loas  transmit point error");
            return;
        }
        List<Tab_AutoSearch> tableList = TableManager.GetAutoSearchByID(m_curSceneID);
        if (null != tableList)
        {
			for (int nIndex = 0; nIndex < tableList.Count; ++nIndex)
            {
                Tab_AutoSearch autoSearchInfo = tableList[nIndex];
                if (null != autoSearchInfo)
                {
                    GameObject newTransmitPoint = Utils.BindObjToParent(resObj, m_TransmitPointGrid, "TransmitPoint" + nIndex.ToString());
                    if (null != newTransmitPoint && null != newTransmitPoint.GetComponent<AutoSearchPointLogic>())
                        newTransmitPoint.GetComponent<AutoSearchPointLogic>().CreateAutoSearchPointInfo(autoSearchInfo);
                }
            }
        }

        m_TransmitPointGrid.GetComponent<UIGrid>().Reposition();
		m_bLoadingItem = false;
    }

    
    
    public void CloseWindow()
    {
		if(m_bLoadingItem)
		{
			return;
		}
        UIManager.CloseUI(UIInfo.SceneMapRoot);
    }

    void GoToMainCity()
    {
    }

    void ShowOtherPlayers()
    {
		//Open RelationLogic Window and switch to NearByPlayer Frame
		if (null == RelationLogic.Instance()) 
		{
			UIManager.ShowUI(UIInfo.RelationRoot);
			//If Show UI Success
			if (null != RelationLogic.Instance())
			{
				//RelationLogic.Instance().OpenNearByPlayerFrame();
				RelationLogic.OpenTeamWindow(RelationTeamWindow.TeamTab.TeamTab_NearPlayer);
				CloseWindow();
			}
		}
    }

    void OnWorldMapClick()
    {
        UIManager.CloseUI(UIInfo.SceneMapRoot);
        UIManager.ShowUI(UIInfo.WorldMapWindow);
    }


    #region 特殊地图使用另外的地图文件并且修正位置
    public string NeedOffsetMapName = "";
    public string GetMapTexture(string textureName) 
    {
       if (textureName == "Texture/Map/MinMap_taoyuan")
      {
          NeedOffsetMapName = textureName;
          return "Texture/Map/taoyuan_MinMap2";
      }
      return textureName;
    }

    public Vector3 ApplyMapOffset(Vector3 vec,int typ)
    {
        if (NeedOffsetMapName == "Texture/Map/MinMap_taoyuan")
        {
            vec.y = typ == 0 ? vec.y + 100 : vec.y - 100;
        }
        return vec;
    }
     
    #endregion
    public void ShowSceneInfo(Tab_SceneClass curTabScene)
    {
        if (curTabScene != null)
        {
            m_SceneNameLabel.text = curTabScene.Name;
            m_SceneNameLabel.color = SceneData.GetSceneNameColor(curTabScene.SceneID);
            string TextureName = GetMapTexture(curTabScene.SceneMapTexture);
            Texture curTexture = ResourceManager.LoadResource(TextureName, typeof(Texture)) as Texture;
            if (null != curTexture)
            {
                m_TextureMap.mainTexture = curTexture;
                m_TextureMap.gameObject.SetActive(true);
            }
            
        }

        // m_SceneMapSprite.spriteName = "shop01";
        //m_SceneMapSprite.MakePixelPerfect();

        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (_mainPlayer != null)
        {
            m_ScenePosLable.text = StrDictionary.GetClientDictionaryString("#{1185}", (int)(_mainPlayer.gameObject.transform.position.x), (int)(_mainPlayer.gameObject.transform.position.z));

            if(m_TextureMap.gameObject.activeSelf && null != curTabScene)
            {
                m_TextureRole.transform.localPosition = ScenePosToMapPos(_mainPlayer.transform.localPosition, curTabScene);
            }
           
        }

        Utils.CleanGrid(m_TransmitPointGrid);

        InitSceneTransmitPoint();
	
    }

    void OnChangeChannelClick()
    {
        Tab_SceneClass curTabScene = TableManager.GetSceneClassByID(GameManager.gameManager.RunningScene, 0);
        if (null == curTabScene)
        {
            return;
        }

        if (curTabScene.Type == 2)
        {
            // 副本不允许切线
            return;
        }

		UIManager.ShowUI(UIInfo.ChannelChange);
    }

    Vector3 ScenePosToMapPos(Vector3 scenePos, Tab_SceneClass curTabScene)
    {
        float posScale = (float)m_TextureMap.width / curTabScene.SceneMapLogicWidth;
		float curXPos = scenePos.x * posScale - m_TextureMap.width * 0.5f;
		float curYPos = scenePos.z * posScale - m_TextureMap.height * 0.5f;


        return ApplyMapOffset(new Vector3(curXPos, curYPos, 0),0); ;
    }

    Vector3 MapPosToScenePos(Vector3 mapPos, Tab_SceneClass curTabScene)
    {
        mapPos = ApplyMapOffset(mapPos,1);
        float posScale = (float)m_TextureMap.width / curTabScene.SceneMapLogicWidth;
        float curXPos = (mapPos.x + m_TextureMap.width * 0.5f) / posScale;
        float curYPos = (mapPos.y + m_TextureMap.height * 0.5f) / posScale;
        return new Vector3(curXPos, 0, curYPos);
    }

    void OnSceneMapClick()
    {
        Vector3 worldPos = UICamera.currentCamera.ScreenToWorldPoint(UICamera.lastTouchPosition);
        Vector3 localPos = m_TextureMap.transform.InverseTransformPoint(worldPos);
        Vector3 mapPos = MapPosToScenePos(localPos, m_curTabScene);

        AutoSearchPoint point = new AutoSearchPoint(m_curSceneID, mapPos.x, mapPos.z);
        if (GameManager.gameManager && GameManager.gameManager.AutoSearch)
        {
			if (Singleton<ObjManager>.GetInstance().MainPlayer != null)
			{
				Vector3 orgPosition = new Vector3(point.PosX, 0f, point.PosZ);
				orgPosition = ActiveScene.GetTerrainPosition(orgPosition);
				if (!Singleton<ObjManager>.GetInstance().MainPlayer.IsMoveNavAgent(orgPosition))
				{
					return;
				}
			}


            GameManager.gameManager.AutoSearch.BuildPath(point);
        }

        if (m_ClickEffectSprite != null && m_ClickEffectTran != null)
        {
            m_ClickEffectTran.localPosition = localPos;
            m_ClickEffectSprite.gameObject.SetActive(true);
        }
        else
        {
            LogModule.ErrorLog("OnSceneMapClick::m_ClickEffectSprite = null || m_ClickEffectTran = null");
        }
    }

    public void StopPlayClickEffect()
    {
        if (m_ClickEffectSprite != null)
        {
            m_ClickEffectSprite.gameObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        if (m_ClickEffectSprite != null)
        {
            m_ClickEffectSprite.gameObject.SetActive(false);
        }
    }
}
