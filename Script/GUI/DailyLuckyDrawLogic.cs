/************************************************************************/
/* 文件名：DailyLuckyDrawLogic.cs    
 * 创建日期：2014.04.17
 * 创建人：gaona
 * 功能说明：每日幸运抽奖界面
/************************************************************************/
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GCGame.Table;
using Games.DailyLuckyDraw;
using Module.Log;

public class DailyLuckyDrawLogic :  UIControllerBase<DailyLuckyDrawLogic>
{

    public const int m_nMaxBonusBoxCount = 14;//有修改，需要同步修改DailyLuckyDrawdata.cs
    public const int m_nMaxGainBonusCount = 10;//有修改，需要同步修改DailyLuckyDrawdata.cs
    public const int m_nDrawOneMoney = 25;
    public const int m_nDrawTenMoney = 225;
    public const int m_BonusEffectID = 134;

    //转盘转动时间差
    public const float m_fRoteateTimeDiffOne = 0.15f;
    public const float m_fRoteateTimeDiffTen = 0.3f;
    //打开状态图片
    public const string m_OpenSpriteName = "Hovxiangzi";
    //关闭状态图片
    public const string m_CloseSpriteName = "xiangzi";
    //高亮状态图片
    public const string m_HoverSpriteName = "Hovxiangzi";

    public UILabel m_LabelFreeTimes;
    public UILabel m_LabelBindYuanBaoCount;
    public UILabel m_LabelYuanBaoCount;

    public GameObject m_BtnBonuxArraw;

    public UISprite[] m_BonusBoxArray;

    public UILabel m_ResultTextLabel;

    public UIImageButton m_BtnDrawOne;
    public UIImageButton m_BtnDrawTen;
    public UIImageButton m_BtnDrawTime;
    public UIImageButton m_BtnDrawOneCost;

    public UIPanel m_InfoTextPanel;

   //两帧帧之间的时间差
    private float m_fFrameTimeDiff;
    //显示抽取一次的转盘
    private bool m_bShowDrawOneTurnTable;
    //显示抽取十次的转盘
    private bool m_bShowDrawTenTurnTalbe;
    //转盘箭头转过角度计数
    private int m_nArrawTurnCount;
    //转盘箭头需要转过的角度
    private int m_nMaxArrawTurnCount;
    //抽取十次，每次变换的时间间隔
    private float m_fRoteateTimeTick;
    //特效是否显示中
    private bool m_bBonusEffectShow;
    //抽取一次类型
    private DLDDRAWTYPE m_nDrawOneType;

    private List<string> m_ResultTextList = new List<string>();

    void OnEnable()
    {
        SetInstance(this);
        DailyLuckyDrawDataShow();
        InvokeRepeating("UpdateTurnTable", 0f, m_fRoteateTimeDiffOne);
    }
    void OnDisable()
    {
        BeforeDestory();
        SetInstance(null);
        CancelInvoke("UpdateTurnTable");
    }
 
    public void BeforeDestory()
    {
        //没有播放特效即没有发送抽奖消息包
        if (!m_bBonusEffectShow)
        {
            GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.Drawing = false;
        }
    }
    //DailyLuckyDrawDataShow
    public void DailyLuckyDrawDataShow()
    {
        CleanUp();
        UpdateTurnTableOnStart();
        UpdateNumbers();
        UpdateMoney();
        ClearUpAllResultTestLabel();
        SetDrawButtonState(!GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.Drawing);
    }
    public void CleanUp()
    {
        m_fFrameTimeDiff = 0;
        m_bShowDrawOneTurnTable = false;
        m_bShowDrawTenTurnTalbe = false;
        m_nArrawTurnCount = 0;
        m_nMaxArrawTurnCount = 0;
        m_bBonusEffectShow = false;
    }
    //更新转盘
    public void UpdateTurnTableOnStart()
    {              
       //更新box状态
        CloseAllBonusBox();
     
        //复位方向
        m_nArrawTurnCount = 0;
    }

