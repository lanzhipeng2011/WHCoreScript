using UnityEngine;
using System.Collections;
using GCGame;
using Module.Log;

public class RechargeItem : MonoBehaviour
{

    public UISprite m_SprIcon;
    public UILabel m_LabelPrice;
    public UILabel m_LabelYuanbao;
    public UISprite m_SprYuanBaoIcon;
    public UISprite m_SprOnlyShowOnce;
    public GameObject m_objYuanSprite;

	public GameObject m_btnObj;

    private bool m_bEnableTimes = false;

    public static RechargeItem CreateItem(GameObject resItem, Transform grid, string registerID, string strIconName, string price, string goodName, bool bShowOnce, bool bEnableTimes)
    {
        GameObject newItem = Utils.BindObjToParent(resItem, grid.gameObject, registerID);

        if(null == newItem)
        {
            LogModule.ErrorLog("load recharge item error!");
            return null;
        }
        RechargeItem curItem = newItem.GetComponent<RechargeItem>();
        if (null != curItem)
            curItem.InitData(strIconName, price, goodName, bShowOnce, bEnableTimes);

        return curItem;
    }

    void OnBuyClick()
    {
		NGUILogHelpler.Log("OnBuyClick","PlatformHelper");
        if (null == RechargeController.Instance())
        {
            return;
        }

        if (m_bEnableTimes)
        {
            NumChooseController.OpenWindow(1, 99999, Utils.GetDicByID(3225), OnChooseBuyCountOk, 1);
        }
        else
        {
			int money=int.Parse(this.m_LabelYuanbao.text)/10;//充值元宝比率
			NGUILogHelpler.Log("money" + money.ToString(),"PlatformHelper");
            RechargeController.Instance().OnMakePay(gameObject.name, money);
        }
        
    }

    public void InitData(string strIconName, string price, string goodName, bool bShowOnce, bool bEnableTimes)
    {
        if (!goodName.Contains("#y"))
        {
            m_SprYuanBaoIcon.gameObject.SetActive(false);
            switch (goodName) 
            {
                case "成长基金":
                    price = "198";
                break;
                case "招财进宝":
                    price = "25";
                break;
            }
        }
        else
        {
            
            goodName = goodName.Remove(goodName.Length - 2);
            price = (int.Parse(goodName) / 10).ToString();
        }
        m_SprIcon.spriteName = strIconName;
        if (bEnableTimes)
        {
            m_LabelPrice.text = Utils.GetDicByID(3226);
            m_objYuanSprite.SetActive(false);
        }
        else
        {
            m_objYuanSprite.SetActive(true);
            m_LabelPrice.text = price;
        }
        
        m_LabelYuanbao.text = goodName;
        m_bEnableTimes = bEnableTimes;
        m_SprOnlyShowOnce.gameObject.SetActive(bShowOnce);


		//======判断周卡月卡
		if(m_btnObj != null)
		{
			if(price == "8")
			{
				if(GameManager.gameManager.PlayerDataPool.WeekDay != 0)
					m_btnObj.GetComponent<UIImageButton> ().isEnabled = false;
				else
					m_btnObj.GetComponent<UIImageButton> ().isEnabled = true;
			}else if(price == "25")
			{
				if(GameManager.gameManager.PlayerDataPool.MonthDay != 0)
					m_btnObj.GetComponent<UIImageButton> ().isEnabled = false;
				else
					m_btnObj.GetComponent<UIImageButton> ().isEnabled = true;
			}else{
				m_btnObj.GetComponent<UIImageButton> ().isEnabled = true;
			}
		}

    }

    public void UpdateData(bool bShowOnce)
    {
        m_SprOnlyShowOnce.gameObject.SetActive(bShowOnce);
    }

    public void OnChooseBuyCountOk(int num)
    {
        if (null != RechargeController.Instance())
            RechargeController.Instance().OnMakePay(gameObject.name, num);
    }
}
