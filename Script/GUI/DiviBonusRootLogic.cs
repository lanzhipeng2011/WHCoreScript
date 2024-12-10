using UnityEngine;
using System.Collections;
using Module.Log;
using Games.DailyLuckyDraw;
using Games.GlobeDefine;
using GCGame.Table;
using Games.Item;
using GCGame;

public class DiviBonusRootLogic : UIControllerBase<DiviBonusRootLogic> {

	//物品图标最终位置
	public const int m_nMaxItemIconCount = 10;//最多显示的物品数
	public const float m_fItemIconTweenTime = 0.15f;//tween效果持续时间
	private Vector3[] m_ItemIconPositionArray = new Vector3[m_nMaxItemIconCount] 
		{
			new Vector3(-400f, 125f, 0f),	new Vector3(-200f, 125f, 0f),
			new Vector3(0f, 125f, 0f),		new Vector3(200f, 125f, 0f),
			new Vector3(400f, 125f, 0f),	new Vector3(-400f, -75f, 0f),
			new Vector3(-200f, -75f, 0f),	new Vector3(0f, -75f, 0f),
			new Vector3(200f, -75f, 0f), 	new Vector3(400f, -75f, 0f)
		};

	public DiviBonusItemLogic[] m_ItemObjArray;
	
	public UIImageButton m_CancleButton;
	public UIImageButton m_ContinueButton;
	public UILabel       m_ContinueButtonLabel;

	private Vector3 m_InitPosition;  //物品图标最终位置
	private Vector3 m_InitRotation;  //物品图标初始旋转角度
	private Vector3 m_FinalRotation; //物品图标最终旋转角度
	private Vector3 m_InitScale;//物品图标初始大小
	private Vector3 m_FinalScale;//物品图标最终大小
	private Vector3 m_CenterPosition;

	private float m_InVakeTime = 0.4f;
	
	private int m_BonusType;
	private const float m_fShowIconTime = 0.25f;
	private float m_fFrameTimeDiff;
	
	private int m_nCurItemShow;
	private int m_nShowItemNum;

	private bool m_bShowItemObj;
	public bool ShowItemObj
	{
		get { return m_bShowItemObj; }
		set { m_bShowItemObj = value; }
	}

	private int m_NewPlayerGuide_Step = -1;
	public int NewPlayerGuide_Step
	{
		get { return m_NewPlayerGuide_Step; }
		set { m_NewPlayerGuide_Step = value; }
	}
	// Use this for initialization
	void Start () {
	
	}

	void OnEnable ()
	{

		SetInstance(this);
	}

	void OnDisable()
	{
		if (BackCamerControll.Instance() != null)
		{
			BackCamerControll.Instance().StopSceneEffect(138, true);
		}
		SetInstance (null);
	}
	void Check_NewPlayerGuide()
	{
		if (null == DiviBonusRootLogic.Instance()) 
		{
			return;		
		}
		if((int)GameDefine_Globe.NEWOLAYERGUIDE.DIVI_MONEY == FunctionButtonLogic.Instance ().NewPlayerGuide_Step
		   || (int)GameDefine_Globe.NEWOLAYERGUIDE.DIVI_DRAW == FunctionButtonLogic.Instance ().NewPlayerGuide_Step)
		{
			NewPlayerGuide(1);
		}
	}

	// Update is called once per frame
	void Update () {
		if (ShowItemObj)
		{
			m_fFrameTimeDiff += Time.deltaTime;
			
			if (m_fFrameTimeDiff >= m_fShowIconTime)
			{
				m_fFrameTimeDiff = 0f;
				ShowItemS();
			}
		}
	}

	public static void InitDiviBonusInfo(bool OK)
	{
		UIManager.ShowUI(UIInfo.BonusItemGetRoot, OnLoadDiviBonusItemGetRoot, OK);
	}

