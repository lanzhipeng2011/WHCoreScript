using UnityEngine;
using System.Collections;
using Games.Item;
using GCGame.Table;

/// <summary>
/// 装备属性显示规则:
/// 1.装备初始配表属性 和 打星加成属性 合并显示
/// 2.强化加成属性额外显示
/// 
/// 例： 初始属性+100 打星加成属性+10 强化加成属性+20
/// 显示： “血上限+110  +20”
/// </summary>

public class TooltipsEquipAttr : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    public GameObject[] m_AttrWhole;
    public UILabel[] m_Attr;
    public UILabel[] m_AttrValue;
    public UILabel[] m_EnchanceAttr;
    public GameObject[] m_CutLine;
    public UIGrid m_Grid;
    public UITopGrid m_TopGrid;
    //public UILabel[] m_StarAttr;

    public void ClearInfo()
    {
        for (int i = 0; i < m_Attr.Length; ++i)
        {
            m_Attr[i].text = "";
            m_EnchanceAttr[i].text = "";
            m_AttrValue[i].text = "";
            m_CutLine[i].SetActive(false);
            m_AttrWhole[i].SetActive(false);
        }
    }

    //显示装备属性
    public void ShowAttr(GameItem item, bool bUnEquiped = false)
    {
        for (int i = 0; i < m_Attr.Length; ++i)
        {
            m_Attr[i].text = "";
            m_EnchanceAttr[i].text = "";
            m_AttrValue[i].text = "";
            m_CutLine[i].SetActive(false);
            m_AttrWhole[i].SetActive(false);
            m_Attr[i].effectStyle = UILabel.Effect.None;
            m_AttrValue[i].effectStyle = UILabel.Effect.None;
            
        }

        //获得身上对应槽位的装备
        int slotindex = item.GetEquipSlotIndex();
        GameItem compareEquip = GameManager.gameManager.PlayerDataPool.EquipPack.GetItem(slotindex);

        Tab_EquipAttr line = TableManager.GetEquipAttrByID(item.DataID, 0);
        if (line != null)
        {
            int index = 0;
            //血上限
            if (line.HP > 0 && index < m_Attr.Length && index >= 0)
            {
                string strColor = GetAttrColor(bUnEquiped, compareEquip, line.HP, "HP");
                m_Attr[index].text = strColor + ItemTool.ConvertAttrToString(COMBATATTE.MAXHP);
                m_AttrValue[index].text = strColor + string.Format("+{0}", (line.HP));
                m_AttrWhole[index].SetActive(true);
                //强化
                SetEnchanceAndStarAttr(item, index, line.HP);
                ++index;
            }
            //血上限(百分比)
            if (line.HPPer > 0 && index < m_Attr.Length && index >= 0)
            {
                string strColor = GetAttrColor(bUnEquiped, compareEquip, line.HPPer, "HPPer");
                m_Attr[index].text = strColor + ItemTool.ConvertAttrToString(COMBATATTE.MAXHP);
                m_AttrValue[index].text = strColor + string.Format("+{0}%", line.HPPer * 100);
                m_AttrWhole[index].SetActive(true);
                ++index;
            }
            //蓝上限
            if (line.MP > 0 && index < m_Attr.Length && index >= 0)
            {
                string strColor = GetAttrColor(bUnEquiped, compareEquip, line.MP, "MP");
                m_Attr[index].text = strColor + ItemTool.ConvertAttrToString(COMBATATTE.MAXMP);
                m_AttrValue[index].text = strColor + string.Format("+{0}", (line.MP));
                m_AttrWhole[index].SetActive(true);
                //强化
                SetEnchanceAndStarAttr(item, index, line.MP);
                ++index;
            }
            //蓝上限(百分比)
            if (line.MPPer > 0 && index < m_Attr.Length && index >= 0)
            {
                string strColor = GetAttrColor(bUnEquiped, compareEquip, line.MPPer, "MPPer");
                m_Attr[index].text = strColor + ItemTool.ConvertAttrToString(COMBATATTE.MAXMP);
                m_AttrValue[index].text = strColor + string.Format("+{0}%", line.MPPer * 100);
                m_AttrWhole[index].SetActive(true);
                ++index;
            }
            //物理攻击
            if (line.PhysicsAttack > 0 && index < m_Attr.Length && index >= 0)
            {
                string strColor = GetAttrColor(bUnEquiped, compareEquip, line.PhysicsAttack, "PhysicsAttack");
                m_Attr[index].text = strColor + ItemTool.ConvertAttrToString(COMBATATTE.PYSATTACK);
                m_AttrValue[index].text = strColor + string.Format("+{0}", (line.PhysicsAttack));
                m_AttrWhole[index].SetActive(true);
                //强化
                SetEnchanceAndStarAttr(item, index, line.PhysicsAttack);
                ++index;
            }
            //物理攻击(百分比)
            if (line.PhysicsAttackPer > 0 && index < m_Attr.Length && index >= 0)
            {
                string strColor = GetAttrColor(bUnEquiped, compareEquip, line.PhysicsAttackPer, "PhysicsAttackPer");
                m_Attr[index].text = strColor + ItemTool.ConvertAttrToString(COMBATATTE.PYSATTACK);
                m_AttrValue[index].text = strColor + string.Format("+{0}%", line.PhysicsAttackPer * 100);
                m_AttrWhole[index].SetActive(true);
                ++index;
            }
            //法术攻击
            if (line.MagicAttack > 0 && index < m_Attr.Length && index >= 0)
            {
                string strColor = GetAttrColor(bUnEquiped, compareEquip, line.MagicAttack, "MagicAttack");
                m_Attr[index].text = strColor + ItemTool.ConvertAttrToString(COMBATATTE.MAGATTACK);
                m_AttrValue[index].text = strColor + string.Format("+{0}", (line.MagicAttack));
                m_AttrWhole[index].SetActive(true);
                //强化
                SetEnchanceAndStarAttr(item, index, line.MagicAttack);
                ++index;
            }
            //法术攻击(百分比)
            if (line.MagicAttackPer > 0 && index < m_Attr.Length && index >= 0)
            {
                string strColor = GetAttrColor(bUnEquiped, compareEquip, line.MagicAttackPer, "MagicAttackPer");
                m_Attr[index].text = strColor + ItemTool.ConvertAttrToString(COMBATATTE.MAGATTACK);
                m_AttrValue[index].text = strColor + string.Format("+{0}%", line.MagicAttackPer * 100);
                m_AttrWhole[index].SetActive(true);
                ++index;
            }
            //物理防御
            if (line.PhysicsDefence > 0 && index < m_Attr.Length && index >= 0)
            {
                string strColor = GetAttrColor(bUnEquiped, compareEquip, line.PhysicsDefence, "PhysicsDefence");
                m_Attr[index].text = strColor + ItemTool.ConvertAttrToString(COMBATATTE.PYSDEF);
                m_AttrValue[index].text = strColor + string.Format("+{0}", (line.PhysicsDefence));
                m_AttrWhole[index].SetActive(true);
                //强化
                SetEnchanceAndStarAttr(item, index, line.PhysicsDefence);
                ++index;
            }
            //物理防御(百分比)
            if (line.PhysicsDefencePer > 0 && index < m_Attr.Length && index >= 0)
            {
                string strColor = GetAttrColor(bUnEquiped, compareEquip, line.PhysicsDefencePer, "PhysicsDefencePer");
                m_Attr[index].text = strColor + ItemTool.ConvertAttrToString(COMBATATTE.PYSDEF);
                m_AttrValue[index].text = strColor + string.Format("+{0}%", line.PhysicsDefencePer * 100);
                m_AttrWhole[index].SetActive(true);
                ++index;
            }
            //法术防御
            if (line.MagicDefence > 0 && index < m_Attr.Length && index >= 0)
            {
                string strColor = GetAttrColor(bUnEquiped, compareEquip, line.MagicDefence, "MagicDefence");
                m_Attr[index].text = strColor + ItemTool.ConvertAttrToString(COMBATATTE.MAGDEF);
                m_AttrValue[index].text = strColor + string.Format("+{0}", (line.MagicDefence));
                m_AttrWhole[index].SetActive(true);
                //强化
                SetEnchanceAndStarAttr(item, index, line.MagicDefence);
                ++index;
            }
            //法术防御(百分比)
            if (line.MagicDefencePer > 0 && index < m_Attr.Length && index >= 0)
            {
                string strColor = GetAttrColor(bUnEquiped, compareEquip, line.MagicDefencePer, "MagicDefencePer");
                m_Attr[index].text = strColor + ItemTool.ConvertAttrToString(COMBATATTE.MAGDEF);
                m_AttrValue[index].text = strColor + string.Format("+{0}%", line.MagicDefencePer * 100);
                m_AttrWhole[index].SetActive(true);
                ++index;
            }
            //攻击速度
            if (line.AttackSpeed > 0 && index < m_Attr.Length && index >= 0)
            {
                //策划要求 隐藏装备攻速属性
                //m_Attr[index].text = string.Format("{0}+{1}", ConvertAttrToString(COMBATATTE.ATTACKSPEED), line.AttackSpeed);
                //m_Attr[index].gameObject.SetActive(true);
                ////策划要求 攻速不受强化和打星影响
                //++index;
            }
            //攻击速度(百分比)
            if (line.AttackSpeedPer > 0 && index < m_Attr.Length && index >= 0)
            {
                //策划要求 隐藏装备攻速属性
                //m_Attr[index].text = string.Format("{0}+{1}%", ConvertAttrToString(COMBATATTE.ATTACKSPEED), line.AttackSpeedPer * 100);
                //m_Attr[index].gameObject.SetActive(true);
                //++index;
            }
            //全攻击
            if (line.AllAttack > 0 && index < m_Attr.Length && index >= 0)
            {
                string strColor = GetAttrColor(bUnEquiped, compareEquip, line.AllAttack, "AllAttack");
                m_Attr[index].text = strColor + ItemTool.ConvertAttrToString((COMBATATTE)1000);
                m_AttrValue[index].text = strColor + string.Format("+{0}", (line.AllAttack));
                m_AttrWhole[index].SetActive(true);
                //强化
                SetEnchanceAndStarAttr(item, index, line.AllAttack);
                ++index;
            }
            //全攻击(百分比)
            if (line.AllAttackPer > 0 && index < m_Attr.Length && index >= 0)
            {
                string strColor = GetAttrColor(bUnEquiped, compareEquip, line.AllAttackPer, "AllAttackPer");
                m_Attr[index].text = strColor + ItemTool.ConvertAttrToString((COMBATATTE)1000);
                m_AttrValue[index].text = strColor + string.Format("+{0}%", line.AllAttackPer * 100);
                m_AttrWhole[index].SetActive(true);
                ++index;
            }
            //全防御
            if (line.AllDefence > 0 && index < m_Attr.Length && index >= 0)
            {
                string strColor = GetAttrColor(bUnEquiped, compareEquip, line.AllDefence, "AllDefence");
                m_Attr[index].text = strColor + ItemTool.ConvertAttrToString((COMBATATTE)1001);
                m_AttrValue[index].text = strColor + string.Format("+{0}", (line.AllDefence));
                m_AttrWhole[index].SetActive(true);
                //强化
                SetEnchanceAndStarAttr(item, index, line.AllDefence);
                ++index;
            }
            //全防御(百分比)
            if (line.AllDefencePer > 0 && index < m_Attr.Length && index >= 0)
            {
                string strColor = GetAttrColor(bUnEquiped, compareEquip, line.AllDefencePer, "AllDefencePer");
                m_Attr[index].text = strColor + ItemTool.ConvertAttrToString((COMBATATTE)1001);
                m_AttrValue[index].text = strColor + string.Format("+{0}%", line.AllDefencePer * 100);
                m_AttrWhole[index].SetActive(true);
                ++index;
            }
            if (index >= 1)
            {
                m_CutLine[index - 1].SetActive(true);
            }
            //附加属性
            for (int attrIndex = 0; attrIndex < line.getAddAttrTypeCount(); ++attrIndex)
            {
                int attrType = line.GetAddAttrTypebyIndex(attrIndex);
                float attrValue = line.GetAddAttrValuebyIndex(attrIndex);
                if ( (attrType >= (int)COMBATATTE.MAXHP && attrType < (int)COMBATATTE.COMBATATTE_MAXNUM) || attrType == 1000 || attrType == 1001)
                {
                    if (attrValue > 0)
                    {
                        if (index < m_Attr.Length && index >= 0)
                        {
                            //策划要求隐藏装备攻速属性
                            if (attrType == (int)COMBATATTE.ATTACKSPEED)
                            {
                                continue;
                            }

                            string strColor = GetAttrColor(bUnEquiped, compareEquip, attrValue, "AddAttr", attrIndex);
                            m_Attr[index].text = strColor + ItemTool.ConvertAttrToString((COMBATATTE)attrType);
                            m_AttrValue[index].text = strColor + string.Format("+{0}", attrValue);
                            m_AttrWhole[index].SetActive(true);
                            if (attrType != (int)COMBATATTE.ATTACKSPEED) //策划要求 攻速不受强化和打星影响
                            {
                                //强化
                                SetEnchanceAndStarAttr(item, index, attrValue, true);
                                //策划要求 附加属性不受打星影响
                            }
                            ++index;
                        }
                    }
                }
            }
            //附加属性(百分比)
            for (int attrIndex = 0; attrIndex < line.getAddAttrTypeCount(); ++attrIndex)
            {
                int attrType = line.GetAddAttrTypebyIndex(attrIndex);
                float attrPer = line.GetAddAttrPerbyIndex(attrIndex);
                if (attrType >= (int)COMBATATTE.MAXHP && attrType < (int)COMBATATTE.COMBATATTE_MAXNUM && attrPer > 0)
                {
                    if (index < m_Attr.Length && index >= 0)
                    {
                        string strColor = GetAttrColor(bUnEquiped, compareEquip, attrPer, "AddAttrPer", attrIndex);

                        m_Attr[index].text = strColor + ItemTool.ConvertAttrToString((COMBATATTE)attrIndex);
                        m_AttrValue[index].text = strColor + string.Format("+{0}%", attrPer * 100);
                        m_AttrWhole[index].SetActive(true);
                        ++index;
                    }
                }
            }

            m_Grid.Reposition();
            m_TopGrid.Recenter(true);
        }
    }

    string GetAttrColor(bool bUnEquiped, GameItem compareEquip, float newEquipAttrValue, string attrName, int AddAttrIndex = 0)
    {
        if (!bUnEquiped)
        {
            return "";
        }

        bool bHaveEquip = false;
        Tab_EquipAttr compareLine = null;
        if (compareEquip != null && compareEquip.IsValid())
        {
            bHaveEquip = true;
            compareLine = TableManager.GetEquipAttrByID(compareEquip.DataID, 0);
        }

        if (!bHaveEquip)
        {
            return "[FF2222]";
        }
        else
        {
            float oldEquipAttrValue = 0;
            switch (attrName)
            {
                case "HP":
                    oldEquipAttrValue = compareLine.HP + GetStarAttr(compareEquip, compareLine.HP);
                    break;
                case "HPPer":
                    oldEquipAttrValue = compareLine.HPPer;
                    break;
                case "MP":
                    oldEquipAttrValue = compareLine.MP + GetStarAttr(compareEquip, compareLine.MP);
                    break;
                case "MPPer":
                    oldEquipAttrValue = compareLine.MPPer;
                    break;
                case "PhysicsAttack":
                    oldEquipAttrValue = compareLine.PhysicsAttack + GetStarAttr(compareEquip, compareLine.PhysicsAttack);
                    break;
                case "PhysicsAttackPer":
                    oldEquipAttrValue = compareLine.PhysicsAttackPer;
                    break;
                case "MagicAttack":
                    oldEquipAttrValue = compareLine.MagicAttack + GetStarAttr(compareEquip, compareLine.MagicAttack);
                    break;
                case "MagicAttackPer":
                    oldEquipAttrValue = compareLine.MagicAttackPer;
                    break;
                case "PhysicsDefence":
                    oldEquipAttrValue = compareLine.PhysicsDefence + GetStarAttr(compareEquip, compareLine.PhysicsDefence);
                    break;
                case "PhysicsDefencePer":
                    oldEquipAttrValue = compareLine.PhysicsDefencePer;
                    break;
                case "MagicDefence":
                    oldEquipAttrValue = compareLine.MagicDefence + GetStarAttr(compareEquip, compareLine.MagicDefence);
                    break;
                case "MagicDefencePer":
                    oldEquipAttrValue = compareLine.MagicDefencePer;
                    break;
                case "AllAttack":
                    oldEquipAttrValue = compareLine.AllAttack + GetStarAttr(compareEquip, compareLine.AllAttack);
                    break;
                case "AllAttackPer":
                    oldEquipAttrValue = compareLine.AllAttackPer;
                    break;
                case "AllDefence":
                    oldEquipAttrValue = compareLine.AllDefence + GetStarAttr(compareEquip, compareLine.AllDefence);
                    break;
                case "AllDefencePer":
                    oldEquipAttrValue = compareLine.AllDefencePer;
                    break;
                case "AddAttr":
                    oldEquipAttrValue = compareLine.GetAddAttrValuebyIndex(AddAttrIndex);
                    break;
                case "AddAttrPer":
                    oldEquipAttrValue = compareLine.GetAddAttrPerbyIndex(AddAttrIndex);
                    break;
                default:
                    break;
            }

            if (oldEquipAttrValue < newEquipAttrValue)
            {
                return "[FF2222]";
            }
            else if (oldEquipAttrValue == newEquipAttrValue)
            {
                return "[00A0FF]";
            }
            else
            {
                return "[32A100]";
            }
        }
    }

    //显示套装属性
    public void ShowEquipSetAttr(GameItem item)
    {
        for (int i = 0; i < m_Attr.Length; ++i)
        {
            m_Attr[i].text = "";
            m_EnchanceAttr[i].text = "";
            m_AttrValue[i].text = "";
            m_CutLine[i].SetActive(false);
            m_AttrWhole[i].SetActive(false);
        }

        int setId = item.GetEquipSetId();
        if (setId >= 0)
        {
            Tab_EquipSet line = TableManager.GetEquipSetByID(setId, 0);
            if (line != null)
            {
                for (int index = 0; index < 8; index++ )
                {
                    m_Attr[index].text = "[626262]";
                }
                GameItemContainer equipPack = GameManager.gameManager.PlayerDataPool.EquipPack;

                //显示套装名称
                int labelIndex = 0;
                m_Attr[labelIndex].text = "[FF9933]" + line.Name;
                m_AttrWhole[labelIndex].SetActive(true);
                labelIndex++;

                for (int index = 0; index < line.getEquipIdCount(); index++)
                {
                    int equipid = line.GetEquipIdbyIndex(index);
                    if (equipid != -1)
                    {
                        Tab_CommonItem lineCommonitem = TableManager.GetCommonItemByID(equipid, 0);
                        if (lineCommonitem != null)
                        {
                            if (equipPack.GetItemCountByDataId(equipid) > 0)
                            {
                                m_Attr[labelIndex].text += "[FF9933]";
                            }
                            else
                            {
                                m_Attr[labelIndex].text += "[626262]";
                            }
                            m_Attr[labelIndex].text += lineCommonitem.Name;
                            m_AttrWhole[labelIndex].SetActive(true);
                            labelIndex++;
                        }
                    }
                }

                //m_Attr[labelIndex].text = "[FF9933]套装属性:";
                m_Attr[labelIndex].text = StrDictionary.GetClientDictionaryString("#{2878}");
                m_AttrWhole[labelIndex].SetActive(true);
                labelIndex++;

                int equipCount = equipPack.GetEquipCountBySetId(setId);
                string color = "[626262]";
                //显示两件套效果
                if (line.GetEffectTypebyIndex(0) != -1)
                {
                    if (equipCount >= 2)
                    {
                        color += "[FF9933]";
                    }
                    m_Attr[labelIndex].text += color;
                    m_Attr[labelIndex].text += StrDictionary.GetClientDictionaryString("#{1448}");
                    m_AttrWhole[labelIndex].SetActive(true);
                    labelIndex++;
                    string text = string.Format("{0}+{1}", ItemTool.ConvertAttrToString((COMBATATTE)line.GetEffectTypebyIndex(0)), line.GetEffectValuebyIndex(0));
                    m_Attr[labelIndex].text += color;
                    m_Attr[labelIndex].text += "    " + text;
                    m_AttrWhole[labelIndex].SetActive(true);
                    labelIndex++;
                }
                color = "[626262]";
                //显示三件套效果
                if (line.GetEffectTypebyIndex(1) != -1)
                {
                    if (equipCount >= 3)
                    {
                        color += "[FF9933]";
                    }
                    m_Attr[labelIndex].text += color;
                    m_Attr[labelIndex].text += StrDictionary.GetClientDictionaryString("#{1449}");
                    m_AttrWhole[labelIndex].SetActive(true);
                    labelIndex++;
                    string text = string.Format("{0}+{1}", ItemTool.ConvertAttrToString((COMBATATTE)line.GetEffectTypebyIndex(1)), line.GetEffectValuebyIndex(1));
                    m_Attr[labelIndex].text += color;
                    m_Attr[labelIndex].text += "    " + text;
                    m_AttrWhole[labelIndex].SetActive(true);
                    labelIndex++;
                }
                color = "[626262]";
                //显示四件套效果
                if (line.GetEffectTypebyIndex(2) != -1)
                {
                    if (equipCount >= 4)
                    {
                        color += "[FF9933]";
                    }
                    m_Attr[labelIndex].text += color;
                    m_Attr[labelIndex].text += StrDictionary.GetClientDictionaryString("#{1450}");
                    m_AttrWhole[labelIndex].SetActive(true);
                    labelIndex++;
                    string text = string.Format("{0}+{1}", ItemTool.ConvertAttrToString((COMBATATTE)line.GetEffectTypebyIndex(2)), line.GetEffectValuebyIndex(2));
                    m_Attr[labelIndex].text += color;
                    m_Attr[labelIndex].text += "    " + text;
                    m_AttrWhole[labelIndex].SetActive(true);
                    labelIndex++;
                }
                m_Grid.Reposition();
                m_TopGrid.Recenter(true);
            }
        }
    }


    void SetEnchanceAndStarAttr(GameItem item, int index, float attr, bool extraAddAttr = false)
    {
        int EnchanceAndStarValue = 0;
        //强化属性
        if (item.EnchanceLevel > 0)
        {
            Tab_EquipEnchance enchanceline = TableManager.GetEquipEnchanceByID(item.EnchanceLevel, 0);
            if (enchanceline != null)
            {
                float rate = enchanceline.EnchanceRate;
                if (rate > 0)
                {
                    EnchanceAndStarValue += (int)(attr * rate) + 1;
                }
            }
        }
        //打星属性
        if (item.StarLevel > 0 && extraAddAttr == false)
        {
            Tab_EquipStar starline = TableManager.GetEquipStarByID(item.StarLevel, 0);
            if (starline != null)
            {
                float rate = starline.AttrRate;
                if (rate > 0)
                {
                    EnchanceAndStarValue += (int)(attr * rate) + 1;
                }
            }
        }
        if (EnchanceAndStarValue > 0)
        {
            m_EnchanceAttr[index].text = string.Format("+{0}", EnchanceAndStarValue);
            m_EnchanceAttr[index].gameObject.SetActive(true);
        }
    }

    void SetStarAttr(GameItem item, int index, float attr)
    {
        if (item.StarLevel > 0)
        {
            Tab_EquipStar starline = TableManager.GetEquipStarByID(item.StarLevel, 0);
            if (starline != null)
            {
                float rate = starline.AttrRate;
                if (rate > 0)
                {
                    //m_StarAttr[index].text = string.Format("+{0}", (int)(attr * rate)+1);
                    //m_StarAttr[index].gameObject.SetActive(true);
                }
            }
        }
    }

    int GetStarAttr(GameItem item, float attr)
    {
        int starValue = 0;
        if (null != item && item.StarLevel > 0)
        {
            Tab_EquipStar starline = TableManager.GetEquipStarByID(item.StarLevel, 0);
            if (starline != null)
            {
                float rate = starline.AttrRate;
                if (rate > 0)
                {
                    starValue = (int)(attr * rate) + 1;
                }
            }
        }
        return starValue;
    }
}
