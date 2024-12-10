//********************************************************************
// 文件名: EquipTooltipsLogic.cs
// 描述: 装备tips界面
// 作者: WangZhe
//********************************************************************

using UnityEngine;
using System.Collections;
using GCGame.Table;
using Games.Item;
using GCGame;
using System.Collections.Generic;
using Games.GlobeDefine;
using Games.LogicObj;
using Module.Log;
using Games.UserCommonData;

public class EquipTooltipsLogic : MonoBehaviour
{

    public enum ShowType
    {
        UnEquiped = 1,      //未装备的tooltips
        Equiped,            //已装备的tooltips
        Info,               //无操作按钮的tooltips
        ShopBuy,            // 购买界面
        ShopBuyBatch,       // 可以批量购买
        InfoCompare,     //无操作按钮的tooltips(可以比较身上的装备)
        QianKunDaiStuff,    // 乾坤袋放入 目前没有装备参与的配方 先留着
        ChatLink,//对话的超链接TIPS
        CangKu,             //仓库取出
        CangKuBackPack,     //仓库放入
    }

    public UILabel m_EquipNameLabel;
    public UILabel m_EquipProLabel;
    public UILabel m_EquipLevelLabel;
    public UILabel m_EquipBindLabel;
    public TooltipsEquipStar m_EquipStarLevel;
    public UILabel m_EquipStrengthenLevelLabel;
    public UILabel m_EquipPowerLabel;
    public TooltipsEquipAttr m_EquipAttrInfo;
    public TooltipsEquipGem m_EquipGemInfo;
    public UILabel m_EquipDescLabel;
    public UILabel m_EquipPriceLabel;
    public UISprite m_EquipPriceIcon;
    public UISprite m_EquipIconSprite;
    public UISprite m_EquipQualityGrid;
    public GameObject m_EquipTooltips;
    public GameObject m_CompareEquipTooltips;
    public UIGrid m_TooltipsGrid;
    public UIImageButton m_EquipBackButton;
    public UIImageButton m_EquipSellButton;
    public UIImageButton m_EquipSuitButton;
    public UIImageButton m_EquipTakeOffButton;
    public UIImageButton m_EquipUseButton;
    public UIImageButton m_EquipShareLinkButton;
    public UIImageButton m_EquipBuyButton;
    public UIImageButton m_EquipBuyBatchButton;
    public UIImageButton m_EquipEnchanceButton;
    public UIImageButton m_ConsignSaleBuyButton;
    public UIImageButton m_CangKuInButton;
    public UIImageButton m_CangKuOutButton;
    public UISlider m_EquipStrenExp;
    public UIGrid m_EquipOpButtonGrid;
    public UILabel m_EquipStrenExpInfo;
    public UILabel m_EquipStrenExpTitle;
    public UILabel m_EquipSellLabel;
    public UISprite m_EquipPowerArrowUpSprite;
    public UISprite m_EquipPowerArrowDownSprite;
    public GameObject m_EquipDisableSprite;
    public UILabel m_RemainTimeLable;
    public GameObject m_PutInQianKunDaiButton;
    public UIGrid m_OpButtonGrid;

    private GameItem m_Equip = null;
    private ShowType m_Type;

    private static EquipTooltipsLogic m_Instance = null;
    public static EquipTooltipsLogic Instance()
    {
        return m_Instance;
    }

    private static GameItem m_curItem;
    private static ShowType m_curType;
    private static ItemSlotLogic m_curItemSlot;
    public static void ShowEquipTooltip(int dataId, ShowType type, ItemSlotLogic slot = null)
    {
        GameItem item = new GameItem();
        item.DataID = dataId;
        if (item.IsValid() && item.IsEquipMent())
        {
            ShowEquipTooltip(item, type, slot);
        }
    }
    public static void ShowEquipTooltip(GameItem equip, ShowType type, ItemSlotLogic slot = null)
    {
        m_curItem = equip;
        m_curType = type;
        m_curItemSlot = slot;
        UIManager.ShowUI(UIInfo.EquipTooltipsRoot, EquipTooltipsLogic.OnShowEquipTip);
    }

    private static void OnShowEquipTip(bool bSuccess, object param)
    {
        if (!bSuccess)
        {
            LogModule.ErrorLog("load equiptooltip error");
            return;
        }

        EquipTooltipsLogic.Instance().ShowTooltips(m_curItem, m_curType);
    }

