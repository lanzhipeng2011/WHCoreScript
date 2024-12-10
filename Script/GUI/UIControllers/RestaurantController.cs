using UnityEngine;
using System.Collections;
using GCGame.Table;
using Module.Log;
using System.Collections.Generic;
using Games.GlobeDefine;
using System;

public class RestaurantController : UIControllerBase<RestaurantController> {

    public GameObject m_FoodWindow;
	public GameObject m_MainWindow;
    public GameObject[] m_Desks;
    public UILabel m_LabelRestaurantLevel;
    public UISlider m_SliderLevel;
    public UILabel m_LabelEarnings;         // 收益
    public GameObject m_BtnFriendRestaurant;
    public GameObject m_BtnPrepareAll;
    public GameObject m_BtnFinishPrepareAll;
    public GameObject m_DetailInfoParent;
    public GameObject m_BackButton;
    public UILabel m_LabelTitle;         // 收益
    public UILabel m_LabelExp;         // EXP

    public UILabel m_LabelVisitFriend;         

    private const float RequestFrinedDataDelay = 3.0f;
    private const int  Max_Billing_Friend = 10;

    private RestaurantData.RestaurantInfo m_curRestaurant;
    private RestaurantDesk m_curOperationDesk;

    private bool m_bSelfData = true;
    public RestaurantData.RestaurantInfo CurRestaurant() { return m_curRestaurant;}
    private float m_lastRequestFriendTime = 0;

    private int m_NewPlayerGuide_Step = -1;
    public int NewPlayerGuide_Step
    {
        get { return m_NewPlayerGuide_Step; }
        set { m_NewPlayerGuide_Step = value; }
    }
    public GameObject m_BtnClose;

    public bool SelfData
    {
        get { return m_bSelfData;  }
    }
    public UIToggle m_PushToggle;   //是否推送
    public static void OpenWindow(bool bSelfData)
    {
        UIManager.ShowUI(UIInfo.Restaurant, OnOpenWindow, bSelfData);
    }

    public static void OnOpenWindow(bool bSuccess, object param)
    {
        if (bSuccess && RestaurantController.Instance() != null)
        {
            bool bSelfData = (bool)param;
            RestaurantController.Instance().UpdateRestaurantInfo(bSelfData);
        }
    }

    void Awake()
    {
        RestaurantData.delUpdatePlayerData += OnPlayerDataUpdate;
        RestaurantData.delUpdateFriendData += OnFriendDataUpdate;

        RestaurantData.delUpdatePlayerDeskData += OnPlayerDeskDataUpdate;
        RestaurantData.delUpdateFriendDeskData += OnFriendDeskDataUpdate;

        SetInstance(this);
        if (m_FoodWindow != null)
        {
            m_FoodWindow.SetActive(false);
			m_MainWindow.SetActive(true);
        }
    }

   
	// Use this for initialization
	void Start () {
        Check_NewPlayerGuide(); // 新手指引
        RestaurantData.CleanRestaurantTip();
        m_PushToggle.value = PlayerPreferenceData.SystemIsPushRestaurant;
	}

    void OnDestroy()
    {
        RestaurantData.delUpdatePlayerData -= OnPlayerDataUpdate;
        RestaurantData.delUpdateFriendData -= OnFriendDataUpdate;

        RestaurantData.delUpdatePlayerDeskData -= OnPlayerDeskDataUpdate;
        RestaurantData.delUpdateFriendDeskData -= OnFriendDeskDataUpdate;
        SetInstance(null);
    }

    void OnCloseClick()
    {  
		NewPlayerGuidLogic.CloseWindow ();
		m_NewPlayerGuide_Step = -1;

        UIManager.CloseUI(UIInfo.Restaurant);
    }

    // 好友酒楼
    void OnFriendRestaurantClick()
    {
        UIManager.ShowUI(UIInfo.RelationNamePopWindow, OnShowRelationNamePopWindow);
        //MessageBoxLogic.OpenOKBox("此功能暂时未开放");
    }