	static void OnLoadDiviBonusItemGetRoot(bool bSucess,object param)
	{
		if (DiviBonusRootLogic.Instance() != null)
		{
			DiviBonusRootLogic.Instance().Init();
		}
	}

	void Init()
	{
		if (null != GameManager.gameManager.SoundManager)
			GameManager.gameManager.SoundManager.PlaySoundEffect(117); 
		
		InitBonusData();
		InitItemButtonPosition();
		InitItem();
		ResetAllItem();
		m_BonusType = (int)GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawType;

		m_CancleButton.gameObject.SetActive (false);
		m_ContinueButton.gameObject.SetActive (false);

		Invoke ("SetShowItemTrue", m_InVakeTime);
	}

	void SetShowItemTrue()
	{
		ShowItemObj = true;
	}


	void InitBonusData()
	{
		m_BonusType = -1;
		m_nCurItemShow = 0;
		m_nShowItemNum = 0;
		m_fFrameTimeDiff = 0;
		ShowItemObj = false;
	}

	//初始化坐标位置
	void InitItemButtonPosition()
	{
		m_InitPosition = new Vector3(0f, -190f, 0f);
		m_InitRotation = new Vector3(0f, 0f, 180f);
		m_FinalRotation = new Vector3(0f, 0f, 0f);
		m_InitScale = new Vector3(0f,0f,0f);
		m_FinalScale = new Vector3(1f,1f,1f);
		m_CenterPosition = new Vector3 (0f, 25f, 0f);
	} 

