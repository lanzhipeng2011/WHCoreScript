using UnityEngine;
using System.Collections;
using GCGame;

public class HuashanPvPMemberListItem : MonoBehaviour
{

	public UILabel LabelPos;
    public UILabel LableName;
    public UILabel LableCombat;

    //private HuaShanPvPRegisterMemberWindow m_parent;
	

    public static HuashanPvPMemberListItem CreateItem(GameObject grid, GameObject resItem, string name, HuaShanPvPRegisterMemberWindow parent, string pos, string mname, string combat)
	{
        GameObject curItem = Utils.BindObjToParent(resItem, grid, name);
        if (null != curItem)
        {
            HuashanPvPMemberListItem curItemComponent = curItem.GetComponent<HuashanPvPMemberListItem>();
            if (null != curItemComponent)
                curItemComponent.SetData(parent, pos, mname, combat);

            return curItemComponent;
        }

        return null;
	}

    void SetData(HuaShanPvPRegisterMemberWindow parent, string pos, string name, string combat)
	{
		LabelPos.text = pos;
        LableName.text = name;
        LableCombat.text = combat;
        //m_parent = parent;
	}
}
