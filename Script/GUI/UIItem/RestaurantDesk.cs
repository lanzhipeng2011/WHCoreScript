using UnityEngine;
using System.Collections;
using Module.Log;
using GCGame.Table;
using GCGame;
using System;

public class RestaurantDesk : MonoBehaviour {

    public GameObject m_BtnNormal;
    public GameObject m_BtnLock;
    public UILabel m_LabelUnLockTip;
    public UILabel m_LabelStateDetail;
	public GameObject  m_LaZhuSpr;
    public GameObject[] m_GuestWindows;
    public UILabel[] m_GuestNameLable;
    public GameObject[] m_GuestSprites;

    public GameObject PreFoodDestBk;
    public GameObject EatFoodDestBk;

    private int m_curDeskIndex;
    private RestaurantData.RestaurantInfo m_curRestaurantData;
    private RestaurantData.DeskInfo m_curDestData;
    private Tab_RestaurantDesk m_curTabDesk;
    private string[] m_strGuestIndexs = new string[RestaurantData.GuestCountMax] { "", "", "", "" };

    private Color m_WhiteQualityColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    private Color m_GreenQualityColor = new Color(51.0f/255.0f, 204.0f/255.0f, 102.0f/255.0f, 1.0f);
    private Color m_BlueQualityColor = new Color(51.0f/255.0f, 204.0f/255.0f, 255.0f/255.0f, 1.0f);
    private Color m_PurpleQualityColor = new Color(204.0f/255.0f, 102.0f/255.0f, 255.0f/255.0f, 1.0f);
    private Color m_OrangeQualityColor = new Color(255.0f/255.0f, 153.0f/255.0f, 51.0f/255.0f, 1.0f);

    public enum Guest_Quality
    {
        QUALITY_INVALID = 0,
        QUALITY_WHITE,				//白
        QUALITY_GREEN,				//绿
        QUALITY_BLUE,				//蓝
        QUALITY_PURPLE,				//紫
        QUALITY_ORANGE,				//橙
    }

    public int Index { get { return m_curDeskIndex; } }
    
    void UpdateDest()
    {
        UpdateDestInfo();
    }

    void Start () {
        InvokeRepeating("UpdateDest", 0.5f, 1.1f);
	}

    void OnDestroy()
    {
        CancelInvoke("UpdateDest");
    }
   
