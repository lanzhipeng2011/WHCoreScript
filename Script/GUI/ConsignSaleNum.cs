/********************************************************************
	创建时间:	2014/06/12 13:16
	全路径:		\TLOL\Version\Main\Project\Client\Assets\MLDJ\Script\GUI\ConsignSaleNum.cs
	创建人:		luoy
	功能说明:	寄售行上架背包 上架信息输入UI
	修改记录:
*********************************************************************/
using Games.GlobeDefine;
using Games.Item;
using GCGame.Table;
using UnityEngine;
using System.Collections;

public class ConsignSaleNum : MonoBehaviour
{
    public UILabel m_ProfessionLabel;
    public UILabel m_LevelLabel;
    public UILabel m_ItemNameLabel;
    public UILabel m_FightValueLabel;
    public UISprite m_IconSprite;
    public UISprite m_QualityFrameSprite;
    public UILabel m_NumLabel;
    private GameItem m_itemInfo;
    private int m_ItemCount;
    private int m_itemPrice;
    public UIInput m_CountInput;
    public UIInput m_PriceInput;
    private  int m_nMaxCount = 999;
    private const int m_nMaxPrice = 999999;
    //void Start()
    //{
    //}
    //void OnEnable()
    //{
        
    //}
    //void OnDisable()
    //{
    //}
    //更新显示 左上方物品条目信息
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
            m_ItemCount =1;
            m_itemPrice = 2;
            m_CountInput.value = "1";
            m_PriceInput.value = "2";
        }
    }
    //图标
    private void SetIcon(GameItem item)
    {
        Tab_CommonItem tabItem = TableManager.GetCommonItemByID(item.DataID, 0);
        if (null != tabItem)
        {
            m_IconSprite.spriteName = tabItem.Icon;
           // m_IconSprite.MakePixelPerfect();
        }
    }
    //堆叠
    private void SetStackCount(GameItem item)
    {
        int nStackCount = item.StackCount;
        m_NumLabel.text = nStackCount.ToString();
        m_NumLabel.gameObject.SetActive(true);
		m_nMaxCount = nStackCount;
    }
    //战斗力数字
    private void SetFightValue(GameItem item)
    {
        int CombatValue = item.GetCombatValue();
        m_FightValueLabel.text = "[ffff69]" + StrDictionary.GetClientDictionaryString("#{1241}", CombatValue);
    }
    //名字
    private void SetItemName(GameItem item)
    {
        Tab_CommonItem tabItem = TableManager.GetCommonItemByID(item.DataID, 0);
        if (null != tabItem)
        {
            string name = tabItem.Name;
            m_ItemNameLabel.text = string.Format("[ffff69]{0}", name);
        }
    }
    //等级
    private void SetLevel(GameItem item)
    {
        Tab_CommonItem tabItem = TableManager.GetCommonItemByID(item.DataID, 0);
        if (null != tabItem)
        {
            int level = tabItem.MinLevelRequire;
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
    }
    //职业
    private void SetProfession(GameItem item)
    {
        Tab_CommonItem tabItem = TableManager.GetCommonItemByID(item.DataID, 0);
        if (null != tabItem)
        {
            int Profession = tabItem.ProfessionRequire;
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
    }
    //品质框
    public void SetQualityFrame(GameItem item)
    {
        m_QualityFrameSprite.gameObject.SetActive(true);
        m_QualityFrameSprite.spriteName = item.GetQualityFrame();
        m_QualityFrameSprite.MakePixelPerfect();
    }
    // +1
    void OnClickAddCountNum()
    {
        int curNum = 0;
        bool bCanParse = int.TryParse(m_CountInput.value, out curNum);
        if (bCanParse)
        {
		
            curNum = Mathf.Min(m_nMaxCount+1, Mathf.Max(1,curNum+1));
			if(curNum>m_nMaxCount)
			{
				curNum=1;
			}
            m_CountInput.value = curNum.ToString();
        }
        else
        {
            m_CountInput.value = "1";
            curNum = 1;
        }
        m_ItemCount = curNum;
    }

    // -1
    void OnClickDelCountNum()
    {
        int curNum = 0;
        bool bCanParse = int.TryParse(m_CountInput.value, out curNum);
        if (bCanParse)
        {
            curNum = Mathf.Min(m_nMaxCount, Mathf.Max(0, curNum-1));
			if(curNum<1)
			{
				curNum=m_nMaxCount;
			}
            m_CountInput.value = curNum.ToString();
        }
        else
        {
            m_CountInput.value = "1";
            curNum = 1;
        }
        m_ItemCount = curNum;
    }
    //上架
    void OnClickOk()
    {
        if (ConsignSaleLogic.Instance()!=null)
        {
			OnPriceInputSubmit();
            ConsignSaleLogic.Instance().SaleItem(m_itemInfo,m_ItemCount,m_itemPrice);
            gameObject.SetActive(false);
            ConsignSaleLogic.Instance().m_SaleBag.GetComponent<ConsignSaleBag>().m_InfoLable.SetActive(true);
			//ConsignSaleLogic.Instance().m_SaleBag.GetComponent<ConsignSaleBag>().m_BackPackItems.SetActive(false);

			Invoke("Delay",1.0f);


        }
    }
	void Delay()
	{
		ConsignSaleLogic.Instance().m_SaleBag.GetComponent<ConsignSaleBag>().UpdateBackPack();
//		ConsignSaleLogic.Instance().m_SaleBag.GetComponent<ConsignSaleBag>().m_BackPackItemGrid.GetComponent<UIGrid>().Reposition();
//		ConsignSaleLogic.Instance().m_SaleBag.GetComponent<ConsignSaleBag>().m_BackPackItems.SetActive(true);
	}
    // 回车时响应
    public void OnPriceInputSubmit()
    {
        int curNum = 0;
        bool bCanParse = int.TryParse(m_PriceInput.value, out curNum);
        if (bCanParse)
        {
            curNum = Mathf.Min(m_nMaxPrice, Mathf.Max(1, curNum));
            m_PriceInput.value = curNum.ToString();
        }
        else
        {
            m_PriceInput.value = "2";
            curNum =2;
        }
        m_itemPrice = curNum;
    }
    // 回车时响应
    public void OnCountInputSubmit()
    {
        int curNum = 0;
        bool bCanParse = int.TryParse(m_CountInput.value, out curNum);
        if (bCanParse)
        {
            curNum = Mathf.Min(m_nMaxCount, Mathf.Max(1, curNum));
            m_CountInput.value = curNum.ToString();
        }
        else
        {
            m_CountInput.value = "1";
            curNum = 1;
        }
        m_ItemCount = curNum;
    }
}