    void OnShowRelationNamePopWindow(bool bSuccess, object param)
    {
        if (bSuccess)
        {
            RelationNamePopController.Instance().SetDelegate(OnChooseFriend);
            RelationNamePopController.Instance().SetIsPopWindow(false);
            RelationNamePopController.Instance().SetIsRestaurantFriend(true);
        }
    }

    // 全部结账
    void OnBillingAllClick()
    {
        if (null == m_curRestaurant)
        {
            return;
        }

        bool bCanBilling = false;
        foreach (RestaurantData.DeskInfo curDesk in m_curRestaurant.m_Desks)
        {
            if (curDesk.m_IsActive && curDesk.m_DestState == RestaurantData.DeskState.WaitBilling)
            {
                bCanBilling = true;
                break;
            }
        }

        if (!bCanBilling)
        {
            // 没有可以结账的餐桌
            GUIData.AddNotifyData2Client(false,"#{1980}");
            return;
        }

        CG_RESTAURANT_BILLINGALL packet = (CG_RESTAURANT_BILLINGALL)PacketDistributed.CreatePacket(MessageID.PACKET_CG_RESTAURANT_BILLINGALL);
        packet.None = 0;
        if(!m_bSelfData)
        {
            packet.FriendGuid = RestaurantData.m_FriendRestaurantInfo.m_FrinedGuid;
        }
        packet.SendPacket();
    }

    // 全部上菜
    void OnServingAllClick()
    {
        //Singleton<ObjManager>.GetInstance().MainPlayer.ReqViewOtherPlayer(Singleton<ObjManager>.GetInstance().MainPlayer.GUID, OtherRoleViewLogic.OPEN_TYPE.OPEN_TYPE_LASTSPEAKER);

        if (!m_bSelfData)
        {
			GUIData.AddNotifyData2Client(false,"#{2017}");
            return;
        }
        // 新手指引
        if (m_NewPlayerGuide_Step == 0)
        {
            NewPlayerGuide(1);
        }
        m_curOperationDesk = null;
        if (m_FoodWindow != null)
        {
            m_FoodWindow.SetActive(true);
			m_MainWindow.SetActive(false);
        }     
    }
    // 全部上菜完成
    void OnFinishServingAllClick()
    {
        if ( !m_bSelfData )
        {
			GUIData.AddNotifyData2Client(false,"#{2017}");
            return;
        }
        if (null == m_curRestaurant)
        {
            return;
        }
        // 超出当日完成次数上限
        int nTodayFinishNum = GameManager.gameManager.PlayerDataPool.CommonData.GetCommonData((int)Games.UserCommonData.USER_COMMONDATA.CD_RESTAURANT_FINISHFOOD_NUM);
        if (nTodayFinishNum >= RestaurantData.FinishFoodMax)
        {
            MessageBoxLogic.OpenOKBox(1563, 1000);
            return;
        }
        bool bHavePreDesk = false;
        int nCostYuanBao = 0;
        for (int i = 0; i < m_curRestaurant.m_Desks.Length; i++)
        {
            if ( m_curRestaurant.m_Desks[i].m_DestState != RestaurantData.DeskState.PrepareFood )
            {
                continue;
            }
            int nFoodID = m_curRestaurant.m_Desks[i].m_FoodID;
            Tab_RestaurantFood curTableFood = TableManager.GetRestaurantFoodByID(nFoodID, 0);
            if (null == curTableFood)
            {
                LogModule.ErrorLog("OnFinishServingAllClick:: curTableFood is null FoodID =" + nFoodID.ToString());
                continue;
            }
            bHavePreDesk = true;

            int nCurDestCostYuanBao = 0;
            int nLeftTenMinues = m_curRestaurant.m_Desks[i].GetFoodLeftTime() / 600;
            if (m_curRestaurant.m_Desks[i].GetFoodLeftTime() % 600 != 0)
            {
                nLeftTenMinues = nLeftTenMinues + 1;
            }
            if (nLeftTenMinues <= 0)
            {
                nLeftTenMinues = 1;
            }
            nCurDestCostYuanBao = nLeftTenMinues * curTableFood.CostYuanBao;
            nCostYuanBao = nCostYuanBao + nCurDestCostYuanBao;
        }
        if (false == bHavePreDesk)
        {
            MessageBoxLogic.OpenOKBox(2381, 1000);
            return;
        }
        string strTip = StrDictionary.GetClientDictionaryString("#{2338}", nCostYuanBao);
        string strTitle = StrDictionary.GetClientDictionaryString("#{1000}");
        MessageBoxLogic.OpenOKCancelBox(strTip, strTitle, OnFinishServingAll, null);
    }