    public void UpdateDestInfo()
    {
        if (null == m_curDestData)
        {
            LogModule.ErrorLog("m_curDestData is null");
            return;
        }
        if (null == m_BtnLock)
        {
            LogModule.ErrorLog("m_BtnLock is null");
            return;
        }
        if (null == m_BtnNormal)
        {
            LogModule.ErrorLog("m_BtnNormal is null");
            return;
        }
        if (null == m_LabelStateDetail)
        {
            LogModule.ErrorLog("m_LabelStateDetail is null");
            return;
        }
        if (null == PreFoodDestBk)
        {
            LogModule.ErrorLog("PreFoodDestBk is null");
            return;
        }
        if (null == EatFoodDestBk)
        {
            LogModule.ErrorLog("EatFoodDestBk is null");
            return;
        }     
        m_BtnLock.SetActive(!m_curDestData.m_IsActive);
        m_BtnNormal.SetActive(m_curDestData.m_IsActive);
		NGUIDebug.Log (m_curDestData.m_DestState.ToString ());
        if (m_curDestData.m_IsActive)
        {
            switch (m_curDestData.m_DestState)
            {
                case RestaurantData.DeskState.PrepareFood:
                    {
                        // 正在准备菜肴：剩余时间：
                        m_LabelStateDetail.text = StrDictionary.GetClientDictionaryString("#{1982}", m_curDestData.GetLeftTime());
                        PreFoodDestBk.SetActive(true);
                        EatFoodDestBk.SetActive(false);
			        	m_LaZhuSpr.SetActive(true);
				       if (m_curDestData.GetFoodLeftTime() <= 0)
				       {
					      m_curDestData.m_DestState = RestaurantData.DeskState.EatFood;
					OnFinishPrepareDesk();
				       }
//                        for (int i = 0; i < m_GuestWindows.Length; i++)
//                        {
//                            m_GuestWindows[i].SetActive(false);
//                        }
                    }
                    break;
                case RestaurantData.DeskState.EatFood:
                    {
                        PreFoodDestBk.SetActive(false);
                        EatFoodDestBk.SetActive(true);
				        m_LaZhuSpr.SetActive(false);
                        string guestName = "";
//                        for (int i = 0; i < m_curDestData.m_GuestIDs.Length; i++)
//                        {
//                            int curGuestId = m_curDestData.m_GuestIDs[i];
//                            Tab_RestaurantGuest curTabGuest = TableManager.GetRestaurantGuestByID(Int32.Parse(m_strGuestIndexs[i]), 0);
//                            if (null != curTabGuest)
//                            {
//                                //guestName += curTabGuest.Name + " ";
//                                if (i < m_GuestNameLable.Length)
//                                {
//                                    m_GuestNameLable[i].text = curTabGuest.Name;
//                                    Guest_Quality quality = (Guest_Quality)curTabGuest.Quality;
//                                    switch (quality)
//                                    {
//                                        case Guest_Quality.QUALITY_WHITE:
//                                            m_GuestNameLable[i].color = m_WhiteQualityColor;
//                                            break;
//                                        case Guest_Quality.QUALITY_GREEN:
//                                            m_GuestNameLable[i].color = m_GreenQualityColor;
//                                            break;
//                                        case Guest_Quality.QUALITY_BLUE:
//                                            m_GuestNameLable[i].color = m_BlueQualityColor;
//                                            break;
//                                        case Guest_Quality.QUALITY_PURPLE:
//                                            m_GuestNameLable[i].color = m_PurpleQualityColor;
//                                            break;
//                                        case Guest_Quality.QUALITY_ORANGE:
//                                            m_GuestNameLable[i].color = m_OrangeQualityColor;
//                                            break;
//                                    }
//                                }
//                                if ( i < m_GuestSprites.Length )
//                                {                                  
//                                    m_GuestSprites[i].GetComponent<UISprite>().spriteName = curTabGuest.Icon;
//                                }
//                                if ( i < m_GuestWindows.Length )
//                                {
//                                    m_GuestWindows[i].SetActive(true);
//                                }
//                            }
//                          
//                        }
                        //正在品尝菜肴：剩余时间：
                        m_LabelStateDetail.text = guestName + StrDictionary.GetClientDictionaryString("#{1983}", m_curDestData.GetLeftTime());
                        if (m_curDestData.GetFoodLeftTime() <= 0)
                        {
                            m_curDestData.m_DestState = RestaurantData.DeskState.WaitBilling;
                        }
                    }

                    break;
                case RestaurantData.DeskState.WaitBilling:
                    {
                        // 客人正在等待结账
                        PreFoodDestBk.SetActive(false);
                        EatFoodDestBk.SetActive(true);
			        	m_LaZhuSpr.SetActive(false);
                        m_LabelStateDetail.text = StrDictionary.GetClientDictionaryString("#{1984}");
//
//                        for (int i = 0; i < m_curDestData.m_GuestIDs.Length; i++)
//                        {
//                            int curGuestId = m_curDestData.m_GuestIDs[i];
//
//                            Tab_RestaurantGuest curTabGuest = TableManager.GetRestaurantGuestByID(Int32.Parse(m_strGuestIndexs[i]), 0);
//                            if (null ==  curTabGuest)
//                            {
//                                continue;
//                            }
//                            if (i < m_GuestNameLable.Length)
//                            {
//                                m_GuestNameLable[i].text = curTabGuest.Name;
//                                Guest_Quality quality = (Guest_Quality)curTabGuest.Quality;
//                                switch (quality)
//                                {
//                                    case Guest_Quality.QUALITY_WHITE:
//                                        m_GuestNameLable[i].color = m_WhiteQualityColor;
//                                        break;
//                                    case Guest_Quality.QUALITY_GREEN:
//                                        m_GuestNameLable[i].color = m_GreenQualityColor;
//                                        break;
//                                      case Guest_Quality.QUALITY_BLUE:
//                                        m_GuestNameLable[i].color = m_BlueQualityColor;
//                                        break;
//                                      case Guest_Quality.QUALITY_PURPLE:
//                                        m_GuestNameLable[i].color = m_PurpleQualityColor;
//                                        break;
//                                      case Guest_Quality.QUALITY_ORANGE:
//                                        m_GuestNameLable[i].color = m_OrangeQualityColor;
//                                        break;
//                                }
//                            }
//                            if (i < m_GuestSprites.Length)
//                            {                               
//                                m_GuestSprites[i].GetComponent<UISprite>().spriteName = curTabGuest.Icon;
//                            }
//                            if ( i < m_GuestWindows.Length )
//                            {
//                                m_GuestWindows[i].SetActive(true);
//                            }
//                        }
                    }
                    break;
                default:
                    {
                        // 请为餐桌准备菜肴
                        m_LabelStateDetail.text = StrDictionary.GetClientDictionaryString("#{1985}");
                        PreFoodDestBk.SetActive(true);
                        EatFoodDestBk.SetActive(false);
				        m_LaZhuSpr.SetActive(false);
//                        for (int i = 0; i < m_GuestWindows.Length; i++)
//                        {
//                            m_GuestWindows[i].SetActive(false);
//                        }
                    }
                    break;
            }
        }
        else
        {
            m_LabelStateDetail.text = "-1";
        }
    }
	
