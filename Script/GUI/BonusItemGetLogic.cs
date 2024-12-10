/************************************************************************/
/* 文件名：BonusitemGetLogic.cs    
 * 创建日期：2014.07.15
 * 创建人：gaona
 * 功能说明：抽奖结果显示界面
/************************************************************************/
using UnityEngine;
using System;
using System.Collections;
using Module.Log;
using Games.DailyLuckyDraw;
using GCGame.Table;
using Games.Item;
using GCGame;

public class BonusItemGetLogic : UIControllerBase<BonusItemGetLogic> {
    
     public enum BONUSTYPE
    {
        TYPE_INVALIDID = -1,
        TYPE_DAILYLUKCYDRAW = 0,//每日幸运抽奖
    }

    public const int m_nMaxItemIconCount = 10;//最多显示的物品数
    public const float m_fItemIconTweenTime = 0.15f;//tween效果持续时间
    public UISprite[] m_ItemIconArray;
	public UILabel[] m_ItemNameArray;
	public UISprite[] m_ItemObjArray;
    public UISprite[] m_ItemImpactArray;
    public UISprite[] m_ItemQualityArray;
    public UISprite m_BoxSprite;
    public GameObject m_BoxObj;
    public UIImageButton m_CancleButton;
	public UIImageButton m_ContinueButton;
	public UILabel       m_ContinueButtonLabel;
    public Animation m_BoxLightAnimation;
    public Animation[] m_StarAnimationArray;
	public Animation[] m_StarFlyAnimationArray;
	public UISprite[] m_StarSpriteArray;
	public UISprite[] m_StarFLySpriteArray;

    public TweenPosition[] m_ItemObjTweenPosition;
	public TweenRotation[] m_ItemObjTweenRotation;
	public TweenScale[] m_ItemObjTweenScale;

	public TweenPosition[] m_StarFlyTweenPosition;
	public TweenScale[] m_StarFlyTweenScale;
	public TweenAlpha[] m_StarFlyTweenAlpha; 
	   
    private Vector3 m_InitPosition;  //物品图标最终位置
    private Vector3 m_InitRotation;  //物品图标初始旋转角度
    private Vector3 m_FinalRotation; //物品图标最终旋转角度
    private Vector3 m_InitScale;//物品图标初始大小
    private Vector3 m_FinalScale;//物品图标最终大小
    //物品图标最终位置
	private Vector3[] m_ItemIconPositionArray = new Vector3[m_nMaxItemIconCount] 
	{new Vector3(-400f, 125f, 0f),new Vector3(-200f, 125f, 0f),new Vector3(0f, 125f, 0f),new Vector3(200f, 125f, 0f),new Vector3(400f, 125f, 0f),
		new Vector3(-400f, -75f, 0f),new Vector3(-200f, -75f, 0f),new Vector3(0f, -75f, 0f),new Vector3(200f, -75f, 0f), new Vector3(400f, -75f, 0f)};
    private bool[] m_ShowItemImpctArray = new bool[m_nMaxItemIconCount];
    private string[] m_ItemQualityName = new string[m_nMaxItemIconCount];
    private int m_nItemIconShowCount;
	private int m_nCurItemIconShow;
    private BonusItemGetLogic.BONUSTYPE m_BonusType;
    private float m_fFrameTimeDiff;
    private const float m_fShowIconTime = 0.35f;
    private bool m_bShowItemObj;
    public bool ShowItemObj
    {
        get { return m_bShowItemObj; }
        set { m_bShowItemObj = value; }
    }

    private class BonusDataInfo
    {
        public BonusDataInfo(BONUSTYPE type)
        {
            m_type = type;
        }
        public BONUSTYPE m_type;
    }
    void OnEnable ()
    {
        SetInstance(this);
    }
    void OnDisable()
    {
        SetInstance(null);

        HideStars();
        if (m_BoxObj)
        {
            m_BoxObj.transform.localPosition = Vector3.zero;
        }
        if (m_BoxSprite)
        {
            m_BoxSprite.spriteName = "baoxiang01";
        }
    }
    //// Update is called once per frame
    void Update () {
        if (ShowItemObj)
        {
            m_fFrameTimeDiff += Time.deltaTime;

            if (m_fFrameTimeDiff >= m_fShowIconTime)
            {
                m_fFrameTimeDiff = 0f;
                ShowItemIcon();
            }
        }
    }

