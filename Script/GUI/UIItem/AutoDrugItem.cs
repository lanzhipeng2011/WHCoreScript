using UnityEngine;
using System.Collections;
using GCGame;
using Games.Item;
using System;
using Games.GlobeDefine;
using GCGame.Table;
public class AutoDrugItem : MonoBehaviour {

    public UILabel m_ItemName;
    public UISprite m_ItemIcon;
    public UISprite m_SelectIcon;
    public UISprite m_BackgroundSprite;
    public UISprite m_QualitySprite;
    public int m_nId;   //物品id
    public UInt64 m_ItemGuid;
    private AutoDrugWindow m_parent;
	// Use this for initialization
	void Start () {
        m_SelectIcon.gameObject.SetActive(false);
        if (Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            if (Singleton<ObjManager>.GetInstance().MainPlayer.AutoMpID == m_nId 
                || Singleton<ObjManager>.GetInstance().MainPlayer.AutoHpID == m_nId
                || (m_ItemGuid != GlobeVar.INVALID_GUID && Singleton<ObjManager>.GetInstance().MainPlayer.AutoEquipGuid == m_ItemGuid))
            {
                m_SelectIcon.gameObject.SetActive(true);
            }
        }
	}

    public static AutoDrugItem CreateItem(GameObject grid, GameObject resItem, AutoDrugWindow parent, Tab_CommonItem CommonItem)
    {

        GameObject curItemObject = Utils.BindObjToParent(resItem, grid, CommonItem.Id.ToString());
        if (null != curItemObject)
        {
            AutoDrugItem curItemComponent = curItemObject.GetComponent<AutoDrugItem>();
            if (null != curItemComponent)
            {
                curItemComponent.m_ItemName.text = CommonItem.Name;
                curItemComponent.m_ItemIcon.spriteName = CommonItem.Icon;
                curItemComponent.m_nId = CommonItem.Id;
                curItemComponent.m_parent = parent;
                curItemComponent.m_ItemGuid = GlobeVar.INVALID_GUID;
                //curItemComponent.m_BackgroundSprite.spriteName = "";
                curItemComponent.m_QualitySprite.spriteName = GlobeVar.QualityColorGrid[CommonItem.Quality - 1]; ;
                return curItemComponent;
            }
        }
        return null;
    }
    public static AutoDrugItem CreateEquip(GameObject grid, GameObject resItem, AutoDrugWindow parent, GameItem equip)
    {
        GameObject curItemObject = Utils.BindObjToParent(resItem, grid, equip.DataID.ToString());
        if (null != curItemObject)
        {
            AutoDrugItem curItemComponent = curItemObject.GetComponent<AutoDrugItem>();
            if (null != curItemComponent)
            {
                Tab_CommonItem curItem = TableManager.GetCommonItemByID(equip.DataID, 0);
                if (null != curItem)
                {
                    curItemComponent.m_ItemName.text = curItem.Name;
                    curItemComponent.m_ItemIcon.spriteName = curItem.Icon;
                    curItemComponent.m_nId = curItem.Id;
                    curItemComponent.m_parent = parent;
                    curItemComponent.m_ItemGuid = equip.Guid;
                    //curItemComponent.m_BackgroundSprite.spriteName = background;
                    curItemComponent.m_QualitySprite.spriteName = equip.GetQualityFrame();
                    return curItemComponent;
                }
            }
        }
        return null;
    }
    void OnItemClick()
    {
        if (null != m_parent) m_parent.OnOpItemClick(this);
        //m_SelectIcon.gameObject.SetActive(true);
    }
}