	public void SetIndex(RestaurantData.RestaurantInfo curRestaurant, int deskIndex)
    {
        m_curDeskIndex = deskIndex;
        m_curRestaurantData = curRestaurant;
        if (null == m_BtnLock)
        {
            LogModule.ErrorLog("m_BtnLock is null");
            return;
        }
        if (null == m_BtnNormal)
        {
            LogModule.ErrorLog("m_BtnNormal is null");
            return;
        }
        if (null == m_LabelUnLockTip)
        {
            LogModule.ErrorLog("m_LabelUnLockTip is null");
            return;
        }
        if (null ==  m_curRestaurantData)
        {
            LogModule.ErrorLog("m_curRestaurantData is null");
            return;
        }
        if (m_curDeskIndex >= m_curRestaurantData.m_Desks.Length)
        {
            m_BtnLock.SetActive(true);
            m_BtnNormal.SetActive(false);
            LogModule.ErrorLog("cur index is big than define " + deskIndex.ToString());
            return;
        }

        m_curTabDesk = TableManager.GetRestaurantDeskByID(deskIndex, 0);
        if (null == m_curTabDesk)
        {
            LogModule.ErrorLog("cur desk is not define in table :" + deskIndex);
            return;
        }
        m_curDestData = m_curRestaurantData.m_Desks[deskIndex];
        if (null == m_curDestData)
        {
            LogModule.ErrorLog("m_curDestData is null");
            return;
        }
        m_LabelUnLockTip.text = "";

        switch (m_curTabDesk.OpenConditionType)
        {
            case 1:
                // 等级
                m_LabelUnLockTip.text = StrDictionary.GetClientDictionaryString("#{1927}", m_curTabDesk.OpenConditionValue);
                break;
            case 2:
                // 元宝
                m_LabelUnLockTip.text = StrDictionary.GetClientDictionaryString("#{1929}", m_curTabDesk.OpenConditionValue);
                break;
            case 3:
                // VIP
                m_LabelUnLockTip.text = StrDictionary.GetClientDictionaryString("#{1928}", m_curTabDesk.OpenConditionValue);
                break;
            default:
                m_LabelUnLockTip.text = "";
                m_BtnLock.SetActive(false);
                m_BtnNormal.SetActive(true);
                break;
        }
//        if (RestaurantData.DeskState.EatFood == m_curDestData.m_DestState ||
//            RestaurantData.DeskState.WaitBilling == m_curDestData.m_DestState)
//        {
//            for (int i = 0; i < RestaurantData.GuestCountMax; i++)
//            {
//                m_strGuestIndexs[i] = m_curDestData.m_GuestIDs[i].ToString();
//            }
//        }
//        else
//        {
//            for (int i = 0; i < RestaurantData.GuestCountMax; i++)
//            {
//                m_strGuestIndexs[i] = "";
//            }
//        }

        UpdateDestInfo();
    }
    void OnDeskClick()
    {
        if (null == m_curTabDesk || null == m_curDestData)
        {
            LogModule.ErrorLog("m_curTabDesk is null or m_curDestData is null");
            return;
        }
        if (null == RestaurantController.Instance())
        {
            LogModule.ErrorLog("OnDeskClick：：RestaurantController.Instance() is null ");
            return;
        }
        if (!RestaurantController.Instance().SelfData)
        {
			GUIData.AddNotifyData2Client(false,"#{2017}");
            return;
        }

        if (!m_curDestData.m_IsActive)
        {
            string strTip;
            switch (m_curTabDesk.OpenConditionType)
            {
                case 1:
                    // 等级 少侠莫急，到达{0}级会开启此桌位
                    strTip = StrDictionary.GetClientDictionaryString("#{1986}", m_curTabDesk.OpenConditionValue);
                    MessageBoxLogic.OpenOKBox(strTip);
                    break;
                case 2:
                    // 元宝   解锁该桌位需要花费{0}元宝，是否开启?
                    strTip = StrDictionary.GetClientDictionaryString("#{1987}", m_curTabDesk.OpenConditionValue);
                    MessageBoxLogic.OpenOKCancelBox(strTip, "", OnActiveDesk);
                    break;
                case 3:
                    // VIP 少侠莫急，到达VIP{0}会开启此桌位
                    strTip = StrDictionary.GetClientDictionaryString("#{1988}", m_curTabDesk.OpenConditionValue);
                    MessageBoxLogic.OpenOKBox(strTip);
                    break;
                default:
                    LogModule.ErrorLog("the open condition type is not defined " + m_curTabDesk.OpenConditionType);
                    break;
            }
        }
        else
        {
            if (m_curDestData.m_DestState == RestaurantData.DeskState.None)
            {
                RestaurantController.Instance().OpenFoodWindow(this);
            }
            else if (m_curDestData.m_DestState == RestaurantData.DeskState.PrepareFood)
            {
                FinishPrepareDesk();
            }
            else if (m_curDestData.m_DestState == RestaurantData.DeskState.WaitBilling)
            {
                RestaurantController.Instance().BillingDesk(this);
            }
            return;
        }
    }

