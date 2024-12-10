using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GCGame.Table;
using Games.UserCommonData;
using GCGame;


public class FirstRechargeLogin : MonoBehaviour {

	public GameObject m_ItemReward;
	public GameObject m_ShoppingBtn;
	public UILabel m_LeftLabel;
	public UILabel m_RightLabel;
	public UILabel m_LeftTime;
	public UILabel m_RightTime;


	public GameObject m_WeekBtn;
	public GameObject m_MonthBtn;

		// Use this for initialization
	void Start () {
		init ();

	}

	void OnEnable()
	{
		ShowBtn ();
		ShowLable ();
		ShowTimeLabel ();
	}
		
		// Update is called once per frame
	void Update () {

	}

	void ShowBtn()
	{
		if (null == m_ShoppingBtn)
			return;
		bool bRet = GameManager.gameManager.PlayerDataPool.CommonData.GetCommondFlag ((int)USER_COMMONFLAG.CF_FIRSTCONST);
		if(bRet)
			m_ShoppingBtn.GetComponent<UIImageButton> ().isEnabled = false;
	}


	void ShowTimeLabel()
	{
		if(m_LeftTime  == null || m_RightTime == null || m_WeekBtn == null || m_MonthBtn == null)
			return;
		if(GameManager.gameManager.PlayerDataPool.WeekDay != 0)//判断剩余天数
		{
			m_LeftTime.text = StrDictionary.GetClientDictionaryString("#{6015}",  7 - GameManager.gameManager.PlayerDataPool.WeekDay);
			m_WeekBtn.GetComponent<UIImageButton> ().isEnabled = false;
		}
		else
		{
			m_LeftTime.text = Utils.GetDicByID(6017);//"未享有此福利";
			m_WeekBtn.GetComponent<UIImageButton> ().isEnabled = true;
		}
		if(GameManager.gameManager.PlayerDataPool.MonthDay != 0)//判断剩余天数
		{
			m_RightTime.text = StrDictionary.GetClientDictionaryString ("#{6015}",  30 - GameManager.gameManager.PlayerDataPool.MonthDay);
			m_MonthBtn.GetComponent<UIImageButton> ().isEnabled = false;
		}
		else
		{
			m_RightTime.text = Utils.GetDicByID(6017);//"未享有此福利";
			m_MonthBtn.GetComponent<UIImageButton> ().isEnabled = true;
		}
	}

	void ShowLable()
	{
		//if (null == m_LeftLabel && null == m_RightLabel)
		//	return;
		//bool bRet = GameManager.gameManager.PlayerDataPool.CommonData.GetCommondFlag ((int)USER_COMMONFLAG.CF_WEEKFLAG);
		//if (bRet)
		//	m_LeftLabel.text = Utils.GetDicByID(6016);//"享有此福利";
		//else
		//	m_LeftLabel.text = Utils.GetDicByID(6017);//"未享有此福利";
		//bRet = GameManager.gameManager.PlayerDataPool.CommonData.GetCommondFlag ((int)USER_COMMONFLAG.CF_MONTH);
		//if (bRet)
		//	m_RightLabel.text = Utils.GetDicByID(6016);//"享有此福利";
		//else
		//	m_RightLabel.text = Utils.GetDicByID(6017);//"未享有此福利";
	}
		
	public void OnClickChongZhi()
	{

		RechargeData.PayUI();

	}
		
	private void init()
	{

		if (null != m_ItemReward) 
		{
				Tab_Activity acti = TableManager.GetActivityByID (1, 0);
				int n = acti.getItemCount ();
				for (int i=0; i<n; i++)
				{
					int index = acti.GetItembyIndex (i);
					if (index != 0) 
					{
						Tab_CommonItem item = TableManager.GetCommonItemByID (index, 0);
						GameObject rewardItem = (GameObject)Instantiate (m_ItemReward);
						VipItem vip = rewardItem.GetComponent<VipItem> ();				
						vip.m_BonusImage.spriteName = item.Icon;
						vip.gameObject.name = item.Id.ToString();
						rewardItem.transform.parent = this.transform;
						rewardItem.transform.localScale = Vector3.one;
						rewardItem.transform.localPosition = new Vector3 (0 + 130f * i, -63f, 0f);											
					}
				}
		}

	}
	
}