    void Awake()
    {
        m_Instance = this;
    }

    void Start()
    {
    }
    
    void OnDestroy()
    {
        m_Instance = null;
    }

    /// <summary>
    /// 显示装备Tooltips
    /// </summary>
    /// <param name="equip">装备</param>
    private void ShowTooltips(GameItem equip, ShowType type)
    {
        if (equip == null)
        {
            CloseWindow();
            return;
        }
        if (equip.IsValid() == false)
        {
            CloseWindow();
            return;
        }

        m_Equip = equip;
        m_Type = type;

        HideAllButtons();
        // 装备图标
        ShowEquipIcon(equip);
        // 玩家是否可用
        ShowEquipDisableSprite(equip);
        // 装备颜色品质
        ShowEquipQualityGrid(equip);
        // 装备名
        ShowEquipName(equip);
        // 职业
        ShowEquipPro(equip);
        // 人物等级需求
        ShowEquipLevel(equip);
        // 绑定信息
        ShowEquipBind(equip);
        // 是否可出售
        SetEquipSellInfo(equip);
        // 星级
        ShowEquipStarLevel(equip);
        // 强化等级
        ShowEquipStrengthenLevel(equip);
        // 战斗力
        ShowEquipPower(equip);
        // 属性
        ShowEquipAttrInfo(equip);
        // 宝石
        ShowEquipGemInfo(equip);
        // 描述
        ShowEquipDesc(equip);
        // 售价
        ShowEquipPrice(equip);
        // 强化经验
        ShowEquipStrenExp(equip);
        // 战斗力箭头
        ShowPowerArrow(equip);
        // 显示剩余时间
        ShowRemainTime(equip);

        if (type == ShowType.Equiped)  //点击装备槽位弹出的tips
        {
            //脱下按钮 显示
            m_EquipTakeOffButton.gameObject.SetActive(true);

            //分享链接按钮 显示
            m_EquipShareLinkButton.gameObject.SetActive(true);

            //装备强化按钮 显示
            if (equip.IsBelt() == false)
            { 
                m_EquipEnchanceButton.gameObject.SetActive(true);
            }

            //套装按钮 显示
            if (equip.GetEquipSetId() >= 0)
            {
                m_EquipSuitButton.gameObject.SetActive(true);
            }

            //隐藏战斗力箭头
            m_EquipPowerArrowUpSprite.gameObject.SetActive(false);
            m_EquipPowerArrowDownSprite.gameObject.SetActive(false);

        }
        else if (type == ShowType.UnEquiped)  //点击物品背包弹出的tips
        {
            //售出按钮 显示
            m_EquipSellButton.gameObject.SetActive(true);
            //装备按钮 显示
            m_EquipUseButton.gameObject.SetActive(true);
            //分享链接按钮 显示
            m_EquipShareLinkButton.gameObject.SetActive(true);
            //装备强化按钮 显示
            //if (equip.IsBelt() == false && equip.IsCharm() == false)
            //{
              //  m_EquipEnchanceButton.gameObject.SetActive(true);
           // }
            //套装按钮 显示
            if (equip.GetEquipSetId() >= 0)
            {
                m_EquipSuitButton.gameObject.SetActive(true);
            }

            m_OpButtonGrid.Reposition();

            //如果本装备位已经装备物品 弹出对比tips
            int slotindex = equip.GetEquipSlotIndex();
            GameItem CompareEquip = GameManager.gameManager.PlayerDataPool.EquipPack.GetItem(slotindex);
            if (CompareEquip != null && CompareEquip.IsValid())
            {
                OpenCompare(slotindex);
            }
        }
        else if (type == ShowType.QianKunDaiStuff)
        {
            m_PutInQianKunDaiButton.SetActive(true);

            //售出按钮 显示
            m_EquipSellButton.gameObject.SetActive(true);
            //装备按钮 显示
            m_EquipUseButton.gameObject.SetActive(true);
            //分享链接按钮 显示
            m_EquipShareLinkButton.gameObject.SetActive(true);
            //装备强化按钮 显示
            if (equip.IsBelt() == false && equip.IsCharm() == false)
            {
                m_EquipEnchanceButton.gameObject.SetActive(true);
            }
            //套装按钮 显示
            if (equip.GetEquipSetId() >= 0)
            {
                m_EquipSuitButton.gameObject.SetActive(true);
            }

            m_OpButtonGrid.Reposition();
        }
        else if (type == ShowType.ShopBuy)
        {
            m_PutInQianKunDaiButton.SetActive(false);
            // 购买隐藏
            m_EquipBuyButton.gameObject.SetActive(true);

            //隐藏战斗力箭头
            m_EquipPowerArrowUpSprite.gameObject.SetActive(false);
            m_EquipPowerArrowDownSprite.gameObject.SetActive(false);
        }
        else if (type == ShowType.ShopBuyBatch)
        {
            // 批量购买隐藏
            m_EquipBuyBatchButton.gameObject.SetActive(true);
            // 购买隐藏
            m_EquipBuyButton.gameObject.SetActive(true);

            //隐藏战斗力箭头
            m_EquipPowerArrowUpSprite.gameObject.SetActive(false);
            m_EquipPowerArrowDownSprite.gameObject.SetActive(false);
        }
        else if (type == ShowType.Info)  //仅显示信息 没有操作按钮的tips
        {
            //隐藏战斗力箭头
            m_EquipPowerArrowUpSprite.gameObject.SetActive(false);
            m_EquipPowerArrowDownSprite.gameObject.SetActive(false);
            m_EquipUseButton.gameObject.SetActive(false);
        }
        else if (type == ShowType.InfoCompare)  //仅显示信息 没有操作按钮的tips 同时做对比
        {
            //隐藏战斗力箭头
            m_EquipPowerArrowUpSprite.gameObject.SetActive(false);
            m_EquipPowerArrowDownSprite.gameObject.SetActive(false);
            //如果本装备位已经装备物品 弹出对比tips
            int slotindex = equip.GetEquipSlotIndex();
            GameItem CompareEquip = GameManager.gameManager.PlayerDataPool.EquipPack.GetItem(slotindex);
            if (CompareEquip != null && CompareEquip.IsValid())
            {
                OpenCompare(slotindex);
            }
        }
        else if (type == ShowType.ChatLink)  //仅显示信息 没有操作按钮的tips
        {
            //隐藏战斗力箭头
            m_EquipPowerArrowUpSprite.gameObject.SetActive(false);
            m_EquipPowerArrowDownSprite.gameObject.SetActive(false);
            //根据是否可以上架 决定是否显示求购按钮
            if (ConsignSaleBag.isCanConsignSale(m_Equip,true))
            {
               // m_ConsignSaleBuyButton.gameObject.SetActive(true);
            }
        }
        else if (type == ShowType.CangKu)  //仓库界面 仓库物品tips
        {
            //隐藏战斗力箭头
            m_EquipPowerArrowUpSprite.gameObject.SetActive(false);
            m_EquipPowerArrowDownSprite.gameObject.SetActive(false);
            
            //显示取回按钮
            m_CangKuOutButton.gameObject.SetActive(true);
        }
        else if (type == ShowType.CangKuBackPack)  //仓库界面 背包物品tips
        {
            //隐藏战斗力箭头
            m_EquipPowerArrowUpSprite.gameObject.SetActive(false);
            m_EquipPowerArrowDownSprite.gameObject.SetActive(false);

            //显示放入仓库按钮
            m_CangKuInButton.gameObject.SetActive(true);
        }

        m_EquipOpButtonGrid.repositionNow = true;

       // gameObject.SetActive(true);
       // UIManager.ShowUI(UIInfo.EquipTooltipsRoot);
    }

