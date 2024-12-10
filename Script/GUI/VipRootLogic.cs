using UnityEngine;
using System.Collections;
using GCGame.Table;
using GCGame;
using Module.Log;
using Games.GlobeDefine;
using Games.Item;
using System.Collections.Generic;
using Games.LogicObj;

public class VipRootLogic : UIControllerBase<VipRootLogic>
{
    public UILabel      m_ProgressTips;
    public UILabel      m_ProgressText;
    public GameObject   m_Progress;
    public UILabel      m_LevelIntro;
    public UIDraggablePanel m_Grid;
    public TabController m_Tab;
    public UISprite m_VipTips;
    public UILabel m_BonusText;

    public UISprite m_NewVIP;
    public UISprite m_NewVIPGold;
    public UILabel m_NowVipLevel;
    public UILabel m_NextVipLevel;
    public UILabel m_MaxVipLevel;

    public VipItem m_Bonus1;
    public VipItem m_Bonus2;
    public VipItem m_Bonus3;
    public VipItem m_Bonus4;
    public VipItem m_Bonus5;

    //buffer data
    private int         m_SavedItem;
    private int         m_ShowLevel;
    

	public UIPanel m_TextPanel;

    void Awake()
    {
        SetInstance(this);
    }

	void Start () 
    {
        m_Tab.delTabChanged = OnClickedList;
        UpdateVipInfo();
        RefreshVIPTips();
	}

    void OnDestroy()
    {
        SetInstance(null);
    }

    void OnCloseClick()
    {
        UIManager.CloseUI(UIInfo.VipRoot);
    }   

