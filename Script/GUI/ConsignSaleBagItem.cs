/********************************************************************
	创建时间:	2014/06/12 13:14
	全路径:		\TLOL\Version\Main\Project\Client\Assets\MLDJ\Script\GUI\ConsignSaleBagItem.cs
	创建人:		luoy
	功能说明:	寄售行上架背包 物品条目信息
	修改记录:
*********************************************************************/
using System;
using Games.GlobeDefine;
using Games.Item;
using GCGame.Table;
using UnityEngine;
using System.Collections;

public class ConsignSaleBagItem : MonoBehaviour
{
    public UILabel m_ProfessionLabel;
    public UILabel m_LevelLabel;
    public UILabel m_ItemNameLabel;
    public UILabel m_FightValueLabel;
    public UISprite m_IconSprite;
    public UISprite m_QualityFrameSprite;
    public UILabel m_NumLabel;
    private GameItem m_itemInfo;
    public GameObject m_HightSprite;

    /// <summary>
    /// 更新上架背包条目信息
    /// </summary>
    /// <param name="item"></param>
    public void UpdateBackPackItem(GameItem item)
    {
        if (item != null && item.IsValid())
        {
            SetItemName(item);
            SetIcon(item);
            SetStackCount(item);
            SetFightValue(item);
            SetLevel(item);
            SetProfession(item);
            SetQualityFrame(item);
            m_itemInfo = item;
            m_HightSprite.SetActive(false);
        }
    }
    private void SetIcon(GameItem item)
    {
        m_IconSprite.spriteName = TableManager.GetCommonItemByID(item.DataID, 0).Icon;
        m_IconSprite.MakePixelPerfect();
    }

    private void SetStackCount(GameItem item)
    {
        int nStackCount = item.StackCount;
        m_NumLabel.text = nStackCount.ToString();
        m_NumLabel.gameObject.SetActive(true);
    }
    private void SetFightValue(GameItem item)
    {
        int CombatValue = item.GetCombatValue();
        m_FightValueLabel.text = "[ffff69]" + StrDictionary.GetClientDictionaryString("#{1241}", CombatValue);
    }

    private void SetItemName(GameItem item)
    {
        string name = TableManager.GetCommonItemByID(item.DataID, 0).Name;
        m_ItemNameLabel.text = string.Format("[ffff69]{0}", name);
    }

    private void SetLevel(GameItem item)
    {
        int level = TableManager.GetCommonItemByID(item.DataID, 0).MinLevelRequire;
        int nPlayerLevel = Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.Level;
        if (nPlayerLevel >= level)
        {
            m_LevelLabel.text = "[ffff69]";
        }
        else
        {
            m_LevelLabel.text = "[E60012]"; // 红色
        }
        m_LevelLabel.text += StrDictionary.GetClientDictionaryString("#{1242}", level);
    }

    private void SetProfession(GameItem item)
    {
        int Profession = TableManager.GetCommonItemByID(item.DataID, 0).ProfessionRequire;
        int nPlayerProfession = Singleton<ObjManager>.Instance.MainPlayer.Profession;
        if (Profession == -1 || Profession == nPlayerProfession)
        {
            m_ProfessionLabel.text = "[ffff69]";
        }
        else
        {
            m_ProfessionLabel.text = "[E60012]"; // 红色
        }

        string professionStr = StrDictionary.GetClientDictionaryString("#{1243}");
        if (0 <= Profession && Profession < (int)CharacterDefine.PROFESSION.MAX)
        {
            professionStr = StrDictionary.GetClientDictionaryString("#{" + CharacterDefine.PROFESSION_DICNUM[Profession].ToString() + "}");
        }
        m_ProfessionLabel.text += professionStr;
    }
    public void SetQualityFrame(GameItem item)
    {
        m_QualityFrameSprite.gameObject.SetActive(true);
        m_QualityFrameSprite.spriteName = item.GetQualityFrame();
        m_QualityFrameSprite.MakePixelPerfect();
    }
    /// <summary>
    /// 单击
    /// </summary>
    public void OnItemClick()
    {
        if (ConsignSaleLogic.Instance() !=null &&
            ConsignSaleLogic.Instance().m_SaleBag !=null)
        {
           ConsignSaleBag saleBag= ConsignSaleLogic.Instance().m_SaleBag.GetComponent<ConsignSaleBag>();
           if (saleBag != null && saleBag.m_BagSaleUI!=null)
           {
                saleBag.m_BagSaleUI.SetActive(true);
                saleBag.m_InfoLable.SetActive(false);
                saleBag.CleanAllBackItemHighLight();
                if (null != saleBag.m_BagSaleUI.GetComponent<ConsignSaleNum>())
                    saleBag.m_BagSaleUI.GetComponent<ConsignSaleNum>().UpdateBackPackItem(m_itemInfo);
                m_HightSprite.SetActive(true);
           }
        }         
    }
}