    void HideAllButtons()
    {
        //返回按钮 显示
        m_EquipBackButton.gameObject.SetActive(false);
        //售出按钮 隐藏
        m_EquipSellButton.gameObject.SetActive(false);
        //套装按钮 隐藏
        m_EquipSuitButton.gameObject.SetActive(false);
        //脱下按钮 显示
        m_EquipTakeOffButton.gameObject.SetActive(false);
        //装备按钮 隐藏
        m_EquipUseButton.gameObject.SetActive(false);
        //分享链接按钮 隐藏
        m_EquipShareLinkButton.gameObject.SetActive(false);
        // 批量购买隐藏
        m_EquipBuyBatchButton.gameObject.SetActive(false);
        // 购买隐藏
        m_EquipBuyButton.gameObject.SetActive(false);
        // 强化按钮 隐藏
        m_EquipEnchanceButton.gameObject.SetActive(false);
        //隐藏战斗力箭头
        m_EquipPowerArrowUpSprite.gameObject.SetActive(false);
        m_EquipPowerArrowDownSprite.gameObject.SetActive(false);
        //寄售行求购信息
        m_ConsignSaleBuyButton.gameObject.SetActive(false);
        //仓库放入按钮
        m_CangKuInButton.gameObject.SetActive(false);
        //仓库取回按钮
        m_CangKuOutButton.gameObject.SetActive(false);

        m_PutInQianKunDaiButton.SetActive(false);
    }

