using UnityEngine;
using System.Collections;
using GCGame;
using GCGame.Table;
using Games.GlobeDefine;

public class MercenaryListItem : MonoBehaviour
{

    public UILabel LabelName;
    public UILabel LabelCost;
    public UILabel LabelProfession;
    public UILabel LabelCombat;
    public UILabel LabelRelationShip;
    public UIToggle UIToggleSel;
    private MercenaryWindow m_parent;
    private HuaShanPVPData.MercenaryInfo m_data;


    public static MercenaryListItem CreateItem(GameObject grid, GameObject resItem, string name, MercenaryWindow parent, HuaShanPVPData.MercenaryInfo data)
    {
        GameObject curItem = Utils.BindObjToParent(resItem, grid, name);
        if (null != curItem)
        {
            MercenaryListItem curItemComponent = curItem.GetComponent<MercenaryListItem>();
            if (null != curItemComponent)
                curItemComponent.SetData(parent, data);

            return curItemComponent;
        }

        return null;
    }

    public void SetData(MercenaryWindow parent, HuaShanPVPData.MercenaryInfo data)
    {
        m_parent = parent;
        LabelName.text = data.name;
        LabelCost.text = data.cost.ToString();
        LabelCombat.text = data.combat.ToString();
        LabelProfession.text = Utils.GetDicByID(CharacterDefine.PROFESSION_DICNUM[(int)data.profession]);
        UIToggleSel.value = false;

        switch (data.relationship)
        {
            case 0:
                LabelRelationShip.text = Utils.GetDicByID(1965);
                break;
            case 1:
                LabelRelationShip.text = Utils.GetDicByID(1966);
                break;
            case 2:
                LabelRelationShip.text = Utils.GetDicByID(1967);
                break;
            default:
                LabelRelationShip.text = "???";
                break;
        }
  
        
        m_data = data;
    }


    public HuaShanPVPData.MercenaryInfo GetData()
    {
        return m_data;
    }

    void OnItemClick()
    {
        if (null != m_parent)
        {
            UIToggleSel.value = !UIToggleSel.value;
             if (!m_parent.OnOpItemClick(this, UIToggleSel.value))
                 UIToggleSel.value = false;
 
        }
    }
}
