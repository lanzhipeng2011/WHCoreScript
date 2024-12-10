using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Games.GlobeDefine;
using GCGame.Table;
using Games.DailyLuckyDraw;
using Games.MoneyTree;
using Module.Log;
using System;
public class DivinationLogic : UIControllerBase<DivinationLogic> 
{
	public GameObject m_HelpPanel;
	
	public UILabel m_BindYuanBaoCountLabel;
	public UILabel m_YuanBaoCountLabel;
	
	// 财运占星 DiviM
	//public const int MaxAwardCount = 20; 			// 最大领取次数
	public const int DiviMFreeAwardTimes = 8;   	// 免费领取次数
	public const int DiviMBuyAwardTimes = 12;   // 元宝领取次数
	
	public UILabel m_DiviMTimerLabel;
	public UILabel m_DiviMFreeCountLabel;
	public UILabel m_DiviMBuyCostLabel;
	public UILabel m_DiviMBuyCountLabel;
	
	public UIImageButton m_DiviMFreeBtn;
	public UIImageButton m_DiviMBuyBbtn;
	
	private int m_DiviMCDTime;
	public int DiviMCDTime
	{
		get { return m_DiviMCDTime; }
		set {
			m_DiviMCDTime = value;
			UpdateDiviMTimerLabel();
		}
	}
	
	private int m_CurDiviMFreeID;
	public int CurDiviMFreeID
	{
		get { return m_CurDiviMFreeID; }
		set {
			m_CurDiviMFreeID = value;
			UpdateDiviMFreeCount(m_CurDiviMFreeID);
		}
	}
	
	private int m_CurDiviMBuyID;
	public int CurDiviMBuyID
	{
		get { return m_CurDiviMBuyID; }
		set {
			m_CurDiviMBuyID = value;
			UpdateDiviMBuyCount(m_CurDiviMBuyID);
		}
	}

	public const int m_nMaxBonusBoxCount = 14;//有修改，需要同步修改DailyLuckyDrawdata.cs
	public const int m_nMaxGainBonusCount = 10;//有修改，需要同步修改DailyLuckyDrawdata.cs
	public const int m_nDiviTOneCost = 25;
	public const int m_nDiviTTenCost = 225;

	public UILabel m_DiviTTimerLabel;
	public UILabel m_DiviTOneCostLabel;
	public UILabel m_DiviTFreeCountLabel;

	public UIImageButton m_DiviTFreeBtn;
	public UIImageButton m_DiviTOneBtn;
	public UIImageButton m_DiviTTenBtn;

	private int m_DiviTCDTime;
	public int DiviTCDTime
	{
		get { return m_DiviTCDTime; }
		set {
			m_DiviTCDTime = value;
			UpdateDiviTTimerLabel();
		}
	}

	private int m_CurDiviTFreeID;
	public int CurDiviTFreeID
	{
		get { return m_CurDiviTFreeID; }
		set {
			m_CurDiviTFreeID = value;
		}
	}

	public GameObject m_BtnClose;

	private int m_NewPlayerGuide_Step = -1;
	public int NewPlayerGuide_Step
	{
		get { return m_NewPlayerGuide_Step; }
		set { m_NewPlayerGuide_Step = value; }
	}

	private int vipTotleTimes;

	void OnEnable()
	{
		SetInstance(this);
	
		
			int nVip = VipData.GetVipLv();
			Tab_VipBook tab = TableManager.GetVipBookByID(nVip, 0);
			
			
		vipTotleTimes = DiviMBuyAwardTimes + tab.VipMoneyTree;
			

		DivinationDataShow();
		Check_NewPlayerGuide();
	}
	
	void OnDisable()
	{
		SetInstance(null);

	}

	void Check_NewPlayerGuide()
	{
		if (null == DivinationLogic.Instance()) 
		{
			return;		
		}
		if ((int)GameDefine_Globe.NEWOLAYERGUIDE.DIVI_MONEY == FunctionButtonLogic.Instance ().NewPlayerGuide_Step) 
		{
			NewPlayerGuide(1);
		}
		else if((int)GameDefine_Globe.NEWOLAYERGUIDE.DIVI_DRAW == FunctionButtonLogic.Instance ().NewPlayerGuide_Step)
		{
			NewPlayerGuide(2);
		}
		else if((int)GameDefine_Globe.NEWOLAYERGUIDE.UI_CLOSE == FunctionButtonLogic.Instance ().NewPlayerGuide_Step)
		{
			FunctionButtonLogic.Instance ().NewPlayerGuide_Step = -1;
			NewPlayerGuide((int)GameDefine_Globe.NEWOLAYERGUIDE.UI_CLOSE);
		}
	}