    void SetEquipSellInfo(GameItem equip)
    {
        if (equip != null && equip.IsValid())
        {
            Tab_CommonItem tabItem = TableManager.GetCommonItemByID(equip.DataID, 0);
            if (tabItem != null)
            {
                m_EquipSellLabel.text = "[FFFFCC]";
                m_EquipSellLabel.text += (ConsignSaleBag.isCanConsignSale(equip) && !equip.BindFlag) ? StrDictionary.GetClientDictionaryString("#{2382}") : StrDictionary.GetClientDictionaryString("#{2383}");
            }
        }
    }

    void ShowEquipIcon(GameItem equip)
    {
        if (equip != null && equip.IsValid())
        {
            m_EquipIconSprite.spriteName = equip.GetIcon();
            //m_EquipIconSprite.MakePixelPerfect();  
        }
    }

    void ShowEquipDisableSprite(GameItem equip)
    {
        if (equip != null && equip.IsValid())
        {
            if (equip.IsFitForPlayer())
            {
                m_EquipDisableSprite.SetActive(false);
            }
            else
            {
                m_EquipDisableSprite.SetActive(true);
            }
        }
    }

    void ShowEquipName(GameItem equip)
    {
        if (equip != null && equip.IsValid())
        {
            int nQuality = (int)equip.GetQuality();
            m_EquipNameLabel.text = Utils.GetItemNameColor(nQuality);
            Tab_CommonItem tabItem = TableManager.GetCommonItemByID(equip.DataID, 0);
            if (tabItem != null)
            {
                m_EquipNameLabel.text += tabItem.Name;
                int nExistTime = tabItem.ExistTime;
                if (nExistTime > 0)
                {
                    m_EquipNameLabel.text += "(" + (float)nExistTime / 60f / 24f + "天)";
                }
            }
        }
    }

    void ShowEquipQualityGrid(GameItem equip)
    {
        if (equip != null && equip.IsValid())
        {
            Tab_CommonItem tabItem = TableManager.GetCommonItemByID(equip.DataID, 0);
            if (tabItem != null)
            {
                int nQuality = tabItem.Quality;
                m_EquipQualityGrid.spriteName = GlobeVar.QualityColorGrid[nQuality - 1];
            }
        }
    }

    void ShowEquipPro(GameItem equip)
    {
        if (equip != null && equip.IsValid())
        {
            int nProfession = equip.GetProfessionRequire();
            if (0 <= nProfession && nProfession < (int)CharacterDefine.PROFESSION.MAX)
            {
                string strProText = "";
                Obj_MainPlayer mainplayer = Singleton<ObjManager>.Instance.MainPlayer;
                if (mainplayer != null)
                {
                    if (mainplayer.Profession == nProfession)
                    {
                        strProText = "[FFFFCC]";
                    }
                    else
                    {
                        strProText = "[E60012]";
                    }
                }
                strProText += StrDictionary.GetClientDictionaryString("#{" + CharacterDefine.PROFESSION_DICNUM[nProfession].ToString() + "}");
                m_EquipProLabel.text = strProText;
            }
            else
            {
                m_EquipProLabel.text = "";
            }
        }
    }

    void ShowEquipLevel(GameItem equip)
    {
        if (equip != null && equip.IsValid())
        {
            int nPlayerLevel = Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.Level;
            Tab_CommonItem tabItem = TableManager.GetCommonItemByID(equip.DataID, 0);
            if (null != tabItem)
            {
                int nEquipLevel = tabItem.MinLevelRequire;
                if (nPlayerLevel >= nEquipLevel)
                {
                    m_EquipLevelLabel.text = "[FFFFCC]";
                }
                else
                {
                    m_EquipLevelLabel.text = "[E60012]";
                }
                //string strEquipLevel = "人物等级需求: {0}";
                string strEquipLevel = StrDictionary.GetClientDictionaryString("#{2832}", nEquipLevel);
                //m_EquipLevelLabel.text += string.Format(strEquipLevel, nEquipLevel);
                m_EquipLevelLabel.text += strEquipLevel;
            }
        }
    }

