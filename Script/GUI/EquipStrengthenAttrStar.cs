using UnityEngine;
using System.Collections;
using GCGame.Table;
using Games.Item;

public class EquipStrengthenAttrStar : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    public GameObject[] m_AttrWhole;
    public UILabel[] m_AttrName;
    public UILabel[] m_AttrValue;
    public UILabel[] m_AddAttr;
    public UIGrid m_Grid;
    public UITopGrid m_TopGrid;
    //public UILabel[] m_StarAttr;

    public void ClearInfo()
    {
        for (int i = 0; i < m_AttrName.Length; ++i)
        {
            m_AttrName[i].text = "";
            m_AddAttr[i].text = "";
            m_AttrValue[i].text = "";
            m_AttrWhole[i].SetActive(false);
        }
    }

    //显示装备属性
    public void ShowAttr(GameItem item)
    {
        for (int i = 0; i < m_AttrName.Length; ++i)
        {
            m_AttrName[i].text = "";
            m_AddAttr[i].text = "";
            m_AttrValue[i].text = "";
            m_AttrWhole[i].SetActive(false);
        }
        string strStar = "";
        Tab_EquipStar starline = TableManager.GetEquipStarByID(item.StarLevel + 1, 0);
        if (starline != null)
        {
            strStar = "+" + (starline.AttrRate * 100 + 100).ToString() + "%";
        }
        string strColor = "[00A0FF]";
        Tab_EquipAttr line = TableManager.GetEquipAttrByID(item.DataID, 0);
        if (line != null)
        {
            int index = 0;
            //血上限
            if (line.HP > 0 && index < m_AttrName.Length && index >= 0)
            {
                //打星加成属性
                m_AttrName[index].text = strColor + ItemTool.ConvertAttrToString(COMBATATTE.MAXHP);
                m_AddAttr[index].text = strStar;
                m_AttrWhole[index].SetActive(true);
                ++index;
            }
            //蓝上限
            if (line.MP > 0 && index < m_AttrName.Length && index >= 0)
            {
                m_AttrName[index].text = strColor + ItemTool.ConvertAttrToString(COMBATATTE.MAXMP);
                m_AddAttr[index].text = strStar;
                m_AttrWhole[index].SetActive(true);
                ++index;
            }
            //物理攻击
            if (line.PhysicsAttack > 0 && index < m_AttrName.Length && index >= 0)
            {
                m_AttrName[index].text = strColor + ItemTool.ConvertAttrToString(COMBATATTE.PYSATTACK);
                m_AddAttr[index].text = strStar;
                m_AttrWhole[index].SetActive(true);
                ++index;
            }
            //法术攻击
            if (line.MagicAttack > 0 && index < m_AttrName.Length && index >= 0)
            {
                m_AttrName[index].text = strColor + ItemTool.ConvertAttrToString(COMBATATTE.MAGATTACK);
                m_AddAttr[index].text = strStar;
                m_AttrWhole[index].SetActive(true);
                ++index;
            }
            //物理防御
            if (line.PhysicsDefence > 0 && index < m_AttrName.Length && index >= 0)
            {
                m_AttrName[index].text = strColor + ItemTool.ConvertAttrToString(COMBATATTE.PYSDEF);
                m_AddAttr[index].text = strStar;
                m_AttrWhole[index].SetActive(true);
                ++index;
            }
            //法术防御
            if (line.MagicDefence > 0 && index < m_AttrName.Length && index >= 0)
            {
                m_AttrName[index].text = strColor + ItemTool.ConvertAttrToString(COMBATATTE.MAGDEF);
                m_AddAttr[index].text = strStar;
                m_AttrWhole[index].SetActive(true);
                ++index;
            }
            //全攻击
            if (line.AllAttack > 0 && index < m_AttrName.Length && index >= 0)
            {
                m_AttrName[index].text = strColor + ItemTool.ConvertAttrToString((COMBATATTE)1000);
                m_AddAttr[index].text = strStar;
                m_AttrWhole[index].SetActive(true);
                ++index;
            }
            //全防御
            if (line.AllDefence > 0 && index < m_AttrName.Length && index >= 0)
            {
                m_AttrName[index].text = strColor + ItemTool.ConvertAttrToString((COMBATATTE)1001);
                m_AddAttr[index].text = strStar;
                m_AttrWhole[index].SetActive(true);
                ++index;
            }
            ////打星不加附加属性
            //for (int attrIndex = 0; attrIndex < line.getAddAttrTypeCount(); ++attrIndex)
            //{
            //    int attrType = line.GetAddAttrTypebyIndex(attrIndex);
            //    float attrValue = line.GetAddAttrValuebyIndex(attrIndex);
            //    if ((attrType >= (int)COMBATATTE.MAXHP && attrType < (int)COMBATATTE.COMBATATTE_MAXNUM) || attrType == 1000 || attrType == 1001)
            //    {
            //        if (attrValue > 0)
            //        {
            //            if (index < m_AttrName.Length && index >= 0)
            //            {
            //                //策划要求隐藏装备攻速属性
            //                if (attrType == (int)COMBATATTE.ATTACKSPEED)
            //                {
            //                    continue;
            //                }
            //                m_AttrName[index].text = strColor + ItemTool.ConvertAttrToString((COMBATATTE)attrType);
            //                m_AddAttr[index].text = strStar;
            //                m_AttrWhole[index].SetActive(true);
            //                ++index;
            //            }
            //        }
            //    }
            //}
            m_Grid.Reposition();
            m_TopGrid.Recenter(true);
        }
    }
}