	public static void InitBonusInfo( BONUSTYPE type)
	{
		BonusDataInfo curInfo = new BonusDataInfo(type);
		UIManager.ShowUI(UIInfo.BonusItemGetRoot, OnLoadBonusItemGetRoot, curInfo);
	}

    static void OnLoadBonusItemGetRoot(bool bSucess,object param)
    {
        if (BonusItemGetLogic.Instance() != null)
        {
            BonusDataInfo curInfo = param as BonusDataInfo;
            BonusItemGetLogic.Instance().Init(curInfo);
        }
    }
    void Init(BonusDataInfo curInfo)
    {
        if (null != GameManager.gameManager.SoundManager)
            GameManager.gameManager.SoundManager.PlaySoundEffect(117); 

        InitBonusData();
        InitItemButtonPosition();
		ResetAllItemObj();
        HideAllButton();
        InitBoxAnimation();

        m_BonusType = curInfo.m_type;
        
        SetItemButtonImage(m_BonusType);
    }

    //初始化数值
    void InitBonusData()
    {
        m_BonusType = BONUSTYPE.TYPE_INVALIDID;
		m_nItemIconShowCount = 0;
		m_nCurItemIconShow = 0;
        m_fFrameTimeDiff = 0;
        ShowItemObj = false;
    }
    //初始化坐标位置
    void InitItemButtonPosition()
    {
        m_InitPosition = new Vector3(0f, 200f, 0f);
        m_InitRotation = new Vector3(0f, 0f, 180f);
        m_FinalRotation = new Vector3(0f, 0f, 0f);
        m_InitScale = new Vector3(0f,0f,0f);
        m_FinalScale = new Vector3(1f,1f,1f);
    } 
    //重置控件
	void ResetAllItemObj()
    {
		HideAllItemObj();
		ResetAllItemObjPosition();      
		ResetAllItemObjTween ();
    }
	//重置物品图标Tween
	void ResetAllItemObjTween()
	{
		for (int nIndex = 0; nIndex < m_nMaxItemIconCount; nIndex++)
		{
			if(m_ItemObjTweenPosition[nIndex])
			{
				m_ItemObjTweenPosition[nIndex].Reset();
			}
			if(m_ItemObjTweenRotation[nIndex])
			{
				m_ItemObjTweenRotation[nIndex].Reset();
			}
			if(m_ItemObjTweenScale[nIndex])
			{
				m_ItemObjTweenScale[nIndex].Reset();
			}
		}
	}
    //重置物品图标位置
	void ResetAllItemObjPosition()
    {
        for (int nIndex = 0; nIndex < m_nMaxItemIconCount; nIndex++)
        {
			if (m_ItemObjArray[nIndex])
            {
				m_ItemObjArray[nIndex].gameObject.transform.localPosition = m_InitPosition;
            }
        }
    }
    //隐藏所有物品图标
	void HideAllItemObj()
    {
        for (int nIndex = 0; nIndex < m_nMaxItemIconCount; nIndex++)
        {
			if(m_ItemObjArray[nIndex])
			{
				m_ItemObjArray[nIndex].gameObject.SetActive(false);
			}
            if (m_ItemIconArray[nIndex])
            {
                m_ItemIconArray[nIndex].gameObject.SetActive(false);
            }
			if (m_ItemNameArray[nIndex])
			{
				m_ItemNameArray[nIndex].gameObject.SetActive(false);
			}
            if (m_ItemImpactArray[nIndex])
            {
                m_ItemImpactArray[nIndex].gameObject.SetActive(false);
            }
            if (m_ItemQualityArray[nIndex])
            {
                m_ItemQualityArray[nIndex].gameObject.SetActive(false);
            }
            
        }
    }
    //初始化宝箱动画
    void InitBoxAnimation()
    {
        if (m_BoxLightAnimation)
        {
            m_BoxLightAnimation.animation.wrapMode = WrapMode.Loop;
        }
        for (int nIndex = 0; nIndex < m_StarAnimationArray.Length; nIndex++ )
        {
            if (m_StarAnimationArray[nIndex])
            {
                m_StarAnimationArray[nIndex].animation.wrapMode = WrapMode.Loop;
            }
        }
		for (int nIndex = 0; nIndex < m_StarFlyAnimationArray.Length; nIndex++ )
		{
			if (m_StarFlyAnimationArray[nIndex])
			{
				m_StarFlyAnimationArray[nIndex].animation.wrapMode = WrapMode.Loop;
			}
		}
    }
    //显示物品图标动画
	void ShowItemObjTween(int nIndex)
    {
        if (nIndex >= 0 && nIndex < m_nMaxItemIconCount)
        {
			if (m_ItemObjArray[nIndex] && m_ItemIconArray[nIndex])
             {
				m_ItemObjArray[nIndex].gameObject.SetActive(true);
                m_ItemIconArray[nIndex].gameObject.SetActive(true);

				if (null == m_ItemObjTweenPosition[nIndex])
                 {
					m_ItemObjTweenPosition[nIndex] = m_ItemObjArray[nIndex].gameObject.AddComponent<TweenPosition>();
                 }
				if (null != m_ItemObjTweenPosition[nIndex])
                 {
					EventDelegate.Add(m_ItemObjTweenPosition[nIndex].onFinished, OnTweenPositionFinished);
					m_ItemObjTweenPosition[nIndex].from = m_InitPosition;
					if(1 == m_nItemIconShowCount)
					{
						m_ItemObjTweenPosition[nIndex].to = new Vector3(0f, 25f, 0f);
					}
					else
					{
						m_ItemObjTweenPosition[nIndex].to = m_ItemIconPositionArray[nIndex];
					}
					m_ItemObjTweenPosition[nIndex].duration = m_fItemIconTweenTime;
                    m_ItemObjTweenPosition[nIndex].Reset();
					m_ItemObjTweenPosition[nIndex].Play(true);
                 }

				if (null == m_ItemObjTweenRotation[nIndex])
                 {
					m_ItemObjTweenRotation[nIndex] = m_ItemObjArray[nIndex].gameObject.AddComponent<TweenRotation>();
                 }
				if (null != m_ItemObjTweenRotation[nIndex])
                 {
					m_ItemObjTweenRotation[nIndex].from = m_InitRotation;
					m_ItemObjTweenRotation[nIndex].to = m_FinalRotation;
					m_ItemObjTweenRotation[nIndex].duration = m_fItemIconTweenTime;
                    m_ItemObjTweenRotation[nIndex].Reset();
					m_ItemObjTweenRotation[nIndex].Play(true);
                 }

				if (null == m_ItemObjTweenScale[nIndex])
                 {
					m_ItemObjTweenScale[nIndex] = m_ItemObjArray[nIndex].gameObject.AddComponent<TweenScale>();
                 }
				if (null != m_ItemObjTweenScale[nIndex])
                 {
					m_ItemObjTweenScale[nIndex].from = m_InitScale;
					m_ItemObjTweenScale[nIndex].to = m_FinalScale;
					m_ItemObjTweenScale[nIndex].duration = m_fItemIconTweenTime;
                    m_ItemObjTweenScale[nIndex].Reset();
					m_ItemObjTweenScale[nIndex].Play(true);
                 }
             }
        }
    }
	void ShowBoxObjTween()
	{
		if (m_BoxObj)
		{
			TweenPosition BoxTweenPosition = m_BoxObj.GetComponent<TweenPosition>();
			if (BoxTweenPosition)
			{
				EventDelegate.Add(BoxTweenPosition.onFinished, BoxObjTweenFinish);
                BoxTweenPosition.Reset();
                BoxTweenPosition.Play(true);
			}
			TweenScale BoxTweenScale = m_BoxObj.GetComponent<TweenScale>();
			if (BoxTweenScale)
			{
                BoxTweenScale.Reset();
				BoxTweenScale.Play(true);
			}
		}
	}
    //显示开箱图片
    public void ShowOpenBoxAnimation()
	{
		ShowOpenBoxSprite ();
		ShowFlyStars ();
	}
	void ShowOpenBoxSprite()
    {
        if (m_BoxSprite)
        {
            m_BoxSprite.spriteName = "baoxiang02";
        }
    }
	//显示飞的小星星
	void ShowFlyStars()
	{
		for (int nIndex = 0; nIndex < m_StarFLySpriteArray.Length; nIndex++) 
		{
			m_StarFLySpriteArray[nIndex].gameObject.SetActive(true);

			TweenPosition StarFlyTweenPosition = m_StarFlyTweenPosition[nIndex];
			if (StarFlyTweenPosition)
			{
				if(nIndex == 0)
				{
					//只增加一个结束处理函数
					EventDelegate.Add(StarFlyTweenPosition.onFinished, StarFlyTweenFinish);
				}
				StarFlyTweenPosition.Reset();
				StarFlyTweenPosition.Play(true);
			}
			TweenScale StarFlyTweenScale = m_StarFlyTweenScale[nIndex];
			if (StarFlyTweenScale)
			{
				StarFlyTweenScale.Reset ();
				StarFlyTweenScale.Play(true);
			}

			TweenAlpha StarFlyTweenAlhpa= m_StarFlyTweenAlpha[nIndex];
			if (StarFlyTweenAlhpa)
			{
				StarFlyTweenAlhpa.Reset ();
				StarFlyTweenAlhpa.Play(true);
			}
		}
	}
	//隐藏飞行小星星
	void HideAllFlyStar()
	{
		for (int nIndex = 0; nIndex < m_StarFLySpriteArray.Length; nIndex++) 
		{
			if(m_StarFLySpriteArray [nIndex])
			{
				m_StarFLySpriteArray [nIndex].gameObject.SetActive (false);
			}
		}
	}
	//小星星飞结束
	void StarFlyTweenFinish()
	{
		HideAllFlyStar ();
		ShowBoxObjTween();
	}
	//显示小星星
	void ShowStars()
	{
		for (int nIndex = 0; nIndex < m_StarSpriteArray.Length; nIndex++) 
		{
			if(m_StarSpriteArray[nIndex])
			{
				m_StarSpriteArray[nIndex].gameObject.SetActive(true);
			}
		}
	}
    //隐藏小星星
    void HideStars()
    {
        for (int nIndex = 0; nIndex < m_StarSpriteArray.Length; nIndex++)
        {
            if (m_StarSpriteArray[nIndex])
            {
                m_StarSpriteArray[nIndex].gameObject.SetActive(false);
            }
        }
    }
    //开箱动画结束
    void BoxObjTweenFinish()
    {
		ShowStars();
        ShowItemObj = true;
    }
	//物品图标动画结束
	void OnTweenPositionFinished()
	{
		ShowItemName();
	}
	//显示物品名称
	void ShowItemName()
	{
		int nIndex = m_nCurItemIconShow - 1;

		if(nIndex >= 0 && nIndex <m_nMaxItemIconCount)
		{
            if (m_ItemNameArray[nIndex])
	        {
                m_ItemNameArray[nIndex].gameObject.SetActive(true);
	        }
            if (m_ItemQualityArray[nIndex] && "" != m_ItemQualityName[nIndex])
            {
                m_ItemQualityArray[nIndex].spriteName = m_ItemQualityName[nIndex];
                m_ItemQualityArray[nIndex].gameObject.SetActive(true);
            }
            if ( m_ItemImpactArray[nIndex] && m_ShowItemImpctArray[nIndex] )
            {
                m_ItemImpactArray[nIndex].gameObject.SetActive(true);
                GameManager.gameManager.SoundManager.PlaySoundEffect(133);
            }
		}

	}
    //显示物品图标
	void ShowItemIcon()
    {
        //物品效果已经显示完毕
		if (m_nCurItemIconShow >= m_nItemIconShowCount)
        {
            //显示按钮
			ShowButtonAfterAllTween();
 
			//显示完效果后的处理
			ProcessAfterAllTween();

            //结束显示物品obj
            ShowItemObj = false;
            return;
        }

        //显示多个物品
		if (m_nCurItemIconShow < m_nItemIconShowCount)
        {
			ShowItemObjTween(m_nCurItemIconShow);
            GameManager.gameManager.SoundManager.PlaySoundEffect(131);
			m_nCurItemIconShow++;
        }
    }
	//显示按钮
	void ShowButtonAfterAllTween()
	{
		m_CancleButton.gameObject.SetActive(true);

		switch(m_BonusType)
		{
			case BONUSTYPE.TYPE_DAILYLUKCYDRAW:
			{
				ShowButtonAfterAllTween_DailyLukcyDraw();
			}
				break;
			default:
				break;
		}
	}
    //点击界面取消按钮
    void CancleButtonClick()
    {
        UIManager.CloseUI(UIInfo.BonusItemGetRoot);
		if (BackCamerControll.Instance() != null)
		{
			BackCamerControll.Instance().StopSceneEffect(138, true);
		}

		if (MainUILogic.Instance() != null)
		{
			UIManager.ShowUI(UIInfo.DivinationRoot);
		}
    }
	//点击界面继续按钮
	void CotinueButtonClick()
	{
		switch(m_BonusType)
		{
		case BONUSTYPE.TYPE_DAILYLUKCYDRAW:
		{
			CotinueButtonClick_DailyLukcyDraw();
		}
			break;
		default:
			break;
		}
	}
    //隐藏取消按钮
    void HideAllButton()
    {
        m_CancleButton.gameObject.SetActive(false);
		m_ContinueButton.gameObject.SetActive(false);
    }
    //根据奖励类型设置物品图标
    void SetItemButtonImage(BonusItemGetLogic.BONUSTYPE BonusType)
    {
        switch(BonusType)
        {
            case BONUSTYPE.TYPE_DAILYLUKCYDRAW:
                {
                    SetItemButtonImage_DailyLukcyDraw();
                }
                break;
            default:
                break;
        }
    }
	//增加物品信息
    void AddItemInfo(string ImageName,string ItemName,string ItemQuality,bool ShowImpact)
    {
        if ("" == ImageName || "" == ItemName)
        {
            return;
        }
		if (m_nItemIconShowCount >= m_nMaxItemIconCount)
        {
            return;
        }
		if (m_nItemIconShowCount >= 0 && m_nItemIconShowCount < m_nMaxItemIconCount)
        {
			if (m_ItemIconArray[m_nItemIconShowCount] && m_ItemNameArray[m_nItemIconShowCount])
            {
				m_ItemIconArray[m_nItemIconShowCount].spriteName = ImageName;
				m_ItemNameArray[m_nItemIconShowCount].text = ItemName;
                m_ItemQualityName[m_nItemIconShowCount] = ItemQuality;
                m_ShowItemImpctArray[m_nItemIconShowCount] = ShowImpact;
				m_nItemIconShowCount++;
            }
        }
    }
	//所有动画结束后的处理
	void ProcessAfterAllTween()
	{
		switch(m_BonusType)
		{
		case BONUSTYPE.TYPE_DAILYLUKCYDRAW:
		{
			ProcessAfterAllTween_DailyLukcyDraw();
		}
			break;
		default:
			break;
		}
	}
	//每日幸运抽奖
	void SetItemButtonImage_DailyLukcyDraw()
	{
		int MaxGainBonusCount = GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.GetMaxGainBonusCount();
		//根据已获取的奖励ID，刷新奖励信息
		for (int i = 0; i < GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.GetMaxGainBonusCount(); i++)
		{
			int nBonusID = GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.GetGainBonusID(i);
			if(100 < nBonusID)
			{
				string ImageName = "jinbi";
				Tab_MoneyTree BonusInfo = TableManager.GetMoneyTreeByID(nBonusID - 100, 0);
				AddItemInfo(ImageName, BonusInfo.Money.ToString(),"", false);

			}
			else if (nBonusID > 0)
			{
				Tab_DailyLuckyDrawBonusInfo BonusInfo = TableManager.GetDailyLuckyDrawBonusInfoByID(nBonusID, 0);
				if (null == BonusInfo)
				{
					LogModule.DebugLog("DailyLuckyDrawBonusInfo.txt has not Line ID=" + nBonusID);
					return;
				}
                bool bShowItemImpact = (BonusInfo.RareDegree > (int)BONUSRAREDEGREE.DLD_BONUS_MIDDLE)?true:false;
				//金钱提示
				if (BonusInfo.MoneyCount > 0)
				{
					string ImageName = "";
					switch (BonusInfo.MoneyType)
					{
					case (int)MONEYTYPE.MONEYTYPE_YUANBAO:
						ImageName = "bdyuanbao";
						break;
					case (int)MONEYTYPE.MONEYTYPE_COIN:
						ImageName = "jinbi";
						break;
					case (int)MONEYTYPE.MONEYTYPE_YUANBAO_BIND:
						ImageName = "bdyuanbao";
						break;
					default:
						break;
					}
                    AddItemInfo(ImageName, BonusInfo.MoneyCount.ToString(),"", bShowItemImpact);
				}
				//物品提示
				for (int ItemIndex = 0; ItemIndex < BonusInfo.ItemNum; ItemIndex++)
				{
					int ItemID = BonusInfo.GetItemIDbyIndex(ItemIndex);
					int ItemCount = BonusInfo.GetItemCountbyIndex(ItemIndex);
					if (ItemID >= 0 && ItemCount > 0)
					{
						Tab_CommonItem ItemInfo = TableManager.GetCommonItemByID(ItemID, 0);
						if (null == ItemInfo)
						{
							LogModule.DebugLog("CommonItem.txt has not Line ID=" + ItemID);
							return;
						}
                        string strQualityIcon = "";
                        switch ((ItemQuality)ItemInfo.Quality)
                        {
							case ItemQuality.QUALITY_WHITE:
								strQualityIcon = "ui_pub_012";
								break;
							case ItemQuality.QUALITY_GREEN:
								strQualityIcon = "ui_pub_013";
								break;
							case ItemQuality.QUALITY_BLUE:
								strQualityIcon = "ui_pub_014";
								break;
							case ItemQuality.QUALITY_PURPLE:
								strQualityIcon = "ui_pub_015";
								break;
							case ItemQuality.QUALITY_ORANGE:
								strQualityIcon = "ui_pub_016";
								break;
							default:
								strQualityIcon = "QualityGrey";
								break;
                        }
                        AddItemInfo(ItemInfo.Icon, ItemInfo.Name, strQualityIcon, bShowItemImpact);
					}
				}
				//经验提示
				if (BonusInfo.Exp > 0)
				{
					AddItemInfo("jingyan",BonusInfo.Exp.ToString(),"",bShowItemImpact);
				}
			}
		}
	}
	//更新每日幸运抽奖界面
	void ProcessAfterAllTween_DailyLukcyDraw()
	{
		if (DailyLuckyDrawLogic.Instance())
		{
            if ((int)DLDDRAWTYPE.DLD_DRAWTYPE_ONE == GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawType ||
            (int)DLDDRAWTYPE.DLD_DRAWTYPE_ONE_YUANBAO == GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawType)
			{
				DailyLuckyDrawLogic.Instance().DrawOneTurnTableShow();
			}
			else if ((int)DLDDRAWTYPE.DLD_DRAWTYPE_TEN == GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawType)
			{
				DailyLuckyDrawLogic.Instance().DrawTenTurnTableShow();
			}
		}
	}
	//根据每日幸运抽奖类型，显示按钮文字
	void ShowButtonAfterAllTween_DailyLukcyDraw()
	{
        if (null == m_ContinueButtonLabel.text)
        {
            return;
        }
        m_ContinueButtonLabel.text = "";

        m_ContinueButton.gameObject.SetActive(true);

		if ((int)DLDDRAWTYPE.DLD_DRAWTYPE_ONE ==GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawType ||
            (int)DLDDRAWTYPE.DLD_DRAWTYPE_ONE_YUANBAO == GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawType)
		{
			m_ContinueButtonLabel.text = Utils.GetDicByID(5656);
		}
		else if ((int)DLDDRAWTYPE.DLD_DRAWTYPE_TEN == GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawType)
		{
			m_ContinueButtonLabel.text = Utils.GetDicByID(5657);
		}
		else if(3 == GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawType 
		        ||4 == GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawType)
		{
			m_ContinueButtonLabel.text = Utils.GetDicByID(5655);
		}
	}
	//每日幸运抽奖继续按钮点击处理
	void CotinueButtonClick_DailyLukcyDraw()
	{
		int DrawType = GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawType;
		switch (DrawType) 
		{
		case (int)DLDDRAWTYPE.DLD_DRAWTYPE_ONE:
		case (int)DLDDRAWTYPE.DLD_DRAWTYPE_ONE_YUANBAO:
			DoDiviTreasureOne();
			break;
		case (int)DLDDRAWTYPE.DLD_DRAWTYPE_TEN:
			DoDiviTreasure();
			break;
		default:
			DoDiviMoney();
			break;
		}

//		if (DailyLuckyDrawLogic.Instance())
//		{
//            if ((int)DLDDRAWTYPE.DLD_DRAWTYPE_ONE == GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawType ||
//            (int)DLDDRAWTYPE.DLD_DRAWTYPE_ONE_YUANBAO == GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawType)
//			{
//				DailyLuckyDrawLogic.Instance().DailyLuckyDrawOne();
//			}
//			else if ((int)DLDDRAWTYPE.DLD_DRAWTYPE_TEN == GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawType)
//			{
//				DailyLuckyDrawLogic.Instance().DailyLuckyDrawTen();
//			}
//		}
	}