    //抽取一次
    public void DailyLuckyDrawOne()
    {
        //正在等待抽奖回包，不能抽奖
        if (GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.Drawing)
        {
            return;
        }

        //判断条件
        if (GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawFreeTimes <= 0 || GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawFreeCDTime > 0)
        {
            int nYuanBaoCount = GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBao() + GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBaoBind();
            if ( nYuanBaoCount < m_nDrawOneMoney)
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1817}");
                return;
            }
            else
            {
                string dicStr = "";
                if (GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawFreeCDTime > 0)
                {
                    dicStr = StrDictionary.GetClientDictionaryString("#{2411}", m_nDrawOneMoney);
                }
                else
                {
                    dicStr = StrDictionary.GetClientDictionaryString("#{1813}", m_nDrawOneMoney);
                }
                //会扣元宝，要继续吗？
                m_nDrawOneType = DLDDRAWTYPE.DLD_DRAWTYPE_ONE_YUANBAO;
                MessageBoxLogic.OpenOKCancelBox(dicStr, "", DoDailyLuckyDrawOne);
            }
        }
        else if (GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawFreeTimes > 0 && GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawFreeCDTime <= 0)
        {
            m_nDrawOneType = DLDDRAWTYPE.DLD_DRAWTYPE_ONE;
            DoDailyLuckyDrawOne();
        }
    }
    public void DoDailyLuckyDrawOne()
    {
        //设置处于抽奖状态
        GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.Drawing = true;
        //抽奖按钮灰化
        SetDrawButtonState(false);
        //没有处于旋转状态
        if (!m_bShowDrawOneTurnTable && !m_bShowDrawTenTurnTalbe)
        {
            m_bShowDrawOneTurnTable = true;

            //音效
            GameManager.gameManager.SoundManager.PlaySoundEffect(130);

            //转盘全部置为关闭状态
            CloseAllBonusBox();

            //复位方向 重新开始计数
            m_nMaxArrawTurnCount = 0;
            m_nArrawTurnCount = 0;

            m_nMaxArrawTurnCount = m_nMaxBonusBoxCount + UnityEngine.Random.Range(0, m_nMaxBonusBoxCount);
        }
    }

    //抽取十次
    public void DailyLuckyDrawTen()
    {
        //正在等待抽奖回包，不能抽奖
        if (GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.Drawing)
        {
            return;
        }
        //判断条件
        int nYuanBaoCount = GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBao() + GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBaoBind();
        if (nYuanBaoCount < m_nDrawTenMoney)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1817}");
            return;
        }
        else
        {
            string dicStr = StrDictionary.GetClientDictionaryString("#{1814}", m_nDrawTenMoney);
            //会扣元宝，要继续吗？
            MessageBoxLogic.OpenOKCancelBox(dicStr, "", DoDailyLuckyDrawTen);
        }
    }
    public void DoDailyLuckyDrawTen()
    {
        //设置处于抽奖状态
        GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.Drawing = true;
        //抽奖按钮灰化
        SetDrawButtonState(false);
        //没有处于旋转状态
        if (!m_bShowDrawOneTurnTable && !m_bShowDrawTenTurnTalbe)
        {
            m_bShowDrawTenTurnTalbe = true;

            //音效
            GameManager.gameManager.SoundManager.PlaySoundEffect(132);

            //转盘全部置为关闭状态
            CloseAllBonusBox();

            //复位方向 重新开始计数
            m_nMaxArrawTurnCount = 0;
            m_nArrawTurnCount = 0;
        }

    }

    //设置抽奖按钮状态
    public void SetDrawButtonState(bool bEnable)
    {
        m_BtnDrawOne.isEnabled = bEnable;
        m_BtnDrawTen.isEnabled = bEnable;
    }

    //抽取一次界面转盘处理
    public void DrawOneTurnTableShow()
    {
        CloseAllBonusBox();

        int nIndex = m_nMaxArrawTurnCount % m_nMaxBonusBoxCount;        
        if(nIndex >= 0 && nIndex < m_BonusBoxArray.Length)
            m_BonusBoxArray[nIndex].spriteName = m_OpenSpriteName;

        m_nArrawTurnCount = 0;
        m_nMaxArrawTurnCount = 0;

        m_bShowDrawOneTurnTable = false;
        m_bBonusEffectShow = false;

        //抽奖按钮可使用
        SetDrawButtonState(true);
        //刷新结果信息
        //UpdateResultText();
        //刷新数字信息
        UpdateNumbers();
    }

    //抽取十次界面转盘处理
    public void DrawTenTurnTableShow()
    {
        RandomTurnTableBonusBox();
        m_bShowDrawTenTurnTalbe = false;
        m_bBonusEffectShow = false;
        //抽奖按钮可使用
        SetDrawButtonState(true);
        //刷新结果信息
        //UpdateResultText();
        //刷新数字信息
        UpdateNumbers();
    }
    public void UpdateTurnTable()
    {
        m_fFrameTimeDiff += m_fRoteateTimeDiffOne;

        //需要显示抽取一次的旋转效果 
        if (m_bShowDrawOneTurnTable && m_nMaxArrawTurnCount > 0)
        {
            //旋转一次时间差到了
            if (m_fFrameTimeDiff >= m_fRoteateTimeDiffOne)
            {
                m_fFrameTimeDiff = 0;

                if ( !m_bBonusEffectShow)
                {
                    //全部关闭状态
                    CloseAllBonusBox();
                    //设置高亮
                    if (m_nArrawTurnCount >= 0 && m_nArrawTurnCount < m_nMaxBonusBoxCount)
                    {
                        m_BonusBoxArray[m_nArrawTurnCount].spriteName = m_HoverSpriteName;
                    }
                }
           
                m_nArrawTurnCount++;

                //旋转的次数达到最大值 发送消息包
                if (m_nArrawTurnCount == m_nMaxBonusBoxCount)
                {
                    m_bBonusEffectShow = true;
                    CG_DAILYLUCKYDRAW_DRAW packet = (CG_DAILYLUCKYDRAW_DRAW)PacketDistributed.CreatePacket(MessageID.PACKET_CG_DAILYLUCKYDRAW_DRAW);
					packet.SetDrawtype((UInt32)m_nDrawOneType);                  
                    packet.SendPacket();
                }
            }
        }
        //需要显示抽取十次的旋转效果 
        else if (m_bShowDrawTenTurnTalbe)
        {
            //旋转一次时间差到了
            if (m_fFrameTimeDiff >= m_fRoteateTimeDiffTen)
            {
                m_fFrameTimeDiff = 0;

                if (!m_bBonusEffectShow)
                {
                    //全部关闭状态
                    CloseAllBonusBox();
                    //设置高亮
                    if (m_nArrawTurnCount % 2 == 0)
                    {
                        for (int nIndex = 0; nIndex < m_nMaxBonusBoxCount; )
                        {
                            m_BonusBoxArray[nIndex].spriteName = m_HoverSpriteName;
                            nIndex += 2;
                        }
                    }
                    else
                    {
                        for (int nIndex = 1; nIndex < m_nMaxBonusBoxCount; )
                        {
                            m_BonusBoxArray[nIndex].spriteName = m_HoverSpriteName;
                            nIndex += 2;
                        }
                    }
                }

                m_nArrawTurnCount++;


                // 发送消息包
                if (m_nArrawTurnCount == (m_nMaxBonusBoxCount/2))
                {
                    m_bBonusEffectShow = true;

                    CG_DAILYLUCKYDRAW_DRAW packet = (CG_DAILYLUCKYDRAW_DRAW)PacketDistributed.CreatePacket(MessageID.PACKET_CG_DAILYLUCKYDRAW_DRAW);
                    packet.SetDrawtype((int)DLDDRAWTYPE.DLD_DRAWTYPE_TEN);
                    packet.SendPacket();

                }
            }
        }
    }
    public void UpdateTurnTableBonusBox(int nBoxIndex)
    {
        if (nBoxIndex >= 0 && nBoxIndex < m_nMaxBonusBoxCount)
        {
            m_BonusBoxArray[nBoxIndex].spriteName = m_OpenSpriteName;
        }        
    }
    public void RandomTurnTableBonusBox()
    {
        bool [] bOpenFlag = new bool[m_nMaxBonusBoxCount];
       for (int i = 0; i < m_nMaxBonusBoxCount; i++ )
        {
           bOpenFlag[i] = false;
        }
        //随机十个OpenFlag
        for (int i = 0; i < m_nMaxGainBonusCount; i++ )
        {
            int nRandomIndex = UnityEngine.Random.Range(0, m_nMaxBonusBoxCount);
           //先随机
            if (!bOpenFlag[nRandomIndex])
            {
                bOpenFlag[nRandomIndex] = true;
            }
            else
            {
                //随机的已经置脏，遍历
                for (int j = 0; j < m_nMaxBonusBoxCount; j++)
                {
                    if (!bOpenFlag[j])
                    {
                        bOpenFlag[j] = true;
                        break;
                    }
                }
            }
        }
        //根据OpenFlag设置Box状态
        for (int i = 0; i < m_nMaxBonusBoxCount; i++)
        {
            if (bOpenFlag[i])
            {
                m_BonusBoxArray[i].spriteName = m_OpenSpriteName;
            }
            else
            {
                m_BonusBoxArray[i].spriteName = m_CloseSpriteName;
            }
        }
    }
    public void UpdateCDTime()
    {
        if (GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawFreeTimes > 0 && GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawFreeCDTime > 0)
        {
            int nCDTime = GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawFreeCDTime;
            int nMin = nCDTime / 60;
            int nSec = nCDTime % 60;
            m_LabelFreeTimes.text = (nMin / 10).ToString() + (nMin % 10).ToString() + ":" + (nSec / 10).ToString() + (nSec % 10).ToString();

        }
    }
    public void UpdateMoney()
    {
        m_LabelBindYuanBaoCount.text = GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBaoBind().ToString();
        m_LabelYuanBaoCount.text = GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBao().ToString();
    }
    public void UpdateNumbers()
    {
        if (GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawFreeTimes > 0)
        {
            if (GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawFreeCDTime > 0)
            {
                UpdateCDTime();
            }
            else
            {
                m_LabelFreeTimes.text = StrDictionary.GetClientDictionaryString("#{1821}", GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.DrawFreeTimes);
            }
            m_BtnDrawTime.gameObject.SetActive(true);
            m_BtnDrawOneCost.gameObject.SetActive(false);
        }
        else
        {
            m_BtnDrawTime.gameObject.SetActive(false);
            m_BtnDrawOneCost.gameObject.SetActive(true);
        }
    }
    public void CloseButtonClick()
    {
        UIManager.CloseUI(UIInfo.DailyDrawRoot);
    }
    public void ChongZhiButtonClick()
    {
        RechargeData.PayUI();
    }
    public void UpdateResultText()
    {
        //清空结果信息列表
        CleanResultText();
        //根据已获取的奖励ID，刷新奖励信息
        for (int i = 0; i < m_nMaxGainBonusCount; i++)
        {
            int nBonusID = GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.GetGainBonusID(i);
            if (nBonusID > 0)
            {
                Tab_DailyLuckyDrawBonusInfo BonusInfo = TableManager.GetDailyLuckyDrawBonusInfoByID(nBonusID,0);
                if (null == BonusInfo)
                {
                    LogModule.DebugLog("DailyLuckyDrawBonusInfo.txt has not Line ID=" + nBonusID);
                    return;
                }
                //金钱提示
                if (BonusInfo.MoneyCount > 0)
                {
                    string MoneyResultText = "";
                    switch(BonusInfo.MoneyType)
                    {
                        case (int)MONEYTYPE.MONEYTYPE_YUANBAO:
                            MoneyResultText = StrDictionary.GetClientDictionaryString("#{1818}",BonusInfo.MoneyCount);
                            break;
                        case (int)MONEYTYPE.MONEYTYPE_COIN:
                            MoneyResultText = StrDictionary.GetClientDictionaryString("#{1819}", BonusInfo.MoneyCount);
                            break;
                        case (int)MONEYTYPE.MONEYTYPE_YUANBAO_BIND:
                            MoneyResultText = StrDictionary.GetClientDictionaryString("#{1820}", BonusInfo.MoneyCount);
                            break;
                        default:
                            break;
                    }
                    AddResultText(MoneyResultText);
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
                        string ItemResultText =  ItemInfo.Name + "*" + ItemCount.ToString();
                        AddResultText(ItemResultText);
                    }
                }
                //经验提示
                if (BonusInfo.Exp > 0)
                {
                    string ExpResultText = BonusInfo.Exp.ToString() + StrDictionary.GetClientDictionaryString("#{1325}");
                    AddResultText(ExpResultText);
                }
            }
        }
        UpdateResultTextLabel();
    }
    public void CleanResultText()
    {
        m_ResultTextList.Clear();
    }
    public void AddResultText(string ResultText)
    {
        if (ResultText.Length <= 0)
        {
            return;
        }
        if (m_ResultTextList.Count > 0 && m_ResultTextList.Count >= m_nMaxGainBonusCount)
        {
            m_ResultTextList.RemoveAt(0);
        }
         m_ResultTextList.Add(ResultText);
    }
    public void UpdateResultTextLabel()
    {
        ClearUpAllResultTestLabel();

        string strResultText = "";
        for (int i = 0;  i < m_ResultTextList.Count; i++)
        {
            if ("" != m_ResultTextList[i])
            {
                if (strResultText.Length > 0)
                {
                    strResultText += "\n";
                }
               strResultText += m_ResultTextList[i];
            }
        }
       m_ResultTextLabel.text = strResultText;
    }
    public void ClearUpAllResultTestLabel()
    {
        m_ResultTextLabel.text = "";
    }
    public void HandleFailMsg(int nFailReason)
    {
        CleanUp();
        SetDrawButtonState(true);
        switch (nFailReason)
        {
            case (int)GC_DAILYLUCKYDRAW_FAIL.FAILTYPE.TYPE_FREETIME:
                {
                    Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1816}");
                }
                break;
            case (int)GC_DAILYLUCKYDRAW_FAIL.FAILTYPE.TYPE_MONEY:
                {
                    Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1817}");
                }
                break;
            case (int)GC_DAILYLUCKYDRAW_FAIL.FAILTYPE.TYPE_CD:
                {
                    if (!BonusItemGetLogic.Instance())
                    {
                         string dicStr = StrDictionary.GetClientDictionaryString("#{2411}", m_nDrawOneMoney);
                        //会扣元宝，要继续吗？
                        m_nDrawOneType = DLDDRAWTYPE.DLD_DRAWTYPE_ONE_YUANBAO;
                        MessageBoxLogic.OpenOKCancelBox(dicStr, "", DoDailyLuckyDrawOne);
                    }
                }
                break;
            case (int)GC_DAILYLUCKYDRAW_FAIL.FAILTYPE.TYPE_LEVEL:
                {
                    Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{3187}");
                }
                break;
           default:
                break;
        }
    }
    public void CloseAllBonusBox()
    {
        for (int i = 0; i < m_nMaxBonusBoxCount; i++)
        {
              m_BonusBoxArray[i].spriteName = m_CloseSpriteName;

        }
    }
    public void ShowBonusBoxEffect()
     {
         if (BackCamerControll.Instance() != null)
         {
             BackCamerControll.Instance().PlaySceneEffect(m_BonusEffectID);
         }
         if (null != GameManager.gameManager)
         {
             GameManager.gameManager.SoundManager.PlaySoundEffect(28);
         }
     }
    public void StopBonusBoxEffect()
    {
        if (BackCamerControll.Instance() != null)
        {
            BackCamerControll.Instance().StopSceneEffect(m_BonusEffectID, true);
        }
        if (null != GameManager.gameManager)
        {
            GameManager.gameManager.SoundManager.StopSoundEffect(28);
        }
    }
    public void ShowInfoText()
    {
        if (m_InfoTextPanel)
        {
            if (false == m_InfoTextPanel.gameObject.activeSelf)
            {
                m_InfoTextPanel.gameObject.SetActive(true);
            }
            else
            {
                m_InfoTextPanel.gameObject.SetActive(false);
            }
        }
    }
}