    void OnLockBtnClick()
    {
        if (null == m_curTabDesk || null == m_curDestData)
        {
            return;
        }

        if (!m_curDestData.m_IsActive)
        {
            string strTip;
            switch (m_curTabDesk.OpenConditionType)
            {
                case 1:
                    // 等级 少侠莫急，到达{0}级会开启此桌位
                    strTip = StrDictionary.GetClientDictionaryString("#{1986}", m_curTabDesk.OpenConditionValue);
                    MessageBoxLogic.OpenOKBox(strTip);
                    break;
                case 2:
                    // 元宝   解锁该桌位需要花费{0}元宝，是否开启?
                    strTip = StrDictionary.GetClientDictionaryString("#{1987}", m_curTabDesk.OpenConditionValue);
                    MessageBoxLogic.OpenOKCancelBox(strTip, "", OnActiveDesk);
                    break;
                case 3:
                    // VIP 少侠莫急，到达VIP{0}会开启此桌位
                    strTip = StrDictionary.GetClientDictionaryString("#{1988}", m_curTabDesk.OpenConditionValue);
                    MessageBoxLogic.OpenOKBox(strTip);
                    break;
                default:
                    LogModule.ErrorLog("the open condition type is not defined " + m_curTabDesk.OpenConditionType);
                    break;
            }
        }
    }