    void ShowEquipBind(GameItem equip)
    {
        if (equip != null && equip.IsValid())
        {
            m_EquipBindLabel.text = "[FFFFCC]";
            if (equip.BindFlag)
            {
                //m_EquipBindLabel.text += "已绑定";
                m_EquipBindLabel.text += StrDictionary.GetClientDictionaryString("#{2827}");
            }
            else
            {
                //m_EquipBindLabel.text += "未绑定";
                m_EquipBindLabel.text += StrDictionary.GetClientDictionaryString("#{2828}");
            }
        }
    }

    void ShowEquipStarLevel(GameItem equip)
    {
        if (equip != null && equip.IsValid())
        {
            int nStarLevel = equip.StarLevel;
            m_EquipStarLevel.ShowStar(nStarLevel);
        }
    }

    void ShowEquipStrengthenLevel(GameItem equip)
    {
        if (equip != null && equip.IsValid())
        {
            int nStrengthenLevel = equip.EnchanceLevel;
            //string strStrengthenLevel = "[FFFF33]强化 +{0}";
            m_EquipStrengthenLevelLabel.text = StrDictionary.GetClientDictionaryString("#{2830}", nStrengthenLevel);
            //m_EquipStrengthenLevelLabel.text = string.Format(strStrengthenLevel, nStrengthenLevel);
            //腰带不显示强化等级
            if (equip.IsBelt() && equip.IsCharm())
            {
                m_EquipStrengthenLevelLabel.text = "";
            }
        }
    }

    void ShowEquipPower(GameItem equip)
    {
        if (equip != null && equip.IsValid())
        {
            //m_EquipPowerLabel.text = "战斗力: " + equip.GetCombatValue().ToString();
            m_EquipPowerLabel.text = StrDictionary.GetClientDictionaryString("#{2829}", equip.GetCombatValue());
        }
    }

    void ShowPowerArrow(GameItem equip)
    {
        if (equip != null && equip.IsValid())
        {
            if (equip.IsEquipMent())
            {
                if (equip.GetProfessionRequire() == GlobeVar.INVALID_ID || equip.GetProfessionRequire() == Singleton<ObjManager>.Instance.MainPlayer.Profession)
                {
                    //获得身上对应槽位的装备
                    int slotindex = equip.GetEquipSlotIndex();
                    GameItem compareEquip = GameManager.gameManager.PlayerDataPool.EquipPack.GetItem(slotindex);
                    if (compareEquip != null && compareEquip.IsValid())
                    {
                        if (compareEquip.GetCombatValue_NoStarEnchance() > equip.GetCombatValue_NoStarEnchance())
                        {
                            m_EquipPowerArrowDownSprite.gameObject.SetActive(true);
                            m_EquipPowerArrowUpSprite.gameObject.SetActive(false);
                            return;
                        }
                        else if (compareEquip.GetCombatValue_NoStarEnchance() == equip.GetCombatValue_NoStarEnchance())
                        {
                            m_EquipPowerArrowDownSprite.gameObject.SetActive(false);
                            m_EquipPowerArrowUpSprite.gameObject.SetActive(false);
                            return;
                        }
                    }
                    m_EquipPowerArrowDownSprite.gameObject.SetActive(false);
                    m_EquipPowerArrowUpSprite.gameObject.SetActive(true);
                }
            }
        }
    }

