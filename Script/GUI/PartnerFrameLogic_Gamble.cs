using UnityEngine;
using System.Collections;
using Games.FakeObject;
using GCGame.Table;
using Games.Fellow;
using GCGame;
using System;
using Module.Log;
using Games.UserCommonData;

public class PartnerFrameLogic_Gamble : MonoBehaviour {

    public UILabel m_FreeTimeLabel;
    public UILabel m_ExpendCoinLabel;
    public UISprite m_ExpendCoinSprite;
    public UILabel m_ExpendYuanBaoLabel;
    public GameObject m_GainPartnerListGrid;
    public UIImageButton m_CoinGainButton;    //金币抽取伙伴按钮
    public UILabel m_Expend10CoinLabel;         //金币10连抽花费
    public UILabel m_Expend10YuanBaoLabel;      //元宝10连抽花费
    public UILabel m_PartnerGainRemain;
    public GameObject m_GainPartnerPanel;       //显示抽取伙伴

    private FakeObject m_PartnerFakeObj;

    private int m_CDSeconds = 0;            //抽取CD
    private int m_CurGainType = -1;

    private Fellow m_CurFellow;
    private int m_FellowIndex = 99;

    private static PartnerFrameLogic_Gamble m_Instance = null;
    public static PartnerFrameLogic_Gamble Instance()
    {
        return m_Instance;
    }

    void OnEnable()
    {
        m_Instance = this;

        UpdateGainRemain();
    }

    void OnDisable()
    {
        m_Instance = null;
    }

    public void UpdateMainInfo()
    {
        int freeGainCount = GameManager.gameManager.PlayerDataPool.FellowGainCount_Free;
        int coinGainCount = GameManager.gameManager.PlayerDataPool.FellowGainCount_Coin;
        if (coinGainCount > TableManager.GetFellowGainCost().Count - 1)
        {
            coinGainCount = TableManager.GetFellowGainCost().Count - 1;
        }
        int yuanbaoGainCount = GameManager.gameManager.PlayerDataPool.FellowGainCount_YuanBao;
        if (yuanbaoGainCount > TableManager.GetFellowGainCost().Count - 1)
        {
            yuanbaoGainCount = TableManager.GetFellowGainCost().Count - 1;
        }

        Tab_FellowGainCost coinline = TableManager.GetFellowGainCostByID((coinGainCount+1), 0);
        Tab_FellowGainCost yuanbaoline = TableManager.GetFellowGainCostByID(yuanbaoGainCount+1, 0);
        if (coinline != null)
        {
            if (freeGainCount >= 10)
            {
                m_ExpendCoinLabel.text = coinline.NormalConsumeNum.ToString();
                m_ExpendCoinLabel.gameObject.SetActive(true);
                m_ExpendCoinSprite.gameObject.SetActive(true);
                m_FreeTimeLabel.gameObject.SetActive(false);
            }
            else
            {
                m_ExpendCoinLabel.gameObject.SetActive(false);
                m_ExpendCoinSprite.gameObject.SetActive(false);
                m_FreeTimeLabel.text = StrDictionary.GetClientDictionaryString("#{3148}", (10 - freeGainCount), 10);
                m_FreeTimeLabel.gameObject.SetActive(true);
            }
        }
        //抽取所需元宝
        if (yuanbaoline != null)
        {
            m_ExpendYuanBaoLabel.text = yuanbaoline.YuanBaoConsumeNum.ToString();
        }

        //10连抽所需金币
        if (GetNeedCoin10(coinGainCount) >= 0)
        {
            m_Expend10CoinLabel.text = GetNeedCoin10(coinGainCount).ToString();
        }

        //10连抽所需元宝
        if (GetNeedYuanBao10(yuanbaoGainCount) >= 0)
        {
            m_Expend10YuanBaoLabel.text = GetNeedYuanBao10(yuanbaoGainCount).ToString();
        }

        int coinCDTimes = GameManager.gameManager.PlayerDataPool.FellowGainCD_Coin;
        if (coinCDTimes > 0)
        {
            SetGainCDTime(coinCDTimes);
        }

        //更新红点角标
        UpdateGainRemain();
        if (PartnerFrameLogic.Instance())
        {
            PartnerFrameLogic.Instance().UpdateGainRemain();
        }
    }