    void OnNormalBtnClick()
    {
        if (null == m_curTabDesk || null == m_curDestData)
        {
            return;
        }
        if (null == RestaurantController.Instance())
        {
            LogModule.ErrorLog("OnNormalBtnClick::RestaurantController.Instance() is null ");
            return;
        }

        if (m_curDestData.m_IsActive)
        {
            if (m_curDestData.m_DestState == RestaurantData.DeskState.None)
            {
                RestaurantController.Instance().OpenFoodWindow(this);
            }
            else if (m_curDestData.m_DestState == RestaurantData.DeskState.PrepareFood)
            {
                FinishPrepareDesk();
            }
            else if (m_curDestData.m_DestState == RestaurantData.DeskState.WaitBilling)
            {
                RestaurantController.Instance().BillingDesk(this);
            }
            return;
        }
    }

    void FinishPrepareDesk()
    {
        if (null == RestaurantController.Instance())
        {
            LogModule.ErrorLog("FinishPrepareDesk::RestaurantController.Instance() is null ");
            return;
        }
        if (!RestaurantController.Instance().SelfData)
        {
			GUIData.AddNotifyData2Client(false,"#{2017}");
            return;
        }
        if (null == m_curDestData)
        {
            LogModule.ErrorLog("FinishPrepareDesk:: m_curDestData is null");
            return;
        }
        int nTodayFinishNum = GameManager.gameManager.PlayerDataPool.CommonData.GetCommonData((int)Games.UserCommonData.USER_COMMONDATA.CD_RESTAURANT_FINISHFOOD_NUM);
        if (nTodayFinishNum >= RestaurantData.FinishFoodMax)
        {
            MessageBoxLogic.OpenOKBox(1563, 1000);
            return;
        }

        if (null == m_curDestData)
        {
            LogModule.ErrorLog("FinishPrepareDesk:: m_curDestData is null");
            return;
        }

        Tab_RestaurantFood curTableFood = TableManager.GetRestaurantFoodByID(m_curDestData.m_FoodID, 0);
        if (null == curTableFood)
        {
            LogModule.ErrorLog("FinishPrepareDesk:: curTableFood is null");
            return;
        }

        int nCostYuanBao = 0;
        int nLeftTenMinues = m_curDestData.GetFoodLeftTime() / 600;
        if (m_curDestData.GetFoodLeftTime() % 600 != 0)
        {
            nLeftTenMinues = nLeftTenMinues + 1;
        }
        if (nLeftTenMinues <= 0)
        {
            nLeftTenMinues = 1;
        }
        nCostYuanBao = nLeftTenMinues * curTableFood.CostYuanBao;
        string strTip = StrDictionary.GetClientDictionaryString("#{2339}", (int)nCostYuanBao);
        string strTitle = StrDictionary.GetClientDictionaryString("#{1000}");
        MessageBoxLogic.OpenOKCancelBox(strTip, strTitle, OnFinishPrepareDesk, null);
    }

     void OnFinishPrepareDesk()
    {
        CG_RESTAURANT_FINISHPREPARE packet = (CG_RESTAURANT_FINISHPREPARE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_RESTAURANT_FINISHPREPARE);
        packet.None = 0;
        packet.SetDeskIndex( Index);
        packet.SendPacket();
    }

    void OnActiveDesk()
    {
        if (null == m_curDestData)
        {
            LogModule.ErrorLog("OnActiveDesk:: m_curDestData is null");
            return;
        }
        if (null == m_curTabDesk)
        {
            LogModule.ErrorLog("OnActiveDesk:: m_curTabDesk is null");
            return;
        }
        int nCostValue = m_curTabDesk.OpenConditionValue;
        int nPlayerYuanBao = GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBao();
        int nPlayerBindYuanBao = GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBaoBind();
        int nTotalYuanBao = nPlayerBindYuanBao + nPlayerYuanBao;
        if ( nTotalYuanBao < nCostValue )
        {
			GUIData.AddNotifyData2Client(false,"#{1018}");
            return;
        }             
        CG_RESTAURANT_ACTIVEDESK packet = (CG_RESTAURANT_ACTIVEDESK)PacketDistributed.CreatePacket(MessageID.PACKET_CG_RESTAURANT_ACTIVEDESK);
        packet.SetDeskIndex(m_curDeskIndex);
        packet.SendPacket();
    }
}