    void ShowRemainTime(GameItem equip)
    {
        if (equip != null && equip.IsValid())
        {
            Tab_CommonItem line = TableManager.GetCommonItemByID(equip.DataID, 0);
            if (line != null)
            {
                if (line.ExistTime != -1)
                {
                    int remainSecond = (line.ExistTime * 60) - (GlobalData.ServerAnsiTime - equip.CreateTime);
                    if (remainSecond > 0)
                    {
                        if (remainSecond >= 24 * 3600)
                        {
                            //m_RemainTimeLable.text = string.Format("{0}天", (int)(remainSecond / 3600 / 24));
                            m_RemainTimeLable.text = StrDictionary.GetClientDictionaryString("#{2833}", Mathf.RoundToInt((float)remainSecond / 3600.0f / 24.0f));
                        }
                        else if (remainSecond >= 60 * 60)
                        {
                            //m_RemainTimeLable.text = string.Format("{0}小时", (int)(remainSecond / 60 / 60));
                            m_RemainTimeLable.text = StrDictionary.GetClientDictionaryString("#{2834}", Mathf.RoundToInt((float)remainSecond / 60.0f / 60.0f));
                        }
                        else
                        {
                            //m_RemainTimeLable.text = string.Format("{0}小时", 1);
                            m_RemainTimeLable.text = StrDictionary.GetClientDictionaryString("#{2834}", 1);
                        }
                    }
                    else
                    {
                        //m_RemainTimeLable.text = string.Format("{0}小时", 0);
						//======如果取不到时间或者时间小于0则不显示
                        //m_RemainTimeLable.text = StrDictionary.GetClientDictionaryString("#{2834}", 0);
						m_RemainTimeLable.text = "";
						m_RemainTimeLable.gameObject.SetActive(false);
						//===end
                    }
                    m_RemainTimeLable.gameObject.SetActive(true);
                }
                else
                {
                    m_RemainTimeLable.gameObject.SetActive(false);
                }
            }
        }
    }

    void ShowEquipAttrInfo(GameItem equip)
    {
        if (equip != null && equip.IsValid())
        {
            bool bUnEquiped = false;
            //Debug.Log(m_Type);
            if (m_Type == ShowType.UnEquiped)
            {
                bUnEquiped = true;
            }
            m_EquipAttrInfo.ShowAttr(equip, bUnEquiped);
        }
    }

    void ShowEquipSetAttrInfo(GameItem equip)
    {
        if (equip != null && equip.IsValid())
        {
            m_EquipAttrInfo.ShowEquipSetAttr(equip);
        }
    }

    void ShowEquipGemInfo(GameItem equip)
    {
        if (equip != null && equip.IsValid())
        {
            if (equip.IsPlayerEquiped())
            {
                m_EquipGemInfo.InitGemInfo(equip.GetEquipSlotIndex());
            }
            else
            {
                m_EquipGemInfo.HideGemInfo();
            }
        }
    }

    void ShowEquipDesc(GameItem equip)
    {
        if (equip != null && equip.IsValid())
        {
            m_EquipDescLabel.text = "[FFFFCC]";
            Tab_CommonItem tabItem = TableManager.GetCommonItemByID(equip.DataID, 0);
            if (null != tabItem)
            {
                string strEquipDesc = tabItem.Tips;
                m_EquipDescLabel.text += strEquipDesc;
            }
        }
    }

    void ShowEquipPrice(GameItem equip)
    {
        if (equip != null && equip.IsValid())
        {
            Tab_CommonItem tabItem = TableManager.GetCommonItemByID(equip.DataID, 0);
            if (tabItem != null)
            {
                // 价格
                m_EquipPriceLabel.text = "[FFFFCC]";
                m_EquipPriceLabel.text += tabItem.SellPrice.ToString();

                // 出售方式 图标
                //             int nPower = Mathf.FloorToInt(Mathf.Log10((float)nEquipPrice));
                //             m_EquipPriceIcon.gameObject.transform.localPosition = new Vector3(84 + 12 * nPower, 0, 0);
                if (tabItem.SellMoneyType == (int)ItemSellMoneyType.TYPE_COIN)
                {
                    m_EquipPriceIcon.spriteName = "bi";
                }
                else if (tabItem.SellMoneyType == (int)ItemSellMoneyType.TYPE_YUANBAO)
                {
                    m_EquipPriceIcon.spriteName = "qian2";
                }
                else if (tabItem.SellMoneyType == (int)ItemSellMoneyType.TYPE_BIND_YUANBAO)
                {
                    m_EquipPriceIcon.spriteName = "qian3";
                }
            }
        }
    }

    //点击套装按钮
    void OnSuitClick()
    {
        if (m_Equip != null && m_Equip.IsValid())
        {
            ShowEquipSetAttrInfo(m_Equip);
            m_EquipSuitButton.gameObject.SetActive(false);
            m_EquipBackButton.gameObject.SetActive(true);
            m_EquipOpButtonGrid.repositionNow = true;
        }
    }

