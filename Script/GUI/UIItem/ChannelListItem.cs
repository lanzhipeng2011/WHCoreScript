using UnityEngine;
using System.Collections;
using GCGame;

public class ChannelListItem : MonoBehaviour {

	public UILabel LabelText;

	private ChangeChannelController m_parent;
	// Use this for initialization
	void Start () {
	
	}

	public static ChannelListItem CreateItem(GameObject grid, GameObject resItem,string name, ChangeChannelController parent, string text)
	{
        GameObject curItem = Utils.BindObjToParent(resItem, grid, name);
        if (null != curItem)
        {
            ChannelListItem curItemComponent = curItem.GetComponent<ChannelListItem>();
            if (null != curItemComponent)
                curItemComponent.SetData(parent, text);

            return curItemComponent;
        }

        return null;
	}
	
	void SetData(ChangeChannelController parent,  string text)
	{
		m_parent = parent;
		LabelText.text = text;
	}
	
	void OnItemClick()
	{
		if (null != m_parent)
		{
			m_parent.OnChangeChannel(this);
		}
	}

}
