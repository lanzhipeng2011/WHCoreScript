using UnityEngine;
using System.Collections;
using GCGame.Table;
using System.Collections.Generic;
using Games.Item;
using Module.Log;
using Games.GlobeDefine;
public class AutoDrugWindow : MonoBehaviour {

    public GameObject ItemParent;
    public int m_nType = 1;
    private AutoFightLogic m_parent;
    private int m_curType;


	// Use this for initialization
	void Start () {
        //UpdateData(m_nType);
	}
	
    public void UpdateData(int nType,AutoFightLogic fightlogic)
    {
        m_parent = fightlogic;
        m_curType = nType;
        UIManager.LoadItem(UIInfo.AutoDrugItem, OnLoadDrugItem);
    }

    void OnLoadDrugItem(GameObject resItem, object param)
    {
        if (null == resItem)
        {
            LogModule.ErrorLog("OnLoadDrugItem resItem is null");
            return;
        }

        GameItemContainer BackPack = GameManager.gameManager.PlayerDataPool.BackPack;
        if (Singleton<ObjManager>.Instance.MainPlayer)
        {
            if (m_curType == 1)
            {
                for (int i = (int)GameDefine_Globe.AUTOCOMBAT_DRUG_ID.AUTO_DRUG_START_HP; i <= (int)GameDefine_Globe.AUTOCOMBAT_DRUG_ID.AUTO_DRUG_END_HP; i++)
                {
                    if (BackPack.GetItemCountByDataId(i) <= 0
                    && Singleton<ObjManager>.Instance.MainPlayer.AutoHpID != i
                    && Singleton<ObjManager>.Instance.MainPlayer.AutoMpID != i)
                    {
                        continue;
                    }
                    Tab_CommonItem curItem = TableManager.GetCommonItemByID(i, 0);
                    if (null != curItem)
                    {
                        AutoDrugItem.CreateItem(ItemParent, resItem, this, curItem);
                    }
                   
                }
                for (int i = (int)GameDefine_Globe.AUTOCOMBAT_DRUG_ID.AUTO_DRUG_START_DYHP; i <= (int)GameDefine_Globe.AUTOCOMBAT_DRUG_ID.AUTO_DRUG_END_DYHP; i++)
                {
                    if (BackPack.GetItemCountByDataId(i) <= 0
                    && Singleton<ObjManager>.Instance.MainPlayer.AutoHpID != i
                    && Singleton<ObjManager>.Instance.MainPlayer.AutoMpID != i)
                    {
                        continue;
                    }
                    Tab_CommonItem curItem = TableManager.GetCommonItemByID(i, 0);
                    if (null != curItem)
                    {
                        AutoDrugItem.CreateItem(ItemParent, resItem, this, curItem);
                    }

                }
            }
            else if (m_curType == 2)
            {
                for (int i = (int)GameDefine_Globe.AUTOCOMBAT_DRUG_ID.AUTO_DRUG_START_MP; i <= (int)GameDefine_Globe.AUTOCOMBAT_DRUG_ID.AUTO_DRUG_END_MP; i++)
                {
                    if (BackPack.GetItemCountByDataId(i) <= 0
                    && Singleton<ObjManager>.Instance.MainPlayer.AutoHpID != i
                    && Singleton<ObjManager>.Instance.MainPlayer.AutoMpID != i)
                    {
                        continue;
                    }
                    Tab_CommonItem curItem = TableManager.GetCommonItemByID(i, 0);
                    if (null != curItem)
                    {
                        AutoDrugItem.CreateItem(ItemParent, resItem, this, curItem);
                    }

                }
                for (int i = (int)GameDefine_Globe.AUTOCOMBAT_DRUG_ID.AUTO_DRUG_START_DYMP; i <= (int)GameDefine_Globe.AUTOCOMBAT_DRUG_ID.AUTO_DRUG_END_DYMP; i++)
                {
                    if (BackPack.GetItemCountByDataId(i) <= 0
                    && Singleton<ObjManager>.Instance.MainPlayer.AutoHpID != i
                    && Singleton<ObjManager>.Instance.MainPlayer.AutoMpID != i)
                    {
                        continue;
                    }
                    Tab_CommonItem curItem = TableManager.GetCommonItemByID(i, 0);
                    if (null != curItem)
                    {
                        AutoDrugItem.CreateItem(ItemParent, resItem, this, curItem);
                    }

                }
            }
            else if (m_curType == 3 )
            {
                GameItemContainer EquipPack = GameManager.gameManager.PlayerDataPool.EquipPack;
                if(EquipPack != null)
                {
                    for (int index = 0; index < EquipPack.ContainerSize; index++)
                    {
                        GameItem equip = EquipPack.GetItem(index);
                        if (equip != null && equip.IsValid() && equip.IsBelt() == false)
                        {
                            AutoDrugItem.CreateEquip(ItemParent, resItem, this, equip);
                        }
                    }
                }
            }    
        }

//         foreach (List<Tab_CommonItem> curList in TableManager.GetCommonItem().Values)
//         {
//             Tab_CommonItem curItem = curList[0];
//             if (null != curItem)
//             {
//                 if (curItem.ClassID == (int)ItemClass.MEDIC)
//                 {
//                     if ((m_curType == 1 && curItem.SubClassID == (int)MedicSubClass.HP)
//                         || (m_curType == 2 && curItem.SubClassID == (int)MedicSubClass.MP))
//                     {
// 
//                         bool IsShow = false;
//                         int ShopId = 3;
//                         Tab_SystemShop curShop = TableManager.GetSystemShopByID(ShopId.ToString(), 0);
//                         if (curShop != null)
//                         {
//                             for (int i = 0; i < curShop.Pnum; i++)
//                             {
//                                 if (curItem.Id == curShop.GetPidbyIndex(i))
//                                 {
//                                     IsShow = true;
//                                     break;
//                                 }
// 
//                             }
//                             if (IsShow)
//                             {
//                                 AutoDrugItem.CreateItem(ItemParent,resItem, curItem.Id, curItem.Icon, curItem.Name, this);
//                             }
//                         }
// 
//                     }
//                 }
// 
//             }
//         }
        ItemParent.GetComponent<UIGrid>().Reposition();
        ItemParent.GetComponent<UITopGrid>().Recenter(true);
    }
    public void OnOpItemClick(AutoDrugItem item)
    {
        if (Singleton<ObjManager>.Instance.MainPlayer)
        {
            if (m_nType == 1)
            {
                Singleton<ObjManager>.Instance.MainPlayer.AutoHpID = item.m_nId;
                Singleton<ObjManager>.Instance.MainPlayer.AutoIsSelectDrug = true;
            }
            else if (m_nType == 2)
            {
                Singleton<ObjManager>.Instance.MainPlayer.AutoMpID = item.m_nId;
                Singleton<ObjManager>.Instance.MainPlayer.AutoIsSelectDrug = true;
            }
            else if (m_nType == 3)
            {
                Singleton<ObjManager>.Instance.MainPlayer.AutoEquipGuid = item.m_ItemGuid;
            }
            
        }

        if (null != m_parent)
            m_parent.UpdateDrug();

        UIManager.CloseUI(UIInfo.AutoDrug);
       
    }
    //public void OpenUI(int nType)
    //{
    //    m_nType = nType;
        //UIManager.ShowUI(UIInfo.AutoDrug);
 
    //}
}