	// 界面刷新
	private void DivinationDataShow()
	{
		CleanUpDiviMoneyWindow ();
		CleanUpDiviTreasureWindow ();
		InitDiviMoneyWindow ();
		InitDiviTreasureWindow ();
		UpdateYuanbao ();
	}
	
	// 初始化财运占星
	private void InitDiviMoneyWindow()
	{
		DiviMCDTime = GameManager.gameManager.PlayerDataPool.MoneyTreeData.CDTime;
		CurDiviMFreeID = GameManager.gameManager.PlayerDataPool.MoneyTreeData.MoneyTreeID;
		CurDiviMBuyID = GameManager.gameManager.PlayerDataPool.MoneyTreeData.YuanBaoAwardCount;
	}

	private void InitDiviTreasureWindow()
	{
		CurDiviTFreeID = GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawFreeTimes;
		DiviTCDTime = GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawFreeCDTime;
	}
	
	// 清空财运占星
	private void CleanUpDiviMoneyWindow()
	{
		DiviMCDTime = 0;
		m_DiviMTimerLabel.text = "";
		m_DiviMFreeCountLabel.text = "";
		m_DiviMBuyCostLabel.text = "";
		m_DiviMBuyCountLabel.text = "";
		
		if (m_DiviMFreeBtn && m_DiviMBuyBbtn)
		{
			m_DiviMFreeBtn.isEnabled = false;
			m_DiviMBuyBbtn.isEnabled = false;
		}
	}

	// 清空宝物占星
	private void CleanUpDiviTreasureWindow()
	{
		DiviTCDTime = 0;
		m_DiviTTimerLabel.text = "";
		m_DiviTFreeCountLabel.text = "";
		
		m_DiviTFreeBtn.gameObject.SetActive(false);
		m_DiviTOneBtn.gameObject.SetActive(true);
		m_DiviTTenBtn.gameObject.SetActive(true);
	}
	
	// 更新免费财运占星时间信息
	private void UpdateDiviMTimerLabel()
	{
		if (m_DiviMCDTime <= 0)
		{
			m_DiviMTimerLabel.text = "";
			if (m_DiviMFreeBtn && m_DiviMFreeBtn.isEnabled == false)
			{
				m_DiviMFreeBtn.isEnabled = true;
			}
			return;
		}
		
		int nMin = m_DiviMCDTime / 60;
		int nSec = m_DiviMCDTime % 60;
		if (m_DiviMTimerLabel)
		{
			m_DiviMTimerLabel.text = nMin / 10 + nMin % 10 + ":" + nSec / 10 + nSec % 10;
		}
		
		if (m_DiviMFreeBtn && m_DiviMFreeBtn.isEnabled)
		{
			m_DiviMFreeBtn.isEnabled = false;
		}
	}
	
	// 更新免费财运占星信息
	private void UpdateDiviMFreeCount(int nCurDiviMFreeID)
	{
		if (m_DiviMFreeCountLabel)
		{
			m_DiviMFreeCountLabel.text = StrDictionary.GetClientDictionaryString("#{5629}", DiviMFreeAwardTimes-nCurDiviMFreeID);
		}
		
		if (nCurDiviMFreeID >= DiviMFreeAwardTimes)
		{
			m_DiviMFreeBtn.isEnabled = false;
		}
	}
	
	// 更新财运占星购买信息 
	private void UpdateDiviMBuyCount(int nCount)
	{
		if(Singleton<ObjManager>.Instance.MainPlayer == null) 
		{
			return;
		}
	
		if (m_DiviMBuyCountLabel)
		{
			int nVip = VipData.GetVipLv();
			Tab_VipBook tab = TableManager.GetVipBookByID(nVip, 0);
		
			if(0 == (DiviMBuyAwardTimes + tab.VipMoneyTree - nCount))
			{
				m_DiviMBuyCountLabel.text ="元宝占星";
			}
			else{
				m_DiviMBuyCountLabel.text = StrDictionary.GetClientDictionaryString("#{5630}", DiviMBuyAwardTimes + tab.VipMoneyTree - nCount);
			}

		}
		
		if (m_DiviMBuyCostLabel && nCount < vipTotleTimes) //DiviMBuyAwardTimes
		{
			Tab_MoneyTree tab = TableManager.GetMoneyTreeByID(DiviMFreeAwardTimes + nCount, 0);
			if(null != tab)
			{
				m_DiviMBuyCostLabel.text = tab.BindYuanbao.ToString();;
			}
		}
		else
		{
			m_DiviMBuyCostLabel.text = "";
		}
		
		if (nCount >= vipTotleTimes)//DiviMBuyAwardTimes
		{
			m_DiviMBuyBbtn.isEnabled = false;
		}
		else
		{
			m_DiviMBuyBbtn.isEnabled = true;
		}
	}
	