    void OnFinishServingAll()
    {
        CG_RESTAURANT_FINISHPREPARE packet = (CG_RESTAURANT_FINISHPREPARE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_RESTAURANT_FINISHPREPARE);
        packet.None = 0;
        packet.SendPacket();
    }

    // 收到玩家酒楼更新
    void OnPlayerDataUpdate()
    {
        if (m_bSelfData)
        {
            UpdateRestaurantInfo(true);
        }
    }
    // 收到好友酒楼更新
    void OnFriendDataUpdate()
    {
        UpdateRestaurantInfo(false);
    }

    // 收到玩家酒楼更新
    void OnPlayerDeskDataUpdate()
    {
        if (m_bSelfData)
        {
            UpdateRestaurantInfo(true);
        }
    }
    // 收到好友酒楼更新
    void OnFriendDeskDataUpdate()
    {
        if (m_bSelfData == false)
        {
            UpdateRestaurantInfo(false);
        }
    }

    // 选择了一个好友名字，发送进入好友酒楼申请
    void OnChooseFriend(System.UInt64 friendID, string friendName)
    {
        //if (!m_bSelfData)
        //{
        //    GUIData.AddNotifyData("#{2017}");
        //    return;
        //}
        //if (Time.realtimeSinceStartup - m_lastRequestFriendTime < RequestFrinedDataDelay)
        //{
        //    // 操作过于频繁，请稍后再试
        //    GUIData.AddNotifyData("#{1981}");
        //    return;
        //}
      
        m_lastRequestFriendTime = Time.realtimeSinceStartup;
        CG_RESTAURANT_VISITFRIEND packet = (CG_RESTAURANT_VISITFRIEND)PacketDistributed.CreatePacket(MessageID.PACKET_CG_RESTAURANT_VISITFRIEND);
        packet.FriendGuid = friendID;
        packet.SendPacket();
        
        UIManager.CloseUI(UIInfo.RelationNamePopWindow);

    }

    void OnBackClick()
    {
        if (m_bSelfData)
        {
            return;
        }
        m_bSelfData = true;
        UpdateRestaurantInfo(true);
    }