	void InitItem()
	{
		for (int i = 0; i < GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.GetMaxGainBonusCount(); i++)
		{
			string ImageName = "";
			string ItemName = "";
			string strQualityIcon = "";
			bool bShowItemImpact = false;
			
			int nBonusID = GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.GetGainBonusID(i);
			if(100 <= nBonusID)
			{
				ImageName = "jinbi";
				Tab_MoneyTree BonusInfo = TableManager.GetMoneyTreeByID(nBonusID - 100, 0);
				ItemName = BonusInfo.Money.ToString();
				m_ItemObjArray[i].InitItem(ImageName, ItemName, strQualityIcon, bShowItemImpact);
				m_nShowItemNum ++;
			}
			else if (nBonusID > 0)
			{
				Tab_DailyLuckyDrawBonusInfo BonusInfo = TableManager.GetDailyLuckyDrawBonusInfoByID(nBonusID, 0);
				if (null == BonusInfo)
				{
					LogModule.DebugLog("DailyLuckyDrawBonusInfo.txt has not Line ID=" + nBonusID);
					return;
				}
				bShowItemImpact = (BonusInfo.RareDegree > (int)BONUSRAREDEGREE.DLD_BONUS_MIDDLE)?true:false;
				//金钱提示
				if (BonusInfo.MoneyCount > 0)
				{
					ImageName = GetMoneyImageName(BonusInfo.MoneyType);
					ItemName = BonusInfo.MoneyCount.ToString();
					m_ItemObjArray[i].InitItem(ImageName, ItemName, strQualityIcon, bShowItemImpact);
					m_nShowItemNum ++;
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
						ImageName = ItemInfo.Icon;
						ItemName = ItemInfo.Name;
						strQualityIcon = GetItemQualityStr(ItemInfo.Quality);
						m_ItemObjArray[i].InitItem(ItemInfo.Icon, ItemInfo.Name, strQualityIcon, bShowItemImpact);
						m_ItemObjArray[i].InitItem(ImageName, ItemName, strQualityIcon, bShowItemImpact);
						m_nShowItemNum ++;
					}
				}
				//经验提示
				if (BonusInfo.Exp > 0)
				{
					ImageName = "jingyan";
					ItemName = BonusInfo.Exp.ToString();
					m_ItemObjArray[i].InitItem(ImageName, ItemName, strQualityIcon, bShowItemImpact);
					m_nShowItemNum ++;
				}
			}


		}
	}

	void ResetAllItem()
	{
		for (int i = 0; i < m_ItemObjArray.Length; i++) 
		{
			m_ItemObjArray[i].gameObject.SetActive(false);
		}
	}

	string GetMoneyImageName(int MoneyType)
	{
		switch (MoneyType)
		{
		case (int)MONEYTYPE.MONEYTYPE_YUANBAO:
			return "bdyuanbao";
		case (int)MONEYTYPE.MONEYTYPE_COIN:
			return "jinbi";
		case (int)MONEYTYPE.MONEYTYPE_YUANBAO_BIND:
			return "bdyuanbao";
		default:
			return "";
		}
	}

	string GetItemQualityStr(int Quality)
	{
		switch ((ItemQuality)Quality)
		{
		case ItemQuality.QUALITY_WHITE:
			return "ui_pub_012";
		case ItemQuality.QUALITY_GREEN:
			return "ui_pub_013";
		case ItemQuality.QUALITY_BLUE:
			return "ui_pub_014";
		case ItemQuality.QUALITY_PURPLE:
			return "ui_pub_015";
		case ItemQuality.QUALITY_ORANGE:
			return "ui_pub_016";
		default:
			return "";
		}
	}
	
	void ShowItemS()
	{
		//物品效果已经显示完毕
		if (m_nCurItemShow >= m_nShowItemNum)
		{
			//显示按钮
			ShowButtonAfterAllTween();

			//结束显示物品obj
			ShowItemObj = false;
			return;
		}

		Vector3 ToPosition = m_ItemIconPositionArray[m_nCurItemShow];
		if(m_nShowItemNum == 1)
		{
			ToPosition = m_CenterPosition;
		}
		
		//显示多个物品
		m_ItemObjArray[m_nCurItemShow].ShowItemObjTween(
			m_InitPosition, ToPosition, 
			m_InitRotation, m_FinalRotation, 
			m_InitScale, m_FinalScale, m_fShowIconTime);
		GameManager.gameManager.SoundManager.PlaySoundEffect(131);
		m_nCurItemShow++;
	}

	//根据每日幸运抽奖类型，显示按钮文字
	void ShowButtonAfterAllTween()
	{
		if(-1 == m_BonusType)
		{
			return;
		}
		if (null == m_CancleButton) 
		{
			return;
		}
		
		m_CancleButton.gameObject.SetActive(true);

		if (null == m_ContinueButtonLabel.text)
		{
			return;
		}
		m_ContinueButtonLabel.text = "";
		
		m_ContinueButton.gameObject.SetActive(true);
		
		if ((int)DLDDRAWTYPE.DLD_DRAWTYPE_ONE == m_BonusType ||
		    (int)DLDDRAWTYPE.DLD_DRAWTYPE_ONE_YUANBAO == m_BonusType)
		{
			m_ContinueButtonLabel.text = Utils.GetDicByID(5656);
		}
		else if ((int)DLDDRAWTYPE.DLD_DRAWTYPE_TEN == m_BonusType)
		{
			m_ContinueButtonLabel.text = Utils.GetDicByID(5657);
		}
		else if(3 == m_BonusType || 4 == m_BonusType)
		{
			m_ContinueButtonLabel.text = Utils.GetDicByID(5655);
		}

		Check_NewPlayerGuide();
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

	void NewPlayerGuide(int nIndex)
	{
		if (nIndex < 0) 
		{
			return;
		}
		
		NewPlayerGuidLogic.CloseWindow();

		m_NewPlayerGuide_Step = nIndex;

		switch (nIndex)
		{
		case 1:
			if(m_CancleButton)
			{
				NewPlayerGuidLogic.OpenWindow(m_CancleButton.gameObject, 202, 64, "", "left", 2, true, true);
				FunctionButtonLogic.Instance ().NewPlayerGuide_Step = (int)GameDefine_Globe.NEWOLAYERGUIDE.UI_CLOSE;
			}
			break;
		}
	}
}
