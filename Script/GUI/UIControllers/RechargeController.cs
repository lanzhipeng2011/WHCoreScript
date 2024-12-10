using UnityEngine;
using System.Collections;
using GCGame.Table;
using Module.Log;
using System.Collections.Generic;
using Games.LogicObj;

public class RechargeController : UIControllerBase<RechargeController> {

    public GameObject m_ObjRechargeItem;
    public Transform m_TransRechargeGrid;

    public UILabel m_LabelYuanbao;
    public UILabel m_LabelBindYuanbao;

    public UILabel m_LabelYBPrizeRate;

    private float m_lastPayTime = 0;
    void Awake()
    {
        SetInstance(this);
    }
	// Use this for initialization
	void Start () {
        UpdateMoney();
        UpdateYBPrizeRate();
        if (RechargeData.m_dicGoodInfos.Count == 0)
        {
            PlatformHelper.ReqPaymentGoodInfoList();
        }
        else
        {
            UpdateRechargeList();
        }

        this.UpdateRechargeList();
        GUIData.delMoneyChanged += UpdateMoney;
        
	}
	
	// Update is called once per frame
	void OnDestroy ()
    {
        GUIData.delMoneyChanged -= UpdateMoney;
        SetInstance(null);
	}

    void OnCancelClick()
    {
        UIManager.CloseUI(UIInfo.Recharge);
    }

    void OnRechargeRecord()
    {
        PlatformHelper.ShowRechargeRecord();
    }

    void OnRechargeError()
    {
        PlatformListener.SendCYPay(2);
    }

    public void UpdateMoney()
    {
        m_LabelYuanbao.text = GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBao().ToString();
        m_LabelBindYuanbao.text = GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBaoBind().ToString();
    }

    public void UpdateYBPrizeRate()
    {
        float nRate = GameManager.gameManager.PlayerDataPool.PayActivity.GetYBPrizeRate();
        m_LabelYBPrizeRate.text = StrDictionary.GetClientDictionaryString("#{3061}", nRate.ToString());
    }

    public void UpdateRechargeList()
    {
        GCGame.Utils.CleanGrid(m_TransRechargeGrid.gameObject);

        Obj_MainPlayer mainPlayer = Singleton<ObjManager>.Instance.MainPlayer;
        if (null == mainPlayer)
        {
            LogModule.ErrorLog("main player is not init");
            return;
        }
        int i = 0;
        foreach(KeyValuePair<int,List<Tab_Recharge>> item in TableManager.GetRecharge())
        {
            Tab_Recharge curTabRecharge = item.Value[0];
           if (curTabRecharge==null||!RechargeData.m_dicGoodInfos.ContainsKey(curTabRecharge.RegisterID))
           {
             continue;
           }

           RechargeData.GoodInfo curGoodInfo = RechargeData.m_dicGoodInfos[curTabRecharge.RegisterID];

           string iconName = "";
           string goodName = curGoodInfo.goods_number + "#y";
           bool bShowOnlyOnce = false; // 仅卖一次标识
           bool bEnableTimes = false;

            #region
            /*
            if (curTabRecharge.Type == 1)
            {
                if (GameManager.gameManager.PlayerDataPool.PayActivity.IsMonthCardFlag())
                {
                    // 月卡并且已经生效
                  //  continue;
                }
                else
                {
                    bShowOnlyOnce = true;
                }
            }
            else if (curTabRecharge.Type == 2)
            {
                if (GameManager.gameManager.PlayerDataPool.PayActivity.IsGrowUpFlag() || mainPlayer.BaseAttr.Level >= 50)
                {
                    // 成长基金并且已经生效或者玩家已经大于50级
                   // continue;
                }
                else
                {
                    bShowOnlyOnce = true;
                }
            }
            else if (curTabRecharge.Type == 3)
            {
                bEnableTimes = true;
            }
            */
            #endregion

           iconName = curTabRecharge.IconName;
           goodName = curTabRecharge.GoodName;
           //RechargeItem.CreateItem(m_ObjRechargeItem, m_TransRechargeGrid, curGoodInfo.goods_register_id, iconName, curGoodInfo.goods_price, goodName, bShowOnlyOnce, bEnableTimes);
           RechargeItem.CreateItem(m_ObjRechargeItem, m_TransRechargeGrid, curTabRecharge.RegisterID,curTabRecharge.IconName, curGoodInfo.goods_price, curTabRecharge.GoodName,false, false);
           i++;
        }

        UIGrid curGrid = m_TransRechargeGrid.GetComponent<UIGrid>();

        if (null != curGrid)
        {
            curGrid.repositionNow = true;
        }
    }

    public void OnMakePay(string id, int times)
    {
        /*
        float timeDiff = Time.time - m_lastPayTime;
        if (timeDiff < 3)
        {
            Obj_MainPlayer mainPlayer = Singleton<ObjManager>.Instance.MainPlayer;
            if (null != mainPlayer)
            {
                mainPlayer.SendNoticMsg(false, "#{1076}");
            }
            return;
        }

        m_lastPayTime = Time.time;

        RechargeData.GoodInfo curGoodInfo = RechargeData.GetGoodInfo(id);
        if (null == curGoodInfo)
        {
            LogModule.ErrorLog(" can not find cur good info:" + id.ToString());
            return;
        }

        if (times > 1)
        {
            curGoodInfo = curGoodInfo.GetGoodInfoWithTimes(times);
        }

        if (null == curGoodInfo)
        {
            LogModule.ErrorLog("pay stop:good info error");
            return;
        }

        PlatformHelper.MakePayWithGoodInfo(curGoodInfo);
         * */

//        int serverId = PlayerPreferenceData.LastServer;
//        PayOrderId = System.Guid.NewGuid().ToString();


		#if CommonSDK
		RechargeData.GoodInfo curGoodInfo = RechargeData.GetGoodInfo(id);

		CG_PAY_ORDER packet = (CG_PAY_ORDER)PacketDistributed.CreatePacket(MessageID.PACKET_CG_PAY_ORDER);
		//uint.Parse((string)jd["PlatformId"]);
		packet.SetAmount ((uint)times);
		packet.SetGoods_id (uint.Parse(curGoodInfo.goods_id));
		packet.SetGoods_name (curGoodInfo.goods_name);
		packet.SetGoods_desc (curGoodInfo.goods_describe);
		packet.SetAccount (CommonSDKPlaform.Instance.GetUID ());
		packet.SendPacket();
		return;

		#endif
        PlatformHelper.OnChargeRequest(PayOrderId, id.ToString(),times,"CNY",times*100,"Default");
        
    }
    
    //TODO 暂时在这里加一个临时的orderId
    public string PayOrderId;

    Tab_Recharge GetRechargeData(string registerID)
    {
        foreach(KeyValuePair<int, List<Tab_Recharge>> curPair in TableManager.GetRecharge())
        {
            if(curPair.Value[0].RegisterID == registerID)
            {
                return curPair.Value[0];
            }
        }

        return null;
    }
}
