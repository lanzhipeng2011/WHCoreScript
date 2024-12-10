using UnityEngine;
using System.Collections;
using Games.Item;

public class TooltipsEquipStar : MonoBehaviour {

    public UISprite[] m_EquipStar = new UISprite[12];

	// Use this for initialization
	void Start () {
	
	}


    public void ShowStar(int nStarLevel)
    {
        int starnum = 0;
        int starcolour = 0;
        if (nStarLevel > 0)
        {
            starnum = ((nStarLevel - 1) % 12) + 1;
            starcolour = (int)((nStarLevel - 1) / 12);
        }
        for (int i = 0; i < 12; i++)
        {
            if (i < starnum)
            {
                m_EquipStar[i].spriteName = ItemTool.GetStarColourSprite(starcolour);
                m_EquipStar[i].gameObject.SetActive(true);
            }
            else
            {
                if (starcolour >= 1)
                {
                    m_EquipStar[i].spriteName = ItemTool.GetStarColourSprite(starcolour - 1);
                    m_EquipStar[i].gameObject.SetActive(true);
                }
                else
                {
                    m_EquipStar[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
