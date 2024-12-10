using UnityEngine;
using System.Collections;
using GCGame;
using GCGame.Table;
using Games.GlobeDefine;

public class PvPRankListItem : MonoBehaviour {

    public UILabel m_Label1;
    public UILabel m_Label2;
    public UILabel m_Label3;
    public UILabel m_Label4;
    public UILabel m_Label5;
    public UILabel m_Label6;	

    public static PvPRankListItem CreateItem(GameObject grid, GameObject resItem, string name, PVPData.PvPRankListItemInfo data)
    {
        GameObject curItem = Utils.BindObjToParent(resItem, grid, name);
        if (null != curItem)
        {
            PvPRankListItem curItemComponent = curItem.GetComponent<PvPRankListItem>();
            if (null != curItemComponent)
                curItemComponent.SendData(data);
            return curItemComponent;
        }

        return null;
    }

    public void SendData(PVPData.PvPRankListItemInfo data)
    {
        m_Label1.text = data.pos.ToString();
        m_Label2.text = data.name;
        m_Label3.text = Utils.GetDicByID(CharacterDefine.PROFESSION_DICNUM[(int)data.profession]); ;
        m_Label4.text = data.level.ToString();
        m_Label5.text = data.com.ToString();
        m_Label6.text = data.zhenqi.ToString();
    }

    public void Cleanup()
    {
        m_Label1.text = "";
        m_Label2.text = "";
        m_Label3.text = "";
        m_Label4.text = "";
        m_Label5.text = "";
        m_Label6.text = "";
    }
}
