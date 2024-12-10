using UnityEngine;
using System.Collections;
using Games.Item;
using GCGame.Table;

public class EquipStrengthenAttrEnchance : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    public GameObject[] m_AttrWhole;
    public UILabel[] m_AttrName;
    public UILabel[] m_AddAttr;
    public UIGrid m_Grid;
    public UITopGrid m_TopGrid;

    public void ClearInfo()
    {
        for (int i = 0; i < m_AttrName.Length; ++i)
        {
            m_AttrName[i].text = "";
            m_AddAttr[i].text = "";
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
            m_AttrWhole[i].SetActive(false);
        }
        string strColor = "[00A0FF]";
        //Tab_EquipEnchance curEnchanceLine = TableManager.GetEquipEnchanceByID(item.EnchanceLevel.ToString(), 0);
        Tab_EquipEnchance nextEnchanceLine = TableManager.GetEquipEnchanceByID(item.EnchanceLevel + 1, 0);
        if (nextEnchanceLine != null)
        {
            Tab_EquipAttr line = TableManager.GetEquipAttrByID(item.DataID, 0);
            if (line != null)
            {
                int index = 0;
                //血上限
                if (line.HP > 0 && index < m_AttrName.Length && index >= 0)
                {
                    //打星加成属性
                    m_AttrName[index].text = strColor + ItemTool.ConvertAttrToString(COMBATATTE.MAXHP);
                    m_AddAttr[index].text = string.Format("+{0}", GetNextEnchanceAttr(item, (int)line.HP).ToString());
                    m_AttrWhole[index].SetActive(true);
                    ++index;
                }
                //蓝上限
                if (line.MP > 0 && index < m_AttrName.Length && index >= 0)
                {
                    m_AttrName[index].text = strColor + ItemTool.ConvertAttrToString(COMBATATTE.MAXMP);
                    m_AddAttr[index].text = string.Format("+{0}", GetNextEnchanceAttr(item, (int)line.MP).ToString());
                    m_AttrWhole[index].SetActive(true);
                    ++index;
                }
                //物理攻击
                if (line.PhysicsAttack > 0 && index < m_AttrName.Length && index >= 0)
                {
                    m_AttrName[index].text = strColor + ItemTool.ConvertAttrToString(COMBATATTE.PYSATTACK);
                    m_AddAttr[index].text = string.Format("+{0}", GetNextEnchanceAttr(item, (int)line.PhysicsAttack).ToString());
                    m_AttrWhole[index].SetActive(true);
                    ++index;
                }
                //法术攻击
                if (line.MagicAttack > 0 && index < m_AttrName.Length && index >= 0)
                {
                    m_AttrName[index].text = strColor + ItemTool.ConvertAttrToString(COMBATATTE.MAGATTACK);
                    m_AddAttr[index].text = string.Format("+{0}", GetNextEnchanceAttr(item, (int)line.MagicAttack).ToString());
                    m_AttrWhole[index].SetActive(true);
                    ++index;
                }
                //物理防御
                if (line.PhysicsDefence > 0 && index < m_AttrName.Length && index >= 0)
                {
                    m_AttrName[index].text = strColor + ItemTool.ConvertAttrToString(COMBATATTE.PYSDEF);
                    m_AddAttr[index].text = string.Format("+{0}", GetNextEnchanceAttr(item, (int)line.PhysicsDefence).ToString());
                    m_AttrWhole[index].SetActive(true);
                    ++index;
                }
                //法术防御
                if (line.MagicDefence > 0 && index < m_AttrName.Length && index >= 0)
                {
                    m_AttrName[index].text = strColor + ItemTool.ConvertAttrToString(COMBATATTE.MAGDEF);
                    m_AddAttr[index].text = string.Format("+{0}", GetNextEnchanceAttr(item, (int)line.MagicDefence).ToString());
                    m_AttrWhole[index].SetActive(true);
                    ++index;
                }
                //全攻击
                if (line.AllAttack > 0 && index < m_AttrName.Length && index >= 0)
                {
                    m_AttrName[index].text = strColor + ItemTool.ConvertAttrToString((COMBATATTE)1000);
                    m_AddAttr[index].text = string.Format("+{0}", GetNextEnchanceAttr(item, (int)line.AllAttack).ToString());
                    m_AttrWhole[index].SetActive(true);
                    ++index;
                }
                //全防御
                if (line.AllDefence > 0 && index < m_AttrName.Length && index >= 0)
                {
                    m_AttrName[index].text = strColor + ItemTool.ConvertAttrToString((COMBATATTE)1001);
                    m_AddAttr[index].text = string.Format("+{0}", GetNextEnchanceAttr(item, (int)line.AllDefence).ToString());
                    m_AttrWhole[index].SetActive(true);
                    ++index;
                }
                //附加属性
                for (int attrIndex = 0; attrIndex < line.getAddAttrTypeCount(); ++attrIndex)
                {
                    int attrType = line.GetAddAttrTypebyIndex(attrIndex);
                    float attrValue = line.GetAddAttrValuebyIndex(attrIndex);
                    if ((attrType >= (int)COMBATATTE.MAXHP && attrType < (int)COMBATATTE.COMBATATTE_MAXNUM) || attrType == 1000 || attrType == 1001)
                    {
                        if (attrValue > 0)
                        {
                            if (index < m_AttrName.Length && index >= 0)
                            {
                                //策划要求隐藏装备攻速属性
                                if (attrType == (int)COMBATATTE.ATTACKSPEED)
                                {
                                    continue;
                                }
                                m_AttrName[index].text = strColor + ItemTool.ConvertAttrToString((COMBATATTE)attrType);
                                m_AddAttr[index].text = string.Format("+{0}", GetNextEnchanceAttr(item, (int)attrValue).ToString());
                                m_AttrWhole[index].SetActive(true);
                                ++index;
                            }
                        }
                    }
                }
                m_Grid.Reposition();
                m_TopGrid.Recenter(true);
            }
        }
    }

    int GetEnchanceAdd(GameItem item, int attr)
    {
        int EnchanceAddValue = 0;
        Tab_EquipEnchance curEnchanceLine = TableManager.GetEquipEnchanceByID(item.EnchanceLevel, 0);
        Tab_EquipEnchance nextEnchanceLine = TableManager.GetEquipEnchanceByID(item.EnchanceLevel + 1, 0);
        if (nextEnchanceLine != null)
        {
            if (curEnchanceLine != null)
            {
                int curValue = (int)(curEnchanceLine.EnchanceRate * attr);
                int nextValue = (int)(nextEnchanceLine.EnchanceRate * attr);
                EnchanceAddValue = (int)(nextValue - curValue);
            }
            else
            {
                EnchanceAddValue = (int)(nextEnchanceLine.EnchanceRate * attr) + 1;
            }
        }
        return EnchanceAddValue;
    }

    int GetNextEnchanceAttr(GameItem item, int attr)
    {
        int nextValue = 0;
        Tab_EquipEnchance nextEnchanceLine = TableManager.GetEquipEnchanceByID(item.EnchanceLevel + 1, 0);
        if (nextEnchanceLine != null)
        {
            nextValue = (int)(nextEnchanceLine.EnchanceRate * attr) + 1;
        }
        return nextValue;
    }
}