    public void UpdateRestaurantInfo(bool bSelfData)
    {
        m_bSelfData = bSelfData;
        m_curRestaurant = bSelfData ? RestaurantData.m_PlayerRestaurantInfo : RestaurantData.m_FriendRestaurantInfo;
        if (null == m_curRestaurant)
        {
            LogModule.ErrorLog("m_curRestaurant is null");
            return;
        }
        if (null == m_DetailInfoParent)
        {
            LogModule.ErrorLog("m_DetailInfoParent is null");
            return;
        }
        if (null == m_LabelRestaurantLevel)
        {
            LogModule.ErrorLog("m_LabelRestaurantLevel is null");
            return;
        }
        if (null == m_BtnFinishPrepareAll)
        {
            LogModule.ErrorLog("m_BtnFinishPrepareAll is null");
            return;
        }
        if (null == m_BtnFriendRestaurant)
        {
            LogModule.ErrorLog("m_BtnFriendRestaurant is null");
            return;
        }
        if (null == m_BtnPrepareAll)
        {
            LogModule.ErrorLog("m_BtnPrepareAll is null");
            return;
        }
        if (null == m_LabelEarnings)
        {
            LogModule.ErrorLog("m_LabelEarnings is null");
            return;
        }
        if (null == m_SliderLevel)
        {
            LogModule.ErrorLog("m_SliderLevel is null");
            return;
        }
        if (null == m_BackButton)
        {
            LogModule.ErrorLog("m_BackButton is null");
            return;
        }
        if (null == m_LabelTitle)
        {
            LogModule.ErrorLog("m_LabelTitle is null");
            return;
        }
        if (null == m_LabelExp)
        {
             LogModule.ErrorLog("m_LabelExp is null");
            return;
        }


        List<int> DeskIndexlist = new List<int>();
        for (int i = 0; i < m_curRestaurant.m_Desks.Length; i++)
        {
            if (m_curRestaurant.m_Desks[i].m_IsActive)
            {
                DeskIndexlist.Add(i);
            }
        }

        for (int i = 0; i < m_curRestaurant.m_Desks.Length; i++)
        {
            if (false == m_curRestaurant.m_Desks[i].m_IsActive)
            {
                DeskIndexlist.Add(i);
            }
        }

        for (int i = 0; i < m_Desks.Length && i < DeskIndexlist.Count; i++)
        {
            if (m_Desks[i].GetComponent<RestaurantDesk>() != null)
            {
                m_Desks[i].GetComponent<RestaurantDesk>().SetIndex(m_curRestaurant, DeskIndexlist[i]);
            }
        }
        
        m_DetailInfoParent.SetActive(true);            
        m_LabelRestaurantLevel.text = m_curRestaurant.m_RestaurantLevel.ToString();
       
        m_BtnFinishPrepareAll.SetActive(m_bSelfData);
        //m_BtnFriendRestaurant.SetActive(m_bSelfData);

        m_BtnPrepareAll.SetActive(m_bSelfData);
        Tab_RestaurantLevel curTabLevel = TableManager.GetRestaurantLevelByID(m_curRestaurant.m_RestaurantLevel, 0);
        if (null == curTabLevel)
        {
            LogModule.ErrorLog("restaurant level big than table define " + m_curRestaurant.m_RestaurantLevel.ToString());
        }
        else
        {
            m_SliderLevel.value = (float)m_curRestaurant.m_RestaurantExp / (float)curTabLevel.LevelupExp;
            m_LabelEarnings.text = ((int)(curTabLevel.AdditionPrecent * 100)).ToString() + "%";
            //m_LabelExp.text = StrDictionary.GetClientDictionaryString("#{2980}", m_curRestaurant.m_RestaurantExp, curTabLevel.LevelupExp);
            m_LabelExp.text = m_curRestaurant.m_RestaurantExp.ToString() +"/" + curTabLevel.LevelupExp.ToString();
        }

        if (m_bSelfData)
        {
            m_BackButton.SetActive(false);
            //m_LabelTitle.text = m_curRestaurant.m_MasterName + "的酒楼";
        }
        else
        {
            m_BackButton.SetActive(true);
        }
        m_LabelTitle.text = StrDictionary.GetClientDictionaryString("#{2676}", m_curRestaurant.m_MasterName);
       // UpdateVisitFriendInfo();
    }

    /// <summary>
    /// 
    /// </summary>
    public void UpdateVisitFriendInfo()
    {
        int nVisitFrinedCount = 0;
         if (RestaurantData.m_PlayerRestaurantInfo != null && 
                RestaurantData.m_PlayerRestaurantInfo.m_VisitFrindList != null)
         {
             nVisitFrinedCount = RestaurantData.m_PlayerRestaurantInfo.m_VisitFrindList.Count;
         }
         int nLeftVisitCount = Max_Billing_Friend - nVisitFrinedCount;
        if (nLeftVisitCount < 0)
        {
            nLeftVisitCount = 0;
        }
       // m_LabelVisitFriend.text = string.Format("{0}/{1}", nLeftVisitCount, Max_Billing_Friend);
    }
    