    public void UpdateGainPartner(int fellowId, int quality, UInt64 fellowGuid)
    {
        if (m_GainPartnerPanel.activeSelf == false)
        {
            m_GainPartnerPanel.gameObject.SetActive(true);
        }
        m_CurFellow = GameManager.gameManager.PlayerDataPool.FellowContainer.GetFellowByGuid(fellowGuid);
        AddGainPartnerList();
    }

    public void AddGainPartnerList()
    {
        UIManager.LoadItem(UIInfo.PartnerFrameItem, OnLoadPartnerFrameItem);
    }

    void OnLoadPartnerFrameItem(GameObject resItem, object param)
    {
        if (null == resItem)
        {
            LogModule.ErrorLog("load partner frame error");
            return;
        }

        if (m_GainPartnerListGrid != null)
        {
            m_FellowIndex--;
            string objectName = m_FellowIndex.ToString();
            GameObject fellowobject = Utils.BindObjToParent(resItem, m_GainPartnerListGrid, objectName);
            if (fellowobject != null)
            {
                fellowobject.GetComponent<PartnerFrameItemLogic>().UpdateFellowInfo(m_CurFellow);
				//fellowobject.GetComponent<UIButtonMessage>().enabled = false;
				UIButtonMessage[] btnMessage= fellowobject.GetComponents<UIButtonMessage>();
				for(int i = 0; i < btnMessage.Length; i++)
				{
					btnMessage[i].enabled = false;
				}
            }
            m_GainPartnerListGrid.GetComponent<UIGrid>().repositionNow = true;
            m_GainPartnerListGrid.GetComponent<UITopGrid>().Recenter(true);
        }
    }

    public void SetGainType(int type)
    {
        m_CurGainType = type;
    }

    public void SetGainCDTime(int CDTime)
    {
        m_CDSeconds = CDTime;
        if (m_CDSeconds != 0)
        {
            //m_ExpendCoinLabel.text = string.Format("{0}秒", m_CDSeconds);
            m_ExpendCoinLabel.text = StrDictionary.GetClientDictionaryString("#{2869}", m_CDSeconds);
			m_ExpendCoinLabel.gameObject.SetActive(true);
			m_ExpendCoinSprite.gameObject.SetActive(false);
			m_FreeTimeLabel.gameObject.SetActive(false);
            if (m_CoinGainButton.isEnabled)
            {
                m_CoinGainButton.isEnabled = false;
            }
        }
        else
        {
            m_ExpendCoinLabel.text = "";
			m_ExpendCoinLabel.gameObject.SetActive(false);
			m_ExpendCoinSprite.gameObject.SetActive(true);
			m_FreeTimeLabel.gameObject.SetActive(true);
			//==========倒计时后修正免费获取文字
			UpdateMainInfo();
			//==========倒计时后修正免费获取文字end
            m_CoinGainButton.isEnabled = true;
        }
    }