    //点击套装返回按钮
    void OnSuitBackClick()
    {
        if (m_Equip != null && m_Equip.IsValid())
        {
            ShowEquipAttrInfo(m_Equip);
            m_EquipSuitButton.gameObject.SetActive(true);
            m_EquipBackButton.gameObject.SetActive(false);
            m_EquipOpButtonGrid.repositionNow = true;
        }
    }

    void ShowEquipStrenExp(GameItem equip)
    {
        if (m_Equip != null && m_Equip.IsValid())
        {
            m_EquipStrenExp.gameObject.SetActive(true);
            m_EquipStrenExpInfo.gameObject.SetActive(true);
            m_EquipStrenExpTitle.gameObject.SetActive(true);

            int nEnchanceExpMax = equip.GetEnchanceExpMax();
            if (nEnchanceExpMax > 0)
            {
                m_EquipStrenExp.value = (float)(equip.EnchanceExp) / (float)nEnchanceExpMax;
                m_EquipStrenExpInfo.text = equip.EnchanceExp.ToString() + "/" + nEnchanceExpMax.ToString();
            }
            else
            {
                m_EquipStrenExp.value = 0;
                m_EquipStrenExpInfo.text = "0/0";
            }
            //腰带不显示强化经验 强化满级不显示强化经验
            if (equip.IsBelt()||equip.IsCharm() || equip.EnchanceLevel >= GlobeVar.EQUIP_ENCHANCE_MAX_LEVEL)
            {
                m_EquipStrenExp.gameObject.SetActive(false);
                m_EquipStrenExpInfo.gameObject.SetActive(false);
                m_EquipStrenExpTitle.gameObject.SetActive(false);
            }
        }
    }

    public void CloseWindow()
    {
        m_Equip = null;
        //gameObject.SetActive(false);       
        if (m_CompareEquipTooltips.activeSelf == true)
        {
            CloseCompare();
        }
        UIManager.CloseUI(UIInfo.EquipTooltipsRoot);
    }

    void OpenCompare(int nEquipSloct)
    {
        m_CompareEquipTooltips.SetActive(true);
        m_CompareEquipTooltips.GetComponent<CompareEquipTooltipsLogic>().ShowTooltips(nEquipSloct);
        m_TooltipsGrid.gameObject.transform.localPosition = new Vector3(-340, 0, 0);
        m_TooltipsGrid.repositionNow = true;
    }

    void CloseCompare()
    {
        m_CompareEquipTooltips.SetActive(false);
        m_TooltipsGrid.repositionNow = true;
        m_EquipTooltips.transform.localPosition = new Vector3(0, 0, 0);
        m_TooltipsGrid.gameObject.transform.localPosition = new Vector3(0, 0, 0);
    }

    void EquipSell()
    {

        if (m_Equip != null && m_Equip.IsValid())
        {
            if ((int)m_Equip.GetQuality() > (int)ItemQuality.QUALITY_BLUE)
            {

                MessageBoxLogic.OpenOKCancelBox(Utils.GetDicByID(4668), "提示信息", () =>
                {
                    List<ulong> selllist = new List<ulong>();
                    selllist.Add(m_Equip.Guid);
                    SysShopController.SellItem((int)GameItemContainer.Type.TYPE_BACKPACK, selllist);
                    CloseWindow();
                });
            }
            else 
            {
                List<ulong> selllist = new List<ulong>();
                selllist.Add(m_Equip.Guid);
                SysShopController.SellItem((int)GameItemContainer.Type.TYPE_BACKPACK, selllist);
                CloseWindow();
            }      
            
        }
    }

    void EquipUse(GameObject go)
    {
        if (m_Equip != null && m_Equip.IsValid())
        {
            int nPlayerLevel = Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.Level;
            if (nPlayerLevel < m_Equip.GetMinLevelRequire())
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1255}");
                return;
            }

