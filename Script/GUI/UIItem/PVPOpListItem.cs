using UnityEngine;
using System.Collections;
using GCGame;
using GCGame.Table;
using Games.GlobeDefine;

public class PVPOpListItem : MonoBehaviour {

    public UILabel LabelName;
    public UILabel LabelLevel;
    public UILabel LabelPower;
    public UILabel LabelProfession;
    public UILabel LabelRange;
    public GameObject SprHightlight;
    public UISprite[] ProfessionSprites;

	private PVPFindOpWindow m_parent;
	private PVPData.OpponentInfo m_data;


	public static PVPOpListItem CreateItem(GameObject grid, GameObject resObj, string name, PVPFindOpWindow parent, PVPData.OpponentInfo data)
	{
		GameObject curItem = Utils.BindObjToParent(resObj, grid);
        if (null != curItem)
        {
            PVPOpListItem curItemComponent = curItem.GetComponent<PVPOpListItem>();
            if (null != curItemComponent)
                curItemComponent.SetData(parent, data);
            return curItemComponent;
        }

        return null;
	}

	public void SetData(PVPFindOpWindow parent, PVPData.OpponentInfo data)
	{
		m_parent = parent;
		LabelName.text = data.name;
		LabelLevel.text = data.level.ToString();
		LabelPower.text = data.fightPower.ToString();
        LabelProfession.text = Utils.GetDicByID(CharacterDefine.PROFESSION_DICNUM[(int)data.profession]);//data.profession.ToString();
		LabelRange.text = data.range.ToString();
        m_data = data;

        int profession = (int)data.profession;
        for (int i = 0; i < 4; i++)
        {
            ProfessionSprites[i].gameObject.SetActive(profession == i);
        }
    }

	public PVPData.OpponentInfo GetData()
	{
		return m_data;
	}

	void OnItemClick()
	{
		if(null != m_parent) m_parent.OnOpItemClick(this);
	}

    public void EnableHighlight(bool bEnable)
    {
        SprHightlight.SetActive(bEnable);
    }
    
}