    void OnClickItem(GameObject go)
    {
       
            GameItem item = new GameItem();
            item.DataID =int.Parse( go.name);
            if (item.IsEquipMent())
            {
                EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Info);
            }
            else
            {
                ItemTooltipsLogic.ShowItemTooltip(item, ItemTooltipsLogic.ShowType.Info);
            }
        
    }

    public void RefreshVIPTips()
    {
        m_VipTips.gameObject.SetActive(GameManager.gameManager.PlayerDataPool.IsShowVipTip);

        if (GameManager.gameManager.PlayerDataPool.IsShowVipTip)
        {
            GameManager.gameManager.PlayerDataPool.IsShowVipTip = false;

            if (RechargeBarLogic.Instance() != null)
            {
                RechargeBarLogic.Instance().RefreshVIPTips();
            }
        }
    }

    void UpdateVipInfo()
    {
        int nCost = 0;
        m_SavedItem = 0;
        m_ShowLevel = 0;
        Obj_MainPlayer obj = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (obj != null)
        {
            nCost = obj.VipCost;
        }
        if (  nCost >= 0  )
        {
            int nLevel = 0;
            int nLeft = 0;
            int nMax = 0;
            VipData.GetVipLevel(nCost, ref nLevel, ref nLeft);            

            if (nLevel >= VipData.m_MaxShowLevel)
            {
                nMax = GetMaxCostByLevel(nLevel);
                nLeft = nMax;
                m_ShowLevel = VipData.m_MaxShowLevel;
            }
            else
            {
                nMax = GetMaxCostByLevel(nLevel + 1);
                m_ShowLevel = nLevel;
            }

            if ( nMax >= 0 )
            {
                float nPercent = (float)nLeft / nMax;    
                //进度条
                m_Progress.transform.localScale = new Vector3(nPercent, 1, 1);
                //进度条数字
                m_ProgressText.text = nLeft.ToString() + "/" + nMax.ToString();
                //进度条提示
                if (nLevel < VipData.m_MaxShowLevel)
                {                   
                    //m_ProgressTips.text = "再充值" + (nMax - nLeft).ToString() + "升级为VIP" + (nLevel + 1).ToString();
                    m_ProgressTips.text = StrDictionary.GetClientDictionaryString("#{2880}", (nMax - nLeft), nLevel+1);
                    
                }
                else
                {
                    m_ProgressTips.text = "";
                }
                if (nLeft == nMax)
                {
                    m_NewVIPGold.gameObject.SetActive(false);
                    m_NewVIP.gameObject.SetActive(false);
                    m_NewVIP.gameObject.SetActive(false);
                }
                else
                {
                    m_NewVIPGold.gameObject.SetActive(false);
                    m_NewVIP.gameObject.SetActive(true);
                }
             }
            //滚动
            if (m_ShowLevel == 0)
            {
                m_ShowLevel = 1;
            }
            m_Grid.MoveRelative(new Vector3(0,62*(m_ShowLevel-1),0));
            m_Tab.ChangeTab(System.String.Format("ImageButton{0:D2}", m_ShowLevel));
            //默认显示详细信息
            RefreshVIPIntro(m_ShowLevel);



            //等级跨度显示
            if ( nLevel >= VipData.m_MaxShowLevel )
            {
                m_NowVipLevel.text =  VipData.m_MaxShowLevel.ToString(); 
                m_NextVipLevel.text =  nLevel.ToString();
            }
            else
            {
                m_NowVipLevel.text = nLevel.ToString();
                m_NextVipLevel.text =  (nLevel + 1).ToString();
            }
        }
        else
        {
            //no show 
            m_ProgressText.text = "";
            m_ProgressTips.text = "";
            m_Progress.transform.localScale = new Vector3(0, 1, 1);
            m_BonusText.text = "";
            m_LevelIntro.text = ""; 
        }
    }

    

    int GetMaxCostByLevel(int nLevel)
    {
        Tab_VipBook tBook = TableManager.GetVipBookByID(nLevel, 0);
        if (tBook != null)
        {
            return tBook.VipCost;
        }
        return 99999999;
    }

    bool ShowItemS(int nLevel, int nProfession)
    {
        Tab_VipBook tBook = TableManager.GetVipBookByID(nLevel, 0);
       
        m_Bonus1.gameObject.SetActive(true);
        m_Bonus1.UpdateItem(tBook.VipItem1Num, tBook.GetVipItem1byIndex(nProfession));

        m_Bonus2.gameObject.SetActive(true);
        m_Bonus2.UpdateItem(tBook.VipItem2Num, tBook.GetVipItem2byIndex(nProfession));

        m_Bonus3.gameObject.SetActive(true);
        m_Bonus3.UpdateItem(tBook.VipItem3Num, tBook.GetVipItem3byIndex(nProfession));

        m_Bonus4.gameObject.SetActive(true);
        m_Bonus4.UpdateItem(tBook.VipItem4Num, tBook.GetVipItem4byIndex(nProfession));

        m_Bonus5.gameObject.SetActive(true);
        m_Bonus5.UpdateItem(tBook.VipItem5Num, tBook.GetVipItem5byIndex(nProfession));
        return true;
    }

    void HideItem()
    {
        m_Bonus1.gameObject.SetActive(false);
        m_Bonus2.gameObject.SetActive(false);
        m_Bonus3.gameObject.SetActive(false);
        m_Bonus4.gameObject.SetActive(false);
        m_Bonus5.gameObject.SetActive(false);
    }


    string GetItemIconById(int nItemId)
    {
        Tab_CommonItem tBook = TableManager.GetCommonItemByID(nItemId, 0);
        if (tBook != null)
        {
            return tBook.Icon;
        }
        return "";
    }    

    void RefreshVIPIntro( int nLevel )
    {
        m_LevelIntro.text = "";
        Tab_VipBook tBook = TableManager.GetVipBookByID(nLevel, 0);
        if (tBook != null && nLevel > 0)
        {
            //m_LevelIntro.text += "升级到VIP" + nLevel.ToString() + "级后，您将享有以下特权：\n";
            m_LevelIntro.text += StrDictionary.GetClientDictionaryString("#{2883}", nLevel);

            //m_LevelIntro.text += "1 每天购买体力次数最多为" + VipData.GetVipStamina(nLevel).ToString() + "次\n";
            m_LevelIntro.text += StrDictionary.GetClientDictionaryString("#{2884}", VipData.GetVipStamina(nLevel));

            int deskindex = VipData.GetDeskIndex(nLevel);
            if ( deskindex >= 0 )
            {
                m_LevelIntro.text += StrDictionary.GetClientDictionaryString("#{3243}", deskindex);
            }
            if ( nLevel >= GlobeVar.USE_AUTOFIGHT_VIPLEVEL )
            {
                m_LevelIntro.text += StrDictionary.GetClientDictionaryString("#{3244}");
            }

            //m_LevelIntro.text += "4 每日各副本免费进入次数增加" + tBook.VipCopyScene.ToString() + "次\n";
            int nTotal = tBook.VipYZW + tBook.VipYWGM + tBook.VipZLQJ + tBook.VipZLQJ + tBook.VipSSS + tBook.VipNHCJ + tBook.VipJXZ;
            if (nTotal > 0)
            {
                m_LevelIntro.text += StrDictionary.GetClientDictionaryString("#{2887}"); 
            }
            if ( tBook.VipYZW > 0 )
            {
                m_LevelIntro.text += StrDictionary.GetClientDictionaryString("#{3139}", tBook.VipYZW);
            }
            if ( tBook.VipYWGM > 0 )
            {
                m_LevelIntro.text += StrDictionary.GetClientDictionaryString("#{3141}", tBook.VipYWGM);
            }
            if ( tBook.VipZLQJ > 0 )
            {
                m_LevelIntro.text += StrDictionary.GetClientDictionaryString("#{3142}", tBook.VipZLQJ);
            }
            if ( tBook.VipSSS > 0 )
            {
                m_LevelIntro.text += StrDictionary.GetClientDictionaryString("#{3143}", tBook.VipSSS);
            }
            if ( tBook.VipNHCJ > 0 )
            {
                m_LevelIntro.text += StrDictionary.GetClientDictionaryString("#{3144}", tBook.VipNHCJ);
            }
            if ( tBook.VipJXZ > 0 )
            {
                m_LevelIntro.text += StrDictionary.GetClientDictionaryString("#{3140}", tBook.VipJXZ);
            }

            //m_LevelIntro.text += " 清杀气" + tBook.VipPKValue.ToString();
           // m_LevelIntro.text += "6 每日免费摇奖次数增加" + tBook.VipBonusBox.ToString()+"次";

            /*Tab_CommonItem tItemBook = TableManager.GetCommonItemByID(tBook.VipItem.ToString(), 0);
            if (tItemBook != null)
            {
                m_LevelIntro.text += " 升级奖励物品" + tItemBook.Name;
            } */

            int nProfession = GameManager.gameManager.PlayerDataPool.Profession;
            bool isShowItemS = ShowItemS(nLevel, nProfession);
            //0级不应该有物品领取，直接不显示。
            if (isShowItemS == true && nLevel > 0)
            {

               
                //m_BonusEquip1.SetActive(true);
                //奖励物品提示
                //m_BonusText.text = nLevel.ToString() + "级会员可以领取";// +GetItemNameById(nItem);
                m_BonusText.text = StrDictionary.GetClientDictionaryString("#{2881}", nLevel);

                //奖励物品图标
                //m_BonusImage.spriteName = GetItemIconById(nItem);
                //m_SavedItem = nItem;
                //if (tBook.Number > 1)
                //{
                //    m_BonusNum.text = tBook.Number.ToString();
                //}
                //else
                //{
                //    m_BonusNum.text = "";
                //}
            }
            else
            {

                
                //m_BonusEquip.SetActive(false);
                HideItem();
                m_BonusText.text = "";
               // m_BonusImage.spriteName = "";
            }
        }

	

    }

    void OnClickedList(TabButton value)
    {
        for (int n = 1; n <= VipData.m_MaxShowLevel; ++n)
        {
            if (value.name.EndsWith(System.String.Format("{0:D2}", n)))
            {
                RefreshVIPIntro(n);
				m_TextPanel.gameObject.transform.localPosition =  new Vector3(-158f,-130f,0f);
				m_TextPanel.clipRange = new Vector4(242.1f,-69,700,138);
                return;
            }
        }
    }

    void OnClickCharge()
    {
        RechargeData.PayUI();
    }
}