    /// <summary>
    ///  // 打开准备食物界面，只有自己酒楼才有此功能
    /// </summary>
    /// <param name="curDesk"></param>
    public void OpenFoodWindow(RestaurantDesk curDesk)
    {
        if (!m_bSelfData)
        {
			GUIData.AddNotifyData2Client(false,"#{2017}");
            return;
        }
        m_curOperationDesk = curDesk;
        if (m_FoodWindow != null)
        {
            m_FoodWindow.SetActive(true);
			m_MainWindow.SetActive(false);
        }       
    }

    public void CloseFoodWindow()
    {
        m_curOperationDesk = null;
        if (m_FoodWindow != null)
        {
            m_FoodWindow.SetActive(false);
        }

	}
	// 上菜操作
    public void OnChooseFoodFinish(int foodID)
    {
        if (!m_bSelfData)
        {
			GUIData.AddNotifyData2Client(false,"#{2017}");
            return;
        }
        if (m_FoodWindow != null)
        {
            m_FoodWindow.SetActive(false);
			m_MainWindow.SetActive(true);
        }

      //  GameManager.gameManager.SoundManager.PlaySoundEffect(134);
       
        CG_RESTAURANT_PREPAREFOOD packet = (CG_RESTAURANT_PREPAREFOOD)PacketDistributed.CreatePacket(MessageID.PACKET_CG_RESTAURANT_PREPAREFOOD);
        if (null != m_curOperationDesk)
        {
            packet.SetDeskIndex(m_curOperationDesk.Index);
            m_curOperationDesk = null;
        }
        packet.SetFoodID(foodID);
        packet.SendPacket();
    }


    public void BillingDesk(RestaurantDesk curDesk)
    {
        if (!m_bSelfData)
        {
			GUIData.AddNotifyData2Client(false,"#{2017}");
            return;
        }
        if (null == curDesk)
        {
            LogModule.ErrorLog("curDesk is null");
            return;
        }
        CG_RESTAURANT_BILLINGALL packet = (CG_RESTAURANT_BILLINGALL)PacketDistributed.CreatePacket(MessageID.PACKET_CG_RESTAURANT_BILLINGALL);
        packet.None = 0;
        packet.SetDeskIndex(curDesk.Index);
        packet.SendPacket();

       // GameManager.gameManager.SoundManager.PlaySoundEffect(118);  //check
    }

    public void FinishPrepareDesk(RestaurantDesk curDesk)
    {
  
    }

    void Check_NewPlayerGuide()
    {
        if (MenuBarLogic.Instance() == null)
        {
            return;
        }
        int nIndex = MenuBarLogic.Instance().NewPlayerGuideIndex;
		if (nIndex == (int)GameDefine_Globe.NEWOLAYERGUIDE.RESTAURANT)
        {
            NewPlayerGuide(0);
            MenuBarLogic.Instance().NewPlayerGuideIndex = -1;
        }
    }

    public void NewPlayerGuide(int nIndex)
    {
		if(nIndex < 0)
		{
			return;
		}
		NewPlayerGuidLogic.CloseWindow ();

        m_NewPlayerGuide_Step = nIndex;

        switch (nIndex)
        {
            case 0:
                {
                    NewPlayerGuidLogic.OpenWindow(m_BtnPrepareAll, 202, 64, "", "top", 2, true, true);
                }
                break;
            case 1:
                {
                    if (m_FoodWindow)
                    {
                        RestaurantFoodWindow FoodWindow = m_FoodWindow.GetComponent<RestaurantFoodWindow>();
                        if (FoodWindow)
                        {
                            FoodWindow.NewPlayerGuide(0);
                        }
                        m_NewPlayerGuide_Step = -1;
                    }
                }
                break;
            case 2:
                NewPlayerGuidLogic.OpenWindow(m_BtnClose, 78, 78, "", "top", 2, true, true);
                break;
        }

    }
    //遮挡半透
    public void OnIsPush()
    {

        PlayerPreferenceData.SystemIsPushRestaurant = m_PushToggle.value;
    }
     
}
