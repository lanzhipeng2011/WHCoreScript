using UnityEngine;
using System.Collections;
using GCGame;

public class PVPSkillListItem : MonoBehaviour {

    public UILabel m_LabelName;
    public UILabel m_LabelLev;
	public GameObject m_SprHightlight;
    private int m_nSkillId =-1;
    public int SkillId
    {
        get { return m_nSkillId; }
        set { m_nSkillId = value; }
    }

	private PVPPowerWindow m_parent;

	
	public static PVPSkillListItem CreateItem(GameObject grid, GameObject resItem, string name, PVPPowerWindow parent, string SkillName,string SkillLev,int nSkillId)
	{
        GameObject curItem = Utils.BindObjToParent(resItem, grid, name);
        if (null != curItem)
        {
            PVPSkillListItem curItemComponent = curItem.GetComponent<PVPSkillListItem>();
            if (null != curItemComponent)
                curItemComponent.SetData(parent, SkillName, SkillLev, nSkillId);
            return curItemComponent;
        }

        return null;
	}
	
	void SetData(PVPPowerWindow parent, string skillName,string SkillLev,int nSkillId)
	{
        m_LabelLev.text = SkillLev;
		m_LabelName.text = skillName;
		m_parent = parent;
	    m_nSkillId = nSkillId;
	}
	
	void OnItemClick()
	{
		if (null != m_parent)
		{
			m_parent.ShowCurSkill(this);
		}
	}
	
	public void EnableHighlight(bool bEnable)
	{
        m_SprHightlight.SetActive(bEnable);
	}
}