            int nPlayerProfession = Singleton<ObjManager>.Instance.MainPlayer.Profession;
            if (nPlayerProfession != m_Equip.GetProfessionRequire() && m_Equip.GetProfessionRequire() != -1)
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1256}");
                return;
            }

            if (m_Equip.BindFlag == false && m_Equip.GetBindType() != 0)
            {
                MessageBoxLogic.OpenOKCancelBox(3028, 1000, EquipUseOK, null);
            }
            else
            {
                EquipUseOK();
            }
        }
    }

    void EquipUseOK()
    {
        if (Singleton<ObjManager>.Instance.MainPlayer.CheckEquipItem(m_Equip))
        {
            GameManager.gameManager.SoundManager.PlaySoundEffect(144);
            Singleton<ObjManager>.Instance.MainPlayer.EquipItem(m_Equip);
        }

        CloseWindow();
    }

    void EquipTakeOff()
    {
        if (m_Equip != null && m_Equip.IsValid())
        {
            if (Singleton<ObjManager>.Instance.MainPlayer.CheckUnEquipItem(m_Equip))
            {
                Singleton<ObjManager>.Instance.MainPlayer.UnEquipItem(m_Equip);
            }
            CloseWindow();
        }
    }

    void EquipShareLink()
    {
        if (m_Equip != null && m_Equip.IsValid())
        {
            ShareTargetChooseLogic.InitEquipShare(m_Equip);
        }
        //ShareLinkDirectChatInfo();
    }

    void PutInQianKunDai()
    {
        if (m_Equip != null && m_Equip.IsValid())
        {
            if (QianKunDaiLogic.Instance() != null && QianKunDaiLogic.Instance().gameObject.activeSelf)
            {
                QianKunDaiLogic.Instance().ChooseStuff(m_Equip, m_curItemSlot);
            }
            CloseWindow();
        }
    }

    void EquipEnchance()
    {
        if (m_Equip != null && m_Equip.IsValid())
        {
            bool bRet = GameManager.gameManager.PlayerDataPool.CommonData.GetCommondFlag((int)USER_COMMONFLAG.CF_STRENGTHENFUNCTION_OPENFLAG);
            if (bRet == false)
            {
                 Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2182}");
                return;
            }
            if (m_CompareEquipTooltips.activeSelf == true)
            {
                CloseCompare();
            }
            UIManager.CloseUI(UIInfo.EquipTooltipsRoot);
            UIManager.ShowUI(UIInfo.EquipStren, OnShowEquipEnchance);
        }
    }

    void OnShowEquipEnchance(bool bSuccess, object param)
    {
        if (m_Equip != null && m_Equip.IsValid())
        {
            if (EquipStrengthenLogic.Instance() != null)
            {
                //设置勾选当前装备
                EquipStrengthenLogic.Instance().SetCurSelectEquip(m_Equip);
                EquipStrengthenLogic.Instance().UpdateTab();
            }
        }
    }

    void OnBuyClick()
    {
        if (SysShopController.Instance() != null)
        {
            SysShopController.Instance().BuyCurItem();
        }
    }

    void OnBuyBatchClick()
    {
        if (SysShopController.Instance() != null)
        {
            SysShopController.Instance().BuyBatchCurItem();
        }
    }

    void ShareLinkDirectChatInfo()
    {
        UIManager.ShowUI(UIInfo.ChatInfoRoot, ShowChatInfoRootOver);
    }
    //寄售行求购
    void ConsignSaleBuyBt()
    {
        UIManager.ShowUI(UIInfo.ConsignSaleRoot, BuyItemOpenConsignSale);
    }

    void BuyItemOpenConsignSale(bool bSuccess, object param)
    {
        if (bSuccess)
        {
            if (ConsignSaleLogic.Instance() != null)
            {
                ConsignSaleLogic.Instance().SearchForAskBuy(m_Equip.GetName());
            }
            CloseWindow();
        }
    }
    void ShowChatInfoRootOver(bool bSuccess, object param)
    {
        if (bSuccess)
        {
            ChatInfoLogic.Instance().InsertEquipLinkText(m_Equip);
            EquipTooltipsLogic.Instance().CloseWindow();
        }
    }

    void CangKuIn()
    {
        if (m_Equip != null && m_Equip.IsValid())
        {
            Singleton<ObjManager>.Instance.MainPlayer.CangKuPutIn(m_Equip);
            CloseWindow();
        }
    }

    void CangKuOut()
    {
        if (m_Equip != null && m_Equip.IsValid())
        {
            Singleton<ObjManager>.Instance.MainPlayer.CangKuTakeOut(m_Equip);
            CloseWindow();
        }
    }
}