using UnityEngine;
using System.Collections;
using GCGame.Table;
using Games.GlobeDefine;
using Games.Item;

public class ItemSlotLogic : MonoBehaviour {

    public enum SLOT_TYPE
    {
        TYPE_INVALID = -1,
        TYPE_ITEM = 0,
        TYPE_FASHION,
        TYPE_FELLOW,
        TYPE_MOUNT,
        TYPE_FORMULA,
        TYPE_STUFF,
        TYPE_RESTAURANT,
        TYPE_COIN,
        TYPE_YUANBAO,
        TYPE_YUANBAOBIND,
    }

    public UISprite m_ItemIcon;
    public UISprite m_QualitySprite;
    public UILabel m_NumLabel;
    public UISprite m_DisableSprite;
    public UISprite m_ChooseSprite;

    private int m_ItemID;
    private SLOT_TYPE m_eItemType = SLOT_TYPE.TYPE_INVALID;

    public delegate void OnClick(int nItemID, SLOT_TYPE eItemType, string strSlotName);
    public OnClick m_delSlotOnClick;

    public static void OnClickOpenTips(int nItemID, ItemSlotLogic.SLOT_TYPE eItemType, string strSlotName)
    {
        if (eItemType == SLOT_TYPE.TYPE_ITEM)
        {
            GameItem item = new GameItem();
            item.DataID = nItemID;
            if (item != null && item.IsValid())
            {
                if (item.IsEquipMent())
                {
                    EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.Info);
                }
                else
                {
                    ItemTooltipsLogic.ShowItemTooltip(item, ItemTooltipsLogic.ShowType.Info);
                }
            }
        }
    }

	// Use this for initialization
	void Start () {

	}
	
    public void ClearInfo()
    {
        m_ItemID = GlobeVar.INVALID_ID;
        m_eItemType = SLOT_TYPE.TYPE_INVALID;
        m_delSlotOnClick = null;

        m_ItemIcon.gameObject.SetActive(false);
        m_QualitySprite.gameObject.SetActive(false);
        m_NumLabel.gameObject.SetActive(false);
        m_DisableSprite.gameObject.SetActive(false);
        m_ChooseSprite.gameObject.SetActive(false);
    }

    public void InitInfo(SLOT_TYPE type, int id, OnClick delSlotOnClick = null, string strNum = "", bool showNum = false)
    {
        if (id == GlobeVar.INVALID_ID || type == SLOT_TYPE.TYPE_INVALID)
        {
            ClearInfo();
        }

        if (type == SLOT_TYPE.TYPE_ITEM)
        {
            InitInfo_Item(id, delSlotOnClick, strNum, showNum);
        }
        else if (type == SLOT_TYPE.TYPE_FASHION)
        {
            InitInfo_Fashion(id, delSlotOnClick, strNum, showNum);
        }
        else if (type == SLOT_TYPE.TYPE_FELLOW)
        {
            InitInfo_Fellow(id, delSlotOnClick, strNum, showNum);
        }
        else if (type == SLOT_TYPE.TYPE_MOUNT)
        {
            InitInfo_Mount(id, delSlotOnClick, strNum, showNum);
        }
        else if (type == SLOT_TYPE.TYPE_FORMULA)
        {
            InitInfo_Formula(id, delSlotOnClick, strNum, showNum);
        }
        else if (type == SLOT_TYPE.TYPE_STUFF)
        {
            InitInfo_Stuff(id, delSlotOnClick, strNum, showNum);
        }
        else if (type == SLOT_TYPE.TYPE_RESTAURANT)
        {
            InitInfo_Restaurant(id, delSlotOnClick, strNum, showNum);
        }
        else if (type == SLOT_TYPE.TYPE_COIN)
        {
            InitInfo_Coin(delSlotOnClick, strNum, showNum);
        }
        else if (type == SLOT_TYPE.TYPE_YUANBAO)
        {
            InitInfo_Yuanbao(delSlotOnClick, strNum, showNum);
        }
        else if (type == SLOT_TYPE.TYPE_YUANBAOBIND)
        {
            InitInfo_YuanbaoBind(delSlotOnClick, strNum, showNum);
        }
    }

    public void InitInfo_Item(int id, OnClick delSlotOnClick = null, string strNum = "", bool showNum = false)
    {
        m_ItemID = id;
        m_eItemType = SLOT_TYPE.TYPE_ITEM;
        m_delSlotOnClick = delSlotOnClick;       
		this.gameObject.SetActive (false);
        Tab_CommonItem tabCommonItem = TableManager.GetCommonItemByID(id, 0);
        if (tabCommonItem == null)
        {
            ClearInfo();
        }
        else
        {
            m_ItemIcon.spriteName = tabCommonItem.Icon;
            m_ItemIcon.gameObject.SetActive(true);

            m_QualitySprite.spriteName = GlobeVar.QualityColorGrid[tabCommonItem.Quality - 1];
            m_QualitySprite.gameObject.SetActive(true);

            if (showNum || strNum != "")
            {
                m_NumLabel.text = strNum;
                m_NumLabel.gameObject.SetActive(true);
            }
            else
            {
                m_NumLabel.gameObject.SetActive(false);
            }

            GameItem item = new GameItem();
            item.DataID = id;
            if (item.IsFitForPlayer())
            {
                m_DisableSprite.gameObject.SetActive(false);
            }
            else
            {
                m_DisableSprite.gameObject.SetActive(true);
            }
        } 
		this.gameObject.SetActive (true);
    }

    public void InitInfo_Fashion(int id, OnClick delSlotOnClick = null, string strNum = "", bool showNum = false)
    {
        m_ItemID = id;
        m_eItemType = SLOT_TYPE.TYPE_FASHION;
        m_delSlotOnClick = delSlotOnClick;    

        Tab_FashionData tabFashionData = TableManager.GetFashionDataByID(id, 0);
        if (tabFashionData == null)
        {
            ClearInfo();
        }
        else
        {
            m_ItemIcon.spriteName = tabFashionData.Icon;
            m_ItemIcon.gameObject.SetActive(true);
            m_QualitySprite.gameObject.SetActive(false);
            m_NumLabel.gameObject.SetActive(false);
        } 
    }

    public void InitInfo_Fellow(int id, OnClick delSlotOnClick = null, string strNum = "", bool showNum = false)
    {
        m_ItemID = id;
        m_eItemType = SLOT_TYPE.TYPE_FASHION;
        m_delSlotOnClick = delSlotOnClick;

        m_NumLabel.gameObject.SetActive(false);
    }

    public void InitInfo_Mount(int id, OnClick delSlotOnClick = null, string strNum = "", bool showNum = false)
    {
        m_ItemID = id;
        m_eItemType = SLOT_TYPE.TYPE_MOUNT;
        m_delSlotOnClick = delSlotOnClick;

        m_NumLabel.gameObject.SetActive(false);
    }

    public void InitInfo_Formula(int id, OnClick delSlotOnClick = null, string strNum = "", bool showNum = false)
    {
        m_ItemID = id;
        m_eItemType = SLOT_TYPE.TYPE_FORMULA;
        m_delSlotOnClick = delSlotOnClick;

        m_NumLabel.gameObject.SetActive(false);

        Tab_LivingSkill tabFormula = TableManager.GetLivingSkillByID(id, 0);
        if (tabFormula == null)
        {
            ClearInfo();
        }
        else
        {
            m_ItemIcon.spriteName = tabFormula.Icon;
            m_ItemIcon.gameObject.SetActive(true);
        }
    }

    public void InitInfo_Stuff(int id, OnClick delSlotOnClick = null, string strNum = "", bool showNum = false)
    {
        m_ItemID = id;
        m_eItemType = SLOT_TYPE.TYPE_STUFF;
        m_delSlotOnClick = delSlotOnClick;

        Tab_CommonItem tabCommonItem = TableManager.GetCommonItemByID(id, 0);
        if (tabCommonItem == null)
        {
            ClearInfo();
        }
        else
        {
            m_ItemIcon.spriteName = tabCommonItem.Icon;
            m_ItemIcon.gameObject.SetActive(true);

            m_QualitySprite.spriteName = GlobeVar.QualityColorGrid[tabCommonItem.Quality - 1];
            m_QualitySprite.gameObject.SetActive(true);

            if (showNum || strNum != "")
            {
                m_NumLabel.text = strNum;
                m_NumLabel.gameObject.SetActive(true);
            }
            else
            {
                m_NumLabel.gameObject.SetActive(false);
            }
            
            m_DisableSprite.gameObject.SetActive(false);
        }
    }

    public void InitInfo_Restaurant(int id, OnClick delSlotOnClick = null, string strNum = "", bool showNum = false)
    {
        m_ItemID = id;
        m_eItemType = SLOT_TYPE.TYPE_RESTAURANT;
        m_delSlotOnClick = delSlotOnClick;

        Tab_CommonItem tabCommonItem = TableManager.GetCommonItemByID(id, 0);
        if (tabCommonItem == null)
        {
            ClearInfo();
        }
        else
        {
            m_ItemIcon.spriteName = tabCommonItem.Icon;
            m_ItemIcon.gameObject.SetActive(true);

            m_QualitySprite.spriteName = GlobeVar.QualityColorGrid[tabCommonItem.Quality - 1];
            m_QualitySprite.gameObject.SetActive(true);

            if (showNum || strNum != "")
            {
                m_NumLabel.text = strNum;
                m_NumLabel.gameObject.SetActive(true);
            }
            else
            {
                m_NumLabel.gameObject.SetActive(false);
            }

            m_DisableSprite.gameObject.SetActive(false);
        }
    }

    public void InitInfo_Coin(OnClick delSlotOnClick = null, string strNum = "", bool showNum = false)
    {
        m_ItemID = GlobeVar.INVALID_ID;
        m_eItemType = SLOT_TYPE.TYPE_COIN;
        m_delSlotOnClick = delSlotOnClick;

        m_ItemIcon.spriteName = "jinbi";
        m_ItemIcon.gameObject.SetActive(true);

        m_QualitySprite.spriteName = GlobeVar.QualityColorGrid[4];
        m_QualitySprite.gameObject.SetActive(true);

        if (showNum || strNum != "")
        {
            m_NumLabel.text = strNum;
            m_NumLabel.gameObject.SetActive(true);
        }
        else
        {
            m_NumLabel.gameObject.SetActive(false);
        }

        m_DisableSprite.gameObject.SetActive(false);
    }

    public void InitInfo_Yuanbao(OnClick delSlotOnClick = null, string strNum = "", bool showNum = false)
    {
        m_ItemID = GlobeVar.INVALID_ID;
        m_eItemType = SLOT_TYPE.TYPE_YUANBAO;
        m_delSlotOnClick = delSlotOnClick;

        m_ItemIcon.spriteName = "yuanbao2";
        m_ItemIcon.gameObject.SetActive(true);

        m_QualitySprite.spriteName = GlobeVar.QualityColorGrid[4];
        m_QualitySprite.gameObject.SetActive(true);

        if (showNum || strNum != "")
        {
            m_NumLabel.text = strNum;
            m_NumLabel.gameObject.SetActive(true);
        }
        else
        {
            m_NumLabel.gameObject.SetActive(false);
        }

        m_DisableSprite.gameObject.SetActive(false);
    }

    public void InitInfo_YuanbaoBind(OnClick delSlotOnClick = null, string strNum = "", bool showNum = false)
    {
        m_ItemID = GlobeVar.INVALID_ID;
        m_eItemType = SLOT_TYPE.TYPE_YUANBAOBIND;
        m_delSlotOnClick = delSlotOnClick;

        m_ItemIcon.spriteName = "bdyuanbao";
        m_ItemIcon.gameObject.SetActive(true);

        m_QualitySprite.spriteName = GlobeVar.QualityColorGrid[4];
        m_QualitySprite.gameObject.SetActive(true);

        if (showNum || strNum != "")
        {
            m_NumLabel.text = strNum;
            m_NumLabel.gameObject.SetActive(true);
        }
        else
        {
            m_NumLabel.gameObject.SetActive(false);
        }

        m_DisableSprite.gameObject.SetActive(false);
    }

    void SlotOnClick(GameObject value)
    {
        if (m_delSlotOnClick != null)
        {
            m_delSlotOnClick(m_ItemID, m_eItemType, value.name);
        }
    }

    public void ItemSlotChoose()
    {
        m_ChooseSprite.gameObject.SetActive(true);
    }

    public void ItemSlotChooseCancel()
    {
        m_ChooseSprite.gameObject.SetActive(false);
    }

    public void SetItemSlotChoose(bool bChoose)
    {
        m_ChooseSprite.gameObject.SetActive(bChoose);
    }

    public void ItemSlotDisable()
    {
        m_DisableSprite.gameObject.SetActive(true);
    }

    public void ItemSlotEnable()
    {
        m_DisableSprite.gameObject.SetActive(false);
    }

    public void SetItemSlotDisable(bool bDisable)
    {
        m_DisableSprite.gameObject.SetActive(bDisable);
    }

    public bool IsInit()
    {
        return (m_ItemID != GlobeVar.INVALID_ID);
    }

    public void SetIconDirect(string icon)
    {
        m_ItemIcon.spriteName = icon;
        m_ItemIcon.gameObject.SetActive(true);
    }

    public void SetOnClickDirect(OnClick delSlotOnClick)
    {
        m_delSlotOnClick = delSlotOnClick;
    }
}