	void DoDiviTreasureOne()
	{
		//判断条件
		if (GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawFreeTimes <= 0 || GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawFreeCDTime > 0)
		{
			int nYuanBaoCount = GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBao() + GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBaoBind();
			if ( nYuanBaoCount < 25)
			{
				Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1817}");
				return;
			}
			else
			{
				string dicStr = "";
				if (GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawFreeCDTime > 0)
				{
					dicStr = StrDictionary.GetClientDictionaryString("#{2411}", 25);
				}
				else
				{
					dicStr = StrDictionary.GetClientDictionaryString("#{1813}", 25);
				}
				MessageBoxLogic.OpenOKCancelBox(dicStr, "", DoDiviTreasure);
				GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawType = 2;
			}
		}
		else if (GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawFreeTimes > 0 && GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawFreeCDTime <= 0)
		{
			GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawType = 0;
			DoDiviTreasure();
		}
	}

	void DoDiviTreasure()
	{
		UIManager.CloseUI(UIInfo.BonusItemGetRoot);
		if (BackCamerControll.Instance() != null)
		{
			BackCamerControll.Instance().StopSceneEffect(138, true);
		}

		int DrawType = GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawType;
		CG_DAILYLUCKYDRAW_DRAW packet = (CG_DAILYLUCKYDRAW_DRAW)PacketDistributed.CreatePacket(MessageID.PACKET_CG_DAILYLUCKYDRAW_DRAW);
		packet.SetDrawtype((uint)DrawType);                  
		packet.SendPacket();
	}


	void DoDiviMoney()
	{
		if (GameManager.gameManager.PlayerDataPool.MoneyTreeData.CDTime > 0) 
		{
			int MoneyTreeId = GameManager.gameManager.PlayerDataPool.MoneyTreeData.YuanBaoAwardCount + 8;
			Tab_MoneyTree tab = TableManager.GetMoneyTreeByID(MoneyTreeId, 0);
			if(null == tab)
			{
				return;
			}
			
			int nNeedYuanbao = tab.BindYuanbao;
			if(nNeedYuanbao > GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBaoBind () 
			   + GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBao ())
			{
				Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{1522}");
				return;
			}
			
			int nAwardCoin = tab.Money;
			string str = "";
			str = StrDictionary.GetClientDictionaryString("#{2718}", nNeedYuanbao, nAwardCoin);
			MessageBoxLogic.OpenOKCancelBox(str, null, OnDiviMBuyOKClick, OnDiviMBuyCancelClick);
			return;
		}
		//===========如果在元宝抽取中同时具有免费次数则需删除界面后发送抽取消息
		UIManager.CloseUI(UIInfo.BonusItemGetRoot);
		if (BackCamerControll.Instance() != null)
		{
			BackCamerControll.Instance().StopSceneEffect(138, true);
		}

		GameManager.gameManager.PlayerDataPool.MoneyTreeData.SendAwardPacket(1, 1);
	}

	void OnDiviMBuyOKClick()
	{
		UIManager.CloseUI(UIInfo.BonusItemGetRoot);
		if (BackCamerControll.Instance() != null)
		{
			BackCamerControll.Instance().StopSceneEffect(138, true);
		}

		GameManager.gameManager.PlayerDataPool.MoneyTreeData.SendAwardPacket(1, 2);
	}
	
	void OnDiviMBuyCancelClick()
	{
		MessageBoxLogic.CloseBox();
	}
}
