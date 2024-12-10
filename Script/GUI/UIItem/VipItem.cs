using UnityEngine;
using System.Collections;
using GCGame.Table;
using Games.Item;

public class VipItem : MonoBehaviour {

    public UILabel m_BonusText;
    public UISprite m_BonusImage;
    public UISprite m_BonusQuality;

	// Use this for initialization
	

    public void UpdateItem(int num, int itemId)
    {
        if (itemId == -1) 
        {
            this.gameObject.SetActive(false);
            return;
        }
        if (num <= 1)
        {
            m_BonusText.gameObject.SetActive(false);
        }
        else
        {

            m_BonusText.gameObject.SetActive(true);
            m_BonusText.text = num.ToString();
        }

        Tab_CommonItem tBook = TableManager.GetCommonItemByID(itemId, 0);
        if (tBook == null) 
        {
            Debug.LogError("On VipItem CommonItem Not Find "+itemId.ToString());
            return;
        }
        this.gameObject.name = itemId.ToString();
        m_BonusImage.gameObject.SetActive(true);
        m_BonusImage.spriteName = tBook.Icon;

        m_BonusImage.gameObject.SetActive(true);
        m_BonusQuality.spriteName = GetQuality(tBook.Quality); ;
    }
    string GetQuality(int Quality)
    {
        switch ((ItemQuality)Quality)
        {
		case ItemQuality.QUALITY_WHITE:
			return "ui_pub_012";
		case ItemQuality.QUALITY_GREEN:
			return "ui_pub_013";
		case ItemQuality.QUALITY_BLUE:
			return "ui_pub_014";
		case ItemQuality.QUALITY_PURPLE:
			return "ui_pub_015";
		case ItemQuality.QUALITY_ORANGE:
			return "ui_pub_016";
		default:
			return "QualityGrey";
        }
    }
}