	// 更新player元宝
	private void UpdateYuanbao()
	{
		if (null != Singleton<ObjManager>.GetInstance().MainPlayer) 
		{
			m_BindYuanBaoCountLabel.text = GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBaoBind ().ToString ();
			m_YuanBaoCountLabel.text = GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBao ().ToString ();
			return;
		}
		
		m_BindYuanBaoCountLabel.text = "0";
		m_YuanBaoCountLabel.text = "0";
	}
	
	// 免费财运占星一次
	private void DiviMFreeBtnClick()
	{
		if(Singleton<ObjManager>.Instance.MainPlayer == null) 
		{
			return;
		}

		if (CurDiviMFreeID >= DiviMFreeAwardTimes)
		{
			return;
		}
		
		if (DiviMCDTime > 0)
		{
			Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2478}");
		}
		else
		{
			GameManager.gameManager.PlayerDataPool.MoneyTreeData.SendAwardPacket(1, 1);
		}
	}
	
	// 购买财运占星一次
	private void DiviMBuyBtnClick()
	{
		if(Singleton<ObjManager>.Instance.MainPlayer == null) 
		{
			return;
		}

		if (m_CurDiviMBuyID >= vipTotleTimes)
		{
			return;
		}
		
		Tab_MoneyTree tab = TableManager.GetMoneyTreeByID( DiviMFreeAwardTimes + m_CurDiviMBuyID, 0);
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
		
	}
	
	void OnDiviMBuyOKClick()
	{
		GameManager.gameManager.PlayerDataPool.MoneyTreeData.SendAwardPacket(1, 2);
	}
	
	void OnDiviMBuyCancelClick()
	{
		MessageBoxLogic.CloseBox();
	}

	void UpdateDiviTTimerLabel()
	{
		if (DiviTCDTime > 0) 
		{
			m_DiviTOneCostLabel.gameObject.SetActive(false);
			int nMin = m_DiviTCDTime / 60;
			int nSec = m_DiviTCDTime % 60;
			if (m_DiviTTimerLabel)
			{
				m_DiviTTimerLabel.text = nMin / 10 + nMin % 10 + ":" + nSec / 10 + nSec % 10;
			}
			m_DiviTTimerLabel.gameObject.SetActive(true);
			m_DiviTFreeBtn.gameObject.SetActive(false);
			m_DiviTOneBtn.gameObject.SetActive(true);
			return;
		}
		
		if (CurDiviTFreeID > 0) 
		{
			m_DiviTOneCostLabel.gameObject.SetActive(false);
			m_DiviTTimerLabel.gameObject.SetActive(false);
			m_DiviTFreeCountLabel.text = StrDictionary.GetClientDictionaryString("#{5629}", CurDiviTFreeID);
			m_DiviTFreeBtn.gameObject.SetActive(true);
			m_DiviTOneBtn.gameObject.SetActive(false);
			return;
		}
		
		m_DiviTOneCostLabel.gameObject.SetActive(true);
		m_DiviTTimerLabel.gameObject.SetActive(false);
		m_DiviTFreeBtn.gameObject.SetActive(false);
		m_DiviTOneBtn.gameObject.SetActive(true);
	}

	private void DiviTFreeBtnClick()
	{
		if(DiviTCDTime > 0 || CurDiviTFreeID <= 0)
		{
			return;
		}

		CG_DAILYLUCKYDRAW_DRAW packet = (CG_DAILYLUCKYDRAW_DRAW)PacketDistributed.CreatePacket(MessageID.PACKET_CG_DAILYLUCKYDRAW_DRAW);
		packet.SetDrawtype((int)DLDDRAWTYPE.DLD_DRAWTYPE_ONE);                  
		packet.SendPacket();
	}

	private void DiviTOneBtnClick()
	{
		if(Singleton<ObjManager>.Instance.MainPlayer == null) 
		{
			return;
		}

		int nYuanBaoCount = GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBao() + GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBaoBind();
		if ( nYuanBaoCount < m_nDiviTOneCost)
		{
			Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1522}");
			return;
		}
		else
		{
			string dicStr = "";
			if (DiviTCDTime > 0)
			{
				dicStr = StrDictionary.GetClientDictionaryString("#{2411}", m_nDiviTOneCost);
			}
			else
			{
				dicStr = StrDictionary.GetClientDictionaryString("#{1813}", m_nDiviTOneCost);
			}

			//会扣元宝，要继续吗？
			MessageBoxLogic.OpenOKCancelBox(dicStr, "", DoDiviTreasureOne);
		}
	}

	private void DiviTTenBtnClick()
	{
		if(Singleton<ObjManager>.Instance.MainPlayer == null) 
		{
			return;
		}

		//判断条件
		int nYuanBaoCount = GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBao() + GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBaoBind();
		if (nYuanBaoCount < m_nDiviTTenCost)
		{
			Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1522}");
			return;
		}
		else
		{
			string dicStr = StrDictionary.GetClientDictionaryString("#{1814}", m_nDiviTTenCost);
			//会扣元宝，要继续吗？
			MessageBoxLogic.OpenOKCancelBox(dicStr, "", DoDiviTreasureTen);
		}
	}

	private void DoDiviTreasureOne()
	{
		CG_DAILYLUCKYDRAW_DRAW packet = (CG_DAILYLUCKYDRAW_DRAW)PacketDistributed.CreatePacket(MessageID.PACKET_CG_DAILYLUCKYDRAW_DRAW);
		packet.SetDrawtype((int)DLDDRAWTYPE.DLD_DRAWTYPE_ONE_YUANBAO);                  
		packet.SendPacket();
	}

	private void DoDiviTreasureTen()
	{
		CG_DAILYLUCKYDRAW_DRAW packet = (CG_DAILYLUCKYDRAW_DRAW)PacketDistributed.CreatePacket(MessageID.PACKET_CG_DAILYLUCKYDRAW_DRAW);
		packet.SetDrawtype((int)DLDDRAWTYPE.DLD_DRAWTYPE_TEN);                  
		packet.SendPacket();
	}

	public void DoDivination()
	{
		int DiviType = GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawType;
		switch (DiviType) 
		{
		case (int)DLDDRAWTYPE.DLD_DRAWTYPE_ONE:
		case (int)DLDDRAWTYPE.DLD_DRAWTYPE_ONE_YUANBAO:
			DiviTOneBtnClick();
			return;
		case (int)DLDDRAWTYPE.DLD_DRAWTYPE_TEN:
			DiviTTenBtnClick();
			return;
		case 4:
			DiviMBuyBtnClick();
			return;
		default:
			break;
		}
	}

	// 打开帮助界面
	private void HelpBtnClick()
	{
		m_HelpPanel.SetActive (true);
	}
	
	// 关闭帮助界面
	private void HelpCloseBtnClick()
	{
		m_HelpPanel.SetActive (false);
	}
	
	// 充值
	private void ChongZhiBtnClick()
	{
		RechargeData.PayUI();
	}
	
	// 关闭占星界面
	public void CloseDivination()
	{
		UIManager.CloseUI(UIInfo.DivinationRoot);
		m_NewPlayerGuide_Step = -1;
		NewPlayerGuidLogic.CloseWindow();
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
			if(null != m_DiviMFreeBtn && m_DiviMFreeBtn.isEnabled)
			{
				NewPlayerGuidLogic.OpenWindow(m_DiviMFreeBtn.gameObject, 202, 64, "", "left", 2, true, true);
			}
			break;
		case 2:
			if(null != m_DiviTFreeBtn)
			{
				NewPlayerGuidLogic.OpenWindow(m_DiviTFreeBtn.gameObject, 202, 64, "", "left", 2, true, true);
			}
			break;
		case (int)GameDefine_Globe.NEWOLAYERGUIDE.UI_CLOSE:
		{
			if(null != m_BtnClose)
			{
				NewPlayerGuidLogic.OpenWindow(m_BtnClose, 78, 78, "", "left", 2, true, true);
			}

		}
			break;
		}
	}

}