    void OnClickContinueButton()
    {
        //背包空间不足
        int emptyCount = GameManager.gameManager.PlayerDataPool.FellowContainer.GetEmptySlotCount();
        if (emptyCount <= 0)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1831}");
            return;
        }

        if (m_CurGainType == 1)
        {
            //金币抽取
            CG_ASK_GAIN_FELLOW fellowPacket = (CG_ASK_GAIN_FELLOW)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_GAIN_FELLOW);
            fellowPacket.SetGainTypee(1);
            fellowPacket.SendPacket();
        }
        else if (m_CurGainType == 2)
        {
            //元宝抽取
            CG_ASK_GAIN_FELLOW fellowPacket = (CG_ASK_GAIN_FELLOW)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_GAIN_FELLOW);
            fellowPacket.SetGainTypee(2);
            fellowPacket.SendPacket();
        }
    }

    void OnClickGamble_Coin()
    {
        // 新手指引
        if (PartnerFrameLogic.Instance() && PartnerFrameLogic.Instance().NewPlayerGuide_Step == 7)
        {
            PartnerFrameLogic.Instance().NewPlayerGuide(8);
        }

        //伙伴槽位是否已满
        int emptyCount = GameManager.gameManager.PlayerDataPool.FellowContainer.GetEmptySlotCount();
        if (emptyCount <= 0)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1831}");
            return;
        }

        int freeGainCount = GameManager.gameManager.PlayerDataPool.FellowGainCount_Free;
        if (freeGainCount < 10)
        {
//            if (null != GameManager.gameManager)
//            {
//                GameManager.gameManager.SoundManager.PlaySoundEffect(135);  //yes
//            }

            //发包抽取
            CG_ASK_GAIN_FELLOW fellowPacket = (CG_ASK_GAIN_FELLOW)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_GAIN_FELLOW);
            fellowPacket.SetGainTypee(3);
            fellowPacket.SendPacket();
        }
        else
        {
            //金币是否足够
            int coinGainCount = GameManager.gameManager.PlayerDataPool.FellowGainCount_Coin;
            if (coinGainCount >= TableManager.GetFellowGainCost().Count)
            {
                coinGainCount = TableManager.GetFellowGainCost().Count - 1;
            }
            Tab_FellowGainCost coinline = TableManager.GetFellowGainCostByID(coinGainCount + 1, 0);
            if (coinline != null)
            {
                if (coinline.NormalConsumeNum > GameManager.gameManager.PlayerDataPool.Money.GetMoney_Coin())
                {
                    Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1830}");
                    return;
                }

                if (null != GameManager.gameManager)
                {
                    GameManager.gameManager.SoundManager.PlaySoundEffect(135);  //yes
                }

                //发包抽取
                CG_ASK_GAIN_FELLOW fellowPacket = (CG_ASK_GAIN_FELLOW)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_GAIN_FELLOW);
                fellowPacket.SetGainTypee(1);
                fellowPacket.SendPacket();
            }
        }
    }

    void OnClickGamble_YuanBao()
    {
        //元宝是否足够
        int yuanbaoGainCount = GameManager.gameManager.PlayerDataPool.FellowGainCount_YuanBao;
        if (yuanbaoGainCount >= TableManager.GetFellowGainCost().Count)
        {
            yuanbaoGainCount = TableManager.GetFellowGainCost().Count - 1;
        }
        Tab_FellowGainCost yuanbaoline = TableManager.GetFellowGainCostByID(yuanbaoGainCount + 1, 0);
        if (yuanbaoline != null)
        {
            int totalYuanbao = GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBao() + GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBaoBind();
            if (yuanbaoline.YuanBaoConsumeNum > totalYuanbao)
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1832}");
                return;
            }

			MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{4969}",yuanbaoline.YuanBaoConsumeNum), "", Gamble_YuanBaoOk, null);
        }
    }
	void Gamble_YuanBaoOk()
	{
		//背包是否已满
		int emptyCount = GameManager.gameManager.PlayerDataPool.FellowContainer.GetEmptySlotCount();
		if (emptyCount <= 0)
		{
			Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1831}");
			return;
		}
		
		if (null != GameManager.gameManager)
		{
			GameManager.gameManager.SoundManager.PlaySoundEffect(135);   //yes
		}
		
		//发包抽取
		CG_ASK_GAIN_FELLOW fellowPacket = (CG_ASK_GAIN_FELLOW)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_GAIN_FELLOW);
		fellowPacket.SetGainTypee(2);
		fellowPacket.SendPacket();
	}

    void OnClickGamble_10Coin()
    {
        //伙伴槽位是否已满
        int emptyCount = GameManager.gameManager.PlayerDataPool.FellowContainer.GetEmptySlotCount();
        if (emptyCount < 10)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2734}");
            return;
        }

        //金币是否足够
        int coinGainCount = GameManager.gameManager.PlayerDataPool.FellowGainCount_Coin;
        int totalCoin = GetNeedCoin10(coinGainCount);
        if (totalCoin > GameManager.gameManager.PlayerDataPool.Money.GetMoney_Coin())
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1830}");
            return;
        }

        if (null != GameManager.gameManager)
        {
            GameManager.gameManager.SoundManager.PlaySoundEffect(135);  //yes
        }

        //发包抽取
        CG_ASK_GAIN_10_FELLOW fellowPacket = (CG_ASK_GAIN_10_FELLOW)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_GAIN_10_FELLOW);
        fellowPacket.SetGainTypee(1);
        fellowPacket.SendPacket();
    }

    void OnClickGamble_10YuanBao()
    {
        //伙伴槽位是否已满
        int emptyCount = GameManager.gameManager.PlayerDataPool.FellowContainer.GetEmptySlotCount();
        if (emptyCount < 10)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2734}");
            return;
        }

        //元宝是否足够
        int yuanbaoGainCount = GameManager.gameManager.PlayerDataPool.FellowGainCount_YuanBao;
        int needYuanBao = GetNeedYuanBao10(yuanbaoGainCount);
        int totalYuanbao = GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBao() + GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBaoBind();
        if (needYuanBao > totalYuanbao)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1832}");
            return;
        }

		MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{4969}",needYuanBao), "", Gamble_10YuanBaoOk, null);

    }

	void Gamble_10YuanBaoOk()
	{
		if (null != GameManager.gameManager)
		{
			GameManager.gameManager.SoundManager.PlaySoundEffect(135);       //yes
		}
		
		//发包抽取
		CG_ASK_GAIN_10_FELLOW fellowPacket = (CG_ASK_GAIN_10_FELLOW)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_GAIN_10_FELLOW);
		fellowPacket.SetGainTypee(2);
		fellowPacket.SendPacket();
	}

    //连抽10次所需金币数
    int GetNeedCoin10(int curCount)
    {
        int totalCoin = 0;
        int coinGainCount = curCount;
        for (int i = 0; i < 10; i++)
        {
            coinGainCount++;
            if (coinGainCount >= TableManager.GetFellowGainCost().Count)
            {
                coinGainCount = TableManager.GetFellowGainCost().Count;
            }
            Tab_FellowGainCost coinline = TableManager.GetFellowGainCostByID(coinGainCount, 0);
            if (coinline != null)
            {
                totalCoin = totalCoin + coinline.NormalConsumeNum;
            }
        }
        return totalCoin;
    }

    //连抽10次所需元宝数
    int GetNeedYuanBao10(int curCount)
    {
        return 1350;
        /*int needYuanBao = 0;
        int yuanbaoGainCount = curCount;
        for (int i = 0; i < 10; i++)
        {
            yuanbaoGainCount++;
            if (yuanbaoGainCount >= TableManager.GetFellowGainCost().Count)
            {
                yuanbaoGainCount = TableManager.GetFellowGainCost().Count;
            }
            Tab_FellowGainCost yuanbaoline = TableManager.GetFellowGainCostByID(yuanbaoGainCount, 0);
            if (yuanbaoline != null)
            {
                needYuanBao = needYuanBao + yuanbaoline.YuanBaoConsumeNum;
            }
        }
        return needYuanBao;*/
    }

    public void UpdateGainRemain()
    {
        int remainCount = 10 - GameManager.gameManager.PlayerDataPool.FellowGainCount_Free;
        bool bRet = GameManager.gameManager.PlayerDataPool.CommonData.GetCommondFlag((int)USER_COMMONFLAG.CF_FELLOWFUNCTION_OPENFLAG);
        if (remainCount > 0 && bRet)
        {
            m_PartnerGainRemain.gameObject.SetActive(true);
            m_PartnerGainRemain.text = remainCount.ToString();
        }
        else
        {
            m_PartnerGainRemain.gameObject.SetActive(false);
        }
    }

    public void OnClickCloseGainFellow()
    {
        m_GainPartnerPanel.gameObject.SetActive(false);
        Utils.CleanGrid(m_GainPartnerListGrid);
    }
}
